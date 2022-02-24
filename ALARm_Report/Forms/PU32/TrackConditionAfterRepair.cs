using ALARm.Core;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm_Report.Forms
{
    public class TrackConditionAfterRepair : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(parentId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel) return;
                admTracksId = choiceForm.admTracksIDs;
            }

            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();

                var trips = RdStructureService.GetTripsOnDistance(parentId, period);
                if (trips.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                XElement report = new XElement("report");

                foreach (var tripProcess in trips)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        var kms = RdStructureService.GetKilometerTrip(tripProcess.Id);
                        if (kms.Count() == 0) continue;

                        XElement tripElem = new XElement("trip",
                                     new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                                     new XAttribute("direction", $"{tripProcess.Direction}({tripProcess.DirectionCode})"),
                                     new XAttribute("put", trackName),
                                     new XAttribute("pch", distance.Code),
                                     new XAttribute("trip_date", tripProcess.Trip_date.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))),
                                     new XAttribute("start", kms.Min()),
                                     new XAttribute("final", kms.Max()),
                                     new XAttribute("repairs_type", "Капитальный усиленный"),
                                     new XAttribute("road", road));

                        //1. Общие характеристики участка ремонта
                        var mainPar = MainParametersService.GetMainParametersFromDB(tripProcess.Id);
                        mainPar = mainPar.Where(o => (kms.Min() <= o.Km && o.Km <= kms.Max())).ToList();

                        tripElem.Add("Note",
                                 new XAttribute("MAX_speed", mainPar.Select(o => o.Sssp_gor).ToList().Max().ToString("0")),
                                 new XAttribute("MAX_speed_after", mainPar.Select(o => o.Sssp_gor).ToList().Max().ToString("0")),

                                 new XAttribute("cccp_max", mainPar.Select(o => o.SSSP_speed).ToList().Max().ToString("0")),
                                 new XAttribute("cccp_max_after", mainPar.Select(o => o.SSSP_speed).ToList().Max().ToString("0")));

                        //2. Характеристики кривых
                        List<Curve> curves = (MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
                        curves = curves.Where(o => (kms.Min() <= o.Start_Km && o.Final_Km <= kms.Max())).ToList();
                        int iter = 1;
                        foreach (var curve in curves)
                        {
                            List<RDCurve> rdcs = RdStructureService.GetRDCurves(curve.Id, tripProcess.Id);
                            curve.Elevations = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();
                            curve.Straightenings = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();

                            if (curve.Straightenings.Count < 1)
                                continue;

                            List<MatchCurveCoords> matchCurveCoords = new List<MatchCurveCoords>();

                            foreach (var stCurve in curve.Straightenings)
                            {
                                var stAbsCoords = MainTrackStructureService.GetDistanceBetween2Coord(curve.Start_Km, curve.Start_M, stCurve.Start_Km, stCurve.Start_M, curve.Track_Id, curve.Start_Date);
                                matchCurveCoords.Add(new MatchCurveCoords
                                {
                                    StAbsCoords = stAbsCoords
                                });

                                matchCurveCoords.Add(new MatchCurveCoords
                                {
                                    StAbsCoords = stAbsCoords + stCurve.Transition_1
                                });
                                stAbsCoords = MainTrackStructureService.GetDistanceBetween2Coord(curve.Start_Km, curve.Start_M, stCurve.Final_Km, stCurve.Final_M, curve.Track_Id, curve.Start_Date);
                                if (stCurve.Transition_2 != 0)
                                {
                                    matchCurveCoords.Add(new MatchCurveCoords
                                    {
                                        StAbsCoords = stAbsCoords - stCurve.Transition_2
                                    });

                                    matchCurveCoords.Add(new MatchCurveCoords
                                    {
                                        StAbsCoords = stAbsCoords
                                    });
                                }
                            }

                            int minX = -1;
                            int maxX = -1;
                            int width = -1;

                            Data rdcsData = new Data();
                            List<float> plan, level, gauge, passBoost, freightBoost;
                            List<int> x;
                            List<int> passSpeed, freightSpeed;
                            x = new List<int>();
                            passSpeed = new List<int>();
                            freightSpeed = new List<int>();
                            plan = new List<float>();
                            level = new List<float>();
                            gauge = new List<float>();
                            passBoost = new List<float>();
                            freightBoost = new List<float>();

                            //радиусы
                            int radiusH = 0;
                            int radiusLength = -1;
                            //уровень
                            int transitionLength1 = 0, transitionLength2 = 0;

                            foreach (var stCurve in curve.Straightenings)
                            {
                                int rH = Convert.ToInt32(17860 / stCurve.Radius);
                                radiusH = rH;

                                if (minX < 0)
                                {
                                    minX = 0;
                                    maxX = rdcs.Count();
                                    width = Math.Abs(maxX - minX);
                                    radiusLength = Convert.ToInt32(curve.Straightenings.Max(s => s.Radius));
                                    transitionLength1 = curve.Straightenings.First().Transition_1;
                                    transitionLength2 = curve.Straightenings.Last().Transition_2;
                                }
                            }
                            string radiusAverage = "";
                            string levelAverage = "";
                            string gaugeAverage = "";
                            string passboost = "";
                            string freightboost = "";

                            //если колл меньше нуля 
                            if (rdcs.Count < 1) continue;
                            foreach (var rdc in rdcs)
                            {
                                x.Add(rdcs.IndexOf(rdc));
                                plan.Add(rdc.Radius);
                                level.Add(rdc.Level);
                                gauge.Add(rdc.Gauge);
                                passBoost.Add(rdc.PassBoost);
                                freightBoost.Add(rdc.FreightBoost);
                                passSpeed.Add(rdc.PassSpeed);
                                freightSpeed.Add(rdc.FreightSpeed);
                                radiusAverage += rdc.GetRadiusCoord();
                                levelAverage += rdc.GetLevelCoord();
                                passboost += rdc.GetPassBoostCoord();
                                freightboost += rdc.GetFreightBoostCoord();
                                gaugeAverage += rdc.GetGaugeCoord();
                            }
                            rdcsData = new Data(x, plan, level, gauge, passBoost, freightBoost, passSpeed, freightSpeed, transitionLength1, transitionLength2);

                            var ProjectRadius = curve.Straightenings[0].Radius;
                            var AvgRADius = rdcsData.GetAvgPlan();
                            int DeltaR = (int)Math.Abs(ProjectRadius - AvgRADius);
                            var Lvl = curve.Elevations[0].Lvl;
                            var avgLvl = rdcsData.GetAvgLevel();
                            int DeltaH = (int)Math.Abs(Lvl - avgLvl);

                            
                            foreach (var stCurve in curve.Straightenings)
                            {
                                var crv = new XElement("Curve",
                                         new XAttribute("ind", iter),
                                         new XAttribute("start_km", stCurve.Start_Km),
                                         new XAttribute("start_m", stCurve.Start_M),
                                         new XAttribute("final_km", stCurve.Final_Km),
                                         new XAttribute("final_m", stCurve.Final_M),
                                         new XAttribute("len", Math.Abs(stCurve.Start_Km * 1000 + stCurve.Start_M - stCurve.Final_Km * 1000 - stCurve.Final_M)),
                                         new XAttribute("len2", stCurve.Transition_1),
                                         new XAttribute("len2_lvl", stCurve.Transition_2),
                                         new XAttribute("P", curve.Straightenings[0].Radius),
                                         new XAttribute("radius", rdcsData.GetAvgPlan()),
                                         new XAttribute("DeltaR", DeltaR),
                                         new XAttribute("lvl", curve.Elevations[0].Lvl),
                                         new XAttribute("lvl_mid", rdcsData.GetAvgLevel()),
                                         new XAttribute("lvl_min", rdcsData.GetMinLevel()),
                                         new XAttribute("scatter", Math.Round(DeltaR * 100 / curve.Straightenings[0].Radius)),
                                         new XAttribute("withdrawal", Math.Max(rdcsData.GetPlanLeftMaxRetractionSlope(), rdcsData.GetPlanRightMaxRetractionSlope()).ToString("0.00")),
                                         new XAttribute("DeltaH", DeltaH),
                                         new XAttribute("anp", rdcsData.GetUnliquidatedAccelerationPassengerAvg().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                         new XAttribute("psi", rdcsData.BoostChangeRateMax().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                         new XAttribute("Ocenka", "-")
                                         );
                                iter = iter + 1;

                                tripElem.Add(crv);
                            }
                        }

                        //3. Характеристики километра
                        //Продольный профиль------------------------------------------------------------------------
                        var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);
                        var raw_rd_profile = RdStructureService.GetRdTables(tripProcesses[0], 1) as List<RdProfile>;
                        //Реперные точки
                        var raw_RefPoints = MainTrackStructureService.GetRefPointsByTripIdToDate(track_id, tripProcesses[0].Date_Vrem);

                        //var filterForm = new FilterForm();
                        //var filters = new List<Filter>();

                        //var min = raw_RefPoints.Select(o => o.Km).Min();
                        //var max = raw_RefPoints.Select(o => o.Km).Max();

                        //filters.Add(new Filter() { Name = "Начало (км)", Value = min });
                        //filters.Add(new Filter() { Name = "Конец (км)", Value = max });

                        //filterForm.SetDataSource(filters);
                        //if (filterForm.ShowDialog() == DialogResult.Cancel)
                        //    return;
                        //фильтр
                        raw_RefPoints = raw_RefPoints.Where(o => (kms.Min() <= o.Km && o.Km <= kms.Max())).ToList();

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

                        var startP = rd_profile.IndexOf(rd_profile.Where(o => (kms.Min() <= o.Km && o.Km <= kms.Max())).ToList().First());
                        var finalP = rd_profile.IndexOf(rd_profile.Where(o => (kms.Min() <= o.Km && o.Km <= kms.Max())).ToList().Last());

                        var countP = finalP - startP;

                        var track_profile = rd_profile;
                        //var track_profile = rd_profile.Where(r => r.Track_id == trackId).OrderBy(r => r.X).ToList();


                        var trackId = track_id;
                        var tripDate = tripProcesses[0].Date_Vrem;

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
                        //Линеар на график профиля 0,05
                        profileData.getLinearGraphNew();

                        //----------------------------------------------------------------------------------------------

                        var mainParamRiht = MainParametersService.GetMainParametersFromDBMeter(tripProcesses[0].Trip_id);
                        mainParamRiht = mainParamRiht.Where(o =>
                                                              RefPoints.First().Km * 1000 + RefPoints.First().Meter <= o.Km * 1000 + o.Meter &&
                                                              RefPoints.Last().Km * 1000 + RefPoints.Last().Meter >= o.Km * 1000 + o.Meter
                                                              ).ToList();

                        var mainParameters = RdStructureService.GetRdTablesByKM(tripProcesses[0], RefPoints.First().Km * 1000 + RefPoints.First().Meter, RefPoints.Last().Km * 1000 + RefPoints.Last().Meter) as List<RdProfile>;
                        var mainParameters2 = RdStructureService.GetRdTablesByKM(tripProcesses[0], kms.Min() * 1000, kms.Max() * 1000) as List<RdProfile>;


                        //определяем входят ли в кривые участки
                        var GetCurvesList = MainTrackStructureService.GetCurveByTripIdToDate(tripProcesses[0]) as List<Curve>;
                        for (int jj = 0; jj < mainParameters.Count; jj++)
                        {
                            var current_coord = mainParameters[jj].Km * 1000 + mainParameters[jj].Meter;

                            var curve = GetCurvesList.Where(o => o.Start_coord <= current_coord && current_coord <= o.Final_coord).ToList();

                            mainParameters[jj].IsCurve = curve.Count() > 0 ? true : false;
                        }
                        //------------------------------------------------------------------------------------------------------------------
                        var calc_mainParameters = new List<RdProfile> { };

                        var RollAver_nerov = new List<double>();
                        for (int ii = 0; ii < mainParamRiht.Count - 1; ii++)
                        {
                            var nerov = (mainParameters[ii].IsCurve == true ? 0.0 : (mainParamRiht[ii].Stright_left - mainParamRiht[ii].Stright_avg));
                            if (RollAver_nerov.Count() >= 50)
                            {
                                RollAver_nerov.Add(nerov);
                                nerov = (RollAver_nerov.Skip(RollAver_nerov.Count() - 50).Take(50).Average());
                            }
                            else
                            {
                                RollAver_nerov.Add(nerov);
                            }

                            mainParameters[ii].Irregularities_in_plan = ((nerov * 25 * 2) * Math.Exp(-Math.Abs(nerov / 2)) + 55.0);

                            double grapH = 151.0;
                            double middleH = grapH / 2.0;
                            double y_coef = grapH / 335.0;

                            if (ii < profileData.ORIGINminusLBOplusLRrrr.Count())
                            {
                                //Девиация
                                var calcY_Dev = (y_coef * (profileData.linearPointYNew[ii] - profileData.ORIGINminusLBOplusLRrrr[ii]) * 1000.0);

                                mainParameters[ii].Profile_irregularities = calcY_Dev;
                            }
                        }

                        
                        var SSSP_speed = new List<float> { };
                        SSSP_speed.Add(mainPar.Select(o => o.SSSP_speed).ToList().Max());

                        foreach (var kilometer in kms)
                        {
                            var profile = mainParameters.Where(o => o.Km == kilometer).ToList();
                            if (profile.Count == 0) continue;

                            var mainPar2 = MainParametersService.GetMainParametersSKOvedomDB(kilometer, tripProcess.Id);
                            if (mainPar2.Count == 0) continue;

                            var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Trip_date, kilometer, MainTrackStructureConst.MtoSpeed, tripProcess.Direction, "1") as List<Speed>;
                            var Vpz = speed.Count > 0 ? speed[0].Passenger : -1;
                            var trackclasses = (List<TrackClass>)MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Trip_date, kilometer, MainTrackStructureConst.MtoTrackClass, track_id);

                            var curve_Lvl_value = new List<double> { };
                            var curve_gauge_value = new List<string> { };
                            var current_curve = curves.Where(o => o.Start_Km <= kilometer && kilometer <= o.Final_Km).ToList();
                            foreach (var item in current_curve)
                            {
                                item.Elevations = (MainTrackStructureService.GetCurves(item.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();
                                item.Straightenings = (MainTrackStructureService.GetCurves(item.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(stt => stt.RealStartCoordinate).ToList();

                                var curve_center_koord = Math.Abs((item.Elevations[0].Start_Km * 1000 + item.Elevations[0].Start_M) - (item.Elevations[0].Final_Km * 1000 + item.Elevations[0].Final_M)) / 2;
                                curve_center_koord = item.Elevations[0].Start_Km * 1000 + item.Elevations[0].Start_M + curve_center_koord;

                                if (curve_center_koord >= kilometer * 1000 && curve_center_koord < (kilometer + 1) * 1000)
                                {

                                    var nature_value = mainParameters2.Where(o => o.Km * 1000 + o.Meter == curve_center_koord).ToList();

                                    if (nature_value.Count > 0)
                                    {
                                        curve_Lvl_value.Add(item.Elevations[0].Lvl - Math.Abs(nature_value.First().Level));

                                        var g = "";
                                        g = g + (profile.Select(o => item.Straightenings[0].Width - o.Gauge).ToList().Max()).ToString("0");
                                        g = g + "/" + (profile.Select(o => item.Straightenings[0].Width - o.Gauge).ToList().Min()).ToString("0");

                                        curve_gauge_value.Add(g);
                                    }
                                    else
                                    {
                                        curve_Lvl_value.Add(profile.Select(o => o.Level).Max());
                                        curve_gauge_value.Add("");
                                    }
                                }
                            }

                            //отступления кол-во и оценка на километр
                            var two = 0;
                            var three = 0;
                            var four = 0;
                            var dig = RdStructureService.GetDigressionMarks(tripProcess.Id, track_id, kilometer);
                            foreach (var note in dig)
                            {
                                int count = note.GetCount();
                                if (count > 0)
                                {
                                    switch (note.Degree)
                                    {
                                        case 2: two += count; break;
                                        case 3: three += count; break;
                                        case 4: four++; break;
                                        default: break;
                                    }
                                }
                            }

                            var st = "хор";
                            if (two == 0 && three == 0 && four == 0)
                            {
                                st = "хор";
                            }
                            else if (two <= 5 && three == 0 && four == 0)
                            {
                                st = "уд";
                            }
                            else if (two > 5 || (three > 0 || four > 0))
                            {
                                st = "неуд";
                            }

                            if (40 < SSSP_speed.Average())
                            {
                                switch (st)
                                {
                                    case "хор": st = "хор"; break;
                                    case "уд": st = "уд"; break;
                                    case "неуд": st = "неуд"; break;
                                    default: break;
                                }
                            }
                            else if (SSSP_speed.Average() <= 40)
                            {
                                switch (st)
                                {
                                    case "хор": st = "уд"; break;
                                    case "уд": st = "уд"; break;
                                    case "неуд": st = "неуд"; break;
                                    default: break;
                                }
                            }
                            //------------------------------------------
                            progressBar.Value = kms.IndexOf(kilometer) + 1;

                            var Notes = new XElement("Notes",
                                         new XAttribute("km", kilometer),
                                         new XAttribute("MaxProfile", profile.Any() ? profile.Select(o => o.Irregularities_in_plan).Max().ToString("0.0") : "нет данных"),
                                         new XAttribute("MaxPlan", profile.Any() ? profile.Select(o => o.Profile_irregularities).Max().ToString("0.0") : "нет данных"),
                                         new XAttribute("OtklanenieUroveni", curve_Lvl_value.Any() ? curve_Lvl_value.Max().ToString("0") : $"{profile.Where(o => o.Km == kilometer).ToList().Select(o => Math.Abs(o.Level)).ToList().Max().ToString("0")}"),
                                         new XAttribute("OtklanenieShiriny", curve_gauge_value.Any() ? curve_gauge_value.Max() : $@"{profile.Where(o => o.Km == kilometer).ToList().Select(o => -1520 + o.Gauge).ToList().Max().ToString("0")}/{profile.Where(o => o.Km == kilometer).ToList().Select(o => -1520 + o.Gauge).ToList().Min().ToString("0")}"),
                                         new XAttribute("KollichestvoOtst", $"{two}/{three}/{four}"),
                                         new XAttribute("cccp", Math.Round(SSSP_speed.Average())),
                                         new XAttribute("VP", Vpz),
                                         new XAttribute("Vdop", speed.Count > 0 ? speed[0].Passenger.ToString() : ""),
                                         new XAttribute("Ocenka", st),
                                         new XAttribute("CCCPafterYear", ""));
                            tripElem.Add(Notes);
                        }
                        var original2 = profileData.getLinearGraphNew();

                        var slope_list = profileData.straightProfilesNew.Where(o => o.SlopeText != "-").ToList();
                        var slope_diff_list = profileData.straightProfilesNew.Where(o => o.SlopeDiffText != "-").ToList();
                        tripElem.Add("Note",
                                 new XAttribute("trackclasses", "1"),
                                 new XAttribute("trackclasses_after", "1"),

                                 new XAttribute("slope", slope_list.Select(o => Math.Abs(Convert.ToDouble(o.SlopeText.Replace('.', ',')))).Max()),
                                 new XAttribute("slope_after", slope_list.Select(o => Math.Abs(Convert.ToDouble(o.SlopeText.Replace('.', ',')))).Max()),

                                 new XAttribute("uklon", slope_diff_list.Select(o => Math.Abs(Convert.ToDouble(o.SlopeDiffText.Replace('.', ',')))).Max()),
                                 new XAttribute("uklon_after", slope_diff_list.Select(o => Math.Abs(Convert.ToDouble(o.SlopeDiffText.Replace('.', ',')))).Max()));

                        report.Add(tripElem);
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
