using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;

namespace ALARm_Report.Forms
{
    public class CurveSpeedCharacterstics : Report
    {
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod current, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                //Сделать выбор периода
                List<long> admTracksId = new List<long>();
                using (var choiceForm = new ChoiseForm(0))
                {
                    choiceForm.SetTripsDataSource(parentId, current);
                    choiceForm.ShowDialog();
                    if (choiceForm.dialogResult == DialogResult.Cancel)
                        return;
                    admTracksId = choiceForm.admTracksIDs;
                }
                // var road = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                var trips = RdStructureService.GetMainParametersProcesses(current, parentId, true);
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report",
                                    new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                                    //new XAttribute("dateandtype", trip.GetProcessTypeName),
                                    //new XAttribute("DKI", trip.Car),
                                    new XAttribute("way", roadName),
                                    new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                                    new XAttribute("distance", ((AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId)).Code),
                                    new XAttribute("source", "база данных"),
                                    new XAttribute("file_travel", "база данных")
                                    
                                    );

                foreach (var tripProcess in trips)
                {

                    XElement tripElem = new XElement("trip",
                          new XAttribute("direction", tripProcess.DirectionName),
                          new XAttribute("chief", tripProcess.Chief),
                          new XAttribute("ps", tripProcess.Car),
                          new XAttribute("way", roadName),
                          new XAttribute("distance", ((AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId)).Code),
                          new XAttribute("check", RdStructureService.GetTrip(tripProcess.Id).GetProcessTypeName),
                          new XAttribute("periodDate", current.Period)
                          );
                    bool curvesintrip = false;
                    foreach (var track_id in admTracksId)
                    {
                        XElement trackElem = new XElement("track",
                         new XAttribute("direction", tripProcess.DirectionName),
                         new XAttribute("chief", tripProcess.Chief),
                         new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                         new XAttribute("ps", tripProcess.Car),
                         new XAttribute("track", AdmStructureService.GetTrackName(track_id)));

                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);
                        if (!kilometers.Any()) continue;

                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();
                        if (kilometers.Count == 0) continue;
                        trip.Track_Id = track_id;
                        var lkm = kilometers.Select(o => o.Number).ToList();

                        if (lkm.Count() == 0) continue;


                        

                        string[] subs = tripProcess.DirectionName.Split('(');

                        List<Curve> curves = RdStructureService.GetCurvesInTrip(tripProcess.Id) as List<Curve>;

                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        //фильтр по выбранным км
                        var filter_curves = curves.Where(o => ((float)(float)filters[0].Value <= o.Start_Km && o.Final_Km <= (float)(float)filters[1].Value)).ToList();

                        int i = 0;
                        foreach (var curve in filter_curves)
                        {
                            List<Speed> speeds = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, curve.Start_Km, MainTrackStructureConst.MtoSpeed, subs.Any() ? subs.First() : "", trackName.ToString()) as List<Speed>;
                            List<RDCurve> rdcs = RdStructureService.GetRDCurves(curve.Id, trip.Id);

                            if (rdcs.First().Track_Id != track_id)
                            {
                                continue;
                            }

                            curvesintrip = true;
                            //var LevelPoins = rdcs.Where(o => o.Point_level > 0).ToList();
                            //var StrPoins = rdcs.Where(o => o.Point_str > 0).ToList();

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
                                else if (strData[versh].FiList > 0.1)
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

                            if (LevelPoins.Count() < 4)
                                continue;
                            if (StrPoins.Count() < 4)
                                continue;

                            curve.Elevations =
                                (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();
                            curve.Straightenings =
                                (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();
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
                            int transitionLength1 = 0, transitionLength2 = 0, radiusH = 0, levelH = 0;
                            string radiusStart = string.Empty;
                            var rdcsExisting = rdcs.Count > 0;
                            if (curve.Straightenings.Count < 1)
                                continue;
                            transitionLength1 = curve.Straightenings.First().Transition_1;
                            transitionLength2 = curve.Straightenings.Last().Transition_2;
                            radiusH = Convert.ToInt32(17860 / curve.Straightenings.Min(s => s.Radius));
                            levelH = curve.Elevations.Any() ? Convert.ToInt32(curve.Elevations.Max(e => Math.Abs(e.Lvl))) : 0;
                            string radiusAverage = "";
                            string levelAverage = "";
                            string gaugeAverage = "";
                            string passboost = "";
                            string freightboost = "";
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
                            var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curve.Id) as List<CurvesAdmUnits>;
                            CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Count > 0 ? curvesAdmUnits[0] : null;
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
                                }
                            }

