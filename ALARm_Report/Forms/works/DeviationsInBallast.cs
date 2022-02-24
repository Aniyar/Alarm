using ALARm.Core;
using ALARm.Core.Report;
using MetroFramework.Controls;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using ALARm.Services;
using ALARm_Report.controls;
using System.Collections.Generic;

namespace ALARm_Report.Forms
{
    public class DeviationsInBallast : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            //Сделать выбор периода
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(distanceId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel)
                    return;
                admTracksId = choiceForm.admTracksIDs;
            }

            Int64 lastProcess = -1;
            int index = 1;

            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");
                XElement xePages = new XElement("pages");

                var mainProcesses = RdStructureService.GetMainParametersProcesses(period, distanceId);
                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                var roadName = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);

                foreach (var mainProcess in mainProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var digressions = RdStructureService.GetDigressions(mainProcess, new int[] { 1025, 1026, 1027 });
                        //if (digressions.Count < 1)
                        //{
                        //    continue;
                        //}

                        if (mainProcess.Id == lastProcess)
                        {
                            XElement xeTracks = new XElement("tracks",
                                new XAttribute("trackinfo", mainProcess.DirectionName + ", Путь: " + trackName + ", ПЧ: " + distance.Code));

                            foreach (var finddeg in digressions)
                            {
                                string amount = (int)finddeg.Typ == 1025 ? finddeg.Length.ToString() + " шп.ящиков" : finddeg.Length.ToString() + "%";
                                string meter = (int)finddeg.Typ == 1025 ? (finddeg.Meter).ToString() : "";
                                string piket = (int)finddeg.Typ != 1026 ? (finddeg.Meter / 100 + 1).ToString() : "";

                                XElement xeElements = new XElement("elements",
                                    new XAttribute("n", index),
                                    new XAttribute("pchu", "ПЧУ-" + finddeg.PCHU + "/ПД-" + finddeg.PD + "/ПДБ-" + finddeg.PDB),
                                    new XAttribute("station", finddeg.StationName),
                                    new XAttribute("km", finddeg.Km),
                                    new XAttribute("piket", piket),
                                    new XAttribute("meter", meter),
                                    new XAttribute("speed", finddeg.FullSpeed),
                                    new XAttribute("trackclass", finddeg.TrackClass),
                                    new XAttribute("digression", finddeg.Name),
                                    new XAttribute("percentage", amount),
                                    new XAttribute("addspeed", finddeg.AllowSpeed),
                                    new XAttribute("notice", finddeg.Primech));
                                xeTracks.Add(xeElements);

                                index++;
                            }

                            xePages.Add(xeTracks);
                        }
                        else
                        {
                            if (lastProcess != -1)
                                report.Add(xePages);
                            lastProcess = mainProcess.Id;
                            index = 1;

                            xePages = new XElement("pages",
                                new XAttribute("road", roadName),
                                new XAttribute("period", period.Period),
                                new XAttribute("type", mainProcess.GetProcessTypeName),
                                new XAttribute("car", mainProcess.Car),
                                new XAttribute("ps", mainProcess.Car),
                                new XAttribute("chief", mainProcess.Chief),
                                new XAttribute("data", "Проезд: " + mainProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                                new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief));

                            XElement xeTracks = new XElement("tracks",
                                new XAttribute("trackinfo", mainProcess.DirectionName + " Путь: " + trackName + ", ПЧ: " + distance.Code));

                            foreach (var finddeg in digressions)
                            {
                                string amount = (int)finddeg.Typ == 1025 ? finddeg.Length.ToString() + " шп.ящиков" : finddeg.Length.ToString() + "%";
                                string meter = (int)finddeg.Typ == 1025 ? (finddeg.Meter).ToString() : "";
                                string piket = (int)finddeg.Typ != 1026 ? (finddeg.Meter / 100 + 1).ToString() : "";

                                XElement xeElements = new XElement("elements",
                                    new XAttribute("n", index),
                                    new XAttribute("pchu", "ПЧУ-" + finddeg.PCHU + "/ПД-" + finddeg.PD + "/ПДБ-" + finddeg.PDB),
                                    new XAttribute("station", finddeg.StationName),
                                    new XAttribute("km", finddeg.Km),
                                    new XAttribute("piket", piket),
                                    new XAttribute("meter", meter),
                                    new XAttribute("speed", finddeg.FullSpeed),
                                    new XAttribute("trackclass", finddeg.TrackClass),
                                    new XAttribute("digression", finddeg.Name),
                                    new XAttribute("percentage", amount),
                                    new XAttribute("addspeed", finddeg.AllowSpeed),
                                    new XAttribute("notice", finddeg.Primech));
                                xeTracks.Add(xeElements);

                                index++;
                            }

                            xePages.Add(xeTracks);
                        }
                    }
                }

                report.Add(xePages);
                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_DeviationsInBallast.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_DeviationsInBallast.html");
            }
        }
    }
}

