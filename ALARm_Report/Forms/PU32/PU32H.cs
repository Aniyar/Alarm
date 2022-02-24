//using ALARm.Core;
//using ALARm.Core.AdditionalParameteres;
//using ALARm.Core.Report;
//using ALARm.Services;
//using ALARm_Report.controls;
//using MetroFramework.Controls;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Windows.Forms;
//using System.Xml;
//using System.Xml.Linq;
//using System.Xml.Xsl;

//namespace ALARm_Report.Forms
//{
//    public class PU32H : Report
//    {
//        private string engineer { get; set; } = "Komissia K";
//        private string chief { get; set; } = "Komissia K";
//        private DateTime from, to;
//        private TripType tripType;
//        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
//        {
//            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
//            using (var filterForm = new FilterForm())
//            {
//                var filters = new List<Filter>();
//                filters.Add(new StringFilter() { Name = "Начальник путеизмерительного вагона: ", Value = chief });
//                filters.Add(new StringFilter() { Name = "Данные обработали и оформили ведомость: ", Value = engineer });
//                filters.Add(new DateFilter() { Name = "Дата проверки с:", Value = period.StartDate.ToString("dd.MM.yyyy") });
//                filters.Add(new DateFilter() { Name = "                         по:", Value = period.FinishDate.ToString("dd.MM.yyyy") });
//                filters.Add(new TripTypeFilter() { Name = "Тип поездки", Value = "рабочая" });

//                filterForm.SetDataSource(filters);
//                if (filterForm.ShowDialog() == DialogResult.Cancel)
//                    return;

//                chief = (string)filters[0].Value;
//                engineer = (string)filters[1].Value;
//                tripType = ((TripTypeFilter)filters[4]).TripType;
//                from = DateTime.Parse((string)filters[2].Value);
//                to = DateTime.Parse((string)filters[3].Value + " 23:59:59");
//            }
//            XDocument htReport = new XDocument();
//            using (XmlWriter writer = htReport.CreateWriter())
//            {
//                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
//                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;

//                XDocument xdReport = new XDocument();

//                var kilometers = RdStructureService.GetPU32Kilometers(from, to, parentId, tripType); //.GetRange(65,15);
//                if (kilometers.Count == 0)
//                    return;

//                XElement report = new XElement("report",
//                    new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
//                    new XAttribute("engineer", engineer),
//                    new XAttribute("from", from.ToString("dd.MM.yyyy")),
//                    new XAttribute("to", to.ToString("dd.MM.yyyy")),
//                    new XAttribute("road", roadName),
//                    new XAttribute("distance", distance.Code),
//                    new XAttribute("month", CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(period.PeriodMonth)),
//                    new XAttribute("triptype", tripType == TripType.Control ? "контрольной" : tripType == TripType.Work ? "рабочей" : "дополнительной"),
//                    new XAttribute("soft", "ALARm 1.0(436-р)"),
//                    new XAttribute("tripdate", kilometers[0].TripDate.ToString("dd.MM.yyyy_hh:mm")),
//                    new XAttribute("car", 24543),
//                    new XAttribute("chief", chief)
//                );



//                var byKilometer = new XElement("bykilometer",
//                                    new XAttribute("code", kilometers[0].Direction_code),
//                                    new XAttribute("track", kilometers[0].Track_name),
//                                    new XAttribute("name", kilometers[0].Direction_name),
//                                    new XAttribute("pch", distance.Code));

//                var distanceTotal = new Total
//                {
//                    Code = distance.Code
//                };
//                var sectionTotal = new Total
//                {
//                    Code = kilometers[0].Track_name,
//                    DirectionCode = kilometers[0].Direction_code,
//                    DirectionName = kilometers[0].Direction_name
//                };

//                //запрос доп параметров с бд
//                var AddParam = AdditionalParametersService.GetAddParam(kilometers.First().Trip_id) as List<S3>; //износы
//                if (AddParam == null)
//                {
//                    MessageBox.Show("Не удалось сформировать отчет, так как возникала ошибка во время загрузки данных по доп параметрам");
//                    return;
//                }
//                List<Gap> check_gap_state = AdditionalParametersService.Check_gap_state(kilometers.First().Trip_id, template.ID); //стыки



//                var ListS3 = RdStructureService.GetS3(kilometers.First().Trip_id) as List<S3>; //пру


//                var PRU = ListS3.Where(o => o.Ots == "ПрУ").ToList();
//                var gapV = check_gap_state.Where(o => o.Vdop != "" && o.Vdop != "-/-").ToList();

//                var Pu32_gap = check_gap_state.Where(o => o.Zazor >= 31 || o.R_zazor >= 31).ToList();




//                var dopBall = PRU.Count() * 50 + (gapV.Count() > 0 ? 50 : 0) + (AddParam.Count() > 0 ? 50 : 0);
//                var DopCount = gapV.Count() + AddParam.Count();

//                var sectionElement = new XElement("section",
//                    new XAttribute("name", kilometers[0].Direction_name),
//                    new XAttribute("code", kilometers[0].Direction_code),
//                    new XAttribute("track", kilometers[0].Track_name)
//                    );
//                var pchuElement = new XElement("pchu",
//                    new XAttribute("pch", distance.Code),
//                    new XAttribute("code", kilometers[0].PchuCode),
//                    new XAttribute("chief", kilometers[0].PchuChief)
//                    );
//                var pchuTotal = new Total();
//                pchuTotal.Code = kilometers[0].PchuCode;

