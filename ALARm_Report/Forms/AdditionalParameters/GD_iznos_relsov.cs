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
using System.Globalization;
using ALARm_Report.controls;

namespace ALARm_Report.Forms
{
    /// <summary>
    /// График диаграммы сводной по доп параметрам
    /// </summary>
    /// 
    public class GD_iznos_relsov : GraphicDiagrams
    {
        
        private float izBokR = 16.991f;
        private float izverL = 34.39f;
        private float izverR = 51.83f;
        private float izPRL = 69.77f;
        private float izPRR = 88.44f;
        private float iz45L = 107.15443f;
        private float iz45R = 125.844f;


        private float angleRuleWidth = 9.7f;
        private float gapsKoef = 9.5f/30f;
        private float iznosKoef = 8.8f / 20f;
        private float iznosKoef18 = 8.8f / 18f;
        private float iznosKoef16 = 8.8f / 16f;
        private float iznosKoef13 = 8.8f / 13f;

        private float gaspRightPosition = 14f;
        private float iznosLeftPosition = 28f;
        private float iznosRightPosition = 42f;
        private float PULeftPosition = 86.1f;
        private float PURightPosition = 100f;

        private float NPKLeftPosition = 57.5f;
        private float NPKRightPosition = 71.5f;

        /// <summary>
        /// шаблон
        ///  </summary>
        private float gaugeKoef = 0.5f;
        private float gaugePosition = 125f;
        private float GetDIstanceFrom1div60(float x)
        {
            return 15*angleRuleWidth * (1f/x - 1/60f);
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
            diagramName = "Износ рельсов";
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
                    tripProcess.Direction = Direction.Direct;

                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        //var kilometers = RdStructureService.GetKilometersByProcessId(tripProcess.Id);
                        var kilometers = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                        if (kilometers.Count() == 0) continue;

                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();
                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = kilometers.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = kilometers.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        kilometers = kilometers.Where(Km => ((float)(float)filters[0].Value <= Km && Km <= (float)(float)filters[1].Value)).ToList();

                        kilometers = kilometers.OrderByDescending(o => o).ToList();

                        progressBar.Maximum = kilometers.Count;
                        

                        foreach (var kilometer in kilometers)
                        {
                            //if (kilometer < 144) continue;

                            tripProcess.TrackName = "1";

                            progressBar.Value = kilometers.IndexOf(kilometer) + 1;

                            var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, $"{trackName}") as List<Speed>;
                            var sector = MainTrackStructureService.GetSector(track_id, kilometer, tripProcess.Date_Vrem);
                            var fragment = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.Fragments, tripProcess.DirectionName, $"{trackName}") as Fragment;
                            var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.MtoPdbSection, tripProcess.DirectionName, $"{trackName}") as List<PdbSection>;

