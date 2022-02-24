using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm_Report.Forms
{
    public class Comparativestatement1 : Report
    {
        private string engineer { get; set; } = "Komissia K";
        private string chief { get; set; } = "Komissia K";
        private DateTime from, to;
        private TripType tripType;

        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            long firstTripId = -1, secondTripId = -1;
            List<long> firstAdmTracksIDs = new List<long>(), secondAdmTracksIDs = new List<long>(), admTracksIDs = new List<long>();
            using (var compareForm = new CompareTripsForm())
            {
                compareForm.SetTripsDataSource(parentId, period);
                compareForm.ShowDialog();
                if (compareForm.dialogResult != DialogResult.OK)
                {
                    return;
                }

                firstTripId = compareForm.firstTripId;
                secondTripId = compareForm.secondTripId;
                firstAdmTracksIDs.AddRange(compareForm.firstAdmTracksIDs);
                admTracksIDs.AddRange(compareForm.firstAdmTracksIDs);
                secondAdmTracksIDs.AddRange(compareForm.secondAdmTracksIDs);
                foreach (var trackId in secondAdmTracksIDs)
                    if (!admTracksIDs.Contains(trackId))
                        admTracksIDs.Add(trackId);
            }
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            using (var filterForm = new FilterForm())
            {
                var filters = new List<Filter>();
                filters.Add(new StringFilter() { Name = "Начальник путеизмерительного вагона: ", Value = chief });
                filters.Add(new StringFilter() { Name = "Данные обработали и оформили ведомость: ", Value = engineer });
                filters.Add(new DateFilter() { Name = "Дата проверки с:", Value = period.StartDate.ToString("dd.MM.yyyy") });
                filters.Add(new DateFilter() { Name = "                         по:", Value = period.FinishDate.ToString("dd.MM.yyyy") });
                filters.Add(new TripTypeFilter() { Name = "Тип поездки", Value = "рабочая" });

                filterForm.SetDataSource(filters);
                if (filterForm.ShowDialog() == DialogResult.Cancel)
                    return;

                chief = (string)filters[0].Value;
                engineer = (string)filters[1].Value;
                tripType = ((TripTypeFilter)filters[4]).TripType;
                from = DateTime.Parse((string)filters[2].Value);
                to = DateTime.Parse((string)filters[3].Value + " 23:59:59");
            }
            using (var filterForm = new FilterForm())
            {
                var filters2 = new List<Filter>();
                filters2.Add(new StringFilter() { Name = "Начальник путеизмерительного вагона: ", Value = chief });
                filters2.Add(new StringFilter() { Name = "Данные обработали и оформили ведомость: ", Value = engineer });
                filters2.Add(new DateFilter() { Name = "Дата проверки с:", Value = period.StartDate.ToString("dd.MM.yyyy") });
                filters2.Add(new DateFilter() { Name = "                         по:", Value = period.FinishDate.ToString("dd.MM.yyyy") });
                filters2.Add(new TripTypeFilter() { Name = "Тип поездки", Value = "рабочая" });

                filterForm.SetDataSource(filters2);
                if (filterForm.ShowDialog() == DialogResult.Cancel)
                    return;

                chief = (string)filters2[0].Value;
                engineer = (string)filters2[1].Value;
                tripType = ((TripTypeFilter)filters2[4]).TripType;
                from = DateTime.Parse((string)filters2[2].Value);
                to = DateTime.Parse((string)filters2[3].Value + " 23:59:59");
            }
            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;

                XDocument xdReport = new XDocument();

                var kilometers = RdStructureService.GetPU32Kilometers(from, to, parentId, tripType); //.GetRange(65,15);
                if (kilometers.Count == 0)
                    return;

                var trips = RdStructureService.GetTrips();
                var trip = trips.Where(o => o.Id == kilometers[0].Id).ToList();

                XElement report = new XElement("report",
                    new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                    new XAttribute("engineer", engineer),
                    new XAttribute("from", from.ToString("dd.MM.yyyy")),
                    new XAttribute("to", to.ToString("dd.MM.yyyy")),
                    new XAttribute("road", roadName),
                    new XAttribute("distance", distance.Code),
                    new XAttribute("month", period.Period),
                    new XAttribute("triptype", tripType == TripType.Control ? "контрольная" : tripType == TripType.Work ? "рабочая" : "дополнительная"),
                    new XAttribute("soft", "ALARm 1.0(436-р)"),
                    new XAttribute("tripdate", kilometers[0].TripDate.ToString("dd.MM.yyyy_hh:mm")),
                    new XAttribute("car", trip.Any() ? trip.First().Car.ToString() : "нет данных"),
                    new XAttribute("chief", chief)
                );

                var DevDegree2 = 0;



                var byKilometer = new XElement("bykilometer",
                                    new XAttribute("code", kilometers[0].Direction_code),
                                    new XAttribute("track", kilometers[0].Track_name),
                                    new XAttribute("name", kilometers[0].Direction_name),
                                    new XAttribute("pch", distance.Code));

                var distanceTotal = new Total
                {
                    Code = distance.Code
                };
                var sectionTotal = new Total
                {
                    Code = kilometers[0].Track_name,
                    DirectionCode = kilometers[0].Direction_code,
                    DirectionName = kilometers[0].Direction_name
                };
                //запрос доп параметров с бд
                var AddParam = AdditionalParametersService.GetAddParam(kilometers.First().Id) as List<S3>; //износы
                if (AddParam == null)
                {
                    MessageBox.Show("Не удалось сформировать отчет, так как возникала ошибка во время загрузки данных по доп параметрам");
                    return;
                }
                List<Gap> check_gap_state = AdditionalParametersService.Check_gap_state(kilometers.First().Id, template.ID); //стыки
                var ListS3 = RdStructureService.GetS3(kilometers.First().Id) as List<S3>; //пру


                var PRU = ListS3.Where(o => o.Ots == "ПрУ").ToList();
                var gapV = check_gap_state.Where(o => o.Vdop != "" && o.Vdop != "-/-").ToList();

                var Pu32_gap = check_gap_state.Where(o => o.Zazor >= 31 || o.R_zazor >= 31).ToList();




                var dopBall = PRU.Count() * 50 + (gapV.Count() > 0 ? 50 : 0) + (AddParam.Count() > 0 ? 50 : 0);
                var DopCount = gapV.Count() + AddParam.Count();

                var sectionElement = new XElement("section",
                    new XAttribute("name", kilometers[0].Direction_name),
                    new XAttribute("code", kilometers[0].Direction_code),
                    new XAttribute("track", kilometers[0].Track_name)
                    );
                var pchuElement = new XElement("pchu",
                    new XAttribute("pch", distance.Code),
                    new XAttribute("code", kilometers[0].PchuCode),
                    new XAttribute("chief", kilometers[0].PchuChief)
                    );
                var pchuTotal = new Total();
                pchuTotal.Code = kilometers[0].PchuCode;

                var pdElement = new XElement("pd",
                    new XAttribute("code", kilometers[0].PdCode),
                    new XAttribute("chief", kilometers[0].PdChief)
                    );
                var pdTotal = new Total();
                pdTotal.Code = kilometers[0].PdCode;

                var pdbElement = new XElement("pdb",
                    new XAttribute("code", kilometers[0].PdbCode),
                    new XAttribute("chief", kilometers[0].PdbChief));
                var pdbTotal = new Total();
                pdbTotal.Code = kilometers[0].PdbCode;

                int Grk = 0, Sochet = 0, Kriv = 0, Pru = 0, Oshk = 0, Iznos = 0, Zazor = 0, NerProf = 0, KmSpeedLimit = 0;

                progressBar.Maximum = kilometers.Count;
            }
        }
    }
}

			