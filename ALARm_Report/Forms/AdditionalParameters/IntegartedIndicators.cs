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
using Dapper;
using ALARm.DataAccess;

namespace ALARm_Report.Forms
{
    // убрать 22  IntegartedIndicators22 
    public class IntegartedIndicators : Report
    {
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod period,
            MetroProgressBar progressBar)
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
                tripProcesses = tripProcesses.Where(o => o.Trip_id == 242).ToList();

                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var trip = RdStructureService.GetTrip(tripProcess.Trip_id);

                        var kilometers = RdStructureService.GetKilometersByTrip(trip);
                        kilometers = kilometers.OrderBy(o => o.Number).ToList();
                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();
                        if (kilometers.Count == 0) continue;

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

                        XElement nhaprElement = null;

                        var curves = RdStructureService.GetCurvesInTrip(trip.Id);
                        var min = curves.Select(o => o.Start_Km).Min();
                        var max = curves.Select(o => o.Final_Km).Max();

                        var filterForm1 = new FilterForm();
                        var filters1 = new List<Filter>();
                        filters1.Add(new FloatFilter() { Name = "Начало (км)", Value = min });
                        filters1.Add(new FloatFilter() { Name = "Конец (км)", Value = max });
                        filterForm1.SetDataSource(filters1);
                        if (filterForm1.ShowDialog() == DialogResult.Cancel)
                            return;
                        //фильтр по выбранным км
                        var filter_curves = curves.Where(o => ((float)(float)filters1[0].Value <= o.Start_Km && o.Final_Km <= (float)(float)filters1[1].Value)).ToList();
                        progressBar.Maximum = curves.Count;

                        var direction = AdmStructureService.GetDirectionByTrack(track_id) as AdmDirection;
                        prevDirection = (direction != null ? $"{direction.Name} ({direction.Code})" : "Неизвестный") + " Путь : " + trackName;

                        nhaprElement = new XElement("nhapr", new XAttribute("name", prevDirection));

