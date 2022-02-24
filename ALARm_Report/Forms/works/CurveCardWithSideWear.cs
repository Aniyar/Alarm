using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using MetroFramework.Controls;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
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
    public class CurveCardWithSideWear : Report
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
       
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod current, MetroProgressBar progressBar)
        {
            /*XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                ProcessDataTime = new DateTime();
                List<Curve> curves = MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>;
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report",
                     new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                     new XAttribute("distance", ((AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId)).Name));
                int i = 1;
                foreach (var curve in curves)
                {
                    List<RDCurve> rdcs =
                        MainTrackStructureService.GetMtoObjects(curve.Id, MainTrackStructureConst.MtoRdCurve) as List<RDCurve>;
                    if (rdcs.Count > 0)
                        ProcessDataTime = RdStructureService.GetMainParametersProcess(rdcs[0].Process_Id).Date_Vrem;
                    else
                    {
                        ProcessDataTime = new DateTime();
                    }
                    CurveId = curve.Id;

                    var rdcsExisting = rdcs.Count > 0;
                    string radiusStart = string.Empty;
                    string radiusFinal = string.Empty;
                    string levelStart = string.Empty;
                    string levelFinal = string.Empty;
                    curve.Radiuses =
                        (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(radius => radius.Start_Km * 1000 + radius.Start_M).ToList();

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

                    foreach (var elCurve in curve.Radiuses)
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
                    }

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


                    if (rdcsExisting)
                    {
                        rdcsData = new Data(x, plan, level, gauge, passBoost, freightBoost, passSpeed, freightSpeed, transitionLength1, transitionLength2);
                    }

                    if (rdcsExisting && minX > rdcsData.X1IndexPlan - rdcsData.X1LengthPlan)
                    {
                        maxX = maxX + minX - (rdcsData.X1IndexPlan - rdcsData.X1LengthPlan);
                        minX = (rdcsData.X1IndexPlan - rdcsData.X1LengthPlan);
                    }

                    int xAxisInterval = RoundNum(width / 6);
                    double intervalKoef = 620.0 / (width + 20);
                    int xAxisIntervalReal = Convert.ToInt32(xAxisInterval * intervalKoef) - 24;
                    int xCurrentPositionReal = Convert.ToInt32(Math.Abs(RoundNum(minX) - minX) * intervalKoef);
                    XElement xAxisLabels = new XElement("labels");


                    string rXScale = ((width + 20) / 620.0).ToString();
                    XElement xAxisKmLabels = new XElement("kmlabels", new XElement("label", new XAttribute("value", curve.Start_Km), new XAttribute("style", "display:inline;position: absolute;font-size:12px;left:0")));
                    XElement xeXaxis = new XElement("xaxis", new XElement("xparam",
                        new XAttribute("minX", minX - 10),
                        new XAttribute("maxX", maxX + 10))
                    );

                    int currentKm = curve.Start_Km;
                    for (int XCurrentPosition = RoundNum(minX); XCurrentPosition <= maxX; XCurrentPosition = XCurrentPosition + xAxisInterval)
                    {
                        xeXaxis.Add(new XElement("line",
                            new XAttribute("x1", XCurrentPosition),
                            new XAttribute("y2", (-radiusH - 5)),
                            new XAttribute("y2-level", (-levelH - 5))
                            )
                        );
                        xAxisLabels.Add(new XElement("label",
                            new XAttribute("value", GetMeter(XCurrentPosition)),
                            new XAttribute("style", "display:inline;position: absolute;font-size:12px;left:" + Convert.ToInt32(((XCurrentPosition - minX + 10) * intervalKoef) - 5))));
                        if (GetKm(XCurrentPosition) != currentKm)
                        {

                            currentKm = GetKm(XCurrentPosition);
                            xAxisKmLabels.Add(new XElement("label", new XAttribute("value", currentKm), new XAttribute("style", "display:inline;position: absolute;font-size:12px;left:" + Convert.ToInt32((XCurrentPosition - GetMeter(XCurrentPosition) - minX + 10) * intervalKoef))));
                        }
                    }
                    xeXaxis.Add(xAxisLabels);
                    xeXaxis.Add(xAxisKmLabels);
                    float heightR = (radiusH + 10) * 0.1f;
                    float widthR = (width + 20) * 0.0045f;
                    xeXaxis.Add(new XElement("rectangle",
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
                        new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));

                    heightR = (levelH + 10) * 0.1f;

                    xeXaxis.Add(new XElement("rectangle_lvl",
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
                        new XAttribute("height", heightR.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture)))));

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
                    if (rdcsExisting)
                    {
                        passSpeeds[0] = rdcsData.GetCriticalSpeed();
                        passSpeeds[1] = rdcsData.GetPRSpeed()[0];
                        passSpeeds[2] = rdcsData.GetIZPassSpeed();

                        freightSpeeds[0] = rdcsData.GetCriticalSpeed() > 90 ? 90 : rdcsData.GetCriticalSpeed() - Convert.ToInt32(rdcsData.GetCriticalSpeed() * 0.03);
                        freightSpeeds[1] = rdcsData.GetPRSpeed()[1];
                        freightSpeeds[2] = rdcsData.GetIZFreightSpeed();
                    }

                    CurvesAdmUnits curvesAdmUnits = (CurvesAdmUnits)AdmStructureService.GetCurvesAdmUnits(curve.Id);
                    string site = curvesAdmUnits.StationStart + " - " + curvesAdmUnits.StationFinal;
                    XElement xeCurve = new XElement("curve",
                        new XAttribute("date_trip", ProcessDataTime.ToShortDateString()),
                        new XAttribute("site", site),
                        new XAttribute("road", curvesAdmUnits.Track),
                        new XAttribute("direction", curvesAdmUnits.Direction),
                        new XAttribute("km", curve.Start_Km.ToString() + " - " + curve.Final_Km.ToString()),
                        new XAttribute("side", curve.Side.ToLower()),
                        new XAttribute("order", i.ToString()),
                        new XAttribute("PC", "ALARMDK"),
                        new XAttribute("radius", 0 + ",0 " + radiusStart + radiusFinal + " " + (maxX + 20) + ",0"),
                        new XAttribute("radius-average", 0 + ",0 " + radiusAverage + " " + (maxX + 20) + ",0"),
                        new XAttribute("gauge", 0 + ",0 " + gaugeAverage + " " + (maxX + 20) + ",0"),
                        new XAttribute("passboost", 0 + ",0 " + passboost + " " + (maxX + 20) + ",0"),
                        new XAttribute("freightboost", 0 + ",0 " + freightboost + " " + (maxX + 20) + ",0"),
                        new XAttribute("viewbox", (minX - 10) + " " + (-radiusH - 5) + " " + (width + 20) + " " + (radiusH + 10)),
                        new XAttribute("radius-length", radiusLength),
                        new XAttribute("level", 0 + ",0 " + levelStart + levelFinal + " " + (maxX + 20) + ",0"),
                        new XAttribute("level-average", 0 + ",0 " + levelAverage + " " + (maxX + 20) + ",0"),
                        new XAttribute("viewbox-level", (minX - 10) + " " + (-levelH - 5) + " " + (width + 20) + " " + (levelH + 10)),
                        new XAttribute("boost-level", (minX - 10) + " " + (-1) + " " + (width + 20) + " 1.1")
                        );

                    int len = rdcsExisting ? rdcsData.X3IndexPlan - rdcsData.X0IndexPlan : (curve.Final_Km - curve.Start_Km) * 1000 + (curve.Final_M - curve.Start_M);
                    int len2 = rdcsExisting ? rdcsData.X2IndexPlan - rdcsData.X1IndexPlan : (curve.Radiuses[0].Lvl_final_km - curve.Radiuses[0].Lvl_start_km) * 1000 + (curve.Radiuses[0].Lvl_final_m - curve.Radiuses[0].Lvl_start_m);
                    int gaugeStandard = curve.Radiuses.Count > 0 ? curve.Radiuses[0].Width : 1520;
                    if (curve.Radiuses.Count > 0)
                        curve.Radius = Convert.ToInt32(curve.Radiuses[0].Radius);

                    XElement paramCurve = new XElement("param_curve",
                        new XAttribute("start_km", rdcsExisting ? rdcsData.X0IndexPlan / 1000 : curve.Start_Km),
                        new XAttribute("start_m", rdcsExisting ? rdcsData.X0IndexPlan % 1000 : curve.Start_M),
                        new XAttribute("final_km", rdcsExisting ? rdcsData.X3IndexPlan / 1000 : curve.Final_Km),
                        new XAttribute("final_m", rdcsExisting ? rdcsData.X3IndexPlan % 1000 : curve.Final_M),
                        new XAttribute("start_lvl", rdcsExisting ? rdcsData.X0IndexPlan - rdcsData.X0IndexLevel : 0),
                        new XAttribute("final_lvl", rdcsExisting ? rdcsData.X3IndexPlan - rdcsData.X3IndexLevel : 0),
                        new XAttribute("len", len),
                        new XAttribute("len_lvl", rdcsExisting ? (rdcsData.X3IndexLevel - rdcsData.X0IndexLevel) : len),
                        new XAttribute("angle", (rdcsExisting ? curveAngle(rdcsData.Y1IndexPlan, rdcsData.X1LengthPlan) : curveAngleByString(radiusStart)).ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))));
                    XElement paramCircleCurve = new XElement("param_circle_curve",
                        new XAttribute("pr", "н"),
                        new XAttribute("sl", "н"),
                        new XAttribute("start_km", rdcsExisting ? rdcsData.X1IndexPlan / 1000 : curve.Radiuses[0].Lvl_start_km),
                        new XAttribute("start_m", rdcsExisting ? rdcsData.X1IndexPlan % 1000 : curve.Radiuses[0].Lvl_start_m),
                        new XAttribute("final_km", rdcsExisting ? rdcsData.X2IndexPlan / 1000 : curve.Radiuses[0].Lvl_final_km),
                        new XAttribute("final_m", rdcsExisting ? rdcsData.X2IndexPlan % 1000 : curve.Radiuses[0].Lvl_final_m),
                        new XAttribute("start_lvl", rdcsExisting ? rdcsData.X1IndexPlan - rdcsData.X1IndexLevel : 0),
                        new XAttribute("final_lvl", rdcsExisting ? rdcsData.X2IndexPlan - rdcsData.X2IndexLevel : 0),
                        new XAttribute("len", len2),
                        new XAttribute("len_lvl", rdcsExisting ? (rdcsData.X2IndexLevel - rdcsData.X1IndexLevel) : len2),
                        new XAttribute("rad_min", rdcsExisting ? rdcsData.GetMinPlan() : curve.Radius),
                        new XAttribute("rad_max", rdcsExisting ? rdcsData.GetMaxPlan() : curve.Radius),
                        new XAttribute("rad_mid", rdcsExisting ? rdcsData.GetAvgPlan() : curve.Radius),
                        new XAttribute("gauge_min", rdcsExisting ? gaugeStandard + rdcsData.GetMinGauge() : gaugeStandard),
                        new XAttribute("gauge_max", rdcsExisting ? gaugeStandard + rdcsData.GetMaxGauge() : gaugeStandard),
                        new XAttribute("gauge_mid", rdcsExisting ? gaugeStandard + rdcsData.GetAvgGauge() : gaugeStandard),
                        new XAttribute("lvl_min", rdcsExisting ? rdcsData.GetMinLevel() : levelH),
                        new XAttribute("lvl_max", rdcsExisting ? rdcsData.GetMaxLevel() : levelH),
                        new XAttribute("lvl_mid", rdcsExisting ? rdcsData.GetAvgLevel() : levelH));
                    XElement sideWear = new XElement("side_wear",
                        new XAttribute("mm6", "н"),
                        new XAttribute("mm10", "н"),
                        new XAttribute("mm15", "н"),
                        new XAttribute("len", "н"),
                        new XAttribute("angle", "н"));
                    XElement withdrawal = new XElement("withdrawal",
                        new XAttribute("tap_max1", (rdcsExisting ? rdcsData.GetPlanLeftMaxRetractionSlope() : defaultRetractionSlopePlanLeft).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                        new XAttribute("tap_mid1", (rdcsExisting ? rdcsData.GetPlanLeftAvgRetractionSlope() : defaultRetractionSlopePlanLeft).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                        new XAttribute("tap_len1", rdcsExisting ? rdcsData.X1LengthPlan : transitionLength1),
                        new XAttribute("tap_max1_lvl", (rdcsExisting ? rdcsData.GetLevelLeftMaxRetractionSlope() : defaultRetractionSlopeLevelLeft).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                        new XAttribute("tap_mid1_lvl", (rdcsExisting ? rdcsData.GetLevelLeftAvgRetractionSlope() : defaultRetractionSlopeLevelLeft).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                        new XAttribute("tap_len1_lvl", rdcsExisting ? rdcsData.X1LengthLevel : transitionLength1),
                        new XAttribute("tap_max2", (rdcsExisting ? rdcsData.GetPlanRightMaxRetractionSlope() : defaultRetractionSlopePlanRight).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                        new XAttribute("tap_mid2", (rdcsExisting ? rdcsData.GetPlanRightAvgRetractionSlope() : defaultRetractionSlopePlanRight).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                        new XAttribute("tap_len2", rdcsExisting ? rdcsData.X2LengthPlan : transitionLength2),
                        new XAttribute("tap_max2_lvl", (rdcsExisting ? rdcsData.GetLevelRightMaxRetractionSlope() : defaultRetractionSlopeLevelRight).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                        new XAttribute("tap_mid2_lvl", (rdcsExisting ? rdcsData.GetLevelRightAvgRetractionSlope() : defaultRetractionSlopeLevelRight).ToString("f2", System.Globalization.CultureInfo.InvariantCulture)),
                        new XAttribute("tap_len2_lvl", rdcsExisting ? rdcsData.X2LengthLevel : transitionLength2));
                    XElement computing = new XElement("computing",
                        new XAttribute("a1", rdcsExisting ? rdcsData.GetUnliquidatedAccelerationPassengerAvg().ToString("f2", System.Globalization.CultureInfo.InvariantCulture) : "н"),
                        new XAttribute("a2", rdcsExisting ? rdcsData.GetUnliquidatedAccelerationPassengerMax().ToString("f2", System.Globalization.CultureInfo.InvariantCulture) : "н"),
                        new XAttribute("a3", rdcsExisting ? rdcsData.GetUnliquidatedAccelerationFreightAvg().ToString("f2", System.Globalization.CultureInfo.InvariantCulture) : "н"),
                        new XAttribute("a4", rdcsExisting ? rdcsData.GetUnliquidatedAccelerationFreightMax().ToString("f2", System.Globalization.CultureInfo.InvariantCulture) : "н"),
                        new XAttribute("a5", rdcsExisting ? rdcsData.GetUnliquidatedAccelerationPassengerMaxCoordinate().ToString() : "н"),
                        new XAttribute("psi1", rdcsExisting ? rdcsData.BoostChangeRateMax().ToString("f2", System.Globalization.CultureInfo.InvariantCulture) : "н"),
                        new XAttribute("psi2", rdcsExisting ? rdcsData.BoostChangeRateMaxCoordinate().ToString() : "н"),
                        new XAttribute("P", rdcsExisting ? rdcsData.GetR().ToString("f2", System.Globalization.CultureInfo.InvariantCulture) : "н"),
                        new XAttribute("V1", rdcsExisting ? rdcsData.GetCriticalSpeed03up().ToString() : "н"),
                        new XAttribute("V2", rdcsExisting ? rdcsData.GetCriticalSpeed03down().ToString() : "н"));
                    XElement speedElement = new XElement("speed",
                        new XAttribute("pass1", rdcsExisting ? rdcsData.GetPassSpeed().ToString() : "н"),
                        new XAttribute("pass2", rdcsExisting ? passSpeeds[0].ToString() : "н"),
                        new XAttribute("pass3", rdcsExisting ? passSpeeds[1].ToString() : "н"),
                        new XAttribute("pass4", rdcsExisting ? passSpeeds[2].ToString() : "н"),
                        new XAttribute("pass5", "-"),
                        new XAttribute("pass6", rdcsExisting ? RoundNumToFive(passSpeeds.Min()).ToString() : "н"),
                        new XAttribute("frei1", rdcsExisting ? rdcsData.GetFreightSpeed().ToString() : "н"),
                        new XAttribute("frei2", rdcsExisting ? freightSpeeds[0].ToString() : "н"),
                        new XAttribute("frei3", rdcsExisting ? freightSpeeds[1].ToString() : "н"),
                        new XAttribute("frei4", rdcsExisting ? freightSpeeds[2].ToString() : "н"),
                        new XAttribute("frei5", "-"),
                        new XAttribute("frei6", rdcsExisting ? RoundNumToFive(freightSpeeds.Min()).ToString() : "н"));
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
                xdReport.Add(report);
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report.html");
                var word = new Microsoft.Office.Interop.Word.Application();
                word.Visible = false;

                //var filePath = Path.GetTempPath() + "/report.html";
                //var savePathPdf = Path.GetTempPath() + "/report.docx";
                //var wordDoc = word.Documents.Open(FileName: filePath, ReadOnly: false);
                // wordDoc.SaveAs2(FileName: savePathPdf, FileFormat: WdSaveFormat.wdFormatXMLDocument);
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report.html");
            }*/
        }
    }
}

