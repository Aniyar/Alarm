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
    public class DN : ALARm.Core.Report.GraphicDiagrams
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
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);
                        if (!kilometers.Any()) continue;



                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();
                        if (kilometers.Count == 0) continue;
                        trip.Track_Id = track_id;
                        var lkm = kilometers.Select(o => o.Number).ToList();

                        if (lkm.Count() == 0) continue;
                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        lkm = lkm.Where(o => ((float)(float)filters[0].Value <= o && o <= (float)(float)filters[1].Value)).ToList();
                        

                   

                     

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

                        XElement tripElem = new XElement("trip",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            //new XAttribute("direction", kilometers[0].Direction_name),
                            new XAttribute("direction", trip.Direction),
                            new XAttribute("directioncode", trip.DirectionCode),
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
                            var PRUbyKmMAIN = ListS3.Where(o => (o.Ots == "Анп") && o.Km == km.Number).ToList();

                            foreach (var s3 in PRUbyKmMAIN)
                            {
                                switch (s3.Ots)
                                {
                                    case "Пр.п":
                                    case "Пр.л":
                                        drawdownCount += s3.Kol;
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
                                }

                                XElement xeMain = new XElement("main",

                                    new XAttribute("km", s3.Km),
                                    new XAttribute("m", s3.Meter),
                                    new XAttribute("vpz", s3.Uv + "/" + s3.Uvg),
                                    new XAttribute("nerov", s3.TripDateTime.ToString("dd.MM.yyyy")),
                                    new XAttribute("len", s3.Len),
                                    new XAttribute("vplane", s3.Otkl),
                                    new XAttribute("vprofile", s3.Otkl),
                                    new XAttribute("vogr", s3.Ovp + "/" + s3.Ogp),
                                    new XAttribute("primech", s3.Primech));


                                xeTracks.Add(xeMain);
                                //        xeTracks.Add(new XAttribute("countDistance", drawdownCount + constrictionCount + broadeningCount + levelCount + skewnessCount + straighteningCount));


                            }


                        }
                        Itog += drawdownCount + constrictionCount + broadeningCount + levelCount + skewnessCount + straighteningCount;

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

                        xeTracks.Add(new XAttribute("countDistance", drawdownCount + constrictionCount + broadeningCount + levelCount + skewnessCount + straighteningCount));
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
