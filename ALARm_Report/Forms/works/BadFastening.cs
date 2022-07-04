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
using ALARm.Core.AdditionalParameteres;
using System.Linq;
using System.Globalization;

namespace ALARm_Report.Forms
{
    public class BadFastening : Report
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

                var videoProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
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
                        xePages = new XElement("pages",
                            new XAttribute("road", road),
                            new XAttribute("period", period.Period),
                            new XAttribute("type", videoProcess.GetProcessTypeName),
                            new XAttribute("car",  videoProcess.Car),
                            new XAttribute("chief", videoProcess.Chief),
                            new XAttribute("ps", videoProcess.Car),
                            new XAttribute("data", "" + videoProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                            new XAttribute("info", videoProcess.Car + " " + videoProcess.Chief));

                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", $"{videoProcess.DirectionName}({videoProcess.DirectionCode}) / Путь: {trackName} / ПЧ: { distance.Code }")
                            );

                        List<Digression> Check_badfastening_state = AdditionalParametersService.Check_badfastening_state(videoProcess.Trip_id, template.ID);

                        //Check_badfastening_state = Check_badfastening_state.Where(o => o.Km.Between(710, 720)).ToList();

                        //Check_badfastening_state = Check_badfastening_state.Where(o => o.Km > 128).ToList();
                        for (int index = 0; index < Check_badfastening_state.Count(); index++)
                        {

                            XElement xeElements = new XElement("elements",
                                new XAttribute("n", index + 1),
                                new XAttribute("pchu", Check_badfastening_state[index].Pchu),
                                new XAttribute("station", Check_badfastening_state[index].Station),
                                new XAttribute("km", Check_badfastening_state[index].Km),
                                new XAttribute("piket", Check_badfastening_state[index].Mtr / 100 + 1),
                                new XAttribute("mtr", Check_badfastening_state[index].Mtr),
                                new XAttribute("otst", Check_badfastening_state[index].Otst),
                                new XAttribute("fastening", Check_badfastening_state[index].Fastening),
                                new XAttribute("threat_id", Check_badfastening_state[index].Threat_id),
                                new XAttribute("notice", ""),

                                new XAttribute("CarPosition", (int)videoProcess.Car_Position),
                                new XAttribute("repType", (int)RepType.Fastener),
                                new XAttribute("fileId", Check_badfastening_state[index].Fileid),
                                new XAttribute("Ms", Check_badfastening_state[index].Ms),
                                new XAttribute("fNum", Check_badfastening_state[index].Fnum)
                                );
                            xeTracks.Add(xeElements);
                        }
                        xePages.Add(xeTracks);
                    }
                }
                report.Add(xePages);
                xdReport.Add(report);

                progressBar.Value = 0;

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                //htReport.Save(@"\\DESKTOP-EMAFC5J\sntfi\BadFastening.html");
                htReport.Save(Path.GetTempPath() + "/BadFastening.html");
           //     htReport.Save($@"G:\form\4.Выходные формы Видеоконтроля ВСП\1.Ведомость негодных скреплений.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                //System.Diagnostics.Process.Start(@"http://DESKTOP-EMAFC5J:5500/BadFastening.html");
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/BadFastening.html");
            }
        }



        //private object GetName(DigressionName digName)
        private object GetName(string digName)
        {
            var nameD = "";
            if (digName == DigressionName.D65_NoPad.Name || digName == DigressionName.Missing2OrMoreMainSpikes.Name)
            {
                nameD = "Д65";
            }
            if (digName == DigressionName.KB65_NoPad.Name || digName == DigressionName.KB65_MissingClamp.Name)
            {
                nameD = "КБ65";
            }
            if (digName == DigressionName.SklNoPad.Name || digName == DigressionName.SklBroken.Name)
            {
                nameD = "СКЛ";
            }
            if (digName == DigressionName.GBRNoPad.Name || digName == DigressionName.WW.Name)
            {
                nameD = "ЖБР";
            }
            if (digName == DigressionName.P350MissingClamp.Name)
            {
                nameD = "П-350";
            }
            if (digName == DigressionName.KD65NB.Name)
            {
                nameD = "КД65";
            }
            if (digName == DigressionName.KppNoPad.Name)
            {
                nameD = "КПП";
            }
            return nameD;
        }
    }
}
