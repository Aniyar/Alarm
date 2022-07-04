using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm_Report.Forms
{
    public class AbsenceBoltJoint : Report
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

            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");
                XElement xePages = new XElement("pages");

                var videoProcesses = RdStructureService.GetTripsOnDistance(distanceId, period);
                if (videoProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);

                foreach (var mainProcess in videoProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        int n = 1;
                        var trackName = AdmStructureService.GetTrackName(track_id);
                          
                        var trip = RdStructureService.GetTrip(mainProcess.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);

                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();

                        if (kilometers.Count == 0) continue;

                        xePages = new XElement("pages",
                           new XAttribute("road", road),
                           new XAttribute("period", period.Period),
                           new XAttribute("type", mainProcess.GetProcessTypeName),
                           new XAttribute("car", mainProcess.Car),
                           new XAttribute("chief", mainProcess.Chief),
                           new XAttribute("ps", mainProcess.Car),
                            new XAttribute("data", "" + mainProcess.Trip_date.ToString("dd.MM.yyyy_hh:mm")),
                           
                           new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief));

                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", mainProcess.Direction + " (" + mainProcess.DirectionCode + "), Путь: " + trackName + ", ПЧ: " + distance.Code));

                        List<Digression> Check_bolt_state = AdditionalParametersService.Check_bolt_state(mainProcess.Id, template.ID);

                        //Check_bolt_state = Check_bolt_state.Where(o => o.Km.Between(710, 720)).ToList();

                        for (var index = 0; index < Check_bolt_state.Count(); index++)
                        {
                            XElement xeElements = new XElement("elements",
                                new XAttribute("n", index + 1),
                                new XAttribute("pchu", Check_bolt_state[index].Pchu),
                                new XAttribute("station", Check_bolt_state[index].Station),
                                new XAttribute("km", Check_bolt_state[index].Km),
                                new XAttribute("piket", Check_bolt_state[index].Meter / 100 + 1),
                                new XAttribute("meter", Check_bolt_state[index].Meter),
                                new XAttribute("speed", Check_bolt_state[index].Speed),
                                new XAttribute("overlay", Check_bolt_state[index].Overlay),
                                new XAttribute("before", Check_bolt_state[index].Before),
                                new XAttribute("after", Check_bolt_state[index].After), 
                                new XAttribute("Threat_id", Check_bolt_state[index].Threat==Threat.Right ? "Правая": "Левая"), 
                                new XAttribute("speed2", $"{Check_bolt_state[index].FullSpeed}"),
                                new XAttribute("notice", $"{Check_bolt_state[index].Note}"),

                                new XAttribute("CarPosition", (int)mainProcess.Car_Position),
                                new XAttribute("repType", (int)RepType.Bolt),
                                new XAttribute("fileId", Check_bolt_state[index].Fileid),
                                new XAttribute("Ms", Check_bolt_state[index].Ms),
                                new XAttribute("fNum", Check_bolt_state[index].Fnum)
                                );
                            xeTracks.Add(xeElements);
                            n++;
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
               // htReport.Save($@"G:\form\4.Выходные формы Видеоконтроля ВСП\23.Ведомость отсутствующих болтовых соединений .html");
                //htReport.Save(@"\\DESKTOP-EMAFC5J\sntfi\report_AbsenceBoltJoint.html");
                htReport.Save(Path.GetTempPath() + "/report_AbsenceBoltJoint.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                //System.Diagnostics.Process.Start(@"http://DESKTOP-EMAFC5J:5500/report_AbsenceBoltJoint.html");
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_AbsenceBoltJoint.html");
            }
        }
    }
}