using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using ElCurve = ALARm.Core.ElCurve;


namespace ALARm_Report.Forms
{
    public class rd_epur : Report
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
                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var directName = AdditionalParametersService.DirectName(tripProcess.Id, (int)distance.Id);


                        XElement tripElem = new XElement("trip",
                             new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("date_statement", tripProcess.Date_Vrem.Date.ToShortDateString()),
                            new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                            new XAttribute("road", road),
                            new XAttribute("distance", distance.Name),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("ps", tripProcess.Car),
                            new XAttribute("trip_date", tripProcess.Trip_date)
                        );
                        XElement Direct = new XElement("direction",
                            new XAttribute("name", directName.Count == 0 ? "-/-/-" : directName[0].Name + " / Путь: " + directName[0].Put + " / ПЧ: " + directName[0].Pch));

                        

                        var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                        //kms = kms.Where(o => o > 128).ToList();

                        

                        foreach (var kmetr in kms)
                        {
                            var RDGetShpal = AdditionalParametersService.RDGetShpal(tripProcess.Trip_id, (int)tripProcess.Direction, kmetr);
                            if (RDGetShpal == null || RDGetShpal.Count == 0) { continue; }
                            float sum = 0;
                            int StartM = -1;
                            var trackclasses = new List<TrackClass>();
                            var curves = new List<StCurve>();
                            var previousKm = -1;

                            int iter = 1;
                            int n = 1;
                            int prev_pk = -1;
                            var pk = 1;
                            var epur_bpd = 0;
                            var epure_fact = 0f;

                            for (int i = 0; i < RDGetShpal.Count - 1; i++)
                            {
                                var km = RDGetShpal[i].Km;

                                if ((previousKm == -1) || (previousKm != km))
                                {
                                    trackclasses = (List<TrackClass>)MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, km, MainTrackStructureConst.MtoTrackClass, track_id);
                                    curves = (List<StCurve>)MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, km, MainTrackStructureConst.MtoStCurve, track_id);
                                }
                                previousKm = km;

                                var trackclass = trackclasses.Count > 0 ? trackclasses[0].Class_Id : -1;
                                var curve = curves.Count > 0 ? (int)curves[0].Radius : -1;

                                //эпюра есептейміз
                                //Эпюры шпал на путях линий 1 - 3 - го классов должны быть: в прямых участках и в кривых радиусом более 1200 м - 1840 шт./ км, 
                                //радиусом 1200 м и менее - 2000 шт./ км; на путях 4 - 5 - го класса: в прямых и кривых радиусом более 1200 м - 1600 шт./ км, 
                                //радиусом 1200 м и менее - 1840 шт./ км.
                                

                                switch (trackclass)
                                {
                                    case int clas when clas >= 1 && clas <= 3:
                                        if (curve == -1 || curve > 1200)
                                        {
                                            epur_bpd = 1840;
                                        }
                                        else if (curve != -1 && curve <= 1200)
                                        {
                                            epur_bpd = 2000;
                                        }
                                        else
                                        {
                                            epur_bpd = 0;
                                        }
                                        break;
                                    case int clas when clas >= 4 && clas <= 5:
                                        if (curve == -1 || curve > 1200)
                                        {
                                            epur_bpd = 1600;
                                        }
                                        else if (curve != -1 && curve <= 1200)
                                        {
                                            epur_bpd = 1840;
                                        }
                                        else
                                        {
                                            epur_bpd = 0;
                                        }
                                        break;
                                    default:
                                        epur_bpd = 0;
                                        break;
                                }


                                XElement Notes = new XElement("Note");

                                if (StartM == -1) StartM = RDGetShpal[i].Meter;

                                var nextShpal = RDGetShpal[i + 1].Meter;

                                if (Math.Abs(StartM - nextShpal) >= 25)
                                {

                                    epure_fact = (sum / 25) * 1000;

                                    pk = (StartM) / 100 + 1;

                                    if(prev_pk!= pk)
                                    {
                                        iter = 1;
                                    }
                                    prev_pk = pk;

                                    Notes.Add(
                                        new XAttribute("iter", n),
                                        new XAttribute("km", RDGetShpal[i].Km),
                                        new XAttribute("m", pk),
                                        new XAttribute("n", iter),
                                        new XAttribute("epure_fact", epure_fact + ""),
                                        new XAttribute("epure_ocen", epur_bpd),
                                        new XAttribute("primech", "")
                                        );

                                    Direct.Add(Notes);
                                    sum = 0;

                                    StartM = nextShpal;

                                    iter++;
                                    n++;

                                }
                                else
                                {
                                    sum++;
                                }
                            }
                            XElement Notes1 = new XElement("Note");
                            Notes1.Add(
                                        new XAttribute("iter", n),
                                        new XAttribute("km", RDGetShpal.Last().Km),
                                        new XAttribute("m", pk),
                                        new XAttribute("n", iter),
                                        new XAttribute("epure_fact", epure_fact + ""),
                                        new XAttribute("epure_ocen", epur_bpd),
                                        new XAttribute("primech", "")
                                        );

                            Direct.Add(Notes1);
                        }

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