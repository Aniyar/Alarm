using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// <summary>
    /// График диаграммы КН-1
    /// 
    /// 
    /// 
    /// </summary>
    public class AdditionalParameters : GraphicDiagrams
    {
        private float longWaveLeftPosition;
        private float LongWaveRightPosition;
        private float MiddleWaveLeftPosition;
        private float MiddleWaveRightPosition;
        private float ShortWaveLeftPosition;
        private float ShortWaveRightPosition;
        private float ImpulsRoughnessLeftPosition;
        private float ImpulsRoughnessRightPosition;
        

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


            diagramName = "КН-1";
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                float koef = 3.5f / 0.6f;
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");


                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);


                //var tripProcesses = RdStructureService.GetMainParametersProcess(period, parentId.ToString());
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                int svgIndex = template.Xsl.IndexOf("</svg>");
                template.Xsl = template.Xsl.Insert(svgIndex, righstSideXslt());
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        var kilometers = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);

                        if (kilometers.Count() == 0) continue;

                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = kilometers.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = kilometers.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        kilometers = kilometers.Where(Km => ((float)filters[0].Value <= Km && Km <= (float)filters[1].Value)).ToList();
                        //--------------------------------------------


                        kilometers = kilometers.OrderByDescending(ii => ii).ToList();

                        progressBar.Maximum = kilometers.Count;

                        var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyTripId(tripProcess.Trip_id);
                        if (DBcrossRailProfile == null || DBcrossRailProfile.Count() == 0) continue;



                        ////Linaer graph-----------
                        //var straightProfilesNew = new List<StraightProfile>();
                        //var linearPointYNew = new List<float>();
                        //var Y = DBcrossRailProfile.Select(o=>o.Vert_l).ToList();
                        //var X = new List<int> { };

                        //for (int kk = 0; kk < DBcrossRailProfile.Count; kk++)
                        //{
                        //    X.Add(kk + 1);
                        //}

                        //straightProfilesNew.Add(new StraightProfile
                        //{
                        //    CoordAbs = X[0],
                        //    Profile = Y[0],
                        //    SlopeDiff = double.NaN
                        //});

                        //int i = 0;

                        //for (int j = i + 1; j < Y.Count - 1; j++)
                        //{
                        //    double
                        //        Bx = X[j] - X[i],
                        //        By = Y[j] - Y[i];

                        //    //double otnosh = By / Bx;

                        //    for (int k = i; k < j + 1; k++)
                        //    {
                        //        var A = -By;
                        //        var B = Bx;
                        //        var C = X[i] * Y[j] - X[j] * Y[i];

                        //        var d = Math.Abs(A * X[k] + B * Y[k] + C) / Math.Sqrt(A * A + B * B);

                        //        //double maxDistance = Math.Abs((Y[k] - Y[i]) - otnosh * (X[k] - X[i]));

                        //        if (d > 0.5 && Math.Abs(straightProfilesNew.Last().CoordAbs - X[j - 1]) > 2)
                        //        {
                        //            var tempSP = straightProfilesNew.Last();
                        //            tempSP.Len = X[j - 1] - tempSP.CoordAbs;
                        //            //tempSP.Slope = Convert.ToDouble(Y[j - 1] - tempSP.Profile) * 10/ tempSP.Len;
                        //            tempSP.Slope = Convert.ToDouble(Y[j - 1] - tempSP.Profile);


                        //            straightProfilesNew.Add(new StraightProfile
                        //            {
                        //                CoordAbs = X[j - 1],
                        //                Profile = Y[j - 1]
                        //            });

                        //            i = j - 1;
                        //            break;
                        //        }
                        //    }
                        //}



                        //if (!straightProfilesNew.Where(s => s.CoordAbs == X[Y.IndexOf(Y.Last())] && s.Profile == Y.Last()).Any())
                        //{
                        //    var tempSP = straightProfilesNew.Last();
                        //    tempSP.Len = X[Y.IndexOf(Y.Last())] - tempSP.CoordAbs;
                        //    //tempSP.Slope = Convert.ToDouble(Y.Last() - tempSP.Profile) * 10 / tempSP.Len;
                        //    tempSP.Slope = Convert.ToDouble(Y.Last() - tempSP.Profile);

                        //    straightProfilesNew.Add(new StraightProfile
                        //    {

                        //        CoordAbs = X[Y.IndexOf(Y.Last())],
                        //        Profile = Y.Last(),
                        //        Len = 0,
                        //        Slope = double.NaN,
                        //        SlopeDiff = double.NaN
                        //    });
                        //}

                        //for (i = 1; i < straightProfilesNew.Count - 1; i++)
                        //{
                        //    straightProfilesNew[i].SlopeDiff = straightProfilesNew[i].Slope - straightProfilesNew[i - 1].Slope;
                        //}



                        //for (int t = 0; t < straightProfilesNew.Count() - 1; t++)
                        //{
                        //    for (int c = 0; c < straightProfilesNew[t + 1].CoordAbs - straightProfilesNew[t].CoordAbs; c++)
                        //    {

                        //        // Console.Out.WriteLine("str x "+ straightProfiles[t + 1].CoordAbs);
                        //        double x = straightProfilesNew[t].CoordAbs + c;
                        //        double dx1 = x - straightProfilesNew[t].CoordAbs;

                        //        double bottom_dx1 = straightProfilesNew[t + 1].CoordAbs - straightProfilesNew[t].CoordAbs;

                        //        double y2 = straightProfilesNew[t + 1].Profile;
                        //        double dx2 = x - straightProfilesNew[t + 1].CoordAbs;
                        //        double bottom_dx2 = straightProfilesNew[t].CoordAbs - straightProfilesNew[t + 1].CoordAbs;

                        //        double y1 = straightProfilesNew[t].Profile;
                        //        var linearY = (y2 - y1) / bottom_dx1 * c + y1;

                        //        linearPointYNew.Add((float)linearY);
                        //    }
                        //}

                        //for (int e = 0; e < linearPointYNew.Count; e++)
                        //{
                        //    DBcrossRailProfile[e+3].Longwavesleft = DBcrossRailProfile[e+3].Vert_l - linearPointYNew[e];
                        //}
                        //-----------------------

                        foreach (var kilometer in kilometers)
                        {                         
                            progressBar.Value = kilometers.IndexOf(kilometer) + 1;

                            LongWaveRightPosition = 5f;
                            longWaveLeftPosition = 23.8f;
                            MiddleWaveRightPosition = 42.6f;
                            MiddleWaveLeftPosition = 61.4f;
                            ShortWaveRightPosition = 80.2f;
                            ShortWaveLeftPosition = 99f;
                            ImpulsRoughnessRightPosition = 122f;
                            ImpulsRoughnessLeftPosition = 138f;

                            var speed =      MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, $"{trackName}") as List<Speed>;
                            var sector =     MainTrackStructureService.GetSector(track_id, kilometer, tripProcess.Date_Vrem);
                            var fragment =   MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.Fragments, tripProcess.DirectionName, $"{trackName}") as Fragment;
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
                                    //"<" + DateTime.Now.ToShortDateString() +" " + DateTime.Now.ToShortTimeString() + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.Direction.ToString())) + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.CarPosition.ToString())) + ">" +
                                    "<" + period.PeriodMonth + "-" + period.PeriodYear + " " + "контр. Проезд:" + tripProcess.Date_Vrem.ToShortDateString() + " " + tripProcess.Date_Vrem.ToShortTimeString() +
                                    " " + diagramName + ">" + " Л: " + (kilometers.IndexOf(kilometer) + 1)),

                                new XAttribute("fragment", $"{sector}  Км:{kilometer}"),
                                new XAttribute("viewbox", "0 0 770 1015"),
                                new XAttribute("minY", 0),
                                new XAttribute("maxY", 1000),

                                    RightSideChart(tripProcess.Date_Vrem, kilometer, Direction.Direct, tripProcess.DirectionID, trackName.ToString(), new float[] { 151f, 146f, 152.5f, 155f, -760 }),

                                new XElement("xgrid",
                                    new XElement("x", MMToPixelChartString(longWaveLeftPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black")),
                                    new XElement("x", MMToPixelChartString(longWaveLeftPosition + 3.5f), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey")),
                                    new XElement("x", MMToPixelChartString(LongWaveRightPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black")),
                                    new XElement("x", MMToPixelChartString(LongWaveRightPosition + 3.5f), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey")),
                                    new XElement("x", MMToPixelChartString(MiddleWaveLeftPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black")),
                                    new XElement("x", MMToPixelChartString(MiddleWaveLeftPosition + 2.625f), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey")),
                                    new XElement("x", MMToPixelChartString(MiddleWaveRightPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black")),
                                    new XElement("x", MMToPixelChartString(MiddleWaveRightPosition + 2.625f), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey")),
                                    new XElement("x", MMToPixelChartString(ShortWaveLeftPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black")),
                                    new XElement("x", MMToPixelChartString(ShortWaveLeftPosition + 1.16666f), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey")),
                                    new XElement("x", MMToPixelChartString(ShortWaveRightPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black")),
                                    new XElement("x", MMToPixelChartString(ShortWaveRightPosition + 1.16666f), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey")),
                                    new XElement("x", MMToPixelChartString(ImpulsRoughnessLeftPosition), new XAttribute("dasharray", "3,0"), new XAttribute("stroke", "black")),
                                    new XElement("x", MMToPixelChartString(ImpulsRoughnessLeftPosition + 1.5f), new XAttribute("dasharray", "3,0"), new XAttribute("stroke", "black")),
                                    new XElement("x", MMToPixelChartString(ImpulsRoughnessRightPosition), new XAttribute("dasharray", "3,0"), new XAttribute("stroke", "black")),
                                    new XElement("x", MMToPixelChartString(ImpulsRoughnessRightPosition + 1.5f), new XAttribute("dasharray", "3,0"), new XAttribute("stroke", "black"))
                                    ));

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

                            float koefShort = 3.5f / 0.6f;
                            float koefMedium = 2.625f / 0.45f;
                            float koefLong = 1.1666f / 0.2f;


                            var Current_DBcrossRailProfile = DBcrossRailProfile.Where(o=>o.Km==kilometer).ToList();
                            if (Current_DBcrossRailProfile == null || Current_DBcrossRailProfile.Count() == 0) continue;

                            var shortRoughness = AdditionalParametersService.GetShortRoughnessFromDBParse(Current_DBcrossRailProfile);

                            shortRoughness.ShortWaveRight.Clear();
                            shortRoughness.MediumWaveRight.Clear();
                            shortRoughness.LongWaveRight.Clear();

                            shortRoughness.ShortWaveLeft.Clear();
                            shortRoughness.MediumWaveLeft.Clear();
                            shortRoughness.LongWaveLeft.Clear();

                            shortRoughness.ShortWaveRight.AddRange(Current_DBcrossRailProfile.Select(o=>o.Shortwavesright));
                            shortRoughness.MediumWaveRight.AddRange(Current_DBcrossRailProfile.Select(o => o.Mediumwavesright));
                            shortRoughness.LongWaveRight.AddRange(Current_DBcrossRailProfile.Select(o => o.Longwavesright));

                            shortRoughness.ShortWaveLeft.AddRange(Current_DBcrossRailProfile.Select(o => o.Shortwavesleft));
                            shortRoughness.MediumWaveLeft.AddRange(Current_DBcrossRailProfile.Select(o => o.Mediumwavesleft));
                            shortRoughness.LongWaveLeft.AddRange(Current_DBcrossRailProfile.Select(o => o.Longwavesleft));

                            //List<Speed> settedSpeeds = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem,
                            //                                                                      kilometer, MainTrackStructureConst.MtoSpeed,
                            //                                                                      tripProcess.DirectionName,
                            //                                                                      "1"
                            //                                                                      ) as List<Speed>; //toDo trackNumber
                            //var indicators = shortRoughness.GetIntegratedIndicators(settedSpeeds.Count > 0 ? settedSpeeds[0].Passenger : 140);

                            //сколь сред
                            var widthSH = 10;
                            var widthMD = 60;
                            var widthLG = 60;

                            List<double> RollAver_ShortWaveRight = new List<double>();
                            List<double> RollAver_MediumWaveRight = new List<double>();
                            List<double> RollAver_LongWaveRight = new List<double>();

                            List<double> RollAver_ShortWaveLeft = new List<double>();
                            List<double> RollAver_MediumWaveLeft = new List<double>();
                            List<double> RollAver_LongWaveLeft = new List<double>();

                            List<double> ShortWaveRight = new List<double>();
                            List<double> MediumWaveRight = new List<double>();
                            List<double> LongWaveRight = new List<double>();

                            List<double> ShortWaveLeft = new List<double>();
                            List<double> MediumWaveLeft = new List<double>();
                            List<double> LongWaveLeft = new List<double>();

                            for (int i = 0; i < shortRoughness.MetersRight.Count(); i++)
                            {
                                if (RollAver_ShortWaveRight.Count >= widthSH)
                                {
                                    RollAver_ShortWaveRight.Add(shortRoughness.ShortWaveRight[i]);

                                    RollAver_ShortWaveLeft.Add(shortRoughness.ShortWaveLeft[i]);

                                    var rasr = RollAver_ShortWaveRight.Skip(RollAver_ShortWaveRight.Count() - widthSH).Take(widthSH).Average();

                                    var rasl = RollAver_ShortWaveLeft.Skip(RollAver_ShortWaveLeft.Count() - widthSH).Take(widthSH).Average();

                                    ShortWaveRight.Add(rasr);

                                    ShortWaveLeft.Add(rasl);
                                }
                                else
                                {
                                    RollAver_ShortWaveRight.Add(0.0);

                                    RollAver_ShortWaveLeft.Add(0.0);

                                    ShortWaveRight.Add(0.0);

                                    ShortWaveLeft.Add(0.0);
                                }

                                if (RollAver_MediumWaveRight.Count >= widthMD)
                                {
                                    RollAver_MediumWaveRight.Add(shortRoughness.MediumWaveRight[i]);

                                    RollAver_MediumWaveLeft.Add(shortRoughness.MediumWaveLeft[i]);

                                    var ramr = RollAver_MediumWaveRight.Skip(RollAver_MediumWaveRight.Count() - widthMD).Take(widthMD).Average();

                                    var raml = RollAver_MediumWaveLeft.Skip(RollAver_MediumWaveLeft.Count() - widthMD).Take(widthMD).Average();

                                    MediumWaveRight.Add(ramr);

                                    MediumWaveLeft.Add(raml);
                                }
                                else
                                {
                                    RollAver_MediumWaveRight.Add(0.0);

                                    RollAver_MediumWaveLeft.Add(0.0);

                                    MediumWaveRight.Add(0.0);

                                    MediumWaveLeft.Add(0.0);
                                }

                                if (RollAver_LongWaveRight.Count >= widthLG)
                                {
                                    RollAver_LongWaveRight.Add(shortRoughness.LongWaveRight[i]);

                                    RollAver_LongWaveLeft.Add(shortRoughness.LongWaveLeft[i]);

                                    var ralr = RollAver_LongWaveRight.Skip(RollAver_LongWaveRight.Count() - widthLG).Take(widthLG).Average();

                                    var rall = RollAver_LongWaveLeft.Skip(RollAver_LongWaveLeft.Count() - widthLG).Take(widthLG).Average();

                                    LongWaveRight.Add(ralr);

                                    LongWaveLeft.Add(rall);
                                }
                                else
                                {
                                    RollAver_LongWaveRight.Add(0.0);

                                    RollAver_LongWaveLeft.Add(0.0);

                                    LongWaveRight.Add(0.0);

                                    LongWaveLeft.Add(0.0);
                                }
                            }

                            //экспонента
                            //var crossRailProfile = new CrossRailProfile { };
                            var ExponentCoefSH = -5;
                            var ExponentCoefMD = -20;
                            var ExponentCoefLG = -20;

                            List<float> ShortWaveRightExp = new List<float>();
                            List<float> MediumWaveRightExp = new List<float>();
                            List<float> LongWaveRightExp = new List<float>();

                            List<float> ShortWaveLeftExp = new List<float>();
                            List<float> MediumWaveLeftExp = new List<float>();
                            List<float> LongWaveLeftExp = new List<float>();


                            for (int i = 0; i < ShortWaveRight.Count; i++)
                            {
                                var esr = Math.Exp(ExponentCoefSH * Math.Abs(shortRoughness.ShortWaveRight[i] - ShortWaveRight[i]));
                                var ksr = ShortWaveRight[i] + (shortRoughness.ShortWaveRight[i] - ShortWaveRight[i]) * esr;
                                ShortWaveRightExp.Add((float)ksr);

                                var emr = Math.Exp(ExponentCoefMD * Math.Abs(shortRoughness.MediumWaveRight[i] - MediumWaveRight[i]));
                                var kmr = MediumWaveRight[i] + (shortRoughness.MediumWaveRight[i] - MediumWaveRight[i]) * emr;
                                MediumWaveRightExp.Add((float)kmr);

                                var elr = Math.Exp(ExponentCoefLG * Math.Abs(shortRoughness.LongWaveRight[i] - LongWaveRight[i]));
                                var klr = LongWaveRight[i] + (shortRoughness.LongWaveRight[i] - LongWaveRight[i]) * elr;
                                LongWaveRightExp.Add((float)klr);


                                var esl = Math.Exp(ExponentCoefSH * Math.Abs(shortRoughness.ShortWaveLeft[i] - ShortWaveLeft[i]));
                                var ksl = ShortWaveLeft[i] + (shortRoughness.ShortWaveLeft[i] - ShortWaveLeft[i]) * esl;
                                ShortWaveLeftExp.Add((float)ksl);

                                var eml = Math.Exp(ExponentCoefMD * Math.Abs(shortRoughness.MediumWaveLeft[i] - MediumWaveLeft[i]));
                                var kml = MediumWaveLeft[i] + (shortRoughness.MediumWaveLeft[i] - MediumWaveLeft[i]) * eml;
                                MediumWaveLeftExp.Add((float)kml);

                                var ell = Math.Exp(ExponentCoefLG * Math.Abs(shortRoughness.LongWaveLeft[i] - LongWaveLeft[i]));
                                var kll = LongWaveLeft[i] + (shortRoughness.LongWaveLeft[i] - LongWaveLeft[i]) * ell;
                                LongWaveLeftExp.Add((float)kll);
                            }

                            shortRoughness.ShortWaveRight.Clear();
                            shortRoughness.MediumWaveRight.Clear();
                            shortRoughness.LongWaveRight.Clear();

                            shortRoughness.ShortWaveLeft.Clear();
                            shortRoughness.MediumWaveLeft.Clear();
                            shortRoughness.LongWaveLeft.Clear();

                            shortRoughness.ShortWaveRight.AddRange(ShortWaveRightExp);
                            shortRoughness.MediumWaveRight.AddRange(MediumWaveRightExp);
                            shortRoughness.LongWaveRight.AddRange(LongWaveRightExp);

                            shortRoughness.ShortWaveLeft.AddRange(ShortWaveLeftExp);
                            shortRoughness.MediumWaveLeft.AddRange(MediumWaveLeftExp);
                            shortRoughness.LongWaveLeft.AddRange(LongWaveLeftExp);



                            //var shortRoughness = AdditionalParametersService.GetShortRoughnessFromText(kilometer.Number);
                            var polyLines = GetPolylines(tripProcess.Direction, shortRoughness.MetersLeft,
                                new List<float>[] { shortRoughness.ShortWaveLeft, shortRoughness.MediumWaveLeft, shortRoughness.LongWaveLeft },
                                new float[] { ShortWaveLeftPosition, MiddleWaveLeftPosition, longWaveLeftPosition },
                                new float[] { koefShort, koefMedium, koefLong });

                            foreach (var polyLine in polyLines)
                            {
                                addParam.Add(new XElement("polyline", new XAttribute("points", polyLine)));
                            }

                            polyLines = GetPolylines(tripProcess.Direction, shortRoughness.MetersRight,
                                new List<float>[] { shortRoughness.ShortWaveRight, shortRoughness.MediumWaveRight, shortRoughness.LongWaveRight },
                                new float[] { ShortWaveRightPosition, MiddleWaveRightPosition, LongWaveRightPosition },
                                new float[] { koefShort, koefMedium, koefLong });

                            foreach (var polyLine in polyLines)
                            {
                                addParam.Add(new XElement("polyline", new XAttribute("points", polyLine)));
                            }

                            

                            List<Digression> digressions = shortRoughness.GetDigressions();

                            XElement impulsLeft = new XElement("impulsleft", 
                                                        new XAttribute("x1", MMToPixelChartString(ImpulsRoughnessLeftPosition)), 
                                                        new XAttribute("x3", MMToPixelChartString(ImpulsRoughnessLeftPosition + 1.5f))
                                                        );
                            XElement impulsRight = new XElement("impulsright", 
                                                        new XAttribute("x1", MMToPixelChartString(ImpulsRoughnessRightPosition)), 
                                                        new XAttribute("x3", MMToPixelChartString(ImpulsRoughnessRightPosition + 1.5f))
                                                        );

                            addParam.Add(impulsRight);
                            addParam.Add(impulsLeft);


                            var digElemenets = new XElement("digressions");
                            int fs = 9;

                            int picket1 = 998;
                            int picket2 = 902;
                            int picket3 = 798;
                            int picket4 = 702;
                            int picket5 = 598;
                            int picket6 = 502;
                            int picket7 = 398;
                            int picket8 = 302;
                            int picket9 = 198;
                            int picket10 = 102;

                            int x1 = -10, x2 = -10, x3 = -10, x4 = -10, x5 = -10, x6 = -10, x7 = -10, x8 = -10, x9 = -10, x10 = -10;

                            //импульсы
                            var impulses = AdditionalParametersService.GetImpulsesByKm(kilometer);
                            for (int i = 0; i < impulses.Count; i++)
                            {

                                var impuls = impulses[i].Threat == Threat.Left ? impulsLeft : impulsRight;
                                var position = impulses[i].Threat == Threat.Left ? ImpulsRoughnessLeftPosition : ImpulsRoughnessRightPosition;
                                var digname = impulses[i].Threat == Threat.Left ? DigressionName.ImpulsLeft : DigressionName.ImpulsRight;

                                if ((impulses[i].Len> 0 && impulses[i].Len < 301) && impulses[i].Intensity_ra * 0.3 > 0.6f)
                                {
                                    digressions.Add(new Digression() { Meter = impulses[i].Meter, Length = impulses[i].Len, DigName = digname, Value = (float)impulses[i].Intensity_ra * 0.3f });
                                }

                                impuls.Add(new XElement("imp",
                                    new XAttribute("x2", MMToPixelChartString(position - impulses[i].Length * 0.3)),
                                    new XAttribute("x4", MMToPixelChartString(position + 1.5f + impulses[i].Intensity_ra * 0.3)),
                                    new XAttribute("y", impulses[i].Meter)
                                    ));
                                addParam.Add(impuls);
                            }

                            digressions = digressions.OrderByDescending(o => o.Meter).ToList();

                            foreach (var digression in digressions)
                            {
                                var dname = digression.GetName();
                                if (digression.Length < 1)
                                    continue;
                                int y = 0;
                                int x = -4;
                                switch (digression.Meter)
                                {
                                    case int meter when meter > 0 && meter <= 100:
                                        y = picket1;
                                        x = x1;
                                        picket1 += -fs;
                                        if (picket1 == 902)
                                        {
                                            picket1 = 998;
                                            //x1 += 65;
                                        }
                                        break;
                                    case int meter when meter > 100 && meter <= 200:
                                        y = picket2;
                                        x = x2;
                                        picket2 += -fs;

                                        if (picket2 == 798)
                                        {
                                            picket2 = 902;
                                            // x2 += 65;
                                        }
                                        break;
                                    case int meter when meter > 200 && meter <= 300:
                                        y = picket3;
                                        x = x3;
                                        picket3 += -fs;
                                        if (picket3 == 702)
                                        {
                                            picket3 = 798;
                                            // x3 += 65;
                                        }
                                        break;
                                    case int meter when meter > 300 && meter <= 400:
                                        y = picket4;
                                        x = x4;
                                        picket4 += -fs;
                                        if (picket4 == 598)
                                        {
                                            picket4 = 702;
                                            // x4 += 65;
                                        }
                                        break;
                                    case int meter when meter > 400 && meter <= 500:
                                        y = picket5;
                                        x = x5;
                                        picket5 += -fs;
                                        if (picket5 == 502)
                                        {
                                            picket5 = 598;
                                            //x5 += 65;
                                        }
                                        break;
                                    case int meter when meter > 500 && meter <= 600:
                                        y = picket6;
                                        x = x6;
                                        picket6 += -fs;
                                        if (picket6 == 398)
                                        {
                                            picket6 = 502;
                                            // x6 += 65;
                                        }
                                        break;
                                    case int meter when meter > 600 && meter <= 700:
                                        y = picket7;
                                        x = x7;
                                        picket7 += -fs;
                                        if (picket7 == 302)
                                        {
                                            picket7 = 398;
                                            // x7 += 65;
                                        }
                                        break;
                                    case int meter when meter > 700 && meter <= 800:
                                        y = picket8;
                                        x = x8;
                                        picket8 += -fs;
                                        if (picket8 == 198)
                                        {
                                            picket8 = 302;
                                            // x8 += 65;
                                        }
                                        break;
                                    case int meter when meter > 800 && meter <= 900:
                                        y = picket9;
                                        x = x9;
                                        picket9 += -fs;
                                        if (picket9 == 102)
                                        {
                                            picket9 = 198;
                                            // x9 += 65;
                                        }
                                        break;
                                    case int meter when meter > 900 && meter <= 1000:
                                        y = picket10;
                                        x = x10;
                                        picket10 += -fs;
                                        if (picket10 == 6)
                                        {
                                            picket10 = 102;
                                            // x10 += 65;
                                        }
                                        break;
                                }

                                float count = digression.Length / 100.0f;
                                float imp_count = 0.0f;
                                if ((digression.DigName == DigressionName.ImpulsLeft) || (digression.DigName == DigressionName.ImpulsRight))
                                {
                                    imp_count = digression.Length / 40.0f;
                                }

                                //digElemenets.Add(new XElement("dig",

                                //    new XAttribute("top", y),
                                //    new XAttribute("x", x),
                                //    new XAttribute("note", (digression.Meter < 10 ? "   " : (digression.Meter < 100 ? "  " : "")) + digression.Meter + "    " + digression.GetName() + "         " + digression.Value.ToString("0.00") + "      " + count.ToString("0.00")))

                                //    );
                                digElemenets.Add(new XElement("m",
                                                     new XAttribute("top", y),
                                                     new XAttribute("x", -10),
                                                     new XAttribute("note", digression.Meter),
                                                     new XAttribute("fw", "normal bold")
                                        ));
                                digElemenets.Add(new XElement("otst",
                                                     new XAttribute("top", y),
                                                     new XAttribute("x", 15),
                                                     new XAttribute("note", digression.GetName()),
                                                     new XAttribute("fw", "normal bold")
                                    ));
                                digElemenets.Add(new XElement("znach",
                                                     new XAttribute("top", y),
                                                     new XAttribute("x", 57),
                                                     new XAttribute("note", digression.Value.ToString("0.00")),
                                                     new XAttribute("fw", "normal bold")
                                    ));
                                digElemenets.Add(new XElement("dlina",
                                                     new XAttribute("top", y),
                                                     new XAttribute("x", 92),
                                                     new XAttribute("note", (digression.DigName == DigressionName.ImpulsLeft) || (digression.DigName == DigressionName.ImpulsRight) ? imp_count.ToString("0.00") : count.ToString("0.00")),
                                                     new XAttribute("fw", "normal bold")
                                    ));
                                if (!((digression.DigName == DigressionName.ImpulsLeft) || (digression.DigName == DigressionName.ImpulsRight)))
                                {
                                    digElemenets.Add(new XElement("line",
                                        new XAttribute("x", GetXByDigName(digression.DigName.Name)), 
                                        new XAttribute("w", MMToPixelChartWidthString(1.2f)), 
                                        new XAttribute("y1", digression.Meter + count), 
                                        new XAttribute("y2", digression.Meter)
                                        ));
                                }
                                else
                                {
                                    var impuls = digression.DigName == DigressionName.ImpulsLeft ? impulsLeft : impulsRight;
                                    var position = digression.DigName == DigressionName.ImpulsLeft ? ImpulsRoughnessLeftPosition : ImpulsRoughnessRightPosition;

                                    impuls.Add(new XElement("imp",
                                        new XAttribute("x2", MMToPixelChartString(position - (count * 15f))),
                                        new XAttribute("x4", MMToPixelChartString(position + 1.5f + (digression.Value * 2.0f))),
                                        new XAttribute("y", 1000-digression.Meter)
                                        ));
                                    addParam.Add(impuls);
                                }
                            }
                            //addParam.Add(
                            //    new XAttribute("ind", $"лев.: {Math.Round(indicators[0][6] * indicators[0][7] / 1000, 2)}  прав.: {Math.Round(indicators[1][6] * indicators[1][7] / 1000, 2)}")
                            //    );
                            addParam.Add(digElemenets);
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

//        private string GetXByDigName(DigressionName digName)
//        {
//            float move = 6.6f;
//            switch (digName)
//            {
//                case DigressionName.LongWaveLeft:
//                    return MMToPixelChartString(longWaveLeftPosition + move);
//                case DigressionName.LongWaveRight:
//                    return MMToPixelChartString(LongWaveRightPosition + move);
//                case DigressionName.MiddleWaveLeft:
//                    return MMToPixelChartString(MiddleWaveLeftPosition + move);
//                case DigressionName.MiddleWaveRight:
//                    return MMToPixelChartString(MiddleWaveRightPosition + move);
//                case DigressionName.ShortWaveLeft:
//                    return MMToPixelChartString(ShortWaveLeftPosition + move);
//                case DigressionName.ShortWaveRight:
//                    return MMToPixelChartString(ShortWaveRightPosition + move);
//            }
//            return "-100";
//        }
//    }
//}
        private string GetXByDigName(string digName)
        {
            float move = 6.6f;
            switch (digName)
            {
                case string name when name == DigressionName.LongWaveLeft.Name:
                    return MMToPixelChartString(longWaveLeftPosition + move);
                case string name when name == DigressionName.LongWaveRight.Name:
                    return MMToPixelChartString(LongWaveRightPosition + move);
                case string name when name == DigressionName.MiddleWaveLeft.Name:
                    return MMToPixelChartString(MiddleWaveLeftPosition + move);
                case string name when name == DigressionName.MiddleWaveRight.Name:
                    return MMToPixelChartString(MiddleWaveRightPosition + move);
                case string name when name == DigressionName.ShortWaveLeft.Name:
                    return MMToPixelChartString(ShortWaveLeftPosition + move);
                case string name when name == DigressionName.ShortWaveRight.Name:
                    return MMToPixelChartString(ShortWaveRightPosition + move);
            }
            return "-100";
        }
    }
}
