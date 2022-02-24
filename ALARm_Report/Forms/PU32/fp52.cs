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
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm_Report.Forms
{
    public class fp52 : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar )
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

            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                List<Curve> curves = (MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
                XDocument xdReport = new XDocument();
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);


                var tripProcesses = RdStructureService.GetProcess(period, parentId, ProcessType.VideoProcess);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                //tripProcesses.Select(o => o.TrackID == admTracksId.ToList());
                XElement report = new XElement("report");

                foreach (var tripProcess in tripProcesses)
                {
                    //track Узнать трак если совпадает с трипс (Тракайди)
                    //continiu
                    foreach (var track_id in admTracksId)
                    { 
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
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id, track_id);
                        if (kms.Count() == 0) continue;

                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = kms.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = kms.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        kms = kms.Where(o => ((float)(float)filters[0].Value <= o && o <= (float)(float)filters[1].Value)).ToList();
                        //--------------------------------------------

                        progressBar.Maximum = kms.Count();

                        XElement tripElem = new XElement("trip",
                             new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("ps", tripProcess.Car),
                            new XAttribute("periodDate", period.Period ),
                            new XAttribute("direction", tripProcess.DirectionName + "(" + tripProcess.DirectionCode + ")"),
                            new XAttribute("put", "" + trackName),
                            new XAttribute("Date_Vrem", "" + tripProcess.Date_Vrem.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))),
                            new XAttribute("chief", "" + tripProcess.Chief),
                            new XAttribute("check", "" + tripProcess.GetProcessTypeName),
                            new XAttribute("road", roadName),
                            new XAttribute("km", kms.Min() + "-" + kms.Max()),
                            new XAttribute("distance", distance.Code)
                            );

                        var mainPar = MainParametersService.GetMainParametersFromDB(tripProcess.Trip_id);
                        if (mainPar.Count == 0) continue;
                        mainPar = mainPar.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList();

                        foreach (var kilometer in kms)
                        {
                            progressBar.Value = kms.IndexOf(kilometer) + 1;


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

                            var mm = MainParametersService.GetMainParametersSKOvedomDB(kilometer, tripProcess.Trip_id);
                            if (mm.Count == 0) continue;
                            //if (mainPar.Count == 0) continue;
                            foreach (var elem in mm)
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




                            //var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromText(kilometer);
                            //var mainPar = MainParametersService.GetMainParametersSKOvedom(kilometer);

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
                                            item.Km * 1000 + item.Piket * 100 <= ar.Final_Km * 1000 + ar.Final_M+100) 
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
                                        if (item.Km == km && item.Piket == (int)metr /100+1) 
                                        {
                                            item.ART = ar.Type_Id == 1 ? "Мост безбалластный" : "";
                                        }
                                    }
                                }
                            }

                            var current_km = mainPar.Where(o => o.Km == kilometer).ToList();
                            if (current_km.Count == 0) continue;

                            foreach (var elem in current_km)
                            {
                                tripElem.Add(new XElement("Note",
                                    new XAttribute("Km", elem.Km),
                                    new XAttribute("Meter", /*$"{elem.Piket}00"*/elem.Meter.ToString("0")),
                                    new XAttribute("Gauge", elem.Gauge_avg.ToString("0.00")),
                                    new XAttribute("Rihtovka", elem.Stright_left_avg.ToString("0.00")),
                                    new XAttribute("Lvl", elem.Level_avg.ToString("0.00")),
                                    new XAttribute("Gauge_SKO", elem.Gauge_SKO.ToString("0.00")),
                                    new XAttribute("Skewness_SKO", elem.Skewness_SKO.ToString("0.00")),
                                    new XAttribute("Drawdown_left_SKO", elem.Drawdown_left_SKO.ToString("0.00")),
                                    new XAttribute("Drawdown_right_SKO", elem.Drawdown_right_SKO.ToString("0.00")),
                                    new XAttribute("Stright_right", elem.Stright_right_SKO.ToString("0.00")),
                                    new XAttribute("Sssp_vert", elem.Sssp_vert.ToString("0")),
                                    new XAttribute("Sssp_gor", sssp.ToString("0")),
                                    new XAttribute("Strelka", switches.Count > 0 ? (switches[0].Meter / 100 + 1 == elem.Piket ? "1" : "-") : "-"),
                                    new XAttribute("Object", elem.ART + " " + elem.Station)
                                    )
                                );
                            }
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
