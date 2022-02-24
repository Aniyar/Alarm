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
    public class CurveDeviceCharacterstics : Report
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
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                var trips = RdStructureService.GetMainParametersProcesses(current, parentId, true);
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report",
                     new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                     new XAttribute("way", roadName),
                     new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                     new XAttribute("distance", ((AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId)).Code),
                     new XAttribute("source", "база данных"),
                     new XAttribute("file_travel", "база данных"));

                foreach (var tripProcess in trips)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        XElement tripElem = new XElement("trip",
                          new XAttribute("direction", tripProcess.DirectionName),
                          new XAttribute("chief", tripProcess.Chief),
                          new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                          new XAttribute("ps", tripProcess.Car),
                          new XAttribute("track", trackName));

                        List<Curve> curves = RdStructureService.GetCurvesInTrip(tripProcess.Id) as List<Curve>;
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);

                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();
                     
                        var lkm = kilometers.Select(o => o.Number).ToList();
                        if (!lkm.Any()) continue;
                    
                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        //filters.Add(new FloatFilter() { Name = "Начало (км)", Value = 129 });
                        //filters.Add(new FloatFilter() { Name = "Конец (км)", Value = 133 });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        //фильтр по выбранным км
                        var filter_curves = curves.Where(o => ((float)(float)filters[0].Value <= o.Start_Km && o.Final_Km <= (float)(float)filters[1].Value)).ToList();

                        int i = 0;
                        foreach (var curve in filter_curves)
                        {
                            string[] subs = tripProcess.DirectionName.Split('(');

                            List<Speed> speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, curve.Start_Km, MainTrackStructureConst.MtoSpeed, subs.Any() ? subs.First() : "", trackName.ToString()) as List<Speed>;
                            List<RDCurve> rdcs = RdStructureService.GetRDCurves(curve.Id, trip.Id);

                            var LevelPoins = rdcs.Where(o => o.Point_level > 0).ToList();
                            var StrPoins = rdcs.Where(o => o.Point_str > 0).ToList();

                            //if (LevelPoins.Count > 4)
                            //{
                            //    if (LevelPoins[0].Level < 15.0 && LevelPoins[1].Level < 15.0)
                            //    {
                            //        LevelPoins.RemoveAt(0);
                            //        StrPoins.RemoveAt(0);
                            //    }
                            //    else if (LevelPoins[3].Level < 15.0 && LevelPoins[4].Level < 15.0)
                            //    {
                            //        LevelPoins.RemoveAt(4);
                            //        if (StrPoins.Count > 4)
                            //            StrPoins.RemoveAt(4);
                            //    }
                            //}

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

                            XElement xeCurve = new XElement("curve",
                                new XAttribute("check", trip.GetProcessTypeName),
                                new XAttribute("Period", current.Period),
                                 new XAttribute("mm", "-1"),//Пороговое значение износа: to do
                                new XAttribute("date_trip", trip.Trip_date),
                                new XAttribute("site", site),
                                new XAttribute("road", curvesAdmUnit != null ? curvesAdmUnit.Track : "-"),
                                new XAttribute("direction", curvesAdmUnit != null ? curvesAdmUnit.Direction : "-"),
                                new XAttribute("km", curves.Min(c => c.Start_Km).ToString() + " - " + curves.Max(c => c.Final_Km).ToString()),
                                new XAttribute("side", curve.Side == "Левая" ? "П" : "Л"),
                                new XAttribute("order", $"{i + 1} "),
                                new XAttribute("ID_DB", curve.Side_id),
                                new XAttribute("PC", trip.Car));


                            try
                            {
                                //Поиск круговой кивой рихтовки
                                var str_circular = new List<RDCurve> { };

                                for (int strIndex = 1; strIndex < StrPoins.Count - 1; strIndex++)
                                {
                                    if (Math.Abs(StrPoins[strIndex].X - StrPoins[strIndex - 1].X) < 13)
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
                                var transitional_lvl_data = rdcs.GetRange(40, Math.Abs((int)tap_len1_lvl));
                                var transitional_str_data = rdcs.GetRange(40, Math.Abs((int)tap_len1));

                                var transitional_lvl_data2 = rdcs.GetRange((int)Math.Abs(tap_len1_lvl) + 40 + Math.Abs(lenPerKrivlv), Math.Abs((int)tap_len2_lvl));
                                var transitional_str_data2 = rdcs.GetRange((int)Math.Abs(tap_len1) + 40 + Math.Abs(lenPerKriv), Math.Abs((int)tap_len2));


                                //план/ср 1 пер
                                var rad_mid = rdcsData.GetAvgPlan(temp_data_str);
                                var temp1 = (8865.0 / rad_mid) * 4;
                                var perAvg1 = temp1 / Math.Abs(tap_len1);
                                //план/макс 1пер
                                var rad_max = rdcsData.GetMaxPlan(temp_data_str);
                                var temp = (8865.0 / rad_max) * 4;
                                var perMax = temp / Math.Abs(tap_len1);

                                var rad_min = rdcsData.GetMinPlan(temp_data_str);

                                //уровень/ср 1 пер
                                var lvl_mid = rdcsData.GetAvgLevel(temp_data_lvl);
                                var perAvglvl = lvl_mid / Math.Abs(tap_len1_lvl);
                                //уровень/макс 1 пер
                                var lvl_max = rdcsData.GetMaxLevel(temp_data_lvl);
                                var perMaxlvl = lvl_max / Math.Abs(tap_len1_lvl);

                                var lvl_min = rdcsData.GetMinLevel(temp_data_lvl);

                                //lvl = lvl_mid;
                                //str = rad_mid;

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

                                XElement transition = new XElement("transition",
                                    new XAttribute("max1", rdcsData.GetPlanLeftMaxRetractionSlope(transitional_str_data).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("mid1", rdcsData.GetPlanLeftAvgRetractionSlope(transitional_str_data).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("len1", Math.Abs(tap_len1)),

                                    new XAttribute("max1_lvl", rdcsData.GetLevelLeftMaxRetractionSlope(transitional_lvl_data).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("mid1_lvl", rdcsData.GetLevelLeftAvgRetractionSlope(transitional_lvl_data).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("len1_lvl", Math.Abs(tap_len1_lvl)),

                                    new XAttribute("max2", (rdcsData.GetPlanRightMaxRetractionSlope(transitional_str_data2)).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("mid2", (rdcsData.GetPlanRightAvgRetractionSlope(transitional_str_data2)).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("len2", Math.Abs(tap_len2)),

                                    new XAttribute("max2_lvl", rdcsData.GetLevelRightMaxRetractionSlope(transitional_lvl_data2).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("mid2_lvl", rdcsData.GetLevelRightAvgRetractionSlope(transitional_lvl_data2).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("len2_lvl", Math.Abs(tap_len2_lvl)));

                                int[] passSpeeds = new int[] { 0, 0, 0 };
                                int[] freightSpeeds = new int[] { 0, 0, 0 };

                                passSpeeds[0] = rdcsData.GetKRSpeedPass(rdcs);
                                passSpeeds[1] = rdcsData.GetPRSpeed(rdcs)[0];
                                passSpeeds[2] = rdcsData.GetIZPassSpeed();

                                freightSpeeds[0] = rdcsData.GetKRSpeedFreig(rdcs);
                                freightSpeeds[1] = rdcsData.GetPRSpeed(rdcs)[1];
                                freightSpeeds[2] = rdcsData.GetIZFreightSpeed();

                                //var AnpPassMax = rdcs.Select(o => o.PassBoost_anp).Max();
                                //var AnpFreigMax = rdcs.Select(o => o.FreightBoost_anp).Max();

                                var AgPassMax = rdcs.Select(o => o.PassBoost).Max();
                                var AgPassMaxCoord = rdcs[rdcs.Select(o => o.PassBoost).ToList().IndexOf(AgPassMax)].M;

                                var AgFreigMax = rdcs.Select(o => o.FreightBoost).Max();


                                //Пси мах
                                var FiList = new List<double> { };
                                var FiListFreig = new List<double> { };
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
                                    var fi = (Math.Abs(rdcs[ii + l].PassBoost - rdcs[ii].PassBoost) * speed.First().Passenger) / (3.6 * l);
                                    var fiFreig = (Math.Abs(rdcs[ii + l].FreightBoost - rdcs[ii].FreightBoost) * speed.First().Freight) / (3.6 * l);
                                    FiList.Add(fi);
                                    FiListMeter.Add(rdcs[ii].M);

                                    FiListFreig.Add(fiFreig);
                                }

                                var FiMax = FiList.Max();
                                var FiMaxCoord = FiListMeter[FiList.IndexOf(FiList.Max())];
                                var FiListFreigMax = FiListFreig.Max();


                                XElement speeds = new XElement("speed",
                                    new XAttribute("avgPASSA", AnpPassMax.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("PASSAMAX", AgPassMax.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("psi", FiMax.ToString("0.00").Replace(",", ".")),

                                    new XAttribute("V1", rdcsData.GetPassSpeed().ToString()),
                                    new XAttribute("V2", passSpeeds[0].ToString()),
                                    new XAttribute("V3", passSpeeds[1].ToString()),
                                    new XAttribute("V4", passSpeeds[2].ToString()),
                                    new XAttribute("V5", RoundNumToFive(passSpeeds.Min()) >= rdcsData.GetPassSpeed() ? "-" : RoundNumToFive(passSpeeds.Min()).ToString()),

                                    new XAttribute("avgfreightA", AnpFreigMax.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("freightAMAX", AgFreigMax.ToString("0.00").Replace(",", ".")),
                                    new XAttribute("PsiFreigh", FiListFreigMax.ToString("0.00").Replace(",", ".")),

                                    new XAttribute("V1Freigh", rdcsData.GetFreightSpeed().ToString()),
                                    new XAttribute("V2Freigh", freightSpeeds[0].ToString()),
                                    new XAttribute("V3Freigh", freightSpeeds[1].ToString()),
                                    new XAttribute("V4Freigh", freightSpeeds[2].ToString()),
                                    new XAttribute("V5Freigh", RoundNumToFive(freightSpeeds.Min()) >= rdcsData.GetFreightSpeed() ? "-" : RoundNumToFive(freightSpeeds.Min()).ToString()));

                                xeCurve.Add(paramCurve);
                                xeCurve.Add(paramCircleCurve);
                                xeCurve.Add(transition);
                                xeCurve.Add(speeds);
                                report.Add(xeCurve);
                                i++;
                                report.Add(tripElem);
                            }
                            catch
                            {
                                Console.WriteLine("Ошибка при расчете натурной кривой");
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