//                var pdElement = new XElement("pd",
//                    new XAttribute("code", kilometers[0].PdCode),
//                    new XAttribute("chief", kilometers[0].PdChief)
//                    );
//                var pdTotal = new Total();
//                pdTotal.Code = kilometers[0].PdCode;

//                var pdbElement = new XElement("pdb",
//                    new XAttribute("code", kilometers[0].PdbCode),
//                    new XAttribute("chief", kilometers[0].PdbChief));
//                var pdbTotal = new Total();
//                pdbTotal.Code = kilometers[0].PdbCode;

//                int Grk = 0, Sochet = 0, Kriv = 0, Pru = 0, Oshk = 0, Iznos = 0, Zazor = 0, NerProf = 0, KmSpeedLimit = 0;

//                progressBar.Maximum = kilometers.Count;
//                foreach (var km in kilometers)
//                {
//                    progressBar.Value += 1;

//                    km.LoadTrackPasport(MainTrackStructureService.GetRepository(), km.TripDate);
//                    if (!sectionTotal.Code.Equals(km.Track_name) || !sectionTotal.DirectionCode.Equals(km.Direction_code))
//                    {
//                        PDBTotalGenerate(ref pdbElement, ref pdbTotal, ref pdElement, ref pdTotal, km.PdbCode, km.PdbChief);
//                        PDTotalGenerate(ref pdElement, ref pdTotal, ref pchuElement, ref pchuTotal, km.PdCode, km.PdChief);
//                        PCHUTotalGenerate(ref pchuElement, ref pchuTotal, ref sectionElement, ref sectionTotal, km.PchuCode, km.PchuChief);
//                        SectionTotalGenerate(ref sectionElement, ref sectionTotal, ref byKilometer, ref distanceTotal, km.Track_name, km.Direction_code, km.Direction_name);
//                    }
//                    if (!pchuTotal.Code.Equals(km.PchuCode))
//                    {
//                        PDBTotalGenerate(ref pdbElement, ref pdbTotal, ref pdElement, ref pdTotal, km.PdbCode, km.PdbChief);
//                        PDTotalGenerate(ref pdElement, ref pdTotal, ref pchuElement, ref pchuTotal, km.PdCode, km.PdChief);
//                        PCHUTotalGenerate(ref pchuElement, ref pchuTotal, ref sectionElement, ref distanceTotal, km.PchuCode, km.PchuChief);
//                    }
//                    if (!pdTotal.Code.Equals(km.PdCode))
//                    {
//                        PDBTotalGenerate(ref pdbElement, ref pdbTotal, ref pdElement, ref pdTotal, km.PdbCode, km.PdbChief);
//                        PDTotalGenerate(ref pdElement, ref pdTotal, ref pchuElement, ref pchuTotal, km.PdCode, km.PdChief);
//                    }

//                    if (!pdbTotal.Code.Equals(km.PdbCode))
//                    {
//                        PDBTotalGenerate(ref pdbElement, ref pdbTotal, ref pdElement, ref pdTotal, km.PdbCode, km.PdbChief);
//                    }


//                    km.Digressions = RdStructureService.GetDigressionMarks(km.Trip_id, km.Track_id, km.Number);

//                    //данные стыка
//                    var gaps = new List<Digression> { };
//                    foreach (var gap in check_gap_state.Where(o => o.Km == km.Number && (o.Zazor >= 31 || o.R_zazor >= 31)).ToList())
//                    {
//                        gap.PassSpeed = km.Speeds.Count > 0 ? km.Speeds[0].Passenger : -1;
//                        gap.FreightSpeed = km.Speeds.Count > 0 ? km.Speeds[0].Freight : -1;

//                        gaps.Add(gap.GetDigressions());
//                        gaps.Add(gap.GetDigressions3());
//                    }
//                    foreach (var dig in gaps.Where(o => o.Velich >= 31).ToList())
//                    {
//                        int count = dig.Length / 4;
//                        count += dig.Length % 4 > 0 ? 1 : 0;

//                        var side = " " + (dig.Threat == (Threat)Threat.Right ? "п." : dig.Threat == (Threat)Threat.Left ? "л." : "");

//                        km.Digressions.Add(
//                            new DigressionMark()
//                            {
//                                Digression = dig.DigName,
//                                NotMoveAlert = false,
//                                Meter = dig.Meter,
//                                finish_meter = dig.Kmetr,
//                                Length = dig.Length,
//                                Value = dig.Velich,
//                                Count = count,
//                                DigName = dig.GetName() + side,
//                                PassengerSpeedLimit = dig.PassengerSpeedLimit,
//                                FreightSpeedLimit = dig.FreightSpeedLimit,
//                                Comment = "",
//                                AllowSpeed = dig.AllowSpeed
//                            });

//                    }



//                    //данные Износа рельса Бок.из.
//                    var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(km.Number, km.Trip_id);
//                    if (DBcrossRailProfile == null) continue;

//                    var sortedData = DBcrossRailProfile.OrderByDescending(d => d.Meter).ToList();
//                    var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(sortedData);

//                    List<Digression> addDigressions = crossRailProfile.GetDigressions();

//                    var dignatur = new List<DigressionMark> { };
//                    foreach (var dig in addDigressions)
//                    {
//                        int count = dig.Length / 4;
//                        count += dig.Length % 4 > 0 ? 1 : 0;

