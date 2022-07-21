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
using System.Globalization;
using ALARm_Report.controls;
using System.Collections.Generic;

namespace ALARm_Report.Forms
{
    public class AbsenceDefectOverlay : Report
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
            int index = 1;

            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");
                XElement xePages = new XElement("pages");

                var distance =AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);

                var mainProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                //var kilometers = RdStructureService.GetPU32Kilometers(from, to, distanceId, tripType); //.GetRange(65,15);
                //if (kilometers.Count == 0)
                //{
                //    MessageBox.Show("Нет отчетных данных по выбранным параметрам");
                //    return;
                //}
                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;

                foreach (var mainProcess in mainProcesses)
                {
                    var digressions = RdStructureService.GetDigressions(mainProcess, new int[] { 1011, 1012, 1013 });
                    //if (digressions.Count < 1)
                    //{
                    //    continue;
                    //}

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
                                new XAttribute("fastening", finddeg.BraceType),
                                new XAttribute("speed2", finddeg.AllowSpeed),
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
                            new XAttribute("road", road),
                            new XAttribute("period", period.Period),
                            new XAttribute("type", mainProcess.GetProcessTypeName),
                            new XAttribute("car", mainProcess.Car),
                            new XAttribute("chief", mainProcess.Chief),
                            new XAttribute("ps", mainProcess.Car),
                           new XAttribute("data", "" + mainProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                            new XAttribute("info", "ПС: " + mainProcess.Car + " " + mainProcess.Chief));

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
                                new XAttribute("fastening", finddeg.BraceType),
                                new XAttribute("speed2", finddeg.AllowSpeed),
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
                htReport.Save(Path.GetTempPath() + "/report_AbsenceDefectOverlay.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_AbsenceDefectOverlay.html");
            }
        }
    }
}
