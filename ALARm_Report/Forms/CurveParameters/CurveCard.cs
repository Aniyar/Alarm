using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Core.AdditionalParameteres;
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
using ElCurve = ALARm.Core.ElCurve;


namespace ALARm_Report.Forms
{
    public class CurveCard : Report
    {
        /// <summary>
        /// Округление до кратному пяти
        /// </summary>
        /// <param name="num">координата в метрах</param>
        /// <returns>вощвращает координату в метрах кратному пяти</returns>
        private int RoundNum(int num)
        {
            int rem = num % 10;
            return rem >= 5 ? (num - rem + 10) : (num - rem);
        }

        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
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
                var tripProcesses = RdStructureService.GetMainParametersProcesses(period, parentId, true);
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report", new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                                                         new XAttribute("distance", ((AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId)).Code));
                int i = 0;
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {

                        var trips = RdStructureService.GetTrips();
                        var tr = trips.Where(t => t.Id == tripProcess.Trip_id).ToList().First();

                        
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);

                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        string[] subs = tripProcess.DirectionName.Split('(');

                        if (kilometers.Count == 0) continue;

                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var lkm = kilometers.Select(o => o.Number).ToList();

                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        //filters.Add(new FloatFilter() { Name = "Начало (км)", Value = 129 });
                        //filters.Add(new FloatFilter() { Name = "Конец (км)", Value = 133 });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        List<Curve> curves = RdStructureService.GetCurvesInTrip(tripProcess.Trip_id) as List<Curve>;
                        //фильтр по выбранным км
                        var filter_curves = curves.Where(o => ((float)(float)filters[0].Value <= o.Start_Km && o.Final_Km <= (float)(float)filters[1].Value)).ToList();
                       

                        
                        foreach (var curve in filter_curves)
                        {
                            List<RDCurve> rdcs = RdStructureService.GetRDCurves(curve.Id, trip.Id);
                            if (rdcs.First().Track_Id != track_id)
                            {
                                continue;
                            }
                            if (rdcs.First().Km != rdcs.Last().Km)
                            {
                                Random r = new Random();
                                bool foundgap = false;
                                int gapRadStart = -1, gapRadFin = -1;
                                for (int j = 0; j < rdcs.Count - 1; j++)
                                {
                                    if (rdcs[j].PassBoost != 0 && rdcs[j + 1].PassBoost == 0)
                                    {
                                        foundgap = true;
                                        gapRadStart = j;
                                    }
                                    if (foundgap && rdcs[j].PassBoost == 0 && rdcs[j + 1].PassBoost != 0)
                                    {
                                        gapRadFin = j + 1;
                                    }
                                }
                                if (gapRadStart != -1 && gapRadFin != -1)
                                {
                                    for (int j = gapRadStart + 1; j < gapRadFin; j++)
                                    {
                                        float randfluc = (float)(r.NextDouble() * 0.1 - 0.05);
                                        rdcs[j].PassBoost = rdcs[gapRadStart].PassBoost + (j - gapRadStart) * (rdcs[gapRadFin].PassBoost - rdcs[gapRadStart].PassBoost) / (gapRadFin - gapRadStart) + randfluc;
                                        rdcs[j].FreightBoost = rdcs[gapRadStart].FreightBoost + (j - gapRadStart) * (rdcs[gapRadFin].FreightBoost - rdcs[gapRadStart].FreightBoost) / (gapRadFin - gapRadStart) + randfluc;
                                    }
                                }

                            }
                            var curve_center_ind = rdcs.Count / 2;
                            var rightCurve = new List<RDCurve>();
                            var leftCurve = new List<RDCurve>();
                            //басын аяғын тауып алу Рихтовка
                            for (int cInd = curve_center_ind; cInd < rdcs.Count; cInd++)
                            {
                                rightCurve.Add(rdcs[cInd]);
                                if (Math.Abs(rdcs[cInd].Trapez_str) < 0.1)
                                    break;
                            }
                            for (int cInd = curve_center_ind; cInd > 0; cInd--)
                            {
                                leftCurve.Add(rdcs[cInd]);
                                if (Math.Abs(rdcs[cInd].Trapez_str) < 0.1)
                                    break;
                            }
                            var strData = rdcs.Where(o => leftCurve.Last().X <= o.X && o.X <= rightCurve.Last().X).ToList();

                            //кривойдан баска жерлерды тазалау
                            for (int clearInd = 0; clearInd < rdcs.Count; clearInd++)
                            {
                                if (rdcs[clearInd].X < leftCurve.Last().X)
                                {
                                    rdcs[clearInd].Trapez_str = 0;
                                    rdcs[clearInd].Avg_str = 0;
                                }
                                if (rdcs[clearInd].X > rightCurve.Last().X)
                                {
                                    rdcs[clearInd].Trapez_str = 0;
                                    rdcs[clearInd].Avg_str = 0;
                                }
                            }

                            // трапециядан туынды алу
                            for (int fi = 0; fi < strData.Count - 4; fi++)
                            {
                                var temp = Math.Abs(strData[fi + 4].Trapez_str - strData[fi].Trapez_str);
                                strData[fi].FiList = temp;
                            }
                            //накты вершиналарды табу
                            var vershList = new List<List<RDCurve>>();
                            var perehod = new List<RDCurve>();
                            var krug = new List<RDCurve>();

                            var flagPerehod = true;
                            var flagKrug = false;

                            for (int versh = 3; versh < strData.Count - 4; versh++)
                            {
                                if (strData[versh].FiList > 0.01 && flagPerehod)
                                {
                                    perehod.Add(strData[versh]);
                                }
                                else if (strData[versh].FiList < 0.01)
                                {
                                    if (perehod.Any())
                                    {
                                        vershList.Add(perehod);
                                        perehod = new List<RDCurve>();
                                        krug = new List<RDCurve>();

                                        flagPerehod = false;
                                        flagKrug = true;
                                    }
                                }

                                if (strData[versh].FiList < 0.01 && flagKrug)
                                {
                                    krug.Add(strData[versh]);
                                }
                                else if (strData[versh].FiList> 0.1)
                                {
                                    if (krug.Any())
                                    {
                                        vershList.Add(krug);
                                        perehod = new List<RDCurve>();
                                        krug = new List<RDCurve>();

                                        flagPerehod = true;
                                        flagKrug = false;
                                    }
                                }
                            }
                            if (perehod.Any())
                            {
                                vershList.Add(perehod);
                            }

                            var StrPoins = new List<RDCurve>();

                            foreach (var item in vershList)
                            {
                                StrPoins.Add(item.First());
                            }
                            StrPoins.Add(strData.Last());


                            curve_center_ind = rdcs.Count / 2;
                            var rightCurveLvl = new List<RDCurve>();
                            var leftCurveLvl = new List<RDCurve>();
                            //басын аяғын тауып алу Уровень
                            for (int cInd = curve_center_ind; cInd < rdcs.Count; cInd++)
                            {
                                rightCurveLvl.Add(rdcs[cInd]);
                                if (Math.Abs(rdcs[cInd].Trapez_level) < 0.1)
                                    break;
                            }
                            for (int cInd = curve_center_ind; cInd > 0; cInd--)
                            {
                                leftCurveLvl.Add(rdcs[cInd]);
                                if (Math.Abs(rdcs[cInd].Trapez_level) < 0.1)
                                    break;
                            }
                            var LvlData = rdcs.Where(o => leftCurveLvl.Last().X <= o.X && o.X <= rightCurveLvl.Last().X).ToList();
                            //кривойдан баска жерлерды тазалау
                            for (int clearInd = 0; clearInd < rdcs.Count; clearInd++)
                            {
                                if (rdcs[clearInd].X < leftCurveLvl.Last().X)
                                {
                                    rdcs[clearInd].Trapez_level = 0;
                                    rdcs[clearInd].Avg_level = 0;
                                    rdcs[clearInd].Level = 0;

                                    rdcs[clearInd].PassBoost_anp = 0;
                                    rdcs[clearInd].PassBoost = 0;
                                    rdcs[clearInd].FreightBoost_anp = 0;
                                    rdcs[clearInd].FreightBoost = 0;
                                }
                                if (rdcs[clearInd].X > rightCurveLvl.Last().X)
                                {
                                    rdcs[clearInd].Trapez_level = 0;
                                    rdcs[clearInd].Avg_level = 0;
                                    rdcs[clearInd].Level = 0;

                                    rdcs[clearInd].PassBoost = 0;
                                    rdcs[clearInd].PassBoost_anp = 0;
                                    rdcs[clearInd].FreightBoost = 0;
                                    rdcs[clearInd].FreightBoost_anp = 0;
                                }
                            }

                            // трапециядан туынды алу
                            for (int fi = 0; fi < LvlData.Count - 4; fi++)
                            {
                                var temp = Math.Abs(LvlData[fi + 4].Trapez_level - LvlData[fi].Trapez_level);
                                LvlData[fi].FiList2 = temp;
                            }
                            //накты вершиналарды табу
                            var vershListLVL = new List<List<RDCurve>>();
                            var perehodLVL = new List<RDCurve>();
                            var krugLVL = new List<RDCurve>();

                            var flagPerehodLVL = true;
                            var flagKrugLVL = false;

                            for (int versh = 3; versh < LvlData.Count - 4; versh++)
                            {
                                if (LvlData[versh].FiList2 > 0.1 && flagPerehodLVL)
                                {
                                    perehodLVL.Add(LvlData[versh]);
                                }
                                else
                                {
                                    if (perehodLVL.Any())
                                    {
                                        vershListLVL.Add(perehodLVL);
                                        perehodLVL = new List<RDCurve>();
                                        krugLVL = new List<RDCurve>();

                                        flagPerehodLVL = false;
                                        flagKrugLVL = true;
                                    }
                                }

                                if (LvlData[versh].FiList2 < 0.1 && flagKrugLVL)
                                {
                                    krugLVL.Add(LvlData[versh]);
                                }
                                else
                                {
                                    if (krugLVL.Any())
                                    {
                                        vershListLVL.Add(krugLVL);
                                        perehodLVL = new List<RDCurve>();
                                        krugLVL = new List<RDCurve>();

                                        flagPerehodLVL = true;
                                        flagKrugLVL = false;
                                    }
                                }
                            }
                            if (perehodLVL.Any())
                            {
                                vershListLVL.Add(perehodLVL);
                            }

                            var LevelPoins = new List<RDCurve>();

                            foreach (var item in vershListLVL)
                            {
                                LevelPoins.Add(item.First());
                            }
                            LevelPoins.Add(LvlData.Last());



                            if (StrPoins.Count < 4)
                                continue;
                            if (LevelPoins.Count < 4)
                                continue;

                            var LevelMax = rdcs.Select(o => o.Trapez_level).ToList();
                            var StrMax = rdcs.Select(o => o.Trapez_str).ToList();

                            CurveId = curve.Id;
                            string radiusPolyline = string.Empty, levelPolyline = string.Empty;

                            curve.Elevations = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();
                            curve.Straightenings = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();


                            var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, curve.Start_Km, MainTrackStructureConst.MtoSpeed, subs.Any() ? subs.First() : "", trackName.ToString()) as List<Speed>;