//                        if (dig.DigName == DigressionName.SideWearLeft || dig.DigName == DigressionName.SideWearRight)
//                        {
//                            dignatur.Add(new DigressionMark()
//                            {
//                                Digression = dig.DigName,
//                                NotMoveAlert = false,
//                                Meter = dig.Meter,
//                                finish_meter = dig.Kmetr,
//                                Degree = (int)dig.Typ,
//                                Length = dig.Length,
//                                Value = dig.Value,
//                                Count = count,
//                                DigName = dig.GetName(),
//                                PassengerSpeedLimit = -1,
//                                FreightSpeedLimit = -1,
//                                Comment = "",
//                                Diagram_type = "Iznos_relsov",
//                                Digtype = DigressionType.Additional
//                            });
//                        }
//                    }
//                    //выч-е скорости бок износа
//                    int pas = 999, gruz = 999;
//                    foreach (DigressionMark item in dignatur)
//                    {
//                        if (item.Digression == DigressionName.SideWearLeft || item.Digression == DigressionName.SideWearRight)
//                        {
//                            var c = km.Curves.Where(o => o.RealStartCoordinate <= km.Number + item.Meter / 10000.0 && o.RealFinalCoordinate >= km.Number + item.Meter / 10000.0).ToList();

//                            if (c.Any())
//                            {
//                                item.GetAllowSpeedAddParam(km.Speeds.First(), c.First().Straightenings[0].Radius, item.Value);

//                                if (item.PassengerSpeedLimit != -1 && item.PassengerSpeedLimit < pas)
//                                {
//                                    pas = item.PassengerSpeedLimit;
//                                }
//                                if (item.FreightSpeedLimit != -1 && item.FreightSpeedLimit < gruz)
//                                {
//                                    gruz = item.FreightSpeedLimit;
//                                }
//                            }
//                        }
//                        else if (item.Digression == DigressionName.Gap)
//                        {
//                            if (item.PassengerSpeedLimit != -1 && item.PassengerSpeedLimit < pas)
//                            {
//                                pas = item.PassengerSpeedLimit;
//                            }
//                            if (item.FreightSpeedLimit != -1 && item.FreightSpeedLimit < gruz)
//                            {
//                                gruz = item.FreightSpeedLimit;
//                            }
//                        }

//                    }
//                    dignatur = dignatur.Where(o => o.PassengerSpeedLimit >= 0 || o.FreightSpeedLimit >= 0).ToList();

//                    km.Digressions.AddRange(dignatur);

//                    var gap_dig = AddParam.Where(o => o.Km == km.Number).ToList();
//                    foreach (var item in gap_dig)
//                    {
//                        km.Digressions.Add(new DigressionMark
//                        {
//                            Km = item.Km,
//                            Meter = item.Meter,
//                            Length = item.Len,
//                            DigName = item.Ots,
//                            Count = item.Kol,
//                            Value = item.Otkl,
//                            PassengerSpeedLimit = item.Ogp,
//                            FreightSpeedLimit = item.Ogp,
//                            Digtype = DigressionType.Additional
//                        });
//                    }

//                    //AddParam = AdditionalParametersService.GetAddParam(kilometers.First().Trip_id) as List<S3>; //износы
//                    //List<Gap> check_gap_state = AdditionalParametersService.Check_gap_state(kilometers.First().Trip_id, template.ID); //стыки
//                    //var ListS3 = RdStructureService.GetS3(kilometers.First().Trip_id) as List<S3>; //пру

//                    //подсчет для таблицы Количество километров с неисправностями по видам по ПЧ64

//                    var grk = km.Digressions.Where(o => o.DigName.Equals("Суж") || o.DigName.Equals("Уш") || o.DigName.Equals("У") ||
//                                                        o.DigName.Equals("Р") || o.DigName.Equals("П") || o.DigName.Equals("Пр.л") || o.DigName.Equals("Пр.п")).ToList();
//                    if (grk.Any()) Grk++;

//                    var kriv = km.Digressions.Where(o => o.DigName.Equals("Укл") || o.DigName.Equals("?Уkл") || o.DigName.Equals("Анп") || o.DigName.Equals("Пси")).ToList();
//                    if (kriv.Any()) Kriv++;

//                    var pru = km.Digressions.Where(o => o.DigName.Equals("ПрУ")).ToList();
//                    if (pru.Any()) Pru++;

//                    var oshk = km.Digressions.Where(o => o.DigName.Equals("Oтв.ш")).ToList();
//                    if (oshk.Any()) Oshk++;

//                    var iznos = km.Digressions.Where(o => o.DigName.Equals("Иб.л") || o.DigName.Equals("Иб.п") || o.DigName.Equals("Ив.л") || o.DigName.Equals("Ив.п") ||
//                                                          o.DigName.Equals("Ип.л") || o.DigName.Equals("Ип.п")).ToList();
//                    if (iznos.Any()) Iznos++;

//                    var zazor = km.Digressions.Where(o => o.DigName.Equals("З") || o.DigName.Equals("З п.") || o.DigName.Equals("З л.")).ToList();
//                    if (zazor.Any()) Zazor++;


//                    var kmTotal = new Total();
//                    kmTotal.IsKM = true;

//                    var find_add = false;
//                    var add_dig_str = "";

//                    if (km.Number == 147)
//                    {
//                        var cdfg = 0;
//                    }

//                    //Зазор баллы
//                    var isgap = Pu32_gap.Where(o => o.Km == km.Number).ToList();
//                    if (isgap.Any())
//                    {
//                        kmTotal.AddParamPointSum += 50;
//                        Zazor++;
//                        kmTotal.Additional++;

//                    }
//                    //Бок из примеч
//                    //var countBokIz = km.Digressions.Where(o => o.Digression == DigressionName.SideWearLeft || o.Digression == DigressionName.SideWearRight).ToList();
//                    //               if (countBokIz.Any())
//                    //               {
//                    //                   add_dig_str += $" Бок. из {countBokIz.Count}шт";
//                    //               }


