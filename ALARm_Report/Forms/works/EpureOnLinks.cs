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
using System.Linq;
using System.Reflection;

namespace ALARm_Report.Forms
{
    public class EpureOnLinks : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
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
                var road = AdmStructureService.GetUnit(AdmStructureConst.AdmRoad, nod.Parent_Id) as AdmUnit;

                foreach (var mainProcess in mainProcesses)
                {
                    var sleepers = RdStructureService.GetSleepers();
                    if (sleepers.Count < 1)
                    {
                        continue;
                    }
                    /*var digressions = RdStructureService.GetRdTables(mainProcess, 5) as List<RdEpure>;
                    if (digressions.Count < 1)
                    {
                        continue;
                    }*/

                    if (mainProcess.Id == lastProcess)
                    {
                        XElement xeTracks = new XElement("trackstracks",
                            new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + "), Путь: " + mainProcess.TrackName + ", ПЧ: " + mainProcess.DistanceName));

                        int sCoord = sleepers.Min(s => s.Km * 1000 + s.Mtr), fCoord = sleepers.Max(s => s.Km * 1000 + s.Mtr);
                        int mCoord = sCoord - sCoord % 25 + 25;
                        fCoord = fCoord - fCoord % 25 + 25;

                        while (mCoord <= fCoord)
                        {
                            int count = sleepers.Where(s => s.Km * 1000.0 + s.Mtr + s.Mm / 1000.0 <= mCoord && s.Km * 1000.0 + s.Mtr + s.Mm / 1000.0 >= sCoord && s.Threat == Threat.Left).Count() * 40;
                            var trackClasses = (List<TrackClass>)MainTrackStructureService.GetMtoObjectsByCoord(DateTime.Parse(mainProcess.Trip_date), mCoord, MainTrackStructureConst.MtoTrackClass, mainProcess.TrackID);
                            int trackClass = trackClasses.Any() ? trackClasses.First().Class_Id : 1;

                            XElement xeElements = new XElement("elements",
                                new XAttribute("n", index),
                                new XAttribute("km", sCoord / 1000),
                                new XAttribute("piket", ((sCoord % 1000) / 100) + 1),
                                new XAttribute("link", ((sCoord % 1000) % 100) / 25 + 1),
                                new XAttribute("epureactual", count),
                                new XAttribute("epureevaluation", trackClass == 1 ? 2000 : 1840),
                                new XAttribute("notice", ""));
                            xeTracks.Add(xeElements);

                            index++;

                            sCoord = mCoord;
                            mCoord += 25;
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
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                          
                            new XAttribute("road", road.Name),
                            new XAttribute("period", period.Period),
                            new XAttribute("type", mainProcess.GetProcessTypeName),
                            new XAttribute("car", mainProcess.Car),
                            new XAttribute("chief", mainProcess.Chief),
                            new XAttribute("ps", mainProcess.Car),
                            new XAttribute("data", "" + mainProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                            new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief));

                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + "), Путь: " + mainProcess.TrackName + ", ПЧ: " + mainProcess.DistanceName));

                        int sCoord = sleepers.Min(s => s.Km * 1000 + s.Mtr), fCoord = sleepers.Max(s => s.Km * 1000 + s.Mtr);
                        int mCoord = sCoord - sCoord % 25 + 25;
                        fCoord = fCoord - fCoord % 25 + 25;

                        while (mCoord <= fCoord)
                        {
                            int count = sleepers.Where(s => s.Km * 1000.0 + s.Mtr + s.Mm / 1000.0 <= mCoord && s.Km * 1000.0 + s.Mtr + s.Mm / 1000.0 >= sCoord && s.Threat == Threat.Left).Count() * 40;
                            var trackClasses = (List<TrackClass>)MainTrackStructureService.GetMtoObjectsByCoord(DateTime.Parse(mainProcess.Trip_date), mCoord, MainTrackStructureConst.MtoTrackClass, mainProcess.TrackID);
                            int trackClass = trackClasses.Any() ? trackClasses.First().Class_Id : 1;

                            XElement xeElements = new XElement("elements",
                                new XAttribute("n", index),
                                new XAttribute("km", sCoord / 1000),
                                new XAttribute("piket", ((sCoord % 1000) / 100) + 1),
                                new XAttribute("link", ((sCoord % 1000) % 100) / 25 + 1),
                                new XAttribute("epureactual", count),
                                new XAttribute("epureevaluation", trackClass == 1 ? 2000 : 1840),
                                new XAttribute("notice", ""));
                            xeTracks.Add(xeElements);

                            index++;

                            sCoord = mCoord;
                            mCoord += 25;
                        }

                        xePages.Add(xeTracks);
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
                htReport.Save(Path.GetTempPath() + "/report_EpureOnLinks.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_EpureOnLinks.html");
            }
        }
    }
}