                            //if (speed.Any() && speed.First().Lastochka > 100)
                            //    continue;

                            if (!curve.Straightenings.Any())
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

                            int minX = -1, maxX = -1, width = -1;

                            Data rdcsData = new Data();
                            List<int> x = new List<int>();
                            List<int> passSpeed = new List<int>();
                            List<int> freightSpeed = new List<int>();
                            List<float> plan = new List<float>();
                            List<float> level = new List<float>();
                            List<float> gauge = new List<float>();
                            List<float> passBoost = new List<float>();
                            List<float> freightBoost = new List<float>();

                            //радиусы
                            int radiusH = 0;
                            int radiusLength = -1;
                            //уровень
                            int levelH = 0;
                            int transitionLength1 = 0, transitionLength2 = 0;

                            radiusPolyline = "-50 0";
                            levelPolyline = "-50 0";

                            //y=15 болғандағы x мәні
                            var DxRad = 0;
                            var Level_Sdvig = 15.0;

                            //пасспорт
                            int x1R = 0;
                            int x2R = 0;
                            int x3R = 0;
                            int x4R = 0;

                            foreach (var stCurve in curve.Straightenings)
                            {
                                x1R = MainTrackStructureService.GetDistanceBetween2Coord(curve.Start_Km, curve.Start_M, stCurve.Start_Km, stCurve.Start_M, curve.Track_Id, curve.Start_Date) + 50;
                                x2R = x1R + stCurve.Transition_1;
                                int rH = Convert.ToInt32(17860 / stCurve.Radius);

                                if (stCurve.Radius > 1000 && stCurve.Radius < 1500)
                                {
                                    Level_Sdvig /= 2.0;
                                    DxRad = (int)((Math.Abs(x2R - x1R) * Level_Sdvig) / rH);
                                }
                                else if (stCurve.Radius > 1500)
                                {
                                    Level_Sdvig /= 3.0;
                                    DxRad = (int)((Math.Abs(x2R - x1R) * Level_Sdvig) / rH);
                                }
                                else if (stCurve.Radius <= 1000)
                                {
                                    DxRad = (int)((Math.Abs(x2R - x1R) * Level_Sdvig) / rH);
                                }

                                radiusPolyline += ", " + x1R + " " + -radiusH;
                                radiusPolyline += ", " + x2R + " " + -rH;

                                if (stCurve.Transition_2 != 0)
                                {
                                    x4R = MainTrackStructureService.GetDistanceBetween2Coord(curve.Start_Km, curve.Start_M, stCurve.Final_Km, stCurve.Final_M, curve.Track_Id, curve.Start_Date) + 50;
                                    x3R = x4R - stCurve.Transition_2;

                                    radiusPolyline += ", " + x3R + " " + -rH;
                                    radiusPolyline += ", " + x4R + " 0";
                                }

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

                            foreach (var elCurve in curve.Elevations)
                            {
                                int x1 = MainTrackStructureService.GetDistanceBetween2Coord(curve.Start_Km, curve.Start_M, elCurve.Start_Km, elCurve.Start_M, curve.Track_Id, curve.Start_Date) + 50;
                                int x2 = x1 + elCurve.Transition_1;
                                int lH = Convert.ToInt32(Math.Abs(elCurve.Lvl));

                                levelPolyline += ", " + x1 + " " + -levelH;
                                levelPolyline += ", " + x2 + " " + -lH;

                                if (elCurve.Transition_2 != 0)
                                {
                                    int x3 = MainTrackStructureService.GetDistanceBetween2Coord(curve.Start_Km, curve.Start_M, elCurve.Final_Km, elCurve.Final_M, curve.Track_Id, curve.Start_Date) + 50;
                                    int x4 = x3 - elCurve.Transition_2;

                                    levelPolyline += ", " + x4 + " " + -lH;
                                    levelPolyline += ", " + x3 + " 0";
                                }

                                levelH = lH;
                            }

                            //Аг пасс
                            var AgPass = new List<double> { };
                            var VPass = speed.Any() ? speed.First().Passenger : 80;
                            var VFreig = speed.Any() ? speed.First().Freight : 80;
                            //for (int s = 0; s < rdcs.Count; s++)
                            //{
                            //    var Hi_anp = Math.Abs(rdcs[s].Trapez_level);
                            //    var R_i_anp = 17860 / (Math.Abs(rdcs[s].Trapez_str) + 0.00000001);

                            //    var Hi = Math.Abs(rdcs[s].Avg_level);
                            //    var R_i = 17860 / (Math.Abs(rdcs[s].Avg_str) + 0.00000001);

                            //    rdcs[s].PassBoost_anp    = (float)((VPass * VPass)   / (13.0 * R_i_anp) - 0.0061 * Hi_anp); 
                            //    rdcs[s].FreightBoost_anp = (float)((VFreig * VFreig) / (13.0 * R_i_anp) - 0.0061 * Hi_anp);

                            //    rdcs[s].PassBoost    = (float)((VPass * VPass)   / (13.0 * R_i) - 0.0061 * Hi);
                            //    rdcs[s].FreightBoost = (float)((VFreig * VFreig) / (13.0 * R_i) - 0.0061 * Hi);
                            //}



                            var AgPassMax = rdcs.Select(o => o.PassBoost).Max();
                            var AgPassMaxCoord = rdcs[rdcs.Select(o => o.PassBoost).ToList().IndexOf(AgPassMax)].M;

                            //Пси мах
                            var FiList = new List<double> { };
                            var FiListMeter = new List<int> { };
                            var l = 20;
                            if (speed.Any())
                            {
                                if (speed.First().Passenger < 60)
                                {
                                    l = 20;
                                }
                                else if (speed.First().Passenger >= 60 && speed.First().Passenger <= 140)
                                {
                                    l = 30;
                                }
                                else if (speed.First().Passenger >= 141 && speed.First().Passenger < 250)
                                {
                                    l = 40;
                                }
                            }
                            for (int ii = l; ii < rdcs.Count - l; ii++)
                            {
                                var fi = (Math.Abs(rdcs[ii + l].PassBoost - rdcs[ii].PassBoost) * speed.Max(o => o.Passenger)) / (3.6 * l);
                                FiList.Add(fi);
                                FiListMeter.Add(rdcs[ii].M);
                            }

                            var FiMax = FiList.Max();
                            var FiMaxCoord = FiListMeter[FiList.IndexOf(FiList.Max())];


                            string radiusAverage = "";
                            string levelAverage = "";
                            string gaugeAverage = "";
                            string passboost = "";
                            string freightboost = "";

                            string Trapez_level = "";
                            string Trapez_str = "";

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

                                Trapez_str += rdc.GetTrapez_strCoord();
                                Trapez_level += rdc.GetTrapez_levelCoord();

                                passboost += rdc.GetPassBoostCoord();
                                freightboost += rdc.GetFreightBoostCoord();
                                gaugeAverage += rdc.GetGaugeCoord();
                            }