//                    var prevCurve_id = -1;
//                    foreach (var digression in km.Digressions)
//                    {
//                        if (digression.Digtype == DigressionType.Main)
//                        {
//                            kmTotal.MainParamPointSum += digression.GetPoint(km);
//                            //ToDo - ағамен ақылдасу керек
//                            kmTotal.CurvePointSum += digression.GetCurvePoint(km);
//                        }

//                        //Износ баллы
//                        if (digression.Digression == DigressionName.SideWearLeft || digression.Digression == DigressionName.SideWearRight)
//                        {
//                            var noteCoord = km.Number.ToDoubleCoordinate(digression.Meter);
//                            var isCurve = km.Curves.Where(o => o.RealStartCoordinate <= noteCoord && o.RealFinalCoordinate >= noteCoord).ToList();
//                            if (isCurve.Any())
//                            {
//                                if (prevCurve_id != (int)isCurve.First().Id)
//                                {
//                                    kmTotal.AddParamPointSum += 50;
//                                    kmTotal.Additional++;
//                                }
//                                prevCurve_id = (int)isCurve.First().Id;
//                            }
//                            add_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{digression.Meter / 100 + 1} {digression.DigName} {digression.Value:0.0}/{digression.Length} ";
//                        }
//                        //Пру кривая баллы
//                        if (digression.Digression == DigressionName.Pru)
//                        {
//                            kmTotal.MainParamPointSum += 50;
//                            kmTotal.Curves++;

//                            add_dig_str += $"пк{digression.Meter / 100 + 1} {digression.DigName} {digression.Value:0.0}/{digression.Length} ";
//                        }
//                        //ПСИ АНП Укл Аг кривая 
//                        if (digression.Digression == DigressionName.Psi ||
//                            digression.Digression == DigressionName.SpeedUp ||
//                            digression.Digression == DigressionName.Ramp)
//                        {
//                            kmTotal.Curves++;
//                        }
//                        //зазоры
//                        if (digression.Digression.Name == "З п." || digression.Digression.Name == "З л.")
//                        {
//                            add_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{digression.Meter / 100 + 1} {digression.DigName} {digression.Value:0.0}/{digression.Length} ";
//                        }


//                        //if (digression.Digtype == DigressionType.Additional)
//                        //{

//                        //	var ball = find_add == true ? 0 : 50;
//                        //	kmTotal.AddParamPointSum += ball;
//                        //	find_add = true;

//                        //	add_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{digression.Meter / 100 + 1} {digression.DigName} {digression.Value}/{digression.Length} ";
//                        //}

//                        digression.DigNameToDigression(digression.DigName);





//                        if (digression.Degree < 5)
//                            switch (digression.DigName)
//                            {
//                                case string digname when digname.Equals("Суж"):
//                                    digression.Digression = DigressionName.Constriction;
//                                    kmTotal.Constriction.Degrees[digression.Degree - 1] += digression.GetCount();
//                                    break;
//                                case string digname when digname.Equals("Уш"):
//                                    digression.Digression = DigressionName.Broadening;
//                                    kmTotal.Broadening.Degrees[digression.Degree - 1] += digression.GetCount();
//                                    break;
//                                case string digname when digname.Equals("У"):
//                                    digression.Digression = DigressionName.Level;
//                                    kmTotal.Level.Degrees[digression.Degree - 1] += digression.GetCount();
//                                    break;
//                                case string digname when digname.Equals("П"):
//                                    digression.Digression = DigressionName.Sag;
//                                    kmTotal.Sag.Degrees[digression.Degree - 1] += digression.GetCount();
//                                    break;
//                                case string digname when digname.Equals("Пр.л") || digname.Equals("Пр.п"):
//                                    digression.Digression = (digname.Equals("Пр.л")) ? DigressionName.DrawdownLeft : DigressionName.DrawdownRight;
//                                    kmTotal.Drawdown.Degrees[digression.Degree - 1] += digression.GetCount();
//                                    break;
//                                case string digname when digname.Equals("Р"):
//                                    digression.Digression = DigressionName.Strightening;
//                                    kmTotal.Strightening.Degrees[digression.Degree - 1] += digression.GetCount();
//                                    break;
//                            }

//                        if ((digression.LimitSpeedToString() != "-/-") && (digression.LimitSpeedToString() != ""))
//                        {
//                            kmTotal.IsLimited = 1;

//                            if (digression.Degree == 4 && digression.Digtype == DigressionType.Main)
//                            {
//                                kmTotal.Fourth++;
//                            }
//                            //else if (digression.Digtype != DigressionType.Main)
//                            //{
//                            //	kmTotal.Additional++;
//                            //}
//                            //else
//                            //{
//                            //	kmTotal.Other++;
//                            //}
//                        }
//                    }

//                    kmTotal.Broadening.Degrees[0] = km.FDBroad;
//                    kmTotal.Constriction.Degrees[0] = km.FDConstrict;
//                    kmTotal.Sag.Degrees[0] = km.FDSkew;
//                    kmTotal.Strightening.Degrees[0] = km.FDStright;
//                    kmTotal.Drawdown.Degrees[0] = km.FDDrawdown;
//                    kmTotal.Level.Degrees[0] = km.FDLevel;

//                    kmTotal.QualitiveRating = km.CalcQulitiveRating(kmTotal.MainParamPointSum, kmTotal.Fourth + kmTotal.Other);

