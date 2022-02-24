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
using System.Collections.Generic;
using ALARm_Report.controls;
using System.Globalization;
using System.Linq;

namespace ALARm_Report.Forms
{
    public class DiffTypeSleepers : Report
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

                var road = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;

                var mainProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;

                foreach (var mainProcess in mainProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        xePages = new XElement("pages",
                                new XAttribute("road", road),
                                new XAttribute("period", period.Period),
                                new XAttribute("type", mainProcess.GetProcessTypeName),
                                new XAttribute("car", mainProcess.Car),
                                new XAttribute("chief", mainProcess.Chief),
                                new XAttribute("ps", mainProcess.Car),
                                new XAttribute("data", "" + mainProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                                new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief));
                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + "), Путь: " + trackName + ", ПЧ: " + distance.Code));

                        var digressions = RdStructureService.GetDevRail(mainProcess.Trip_id, distance.Code, trackName.ToString());
                        //digressions = digressions.Where(o => o.Km > 128).ToList();
                        //var digressions = RdStructureService.GetDigressions(mainProcess, new int[] { 1024 });
                        //if (digressions.Count < 1)
                        //{
                        //    continue;
                        //}

                        var start = false;
                        var first = "";
                        var firstMeter = 0;
                        var second = "";
                        var third = "";
                        var thirdMeter = 0;

                        var speed = new List<Speed>();
                        var pdbSection = new List<PdbSection>();
                        Digression previous = null;
                        var sector = "";
                        //список изостыков
                        var list = new List<int> { 8, 9, 10 };

                        for (int i = 0; i <= digressions.Count - 2; i++)
                        {
                            //|| digressions[i].Name == "Стык" ||

                            if ((digressions[i].Name == "ЖБ" || digressions[i].Name == "Дерев") && digressions[i + 1].Name == "Стык" && start == false)
                            {
                                first = digressions[i].Name;
                                firstMeter = digressions[i].Meter;
                                second = digressions[i + 1].Name;
                                start = true;
                            }
                            if (digressions[i].Name == "Стык" && (digressions[i + 1].Name == "ЖБ" || digressions[i + 1].Name == "Дерев") && start == true)
                            {
                                third = digressions[i + 1].Name;
                                thirdMeter = digressions[i+1].Meter;

                                if (first != third && third != "Стык" && (Math.Abs(thirdMeter - firstMeter) < 3))
                                {
                                    if ((previous == null) || (previous.Km != digressions[index].Km))
                                    {
                                        if (digressions[index].Km == 0) continue;
                                        sector = MainTrackStructureService.GetSector(track_id, digressions[index].Km, mainProcess.Date_Vrem);
                                        speed = MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, digressions[index].Km, MainTrackStructureConst.MtoSpeed, mainProcess.DirectionName, trackName.ToString()) as List<Speed>;
                                        pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, digressions[index].Km, MainTrackStructureConst.MtoPdbSection, mainProcess.DirectionName, trackName.ToString()) as List<PdbSection>;
                                    }
                                    previous = digressions[i];

                                    start = false;
                                    //Console.WriteLine($"{digressions[i+1].Km} {digressions[i + 1].Meter} {one} {two} {three}");
                                    XElement xeElements = new XElement("elements",
                                    new XAttribute("n", index),
                                    new XAttribute("pchu", pdbSection.Count > 0 ? $"ПЧУ-{pdbSection[0].Pchu}/ПД-{pdbSection[0].Pd}/ПДБ-{pdbSection[0].Pdb}" : "ПЧУ-/ПД-/ПДБ-"),
                                    new XAttribute("station", sector),
                                    new XAttribute("km", digressions[i].Km),
                                    new XAttribute("piket", digressions[i].Meter / 100 + 1),
                                    new XAttribute("meter", digressions[i].Meter),
                                    new XAttribute("speed", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                    new XAttribute("digression", "стык шпалы разного типа"),
                                    new XAttribute("notice", list.Contains(digressions[i].Oid) ? "ис" : ""));
                                    xeTracks.Add(xeElements);

                                    index++;
                                }
                            }
                        }
                        xePages.Add(xeTracks);

                        //if (mainProcess.Id == lastProcess)
                        //{
                        //    XElement xeTracks = new XElement("tracks",
                        //        new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + "), Путь: " + mainProcess.TrackName + ", ПЧ: " + mainProcess.DistanceName));

                        //    foreach (var finddeg in digressions)
                        //    {
                        //        XElement xeElements = new XElement("elements",
                        //            new XAttribute("n", index),
                        //            new XAttribute("pchu", "ПЧУ-" + finddeg.PCHU + "/ПД-" + finddeg.PD + "/ПДБ-" + finddeg.PDB),
                        //            new XAttribute("station", finddeg.StationName),
                        //            new XAttribute("km", finddeg.Km),
                        //            new XAttribute("piket", finddeg.Meter / 100 + 1),
                        //            new XAttribute("meter", finddeg.Meter),
                        //            new XAttribute("speed", finddeg.FullSpeed),
                        //            new XAttribute("digression", finddeg.Name),
                        //            new XAttribute("notice", finddeg.Primech));
                        //        xeTracks.Add(xeElements);

                        //        index++;
                        //    }

                        //    xePages.Add(xeTracks);
                        //}
                        //else
                        //{
                        //    if (lastProcess != -1)
                        //        report.Add(xePages);
                        //    lastProcess = mainProcess.Id;
                        //    index = 1;

                        //    xePages = new XElement("pages",
                        //        new XAttribute("road", road),
                        //        new XAttribute("period", period.Period),
                        //        new XAttribute("type", mainProcess.GetProcessTypeName),
                        //        new XAttribute("car", mainProcess.Car),
                        //        new XAttribute("data", "Проезд: " + mainProcess.Trip_date),
                        //        new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief));

                        //    XElement xeTracks = new XElement("tracks",
                        //        new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + "), Путь: " + mainProcess.TrackName + ", ПЧ: " + mainProcess.DistanceName));

                        //    foreach (var finddeg in digressions)
                        //    {
                        //        XElement xeElements = new XElement("elements",
                        //            new XAttribute("n", index),
                        //            new XAttribute("pchu", "ПЧУ-" + finddeg.PCHU + "/ПД-" + finddeg.PD + "/ПДБ-" + finddeg.PDB),
                        //            new XAttribute("station", finddeg.StationName),
                        //            new XAttribute("km", finddeg.Km),
                        //            new XAttribute("piket", finddeg.Meter / 100 + 1),
                        //            new XAttribute("meter", finddeg.Meter),
                        //            new XAttribute("speed", finddeg.FullSpeed),
                        //            new XAttribute("digression", finddeg.Name),
                        //            new XAttribute("notice", finddeg.Primech));
                        //        xeTracks.Add(xeElements);

                        //        index++;
                        //    }

                        //    xePages.Add(xeTracks);
                        //}
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
                htReport.Save(Path.GetTempPath() + "/report_DiffTypeSleepers.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_DiffTypeSleepers.html");
            }
        }
    }
}
