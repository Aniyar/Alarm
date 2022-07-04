using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm_Report.Forms
{
    public class GDSummaryOfAditionalParameters_new2 : ALARm.Core.Report.GraphicDiagrams
    {
        private float NerovPosition = 30;
        private float NerovKoef = 5.0f / 50.0f;

        private float GapLPosition = 38;
        private float GapRPosition = 56;
        private float GapKoef = 13.7f / 35.0f;

        private float IznosPosition = 73;
        private float IznosKoef = 13f / 20.0f;

        private float OtzhatPosition = 101;
        private float OtzhatKoef = 3.5f / 5;

        private float GaugePosition = 114;
        private float GaugeKoef = 10.0f / 20;

        private float PlanPosition = 137;
        private float PlanKoef = 10 / 30.0f;


        private readonly float LevelPosition = 32.5f;
        private float angleRuleWidth = 9.7f;

        private float GetDIstanceFrom1div60(float x)
        {
            var koef = (angleRuleWidth / (1 / 12f - 1 / 60f));
            var value = 1 / x - 1 / 60f;

            return koef * value;
        }

        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {

            this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();
            this.RdStructureRepository = RdStructureService.GetRepository();
            this.AdmStructureRepository = AdmStructureService.GetRepository();

            //Сделать выбор периода
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(parentId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel)
                    return;
                admTracksId = choiceForm.admTracksIDs;
            }


            diagramName = "Доппараметры";
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;

                var tripProcesses = RdStructureService.GetTripsOnDistance(parentId, period);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                int svgIndex = template.Xsl.IndexOf("</svg>");
                template.Xsl = template.Xsl.Insert(svgIndex, RighstSideXslt());
                var svgLength = 0;

                foreach (var trip in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        trip.Track_Id = track_id;
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);

                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();


                        if (kilometers.Count == 0)
                            continue;

                        ////Выбор километров по проезду -----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var lkm = kilometers.Select(o => o.Number).ToList();

                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                        //filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        //filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = 145 });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = 145 });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        kilometers = kilometers.Where(Km => ((float)(float)filters[0].Value <= Km.Number && Km.Number <= (float)(float)filters[1].Value)).ToList();
                        kilometers = (trip.Travel_Direction == Direction.Reverse ? kilometers.OrderBy(o => o.Number) : kilometers.OrderByDescending(o => o.Number)).ToList();
                        //--------------------------------------------

                        progressBar.Maximum = kilometers.Count;

                        foreach (var kilometer in kilometers)
                        {
                            kilometer.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);
                            if (kilometer.NonstandardKms.Any())
                            {
                                kilometer.Final_m = kilometer.NonstandardKms.First().Len;
                            }
                            //данные
                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(kilometer.Number, trip.Id);
                            if (DBcrossRailProfile == null) continue;

                            var sortedData = DBcrossRailProfile.OrderByDescending(d => d.Meter).ToList();
                            var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(sortedData);

                            //Шаблон
                            var DB_gauge = AdditionalParametersService.GetGaugeFromDB(kilometer.Number, trip.Id);

                            XElement kmlist = new XElement("kmlist");

                            progressBar.Value = kilometers.IndexOf(kilometer) + 1;

                            var outData = (List<OutData>)RdStructureService.GetNextOutDatas(kilometer.Start_Index - 1, kilometer.GetLength() - 1, kilometer.Trip.Id);
                            kilometer.AddDataRange(outData, kilometer);

                            //lvl avg data
                            var Curves = new List<NatureCurves> { };
                            var StrightAvgTrapezoid = kilometer.StrightAvg.GetTrapezoid(new List<double> { }, new List<double> { }, 4, ref Curves);
                            var LevelAvgTrapezoid = kilometer.LevelAvg.GetTrapezoid(new List<double> { }, new List<double> { }, 10, ref Curves);
                            LevelAvgTrapezoid.Add(LevelAvgTrapezoid[LevelAvgTrapezoid.Count - 1]);
                            StrightAvgTrapezoid.Add(StrightAvgTrapezoid[StrightAvgTrapezoid.Count - 1]);

                            //zero line data
                            kilometer.GetZeroLines(outData, trip, MainTrackStructureService.GetRepository());

                            var sector_station = MainTrackStructureService.GetSector(track_id, kilometer.Number, trip.Trip_date);
                            var fragment = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, kilometer.Number, MainTrackStructureConst.Fragments, kilometer.Direction_name, $"{trackName}") as Fragment;
                            var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, kilometer.Number, MainTrackStructureConst.MtoPdbSection, kilometer.Direction_name, $"{trackName}") as List<PdbSection>;

                            //Линий уровня по середине ------------------------------------
                            List<string> result = new List<string>();
                            string drawdownRight = string.Empty, Plan = string.Empty, gauge = string.Empty, zeroGauge = string.Empty,
                                    zeroStraighteningRight = string.Empty, averageStraighteningRight = string.Empty, straighteningRight = string.Empty,
                                    zeroStraighteningLeft = string.Empty, averageStraighteningLeft = string.Empty, straighteningLeft = string.Empty,
                                    level = string.Empty, averageLevel = string.Empty, zeroLevel = string.Empty;

                            int fourStepOgrCoun = 0, otherfourStepOgrCoun = 0;


                            svgLength = kilometer.GetLength() < 1000 ? 1000 : kilometer.GetLength();
                            var xp = (-kilometer.Start_m - svgLength - 50) + (svgLength + 105) - 52;
                            var direction = AdmStructureRepository.GetDirectionByTrack(kilometer.Track_id);
                            var trackclasses = (List<TrackClass>)MainTrackStructureRepository.GetMtoObjectsByCoord(trip.Trip_date, kilometer.Number, MainTrackStructureConst.MtoTrackClass, kilometer.Track_id);

                            XElement addParam = new XElement("addparam",
                                new XAttribute("top-title",
                                    (direction != null ? $"{direction.Name} ({direction.Code} )" : "Неизвестный") + " Путь: " + kilometer.Track_name +
                                    $" Класс: {(trackclasses.Any() ? trackclasses.First().Class_Id.ToString() : "-")} Км:" + kilometer.Number + " " +
                                    (kilometer.PdbSection.Count > 0 ? kilometer.PdbSection[0].ToString() : " ПЧ-/ПЧУ-/ПД-/ПДБ-") + " Уст: " + " " +
                                    (kilometer.Speeds.Count > 0 ? $"{kilometer.Speeds.First().Passenger}/{kilometer.Speeds.First().Freight}" : "-/-")),

                                new XAttribute("right-title",
                                    copyright + ": " + "ПО " + softVersion + "  " +
                                    systemName + ":" + trip.Car + "(" + trip.Chief.Trim() + ") (БПД от " + MainTrackStructureRepository.GetModificationDate() + ") <" +
                                    (kilometer.PdbSection.Count > 0 ? kilometer.PdbSection[0].RoadAbbr : "НЕИЗВ") + ">" + "<" + kilometer.Passage_time.ToString("dd.MM.yyyy  HH:mm") + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(trip.Travel_Direction.ToString())) + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(trip.Car_Position.ToString())) + ">" +
                                    "<" + trip.Trip_date.Month + "-" + trip.Trip_date.Year + " " + (trip.Trip_Type == TripType.Control ? "контр." : trip.Trip_Type == TripType.Work ? "раб." : "доп.") +
                                    " Проезд:" + trip.Trip_date.ToString("dd.MM.yyyy  HH:mm") + " " + diagramName + ">"),

                                new XAttribute("pre", xp + 30),
                                new XAttribute("prer", xp + 21),
                                new XAttribute("topr", -kilometer.Start_m - svgLength - 45),
                                new XAttribute("topf", xp + 10),
                                new XAttribute("topx", -kilometer.Start_m - svgLength),
                                new XAttribute("topx1", -kilometer.Start_m - svgLength - 30),
                                new XAttribute("topx2", -kilometer.Start_m - svgLength - 15),
                                new XAttribute("RailType", "1    🠕Тип рельсов: " + (kilometer.RailSection.Any() ? kilometer.RailSection.First().Name : "")),
                                new XAttribute("fragment", (kilometer.StationSection != null && kilometer.StationSection.Count > 0 ?
                                    "Станция: " + kilometer.StationSection[0].Station : (kilometer.Sector != null ? kilometer.Sector.ToString() : "")) + " Км:" + kilometer.Number),
                                new XAttribute("viewbox", $"-20 {-kilometer.Start_m - svgLength - 50} 830 {svgLength + 105}"),
                                new XAttribute("minY", -kilometer.Start_m),
                                new XAttribute("maxY", -kilometer.Final_m),
                                new XAttribute("minYround", -(kilometer.Start_m - kilometer.Start_m % 100)),

                                RightSideChart(trip.Trip_date, kilometer, kilometer.Track_id, new float[] { 151f, 146f, 152.5f, 155f }),

                                new XElement("xgrid",
                                    new XElement("x", MMToPixelChartString(NerovPosition - NerovKoef * 50f), new XAttribute("text", -50)),
                                    new XElement("L", MMToPixelChartString(NerovPosition), new XAttribute("text", 0)), // неров профил
                                    new XElement("x", MMToPixelChartString(NerovPosition + NerovKoef * 50f), new XAttribute("text", 50)),

                                    new XElement("L", MMToPixelChartString(GapLPosition), new XAttribute("text", 0)), // зазор лев
                                    new XElement("x", MMToPixelChartString(GapLPosition + GapKoef * 25), new XAttribute("text", 25)),
                                    new XElement("x", MMToPixelChartString(GapLPosition + GapKoef * 30), new XAttribute("text", 30)),
                                    new XElement("x", MMToPixelChartString(GapLPosition + GapKoef * 35), new XAttribute("text", 35)),

                                    new XElement("L", MMToPixelChartString(GapRPosition), new XAttribute("text", 0)), // зазор прав
                                    new XElement("x", MMToPixelChartString(GapRPosition + GapKoef * 25), new XAttribute("text", 25)),
                                    new XElement("x", MMToPixelChartString(GapRPosition + GapKoef * 30), new XAttribute("text", 30)),
                                    new XElement("x", MMToPixelChartString(GapRPosition + GapKoef * 35), new XAttribute("text", 35)),

                                    new XElement("L", MMToPixelChartString(IznosPosition), new XAttribute("text", 0)), // Износ
                                    new XElement("x", MMToPixelChartString(IznosPosition + IznosKoef * 15), new XAttribute("text", 15)),
                                    new XElement("x", MMToPixelChartString(IznosPosition + IznosKoef * 20), new XAttribute("text", 20)),

                                    new XElement("x", MMToPixelChartString(OtzhatPosition - OtzhatKoef * 5), new XAttribute("text", -5)),
                                    new XElement("L", MMToPixelChartString(OtzhatPosition), new XAttribute("text", 0)), // Отжание
                                    new XElement("x", MMToPixelChartString(OtzhatPosition + OtzhatKoef * 5), new XAttribute("text", 5)),

                                    new XElement("L", MMToPixelChartString(GaugePosition), new XAttribute("text", 1520)), // Шаблон
                                    new XElement("x", MMToPixelChartString(GaugePosition + GaugeKoef * 10), new XAttribute("text", 1530)),
                                    new XElement("x", MMToPixelChartString(GaugePosition + GaugeKoef * 20), new XAttribute("text", 1540)),

                                    new XElement("L", MMToPixelChartString(PlanPosition), new XAttribute("text", 0)) // План

                                //new XAttribute("x0", MMToPixelChart(gaspLeftPosition)),
                                //new XAttribute("x1", MMToPixelChart(gaspRightPosition)),
                                //new XAttribute("x2", MMToPixelChart(releasePosition)),
                                //new XAttribute("x3", MMToPixelChart(gaugePosition + gaugeKoef * 20)),
                                ////new XAttribute("x31", MMToPixelChart(iznosLeftPosition)),
                                //new XAttribute("x32", MMToPixelChart(iznosRightPosition)),

                                //new XElement("x", MMToPixelChartString(gaspLeftPosition + gapsKoef * 25f)),
                                //new XElement("x", MMToPixelChartString(gaspLeftPosition + gapsKoef * 27.5f)),
                                //new XElement("x", MMToPixelChartString(gaspLeftPosition + gapsKoef * 30f)),

                                //new XElement("x", MMToPixelChartString(gaspRightPosition + gapsKoef * 25f)),
                                //new XElement("x", MMToPixelChartString(gaspRightPosition + gapsKoef * 27.5f)),
                                //new XElement("x", MMToPixelChartString(gaspRightPosition + gapsKoef * 30f)),

                                ////new XElement("x", MMToPixelChartString(iznosLeftPosition + iznosKoef * 8f)),
                                //new XElement("x", MMToPixelChartString(iznosLeftPosition + iznosKoef * 13f)),
                                //new XElement("x", MMToPixelChartString(iznosLeftPosition + iznosKoef * 14f)),
                                //new XElement("x", MMToPixelChartString(iznosLeftPosition + iznosKoef * 20f)),

                                //new XElement("x", MMToPixelChartString(iznosRightPosition + iznosKoef * 8f)),
                                //new XElement("x", MMToPixelChartString(iznosRightPosition + iznosKoef * 13f)),
                                //new XElement("x", MMToPixelChartString(iznosRightPosition + iznosKoef * 15f)),
                                //new XElement("x", MMToPixelChartString(iznosRightPosition + iznosKoef * 20f)),

                                ////НПК лев
                                //new XElement("x", MMToPixelChartString(NPKLeftPosition)),
                                //new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(60))),
                                //new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(44))),
                                //new XElement("x20", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(20))),
                                //new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(18))),
                                //new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(16))),
                                //new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(12))),
                                ////НПК прав.
                                //new XElement("x", MMToPixelChartString(NPKRightPosition)),
                                //new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(60))),
                                //new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(44))),
                                //new XElement("x20", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(20))),
                                //new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(18))),
                                //new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(16))),
                                //new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(12))),
                                ////ПУ лев.
                                //new XElement("x", MMToPixelChartString(PULeftPosition)),
                                //new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(30))),
                                //new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(22))),
                                //new XElement("x20", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(20))),
                                //new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(18))),
                                //new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(16))),
                                //new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(12))),
                                ////ПУ прав.
                                //new XElement("x", MMToPixelChartString(PURightPosition)),
                                //new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(30))),
                                //new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(22))),
                                //new XElement("x20", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(20))),
                                //new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(18))),
                                //new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(16))),
                                //new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(12))),

                                //new XElement("x", MMToPixelChartString(gaugePosition + gaugeKoef * 10)),
                                //new XElement("x", MMToPixelChartString(gaugePosition + gaugeKoef * 12)),
                                //new XElement("x", MMToPixelChartString(gaugePosition + gaugeKoef * 16)),
                                //new XElement("x", MMToPixelChartString(gaugePosition + gaugeKoef * 24)),
                                //new XElement("x", MMToPixelChartString(gaugePosition + gaugeKoef * 28)),
                                //new XElement("x", MMToPixelChartString(gaugePosition + gaugeKoef * 36))
                                ));

                            string downhillLeft = string.Empty;
                            string downhillRight = string.Empty;
                            string treadTiltLeft = string.Empty;
                            string treadTiltRight = string.Empty;
                            string sideWearLeft = string.Empty;
                            string sideWearRight = string.Empty;

                            string sideWear = string.Empty;

                            gauge = string.Empty;
                            string otzh = string.Empty;


                            var side_bok = false;

                            if (crossRailProfile.SideWearLeft.Any())
                            {
                                if (crossRailProfile.SideWearLeft.Average() > crossRailProfile.SideWearRight.Average())
                                {
                                    side_bok = true;
                                }
                            }

                            for (int i = 0; i < kilometer.meter.Count - 1; i++)
                            {
                                int metre = -kilometer.meter[i];

                                var st = crossRailProfile.Meters.Where(o => o == i).ToList();
                                if (st.Any())
                                {
                                    var index = crossRailProfile.Meters.IndexOf(i);

                                    if (side_bok)
                                        sideWear += MMToPixelChartString(IznosPosition + IznosKoef * crossRailProfile.SideWearLeft[index]).Replace(",", ".") + "," + metre + " ";
                                    else
                                        sideWear += MMToPixelChartString(IznosPosition + IznosKoef * crossRailProfile.SideWearRight[index]).Replace(",", ".") + "," + metre + " ";

                                    ////нпк
                                    //treadTiltLeft += MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(1 / crossRailProfile.TreadTiltLeft[index])) + "," + metre + " ";
                                    //treadTiltRight += MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(1 / crossRailProfile.TreadTiltRight[index])) + "," + metre + " ";
                                    ////пу
                                    //downhillLeft += MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(1 / crossRailProfile.DownhillLeft[index])) + "," + metre + " ";
                                    //downhillRight += MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(1 / crossRailProfile.DownhillRight[index])) + "," + metre + " ";


                                }
                                else
                                {
                                    sideWear += MMToPixelChartString(IznosPosition).Replace(",", ".") + "," + metre + " ";
                                }

                                if (i < DB_gauge.Count - 1)
                                {
                                    var index = i;
                                    Plan += MMToPixelChartString(PlanPosition + PlanKoef * DB_gauge[index].Stright_left / 5.0).Replace(",", ".") + "," + metre + " ";
                                    gauge += MMToPixelChartString(GaugePosition + GaugeKoef * (DB_gauge[index].Gauge - 1520)).Replace(",", ".") + "," + metre + " ";
                                    otzh += MMToPixelChartString(OtzhatPosition + OtzhatKoef * (DB_gauge[index].Gauge - DB_gauge[index + 1].Gauge)).Replace(",", ".") + "," + metre + " ";
                                }
                                else
                                {
                                    Plan += MMToPixelChartString(PlanPosition).Replace(",", ".") + "," + metre + " ";
                                    gauge += MMToPixelChartString(GaugePosition).Replace(",", ".") + "," + metre + " ";
                                    otzh += MMToPixelChartString(OtzhatPosition).Replace(",", ".") + "," + metre + " ";
                                }
                            }
                            addParam.Add(new XElement("polyline", new XAttribute("points", sideWear)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", Plan)));

                            //addParam.Add(new XElement("polyline", new XAttribute("points", treadTiltLeft)));
                            //addParam.Add(new XElement("polyline", new XAttribute("points", treadTiltRight)));

                            //addParam.Add(new XElement("polyline", new XAttribute("points", downhillLeft)));
                            //addParam.Add(new XElement("polyline", new XAttribute("points", downhillRight)));

                            addParam.Add(new XElement("polyline", new XAttribute("points", gauge)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", otzh)));


                            char separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];

                            foreach (var picket in kilometer.Pickets)
                            {
                                picket.Digression = picket.Digression.Where(d => d.Digression == DigressionName.Undefined).ToList();
                            }

                            List<Digression> addDigressions = crossRailProfile.GetDigressions();




                            var gapElements = new XElement("gaps");
                            var gaps = AdditionalParametersService.Check_gap_state(trip.Id, template.ID).Where(i => i.Km == kilometer.Number).ToList();

                            //рассчет баллов стыка
                            var ballGap = 0;
                            var SpeedLimitGap = 0;
                            var speedGapPass = -1;
                            var speedGapFrei = -1;

                            var tempG = gaps.Where(o => o.Otst == "З" || o.Otst == "З?").ToList();
                            if (tempG.Any())
                            {
                                if (tempG.Where(o => o.Otst == "З").Count() > 0)
                                {
                                    ballGap = 50;
                                    SpeedLimitGap = tempG.Where(o => o.Otst == "З").Count();
                                }
                                else if (tempG.Where(o => o.Otst == "З?").Count() > 0)
                                {
                                    ballGap = 20;
                                }
                            }

                            var speed = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, kilometer.Number, MainTrackStructureConst.MtoSpeed, direction.Name, $"{trackName}") as List<Speed>;

                            var dignatur = new List<DigressionMark> { };

                            var prev_r = 0;
                            var prev_l = 0;
                            foreach (var gap in gaps)
                            {
                                //gap.Zazor = gap.Zazor + 10;
                                //gap.R_zazor = gap.R_zazor + 10;

                                if (Math.Abs(gap.Meter - prev_l) > 10)
                                {
                                    //if (gap.Zazor > 35) continue;

                                    if (gap.Zazor == 0)
                                    {
                                        gapElements.Add(new XElement("zero",
                                        new XAttribute("x", MMToPixelChartString(GapLPosition)),
                                        new XAttribute("y", -gap.Meter),
                                        new XAttribute("w", MMToPixelChartWidthString(GapKoef * gap.Zazor))));
                                    }
                                    else if (gap.Zazor != -999)
                                    {
                                        gapElements.Add(new XElement("g",
                                        new XAttribute("x", MMToPixelChartString(GapLPosition)),
                                        new XAttribute("y", -gap.Meter),
                                        new XAttribute("w", MMToPixelChartWidthString(GapKoef * gap.Zazor))));
                                    }
                                    prev_l = gap.Meter;
                                }

                                if (Math.Abs(gap.Meter - prev_r) > 10)
                                {
                                    //if (gap.R_zazor > 35) continue;

                                    if (gap.R_zazor == 0)
                                    {
                                        gapElements.Add(new XElement("zero",
                                        new XAttribute("x", MMToPixelChartString(GapRPosition)),
                                        new XAttribute("y", -gap.Meter),
                                        new XAttribute("w", MMToPixelChartWidthString(GapKoef * gap.R_zazor))));
                                    }
                                    else if (gap.R_zazor != -999)
                                    {
                                        gapElements.Add(new XElement("g",
                                        new XAttribute("x", MMToPixelChartString(GapRPosition)),
                                        new XAttribute("y", -gap.Meter),
                                        new XAttribute("w", MMToPixelChartWidthString(GapKoef * gap.R_zazor))));
                                    }
                                    prev_r = gap.Meter;
                                }

                                var PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                                var FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;

                                var digression = new Digression { };

                                digression.Meter = gap.Meter;

                                if (gap.Otst == "З")
                                {
                                    digression.PassengerSpeedLimit = int.Parse(gap.Vdop.Split('/')[0]);
                                    digression.FreightSpeedLimit = int.Parse(gap.Vdop.Split('/')[1]);

                                    speedGapPass = speedGapPass == -1 ? int.Parse(gap.Vdop.Split('/')[0]) :
                                                              speedGapPass > int.Parse(gap.Vdop.Split('/')[0]) ? int.Parse(gap.Vdop.Split('/')[0]) : speedGapPass;
                                    speedGapFrei = speedGapFrei == -1 ? int.Parse(gap.Vdop.Split('/')[1]) :
                                                             speedGapFrei > int.Parse(gap.Vdop.Split('/')[1]) ? int.Parse(gap.Vdop.Split('/')[1]) : speedGapFrei;
                                }
                                else
                                {
                                    digression.PassengerSpeedLimit = -1;
                                    digression.FreightSpeedLimit = -1;
                                }
                                digression.AllowSpeed = gap.Vdop;
                                digression.Velich = Math.Max(gap.Zazor, gap.R_zazor);

                                digression.DigName = gap.Otst == "З" ? DigressionName.Gap : gap.Otst == "З?" ? DigressionName.GapSimbol : DigressionName.Undefined;

                                addDigressions.Add(digression);
                            }
                            addParam.Add(gapElements);



                            foreach (var dig in addDigressions)
                            {
                                if (dig.DigName == DigressionName.ReducedWearLeft) continue;
                                if (dig.DigName == DigressionName.ReducedWearRight) continue;
                                if (dig.DigName == DigressionName.VertIznosR) continue;
                                if (dig.DigName == DigressionName.VertIznosL) continue;

                                if (dig.Length < 1)
                                {
                                    if (dig.DigName != DigressionName.FusingGap)
                                    {
                                        if (dig.DigName != DigressionName.AnomalisticGap)
                                        {
                                            if (dig.DigName != DigressionName.Gap)
                                            {
                                                if (dig.DigName != DigressionName.GapSimbol)
                                                    continue;
                                            }
                                        }
                                    }
                                }

                                if (dig.DigName == DigressionName.FusingGap ||
                                    dig.DigName == DigressionName.GapSimbol ||
                                    dig.DigName == DigressionName.Gap)
                                {

                                    dignatur.Add(
                                        new DigressionMark()
                                        {
                                            Digression = dig.DigName,
                                            NotMoveAlert = false,
                                            Meter = dig.Meter,
                                            finish_meter = 0,
                                            Length = 0,
                                            Value = dig.Velich,
                                            Count = 0,
                                            DigName = dig.GetName(),
                                            PassengerSpeedLimit = dig.PassengerSpeedLimit,
                                            FreightSpeedLimit = dig.FreightSpeedLimit,
                                            Comment = "",
                                            AllowSpeed = dig.AllowSpeed
                                        });

                                    //var picket = kilometer.Pickets.GetPicket(dig.Meter);
                                    //if (picket != null)
                                    //{
                                    //    picket.Digression.Add(Digressions.First());
                                    //}
                                }
                                else
                                {
                                    int count = dig.Length / 4;
                                    count += dig.Length % 4 > 0 ? 1 : 0;

                                    if (dig.Length < 4 && (int)dig.Typ == 3) continue;

                                    if ((new[] { DigressionName.VertIznosL, DigressionName.VertIznosR, }).Contains(dig.DigName))
                                    {
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    dignatur.Add(
                                        new DigressionMark()
                                        {
                                            Digression = dig.DigName,
                                            NotMoveAlert = false,
                                            Meter = dig.Kmetr,
                                            finish_meter = dig.Kmetr + dig.Length,
                                            Degree = (int)dig.Typ,
                                            Length = dig.Length,
                                            Value = dig.Value,
                                            Count = count,
                                            DigName = dig.GetName(),
                                            PassengerSpeedLimit = -1,
                                            FreightSpeedLimit = -1,
                                            Comment = ""
                                        });

                                    //var picket = kilometer.Pickets.GetPicket(dig.Meter);
                                    //if (picket != null)
                                    //{
                                    //    picket.Digression.Add(Digressions.First());

                                    //}
                                }
                            }


                            kilometer.Digressions.AddRange(dignatur);
                            kilometer.LoadDigresions(RdStructureRepository, MainTrackStructureRepository, trip, AdditionalParam: true);


                            var digElemenets = new XElement("digressions");
                            List<int> usedTops = new List<int>();
                            List<int> speedmetres = new List<int>();

                            foreach (var picket in kilometer.Pickets)
                            {
                                picket.Digression = picket.Digression.OrderBy(o => o.Meter).ToList();

                                picket.WriteNotesToReport(
                                    kilometer,
                                    speedmetres,
                                    addParam,
                                    digElemenets,
                                    2,
                                    2,
                                    2,
                                    1,
                                    GaugePosition,
                                    LevelPosition,
                                    this,
                                    ref fourStepOgrCoun,
                                    ref otherfourStepOgrCoun);
                            }


                            addParam.Add(
                                new XAttribute("speedlimit", $"Балл - {ballGap} Кол.огр. {SpeedLimitGap} Огр.{(speedGapPass == -1 ? "" : speedGapPass.ToString())}/{(speedGapFrei == -1 ? "" : speedGapFrei.ToString())} Скор.{(int)kilometer.Speed.Average()} [паспорт]")
                                );

                            addParam.Add(digElemenets);


                            report.Add(addParam);

                        }
                    }
                }
                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);

            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report.html");
                htReport.Save($@"G:\form\G:\form\1.Основные и дополнительные параметры геометрии рельсовой колеи (ГРК)\3.Графическая диаграмма доп.параметров с результатами оценки отступлений.html");

            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report.html");
            }
        }

        private string GetXByDigName(DigName digName)
        {
            float move = 6.6f;
            switch (digName)
            {
                case DigName dn when dn == DigressionName.LongWaveLeft:
                    return MMToPixelChartString(LevelPosition + move);
                case DigName dn when dn == DigressionName.LongWaveRight:
                    return MMToPixelChartString(+move);
                case DigName dn when dn == DigressionName.MiddleWaveLeft:
                    return MMToPixelChartString(+move);
                case DigName dn when dn == DigressionName.MiddleWaveRight:
                    return MMToPixelChartString(+move);
                case DigName dn when dn == DigressionName.ShortWaveLeft:
                    return MMToPixelChartString(+move);
                case DigName dn when dn == DigressionName.ShortWaveRight:
                    return MMToPixelChartString(+move);
            }

            return "-100";
        }
    }
}