//                    //TODO-----
//                    //kmTotal.QualitiveRating = km.Primech.Length > 0 ? Rating.Н : kmTotal.QualitiveRating;
//                    //---------

//                    var kmElement = new XElement("km",
//                        new XAttribute("n", km.Number),
//                        new XAttribute("len", km.Lkm.ToString("0", nfi)),
//                        new XAttribute("c1", kmTotal.Constriction.ToString()),
//                        new XAttribute("c2", kmTotal.Broadening.ToString()),
//                        new XAttribute("c3", kmTotal.Level.ToString()),
//                        new XAttribute("c4", kmTotal.Sag.ToString()),
//                        new XAttribute("c5", kmTotal.Drawdown.ToString()),
//                        new XAttribute("c6", kmTotal.Strightening.ToString()),
//                        new XAttribute("c7", kmTotal.Common),
//                        new XAttribute("c8", kmTotal.FourthOtherAdd == "-/ - / - " ? "" : kmTotal.FourthOtherAdd),
//                        new XAttribute("c9", $"{kmTotal.MainParamPointSum}/{kmTotal.AddParamPointSum}"),
//                        new XAttribute("c10", kmTotal.QualitiveRating),
//                        new XAttribute("c11", km.Primech + " "),
//                        new XAttribute("c12", add_dig_str), //строка отст 
//                        new XAttribute("apoint", (kmTotal.AddParamPointSum > 0 ? kmTotal.AddParamPointSum.ToString() : "---")), //балл отст для доп пар

//                        new XAttribute("speed", $"{(km.Speeds.Count > 0 ? $"{km.Speeds.Last().Passenger}/{km.Speeds.Last().Freight}" : $"{km.GlobPassSpeed}/{km.GlobFreightSpeed}")}"),
//                        new XAttribute("com", km.Corrected ? "+" : ""),
//                        new XAttribute("mpoint", kmTotal.MainParamPointSum),

//                        new XAttribute("allsum", (kmTotal.MainParamPointSum + kmTotal.CurvePointSum + kmTotal.AddParamPointSum)), //итого по км

//                        new XAttribute("cpoint", (kmTotal.CurvePointSum > 0 ? kmTotal.CurvePointSum.ToString() : "---"))
//                        );

//                    kmTotal.Length = km.Lkm;

//                    pdbTotal += kmTotal;
//                    pdbElement.Add(kmElement);
//                }

//                sectionElement.Add(
//                    new XAttribute("Grk", Grk),
//                    new XAttribute("KRIV", Kriv),
//                    new XAttribute("PRU", Pru),
//                    new XAttribute("OSHK", Oshk),
//                    new XAttribute("IZNOS", Iznos),
//                    new XAttribute("ZAZOR", $"{Pu32_gap.Count()}({gapV.Count()})"),
//                    new XAttribute("NEROVNOSTY", NerProf),
//                    new XAttribute("KMSOGRSKOROST", "0")
//                    );

//                PDBTotalGenerate(ref pdbElement, ref pdbTotal, ref pdElement, ref pdTotal, "", "");
//                PDTotalGenerate(ref pdElement, ref pdTotal, ref pchuElement, ref pchuTotal, "", "");
//                PCHUTotalGenerate(ref pchuElement, ref pchuTotal, ref sectionElement, ref sectionTotal, "", "");
//                SectionTotalGenerate(ref sectionElement, ref sectionTotal, ref byKilometer, ref distanceTotal, "", "", "");

//                byKilometer.Add(new XAttribute("len", distanceTotal.Length.ToString("0.000", nfi)),
//                    new XAttribute("point", $"{distanceTotal.MainParamPointSum}/{distanceTotal.AddParamPointSum}"),
//                    new XAttribute("rating", distanceTotal.GetSectorQualitiveRating()),
//                    new XAttribute("ratecount", $"Отл - {distanceTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {distanceTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {distanceTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {distanceTotal.RatingCounts[3].ToString("0", nfi)};        Средний балл - {(distanceTotal.MainParamPointSum / distanceTotal.RatingCounts.Sum()).ToString("0.0", nfi)}/{(distanceTotal.AddParamPointSum / distanceTotal.RatingCounts.Sum()).ToString("0.0", nfi)}"),
//                    new XAttribute("c1", distanceTotal.Constriction.ToString()),
//                    new XAttribute("c2", distanceTotal.Broadening.ToString()),
//                    new XAttribute("c3", distanceTotal.Level.ToString()),
//                    new XAttribute("c4", distanceTotal.Sag.ToString()),
//                    new XAttribute("c5", distanceTotal.Drawdown.ToString()),
//                    new XAttribute("c6", distanceTotal.Strightening.ToString()),
//                    new XAttribute("c7", distanceTotal.Common),
//                    new XAttribute("c8", distanceTotal.FourthOtherAdd),
//                    new XAttribute("fourt", $"{(distanceTotal.Fourth > 0 ? distanceTotal.Fourth.ToString() : "-")}"),
//                    new XAttribute("excellent", distanceTotal.RatingCounts[0].ToString("0.000", nfi)),
//                    new XAttribute("good", distanceTotal.RatingCounts[1].ToString("0.000", nfi)),
//                    new XAttribute("satisfactory", distanceTotal.RatingCounts[2].ToString("0.000", nfi)),
//                    new XAttribute("bad", distanceTotal.RatingCounts[3].ToString("0.000", nfi)),
//                    new XAttribute("limit", distanceTotal.IsLimited),
//                    new XAttribute("d4", distanceTotal.Fourth),
//                    new XAttribute("other", (distanceTotal.Combination + distanceTotal.Curves + distanceTotal.Other)),
//                    new XAttribute("add", distanceTotal.Additional),
//                    new XAttribute("repair", distanceTotal.Repairing),
//                    new XAttribute("mainavg", (distanceTotal.MainParamPointSum / distanceTotal.RatingCounts.Sum()).ToString("0.0", nfi)),
//                    new XAttribute("addavg", ((distanceTotal.MainParamPointSum + distanceTotal.AddParamPointSum) / distanceTotal.RatingCounts.Sum()).ToString("0.0", nfi)),

