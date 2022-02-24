
using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using MetroFramework.Controls;
using System.Linq;
using Svg;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using ALARm_Report.controls;

namespace ALARm_Report.Forms
{
    /// <summary>
    /// График диаграммы сводной по доп параметрам
    /// </summary>
    /// 
    public class GDSummaryOfAditionalParameters : GraphicDiagrams
    {
        private float angleRuleWidth = 9.7f;
        private float gapsKoef = 9.5f / 30f;
        private float iznosKoef = 8.8f / 20f;

        private float gaspRightPosition = 14f;
        private float iznosLeftPosition = 28f;
        private float iznosRightPosition = 42f;
        private float PULeftPosition = 86.1f;
        private float PURightPosition = 100f;

        private float NPKLeftPosition = 57.5f;
        private float NPKRightPosition = 71.5f;

        /// <summary>
        /// отжатие
        /// </summary>
        private float releasePosition = 121f;
        /// <summary>
        /// шаблон
        ///  </summary>
        private float gaugeKoef = 0.5f;
        private float gaugePosition = 125f;
        private float GetDIstanceFrom1div60(float x)
        {
            return 15 * angleRuleWidth * (1f / x - 1 / 60f);
        }
        private readonly int LabelsDivWidthInPixel = 550;
        private readonly float LabelsDivWidthInMM = 146;
        private readonly float BottomLabelHeightInMM = 1.6f;
        private string MMToPixelLabel(float mm)
        {
            return (LabelsDivWidthInPixel / LabelsDivWidthInMM * mm).ToString().Replace(",", ".");
        }

        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
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

            xBegin = 145;
            XDocument htReport = new XDocument();
            diagramName = "Доппараметры";
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);

                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                int svgIndex = template.Xsl.IndexOf("</svg>");
                template.Xsl = template.Xsl.Insert(svgIndex, righstSideXslt());
                foreach (var tripProcess in tripProcesses)
                {
                    //Канонический вид
                    tripProcess.Direction = Direction.Direct;

                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        //кривая
                        List<Curve> curves = MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>;

                        var kilometers = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);

                        if (kilometers.Count() == 0) continue;

                        kilometers = kilometers.OrderByDescending(i => i).ToList();

                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();
                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = kilometers.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = kilometers.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        kilometers = kilometers.Where(Km => ((float)(float)filters[0].Value <= Km && Km <= (float)(float)filters[1].Value)).ToList();