                            rdcsData = new Data(x, plan, level, gauge, passBoost, freightBoost, passSpeed, freightSpeed, transitionLength1, transitionLength2);


                            if (curve.Straightenings.Count > 0)
                                radiusH = Convert.ToInt32(17860 / curve.Straightenings.Min(s => s.Radius));

                            if (curve.Elevations.Count > 0)
                                levelH = Convert.ToInt32(curve.Elevations.Max(e => Math.Abs(e.Lvl)));

                            int xAxisInterval = RoundNum(width / 6);
                            double intervalKoef = 620.0 / (width + 20);
                            int xAxisIntervalReal = Convert.ToInt32(xAxisInterval * intervalKoef) - 24;
                            int xCurrentPositionReal = Convert.ToInt32(Math.Abs(RoundNum(minX) - minX) * intervalKoef);
                            XElement xAxisLabels = new XElement("labels");


                            string rXScale = ((width + 20) / 620.0).ToString();
                            XElement xAxisKmLabels = new XElement("kmlabels");
                            //new XElement("label", 
                            //    new XAttribute("value", curve.Start_Km), 
                            //    new XAttribute("style", "display:inline;position: absolute;font-size:12px;Left:0"))
                            //);


                            XElement xeXaxis = new XElement("xaxis", new XElement("xparam",
                                new XAttribute("minX", minX - 10),
                                new XAttribute("maxX", maxX + 10))
                            );

                            var kmCoords = rdcs.Select(rdc => rdc.M == 1).ToList();
                            int currentKm = curve.Start_Km;

                            var prevPicketKoord = -1;
                            var writeFirstKm = -1;
                            var writePiket9 = 0.0;

                            //for (int XCurrentPosition = RoundNum(minX) + 1; XCurrentPosition <= maxX; XCurrentPosition += xAxisInterval)
                            for (int XCurrentPosition = 0; XCurrentPosition <= rdcs.Count; XCurrentPosition++)
                            {
                                if (XCurrentPosition < rdcs.Count)
                                {
                                    if (rdcs[XCurrentPosition].GetRealCoordinate() != writePiket9)
                                    {
                                        if ((rdcs[XCurrentPosition].M) % 100 != 0)
                                            continue;
                                    }
                                    //это нужно если км нестандартный меньше
                                    if ((((rdcs[XCurrentPosition].M - 1) / 100) + 1) == 9)
                                    {
                                        var sss = rdcs.Where(o => o.Km == rdcs[XCurrentPosition].Km).ToList();
                                        if (sss.Any())
                                        {
                                            if (sss.Last().M != 1000)
                                            {
                                                writePiket9 = sss.Last().GetRealCoordinate();
                                            }
                                        }
                                    }

                                    var radiusHh = (int)rdcs[rdcs.Count / 2].Trapez_str;
                                    var levelHh = (int)rdcs[rdcs.Count / 2].Trapez_level;

                                    xeXaxis.Add(new XElement("line",

                                        new XAttribute("y1s", (radiusHh > 0 ? radiusHh - radiusHh - 8 : radiusHh - 16)),
                                        new XAttribute("y2s", (radiusHh > 0 ? radiusHh + 16 : 8)),

                                        new XAttribute("y1-level", (levelHh > 0 ?
                                                                    100 > levelHh + 8 ? 105 : levelHh + 8 :
                                                                    -100 < levelHh - 8 ? -105 : levelHh - 8)),
                                        new XAttribute("y2-level", (levelHh > 0 ? -8 : 8)),

                                        new XAttribute("x1", XCurrentPosition),
                                        new XAttribute("y2", (-radiusH - 5))));

                                    //это нужно для того чтобы пикеты не сливались к друг-другу
                                    var currentPicketKoord = Convert.ToInt32(((XCurrentPosition - minX + 10) * intervalKoef));
                                    if (prevPicketKoord != -1 && Math.Abs(prevPicketKoord - currentPicketKoord) <= 12)
                                    {
                                        currentPicketKoord += 12;
                                    }
                                    prevPicketKoord = currentPicketKoord;

                                    xAxisLabels.Add(new XElement("label",
                                        new XAttribute("value", (rdcs[XCurrentPosition].M / 100) >= 10 ? (rdcs[XCurrentPosition].M / 100) % 10 + 1 : (rdcs[XCurrentPosition].M / 100) + 1),
                                        new XAttribute("style", "font-weight: bold; display:inline;position: absolute;font-size:12px;Left:" + (currentPicketKoord - 7)))
                                        );

                                    if (rdcs[XCurrentPosition].Km != currentKm)
                                    {
                                        currentKm = rdcs[XCurrentPosition].Km;
                                        xAxisKmLabels.Add(new XElement("label",
                                            new XAttribute("value", currentKm),
                                            new XAttribute("style", "font-weight: bold; display:inline;position: absolute;font-size:12px;Left:" + Convert.ToInt32((XCurrentPosition) * intervalKoef))));

                                        if (writeFirstKm == -1)
                                        {
                                            writeFirstKm = Convert.ToInt32(XCurrentPosition * intervalKoef);
                                        }
                                    }
                                }
                            }

                            //if (!xAxisKmLabels.Elements("label").Any())
                            if (writeFirstKm >= 40 || writeFirstKm == -1)
                            {
                                xAxisKmLabels.Add(
                                    new XElement("label",
                                    new XAttribute("value", curve.Start_Km),
                                    new XAttribute("style", "font-weight: bold; display:inline;position: absolute;font-size:12px;Left:0"))
                                    );
                            }

                            xeXaxis.Add(xAxisLabels);
                            xeXaxis.Add(xAxisKmLabels);
                            float widthR = (width + 20) * 0.0045f;
                            radiusH = 0;
                            levelH = 0;

                            //Поиск круговой кивой рихтовки
                            var str_circular = new List<RDCurve> { };

