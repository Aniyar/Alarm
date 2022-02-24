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
    public class TripPlanProfileIrregularity : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            //Сделать выбор периода
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(distanceId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel)
                    return;
                admTracksId = choiceForm.admTracksIDs;
            }

            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdDocument = new XDocument();
                XElement report = new XElement("report");

                var road = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;

                var mainProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (!mainProcesses.Any())
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                foreach (var process in mainProcesses)
                {
                    ////var irregularities = ((List<RdIrregularity>) RdStructureService.GetRdTables(process, 0)).Where(i => (Math.Abs(i.Amount) > 30 && i.Belong == 0) || (Math.Abs(i.Amount) > 50 && i.Belong == 1));
                    ////var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;

                    //if (!irregularities.Any())
                    //    continue;

                    foreach (var track_id in admTracksId)
                    {

                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var trip = RdStructureService.GetTrip(process.Id);
                        var kms = RdStructureService.GetKilometersByTrip(trip);
                        if (!kms.Any()) continue;



                        kms = kms.Where(o => o.Track_id == track_id).ToList();
                        if (kms.Count == 0) continue;
                        trip.Track_Id = track_id;
                        var lkm = kms.Select(o => o.Number).ToList();

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



                        XElement xePlan = new XElement("plan");
                        int count = 0;

                        XElement xePages = new XElement("pages",
                             new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        new XAttribute("road", road),
                        new XAttribute("distance", distance.Code),
                         new XAttribute("period", period.Period),
                        new XAttribute("trip_info",  process.GetProcessTypeName),
                        new XAttribute("car", process.Car),
                         new XAttribute("chief", process.Chief),
                        new XAttribute("trip_date", process.Trip_date),
                        new XAttribute("direction", process.DirectionName + " (" + process.DirectionCode + ")"),
                        new XAttribute("track", trackName));

                        var rows = RdStructureService.GetRdTables(process, 10) as List<RdProfile>;

                        int prevInd = -1;
                        int EarthRad = 6317*1000;

                        

                        for (int i=0;i<= rows.Count()-150; i++)
                        {
                            //150 елемент аламыз
                            var list150 = rows.Skip(i).Take(150);

                            List<double> x = new List<double>();
                            List<double> y = new List<double>();
                            List<double> z = new List<double>();

                            foreach (var elem in list150)
                            {
                                x.Add(
                                    ((EarthRad+ elem.Heigth)*Math.Sin(elem.Latitude.Radian())*Math.Sin(elem.Longitude.Radian()))-((EarthRad + list150.First().Heigth) * Math.Sin(list150.First().Latitude.Radian()) * Math.Sin(list150.First().Longitude.Radian()))
                                    );
                                y.Add(
                                    ((EarthRad + elem.Heigth) * Math.Sin(elem.Latitude.Radian()) * Math.Cos(elem.Longitude.Radian())) - ((EarthRad + list150.First().Heigth) * Math.Sin(list150.First().Latitude.Radian()) * Math.Cos(list150.First().Longitude.Radian()))
                                    );
                                z.Add(
                                    (EarthRad + elem.Heigth) * Math.Cos(elem.Latitude.Radian())
                                    );
                            }

                            //определяем линию
                            var A1 = (y.Last() - y.First()) / (x.Last() - x.First());
                            var B1 = -1;
                            var C1 = (x.Last() * y.First() - x.First() * y.Last()) / (x.Last() - x.First());

                            //расстояние каждой точки до линий
                            List<double> distList = new List<double>();
                            for (int k = 0; k <= x.Count()-1; k++)
                            {
                                //var up = Math.Abs(A1 * x[k] + B1 * y[k] + C1);
                                //var dou = Math.Sqrt(Math.Pow(A1, 2) + Math.Pow(B1, 2));
                                var D = Math.Abs(x[k] - 1/A1 * y[k]);
                                distList.Add(D);
                            }
                            //max расстояние
                            var maxDist = distList[75];
                            var maxIndDist = distList.IndexOf(maxDist);

                            string notice= "";

                            if (rows[maxIndDist + i].IsSwitch)
                                notice = "стрелки";
                            else if (rows[maxIndDist + i].IsBridge)
                                notice = "мост";
                            else if (rows[maxIndDist + i].IsStation)
                                notice = "Станция";
                            else
                                notice = "";

                            xePlan.Add(new XElement("elements",
                                new XAttribute("track", rows[maxIndDist + i].Track),
                                new XAttribute("km", $"{rows[maxIndDist + i].Km} км, пк {rows[maxIndDist + i].Meter/100+1}"),
                                new XAttribute("irregularity", string.Format("{0:0.0}", maxDist)),
                                new XAttribute("tapvalue", ""),
                                new XAttribute("notice", notice)));

                            count++;
                        }
                        xePlan.Add(new XAttribute("count", count));
                        xePages.Add(xePlan);

                        //foreach (var type in irregularities.GroupBy(i => i.Belong).Select(i => i.Key))
                        //{
                        //    foreach (var trackId in irregularities.Where(i => i.Belong == type).GroupBy(i => i.Track_Id).Select(i => i.Key))
                        //    {
                        //        XElement xePlan = new XElement(type == 0 ? "plan" : "profile");
                        //        int count = 0;
                        //        var irrs = irregularities.Where(i => i.Belong == type && i.Track_Id == trackId).ToList();

                        //        foreach (var irr in irrs)
                        //        {
                        //            string notice;

                        //            if (irr.IsSwitch)
                        //                notice = "стрелки";
                        //            else if (irr.IsBridge)
                        //                notice = "мост";
                        //            else if (irr.IsStation)
                        //                notice = "Станция";
                        //            else
                        //                notice = "";

                        //            xePlan.Add(new XElement("elements",
                        //                new XAttribute("track", irr.TrackName),
                        //                new XAttribute("km", irr.Km.ToString() + " км, пк " + irr.Piket.ToString()),
                        //                new XAttribute("irregularity", Math.Abs(irr.Amount)),
                        //                new XAttribute("tapvalue", irr.Slope_tap),
                        //                new XAttribute("notice", notice)));

                        //            count++;
                        //        }

                        //        xePlan.Add(new XAttribute("count", count));
                        //        xePages.Add(xePlan);
                        //    }
                        //}

                        report.Add(xePages);
                    }
                }

                xdDocument.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdDocument.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_TripPlanProfileIrregularity.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_TripPlanProfileIrregularity.html");
            }
        }
    }
}
