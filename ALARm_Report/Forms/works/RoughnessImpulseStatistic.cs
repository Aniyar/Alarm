using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using Dapper;
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
    public class RoughnessImpulseStatistic : Report
    {
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

                var mainProcesses = RdStructureService.GetMainParametersProcesses(period, distanceId);
                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                foreach (var process in mainProcesses)
                {
                    var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;

                    var roadName = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);

                    var road = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;

                    XElement xePages = new XElement("pages");

                    xePages.Add(new XAttribute("distance", ((AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId)).Code),
                        new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        //     new XAttribute("dateandtype", period.Period + " " + process.GetProcessTypeName),
                        new XAttribute("check", process.GetProcessTypeName),
                        new XAttribute("periodDate", period.Period),
                        new XAttribute("DKI", process.Car),
                        new XAttribute("way", roadName),
                        new XAttribute("car", process.Car),
                        new XAttribute("chief", process.Chief),
                        new XAttribute("direction", process.DirectionName));

                    foreach (var trackId in admTracksId)
                    {
                        var trip = RdStructureService.GetTrip(process.Trip_id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);

                        kilometers = kilometers.Where(o => o.Track_id == trackId).ToList();

                        if (kilometers.Count == 0)
                            continue;

                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var lkm = kilometers.Select(o => o.Number).ToList();

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        kilometers = kilometers.Where(Km => ((float)(float)filters[0].Value <= Km.Number && Km.Number <= (float)(float)filters[1].Value)).ToList();
                        kilometers = (trip.Travel_Direction == Direction.Reverse ? kilometers.OrderBy(o => o.Number) : kilometers.OrderByDescending(o => o.Number)).ToList();

                        //var impulse = RdStructureService.GetRdTables(process, 2) as List<RdStatisticRoughnessImpulse>;

                        if (kilometers.Count < 1)
                        {
                            continue;
                        }


                        var trackName = AdmStructureService.GetTrackName(trackId);

                        XElement xeTrips = new XElement("trips",
                            new XAttribute("tripinfo", $"{process.DirectionName} Путь - {trackName}"));

                        int right1glob = 0, right2glob = 0, right3glob = 0, right4glob = 0,
                            left1glob = 0, left2glob = 0, left3glob = 0, left4glob = 0;

                        foreach (var kms in kilometers)
                        {
                            GetTestData(kms.Number);
                            //var impulses = AdditionalParametersService.GetImpulsesByKm(kms.Number);

                            var impulses = Impuls;

                            if (impulses.Count < 1) continue;

                            int right1 = 0, right2 = 0, right3 = 0, right4 = 0, left1 = 0, left2 = 0, left3 = 0, left4 = 0;

                            foreach (var elems in impulses)
                            {
                                if (elems.Threat == Threat.Right)
                                {
                                    if (1.0 >= elems.Intensity_ra && elems.Intensity_ra > 0.0)
                                    {
                                        right1 += 1;
                                        right1glob += 1;
                                    }
                                    else if (2.0 >= elems.Intensity_ra && elems.Intensity_ra > 1.0)
                                    {
                                        right2 += 1;
                                        right2glob += 1;
                                    }
                                    else if (3.0 >= elems.Intensity_ra && elems.Intensity_ra > 2.0)
                                    {
                                        right3 += 1;
                                        right3glob += 1;
                                    }
                                    else if (elems.Intensity_ra > 3.0)
                                    {
                                        right4 += 1;
                                        right4glob += 1;
                                    }
                                }
                                else
                                {
                                    if (1.0 >= elems.Intensity_ra && elems.Intensity_ra > 0.0)
                                    {
                                        left1 += 1;
                                        left1glob += 1;
                                    }
                                    else if (2.0 >= elems.Intensity_ra && elems.Intensity_ra > 1.0)
                                    {
                                        left2 += 1;
                                        left2glob += 1;
                                    }
                                    else if (3.0 >= elems.Intensity_ra && elems.Intensity_ra > 2.0)
                                    {
                                        left3 += 1;
                                        left3glob += 1;
                                    }
                                    else if (elems.Intensity_ra > 3.0)
                                    {
                                        left4 += 1;
                                        left4glob += 1;
                                    }
                                }
                            }

                            XElement xeElements = new XElement("elements",
                                new XAttribute("km", kms.Number),
                                new XAttribute("rightbefore1", right1 != 0 ? right1.ToString() : "-"),
                                new XAttribute("rightbefore2", right2 != 0 ? right2.ToString() : "-"),
                                new XAttribute("rightbefore3", right3 != 0 ? right3.ToString() : "-"),
                                new XAttribute("rightafter3", right4 != 0 ? right4.ToString() : "-"),
                                new XAttribute("leftbefore1", left1 != 0 ? left1.ToString() : "-"),
                                new XAttribute("leftbefore2", left2 != 0 ? left2.ToString() : "-"),
                                new XAttribute("leftbefore3", left3 != 0 ? left3.ToString() : "-"),
                                new XAttribute("leftafter3", left4 != 0 ? left4.ToString() : "-"));

                            xeTrips.Add(xeElements);
                        }

                        XElement summ = new XElement("summ",
                                    new XAttribute("rightbefore1", right1glob != 0 ? right1glob.ToString() : "-"),
                                    new XAttribute("rightbefore2", right2glob != 0 ? right2glob.ToString() : "-"),
                                    new XAttribute("rightbefore3", right3glob != 0 ? right3glob.ToString() : "-"),
                                    new XAttribute("rightafter3", right4glob != 0 ? right4glob.ToString() : "-"),
                                    new XAttribute("leftbefore1", left1glob != 0 ? left1glob.ToString() : "-"),
                                    new XAttribute("leftbefore2", left2glob != 0 ? left2glob.ToString() : "-"),
                                    new XAttribute("leftbefore3", left3glob != 0 ? left3glob.ToString() : "-"),
                                    new XAttribute("leftafter3", left4glob != 0 ? left4glob.ToString() : "-"));
                        xePages.Add(summ);

                        xePages.Add(xeTrips);
                    }

                    report.Add(xePages);
                }
                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_RoughnessImpulseStatistic.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_RoughnessImpulseStatistic.html");
            }
        }
        

        private void GetTestData(int number)
        {
            var connection = new Npgsql.NpgsqlConnection(ALARm.DataAccess.Helper.ConnectionString());
            var ShortData = connection.Query<DataFlow>($@"SELECT * FROM testdata_242 where km = {number} limit 5000").ToList();

            var shortl = ShortData.Select(o => o.Diff_l / 8.0 < 1 / 8.0 ? 0 : o.Diff_l / 8.0).ToList();
            var shortr = ShortData.Select(o => o.Diff_r / 8.0 < 1 / 8.0 ? 0 : o.Diff_r / 8.0).ToList();

            Meters.AddRange(ShortData.Select(o => o.Meter).ToList());


            var val = new List<double> { };
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
                            H = val.Max() * 0.8,
                            H_ind = val_ind[val.IndexOf(val.Max())],
                        };

                        bolshe0.Add(d);

                        inn = false;
                        val.Clear();
                        val_ind.Clear();
                    }
                }
            }


            var val_r = new List<double> { };
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
                            H = val_r.Max() * 0.8,
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

                LongWavesLeft.Add(l * 0.1);
                MediumWavesLeft.Add(m * 0.1);
                ShortWavesLeft.Add(s * 0.1);

                LongWavesRight.Add(lr * 0.1);
                MediumWavesRight.Add(mr * 0.1);
                ShortWavesRight.Add(sr * 0.1);

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
        }
        public class DATA0
        {
            public double H { get; set; }
            public int H_ind { get; set; }
            public double Count { get; set; }
            public double HR { get; internal set; }
        }

        public class DataFlow
        {
            public int Km { get; set; }
            public int Meter { get; set; }
            public double Diff_l { get; set; }
            public double Diff_r { get; set; }
        }

        public List<double> ShortWavesLeft { get; set; } = new List<double>();
        public List<double> ShortWavesRight { get; set; } = new List<double>();
        public List<double> MediumWavesLeft { get; set; } = new List<double>();
        public List<double> MediumWavesRight { get; set; } = new List<double>();
        public List<double> LongWavesLeft { get; set; } = new List<double>();
        public List<double> LongWavesRight { get; set; } = new List<double>();
        public List<Digression> Impuls { get; set; } = new List<Digression>();
        public List<int> Meters { get; set; } = new List<int>();
    }
}
