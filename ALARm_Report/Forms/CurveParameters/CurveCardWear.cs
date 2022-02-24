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
    public class CurveCardWear : Report
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
            float wear = -1;
            using (var filterForm = new FilterForm())
            {
                var filters = new List<Filter>();
                filters.Add(new FloatFilter() { Name = "Порог износа более: ", Value = wear });
                filterForm.SetDataSource(filters);
                if (filterForm.ShowDialog() == DialogResult.Cancel)
                    return;
                wear = (float)(float)filters[0].Value;
            }
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report",
                     new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                     new XAttribute("distance", ((AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId)).Code),
                     new XAttribute("wear", wear));
                int i = 1;

                var trips = RdStructureService.GetMainParametersProcesses(period, parentId, true);
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
                    curves = curves.Where(o => ((float)filters[0].Value <= o.Start_Km && o.Final_Km <= (float)filters[1].Value)).ToList();

                    foreach (var curve in curves)
                    {
                      
                        List<RDCurve> rdcs = RdStructureService.GetRDCurves(curve.Id, trip.Id); 
                       
                        CurveId = curve.Id;

                        string radiusPolyline = string.Empty;
                        string levelPolyline = string.Empty;
                        curve.Elevations =
                            (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();
                        curve.Straightenings =
                            (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();

                        if (curve.Straightenings.Count < 1)
                            continue;

                        if (curve.Straightenings.Max(s => s.Wear) < wear)
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

                        foreach (var elCurve in curve.Elevations)
                        {
                            var match = matchCurveCoords.OrderBy(m => Math.Abs(elCurve.Start_Km * 1000 + elCurve.Start_M - m.StAbsCoords)).First();
                            match.StElDifference = Math.Abs(elCurve.Start_Km * 1000 + elCurve.Start_M - match.StAbsCoords);
                            match.Lvl = Convert.ToInt32(Math.Abs(elCurve.Lvl));

                            match = matchCurveCoords.OrderBy(m => Math.Abs(elCurve.Start_Km * 1000 + elCurve.Start_M + elCurve.Transition_1 - m.StAbsCoords)).First();
                            match.StElDifference = Math.Abs(elCurve.Start_Km * 1000 + elCurve.Start_M + elCurve.Transition_1 - match.StAbsCoords);
                            match.Lvl = Convert.ToInt32(Math.Abs(elCurve.Lvl));

                            match = matchCurveCoords.OrderBy(m => Math.Abs(elCurve.Final_Km * 1000 + elCurve.Final_M - m.StAbsCoords)).First();
                            match.StElDifference = Math.Abs(elCurve.Final_Km * 1000 + elCurve.Final_M - match.StAbsCoords);
                            match.Lvl = Convert.ToInt32(Math.Abs(elCurve.Lvl));

                            match = matchCurveCoords.OrderBy(m => Math.Abs(elCurve.Final_Km * 1000 + elCurve.Final_M - elCurve.Transition_2 - m.StAbsCoords)).First();
                            match.StElDifference = Math.Abs(elCurve.Final_Km * 1000 + elCurve.Final_M - elCurve.Transition_2 - match.StAbsCoords);
                            match.Lvl = Convert.ToInt32(Math.Abs(elCurve.Lvl));
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
                        foreach (var stCurve in curve.Straightenings)
                        {
                            int x1 = MainTrackStructureService.GetDistanceBetween2Coord(curve.Start_Km, curve.Start_M, stCurve.Start_Km, stCurve.Start_M, curve.Track_Id, curve.Start_Date) + 50;
                            int x2 = x1 + stCurve.Transition_1;
                            int rH = Convert.ToInt32(17860 / stCurve.Radius);

                            radiusPolyline += ", " + x1 + " " + -radiusH;
                            radiusPolyline += ", " + x2 + " " + -rH;

                            if (stCurve.Transition_2 != 0)
                            {
                                int x3 = MainTrackStructureService.GetDistanceBetween2Coord(curve.Start_Km, curve.Start_M, stCurve.Final_Km, stCurve.Final_M, curve.Track_Id, curve.Start_Date) + 50;
                                int x4 = x3 - stCurve.Transition_2;

                                radiusPolyline += ", " + x4 + " " + -rH;
                                radiusPolyline += ", " + x3 + " 0";
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

                        /*foreach (var elCurve in curve.Radiuses)
                        {
                            int x1 = (elCurve.Start_Km * 1000 + elCurve.Start_M);
                            int x2 = (elCurve.Lvl_start_km * 1000 + elCurve.Lvl_start_m);
                            int x3 = (elCurve.Final_Km * 1000 + elCurve.Final_M);
                            int x4 = (elCurve.Lvl_final_km * 1000 + elCurve.Lvl_final_m);

                            var nonStandardkm = MainTrackStructureService.GetNonStandardKm(curve.Id, MainTrackStructureConst.MtoCurve, elCurve.Start_Km);
                            transitionLength1 = GetTransitionLength(elCurve.Start_Km, elCurve.Start_M, elCurve.Lvl_start_km, elCurve.Lvl_start_m, nonStandardkm);


                            if (elCurve.Lvl_final_km != elCurve.Start_Km)
                                nonStandardkm = MainTrackStructureService.GetNonStandardKm(curve.Id, MainTrackStructureConst.MtoCurve, elCurve.Final_Km);
                            transitionLength2 = GetTransitionLength(elCurve.Lvl_final_km, elCurve.Lvl_final_m, elCurve.Final_Km, elCurve.Final_M, nonStandardkm);


                            if (minX < 0)
                            {
                                minX = x1 - 10;
                                maxX = x3 + 10;
                                width = Math.Abs(maxX - minX);
                                radiusLength = Convert.ToInt32(elCurve.Radius);
                            }

                            int rH = radiusH + Convert.ToInt32(17860 / elCurve.Radius);
                            int lH = levelH + Convert.ToInt32(elCurve.Lvl);

                            radiusStart += " " + x1 + "," + -radiusH;
                            radiusStart += " " + x2 + "," + -rH;
                            radiusFinal = " " + x3 + "," + -radiusH + radiusFinal;
                            radiusFinal = " " + x4 + "," + -rH + radiusFinal;
                            radiusH += rH;

                            levelStart += " " + x1 + "," + -levelH;
                            levelStart += " " + x2 + "," + -lH;
                            levelFinal = " " + x3 + "," + -levelH + levelFinal;
                            levelFinal = " " + x4 + "," + -lH + levelFinal;
                            levelH += lH;
                        }*/

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

                        if (minX > rdcsData.X1IndexPlan - rdcsData.X1LengthPlan)
                        {
                            maxX = maxX + minX - (rdcsData.X1IndexPlan - rdcsData.X1LengthPlan);
                            minX = (rdcsData.X1IndexPlan - rdcsData.X1LengthPlan);
                        }

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

                        int currentKm = curve.Start_Km;
                                  for (int XCurrentPosition = RoundNum(minX); XCurrentPosition <= maxX; XCurrentPosition += xAxisInterval)
                        {
                            if (XCurrentPosition >= rdcs.Count())
                                break;
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
                        xeXaxis.Add(xAxisLabels);
                        xeXaxis.Add(xAxisKmLabels);
                        float widthR = (width + 20) * 0.0045f;
                        radiusH = 0;
                        levelH = 0;
                        foreach (var stCurve in curve.Straightenings)
                        {
                            int rH = Convert.ToInt32(17860 / stCurve.Radius);
                            float heightR = (Convert.ToInt32(17860 / curve.Straightenings.Min(s => s.Radius)) + 10) * 0.1f;
                            xeXaxis.Add(new XElement("rectangle",
                                new XAttribute("x", rdcsData.X0IndexPlan),
                                new XAttribute("y", -(rdcsData.Y0IndexPlan + heightR / 2)),
                                new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                            xeXaxis.Add(new XElement("rectangle",
                                new XAttribute("x", rdcsData.X1IndexPlan),
                                new XAttribute("y", -rdcsData.Y1IndexPlan),
                                new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));

                            if (stCurve.Transition_2 != 0)
                            {
                                xeXaxis.Add(new XElement("rectangle",
                                    new XAttribute("x", rdcsData.X2IndexPlan ),
                                    new XAttribute("y", -rdcsData.Y2IndexPlan),
                                    new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                    new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                                xeXaxis.Add(new XElement("rectangle",
                                    new XAttribute("x", rdcsData.X3IndexPlan),
                                    new XAttribute("y", -(rdcsData.Y3IndexPlan + heightR / 2)),
                                    new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                    new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                            }

                            radiusH = rH;
                        }
                        /*xeXaxis.Add(new XElement("rectangle",
                            new XAttribute("x", rdcsExisting ? rdcsData.X0IndexPlan : int.Parse(radiusStart.Split(new char[] { ',', ' ' })[1]) - (widthR / 2)),
                            new XAttribute("y", (-(rdcsExisting ? rdcsData.Y0IndexPlan + heightR / 2 : heightR / 2)).ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                        xeXaxis.Add(new XElement("rectangle",
                            new XAttribute("x", rdcsExisting ? rdcsData.X1IndexPlan : int.Parse(radiusStart.Split(new char[] { ',', ' ' })[3]) - (widthR / 2)),
                            new XAttribute("y", (rdcsExisting ? -rdcsData.Y1IndexPlan : int.Parse(radiusStart.Split(new char[] { ',', ' ' })[4])) - (heightR / 2)),
                            new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                        xeXaxis.Add(new XElement("rectangle",
                            new XAttribute("x", rdcsExisting ? rdcsData.X2IndexPlan : int.Parse(radiusFinal.Split(new char[] { ',', ' ' })[1]) - (widthR / 2)),
                            new XAttribute("y", (rdcsExisting ? -rdcsData.Y2IndexPlan : int.Parse(radiusFinal.Split(new char[] { ',', ' ' })[2])) - (heightR / 2)),
                            new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                        xeXaxis.Add(new XElement("rectangle",
                            new XAttribute("x", rdcsExisting ? rdcsData.X3IndexPlan : int.Parse(radiusFinal.Split(new char[] { ',', ' ' })[3]) - (widthR / 2)),
                            new XAttribute("y", (-(rdcsExisting ? rdcsData.Y3IndexPlan + heightR / 2 : heightR / 2)).ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));*/

                        foreach (var elCurve in curve.Elevations)
                        {
                            int lH = Convert.ToInt32(Math.Abs(elCurve.Lvl));
                            float heightR = (Convert.ToInt32(curve.Elevations.Max(c => Math.Abs(c.Lvl))) + 10) * 0.1f;
                            xeXaxis.Add(new XElement("rectangle_lvl",
                                new XAttribute("x", rdcsData.X0IndexLevel ),
                                new XAttribute("y", -(rdcsData.Y0IndexLevel + heightR / 2 )),
                                new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                            xeXaxis.Add(new XElement("rectangle_lvl",
                                new XAttribute("x", rdcsData.X1IndexLevel),
                                new XAttribute("y", -rdcsData.Y1IndexLevel),
                                new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));

                            if (elCurve.Transition_2 != 0)
                            {
                                xeXaxis.Add(new XElement("rectangle_lvl",
                                    new XAttribute("x", rdcsData.X2IndexLevel),
                                    new XAttribute("y", -rdcsData.Y2IndexLevel),
                                    new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                    new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                                xeXaxis.Add(new XElement("rectangle_lvl",
                                    new XAttribute("x", rdcsData.X3IndexLevel),
                                    new XAttribute("y", rdcsData.Y3IndexLevel + heightR / 2),
                                    new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                    new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                            }

                            levelH = lH;
                        }
                        /*xeXaxis.Add(new XElement("rectangle_lvl",
                            new XAttribute("x", rdcsExisting ? rdcsData.X0IndexLevel : int.Parse(levelStart.Split(new char[] { ',', ' ' })[1]) - (widthR / 2)),
                            new XAttribute("y", (-(rdcsExisting ? rdcsData.Y0IndexLevel + heightR / 2 : heightR / 2)).ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                        xeXaxis.Add(new XElement("rectangle_lvl",
                            new XAttribute("x", rdcsExisting ? rdcsData.X1IndexLevel : int.Parse(levelStart.Split(new char[] { ',', ' ' })[3]) - (widthR / 2)),
                            new XAttribute("y", (rdcsExisting ? -rdcsData.Y1IndexLevel : int.Parse(levelStart.Split(new char[] { ',', ' ' })[4])) - (heightR / 2)),
                            new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                        xeXaxis.Add(new XElement("rectangle_lvl",
                            new XAttribute("x", rdcsExisting ? rdcsData.X2IndexLevel : int.Parse(levelFinal.Split(new char[] { ',', ' ' })[1]) - (widthR / 2)),
                            new XAttribute("y", (rdcsExisting ? -rdcsData.Y2IndexLevel : int.Parse(levelFinal.Split(new char[] { ',', ' ' })[2])) - (heightR / 2)),
                            new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));
                        xeXaxis.Add(new XElement("rectangle_lvl",
                            new XAttribute("x", rdcsExisting ? rdcsData.X3IndexLevel : int.Parse(levelFinal.Split(new char[] { ',', ' ' })[3]) - (widthR / 2)),
                            new XAttribute("y", (-(rdcsExisting ? rdcsData.Y3IndexLevel + heightR / 2 : heightR / 2)).ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("width", widthR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));*/

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
                            new XAttribute("road", roadName),
                            new XAttribute("type", trip.GetProcessTypeName),
                            new XAttribute("period", period.Period),
                            new XAttribute("track", curvesAdmUnit != null ? curvesAdmUnit.Track : "-"),
                            new XAttribute("date_trip", trip.Date_Vrem.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))),
                            new XAttribute("site", site),
                            //new XAttribute("road", curvesAdmUnit != null ? curvesAdmUnit.Track : "-"),
                            new XAttribute("direction", curvesAdmUnit != null ? curvesAdmUnit.Direction : "-"),
                            new XAttribute("km", curve.Start_Km.ToString() + " - " + curve.Final_Km.ToString()),
                            new XAttribute("side", curve.Side.ToLower()),
                            new XAttribute("order", i.ToString()),
                            new XAttribute("PC", trip.Car),
                            new XAttribute("radius", radiusPolyline + ", " + (maxX + 20) + " 0"),
                            new XAttribute("radius-average", 0 + ",0 " + radiusAverage + " " + (maxX + 20) + ",0"),
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
                                foreach (var rdc in rdcs.Where(r => r.X >= stCurve.Start_Km * 1000 + stCurve.Start_M && r.X <= stCurve.Final_Km * 1000 + stCurve.Final_M))
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
                                mRdcsData = new Data(x, plan, level, gauge, passBoost, freightBoost, passSpeed, freightSpeed, transitionLength1, transitionLength2);

                                xeMultiCurves.Add(new XAttribute("order", ind),
                                    new XAttribute("start_km", stCurve.Start_Km),
                                    new XAttribute("start_m", stCurve.Start_M),
                                    new XAttribute("final_km", stCurve.Final_Km),
                                    new XAttribute("final_m", stCurve.Final_M),
                                    new XAttribute("start_lvl", matchCurveCoords.OrderBy(m => Math.Abs(m.StAbsCoords)).FirstOrDefault().StElDifference),
                                    new XAttribute("final_lvl", matchCurveCoords.OrderBy(m => Math.Abs(m.StAbsCoords)).FirstOrDefault().StElDifference),
                                    new XAttribute("len", MainTrackStructureService.GetDistanceBetween2Coord(stCurve.Start_Km,stCurve.Start_M,stCurve.Final_Km,stCurve.Final_M,curve.Track_Id, curve.Start_Date)),
                                    new XAttribute("len_lvl", ""),
                                    new XAttribute("radius", rdcsData.GetAvgPlan()),
                                    new XAttribute("lvl", rdcsData.GetAvgLevel()),
                                    new XAttribute("midtap", rdcsData.GetPlanLeftAvgRetractionSlope().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("midtap_lvl", rdcsData.GetPlanRightAvgRetractionSlope().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("len2", stCurve.Transition_1),
                                    new XAttribute("len2_lvl", stCurve.Transition_2),
                                    new XAttribute("anp",  rdcsData.GetUnliquidatedAccelerationPassengerAvg().ToString("f2", System.Globalization.CultureInfo.InvariantCulture) + "\\" + rdcsData.GetUnliquidatedAccelerationPassengerMax().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("anp2", rdcsData.GetUnliquidatedAccelerationFreightAvg().ToString("f2", System.Globalization.CultureInfo.InvariantCulture) + "\\" + rdcsData.GetUnliquidatedAccelerationFreightMax().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("psi", rdcsData.BoostChangeRateMax().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                                    new XAttribute("pass1", rdcsData.GetPassSpeed().ToString()),
                                    new XAttribute("pass2",  RoundNumToFive(Math.Min(Math.Min(rdcsData.GetCriticalSpeed(), rdcsData.GetPRSpeed()[0]), rdcsData.GetIZPassSpeed())).ToString() ),
                                    new XAttribute("frei1", rdcsData.GetFreightSpeed().ToString() ),
                                    new XAttribute("frei2",  RoundNumToFive(Math.Min(Math.Min(rdcsData.GetCriticalSpeed() > 90 ? 90 : rdcsData.GetCriticalSpeed() - Convert.ToInt32(rdcsData.GetCriticalSpeed() * 0.03), rdcsData.GetPRSpeed()[1]), rdcsData.GetIZFreightSpeed())).ToString() ));

                                xeCurve.Add(xeMultiCurves);
                                ind++;
                            }
                        }

                        int len = rdcsData.X3IndexPlan - rdcsData.X0IndexPlan;
                        int len2 = rdcsData.X2IndexPlan - rdcsData.X1IndexPlan;
                        int gaugeStandard = curve.Straightenings.Max(s => s.Width);
                        curve.Radius = Convert.ToInt32(curve.Straightenings.Max(s => s.Radius));

                        XElement paramCurve = new XElement("param_curve",
                            new XAttribute("start_km", rdcs[rdcsData.X0IndexPlan].Km),
                            new XAttribute("start_m", rdcs[rdcsData.X0IndexPlan].M),
                            new XAttribute("final_km", rdcs[rdcsData.X3IndexPlan].Km),
                            new XAttribute("final_m", rdcs[rdcsData.X3IndexPlan].M),
                            new XAttribute("start_lvl", rdcsData.X0IndexPlan - rdcsData.X0IndexLevel),
                            new XAttribute("final_lvl", rdcsData.X3IndexPlan - rdcsData.X3IndexLevel),
                            new XAttribute("len", len),
                            new XAttribute("len_lvl", (rdcsData.X3IndexLevel - rdcsData.X0IndexLevel)),
                            new XAttribute("angle", CurveAngle(rdcsData.Y1IndexPlan, rdcsData.X1LengthPlan).ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))));
                        XElement paramCircleCurve = new XElement("param_circle_curve",
                            /*new XAttribute("pr", "н"),
                            new XAttribute("sl", "н"),*/
                            new XAttribute("start_km", rdcs[rdcsData.X1IndexPlan].Km),
                            new XAttribute("start_m", rdcs[rdcsData.X1IndexPlan].M),
                            new XAttribute("final_km", rdcs[rdcsData.X2IndexPlan].Km),
                            new XAttribute("final_m", rdcs[rdcsData.X2IndexPlan].M),
                            new XAttribute("start_lvl", rdcsData.X1IndexPlan - rdcsData.X1IndexLevel),
                            new XAttribute("final_lvl", rdcsData.X2IndexPlan - rdcsData.X2IndexLevel),
                            new XAttribute("len", len2),
                            new XAttribute("len_lvl", rdcsData.X2IndexLevel - rdcsData.X1IndexLevel),
                            new XAttribute("rad_min", rdcsData.GetMinPlan()),
                            new XAttribute("rad_max", rdcsData.GetMaxPlan()),
                            new XAttribute("rad_mid", rdcsData.GetAvgPlan()),
                            new XAttribute("gauge_min", gaugeStandard + rdcsData.GetMinGauge()),
                            new XAttribute("gauge_max", gaugeStandard + rdcsData.GetMaxGauge()),
                            new XAttribute("gauge_mid", gaugeStandard + rdcsData.GetAvgGauge()),
                            new XAttribute("lvl_min", rdcsData.GetMinLevel()),
                            new XAttribute("lvl_max", rdcsData.GetMaxLevel()),
                            new XAttribute("lvl_mid", rdcsData.GetAvgLevel()));
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
                            new XAttribute("tap_max2", rdcsData.GetPlanRightMaxRetractionSlope().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                            new XAttribute("tap_mid2", rdcsData.GetPlanRightAvgRetractionSlope().ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
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

