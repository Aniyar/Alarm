using ALARm.Core;
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
    public class Deviation_close_to_the_limit : ALARm.Core.Report.GraphicDiagrams
    {
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
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
            this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                List<Curve> curves = (MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);
                distance.Name = distance.Name.Replace("ПЧ-", "");

                var tripProcesses = RdStructureService.GetTripsOnDistance(parentId, period);
              
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

              
            

                        XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trips = RdStructureService.GetTrips();
                        var tr = trips.Where(t => t.Id == tripProcess.Trip_id).ToList();
                        if (tr.Any()) continue;


                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);
                        //tripp.Track_Id = track_id;
                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();
                        //var kilometers = RdStructureService.GetKilometersByTrip(trip);

                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var lkm = kilometers.Select(o => o.Number).ToList();
                        if (!lkm.Any()) continue;
                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        kilometers = kilometers.Where(Km => ((float)(float)filters[0].Value <= Km.Number && Km.Number <= (float)(float)filters[1].Value)).ToList();
                        kilometers = (trip.Travel_Direction == Direction.Reverse ? kilometers.OrderBy(o => o.Number) : kilometers.OrderByDescending(o => o.Number)).ToList();
                        //--------------------------------------------
                        int constrictionCount = 0; //Суж
                        int broadeningCount = 0; //Уш
                        int levelCount = 0; //У
                        int skewnessCount = 0; //П - просадка
                        int drawdownCount = 0; //Пр - перекос
                        int straighteningCount = 0; //Р
                        int slopeCount = 0;//Укл
                        int PMCount = 0;//П м
                        int IBLCount = 0;//Иб.л
                        int anpCount = 0;//Анп
                        int RNRCount = 0;//Рнр
                        int RPlusPCount = 0;//Р+П
                        int RNRPlusPCount = 0;//Рнр+П
                        int RNRSTCount = 0;//Рнрст
                        int ThirdStepenCount = 0;//3ст



                        XElement tripElem = new XElement("trip",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("direction", kilometers[0].Direction_name),
                            new XAttribute("check", trip.GetProcessTypeName),
                            new XAttribute("track", kilometers[0].Track_name),
                            new XAttribute("road", road),
                            new XAttribute("distance", distance.Code),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("chief", trip.Chief),
                            new XAttribute("ps", trip.Car));

                        // запрос ОСнов параметров с бд
                        var ListS3 = RdStructureService.GetS3(kilometers.First().Trip.Id) as List<S3>; //пру


                        //запрос доп параметров с бд
                        var AddParam = AdditionalParametersService.GetAddParam(kilometers.First().Trip.Id) as List<S3>; //износы
                        if (AddParam == null)
                        {
                            MessageBox.Show("Не удалось сформировать отчет, так как возникала ошибка во время загрузки данных по доп параметрам");
                            return;
                        }

                        XElement xeDirection = new XElement("directions");
                        XElement xeTracks = new XElement("tracks");

                        var Itog = 0;

                        foreach (var km in kilometers)
                        {

                            var PRUbyKmMAIN = ListS3.Where(o => o.Ovp != -1 && o.Ovp != 0 && o.Ogp != -1 && o.Ogp != 0 && o.Km == km.Number).ToList();
                            var PRUbyKmADD = AddParam.Where(o => o.Ovp != -1 && o.Ovp != 0 && o.Ogp != 1 && o.Ogp != 0 && o.Km == km.Number).ToList();


                            //PRUbyKmMAIN.AddRange(PRUbyKmADD);

                            //PRUbyKmMAIN.OrderBy(o => o.Km).ThenBy(o => o.Meter).ToList();




                            km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);


                            foreach (var s3 in PRUbyKmMAIN)
                            {
                                switch (s3.Ots)
                                {
                                    case "Пр.п":
                                    case "Пр.л":
                                        drawdownCount += s3.Kol;
                                        break;
                                    case "Анп":
                                        anpCount += s3.Kol;
                                        break;
                                    case "Суж":
                                        constrictionCount += s3.Kol;
                                        break;
                                    case "Уш":
                                        broadeningCount += s3.Kol;
                                        break;
                                    case "У":
                                        levelCount += s3.Kol;
                                        break;
                                    case "П":
                                        skewnessCount += s3.Kol;
                                        break;
                                    case "Р":
                                        straighteningCount += s3.Kol;
                                        break;
                                    case "Укл":
                                        slopeCount += s3.Kol;
                                        break;
                                    case "П м":
                                        PMCount += s3.Kol;
                                        break;
                                    case "Иб.л":
                                        IBLCount += s3.Kol;
                                        break;

                                    case "Рнр":
                                        RNRCount += s3.Kol;
                                        break;
                                    case "Р+П":
                                        RPlusPCount += s3.Kol;
                                        break;
                                    case "Рнр+П":
                                        RNRPlusPCount += s3.Kol;
                                        break;
                                    case "Рнрст":
                                        RNRSTCount += s3.Kol;
                                        break;
                                    case "3ст":
                                        ThirdStepenCount += s3.Kol;
                                        break;

                                }

                                XElement xeMain = new XElement("main",
                                      new XAttribute("track", s3.Put),
                                    new XAttribute("km", s3.Km),
                                    new XAttribute("m", s3.Meter),
                                    new XAttribute("Data", s3.TripDateTime.ToString("dd.MM.yyyy")),
                                    new XAttribute("Ots", s3.Ots),
                                    new XAttribute("Otkl", s3.Otkl.ToString("0.0")),
                                    new XAttribute("len", s3.Len),
                                    new XAttribute("Stepen", s3.Typ.ToString() == "5" ? "-" : s3.Typ.ToString()),
                                    new XAttribute("vpz", s3.Uv + "/" + s3.Uvg),
                                    new XAttribute("vogr", s3.Ovp + "/" + s3.Ogp),
                                    new XAttribute("Primech", s3.Primech.ToString() == "м;" ? "мост" : s3.Primech.ToString()));


                                xeTracks.Add(xeMain);
                                //xeTracks.Add(new XAttribute("track", s3.Put));
                                //        xeTracks.Add(new XAttribute("countDistance", drawdownCount + constrictionCount + broadeningCount + levelCount + skewnessCount + straighteningCount));


                            }
                            foreach (var s3 in PRUbyKmADD)
                            {
                                switch (s3.Ots)
                                {

                                    case "Иб.л":
                                        IBLCount += s3.Kol;
                                        break;
                                }

                                XElement xeAdd = new XElement("add",
                                      new XAttribute("track", s3.Put),
                                    new XAttribute("km", s3.Km),
                                    new XAttribute("m", s3.Meter),
                                    new XAttribute("Data", trip.Trip_date.ToString("dd.MM.yyyy")),
                                    new XAttribute("Ots", s3.Ots),
                                    new XAttribute("Otkl", s3.Otkl.ToString("0.0")),
                                    new XAttribute("len", s3.Len),
                                    new XAttribute("vpz", km.Speeds.Last().Passenger + "/" + km.Speeds.Last().Freight),
                                    new XAttribute("vogr", s3.Ovp + "/" + s3.Ogp),
                                    new XAttribute("Primech", ""));

                                xeTracks.Add(xeAdd);
                                xeTracks.Add(new XAttribute("track", s3.Put));
                                Itog += 1;
                            }


                        }


                        Itog += drawdownCount + constrictionCount + broadeningCount + levelCount + skewnessCount + straighteningCount + anpCount + RNRCount + RPlusPCount + RNRPlusPCount + RNRSTCount + ThirdStepenCount;

                        //В том числе:
                        if (drawdownCount > 0)
                        {
                            tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Пр - " + drawdownCount)));
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

                        if (anpCount > 0)
                        {
                            tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Анп - " + anpCount)));
                        }
                        if (RNRCount > 0)
                        {
                            tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Рнр - " + RNRCount)));
                        }
                        if (RPlusPCount > 0)
                        {
                            tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Р+П - " + RPlusPCount)));
                        }
                        if (RNRPlusPCount > 0)
                        {
                            tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Рнр+П - " + RNRPlusPCount)));
                        }
                        if (RNRSTCount > 0)
                        {
                            tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Рнрст - " + RNRSTCount)));
                        }
                        if (ThirdStepenCount > 0)
                        {
                            tripElem.Add(new XElement("total", new XAttribute("totalinfo", "3ст - " + ThirdStepenCount)));
                        }


                        xeTracks.Add(new XAttribute("countDistance", drawdownCount + constrictionCount + broadeningCount + levelCount + skewnessCount + straighteningCount + slopeCount + PMCount + IBLCount + anpCount + RNRCount + RPlusPCount + RNRPlusPCount + RNRSTCount + ThirdStepenCount));
                        xeTracks.Add(new XAttribute("track", kilometers[0].Track_name));
                        xeDirection.Add(xeTracks);
                        tripElem.Add(xeDirection);
                        report.Add(tripElem);
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
    }
}
