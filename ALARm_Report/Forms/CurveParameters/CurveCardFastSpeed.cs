using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
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
using ElCurve = ALARm.Core.ElCurve;


namespace ALARm_Report.Forms
{
    /// <summary>
    /// Карточка кривой
    /// </summary>
    /// 

    public class CurveCardFastSpeed : Report
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
        /// <summary>
        /// Возврат метра 
        /// </summary>
        /// <param name="interval">Координата в виде км*1000 + метр</param>
        /// <returns>Метр</returns>
        private int GetMeter(int interval, Curve curve)
        {
            return MainTrackStructureService.GetCoordByLen(curve.Start_Km, curve.Start_M, interval, curve.Track_Id, curve.Start_Date).Meter;
        }

        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                var trips = RdStructureService.GetMainParametersProcesses(period, parentId, true);
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report", new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                    new XAttribute("distance", ((AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId)).Name));
                int i = 1;
                foreach (var trip in trips)
                {
                    List<Curve> curves = RdStructureService.GetCurvesInTrip(trip.Id) as List<Curve>;

                    var filterForm = new FilterForm();
                    var filters = new List<Filter>();
                    var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                    var min = curves.Select(o => o.Start_Km).Min();
                    var max = curves.Select(o => o.Final_Km).Max();

                    filters.Add(new FloatFilter() { Name = "Начало (км)", Value = min });
                    filters.Add(new FloatFilter() { Name = "Конец (км)", Value = max });

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
                        //var speed = MainTrackStructureService.GetMtoObjectsByCoord(trip.Date_Vrem, curve.Start_Km, MainTrackStructureConst.MtoSpeed, trip.DirectionName, "1") as List<Speed>;

                        //curve.Passspeed = speed.Count > 0 ? speed[0].Passenger : -1;
                        //curve.Freightspeed = speed.Count > 0 ? speed[0].Freight : -1;

                        //if ((int)curve.Passspeed <= 100)
                        //{
                        //    continue;
                        //}


                        CurveId = curve.Id;
                        string radiusPolyline = string.Empty;
                        string levelPolyline = string.Empty;
                        curve.Elevations = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();
                        curve.Straightenings = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();
                        

                        //if (curve.Passspeed <= 120)
                        //{
                        //    continue;
                        //}
                                

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
                        int levelH = 0;
                        int transitionLength1 = 0, transitionLength2 = 0;

                        radiusPolyline = "-50 0";
                        levelPolyline = "-50 0";

                        //y=15 болғандағы x мәні
                        var DxRad = 0;
                        var Level_Sdvig = 15.0;

                        //радиус бойынша жылжыту коефиценті
                        var RadKoef = 0;

                        //пасспорт
                        int x1R = 0;
                        int x2R = 0;
                        int x3R = 0;
                        int x4R = 0;

                        //белгілейтін радиус бойынша нүктелер
                        int X1naturRad = 0;
                        int X2naturRad = 0;
                        int X3naturRad = 0;
                        int X4naturRad = 0;

                        foreach (var stCurve in curve.Straightenings)
                        {
                            x1R = MainTrackStructureService.GetDistanceBetween2Coord(curve.Start_Km, curve.Start_M, stCurve.Start_Km, stCurve.Start_M, curve.Track_Id, curve.Start_Date) + 50;
                            x2R = x1R + stCurve.Transition_1;
                            int rH = Convert.ToInt32(17860 / stCurve.Radius);

                            if (stCurve.Radius > 1000 && stCurve.Radius < 1500)
                            {
                                Level_Sdvig = Level_Sdvig / 2.0;
                                DxRad = (int)((Math.Abs(x2R - x1R) * Level_Sdvig) / rH);
                            }
                            else if (stCurve.Radius > 1500)
                            {
                                Level_Sdvig = Level_Sdvig / 3.0;
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

                        //Расчет дистанции от паспорта до натурной линий
                        var k = plan[DxRad + x1R];

                        if (Level_Sdvig < k)
                        {
                            for (int ii = DxRad + x1R; ii > 0; ii--)
                            {
                                var CurrentY = plan[ii];

                                if (Level_Sdvig > CurrentY)
                                {
                                    RadKoef = -((x1R + DxRad) - ii);
                                    break;
                                }
                            }
                        }
                        else if (Level_Sdvig > k)
                        {
                            for (int ii = DxRad + x1R; ii < x3R; ii++)
                            {
                                try
                                {
                                    if (Level_Sdvig < plan[ii])
                                    {
                                        RadKoef = ii - (x1R + DxRad);
                                        break;
                                    }
                                }
                                catch
                                {
                                    RadKoef = ii - (x1R + DxRad);
                                    break;
                                }
                            }
                        }
                        X1naturRad = x1R + RadKoef < 0 ? 0 : x1R + RadKoef;
                        X2naturRad = x2R + RadKoef;
                        X3naturRad = x3R + RadKoef;
                        X4naturRad = x4R + RadKoef > plan.Count ? plan.Count - 1 : x4R + RadKoef;

                        //если длина круговой меньше 150м тогда пересчитываем

                        var LenKrug = Math.Abs(X2naturRad - X3naturRad);

                        if (LenKrug > 0 && LenKrug < 100)
                        {
                            var selectedPlanMaxData = plan.GetRange(X1naturRad, X4naturRad - X1naturRad);
                            var SelectedDataMaxValue = selectedPlanMaxData.Max();
                            var SelectedDataMaxValueInd = selectedPlanMaxData.IndexOf(selectedPlanMaxData.Max());


                            for (int ii = 0; ii < 20; ii++)
                            {
                                var DataR = new List<double> { };
                                var DataL = new List<double> { };

                                for (int kk = 0; kk < LenKrug; kk++)
                                {
                                    var PromR = 0.0;
                                    var PromL = 0.0;
                                    try
                                    {
                                        PromR = Math.Abs(-selectedPlanMaxData[SelectedDataMaxValueInd + kk] - ii + SelectedDataMaxValue);
                                        PromL = Math.Abs(-selectedPlanMaxData[SelectedDataMaxValueInd - kk] - ii + SelectedDataMaxValue);
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    DataR.Add(PromR);
                                    DataL.Add(PromL);
                                }
                                //нийти пересечение прямой с кривой на круговой

                                var DataRMinIndex = DataR.IndexOf(DataR.Min());
                                var DataLMinIndex = DataL.IndexOf(DataL.Min());

                                var dd = DataRMinIndex + DataLMinIndex;

                                if (LenKrug < dd)
                                {
                                    X2naturRad = (int)(X1naturRad + SelectedDataMaxValueInd - DataLMinIndex);
                                    X3naturRad = (int)(X1naturRad + SelectedDataMaxValueInd + DataRMinIndex);

                                    break;
                                }
                            }
                        }

                        //-----------------------

                        //List<double> rdcs_radius_data = new List<double>();

                        //445
                        //var rdcs_radius_data = rdcs.Select(p => (double)p.Radius).ToList();

                        //List<double> strightTransitionPoints;

                        //var StrightAvgTrapezoidData = rdcs_radius_data.GetTrapezoid(new List<double> { }, new List<double> { }, 4, out strightTransitionPoints);

                        //string rA = "";
                        //for (int ii = 0; ii < StrightAvgTrapezoidData.Count; ii++)
                        //{
                        //    rA += ii + "," + (-StrightAvgTrapezoidData[ii]).ToString().Replace(",", ".") + " ";
                        //}


                        rdcsData = new Data(x, plan, level, gauge, passBoost, freightBoost, passSpeed, freightSpeed, transitionLength1, transitionLength2);



                        //if (rdcsExisting && minX > rdcsData.X1IndexPlan - rdcsData.X1LengthPlan)
                        //{
                        //    maxX = maxX + minX - (rdcsData.X1IndexPlan - rdcsData.X1LengthPlan);
                        //    minX = (rdcsData.X1IndexPlan - rdcsData.X1LengthPlan);
                        //}

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
                        XElement xAxisKmLabels = new XElement("kmlabels", new XElement("label", new XAttribute("value", curve.Start_Km), new XAttribute("style", "display:inline;position: absolute;font-size:12px;Left:0")));
                        XElement xeXaxis = new XElement("xaxis", new XElement("xparam",
                            new XAttribute("minX", minX - 10),
                            new XAttribute("maxX", maxX + 10))
                        );

                        var kmCoords = rdcs.Select(rdc => rdc.M == 1).ToList();
                        int currentKm = curve.Start_Km;

                        for (int XCurrentPosition = RoundNum(minX) + 1; XCurrentPosition <= maxX; XCurrentPosition += xAxisInterval)
                        {
                            if (XCurrentPosition < rdcs.Count)
                            {
                                xeXaxis.Add(new XElement("line",
                                    new XAttribute("x1", XCurrentPosition),
                                    new XAttribute("y2", (-radiusH - 5)),
                                    new XAttribute("y2-level", (-levelH - 5))
                                    )
                                );
                                xAxisLabels.Add(new XElement("label",
                                    new XAttribute("value", rdcs[XCurrentPosition].M),
                                    new XAttribute("style", "display:inline;position: absolute;font-size:12px;Left:" + Convert.ToInt32(((XCurrentPosition - minX + 10) * intervalKoef) - 5))));
                                if (rdcs[XCurrentPosition].Km != currentKm)
                                {

                                    currentKm = rdcs[XCurrentPosition].Km;
                                    xAxisKmLabels.Add(new XElement("label", new XAttribute("value", currentKm), new XAttribute("style", "display:inline;position: absolute;font-size:12px;Left:" + Convert.ToInt32((XCurrentPosition) * intervalKoef))));
                                }
                            }
                        }


                        xeXaxis.Add(xAxisLabels);
                        xeXaxis.Add(xAxisKmLabels);
                        float widthR = (width + 20) * 0.0045f;
                        radiusH = 0;
                        levelH = 0;
                        foreach (var stCurve in curve.Straightenings)
                        {
                            int rH = Convert.ToInt32(17860 / stCurve.Radius);
                            float heightR = (Convert.ToInt32(17860 / curve.Straightenings.Min(s => s.Radius)) + 10) * 0.1f;

                            try
                            {
                                if (StrPoins.Count() > 4 && (StrPoins[1].X - StrPoins[0].X) >= (StrPoins[4].X - StrPoins[3].X) || StrPoins.Count() == 4)
                                {
                                    xeXaxis.Add(new XElement("rectangle",
                                    new XAttribute("x", StrPoins[0].X),
                                    new XAttribute("y", -Math.Abs(StrPoins[0].Radius)), //ToDo
                                    new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                    new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                                }
                                xeXaxis.Add(new XElement("rectangle",
                                new XAttribute("x", StrPoins[1].X),
                                new XAttribute("y", -Math.Abs(StrPoins[1].Radius)), //ToDo
                                new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));


                                if (stCurve.Transition_2 != 0)
                                {
                                    xeXaxis.Add(new XElement("rectangle",
                                        new XAttribute("x", StrPoins[2].X),
                                        new XAttribute("y", -Math.Abs(StrPoins[2].Radius)), //ToDo
                                        new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                        new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                                    xeXaxis.Add(new XElement("rectangle",
                                        new XAttribute("x", StrPoins[3].X),
                                        new XAttribute("y", -Math.Abs(StrPoins[3].Radius)), //ToDo
                                        new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                        new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                                    if (StrPoins.Count() > 4 && (StrPoins[1].X - StrPoins[0].X) <= (StrPoins[4].X - StrPoins[3].X) || StrPoins.Count() == 4)
                                    {
                                        xeXaxis.Add(new XElement("rectangle",
                                        new XAttribute("x", StrPoins[4].X),
                                        new XAttribute("y", -Math.Abs(StrPoins[4].Radius)), //ToDo
                                        new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                        new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                                    }
                                }
                                radiusH = rH;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("curve.Straightenings " + e.Message);
                            }
                        }


                        foreach (var elCurve in curve.Elevations)
                        {
                            int lH = Convert.ToInt32(Math.Abs(elCurve.Lvl));
                            float heightR = (Convert.ToInt32(curve.Elevations.Max(c => Math.Abs(c.Lvl))) + 10) * 0.1f;

                            try
                            {
                                if (LevelPoins.Count() > 4 && (LevelPoins[1].X - LevelPoins[0].X) >= (LevelPoins[4].X - LevelPoins[3].X) || LevelPoins.Count() == 4)
                                {
                                    xeXaxis.Add(new XElement("rectangle_lvl",
                                    new XAttribute("x", LevelPoins[0].X),
                                    new XAttribute("y", -Math.Abs(LevelPoins[0].Level)), //ToDo
                                    new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                    new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                                }
                                xeXaxis.Add(new XElement("rectangle_lvl",
                                    new XAttribute("x", LevelPoins[1].X),
                                    new XAttribute("y", -Math.Abs(LevelPoins[1].Level)), //ToDo
                                    new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                    new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));

                                if (elCurve.Transition_2 != 0)
                                {
                                    xeXaxis.Add(new XElement("rectangle_lvl",
                                        new XAttribute("x", LevelPoins[2].X),
                                        new XAttribute("y", -Math.Abs(LevelPoins[2].Level)), //ToDo
                                        new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                        new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                                    xeXaxis.Add(new XElement("rectangle_lvl",
                                        new XAttribute("x", LevelPoins[3].X),
                                        new XAttribute("y", -Math.Abs(LevelPoins[3].Level)), //ToDo
                                        new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                        new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                                    if (LevelPoins.Count() > 4 && (LevelPoins[1].X - LevelPoins[0].X) <= (LevelPoins[4].X - LevelPoins[3].X) || LevelPoins.Count() == 4)
                                    {
                                        xeXaxis.Add(new XElement("rectangle_lvl",
                                        new XAttribute("x", LevelPoins[4].X),
                                        new XAttribute("y", -Math.Abs(LevelPoins[4].Level)), //ToDo
                                        new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                        new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                                    }
                                }

                                levelH = lH;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("curve.Straightenings " + e.Message);
                            }
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
                            double topValue = 2 * 18 + 130 - 20 * koef;
                            marks.Add(new XElement("mark",
                                new XAttribute("topValue", -topValue),
                                new XAttribute("sign", 15)));
                            topValue = 3 * 18 + 130 - 15 * koef;
                            marks.Add(new XElement("mark",
                                new XAttribute("topValue", -topValue),
                                new XAttribute("sign", 10)));
                            topValue = 4 * 18 + 130 - 11 * koef;
                            marks.Add(new XElement("mark",
                                new XAttribute("topValue", -topValue),
                                new XAttribute("sign", 6)));
                        }
                        else if (-10 > -radiusH - 5)
                        {
                            double koef = 127.0 / (radiusH + 10);
                            double topValue = 2 * 18 + 130 - 15 * koef;
                            marks.Add(new XElement("mark",
                                new XAttribute("topValue", -topValue),
                                new XAttribute("sign", 10)));
                            topValue = 3 * 18 + 130 - 11 * koef;
                            marks.Add(new XElement("mark",
                                new XAttribute("topValue", -topValue),
                                new XAttribute("sign", 6)));
                        }
                        else if (-6 > -radiusH - 5)
                        {
                            double koef = 127.0 / (radiusH + 10);
                            double topValue = 2 * 18 + 130 - 11 * koef;
                            marks.Add(new XElement("mark",
                                new XAttribute("topValue", -topValue),
                                new XAttribute("sign", 6)));
                        }

                        //Уровень номер
                        if (-100 > -levelH - 5)
                        {
                            double koef = 127.0 / (levelH + 10);
                            double topValue = 2 * 18 + 130 - 105 * koef;
                            marks.Add(new XElement("markLvl",
                                new XAttribute("topValue", -topValue),
                                new XAttribute("sign", 100)));
                            topValue = 3 * 18 + 130 - 55 * koef;
                            marks.Add(new XElement("markLvl",
                                new XAttribute("topValue", -topValue),
                                new XAttribute("sign", 50)));
                        }
                        else if (-50 > -levelH - 5)
                        {
                            double koef = 127.0 / (levelH + 10);
                            double topValue = 2 * 18 + 130 - 55 * koef;
                            marks.Add(new XElement("markLvl",
                                new XAttribute("topValue", -topValue),
                                new XAttribute("sign", 50)));
                        }

                        float defaultRetractionSlopePlanLeft = radiusH * 1f / transitionLength1;
                        float defaultRetractionSlopePlanRight = radiusH * 1f / transitionLength2;
                        float defaultRetractionSlopeLevelLeft = levelH * 1f / transitionLength1;
                        float defaultRetractionSlopeLevelRight = levelH * 1f / transitionLength2;
                        int[] passSpeeds = new int[] { 0, 0, 0 };
                        int[] freightSpeeds = new int[] { 0, 0, 0 };

                        passSpeeds[0] = rdcsData.GetCriticalSpeed();
                        passSpeeds[1] = rdcsData.GetPRSpeed()[0];
                        passSpeeds[2] = rdcsData.GetIZPassSpeed();

                        freightSpeeds[0] = rdcsData.GetCriticalSpeed() > 90 ? 90 : rdcsData.GetCriticalSpeed() - Convert.ToInt32(rdcsData.GetCriticalSpeed() * 0.03);
                        freightSpeeds[1] = rdcsData.GetPRSpeed()[1];
                        freightSpeeds[2] = rdcsData.GetIZFreightSpeed();

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
                        XElement xeCurve = new XElement("curve",
                             new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("date_trip", trip.Date_Vrem.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))),
                            new XAttribute("site", site),
                            new XAttribute("road", roadName),
                            new XAttribute("type", trip.GetProcessTypeName),
                            new XAttribute("period", period.Period),
                            new XAttribute("track", curvesAdmUnit != null ? curvesAdmUnit.Track : "-"),
                            new XAttribute("direction", curvesAdmUnit != null ? curvesAdmUnit.Direction : "-"),
                            new XAttribute("km", curve.Start_Km.ToString() + " - " + curve.Final_Km.ToString()),
                            new XAttribute("side", curve.Side == "Левая" ? "Правая" : "Левая"),
                            new XAttribute("order", i.ToString()),
                            new XAttribute("PC", trip.Car),
                            new XAttribute("radius", radiusPolyline + ", " + (maxX + 20) + " 0"), //пасспортная линия
                            new XAttribute("radius-average", 0 + ",0 " + radiusAverage + " " + (maxX + 20) + ",0"), //натурная линия
                                                                                                                    //new XAttribute("radius-average", 0 + ",0 " + rA + " " + (maxX + 20) + ",0"),
                            new XAttribute("gauge", 0 + ",0 " + gaugeAverage + " " + (maxX + 20) + ",0"),
                            new XAttribute("passboost", 0 + ",0 " + passboost + " " + (maxX + 20) + ",0"),
                            new XAttribute("freightboost", 0 + ",0 " + freightboost + " " + (maxX + 20) + ",0"),
                            new XAttribute("viewbox", (minX - 10) + " " + (-radiusH - 5) + " " + (width + 20) + " " + (radiusH + 10)),
                            new XAttribute("radius-length", radiusLength),
                            new XAttribute("level", levelPolyline + ", " + (maxX + 20) + " 0"),
                            new XAttribute("level-average", 0 + ",0 " + levelAverage + " " + (maxX + 20) + ",0"),
                            new XAttribute("viewbox-level", (minX - 10) + " " + (-levelH - 5) + " " + (width + 20) + " " + (levelH + 10)),
                            new XAttribute("boost-level", (minX - 10) + " " + (-1) + " " + (width + 20) + " 1.1")
                            );

                        if (curve.Straightenings.Count > 1)
                        {
                            xeCurve.Add(new XAttribute("ismulti", "true"));
                            int ind = 1;
                            foreach (StCurve stCurve in curve.Straightenings)
                            {
                                Data mRdcsData = new Data();
                                XElement xeMultiCurves = new XElement("multicurves");
                                float defaultRetractionSlopeLeft = stCurve.Transition_1 != 0 ? radiusH * 1f / stCurve.Transition_1 : 0;
                                float defaultRetractionSlopeRight = stCurve.Transition_2 != 0 ? radiusH * 1f / stCurve.Transition_2 : 0;
                                x = new List<int>();
                                passSpeed = new List<int>();
                                freightSpeed = new List<int>();
                                plan = new List<float>();
                                level = new List<float>();
                                gauge = new List<float>();
                                passBoost = new List<float>();
                                freightBoost = new List<float>();
                                foreach (var rdc in rdcs.Where(r => r.GetRealCoordinate() >= stCurve.RealStartCoordinate && r.GetRealCoordinate() <= stCurve.RealFinalCoordinate))
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
                                mRdcsData = new Data(x, plan, level, gauge, passBoost, freightBoost, passSpeed, freightSpeed, transitionLength1, transitionLength2);
                                xeMultiCurves.Add(new XAttribute("order", ind),
                                    new XAttribute("start_km", stCurve.Start_Km),
                                    new XAttribute("start_m", stCurve.Start_M),
                                    new XAttribute("final_km", stCurve.Final_Km),
                                    new XAttribute("final_m", stCurve.Final_M),
                                    new XAttribute("start_lvl", matchCurveCoords.OrderBy(m => Math.Abs(m.StAbsCoords)).FirstOrDefault().StElDifference),
                                    new XAttribute("final_lvl", matchCurveCoords.OrderBy(m => Math.Abs(m.StAbsCoords)).FirstOrDefault().StElDifference),

                                    new XAttribute("len", Math.Abs(stCurve.Start_Km * 1000 + stCurve.Start_M - stCurve.Final_Km * 1000 - stCurve.Final_M)),
                                    new XAttribute("len_lvl", ""),
                                    new XAttribute("radius", rdcsData.GetAvgPlan()),
                                    new XAttribute("lvl", rdcsData.GetAvgLevel()),
                                    new XAttribute("midtap", rdcsData.GetPlanLeftAvgRetractionSlope()),
                                    new XAttribute("midtap_lvl", rdcsData.GetPlanRightAvgRetractionSlope()),
                                    new XAttribute("len2", stCurve.Transition_1),
                                    new XAttribute("len2_lvl", stCurve.Transition_2),
                                    new XAttribute("anp", rdcsData.GetUnliquidatedAccelerationPassengerAvg().ToString("f2", System.Globalization.CultureInfo.InvariantCulture) + "\\" + rdcsData.GetUnliquidatedAccelerationPassengerMax().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("anp2", rdcsData.GetUnliquidatedAccelerationFreightAvg().ToString("f2", System.Globalization.CultureInfo.InvariantCulture) + "\\" + rdcsData.GetUnliquidatedAccelerationFreightMax().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("psi", rdcsData.BoostChangeRateMax().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("pass1", rdcsData.GetPassSpeed().ToString()),
                                    new XAttribute("pass2", RoundNumToFive(Math.Min(Math.Min(rdcsData.GetCriticalSpeed(), rdcsData.GetPRSpeed()[0]), rdcsData.GetIZPassSpeed())).ToString()),
                                    new XAttribute("frei1", rdcsData.GetFreightSpeed().ToString()),
                                    new XAttribute("frei2", RoundNumToFive(Math.Min(Math.Min(rdcsData.GetCriticalSpeed() > 90 ? 90 : rdcsData.GetCriticalSpeed() - Convert.ToInt32(rdcsData.GetCriticalSpeed() * 0.03), rdcsData.GetPRSpeed()[1]), rdcsData.GetIZFreightSpeed())).ToString()));

                                xeCurve.Add(xeMultiCurves);
                                ind++;
                            }
                        }

                        int len = rdcsData.X3IndexPlan - rdcsData.X0IndexPlan;
                        int len2 = rdcsData.X2IndexPlan - rdcsData.X1IndexPlan;
                        int gaugeStandard = curve.Straightenings.Max(s => s.Width);
                        curve.Radius = Convert.ToInt32(curve.Straightenings.Max(s => s.Radius));

                        var lenPerKriv10000 = Math.Abs((StrPoins[1].Km + StrPoins[1].M / 10000.0) - (StrPoins[2].Km + StrPoins[2].M / 10000.0)) * 10000;
                        var lenPerKriv = (int)lenPerKriv10000 % 1000;

                        var lenKriv10000 = Math.Abs((StrPoins[0].Km + StrPoins[0].M / 10000.0) - (StrPoins[3].Km + StrPoins[3].M / 10000.0)) * 10000;
                        var lenKriv = (int)lenKriv10000 % 1000;

                        var lenPerKriv10000lv = Math.Abs((LevelPoins[1].Km + LevelPoins[1].M / 10000.0) - (LevelPoins[2].Km + LevelPoins[2].M / 10000.0)) * 10000;
                        var lenPerKrivlv = (int)lenPerKriv10000lv % 1000;

                        var lenKriv10000lv = Math.Abs((LevelPoins[0].Km + LevelPoins[0].M / 10000.0) - (LevelPoins[3].Km + LevelPoins[3].M / 10000.0)) * 10000;
                        var lenKrivlv = (int)lenKriv10000lv % 1000;

                        try
                        {
                            var d = false;
                            if ((StrPoins[0].Km + StrPoins[0].M / 10000.0) > (StrPoins[3].Km + StrPoins[3].M / 10000.0))
                                d = true;

                            XElement paramCurve = new XElement("param_curve",
                                new XAttribute("start_km", d ? StrPoins.Last().Km : StrPoins.First().Km),
                                new XAttribute("start_m", d ? StrPoins.Last().M : StrPoins.First().M),
                                new XAttribute("final_km", d ? StrPoins.First().Km : StrPoins.Last().Km),
                                new XAttribute("final_m", d ? StrPoins.First().M : StrPoins.Last().M),

                                new XAttribute("start_lvl", d ? LevelPoins.Last().M : LevelPoins.First().M),
                                new XAttribute("final_lvl", d ? LevelPoins.First().M : LevelPoins.Last().M),

                                new XAttribute("len", lenKriv),
                                new XAttribute("len_lvl", lenKrivlv),

                                new XAttribute("angle", CurveAngle(curve.Radius, lenPerKriv).ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))));
                            XElement paramCircleCurve = new XElement("param_circle_curve",
                                /*new XAttribute("pr", "н"),
                                new XAttribute("sl", "н"),*/
                                new XAttribute("start_km", d ? StrPoins[2].Km : StrPoins[1].Km),
                                new XAttribute("start_m", d ? StrPoins[2].M : StrPoins[1].M),
                                new XAttribute("final_km", d ? StrPoins[1].Km : StrPoins[2].Km),
                                new XAttribute("final_m", d ? StrPoins[1].M : StrPoins[2].M),

                                new XAttribute("start_lvl", d ? LevelPoins[2].M : LevelPoins[1].M),
                                new XAttribute("final_lvl", d ? LevelPoins[1].M : LevelPoins[2].M),

                                new XAttribute("len", lenPerKriv),
                                new XAttribute("len_lvl", lenPerKrivlv),

                                new XAttribute("rad_min", rdcsData.GetMinPlan()),
                                new XAttribute("rad_max", rdcsData.GetMaxPlan()),
                                new XAttribute("rad_mid", rdcsData.GetAvgPlan()),

                                new XAttribute("lvl_min", rdcsData.GetMinLevel()),
                                new XAttribute("lvl_max", rdcsData.GetMaxLevel()),
                                new XAttribute("lvl_mid", rdcsData.GetAvgLevel()),

                                new XAttribute("gauge_min", gaugeStandard + rdcsData.GetMaxGauge()),
                                new XAttribute("gauge_max", gaugeStandard + rdcsData.GetMinGauge()),
                                new XAttribute("gauge_mid", gaugeStandard + rdcsData.GetAvgGauge()));

                            //данные
                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyCurve(curve, trip.Id);
                            if (DBcrossRailProfile == null) continue;

                            var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(DBcrossRailProfile);

                            XElement sideWear = new XElement("side_wear",
                                new XAttribute("mm6", rdcsData.Get6mmWear()),
                                new XAttribute("mm10", rdcsData.Get10mmWear()),
                                new XAttribute("mm15", rdcsData.Get15mmWear()),
                                new XAttribute("max", rdcsData.GetMaxWear().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                new XAttribute("mid", rdcsData.GetAvgWear().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)));
                            XElement withdrawal = new XElement("withdrawal",
                                new XAttribute("tap_max1", rdcsData.GetPlanLeftMaxRetractionSlope().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                new XAttribute("tap_mid1", rdcsData.GetPlanLeftAvgRetractionSlope().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                new XAttribute("tap_len1", rdcsData.X1LengthPlan),
                                new XAttribute("tap_max1_lvl", rdcsData.GetLevelLeftMaxRetractionSlope().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                new XAttribute("tap_mid1_lvl", rdcsData.GetLevelLeftAvgRetractionSlope().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                new XAttribute("tap_len1_lvl", rdcsData.X1LengthLevel),
                                new XAttribute("tap_max2", (rdcsData.GetPlanRightMaxRetractionSlope()).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                new XAttribute("tap_mid2", (rdcsData.GetPlanRightAvgRetractionSlope()).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                new XAttribute("tap_len2", rdcsData.X2LengthPlan),
                                new XAttribute("tap_max2_lvl", rdcsData.GetLevelRightMaxRetractionSlope().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                new XAttribute("tap_mid2_lvl", rdcsData.GetLevelRightAvgRetractionSlope().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                new XAttribute("tap_len2_lvl", rdcsData.X2LengthLevel));
                            XElement computing = new XElement("computing",
                                new XAttribute("a1", rdcsData.GetUnliquidatedAccelerationPassengerAvg().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                new XAttribute("a2", rdcsData.GetUnliquidatedAccelerationPassengerMax().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
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
                                new XAttribute("pass2", passSpeeds[0].ToString()),
                                new XAttribute("pass3", passSpeeds[1].ToString()),
                                new XAttribute("pass4", passSpeeds[2].ToString()),
                                new XAttribute("pass5", "-"),
                                new XAttribute("pass6", RoundNumToFive(passSpeeds.Min()).ToString()),
                                new XAttribute("frei1", rdcsData.GetFreightSpeed().ToString()),
                                new XAttribute("frei2", freightSpeeds[0].ToString()),
                                new XAttribute("frei3", freightSpeeds[1].ToString()),
                                new XAttribute("frei4", freightSpeeds[2].ToString()),
                                new XAttribute("frei5", "-"),
                                new XAttribute("frei6", RoundNumToFive(freightSpeeds.Min()).ToString()));
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
                xdReport.Add(report);
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report.html");
                //var word = new Microsoft.Office.Interop.Word.Application();
                //word.Visible = false;

                //var filePath = Path.GetTempPath() + "/report.html";
                //var savePathPdf = Path.GetTempPath() + "/report.docx";
                //var wordDoc = word.Documents.Open(FileName: filePath, ReadOnly: false);
                //wordDoc.SaveAs2(FileName: savePathPdf, FileFormat: WdSaveFormat.wdFormatXMLDocument);
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