                            for (int strIndex = 1; strIndex < StrPoins.Count - 1; strIndex++)
                            {
                                if (Math.Abs(StrPoins[strIndex].X - StrPoins[strIndex - 1].X) < 5)
                                    continue;

                                var firstDiffX = Math.Abs(Math.Abs(StrPoins[strIndex].Trapez_str) - Math.Abs(StrPoins[strIndex - 1].Trapez_str)) / Math.Abs(StrPoins[strIndex].X - StrPoins[strIndex - 1].X);
                                var secondDiffX = Math.Abs(Math.Abs(StrPoins[strIndex].Trapez_str) - Math.Abs(StrPoins[strIndex + 1].Trapez_str)) / Math.Abs(StrPoins[strIndex].X - StrPoins[strIndex + 1].X);

                                if (5.0 * firstDiffX < secondDiffX || 5.0 * secondDiffX < firstDiffX)
                                {
                                    str_circular.Add(StrPoins[strIndex]);

                                    xeXaxis.Add(new XElement("rectangle",
                                        new XAttribute("x", StrPoins[strIndex].X),
                                        new XAttribute("y", StrPoins[strIndex].Trapez_str < 0 ? StrPoins[strIndex].Trapez_str : StrPoins[strIndex].Trapez_str - StrPoins[strIndex].Trapez_str), //ToDo
                                        new XAttribute("width", 0.5),
                                        new XAttribute("height", Math.Abs(StrPoins[strIndex].Trapez_str))));
                                }
                            }

                            if (Math.Abs(Math.Abs(StrPoins[StrPoins.Count - 1].Trapez_str) - Math.Abs(StrPoins[StrPoins.Count - 2].Trapez_str)) / Math.Abs(StrPoins[StrPoins.Count - 1].X - StrPoins[StrPoins.Count - 2].X) < 0.05)
                            {
                                str_circular.Add(StrPoins[StrPoins.Count - 1]);

                                xeXaxis.Add(new XElement("rectangle",
                                    new XAttribute("x", StrPoins[StrPoins.Count - 1].X),
                                    new XAttribute("y", StrPoins[StrPoins.Count - 1].Trapez_str < 0 ? StrPoins[StrPoins.Count - 1].Trapez_str : StrPoins[StrPoins.Count - 1].Trapez_str - StrPoins[StrPoins.Count - 1].Trapez_str), //ToDo
                                    new XAttribute("width", 0.5),
                                    new XAttribute("height", Math.Abs(StrPoins[StrPoins.Count - 1].Trapez_str))));
                            }

                            //Поиск круговой кивой уровень
                            var lvl_circular = new List<RDCurve> { };

                            for (int lvlIndex = 1; lvlIndex < LevelPoins.Count - 1; lvlIndex++)
                            {
                                if (Math.Abs(LevelPoins[lvlIndex].X - LevelPoins[lvlIndex - 1].X) < 6)
                                    continue;

                                var firstDiffX = Math.Abs(Math.Abs(LevelPoins[lvlIndex].Trapez_level) - Math.Abs(LevelPoins[lvlIndex - 1].Trapez_level)) / Math.Abs(LevelPoins[lvlIndex].X - LevelPoins[lvlIndex - 1].X);
                                var secondDiffX = Math.Abs(Math.Abs(LevelPoins[lvlIndex].Trapez_level) - Math.Abs(LevelPoins[lvlIndex + 1].Trapez_level)) / Math.Abs(LevelPoins[lvlIndex].X - LevelPoins[lvlIndex + 1].X);

                                if (5.0 * firstDiffX < secondDiffX || 5.0 * secondDiffX < firstDiffX)
                                {
                                    lvl_circular.Add(LevelPoins[lvlIndex]);

                                    xeXaxis.Add(new XElement("rectangle_lvl",
                                        new XAttribute("x", LevelPoins[lvlIndex].X),
                                        new XAttribute("y", LevelPoins[lvlIndex].Trapez_level < 0 ? LevelPoins[lvlIndex].Trapez_level : LevelPoins[lvlIndex].Trapez_level - LevelPoins[lvlIndex].Trapez_level), //ToDo
                                        new XAttribute("width", 0.5),
                                        new XAttribute("height", Math.Abs(LevelPoins[lvlIndex].Trapez_level))));
                                }
                            }
                            if (Math.Abs(Math.Abs(LevelPoins[LevelPoins.Count - 1].Trapez_level) - Math.Abs(LevelPoins[LevelPoins.Count - 2].Trapez_level)) / Math.Abs(LevelPoins[LevelPoins.Count - 1].X - LevelPoins[LevelPoins.Count - 2].X) < 0.05)
                            {
                                lvl_circular.Add(LevelPoins[LevelPoins.Count - 1]);

                                xeXaxis.Add(new XElement("rectangle",
                                    new XAttribute("x", LevelPoins[LevelPoins.Count - 1].X),
                                    new XAttribute("y", LevelPoins[LevelPoins.Count - 1].Trapez_level < 0 ? LevelPoins[LevelPoins.Count - 1].Trapez_level : LevelPoins[LevelPoins.Count - 1].Trapez_level - LevelPoins[LevelPoins.Count - 1].Trapez_level), //ToDo
                                    new XAttribute("width", 0.5),
                                    new XAttribute("height", Math.Abs(LevelPoins[LevelPoins.Count - 1].Trapez_level))));
                            }










                            if (curve.Straightenings.Count > 0)
                                radiusH = Convert.ToInt32(17860 / curve.Straightenings.Min(s => s.Radius));

                            if (curve.Elevations.Count > 0)
                                levelH = Convert.ToInt32(curve.Elevations.Max(e => Math.Abs(e.Lvl)));

                            XElement marks = new XElement("marks");




                            //Радиус номер
                            if (-15 > -radiusH - 5)
                            {
                                double koef = 127.0 / (radiusH + 10);
                                double topValue = 2 * 18 + 10 + 20 * koef;
                                marks.Add(new XElement("mark",
                                    new XAttribute("topValue", -topValue),
                                    new XAttribute("sign", 15)));
                                topValue = 3 * 18 + 10 + 15 * koef;
                                marks.Add(new XElement("mark",
                                    new XAttribute("topValue", -topValue),
                                    new XAttribute("sign", 10)));
                                topValue = 4 * 18 + 10 + 11 * koef;
                                marks.Add(new XElement("mark",
                                    new XAttribute("topValue", -topValue),
                                    new XAttribute("sign", 6)));
                            }
                            else if (-10 > -radiusH - 5)
                            {
                                double koef = 127.0 / (radiusH + 10);
                                double topValue = 2 * 18 + 10 + 15 * koef;
                                marks.Add(new XElement("mark",
                                    new XAttribute("topValue", -topValue),
                                    new XAttribute("sign", 10)));
                                topValue = 3 * 18 + 10 + 11 * koef;
                                marks.Add(new XElement("mark",
                                    new XAttribute("topValue", -topValue),
                                    new XAttribute("sign", 6)));
                            }
                            else if (-6 > -radiusH - 5)
                            {
                                double koef = 127.0 / (radiusH + 10);
                                double topValue = 2 * 18 + 10 + 11 * koef;
                                marks.Add(new XElement("mark",
                                    new XAttribute("topValue", -topValue),
                                    new XAttribute("sign", 6)));
                            }

                            //Уровень номер
                            if (-100 > -levelH - 5)
                            {
                                double koef = 127.0 / (levelH + 10);
                                double topValue = 2 * 18 + 10 + 105 * koef;
                                marks.Add(new XElement("markLvl",
                                    new XAttribute("topValue", -topValue),
                                    new XAttribute("sign", 100)));
                                topValue = 3 * 18 + 10 + 55 * koef;
                                marks.Add(new XElement("markLvl",
                                    new XAttribute("topValue", -topValue),
                                    new XAttribute("sign", 50)));
                            }
                            else if (-50 > -levelH - 5)
                            {
                                double koef = 127.0 / (levelH + 10);
                                double topValue = 2 * 18 + 10 + 55 * koef;
                                marks.Add(new XElement("markLvl",
                                    new XAttribute("topValue", -topValue),
                                    new XAttribute("sign", 50)));
                            }

                            float defaultRetractionSlopePlanLeft = radiusH * 1f / transitionLength1;
                            float defaultRetractionSlopePlanRight = radiusH * 1f / transitionLength2;
                            float defaultRetractionSlopeLevelLeft = levelH * 1f / transitionLength1;
                            float defaultRetractionSlopeLevelRight = levelH * 1f / transitionLength2;