                            var PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                            var FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;

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
                                    "<" + period.PeriodMonth + "-" + period.PeriodYear + " " + 
                                    "Проезд:" + tripProcess.Date_Vrem.ToShortDateString() + " " + tripProcess.Date_Vrem.ToShortTimeString() +
                                    " " + diagramName + ">" + " Л: " + (kilometers.IndexOf(kilometer) + 1)
                                ),
                                new XAttribute("fragment", $"{sector}  Км:{kilometer}"),
                                new XAttribute("viewbox", "0 0 770 1015"),
                                new XAttribute("minY", 0),
                                new XAttribute("maxY", 1000),

                                    RightSideChart(tripProcess.Date_Vrem, kilometer, Direction.Direct, tripProcess.DirectionID, trackName.ToString(), new float[] { 151f, 146f, 152.5f, 155f, -760 }),

                                new XElement("xgrid",
                                    new XAttribute("x0", xBegin),
                                    new XAttribute("x1", MMToPixelChart(izBokR)),
                                    new XAttribute("x2", MMToPixelChart(izPRL)),
                                    new XAttribute("x3", MMToPixelChart(izPRR)),
                                    new XAttribute("x31", MMToPixelChart(izverL)),
                                    new XAttribute("x32", MMToPixelChart(izverR)),
                                    new XAttribute("iz45R", MMToPixelChart(iz45R)),
                                    new XAttribute("iz45L", MMToPixelChart(iz45L))

                                ));
                            //var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromText(kilometer.Number);
                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(kilometer, tripProcess.Trip_id);
                            if (DBcrossRailProfile == null) continue;

                            var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(DBcrossRailProfile);

                            //сколь сред
                            var width = 10;

                            List<float> RollAver_SideWearLeft = new List<float>();
                            List<double> RollAver_SideWearRight = new List<double>();
                            List<double> RollAver_VertIznosL = new List<double>();
                            List<double> RollAver_VertIznosR = new List<double>();
                            List<double> RollAver_ReducedWearLeft = new List<double>();
                            List<double> RollAver_ReducedWearRight = new List<double>();
                            List<double> RollAver_HeadWearLeft = new List<double>();
                            List<double> RollAver_HeadWearRight = new List<double>();

                            List<float> SideWearLeft = new List<float>();
                            List<double> SideWearRight = new List<double>();
                            List<double> VertIznosL = new List<double>();
                            List<double> VertIznosR = new List<double>();
                            List<double> ReducedWearLeft = new List<double>();
                            List<double> ReducedWearRight = new List<double>();
                            List<double> HeadWearLeft = new List<double>();
                            List<double> HeadWearRight = new List<double>();

                            for (int i = 0; i < crossRailProfile.Meters.Count(); i++)
                            {
                                if (RollAver_SideWearLeft.Count >= width)
                                {
                                    RollAver_SideWearLeft.Add(crossRailProfile.SideWearLeft[i]);
                                    RollAver_SideWearRight.Add(crossRailProfile.SideWearRight[i]);
                                    RollAver_VertIznosL.Add(crossRailProfile.VertIznosL[i]);
                                    RollAver_VertIznosR.Add(crossRailProfile.VertIznosR[i]);
                                    RollAver_ReducedWearLeft.Add(crossRailProfile.ReducedWearLeft[i]);
                                    RollAver_ReducedWearRight.Add(crossRailProfile.ReducedWearRight[i]);
                                    RollAver_HeadWearLeft.Add(crossRailProfile.HeadWearLeft[i]);
                                    RollAver_HeadWearRight.Add(crossRailProfile.HeadWearRight[i]);

                                    var rasl = RollAver_SideWearLeft.Skip(RollAver_SideWearLeft.Count() - width).Take(width).Average();
                                    var rasr = RollAver_SideWearRight.Skip(RollAver_SideWearRight.Count() - width).Take(width).Average();
                                    var ravl = RollAver_VertIznosL.Skip(RollAver_VertIznosL.Count() - width).Take(width).Average();
                                    var ravr = RollAver_VertIznosR.Skip(RollAver_VertIznosR.Count() - width).Take(width).Average();
                                    var rarl = RollAver_ReducedWearLeft.Skip(RollAver_ReducedWearLeft.Count() - width).Take(width).Average();
                                    var rarr = RollAver_ReducedWearRight.Skip(RollAver_ReducedWearRight.Count() - width).Take(width).Average();
                                    var rahl = RollAver_HeadWearLeft.Skip(RollAver_HeadWearLeft.Count() - width).Take(width).Average();
                                    var rahr = RollAver_HeadWearRight.Skip(RollAver_HeadWearRight.Count() - width).Take(width).Average();

                                    SideWearLeft.Add(rasl);
                                    SideWearRight.Add(rasr);
                                    VertIznosL.Add(ravl);
                                    VertIznosR.Add(ravr);
                                    ReducedWearLeft.Add(rarl);
                                    ReducedWearRight.Add(rarr);
                                    HeadWearLeft.Add(rahl);
                                    HeadWearRight.Add(rahr);
                                }
                                else
                                {
                                    RollAver_SideWearLeft.Add(crossRailProfile.SideWearLeft[i]);
                                    RollAver_SideWearRight.Add(crossRailProfile.SideWearRight[i]);
                                    RollAver_VertIznosL.Add(crossRailProfile.VertIznosL[i]);
                                    RollAver_VertIznosR.Add(crossRailProfile.VertIznosR[i]);
                                    RollAver_ReducedWearLeft.Add(crossRailProfile.ReducedWearLeft[i]);
                                    RollAver_ReducedWearRight.Add(crossRailProfile.ReducedWearRight[i]);
                                    RollAver_HeadWearLeft.Add(crossRailProfile.HeadWearLeft[i]);
                                    RollAver_HeadWearRight.Add(crossRailProfile.HeadWearRight[i]);

                                    SideWearLeft.Add(crossRailProfile.SideWearLeft[i]);
                                    SideWearRight.Add(crossRailProfile.SideWearRight[i]);
                                    VertIznosL.Add(crossRailProfile.VertIznosL[i]);
                                    VertIznosR.Add(crossRailProfile.VertIznosR[i]);
                                    ReducedWearLeft.Add(crossRailProfile.ReducedWearLeft[i]);
                                    ReducedWearRight.Add(crossRailProfile.ReducedWearRight[i]);
                                    HeadWearLeft.Add(crossRailProfile.HeadWearLeft[i]);
                                    HeadWearRight.Add(crossRailProfile.HeadWearRight[i]);
                                }
                            }

                            //экспонента
                            //var crossRailProfile = new CrossRailProfile { };
                            var ExponentCoef = -5;

                            List<float> SideWearLeftExp = new List<float>();
                            List<float> SideWearRightExp = new List<float>();
                            List<float> VertIznosLExp = new List<float>();
                            List<float> VertIznosRExp = new List<float>();
                            List<float> ReducedWearLeftExp = new List<float>();
                            List<float> ReducedWearRightExp = new List<float>();
                            List<float> HeadWearLeftExp = new List<float>();
                            List<float> HeadWearRightExp = new List<float>();


                            for (int i = 0; i < SideWearLeft.Count; i++)
                            {
                                var esl = Math.Exp(ExponentCoef * Math.Abs(crossRailProfile.SideWearLeft[i] - SideWearLeft[i]));
                                var ksl = SideWearLeft[i] + (crossRailProfile.SideWearLeft[i] - SideWearLeft[i]) * esl;
                                SideWearLeftExp.Add((float)ksl);

                                var esr = Math.Exp(ExponentCoef * Math.Abs(crossRailProfile.SideWearRight[i] - SideWearRight[i]));
                                var ksr = SideWearRight[i] + (crossRailProfile.SideWearRight[i] - SideWearRight[i]) * esr;
                                SideWearRightExp.Add((float)ksl);

                                var evl = Math.Exp(ExponentCoef * Math.Abs(crossRailProfile.VertIznosL[i] - VertIznosL[i]));
                                var kvl = VertIznosL[i] + (crossRailProfile.VertIznosL[i] - VertIznosL[i]) * evl;
                                VertIznosLExp.Add((float)kvl);

                                var evr = Math.Exp(ExponentCoef * Math.Abs(crossRailProfile.VertIznosR[i] - VertIznosR[i]));
                                var kvr = VertIznosR[i] + (crossRailProfile.VertIznosR[i] - VertIznosR[i]) * evr;
                                VertIznosRExp.Add((float)kvr);

                                var elr = Math.Exp(ExponentCoef * Math.Abs(crossRailProfile.ReducedWearLeft[i] - ReducedWearLeft[i]));
                                var klr = ReducedWearLeft[i] + (crossRailProfile.ReducedWearLeft[i] - ReducedWearLeft[i]) * elr;
                                ReducedWearLeftExp.Add((float)klr);

                                var err = Math.Exp(ExponentCoef * Math.Abs(crossRailProfile.ReducedWearRight[i] - ReducedWearRight[i]));
                                var krr = ReducedWearRight[i] + (crossRailProfile.ReducedWearRight[i] - ReducedWearRight[i]) * err;
                                ReducedWearRightExp.Add((float)krr);

                                var ehl = Math.Exp(ExponentCoef * Math.Abs(crossRailProfile.HeadWearLeft[i] - HeadWearLeft[i]));
                                var khl = HeadWearLeft[i] + (crossRailProfile.HeadWearLeft[i] - HeadWearLeft[i]) * ehl;
                                HeadWearLeftExp.Add((float)khl);

                                var ehr = Math.Exp(ExponentCoef * Math.Abs(crossRailProfile.HeadWearRight[i] - HeadWearRight[i]));
                                var khr = HeadWearRight[i] + (crossRailProfile.HeadWearRight[i] - HeadWearRight[i]) * ehr;
                                HeadWearRightExp.Add((float)khr);
                            }

                            crossRailProfile.SideWearLeft.Clear();
                            crossRailProfile.SideWearRight.Clear();
                            crossRailProfile.VertIznosL.Clear();
                            crossRailProfile.VertIznosR.Clear();
                            crossRailProfile.ReducedWearLeft.Clear();
                            crossRailProfile.ReducedWearRight.Clear();
                            crossRailProfile.HeadWearLeft.Clear();
                            crossRailProfile.HeadWearRight.Clear();

                            crossRailProfile.SideWearLeft.AddRange(SideWearLeftExp);
                            crossRailProfile.SideWearRight.AddRange(SideWearRightExp);
                            crossRailProfile.VertIznosL.AddRange(VertIznosLExp);
                            crossRailProfile.VertIznosR.AddRange(VertIznosRExp);
                            crossRailProfile.ReducedWearLeft.AddRange(ReducedWearLeftExp);
                            crossRailProfile.ReducedWearRight.AddRange(ReducedWearRightExp);
                            crossRailProfile.HeadWearLeft.AddRange(HeadWearLeftExp);
                            crossRailProfile.HeadWearRight.AddRange(HeadWearRightExp);


                            List<Digression> addDigressions = crossRailProfile.GetDigressions();
                            var gapElements = new XElement("gaps");
                            var gaps = AdditionalParametersService.GetGaps(tripProcess.Id, (int)tripProcess.Direction, kilometer);

                            addParam.Add(gapElements);

                            List<string> polylines = GetCrossRailProfileLines(crossRailProfile, tripProcess.Direction);
                            var linesElem = new XElement("lines");
                            foreach (var polyline in polylines)
                            {
                                linesElem.Add(new XElement("line", polyline));
                            }
                            var digElemenets = new XElement("digressions");


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
                            addDigressions = addDigressions.OrderBy(o => o.Meter).ToList();

                            int secondSt = 0, thirdSt = 0, fourSt = 0;
                            foreach (var digression in addDigressions)
                            {
                                if (digression.Length < 1) continue;
                                if (digression.DigName != DigressionName.VertIznosL)
                                {
                                    if (digression.DigName != DigressionName.VertIznosR)
                                    {
                                        if (digression.DigName != DigressionName.SideWearLeft)
                                        {
                                            if (digression.DigName != DigressionName.SideWearRight)
                                            {
                                                if (digression.DigName != DigressionName.ReducedWearLeft)
                                                {
                                                    if (digression.DigName != DigressionName.ReducedWearRight) continue;
                                                }
                                            }
                                        }
                                    }
                                }
                                if ((int)digression.Typ == 2) secondSt += 1;
                                if ((int)digression.Typ == 3) secondSt += 1;
                                if ((int)digression.Typ == 4) secondSt += 1;

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
                                if (digression.Meter == 959)
                                {
                                    var m = digression.Meter;
                                }
                                int count = digression.Length / 4;
                                count += digression.Length % 4 > 0 ? 1 : 0;
                                digElemenets.Add(new XElement("dig",
                                    new XAttribute("n", digression.GetName() + " " + digression.Typ),
                                    new XAttribute("x1", digression.DigName == DigressionName.SideWearLeft ? "170" :
                                                        (digression.DigName == DigressionName.SideWearRight ? "238" :
                                                        (digression.DigName == DigressionName.VertIznosL ? "296" :
                                                        (digression.DigName == DigressionName.VertIznosR ? "366" :
                                                        (digression.DigName == DigressionName.ReducedWearLeft ? "448" :
                                                        (digression.DigName == DigressionName.ReducedWearRight ? "523" : "0")))))
                                    ),
                                    new XAttribute("w", MMToPixelChartWidthString((int)digression.Typ == 4 ? 4 : 2)),
                                    new XAttribute("y1", tripProcess.Direction == Direction.Reverse ? digression.Meter : 1000 - digression.Meter),
                                    new XAttribute("y2", tripProcess.Direction == Direction.Reverse ? digression.Kmetr :
                                    -digression.Kmetr),
                                    new XAttribute("top", y),
                                    new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : "normal"),
                                    new XAttribute("x", x),
                                    new XAttribute("note", (digression.Meter < 10 ? "    " : digression.Meter < 100 ? "  " : "") +
                                                            digression.Meter + " " + digression.GetName() + "     " + (int)digression.Typ + "     " +
                                                            digression.Value.ToString("0.0") + "   " + (digression.Length < 10 ? "  " : "") + digression.Length + "    " + (count < 10 ? "  " : "") + count
                                                            )
                                    ));
                                digElemenets.Add(new XElement("m",
                                                         new XAttribute("top", y),
                                                         new XAttribute("x", -23),
                                                         new XAttribute("note", digression.Meter),
                                                         new XAttribute("fw", (int)digression.Typ == 4 ? "bold" :
                                                                              ((int)digression.Typ == 3 ?
                                                                              ((digression.DigName == DigressionName.SideWearLeft || digression.DigName == DigressionName.SideWearRight) ? "bold" : "normal") : "normal"))
                                    ));
                                digElemenets.Add(new XElement("otst",
                                                     new XAttribute("top", y),
                                                     new XAttribute("x", -4),
                                                     new XAttribute("note", digression.GetName()),
                                                     new XAttribute("fw", (int)digression.Typ == 4 ? "bold" :
                                                                              ((int)digression.Typ == 3 ?
                                                                              ((digression.DigName == DigressionName.SideWearLeft || digression.DigName == DigressionName.SideWearRight) ? "bold" : "normal") : "normal"))
                                    ));
                                digElemenets.Add(new XElement("step",
                                                     new XAttribute("top", y),
                                                     new XAttribute("x", 29),
                                                     new XAttribute("note", digression.Typ),
                                                     new XAttribute("fw", (int)digression.Typ == 4 ? "bold" :
                                                                              ((int)digression.Typ == 3 ?
                                                                              ((digression.DigName == DigressionName.SideWearLeft || digression.DigName == DigressionName.SideWearRight) ? "bold" : "normal") : "normal"))
                                    ));
                                digElemenets.Add(new XElement("otkl",
                                                     new XAttribute("top", y),
                                                     new XAttribute("x", 44),
                                                     new XAttribute("note", digression.Value.ToString("0.0")),
                                                     new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : "normal")
                                    ));
                                digElemenets.Add(new XElement("len",
                                                     new XAttribute("top", y),
                                                     new XAttribute("x", 65),
                                                     new XAttribute("note", digression.Length),
                                                     new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : "normal")
                                    ));
                                digElemenets.Add(new XElement("count",
                                                     new XAttribute("top", y),
                                                     new XAttribute("x", 84),
                                                     new XAttribute("note", count == -1 ? "" : count.ToString()),
                                                     new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : "normal")
                                    ));
                                digElemenets.Add(new XElement("ogrsk",
                                                     new XAttribute("top", y),
                                                     new XAttribute("x", 105),
                                                     new XAttribute("note", "" + digression.AllowSpeed),
                                                     new XAttribute("fw", (int)digression.Typ == 4 ? "bold" : "normal")
                                    ));
                            }
                            addParam.Add(new XAttribute("common", "Кол.ст.-2:" + secondSt + "; 3:" + thirdSt + "; 4:" + fourSt));
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

                            addParam.Add(digElemenets);
                            //var mainParameters = MainParametersService.GetMainParameters(kilometer.Number); 
                            //polylines = GetMainParametersLines(mainParameters, tripProcess.Direction);
                            //foreach (var polyline in polylines)
                            //{
                            //    linesElem.Add(new XElement("line", polyline));
                            //}
                            addParam.Add(linesElem);

                            addParam.Add(
                                new XElement("bl", 18,
                                    new XAttribute("style",
                                        "padding-top:" + 34 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 15,
                                    new XAttribute("style",
                                        "padding-top:" + 2 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 8,
                                    new XAttribute("style",
                                        "padding-top:" + 1 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 0,
                                    new XAttribute("style",
                                        "padding-top:" + 13 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),

                                new XElement("bl", 18,
                                    new XAttribute("style",
                                        "padding-top:" + 31 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 15,
                                    new XAttribute("style",
                                        "padding-top:" + 2 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 8,
                                    new XAttribute("style",
                                        "padding-top:" + 1 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 0,
                                    new XAttribute("style",
                                        "padding-top:" + 13 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),

                                new XElement("bl", 16,
                                    new XAttribute("style",
                                        "padding-top:" + 37 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 8,
                                    new XAttribute("style",
                                        "padding-top:" + 1.8f + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 0,
                                    new XAttribute("style",
                                        "padding-top:" + 14 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),

                                new XElement("bl", 16,
                                    new XAttribute("style",
                                        "padding-top:" + 37 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 8,
                                    new XAttribute("style",
                                        "padding-top:" + 1 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 0,
                                    new XAttribute("style",
                                        "padding-top:" + 14 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),

                                new XElement("bl", 13,
                                    new XAttribute("style",
                                        "padding-top:" + 38 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 4,
                                    new XAttribute("style",
                                        "padding-top:" + 7 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 0,
                                    new XAttribute("style",
                                        "padding-top:" + 6 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),

                                new XElement("bl", 13,
                                    new XAttribute("style",
                                        "padding-top:" + 35 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 4,
                                    new XAttribute("style",
                                        "padding-top:" + 7 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 0,
                                    new XAttribute("style",
                                        "padding-top:" + 5 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),

                                new XElement("bl", 20,
                                    new XAttribute("style",
                                        "padding-top:" + 26 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 13,
                                    new XAttribute("style",
                                        "padding-top:" + 3 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 8,
                                    new XAttribute("style",
                                        "padding-top:" + 1 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 0,
                                    new XAttribute("style",
                                        "padding-top:" + 12 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),

                                new XElement("bl", 20,
                                    new XAttribute("style",
                                        "padding-top:" + 25 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 13,
                                    new XAttribute("style",
                                        "padding-top:" + 2 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 8,
                                    new XAttribute("style",
                                        "padding-top:" + 1 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM))),
                                new XElement("bl", 0,
                                    new XAttribute("style",
                                        "padding-top:" + 13 + ";height:" +
                                        MMToPixelLabel(BottomLabelHeightInMM)))
                            );
                            //SaveSVGasPng(template, addParam, kilometer);
                            report.Add(addParam);
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
                //шаблон
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
            string VertIznosL = string.Empty;
            string VertIznosR = string.Empty;
            string ReducedWearLeft = string.Empty;
            string ReducedWearRight = string.Empty;
            string HeadWearLeft = string.Empty;
            string HeadWearRight = string.Empty;

            string treadTiltLeft = string.Empty;
            string treadTiltRight = string.Empty;

            for (int i = 0; i < crossRailProfile.Meters.Count; i++)
            {
                string meter = ( travelDirection == Direction.Reverse ? crossRailProfile.Meters[i] : 1000- crossRailProfile.Meters[i]).ToString().Replace(",", ".");
                ////бок износ
                //sideWearLeft +=  MMToPixelChartString(iznosKoef * crossRailProfile.SideWearLeft[i]).Replace(",", ".") + "," + meter + " ";
                //sideWearRight += MMToPixelChartString(izBokR + iznosKoef * crossRailProfile.SideWearRight[i]).Replace(",", ".") +  "," + meter + " ";
                ////верт износ 
                //VertIznosL += (float.Parse(MMToPixelChartString(izverL + iznosKoef13 * crossRailProfile.VertIznosL[i]).Replace(",", "."), CultureInfo.InvariantCulture) - 4).ToString().Replace(",", ".") + "," + meter + " ";
                //VertIznosR += (float.Parse(MMToPixelChartString(izverR + iznosKoef13 * crossRailProfile.VertIznosR[i]).Replace(",", "."), CultureInfo.InvariantCulture) - 3).ToString().Replace(",", ".") + "," + meter + " ";
                ////прив износ
                //ReducedWearLeft += (float.Parse(MMToPixelChartString(izPRL + iznosKoef16 * crossRailProfile.ReducedWearLeft[i]).Replace(",", "."), CultureInfo.InvariantCulture) + 2.3).ToString().Replace(",", ".") + "," + meter + " ";
                //ReducedWearRight += (float.Parse(MMToPixelChartString(izPRR + iznosKoef16 * crossRailProfile.ReducedWearRight[i]).Replace(",", "."), CultureInfo.InvariantCulture) + 1.5).ToString().Replace(",", ".") + "," + meter + " ";
                ////износ головки рельса 45 град
                //HeadWearLeft += MMToPixelChartString(iz45L + iznosKoef18 * crossRailProfile.HeadWearLeft[i]).Replace(",", ".") + "," + meter + " ";
                //HeadWearRight += MMToPixelChartString(iz45R + iznosKoef18 * crossRailProfile.HeadWearRight[i]).Replace(",", ".") + "," + meter + " ";
                //---------------------------------------------------------------
                //бок износ
                sideWearLeft += MMToPixelChartString(1 * crossRailProfile.SideWearLeft[i]).Replace(",", ".") + "," + meter + " ";
                sideWearRight += MMToPixelChartString(izBokR + 1 * crossRailProfile.SideWearRight[i]).Replace(",", ".") + "," + meter + " ";
                //верт износ 
                VertIznosL += (float.Parse(MMToPixelChartString(izverL + 1 * crossRailProfile.VertIznosL[i]).Replace(",", "."), CultureInfo.InvariantCulture) ).ToString().Replace(",", ".") + "," + meter + " ";
                VertIznosR += (float.Parse(MMToPixelChartString(izverR + 1 * crossRailProfile.VertIznosR[i]).Replace(",", "."), CultureInfo.InvariantCulture) ).ToString().Replace(",", ".") + "," + meter + " ";
                //прив износ
                ReducedWearLeft += (float.Parse(MMToPixelChartString(izPRL +  crossRailProfile.ReducedWearLeft[i]).Replace(",", "."), CultureInfo.InvariantCulture) ).ToString().Replace(",", ".") + "," + meter + " ";
                ReducedWearRight += (float.Parse(MMToPixelChartString(izPRR + crossRailProfile.ReducedWearRight[i]).Replace(",", "."), CultureInfo.InvariantCulture) ).ToString().Replace(",", ".") + "," + meter + " ";
                //износ головки рельса 45 град
                HeadWearLeft += MMToPixelChartString(iz45L + 1 * crossRailProfile.HeadWearLeft[i]).Replace(",", ".") + "," + meter + " ";
                HeadWearRight += MMToPixelChartString(iz45R + 1 * crossRailProfile.HeadWearRight[i]).Replace(",", ".") + "," + meter + " ";

                //treadTiltLeft += MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(1/crossRailProfile.TreadTiltLeft[i])).Replace(",", ".") + "," + meter + " ";
                //treadTiltRight += MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(1/crossRailProfile.TreadTiltRight[i])).Replace(",", ".") + "," + meter + " ";

                //downhillLeft += MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(1 / crossRailProfile.DownhillLeft[i])).Replace(",", ".") + "," + meter + " ";
                //downhillRight += MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(1 / crossRailProfile.DownhillRight[i])).Replace(",", ".") + "," + meter + " ";
            }
            result.Add(sideWearLeft);
            result.Add(sideWearRight);
            result.Add(VertIznosL);
            result.Add(VertIznosR);
            result.Add(ReducedWearLeft);
            result.Add(ReducedWearRight);
            result.Add(HeadWearLeft);
            result.Add(HeadWearRight);
            //result.Add(treadTiltLeft);
            //result.Add(treadTiltRight);
            //result.Add(downhillLeft);
            //result.Add(downhillRight);
            return result;
        }
    }
}