using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using ALARm_Report.controls;
using System.Reflection;

namespace ALARm_Report.Forms
{
    public class IntegartedIndicators2 : Report
    {
        //Рад земли
        public int EarthRad = 6317 * 1000 * 100;
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod period,
            MetroProgressBar progressBar)
        {
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(parentId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel)
                    return;
                admTracksId = choiceForm.admTracksIDs;
            }


            var filterForm = new FilterForm();
            var filters = new List<Filter>();
            var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
            filters.Add(new FloatFilter() { Name = "Величина индекса", Value = 0.0f });
            filters.Add(new FloatFilter() { Name = "Величина съёма металла, мм", Value = 0.10f });
            filterForm.SetDataSource(filters);
            if (filterForm.ShowDialog() == DialogResult.Cancel)
                return;

            var npch = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId);

            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {

                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                //var tripProcesses = RdStructureService.GetAdditionalParametersProcess(period);

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);

                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                string prevDirection = "";

                foreach (var tripProcess in tripProcesses)
                {
                    var rd_profile = RdStructureService.GetRdTables(tripProcess, 1) as List<RdProfile>;
                    var mainParameters = RdStructureService.GetRdTablesByKM(tripProcess,
                                                                               rd_profile.First().Km * 1000 + rd_profile.First().Meter,
                                                                               rd_profile.Last().Km * 1000 + rd_profile.Last().Meter
                                                                               ) as List<RdProfile>;
                    var track_profile = rd_profile.OrderBy(r => r.X).ToList();

                    track_profile = track_profile.Where(o => o.Km > 128).ToList();

                    var tripElement = new XElement("trip",
                        new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        new XAttribute("way", roadName),
                        new XAttribute("npch", "ПЧ:" + (npch != null ? (npch as AdmUnit).Code : "-")),
                        new XAttribute("Period", period.Period + " "),
                        new XAttribute("addinfo", tripProcess.GetProcessTypeName),
                        new XAttribute("car", tripProcess.Car),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("filter1", ((float)(float)filters[0].Value).ToString("0.00")),
                        new XAttribute("filter2", "съём металла за проход = " + ((float)(float)filters[1].Value).ToString("0.00") + " мм")

                        );
                    foreach (var trackId in admTracksId)
                    {

                        //  List<Curve> curves = RdStructureService.GetCurvesInTrip(tripProcess.Id) as List<Curve>;

                        var curves = RdStructureService.GetCurvesAsTripElems(trackId, tripProcess.Date_Vrem, track_profile.First().Km, track_profile.First().M, track_profile.Last().Km, track_profile.Last().M);

                        int km = track_profile.First().Km;
                        int m = track_profile.First().M;
                        var trip = RdStructureService.GetTrip(tripProcess.Trip_id);
                        List<Kilometer> kilometers = RdStructureService.GetKilometersByTrip(trip);
                        kilometers = kilometers.OrderBy(o => o.Number).ToList();
                        var ListS3 = RdStructureService.GetS3(kilometers.First().Trip.Id) as List<S3>; //пру
                        XElement nhaprElement = null;
                        progressBar.Maximum = kilometers.Count;
                        foreach (var kilometer in kilometers)
                        {


                            foreach (var curve in curves)
                            {
                                if (!prevDirection.Equals(kilometer.Direction_name + " Путь : " + kilometer.Track_name))
                                {
                                    if (nhaprElement != null)
                                    {
                                        tripElement.Add(nhaprElement);
                                    }

                                    prevDirection = kilometer.Direction_name + " Путь : " + kilometer.Track_name;
                                    nhaprElement = new XElement("nhapr", new XAttribute("name", prevDirection));
                                }
                                List<Speed> settedSpeeds = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem,
                                                                                                          kilometer.Number, MainTrackStructureConst.MtoSpeed,
                                                                                                          tripProcess.DirectionName,
                                                                                                          "1"
                                                                                                          ) as List<Speed>; //toDo trackNumber
                                progressBar.Value = kilometers.IndexOf(kilometer) + 1;

                                //var shortRoughness = AdditionalParametersService.GetShortRoughnessFromText(kilometer.Number);

                                var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(kilometer.Number, tripProcess.Trip_id);
                                //if (DBcrossRailProfile == null || DBcrossRailProfile.Count() == 0) continue;

                                DBcrossRailProfile = DBcrossRailProfile.OrderBy(o => o.Meter).ToList();

                                var shortRoughness = AdditionalParametersService.GetShortRoughnessFromDBParse(DBcrossRailProfile);



                                var indicators = shortRoughness.GetIntegratedIndicators(settedSpeeds.Count > 0 ? settedSpeeds[0].Passenger : 140);
                                var leftInd = Math.Round((indicators[0][6] * indicators[0][7]) / 100, 2);
                                var rightInd = Math.Round((indicators[1][6] * indicators[1][7]) / 100, 2);

                                int rshp = shortRoughness.GetRshp(settedSpeeds.Count > 0 ? settedSpeeds[0].Passenger : 140, (float)(float)filters[1].Value);

                                if (!((rightInd >= (float)(float)filters[0].Value) || (leftInd >= (float)(float)filters[0].Value)))
                                    continue;

                                var koord = kilometer.Number + Math.Min(shortRoughness.MetersLeft.Min(), shortRoughness.MetersRight.Min()) / 10000.0;

                                var temp_curve = curves.Where(o =>
                                    o.RealStartCoordinate <= koord && koord <= o.RealFinalCoordinate
                                ).ToList();

                                var side = temp_curve.Any() ? (temp_curve.First().Side == "Правая" ? "Пр" : "Лв") : "";
                                //var final = (curves.Start_Km + curves.Start_M / 1000.0).ToString().Replace(',', '.');
                                if (curve.Start_Km * 1000 + curve.Start_M > km * 1000 + m + 1)
                                {
                                    var dev = mainParameters.Where(
                                        t => t.Km * 1000 + t.Meter >= km * 1000 + m && t.Km * 1000 + t.Meter <= curve.Start_Km * 1000 + curve.Start_M).ToList();
                                    var valueMaxDev = GetDeviationByProfile(dev);


                                    nhaprElement.Add(new XElement("datarow",
                                    //new XElement("begin", kilometer.Number + "." + Math.Min(shortRoughness.MetersLeft.Min(), shortRoughness.MetersRight.Min())),
                                    //new XElement("end", kilometer.Number + "." + Math.Max(shortRoughness.MetersLeft.Max(), shortRoughness.MetersRight.Max())),

                                    new XElement("priznak", side),
                                    new XElement("lnitl", Math.Round(indicators[0][4], 2) + "/" + Math.Round(indicators[0][5], 2)),
                                    new XElement("lnitm", Math.Round(indicators[0][2], 2) + "/" + Math.Round(indicators[0][3], 2)),
                                    new XElement("lnits", Math.Round(indicators[0][0], 2) + "/" + Math.Round(indicators[0][1], 2)),
                                    new XElement("lstate", Math.Round((indicators[0][6] * indicators[0][7]) / 1000, 2)),

                                    new XElement("rnitl", Math.Round(indicators[1][4], 2) + "/" + Math.Round(indicators[1][5], 2)),
                                    new XElement("rnitm", Math.Round(indicators[1][2], 2) + "/" + Math.Round(indicators[1][3], 2)),
                                    new XElement("rnits", Math.Round(indicators[1][0], 2) + "/" + Math.Round(indicators[1][1], 2)),
                                    new XElement("rstate", Math.Round((indicators[1][6] * indicators[1][7]) / 1000, 2)),
                                    new XElement("rshp_count", rshp),
                                    new XElement("begin", (curve.Start_Km + curve.Start_M / 1000.0).ToString().Replace(',', '.')),
                                    new XElement("end", (curve.Final_Km + curve.Final_M / 1000.0).ToString().Replace(',', '.')),
                                    new XElement("angle", "0"),
                                    new XElement("length", (curve.Start_Km + curve.Start_M / 1000.0 - km - m / 1000.0).ToString("0.###").Replace(',', '.')),
                                    new XElement("characteristics", "Прямая"),
                                    new XElement("deviation", $"{valueMaxDev}"))
                                        );

                                }
                                if (nhaprElement != null)
                                {
                                    tripElement.Add(nhaprElement);
                                }
                                //curve.Straightenings =
                                //    (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.Start_Km * 1000 + st.Start_M).ToList();

                                ////double angle = (Math.Atan((17860.0 / curve.Straightenings.Max(s => s.Radius)) / Math.Abs((curve.Final_Km - curve.Start_Km) * 1000 + curve.Final_M - curve.Start_M)) * 180) / Math.PI;
                                //var len = (curve.Final_Km + curve.Final_M / 1000.0 - curve.Start_Km - curve.Start_M / 1000.0) * 1000;
                                //double angle = (180 * len) / (Math.PI * Convert.ToInt32(curve.Straightenings.Max(s => s.Radius)));

                                //nhaprElement.Add(new XElement("datarow",

                                //    new XElement("priznak", side),
                                //    new XElement("lnitl", Math.Round(indicators[0][4], 2) + "/" + Math.Round(indicators[0][5], 2)),
                                //    new XElement("lnitm", Math.Round(indicators[0][2], 2) + "/" + Math.Round(indicators[0][3], 2)),
                                //    new XElement("lnits", Math.Round(indicators[0][0], 2) + "/" + Math.Round(indicators[0][1], 2)),
                                //    new XElement("lstate", Math.Round((indicators[0][6] * indicators[0][7]) / 1000, 2)),

                                //    new XElement("rnitl", Math.Round(indicators[1][4], 2) + "/" + Math.Round(indicators[1][5], 2)),
                                //    new XElement("rnitm", Math.Round(indicators[1][2], 2) + "/" + Math.Round(indicators[1][3], 2)),
                                //    new XElement("rnits", Math.Round(indicators[1][0], 2) + "/" + Math.Round(indicators[1][1], 2)),
                                //    new XElement("rstate", Math.Round((indicators[1][6] * indicators[1][7]) / 1000, 2)),
                                //    new XElement("rshp_count", rshp),
                                //    new XAttribute("begin", (curve.Start_Km + curve.Start_M / 1000.0).ToString().Replace(',', '.')),
                                //    new XAttribute("end", (curve.Final_Km + curve.Final_M / 1000.0).ToString().Replace(',', '.')),
                                //    new XAttribute("angle", curve.Side + " " + Math.Round(angle, 1).ToString().Replace(',', '.')),
                                //    new XAttribute("length", (curve.Final_Km + curve.Final_M / 1000.0 - curve.Start_Km - curve.Start_M / 1000.0).ToString("0.###").Replace(',', '.')),
                                //    new XAttribute("characteristics", "Кривая R = " + Convert.ToInt32(curve.Straightenings.Max(s => s.Radius))),
                                //    new XAttribute("deviation", "---"))
                                //    );

                                //km = curve.Final_Km;
                                //m = curve.Final_M;
                                //if (nhaprElement != null)
                                //{
                                //    tripElement.Add(nhaprElement);
                                //}
                            }


                            //if (track_profile.Last().Km * 1000 + track_profile.Last().M > km * 1000 + m + 1)
                            //{
                            //    var dev = mainParameters.Where(
                            //        t => t.Km * 1000 + t.Meter >= km * 1000 + m && t.Km * 1000 + t.Meter <= track_profile.Last().Km * 1000 + track_profile.Last().M).ToList();
                            //    var valueMaxDev = GetDeviationByProfile(dev);

                            //    nhaprElement.Add(new XElement("datarow",
                            //        new XElement("priznak", side),
                            //        new XElement("lnitl", Math.Round(indicators[0][4], 2) + "/" + Math.Round(indicators[0][5], 2)),
                            //        new XElement("lnitm", Math.Round(indicators[0][2], 2) + "/" + Math.Round(indicators[0][3], 2)),
                            //        new XElement("lnits", Math.Round(indicators[0][0], 2) + "/" + Math.Round(indicators[0][1], 2)),
                            //        new XElement("lstate", Math.Round((indicators[0][6] * indicators[0][7]) / 1000, 2)),

                            //        new XElement("rnitl", Math.Round(indicators[1][4], 2) + "/" + Math.Round(indicators[1][5], 2)),
                            //        new XElement("rnitm", Math.Round(indicators[1][2], 2) + "/" + Math.Round(indicators[1][3], 2)),
                            //        new XElement("rnits", Math.Round(indicators[1][0], 2) + "/" + Math.Round(indicators[1][1], 2)),
                            //        new XElement("rstate", Math.Round((indicators[1][6] * indicators[1][7]) / 1000, 2)),
                            //        new XElement("rshp_count", rshp),
                            //        new XAttribute("begin", (km + m / 1000.0).ToString().Replace(',', '.')),
                            //        new XAttribute("end", (track_profile.Last().Km + track_profile.Last().M / 1000.0).ToString().Replace(',', '.')),
                            //        new XAttribute("angle", "0"),
                            //        new XAttribute("length", (track_profile.Last().Km + track_profile.Last().M / 1000.0 - km - m / 1000.0).ToString("0.###").Replace(',', '.')),
                            //        new XAttribute("characteristics", "Прямая"),
                            //        new XAttribute("deviation", $"{valueMaxDev}"))
                            //        );
                            //}
                            //report.Add(tripElement);




                            //    nhaprElement.Add(new XElement("datarow",
                            //    new XElement("begin", kilometer.Number + "." + Math.Min(shortRoughness.MetersLeft.Min(), shortRoughness.MetersRight.Min())),
                            //    new XElement("end", kilometer.Number + "." + Math.Max(shortRoughness.MetersLeft.Max(), shortRoughness.MetersRight.Max())),
                            //    //new XAttribute("begin", (km + m / 1000.0).ToString().Replace(',', '.')),
                            //    //new XAttribute("end", ""),
                            //    new XElement("priznak", side),

                            //    new XElement("lnitl", Math.Round(indicators[0][4], 2) + "/" + Math.Round(indicators[0][5], 2)),
                            //    new XElement("lnitm", Math.Round(indicators[0][2], 2) + "/" + Math.Round(indicators[0][3], 2)),
                            //    new XElement("lnits", Math.Round(indicators[0][0], 2) + "/" + Math.Round(indicators[0][1], 2)),
                            //    new XElement("lstate", Math.Round((indicators[0][6] * indicators[0][7]) / 1000, 2)),

                            //    new XElement("rnitl", Math.Round(indicators[1][4], 2) + "/" + Math.Round(indicators[1][5], 2)),
                            //    new XElement("rnitm", Math.Round(indicators[1][2], 2) + "/" + Math.Round(indicators[1][3], 2)),
                            //    new XElement("rnits", Math.Round(indicators[1][0], 2) + "/" + Math.Round(indicators[1][1], 2)),
                            //    new XElement("rstate", Math.Round((indicators[1][6] * indicators[1][7]) / 1000, 2)),

                            //    new XElement("rshp_count", rshp)
                            //));

                        }



                        report.Add(tripElement);


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