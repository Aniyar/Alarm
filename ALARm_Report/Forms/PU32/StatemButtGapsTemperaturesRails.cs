using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
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
    public class StatemButtGapsTemperaturesRails : Report
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

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                int iter = 1, PrevPut = -1, startKM = 0, finalKM = 0;
                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        var gaps = AdditionalParametersService.RDGetGap(tripProcess.Trip_id, (int)tripProcess.Direction, (int)track_id);
                        gaps = gaps.Where(o => o.Km > 128).ToList();
                        if (gaps == null || gaps.Count == 0) { continue; }
                        gaps = gaps.Where(o => o.Km > 128).ToList();

                        

                        XElement tripElem = new XElement("trip",
                       
                            new XAttribute("direction", tripProcess.DirectionName + " (" + tripProcess.DirectionCode + ")"),
                            new XAttribute("date_statement", $"{ period.Period }    { tripProcess.GetProcessTypeName }"),
                            new XAttribute("nput", trackName),
                            new XAttribute("uchastok", $"{road}"),//todo
                            new XAttribute("npch", distance.Code)
                          
                        //new XAttribute("km", gaps.Count>0?(gaps[0].Km+" - "+gaps[gaps.Count-1].Km):"0")
                        );
                        //tripElem.Add(new XAttribute("km", gaps.Count > 0 ? (gaps[0].Km + " - " + gaps[gaps.Count - 1].Km) : "0"));

                        var speed = new List<Speed>();
                        var temperature = new List<Temperature>();
                        int previousGap = -1;

                        progressBar.Maximum = tripProcesses.Count;

                        //var gaps = AdditionalParametersService.GetGap(tripProcess.Id, (int)tripProcess.Direction);

                        startKM = gaps[0].Km;
                        finalKM = gaps[gaps.Count - 1].Km;

                        foreach (var gap in gaps)
                        {
                            gap.R_zazor = gap.R_zazor == -999 ? -999 : gap.R_zazor == -1 ? 0 : (int)Math.Round(gap.R_zazor / 1.5);
                            gap.Zazor = gap.Zazor == -999 ? -999 : gap.Zazor == -1 ? 0 : (int)Math.Round(gap.Zazor / 1.5);
                            gap.Zabeg = gap.Zabeg == -999 ? -999 : (int)Math.Round(gap.Zabeg / 1.5);

                            if ((previousGap == null) || (previousGap != gap.Km))
                            {
                                speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, trackName.ToString()) as List<Speed>;
                                temperature = MainTrackStructureService.GetTemp(tripProcess.Trip_id, track_id, gap.Km) as List<Temperature>;

                                previousGap = gap.Km;
                            }

                            gap.PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                            gap.FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;



                            var dig = gap.GetDigressions();
                            var dig2 = gap.GetDigressions2();


                            XElement Notes = new XElement("Note");
                            Notes.Add(
                                new XAttribute("km", gap.Km),
                                new XAttribute("m", gap.Meter),
                                new XAttribute("param", gap.Threat == Threat.Left ? "Зазор левый / Т°" : "Зазор правый / Т°"),
                                new XAttribute("velichina", gap.Zazor.ToString() + " / " + (temperature.Count != 0 ? temperature[0].Kupe.ToString(): " ") + "°"),//ToDo temperature
                                new XAttribute("Vpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                new XAttribute("VdopZazor", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : "")
                                );
                            if (gap.Zazor != -999)
                            {
                                tripElem.Add(Notes);
                            }
                            

                            Notes = new XElement("Note");
                            Notes.Add(
                                new XAttribute("km", gap.Km),
                                new XAttribute("m", gap.Meter),
                                new XAttribute("param", gap.R_threat == Threat.Right ? "Зазор правый / Т°" : "Зазор левый / Т°"),
                                new XAttribute("velichina", gap.R_zazor.ToString() + " / " + (temperature.Count != 0 ? temperature[0].Koridor.ToString() : " ") + "°"),//ToDo temperature
                                new XAttribute("Vpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                new XAttribute("VdopZazor", (dig2.R_DigName == DigressionName.Gap || dig2.R_DigName == DigressionName.FusingGap || dig2.R_DigName == DigressionName.AnomalisticGap) ? dig2.R_AllowSpeed : "")
                                );
                            if (gap.R_zazor != -999)
                            {
                                tripElem.Add(Notes);
                            }

                        }
                        tripElem.Add(new XAttribute("km", gaps.Count > 0 ? (startKM + " - " + finalKM) : "0"));
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