                        //foreach (var curve in filter_curves)
                        for (int i = 0; i < filter_curves.Count - 1; i++)
                        {
                            filter_curves[i].Straightenings = (MainTrackStructureService.GetCurves(filter_curves[i].Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();
                            filter_curves[i + 1].Straightenings = (MainTrackStructureService.GetCurves(filter_curves[i + 1].Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();

                            var startStrKm = filter_curves[i].Straightenings.First().Final_Km;
                            var startStrM = filter_curves[i].Straightenings.First().Final_M;
                            var finalStrKm = filter_curves[i + 1].Straightenings.First().Start_Km;
                            var finalStrM = filter_curves[i + 1].Straightenings.First().Start_M;

                            var curveCoordDiff = Math.Abs(filter_curves[i].Straightenings.First().RealFinalCoordinate - filter_curves[i + 1].Straightenings.First().RealStartCoordinate) * 10000 % 1000;

                            var calcDataCurve = GetProfileData(filter_curves[i].Straightenings.First().RealStartCoordinate, filter_curves[i].Straightenings.First().RealFinalCoordinate, tripProcess.Trip_id);

                            if (calcDataCurve.ShortNaturRight.Any())
                            {
                                List<Speed> settedSpeeds = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem,
                                                                                                              filter_curves[i].Straightenings.First().Start_Km, MainTrackStructureConst.MtoSpeed,
                                                                                                              direction.Name, trackName.ToString()) as List<Speed>;
                                var shortRoughness = calcDataCurve;

                                var indicators = shortRoughness.GetIntegratedIndicators(settedSpeeds.Count > 0 ? settedSpeeds[0].Passenger : 140);
                                var leftInd = Math.Round((indicators[0][6] * indicators[0][7]) / 100, 2);
                                var rightInd = Math.Round((indicators[1][6] * indicators[1][7]) / 100, 2);

                                int rshp = shortRoughness.GetRshp(settedSpeeds.Count > 0 ? settedSpeeds[0].Passenger : 140, (float)(float)filters[1].Value);

                                if (!((rightInd >= (float)(float)filters[0].Value) || (leftInd >= (float)(float)filters[0].Value)))
                                    continue;

                                nhaprElement.Add(new XElement("datarow",
                                    new XElement("begin", filter_curves[i].Straightenings.First().Start_Km + "." + filter_curves[i].Straightenings.First().Start_M),
                                    new XElement("end", filter_curves[i].Straightenings.First().Final_Km + "." + filter_curves[i].Straightenings.First().Final_M),
                                    new XElement("priznak", filter_curves[i].Side == "Левая" ? "Правая" : "Левая"),

                                    new XElement("lnitl", Math.Round(indicators[0][4], 2) + "/" + Math.Round(indicators[0][5], 2)),
                                    new XElement("lnitm", Math.Round(indicators[0][2], 2) + "/" + Math.Round(indicators[0][3], 2)),
                                    new XElement("lnits", Math.Round(indicators[0][0], 2) + "/" + Math.Round(indicators[0][1], 2)),
                                    new XElement("lstate", Math.Round((indicators[0][6] * indicators[0][7]) / 1000, 2)),

                                    new XElement("rnitl", Math.Round(indicators[1][4], 2) + "/" + Math.Round(indicators[1][5], 2)),
                                    new XElement("rnitm", Math.Round(indicators[1][2], 2) + "/" + Math.Round(indicators[1][3], 2)),
                                    new XElement("rnits", Math.Round(indicators[1][0], 2) + "/" + Math.Round(indicators[1][1], 2)),
                                    new XElement("rstate", Math.Round((indicators[1][6] * indicators[1][7]) / 1000, 2)),

                                    new XElement("rshp_count", rshp)
                                ));
                            }

                            if (curveCoordDiff > 100)
                            {
                                var calcDataStr = GetProfileData(startStrKm + startStrM / 10000.0, finalStrKm + finalStrM / 10000.0, tripProcess.Trip_id);

                                if (calcDataStr.ShortNaturRight.Any())
                                {
                                    List<Speed> settedSpeeds = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem,
                                                                                                              startStrKm, MainTrackStructureConst.MtoSpeed,
                                                                                                              direction.Name, trackName.ToString()) as List<Speed>;
                                    var shortRoughness = calcDataStr;

                                    var indicators = shortRoughness.GetIntegratedIndicators(settedSpeeds.Count > 0 ? settedSpeeds[0].Passenger : 140);
                                    var leftInd = Math.Round((indicators[0][6] * indicators[0][7]) / 100, 2);
                                    var rightInd = Math.Round((indicators[1][6] * indicators[1][7]) / 100, 2);

                                    int rshp = shortRoughness.GetRshp(settedSpeeds.Count > 0 ? settedSpeeds[0].Passenger : 140, (float)(float)filters[1].Value);

                                    if (!((rightInd >= (float)(float)filters[0].Value) || (leftInd >= (float)(float)filters[0].Value)))
                                        continue;

                                    nhaprElement.Add(new XElement("datarow",
                                        new XElement("begin", startStrKm + "." + startStrM),
                                        new XElement("end", finalStrKm + "." + finalStrM),
                                        new XElement("priznak", ""),

                                        new XElement("lnitl", Math.Round(indicators[0][4], 2) + "/" + Math.Round(indicators[0][5], 2)),
                                        new XElement("lnitm", Math.Round(indicators[0][2], 2) + "/" + Math.Round(indicators[0][3], 2)),
                                        new XElement("lnits", Math.Round(indicators[0][0], 2) + "/" + Math.Round(indicators[0][1], 2)),
                                        new XElement("lstate", Math.Round((indicators[0][6] * indicators[0][7]) / 1000, 2)),

                                        new XElement("rnitl", Math.Round(indicators[1][4], 2) + "/" + Math.Round(indicators[1][5], 2)),
                                        new XElement("rnitm", Math.Round(indicators[1][2], 2) + "/" + Math.Round(indicators[1][3], 2)),
                                        new XElement("rnits", Math.Round(indicators[1][0], 2) + "/" + Math.Round(indicators[1][1], 2)),
                                        new XElement("rstate", Math.Round((indicators[1][6] * indicators[1][7]) / 1000, 2)),

                                        new XElement("rshp_count", rshp)
                                    ));
                                }
                            }
                        }
                        tripElement.Add(nhaprElement);

                        report.Add(tripElement);

                        //var tempKm = kilometers.ToList();

                        //foreach (var kilometer in tempKm)
                        //{
                        //    if (!prevDirection.Equals(kilometer.Direction_name + " Путь : " + kilometer.Track_name))
                        //    {
                        //        if (nhaprElement != null)
                        //        {
                        //            tripElement.Add(nhaprElement);
                        //        }

                        //        prevDirection = kilometer.Direction_name + " Путь : " + kilometer.Track_name;
                        //        nhaprElement = new XElement("nhapr", new XAttribute("name", prevDirection));
                        //    }
                        //    List<Speed> settedSpeeds = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem,
                        //                                                                              kilometer.Number, MainTrackStructureConst.MtoSpeed,
                        //                                                                              tripProcess.DirectionName,
                        //                                                                              "1"
                        //                                                                              ) as List<Speed>; //toDo trackNumber
                        //    progressBar.Value = kilometers.IndexOf(kilometer) + 1;

                        //    //var shortRoughness = AdditionalParametersService.GetShortRoughnessFromText(kilometer.Number);

                        //    var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(kilometer.Number, tripProcess.Trip_id);
                        //    if (DBcrossRailProfile == null || DBcrossRailProfile.Count() == 0) continue;

                            //var shortRoughness = AdditionalParametersService.GetShortRoughnessFromDBParse(DBcrossRailProfile);



                            //var indicators = shortRoughness.GetIntegratedIndicators(settedSpeeds.Count > 0 ? settedSpeeds[0].Passenger : 140);
                        //    var leftInd = Math.Round((indicators[0][6] * indicators[0][7]) / 100, 2);
                        //    var rightInd = Math.Round((indicators[1][6] * indicators[1][7]) / 100, 2);

                        //    int rshp = shortRoughness.GetRshp(settedSpeeds.Count > 0 ? settedSpeeds[0].Passenger : 140, (float)(float)filters[1].Value);

                        //    if (!((rightInd >= (float)(float)filters[0].Value) || (leftInd >= (float)(float)filters[0].Value)))
                        //        continue;
                        //    var side = curves.Select(o => o.Side[0]).ToList().ToString();
                        //    nhaprElement.Add(new XElement("datarow",
                        //        new XElement("begin", kilometer.Number + "." + Math.Min(shortRoughness.MetersLeft.Min(), shortRoughness.MetersRight.Min())),
                        //        new XElement("end", kilometer.Number + "." + Math.Max(shortRoughness.MetersLeft.Max(), shortRoughness.MetersRight.Max())),

                        //        new XElement("priznak", ""),

                        //        new XElement("lnitl", Math.Round(indicators[0][4], 2) + "/" + Math.Round(indicators[0][5], 2)),
                        //        new XElement("lnitm", Math.Round(indicators[0][2], 2) + "/" + Math.Round(indicators[0][3], 2)),
                        //        new XElement("lnits", Math.Round(indicators[0][0], 2) + "/" + Math.Round(indicators[0][1], 2)),
                        //        new XElement("lstate", Math.Round((indicators[0][6] * indicators[0][7]) / 1000, 2)),

                        //        new XElement("rnitl", Math.Round(indicators[1][4], 2) + "/" + Math.Round(indicators[1][5], 2)),
                        //        new XElement("rnitm", Math.Round(indicators[1][2], 2) + "/" + Math.Round(indicators[1][3], 2)),
                        //        new XElement("rnits", Math.Round(indicators[1][0], 2) + "/" + Math.Round(indicators[1][1], 2)),
                        //        new XElement("rstate", Math.Round((indicators[1][6] * indicators[1][7]) / 1000, 2)),

                        //        new XElement("rshp_count", rshp)
                        //    ));

                        //}
                        //if (nhaprElement != null)
                        //{
                        //    tripElement.Add(nhaprElement);
                        //}
                        //report.Add(tripElement);
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

        private ShortRoughness GetProfileData(double start, double final, long tripId)
        {
            var connection = new Npgsql.NpgsqlConnection(ALARm.DataAccess.Helper.ConnectionString());
            var query = $@" SELECT * FROM testdata_{tripId}
                            WHERE
	                            km + meter / 10000.0 BETWEEN {Math.Min(start, final).ToString().Replace(",", ".")} 
	                                                     AND {Math.Max(start, final).ToString().Replace(",", ".")} 
                            ORDER BY
	                            km + meter / 10000.0";

            var ShortData = connection.Query<DataFlow>(query).ToList();

            var shortl = ShortData.Select(o => o.Diff_l / 8.0f < 1 / 8.0f ? 0 : o.Diff_l / 8.0f).ToList();
            var shortr = ShortData.Select(o => o.Diff_r / 8.0f < 1 / 8.0f ? 0 : o.Diff_r / 8.0f).ToList();

            Meters.AddRange(ShortData.Select(o => o.Meter).ToList());


            var val = new List<float> { };
            var val_ind = new List<int> { };
            var bolshe0 = new List<DATA0> { };
            var inn = false;

            //left
            for (int i = 0; i < shortl.Count - 1; i++)
            {
                var temp = shortl[i];
                var next_temp = shortl[i + 1];

                if (!inn && 0 < next_temp)
                {
                    val.Add(temp);
                    val_ind.Add(i);

                    val.Add(next_temp);
                    val_ind.Add(i + 1);

                    inn = true;
                }
                else if (inn && 0 < next_temp)
                {
                    val.Add(next_temp);
                    val_ind.Add(i + 1);

                }
                else if (inn && 0 == next_temp)
                {
                    if (val.Any())
                    {
                        val.Add(next_temp);
                        val_ind.Add(i + 1);

                        var d = new DATA0
                        {
                            Count = val.Count,
                            H = val.Max(),
                            H_ind = val_ind[val.IndexOf(val.Max())],
                        };

                        bolshe0.Add(d);

                        inn = false;
                        val.Clear();
                        val_ind.Clear();
                    }
                }
            }


            var val_r = new List<float> { };
            var val_ind_r = new List<int> { };
            var bolshe0_r = new List<DATA0> { };
            var inn_r = false;

            //right
            for (int i = 0; i < shortr.Count - 1; i++)
            {
                var temp = shortr[i];
                var next_temp = shortr[i + 1];

                if (!inn_r && 0 < next_temp)
                {
                    val_r.Add(temp);
                    val_ind_r.Add(i);

                    val_r.Add(next_temp);
                    val_ind_r.Add(i + 1);

                    inn_r = true;
                }
                else if (inn_r && 0 < next_temp)
                {
                    val_r.Add(next_temp);
                    val_ind_r.Add(i + 1);

                }
                else if (inn_r && 0 == next_temp)
                {
                    if (val_r.Any())
                    {
                        val_r.Add(next_temp);
                        val_ind_r.Add(i + 1);

                        var d = new DATA0
                        {
                            Count = val_r.Count,
                            H = val_r.Max(),
                            H_ind = val_ind_r[val_r.IndexOf(val_r.Max())],
                        };

                        bolshe0_r.Add(d);

                        inn_r = false;
                        val_r.Clear();
                        val_ind_r.Clear();
                    }
                }
            }

            for (int j = 0; j < shortl.Count; j++)
            {
                var m = 0.0;
                var l = 0.0;
                var s = 0.0;

                var mr = 0.0;
                var lr = 0.0;
                var sr = 0.0;

                for (int i = 0; i < bolshe0.Count; i++)
                {
                    l += bolshe0[i].H * Math.Exp(-0.003 * Math.Pow(bolshe0[i].H_ind - j, 2) / bolshe0[i].Count);
                    m += bolshe0[i].H * Math.Exp(-0.02 * Math.Pow(bolshe0[i].H_ind - j, 2) / bolshe0[i].Count);
                    s += bolshe0[i].H * Math.Exp(-0.3 * Math.Pow(bolshe0[i].H_ind - j, 2) / bolshe0[i].Count);
                }

                for (int i = 0; i < bolshe0_r.Count; i++)
                {
                    lr += bolshe0_r[i].H * Math.Exp(-0.003 * Math.Pow(bolshe0_r[i].H_ind - j, 2) / bolshe0_r[i].Count);
                    mr += bolshe0_r[i].H * Math.Exp(-0.02 * Math.Pow(bolshe0_r[i].H_ind - j, 2) / bolshe0_r[i].Count);
                    sr += bolshe0_r[i].H * Math.Exp(-0.3 * Math.Pow(bolshe0_r[i].H_ind - j, 2) / bolshe0_r[i].Count);


                }

                LongWavesLeft.Add((float)(l * 0.05));
                MediumWavesLeft.Add((float)(m * 0.06));
                ShortWavesLeft.Add((float)(s * 0.1));

                LongWavesRight.Add((float)(lr * 0.05));
                MediumWavesRight.Add((float)(mr * 0.06));
                ShortWavesRight.Add((float)(sr * 0.1));
            }
            //импульсы
            for (int i = 0; i < bolshe0.Count; i++)
            {
                if (bolshe0[i].H < 0.6) continue;

                Impuls.Add(new Digression
                {
                    Length = (int)bolshe0[i].Count,
                    Len = (int)bolshe0[i].Count,
                    Intensity_ra = bolshe0[i].H,
                    Meter = Meters[bolshe0[i].H_ind],
                    Threat = Threat.Left
                });
            }
            for (int i = 0; i < bolshe0_r.Count; i++)
            {
                if (bolshe0_r[i].H < 0.6) continue;

                Impuls.Add(new Digression
                {
                    Length = (int)bolshe0_r[i].Count,
                    Len = (int)bolshe0_r[i].Count,
                    Intensity_ra = bolshe0_r[i].H,
                    Meter = Meters[bolshe0_r[i].H_ind],
                    Threat = Threat.Right
                });
            }

            var result = new ShortRoughness();
            result.MetersRight.AddRange(ShortData.Select(o => o.Meter).ToList());
            result.ShortNaturRight.AddRange(ShortWavesRight);
            result.LongNaturRight.AddRange(LongWavesRight);
            result.MediumNaturRight.AddRange(MediumWavesRight);

            result.MetersLeft.AddRange(ShortData.Select(o => o.Meter).ToList());
            result.ShortNaturLeft.AddRange(ShortWavesLeft);
            result.LongNaturLeft.AddRange(LongWavesLeft);
            result.MediumNaturLeft.AddRange(MediumWavesLeft);

            return result;
        }

        public class DATA0
        {
            public float H { get; set; }
            public int H_ind { get; set; }
            public float Count { get; set; }
            public float HR { get; internal set; }
        }

        public class DataFlow
        {
            public int Km { get; set; }
            public int Meter { get; set; }
            public float Diff_l { get; set; }
            public float Diff_r { get; set; }
        }
        public List<float> ShortWavesLeft { get; set; } = new List<float>();
        public List<float> ShortWavesRight { get; set; } = new List<float>();
        public List<float> MediumWavesLeft { get; set; } = new List<float>();
        public List<float> MediumWavesRight { get; set; } = new List<float>();
        public List<float> LongWavesLeft { get; set; } = new List<float>();
        public List<float> LongWavesRight { get; set; } = new List<float>();
        public List<Digression> Impuls { get; set; } = new List<Digression>();
        public List<int> Meters { get; set; } = new List<int>();
    }
}

