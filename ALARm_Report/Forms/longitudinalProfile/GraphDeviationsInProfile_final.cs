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
    public class GraphDeviationsInProfile_final : Report
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
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        process.TrackID = track_id;
                        process.TrackName = trackName.ToString();

                        //Продольный профиль
                        var raw_rd_profile = RdStructureService.GetRdTables(process, 1) as List<RdProfile>;
                       
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var min = raw_rd_profile.Select(o => o.Km).Min();
                        var max = raw_rd_profile.Select(o => o.Km).Max();

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = min });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = max });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;
                        //фильтр
                        raw_rd_profile = raw_rd_profile.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList();

                        var roadName = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);

                        var start = false;
                        var RefPoints = new List<RefPoint> { };
                        var rd_profile = raw_rd_profile;
                        RefPoint PREV = null;

                        
                        

                        var startP = raw_rd_profile.IndexOf(raw_rd_profile.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList().First());
                        var finalP = raw_rd_profile.IndexOf(raw_rd_profile.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList().Last());

                        var countP = finalP - startP;



                        var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                        var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                        var road = AdmStructureService.GetUnit(AdmStructureConst.AdmRoad, nod.Parent_Id) as AdmUnit;
                        if (!raw_rd_profile.Any())
                            continue;

                        var track_profile = raw_rd_profile;
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
                                x_RefPoints.Add(j+1);
                                xInd.Add(j);
                                yr.Add(elem.Y);
                            }
                            x_track_profile.Add(j + 1);

                            j++;
                        }
                        var ccccc = track_profile.Last();

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

                        var original = profileData.GraphProfile();

                        int new_original = original["new_original"].Count;
                        //int ORIGINminusLBOplusLR = original["ORIGINminusLBOplusLR"].Count;
                        //int ReperLOlinear = original["ReperLOlinear"].Count;
                        //int Dev = original["Dev"].Count;


                        for (int gi = 0; gi < new_original - 1; gi++)
                        {
                            //if (gi > 0)
                            //    break;
                            var first = rd_profile.GetRange(gi * 5000, rd_profile.Count() - gi * 5000 > 5000 ? 5000 : rd_profile.Count() - gi * 5000).First();
                            var last = rd_profile.GetRange(gi * 5000, 5000).Last();
                            int start_km = first.Km, start_m = first.M,
                            final_km = last.Km, final_m = last.M;

                            XElement xePages = new XElement("pages",
                                new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version}"),
                                new XAttribute("road", roadName),
                                 new XAttribute("period", period.Period),
                                new XAttribute("distance", distance.Code ?? ""),
                                new XAttribute("trip_info", process.GetProcessTypeName ),
                                new XAttribute("car", process.Car ?? ""),
                                new XAttribute("trip_date", process.Trip_date ?? ""),
                                new XAttribute("direction", $" { process.DirectionName}({ process.DirectionCode})"),
                                new XAttribute("track", trackName)
                            );

                            // all the calculated points 
                            //xePages.Add(

                            //    new XAttribute("deviation_graph", gi < Dev ? original["Dev"][gi] : " "),

                            //    new XAttribute("ORIGINminusLBOplusLR", gi < ORIGINminusLBOplusLR ? original["ORIGINminusLBOplusLR"][gi] : " "), // red

                            //    new XAttribute("linearReper", gi < original["new_linear"].Count ? original["new_linear"][gi] : " ") //green
                            //);

                            //profileData.StraightInfo(xePages);
                            //profileData.PicketInfo(xePages);

                            double x_coef = 25.0 / 5000; //x - km
                            int lastCoord = start_km * 1000+start_m;

                            //Реперные точки
                            var rp = RefPoints.Where(r => r.Km >= start_km && r.Km <= final_km).ToList();

                            //егер сонына репер жетпесе
                            //if(rp.Last().Km <= final_km)
                            //{
                            //    rp.Add(RefPoints[RefPoints.IndexOf(rp.Last()) + 1]);
                            //}
                            ////егер басына репер жетпесе
                            //if (rp.First().Km >= start_km)
                            //{
                            //    try
                            //    {
                            //        rp.Insert(0, RefPoints[RefPoints.IndexOf(rp.First()) - 1]);
                            //    }
                            //    catch { }
                            //}


                            //for (int i = 0; i < rp.Count; i++)
                            //{
                            //    var coord = ((rp[i].Km * 1000 + rp[i].Meter) - (start_km * 1000 + start_m)) * x_coef;
                            //    try
                            //    {
                            //        xePages.Add(new XElement("RefPoint",
                            //            //Метка
                            //            new XAttribute("xr", $"{(coord - 0.11).ToString("0.####").Replace(',', '.')}cm"), //0.131 - это длина метки
                            //            new XAttribute("yr", profileData.ReperY[RefPoints.IndexOf(rp[i])] - 10) //10 - это высота метки
                            //            ));
                            //    }
                            //    catch(Exception e) 
                            //    { 
                            //        Console.WriteLine($"new XElement(RefPoint, {e.Message}"); 
                            //    }

                            //    //value метки
                            //    xePages.Add(new XElement("straights_value",
                            //        new XAttribute("x", -0.9),
                            //        new XAttribute("y", $"{(coord + 0.075 < 0.15 ? 0.15 : coord + 0.075).ToString("0.####").Replace(',', '.')}"),
                            //        new XAttribute("rotate", 270),
                            //        new XAttribute("text", rp[i].Mark)));

                            //    //90 гр линия разделитель
                            //    xePages.Add(new XElement("straights",
                            //        new XAttribute("x1", coord),
                            //        new XAttribute("x2", coord),
                            //        new XAttribute("y1", 1),
                            //        new XAttribute("y2", 2)));

                            //    //между разделителями
                            //    var lineST = false;
                            //    var prev_lineST = false;
                            //    try
                            //    {
                            //        var up = false;
                            //        var down = false;

                            //        var stt = (rp[i].Km * 1000 + rp[i].Meter) - (start_km * 1000 + start_m);
                            //        var cnn = (rp[i + 1].Km * 1000 + rp[i + 1].Meter) - (rp[i].Km * 1000 + rp[i].Meter);

                            //        var gdY = profileData.ORIGINminusLBOplusLRrrr.GetRange(stt < 0 ? 0 : stt, cnn).ToList();
                            //        var rdY = profileData.linearPointYReper.GetRange(stt < 0 ? 0 : stt, cnn).ToList();

                            //        var gdX = profileData.XX.GetRange(stt < 0 ? 0 : stt, cnn).ToList();

                            //        for (int qq = 0; qq < gdY.Count; qq++)
                            //        {
                            //            var dev = rdY[qq] - gdY[qq];

                            //            if (up == true && down == false)
                            //            {
                            //                if (dev < -0.01)
                            //                {
                            //                    lineST = true;
                            //                    break;
                            //                }
                            //            }
                            //            if (up == false && down == true)
                            //            {
                            //                if (dev > 0.01)
                            //                {
                            //                    lineST = true;
                            //                    break;
                            //                }
                            //            }

                            //            if (up == false && down == false)
                            //            {
                            //                if (dev < 0)
                            //                    down = true;
                            //                else if (dev > 0)
                            //                    up = true;
                            //            }
                            //        }
                            //        if (lineST == false)
                            //        {
                            //            for (int qq = 0; qq < gdY.Count; qq++)
                            //            {
                            //                var y = (Math.Pow(Math.Abs(gdX[qq] - gdX.First()), 0.5) * Math.Pow(Math.Abs(gdX[qq] - gdX.Last()), 0.5)) / (Math.Pow((Math.Abs(gdX.First() - gdX.Last())) / 2, 1));
                            //                if (up == true)
                            //                {
                            //                    xePages.Add(new XElement("semicircle_line",
                            //                        new XAttribute("xr", $"{(gdX[qq]).ToString("0.####").Replace(',', '.')}"),
                            //                        new XAttribute("yr", y * 37 + 37.9)
                            //                    ));
                            //                }
                            //                if (down == true)
                            //                {
                            //                    xePages.Add(new XElement("semicircle_line",
                            //                        new XAttribute("xr", $"{(gdX[qq]).ToString("0.####").Replace(',', '.')}"),
                            //                        new XAttribute("yr", -y * 37 + 37.9 + 37.9)
                            //                    ));
                            //                }
                            //            }

                            //            //радиус
                            //            var x1 = gdX.First();
                            //            var y1 = gdY.First();

                            //            var x2 = gdX[gdX.Count / 2];
                            //            var y2 = gdY[gdX.Count / 2];

                            //            var x3 = gdX.Last();
                            //            var y3 = gdY.Last();

                            //            var A = x2 - x1;
                            //            var B = y2 - y1;
                            //            var C = x3 - x1;
                            //            var D = y3 - y1;
                            //            var E = A * (x1 + x2) + B * (y1 + y2);
                            //            var F = C * (x1 + x3) + D * (y1 + y3);

                            //            var G = 2 * (A * (y3 - y2) - B * (x3 - x2));
                            //            //Если G = 0, это значит, что через данный набор точек провести окружность нельзя.
                            //            // координаты центра
                            //            var Cx = (D * E - B * F) / G;
                            //            var Cy = (A * F - C * E) / G;

                            //            // радиус
                            //            var R = Math.Sqrt(Math.Pow(x1 - Cx, 2) + Math.Pow(y1 - Cy, 2)).ToString("0");
                            //            var len = (rp[i + 1].Km * 1000 + rp[i + 1].Meter) - (rp[i].Km * 1000 + rp[i].Meter);

                            //            xePages.Add(new XElement("straights_value",
                            //                    new XAttribute("x", (((rp[i].Km * 1000 + rp[i].Meter) - (start_km * 1000 + start_m)) * x_coef +0.01)),
                            //                    new XAttribute("y", 1.2),
                            //                    new XAttribute("rotate", 0),
                            //                    new XAttribute("text", R)
                            //                    ));
                            //            xePages.Add(new XElement("straights_value",
                            //                    new XAttribute("x", (((rp[i].Km * 1000 + rp[i].Meter) - (start_km * 1000 + start_m)) * x_coef+0.01)),
                            //                    new XAttribute("y", 1.4),
                            //                    new XAttribute("rotate", 0),
                            //                    new XAttribute("text", len)
                            //                    ));
                            //        }
                            //        if (lineST == true)
                            //        {
                            //            if (rp[i].Mark < rp[i + 1].Mark)
                            //            {
                            //                xePages.Add(new XElement("straights",
                            //                    new XAttribute("x1", ((rp[i].Km * 1000 + rp[i].Meter) - (start_km * 1000 + start_m)) * x_coef),
                            //                    new XAttribute("x2", ((rp[i + 1].Km * 1000 + rp[i + 1].Meter) - (start_km * 1000 + start_m)) * x_coef),
                            //                    new XAttribute("y1", 2),
                            //                    new XAttribute("y2", 1)
                            //                    ));

                            //                //величины уклона ---
                            //                var uklon = (rp[i + 1].Mark - rp[i].Mark) * 1000 / ((rp[i + 1].Km * 1000 + rp[i + 1].Meter) - (rp[i].Km * 1000 + rp[i].Meter));
                            //                xePages.Add(new XElement("straights_value",
                            //                    new XAttribute("x", (((rp[i].Km * 1000 + rp[i].Meter) - (start_km * 1000 + start_m)) * x_coef) + 0.01),
                            //                    new XAttribute("y", 1.2),
                            //                    new XAttribute("rotate", 0),
                            //                    new XAttribute("text", uklon.ToString("0.00"))
                            //                    ));
                            //                //---
                            //                xePages.Add(new XElement("straights_value",
                            //                    new XAttribute("x", (((rp[i + 1].Km * 1000 + rp[i + 1].Meter) - (start_km * 1000 + start_m)) * x_coef) - 0.5),
                            //                    new XAttribute("y", 1.95),
                            //                    new XAttribute("rotate", 0),
                            //                    new XAttribute("text", (rp[i + 1].Km * 1000 + rp[i + 1].Meter) - (rp[i].Km * 1000 + rp[i].Meter))
                            //                    ));

                            //            }
                            //            else if (rp[i].Mark > rp[i + 1].Mark)
                            //            {
                            //                xePages.Add(new XElement("straights",
                            //                    new XAttribute("x1", ((rp[i].Km * 1000 + rp[i].Meter) - (start_km * 1000 + start_m)) * x_coef),
                            //                    new XAttribute("x2", ((rp[i + 1].Km * 1000 + rp[i + 1].Meter) - (start_km * 1000 + start_m)) * x_coef),
                            //                    new XAttribute("y1", 1),
                            //                    new XAttribute("y2", 2)
                            //                ));

                            //                //величины уклона ---
                            //                var uklon = (rp[i + 1].Mark - rp[i].Mark) * 1000 / ((rp[i + 1].Km * 1000 + rp[i + 1].Meter) - (rp[i].Km * 1000 + rp[i].Meter));

                            //                xePages.Add(new XElement("straights_value",
                            //                    new XAttribute("x", (((rp[i + 1].Km * 1000 + rp[i + 1].Meter) - (start_km * 1000 + start_m)) * x_coef) - 0.5),
                            //                    new XAttribute("y", 1.2),
                            //                    new XAttribute("rotate", 0),
                            //                    new XAttribute("text", uklon.ToString("0.00"))
                            //                ));
                            //                xePages.Add(new XElement("straights_value",
                            //                    new XAttribute("x", (((rp[i].Km * 1000 + rp[i].Meter) - (start_km * 1000 + start_m)) * x_coef) + 0.01),
                            //                    new XAttribute("y", 1.95),
                            //                    new XAttribute("rotate", 0),
                            //                    new XAttribute("text", (rp[i + 1].Km * 1000 + rp[i + 1].Meter) - (rp[i].Km * 1000 + rp[i].Meter)
                            //                    )));

                            //            }
                            //            else if (rp[i].Mark == rp[i + 1].Mark)
                            //            {
                            //                xePages.Add(new XElement("straights",
                            //                    new XAttribute("x1", ((rp[i].Km * 1000 + rp[i].Meter) - (start_km * 1000 + start_m)) * x_coef),
                            //                    new XAttribute("x2", ((rp[i + 1].Km * 1000 + rp[i + 1].Meter) - (start_km * 1000 + start_m)) * x_coef),
                            //                    new XAttribute("y1", 1.5),
                            //                    new XAttribute("y2", 1.5)
                            //                    ));
                            //                xePages.Add(new XElement("straights_value",
                            //                    new XAttribute("x", (((rp[i].Km * 1000 + rp[i].Meter) - (start_km * 1000 + start_m)) * x_coef) + 0.01),
                            //                    new XAttribute("y", 1.2),
                            //                    new XAttribute("rotate", 0),
                            //                    new XAttribute("text", "")
                            //                    ));
                            //                xePages.Add(new XElement("straights_value",
                            //                    new XAttribute("x", (((rp[i].Km * 1000 + rp[i].Meter) - (start_km * 1000 + start_m)) * x_coef) + 0.01),
                            //                    new XAttribute("y", 1.95),
                            //                    new XAttribute("rotate", 0),
                            //                    new XAttribute("text", (rp[i + 1].Km * 1000 + rp[i + 1].Meter) - (rp[i].Km * 1000 + rp[i].Meter)
                            //                    )));

                            //            }
                            //        }
                            //    }
                            //    catch (Exception e)
                            //    {
                            //        Console.WriteLine($"между разделителями {e.Message }");
                            //        prev_lineST = lineST;
                            //    }
                            //}

                            //Километры и пикеты
                            var CurrentObj = rd_profile.GetRange(gi == 0 ? gi* 5000 : gi * 5000 - 200, 5400);
                            var prevKm = -1;
                            var prevPt = -1;
                            double prevValue = profileData.ORIGINminusLBOplusLRrrr[0];
                            var prevCoord = CurrentObj[0];

                            foreach (var obj in CurrentObj)
                            {
                                if (prevKm!= obj.Km)
                                {
                                    xePages.Add(new XElement("Kms",
                                        new XAttribute("x",
                                            $"{(((obj.Km * 1000 + obj.Meter) - (start_km * 1000 + start_m)) * x_coef).ToString("0.####").Replace(',', '.')}cm"),
                                        new XAttribute("txt", obj.Km)
                                    ));
                                }
                                prevKm = obj.Km;

                                var CurrentPt = (obj.Meter) / 100 ;

                                if ( prevPt != CurrentPt )
                                {
                                    //value по пикетам Высота
                                    xePages.Add(new XElement("pickets_value",
                                        new XAttribute("x", -2.98),

                                        //между пикетами линия
                                        new XAttribute("x1", (prevValue - profileData.ORIGINminusLBOplusLRrrr[CurrentObj.IndexOf(obj)]) > 0 ? 4 : 3),
                                        new XAttribute("x2", (prevValue - profileData.ORIGINminusLBOplusLRrrr[CurrentObj.IndexOf(obj)]) > 0 ? 3 : 4),

                                        new XAttribute("y", $"{(((obj.Km * 1000 + obj.Meter) - (start_km * 1000 + start_m)) * x_coef).ToString("0.####").Replace(',', '.')}"),
                                        new XAttribute("y_prev", $"{(((prevCoord.Km * 1000 + prevCoord.Meter) - (start_km * 1000 + start_m)) * x_coef).ToString("0.####").Replace(',', '.')}"),

                                        new XAttribute("yP", $"{(((obj.Km * 1000 + obj.Meter) - (start_km * 1000 + start_m)) * x_coef - 0.15-0.15).ToString("0.####").Replace(',', '.')}"),
                                        new XAttribute("yR", $"{(((obj.Km * 1000 + obj.Meter) - (start_km * 1000 + start_m)) * x_coef - 0.325).ToString("0.####").Replace(',', '.')}"),
                                        new XAttribute("text", CurrentPt == 0
                                                                        ? "" : profileData.ORIGINminusLBOplusLRrrr[CurrentObj.IndexOf(obj)].ToString("0.##").Replace(',', '.')),
                                        new XAttribute("razn", CurrentPt == 0
                                                                        ? "" : ((profileData.ORIGINminusLBOplusLRrrr[CurrentObj.IndexOf(obj)] - prevValue)*10.0).ToString("0.#").Replace(',', '.')
                                        )));

                                    prevValue = profileData.ORIGINminusLBOplusLRrrr[CurrentObj.IndexOf(obj)];

                                    prevCoord = obj;

                                    prevPt = CurrentPt;

                                    if (CurrentPt == 0 || CurrentPt == 10) continue;

                                    xePages.Add(new XElement("Pikets",
                                        new XAttribute("x",
                                            $"{(((obj.Km * 1000 + obj.Meter) - (start_km * 1000 + start_m)) * x_coef).ToString("0.####").Replace(',', '.')}cm"),
                                        new XAttribute("txt", CurrentPt)
                                    ));
                                }
                                prevPt = CurrentPt;
                            }

                            List<Curve> profileCurves =
                            (RdStructureService.GetRdProfileObjects(trackId, tripDate, 0, start_km, start_m, final_km, final_m) as List<Curve>).OrderBy(c => c.RealStartCoordinate).ToList();

                            Console.Out.WriteLine("curve count 2 draw ");

                            foreach (var curves in profileCurves)
                            {
                                curves.Elevations = (MainTrackStructureService.GetCurves(curves.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();

                                xePages.Add(new XElement("curves",
                                    new XAttribute("x1", (lastCoord - (start_km * 1000 + start_m)) * x_coef),
                                    new XAttribute("y1", 0.5),
                                    new XAttribute("x2", (curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m)) * x_coef),
                                    new XAttribute("y2", 0.5)));

                                var lenCurve = Math.Abs((curves.Start_Km * 1000 + curves.Start_M) - (curves.Final_Km * 1000 + curves.Final_M)) / 2;

                                if ((Side)curves.Side_id == Side.Left)
                                {
                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", (curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m)) * x_coef),
                                        new XAttribute("y1", 0.5),
                                        new XAttribute("x2", ((curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m)) + curves.Elevations[0].Transition_1) * x_coef),
                                        new XAttribute("y2", 0.1)));

                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", ((curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m)) + curves.Elevations[0].Transition_1) * x_coef),
                                        new XAttribute("y1", 0.1),
                                        new XAttribute("x2", ((curves.Final_Km * 1000 + curves.Final_M - (start_km * 1000 + start_m)) - curves.Elevations[0].Transition_2) * x_coef),
                                        new XAttribute("y2", 0.1)));

                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", ((curves.Final_Km * 1000 + curves.Final_M - (start_km * 1000 + start_m)) - curves.Elevations[0].Transition_2) * x_coef),
                                        new XAttribute("y1", 0.1),
                                        new XAttribute("x2", (curves.Final_Km * 1000 + curves.Final_M - (start_km * 1000 + start_m)) * x_coef),
                                        new XAttribute("y2", 0.5)));

                                    string text = "R=" + Convert.ToInt32(curves.MaxRadius);
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", (curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m) + lenCurve) * x_coef + curves.Len * x_coef / 2 - text.Length * 0.05),
                                        new XAttribute("y", 0.3),
                                        new XAttribute("rotate", 0),
                                        new XAttribute("text", text)));
                                    text = "H=" + Convert.ToInt32(curves.MaxLvl);
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", (curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m) + lenCurve) * x_coef + curves.Len * x_coef / 2 - text.Length * 0.05),
                                        new XAttribute("y", 0.5),
                                        new XAttribute("rotate", 0),
                                        new XAttribute("text", text)));

                                    text = curves.Start_M.ToString();
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", -0.55 - text.Length * 0.1),
                                        new XAttribute("y", (curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m)) * x_coef),
                                        new XAttribute("rotate", 270),
                                        new XAttribute("text", text)));
                                    text = curves.Final_M.ToString();
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", -0.55 - text.Length * 0.1),
                                        new XAttribute("y", (curves.Final_Km * 1000 + curves.Final_M - (start_km * 1000 + start_m)) * x_coef + 0.1),
                                        new XAttribute("rotate", 270),
                                        new XAttribute("text", text)));
                                }
                                else if ((Side)curves.Side_id == Side.Right)
                                {
                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", (curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m)) * x_coef),
                                        new XAttribute("y1", 0.5),
                                        new XAttribute("x2", ((curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m)) +curves.Elevations[0].Transition_1) * x_coef),
                                        new XAttribute("y2", 0.9)));

                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", ((curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m)) + curves.Elevations[0].Transition_1) * x_coef),
                                        new XAttribute("y1", 0.9),
                                        new XAttribute("x2", ((curves.Final_Km * 1000 + curves.Final_M - (start_km * 1000 + start_m)) - curves.Elevations[0].Transition_2) * x_coef),
                                        new XAttribute("y2", 0.9)));

                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", ((curves.Final_Km * 1000 + curves.Final_M - (start_km * 1000 + start_m)) - curves.Elevations[0].Transition_2) * x_coef),
                                        new XAttribute("y1", 0.9),
                                        new XAttribute("x2", (curves.Final_Km * 1000 + curves.Final_M - (start_km * 1000 + start_m)) * x_coef),
                                        new XAttribute("y2", 0.5)));


                                    

                                    string text = "R=" + Convert.ToInt32(curves.MaxRadius);
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", (curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m) + lenCurve) * x_coef + curves.Len * x_coef / 2 - text.Length * 0.05),
                                        new XAttribute("y", 0.65),
                                        new XAttribute("rotate", 0),
                                        new XAttribute("text", text)));
                                    text = "H=" + Convert.ToInt32(curves.MaxLvl);
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", (curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m) + lenCurve) * x_coef + curves.Len * x_coef / 2 - text.Length * 0.05),
                                        new XAttribute("y", 0.85),
                                        new XAttribute("rotate", 0),
                                        new XAttribute("text", text)));

                                    text = curves.Start_M.ToString();
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", -0.45),
                                        new XAttribute("y", (curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m)) * x_coef),
                                        new XAttribute("rotate", 270),
                                        new XAttribute("text", text)));
                                    text = curves.Final_M.ToString();
                                    xePages.Add(new XElement("curves_texts",
                                        new XAttribute("x", -0.45),
                                        new XAttribute("y", (curves.Final_Km * 1000 + curves.Final_M - (start_km * 1000 + start_m)) * x_coef + 0.1),
                                        new XAttribute("rotate", 270),
                                        new XAttribute("text", text)));
                                }
                                else
                                {
                                    xePages.Add(new XElement("curves",
                                        new XAttribute("x1", (curves.Start_Km * 1000 + curves.Start_M - (start_km * 1000 + start_m)) * x_coef),
                                        new XAttribute("y1", 0.5),
                                        new XAttribute("x2", (curves.Final_Km * 1000 + curves.Final_M - (start_km * 1000 + start_m)) * x_coef),
                                        new XAttribute("y2", 0.5)));
                                }

                                lastCoord = curves.Final_Km * 1000 + curves.Final_M;
                            }

                            xePages.Add(new XElement("curves",
                                new XAttribute("x1", (lastCoord - (start_km * 1000 + start_m)) * x_coef),
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
    }
}
