using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using MetroFramework.Controls;
using ALARm_Report.controls;

namespace ALARm_Report.Forms
{
    /// <summary>
    /// График диаграммы сводной по доп параметрам
    /// </summary>
    public class DeviationsPassportPositionPlan : GraphicDiagrams
    {
        private int widthInPixel = 622;
        private float widthImMM = 155;
        private int xBegin = -11;

        private float MMToPixelChart(float mm)
        {
            return widthInPixel / widthImMM * mm + xBegin;
        }
        private string MMToPixelChartString(float mm)
        {
            return (widthInPixel / widthImMM * mm + xBegin).ToString().Replace(",", ".");
        }

        private readonly int LabelsDivWidthInPixel = 550;
        private readonly float LabelsDivWidthInMM = 146;
        private readonly float BottomLabelHeightInMM = 1.6f;
        private string MMToPixelLabel(float mm)
        {
            return (LabelsDivWidthInPixel / LabelsDivWidthInMM * mm).ToString().Replace(",", ".");
        }
        private string MMToPixelChartWidthString(float mm)
        {
            return (widthInPixel / widthImMM * mm).ToString().Replace(",", ".");
        }
        private string MMToPixelChartWidthStringKriviz(float mm)
        {
            return (widthInPixel / widthImMM * mm + 241.5).ToString().Replace(",", ".");
        }
        private string MMToPixelChartWidthStringNerov(float mm)
        {
            return (widthInPixel / widthImMM * mm + 80.4f).ToString().Replace(",", ".");
        }
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
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


            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);

                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);

                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();

                        if (kilometers.Count == 0) continue;

                        var trackName = AdmStructureService.GetTrackName(track_id);

                        tripProcess.TrackID = track_id;
                        tripProcess.TrackName = trackName.ToString();

                        var raw_rd_profile = RdStructureService.GetRdTables(tripProcess, 1) as List<RdProfile>;
                        //Реперные точки
                        var raw_RefPoints = MainTrackStructureService.GetRefPointsByTripIdToDate(track_id, tripProcess.Date_Vrem);

                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var min = raw_RefPoints.Select(o => o.Km).Min();
                        var max = raw_RefPoints.Select(o => o.Km).Max();

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = min });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = max });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;
                        //фильтр
                        raw_RefPoints = raw_RefPoints.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList();
                        if (raw_RefPoints.Count == 0) continue;

                        XElement addParam = new XElement("addparam",

                                new XAttribute("right-title", (
                                    copyright + ": " + "ПО " + softVersion + "  " +
                                    systemName + ":" + tripProcess.Car + "(" + tripProcess.Chief.Trim() + ") (БПД от " +
                                    MainTrackStructureService.GetModificationDate() + ") <" + AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, false) + ">" +
                                    
                                    
                                    "<" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ">" +
                                       "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.Direction.ToString())) + ">" +
                                        "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.Car_Position.ToString())) + ">" +
                                    "<" + period.PeriodMonth + "-" + period.PeriodYear +
                                            " " + (tripProcess.Trip_Type == TripType.Control ? "контр." : tripProcess.Trip_Type == TripType.Work ? "раб." : "доп.") +
                                            " Проезд:" + diagramName + ">" + " Л: " + 1)),

                                new XAttribute("viewbox", "0 0 600 1000"),
                                new XAttribute("minY", 20),
                                new XAttribute("maxY", 1000),
                                new XAttribute("minYLine", 25),
                                new XElement("xgrid",
                                new XAttribute("x0", xBegin),
                                new XAttribute("minY", 20),
                                new XAttribute("x00", MMToPixelChart(5.22f)),
                                new XAttribute("x000", MMToPixelChart(23.92f)),
                                new XAttribute("x0000", MMToPixelChart(42.61f)),

                                new XAttribute("x1", MMToPixelChart(48.3f)),
                                new XAttribute("l1", MMToPixelChart(50.08f)),
                                new XAttribute("l2", MMToPixelChart(62.54f)),
                                new XAttribute("l3", MMToPixelChart(75.75f)),

                                new XAttribute("x2", MMToPixelChart(77.49f)),
                                new XAttribute("l4", MMToPixelChart(87.46f)),
                                new XAttribute("l5", MMToPixelChart(97.43f)),
                                new XAttribute("l6", MMToPixelChart(107.40f)),
                                new XAttribute("l7", MMToPixelChart(117.37f)),
                                new XAttribute("l8", MMToPixelChart(127.33f)),
                                new XAttribute("l9", MMToPixelChart(139.79f)),
                                new XAttribute("l10", MMToPixelChart(151.01f)),

                                new XAttribute("x3", MMToPixelChart(93.69f)),

                                new XAttribute("x4", MMToPixelChart(117.37f)),


                                //астынгы сызык 
                                new XAttribute("maxY1", 1000),
                                new XAttribute("maxY2", MMToPixelChart(245.70f)),
                                new XAttribute("maxY3", MMToPixelChart(251.93f)),
                                new XAttribute("lineLow", widthInPixel - xBegin),

                                new XAttribute("ticks", MMToPixelChart(146) - widthInPixel / widthImMM / 4),
                                new XAttribute("x5", MMToPixelChart(151)),
                                new XAttribute("picket", MMToPixelChart(146) + (MMToPixelChart(152.5f) - MMToPixelChart(151f)) / 2),
                                new XAttribute("x6", MMToPixelChart(152.5f)),
                                new XAttribute("x7", MMToPixelChart(155)),
                                new XAttribute("x8", MMToPixelChart(151f) + (MMToPixelChart(152.5f) - MMToPixelChart(151f)) / 2),
                                new XElement("x", MMToPixelChartString(6.5f)),
                                new XElement("x", MMToPixelChartString(7.75f)),
                                new XElement("x", MMToPixelChartString(9f)),
                                new XElement("x", MMToPixelChartString(20.5f)),
                                new XElement("x", MMToPixelChartString(21.75f)),
                                new XElement("x", MMToPixelChartString(23f)),
                                new XElement("x", MMToPixelChartString(32f)),
                                new XElement("x", MMToPixelChartString(34f)),
                                new XElement("x", MMToPixelChartString(35f)),
                                new XElement("x", MMToPixelChartString(37f)),
                                //НПК лев
                                new XElement("x", MMToPixelChartString(57.5f)),
                                new XElement("x", MMToPixelChartString(60f)),
                                new XElement("x", MMToPixelChartString(61.7f)),
                                new XElement("x", MMToPixelChartString(62.5f)),
                                new XElement("x", MMToPixelChartString(63.3f)),
                                new XElement("x", MMToPixelChartString(64.1f)),
                                new XElement("x", MMToPixelChartString(67f)),
                                //НПК пр
                                new XElement("x", MMToPixelChartString(71.5f)),
                                new XElement("x", MMToPixelChartString(74f)),
                                new XElement("x", MMToPixelChartString(75.7f)),
                                new XElement("x", MMToPixelChartString(76.5f)),
                                new XElement("x", MMToPixelChartString(77.3f)),
                                new XElement("x", MMToPixelChartString(78.1f)),
                                new XElement("x", MMToPixelChartString(81f))
                            ));

                        addParam.Add(new XAttribute("directNaprav", $"{tripProcess.DirectionName}({tripProcess.DirectionCode})"));
                        addParam.Add(new XAttribute("Uchastok", (float)(float)filters[0].Value + " - " + (float)(float)filters[1].Value));
                        addParam.Add(new XAttribute("Put", trackName));
                        addParam.Add(new XAttribute("TravelDate", tripProcess.Date_Vrem.ToString("dd/MM/yyyy HH:mm")));
                        addParam.Add(new XAttribute("CarNumber", tripProcess.Car));

                        var kmElements = new XElement("km");
                        double StrLInd = 20, StrRInd = 0, testInd = 0.0;
                        int prevKm = -1;
                        string OtklOtPryam = string.Empty;
                        string Krivizna = string.Empty;
                        string NerovPlana = string.Empty;

                        //Продольный профиль------------------------------------------------------------------------

                        var start = false;
                        var RefPoints = new List<RefPoint> { };
                        var rd_profile = new List<RdProfile> { };
                        RefPoint PREV = null;

                        foreach (var rp in raw_RefPoints)
                        {
                            if (start == true)
                            {
                                var st = raw_rd_profile.Where(o => o.Km * 1000 + o.Meter > rp.Km * 1000 + rp.Meter).ToList();

                                if (st.Count == 0)
                                {
                                    RefPoints.Add(PREV ?? rp);
                                    break;
                                }
                                PREV = rp;
                            }
                            if (start == false)
                            {
                                var st = raw_rd_profile.Where(o => o.Km * 1000 + o.Meter <= rp.Km * 1000 + rp.Meter).ToList();

                                if (st.Count > 0)
                                {
                                    RefPoints.Add(rp);
                                    start = true;
                                }
                            }
                        }
                        if (RefPoints.Count == 1)
                        {
                            RefPoints.Add(raw_RefPoints.Last());
                        }

                        rd_profile = raw_rd_profile.Where(o =>
                                                              RefPoints.First().Km * 1000 + RefPoints.First().Meter <= o.Km * 1000 + o.Meter &&
                                                              RefPoints.Last().Km * 1000 + RefPoints.Last().Meter >= o.Km * 1000 + o.Meter
                                                         ).ToList();
                        RefPoints = raw_RefPoints.Where(o =>
                                                            RefPoints.First().Km * 1000 + RefPoints.First().Meter <= o.Km * 1000 + o.Meter &&
                                                            RefPoints.Last().Km * 1000 + RefPoints.Last().Meter >= o.Km * 1000 + o.Meter
                                                       ).ToList();

                        var mainParamRiht = MainParametersService.GetMainParametersFromDBMeter(tripProcess.Trip_id);
                        
                        mainParamRiht = mainParamRiht.Where(o =>
                                                              RefPoints.First().Km * 1000 + RefPoints.First().Meter <= o.Km * 1000 + o.Meter &&
                                                              RefPoints.Last().Km * 1000 + RefPoints.Last().Meter >= o.Km * 1000 + o.Meter
                                                              ).ToList();

                        //var mainParameters = RdStructureService.GetRdTables(tripProcess, 11) as List<RdProfile>;

                        var mainParameters = RdStructureService.GetRdTablesByKM(
                                                                                tripProcess,
                                                                                RefPoints.First().Km * 1000 + RefPoints.First().Meter,
                                                                                RefPoints.Last().Km * 1000 + RefPoints.Last().Meter) as List<RdProfile>;

                        //определяем входят ли в кривые участки
                        var GetCurvesList = MainTrackStructureService.GetCurveByTripIdToDate(tripProcess) as List<Curve>;

                        for (int j = 0; j < mainParameters.Count; j++)
                        {
                            var current_coord = mainParameters[j].Km * 1000 + mainParameters[j].Meter;

                            var curve = GetCurvesList.Where(o => o.Start_coord <= current_coord && current_coord <= o.Final_coord).ToList();

                            mainParameters[j].IsCurve = curve.Count() > 0 ? true : false;
                        }

                        //mainParameters = mainParameters.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList();

                        testInd = mainParameters.Count();
                        var rectHeig = 989.0 / testInd;
                        int EarthRad = 6317 * 1000 * 100;

                        List<double> list_Cur_Stat = new List<double>();
                        List<double> list_Cur_Stat_ind = new List<double>();
                        
                        List<double> list_pryamoi_Lat = new List<double>();
                        List<double> list_pryamoi_longe = new List<double>();
                        List<double> list_pryamoi_ind = new List<double>();
                        
                        List<double> list_new_data_lat = new List<double>();
                        List<double> list_new_data_longe = new List<double>();


                        var width = 100;
                        List<double> RollAver_lat = new List<double>();
                        List<double> RollAver_longe = new List<double>();

                        for (int c = 0; c < mainParameters.Count(); c++)
                        {
                            if (RollAver_lat.Count() >= width)
                            {
                                RollAver_lat.Add(mainParameters[c].Latitude);
                                var ra_lat = RollAver_lat.Skip(RollAver_lat.Count() - width).Take(width).Average();

                                RollAver_longe.Add(mainParameters[c].Longitude);
                                var ra_longe = RollAver_longe.Skip(RollAver_longe.Count() - width).Take(width).Average();

                                mainParameters[c].Latitude = ra_lat;
                                mainParameters[c].Longitude = ra_longe;
                            }
                            else
                            {
                                RollAver_lat.Add(mainParameters[c].Latitude);
                                RollAver_longe.Add(mainParameters[c].Longitude);
                            }
                        }


                        //скользящ сред для д
                        var RollAver_d01 = new List<double>();
                        var RollAver_nerov = new List<double>();

                        for (int ii = 0; ii < mainParamRiht.Count-1; ii++)
                        {
                            //обнуляем при станции и кривой
                            if (mainParameters[ii].IsCurve == true && mainParameters[ii].Latitude != -999)
                            {
                                mainParameters[ii].Latitude = -999.0;
                                mainParameters[ii].Longitude = -999.0;

                                list_Cur_Stat.Add(0.0);
                                list_Cur_Stat_ind.Add(StrLInd);
                            }
                            if (mainParameters[ii].IsStation == true && mainParameters[ii].Latitude != -999)
                            {
                                mainParameters[ii].Latitude = -999.0;
                                mainParameters[ii].Longitude = -999.0;

                                list_Cur_Stat.Add(0.0);
                                list_Cur_Stat_ind.Add(StrLInd);
                            }

                            if (mainParameters[ii].Latitude == -999.0)
                            {
                                if (list_pryamoi_Lat.Count > 0)
                                {
                                    var x1 = EarthRad * Math.Cos(list_pryamoi_Lat.First().Radian()) * Math.Cos(list_pryamoi_longe.First().Radian());
                                    var y1 = EarthRad * Math.Cos(list_pryamoi_Lat.First().Radian()) * Math.Sin(list_pryamoi_longe.First().Radian());
                                    var z1 = EarthRad * Math.Sin(list_pryamoi_Lat.First().Radian());

                                    var x2 = EarthRad * Math.Cos(list_pryamoi_Lat.Last().Radian()) * Math.Cos(list_pryamoi_longe.Last().Radian());
                                    var y2 = EarthRad * Math.Cos(list_pryamoi_Lat.Last().Radian()) * Math.Sin(list_pryamoi_longe.Last().Radian());
                                    var z2 = EarthRad * Math.Sin(list_pryamoi_Lat.Last().Radian());

                                    var l = x2 - x1;
                                    var m = y2 - y1;
                                    var n = z2 - z1;
                                    var dh = 1.0 / list_pryamoi_Lat.Count;

                                    //прямая линия
                                    List<double> xt_list = new List<double>();
                                    List<double> yt_list = new List<double>();
                                    List<double> zt_list = new List<double>();

                                    //список средних знач
                                    List<double> ra_d01_list = new List<double>();

                                    //траектория
                                    //List<double> xtr_list = new List<double>();
                                    //List<double> ytr_list = new List<double>();
                                    //List<double> ztr_list = new List<double>();

                                    for (int k = 0; k < list_pryamoi_Lat.Count; k++)
                                    {
                                        //xt_list.Add(x1 + list_pryamoi_ind[k] * dh * l);
                                        //yt_list.Add(y1 + list_pryamoi_ind[k] * dh * m);
                                        //zt_list.Add(z1 + list_pryamoi_ind[k] * dh * n);

                                        var xtr_list = (EarthRad * Math.Cos(list_pryamoi_Lat[k].Radian()) * Math.Cos(list_pryamoi_longe[k].Radian()));
                                        var ytr_list = (EarthRad * Math.Cos(list_pryamoi_Lat[k].Radian()) * Math.Sin(list_pryamoi_longe[k].Radian()));
                                        var ztr_list = (EarthRad * Math.Sin(list_pryamoi_Lat[k].Radian()));


                                        //var d01 = Math.Sqrt(
                                        //    Math.Pow((xtr_list - x1) * m - l * (ytr_list - y1), 2) +
                                        //    Math.Pow((ytr_list - y1) * n - m * (ztr_list - z1), 2) +
                                        //    Math.Pow((ztr_list - z1) * l - n * (xtr_list - x1), 2)) / Math.Sqrt(l * l + m * m + n * n) ;

                                        ////проекция на плоскость
                                        //d01 = Math.Sqrt(d01*d01 - Math.Pow(z1 - z2, 2));

                                        //параметры
                                        var A = l * l + m * m + n * n;
                                        var B = -2.0 * (xtr_list - x1) * l - 2.0 * (ytr_list - y1) * m - 2.0 * (ztr_list - z1) * n;
                                        var t = -B / (2.0 * A);

                                        //проекция
                                        var xpr = x1 + l * t;
                                        var ypr = y1 + m * t;
                                        var zpr = z1 + n * t;

                                        //проекция на плоскость плана
                                        var d01 = Math.Sqrt(
                                            Math.Pow(xtr_list - xpr, 2) + Math.Pow(ytr_list - ypr, 2)
                                            ) * Math.Sign(xpr - xtr_list);



                                        if (RollAver_d01.Count() >= width)
                                        {
                                            RollAver_d01.Add(d01);

                                            ra_d01_list.Add(RollAver_d01.Skip(RollAver_d01.Count() - width).Take(width).Average());
                                        }
                                        else
                                        {
                                            RollAver_d01.Add(d01);
                                            ra_d01_list.Add(d01);
                                        }

                                        //var diff_lat = (list_pryamoi_Lat.Last() - list_pryamoi_Lat.First()) / list_pryamoi_Lat.Count;
                                        //var diff_longe = (list_pryamoi_longe.Last() - list_pryamoi_longe.First()) / list_pryamoi_longe.Count;

                                        //var latitude = (list_pryamoi_Lat[k] - list_pryamoi_Lat[0] - diff_lat * k) * 1000000.0;
                                        //var longiyude = (list_pryamoi_longe[k] - list_pryamoi_longe[0] - diff_longe * k) * 1000000.0;

                                        //NerovPlana += (D_exp + 459.994476).ToString().Replace(",", ".") + "," + list_pryamoi_ind[k].ToString().Replace(",", ".") + " ";
                                        //Krivizna += (longiyude + 240.6f).ToString().Replace(",", ".") + "," + list_pryamoi_ind[k].ToString().Replace(",", ".") + " ";
                                    }
                                    

                                    var ExponentCoef = -10;
                                    for (var x = 0; x < ra_d01_list.Count; x++)
                                    {
                                        var e = Math.Exp(ExponentCoef * Math.Abs(RollAver_d01[x] - ra_d01_list[x]));
                                        var D_exp = ra_d01_list[x] + (RollAver_d01[x] - ra_d01_list[x]) * e;

                                        OtklOtPryam += (D_exp*0.4 + 459.994476).ToString().Replace(",", ".") + "," + list_pryamoi_ind[x].ToString().Replace(",", ".") + " ";

                                    }

                                    list_pryamoi_Lat.Clear();
                                    list_pryamoi_longe.Clear();
                                    list_pryamoi_ind.Clear();
                                }
                            }
                            else
                            {
                                if (list_Cur_Stat.Count > 0)
                                {
                                    for (int k = 0; k < list_Cur_Stat.Count; k++)
                                    {
                                        OtklOtPryam += (459.994476).ToString().Replace(",", ".") + "," + list_Cur_Stat_ind[k].ToString().Replace(",", ".") + " ";
                                    }
                                    list_Cur_Stat.Clear();
                                    list_Cur_Stat_ind.Clear();
                                }

                                list_pryamoi_Lat.Add(mainParameters[ii].Latitude);
                                list_pryamoi_longe.Add(mainParameters[ii].Longitude);
                                list_pryamoi_ind.Add(StrLInd);
                            }


                            ////Спутники
                            //if (prevC_sat != -1 && mainParameters[ii].Jgps_sats!=-999 && prevC_sat !=-999 && prevC_sat != mainParameters[ii].Jgps_sats)
                            //{
                            //    kmElements.Add(new XElement("text", $"sat {prevC_sat}=>{mainParameters[ii].Jgps_sats} {mainParameters[ii].Km}km {mainParameters[ii].Meter}m",
                            //        new XAttribute("y", 10),
                            //        new XAttribute("x", StrLInd))
                            //        );
                            //}
                            //prevC_sat = mainParameters[ii].Jgps_sats;

                            ////Станции
                            //if(mainParameters[ii].IsStation == false && mainParameters[ii + 1].IsStation == true)
                            //{
                            //    kmElements.Add(new XElement("text", $"нач Стан {mainParameters[ii].Km}km {mainParameters[ii].Meter}m",
                            //        new XAttribute("y", -15),
                            //        new XAttribute("x", StrLInd))
                            //        );
                            //}
                            //if (mainParameters[ii].IsStation == true && mainParameters[ii + 1].IsStation == false)
                            //{
                            //    kmElements.Add(new XElement("text", $"кон Стан {mainParameters[ii].Km}km {mainParameters[ii].Meter}m",
                            //        new XAttribute("y", -15),
                            //        new XAttribute("x", StrLInd))
                            //        );
                            //}
                            ////Кривая
                            //if (mainParameters[ii].IsCurve == false && mainParameters[ii + 1].IsCurve == true)
                            //{
                            //    kmElements.Add(new XElement("text", $"нач Крив {mainParameters[ii].Km}km {mainParameters[ii].Meter}m",
                            //        new XAttribute("y", -10),
                            //        new XAttribute("x", StrLInd))
                            //        );
                            //}
                            //if (mainParameters[ii].IsCurve == true && mainParameters[ii + 1].IsCurve == false)
                            //{
                            //    kmElements.Add(new XElement("text", $"кон Крив {mainParameters[ii].Km}km {mainParameters[ii].Meter}m",
                            //        new XAttribute("y", -5),
                            //        new XAttribute("x", StrLInd))
                            //        );
                            //}

                            //КМ
                            if (prevKm != mainParameters[ii].Km)
                            {
                                kmElements.Add(new XElement("text", mainParameters[ii].Km + " км",
                                    new XAttribute("y", -600),
                                    new XAttribute("x", StrLInd))
                                );
                                prevKm = mainParameters[ii].Km;
                            }

                            //NerovPlana += ((mainParameters[ii].Stright_right - mainParameters[ii].Stright_avg) + 80).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";
                            //Krivizna += (mainParameters[ii].Stright_avg + 240.6f).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";

                            //if (RollAver_lat.Count() >= width)
                            //{
                            //    RollAver_lat.Add(latitude);
                            //    var ra_lat = RollAver_lat.Skip(RollAver_lat.Count() - width).Take(width).Average();

                            //    RollAver_longe.Add(longiyude);
                            //    var ra_longe = RollAver_longe.Skip(RollAver_longe.Count() - width).Take(width).Average();

                            //    NerovPlana += (ra_lat + 85.0).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";
                            //    Krivizna += (ra_longe + 240.6f).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";

                            //    RollAver_lat.Add(ra_lat);
                            //    RollAver_longe.Add(ra_longe);
                            //}
                            //else
                            //{
                            //    RollAver_lat.Add(latitude);
                            //    RollAver_longe.Add(longiyude);

                            //    NerovPlana += (latitude + 85.0).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";
                            //    Krivizna += (longiyude + 240.6f).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";
                            //}

                            var nerov = (mainParameters[ii].IsCurve == true ? 0.0 : (mainParamRiht[ii].Stright_left - mainParamRiht[ii].Stright_avg));


                            if (RollAver_nerov.Count() >= 50)
                            {
                                RollAver_nerov.Add(nerov);

                                nerov = (RollAver_nerov.Skip(RollAver_nerov.Count() - 50).Take(50).Average());
                            }
                            else
                            {
                                nerov = nerov / 5;
                                RollAver_nerov.Add(nerov);
                            }


                            NerovPlana += ((mainParameters[ii].IsCurve == true ? 0.0 : (nerov*25*2)*Math.Exp(-Math.Abs(nerov/2))) + 85.0).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";

                            Krivizna += (mainParamRiht[ii].Stright_avg + 240.6f).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";

                            StrLInd = StrLInd + rectHeig;
                        }

                        var linesElem = new XElement("lines");
                        linesElem.Add(new XElement("NerovPlana", NerovPlana));
                        linesElem.Add(new XElement("Krivizna", Krivizna));
                        linesElem.Add(new XElement("Krivizna", OtklOtPryam));

                        addParam.Add(linesElem);
                        addParam.Add(kmElements);

                        report.Add(addParam);
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

    }
}
