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

                //var tripProcesses = RdStructureService.GetTripsOnDistance(parentId, period);

                //if (tripProcesses.Count == 0)
                //{
                //    MessageBox.Show(Properties.Resources.paramDataMissing);
                //    return;
                //}
                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }




                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {

          
                    int skewnessCount = 0; //П - просадка
                    int drawdownCount = 0; //Пр - перекос
                
                 
                    int PMCount = 0;//П м
               
                    int anpCount = 0;//Анп
                    int anpquestiomCount = 0; // ?Анп
                    int uklonvopros = 0;
                    int otklonvopros = 0;
                    int uklonobr = 0;
                    int uklon150vopros = 0;
                    int uklon75vopros = 0;
                    int Zazorvopros = 0;
                    int IBLCount = 0;

                    XElement tripElem = new XElement("trip",
                          new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),

                          new XAttribute("check", tripProcess.GetProcessTypeName),

                          new XAttribute("road", road),
                          new XAttribute("distance", distance.Code),
                          new XAttribute("periodDate", period.Period),
                          new XAttribute("chief", tripProcess.Chief),
                          new XAttribute("ps", tripProcess.Car));

                    var Itog = 0;
                    bool founddigression = false;
                    foreach (var track_id in admTracksId)
                    {
                        var trips = RdStructureService.GetTrips();
                        var tr = trips.Where(t => t.Id == tripProcess.Trip_id).ToList();
                        if (!tr.Any()) continue;

                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);
                        //tripp.Track_Id = track_id;
                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();

                        //tripElem.Add(new XAttribute("direction", kilometers[0].Direction));
                        //tripElem.Add(new XAttribute("track", kilometers[0].Track_name));
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





                        // запрос ОСнов параметров с бд



                        //запрос доп параметров с бд
                        var AddParam = AdditionalParametersService.GetAddParam(kilometers.First().Trip.Id) as List<S3>; //износы
                        if (AddParam == null)
                        {
                            MessageBox.Show("Не удалось сформировать отчет, так как возникала ошибка во время загрузки данных по доп параметрам");
                            return;
                        }

                        //XElement xeDirection = new XElement("directions");
                        //XElement xeTracks = new XElement("tracks");

                        List<DigressionTotal> totals = new List<DigressionTotal>();
                        DigressionTotal digressionTotal = new DigressionTotal();
                        XElement xeDirection = new XElement("directions");
                        XElement xeTracks = new XElement("tracks");
                        XElement xeTrackscount = new XElement("trackscount");
                        foreach (var km in kilometers)
                        {

                            if (km.Number == 707)
                            {
                                km.Number = km.Number;
                            }
                            var ListS3 = RdStructureService.GetS3(kilometers.First().Trip.Id) as List<S3>; //пру
                            if (ListS3 != null && ListS3.Count > 0)
                            {
                                founddigression = true;
                            }
                            ListS3 = ListS3.Where(o => o.Km == km.Number).ToList();
                            var closeToDanger = new List<string>() { DigressionName.RampNear.Name, DigressionName.IzoGapNear.Name, DigressionName.SpeedUpNear.Name, DigressionName.PatternRetractionNear.Name,
                                DigressionName.LevelReverse.Name, DigressionName.Level150.Name, DigressionName.Level75.Name, DigressionName.GapSimbol.Name };
                            //var PRUbyKmMAIN = ListS3.Where(o => (o.Ovp != -1 && o.Ovp != 0 && o.Ogp != -1 && o.Ogp != 0 && o.Km == km.Number)).ToList();
                            //var PRUbyKmADD = AddParam.Where(o => (o.Ovp != -1 && o.Ovp != 0 && o.Ogp != 1 && o.Ogp != 0 && o.Km == km.Number)).ToList();


                            var PRUbyKmMAIN = ListS3.Where(o => closeToDanger.Contains(o.Ots)).ToList();
                            PRUbyKmMAIN.AddRange(ListS3.Where(o => o.Primech.Contains("ис?")).ToList());
                            //PRUbyKmMAIN.AddRange(ListS3.Where(o => o.Primech.Contains("З?")).ToList());

                            var PRUbyKmADD = AddParam.Where(o => o.Km == km.Number && (closeToDanger.Contains(o.Ots) || closeToDanger.Contains(o.Primech))).ToList();


                            //PRUbyKmMAIN.AddRange(PRUbyKmADD);

                            //PRUbyKmMAIN.OrderBy(o => o.Km).ThenBy(o => o.Meter).ToList();




                            km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);


                            foreach (var s3 in PRUbyKmMAIN)
                            {
                                switch (s3.Ots)
                                {
                                    case "Пр.п":
                                    case "Пр.л":
                                        drawdownCount += 1;
                                        break;
                                  
                                    case "?Анп":
                                        anpquestiomCount += 1;
                                        break;
                                    case "?Укл":
                                        uklonvopros += 1;
                                        break;
                                    case "ОШК?":
                                        otklonvopros += 1;
                                        break;
                                    case "Уобр":
                                        uklonobr += 1;
                                        break;
                                    case "?У150":
                                        break;
                                        uklon150vopros += 1;
                                    case "У75":
                                        uklon75vopros += 1;
                                        break;
                                    case "З?":
                                        Zazorvopros += 1;
                                        break;


                                }

                                XElement xeMain = new XElement("main",
                                      new XAttribute("track", s3.Put),
                                    new XAttribute("km", s3.Km),
                                    new XAttribute("m", s3.Meter),
                                    new XAttribute("Data", s3.TripDateTime.ToString("dd.MM.yyyy")),
                                    new XAttribute("Ots", s3.Ots),
                                    new XAttribute("Otkl", s3.Ots == "?Анп" ? s3.Primech : s3.Otkl.ToString("0.0")),
                                    new XAttribute("len", s3.Len),
                                    new XAttribute("Stepen", s3.Typ.ToString() == "5" ? "-" : s3.Typ.ToString()),
                                    new XAttribute("vpz", s3.Uv + "/" + s3.Uvg),
                                    new XAttribute("vogr", s3.Ovp + "/" + s3.Ogp),
                                    new XAttribute("Primech", s3.Primech.ToString() == "м;" ? "мост" : ""));

                                if (totals.Any(t => t.Name == s3.Ots))
                                {
                                    totals.Where(t => t.Name == s3.Ots).First().Count += 1;
                                }
                                else
                                {
                                    digressionTotal.Name = s3.Ots;
                                    digressionTotal.Count += 1;
                                    totals.Add(digressionTotal);
                                }

                               
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

                             


                                xeTracks.Add(xeAdd);
                                xeTracks.Add(new XAttribute("track", s3.Put));
                                Itog += s3.Kol;
                            }


                        }




                     
                        xeTracks.Add(new XAttribute("direction",tripProcess.DirectionName ));
                        xeTracks.Add(new XAttribute("countbyput", digressionTotal.Count));
                        xeTracks.Add(new XAttribute("distance", distance.Code));
                        xeTracks.Add(new XAttribute("track", kilometers[0].Track_name));
                        xeDirection.Add(xeTracks);
                        tripElem.Add(xeDirection);

                    }
                    Itog += drawdownCount +  skewnessCount +  anpCount ;

                    //В том числе:
                    if (drawdownCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Пр - " + drawdownCount)));
                    }
                  
                    if (skewnessCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "П - " + skewnessCount)));
                    }
                 
                    if (PMCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "П м - " + PMCount)));
                    }
                    if (IBLCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Ибл - " + IBLCount)));
                    }

                    
                    if (anpCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Анп - " + anpCount)));
                    }
                    if (anpquestiomCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("totalinfo", "?Анп - " + anpquestiomCount)));
                    }
                 
                    tripElem.Add(new XAttribute("countDistance", drawdownCount + anpquestiomCount + IBLCount+  skewnessCount +  PMCount +  anpCount));
                    if (founddigression == true)
                    {
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
                //htReport.Save($@"G:\form\G:\form\1.Основные и дополнительные параметры геометрии рельсовой колеи (ГРК)\Ведомость «Отступления близкие к предельным».pdf");
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка сохранения файлы" + e);
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report.html");
            }
        }
    }
}
