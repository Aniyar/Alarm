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
using System.Linq;
using ALARm_Report.controls;
using System.Collections.Generic;
using System.Globalization;

namespace ALARm_Report.Forms
{
    public class SleepersFilingBallast : Report
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
            int distanceTotal = 0, trackTotal = 0;

            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");
                XElement xePages = new XElement("pages");

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;

                var mainProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                var roadName = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);


                foreach (var mainProcess in mainProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        //var digressions = RdStructureService.GetDigressions(mainProcess, new int[] { 1014 });
                        var Ballast = RdStructureService.GetBallast(mainProcess);
                        if (Ballast.Count < 1)
                        {
                            continue;
                        }
                        var kms = Ballast.Select(o => o.Km).Distinct().ToList();

                        xePages = new XElement("pages",
                                new XAttribute("road", roadName),
                                new XAttribute("period", period.Period),
                                new XAttribute("type", mainProcess.GetProcessTypeName),
                                new XAttribute("car", mainProcess.Car),
                                new XAttribute("chief", mainProcess.Chief),
                                new XAttribute("ps", mainProcess.Car),
                                new XAttribute("data", mainProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                                new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief));


                        trackTotal = 0;
                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + "), Путь: " + trackName + ", ПЧ: " + distance.Code));

                        foreach (var km in kms)
                        {
                            Ballast = Ballast.Where(o => o.Km == km).ToList();
                            var digression = "скрепления засыпаны щебнем";
                            var fastening = "КБ";
                            var length = 1;
                            var kmm = Ballast.First().Km * 1000.0;
                            var mmm = Ballast.Last().Km * 1000.0;
                            var lengthm = (int)Math.Abs(((Ballast.First().Km + (double)Ballast.First().Meter / 10000) - (Ballast.Last().Km + (double)Ballast.Last().Meter / 10000)) * 10000);
                            //todo  
                            for (int i = 1; i < Ballast.Count(); i++)
                            {
                                if (Math.Min(Ballast[i].Right_data, Ballast[i].Left_data) < 20)
                                    length++;
                            }


                            //if (length == 0) break;

                            XElement xeElements = new XElement("elements",
                                    new XAttribute("n", index),
                                    new XAttribute("startkm", Ballast.First().Km),
                                    new XAttribute("startm", Ballast.First().Meter),
                                    new XAttribute("finalkm", Ballast.Last().Km),
                                    new XAttribute("finalm", Ballast.Last().Meter),
                                    new XAttribute("digression", digression),
                                    new XAttribute("fastening", fastening),
                                    new XAttribute("length", lengthm));
                            xeTracks.Add(xeElements);


                            index++;
                            distanceTotal += length;
                            trackTotal += length;
                        }

                        xeTracks.Add(new XAttribute("tracktotal", "Итого путь-" + trackName + ": " + trackTotal.ToString() + " м"));
                        xePages.Add(xeTracks);
                    }
                }

                xePages.Add(new XAttribute("distancetotal", "Итого по ПЧ-" + distance.Code + ": " + distanceTotal.ToString() + " м"));
                report.Add(xePages);
                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_SleepersFilingBallast.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_SleepersFilingBallast.html");
            }
        }
    }
}