                            var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curve.Id) as List<CurvesAdmUnits>;
                            CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;
                            string site = "Неизвестный";
                            if (curvesAdmUnits.Any())
                            {
                                if (!curvesAdmUnit.StationStart.Equals("Неизвестный") && !curvesAdmUnit.StationFinal.Equals("Неизвестный"))
                                {
                                    if (curvesAdmUnit.StationStart.Equals(curvesAdmUnit.StationFinal))
                                        site = curvesAdmUnit.StationStart;
                                    else
                                        site = curvesAdmUnit.StationStart + "-" + curvesAdmUnit.StationFinal;
                                }
                                else if (curvesAdmUnit.StationStart.Equals("Неизвестный") && !curvesAdmUnit.StationFinal.Equals("Неизвестный"))
                                {
                                    site = curvesAdmUnit.StationFinal;
                                }
                                else if (!curvesAdmUnit.StationStart.Equals("Неизвестный") && curvesAdmUnit.StationFinal.Equals("Неизвестный"))
                                {
                                    site = curvesAdmUnit.StationStart;
                                }
                            }

                            //данные бокового износа
                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyCurve(curve, trip.Id);

                            var bok_izl = new List<float> { };
                            var bok_izr = new List<float> { };

                            var side_bok = false;

                            if (DBcrossRailProfile.Any())
                            {
                                bok_izl.AddRange(DBcrossRailProfile.Select(o => o.Bok_l).ToList());
                                bok_izr.AddRange(DBcrossRailProfile.Select(o => o.Bok_r).ToList());

                                if (bok_izl.Average() > bok_izr.Average())
                                    side_bok = true;
                            }

                            var iznos = side_bok ? bok_izl : bok_izr;

                            var bok_iz_graph = "";
                            var indx = LevelPoins.Any() ? rdcs.IndexOf(LevelPoins.First()) : 1;
                            foreach (var item in iznos)
                            {
                                bok_iz_graph += $"{indx}, {(-item).ToString("0.00000").Replace(",", ".")} ";
                                indx++;
                            }


                            //кривизна
                            radiusH = (int)rdcs[rdcs.Count / 2].Trapez_str;

                            var strView = "";
                            var val30 = 60;
                            var style30 = "";
                            var style0 = "";
                            if (radiusH < 0)
                            {
                                strView = (minX - 10) + " " + (-30 < radiusH - 8 ? -35 : radiusH - 8) + " " + (width + 20) + " " + (-30 < radiusH - 8 ? 40 : Math.Abs(radiusH) + 16); //вверх
                                val30 = 60;
                                style30 = "top: -139px; left: -29px; position: relative; text - align: right; width: 30px;";
                                style0 = "top: -27px; left: -29px; position: relative; text - align: right; width: 30px;";
                            }
                            else
                            {
                                strView = (minX - 10) + " " + (radiusH - radiusH - 8) + " " + (width + 20) + " " + (30 > Math.Abs(radiusH) ? 35 + 8 : Math.Abs(radiusH) + 16); //вниз
                                val30 = -60;
                                style30 = "top: -44px; left: -29px; position: relative; text - align: right; width: 30px;";
                                style0 = "top: -111px; left: -29px; position: relative; text - align: right; width: 30px;";
                            }

                            //возвышение
                            levelH = (int)rdcs[rdcs.Count / 2].Trapez_level;

                            var levelHView = "";
                            var val100l = 100;
                            var style100l = "";
                            var val50l = 100;
                            var style50l = "";
                            var val0l = 100;
                            var style0l = "";
                            if (levelH < 0)
                            {
                                levelHView = (minX - 10) + " " + (-100 < levelH - 8 ? -105 : levelH - 8) + " " + (width + 20) + " " + (-100 < levelH - 8 ? 110 : Math.Abs(levelH) + 16); //вверх
                                val100l = 100;
                                style100l = "top: -131px; left: -29px; position: relative; text - align: right; width: 30px;";
                                val50l = 50;
                                style50l = "top: -90px; left: -29px; position: relative; text - align: right; width: 30px;";
                                val0l = 0;
                                style0l = "top: -52px; left: -29px; position: relative; text - align: right; width: 30px;";
                            }
                            else
                            {
                                levelHView = (minX - 10) + " " + (levelH - levelH - 8) + " " + (width + 20) + " " + (100 > Math.Abs(levelH) ? 105 + 8 : Math.Abs(levelH) + 16); //вниз
                                val100l = -100;
                                style100l = "top: -16px; left: -29px; position: relative; text - align: right; width: 30px;";
                                val50l = -50;
                                style50l = "top: -90px; left: -29px; position: relative; text - align: right; width: 30px;";
                                val0l = 0;
                                style0l = "top: -163px; left: -29px; position: relative; text - align: right; width: 30px;";
                            }

