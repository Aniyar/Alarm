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
///надо доделать сервис
{
    public class DeviationsInSleepers : Report
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
           // int index = 1;

            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");
                XElement xePages = new XElement("pages");

                var road = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;

                var mainProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;

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
                                new XAttribute("chief", mainProcess.Chief),
                                new XAttribute("ps", mainProcess.Car),
                                new XAttribute("data", mainProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                                new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief));

                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + "), Путь: " + trackName + ", ПЧ: " + distance.Code));

                        List<Digression> Check_sleepers_state = AdditionalParametersService.Check_sleepers_state(mainProcess.Trip_id, template.ID);

                        Check_sleepers_state = Check_sleepers_state.ToList();

                        Check_sleepers_state = Check_sleepers_state.OrderBy(o => o.Km + o.Meter / 10000.0).ToList();

                        int i = 1;
                        for (int index = 0; index < Check_sleepers_state.Count(); index++)
                        {
                            if (Check_sleepers_state[index].Vdop == "") continue;

                            XElement xeElements = new XElement("elements",
                                new XAttribute("n", i),
                                new XAttribute("pchu", Check_sleepers_state[index].Pchu),
                                new XAttribute("station", Check_sleepers_state[index].Station),
                                new XAttribute("km", Check_sleepers_state[index].Km),
                                new XAttribute("piket", Check_sleepers_state[index].Meter / 100 + 1),
                                new XAttribute("meter", Check_sleepers_state[index].Meter),
                                new XAttribute("speed", Check_sleepers_state[index].Vpz),
                                new XAttribute("trackclass", Check_sleepers_state[index].TrackClass),
                                new XAttribute("digression", Check_sleepers_state[index].Ots),
                                new XAttribute("fastening", Check_sleepers_state[index].Fastening),
                                new XAttribute("size", Check_sleepers_state[index].Kol ),
                                new XAttribute("railtype", Check_sleepers_state[index].RailType),
                                new XAttribute("trackplan", Check_sleepers_state[index].Tripplan),
                                new XAttribute("template", Check_sleepers_state[index].Norma),
                                new XAttribute("addspeed", Check_sleepers_state[index].Vdop),

                                new XAttribute("fileId", Check_sleepers_state[index].Fileid),
                                new XAttribute("Ms", Check_sleepers_state[index].Ms),
                                new XAttribute("fNum", Check_sleepers_state[index].Fnum)
                                );
                            xeTracks.Add(xeElements);
                            i++;
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
                htReport.Save(Path.GetTempPath() + "/report_DeviationsInSleepers.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_DeviationsInSleepers.html");
            }
        }
    }
}

