
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
using System.Globalization;

namespace ALARm_Report.Forms
{
    /// <summary>
    /// График диаграммы сводной по доп параметрам
    /// </summary>
    /// 
    public class GD_PR : GraphicDiagrams
    {
        private float angleRuleWidth = 9.7f;
        private float gapsKoef = 9.5f / 30f;
        private float iznosKoef = 8.8f / 20f;

        private float gaspRightPosition = 14f;
        private float iznosLeftPosition = 28f;
        private float iznosRightPosition = 42f;
        private float PULeftPosition = 8.5f;
        private float PURightPosition = 41.5f;

        private float NPKLeftPosition = 57.5f + 23f;
        private float NPKRightPosition = 71.5f + 43f;

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
            diagramName = "ПР";
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
                    //if(tripProcess.Id!=228) continue;
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var trip = RdStructureService.GetTrip(tripProcess.Trip_id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);

                        if (kilometers.Count() == 0) continue;

                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var lkm = kilometers.Select(o => o.Number).ToList();

                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        kilometers = kilometers.Where(Km => ((float)(float)filters[0].Value <= Km.Number && Km.Number <= (float)(float)filters[1].Value)).ToList();
                        kilometers = (tripProcess.Direction == Direction.Reverse ? kilometers.OrderBy(o => o.Number) : kilometers.OrderByDescending(o => o.Number)).ToList();
                        //--------------------------------------------

                        progressBar.Maximum = kilometers.Count;

