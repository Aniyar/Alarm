using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class GRK_param : Report
    {
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
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
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                List<Curve> curves = (MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);
                distance.Name = distance.Name.Replace("ПЧ-", "");

                var tripProcesses = RdStructureService.GetProcess(period, parentId, ProcessType.VideoProcess);
                //var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
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
                        var kms = RdStructureService.GetKilometersByTrip(trip);
                        if (!kms.Any()) continue;

                        kms = kms.Where(o => o.Track_id == track_id).ToList();

                        trip.Track_Id = track_id;
                        var lkm = kms.Select(o => o.Number).ToList();

                        if (lkm.Count() == 0) continue;




                        //sssp calculate------------
                        double[] Со = { 0.40, 0.44, 0.51, 0.59, 0.68, 0.80, 1.0, 1.28, 1.55, 1.85, 2.25, 2.75 };
                        int[] SSSP = { 250, 220, 200, 180, 160, 140, 120, 100, 80, 60, 40, 15 };

                        var LinearSSSP = new List<double> { };
                        var LinearCo = new List<double> { };

                        for (int x = 0; x < SSSP.Count() - 1; x++)
                        {
                            var k = (SSSP[x + 1] - SSSP[x]) / (Со[x + 1] - Со[x]);
                            var d = (Со[x + 1] - Со[x]) / 50.0;

                            for (int y = 0; y < 50; y++)
                            {
                                var SSSP_linear = SSSP[x] + k * y * d;

                                var Co_linear = Со[x] + y * d;

                                LinearSSSP.Add(SSSP_linear);
                                LinearCo.Add(Co_linear);
                            }
                        }
                        //-------------------------



                        //var kms = RdStructureService.GetKilometersByTrip(trip);

                        //if (kms.Count() == 0) continue;

                        //kms = kms.OrderBy(o => o.Number).ToList();

                        //if (kms.Count() == 0) continue;

                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        lkm = lkm.Where(o => ((float)(float)filters[0].Value <= o && o <= (float)(float)filters[1].Value)).ToList();
                        //////--------------------------------------------

                        progressBar.Maximum = kms.Count();

                        XElement tripElem = new XElement("trip",
                             new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                            new XAttribute("direction", tripProcess.DirectionName),
                            new XAttribute("directioncode", tripProcess.DirectionCode),
                            new XAttribute("check", tripProcess.GetProcessTypeName),
                            new XAttribute("track", trackName),
                            new XAttribute("road", road),
                            new XAttribute("distance", distance.Code),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("ps", tripProcess.Car)
                            );

                        
                        var iter = 1;
                        var PrevKm = -1;
                        var mainParam = MainParametersService.GetMainParametersFromDB(tripProcess.Trip_id);
                        foreach (var kmm in kms)
                        {
                            progressBar.Value = kms.IndexOf(kmm) + 1;

                            var Skewness_SKO = new List<float>();
                            var Drawdown_left_SKO = new List<float>();
                            var Drawdown_right_SKO = new List<float>();
                            var Gauge = new List<float>();
                            var Stright_left = new List<float>();
                            var Stright_right = new List<float>();
                            var SSSP_speed = new List<float>();
                            var Cv = new List<float>();
                            var Cr = new List<float>();
                            var Co = new List<float>();

                          
                       
                            var mainPar = mainParam.Where(o => o.Km == kmm.Number).ToList();
                          
                            if (mainPar.Count == 0) continue;


                            foreach (var elem in mainPar)
                            {
                                Skewness_SKO.Add(elem.Skewness_SKO);
                                Drawdown_left_SKO.Add(elem.Drawdown_left_SKO);
                                Drawdown_right_SKO.Add(elem.Drawdown_right_SKO);
                                Gauge.Add(elem.Gauge);
                                Stright_left.Add(elem.Stright_left);
                                Stright_right.Add(elem.Stright_right);
                                Cv.Add(elem.Cv);
                                Cr.Add(elem.Cr);
                                Co.Add(elem.Co);
                                
                            }

                            var calc_c0 = Cv.Average() * 0.615 + Cr.Average() * 0.39;
                            var C_list = LinearCo.Select(o => Math.Abs(o - calc_c0)).ToList();

                            var min = C_list.Min();
                            var ind = C_list.IndexOf(min);

                            var sssp = LinearSSSP[ind];
                            ///
                            var kilometer = kmm.Number;
                          
                            ///
                            var switches = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.MtoSwitch, track_id) as List<Switch>;
                            var ArtificialConstruction = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.MtoArtificialConstruction, track_id) as List<ArtificialConstruction>;
                            var StationSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.MtoStationSection, track_id) as List<StationSection>;

                            if (StationSection.Any())
                            {
                                foreach (var ar in StationSection)
                                {
                                    foreach (var item in mainPar)
                                    {
                                        if (item.Km * 1000 + item.Piket * 100 >= ar.Start_Km * 1000 + ar.Start_M &&
                                            item.Km * 1000 + item.Piket * 100 <= ar.Final_Km * 1000 + ar.Final_M + 100)
                                        {
                                            item.Station = ar.Station;
                                        }
                                    }
                                }
                            }
                            if (ArtificialConstruction.Any())
                            {
                                foreach (var ar in ArtificialConstruction)
                                {
                                    var km = (int)(ar.Km * 1000 + ar.Meter + ar.Len / 2.0) / 1000;
                                    var metr = (ar.Km * 1000 + ar.Meter + ar.Len / 2.0) % 1000;

                                    foreach (var item in mainPar)
                                    {
                                        if (item.Km == km && item.Piket == (int)metr / 100 + 1)
                                        {
                                            item.ART = ar.Type_Id == 1 ? "Мост безбалластный" : "";
                                        }
                                    }
                                }
                            }
                            var mainParamm = mainPar;
                            tripElem.Add(new XElement("note",
                                new XAttribute("n", iter),
                                    new XAttribute("km", kilometer),
                                    new XAttribute("Perekos", mainParamm.Any() ? mainParamm.Select(o => o.Skewness_SKO).ToList().Average().ToString("0.00") : "нет данных"),
                                    new XAttribute("level", mainParamm.Any() ? mainParamm.Select(o => o.Level_SKO).ToList().Average().ToString("0.00") : "нет данных"),//ToDo temperature
                                    new XAttribute("Pros.pr", mainParamm.Any() ? mainParamm.Select(o => o.Drawdown_right_SKO).ToList().Average().ToString("0.00") : "нет данных"),//ToDo temperature
                                    new XAttribute("Pros.lv", mainParamm.Any() ? mainParamm.Select(o => o.Drawdown_left_SKO).ToList().Average().ToString("0.00") : "нет данных"),
                                    new XAttribute("CB", calc_c0.ToString("0.00")),
                                    new XAttribute("Shablon", mainParamm.Any() ? mainParamm.Select(o => o.Gauge_SKO).ToList().Average().ToString("0.00") : "нет данных"),
                                    new XAttribute("Riht.lv", mainParamm.Any() ? mainParamm.Select(o => o.Stright_left_SKO).ToList().Average().ToString("0.00") : "нет данных"),
                                    new XAttribute("Riht.pr", mainParamm.Any() ? mainParamm.Select(o => o.Stright_right_SKO).ToList().Average().ToString("0.00") : "нет данных"))
                                    );
                            iter++;
                            PrevKm = kilometer;
                        }
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
