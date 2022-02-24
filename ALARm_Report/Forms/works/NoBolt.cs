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

namespace ALARm_Report.Forms
{
    public class NoBolt : Report
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

                var videoProcesses = RdStructureService.GetMainParametersProcesses(period, distanceId, true);
                if (videoProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                var road = AdmStructureService.GetUnit(AdmStructureConst.AdmRoad, nod.Parent_Id) as AdmUnit;

                
                foreach (var mainProcess in videoProcesses)
                {
                    //if (mainProcess.Trip_id != 126) { continue; }

                    var digressions = RdStructureService.NoBolt(mainProcess, Threat.Left);
                    if (digressions == null) { continue; }

                    if (digressions.Count < 1)
                    {
                        continue;
                    }

                    
                    if (mainProcess.Id == lastProcess)
                    {
                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + "), Путь: " + mainProcess.TrackName + ", ПЧ: " + mainProcess.DistanceName));


                        digressions = digressions.Where(o => o.Km > 128).ToList();

                        foreach (var finddeg in digressions)
                        {
                            XElement xeElements = new XElement("elements",
                                new XAttribute("n", index),
                                new XAttribute("km", finddeg.Km),
                                new XAttribute("piket", finddeg.Meter / 100 + 1),
                                new XAttribute("meter", finddeg.Meter),
                                new XAttribute("speed", finddeg.Fnum+""),
                                new XAttribute("deviation", finddeg.Y+""),
                                new XAttribute("fileId", finddeg.File_id),
                               
                                new XAttribute("Ms", finddeg.Ms),
                                new XAttribute("fNum", finddeg.Fnum),

                                new XAttribute("fastening", finddeg.BraceType+""));
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
                              new XAttribute("chief", mainProcess.Chief),
                                  new XAttribute("ps", mainProcess.Car),
                            new XAttribute("period", period.Period),
                            new XAttribute("type", mainProcess.GetProcessTypeName),
                            new XAttribute("car", mainProcess.Car),
                            new XAttribute("data",  mainProcess.Trip_date),
                            new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief));

                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + "), Путь: " + mainProcess.TrackName + ", ПЧ: " + mainProcess.DistanceName));

                        digressions = digressions.Where(o => o.Km > 128).ToList();

                        for (int i=0; i< digressions.Count -2;  i++)
                        {
                            if(digressions[i].Oid == 2 && digressions[i+1].Oid != 2 && digressions[i].Meter == digressions[i+1].Meter)
                            {
                                XElement xeElements = new XElement("elements",
                                    new XAttribute("n", index),
                                    new XAttribute("km", digressions[i].Km),
                                    new XAttribute("piket", digressions[i].Meter / 100 + 1),
                                    new XAttribute("meter", digressions[i].Meter),
                                    new XAttribute("speed", "-/-/-"),
                                    new XAttribute("deviation", digressions[i].Oid == 2 ? "Отсутствие болта" : ""),
                                    new XAttribute("fastening", "GBR"),
                                    new XAttribute("fileId", digressions[i].File_id),
                                    new XAttribute("Ms", digressions[i].Ms),
                                    new XAttribute("fNum", digressions[i].Fnum)
                                    );
                                xeTracks.Add(xeElements);
                                index++;
                                i++;
                            }else if(digressions[i].Oid != 2 && digressions[i+1].Oid == 2 && digressions[i].Meter == digressions[i+1].Meter)
                            {
                                XElement xeElements = new XElement("elements",
                                    new XAttribute("n", index),
                                    new XAttribute("km", digressions[i+1].Km),
                                    new XAttribute("piket", digressions[i + 1].Meter / 100 + 1),
                                    new XAttribute("meter", digressions[i + 1].Meter),
                                    new XAttribute("speed", "-/-/-"),
                                    new XAttribute("deviation", digressions[i + 1].Oid == 2 ? "Отсутствие болта" : ""),
                                    new XAttribute("fileId", digressions[i].File_id),
                                    new XAttribute("Ms", digressions[i].Ms),
                                    new XAttribute("fNum", digressions[i].Fnum),

                                    new XAttribute("fastening", "GBR"));
                                xeTracks.Add(xeElements);
                                index++;
                                i++;
                            }
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
                htReport.Save("C:/sntfi/report_NoBolt.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start("C:/sntfi/report_NoBolt.html");
            }
        }
    }
}