//                    new XAttribute("ns", distanceTotal.GetSectorQualitiveRating().Split('/')[0]),
//                    new XAttribute("ratings", distanceTotal.GetSectorQualitiveRating().Split('/')[1]));

//                for (int i = 0; i < 4; i++)
//                {
//                    byKilometer.Add(new XElement("countbytype",
//                        new XAttribute("degree", (i == 0 ? "I" : i == 1 ? "II" : i == 2 ? "III" : i == 3 ? "IV" : "")),
//                        new XAttribute("const", distanceTotal.Constriction.Degrees[i] > 0 ? $"{distanceTotal.Constriction.Degrees[i]}" : ""),
//                        new XAttribute("broad", distanceTotal.Broadening.Degrees[i] > 0 ? $"{distanceTotal.Broadening.Degrees[i]}" : ""),
//                        new XAttribute("level", distanceTotal.Level.Degrees[i] > 0 ? $"{distanceTotal.Level.Degrees[i]}" : ""),
//                        new XAttribute("sag", distanceTotal.Sag.Degrees[i] > 0 ? $"{distanceTotal.Sag.Degrees[i]}" : ""),
//                        new XAttribute("down", distanceTotal.Drawdown.Degrees[i] > 0 ? $"{distanceTotal.Drawdown.Degrees[i]}" : ""),
//                        new XAttribute("stright", distanceTotal.Strightening.Degrees[i] > 0 ? $"{distanceTotal.Strightening.Degrees[i]}" : ""),

//                        new XAttribute("sum", (distanceTotal.Constriction.Degrees[i] + distanceTotal.Broadening.Degrees[i] +
//                                                distanceTotal.Level.Degrees[i] + distanceTotal.Sag.Degrees[i] +
//                                                distanceTotal.Strightening.Degrees[i] + distanceTotal.Drawdown.Degrees[i]))));
//                }

//                byKilometer.Add(new XElement("countbytype",
//                    new XAttribute("combination", distanceTotal.Combination),
//                    new XAttribute("other", distanceTotal.Other),
//                    new XAttribute("curves", distanceTotal.Curves),
//                    new XAttribute("additional", distanceTotal.Additional),
//                    new XAttribute("sum", distanceTotal.Combination + distanceTotal.Other + distanceTotal.Curves + distanceTotal.Additional)));

//                var constriction = distanceTotal.Constriction.Degrees.Sum();
//                var broadening = distanceTotal.Broadening.Degrees.Sum();
//                var level = distanceTotal.Level.Degrees.Sum();
//                var sag = distanceTotal.Sag.Degrees.Sum();
//                var drawdown = distanceTotal.Drawdown.Degrees.Sum();
//                var strightening = distanceTotal.Strightening.Degrees.Sum();
//                var sum = distanceTotal.Combination + distanceTotal.Other + distanceTotal.Curves + distanceTotal.Additional
//                        + constriction + broadening + level + sag + drawdown + strightening;

//                byKilometer.Add(new XElement("countbytype",
//                        new XAttribute("degree", "Итого"),
//                        new XAttribute("const", constriction),
//                        new XAttribute("broad", broadening),
//                        new XAttribute("level", level),
//                        new XAttribute("sag", sag),
//                        new XAttribute("down", drawdown),
//                        new XAttribute("stright", strightening),
//                        new XAttribute("combination", distanceTotal.Combination),
//                        new XAttribute("other", distanceTotal.Other),
//                        new XAttribute("curves", distanceTotal.Curves),
//                        new XAttribute("additional", distanceTotal.Additional),

//                        new XAttribute("sum", sum)));

//                byKilometer.Add(new XElement("countbytype",
//                        new XAttribute("degree", "%"),
//                        new XAttribute("const", GetPercentage(constriction, sum)),
//                        new XAttribute("broad", GetPercentage(broadening, sum)),
//                        new XAttribute("level", GetPercentage(level, sum)),
//                        new XAttribute("sag", GetPercentage(sag, sum)),
//                        new XAttribute("down", GetPercentage(drawdown, sum)),
//                        new XAttribute("stright", GetPercentage(strightening, sum)),
//                        new XAttribute("combination", GetPercentage(distanceTotal.Combination, sum)),
//                        new XAttribute("other", GetPercentage(distanceTotal.Other, sum)),
//                        new XAttribute("curves", GetPercentage(distanceTotal.Curves, sum)),
//                        new XAttribute("additional", GetPercentage(distanceTotal.Additional, sum)),
//                        new XAttribute("sum", 100)));

//                report.Add(byKilometer,
//                    new XAttribute("code", kilometers[0].Direction_code),
//                    new XAttribute("track", kilometers[0].Track_name),
//                    new XAttribute("name", kilometers[0].Direction_name),
//                    new XAttribute("pch", distance.Code));

