using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm.Core.Report
{
    public class MainParameters : GraphicDiagrams
    {
        private readonly float LevelPosition = 32.5f;
        private readonly float LevelStep = 7.5f;
        private readonly float LevelKoef = 0.25f;

        private readonly float StraighRighttPosition = 62f;
        private readonly float StrightLeftPosition = 71f;
        private readonly float StrightStep = 15f;
        private readonly float StrightKoef = 0.5f;

        private readonly float GaugePosition = 100.5f;
        private readonly float GaugeKoef = 0.5f;

        private readonly float ProsRightPosition = 138.5f;
        private readonly float ProsLeftPosition = 124.5f;
        private readonly float ProsKoef = 0.5f;

        public MainParameters(IRdStructureRepository rdStructureRepository, IMainTrackStructureRepository mainTrackStructureRepository, IAdmStructureRepository admStructureRepository)
        {
            this.RdStructureRepository = rdStructureRepository;
            this.MainTrackStructureRepository = mainTrackStructureRepository;
            this.AdmStructureRepository = admStructureRepository;
        }
        public void Process(ReportTemplate template, Kilometer kilometer, Trips trip, bool autoprint, Kilometer previous, Kilometer next)
        {
            XDocument htReport = new XDocument();
            var svgLength = 0;
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                int svgIndex = template.Xsl.IndexOf("</svg>");
                template.Xsl = template.Xsl.Insert(svgIndex, RighstSideXslt());

                var direction = AdmStructureRepository.GetDirectionByTrack(kilometer.Track_id);


                //ПРУ
                var PasSpeed = kilometer.Speeds.Any() ? kilometer.Speeds.First().Passenger : -1;
                var pru_dig_list = new List<DigressionMark> { };

                var curve_bpd_list = new List<DigressionMark> { };

                var Curve_nature_value = new List<DigressionMark>();
                int prevIndex = kilometer.Number + 1;
                //var rezulat = new List<> ;
                //var rezulat_next = new List<> ;
                //ezulat_next=
                var result = new List<Curve>();

                //if (previous != null)
                // result.AddRange(previous.Curve.s);

                result.AddRange(kilometer.Curves.GroupBy(p => p.Id).Select(g => g.First()).ToList());
                if (next != null)
                {
                    // result.AddRange(next.Curves); nb     
                    // var intersect = kilometer.Curves.Intersect(next.Curves);
                    //result.AddRange(intersect);
                }

                foreach (var bpd_curve in result)
                {
                    List<RDCurve> rdcs = RdStructureRepository.GetRDCurves(bpd_curve.Id, trip.Id);
                    //var LevelPoins = rdcs.Where(o => o.Point_level > 0).ToList();
                    //var StrPoins = rdcs.Where(o => o.Point_str > 0).ToList();
                    var nn = kilometer.Number;
                    var curve_center_ind = rdcs.Count / 2;
                    //басын аяғын тауып алу Рихтовка
                    RDCurve rightbound = rdcs[curve_center_ind], leftbound = rdcs[curve_center_ind];
                    for (int cInd = rdcs.Count-1; cInd > curve_center_ind; cInd--)
                    {
                        if (Math.Abs(rdcs[cInd].Trapez_str) < 0.1)
                            continue;
                        rightbound = rdcs[cInd];
                        break;
                    }
                    for (int cInd = 0; cInd < curve_center_ind; cInd++)
                    {
                        if (Math.Abs(rdcs[cInd].Trapez_str) < 0.1)
                            continue;
                        leftbound = rdcs[cInd];
                        break;
                    }

                    var strData = rdcs.Where(o => leftbound.X <= o.X && o.X <= rightbound.X).ToList();


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
                        if (strData[versh].FiList > 0.1 && flagPerehod)
                        {
                            perehod.Add(strData[versh]);
                        }
                        else if (strData[versh].FiList < 0.1)
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

                        if (strData[versh].FiList < 0.1 && flagKrug)
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
                    RDCurve rightboundlvl = rdcs[curve_center_ind], leftboundlvl = rdcs[curve_center_ind];
                    for (int cInd = rdcs.Count - 1; cInd > curve_center_ind; cInd--)
                    {
                        if (Math.Abs(rdcs[cInd].Trapez_level) < 0.1)
                            continue;
                        rightboundlvl = rdcs[cInd];
                        break;
                    }
                    for (int cInd = 0; cInd < curve_center_ind; cInd++)
                    {
                        if (Math.Abs(rdcs[cInd].Trapez_level) < 0.1)
                            continue;
                        leftboundlvl = rdcs[cInd];
                        break;
                    }
                    var LvlData = rdcs.Where(o => leftboundlvl.X <= o.X && o.X <= rightboundlvl.X).ToList();

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



                    bool flaglvl = false;
                    if (StrPoins.Count < 4 && LevelPoins.Count < 4)
                        continue;
                    if (StrPoins.Count < 4 && LevelPoins.Count >= 4)
                        flaglvl = true;


                    int lvl = -1, str = -1, lenPerKrivlv = -1;
                    int lenPru = -1;

                    //var vershinyLVL = vershListLVL.Where(o => Math.Abs(o.First().FiList2) < 1).ToList();
                    //var vershinyLVL = vershListLVL.Where(o => Math.Abs(o.First().FiList2) < 1).ToList();
                    // var vershinyLVL = vershListLVL.Where(o =>o.Add(Math.Abs((int)0.FiList2()))< 1).ToList();
                    var minH = new List<RDCurve>();
                    var minOb = 9000.0;

                    var maxAnp = new List<RDCurve>();
                    var max = -1.0;

                    //  foreach (var item in vershinyLVL)
                    var xx = 0;

                    List<List<RDCurve>> vershl = flaglvl ? vershListLVL : vershList;
                    for (int i=1; i < vershl.Count; i = i+2)
                    {
                        var item = vershl[i];  
                        var r = item.Select(o => o.Trapez_str).Max();
                        var h = item.Select(o => o.Trapez_level).Max();
                        var avgmeterbyItem = item.Select(o => o.M).Average();
                        xx = xx + 1;
                        // var metr= item.Select(o => o.Trapez_level).;
                        if (Math.Abs(h) < minOb)
                        {
                            minOb = Math.Abs(h);
                            minH = item;
                        }

                        //макс ыздеу анп га
                        var tempM = item.Select(o => o.PassBoost).Max();
                        if (tempM > max)
                        {
                            max = tempM;
                            maxAnp = item;
                        }

                        //if (kilometer.Number != rdcs[curve_center_ind].Km)
                        //    continue;

                        if (Math.Abs(h) < 4)
                            continue;

                        int curvestrindex = result.IndexOf(bpd_curve) < bpd_curve.Straightenings.Count ? result.IndexOf(bpd_curve) : 0;
                        var passportcurves = kilometer.CurvesBPD.Where(o => o.Id == bpd_curve.Id).ToList();
                        Curve curvepass = passportcurves.Count() > i / 2 ? passportcurves[i / 2] : passportcurves.First();
                        int startm = (int)curvepass.Start_M + (int)curvepass.Elevations[0].Transition_1;

                        pru_dig_list.Add(new DigressionMark()
                        {
                            Km = item.First().Km,
                            Meter = (item.First().Km == item.Last().Km) ? (int)avgmeterbyItem : startm,
                            Alert = $"кривая факт. R:{ (17860 / Math.Abs(r)):0} H:{ Math.Abs(h):0}"
                        });


                        var curvelistitem = new DigressionMark()
                        {
                            Km = item.First().Km,
                            lvl = (int)bpd_curve.Elevations[0].Lvl,
                            Radius = bpd_curve.Radius,
                            Meter = (item.First().Km == item.Last().Km) ? (int)avgmeterbyItem - 30 : startm- 30,
                            Alert = $" { startm } R:{bpd_curve.Radius} h:{bpd_curve.Elevations[0].Lvl} Ш:{bpd_curve.Straightenings[curvestrindex].Width} И:{bpd_curve.Straightenings[curvestrindex].Wear} "
                        };

                        if (!curve_bpd_list.Contains(curvelistitem))
                        {
                            curve_bpd_list.Add(curvelistitem);
                        }


                        //bool pr1 = false;
                        //bool pr2 = false;
                        //int prMeter = 0;
                        //int prlvl = 0;
                        //int prrad = 0;

                        //var prr1 = false;

                        //var prr2 = false;
                        //var newrad = false;
                        //if (curve_bpd_list[curve_bpd_list.Count - 1].Meter != (int)avgmeterbyItem - 30) prr1 = true;
                        //if (curve_bpd_list[0].Meter != (int)avgmeterbyItem - 30) prr2 = true;
                        //  (curve_bpd_list[0].lvl != (int)bpd_curve.Elevations[0].Lvl)||
                     
                        //if (((prr1 && prr2)) && curve_bpd_list.Any())
                        //{
                        //    curve_bpd_list.Add(new DigressionMark()
                        //    {
                        //        lvl = (int)bpd_curve.Elevations[0].Lvl,
                        //        Km = item.First().Km,
                        //        Radius = bpd_curve.Radius,
                        //        Meter = (int)curvepass.Start_M + (int)curvepass.Elevations[0].Transition_1,
                        //        Alert = $" {(int)curvepass.Start_M + (int)curvepass.Elevations[0].Transition_1 } R:{bpd_curve.Radius} h:{bpd_curve.Elevations[0].Lvl} Ш:{bpd_curve.Straightenings[curvestrindex].Width} И:{bpd_curve.Straightenings[curvestrindex].Wear} "
                        //    });
                        //}

                        lvl = (int)h;

                        //------------------------------------------------------------------------------------------
                        //----ПРУ-----------------------------------------------------------------------------------
                        //------------------------------------------------------------------------------------------
                        if (kilometer.Number == rdcs[curve_center_ind].Km)
                        {
                            var realCurveCenter = item[item.Count / 2].Km.ToDoubleCoordinate((int)avgmeterbyItem);
                            //var realCurveCenter = item[item.Count / 2].Km.ToDoubleCoordinate(item[item.Count / 2].M);
                            // var pmeter = item[item.Count / 2].M;
                            var pmeter = (int)avgmeterbyItem;
                            pmeter = kilometer.LevelAvgTrapezoid.Count - 1 < pmeter ? kilometer.LevelAvgTrapezoid.Count - 1 : pmeter;

                            var passportLvlData = bpd_curve.Elevations.Where(o => realCurveCenter.Between(o.RealStartCoordinate, o.RealFinalCoordinate)).ToList();

                            var pLvl = passportLvlData.Any() ? passportLvlData.First().Lvl : lvl;

                            var diff = Math.Abs(Math.Abs(lvl) - (int)Math.Abs(pLvl));
                            var rdcsData = new Data { };
                            var Vkr = RoundNumToFive(rdcsData.GetKRSpeedPass(rdcs));
                            var Ogr = -1;
                            if (kilometer.Speeds.First().Passenger > Vkr)
                                Ogr = Vkr;
                            //балл
                            var ball = -1;
                            var razn = -1;
                            if (140 < PasSpeed && 10 < diff)
                            {
                                ball = 50;
                                razn = diff - 10;
                            }
                            else if ((101 <= PasSpeed && PasSpeed <= 140) && 15 < diff)
                            {
                                ball = 50;
                                razn = diff - 15;
                            }
                            else if ((61 <= PasSpeed && PasSpeed <= 100) && 20 < diff)
                            {
                                ball = 50;
                                razn = diff - 20;
                            }
                            else if (PasSpeed <= 60 && 25 < diff)
                            {
                                ball = 50;
                                razn = diff - 25;
                            }

                            if (ball != -1)
                            {
                                try
                                {
                                    pru_dig_list.Add(new DigressionMark
                                    {
                                        Km = kilometer.Number,
                                        Meter = (int)avgmeterbyItem,
                                        Value = diff, //высота
                                        Length = item.Count, // длина круговой
                                        Count = ball,
                                        DigName = DigressionName.Pru.Name,
                                        Pch = kilometer.PdbSection[0].Distance,
                                        DirectionName = direction.Name,
                                        TrackName = kilometer.Track_name,
                                        PassengerSpeedAllow = kilometer.Speeds.First().Passenger,
                                        PassengerSpeedLimit = kilometer.Speeds.First().Passenger > Ogr ? Ogr : -1,

                                        FreightSpeedAllow = kilometer.Speeds.First().Freight,
                                        FreightSpeedLimit = kilometer.Speeds.First().Freight > Ogr ? Ogr : -1



                                    });
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"ПрУ write s3 error {e.Message}");
                                }
                            }


                            //if (lvl != -1)
                            //{
                            //    //Curve_nature_value.Add(new DigressionMark()
                            //    pru_dig_list.Add(new DigressionMark()
                            //    {
                            //        Km = -999,
                            //        Meter = first_pmeter,
                            //        Alert = $"{first_pmeter} Крив. факт R:{ str } h:{ lvl }"
                            //    });
                            //}

                            //else
                            //{
                            //    //пустая если ошибка при вычислений
                            //    ////Curve_nature_value.Add(new DigressionMark()
                            //    pru_dig_list.Add(new DigressionMark()
                            //    {
                            //        Km = -999,
                            //        Meter = first_pmeter,
                            //        Alert = $"{first_pmeter} Крив. факт R: h:"
                            //    });
                            //}


                        }
                        //---------------
                        //---------------
                        //---------------

                    }

                    lenPru = minH.Count;
                    lvl = (int)minOb;

                    if (StrPoins.Any() && LevelPoins.Any())
                    {
                        try
                        {
                            //Поиск круговой кривой рихтовкие
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

                            if (Math.Abs(Math.Abs(StrPoins[StrPoins.Count - 1].Trapez_str) - Math.Abs(StrPoins[StrPoins.Count - 2].Trapez_str)) / Math.Abs(StrPoins[StrPoins.Count - 1].X - StrPoins[StrPoins.Count - 2].X) < 0.05)
                            {
                                str_circular.Add(StrPoins[StrPoins.Count - 1]);
                            }
                            //Поиск круговой кривой уровень
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

                            //надо проверить на ошибки
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
                            lenPerKrivlv = Math.Abs((int)lenPerKriv10000lv % 1000);

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

                            Data rdcsData = new Data();

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
                            str = rad_mid;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Ошибка при расчете натурной кривой" + e);
                        }
                    }

                    var curve_center = (bpd_curve.Start_Km * 1000 + bpd_curve.Start_M) + ((bpd_curve.Final_Km * 1000 + bpd_curve.Final_M) - (bpd_curve.Start_Km * 1000 + bpd_curve.Start_M)) / 2;
                    var pkm = curve_center / 1000;
                    var first_pmeter = curve_center % 1000;

                    int curve_krug = -1;
                    if (lenPerKrivlv != -1)
                    {
                        curve_krug = lenPerKrivlv;
                    }
                    else
                        curve_krug = Math.Abs((bpd_curve.Elevations.First().Start_Km * 1000 + bpd_curve.Elevations.First().Start_M) -
                                                   (bpd_curve.Elevations.First().Final_Km * 1000 + bpd_curve.Elevations.First().Final_M)) -
                                                   (bpd_curve.Elevations.First().Transition_1 + bpd_curve.Elevations.First().Transition_2);

                    //АНП
                    if (rdcs.Any())
                    {
                        try
                        {
                            //Passenger
                            var PassBoostAbs = rdcs.Select(o => Math.Abs(o.PassBoost_anp)).ToList();
                            var PassboostMax = PassBoostAbs.Max();
                            var MaxPassboostIndex = PassBoostAbs.IndexOf(PassboostMax);
                            var AnpPassMax = PassboostMax * Math.Sign(rdcs[MaxPassboostIndex].PassBoost_anp);
                            //Freight
                            var FreightBoostAbs = rdcs.Select(o => Math.Abs(o.FreightBoost_anp)).ToList();
                            var FreightboostMax = FreightBoostAbs.Max();
                            var MaxFreightboostIndex = FreightBoostAbs.IndexOf(FreightboostMax);
                            var AnpFreigMax = FreightboostMax * Math.Sign(rdcs[MaxFreightboostIndex].FreightBoost_anp);


                            //var AnpPassMax = rdcs.Select(o => o.PassBoost_anp).Max();
                            //var AnpFreigMax = rdcs.Select(o => o.FreightBoost_anp).Max();

                            var rdcsData = new Data { };
                            var Vkr = RoundNumToFive(rdcsData.GetKRSpeedPass(rdcs));
                            var Ogr = -1;
                            if (kilometer.Speeds.First().Passenger > Vkr)
                                Ogr = Vkr;


                            var Dname = "";

                            //if (AnpPassMax > 0.70)
                            //{
                            //    Dname = DigressionName.SpeedUp.Name;
                            //    Ogr = RoundNumToFive(Ogr);
                            //}
                            //else if (0.65 <= AnpPassMax && AnpPassMax <= 0.70)

                            if (AnpPassMax > 0.70 && kilometer.Number == pkm)
                            {
                                Dname = DigressionName.SpeedUp.Name;
                                Ogr = RoundNumToFive(Ogr);
                            }
                            else if (0.65 <= AnpPassMax && AnpPassMax <= 0.70 && kilometer.Number == pkm)
                            {
                                Dname = DigressionName.SpeedUpNear.Name;
                                Ogr = -1;
                            }

                            if (Dname != "")
                            {
                                pru_dig_list.Add(new DigressionMark
                                {
                                    Km = kilometer.Number,
                                    Meter = maxAnp[maxAnp.Count / 2].M,

                                    Length = maxAnp.Count, // длина круговой

                                    DigName = Dname,

                                    //Comment = $"П:{AnpPassMax:0.00}      Г:{AnpFreigMax:0.00}",

                                    Comment = $"{AnpPassMax:0.00}",

                                    PassengerSpeedAllow = kilometer.Speeds.First().Passenger,
                                    PassengerSpeedLimit = kilometer.Speeds.First().Passenger > Ogr ? Ogr : -1,

                                    FreightSpeedAllow = kilometer.Speeds.First().Freight,
                                    FreightSpeedLimit = kilometer.Speeds.First().Freight > Ogr ? Ogr : -1,

                                    Pch = kilometer.PdbSection[0].Distance,
                                    DirectionName = direction.Name,
                                    TrackName = kilometer.Track_name
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"АНП write s3 error {e.Message}");
                        }
                    }

                    if (pkm == kilometer.Number)
                    {
                        var pmeter = kilometer.Length - first_pmeter;
                        pmeter = kilometer.LevelAvgTrapezoid.Count - 1 < pmeter ? kilometer.LevelAvgTrapezoid.Count - 1 : pmeter;


                        var diff = Math.Abs(Math.Abs(lvl) - (int)Math.Abs(bpd_curve.Elevations.First().Lvl));

                        //балл
                        var ball = -1;
                        var razn = -1;
                        if (140 < PasSpeed && 10 < diff)
                        {
                            ball = 50;
                            razn = diff - 10;
                        }
                        else if ((101 <= PasSpeed && PasSpeed <= 140) && 15 < diff)
                        {
                            ball = 50;
                            razn = diff - 15;
                        }
                        else if ((61 <= PasSpeed && PasSpeed <= 100) && 20 < diff)
                        {
                            ball = 50;
                            razn = diff - 20;
                        }
                        else if (PasSpeed <= 60 && 25 < diff)
                        {
                            ball = 50;
                            razn = diff - 25;
                        }
                    }
                }

                if (pru_dig_list.Any())
                    MainTrackStructureRepository.Pru_write(kilometer.Track_id, kilometer, pru_dig_list);

                if (curve_bpd_list.Any())
                    MainTrackStructureRepository.Bpd_write(kilometer.Track_id, kilometer, curve_bpd_list);

                // добавление ПрУ и натурные значения кривой
                kilometer.Digressions = Curve_nature_value;

                //контрольные участки КУ МО СКО
                var ku = new List<DigressionMark> { };
                try
                {
                    var MtoCheckSection = MainTrackStructureRepository.GetMtoObjectsByCoord(
                        trip.Trip_date,
                        kilometer.Number,
                        MainTrackStructureConst.MtoCheckSection,
                        trip.Direction_Name,
                        kilometer.Track_name) as List<CheckSection>;

                    foreach (var sect in MtoCheckSection)
                    {
                        var CheckSection_center = (sect.Start_Km * 1000 + sect.Start_M) + ((sect.Final_Km * 1000 + sect.Final_M) - (sect.Start_Km * 1000 + sect.Start_M)) / 2;
                        var pkm = CheckSection_center / 1000;
                        var first_pmeter = CheckSection_center % 1000;

                        if (pkm == kilometer.Number)
                        {
                            var CheckVerifyKm = RdStructureRepository.CheckVerify(
                                                            kilometer.Trip.Id,
                                                            sect.Start_Km * 1000 + sect.Start_M,
                                                            sect.Final_Km * 1000 + sect.Final_M);
                            if (CheckVerifyKm.Any())
                            {
                                //curve
                                var curve_msg = $"КУ: параметр уровень в норме_" +
                                                $"(МО: {sect.Avg_level:0.0}/{CheckVerifyKm.First().Trip_mo_level:0.0})";
                                var diff_curve = Math.Abs(sect.Avg_level - CheckVerifyKm.First().Trip_mo_level);
                                if (diff_curve >= 2)
                                {
                                    curve_msg = $"КУ: превыш. допуска смещения_" +
                                          $"({diff_curve:0.0}) пар. уровень (МО: {sect.Avg_level:0.0}/{CheckVerifyKm.First().Trip_mo_level:0.0})!";
                                }
                                //gauge
                                var gauge_msg = $"КУ: параметр шаблон в норме_" +
                                                $"(МО: {sect.Avg_width:0.0}/{CheckVerifyKm.First().Trip_mo_gauge:0.0})";
                                var diff_gauge = Math.Abs(sect.Avg_width - CheckVerifyKm.First().Trip_mo_gauge);
                                if (diff_gauge > 2)
                                {
                                    gauge_msg = $"КУ: превыш. допуска смещения_" +
                                          $"({diff_gauge:0.0}) пар. шаблон (МО: {sect.Avg_width:0.0}/{CheckVerifyKm.First().Trip_mo_gauge:0.0})!";
                                }
                                ku.Add(new DigressionMark()
                                {
                                    Meter = first_pmeter,
                                    Alert = $"{curve_msg}"
                                });

                                ku.Add(new DigressionMark()
                                {
                                    Meter = first_pmeter,
                                    Alert = $"{gauge_msg}"
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("контрольные участки КУ МО СКО error " + e.Message);
                }

                kilometer.Digressions.AddRange(ku);
                kilometer.LoadDigresions(RdStructureRepository, MainTrackStructureRepository, trip);


                //foreach (var item in kilometer.Digressions)
                //{
                //    if (item.Digression.Name == DigressionName.DrawdownLeft.Name || item.Digression.Name == DigressionName.DownhillRight.Name)
                //    {
                //        foreach (var IsoJoints in kilometer.IsoJoints)
                //        {
                //            if (IsoJoints.Meter.Between(item.Meter - item.Length / 2, item.Meter + item.Length / 2) && item.Comment == "")
                //                item.Comment = "ис";
                //        }
                //    }
                //}

                string drawdownRight = string.Empty, drawdownLeft = string.Empty, gauge = string.Empty, zeroGauge = string.Empty,
                        zeroStraightening = string.Empty, averageStraighteningRight = string.Empty, straighteningRight = string.Empty,
                        zeroStraighteningLeft = string.Empty, averageStraighteningLeft = string.Empty, straighteningLeft = string.Empty,
                        level = string.Empty, averageLevel = string.Empty, zeroLevel = string.Empty, avglevel = string.Empty;

                int fourStepOgrCoun = 0, otherfourStepOgrCoun = 0;

                var trackclasses = (List<TrackClass>)MainTrackStructureRepository.GetMtoObjectsByCoord(trip.Trip_date, kilometer.Number, MainTrackStructureConst.MtoTrackClass, kilometer.Track_id);

                svgLength = kilometer.GetLength() < 1000 ? 1000 : kilometer.GetLength();
                var xp = (-kilometer.Start_m - svgLength - 50) + (svgLength + 105) - 52;
                var runnin = "";
                if (kilometer.Runninin.Count > 0)
                {
                    if (kilometer.Runninin[0].Start_Km == kilometer.Number)
                    {
                        runnin = kilometer.Runninin[0].Reason + "пк" + (((kilometer.Runninin[0].Start_M + 1) / 100) + 1) + "-" + (((kilometer.Runninin[0].Final_M + 1) / 100) + 1) + "V:" + kilometer.Runninin[0].Passenger;
                    }

                }
              
              
              
                XElement addParam = new XElement("addparam",

                    new XAttribute("top-title", (runnin!= "" ? runnin : "")+
                           (direction != null ? $"{direction.Name} ({direction.Code})" : "Неизвестный") + " Путь: " +
                        kilometer.Track_name + $" Класс: {(trackclasses.Any() ? trackclasses.First().Class_Id.ToString() : "-")} Км:" + kilometer.Number + " " +
                        (kilometer.PdbSection.Count > 0 ? $" ПЧ-{kilometer.PdbSection[0].Distance}" : " ПЧ-") + " Уст: " + " " +
                        (kilometer.Speeds.Count > 0 ? $"{kilometer.Speeds.First().Passenger}/{kilometer.Speeds.First().Freight}" : "-/-"))
                    {

                    },
                    //new XAttribute("top-title",
                    //    (direction != null ? $" {(kilometer.Runninin[0].Start_Km == kilometer.Number ? kilometer.Runninin[0].Reason + "V:" + kilometer.Runninin[0].Passenger + "Км:" + kilometer.Runninin[0].Start_Km + "ПК:" + (((kilometer.Runninin[0].Start_M + 1) / 100) + 1) : "")} {direction.Name} ({direction.Code})" : "Неизвестный") + " Путь: " +
                    // kilometer.Track_name + $" Класс: {(trackclasses.Any() ? trackclasses.First().Class_Id.ToString() : "-")} Км:" + kilometer.Number + " " +
                    // (kilometer.PdbSection.Count > 0 ? $" ПЧ-{kilometer.PdbSection[0].Distance}" : " ПЧ-") + " Уст: " + " " +
                    // (kilometer.Speeds.Count > 0 ? $"{kilometer.Speeds.First().Passenger}/{kilometer.Speeds.First().Freight}" : "-/-")),


                    new XAttribute("right-title",
                        copyright + ": " + "ПО " + softVersion + "  " +
                        systemName + ":" + trip.Car + "(" + trip.Chief.Trim() + ") (БПД от " + MainTrackStructureRepository.GetModificationDate() + ") <" + (kilometer.PdbSection.Count > 0 ? kilometer.PdbSection[0].RoadAbbr : "НЕИЗВ") + ">" + "<" + kilometer.Passage_time.ToString("dd.MM.yyyy  HH:mm") + ">" +
                        "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(trip.Travel_Direction.ToString())) + ">" +
                        "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(trip.Car_Position.ToString())) + ">" +
                        "<" + trip.Trip_date.Month + "-" + trip.Trip_date.Year + " " + (trip.Trip_Type == TripType.Control ? "контр." : trip.Trip_Type == TripType.Work ? "раб." : "доп.") + " Проезд:" + trip.Trip_date.ToString("dd.MM.yyyy  HH:mm") + " " + diagramName + ">"
                        ),
                    new XAttribute("pre", xp + 30),
                    new XAttribute("prer", xp + 21),
                    new XAttribute("topr", -kilometer.Start_m - svgLength - 45),
                    new XAttribute("topf", xp + 10),
                    new XAttribute("topx", -kilometer.Start_m - svgLength),
                    new XAttribute("topx1", -kilometer.Start_m - svgLength - 30),
                    new XAttribute("topx2", -kilometer.Start_m - svgLength - 15),
                    new XAttribute("fragment", (kilometer.StationSection != null && kilometer.StationSection.Count > 0 ? "Станция: " + kilometer.StationSection[0].Station : (kilometer.Sector != null ? kilometer.Sector.ToString() : "")) + " Км:" + kilometer.Number),
                    new XAttribute("viewbox", $"-20 {-kilometer.Start_m - svgLength - 50} 830 {svgLength + 105}"),
                    new XAttribute("minY", -kilometer.Start_m),
                    new XAttribute("maxY", -kilometer.Final_m),
                    new XAttribute("minYround", -(kilometer.Start_m - kilometer.Start_m % 100)),

                    RightSideChart(trip.Trip_date, kilometer, kilometer.Track_id, new float[] { 151f, 146f, 152.5f, 155f }),

                    new XElement("xgrid",
                        new XElement("x", MMToPixelChartString(LevelPosition - LevelStep),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "  –30"),
                        new XAttribute("y", MMToPixelChartString(LevelPosition - LevelStep - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(LevelPosition),
                        new XAttribute("dasharray", "3,3"),
                        new XAttribute("stroke", "black"),
                        new XAttribute("label", "      0"),
                        new XAttribute("y", MMToPixelChartString(LevelPosition - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(LevelPosition + LevelStep),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "    30"),
                        new XAttribute("y", MMToPixelChartString(LevelPosition + LevelStep - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(StraighRighttPosition - StrightStep),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "  –30"),
                        new XAttribute("y", MMToPixelChartString(StraighRighttPosition - StrightStep - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(StraighRighttPosition),
                        new XAttribute("dasharray", "3,3"),
                        new XAttribute("stroke", "black"),
                        new XAttribute("label", "      0"),
                        new XAttribute("y", MMToPixelChartString(StraighRighttPosition - 1f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(StraighRighttPosition + StrightStep / 10f),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "      3"),
                        new XAttribute("y", MMToPixelChartString(StraighRighttPosition + StrightStep / 10f + 0.2f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(StrightLeftPosition - StrightStep / 10f),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "    –3"),
                        new XAttribute("y", MMToPixelChartString(StrightLeftPosition - StrightStep / 10f - 1f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(StrightLeftPosition),
                        new XAttribute("dasharray", "3,3"),
                        new XAttribute("stroke", "black"),
                        new XAttribute("label", "      0"),
                        new XAttribute("y", MMToPixelChartString(StrightLeftPosition + 0.2f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(StrightLeftPosition + StrightStep),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "    30"),
                        new XAttribute("y", MMToPixelChartString(StrightLeftPosition + StrightStep - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(GaugePosition - 10 * GaugeKoef),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey")),

                        new XElement("x", MMToPixelChartString(GaugePosition - 8 * GaugeKoef),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "1512"),
                        new XAttribute("y", MMToPixelChartString(GaugePosition - 8 * GaugeKoef - 0.5f)),

                        new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(GaugePosition - 4 * GaugeKoef),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey")),

                        new XElement("x", MMToPixelChartString(GaugePosition),
                        new XAttribute("dasharray", "3,3"),
                        new XAttribute("stroke", "black"),
                        new XAttribute("label", "1520"),
                        new XAttribute("y", MMToPixelChartString(GaugePosition - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(GaugePosition + 8 * GaugeKoef),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "1528"),
                        new XAttribute("y", MMToPixelChartString(GaugePosition + 8 * GaugeKoef - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(GaugePosition + 16 * GaugeKoef),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "1536"),
                        new XAttribute("y", MMToPixelChartString(GaugePosition + 16 * GaugeKoef - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(GaugePosition + 22 * GaugeKoef),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "1542"),
                        new XAttribute("y", MMToPixelChartString(GaugePosition + 22 * GaugeKoef - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(GaugePosition + 26 * GaugeKoef),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey")),

                        new XElement("x", MMToPixelChartString(GaugePosition + 28 * GaugeKoef),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "1548"),
                        new XAttribute("y", MMToPixelChartString(GaugePosition + 28 * GaugeKoef - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(ProsRightPosition - 10 * ProsKoef),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "  –10"),
                        new XAttribute("y", MMToPixelChartString(ProsRightPosition - 10 * ProsKoef - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(ProsRightPosition),
                        new XAttribute("dasharray", "3,3"),
                        new XAttribute("stroke", "black"),
                        new XAttribute("label", "      0"),
                        new XAttribute("y", MMToPixelChartString(ProsRightPosition - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(ProsRightPosition + 10 * ProsKoef),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "    10"),
                        new XAttribute("y", MMToPixelChartString(ProsRightPosition + 10 * ProsKoef - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(ProsLeftPosition - 10 * ProsKoef),
                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "  –10"),
                        new XAttribute("y", MMToPixelChartString(ProsLeftPosition - 10 * ProsKoef - 0.5f)),
                        new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(ProsLeftPosition),
                        new XAttribute("dasharray", "3,3"),
                        new XAttribute("stroke", "black"),
                        new XAttribute("label", "      0"),
                        new XAttribute("y", MMToPixelChartString(ProsLeftPosition - 0.5f)),
                        new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(ProsLeftPosition + 10 * ProsKoef),

                        new XAttribute("dasharray", "0.5,2"),
                        new XAttribute("stroke", "grey"),
                        new XAttribute("label", "    10"),
                        new XAttribute("y", MMToPixelChartString(ProsLeftPosition + 10 * ProsKoef - 0.5f)),
                        new XAttribute("x", xp + 15))
                        ));

                //kil
                for (int index = 0; index < kilometer.meter.Count - 1; index++)
                {
                    try
                    {
                        int metre = -kilometer.meter[index];
                        drawdownRight += MMToPixelChartString(kilometer.DrawdownLeft[index] * ProsKoef + ProsRightPosition) + "," + metre + " ";
                        drawdownLeft += MMToPixelChartString(kilometer.DrawdownRight[index] * ProsKoef + ProsLeftPosition) + "," + metre + " ";
                        gauge += MMToPixelChartString((kilometer.Gauge[index] - 1520) * GaugeKoef + GaugePosition) + "," + metre + " ";
                        zeroGauge += MMToPixelChartString((kilometer.fsh0[index] - 1520) * GaugeKoef + GaugePosition) + "," + metre + " ";

                        //пасспорт рихт
                        zeroStraightening += MMToPixelChartString(kilometer.fZeroStright[index] * StrightKoef + StraighRighttPosition) + "," + metre + " ";
                        zeroStraighteningLeft += MMToPixelChartString(kilometer.fZeroStright[index] * StrightKoef + StrightLeftPosition) + "," + metre + " ";
                        //Паспорт уровень
                        zeroLevel += MMToPixelChartString(kilometer.flvl0[index] * LevelKoef + LevelPosition) + "," + metre + " ";

                        //ср линия рихт
                        averageStraighteningRight += MMToPixelChartString(kilometer.StrightAvgTrapezoid[index] * StrightKoef + StraighRighttPosition) + "," + metre + " ";
                        averageStraighteningLeft += MMToPixelChartString(kilometer.StrightAvgTrapezoid[index] * StrightKoef + StrightLeftPosition) + "," + metre + " ";

                        //трапеция рихт
                        var drh = kilometer.StrightAvgTrapezoid[index] + (kilometer.StrightRight[index] - kilometer.StrightAvgTrapezoid[index]);
                        straighteningRight += MMToPixelChartString(drh * StrightKoef + StraighRighttPosition) + "," + metre + " ";

                        var drh1 = kilometer.StrightAvgTrapezoid[index] + (kilometer.StrightLeft[index] - kilometer.StrightAvgTrapezoid[index]);
                        straighteningLeft += MMToPixelChartString(drh1 * StrightKoef + StrightLeftPosition) + "," + metre + " ";



                        level += MMToPixelChartString(kilometer.Level[index] * LevelKoef + LevelPosition) + "," + metre + " ";
                        averageLevel += MMToPixelChartString(kilometer.LevelAvgTrapezoid[index] * LevelKoef + LevelPosition) + "," + metre + " ";
                       
                        avglevel += MMToPixelChartString(kilometer.LevelAvg[index] * LevelKoef + LevelPosition) + "," + metre + " ";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Рисование линий А4 ошибка " + e.Message);
                    }
                }

                var style = "fill:none;stroke:dimgray;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3";
                var styleAverage = "fill:none;stroke:dimgray;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.7; stroke-dasharray:0.7 0.6;";
                var styleAvg = "fill:none;stroke:dimgray;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.5; stroke-dasharray:4 2;stroke:green";

                addParam.Add(new XElement("polyline", new XAttribute("points", drawdownRight), new XAttribute("style", style)));
                addParam.Add(new XElement("polyline", new XAttribute("points", drawdownLeft), new XAttribute("style", style)));
                addParam.Add(new XElement("polyline", new XAttribute("points", gauge), new XAttribute("style", style)));
                addParam.Add(new XElement("polyline", new XAttribute("points", zeroGauge), new XAttribute("style", style)));

                addParam.Add(new XElement("polyline", new XAttribute("points", zeroStraightening), new XAttribute("style", style)));
                addParam.Add(new XElement("polyline", new XAttribute("points", averageStraighteningRight), new XAttribute("style", styleAverage)));
                addParam.Add(new XElement("polyline", new XAttribute("points", straighteningRight), new XAttribute("style", style)));

                addParam.Add(new XElement("polyline", new XAttribute("points", zeroStraighteningLeft), new XAttribute("style", style)));
                addParam.Add(new XElement("polyline", new XAttribute("points", averageStraighteningLeft), new XAttribute("style", styleAverage)));
                addParam.Add(new XElement("polyline", new XAttribute("points", straighteningLeft), new XAttribute("style", style)));

                addParam.Add(new XElement("polyline", new XAttribute("points", level), new XAttribute("style", style)));
                addParam.Add(new XElement("polyline", new XAttribute("points", averageLevel), new XAttribute("style", styleAverage)));
                addParam.Add(new XElement("polyline", new XAttribute("points", avglevel), new XAttribute("style", styleAvg)));
                addParam.Add(new XElement("polyline", new XAttribute("points", zeroLevel), new XAttribute("style", style)));


                char separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];

                var digElemenets = new XElement("digressions");
                List<int> usedTops = new List<int>();
                List<int> speedmetres = new List<int>();

                var gmeter = kilometer.Start_m.RoundTo10() + 10;


                foreach (var picket in kilometer.Pickets)
                {
                    picket.WriteNotesToReport(
                        kilometer,
                        speedmetres,
                        addParam,
                        digElemenets,
                        ProsRightPosition,
                        ProsLeftPosition,
                        StraighRighttPosition,
                        StrightLeftPosition,
                        GaugePosition,
                        LevelPosition,
                        this,
                        ref fourStepOgrCoun,
                        ref otherfourStepOgrCoun);
                }

                //addParam.Add(new XAttribute("common", kilometer.GetdigressionsCount));

                var GetBedomost = new List<Bedemost> { };
                try
                {
                    // 1степень санау
                    GetBedomost = ((List<Bedemost>)RdStructureRepository.GetBedemost(kilometer.Trip.Id)).Where(o => o.Km == kilometer.Number).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetBedomost " + e.Message);
                }
                addParam.Add(
                    new XAttribute("speedlimit",
                    kilometer.CalcMainPoint() + " " + $"Кол.ст.- 1:{(GetBedomost.Any() ? GetBedomost.First().Type1 : 0)}; " + kilometer.GetdigressionsCount +
                    "  Кол.огр." + fourStepOgrCoun + "/" + otherfourStepOgrCoun + " Огр. " + kilometer.SpeedLimit + $" Скор.{(int)kilometer.Speed.Average()}")
                    );

                addParam.Add(digElemenets);
                report.Add(addParam);

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);

            }
            try
            {
                htReport.Save($@"g:\form\{kilometer.Number}_{kilometer.Track_id}.html");

                //htReport.Save($@"\\192.168.1.200\form\{kilometer.Number}_{kilometer.Track_id}.html");
                var svg = htReport.Element("html").Element("body").Element("div").Element("div").Element("svg");
                var svgDoc = SvgDocument.FromSvg<SvgDocument>(svg.ToString());
                svgDoc.Width = 800 * 3;
                svgDoc.Height = (svgLength + 95) * 3;

                if (autoprint)
                {

                    PrintDocument pd = new PrintDocument();
                    //pd.PrinterSettings.PrinterName = "EPSON5EEE22";
                    Console.WriteLine(pd.PrinterSettings.PrinterName);

                    pd.PrintPage += (sender, args) =>
                    {
                        var i = svgDoc.Draw();
                        //i.Save($@"g:\form\{kilometer.Number}_{kilometer.Track_id}.png");
                        htReport.Save($@"g:\form\{kilometer.Number}_{kilometer.Track_id}.html");
                        pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                        pd.OriginAtMargins = true;
                        pd.DefaultPageSettings.Landscape = true;
                        Point p = new Point(0, 0);
                        args.Graphics.DrawImage(i, 0, 0, i.Width / 3, i.Height / 3 - 15);
                    };
                    pd.Print();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Ошибка при сохранении отчетной формы: " + e.Message);
            }
            finally
            {
                // System.Diagnostics.Process.Start(Path.GetTempPath() + "/report.html");
            }
        }

        private void pd_print(object sender, PrintPageEventArgs e)
        {
            Graphics gr = e.Graphics;
            gr.DrawString("Sales", new Font(FontFamily.GenericMonospace, 12, FontStyle.Bold), new SolidBrush(Color.Black), new Point(40, 40));
        }
        public void ProcessRegion(ReportTemplate template, Kilometer kilometer, Trips trip, bool autoprint, int currentKmMeter)
        {
            XDocument htReport = new XDocument();
            var svgLength = 0;
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                int svgIndex = template.Xsl.IndexOf("</svg>");
                template.Xsl = template.Xsl.Insert(svgIndex, RighstSideXslt());

                //ПРУ
                var PasSpeed = kilometer.Speeds.Any() ? kilometer.Speeds.First().Passenger : -1;
                var pru_dig_list = new List<DigressionMark> { };
                var curve_bpd_list = new List<DigressionMark> { };
                var Curve_nature_value = new List<DigressionMark>();

                foreach (var bpd_curve in kilometer.Curves)
                {
                    List<RDCurve> rdcs = RdStructureRepository.GetRDCurves(bpd_curve.Id, trip.Id);
                    var LevelPoins = rdcs.Where(o => o.Point_level > 0).ToList();
                    var StrPoins = rdcs.Where(o => o.Point_str > 0).ToList();

                    int lvl = -1, str = -1, lenPerKrivlv = -1;

                    try
                    {
                        var lenPerKriv10000 = ((StrPoins[1].Km + StrPoins[1].M / 10000.0) - (StrPoins[2].Km + StrPoins[2].M / 10000.0)) * 10000;
                        var lenPerKriv = Math.Abs((int)lenPerKriv10000 % 1000);

                        var lenKriv10000 = ((StrPoins[0].Km + StrPoins[0].M / 10000.0) - (StrPoins[3].Km + StrPoins[3].M / 10000.0)) * 10000;
                        var lenKriv = Math.Abs((int)lenKriv10000 % 1000);

                        var lenPerKriv10000lv = ((LevelPoins[1].Km + LevelPoins[1].M / 10000.0) - (LevelPoins[2].Km + LevelPoins[2].M / 10000.0)) * 10000;
                        lenPerKrivlv = Math.Abs((int)lenPerKriv10000lv % 1000);

                        var lenKriv10000lv = ((LevelPoins[0].Km + LevelPoins[0].M / 10000.0) - (LevelPoins[3].Km + LevelPoins[3].M / 10000.0)) * 10000;
                        var lenKrivlv = Math.Abs((int)lenKriv10000lv % 1000);

                        var d = false;
                        if ((StrPoins[0].Km + StrPoins[0].M / 10000.0) > (StrPoins[3].Km + StrPoins[3].M / 10000.0))
                            d = true;

                        //нижние 2 точки трапеции
                        var start_km = d ? StrPoins.Last().Km : StrPoins.First().Km;
                        var start_m = d ? StrPoins.Last().M : StrPoins.First().M;
                        var final_km = d ? StrPoins.First().Km : StrPoins.Last().Km;
                        var final_m = d ? StrPoins.First().M : StrPoins.Last().M;

                        var start_lvl_km = d ? LevelPoins.Last().Km : LevelPoins.First().Km;
                        var start_lvl_m = d ? LevelPoins.Last().M : LevelPoins.First().M;
                        var final_lvl_km = d ? LevelPoins.First().Km : LevelPoins.Last().Km;
                        var final_lvl_m = d ? LevelPoins.First().M : LevelPoins.Last().M;

                        var razn1 = (int)(((start_km + start_m / 10000.0) - (start_lvl_km + start_lvl_m / 10000.0)) * 10000) % 1000; // start
                        var razn2 = (int)(((final_km + final_m / 10000.0) - (final_lvl_km + final_lvl_m / 10000.0)) * 10000) % 1000; // final
                        var razn3 = lenKriv - lenKrivlv; // общая длина нижних

                        //верхние 2 точки трапеции
                        var start_kmc = d ? StrPoins[2].Km : StrPoins[1].Km;
                        var start_mc = d ? StrPoins[2].M : StrPoins[1].M;
                        var final_kmc = d ? StrPoins[1].Km : StrPoins[2].Km;
                        var final_mc = d ? StrPoins[1].M : StrPoins[2].M;

                        var start_lvl_kmc = d ? LevelPoins[2].Km : LevelPoins[1].Km;
                        var start_lvl_mc = d ? LevelPoins[2].M : LevelPoins[1].M;
                        var final_lvl_kmc = d ? LevelPoins[1].Km : LevelPoins[2].Km;
                        var final_lvl_mc = d ? LevelPoins[1].M : LevelPoins[2].M;

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

                        lvl = (int)(temp_data_lvl.Select(o => o.Level).Average());
                        str = (int)(17860 / Math.Abs(temp_data_str.Select(o => o.Radius).Average()));
                    }
                    catch
                    {
                        Console.WriteLine("Ошибка при расчете натурной кривой");
                    }

                    var curve_center = (bpd_curve.Start_Km * 1000 + bpd_curve.Start_M) + ((bpd_curve.Final_Km * 1000 + bpd_curve.Final_M) - (bpd_curve.Start_Km * 1000 + bpd_curve.Start_M)) / 2;
                    var pkm = curve_center / 1000;
                    var first_pmeter = curve_center % 1000;

                    int curve_krug = -1;
                    if (lenPerKrivlv != -1)
                    {
                        curve_krug = lenPerKrivlv;
                    }
                    else
                        curve_krug = Math.Abs((bpd_curve.Elevations.First().Start_Km * 1000 + bpd_curve.Elevations.First().Start_M) -
                                                   (bpd_curve.Elevations.First().Final_Km * 1000 + bpd_curve.Elevations.First().Final_M)) -
                                                   (bpd_curve.Elevations.First().Transition_1 + bpd_curve.Elevations.First().Transition_2);

                    if (pkm == kilometer.Number)
                    {
                        var rdcsData = new Data { };
                        var Vkr = RoundNumToFive(rdcsData.GetKRSpeedPass(rdcs));
                        var Ogr = -1;
                        if (kilometer.Speeds.First().Passenger > Vkr)
                            Ogr = Vkr;
                        var pmeter = kilometer.Length - first_pmeter;
                        pmeter = kilometer.LevelAvgTrapezoid.Count - 1 < pmeter ? kilometer.LevelAvgTrapezoid.Count - 1 : pmeter;


                        var diff = Math.Abs(Math.Abs((int)kilometer.LevelAvgTrapezoid[pmeter]) - Math.Abs((int)bpd_curve.Elevations.First().Lvl));

                        //балл
                        var ball = -1;
                        var razn = -1;
                        if (140 < PasSpeed && 10 < diff)
                        {
                            ball = 50;
                            razn = diff - 10;
                        }
                        else if ((101 <= PasSpeed && PasSpeed <= 140) && 15 < diff)
                        {
                            ball = 50;
                            razn = diff - 15;
                        }
                        else if ((61 <= PasSpeed && PasSpeed <= 100) && 20 < diff)
                        {
                            ball = 50;
                            razn = diff - 20;
                        }
                        else if (PasSpeed <= 60 && 25 < diff)
                        {
                            ball = 50;
                            razn = diff - 25;
                        }

                        if (ball != -1)
                        {
                            try
                            {
                                pru_dig_list.Add(new DigressionMark
                                {
                                    Km = kilometer.Number,
                                    Meter = first_pmeter,
                                    Value = diff, //высота
                                    Length = curve_krug, // длина круговой
                                    Count = ball,
                                    DigName = DigressionName.Pru.Name,
                                    PassengerSpeedAllow = kilometer.Speeds.First().Passenger,
                                    PassengerSpeedLimit = kilometer.Speeds.First().Passenger > Ogr ? Ogr : -1,

                                    FreightSpeedAllow = kilometer.Speeds.First().Freight,
                                    FreightSpeedLimit = kilometer.Speeds.First().Freight > Ogr ? Ogr : -1,
                                });
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"ПрУ write s3 error {e.Message}");
                            }
                        }


                        if (lvl != -1)
                        {
                            Curve_nature_value.Add(new DigressionMark()
                            {
                                Meter = first_pmeter,
                                Alert = $"{first_pmeter} Крив. факт R:{ str } h:{ lvl }"
                            });
                        }
                    }

                   //if (!curve_bpd_list.Any())
                   {
                        curve_bpd_list.Add(new DigressionMark()
                        {
                            Km = -999,
                            Meter = first_pmeter,

                            Alert = $"{first_pmeter - 30}  Паспорт R:{ str} h:{lvl}"
                        });
                    }

                    //if (curve_bpd_list.Any())
                    // curve_bpd_list.Select()



                }

                if (pru_dig_list.Any())
                    MainTrackStructureRepository.Pru_write(kilometer.Track_id, kilometer, pru_dig_list);
                if (curve_bpd_list.Any())
                    MainTrackStructureRepository.Bpd_write(kilometer.Track_id, kilometer, curve_bpd_list);

                // добавление ПрУ и натурные значения кривой
                kilometer.Digressions = Curve_nature_value;

                //контрольные участки КУ МО СКО
                var ku = new List<DigressionMark> { };
                try
                {
                    var MtoCheckSection = MainTrackStructureRepository.GetMtoObjectsByCoord(
                        trip.Trip_date,
                        kilometer.Number,
                        MainTrackStructureConst.MtoCheckSection,
                        trip.Direction_Name,
                        kilometer.Track_name) as List<CheckSection>;

                    foreach (var sect in MtoCheckSection)
                    {
                        var CheckSection_center = (sect.Start_Km * 1000 + sect.Start_M) + ((sect.Final_Km * 1000 + sect.Final_M) - (sect.Start_Km * 1000 + sect.Start_M)) / 2;
                        var pkm = CheckSection_center / 1000;
                        var first_pmeter = CheckSection_center % 1000;

                        if (pkm == kilometer.Number)
                        {
                            var CheckVerifyKm = RdStructureRepository.CheckVerify(
                                                            kilometer.Trip.Id,
                                                            sect.Start_Km * 1000 + sect.Start_M,
                                                            sect.Final_Km * 1000 + sect.Final_M);
                            if (CheckVerifyKm.Any())
                            {
                                //curve
                                var curve_msg = $"КУ: параметр уровень в норме_" +
                                                $"(МО: {sect.Avg_level:0.0}/{CheckVerifyKm.First().Trip_mo_level:0.0})";
                                var diff_curve = Math.Abs(sect.Avg_level - CheckVerifyKm.First().Trip_mo_level);
                                if (diff_curve >= 2)
                                {
                                    curve_msg = $"КУ: превыш. допуска смещения_" +
                                          $"({diff_curve:0.0}) пар. уровень (МО: {sect.Avg_level:0.0}/{CheckVerifyKm.First().Trip_mo_level:0.0})!";
                                }
                                //gauge
                                var gauge_msg = $"КУ: параметр шаблон в норме_" +
                                                $"(МО: {sect.Avg_width:0.0}/{CheckVerifyKm.First().Trip_mo_gauge:0.0})";
                                var diff_gauge = Math.Abs(sect.Avg_width - CheckVerifyKm.First().Trip_mo_gauge);
                                if (diff_gauge > 2)
                                {
                                    gauge_msg = $"КУ: превыш. допуска смещения_" +
                                          $"({diff_gauge:0.0}) пар. шаблон (МО: {sect.Avg_width:0.0}/{CheckVerifyKm.First().Trip_mo_gauge:0.0})!";
                                }
                                ku.Add(new DigressionMark()
                                {
                                    Meter = first_pmeter,
                                    Alert = $"{curve_msg}"
                                });

                                ku.Add(new DigressionMark()
                                {
                                    Meter = first_pmeter,
                                    Alert = $"{gauge_msg}"
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("контрольные участки КУ МО СКО error " + e.Message);
                }

                kilometer.Digressions.AddRange(ku);
                kilometer.LoadDigresions(RdStructureRepository, MainTrackStructureRepository, trip);

                string drawdownRight = string.Empty, drawdownLeft = string.Empty, gauge = string.Empty, zeroGauge = string.Empty,
                        zeroStraightening = string.Empty, averageStraighteningRight = string.Empty, straighteningRight = string.Empty,
                        zeroStraighteningLeft = string.Empty, averageStraighteningLeft = string.Empty, straighteningLeft = string.Empty,
                        level = string.Empty, averageLevel = string.Empty, zeroLevel = string.Empty;

                int fourStepOgrCoun = 0, otherfourStepOgrCoun = 0;

                svgLength = kilometer.GetLength() < 1000 ? 1000 : kilometer.GetLength();
                var xp = (-kilometer.Start_m - svgLength - 50) + (svgLength + 105) - 52;
                var direction = AdmStructureRepository.GetDirectionByTrack(kilometer.Track_id);

                XElement addParam = new XElement("addparam",
                    new XAttribute("top-title",
                        (direction != null ? $"{direction.Name} ({direction.Code} )" : "Неизвестный") + " Путь: " + kilometer.Track_name + " Км:" +
                        kilometer.Number + " " + (kilometer.PdbSection.Count > 0 ? kilometer.PdbSection[0].ToString() : " ПЧ-/ПЧУ-/ПД-/ПДБ-") + " Уст: " + " " +
                        (kilometer.Speeds.Count > 0 ? $"{kilometer.Speeds.First().Passenger}/{kilometer.Speeds.First().Freight}" : "-/-") +
                        $" Скор:{(int)kilometer.Speed.Average()}"),

                    new XAttribute("right-title",
                        copyright + ": " + "ПО " + softVersion + "  " +
                        systemName + ":" + trip.Car + "(" + trip.Chief.Trim() + ") (БПД от " + MainTrackStructureRepository.GetModificationDate() + ") <" + (kilometer.PdbSection.Count > 0 ? kilometer.PdbSection[0].RoadAbbr : "НЕИЗВ") + ">" + "<" + kilometer.Passage_time.ToString("dd.MM.yyyy  HH:mm") + ">" +
                        "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(trip.Travel_Direction.ToString())) + ">" +
                        "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(trip.Car_Position.ToString())) + ">" +
                        "<" + trip.Trip_date.Month + "-" + trip.Trip_date.Year + " " + (trip.Trip_Type == TripType.Control ? "контр." : trip.Trip_Type == TripType.Work ? "раб." : "доп.") + " Проезд:" + trip.Trip_date.ToString("dd.MM.yyyy  HH:mm") + " " + diagramName + ">"
                        ),
                    new XAttribute("pre", xp + 30),
                    new XAttribute("prer", xp + 21),
                    new XAttribute("topr", -kilometer.Start_m - svgLength - 45),
                    new XAttribute("topf", xp + 10),
                    new XAttribute("topx", -kilometer.Start_m - svgLength),
                    new XAttribute("topx1", -kilometer.Start_m - svgLength - 30),
                    new XAttribute("topx2", -kilometer.Start_m - svgLength - 15),
                    new XAttribute("fragment", (kilometer.StationSection != null && kilometer.StationSection.Count > 0 ? "Станция: " + kilometer.StationSection[0].Station : (kilometer.Sector != null ? kilometer.Sector.ToString() : "")) + " Км:" + kilometer.Number),
                    new XAttribute("viewbox", $"-20 {-kilometer.Start_m - svgLength - 50} 830 {svgLength + 105}"),
                    new XAttribute("minY", -kilometer.Start_m),
                    new XAttribute("maxY", -kilometer.Final_m),
                    new XAttribute("minYround", -(kilometer.Start_m - kilometer.Start_m % 100)),

                    RightSideChart(trip.Trip_date, kilometer, kilometer.Track_id, new float[] { 151f, 146f, 152.5f, 155f }),

                    new XElement("xgrid",
                        new XElement("x", MMToPixelChartString(LevelPosition - LevelStep), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "  –30"), new XAttribute("y", MMToPixelChartString(LevelPosition - LevelStep - 0.5f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(LevelPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(LevelPosition - 0.5f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(LevelPosition + LevelStep), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "    30"), new XAttribute("y", MMToPixelChartString(LevelPosition + LevelStep - 0.5f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(StraighRighttPosition - StrightStep), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "  –30"), new XAttribute("y", MMToPixelChartString(StraighRighttPosition - StrightStep - 0.5f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(StraighRighttPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(StraighRighttPosition - 1f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(StraighRighttPosition + StrightStep / 10f), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "      3"), new XAttribute("y", MMToPixelChartString(StraighRighttPosition + StrightStep / 10f + 0.2f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(StrightLeftPosition - StrightStep / 10f), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "    –3"), new XAttribute("y", MMToPixelChartString(StrightLeftPosition - StrightStep / 10f - 1f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(StrightLeftPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(StrightLeftPosition + 0.2f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(StrightLeftPosition + StrightStep), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "    30"), new XAttribute("y", MMToPixelChartString(StrightLeftPosition + StrightStep - 0.5f)), new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(GaugePosition - 10 * GaugeKoef), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey")),
                        new XElement("x", MMToPixelChartString(GaugePosition - 8 * GaugeKoef), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "1512"), new XAttribute("y", MMToPixelChartString(GaugePosition - 8 * GaugeKoef - 0.5f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(GaugePosition - 4 * GaugeKoef), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey")),
                        new XElement("x", MMToPixelChartString(GaugePosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "1520"), new XAttribute("y", MMToPixelChartString(GaugePosition - 0.5f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(GaugePosition + 8 * GaugeKoef), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "1528"), new XAttribute("y", MMToPixelChartString(GaugePosition + 8 * GaugeKoef - 0.5f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(GaugePosition + 16 * GaugeKoef), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "1536"), new XAttribute("y", MMToPixelChartString(GaugePosition + 16 * GaugeKoef - 0.5f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(GaugePosition + 22 * GaugeKoef), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "1542"), new XAttribute("y", MMToPixelChartString(GaugePosition + 22 * GaugeKoef - 0.5f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(GaugePosition + 26 * GaugeKoef), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey")),
                        new XElement("x", MMToPixelChartString(GaugePosition + 28 * GaugeKoef), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "1548"), new XAttribute("y", MMToPixelChartString(GaugePosition + 28 * GaugeKoef - 0.5f)), new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(ProsRightPosition - 10 * ProsKoef), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "  –10"), new XAttribute("y", MMToPixelChartString(ProsRightPosition - 10 * ProsKoef - 0.5f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(ProsRightPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(ProsRightPosition - 0.5f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(ProsRightPosition + 10 * ProsKoef), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "    10"), new XAttribute("y", MMToPixelChartString(ProsRightPosition + 10 * ProsKoef - 0.5f)), new XAttribute("x", xp + 15)),

                        new XElement("x", MMToPixelChartString(ProsLeftPosition - 10 * ProsKoef), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "  –10"), new XAttribute("y", MMToPixelChartString(ProsLeftPosition - 10 * ProsKoef - 0.5f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(ProsLeftPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(ProsLeftPosition - 0.5f)), new XAttribute("x", xp + 15)),
                        new XElement("x", MMToPixelChartString(ProsLeftPosition + 10 * ProsKoef), new XAttribute("dasharray", "0.5,2"), new XAttribute("stroke", "grey"), new XAttribute("label", "    10"), new XAttribute("y", MMToPixelChartString(ProsLeftPosition + 10 * ProsKoef - 0.5f)), new XAttribute("x", xp + 15))
                        ));
                //kil
                var meterCount = (currentKmMeter + 500) > kilometer.meter.Count() ? kilometer.meter.Count() : currentKmMeter + 500;
                var startIndex = currentKmMeter - 250;
                for (int index = currentKmMeter; index < meterCount - 1; index++)
                {
                    try
                    {
                        int metre = -kilometer.meter[index + 1];
                        drawdownRight += MMToPixelChartString(kilometer.DrawdownLeft[index] * ProsKoef + ProsRightPosition) + "," + metre + " ";
                        drawdownLeft += MMToPixelChartString(kilometer.DrawdownRight[index] * ProsKoef + ProsLeftPosition) + "," + metre + " ";
                        gauge += MMToPixelChartString((kilometer.Gauge[index] - 1520) * GaugeKoef + GaugePosition) + "," + metre + " ";
                        zeroGauge += MMToPixelChartString((kilometer.fsh0[index] - 1520) * GaugeKoef + GaugePosition) + "," + metre + " ";

                        zeroStraightening += MMToPixelChartString(Math.Abs(kilometer.fZeroStright[index]) * Math.Sign(kilometer.StrightRight[index]) * StrightKoef + StraighRighttPosition) + "," + metre + " ";
                        averageStraighteningRight += MMToPixelChartString(kilometer.StrightAvgTrapezoid[index] * StrightKoef + StraighRighttPosition) + "," + metre + " ";
                        var drh = kilometer.StrightAvgTrapezoid[index] + (kilometer.StrightRight[index] - kilometer.StrightAvgTrapezoid[index]);
                        straighteningRight += MMToPixelChartString(drh * StrightKoef + StraighRighttPosition) + "," + metre + " ";

                        zeroStraighteningLeft += MMToPixelChartString(Math.Abs(kilometer.fZeroStright[index]) * Math.Sign(kilometer.StrightRight[index]) * StrightKoef + StrightLeftPosition) + "," + metre + " ";


                        averageStraighteningLeft += MMToPixelChartString(kilometer.StrightAvgTrapezoid[index] * StrightKoef + StrightLeftPosition) + "," + metre + " ";
                        drh = kilometer.StrightAvgTrapezoid[index] + (kilometer.StrightLeft[index] - kilometer.StrightAvgTrapezoid[index]);
                        straighteningLeft += MMToPixelChartString(drh * StrightKoef + StrightLeftPosition) + "," + metre + " ";


                        level += MMToPixelChartString(kilometer.Level[index] * LevelKoef + LevelPosition) + "," + metre + " ";
                        averageLevel += MMToPixelChartString(kilometer.LevelAvgTrapezoid[index] * LevelKoef + LevelPosition) + "," + metre + " ";
                        zeroLevel += MMToPixelChartString(Math.Abs(kilometer.flvl0[index]) * Math.Sign(kilometer.StrightRight[index]) * LevelKoef + LevelPosition) + "," + metre + " ";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Рисование линий А4 ошибка " + e.Message);
                    }
                }

                var style = "fill:none;stroke:dimgray;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3";
                var styleAverage = "fill:none;stroke:dimgray;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.7; stroke-dasharray:0.7 0.6;";

                addParam.Add(new XElement("polyline", new XAttribute("points", drawdownRight), new XAttribute("style", style)));
                addParam.Add(new XElement("polyline", new XAttribute("points", drawdownLeft), new XAttribute("style", style)));
                addParam.Add(new XElement("polyline", new XAttribute("points", gauge), new XAttribute("style", style)));
                addParam.Add(new XElement("polyline", new XAttribute("points", zeroGauge), new XAttribute("style", style)));

                addParam.Add(new XElement("polyline", new XAttribute("points", zeroStraightening), new XAttribute("style", style)));
                addParam.Add(new XElement("polyline", new XAttribute("points", averageStraighteningRight), new XAttribute("style", styleAverage)));
                addParam.Add(new XElement("polyline", new XAttribute("points", straighteningRight), new XAttribute("style", style)));

                addParam.Add(new XElement("polyline", new XAttribute("points", zeroStraighteningLeft), new XAttribute("style", style)));
                addParam.Add(new XElement("polyline", new XAttribute("points", averageStraighteningLeft), new XAttribute("style", styleAverage)));
                addParam.Add(new XElement("polyline", new XAttribute("points", straighteningLeft), new XAttribute("style", style)));

                addParam.Add(new XElement("polyline", new XAttribute("points", level), new XAttribute("style", style)));
                addParam.Add(new XElement("polyline", new XAttribute("points", averageLevel), new XAttribute("style", styleAverage)));
                addParam.Add(new XElement("polyline", new XAttribute("points", zeroLevel), new XAttribute("style", style)));


                char separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];

                var digElemenets = new XElement("digressions");
                List<int> usedTops = new List<int>();
                List<int> speedmetres = new List<int>();

                var gmeter = kilometer.Start_m.RoundTo10() + 10;


                foreach (var picket in kilometer.Pickets)
                {
                    picket.WriteNotesToReport(
                        kilometer,
                        speedmetres,
                        addParam,
                        digElemenets,
                        ProsRightPosition,
                        ProsLeftPosition,
                        StraighRighttPosition,
                        StrightLeftPosition,
                        GaugePosition,
                        LevelPosition,
                        this,
                        ref fourStepOgrCoun,
                        ref otherfourStepOgrCoun);
                }

                //addParam.Add(new XAttribute("common", kilometer.GetdigressionsCount));

                var GetBedomost = new List<Bedemost> { };
                try
                {
                    // 1степень санау
                    GetBedomost = ((List<Bedemost>)RdStructureRepository.GetBedemost(kilometer.Trip.Id)).Where(o => o.Km == kilometer.Number).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetBedomost " + e.Message);
                }
                addParam.Add(
                    new XAttribute("speedlimit",
                    kilometer.CalcMainPoint() + " " +
                    $"Кол.ст.- 1:{(GetBedomost.Any() ? GetBedomost.First().Type1 : 0)}; " + kilometer.GetdigressionsCount +
                    "  Кол.огр.:" + fourStepOgrCoun + "/" + otherfourStepOgrCoun + " Огр: " + kilometer.SpeedLimit + " Пред:- ")
                    );

                addParam.Add(digElemenets);
                report.Add(addParam);

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save($@"C:\work_shifrovka\{kilometer.Number}_{kilometer.Track_id}.html");

                //htReport.Save($@"\\192.168.1.200\form\{kilometer.Number}_{kilometer.Track_id}.html");
                var svg = htReport.Element("html").Element("body").Element("div").Element("div").Element("svg");
                var svgDoc = SvgDocument.FromSvg<SvgDocument>(svg.ToString());
                svgDoc.Width = 830 * 3;
                svgDoc.Height = (svgLength + 115) * 3;

                if (autoprint)
                {

                    PrintDocument pd = new PrintDocument();
                    //pd.PrinterSettings.PrinterName = "EPSON5EEE22";
                    Console.WriteLine(pd.PrinterSettings.PrinterName);

                    pd.PrintPage += (sender, args) =>
                    {
                        var i = svgDoc.Draw();
                        //i.Save($@"g:\form\{kilometer.Number}_{kilometer.Track_id}.png");
                        pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                        pd.OriginAtMargins = false;
                        pd.DefaultPageSettings.Landscape = true;
                        Point p = new Point(0, 0);
                        args.Graphics.DrawImage(i, 0, 0, i.Width / 3, i.Height / 3 - 15);
                    };
                    pd.Print();

                }



            }
            catch (Exception e)
            {
                System.Console.WriteLine("Ошибка при сохранении отчетной формы: " + e.Message);
            }
            finally
            {
                // System.Diagnostics.Process.Start(Path.GetTempPath() + "/report.html");
            }
        }
    }
}