                        foreach (var kilometer in kilometers)
                        {
                            //tripProcess.Direction = Direction.Direct;
                            var outData = (List<OutData>)RdStructureService.GetNextOutDatas(kilometer.Start_Index - 1, kilometer.GetLength() - 1, kilometer.Trip.Id);
                            kilometer.AddDataRange(outData, kilometer);

                            progressBar.Value = kilometers.IndexOf(kilometer) + 1;

                            var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer.Number, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, $"{trackName}") as List<Speed>;
                            var sector = MainTrackStructureService.GetSector(track_id, kilometer.Number, tripProcess.Date_Vrem);
                            var fragment = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer.Number, MainTrackStructureConst.Fragments, tripProcess.DirectionName, $"{trackName}") as Fragment;
                            var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer.Number, MainTrackStructureConst.MtoPdbSection, tripProcess.DirectionName, $"{trackName}") as List<PdbSection>;

                            XElement addParam = new XElement("addparam",
                                new XAttribute("top-title",

                                    $"{tripProcess.DirectionName}({tripProcess.DirectionCode})  Путь:{trackName}  Км:" + kilometer.Number +

                                    $"  {(pdbSection.Count > 0 ? pdbSection[0].ToString() : "ПЧ-/ПЧУ-/ПД-/ПДБ-")}" +

                                    $" Уст: " + " " + (speed.Count > 0 ? $"{speed.First().Passenger}/{speed.First().Freight}" : "-/-") +
                                    $"  Скор:"),
                                  //$" Уст: " + " " + (speed.Count > 0 ? $"{speed.First().Passenger}/{speed.First().Freight}" : "-/-") +
                                  //  $"  Скор:{(int)kilometer.Speed.Average()}"),

                                new XAttribute("right-title",
                                    copyright + ": " + "ПО " + softVersion + "  " +
                                    systemName + ":" + tripProcess.Car + "(" + tripProcess.Chief + ") (БПД от " +
                                    MainTrackStructureService.GetModificationDate() + ") <" + AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, false) + ">" +
                                    "<" + kilometer.Passage_time.ToString("dd.MM.yyyy  HH:mm") + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.Direction.ToString())) + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.CarPosition.ToString())) + ">" +
                                    "<" + period.PeriodMonth + "-" + period.PeriodYear + " " + "контр. Проезд:" + tripProcess.Date_Vrem.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE")) +
                                    " " + diagramName + ">" + " Л: " + (kilometers.IndexOf(kilometer) + 1)),

                                new XAttribute("fragment", $"{sector}  Км:{kilometer.Number}"),

                                new XAttribute("viewbox", "0 0 770 1015"),
                                new XAttribute("minY", 0),
                                new XAttribute("maxY", 1000),

                                    RightSideChart(tripProcess.Date_Vrem, kilometer.Number, tripProcess.Direction, kilometer.Trip.Direction_id, kilometer.Track_name, new float[] { 151f, 146f, 152.5f, 155f, -760 }),

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

                            //var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromText(kilometer.Number);

                            //доп пар
                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(kilometer.Number, tripProcess.Trip_id);
                            if (DBcrossRailProfile == null) continue;

                            var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(DBcrossRailProfile);

                            ////сколь сред
                            var width = 150;

                            List<float> RollAver_TreadTiltLeft = new List<float>();
                            List<double> RollAver_TreadTiltRight = new List<double>();
                            List<double> RollAver_DownhillLeft = new List<double>();
                            List<double> RollAver_DownhillRight = new List<double>();

                            List<float> TreadTiltLeft = new List<float>();
                            List<double> TreadTiltRight = new List<double>();
                            List<double> DownhillLeft = new List<double>();
                            List<double> DownhillRight = new List<double>();

                            for (int i = 0; i < crossRailProfile.Meters.Count(); i++)
                            {
                                if (RollAver_TreadTiltLeft.Count >= width)
                                {
                                    RollAver_TreadTiltLeft.Add(crossRailProfile.TreadTiltLeft[i]);
                                    RollAver_TreadTiltRight.Add(crossRailProfile.TreadTiltRight[i]);
                                    RollAver_DownhillLeft.Add(crossRailProfile.DownhillLeft[i]);
                                    RollAver_DownhillRight.Add(crossRailProfile.DownhillRight[i]);

                                    var ratl = RollAver_TreadTiltLeft.Skip(RollAver_TreadTiltLeft.Count() - width).Take(width).Average();
                                    var ratr = RollAver_TreadTiltRight.Skip(RollAver_TreadTiltRight.Count() - width).Take(width).Average();
                                    var radl = RollAver_DownhillLeft.Skip(RollAver_DownhillLeft.Count() - width).Take(width).Average();
                                    var radr = RollAver_DownhillRight.Skip(RollAver_DownhillRight.Count() - width).Take(width).Average();

                                    TreadTiltLeft.Add(ratl);
                                    TreadTiltRight.Add(ratr);
                                    DownhillLeft.Add(radl);
                                    DownhillRight.Add(radr);
                                }
                                else
                                {
                                    RollAver_TreadTiltLeft.Add(crossRailProfile.TreadTiltLeft[i]);
                                    RollAver_TreadTiltRight.Add(crossRailProfile.TreadTiltRight[i]);
                                    RollAver_DownhillLeft.Add(crossRailProfile.DownhillLeft[i]);
                                    RollAver_DownhillRight.Add(crossRailProfile.DownhillRight[i]);

                                    TreadTiltLeft.Add(crossRailProfile.TreadTiltLeft[i]);
                                    TreadTiltRight.Add(crossRailProfile.TreadTiltRight[i]);
                                    DownhillLeft.Add(crossRailProfile.DownhillLeft[i]);
                                    DownhillRight.Add(crossRailProfile.DownhillRight[i]);
                                }
                            }

                            //экспонента
                            //var crossRailProfile = new CrossRailProfile { };
                            var ExponentCoef = -350;

                            List<float> TreadTiltLeftExp = new List<float>();
                            List<float> TreadTiltRightExp = new List<float>();
                            List<float> DownhillLeftExp = new List<float>();
                            List<float> DownhillRightExp = new List<float>();


                            for (int i = 0; i < TreadTiltLeft.Count; i++)
                            {
                                var etl = Math.Exp(ExponentCoef * Math.Abs(crossRailProfile.TreadTiltLeft[i] - TreadTiltLeft[i]));
                                var ktl = TreadTiltLeft[i] + (crossRailProfile.TreadTiltLeft[i] - TreadTiltLeft[i]) * etl;
                                TreadTiltLeftExp.Add((float)ktl);

                                var etr = Math.Exp(ExponentCoef * Math.Abs(crossRailProfile.TreadTiltRight[i] - TreadTiltRight[i]));
                                var ktr = TreadTiltRight[i] + (crossRailProfile.TreadTiltRight[i] - TreadTiltRight[i]) * etr;
                                TreadTiltRightExp.Add((float)ktr);

                                var edl = Math.Exp(ExponentCoef * Math.Abs(crossRailProfile.DownhillLeft[i] - DownhillLeft[i]));
                                var kdl = DownhillLeft[i] + (crossRailProfile.DownhillLeft[i] - DownhillLeft[i]) * edl;
                                DownhillLeftExp.Add((float)kdl);

                                var edr = Math.Exp(ExponentCoef * Math.Abs(crossRailProfile.DownhillRight[i] - DownhillRight[i]));
                                var kdr = DownhillRight[i] + (crossRailProfile.DownhillRight[i] - DownhillRight[i]) * edr;
                                DownhillRightExp.Add((float)kdr);
                            }

                            crossRailProfile.TreadTiltLeft.Clear();
                            crossRailProfile.TreadTiltRight.Clear();
                            crossRailProfile.DownhillLeft.Clear();
                            crossRailProfile.DownhillRight.Clear();

                            crossRailProfile.TreadTiltLeft.AddRange(TreadTiltLeftExp);
                            crossRailProfile.TreadTiltRight.AddRange(TreadTiltRightExp);
                            crossRailProfile.DownhillLeft.AddRange(DownhillLeftExp);
                            crossRailProfile.DownhillRight.AddRange(DownhillRightExp);

                            List<string> polylines = GetCrossRailProfileLines(crossRailProfile, tripProcess.Direction);
                            var linesElem = new XElement("lines");
                            foreach (var polyline in polylines)
                            {
                                linesElem.Add(new XElement("line", polyline));
                            }
                            var digElemenets = new XElement("digressions");
                            List<Digression> addDigressions = crossRailProfile.GetDigressions();

                            var gapElements = new XElement("gaps");
                            var gaps = AdditionalParametersService.GetGaps(tripProcess.Id, (int)tripProcess.Direction, kilometer.Number);

                            foreach (var gap in gaps)
                            {
                                if (gap.Length > 35) continue;

                                gapElements.Add(new XElement("g",
                                    new XAttribute("x", gap.Threat == Threat.Right ? xBegin : MMToPixelChart(gaspRightPosition)),
                                    new XAttribute("y", tripProcess.Direction == Direction.Reverse ? gap.Meter : 1000 - gap.Meter),
                                    new XAttribute("w", MMToPixelChartWidthString(gap.Length * gapsKoef))
                                    )
                                );
                                gap.PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                                gap.FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;
                                addDigressions.Add(gap.GetDigressions());
                            }
                            addParam.Add(gapElements);

                            int fs = 9;
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
                            addDigressions = addDigressions.OrderBy(o => o.Meter).ToList();
                            int secondSt = 0, thirdSt = 0, fourSt = 0;
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

                                int count = digression.Length / 4;
                                count += digression.Length % 4 > 0 ? 1 : 0;
                                if (digression.Typ != null)
                                {
                                    if ((int)digression.Typ == 2)
                                    {
                                        secondSt += count;
                                    }
                                    if ((int)digression.Typ == 3)
                                    {
                                        thirdSt += count;
                                    }
                                    if ((int)digression.Typ == 4)
                                    {
                                        fourSt += count;
                                    }
                                }

                                if ((digression.DigName == DigressionName.FusingGap) || (digression.DigName == DigressionName.AnomalisticGap) || (digression.DigName == DigressionName.Gap))
                                {
                                    digElemenets.Add(new XElement("dig",
                                    new XAttribute("n", digression.GetName()),
                                    new XAttribute("x1",
                                        digression.DigName == DigressionName.TreadTiltLeft ? "461" :
                                         (digression.DigName == DigressionName.TreadTiltRight ? "606.1829" :
                                            (digression.DigName == DigressionName.DownhillLeft ? "169" :
                                                (digression.DigName == DigressionName.DownhillRight ? "315" : "0")))
                                        ),
                                    new XAttribute("w", MMToPixelChartWidthString(4)),
                                    new XAttribute("y1", tripProcess.Direction == Direction.Reverse ? digression.Meter : 1000 - digression.Meter),
                                    new XAttribute("y2", tripProcess.Direction == Direction.Reverse ? digression.Kmetr : 1000 - digression.Kmetr),
                                    new XAttribute("top", y),
                                    new XAttribute("fw", "normal"),
                                    new XAttribute("x", -23),
                                    new XAttribute("note", (digression.Meter < 10 ? "    " : digression.Meter < 100 ? "  " : "") + digression.Meter + " " + digression.GetName()
                                                            + " " + (digression.Threat.ToString() == "1" ? "п." : "л.") +
                                                            (digression.DigName == DigressionName.Gap ? "            " : (digression.DigName == DigressionName.FusingGap ? "          " : "       ")) +
                                                            digression.Velich + "                   " + (digression.DigName == DigressionName.AnomalisticGap ? "" : digression.AllowSpeed)
                                    )));
                                    digElemenets.Add(new XElement("m",
                                                         new XAttribute("top", y),
                                                         new XAttribute("x", -23),
                                                         new XAttribute("note", digression.Meter),
                                                         new XAttribute("fw", (digression.DigName == DigressionName.Gap) ? "bold" : "normal")
                                        ));
                                    digElemenets.Add(new XElement("otst",
                                                         new XAttribute("top", y),
                                                         new XAttribute("x", -3),
                                                         new XAttribute("note", digression.GetName() + " " + (digression.Threat.ToString() == "1" ? "п." : "л.")),
                                                         new XAttribute("fw", (digression.DigName == DigressionName.Gap) ? "bold" : "normal")
                                        ));
                                    //digElemenets.Add(new XElement("step",
                                    //                     new XAttribute("top", y),
                                    //                     new XAttribute("x", 29),
                                    //                     new XAttribute("note", stepen),
                                    //                     new XAttribute("fw", "normal")
                                    //    ));
                                    digElemenets.Add(new XElement("otkl",
                                                         new XAttribute("top", y),
                                                         new XAttribute("x", 43),
                                                         new XAttribute("note", digression.Velich),
                                                         new XAttribute("fw", (digression.DigName == DigressionName.Gap) ? "bold" : "normal")
                                        ));
                                    //digElemenets.Add(new XElement("len",
                                    //                     new XAttribute("top", y),
                                    //                     new XAttribute("x", 68),
                                    //                     new XAttribute("note", dlina),
                                    //                     new XAttribute("fw", "normal")
                                    //    ));
                                    //digElemenets.Add(new XElement("count",
                                    //                     new XAttribute("top", y),
                                    //                     new XAttribute("x", 85),
                                    //                     new XAttribute("note", count == -1 ? "" : count.ToString()),
                                    //                     new XAttribute("fw", "normal")
                                    //    ));
                                    digElemenets.Add(new XElement("ogrsk",
                                                         new XAttribute("top", y),
                                                         new XAttribute("x", 105),
                                                         new XAttribute("note", "" + (digression.DigName == DigressionName.AnomalisticGap ? "" : digression.AllowSpeed)),
                                                         new XAttribute("fw", (digression.DigName == DigressionName.Gap) ? "bold" : "normal")
                                        ));
                                }
                                else
                                {
                                    if (digression.AllowSpeed != null)
                                    {
                                        var v = digression.AllowSpeed;
                                    }
                                    digElemenets.Add(new XElement("dig",
                                    new XAttribute("n", digression.GetName() + " " + digression.Typ),
                                    new XAttribute("x1", MMToPixelChartString(
                                                                digression.DigName == DigressionName.TreadTiltLeft ? NPKLeftPosition - 4f + GetDIstanceFrom1div60(30) :
                                                               (digression.DigName == DigressionName.TreadTiltRight ? NPKRightPosition - 2f + GetDIstanceFrom1div60(30) : -2))),
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
                                                         new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : ((digression.DigName == DigressionName.TreadTiltLeft) || (digression.DigName == DigressionName.TreadTiltRight) ? "bold" : "normal"))
                                        ));
                                    digElemenets.Add(new XElement("otst",
                                                         new XAttribute("top", y),
                                                         new XAttribute("x", -3),
                                                         new XAttribute("note", digression.GetName()),
                                                         new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : ((digression.DigName == DigressionName.TreadTiltLeft) || (digression.DigName == DigressionName.TreadTiltRight) ? "bold" : "normal"))
                                        ));
                                    digElemenets.Add(new XElement("step",
                                                         new XAttribute("top", y),
                                                         new XAttribute("x", 29),
                                                         new XAttribute("note", (int)digression.Typ),
                                                         new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : ((digression.DigName == DigressionName.TreadTiltLeft) || (digression.DigName == DigressionName.TreadTiltRight) ? "bold" : "normal"))
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
                                                         new XAttribute("x", 85),
                                                         new XAttribute("note", count),
                                                         new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : "normal")
                                        ));
                                    //digElemenets.Add(new XElement("ogrsk",
                                    //                     new XAttribute("top", y),
                                    //                     new XAttribute("x", 105),
                                    //                     new XAttribute("note", digression.DigName == DigressionName.AnomalisticGap ? "" : digression.AllowSpeed),
                                    //                     new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : "normal")
                                    //    ));
                                }
                            }
                            List<int> speedmetres = new List<int>();

                            if (speed.Count == 2)
                            {
                                int metre = int.Parse(speed.Count > 0 ? speed[0].Final_M.ToString() : "0");
                                speedmetres.Add(metre);
                                addParam.Add(new XElement("speedline",
                                    new XAttribute("y1", metre),
                                    new XAttribute("y2", metre + 10),
                                    new XAttribute("y3", metre - 10),
                                    new XAttribute("note1", $"{metre} Уст.ск:{speed[0]}"),
                                    new XAttribute("note2", "       Уст.ск:" + speed[1])));
                            }

                            addParam.Add(
                                new XAttribute("kol", "Кол.ст.-2:" + secondSt + "; 3:" + thirdSt + "; 4:" + fourSt)
                                );
                            addParam.Add(digElemenets);
                            //var mainParameters = MainParametersService.GetMainParameters(kilometer.Number);
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
                string meter = (travelDirection == Direction.Reverse ? crossRailProfile.Meters[i] : 1000 - crossRailProfile.Meters[i]).ToString().Replace(",", ".");

                //sideWearLeft += MMToPixelChartString(iznosLeftPosition + iznosKoef * crossRailProfile.SideWearLeft[i]).Replace(",", ".") + "," + meter + " ";
                //sideWearRight += MMToPixelChartString(iznosRightPosition + iznosKoef * crossRailProfile.SideWearRight[i]).Replace(",", ".") + "," + meter + " ";
                //нпк
                treadTiltLeft += MMToPixelChartString(NPKLeftPosition + 1f * GetDIstanceFrom1div60(1 / crossRailProfile.TreadTiltLeft[i])).Replace(",", ".") + "," + meter + " ";
                treadTiltRight += MMToPixelChartString(NPKRightPosition + 1f * GetDIstanceFrom1div60(1 / crossRailProfile.TreadTiltRight[i])).Replace(",", ".") + "," + meter + " ";
                //пу
                downhillLeft += MMToPixelChartString(PULeftPosition + 1f * GetDIstanceFrom1div60(1 / crossRailProfile.DownhillLeft[i])).Replace(",", ".") + "," + meter + " ";
                downhillRight += MMToPixelChartString(PURightPosition + 1f * GetDIstanceFrom1div60(1 / crossRailProfile.DownhillRight[i])).Replace(",", ".") + "," + meter + " ";
            }
            //result.Add(sideWearLeft);
            //result.Add(sideWearRight);
            result.Add(treadTiltLeft);
            result.Add(treadTiltRight);
            result.Add(downhillLeft);
            result.Add(downhillRight);
            return result;
        }
    }
}