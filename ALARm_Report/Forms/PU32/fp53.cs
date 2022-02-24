using ALARm.Core;
using ALARm.Core.Report;
using ALARm.DataAccess;
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
    public class fp53 : Report
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
                List<Curve> curves = (MainTrackStructureService.GetCurves(distanceId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                //var road = AdmStructureService.GetUnit(AdmStructureConst.AdmDirection, distance.Parent_Id) as AdmUnit;

                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                var tripProcesses = RdStructureService.GetProcess(period, distanceId, ProcessType.VideoProcess);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var trackId in admTracksId)
                    {

                        var trackName = AdmStructureService.GetTrackName(trackId);
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kms = RdStructureService.GetKilometersByTrip(trip);
                        kms = kms.Where(o => o.Track_id == trackId).ToList();
                        if (kms.Count() == 0) continue;



                       
                        var lkm = kms.Select(o => o.Number).ToList();

                        if (lkm.Count() == 0) continue;

                        //var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
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
                        ////--------------------------------------------

                        progressBar.Maximum = lkm.Count();

                        XElement tripElem = new XElement("trip",
                             new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("ps", tripProcess.Car),
                            new XAttribute("km", kms.Select(o => o.Number).Min() + "-" + kms.Select(o => o.Number).Max()),
                            new XAttribute("track", 1),
                            new XAttribute("check", " " + tripProcess.GetProcessTypeName),
                            new XAttribute("direction", tripProcess.DirectionName + "(" + tripProcess.DirectionCode + ")"),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("road", road),
                            new XAttribute("date_statement", tripProcess.Date_Vrem.ToString("dd.MM.yyyy")),
                            new XAttribute("track_info", $"{tripProcess.DirectionName}({tripProcess.DirectionCode}) Путь:{trackName}"),
                            new XAttribute("distance", distance.Code)
                            );

                        XElement Note = new XElement("Note");

                        int iter = 1;
                        var CCCPsred = new List<double>();

                        //var mainPar = MainParametersService.GetMainParametersFromDB(tripProcess.Trip_id);

                        //mainPar = mainPar.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList();

                        //CCCPsred.AddRange(mainPar.Select(o => o.SSSP_speed));
                        var Speed = new List<float>();
                        var Vpz = 200;
                        var speed = new List<Speed>();
                        var PrevKm = -1;

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


                        var mainParam = MainParametersService.GetMainParametersFromDB(tripProcess.Trip_id);
                        var SSPAvg = new List<double> { };

                        foreach (var km in kms)
                        {
                            var Skewness_SKO = new List<float>();
                            var Drawdown_left_SKO = new List<float>();
                            var Drawdown_right_SKO = new List<float>();
                            var Gauge_SKO = new List<float>();
                            var Stright_left_SKO = new List<float>();
                            var Stright_right_SKO = new List<float>();
                            var SSSP_speed = new List<float>();
                            var Cv = new List<float>();
                            var Cr = new List<float>();
                            var Co = new List<float>();


                            progressBar.Value = kms.IndexOf(km) + 1;

                            var kilometer = km.Number;

                            speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, "1") as List<Speed>;
                            Vpz = speed.Count > 0 ? speed[0].Passenger : -1;


                            var mainPar = mainParam.Where(o => o.Km == km.Number).ToList();

                            if (mainPar.Count == 0) continue;
                            foreach (var elem in mainPar)
                            {
                                Skewness_SKO.Add(elem.Skewness_SKO);
                                Drawdown_left_SKO.Add(elem.Drawdown_left_SKO);
                                Drawdown_right_SKO.Add(elem.Drawdown_right_SKO);
                                Gauge_SKO.Add(elem.Gauge_SKO);
                                Stright_left_SKO.Add(elem.Stright_left_SKO);
                                Stright_right_SKO.Add(elem.Stright_right_SKO);
                                Cv.Add(elem.Cv);
                                Cr.Add(elem.Cr);
                                Co.Add(elem.Co);
                                   
                            }

                            var calc_c0 = Cv.Average() * 0.615 + Cr.Average() * 0.39;
                            var C_list = LinearCo.Select(o => Math.Abs(o - Cv.Average())).ToList();

                            var min = C_list.Min();
                            var ind = C_list.IndexOf(min);

                            var sssp = LinearSSSP[ind];
                            SSPAvg.Add(sssp);

                            //СССП меньше Vпз для пассажирских поездов
                            if (Vpz < sssp) continue;

                            Note = new XElement("Note",
                                    new XAttribute("n", iter),
                                    new XAttribute("km", kilometer),
                                    new XAttribute("Perekos", Skewness_SKO.Average().ToString("0.00")),
                                    new XAttribute("prosR", Drawdown_left_SKO.Average().ToString("0.00")),
                                    new XAttribute("prosL", Drawdown_right_SKO.Average().ToString("0.00")),
                                    new XAttribute("cv", Cv.Average().ToString("0.00")),
                                    new XAttribute("shablon", Gauge_SKO.Average().ToString("0.00")),
                                    new XAttribute("rihtR", Stright_right_SKO.Average().ToString("0.00")),
                                    new XAttribute("rihtL", Stright_left_SKO.Average().ToString("0.00")),
                                    new XAttribute("cr", Cr.Average().ToString("0.00")),
                                    new XAttribute("co", calc_c0.ToString("0.00")),
                                    new XAttribute("cccp", sssp.ToString("0.00")),
                                    new XAttribute("vpz", /*speed.Count > 0 ? speed[0].Passenger.ToString() :*/ "" + Vpz)
                                    );

                            CCCPsred.Add(sssp);
                            iter = iter + 1;
                            tripElem.Add(Note);

                            PrevKm = kilometer;
                            

                        }
                        if (SSPAvg.Count != 0)
                        {
                            tripElem.Add(new XAttribute("sred", SSPAvg.Average().ToString("0")));
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