//using ALARm.Core;
//using ALARm.Core.Report;
//using MetroFramework.Controls;
//using System;
//using System.IO;
//using System.Windows.Forms;
//using System.Xml;
//using System.Xml.Linq;
//using System.Xml.Xsl;
//using ALARm.Services;
//using System.Collections.Generic;
//using ALARm.Core.AdditionalParameteres;
//using ALARm_Report.controls;
//using ALARm.DataAccess;
//using System.Globalization;
//using System.Linq;

//namespace ALARm_Report.Forms
//{
//    public class DeviationsInBallast : Report
//    {
//        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
//        {
//            //Сделать выбор периода
//            List<long> admTracksId = new List<long>();
//            using (var choiceForm = new ChoiseForm(0))
//            {
//                choiceForm.SetTripsDataSource(distanceId, period);
//                choiceForm.ShowDialog();
//                if (choiceForm.dialogResult == DialogResult.Cancel)
//                    return;
//                admTracksId = choiceForm.admTracksIDs;
//            }

//            Int64 lastProcess = -1;

//            XDocument htReport = new XDocument();

//            using (XmlWriter writer = htReport.CreateWriter())
//            {
//                XDocument xdReport = new XDocument();
//                XElement report = new XElement("report");
//                XElement xePages = new XElement("pages");

//                var road = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);
//                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;

//                var videoProcesses =// RdStructureService.GetMainParametersProcesses(period, distanceId);
//                RdStructureService.GetMainParametersProcess(period, distance.Name);
//                if (videoProcesses.Count == 0)
//                {
//                    MessageBox.Show(Properties.Resources.paramDataMissing);
//                    return;
//                }


//                progressBar.Maximum = videoProcesses.Count;

//                foreach (var videoProcess in videoProcesses)
//                {
//                    progressBar.Value = videoProcesses.IndexOf(videoProcess) + 1;

//                    foreach (var track_id in admTracksId)
//                    {
//                        var trackName = AdmStructureService.GetTrackName(track_id);
//                        videoProcess.TrackName = trackName.ToString();


//                        xePages = new XElement("pages",
//                            new XAttribute("road", road),
//                            new XAttribute("period", period.Period),
//                            new XAttribute("type", videoProcess.GetProcessTypeName),
//                            new XAttribute("car", videoProcess.Car),
//                            new XAttribute("chief", videoProcess.Chief),
//                            new XAttribute("ps", videoProcess.Car),
//                            new XAttribute("data", videoProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
//                            new XAttribute("info", videoProcess.Car + " " + videoProcess.Chief));

//                        XElement xeTracks = new XElement("tracks",
//                            new XAttribute("trackinfo", videoProcess.DirectionName + " (" + videoProcess.DirectionCode + "), Путь: " + trackName + ", ПЧ: " + distance.Code));
//                        progressBar.Value = videoProcesses.IndexOf(videoProcess) + 1;
//                        List<Digression> Check_deviationsinbalast_state = AdditionalParametersService.Check_Total_balast(videoProcess.Trip_id, template.ID);