                            if (Math.Abs(Math.Abs(StrPoins[StrPoins.Count - 1].Trapez_str) - Math.Abs(StrPoins[StrPoins.Count - 2].Trapez_str)) / Math.Abs(StrPoins[StrPoins.Count - 1].X - StrPoins[StrPoins.Count - 2].X) < 0.05)
                            {
                                str_circular.Add(StrPoins[StrPoins.Count - 1]);
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

                                }
                            }
                            if (Math.Abs(Math.Abs(LevelPoins[LevelPoins.Count - 1].Trapez_level) - Math.Abs(LevelPoins[LevelPoins.Count - 2].Trapez_level)) / Math.Abs(LevelPoins[LevelPoins.Count - 1].X - LevelPoins[LevelPoins.Count - 2].X) < 0.05)
                            {
                                lvl_circular.Add(LevelPoins[LevelPoins.Count - 1]);
                            }

                            XElement xeCurve = new XElement("curve",
                                new XAttribute("check", trip.GetProcessTypeName),
                                new XAttribute("Period", current.Period),
                                 new XAttribute("mm", "-1"),//Пороговое значение износа: to do
                                new XAttribute("date_trip", trip.Trip_date),
                                new XAttribute("site", site),
                                new XAttribute("road", curvesAdmUnit != null ? curvesAdmUnit.Track : "-"),
                                new XAttribute("direction", curvesAdmUnit != null ? curvesAdmUnit.Direction : "-"),
                                new XAttribute("km", curves.Min(c => c.Start_Km).ToString() + " - " + curves.Max(c => c.Final_Km).ToString()),
                                 //new XAttribute("side", curve.Side.ToLower() == "левая" ? "Л" : "П"),
                                 new XAttribute("side", curve.Side == "Левая" ? "П" : "Л"),
                                new XAttribute("order", $"{i + 1} "),
                                new XAttribute("ID_DB", curve.Side_id),
                                new XAttribute("PC", trip.Car));

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
                                //var temp1 = (8865.0 / rad_mid) * 4;
                                //var perAvg1 = temp1 / Math.Abs(tap_len1);
                                var perAvg1 = rad_mid / Math.Abs(tap_len1);
                                //план/макс 1пер
                                var rad_max = rdcsData.GetMaxPlan(temp_data_str);
                                //var temp = (8865.0 / rad_max) * 4;
                                //var perMax = temp / Math.Abs(tap_len1);
                                var perMax = rad_max / Math.Abs(tap_len1);

                                var rad_min = rdcsData.GetMinPlan(temp_data_str);

                                //уровень/ср 1 пер
                                var lvl_mid = rdcsData.GetAvgLevel(temp_data_lvl);
                                var perAvglvl = lvl_mid / Math.Abs(tap_len1_lvl);
                                //уровень/макс 1 пер
                                var lvl_max = rdcsData.GetMaxLevel(temp_data_lvl);
                                var perMaxlvl = lvl_max / Math.Abs(tap_len1_lvl);

                                var lvl_min = rdcsData.GetMinLevel(temp_data_lvl);


                                var AnpPassMax = (Math.Pow(passSpeed.First(), 2) / (13.0 * rad_mid)) - (0.0061 * lvl_mid);

                                var AnpFreigMax = (Math.Pow(freightSpeed.First(), 2) / (13.0 * rad_mid)) - (0.0061 * lvl_mid);




                                XElement paramCurve = new XElement("param_curve",
                                    //план
                                    new XAttribute("start_km", start_km),
                                    new XAttribute("start_m", start_m),
                                    new XAttribute("final_km", final_km),
                                    new XAttribute("final_m", final_m),
                                    //уровень
                                    new XAttribute("start_lvl", start_lvl_m),
                                    new XAttribute("final_lvl", final_lvl_m),
                                    new XAttribute("angle", CurveAngle(rdcsData.GetAvgPlan(temp_data_str), lenPerKriv).ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))));

                                XElement paramCircleCurve = new XElement("param_circle_curve",
                                    new XAttribute("len", lenPerKriv),
                                    new XAttribute("len_lvl", lenPerKrivlv),
                                    new XAttribute("min", rdcsData.GetMinPlan(temp_data_str)),
                                    new XAttribute("max", rdcsData.GetMaxPlan(temp_data_str)),
                                    new XAttribute("mid", rdcsData.GetAvgPlan(temp_data_str)),
                                    new XAttribute("min_lvl", rdcsData.GetMinLevel(temp_data_lvl)),
                                    new XAttribute("max_lvl", rdcsData.GetMaxLevel(temp_data_lvl)),
                                    new XAttribute("mid_lvl", rdcsData.GetAvgLevel(temp_data_lvl)));

                                var passmax = rdcsData.GetPassSpeed();
                                int len_rs; //len retraction slope
                                if (passmax > 140)
                                    len_rs = 40;
                                else if (passmax <= 140 && passmax >= 60)
                                    len_rs = 30;
                                else
                                    len_rs = 20;

                                XElement transition = new XElement("transition",
                                    new XAttribute("max1", rdcsData.GetPlanMaxRetractionSlope(transitional_str_data, len_rs).ToString("0.00")),
                                new XAttribute("mid1", rdcsData.GetPlanAvgRetractionSlope(transitional_str_data, tap_len1).ToString("0.00")),
                                new XAttribute("len1", Math.Abs(tap_len1)),

                                new XAttribute("max1_lvl", rdcsData.GetLvlMaxRetractionSlope(transitional_lvl_data, len_rs).ToString("0.00")),
                                new XAttribute("mid1_lvl", perAvglvl.ToString("0.00")),
                                new XAttribute("len1_lvl", Math.Abs(tap_len1_lvl)),

                                new XAttribute("max2", transitional_str_data2.Any() ? rdcsData.GetPlanMaxRetractionSlope(transitional_str_data2, len_rs).ToString("0.00") : "0"),
                                new XAttribute("mid2", transitional_str_data2.Any() ? rdcsData.GetPlanAvgRetractionSlope(transitional_str_data2, tap_len2).ToString("0.00") : "0"),
                                new XAttribute("len2", Math.Abs(tap_len2)),

                                new XAttribute("max2_lvl", rdcsData.GetLvlMaxRetractionSlope(transitional_lvl_data2, len_rs).ToString("0.00")),
                                new XAttribute("mid2_lvl", (lvl_mid / Math.Abs(tap_len2_lvl)).ToString("0.00")),
                                new XAttribute("len2_lvl", Math.Abs(tap_len2_lvl)));

                             

                                //Аг пасс
                                var AgPass = new List<double> { };
                                var Vsap = speeds.Any() ?Math.Max( speeds.First().Sapsan , speeds.Last().Sapsan): 80;
                                var Vlas = speeds.Any() ? Math.Max(speeds.First().Lastochka, speeds.Last().Lastochka) : 80;
                                var Vstr = speeds.Any() ? Math.Max(speeds.First().Empty_Freight, speeds.Last().Empty_Freight) : 80; //стриж

                                for (int s = 5; s < rdcs.Count-5; s++)
                                {
                                    var Hi_anp = Math.Abs(rdcs[s].Trapez_level);
                                    var R_i_anp = 17860 / (Math.Abs(rdcs[s].Trapez_str) + 0.00000001);
                                    rdcs[s].SapsanBoost_anp = (float)((Vsap * Vsap) / (13.0 * R_i_anp) - 0.0061 * Hi_anp);
                                    rdcs[s].LastochkaBoost_anp = (float)((Vlas * Vlas) / (13.0 * R_i_anp) - 0.0061 * Hi_anp);
                                    rdcs[s].FreightBoost_anp = (float)((Vstr * Vstr) / (13.0 * R_i_anp) - 0.0061 * Hi_anp);

                                    var Hi = Math.Abs(rdcs[s].Avg_level);
                                    var R_i = 17860 / (Math.Abs(rdcs[s].Avg_str) + 0.00000001);
                                    rdcs[s].SapsanBoost = (float)((Vsap * Vsap) / (13.0 * R_i) - 0.0061 * Hi);
                                    rdcs[s].LastochkaBoost = (float)((Vlas * Vlas) / (13.0 * R_i) - 0.0061 * Hi);
                                    rdcs[s].FreightBoost = (float)((Vstr * Vstr) / (13.0 * R_i) - 0.0061 * Hi);
                                }
                                var AnpSapsanMax = rdcs.Select(o => o.SapsanBoost_anp).Max();
                                var AnpLastochkaMax = rdcs.Select(o => o.LastochkaBoost_anp).Max();
                                var AnpStrizhMax = rdcs.Select(o => o.FreightBoost_anp).Max();

                                var AgSapsanMax = rdcs.Select(o => o.SapsanBoost).Max();
                                var AgLastochkaMax = rdcs.Select(o => o.LastochkaBoost).Max();
                                var AgStrizhMax = rdcs.Select(o => o.FreightBoost).Max();

                                //Пси мах
                                var FiListSapsan = new List<double> { };
                                var FiListMeterSapsan = new List<int> { };

                                var FiListLastochka = new List<double> { };
                                var FiListMeterLastochka = new List<int> { };

                                var FiListStrizh = new List<double> { };
                                var FiListMeterStrizh = new List<int> { };

                                var l = 20;
                                if (speeds.Any())
                                {
                                    if (speeds.First().Passenger < 60)
                                    {
                                        l = 20;
                                    }
                                    else if (speeds.First().Passenger >= 60 && speeds.First().Passenger <= 140)
                                    {
                                        l = 30;
                                    }
                                    else if (speeds.First().Passenger >= 141 && speeds.First().Passenger < 250)
                                    {
                                        l = 40;
                                    }
                                }
                                for (int ii = l; ii < rdcs.Count - l; ii++)
                                {
                                    var fi = (Math.Abs(rdcs[ii + l].SapsanBoost - rdcs[ii].SapsanBoost) * speeds.First().Sapsan) / (3.6 * l);
                                    FiListSapsan.Add(fi);
                                    FiListMeterSapsan.Add(rdcs[ii].M);

                                    var fi2 = (Math.Abs(rdcs[ii + l].LastochkaBoost - rdcs[ii].LastochkaBoost) * speeds.First().Lastochka) / (3.6 * l);
                                    FiListLastochka.Add(fi2);
                                    FiListMeterLastochka.Add(rdcs[ii].M);

                                    var fi3 = (Math.Abs(rdcs[ii + l].FreightBoost - rdcs[ii].FreightBoost) * speeds.First().Empty_Freight) / (3.6 * l);
                                    FiListStrizh.Add(fi2);
                                    FiListMeterStrizh.Add(rdcs[ii].M);
                                }

                                var FiMaxSapsan = FiListSapsan.Max();
                                var FiMaxLastochka = FiListLastochka.Max();
                                var FiMaxStrizh= FiListStrizh.Max();

                                int[] sapsan = new int[] { 0, 0, 0 };
                                int[] lastochka = new int[] { 0, 0, 0 };
                                int[] strizh = new int[] { 0, 0, 0 };

                                sapsan[0] = rdcsData.GetKRSpeedPass(rdcs);
                                sapsan[1] = rdcsData.GetPRSpeed(rdcs)[0];
                                sapsan[2] = rdcsData.GetIZSpeed(FiListSapsan.ToArray(), Vsap);

                                lastochka[0] = rdcsData.GetKRSpeedFreig(rdcs);
                                lastochka[1] = rdcsData.GetPRSpeed(rdcs)[0];
                                lastochka[2] = rdcsData.GetIZSpeed(FiListLastochka.ToArray(), Vlas);

                                strizh[0] = rdcsData.GetKRSpeedFreig(rdcs);
                                strizh[1] = rdcsData.GetPRSpeed(rdcs)[0];
                                strizh[2] = rdcsData.GetIZSpeed(FiListStrizh.ToArray(), Vstr);

                                XElement speed = new XElement("speed",
                                    new XAttribute("AnpAvg", AnpSapsanMax.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("AgMax", AgSapsanMax.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("Psi", FiMaxSapsan.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("Vpz", speeds.Any() ? speeds.First().Sapsan : -1),
                                    new XAttribute("Vkr", sapsan[0].ToString()),
                                    new XAttribute("Vpr", sapsan[1].ToString()),
                                    new XAttribute("Viz", sapsan[2].ToString()),
                                    new XAttribute("Vdp", RoundNumToFive(sapsan.Min()) >= speeds.First().Sapsan ? "-" : RoundNumToFive(sapsan.Min()).ToString()),

                                    new XAttribute("AnpAvg2", AnpLastochkaMax.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("AgMax2", AgLastochkaMax.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("Psi2", FiMaxLastochka.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("Vpz2", speeds.Any() ? speeds.First().Lastochka : -1),
                                    new XAttribute("Vkr2", lastochka[0].ToString()),
                                    new XAttribute("Vpr2", lastochka[1].ToString()),
                                    new XAttribute("Viz2", lastochka[2].ToString()),
                                    new XAttribute("Vdp2", RoundNumToFive(lastochka.Min()) >= speeds.First().Lastochka ? "-" : RoundNumToFive(lastochka.Min()).ToString()),

                                    new XAttribute("AnpAvg3", AnpStrizhMax.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("AgMax3", AgStrizhMax.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("Psi3", FiMaxStrizh.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("Vpz3", speeds.Any() ? speeds.First().Empty_Freight : -1),
                                    new XAttribute("Vkr3", strizh[0].ToString()),
                                    new XAttribute("Vpr3", strizh[1].ToString()),
                                    new XAttribute("Viz3", strizh[2].ToString()),
                                    new XAttribute("Vdp3", RoundNumToFive(strizh.Min()) >= speeds.First().Empty_Freight ? "-" : RoundNumToFive(strizh.Min()).ToString()));

                                xeCurve.Add(paramCurve);
                                xeCurve.Add(paramCircleCurve);
                                xeCurve.Add(transition);
                                xeCurve.Add(speed);
                                trackElem.Add(xeCurve);
                                
                                i++;

                            }
                            catch
                            {
                                Console.WriteLine("Ошибка при расчете натурной кривой");
                            }

                        }
                        if (curvesintrip)
                        {
                          tripElem.Add(trackElem);
                        }
                        
                    }
                    if (curvesintrip)
                    {
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
                htReport.Save($@"G:\form\2.Характеристики положения пути в плане и профиле\15.Ведомость характеристик устройства кривых скоростных и высокоскоростных участков пути.html");
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