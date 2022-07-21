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
    public class Listofdigretionrailjoints : Report
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

                var kilometers = RdStructureService.GetPU32Kilometers(from, to, distanceId, tripType); //.GetRange(65,15);
                if (kilometers.Count == 0)
                {
                    MessageBox.Show("Нет отчетных данных по выбранным параметрам");
                    return;
                }
                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;

                foreach (var mainProcess in mainProcesses)
                {


                    if (!mainProcess.GetProcessTypeName.Equals(kilometers[0].Trip.GetProcessTypeName))
                    {
                        continue;
                    }
                    else
                    {
                        //var trackName = AdmStructureService.GetTrackName(track_id);
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
                            new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + "), Путь: " +  kilometers[0].Track_name  + ", ПЧ: " + distance.Code));

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