//                xdReport.Add(report);
//                XslCompiledTransform transform = new XslCompiledTransform();
//                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
//                transform.Transform(xdReport.CreateReader(), writer);
//            }
//            try
//            {
//                htReport.Save(Path.GetTempPath() + "/report-pu32.html");
//            }
//            catch
//            {
//                MessageBox.Show("Ошибка сохранения файлы");
//            }
//            finally
//            {
//                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report-pu32.html");
//            }

//            void SectionTotalGenerate(ref XElement sectionElement, ref Total sectionTotal, ref XElement distanceElement, ref Total distanceTotal,
//                string code, string directionCode, string directionName)
//            {
//                sectionElement.Add(
//                    new XAttribute("len", sectionTotal.Length.ToString("0.000", nfi)),
//                    new XAttribute("excellent", sectionTotal.RatingCounts[0].ToString("0.000", nfi)),
//                    new XAttribute("good", sectionTotal.RatingCounts[1].ToString("0.000", nfi)),
//                    new XAttribute("satisfactory", sectionTotal.RatingCounts[2].ToString("0.000", nfi)),
//                    new XAttribute("bad", sectionTotal.RatingCounts[3].ToString("0.000", nfi)),
//                    new XAttribute("limit", sectionTotal.IsLimited),
//                    new XAttribute("d4", sectionTotal.Fourth),
//                    new XAttribute("other", (sectionTotal.Combination + sectionTotal.Curves + sectionTotal.Other)),
//                    new XAttribute("add", sectionTotal.Additional),
//                    new XAttribute("repair", sectionTotal.Repairing),
//                    new XAttribute("mainavg", (sectionTotal.MainParamPointSum / sectionTotal.RatingCounts.Sum()).ToString("0.0", nfi)),
//                    new XAttribute("addavg", ((sectionTotal.MainParamPointSum + sectionTotal.AddParamPointSum) / sectionTotal.RatingCounts.Sum()).ToString("0.0", nfi)),
//                    new XAttribute("ns", sectionTotal.GetSectorQualitiveRating().Split('/')[0]),
//                    new XAttribute("rating", sectionTotal.GetSectorQualitiveRating().Split('/')[1]));

//                distanceTotal += sectionTotal;
//                sectionTotal = new Total();
//                sectionTotal.Code = code;
//                sectionTotal.DirectionCode = directionCode;
//                sectionTotal.DirectionName = directionCode;

//                distanceElement.Add(sectionElement);
//                sectionElement = new XElement("section",
//                                    new XAttribute("name", directionName),
//                                    new XAttribute("code", directionCode),
//                                    new XAttribute("track", code));
//            }
//            string GetPercentage(int value, int total)
//            {
//                return (value * 100 / (double)total).ToString("0.0", nfi);
//            }

//            void PCHUTotalGenerate(ref XElement pchuElement, ref Total pchuTotal, ref XElement distanceElement, ref Total distanceTotal, string code, string chief)
//            {
//                distanceTotal += pchuTotal;
//                pchuTotal = new Total();
//                pchuTotal.Code = code;

//                distanceElement.Add(pchuElement);
//                pchuElement = new XElement("pchu",
//                               new XAttribute("pch", distanceTotal.Code),
//                               new XAttribute("code", code),
//                               new XAttribute("chief", chief));
//            }

//            void PDTotalGenerate(ref XElement pdElement, ref Total pdTotal, ref XElement pchuElement, ref Total pchuTotal, string code, string chief)
//            {
//                pdElement.Add(
//                    new XAttribute("len", pdTotal.Length.ToString("0", nfi)),
//                    new XAttribute("point", $"{pdTotal.MainParamPointSum}/{pdTotal.AddParamPointSum}"),
//                    new XAttribute("rating", pdTotal.GetSectorQualitiveRating()),
//                    new XAttribute("ratecount", $"Отл - {pdTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {pdTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {pdTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {pdTotal.RatingCounts[3].ToString("0", nfi)};        Средний балл - {(pdTotal.MainParamPointSum / pdTotal.RatingCounts.Sum()).ToString("0.0")}"),
//                    new XAttribute("c1", pdTotal.Constriction.ToString()),
//                    new XAttribute("c2", pdTotal.Broadening.ToString()),
//                    new XAttribute("c3", pdTotal.Level.ToString()),
//                    new XAttribute("c4", pdTotal.Sag.ToString()),
//                    new XAttribute("c5", pdTotal.Drawdown.ToString()),
//                    new XAttribute("c6", pdTotal.Strightening.ToString()),
//                    new XAttribute("c7", pdTotal.Common),
//                    new XAttribute("c8", pdTotal.FourthOtherAdd));

//                pchuTotal += pdTotal;
//                pdTotal = new Total();
//                pdTotal.Code = code;

//                pchuElement.Add(pdElement);

//                pdElement = new XElement("pd",
//                                new XAttribute("code", code),
//                                new XAttribute("chief", chief));
//            }
//            void PDBTotalGenerate(ref XElement pdbElement, ref Total pdbTotal, ref XElement pdElement, ref Total pdTotal, string code, string chief)
//            {
//                pdbElement.Add(
//                    new XAttribute("len", pdbTotal.Length.ToString("0", nfi)),
//                    new XAttribute("point", $"{pdbTotal.MainParamPointSum}/{pdbTotal.AddParamPointSum}"),
//                    new XAttribute("rating", pdbTotal.GetSectorQualitiveRating()),
//                    new XAttribute("ratecount", $"Отл - {pdbTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {pdbTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {pdbTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {pdbTotal.RatingCounts[3].ToString("0", nfi)};        Средний балл - {(pdbTotal.MainParamPointSum / pdbTotal.RatingCounts.Sum()).ToString("0.0")}"));

