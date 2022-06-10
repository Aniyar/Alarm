using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm_Report.Forms
{
    public class speed_limits : ALARm.Core.Report.GraphicDiagrams
    {
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {

            this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();
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
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                List<Curve> curves = (MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);
                distance.Name = distance.Name.Replace("ПЧ-", "");

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {

                    List<Digression> notes = RdStructureService.GetDigressions(tripProcess.Trip_id, distance.Code, new int[] { 2 });
                    var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;

                    CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;

                    XElement tripElem = new XElement("trip",
                       new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                       new XAttribute("direction", curvesAdmUnit.Direction),
                       new XAttribute("check", tripProcess.GetProcessTypeName),
                       new XAttribute("track", curvesAdmUnit.Track),
                       new XAttribute("road", road),
                       new XAttribute("distance", distance.Code),
                       new XAttribute("periodDate", period.Period),
                       new XAttribute("chief", tripProcess.Chief),
                       new XAttribute("ps", tripProcess.Car));

                    var ItogADD = 0;
                    var ItogMain = 0;

                    int constrictionCount = 0; //Суж
                    int broadeningCount = 0; //Уш
                    int levelCount = 0; //У
                    int skewnessCount = 0; //П - просадка
                    int drawdownCount = 0; //Пр - перекос
                    int straighteningCount = 0; //Р
                    int straighteningRNRCount = 0; //Рнр
                    int slopeCount = 0;//Укл
                    int PMCount = 0;//П м
                    int IBLCount = 0;//Иб.л
                    int outstandingaccelerationcount = 0;//анп
                    int outstandingaccelerationcountoutstandingaccelerationcount = 0;//?Анп
                    int gapCount = 0;//зазор


                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);
                        if (!kilometers.Any()) continue;



                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();
                        if (kilometers.Count == 0) continue;
                        trip.Track_Id = track_id;
                        var lkm = kilometers.Select(o => o.Number).Distinct().ToList();

                        if (lkm.Count() == 0) continue;



                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();


                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        lkm = lkm.Where(o => ((float)(float)filters[0].Value <= o && o <= (float)(float)filters[1].Value)).ToList();
                        kilometers = kilometers.Where(Km => ((float)(float)filters[0].Value <= Km.Number && Km.Number <= (float)(float)filters[1].Value)).ToList();
                        //kilometers = (trip.Travel_Direction == Direction.Reverse ? kilometers.OrderBy(o => o.Number) : kilometers.OrderByDescending(o => o.Number)).ToList();
                        //--------------------------------------------

                   

                        List<Gap> check_gap_state = AdditionalParametersService.Check_gap_state(kilometers.First().Trip.Id, template.ID); //стыки
                                                                                                                                          // запрос ОСнов параметров с бд

                        var ListS3 = RdStructureService.GetS3(kilometers.First().Trip.Id) as List<S3>; //пру
                        //var ListS3 = RdStructureService.GetDigressionMarks(kilometers.First().Trip.Id, kilometers.First().Track_id, kilometers.First().nb) as List<S3>; //пру
                        var PRU = ListS3.Where(o => o.Ots == "ПрУ").ToList();
                        var gapV = check_gap_state.Where(o => o.Vdop != "" && o.Vdop != "-/-").ToList();
                        var Pu32_gap = check_gap_state;

                        //запрос доп параметров с бд
                        var AddParam = AdditionalParametersService.GetAddParam(kilometers.First().Trip.Id) as List<S3>; //износы
                        if (AddParam == null)
                        {
                            MessageBox.Show("Не удалось сформировать отчет, так как возникала ошибка во время загрузки данных по доп параметрам");
                            return;
                        }
                        List<DigressionTotal> totals = new List<DigressionTotal>();
                        DigressionTotal digressionTotal = new DigressionTotal();
                        XElement xeDirection = new XElement("directions");
                        XElement xeTracks = new XElement("tracks");
                        XElement xeTrackscount = new XElement("trackscount");

                        
                        foreach (var km in kilometers)
                        {

                            km.Digressions = RdStructureService.GetDigressionMarks(km.Trip.Id, km.Track_id, km.Number);
                            var kmTotal = new Total();
                            var add_dig_str = "";
                            var prevCurve_id = -1;
                            var Curve_dig_str = "";
                            var PassengerSpeedLimit = "";
                            var gap_dig = AddParam.Where(o => o.Km == km.Number).ToList();


                            //данные Износа рельса Бок.из.
                            //var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(km.Number, km.Trip_id);
                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(km.Number, km.Trip.Id);
                            if (DBcrossRailProfile == null) continue;

                            var sortedData = DBcrossRailProfile.OrderByDescending(d => d.Meter).ToList();
                            var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(sortedData);
                            List<Digression> addDigressions = crossRailProfile.GetDigressions();

                            var dignatur = new List<DigressionMark> { };
                            foreach (var dig in addDigressions)
                            {
                                int count = dig.Length / 4;
                                count += dig.Length % 4 > 0 ? 1 : 0;

                                if (dig.DigName == DigressionName.SideWearLeft || dig.DigName == DigressionName.SideWearRight)
                                {
                                    dignatur.Add(new DigressionMark()
                                    {
                                        Digression = dig.DigName,
                                        NotMoveAlert = false,
                                        Meter = dig.Meter,
                                        finish_meter = dig.Kmetr,
                                        Degree = (int)dig.Typ,
                                        Length = dig.Length,
                                        Value = dig.Value,
                                        Count = count,
                                        DigName = dig.GetName(),
                                        PassengerSpeedLimit = -1,
                                        FreightSpeedLimit = -1,
                                        Comment = "",
                                        Diagram_type = "Iznos_relsov",
                                        Digtype = DigressionType.Additional
                                    });
                                }
                            }
                            //выч-е скорости бок износа
                            int pas = 999, gruz = 999;
                            foreach (DigressionMark item in dignatur)
                            {
                                if (item.Digression == DigressionName.SideWearLeft || item.Digression == DigressionName.SideWearRight)
                                {
                                    var c = km.Curves.Where(o => o.RealStartCoordinate <= km.Number + item.Meter / 10000.0 && o.RealFinalCoordinate >= km.Number + item.Meter / 10000.0).ToList();

                                    if (c.Any())
                                    {
                                        item.GetAllowSpeedAddParam(km.Speeds.First(), c.First().Straightenings[0].Radius, item.Value);

                                        if (item.PassengerSpeedLimit != -1 && item.PassengerSpeedLimit < pas)
                                        {
                                            pas = item.PassengerSpeedLimit;
                                        }
                                        if (item.FreightSpeedLimit != -1 && item.FreightSpeedLimit < gruz)
                                        {
                                            gruz = item.FreightSpeedLimit;
                                        }
                                    }
                                }
                                else if (item.Digression == DigressionName.Gap)
                                {
                                    if (item.PassengerSpeedLimit != -1 && item.PassengerSpeedLimit < pas)
                                    {
                                        pas = item.PassengerSpeedLimit;
                                    }
                                    if (item.FreightSpeedLimit != -1 && item.FreightSpeedLimit < gruz)
                                    {
                                        gruz = item.FreightSpeedLimit;
                                    }
                                }

                            }


                            foreach (var item in gap_dig)
                            {
                                km.Digressions.Add(new DigressionMark
                                {
                                    Km = item.Km,
                                    Meter = item.Meter,
                                    Length = item.Len,
                                    DigName = item.Ots,
                                    Count = item.Kol,
                                    Value = item.Otkl,
                                    PassengerSpeedLimit = item.Ogp,
                                    FreightSpeedLimit = item.Ogp,
                                    Digtype = DigressionType.Additional
                                });
                            }


                            var PRUbyKmMAIN = ListS3.Where(o => (o.Ovp != -1 && o.Ogp != -1 || (o.Primech == "гр")) && o.Km == km.Number).ToList();
                            PRUbyKmMAIN = PRUbyKmMAIN.Where(o => (o.Ovp != 0 && o.Ogp != 0 || (o.Ovp == 0 && o.Ogp == 0 && o.Typ == 4)) && o.Km == km.Number).ToList();

                            var PRUbyKmADD = Pu32_gap.Where(o => o.Otst == "З" && o.Km == km.Number).ToList();

                            km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);

                            foreach (var s3 in PRUbyKmMAIN)
                            {

                                if (s3.Put == null) continue;
                                if (s3.Primech.Contains("Натурная кривая")) continue;
                                switch (s3.Ots)
                                {
                                    case "Пр.п":
                                    case "Пр.л":
                                        drawdownCount += 1;
                                        break;
                                    case "Анп":
                                        outstandingaccelerationcount += 1;
                                        break;
                                    case "?Анп":
                                        outstandingaccelerationcountoutstandingaccelerationcount += 1;
                                        break;
                                    case "Суж":
                                        constrictionCount += 1;
                                        break;
                                    case "Уш":
                                        broadeningCount += 1;
                                        break;
                                    case "У":
                                        levelCount += 1;
                                        break;
                                    case "П":
                                        skewnessCount += 1;
                                        break;
                                    case "Р":
                                        straighteningCount += 1;
                                        break;
                                    case "Укл":
                                        slopeCount += 1;
                                        break;
                                    case "П м":
                                        PMCount += 1;
                                        break;
                                    case "Иб.л":
                                        IBLCount += 1;
                                        break;
                                    case "Рнр":
                                        straighteningRNRCount += 1;
                                        break;

                                }
                                if (s3.Ots == "Анп")
                                {
                                    var otkl = "";
                                    if (s3.Primech.Any())
                                    {
                                        try
                                        {
                                            otkl = s3.Primech.Split().First().Split(':').Last();
                                        }
                                        catch
                                        {
                                            otkl = "ошибка при разделении";
                                        }
                                    }
                                    XElement xeMain = new XElement("main",
                                        new XAttribute("track", s3.Put),
                                        new XAttribute("km", km.Number),
                                        new XAttribute("m", s3.Meter),
                                        new XAttribute("Data", s3.TripDateTime.ToString("dd.MM.yyyy")),
                                        new XAttribute("Ots", s3.Ots),
                                        new XAttribute("Otkl", otkl.ToString()),
                                        new XAttribute("len", s3.Len),
                                        new XAttribute("Stepen", "-"),
                                        new XAttribute("vpz", (s3.Uv.ToString() == "-1" ? "-" : s3.Uv.ToString()) + "/" + (s3.Uvg.ToString() == "-1" ? "-" : s3.Uvg.ToString())),
                                        new XAttribute("vogr", (s3.Ovp.ToString() == "-1" ? "-" : s3.Ovp.ToString()) + "/" + (s3.Ogp.ToString() == "-1" ? "-" : s3.Ogp.ToString())),
                                        new XAttribute("Primech", s3.Primech.ToString() == "м;" ? "мост" : ""));
                                    if (totals.Any(t => t.Name == s3.Ots))
                                    {
                                        totals.Where(t => t.Name == s3.Ots).First().Count += s3.Kol;
                                    }
                                    else
                                    {
                                        digressionTotal.Name = s3.Ots;
                                        digressionTotal.Count += s3.Kol;
                                        totals.Add(digressionTotal);
                                    }

                                    xeTracks.Add(xeMain);

                                }
                                else
                                {

                                    XElement xeMain = new XElement("main",
                                     new XAttribute("track", s3.Put),
                                     new XAttribute("km", km.Number),
                                     new XAttribute("m", s3.Meter),
                                     new XAttribute("Data", s3.TripDateTime.ToString("dd.MM.yyyy")),
                                     new XAttribute("Ots", s3.Ots),
                                     new XAttribute("Otkl", s3.Ots == "Аг" || s3.Ots == "Анп" || s3.Ots == "Пси" || s3.Ots == "ОШК" || s3.Ots == "ОШП" || s3.Ots == "Укл" ? s3.Otkl.ToString("0.00") : s3.Otkl.ToString()),
                                     new XAttribute("len", s3.Len),
                                     new XAttribute("Stepen", s3.Typ.ToString() == "5" ? "-" : s3.Typ.ToString()),
                                     new XAttribute("vpz", (s3.Uv.ToString() == "-1" ? "-" : s3.Uv.ToString()) + "/" + (s3.Uvg.ToString() == "-1" ? "-" : s3.Uvg.ToString())),
                                     new XAttribute("vogr", (s3.Ovp.ToString() == "-1" ? "-" : s3.Ovp.ToString()) + "/" + (s3.Ogp.ToString() == "-1" ? "-" : s3.Ogp.ToString())),
                                     new XAttribute("Primech", s3.Primech == "м;" ? "мост" : s3.Primech == "гр" ? "гр" : ""));
                                    if (totals.Any(t => t.Name == s3.Ots))
                                    {
                                        totals.Where(t => t.Name == s3.Ots).First().Count += s3.Kol;
                                    }
                                    else
                                    {
                                        digressionTotal.Name = s3.Ots;
                                        digressionTotal.Count += s3.Kol;
                                        totals.Add(digressionTotal);
                                    }

                                    xeTracks.Add(xeMain);

                                }
                            }
                            ItogMain += PRUbyKmMAIN.Count;

                            foreach (var s3 in PRUbyKmADD)
                            {
                                switch (s3.Otst)
                                {
                                    case "З":
                                        gapCount += 1;
                                        break;

                                    case "Анп":
                                        outstandingaccelerationcount += 1;
                                        break;
                                    case "?Анп":
                                        outstandingaccelerationcountoutstandingaccelerationcount += 1;
                                        break;


                                }


                                XElement xeAdd = new XElement("add",
                                    new XAttribute("track", s3.track_id),
                                    new XAttribute("km", s3.Km),
                                    new XAttribute("m", s3.Meter),
                                    new XAttribute("Data", trip.Trip_date.ToString("dd.MM.yyyy")),
                                    new XAttribute("Ots", s3.Otst),
                                    new XAttribute("Otkl", Math.Max(s3.Zazor, s3.R_zazor)),
                                    new XAttribute("len", "-"),
                                    new XAttribute("vpz", s3.Vpz),
                                    new XAttribute("vogr", s3.Vdop),
                                    new XAttribute("Primech", ""));
                                xeTracks.Add(xeAdd);
                                if (totals.Any(t => t.Name == s3.Otst))
                                {
                                    totals.Where(t => t.Name == s3.Otst).First().Count += s3.Kol;
                                }
                                else
                                {
                                    digressionTotal.Name = s3.Otst;
                                    digressionTotal.Count += s3.Kol;
                                    totals.Add(digressionTotal);
                                }

                              

                            }

                            ItogADD += PRUbyKmADD.Count();
                        }

                        //xeTracks.Add(new XAttribute("countbyput", digressionTotal.Count));
                        
                        xeTracks.Add(new XAttribute("track", trackName));
                        xeTracks.Add(new XAttribute("direction", curvesAdmUnit.Direction));
                        xeTracks.Add(new XAttribute("countbyput", digressionTotal.Count));
                        xeTracks.Add(new XAttribute("distance", distance.Code));
                        xeDirection.Add(xeTracks);
                        tripElem.Add(xeDirection);
                    }

                    //В том числе:
                    if (drawdownCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Пр - " + drawdownCount)));
                    }
                    if (outstandingaccelerationcount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Анп - " + outstandingaccelerationcount)));
                    }
                    if (outstandingaccelerationcountoutstandingaccelerationcount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "?Анп - " + outstandingaccelerationcountoutstandingaccelerationcount)));
                    }
                    if (constrictionCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Суж - " + constrictionCount)));
                    }
                    if (broadeningCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Уш - " + broadeningCount)));
                    }
                    if (levelCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "У - " + levelCount)));
                    }
                    if (skewnessCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "П - " + skewnessCount)));
                    }
                    if (straighteningCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Р - " + straighteningCount)));
                    }
                    if (slopeCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Укл - " + slopeCount)));
                    }
                    if (PMCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "П м - " + PMCount)));
                    }
                    if (IBLCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Иб.л - " + IBLCount)));
                    }
                    if (gapCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "З - " + gapCount)));
                    }
                    if (straighteningRNRCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", " Рнр- " + straighteningRNRCount)));
                    }



                    tripElem.Add(new XAttribute("countDistance", ItogMain + ItogADD));
                    report.Add(tripElem);

                }
                xdReport.Add(report);
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report.html");
                htReport.Save($@"G:\form\1.Основные и дополнительные параметры геометрии рельсовой колеи (ГРК)\6.Ведомость неисправностей, требующих ограничения скорости  по основным и дополнительным параметрам.html");
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
