using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using ElCurve = ALARm.Core.ElCurve;


namespace ALARm_Report.Forms
{
    public class BlindStateGaps : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            //Сделать выбор периода
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(parentId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel)
                    return;
                admTracksId = choiceForm.admTracksIDs;
            }

            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var tripProcesses = //RdStructureService.GetAdditionalParametersProcess(period);
                                    RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                int iter = 1;
                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        XElement tripElem = new XElement("trip",
                            new XAttribute("date_statement", tripProcess.Date_Vrem.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))),
                            new XAttribute("type", tripProcess.GetProcessTypeName), //ToDo
                            new XAttribute("road", road),
                            new XAttribute("distance", distance.Name),
                            new XAttribute("periodDate", period.Period),
                           
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("ps", tripProcess.Car),
                            new XAttribute("trip_date", tripProcess.Date_Vrem.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE")))
                        );
                        XElement Direct = new XElement("direction",
                            new XAttribute("name",
                                 tripProcess.DirectionName + " (" + tripProcess.DirectionCode + "), Путь: " + trackName + ", ПЧ: " + distance.Code)
                        );
                        var gaps =//AdditionalParametersService.GetGap(tripProcess.Id, (int)tripProcess.Direction);
                                AdditionalParametersService.Check_Sleep_gap_state(tripProcess.Trip_id, template.ID);
                        
                        var stat = false;
                        
                        //foreach (var gap in gaps)
                        for (int i = 0; i < gaps.Count-1; i++)
                        {
                            var gap = gaps[i];

                            if (gaps[i].Zazor != gaps[i+1].Zazor)
                            {
                                if (gaps[i].R_zazor != gaps[i+1].R_zazor)
                                    continue;
                            }
                            if (gaps[i].R_zazor != gaps[i + 1].R_zazor)
                            {
                                if (gaps[i].Zazor != gaps[i].Zazor)
                                    continue;
                            }


                            if (stat == false)
                            {
                                if (gap.Razn < 23 || gap.Razn > 28) continue;
                                if (gap.Next_otst == "") continue;
                                stat = true;
                            }
                            if (gap.Next_otst == "")
                            {
                                stat = false;
                            }
                            var temperature = MainTrackStructureService.GetTemp(tripProcess.Trip_id, track_id, gap.Km);
                            var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, trackName.ToString()) as List<Speed>;
                            var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km,MainTrackStructureConst.MtoPdbSection, tripProcess.DirectionName, trackName.ToString()) as List<PdbSection>;
                            var sector = MainTrackStructureService.GetSector(track_id, gap.Km, tripProcess.Date_Vrem);
                            gap.PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                            gap.FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;
                            var dig = gap.GetDigressions();

                            XElement Notes = new XElement("Note");

                            Notes.Add(
                                new XAttribute("n", iter),
                                new XAttribute("PPP", pdbSection.Count > 0 ? pdbSection[0].ToString() : "ПЧУ-/ПД-/ПДБ-"),
                                new XAttribute("PeregonStancia", sector == null ? "" : sector),
                                new XAttribute("km", gap.Km),
                                new XAttribute("piket", gap.Meter / 100 + 1),
                                new XAttribute("m", gap.Meter),
                                new XAttribute("Vpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                new XAttribute("T", (temperature.Count != 0 ? temperature[0].Kupe.ToString() : " ") + "°"),//ToDo
                                new XAttribute("nit", gaps[i].Zazor==0 ? "левая" : "правая"),
                                new XAttribute("Vdop", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),
                                new XAttribute("Otst", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.GetName() : ""),// Otst(speed.Count > 0 ? speed[0].ToString() : "-/-", gap.Zazor)),//ToDo
                                new XAttribute("Primech", "")//ToDo
                                );
                            Direct.Add(Notes);
                            iter++;
                        }
                        iter = 1;
                        tripElem.Add(Direct);
                        report.Add(tripElem);
                    }
                }

                xdReport.Add(report);
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save($@"G:\form\4.Выходные формы Видеоконтроля ВСП\22.Два и более слепых стыковых зазоров подряд .html");
                htReport.Save(Path.GetTempPath() + "/report.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report.html");
            }
        }
        public override string ToString()
        {
            return "Отступления 2 степени, близкие к 3";
        }

    }

}