                        progressBar.Maximum = kilometers.Count;
                        foreach (var kilometer in kilometers)
                        {
                            //if (kilometer != 139) continue;

                            progressBar.Value = kilometers.IndexOf(kilometer) + 1;

                            var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, $"{trackName}") as List<Speed>;
                            var sector = MainTrackStructureService.GetSector(track_id, kilometer, tripProcess.Date_Vrem);
                            var fragment = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.Fragments, tripProcess.DirectionName, $"{trackName}") as Fragment;
                            var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.MtoPdbSection, tripProcess.DirectionName, $"{trackName}") as List<PdbSection>;

                            XElement addParam = new XElement("addparam",
                                new XAttribute("top-title",

                                    $"{tripProcess.DirectionName}({tripProcess.DirectionCode})  Путь:{trackName}  Км:" + kilometer +

                                    $"  {(pdbSection.Count > 0 ? pdbSection[0].ToString() : "ПЧ-/ПЧУ-/ПД-/ПДБ-")}" +

                                    $"  Уст:{(speed.Count > 0 ? speed[0].ToString() : "-/-/-")}" + "  Скор:58"),

                                new XAttribute("right-title",
                                    copyright + ": " + "ПО " + softVersion + "  " +
                                    systemName + ":" + tripProcess.Car + "(" + tripProcess.Chief + ") (БПД от " +
                                    MainTrackStructureService.GetModificationDate() + ") <" + AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, false) + ">" +
                                    //"<" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.Direction.ToString())) + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.CarPosition.ToString())) + ">" +
                                    "<" + period.PeriodMonth + "-" + period.PeriodYear + " " + "контр. Проезд:" + tripProcess.Date_Vrem.ToShortDateString() + " " + tripProcess.Date_Vrem.ToShortTimeString() +
                                    " " + diagramName + ">" + " Л: " + (kilometers.IndexOf(kilometer) + 1)
                                ),
                                new XAttribute("fragment", $"{sector}  Км:{kilometer}"),
                                new XAttribute("viewbox", "0 0 770 1015"),
                                new XAttribute("minY", 0),
                                new XAttribute("maxY", 1000),

                                    RightSideChart(tripProcess.Date_Vrem, kilometer, Direction.Direct, tripProcess.DirectionID, trackName.ToString(), new float[] { 151f, 146f, 152.5f, 155f, -760 }),

                                new XElement("xgrid",
                                    new XAttribute("x0", xBegin),
                                    new XAttribute("x1", MMToPixelChart(gaspRightPosition)),
                                    new XAttribute("x2", MMToPixelChart(releasePosition)),
                                    new XAttribute("x3", MMToPixelChart(gaugePosition + gaugeKoef * 20)),
                                    new XAttribute("x31", MMToPixelChart(iznosLeftPosition)),
                                    new XAttribute("x32", MMToPixelChart(iznosRightPosition)),
                                    //new XAttribute("x4", MMToPixelChart(146)),
                                    new XElement("x", MMToPixelChartString(gapsKoef * 25f)),
                                    new XElement("x", MMToPixelChartString(gapsKoef * 27.5f)),
                                    new XElement("x", MMToPixelChartString(gapsKoef * 30f)),

                                    new XElement("x", MMToPixelChartString(gaspRightPosition + gapsKoef * 25f)),
                                    new XElement("x", MMToPixelChartString(gaspRightPosition + gapsKoef * 27.5f)),
                                    new XElement("x", MMToPixelChartString(gaspRightPosition + gapsKoef * 30f)),

                                    new XElement("x", MMToPixelChartString(iznosLeftPosition + iznosKoef * 8f)),
                                    new XElement("x", MMToPixelChartString(iznosLeftPosition + iznosKoef * 13f)),
                                    new XElement("x", MMToPixelChartString(iznosLeftPosition + iznosKoef * 14f)),
                                    new XElement("x", MMToPixelChartString(iznosLeftPosition + iznosKoef * 20f)),

                                    new XElement("x", MMToPixelChartString(iznosRightPosition + iznosKoef * 8f)),
                                    new XElement("x", MMToPixelChartString(iznosRightPosition + iznosKoef * 13f)),
                                    new XElement("x", MMToPixelChartString(iznosRightPosition + iznosKoef * 14f)),
                                    new XElement("x", MMToPixelChartString(iznosRightPosition + iznosKoef * 20f)),

                                    //НПК лев
                                    new XElement("x", MMToPixelChartString(NPKLeftPosition)),
                                    new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(30))),
                                    new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(22))),
                                    new XElement("x20", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(20))),
                                    new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(18))),
                                    new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(16))),
                                    new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(12))),
                                    //НПК прав.
                                    new XElement("x", MMToPixelChartString(NPKRightPosition)),
                                    new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(30))),
                                    new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(22))),
                                    new XElement("x20", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(20))),
                                    new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(18))),
                                    new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(16))),
                                    new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(12))),
                                    //ПУ лев.
                                    new XElement("x", MMToPixelChartString(PULeftPosition)),
                                    new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(30))),
                                    new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(22))),
                                    new XElement("x20", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(20))),
                                    new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(18))),
                                    new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(16))),
                                    new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(12))),
                                    //ПУ прав.
                                    new XElement("x", MMToPixelChartString(PURightPosition)),
                                    new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(30))),
                                    new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(22))),
                                    new XElement("x20", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(20))),
                                    new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(18))),
                                    new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(16))),
                                    new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(12))),
                                    new XElement("x", MMToPixelChartString(gaugePosition + gaugeKoef * 10)),
                                    new XElement("x", MMToPixelChartString(gaugePosition + gaugeKoef * 12)),
                                    new XElement("x", MMToPixelChartString(gaugePosition + gaugeKoef * 16)),
                                    new XElement("x", MMToPixelChartString(gaugePosition + gaugeKoef * 24)),
                                    new XElement("x", MMToPixelChartString(gaugePosition + gaugeKoef * 28)),
                                    new XElement("x", MMToPixelChartString(gaugePosition + gaugeKoef * 36))
                                ));


                            //Шаблон
                            var DB_gauge = AdditionalParametersService.GetGaugeFromDB(kilometer, tripProcess.Trip_id);
                            List<string> polylineGauge = GetCrossRailProfileLines2(DB_gauge, tripProcess.Direction);
                            var linesElem = new XElement("lines");
                            foreach (var polyline in polylineGauge)
                            {
                                linesElem.Add(new XElement("line", polyline));
                            }


                            //доп пар
                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(kilometer, tripProcess.Trip_id);
                            if (DBcrossRailProfile == null || DBcrossRailProfile.Count == 0) continue;

                            var cccc = DBcrossRailProfile.Last();

                            var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(DBcrossRailProfile);

                            

                            List<Digression> addDigressions = crossRailProfile.GetDigressions();

                            List<string> polylines = GetCrossRailProfileLines(crossRailProfile, tripProcess.Direction);
                            foreach (var polyline in polylines)
                            {
                                linesElem.Add(new XElement("line", polyline));
                            }
                            var digElemenets = new XElement("digressions");
                            

                            var gapElements = new XElement("gaps");
                            //var gaps = AdditionalParametersService.GetGaps(tripProcess.Id, (int)tripProcess.Direction, kilometer);
                            var gaps = AdditionalParametersService.Check_gap_state(tripProcess.Trip_id, template.ID).Where(i => i.Km == kilometer).ToList();



                            foreach (var gap in gaps)
                            {
                                if (gap.Zazor != -999)
                                {
                                    gapElements.Add(new XElement("g",
                                    new XAttribute("x", MMToPixelChart(gaspRightPosition)),
                                    new XAttribute("y", tripProcess.Direction == Direction.Reverse ? gap.Meter : 1000 - gap.Meter),
                                    new XAttribute("w", MMToPixelChartWidthString(gap.Zazor * gapsKoef))
                                    )
                                    );
                                }
                                if (gap.R_zazor != -999)
                                {
                                    gapElements.Add(new XElement("g",
                                    new XAttribute("x", xBegin),
                                    new XAttribute("y", tripProcess.Direction == Direction.Reverse ? gap.Meter : 1000 - gap.Meter),
                                    new XAttribute("w", MMToPixelChartWidthString(gap.R_zazor * gapsKoef))
                                    )
                                    );
                                }

                                gap.PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                                gap.FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;
                                
                                addDigressions.Add(gap.GetDigressions());
                                addDigressions.Add(gap.GetDigressions3());
                            }
                            addParam.Add(gapElements);

                            int fs = 8;
                            int picket1 = tripProcess.Direction == Direction.Reverse ? 8 : 998;
                            int picket2 = tripProcess.Direction == Direction.Reverse ? 104 : 902;
                            int picket3 = tripProcess.Direction == Direction.Reverse ? 200 : 798;
                            int picket4 = tripProcess.Direction == Direction.Reverse ? 304 : 702;
                            int picket5 = tripProcess.Direction == Direction.Reverse ? 400 : 598;
                            int picket6 = tripProcess.Direction == Direction.Reverse ? 504 : 502;
                            int picket7 = tripProcess.Direction == Direction.Reverse ? 600 : 398;
                            int picket8 = tripProcess.Direction == Direction.Reverse ? 704 : 302;
                            int picket9 = tripProcess.Direction == Direction.Reverse ? 800 : 198;
                            int picket10 = tripProcess.Direction == Direction.Reverse ? 904 : 102;

                            int x1 = -23, x2 = -23, x3 = -23, x4 = -23, x5 = -23, x6 = -23, x7 = -23, x8 = -23, x9 = -23, x10 = -23;
                            int prev_1_M = -1;
                            int prev_2_M = -1;
                            int prev_3_M = -1;
                            bool flag = true;

                            //стрелочные переводы
                            var switches = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer,
                                MainTrackStructureConst.MtoSwitch, "", "") as List<Switch>;


                            addDigressions = addDigressions.OrderByDescending(o => o.Meter).ToList();

                            foreach (var digression in addDigressions)
                                {
                                    if (digression.DigName == DigressionName.ReducedWearLeft) continue;
                                    if (digression.DigName == DigressionName.ReducedWearRight) continue;
                                    if (digression.DigName == DigressionName.VertIznosR) continue;
                                    if (digression.DigName == DigressionName.VertIznosL) continue;

                                    if (digression.Length < 1)
                                    {
                                        if (digression.DigName != DigressionName.FusingGap)
                                        {
                                            if (digression.DigName != DigressionName.AnomalisticGap)
                                            {
                                                if (digression.DigName != DigressionName.Gap)
                                                {
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                    var switches_status = false;
                                    var start_m = 0;
                                    var final_m = 0;

                                    if (switches.Count > 0)
                                    {
                                        foreach (var switchh in switches)
                                        {
                                            if (switchh.Dir_Id == SwitchDirection.NotDefined) continue;
                                            if (switchh.Dir_Id == SwitchDirection.Direct)
                                            {
                                                start_m = switchh.Meter - switchh.Length;
                                                final_m = switchh.Meter;
                                            }
                                            if (switchh.Dir_Id == SwitchDirection.Reverse)
                                            {
                                                start_m = switchh.Meter;
                                                final_m = switchh.Meter + switchh.Length;
                                            }
                                            if (switchh.Km == digression.Km && start_m <= digression.Meter && final_m >= digression.Meter) switches_status = true;
                                        }
                                    }
                                    if (switches_status == true) continue;

                                    int y = 0;
                                    int x = -4;
                                    switch (digression.Meter)
                                    {
                                        case int meter when meter > 0 && meter <= 100:
                                            y = picket1;
                                            x = x1;
                                            picket1 += tripProcess.Direction == Direction.Reverse ? fs : -fs;
                                            if (((tripProcess.Direction == Direction.Reverse) && (picket1 == 104)) ||
                                                ((tripProcess.Direction == Direction.Direct) && (picket1 == 902)))
                                            {
                                                picket1 = tripProcess.Direction == Direction.Reverse ? 8 : 998;
                                                x1 += 65;
                                            }
                                            break;
                                        case int meter when meter > 100 && meter <= 200:
                                            y = picket2;
                                            x = x2;
                                            picket2 += tripProcess.Direction == Direction.Reverse ? fs : -fs;

                                            if (((tripProcess.Direction == Direction.Reverse) && (picket2 == 200)) ||
                                                ((tripProcess.Direction == Direction.Direct) && (picket2 == 798)))
                                            {
                                                picket2 = tripProcess.Direction == Direction.Reverse ? 104 : 902;
                                                x2 += 65;
                                            }
                                            break;
                                        case int meter when meter > 200 && meter <= 300:
                                            y = picket3;
                                            x = x3;
                                            picket3 += tripProcess.Direction == Direction.Reverse ? fs : -fs;
                                            if (((tripProcess.Direction == Direction.Reverse) && (picket3 == 304)) ||
                                                ((tripProcess.Direction == Direction.Direct) && (picket3 == 702)))
                                            {
                                                picket3 = tripProcess.Direction == Direction.Reverse ? 200 : 798;
                                                x3 += 65;
                                            }
                                            break;
                                        case int meter when meter > 300 && meter <= 400:
                                            y = picket4;
                                            x = x4;
                                            picket4 += tripProcess.Direction == Direction.Reverse ? fs : -fs;
                                            if (((tripProcess.Direction == Direction.Reverse) && (picket4 == 400)) ||
                                                ((tripProcess.Direction == Direction.Direct) && (picket4 == 598)))
                                            {
                                                picket4 = tripProcess.Direction == Direction.Reverse ? 304 : 702;
                                                x4 += 65;
                                            }
                                            break;
                                        case int meter when meter > 400 && meter <= 500:
                                            y = picket5;
                                            x = x5;
                                            picket5 += tripProcess.Direction == Direction.Reverse ? fs : -fs;
                                            if (((tripProcess.Direction == Direction.Reverse) && (picket5 == 504)) ||
                                                ((tripProcess.Direction == Direction.Direct) && (picket5 == 502)))
                                            {
                                                picket5 = tripProcess.Direction == Direction.Reverse ? 400 : 598;
                                                x5 += 65;
                                            }
                                            break;
                                        case int meter when meter > 500 && meter <= 600:
                                            y = picket6;
                                            x = x6;
                                            picket6 += tripProcess.Direction == Direction.Reverse ? fs : -fs;
                                            if (((tripProcess.Direction == Direction.Reverse) && (picket6 == 600)) ||
                                                ((tripProcess.Direction == Direction.Direct) && (picket6 == 398)))
                                            {
                                                picket6 = tripProcess.Direction == Direction.Reverse ? 504 : 502;
                                                x6 += 65;
                                            }
                                            break;
                                        case int meter when meter > 600 && meter <= 700:
                                            y = picket7;
                                            x = x7;
                                            picket7 += tripProcess.Direction == Direction.Reverse ? fs : -fs;
                                            if (((tripProcess.Direction == Direction.Reverse) && (picket7 == 704)) ||
                                                ((tripProcess.Direction == Direction.Direct) && (picket7 == 302)))
                                            {
                                                picket7 = tripProcess.Direction == Direction.Reverse ? 600 : 398;
                                                x7 += 65;
                                            }
                                            break;
                                        case int meter when meter > 700 && meter <= 800:
                                            y = picket8;
                                            x = x8;
                                            picket8 += tripProcess.Direction == Direction.Reverse ? fs : -fs;
                                            if (((tripProcess.Direction == Direction.Reverse) && (picket8 == 800)) ||
                                                ((tripProcess.Direction == Direction.Direct) && (picket8 == 198)))
                                            {
                                                picket8 = tripProcess.Direction == Direction.Reverse ? 704 : 302;
                                                x8 += 65;
                                            }
                                            break;
                                        case int meter when meter > 800 && meter <= 900:
                                            y = picket9;
                                            x = x9;
                                            picket9 += tripProcess.Direction == Direction.Reverse ? fs : -fs;
                                            if (((tripProcess.Direction == Direction.Reverse) && (picket9 == 904)) ||
                                                ((tripProcess.Direction == Direction.Direct) && (picket9 == 102)))
                                            {
                                                picket9 = tripProcess.Direction == Direction.Reverse ? 800 : 198;
                                                x9 += 65;
                                            }
                                            break;
                                        case int meter when meter > 900 && meter <= 1000:
                                            y = picket10;
                                            x = x10;
                                            picket10 += tripProcess.Direction == Direction.Reverse ? fs : -fs;
                                            if (((tripProcess.Direction == Direction.Reverse) && (picket10 == 1000)) ||
                                                ((tripProcess.Direction == Direction.Direct) && (picket10 == 6)))
                                            {
                                                picket10 = tripProcess.Direction == Direction.Reverse ? 904 : 102;
                                                x10 += 65;
                                            }
                                            break;
                                    }
                                    prev_3_M = prev_2_M;
                                    prev_2_M = prev_1_M;
                                    prev_1_M = digression.Meter;

                                    int count = digression.Length / 4;
                                    count += digression.Length % 4 > 0 ? 1 : 0;

                                    if ((digression.DigName == DigressionName.FusingGap) || (digression.DigName == DigressionName.AnomalisticGap) || (digression.DigName == DigressionName.Gap))
                                    {
                                        digElemenets.Add(new XElement("m",
                                                             new XAttribute("top", y),
                                                             new XAttribute("x", -23),
                                                             new XAttribute("note", digression.Meter),
                                                             new XAttribute("fw", (digression.DigName == DigressionName.Gap) ? "bold" : "normal")
                                            ));
                                        digElemenets.Add(new XElement("otst",
                                                             new XAttribute("top", y),
                                                             new XAttribute("x", -3),
                                                             new XAttribute("note", digression.GetName() + " " + (digression.Threat == Threat.Right ? "п." : digression.Threat == Threat.Right ? "л." : "")),
                                                             new XAttribute("fw", (digression.DigName == DigressionName.Gap) ? "bold" : "normal")
                                            ));
                                        digElemenets.Add(new XElement("otkl",
                                                             new XAttribute("top", y),
                                                             new XAttribute("x", 43),
                                                             new XAttribute("note", digression.Velich),
                                                             new XAttribute("fw", (digression.DigName == DigressionName.Gap) ? "bold" : "normal")
                                            ));
                                        digElemenets.Add(new XElement("ogrsk",
                                                             new XAttribute("top", y),
                                                             new XAttribute("x", 105),
                                                             new XAttribute("note", "" + (digression.DigName == DigressionName.AnomalisticGap ? "" : digression.AllowSpeed)),
                                                             new XAttribute("fw", (digression.DigName == DigressionName.Gap) ? "bold" : "normal")
                                            ));
                                    }
                                    else
                                    {

                                    digElemenets.Add(new XElement("dig",
                                    new XAttribute("n", digression.GetName() + " " + digression.Typ),
                                    new XAttribute("x1", digression.DigName == DigressionName.SideWearLeft ? "274" :
                                                        (digression.DigName == DigressionName.SideWearRight ? "330" :
                                                            (digression.DigName == DigressionName.TreadTiltLeft ? "385.4732" :
                                                                (digression.DigName == DigressionName.TreadTiltRight ? "441.5" :
                                                                    (digression.DigName == DigressionName.DownhillLeft ? "500" :
                                                                        (digression.DigName == DigressionName.DownhillRight ? "555.8" : "0"))))
                                    )),
                                    new XAttribute("w", MMToPixelChartWidthString((int)digression.Typ == 4 ? 4 : 2)),
                                    new XAttribute("y1", tripProcess.Direction == Direction.Reverse ? digression.Meter : 1000 - digression.Meter),
                                    new XAttribute("y2", tripProcess.Direction == Direction.Reverse ? digression.Kmetr : 1000 - digression.Kmetr),
                                    new XAttribute("top", y),
                                    new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : "normal"),
                                    new XAttribute("x", x),
                                    new XAttribute("note", (digression.Meter < 10 ? "    " : digression.Meter < 100 ? "  " : "") + digression.Meter + " " + digression.GetName() +
                                                            ((int)digression.Typ == 4 ? " " : "  ") + (int)digression.Typ + "    " +
                                                           (((digression.DigName == DigressionName.TreadTiltLeft) || (digression.DigName == DigressionName.TreadTiltRight) ||
                                                            (digression.DigName == DigressionName.DownhillLeft) || (digression.DigName == DigressionName.DownhillRight)) ?
                                                            (digression.Value > 0 ? ("1/" + ((int)(1 / digression.Value)).ToString()) : "     0") : digression.Value.ToString("0.0") + " ") + "  " + (digression.Length < 10 ? "  " : "") +
                                                            digression.Length + "   " + (count < 10 ? "  " : "") + count)
                                    ));
                                    digElemenets.Add(new XElement("m",
                                                             new XAttribute("top", y),
                                                             new XAttribute("x", -23),
                                                             new XAttribute("note", digression.Meter),
                                                             new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : ((digression.DigName == DigressionName.TreadTiltLeft) || (digression.DigName == DigressionName.TreadTiltRight) ||
                                                                                                                       (digression.DigName == DigressionName.DownhillLeft) || (digression.DigName == DigressionName.DownhillRight) ? "bold" : "normal"))
                                            ));
                                        digElemenets.Add(new XElement("otst",
                                                             new XAttribute("top", y),
                                                             new XAttribute("x", -3),
                                                             new XAttribute("note", digression.GetName()),
                                                             new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : ((digression.DigName == DigressionName.TreadTiltLeft) || (digression.DigName == DigressionName.TreadTiltRight) ||
                                                                                                                       (digression.DigName == DigressionName.DownhillLeft) || (digression.DigName == DigressionName.DownhillRight) ? "bold" : "normal"))
                                            ));
                                        digElemenets.Add(new XElement("step",
                                                             new XAttribute("top", y),
                                                             new XAttribute("x", 29),
                                                             new XAttribute("note", (int)digression.Typ),
                                                             new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : ((digression.DigName == DigressionName.TreadTiltLeft) || (digression.DigName == DigressionName.TreadTiltRight) ||
                                                                                                                       (digression.DigName == DigressionName.DownhillLeft) || (digression.DigName == DigressionName.DownhillRight) ? "bold" : "normal"))
                                            ));
                                        digElemenets.Add(new XElement("otkl",
                                                             new XAttribute("top", y),
                                                             new XAttribute("x", 43),
                                                             new XAttribute("note", (((digression.DigName == DigressionName.TreadTiltLeft) || (digression.DigName == DigressionName.TreadTiltRight) ||
                                                                                      (digression.DigName == DigressionName.DownhillLeft) || (digression.DigName == DigressionName.DownhillRight)) ?
                                                                                        (digression.Value > 0 ? ("1/" + ((int)(1 / digression.Value)).ToString()) : "     0") : digression.Value.ToString("0.0") + " ")
                                                                                        ),
                                                             new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : "normal")
                                            ));
                                        digElemenets.Add(new XElement("len",
                                                             new XAttribute("top", y),
                                                             new XAttribute("x", 68),
                                                             new XAttribute("note", digression.Length),
                                                             new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : "normal")
                                            ));
                                        digElemenets.Add(new XElement("count",
                                                             new XAttribute("top", y),
                                                             new XAttribute("x", 90),
                                                             new XAttribute("note", count),
                                                             new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : "normal")
                                            ));

                                        //боковой износ огр ск
                                        var Vogr = -1;
                                        var Vust = speed.Count > 0 ? speed[0].Passenger : -1;

                                        if (digression.DigName == DigressionName.SideWearLeft || digression.DigName == DigressionName.SideWearRight)
                                        {
                                            //if ((int)digression.Typ == 2) continue;

                                            foreach (var elem in curves)
                                            {/*
                                            elem.Radiuses =
                                            (MainTrackStructureService.GetCurves(elem.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(radius => radius.Start_Km * 1000 + radius.Start_M).ToList();
                                            if (kilometer.Number >= elem.Start_Km && kilometer.Number <= elem.Final_Km)
                                            {
                                                switch (digression.Value)
                                                {
                                                    case float Value when Value >= 13.1 && Value <= 15:
                                                        Vogr = 140;
                                                        break;
                                                    case float Value when Value >= 15.1 && Value <= 20 && elem.Radiuses[0].Radius > 350:
                                                        Vogr = 70;
                                                        break;
                                                    case float Value when Value >= 15.1 && Value <= 20 && elem.Radiuses[0].Radius <= 350:
                                                        Vogr = 50;
                                                        break;
                                                    case float Value when Value > 20:
                                                        Vogr = 50;
                                                        break;
                                                }

                                            }*/
                                            }
                                            Vust = Vogr >= Vust ? -1 : Vogr;
                                            if (Vust != -1)
                                            {
                                                digElemenets.Add(new XElement("ogrsk",
                                                                 new XAttribute("top", y),
                                                                 new XAttribute("x", 105),
                                                                 new XAttribute("note", Vogr + "/" + Vogr + "/" + Vogr),
                                                                 new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : "normal")
                                                ));
                                            }
                                        }

                                    }
                                }

                            addParam.Add(digElemenets);
                            //var mainParameters = MainParametersService.GetMainParameters(kilometer);
                            //polylines = GetMainParametersLines(mainParameters, tripProcess.Direction);
                            //foreach (var polyline in polylines)
                            //{
                            //    linesElem.Add(new XElement("line", polyline));
                            //}
                            addParam.Add(linesElem);

                            addParam.Add(
                                new XElement("bl", 1536,
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(0.15f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 1524,
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(4f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 1520,
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1.2f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 1512,
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(2f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(4.5f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "0",
                                    new XAttribute("style",
                                        "padding-top:" + (MMToPixelLabel(3f)) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),

                                new XElement("bl", "1/12",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(10f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/16",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1.2f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/20",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1.2f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/60",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(2.5f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/12",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(2.6f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/16",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1.2f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/20",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1.2f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/60",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(2.5f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/12",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(3.4f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/16",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1.2f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/20",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1.2f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/60",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(2.5f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/12",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(2.6f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/16",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1.2f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/20",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1.2f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "1/60",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(2.5f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "20",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(4.5f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "13",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1.2f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "8",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "0",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1.6f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),

                                new XElement("bl", "20",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(3.5f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "13",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1.2f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "8",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "0",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1.6f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),

                                new XElement("bl", "30",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(4f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "25",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "0",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(4.6f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),

                                new XElement("bl", "30",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(4f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "25",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(1f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", "0",
                                    new XAttribute("style",
                                        "padding-top:" + MMToPixelLabel(4.6f) + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM)))
                            );

                            //SaveSVGasPng(template, addParam, kilometer);

                            report.Add(addParam);
                            //break;
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

        

        private static void SaveSVGasPng(ReportTemplate template, XElement addParam, int kilometer)
        {
            XDocument htTempReport = new XDocument();
            using (XmlWriter writer1 = htTempReport.CreateWriter())
            {
                XDocument tmpXdReport = new XDocument();
                XElement tmpReport = new XElement("report");
                tmpReport.Add(addParam);
                tmpXdReport.Add(tmpReport);
                XslCompiledTransform transform1 = new XslCompiledTransform();
                transform1.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform1.Transform(tmpXdReport.CreateReader(), writer1);
            }
            //var query = from c in htTempReport.Descendants("svg") select c;
            //query.First().Save("D:\\" + kilometer + ".html");
            //query.First().Save("D:\\" + kilometer + ".svg");
            //var svgDoc = SvgDocument.Open("D:\\" + kilometer + ".svg");
            //svgDoc.Draw(3080, 3760).Save("D:\\forms\\" + kilometer + ".png", ImageFormat.Png);
        }

        private List<string> GetMainParametersLines(ALARm.Core.MainParameters mainParameters, Direction travelDirection)
        {
            List<string> result = new List<string>();
            string gauge = string.Empty;
            for (int i = 0; i < mainParameters.Meters.Count; i++)
            {
                string meter = (travelDirection == Direction.Reverse ? mainParameters.Meters[i] : 1000 - mainParameters.Meters[i]).ToString();
                gauge += MMToPixelChartString(gaugePosition + gaugeKoef * (mainParameters.Gauge[i] - 1500)).Replace(",", ".") + "," + meter + " ";
            }
            result.Add(gauge);
            return result;
        }

        private List<string> GetCrossRailProfileLines2(List<CrosProf> dB_gauge, Direction travelDirection)
        {
            List<string> result = new List<string>();
            string gauge = string.Empty;
            string otzh = string.Empty;

            for (int i = 0; i < dB_gauge.Count-1; i++)
            {
                string meter = (travelDirection == Direction.Reverse ? dB_gauge[i].Meter : 1000 - dB_gauge[i].Meter).ToString().Replace(",", ".");

                gauge += MMToPixelChartString(gaugePosition+12 + gaugeKoef * (dB_gauge[i].Gauge - 1520)).Replace(",", ".") + "," + meter + " ";
                otzh += MMToPixelChartString(releasePosition + (dB_gauge[i].Gauge - dB_gauge[i+1].Gauge)).Replace(",", ".") + "," + meter + " ";
            }
            result.Add(gauge);
            result.Add(otzh);
            return result;
        }

        private List<string> GetCrossRailProfileLines(CrossRailProfile crossRailProfile, Direction travelDirection)
        {
            List<string> result = new List<string>();
            string downhillLeft = string.Empty;
            string downhillRight = string.Empty;
            string sideWearLeft = string.Empty;
            string sideWearRight = string.Empty;
            string treadTiltLeft = string.Empty;
            string treadTiltRight = string.Empty;

            for (int i = 0; i < crossRailProfile.Meters.Count; i++)
            {
                var koef = 8.8 / (1.0 / 12.0 - 1.0 / 60.0);
                string meter = (travelDirection == Direction.Reverse ? crossRailProfile.Meters[i] : 1000 - crossRailProfile.Meters[i]).ToString().Replace(",", ".");

                sideWearLeft += MMToPixelChartString(iznosLeftPosition + iznosKoef * crossRailProfile.SideWearLeft[i]).Replace(",", ".") + "," + meter + " ";
                sideWearRight += MMToPixelChartString(iznosRightPosition + iznosKoef * crossRailProfile.SideWearRight[i]).Replace(",", ".") + "," + meter + " ";

                treadTiltLeft += MMToPixelChartString(NPKLeftPosition +  crossRailProfile.TreadTiltLeft[i] * koef).Replace(",", ".") + "," + meter + " ";
                treadTiltRight += MMToPixelChartString(NPKRightPosition + crossRailProfile.TreadTiltRight[i] * koef).Replace(",", ".") + "," + meter + " ";

                downhillLeft += MMToPixelChartString(PULeftPosition + crossRailProfile.DownhillLeft[i] * koef).Replace(",", ".") + "," + meter + " ";
                downhillRight += MMToPixelChartString(PURightPosition +crossRailProfile.DownhillRight[i]* koef).Replace(",", ".") + "," + meter + " ";
            }
            result.Add(sideWearLeft);
            result.Add(sideWearRight);
            result.Add(treadTiltLeft);
            result.Add(treadTiltRight);
            result.Add(downhillLeft);
            result.Add(downhillRight);
            return result;
        }
    }
}