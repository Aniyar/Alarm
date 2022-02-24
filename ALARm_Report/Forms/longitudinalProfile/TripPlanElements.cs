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
    
    public class TripPlanElements : Report
    {
        //Рад земли
        public int EarthRad = 6317 * 1000 * 100;

        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
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
                XDocument xdReport = new XDocument();
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
                    //Данные профиля
                    var rd_profile = RdStructureService.GetRdTables(process, 1) as List<RdProfile>;
                    var mainParameters = RdStructureService.GetRdTablesByKM(process,
                                                                                rd_profile.First().Km * 1000 + rd_profile.First().Meter,
                                                                                rd_profile.Last().Km * 1000 + rd_profile.Last().Meter
                                                                                ) as List<RdProfile>;
                    if (mainParameters == null) continue;
                    var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;

                    if (!rd_profile.Any())
                        continue;

                 

                    foreach (var trackId in admTracksId)
                    {

                        var trackName = AdmStructureService.GetTrackName(trackId);
                        var trip = RdStructureService.GetTrip(process.Id);
                        var kms = RdStructureService.GetKilometersByTrip(trip);
                        if (!kms.Any()) continue;



                        kms = kms.Where(o => o.Track_id == trackId).ToList();
                        if (kms.Count == 0) continue;
                        trip.Track_Id = trackId;
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



                        var track_profile = rd_profile.OrderBy(r => r.X).ToList();

                        //track_profile = track_profile.Where(o => o.Km > 715 && o.Km <727).ToList();
                        track_profile = track_profile.Where(o => o.Km > lkm.Min() && o.Km < lkm.Max()).ToList();


                        //var trackName = AdmStructureService.GetTrackName(trackId);
                        //var track_profile = rd_profile.OrderBy(r => r.X).ToList();
                        //track_profile = track_profile.Where(o => o.Km > 715 && o.Km <727).ToList();
                        if (!track_profile.Any()) continue;
                        XElement xePages = new XElement("pages",
                                new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                               new XAttribute("period", period.Period),
                               new XAttribute("road", road != null ? road : ""),
                               new XAttribute("distance", distance.Code != null ? distance.Code : ""),
                               new XAttribute("trip_info", process.GetProcessTypeName),
                             
                               //new XAttribute("tripinfo", $"{process.DirectionName} Путь - {trackName}"),
                               new XAttribute("tripinfo", process.DirectionName + " (" + process.DirectionCode + "), Путь: " + trackName),
                               new XAttribute("car", process.Car != null ? process.Car : ""),
                               new XAttribute("trip_date", process.Trip_date != null ? process.Trip_date : ""),
                               new XAttribute("direction", $"{process.DirectionName}({process.DirectionCode})"),
                               new XAttribute("chief", process.Chief));


                        var curves = RdStructureService.GetCurvesAsTripElems(trackId, process.Date_Vrem, track_profile.First().Km, track_profile.First().M, track_profile.Last().Km, track_profile.Last().M);

                        

                        int km = track_profile.First().Km;
                        int m = track_profile.First().M;

                        foreach (var curve in curves)
                        {
                            if (curve.Start_Km * 1000 + curve.Start_M > km * 1000 + m + 1)
                            {
                                var dev = mainParameters.Where(
                                    t => t.Km*1000+t.Meter >= km * 1000 + m && t.Km * 1000 + t.Meter <= curve.Start_Km * 1000 + curve.Start_M).ToList();
                                var valueMaxDev = GetDeviationByProfile(dev);

                                 
                                xePages.Add(new XElement("elements",
                                    new XAttribute("start", (km + m / 1000.0).ToString().Replace(',', '.')),
                                    new XAttribute("final", (curve.Start_Km + curve.Start_M / 1000.0).ToString().Replace(',', '.')),
                                    new XAttribute("angle", "0"),
                                    new XAttribute("length", (curve.Start_Km + curve.Start_M / 1000.0 - km - m / 1000.0).ToString("0.000").Replace(',', '.')),
                                    new XAttribute("characteristics", "Прямая"),
                                    new XAttribute("deviation", $"{valueMaxDev}"))
                                    );
                            }

                            curve.Straightenings = 
                                (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.Start_Km * 1000 + st.Start_M).ToList();

                            //double angle = (Math.Atan((17860.0 / curve.Straightenings.Max(s => s.Radius)) / Math.Abs((curve.Final_Km - curve.Start_Km) * 1000 + curve.Final_M - curve.Start_M)) * 180) / Math.PI;
                            var len = (curve.Final_Km + curve.Final_M / 1000.0 - curve.Start_Km - curve.Start_M / 1000.0) * 1000;
                            double angle = (180 * len) / (Math.PI * Convert.ToInt32(curve.Straightenings.Max(s => s.Radius)));
                            

                            xePages.Add(new XElement("elements",
                                new XAttribute("start", (curve.Start_Km + curve.Start_M / 1000.0).ToString().Replace(',', '.')),
                                new XAttribute("final", (curve.Final_Km + curve.Final_M / 1000.0).ToString().Replace(',', '.')),
                                new XAttribute("angle", curve.Side + " " + Math.Round(angle, 1).ToString().Replace(',', '.')),
                                new XAttribute("length", (curve.Final_Km + curve.Final_M / 1000.0 - curve.Start_Km - curve.Start_M / 1000.0).ToString("0.000").Replace(',', '.')),
                                new XAttribute("characteristics", "Кривая R = " + Convert.ToInt32(curve.Straightenings.Max(s => s.Radius))),
                                new XAttribute("deviation", "---"))
                                );

                            km = curve.Final_Km;
                            m = curve.Final_M;
                        }
                       

                        if (track_profile.Last().Km * 1000 + track_profile.Last().M > km * 1000 + m + 1)
                        {
                            var dev = mainParameters.Where(
                                t => t.Km * 1000 + t.Meter >= km * 1000 + m && t.Km * 1000 + t.Meter <= track_profile.Last().Km * 1000 + track_profile.Last().M).ToList();
                            var valueMaxDev = GetDeviationByProfile(dev);

                            xePages.Add(new XElement("elements",
                                new XAttribute("start", (km + m / 1000.0).ToString().Replace(',', '.')),
                                new XAttribute("final", (track_profile.Last().Km + track_profile.Last().M / 1000.0).ToString().Replace(',', '.')),
                                new XAttribute("angle", "0"),
                                new XAttribute("length", (track_profile.Last().Km + track_profile.Last().M / 1000.0 - km - m / 1000.0).ToString("0.###").Replace(',', '.')),
                                new XAttribute("characteristics", "Прямая"),
                                new XAttribute("deviation", $"{valueMaxDev}"))
                                );
                        }

                      
                        report.Add(xePages);
                    }
                   

                }

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_TripPlanElements.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_TripPlanElements.html");
            }
        }

        public object GetDeviationByProfile(List<RdProfile> list_pryamoi)
        {
            //скользящ сред для д
            var RollAver_d01 = new List<double>();
            var width = 100;
            List<double> d_list = new List<double>();

            if (list_pryamoi.Count > 0)
            {
                var x1 = EarthRad * Math.Cos(list_pryamoi.First().Latitude.Radian()) * Math.Cos(list_pryamoi.First().Longitude.Radian());
                var y1 = EarthRad * Math.Cos(list_pryamoi.First().Latitude.Radian()) * Math.Sin(list_pryamoi.First().Longitude.Radian());
                var z1 = EarthRad * Math.Sin(list_pryamoi.First().Latitude.Radian());

                var x2 = EarthRad * Math.Cos(list_pryamoi.Last().Latitude.Radian()) * Math.Cos(list_pryamoi.Last().Longitude.Radian());
                var y2 = EarthRad * Math.Cos(list_pryamoi.Last().Latitude.Radian()) * Math.Sin(list_pryamoi.Last().Longitude.Radian());
                var z2 = EarthRad * Math.Sin(list_pryamoi.Last().Latitude.Radian());

                var l = x2 - x1;
                var m = y2 - y1;
                var n = z2 - z1;

                //список средних знач
                List<double> ra_d01_list = new List<double>();

                for (int k = 0; k < list_pryamoi.Count; k++)
                {
                    var xtr_list = (EarthRad * Math.Cos(list_pryamoi[k].Latitude.Radian()) * Math.Cos(list_pryamoi[k].Longitude.Radian()));
                    var ytr_list = (EarthRad * Math.Cos(list_pryamoi[k].Latitude.Radian()) * Math.Sin(list_pryamoi[k].Longitude.Radian()));
                    var ztr_list = (EarthRad * Math.Sin(list_pryamoi[k].Latitude.Radian()));

                    //параметры
                    var A = l * l + m * m + n * n;
                    var B = -2.0 * (xtr_list - x1) * l - 2.0 * (ytr_list - y1) * m - 2.0 * (ztr_list - z1) * n;
                    var t = -B / (2.0 * A);

                    //проекция
                    var xpr = x1 + l * t;
                    var ypr = y1 + m * t;
                    var zpr = z1 + n * t;

                    //проекция на плоскость плана
                    var d01 = Math.Sqrt(Math.Pow(xtr_list - xpr, 2) + Math.Pow(ytr_list - ypr, 2));

                    if (RollAver_d01.Count() >= width)
                    {
                        RollAver_d01.Add(d01);
                        ra_d01_list.Add(RollAver_d01.Skip(RollAver_d01.Count() - width).Take(width).Average());
                    }
                    else
                    {
                        RollAver_d01.Add(d01);
                        ra_d01_list.Add(d01);
                    }
                }

                var ExponentCoef = -10;
                for (var x = 0; x < ra_d01_list.Count; x++)
                {
                    var e = Math.Exp(ExponentCoef * Math.Abs(RollAver_d01[x] - ra_d01_list[x]));
                    var D_exp = ra_d01_list[x] + (RollAver_d01[x] - ra_d01_list[x]) * e;

                    d_list.Add(D_exp);

                }
                var d = d_list.Max() > 100 ? 0.0 : d_list.Max();

                return d.ToString("0.00").Replace(",", ".");
            }
            else
            {
                return "Нет данных";
            }
            
        }
    }
}
