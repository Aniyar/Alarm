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
using System.Linq;
using System.Globalization;

namespace ALARm_Report.Forms
{
    public class ViolationPerpendicularity : Report
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

                var videoProcesses = RdStructureService.GetProcess(period, distanceId, ProcessType.VideoProcess);
                if (videoProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);
                foreach (var mainProcess in videoProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {

                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var trip = RdStructureService.GetTrip(mainProcess.Id);
                        var kms = RdStructureService.GetKilometersByTrip(trip);
                        if (!kms.Any()) continue;

                        kms = kms.Where(o => o.Track_id == track_id).ToList();
                      
                        trip.Track_Id = track_id;
                        var lkm = kms.Select(o => o.Number).ToList();

                        if (lkm.Count() == 0) continue;
                     
                        //var trackName = AdmStructureService.GetTrackName(track_id);

                        xePages = new XElement("pages",
                                new XAttribute("road", road),
                                new XAttribute("period", period.Period),
                                new XAttribute("type", mainProcess.GetProcessTypeName),
                                new XAttribute("car", mainProcess.Car),
                                new XAttribute("chief", mainProcess.Chief),
                                new XAttribute("ps", mainProcess.Car),
                                new XAttribute("data", mainProcess.Date_Vrem.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))),
                                new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief));

                        XElement xeTracks = new XElement("tracks",
                                new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + ")" + " / Путь: " + trackName + " / ПЧ: " + distance.Code));
                        List<Digression> check_ViolPerpen = AdditionalParametersService.Check_ViolPerpen(mainProcess.Trip_id);

                        foreach (var finddeg in check_ViolPerpen)
                        {
                            XElement xeElements = new XElement("elements",
                                new XAttribute("n", index),
                                new XAttribute("km", finddeg.Km),
                                new XAttribute("piket", finddeg.Meter / 100 + 1),
                                new XAttribute("meter", finddeg.Meter),
                                new XAttribute("speed", finddeg.Vdop),
                                new XAttribute("deviation", finddeg.Angle.ToString("0.0")),
                                new XAttribute("fastening", finddeg.Fastener),

                                new XAttribute("CarPosition", -1),
                                new XAttribute("repType", (int)RepType.Gaps),

                                new XAttribute("fileId", finddeg.file_id + ""),
                                new XAttribute("Ms", finddeg.Ms + ""),
                                new XAttribute("fNum", finddeg.Fnum + ""));
                            xeTracks.Add(xeElements);
                            index++;
                        }
                        xePages.Add(xeTracks);
                        report.Add(xePages);
                    }
                   
                }
             
              
                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_ViolationPerpendicularity.html");
                //htReport.Save(@"\\DESKTOP-EMAFC5J\sntfi\report_ViolationPerpendicularity.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                //System.Diagnostics.Process.Start(@"http://DESKTOP-EMAFC5J:5500/report_ViolationPerpendicularity.html");
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_ViolationPerpendicularity.html");
            }
        }
    }
}
