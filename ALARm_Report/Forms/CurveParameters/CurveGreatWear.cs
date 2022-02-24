using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm_Report.Forms
{
    public class CurveGreatWear : Report
    {
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod current, MetroProgressBar progressBar)
        {
            float wear = -1;
            using (var filterForm = new FilterForm())
            {
                var filters = new List<Filter>();
                filters.Add(new FloatFilter() { Name = "Порог износа более: ", Value = wear });
                filterForm.SetDataSource(filters);
                if (filterForm.ShowDialog() == DialogResult.Cancel)
                    return;
                wear = (float)filters[0].Value;
            }
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(parentId, current);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel)
                    return;
                admTracksId = choiceForm.admTracksIDs;
            }

            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                var distance = (AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId);
                var nod = (AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id);
                var road = (AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmRoad, nod.Parent_Id);

                int i = 1;
                bool inserted = false;
                XElement xeDirection = new XElement("direction");
                var dir = false;
                var trips = RdStructureService.GetMainParametersProcesses(current, parentId, true);
                foreach (var trackId in admTracksId)
                {
                    
                    foreach (var trip in trips)
                    {
                        //track Узнать трак если совпадает с трипс (Тракайди)
                        //continiu

                        XElement report = new XElement("report",
                              new XAttribute("road", road.Name),
                              new XAttribute("period", current.Period),
                              new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                              new XAttribute("distance", distance.Code),
                              new XAttribute("check", trip.GetProcessTypeName),
                              new XAttribute("wear", wear));
                        var trackName = AdmStructureService.GetTrackName(trackId);

                        List<Curve> curves = RdStructureService.GetCurvesInTrip(trip.Id) as List<Curve>;
                        var tr = RdStructureService.GetTrip(trip.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(tr);

                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var lkm = kilometers.Select(o => o.Number).ToList();

                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                        //filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        //filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = 700 });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = 730 });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        //фильтр по выбранным км
                        var filter_curves = curves.Where(o => ((float)(float)filters[0].Value <= o.Start_Km && o.Final_Km <= (float)(float)filters[1].Value)).ToList();

                        foreach (var curve in filter_curves)
                        {
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

                            if (StrPoins.Count < 4)
                                continue;
                            if (LevelPoins.Count < 4)
                                continue;

                            var LevelMax = rdcs.Select(o => o.Trapez_level).ToList();
                            var StrMax = rdcs.Select(o => o.Trapez_str).ToList();

                            curve.Elevations = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();
                            curve.Straightenings = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();

                            if (curve.Straightenings.Count < 1) continue;
                            if (curve.Straightenings.Max(s => s.Wear) < wear) continue;

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
                            int transitionLength1 = curve.Straightenings.First().Transition_1, transitionLength2 = curve.Straightenings.Last().Transition_2;

                            string radiusAverage = "";
                            string levelAverage = "";
                            string gaugeAverage = "";
                            string passboost = "";
                            string freightboost = "";
                            foreach (var rdc in rdcs)
                            {
                                x.Add(rdc.X);
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
                            int gaugeStandard = curve.Straightenings.Max(s => s.Width);
                            curve.Radius = (int)curve.Straightenings.Max(s => s.Radius);

                            if (dir == false)
                            {
                                xeDirection.Add(
                                    new XAttribute("Direct", curvesAdmUnits.Count > 0 ? curvesAdmUnits[0].Direction : "-"),
                                    new XAttribute("track_info", $"{trip.DirectionName} Путь:{trackName}"),
                                     new XAttribute("check", trip.GetProcessTypeName),
                                    new XAttribute("Track", curvesAdmUnits.Count > 0 ? curvesAdmUnits[0].Track : "-")
                                    );
                                dir = true;
                            }

                            if (!inserted)
                            {
                                report.Add(
                                    new XAttribute("ps", trip.Car),
                                    new XAttribute("chief", trip.Chief),
                                    //new XAttribute("check", trip.GetProcessTypeName),
                                    new XAttribute("tripdate", current.Period));
                                inserted = true;
                            }

                            var lenPerKriv10000 = ((StrPoins[1].Km + StrPoins[1].M / 10000.0) - (StrPoins[2].Km + StrPoins[2].M / 10000.0)) * 10000;
                            var lenPerKriv = Math.Abs((int)lenPerKriv10000 % 1000);

                            var lenKriv10000 = ((StrPoins[0].Km + StrPoins[0].M / 10000.0) - (StrPoins[3].Km + StrPoins[3].M / 10000.0)) * 10000;
                            var lenKriv = Math.Abs((int)lenKriv10000 % 1000);

                            var lenPerKriv10000lv = ((LevelPoins[1].Km + LevelPoins[1].M / 10000.0) - (LevelPoins[2].Km + LevelPoins[2].M / 10000.0)) * 10000;
                            var lenPerKrivlv = Math.Abs((int)lenPerKriv10000lv % 1000);

                            var lenKriv10000lv = ((LevelPoins[0].Km + LevelPoins[0].M / 10000.0) - (LevelPoins[3].Km + LevelPoins[3].M / 10000.0)) * 10000;
                            var lenKrivlv = Math.Abs((int)lenKriv10000lv % 1000);

                            var d = false;
                            if ((StrPoins[0].Km + StrPoins[0].M / 10000.0) > (StrPoins[3].Km + StrPoins[3].M / 10000.0))
                                d = true;

                            //нижние 2 точки трапеции
                            var start_km = d ? StrPoins[3].Km : StrPoins.First().Km;
                            var start_m = d ? StrPoins[3].M : StrPoins.First().M;
                            var final_km = d ? StrPoins.First().Km : StrPoins[3].Km;
                            var final_m = d ? StrPoins.First().M : StrPoins[3].M;

                            //верхние 2 точки трапеции
                            var start_kmc = d ? StrPoins[2].Km : StrPoins[1].Km;
                            var start_mc = d ? StrPoins[2].M : StrPoins[1].M;
                            var final_kmc = d ? StrPoins[1].Km : StrPoins[2].Km;
                            var final_mc = d ? StrPoins[1].M : StrPoins[2].M;

                            var temp_data_str = rdcs.Where(o => (start_kmc + start_mc / 10000.0) <= (o.Km + o.M / 10000.0) && (o.Km + o.M / 10000.0) <= (final_kmc + final_mc / 10000.0)).ToList();

                            //realStart
                            var realStart = start_kmc + start_mc / 10000.0;
                            //realFinal
                            var realFinal = final_kmc + final_mc / 10000.0;


                            //данные бокового износа
                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyCurve(curve, trip.Id);
                            if (DBcrossRailProfile == null) continue;
                            //данные по круговой кривой
                            DBcrossRailProfile = DBcrossRailProfile.Where(o => realStart <= o.RealCoordinate && o.RealCoordinate <= realFinal).ToList();
                           

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

                            var radius = rdcsData.GetAvgPlan(temp_data_str);

                            XElement xeCurve = new XElement("curve",
                                new XAttribute("side", curve.Side[0] == 'Л' ? "Пр" : "Лев"),
                                new XAttribute("start_km", start_km),
                                new XAttribute("start_m", start_m),
                                new XAttribute("final_km", final_km),
                                new XAttribute("final_m", final_m),
                                new XAttribute("radius", radius),
                                new XAttribute("wear_mid", iznos.Any() ? iznos.Average().ToString("0.00") : ""),
                                new XAttribute("wear_max", iznos.Any() ? iznos.Max().ToString("0.00") : ""),
                                new XAttribute("V1", rdcsData.GetPassSpeed().ToString()),
                                new XAttribute("V2", rdcsData.GetFreightSpeed().ToString())
                                //new XAttribute("tempspeed", "-/-/-")
                                );
                            //получаем список пикетов
                            var count_PC = DBcrossRailProfile.Select(e => e.Picket).Distinct().ToList();


                            var prevKm = -1;
                            var speeds = new List<Speed> { };

                            var order = 0;

                            foreach (var picket in count_PC)
                            {
                                var picketData = DBcrossRailProfile.Where(o => o.Picket == picket).ToList();
                                var iznosData = side_bok ? picketData.Select(o => o.Bok_l).ToList() : picketData.Select(o => o.Bok_r).ToList();

                                if (wear > iznosData.Max())
                                    continue;

                                if (prevKm != picketData.First().Km)
                                    speeds = MainTrackStructureService.GetMtoObjectsByCoord(trip.Date_Vrem, picketData.First().Km, MainTrackStructureConst.MtoSpeed, trip.DirectionName.Split('(').First(), "1") as List<Speed>;
                                var speed = speeds.Where(o => o.RealStartCoordinate <= picketData.First().RealCoordinate && picketData.First().RealCoordinate <= o.RealFinalCoordinate).ToList();

                                prevKm = picketData.First().Km;

                                var Vust = speed.First();
                                var Vogr = GetIznosDig(iznosData.Max(), radius);

                                xeCurve.Add(new XElement("PC",
                                        new XAttribute("order", picket),
                                        new XAttribute("wear_mid", iznosData.Average().ToString("0.00")),
                                        new XAttribute("wear_max", iznosData.Max().ToString("0.00")),

                                        new XAttribute("len1215", iznosData.Any() ? iznosData.Where(o => 12 <= o && o <= 15).ToList().Count : 0),
                                        new XAttribute("len1620", iznosData.Any() ? iznosData.Where(o => 16 <= o && o <= 20).ToList().Count : 0),
                                        new XAttribute("len20",   iznosData.Any() ? iznosData.Where(o => 20 < o).ToList().Count : 0),

                                        new XAttribute("Vpz", Vust.ToString()),
                                        new XAttribute("Vogr", Vust.Passenger > Vogr ? $"{ Vogr }/{ Vogr }" : "-"))
                                        );

                                order++;
                            }

                            xeCurve.Add(new XAttribute("count_PC", order));

                            xeDirection.Add(xeCurve);
                            //xeDirection.Add("trackinfo", $"{trip.DirectionName} Путь: {trackName}");
                            i++;
                        }
                       
                        report.Add(xeDirection);
                        xdReport.Add(report);
                    }
                }
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

        private int GetIznosDig(float iznos, int curveRadius)
        {
            int Vogr = -1;

            switch (iznos)
            {
                case var value when value <= 4.0:
                    Vogr = 250;
                    break;
                case var value when 4.0 < value && value <= 6.0:
                    Vogr = 200;
                    break;
                case var value when 6.0 < value && value <= 15.0:
                    Vogr = 140;
                    break;
                case var value when 15.0 < value && value <= 20.0:
                    if (curveRadius < 350)
                        Vogr = 50;
                    else
                        Vogr = 70;
                    break;
                case var value when 20.0 < value:
                    Vogr = 50;
                    break;

                default:
                    break;
            }
            return Vogr;
        }
    }
}
