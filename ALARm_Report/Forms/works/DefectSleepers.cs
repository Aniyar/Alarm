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

namespace ALARm_Report.Forms
{
    public class DefectSleepers : Report
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
                    var digressions = RdStructureService.GetDigressions(mainProcess, new int[] { (int)DigressionName.LongitudinalCrack, (int)DigressionName.SplitsAtTheEnds, (int)DigressionName.PrickingOutPiecesOfWood, (int)DigressionName.FractureOfRCSleeper });
                    if (digressions.Count < 1)
                    {
                        continue;
                    }

                    if (mainProcess.Id == lastProcess)
                    {
                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + "), Путь: " + mainProcess.TrackName + ", ПЧ: " + mainProcess.DistanceName));

                        foreach (var finddeg in digressions)
                        {
                            string eventstr = "";
                            if ((DigName)finddeg.Typ == DigressionName.LongitudinalCrack)
                            {
                                eventstr = "первоначальная замена при текущем содержании";
                            }
                            else if ((DigName)finddeg.Typ == DigressionName.SplitsAtTheEnds)
                            {
                                eventstr = "первоначальная замена при текущем содержании";
                            }
                            else if ((DigName)finddeg.Typ == DigressionName.PrickingOutPiecesOfWood)
                            {
                                //to do
                                //if its in gaps
                                //eventstr = "первоначальная замена при текущем содержании";
                                eventstr = "замена при текущем содержании";
                            }
                            else
                            {
                                //to do
                                //if its in gaps
                                //eventstr = "первоначальная замена при текущем содержании";
                                eventstr = "замена при текущем содержании";
                            }

                            XElement xeElements = new XElement("elements",
                                new XAttribute("n", index),
                                new XAttribute("pchu", "ПЧУ-" + finddeg.PCHU + "/ПД-" + finddeg.PD + "/ПДБ-" + finddeg.PDB),
                                new XAttribute("station", finddeg.StationName),
                                new XAttribute("km", finddeg.Km),
                                new XAttribute("piket", finddeg.Meter / 100 + 1),
                                new XAttribute("meter", finddeg.Meter),
                                new XAttribute("digression", finddeg.Name),
                                new XAttribute("fastening", finddeg.BraceType),
                                new XAttribute("event", eventstr),
                                new XAttribute("notice", ""));
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
                            new XAttribute("road", road.Name),
                            new XAttribute("period", period.Period),
                            new XAttribute("type", mainProcess.GetProcessTypeName),
                            new XAttribute("car", mainProcess.Car),
                            new XAttribute("data", "Проезд: " + mainProcess.Trip_date),
                            new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief));

                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + "), Путь: " + mainProcess.TrackName + ", ПЧ: " + mainProcess.DistanceName));

                        foreach (var finddeg in digressions)
                        {
                            string eventstr = "";
                            if ((int)finddeg.Value == 0)
                                eventstr = " замена при текущем содержании";
                            else if ((int)finddeg.Value == 1)
                                eventstr = "первоначальная замена при текущем содержании";
                            else
                                eventstr = "замена при среднем ремонте";

                            XElement xeElements = new XElement("elements",
                                new XAttribute("n", index),
                                new XAttribute("pchu", "ПЧУ-" + finddeg.PCHU + "/ПД-" + finddeg.PD + "/ПДБ-" + finddeg.PDB),
                                new XAttribute("station", finddeg.StationName),
                                new XAttribute("km", finddeg.Km),
                                new XAttribute("piket", finddeg.Meter / 100 + 1),
                                new XAttribute("meter", finddeg.Meter),
                                new XAttribute("digression", finddeg.Name),
                                new XAttribute("fastening", finddeg.BraceType),
                                new XAttribute("event", eventstr),
                                new XAttribute("notice", finddeg.Primech));
                            xeTracks.Add(xeElements);

                            index++;
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
                htReport.Save(Path.GetTempPath() + "/report_DefectSleepers.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_DefectSleepers.html");
            }
        }
    }
}
