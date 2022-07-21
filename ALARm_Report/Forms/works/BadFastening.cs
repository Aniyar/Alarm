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
        private string engineer { get; set; } = "Komissia K";
        private string chief { get; set; } = "Komissia K";
        private DateTime from, to;
        private TripType tripType, comparativeTripType;
        private PU32Type reportType;
        private ReportPeriod comparativePeriod;
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            using (var filterForm = new FilterForm())
            {
                filterForm.ReportPeriod = period;
                var filters = new List<Filter>();
                filters.Add(new StringFilter() { Name = "Начальник путеизмерительного вагона: ", Value = chief });
                filters.Add(new StringFilter() { Name = "Данные обработали и оформили ведомость: ", Value = engineer });
                filters.Add(new TripTypeFilter() { Name = "Тип поездки", Value = "рабочая" });
                filters.Add(new PU32TypeFilter { Name = "Тип отчета", Value = "Оценка состояния пути по ПЧ" });



                filterForm.SetDataSource(filters);
                filterForm.ReportClasssName = "PU32";

                if (filterForm.ShowDialog() == DialogResult.Cancel)
                    return;

                chief = (string)filters[0].Value;
                engineer = (string)filters[1].Value;
                tripType = ((TripTypeFilter)filters[2]).TripType;
                reportType = ((PU32TypeFilter)filters[3]).PU32Type;
                if (reportType == PU32Type.Comparative)
                {
                    from = period.StartDate;
                    to = period.FinishDate;
                    comparativePeriod = ((PeriodFilter)filters[4]).PeriodValue;
                    comparativeTripType = ((TripTypeFilter)filters[5]).TripType;

                }
                else
                {
                    from = DateTime.Parse((string)filters[4].Value);
                    to = DateTime.Parse((string)filters[5].Value + " 23:59:59");
                }
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
                var kilometers = RdStructureService.GetPU32Kilometers(from, to, distanceId, tripType); //.GetRange(65,15);
                if (kilometers.Count == 0)
                {
                    MessageBox.Show("Нет отчетных данных по выбранным параметрам");
                    return;
                }
               

                foreach (var videoProcess in videoProcesses)
                {
                    progressBar.Value = videoProcesses.IndexOf(videoProcess) + 1;
                    bool founddigression = false;
                 
                    if (!videoProcess.GetProcessTypeName.Equals(kilometers[0].Trip.GetProcessTypeName))
                    {
                        continue;
                    }
                    else
                    {

                        var trackName = 2231231;
                        //var trackName = AdmStructureService.GetTrackName(track_id);
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
                            new XAttribute("trackinfo", $"{videoProcess.DirectionName}({videoProcess.DirectionCode}) / Путь: {kilometers[0].Track_name} / ПЧ: { distance.Code }")
                            );

                        List<Digression> Check_badfastening_state = AdditionalParametersService.Check_badfastening_state(kilometers[0].Trip.Id, template.ID);
                        Check_badfastening_state = Check_badfastening_state.Where(o => o.Trip_id == videoProcess.Trip_id).ToList();
                        if (Check_badfastening_state != null && Check_badfastening_state.Count > 0)
                        {
                            founddigression = true;
                        }
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
                    if (founddigression == true)
                    {
                        report.Add(xePages);
                    }

                }
              

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
