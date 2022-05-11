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
using ALARm.Core.AdditionalParameteres;
using ALARm_Report.controls;
using ALARm.DataAccess;
using System.Globalization;
using System.Linq;

namespace ALARm_Report.Forms
{
    public class DeviationsInFastening : Report
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

                var road = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;

                var videoProcesses = //RdStructureService.GetMainParametersProcess(period, distanceId.ToString());
                                       RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (videoProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }


                progressBar.Maximum = videoProcesses.Count;

                foreach (var videoProcess in videoProcesses)
                {
                    progressBar.Value = videoProcesses.IndexOf(videoProcess) + 1;

                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        videoProcess.TrackName = trackName.ToString();


                            xePages = new XElement("pages",
                                new XAttribute("road", road),
                                new XAttribute("period", period.Period),
                                new XAttribute("type", videoProcess.GetProcessTypeName),
                                new XAttribute("car", videoProcess.Car),
                                new XAttribute("chief", videoProcess.Chief),
                                new XAttribute("ps", videoProcess.Car),
                                new XAttribute("data", videoProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                                new XAttribute("info", videoProcess.Car + " " + videoProcess.Chief));

                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", videoProcess.DirectionName + " (" + videoProcess.DirectionCode + "), Путь: " + trackName + ", ПЧ: " + distance.Code));
                        progressBar.Value = videoProcesses.IndexOf(videoProcess) + 1;
                        List<Digression> Check_deviationsinfastening_state = AdditionalParametersService.Check_deviationsinfastening_state(videoProcess.Trip_id, template.ID);

                      
                        Check_deviationsinfastening_state = Check_deviationsinfastening_state.ToList();
                        
                        for (int index = 0; index < Check_deviationsinfastening_state.Count(); index++)
                        {
                            //if (Check_deviationsinfastening_state[index].Vdop == "") continue;
                            
                            XElement xeElements = new XElement("elements",
                                 new XAttribute("n", index + 1),
                                 new XAttribute("pchu", Check_deviationsinfastening_state[index].Pchu),
                                 new XAttribute("station", Check_deviationsinfastening_state[index].Station),
                                 new XAttribute("km", Check_deviationsinfastening_state[index].Km),
                                 new XAttribute("piket", Check_deviationsinfastening_state[index].Mtr / 100 + 1),
                                 new XAttribute("meter", Check_deviationsinfastening_state[index].Mtr),
                                 new XAttribute("Vpz", Check_deviationsinfastening_state[index].Vpz),
                                 new XAttribute("Otst", Check_deviationsinfastening_state[index].Ots),
                                 new XAttribute("thread", Check_deviationsinfastening_state[index].Threat_id),
                                 new XAttribute("fastening", Check_deviationsinfastening_state[index].Fastening),
                                 new XAttribute("amount", Check_deviationsinfastening_state[index].Kol),
                                 new XAttribute("tripplan", Check_deviationsinfastening_state[index].Tripplan),
                                 new XAttribute("template", Check_deviationsinfastening_state[index].Norma),
                                 new XAttribute("speed2", Check_deviationsinfastening_state[index].Vdop),
                                 new XAttribute("fileId", Check_deviationsinfastening_state[index].Fileid),
                                 new XAttribute("Ms", Check_deviationsinfastening_state[index].Ms),
                                 new XAttribute("fNum", Check_deviationsinfastening_state[index].Fnum)
                                 );
                            xeTracks.Add(xeElements);
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
                //htReport.Save(@"\\Desktop-tolegen\sntfi\report_Fastening.html");
                htReport.Save(Path.GetTempPath() + "/report_DeviationsFastening.html");
                htReport.Save($@"G:\form\4.Выходные формы Видеоконтроля ВСП\20.Ведомость ограничений скорости в содержании скреплений.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                //System.Diagnostics.Process.Start(@"http://Desktop-tolegen:5500/report_Fastening.html");
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_DeviationsFastening.html");
            }
        }
    }
}
