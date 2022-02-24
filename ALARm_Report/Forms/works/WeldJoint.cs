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

namespace ALARm_Report.Forms
{
    public class WeldJoint : Report
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
                //var road = AdmStructureService.GetUnit(AdmStructureConst.AdmRoad, nod.Parent_Id) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);

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
                            new XAttribute("ps", mainProcess.Car),
                            new XAttribute("data", mainProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                            new XAttribute("chief", mainProcess.Chief)
                            );


                        //var videoObjects = RdStructureService.GetVideoObjects("paintingMetal crushRailhead", mainProcess);
                        var videoObjects = new List<VideoObject> { };
                        //if (videoObjects.Count < 1)
                        //{
                        //    continue;
                        //}

                        if (mainProcess.Id == lastProcess)
                        {
                            XElement xeTracks = new XElement("tracks",
                                new XAttribute("trackinfo", mainProcess.DirectionName + ", Путь: " + trackName + ", ПЧ: " + distance.Code));

                            foreach (var weldJoint in videoObjects)
                            {
                                XElement xeElements = new XElement("elements",
                                    new XAttribute("pchu", weldJoint.PdbName),
                                    new XAttribute("station", weldJoint.StationName),
                                    new XAttribute("km", weldJoint.Km),
                                    new XAttribute("piket", weldJoint.Pt),
                                    new XAttribute("meter", weldJoint.Mtr),
                                    new XAttribute("speed", weldJoint.Speed),
                                    new XAttribute("digression", weldJoint.Objname),
                                    new XAttribute("amount", weldJoint.Oid == 29 ? "2,0" : "2,5"),
                                    new XAttribute("notice", ""));
                                xeTracks.Add(xeElements);
                            }

                            xePages.Add(xeTracks);
                        }
                        else
                        {
                            if (lastProcess != -1)
                                report.Add(xePages);
                            lastProcess = mainProcess.Id;

                            xePages = new XElement("pages",
                                new XAttribute("road", road),
                                new XAttribute("period", period.Period),
                                new XAttribute("type", mainProcess.GetProcessTypeName),
                                new XAttribute("car", mainProcess.Car),
                                new XAttribute("ps", mainProcess.Car),
                                new XAttribute("chief", mainProcess.Chief),
                                new XAttribute("data", "Проезд: " + mainProcess.Trip_date),
                                new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief));

                            XElement xeTracks = new XElement("tracks",
                                new XAttribute("trackinfo", mainProcess.DirectionName + " , Путь: " + trackName + ", ПЧ: " + distance.Code));

                            foreach (var weldJoint in videoObjects)
                            {
                                XElement xeElements = new XElement("elements",
                                    new XAttribute("pchu", weldJoint.PdbName),
                                    new XAttribute("station", weldJoint.StationName),
                                    new XAttribute("km", weldJoint.Km),
                                    new XAttribute("piket", weldJoint.Pt),
                                    new XAttribute("meter", weldJoint.Mtr),
                                    new XAttribute("speed", weldJoint.Speed),
                                    new XAttribute("digression", weldJoint.Objname),
                                    new XAttribute("amount", weldJoint.Oid == 29 ? "2,0" : "2,5"),
                                    new XAttribute("notice", ""));
                                xeTracks.Add(xeElements);
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

                htReport.Save(Path.GetTempPath() + "/report_WeldJoint.html");

            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_WeldJoint.html");
            }
        }
    }
}
