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

namespace ALARm_Report.Forms
{
    public class DefShpal : Report
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

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var videoProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (videoProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                foreach (var mainProcess in videoProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        if (lastProcess != -1)
                            report.Add(xePages);
                        lastProcess = mainProcess.Id;
                        index = 1;

                        var directName = AdditionalParametersService.DirectName(mainProcess.Id, (int)distance.Id);

                        xePages = new XElement("pages",
                                    new XAttribute("road", road),
                                    new XAttribute("period", period.Period),
                                    new XAttribute("type", mainProcess.GetProcessTypeName),
                                    new XAttribute("car", mainProcess.Car),
                                    new XAttribute("data",  mainProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                                    new XAttribute("chief", mainProcess.Chief),
                                    new XAttribute("ps", mainProcess.Car),
                                    new XAttribute("info", "ПС: " + mainProcess.Car + " " + mainProcess.Chief));

                        XElement xeTracks = new XElement(
                            "tracks", new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + ")" + " / Путь: " + trackName + " / ПЧ: " + distance.Code));


                        var Check_defshpal_state = AdditionalParametersService.Check_defshpal_state(mainProcess.Trip_id, template.ID);

                        Check_defshpal_state = Check_defshpal_state.Where(o => o.Km.Between(710, 720)).ToList();


                        //Участки дист коррекция
                        var dist_section = MainTrackStructureService.GetDistSectionByDistId(distance.Id);
                        foreach (var item in Check_defshpal_state)
                        {
                            var ds = dist_section.Where(o => item.Km + item.Meter/10000 >= o.Start_Km + o.Start_M/10000 && item.Km + item.Meter/10000 <= o.Final_Km + o.Final_M/10000).ToList();

                            item.Pchu = $"{ds.First().note}";
                        }


                        foreach (var finddeg in Check_defshpal_state)
                        {
                            XElement xeElements = new XElement("elements",
                               new XAttribute("n", index),
                               new XAttribute("pchu", finddeg.Pchu),
                               new XAttribute("station", finddeg.Station),
                               new XAttribute("km", finddeg.Km),
                               new XAttribute("pt", finddeg.Meter / 100 + 1),
                               new XAttribute("meter", finddeg.Meter),
                               new XAttribute("deviation", finddeg.Otst),
                               new XAttribute("fast", finddeg.Fastening),
                               new XAttribute("merop", finddeg.Meropr),
                               new XAttribute("notice", finddeg.Notice),

                               new XAttribute("CarPosition", (int)mainProcess.Car_Position),
                               new XAttribute("repType", (int)RepType.Def_Shpala),
                               new XAttribute("fileId", finddeg.file_id),
                               new XAttribute("Ms", finddeg.Ms),
                               new XAttribute("fNum", finddeg.Fnum)
                               );
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
                //htReport.Save(@"\\DESKTOP-EMAFC5J\sntfi\report_shpal.html");
                htReport.Save(Path.GetTempPath() + "/report.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                //System.Diagnostics.Process.Start(@"http://DESKTOP-EMAFC5J:5500/report_shpal.html");
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report.html");
            }
        }
    }
}




