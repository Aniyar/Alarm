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
    public class DeviationsInRailJoint : Report
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
                    var digressions = RdStructureService.GetDigressions(mainProcess, new int[] { 1028, 1029, 1030, 1031, 1032, 1033, 1034, 1035 });
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
                            XElement xeElements = new XElement("elements",
                                new XAttribute("n", index),
                                new XAttribute("pchu", "ПЧУ-" + finddeg.PCHU + "/ПД-" + finddeg.PD + "/ПДБ-" + finddeg.PDB),
                                new XAttribute("station", finddeg.StationName),
                                new XAttribute("km", finddeg.Km),
                                new XAttribute("piket", finddeg.Meter / 100 + 1),
                                new XAttribute("meter", finddeg.Meter),
                                new XAttribute("speed", finddeg.FullSpeed),
                                new XAttribute("digression", finddeg.Name),
                                new XAttribute("t", finddeg.Length.ToString() + "°"),
                                new XAttribute("size", finddeg.Value),
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
                            XElement xeElements = new XElement("elements",
                                new XAttribute("n", index),
                                new XAttribute("pchu", "ПЧУ-" + finddeg.PCHU + "/ПД-" + finddeg.PD + "/ПДБ-" + finddeg.PDB),
                                new XAttribute("station", finddeg.StationName),
                                new XAttribute("km", finddeg.Km),
                                new XAttribute("piket", finddeg.Meter / 100 + 1),
                                new XAttribute("meter", finddeg.Meter),
                                new XAttribute("speed", finddeg.FullSpeed),
                                new XAttribute("digression", finddeg.Name),
                                new XAttribute("t", finddeg.Length.ToString() + "°"),
                                new XAttribute("size", finddeg.Value),
                                new XAttribute("addspeed", finddeg.AllowSpeed),
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
                htReport.Save(Path.GetTempPath() + "/report_DeviationsInRailJoint.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_DeviationsInRailJoint.html");
            }
        }
    }
}