//                        Check_deviationsinbalast_state = Check_deviationsinbalast_state.ToList();

//                        for (int index = 0; index < Check_deviationsinbalast_state.Count(); index++)
//                        {
//                            //if (Check_deviationsinfastening_state[index].Vdop == "") continue;
//                            XElement xeElements = new XElement("elements",
//                                                             new XAttribute("n", index),
//                                                             new XAttribute("pchu", Check_deviationsinbalast_state[index].Pchu),
//                                                             new XAttribute("station", Check_deviationsinbalast_state[index].Station),
//                                                             new XAttribute("km", Check_deviationsinbalast_state[index].Km),
//                                                             new XAttribute("piket", Check_deviationsinbalast_state[index].Mtr / 100 + 1),
//                                                             new XAttribute("meter", Check_deviationsinbalast_state[index].Mtr),
//                                                             new XAttribute("speed", Check_deviationsinbalast_state[index].Vpz),
//                                                             new XAttribute("trackclass", /*Check_deviationsinbalast_state[index].TrackClass*/""),
//                                                             new XAttribute("digression", Check_deviationsinbalast_state[index].RailType),
//                                                             new XAttribute("percentage", Check_deviationsinbalast_state[index].Norma),
//                                                             new XAttribute("addspeed", Check_deviationsinbalast_state[index].Vdop),
//                                                             new XAttribute("notice", Check_deviationsinbalast_state[index].Primech));

//                            //XElement xeElements = new XElement("elements",
//                            //        new XAttribute("n", index + 1),
//                            //        new XAttribute("pchu", Check_deviationsinbalast_state[index].Pchu),
//                            //        new XAttribute("station", Check_deviationsinbalast_state[index].Station),
//                            //        new XAttribute("km", Check_deviationsinbalast_state[index].Km),
//                            //        new XAttribute("piket", Check_deviationsinbalast_state[index].Mtr / 100 + 1),
//                            //        new XAttribute("meter", Check_deviationsinbalast_state[index].Mtr),
//                            //        new XAttribute("speed", Check_deviationsinbalast_state[index].Vpz),
//                            //        new XAttribute("Otst", Check_deviationsinbalast_state[index].Ots),
//                            //        new XAttribute("thread", Check_deviationsinbalast_state[index].Threat_id),
//                            //        new XAttribute("fastening", Check_deviationsinbalast_state[index].Fastening),
//                            //        new XAttribute("amount", Check_deviationsinbalast_state[index].Kol),
//                            //        new XAttribute("tripplan", Check_deviationsinbalast_state[index].Tripplan),
//                            //        new XAttribute("template", Check_deviationsinbalast_state[index].Norma),
//                            //        new XAttribute("addspeed", Check_deviationsinbalast_state[index].Vdop),
//                            //        new XAttribute("fileId", Check_deviationsinbalast_state[index].Fileid),
//                            //        new XAttribute("Ms", Check_deviationsinbalast_state[index].Ms),
//                            //        new XAttribute("fNum", Check_deviationsinbalast_state[index].Fnum)
//                            //        );
//                            xeTracks.Add(xeElements);
//                        }
//                        xePages.Add(xeTracks);
//                    }
//                }
//                report.Add(xePages);
//                xdReport.Add(report);

//                XslCompiledTransform transform = new XslCompiledTransform();
//                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
//                transform.Transform(xdReport.CreateReader(), writer);
//            }
//            try
//            {
//                //htReport.Save(@"\\Desktop-tolegen\sntfi\report_Fastening.html");
//                htReport.Save(Path.GetTempPath() + "/report_DeviationsInBallast.html");
//            }
//            catch
//            {
//                MessageBox.Show("Ошибка сохранения файлы");
//            }
//            finally
//            {
//                //System.Diagnostics.Process.Start(@"http://Desktop-tolegen:5500/report_Fastening.html");
//                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_DeviationsInBallast.html");
//            }
//        }
//    }
//}