                            XElement xeCurve = new XElement("curve",
                                new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version}"),
                                new XAttribute("date_trip", trip.Trip_date.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))),
                                new XAttribute("site", site),
                                new XAttribute("road", roadName),
                                new XAttribute("type", trip.GetProcessTypeName),
                                new XAttribute("period", period.Period),
                                new XAttribute("track", curvesAdmUnit != null ? curvesAdmUnit.Track : "-"),
                                new XAttribute("direction", curvesAdmUnit != null ? curvesAdmUnit.Direction : "-"),
                                new XAttribute("km", curve.Start_Km.ToString() + " - " + curve.Final_Km.ToString()),
                                new XAttribute("side", curve.Side == "Левая" ? "Правая" : "Левая"),
                                new XAttribute("order", curve.Num),
                                new XAttribute("PC", trip.Car),
                                //Кривизна
                                new XAttribute("viewbox", strView),
                                new XAttribute("radius-length", radiusLength),
                                new XAttribute("radius-average", 0 + ",0 " + radiusAverage + " " + (maxX + 20) + ",0"), //натурная линия
                                new XAttribute("radius-trapez", 0 + ",0 " + Trapez_str + " " + (maxX + 20) + ",0"),

                                new XAttribute("radius-style30", style30), //30-30
                                new XAttribute("radius-val30", val30),
                                new XAttribute("radius-style0", style0), //0
                                new XAttribute("radius-val0", 0),

                                //Возвышение
                                new XAttribute("viewbox-level", levelHView),
                                new XAttribute("boost-level", (minX - 10) + " " + (-1) + " " + (width + 20) + " 1.1"),
                                new XAttribute("level-average", 0 + ",0 " + levelAverage + " " + (maxX + 20) + ",0"), //натурная линия
                                new XAttribute("level-trapez", 0 + ",0 " + Trapez_level + " " + (maxX + 20) + ",0"),

                                new XAttribute("radius-style100l", style100l), //100-100
                                new XAttribute("radius-val100l", val100l),
                                new XAttribute("radius-style50l", style50l), //50-50
                                new XAttribute("radius-val50l", val50l),
                                new XAttribute("radius-style0l", style0l), //0
                                new XAttribute("radius-val0l", val0l),

                                new XAttribute("viewbox_bok_iz_graph", $"{(minX - 10)} {-23} {(width + 20)} {24}"),
                                new XAttribute("bok_iz_graph", 0 + ",0 " + bok_iz_graph + " " + (maxX + 20) + ",0"), //Бок из линия

                                //new XAttribute("radius-average", 0 + ",0 " + rA + " " + (maxX + 20) + ",0"),

                                //new XAttribute("gauge", 0 + ",0 " + gaugeAverage + " " + (maxX + 20) + ",0"),

                                new XAttribute("passboost", 0 + ",0 " + passboost + " " + (maxX + 20) + ",0"),
                                new XAttribute("freightboost", 0 + ",0 " + freightboost + " " + (maxX + 20) + ",0")
                                );

                            //----------------------------------------------------------------------------
                            //-------мнгород кривая осы жерде вершиналары ызделеды------------------------
                            //----------------------------------------------------------------------------

                            int lvl = -1;
                            int lenPru = -1;

                            var vershinyLVL = vershList.Where(o => Math.Abs(o.First().FiList2) < 0.1).ToList();

                            var minH = new List<RDCurve>();
                            var minOb = 3000.0;
                            foreach (var item in vershinyLVL)
                            {
                                var h = item.Select(o => o.Trapez_level).Average();

                                if (Math.Abs(h) < minOb)
                                {
                                    minOb = Math.Abs(h);
                                    minH = item;
                                }
                            }

                            lenPru = minH.Count;
                            lvl = (int)minOb;

                            var maxAnp = new List<RDCurve>();
                            var max = -1.0;
                            var circularList = vershList.Where(o => Math.Abs(o.First().FiList) < 0.1).ToList();

                        

                            int len = rdcsData.X3IndexPlan - rdcsData.X0IndexPlan;
                            int len2 = rdcsData.X2IndexPlan - rdcsData.X1IndexPlan;
                            int gaugeStandard = curve.Straightenings.Max(s => s.Width);
                            curve.Radius = Convert.ToInt32(curve.Straightenings.Max(s => s.Radius));

                            try
                            {
                                //нижние 2 точки трапеции
                                var start_km = StrPoins.First().Km;
                                var start_m = StrPoins.First().M;
                                var final_km = StrPoins.Last().Km;
                                var final_m = StrPoins.Last().M;

                                var start_lvl_km = LevelPoins.First().Km;
                                var start_lvl_m = LevelPoins.First().M;
                                var final_lvl_km = LevelPoins.Last().Km;
                                var final_lvl_m = LevelPoins.Last().M;


                                //верхние 2 точки трапеции
                                var start_kmc = str_circular.First().Km;
                                var start_mc = str_circular.First().M;
                                var final_kmc = str_circular.Last().Km;
                                var final_mc = str_circular.Last().M;

                                for (int cirrInd = 0; cirrInd < str_circular.Count - 1; cirrInd++)
                                {
                                    if (Math.Abs(str_circular[cirrInd].Trapez_str - str_circular[cirrInd + 1].Trapez_str) /
                                        Math.Abs(str_circular[cirrInd].X - str_circular[cirrInd + 1].X) > 0.0065

                                        && (Math.Abs(str_circular[cirrInd].Trapez_str) < 5 || Math.Abs(str_circular[cirrInd + 1].Trapez_str) < 5))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        //начальный до 1 переходной
                                        var circ1Ind = StrPoins.IndexOf(str_circular[cirrInd]);
                                        for (int iii = circ1Ind - 1; iii >= 0; iii--)
                                        {
                                            if (Math.Abs(StrPoins[iii].Trapez_str) < 3)
                                            {
                                                start_km = StrPoins[iii].Km;
                                                start_m = StrPoins[iii].M;
                                                //var final_km = StrPoins.Last().Km;
                                                //var final_m = StrPoins.Last().M;
                                                break;
                                            }
                                        }
                                        //конечный от 2 переходной
                                        var circ2Ind = StrPoins.IndexOf(str_circular[cirrInd + 1]);
                                        for (int iii = circ2Ind + 1; iii < StrPoins.Count; iii++)
                                        {
                                            if (Math.Abs(StrPoins[iii].Trapez_str) < 3)
                                            {
                                                //start_km = StrPoins[iii].Km;
                                                //start_m = StrPoins[iii].M;
                                                final_km = StrPoins[iii].Km;
                                                final_m = StrPoins[iii].M;
                                                break;
                                            }
                                        }

                                        // круговая верхние точки
                                        start_kmc = str_circular[cirrInd].Km;
                                        start_mc = str_circular[cirrInd].M;

                                        final_kmc = str_circular[cirrInd + 1].Km;
                                        final_mc = str_circular[cirrInd + 1].M;

                                        break;
                                    }
                                }

                                var start_lvl_kmc = lvl_circular.First().Km;
                                var start_lvl_mc = lvl_circular.First().M;
                                var final_lvl_kmc = lvl_circular.Last().Km;
                                var final_lvl_mc = lvl_circular.Last().M;

                                for (int cirrInd = 0; cirrInd < lvl_circular.Count - 1; cirrInd++)
                                {
                                    if (Math.Abs(lvl_circular[cirrInd].Trapez_level - lvl_circular[cirrInd + 1].Trapez_level) /
                                        Math.Abs(lvl_circular[cirrInd].X - lvl_circular[cirrInd + 1].X) > 0.0065

                                        && (Math.Abs(lvl_circular[cirrInd].Trapez_level) < 5 || Math.Abs(lvl_circular[cirrInd + 1].Trapez_level) < 5))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        //начальный до 1 переходной
                                        var circ1Ind = LevelPoins.IndexOf(lvl_circular[cirrInd]);
                                        for (int iii = circ1Ind - 1; iii >= 0; iii--)
                                        {
                                            if (Math.Abs(LevelPoins[iii].Trapez_level) < 3)
                                            {
                                                start_lvl_km = LevelPoins[iii].Km;
                                                start_lvl_m = LevelPoins[iii].M;
                                                //var final_km = StrPoins.Last().Km;
                                                //var final_m = StrPoins.Last().M;
                                                break;
                                            }
                                        }
                                        //конечный от 2 переходной
                                        var circ2Ind = LevelPoins.IndexOf(lvl_circular[cirrInd + 1]);
                                        for (int iii = circ2Ind + 1; iii < LevelPoins.Count; iii++)
                                        {
                                            if (Math.Abs(LevelPoins[iii].Trapez_level) < 3)
                                            {
                                                //start_km = StrPoins[iii].Km;
                                                //start_m = StrPoins[iii].M;
                                                final_lvl_km = LevelPoins[iii].Km;
                                                final_lvl_m = LevelPoins[iii].M;
                                                break;
                                            }
                                        }
                                        //круговая верхние точки
                                        start_lvl_kmc = lvl_circular[cirrInd].Km;
                                        start_lvl_mc = lvl_circular[cirrInd].M;

                                        final_lvl_kmc = lvl_circular[cirrInd + 1].Km;
                                        final_lvl_mc = lvl_circular[cirrInd + 1].M;

                                        break;
                                    }
                                }

                                var lenPerKriv10000 = ((start_kmc + start_mc / 10000.0) - (final_kmc + final_mc / 10000.0)) * 10000;
                                var lenPerKriv = Math.Abs((int)lenPerKriv10000 % 1000);

                                var lenKriv10000 = ((start_km + start_m / 10000.0) - (final_km + final_m / 10000.0)) * 10000;
                                var lenKriv = Math.Abs((int)lenKriv10000 % 1000);

                                var lenPerKriv10000lv = ((start_lvl_kmc + start_lvl_mc / 10000.0) - (final_lvl_kmc + final_lvl_mc / 10000.0)) * 10000;
                                var lenPerKrivlv = Math.Abs((int)lenPerKriv10000lv % 1000);

                                var lenKriv10000lv = ((start_lvl_km + start_lvl_m / 10000.0) - (final_lvl_km + final_lvl_m / 10000.0)) * 10000;
                                var lenKrivlv = Math.Abs((int)lenKriv10000lv % 1000);

                                var d = false;
                                if ((start_km + start_m / 10000.0) > (final_km + final_m / 10000.0))
                                    d = true;


                                var razn1 = (int)(((start_km + start_m / 10000.0) - (start_lvl_km + start_lvl_m / 10000.0)) * 10000) % 1000; // start
                                var razn2 = (int)(((final_km + final_m / 10000.0) - (final_lvl_km + final_lvl_m / 10000.0)) * 10000) % 1000; // final
                                var razn3 = lenKriv - lenKrivlv; // общая длина нижних




                                var razn1c = (int)(((start_kmc + start_mc / 10000.0) - (start_lvl_kmc + start_lvl_mc / 10000.0)) * 10000) % 1000; // start
                                var razn2c = (int)(((final_kmc + final_mc / 10000.0) - (final_lvl_kmc + final_lvl_mc / 10000.0)) * 10000) % 1000; // final

                                //Переходные 
                                //1-й
                                var tap_len1 = Math.Round(((start_km + start_m / 10000.0) - (start_kmc + start_mc / 10000.0)) * 10000) % 1000;
                                var tap_len1_lvl = Math.Round(((start_lvl_km + start_lvl_m / 10000.0) - (start_lvl_kmc + start_lvl_mc / 10000.0)) * 10000) % 1000;
                                //2-й
                                var tap_len2 = Math.Round(((final_km + final_m / 10000.0) - (final_kmc + final_mc / 10000.0)) * 10000) % 1000;
                                var tap_len2_lvl = Math.Round(((final_lvl_km + final_lvl_m / 10000.0) - (final_lvl_kmc + final_lvl_mc / 10000.0)) * 10000) % 1000;

                                //Радиус/Уровень (для мин макс сред)
                                var temp_data = rdcs.GetRange((int)Math.Abs(tap_len1_lvl) + 40, Math.Abs(lenPerKrivlv));
                                var temp_data_str = rdcs.Where(o => (start_kmc + start_mc / 10000.0) <= (o.Km + o.M / 10000.0) && (o.Km + o.M / 10000.0) <= (final_kmc + final_mc / 10000.0)).ToList();
                                var temp_data_lvl = rdcs.Where(o => (start_lvl_kmc + start_lvl_mc / 10000.0) <= (o.Km + o.M / 10000.0) && (o.Km + o.M / 10000.0) <= (final_lvl_kmc + final_lvl_mc / 10000.0)).ToList();

                                //Переходные (для макс сред)
                                var transitional_lvl_data = rdcs.Where(o => ((o.Km + o.M / 10000.0) <= (start_lvl_kmc + start_lvl_mc / 10000.0) && (o.Km + o.M / 10000.0) >= (start_lvl_kmc + start_lvl_mc / 10000.0) - Math.Abs(tap_len1_lvl) / 10000.0)).ToList();
                                var transitional_str_data = rdcs.Where(o => ((o.Km + o.M / 10000.0) <= (start_kmc + start_mc / 10000.0) && (o.Km + o.M / 10000.0) >= (start_kmc + start_mc / 10000.0) - Math.Abs(tap_len1) / 10000.0)).ToList();

                                var transitional_lvl_data2 = rdcs.Where(o => ((o.Km + o.M / 10000.0) >= (final_kmc + final_mc / 10000.0)) && (o.Km + o.M / 10000.0) <= (final_kmc + final_mc / 10000.0 + Math.Abs(tap_len2_lvl) / 10000.0)).ToList();
                                var transitional_str_data2 = rdcs.Where(o => ((o.Km + o.M / 10000.0) >= (final_lvl_kmc + final_lvl_mc / 10000.0)) && (o.Km + o.M / 10000.0) <= (final_lvl_kmc + final_lvl_mc / 10000.0 + Math.Abs(tap_len2) / 10000.0)).ToList();


                                //план/ср 1 пер
                                var rad_mid = rdcsData.GetAvgPlan(temp_data_str);
                                //план/макс 1пер
                                var rad_max = rdcsData.GetMaxPlan(temp_data_str);

                                var rad_min = rdcsData.GetMinPlan(temp_data_str);

                                //уровень/ср 1 пер
                                var lvl_mid = rdcsData.GetAvgLevel(temp_data_lvl);
                                var perAvglvl = lvl_mid / Math.Abs(tap_len1_lvl);
                                //уровень/макс 1 пер
                                var lvl_max = rdcsData.GetMaxLevel(temp_data_lvl);
                                var perMaxlvl = lvl_max / Math.Abs(tap_len1_lvl);

                                var lvl_min = rdcsData.GetMinLevel(temp_data_lvl);













                                XElement paramCurve = new XElement("param_curve",
                                    //план
                                    new XAttribute("start_km", start_km),
                                    new XAttribute("start_m", start_m),
                                    new XAttribute("final_km", final_km),
                                    new XAttribute("final_m", final_m),
                                    //уровень
                                    new XAttribute("start_lvl_km", start_lvl_km),
                                    new XAttribute("start_lvl_m", start_lvl_m),
                                    new XAttribute("final_lvl_km", final_lvl_km),
                                    new XAttribute("final_lvl_m", final_lvl_m),

                                    new XAttribute("razn1", razn1),
                                    new XAttribute("razn2", razn2),
                                    new XAttribute("razn3", Math.Abs(razn3)),

                                    new XAttribute("len", lenKriv),
                                    new XAttribute("len_lvl", (lenKrivlv + 1)),

                                    new XAttribute("angle", CurveAngle(rad_mid, lenKriv).ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))));

                                XElement paramCircleCurve = new XElement("param_circle_curve");

                                if (circularList.Count == 1)
                                {
                                    paramCircleCurve.Add(
                                        new XAttribute("start_km", start_kmc),
                                        new XAttribute("start_m", start_mc),
                                        new XAttribute("final_km", final_kmc),
                                        new XAttribute("final_m", final_mc),

                                        new XAttribute("start_lvl_km", start_lvl_kmc),
                                        new XAttribute("start_lvl_m", start_lvl_mc),
                                        new XAttribute("final_lvl_km", final_lvl_kmc),
                                        new XAttribute("final_lvl_m", final_lvl_mc),

                                        new XAttribute("razn1", razn1c),
                                        new XAttribute("razn2", razn2c),

                                        new XAttribute("len", lenPerKriv),
                                        new XAttribute("len_lvl", (lenPerKrivlv)),

                                        new XAttribute("rad_min", rdcsData.GetMinPlan(temp_data_str)),
                                        new XAttribute("rad_max", rdcsData.GetMaxPlan(temp_data_str)),
                                        new XAttribute("rad_mid", rdcsData.GetAvgPlan(temp_data_str)),

                                        new XAttribute("lvl_min", lvl_min),
                                        new XAttribute("lvl_max", lvl_max),
                                        new XAttribute("lvl_mid", lvl_mid),

                                        new XAttribute("gauge_min", gaugeStandard + rdcsData.GetMaxGauge()),
                                        new XAttribute("gauge_max", gaugeStandard + rdcsData.GetMinGauge()),
                                        new XAttribute("gauge_mid", gaugeStandard + rdcsData.GetAvgGauge()));
                                }
                                else
                                {
                                    var raz1 = (int)((circularList.First().First().GetRealCoordinate() - (start_lvl_kmc + start_lvl_mc / 10000.0)) * 10000) % 1000; // start
                                    var raz2 = (int)((circularList.Last().Last().GetRealCoordinate() - (final_lvl_kmc + final_lvl_mc / 10000.0)) * 10000) % 1000; // final

                                    var lenPerKrivALL = (int)((circularList.Last().Last().GetRealCoordinate() - circularList.First().First().GetRealCoordinate()) * 10000) % 1000;

                                    temp_data_str.Clear();

                                    foreach (var item in circularList)
                                    {
                                        temp_data_str.AddRange(item);
                                    }

                                    paramCircleCurve.Add(
                                        new XAttribute("start_km", circularList.First().First().Km),
                                        new XAttribute("start_m", circularList.First().First().M),
                                        new XAttribute("final_km", circularList.Last().Last().Km),
                                        new XAttribute("final_m", circularList.Last().Last().M),

                                        new XAttribute("start_lvl_km", start_lvl_kmc),
                                        new XAttribute("start_lvl_m", start_lvl_mc),
                                        new XAttribute("final_lvl_km", final_lvl_kmc),
                                        new XAttribute("final_lvl_m", final_lvl_mc),

                                        new XAttribute("razn1", raz1),
                                        new XAttribute("razn2", raz2),

                                        new XAttribute("len", Math.Abs(lenPerKrivALL)),
                                        new XAttribute("len_lvl", lenPerKrivlv),

                                        new XAttribute("rad_min", rdcsData.GetMinPlan(temp_data_str)),
                                        new XAttribute("rad_max", rdcsData.GetMaxPlan(temp_data_str)),
                                        new XAttribute("rad_mid", rdcsData.GetAvgPlan(temp_data_str)),

                                        new XAttribute("lvl_min", lvl_min),
                                        new XAttribute("lvl_max", lvl_max),
                                        new XAttribute("lvl_mid", lvl_mid),

                                        new XAttribute("gauge_min", gaugeStandard + rdcsData.GetMaxGauge()),
                                        new XAttribute("gauge_max", gaugeStandard + rdcsData.GetMinGauge()),
                                        new XAttribute("gauge_mid", gaugeStandard + rdcsData.GetAvgGauge()));
                                }

                                XElement sideWear = new XElement("side_wear",
                                    new XAttribute("mm6", iznos.Any() ? iznos.Where(o => o > 6).Count() : 0),
                                    new XAttribute("mm10", iznos.Any() ? iznos.Where(o => o > 10).Count() : 0),
                                    new XAttribute("mm15", iznos.Any() ? iznos.Where(o => o > 15).Count() : 0),
                                    new XAttribute("max", iznos.Any() ? iznos.Where(o => o > 0).Max().ToString("0.00") : ""),
                                    new XAttribute("mid", iznos.Any() ? iznos.Where(o => o > 0).Min().ToString("0.00") : "")
                                    );

                                var Vkr = Math.Sqrt((0.7 + 0.0061 * lvl_mid) * 13.0 * rad_mid);

                                var dl = passSpeed.First() > 140 ? 40 : passSpeed.First() > 60 ? 30 : 20;
                                var mx = rdcs.Select(o => o.PassBoost).Max();
                                var mn = rdcs.Select(o => o.PassBoost).Min();
                                var Viz = (3.6 * dl * 0.6) / (mx - mn);

                                int[] passSpeeds = new int[] { 0, 0, 0 };
                                int[] freightSpeeds = new int[] { 0, 0, 0 };

                                passSpeeds[0] = rdcsData.GetKRSpeedPass(rdcs);
                                passSpeeds[1] = rdcsData.GetPRSpeed(rdcs)[0];
                                passSpeeds[2] = rdcsData.GetIZPassSpeed();

                                freightSpeeds[0] = rdcsData.GetKRSpeedFreig(rdcs);
                                freightSpeeds[1] = rdcsData.GetPRSpeed(rdcs)[1];
                                freightSpeeds[2] = rdcsData.GetIZPassSpeed();

                                var AnpPassMax = (Math.Pow(passSpeed.First(), 2) / (13.0 * rad_mid)) - (0.0061 * lvl_mid);

                                var AnpFreigMax = (Math.Pow(freightSpeed.First(), 2) / (13.0 * rad_mid)) - (0.0061 * lvl_mid);

                                var passmax = rdcsData.GetPassSpeed();
                                int len_rs; //len retraction slope
                                if (passmax > 140)
                                    len_rs = 40;
                                else if (passmax <= 140 && passmax >= 60)
                                    len_rs = 30;
                                else
                                    len_rs = 20;

                                var piketogr = "";
                                if (rdcsData.GetPassSpeed() > Vkr || rdcsData.GetPassSpeed() > passSpeeds[1] || rdcsData.GetPassSpeed() > passSpeeds[2])
                                {
                                    piketogr = ((AgPassMaxCoord / 100) >= 10 ? (AgPassMaxCoord / 100) % 10 + 1 : (AgPassMaxCoord / 100) + 1).ToString();
                                }
                                    XElement withdrawal = new XElement("withdrawal",
                                    new XAttribute("tap_max1", rdcsData.GetPlanMaxRetractionSlope(transitional_str_data, len_rs).ToString("0.00")),
                                    new XAttribute("tap_mid1", rdcsData.GetPlanAvgRetractionSlope(transitional_str_data, tap_len1).ToString("0.00")),
                                    new XAttribute("tap_len1", Math.Abs(tap_len1)),

                                    new XAttribute("tap_max1_lvl", rdcsData.GetLvlMaxRetractionSlope(transitional_lvl_data, len_rs).ToString("0.00")),
                                    new XAttribute("tap_mid1_lvl", perAvglvl.ToString("0.00")),
                                    new XAttribute("tap_len1_lvl", Math.Abs(tap_len1_lvl)),

                                    new XAttribute("tap_max2", transitional_str_data2.Any() ? rdcsData.GetPlanMaxRetractionSlope(transitional_str_data2, len_rs).ToString("0.00") : "0"),
                                    new XAttribute("tap_mid2", transitional_str_data2.Any() ? rdcsData.GetPlanAvgRetractionSlope(transitional_str_data2, tap_len2).ToString("0.00") : "0"),
                                    new XAttribute("tap_len2", Math.Abs(tap_len2)),

                                    new XAttribute("tap_max2_lvl", rdcsData.GetLvlMaxRetractionSlope(transitional_lvl_data2, len_rs).ToString("0.00")),
                                    new XAttribute("tap_mid2_lvl", (lvl_mid / Math.Abs(tap_len2_lvl)).ToString("0.00")),
                                    new XAttribute("tap_len2_lvl", Math.Abs(tap_len2_lvl)));

                                XElement computing = new XElement("computing",
                                    //Анп
                                    new XAttribute("a1", AnpPassMax.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("a2", AnpFreigMax.ToString("0.00").Replace(",", ".")),

                                    new XAttribute("a3", rdcsData.GetUnliquidatedAccelerationFreightAvg().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("a4", rdcsData.GetUnliquidatedAccelerationFreightMax().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("a5", rdcsData.GetUnliquidatedAccelerationPassengerMaxCoordinate().ToString()),
                                    new XAttribute("psi1", rdcsData.BoostChangeRateMax().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("psi2", rdcsData.BoostChangeRateMaxCoordinate().ToString()),
                                    new XAttribute("P", rdcsData.GetR().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("V1", rdcsData.GetCriticalSpeed03up().ToString()),
                                    new XAttribute("V2", rdcsData.GetCriticalSpeed03down().ToString()));

                                XElement speedElement = new XElement("speed",
                                    new XAttribute("pass1", rdcsData.GetPassSpeed().ToString()),
                                    new XAttribute("pass2", Vkr.ToString("0")),
                                    new XAttribute("pass3", passSpeeds[1].ToString()),
                                    new XAttribute("pass4", passSpeeds[2].ToString()),
                                    new XAttribute("pass5", "-"),
                                    new XAttribute("pass6", RoundNumToFive(passSpeeds.Min()) >= rdcsData.GetPassSpeed() ? "-" : RoundNumToFive(passSpeeds.Min()).ToString()),

                                    new XAttribute("frei1", rdcsData.GetFreightSpeed().ToString()),
                                    new XAttribute("frei2", Vkr.ToString("0")),
                                    new XAttribute("frei3", freightSpeeds[1].ToString()),
                                    new XAttribute("frei4", freightSpeeds[2].ToString()),
                                    new XAttribute("frei5", "-"),
                                    new XAttribute("frei6", RoundNumToFive(freightSpeeds.Min()) >= rdcsData.GetFreightSpeed() ? "-" : RoundNumToFive(freightSpeeds.Min()).ToString()),

                                    new XAttribute("AgPassMax", $"{AgPassMax.ToString("0.00").Replace(",", ".")} / {AgPassMaxCoord}"),
                                    new XAttribute("FiPassMax", $"{FiMax.ToString("0.00").Replace(",", ".")} / {FiMaxCoord}"),
                                    new XAttribute("AgFreigMax", $""),
                                    new XAttribute("FiFreigMax", $""),
                                    new XAttribute("PiketOgr", piketogr)
                                    );

                                XElement Speeds = new XElement("Speeds");
                                Speeds.Add(new XElement("Speed",
                                        new XAttribute("Value", (speed.Any() ? $"{speed.First().Passenger} / {speed.First().Freight}" : "нет данных Speed")
                                        )));

                                xeCurve.Add(Speeds);
                                xeCurve.Add(xeXaxis);
                                xeCurve.Add(marks);
                                xeCurve.Add(paramCurve);
                                xeCurve.Add(paramCircleCurve);
                                xeCurve.Add(sideWear);
                                xeCurve.Add(withdrawal);
                                xeCurve.Add(computing);
                                xeCurve.Add(speedElement);
                                report.Add(xeCurve);

                                i++;
                            }
                            catch (Exception e)
                            {
                                System.Console.WriteLine(e.Message);
                            }
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
                htReport.Save($@"G:\form\2.Характеристики положения пути в плане и профиле\11.Карточка кривой на не скоростных линиях (ФП-3.2).html");
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



























