using ALARm.Core;
using ALARm.Core.Report;
using MetroFramework.Controls;
using ALARm.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using ALARm_Report.controls;
using ALARm.DataAccess;
using System.Reflection;

namespace ALARm_Report.Forms
{
    public class GraphLongitudinalProfile : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(distanceId, period);
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

                var mainProcesses = RdStructureService.GetProcess(period, distanceId, ProcessType.VideoProcess);
                if (!mainProcesses.Any())
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                foreach (var process in mainProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trip = RdStructureService.GetTrip(process.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);

                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();

                        if (kilometers.Count == 0) continue;



                        var trackName = AdmStructureService.GetTrackName(track_id);

                        process.TrackID = track_id;
                        process.TrackName = trackName.ToString();

                        //Продольный профиль
                        var raw_rd_profile = RdStructureService.GetRdTables(process, 1) as List<RdProfile>;
                        if (raw_rd_profile.Count() == 0 || raw_rd_profile == null) continue;


                        


                        //Реперные точки
                        var raw_RefPoints = MainTrackStructureService.GetRefPointsByTripIdToDate(track_id, process.Date_Vrem);
                        if (raw_RefPoints.Count() == 0 || raw_RefPoints == null) continue;
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var min = raw_RefPoints.Select(o => o.Km).Min();
                        var max = raw_RefPoints.Select(o => o.Km).Max();

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = 710 });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = 730 });


                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;
                        //фильтр
                        raw_RefPoints = raw_RefPoints.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList();


                        if (raw_RefPoints.Count == 0) continue;

                        var roadName = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);

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
                        if (rd_profile.Count() == 0 || rd_profile == null) continue;
                        RefPoints = raw_RefPoints.Where(o =>
                                                            RefPoints.First().Km * 1000 + RefPoints.First().Meter <= o.Km * 1000 + o.Meter &&
                                                            RefPoints.Last().Km * 1000 + RefPoints.Last().Meter >= o.Km * 1000 + o.Meter
                                                       ).ToList();





                        var startP = rd_profile.IndexOf(rd_profile.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList().First());
                        var finalP = rd_profile.IndexOf(rd_profile.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList().Last());

                        var countP = finalP - startP;

                        var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                        var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);
                        if (!rd_profile.Any())
                            continue;

                        var track_profile = rd_profile;
                        //var track_profile = rd_profile.Where(r => r.Track_id == trackId).OrderBy(r => r.X).ToList();


                        var trackId = track_id;
                        var tripDate = process.Date_Vrem;

                        List<int> x_RefPoints = new List<int>();
                        List<int> x_track_profile = new List<int>();
                        List<int> xInd = new List<int>();
                        List<double> yr = new List<double>();

                        int j = 0;
                        foreach (var elem in track_profile)
                        {
                            var t = RefPoints.Where(o => o.Km == elem.Km && o.Meter == elem.Meter).ToList();
                            if (t.Count > 0)
                            {
                                x_RefPoints.Add(j + 1);
                                xInd.Add(j);
                                yr.Add(elem.Y);
                            }
                            x_track_profile.Add(j + 1);

                            j++;
                        }

                        ProfileData profileData = new ProfileData(
                                                                 //profile
                                                                 x_track_profile,
                                                                 track_profile.Select(p => p.Y).ToList(),

                                                                 track_profile.Select(p => p.Deviation).ToList(),

                                                                 //repers
                                                                 RefPoints.Select(p => p.Mark).ToList(),
                                                                 x_RefPoints,

                                                                 yr,
                                                                 xInd,
                                                                 startP,
                                                                 countP
                                                                 );

                        var original = profileData.GraphProfile2();

                        //int origCount = original["original"].Count;
                        // var linear = profileData.getLinearGraph();
                        //int linearCount = original["linear"].Count;
                        int new_original = original["new_original"].Count;
                        int new_linearCount = original["new_linear"].Count;
                        //int linearReper = original["linearReper"].Count;
                        //int linearReperByOrig = original["linearReperByOrig"].Count;
                        int ORIGINminusLBOplusLR = original["ORIGINminusLBOplusLR"].Count;
                        //int ReperLOlinear = original["ReperLOlinear"].Count;

                        var prevKm = -1;
                        var prevPt = -1;

                        for (int gi = 0; gi < new_original-1; gi++)
                        {
                            //if (gi > 3)
                            //    break;

                            var tempProf = rd_profile.GetRange(gi * 5000, rd_profile.Count() - gi * 5000 > 5000 ? 5000 : rd_profile.Count() - gi * 5000);
                            var first = rd_profile.GetRange(gi * 5000, rd_profile.Count() - gi * 5000 > 5000 ? 5000 : rd_profile.Count() - gi * 5000).First();
                            var last = rd_profile.GetRange(gi * 5000, 5000).Last();
                            int start_km = first.Km, start_m = first.M,
                            final_km = last.Km, final_m = last.M;

                            XElement xePages = new XElement("pages",
                                  new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version}"),

                                 new XAttribute("period", period.Period),
                                new XAttribute("road", roadName),
                                new XAttribute("distance", distance.Code ?? ""),
                                new XAttribute("trip_info", process.GetProcessTypeName),
                                new XAttribute("car", process.Car ?? ""),
                                new XAttribute("trip_date", process.Trip_date ?? ""),
                                new XAttribute("direction", $" { process.DirectionName}({ process.DirectionCode})"),
                                new XAttribute("track", trackName)
                            );

                            // all the calculated points 
                            xePages.Add(
                                //new XAttribute("graph", original["original"][gi])
                                //new XAttribute("straight_graph", gi < linearCount ? original["linear"][gi] : " "),

                                //new XAttribute("new_original", gi < new_original ? original["new_original"][gi] : " "),

                                //new XAttribute("new_straight_graph", gi < new_linearCount ? original["new_linear"][gi] : " "),

                                //new XAttribute("deviation_graph", gi < new_original ? profileData.getDeviationGraphByReper() : " "),

                                //new XAttribute("linearReper", gi < linearReper ? original["linearReper"][gi] : " "),

                                //new XAttribute("linearReperByOrig", gi < linearReperByOrig ? original["linearReperByOrig"][gi] : " "),

                                new XAttribute("ORIGINminusLBOplusLR", gi < ORIGINminusLBOplusLR ? original["ORIGINminusLBOplusLR"][gi] : " "), //green

                                new XAttribute("linearReper", gi < new_linearCount ? original["new_linear"][gi] : " ") //red
                            );

                            //profileData.StraightInfo(xePages);
                            //profileData.PicketInfo(xePages);


                            double x_coef = 25.0 / 5000;
                            int lastCoord = first.X;

                            var rp = profileData.straightProfilesNew.Where(r => r.CoordAbs >= gi * 5000 && r.CoordAbs <= (gi * 5000) + 5000).ToList();


                            for (int i = 0; i < rp.Count; i++)
                            {
                                var coord = (rd_profile[rp[i].CoordAbs - 1].X - first.X) * x_coef;
                                try
                                {
                                    xePages.Add(new XElement("RefPoint",
                                        //Метка
                                        new XAttribute("xr", $"{(coord - 0.131).ToString("0.####").Replace(',', '.')}cm"), //0.131 - это длина метки
                                        new XAttribute("yr", profileData.ReperY[rp[i].CoordAbs - 1] - 10) //10 - это высота метки
                                        ));
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"new XElement(RefPoint, {e.Message}");
                                }

                                //value метки
                                xePages.Add(new XElement("straights_value",
                                    new XAttribute("x", -0.9),
                                    new XAttribute("y", $"{(coord + 0.075 < 0.15 ? 0.15 : coord + 0.075).ToString("0.####").Replace(',', '.')}"),
                                    new XAttribute("rotate", 270),
                                    new XAttribute("text", rp[i].Profile.ToString("0.##").Replace(',', '.'))));

                                //90 гр линия разделитель
                                xePages.Add(new XElement("straights",
                                    new XAttribute("x1", coord),
                                    new XAttribute("x2", coord),
                                    new XAttribute("y1", 1),
                                    new XAttribute("y2", 2)));

                                //между разделителями
                                var lineST = false;
                                var prev_lineST = false;
                                try
                                {
                                    var up = false;
                                    var down = false;

                                    var stt = (rd_profile[rp[i].CoordAbs - 1].X - first.X);
                                    var cnn = (rd_profile[rp[i + 1].CoordAbs - 1].X) - (rd_profile[rp[i].CoordAbs - 1].X);

                                    var gdY = profileData.ORIGINminusLBOplusLRrrr.GetRange(stt < 0 ? 0 : stt, cnn).ToList();
                                    var rdY = profileData.linearPointYNew.GetRange(stt < 0 ? 0 : stt, cnn).ToList();

                                    var gdX = profileData.XX.GetRange(stt < 0 ? 0 : stt, cnn).ToList();

                                    for (int qq = 0; qq < gdY.Count - 2; qq++)
                                    {
                                        var dev = rdY[qq] - gdY[qq];
                                        var div2 = (rdY[qq + 2] - gdY[qq + 2]) - 2.0 * (rdY[qq + 1] - gdY[qq + 1]) + (rdY[qq] - gdY[qq]);

                                        if (up == true && down == false)
                                        {
                                            if (div2 < 0)
                                            {
                                                lineST = true;
                                                break;
                                            }
                                        }
                                        if (up == false && down == true)
                                        {
                                            if (div2 > 0)
                                            {
                                                lineST = true;
                                                break;
                                            }
                                        }

                                        if (up == false && down == false)
                                        {
                                            if (div2 < 0)
                                                down = true;
                                            else if (div2 > 0)
                                                up = true;
                                        }
                                    }

                                    if (lineST == false)
                                    {
                                        //радиус
                                        var x1 = gdX.First();
                                        var y1 = gdY.First();

                                        var x2 = gdX[gdX.Count / 2];
                                        var y2 = gdY[gdX.Count / 2];

                                        var x3 = gdX.Last();
                                        var y3 = gdY.Last();

                                        var A = x2 - x1;
                                        var B = y2 - y1;
                                        var C = x3 - x1;
                                        var D = y3 - y1;
                                        var E = A * (x1 + x2) + B * (y1 + y2);
                                        var F = C * (x1 + x3) + D * (y1 + y3);

                                        var G = 2 * (A * (y3 - y2) - B * (x3 - x2));
                                        //Если G = 0, это значит, что через данный набор точек провести окружность нельзя.
                                        // координаты центра
                                        var Cx = (D * E - B * F) / G;
                                        var Cy = (A * F - C * E) / G;

                                        // радиус
                                        var R = Math.Sqrt(Math.Pow(x1 - Cx, 2) + Math.Pow(y1 - Cy, 2));

                                        var len = ((rd_profile[rp[i + 1].CoordAbs - 1].X)) - (rd_profile[rp[i].CoordAbs - 1].X);

                                        xePages.Add(new XElement("straights_value",
                                                new XAttribute("x", (((rd_profile[rp[i].CoordAbs - 1].X) - (first.X) + len / 4) * x_coef)),
                                                new XAttribute("y", 1.2),
                                                new XAttribute("rotate", 0),
                                                new XAttribute("text", R.ToString("0"))
                                                ));
                                        xePages.Add(new XElement("straights_value",
                                                new XAttribute("x", (((rd_profile[rp[i].CoordAbs - 1].X) - (first.X) + len / 4) * x_coef)),
                                                new XAttribute("y", 1.4),
                                                new XAttribute("rotate", 0),
                                                new XAttribute("text", len)
                                                ));

                                        for (int qq = 0; qq < gdY.Count; qq++)
                                        {
                                            var y = (Math.Pow(Math.Abs(gdX[qq] - gdX.First()), 0.5) * Math.Pow(Math.Abs(gdX[qq] - gdX.Last()), 0.5)) / (Math.Pow((Math.Abs(gdX.First() - gdX.Last())) / 2, 1));
                                            if (up == true)
                                            {
                                                xePages.Add(new XElement("semicircle_line",
                                                    new XAttribute("xr", $"{(gdX[qq]).ToString("0.####").Replace(',', '.')}"),
                                                    new XAttribute("yr", y * 37 + 37.9)
                                                ));
                                            }
                                            if (down == true)
                                            {
                                                xePages.Add(new XElement("semicircle_line",
                                                    new XAttribute("xr", $"{(gdX[qq]).ToString("0.####").Replace(',', '.')}"),
                                                    new XAttribute("yr", -y * 37 + 37.9 + 37.9)
                                                ));
                                            }
                                        }
                                    }
                                    if (lineST == true)
                                    {
                                        if (rp[i].Profile < rp[i + 1].Profile)
                                        {
                                            xePages.Add(new XElement("straights",
                                                new XAttribute("x1", ((rd_profile[rp[i].CoordAbs - 1].X) - (first.X)) * x_coef),
                                                new XAttribute("x2", ((rd_profile[rp[i + 1].CoordAbs - 1].X) - (first.X)) * x_coef),
                                                new XAttribute("y1", 2),
                                                new XAttribute("y2", 1)
                                                ));

                                            //Уклон
                                            var uklon = (rp[i + 1].Profile - rp[i].Profile) / (rp[i + 1].CoordAbs - rp[i].CoordAbs) * 1000;
                                            xePages.Add(new XElement("straights_value",
                                                new XAttribute("x", (((rd_profile[rp[i].CoordAbs - 1].X) - (first.X)) * x_coef) + 0.02),
                                                new XAttribute("y", 1.2),
                                                new XAttribute("rotate", 0),
                                                new XAttribute("text", (uklon).ToString("0.##").Replace(',', '.'))
                                                ));

                                            xePages.Add(new XElement("straights_value",
                                                new XAttribute("x", (((rd_profile[rp[i + 1].CoordAbs - 1].X) - (first.X)) * x_coef) - 0.4),
                                                new XAttribute("y", 1.95),
                                                new XAttribute("rotate", 0),
                                                new XAttribute("text", (rd_profile[rp[i + 1].CoordAbs - 1].X) - (rd_profile[rp[i].CoordAbs - 1].X))
                                                ));

                                        }
                                        else if (rp[i].Profile > rp[i + 1].Profile)
                                        {
                                            xePages.Add(new XElement("straights",
                                                new XAttribute("x1", ((rd_profile[rp[i].CoordAbs - 1].X) - (first.X)) * x_coef),
                                                new XAttribute("x2", ((rd_profile[rp[i + 1].CoordAbs - 1].X) - (first.X)) * x_coef),
                                                new XAttribute("y1", 1),
                                                new XAttribute("y2", 2)
                                            ));
                                            //Уклон
                                            var uklon = (rp[i + 1].Profile - rp[i].Profile) / (rp[i + 1].CoordAbs - rp[i].CoordAbs) * 1000;
                                            xePages.Add(new XElement("straights_value",
                                                new XAttribute("x", (((rd_profile[rp[i + 1].CoordAbs - 1].X) - (first.X)) * x_coef) - 0.4),
                                                new XAttribute("y", 1.2),
                                                new XAttribute("rotate", 0),
                                                new XAttribute("text", (uklon).ToString("0.##").Replace(',', '.'))
                                            ));
                                            xePages.Add(new XElement("straights_value",
                                                new XAttribute("x", (((rd_profile[rp[i].CoordAbs - 1].X) - (first.X)) * x_coef) + 0.02),
                                                new XAttribute("y", 1.95),
                                                new XAttribute("rotate", 0),
                                                new XAttribute("text", (rd_profile[rp[i + 1].CoordAbs - 1].X) - (rd_profile[rp[i].CoordAbs - 1].X)
                                                )));

                                        }
                                        else if (rp[i].Profile == rp[i + 1].Profile)
                                        {
                                            xePages.Add(new XElement("straights",
                                                new XAttribute("x1", ((rd_profile[rp[i].CoordAbs - 1].X) - (first.X)) * x_coef),
                                                new XAttribute("x2", ((rd_profile[rp[i + 1].CoordAbs - 1].X) - (first.X)) * x_coef),
                                                new XAttribute("y1", 1.5),
                                                new XAttribute("y2", 1.5)
                                                ));
                                            xePages.Add(new XElement("straights_value",
                                                new XAttribute("x", (((rd_profile[rp[i].CoordAbs - 1].X) - (first.X)) * x_coef) + 0.02),
                                                new XAttribute("y", 1.2),
                                                new XAttribute("rotate", 0),
                                                new XAttribute("text", "")
                                                ));
                                            xePages.Add(new XElement("straights_value",
                                                new XAttribute("x", (((rd_profile[rp[i].CoordAbs - 1].X) - (first.X)) * x_coef) + 0.02),
                                                new XAttribute("y", 1.95),
                                                new XAttribute("rotate", 0),
                                                new XAttribute("text", (rd_profile[rp[i + 1].CoordAbs - 1].X) - (rd_profile[rp[i].CoordAbs - 1].X)
                                                )));

                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"между разделителями {e.Message }");
                                    prev_lineST = lineST;
                                }
                            }

                            //Километры и пикеты
                            var CurrentObj = rd_profile.GetRange(gi == 0 ? gi * 5000 : gi * 5000, 5000);
                            
                            double prevValue = profileData.ORIGINminusLBOplusLRrrr[0];
                            var prevCoord = CurrentObj[0];


                            foreach (var obj in CurrentObj)
                            {
                                if (prevKm != obj.Km)
                                {
                                    xePages.Add(new XElement("Kms",
                                        new XAttribute("x",
                                            $"{((obj.X - first.X) * x_coef).ToString("0.####").Replace(',', '.')}cm"),
                                        new XAttribute("txt", obj.Km)
                                    ));
                                }
                                prevKm = obj.Km;

                                var CurrentPt = (obj.Meter / 100) + 1;
                                if (prevPt != CurrentPt)
                                {
                                    //value по пикетам Высота
                                    xePages.Add(new XElement("pickets_value",
                                        new XAttribute("x", -2.98),

                                        //между пикетами линия
                                        new XAttribute("x1", (prevValue - profileData.ORIGINminusLBOplusLRrrr[CurrentObj.IndexOf(obj)]) > 0 ? 4 : 3),
                                        new XAttribute("x2", (prevValue - profileData.ORIGINminusLBOplusLRrrr[CurrentObj.IndexOf(obj)]) > 0 ? 3 : 4),

                                        new XAttribute("y", $"{((obj.X - first.X) * x_coef).ToString("0.####").Replace(',', '.')}"),
                                        new XAttribute("y_prev", $"{((prevCoord.X - first.X) * x_coef).ToString("0.####").Replace(',', '.')}"),

                                        new XAttribute("yP", $"{((obj.X - first.X) * x_coef - 0.15 - 0.15).ToString("0.####").Replace(',', '.')}"),
                                        new XAttribute("yR", $"{((obj.X - first.X) * x_coef - 0.325).ToString("0.####").Replace(',', '.')}"),
                                        new XAttribute("text", profileData.ORIGINminusLBOplusLRrrr[CurrentObj.IndexOf(obj)].ToString("0.##").Replace(',', '.')),
                                        new XAttribute("razn", ((profileData.ORIGINminusLBOplusLRrrr[CurrentObj.IndexOf(obj)] - prevValue) * 10.0).ToString("0.#").Replace(',', '.')
                                        )));

                                    prevValue = profileData.ORIGINminusLBOplusLRrrr[CurrentObj.IndexOf(obj)];

                                    prevCoord = obj;

                                    prevPt = CurrentPt;
                                    //if (CurrentPt == 0 || CurrentPt == 10) continue;

                                    xePages.Add(new XElement("Pikets",
                                        new XAttribute("x",
                                            $"{((obj.X - first.X) * x_coef).ToString("0.####").Replace(',', '.')}cm"),
                                        new XAttribute("txt", CurrentPt)
                                    ));
                                }
                                prevPt = CurrentPt;
                            }

                            List<Curve> profileCurves = (RdStructureService.GetRdProfileObjects(trackId, tripDate, 0, start_km, start_m, final_km, final_m) as List<Curve>).OrderBy(c => c.RealStartCoordinate).ToList();

                            //kopirler
                            List<ArtificialConstruction> profileBridges = (RdStructureService.GetRdProfileObjects(trackId, Convert.ToDateTime(tripDate), 1, start_km, start_m, final_km, final_m) as List<ArtificialConstruction>).OrderBy(c => c.RealStartCoordinate).ToList();
                            //joldan basha jolga jurgizetin jerler
                            List<Switch> profileSwitches = (RdStructureService.GetRdProfileObjects(trackId, Convert.ToDateTime(tripDate), 2, start_km, start_m, final_km, final_m) as List<Switch>).OrderBy(c => c.RealStartCoordinate).ToList();
                            //temir jol station der
                            List<StationSection> profileStations = (RdStructureService.GetRdProfileObjects(trackId, Convert.ToDateTime(tripDate), 3, start_km, start_m, final_km, final_m) as List<StationSection>).OrderBy(c => c.RealStartCoordinate).ToList();

                            var lastStation = "";
                            foreach (var station in profileStations)
                            {
                                //if ((station.Final_Km * 1000 + station.Final_M) - (start_km * 1000 + start_m) < 500)
                                //{
                                //    xePages.Add(new XElement("stations",
                                //        //new XAttribute("x", 0.01),
                                //        new XAttribute("x", 0.8),
                                //        new XAttribute("y", 0.35),
                                //        new XAttribute("text", station.Station)));
                                //}
                                //else if ((station.Start_Km * 1000 + station.Start_M) - (start_km * 1000 + start_m) > 4500)
                                //{
                                //    xePages.Add(new XElement("stations",
                                //        new XAttribute("x", 25 - station.Station.Length * 0.15),
                                //        new XAttribute("y", 0.35),
                                //        new XAttribute("text", station.Station)));
                                //}
                                //else

                                var startStInd = GetIndexFromKmM(tempProf, station.Start_Km, station.Start_M);
                                var finalStInd = GetIndexFromKmM(tempProf, station.Final_Km, station.Final_M);

                                var axisInd = GetIndexFromKmM(tempProf, station.Axis_Km, station.Axis_M);
                                {
                                    var pr = station.PrevStation == null ? $"-{station.Station}" : $"{station.PrevStation}-{station.Station}";
                                    var nx = station.NextStation == null ? $"{station.Station}-" : $"{station.Station}-{station.NextStation}";

                                    

                                    

                                    xePages.Add(new XElement("stations",
                                        new XAttribute("start", ((startStInd) - (first.X)) * x_coef),
                                        new XAttribute("startStationCoord", ((startStInd) - (first.X)) * x_coef - pr.Length * 0.1),
                                        new XAttribute("startStation", pr),

                                        new XAttribute("final", ((finalStInd) - (first.X)) * x_coef),
                                        new XAttribute("finalStationCoord", ((finalStInd) - (first.X)) * x_coef + 0.2),
                                        new XAttribute("finalStation", nx),

                                        new XAttribute("x", ((axisInd) - (first.X)) * x_coef - station.Station.Length * 0.025),
                                        new XAttribute("y", 0.25),
                                        new XAttribute("text", station.Station)
                                        ));
                                }
                                if (lastCoord != 0)
                                {
                                    xePages.Add(new XElement("stations",
                                        new XAttribute("x", (lastCoord + (lastCoord + startStInd) / 2.0 - first.X) * x_coef - lastStation.Length * 0.15),
                                        new XAttribute("y", 0.35),
                                        new XAttribute("text", lastStation + station.Station)));
                                }
                                lastCoord = finalStInd;
                                lastStation = station.Station + "-";
                            }

                            lastCoord = first.X;

                            foreach (var switch_ in profileSwitches)
                            {

                                var swInd = GetIndexFromKmM(tempProf, switch_.Km, switch_.Meter);

                                xePages.Add(new XElement("switches",
                                    new XAttribute("x1", ((swInd) - (first.X)) * x_coef),
                                    new XAttribute("y1", 0.9),
                                    new XAttribute("x2", ((swInd) - (first.X)) * x_coef),
                                    new XAttribute("y2", 1.3)));
                            }

                            foreach (var bridge in profileBridges)
                            {
                                var swInd = GetIndexFromKmM(tempProf, bridge.Km, bridge.Meter);

                                var k = swInd;
                                xePages.Add(new XElement("bridges",
                                    new XAttribute("x1", ((swInd) - (first.X)) * x_coef),
                                    new XAttribute("x2", (((swInd) - (first.X)) * x_coef - k.ToString().Length * 0.035)),
                                    new XAttribute("x3", (((swInd) - (first.X)) * x_coef - k.ToString().Length * 0.015)),
                                    new XAttribute("koord", $"{k}"),
                                    new XAttribute("len", $"L={bridge.Len}")));

                            }
                            Console.Out.WriteLine("curve count 2 draw ");

                            foreach (var curves in profileCurves)
                            {

                                curves.Elevations = (MainTrackStructureService.GetCurves(curves.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();

                                var curveStartInd = GetIndexFromKmM(tempProf, curves.Start_Km, curves.Start_M);
                                var curveFinalInd = GetIndexFromKmM(tempProf, curves.Final_Km, curves.Final_M);

                                if (curveStartInd > curveFinalInd)
                                    curveFinalInd = 0;

                                xePages.Add(new XElement("curves",
                                    new XAttribute("x1", (lastCoord - first.X) * x_coef),
                                    new XAttribute("y1", 0.5),
                                    new XAttribute("x2", (curveStartInd - first.X) * x_coef),
                                    new XAttribute("y2", 0.5)));

                                var lenCurve = Math.Abs(curveStartInd - curveFinalInd) / 2;

                                if ((Side)curves.Side_id == Side.Left)
                                {
                                    //Коробка
                                    xePages.Add(new XElement("curvesBox",
                                        new XAttribute("x1", (curveStartInd - first.X) * x_coef),
                                        new XAttribute("y1", 0.5),
                                        new XAttribute("x2", (curveStartInd - first.X) * x_coef),
                                        new XAttribute("y2", 0.05)));
                                    //---

                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", (curveStartInd - first.X) * x_coef),
                                        new XAttribute("y1", 0.5),
                                        new XAttribute("x2", ((curveStartInd - first.X) + curves.Elevations[0].Transition_1) * x_coef),
                                        new XAttribute("y2", 0.1)));

                                    //Коробка
                                    xePages.Add(new XElement("curvesBox",
                                        new XAttribute("x1", (curveStartInd - first.X) * x_coef),
                                        new XAttribute("y1", 0.05),
                                        new XAttribute("x2", (curveFinalInd - first.X) * x_coef),
                                        new XAttribute("y2", 0.05)));
                                    //---

                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", ((curveStartInd - first.X) + curves.Elevations[0].Transition_1) * x_coef),
                                        new XAttribute("y1", 0.1),
                                        new XAttribute("x2", ((curveFinalInd - first.X) - curves.Elevations[0].Transition_2) * x_coef),
                                        new XAttribute("y2", 0.1)));

                                    //Коробка
                                    xePages.Add(new XElement("curvesBox",
                                        new XAttribute("x1", (curveFinalInd - first.X) * x_coef),
                                        new XAttribute("y1", 0.05),
                                        new XAttribute("x2", (curveFinalInd - first.X) * x_coef),
                                        new XAttribute("y2", 0.5)));
                                    //---

                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", ((curveFinalInd - first.X) - curves.Elevations[0].Transition_2) * x_coef),
                                        new XAttribute("y1", 0.1),
                                        new XAttribute("x2", (curveFinalInd - first.X) * x_coef),
                                        new XAttribute("y2", 0.5)));

                                    //длина прямого участка
                                    var text12 = curveStartInd - lastCoord;

                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", (curveStartInd - first.X - text12 / 2) * x_coef),
                                        new XAttribute("y", 0.13),
                                        new XAttribute("rotate", 0),
                                        new XAttribute("text", text12)));
                                    //-----------------

                                    string text = "R=" + Convert.ToInt32(curves.MaxRadius);
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", (curveStartInd - first.X + lenCurve) * x_coef + curves.Len * x_coef / 2 - text.Length * 0.05),
                                        new XAttribute("y", 0.3),
                                        new XAttribute("rotate", 0),
                                        new XAttribute("text", text)));

                                    text = "H=" + Convert.ToInt32(curves.MaxLvl);
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", (curveStartInd - first.X + lenCurve) * x_coef + curves.Len * x_coef / 2 - text.Length * 0.05),
                                        new XAttribute("y", 0.5),
                                        new XAttribute("rotate", 0),
                                        new XAttribute("text", text)));

                                    var curveElevStartInd = GetIndexFromKmM(tempProf, curves.Elevations[0].Start_Km, curves.Elevations[0].Start_M);
                                    var curveElevFinalInd = GetIndexFromKmM(tempProf, curves.Elevations[0].Final_Km, curves.Elevations[0].Final_M);

                                    text = "L=" + Math.Abs(curveElevStartInd - curveElevFinalInd);
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", (curveStartInd - first.X + lenCurve) * x_coef + curves.Len * x_coef / 2 - text.Length * 0.05),
                                        new XAttribute("y", 0.7),
                                        new XAttribute("rotate", 0),
                                        new XAttribute("text", text)));

                                    text = curves.Start_M.ToString();
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", -0.55 - text.Length * 0.1),
                                        new XAttribute("y", (curveStartInd - first.X) * x_coef),
                                        new XAttribute("rotate", 270),
                                        new XAttribute("text", text)));
                                    text = curves.Final_M.ToString();
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", -0.55 - text.Length * 0.1),
                                        new XAttribute("y", (curveFinalInd - first.X) * x_coef + 0.1),
                                        new XAttribute("rotate", 270),
                                        new XAttribute("text", text)));
                                }
                                else if ((Side)curves.Side_id == Side.Right)
                                {
                                    //Коробка
                                    xePages.Add(new XElement("curvesBox",
                                        new XAttribute("x1", (curveStartInd - first.X) * x_coef),
                                        new XAttribute("y1", 0.5),
                                        new XAttribute("x2", (curveStartInd - first.X) * x_coef),
                                        new XAttribute("y2", 0.95)));
                                    //---

                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", (curveStartInd - first.X) * x_coef),
                                        new XAttribute("y1", 0.5),
                                        new XAttribute("x2", (curveStartInd - first.X + curves.Elevations[0].Transition_1) * x_coef),
                                        new XAttribute("y2", 0.9)));

                                    //Коробка
                                    xePages.Add(new XElement("curvesBox",
                                        new XAttribute("x1", (curveStartInd - first.X) * x_coef),
                                        new XAttribute("y1", 0.95),
                                        new XAttribute("x2", (curveFinalInd - first.X) * x_coef),
                                        new XAttribute("y2", 0.95)));
                                    //---

                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", (curveStartInd - first.X + curves.Elevations[0].Transition_1) * x_coef),
                                        new XAttribute("y1", 0.9),
                                        new XAttribute("x2", (curveFinalInd - first.X - curves.Elevations[0].Transition_2) * x_coef),
                                        new XAttribute("y2", 0.9)));

                                    //Коробка
                                    xePages.Add(new XElement("curvesBox",
                                        new XAttribute("x1", (curveFinalInd - first.X) * x_coef),
                                        new XAttribute("y1", 0.95),
                                        new XAttribute("x2", (curveFinalInd - first.X) * x_coef),
                                        new XAttribute("y2", 0.5)));
                                    //---

                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", (curveFinalInd - first.X - curves.Elevations[0].Transition_2) * x_coef),
                                        new XAttribute("y1", 0.9),
                                        new XAttribute("x2", (curveFinalInd - first.X) * x_coef),
                                        new XAttribute("y2", 0.5)));


                                    //длина прямого участка
                                    var text12 = (curveStartInd) - lastCoord;

                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", (curveStartInd - first.X - text12 / 2) * x_coef),
                                        new XAttribute("y", 0.13),
                                        new XAttribute("rotate", 0),
                                        new XAttribute("text", text12)));
                                    //-----------------

                                    string text = "R=" + Convert.ToInt32(curves.MaxRadius);
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", (curveStartInd - first.X + lenCurve) * x_coef + curves.Len * x_coef / 2 - text.Length * 0.05),
                                        new XAttribute("y", 0.45),
                                        new XAttribute("rotate", 0),
                                        new XAttribute("text", text)));

                                    text = "H=" + Convert.ToInt32(curves.MaxLvl);
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", (curveStartInd - first.X + lenCurve) * x_coef + curves.Len * x_coef / 2 - text.Length * 0.05),
                                        new XAttribute("y", 0.65),
                                        new XAttribute("rotate", 0),
                                        new XAttribute("text", text)));

                                    var curveElevStartInd = GetIndexFromKmM(tempProf, curves.Elevations[0].Start_Km, curves.Elevations[0].Start_M);
                                    var curveElevFinalInd = GetIndexFromKmM(tempProf, curves.Elevations[0].Final_Km, curves.Elevations[0].Final_M);

                                    text = "L=" + Math.Abs(curveElevStartInd - curveElevFinalInd);
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", (curveStartInd - first.X + lenCurve) * x_coef + curves.Len * x_coef / 2 - text.Length * 0.05),
                                        new XAttribute("y", 0.85),
                                        new XAttribute("rotate", 0),
                                        new XAttribute("text", text)));

                                    text = curves.Start_M.ToString();
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", -0.45),
                                        new XAttribute("y", (curveStartInd - first.X) * x_coef),
                                        new XAttribute("rotate", 270),
                                        new XAttribute("text", text)));
                                    text = curves.Final_M.ToString();
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", -0.45),
                                        new XAttribute("y", (curveFinalInd - first.X) * x_coef + 0.1),
                                        new XAttribute("rotate", 270),
                                        new XAttribute("text", text)));
                                }
                                else
                                {
                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", (curveStartInd - first.X) * x_coef),
                                        new XAttribute("y1", 0.5),
                                        new XAttribute("x2", (curveFinalInd - first.X) * x_coef),
                                        new XAttribute("y2", 0.5)));
                                }

                                lastCoord = curveFinalInd;
                            }

                            xePages.Add(new XElement("curves",
                                new XAttribute("x1", (lastCoord - first.X) * x_coef),
                                new XAttribute("y1", 0.5),
                                new XAttribute("x2", 25),
                                new XAttribute("y2", 0.5)));
                            report.Add(xePages);
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
                htReport.Save(Path.GetTempPath() + "/report_GraphDeviationsInProfile.html");

            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_GraphDeviationsInProfile.html");
            }
        }

        private int GetIndexFromKmM(List<RdProfile> raw_rd_profile, int km, int m)
        {
            var tempData = raw_rd_profile.Where(o => o.Km == km && o.M == m).ToList();
            

            if (tempData.Any())
            {
                var ind = tempData.First().X;
                return ind;
            }
            else
            {
                if (raw_rd_profile.Last().GetRealCoordinate() < km + m / 10000.0)
                {
                    var dd = raw_rd_profile.Last().X+10;

                    //var d = raw_rd_profile.Last().X;
                    return dd;
                }
                else if (raw_rd_profile.First().GetRealCoordinate() > km + m / 10000.0)
                {
                    var dd = raw_rd_profile.First().X - 10;

                    //var d = raw_rd_profile.Last().X;
                    return dd;
                }
                else
                {
                    var KmLen = raw_rd_profile.Where(o => o.Km == km).ToList();
                    if (KmLen.Any())
                    {
                        var end = KmLen.Last().X;
                        return end;
                    }
                    else
                    {
                        return 500;
                    }
                }
            }
        }
    }
}