//                pdTotal += pdbTotal;
//                pdbTotal = new Total();
//                pdbTotal.Code = code;

//                pdElement.Add(pdbElement);

//                pdbElement = new XElement("pdb", new XAttribute("code", code), new XAttribute("chief", chief));
//            }
//        }
//    }
//    public class Total
//    {
//        public string Code { get; set; }
//        public string DirectionCode { get; set; }
//        public bool IsKM { get; set; } = false;
//        public TotalPart Constriction { get; set; } = new TotalPart();
//        public TotalPart Broadening { get; set; } = new TotalPart();
//        public TotalPart Level { get; set; } = new TotalPart();
//        public TotalPart Sag { get; set; } = new TotalPart();
//        public TotalPart Drawdown { get; set; } = new TotalPart();
//        public TotalPart Strightening { get; set; } = new TotalPart();
//        public int IsLimited { get; set; } = 0;
//        public int Repairing { get; set; } = 0;

//        public double[] RatingCounts { get; set; } = new double[] { 0, 0, 0, 0 };
//        public int Fourth { get; set; }
//        public int Other { get; set; }
//        public int Combination { get; set; }
//        public int Curves { get; set; }
//        public int MainParamPointSum { get; set; }
//        public int AddParamPointSum { get; set; }
//        //ToDo Tolegen
//        public int Additional { get; set; }

//        public TotalPart Common => Constriction + Broadening + Level + Sag + Drawdown + Strightening;
//        public static Total operator +(Total t1, Total t2)
//        {
//            if (t2.IsKM)
//            {
//                if (t2.QualitiveRating == Rating.О)
//                    t1.RatingCounts[0] += t2.Length;
//                if (t2.QualitiveRating == Rating.Х)
//                    t1.RatingCounts[1] += t2.Length;
//                if (t2.QualitiveRating == Rating.У)
//                    t1.RatingCounts[2] += t2.Length;
//                if (t2.QualitiveRating == Rating.Н)
//                    t1.RatingCounts[3] += t2.Length;
//            }
//            else
//            {
//                for (int i = 0; i < t1.RatingCounts.Length; i++)
//                {
//                    t1.RatingCounts[i] += t2.RatingCounts[i];

//                }

//            }
//            return new Total
//            {
//                Code = t1.Code,
//                IsLimited = t1.IsLimited + t2.IsLimited,
//                Constriction = t1.Constriction + t2.Constriction,
//                Strightening = t1.Strightening + t2.Strightening,
//                Broadening = t1.Broadening + t2.Broadening,
//                Level = t1.Level + t2.Level,
//                Sag = t1.Sag + t2.Sag,
//                Drawdown = t1.Drawdown + t2.Drawdown,
//                Fourth = t1.Fourth + t2.Fourth,
//                Other = t1.Other + t2.Other,
//                Additional = t1.Additional + t2.Additional,
//                Curves = t1.Curves + t2.Curves,
//                Length = t1.Length + t2.Length,
//                MainParamPointSum = t1.MainParamPointSum + t2.MainParamPointSum,
//                AddParamPointSum = t1.AddParamPointSum + t2.AddParamPointSum,
//                RatingCounts = t1.RatingCounts
//            };
//        }
//        public string FourthOtherAdd => $"{(Fourth > 0 ? Fourth.ToString() : "-")}/{(Other > 0 ? Other.ToString() : " - ")}/{(Additional > 0 ? Additional.ToString() : " - ")}";
//        //public string Fourth => $"{(Fourth > 0 ? Fourth.ToString() : "-")}";
//        //public string Other => $"{(Other > 0 ? Other.ToString() : " - ")}";
//        //public string Additional => $"{(Additional > 0 ? Additional.ToString() : " - ")}";


//        public Rating QualitiveRating { get; set; }
//        public double Length { get; set; }
//        public int CurvePointSum { get; internal set; }
//        public string DirectionName { get; internal set; }

//        public string GetSectorQualitiveRating()
//        {
//            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
//            var NSector = Math.Round((5 * RatingCounts[0] + 4 * RatingCounts[1] + 3 * RatingCounts[2] - 5 * RatingCounts[3]) / Length, 1);
//            QualitiveRating = NSector > 4.5 ? Rating.О : (NSector.Between(3.9, 4.5) ? Rating.Х : (NSector.Between(3.0, 3.8) ? Rating.У : Rating.Н));

//            //Қосымша тексеру керек
//            NSector = NSector < 0 ? 0 : NSector;

//            return $"{NSector.ToString("0.0", nfi)}/{QualitiveRating}";
//        }
//    }

//    public class TotalPart
//    {
//        public int[] Degrees { get; set; } = new int[] { 0, 0, 0, 0 };
//        public static TotalPart operator +(TotalPart t1, TotalPart t2)
//        {
//            return new TotalPart
//            {
//                Degrees = new int[]
//            {
//                t1.Degrees[0] + t2.Degrees[0],
//                t1.Degrees[1] + t2.Degrees[1],
//                t1.Degrees[2] + t2.Degrees[2],
//                t1.Degrees[3] + t2.Degrees[3],
//            }
//            };
//        }
//        public override string ToString()
//        {
//            if (Degrees[1] == 0 && Degrees[2] == 0)
//                return "";
//            if (Degrees[1] == 0 && Degrees[2] != 0)
//                return $"/{Degrees[2]}";
//            if (Degrees[1] != 0 && Degrees[2] == 0)
//                return $"{Degrees[1]}/";
//            return $"{Degrees[1]}/{Degrees[2]}";
//        }
//    }
//}
