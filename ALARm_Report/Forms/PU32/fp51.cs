using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
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
    /// <summary>
    /// График диаграммы сводной по доп параметрам
    /// </summary>
    public class fp51 : Report
    {

        private int widthInPixel = 622;
        private float widthImMM = 155;
        private int xBegin = -11;

        private float gaspRightPosition = 14f;
        private float gapsKoef = 10f / 60f;

        private float zazorKoef = 1;
        private float MMToPixelChart(float mm)
        {
            return widthInPixel / widthImMM * mm + xBegin;
        }
        private string MMToPixelChartString(float mm)
        {
            return (widthInPixel / widthImMM * mm + xBegin).ToString().Replace(",", ".");
        }
        private string MMToPixelChartWidthString(float mm)
        {
            return (widthInPixel / widthImMM * mm).ToString().Replace(",", ".");
        }

        private readonly int LabelsDivWidthInPixel = 550;
        private readonly float LabelsDivWidthInMM = 146;
        private readonly float BottomLabelHeightInMM = 1.6f;
        private string MMToPixelLabel(float mm)
        {
            return (LabelsDivWidthInPixel / LabelsDivWidthInMM * mm).ToString().Replace(",", ".");
        }

        private float gaugeKoef = 0.5f;
        private float gaugePosition = 110f;

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
                List<Curve> curves = (MainTrackStructureService.GetCurves(distanceId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var road = AdmStructureService.GetUnit(AdmStructureConst.AdmDirection, distance.Parent_Id) as AdmUnit;
                var roadName = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);
                var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;
                CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;

                var tripProcesses = RdStructureService.GetProcess(period, distanceId, ProcessType.VideoProcess);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
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


                        //kms = kms.Where(Km => ((float)(float)filters[0].Value <= Km.Number && Km.Number <= (float)(float)filters[1].Value)).ToList();
                        //var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                        //if (kms.Count() == 0) continue;




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
                        //kms = kms.Where(Km => ((float)(float)filters[0].Value <= Km.Number && Km.Number <= (float)(float)filters[1].Value)).ToList();
                        //--------------------------------------------


                        progressBar.Maximum = tripProcesses.Count();

                        progressBar.Value = lkm.Count();
                        XElement addParam = new XElement("addparam",
                            new XAttribute("date_statement", tripProcess.Date_Vrem.ToString("dd.MM.yyyy")),
                            new XAttribute("distance", distance.Code),
                             new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("km", lkm.Min() + "-" + lkm.Max()),
                            new XAttribute("napr", ""),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("ps", tripProcess.Car),
                            new XAttribute("pch", distance.Code),
                            new XAttribute("road", roadName),
                            new XAttribute("check", "" + tripProcess.GetProcessTypeName),
                            new XAttribute("track", curvesAdmUnit.Track),
                            new XAttribute("direction", curvesAdmUnit.Direction),

                            new XAttribute("viewbox", "137 -10 600 300"),
                            new XAttribute("minY", 0),
                            new XAttribute("maxY", 1000),
                            new XAttribute("minYLine", 25),
                            new XElement("xgrid",
                            new XAttribute("x0", xBegin),
                            new XAttribute("x00", MMToPixelChart(5)),
                            new XAttribute("x000", MMToPixelChart(26)),
                            new XAttribute("x0000", MMToPixelChart(4.5f)),

                            new XAttribute("x1", MMToPixelChart(31)),
                            new XAttribute("l1", MMToPixelChart(33.89f)),
                            new XAttribute("l2", MMToPixelChart(53.82f)),
                            new XAttribute("l3", MMToPixelChart(33.39f)),

                            new XAttribute("x2", MMToPixelChart(57)),
                            new XAttribute("l4", MMToPixelChart(60.05f)),
                            new XAttribute("l5", MMToPixelChart(78.74f)),
                            new XAttribute("l6", MMToPixelChart(59.55f)),

                            new XAttribute("x11", MMToPixelChart(31)),
                            new XAttribute("x111", MMToPixelChart(31)),

                            new XAttribute("x3", MMToPixelChart(82)),
                            new XAttribute("l7", MMToPixelChart(84.72f)),
                            new XAttribute("l8", MMToPixelChart(102.91f)),
                            new XAttribute("l9", MMToPixelChart(84.22f)),

                            new XAttribute("x4", MMToPixelChart(106)),
                            new XAttribute("l10", MMToPixelChart(108.64f)),
                            new XAttribute("l11", MMToPixelChart(124.84f)),
                            new XAttribute("l12", MMToPixelChart(108.15f)),

                            new XAttribute("x41", MMToPixelChart(128)),
                            new XAttribute("l13", MMToPixelChart(130.57f)),
                            new XAttribute("l14", MMToPixelChart(152.25f)),
                            new XAttribute("l15", MMToPixelChart(130.08f)),

                            //астынгы сызык
                            new XAttribute("lineLow", widthInPixel - xBegin),

                            new XAttribute("ticks", MMToPixelChart(146) - widthInPixel / widthImMM / 4),
                            new XAttribute("x5", MMToPixelChart(151)),
                            new XAttribute("picket", MMToPixelChart(146) + (MMToPixelChart(152.5f) - MMToPixelChart(151f)) / 2),
                            new XAttribute("x6", MMToPixelChart(152.5f)),
                            new XAttribute("x7", MMToPixelChart(155)),
                            new XAttribute("x8", MMToPixelChart(151f) + (MMToPixelChart(152.5f) - MMToPixelChart(151f)) / 2),
                            new XElement("x", MMToPixelChartString(6.5f)),
                            new XElement("x", MMToPixelChartString(7.75f)),
                            new XElement("x", MMToPixelChartString(9f)),
                            new XElement("x", MMToPixelChartString(20.5f)),
                            new XElement("x", MMToPixelChartString(21.75f)),
                            new XElement("x", MMToPixelChartString(23f)),
                            new XElement("x", MMToPixelChartString(32f)),
                            new XElement("x", MMToPixelChartString(34f)),
                            new XElement("x", MMToPixelChartString(35f)),
                            new XElement("x", MMToPixelChartString(37f)),
                            //НПК лев
                            new XElement("x", MMToPixelChartString(57.5f)),
                            new XElement("x", MMToPixelChartString(60f)),
                            new XElement("x", MMToPixelChartString(61.7f)),
                            new XElement("x", MMToPixelChartString(62.5f)),
                            new XElement("x", MMToPixelChartString(63.3f)),
                            new XElement("x", MMToPixelChartString(64.1f)),
                            new XElement("x", MMToPixelChartString(67f)),
                            //НПК пр
                            new XElement("x", MMToPixelChartString(71.5f)),
                            new XElement("x", MMToPixelChartString(74f)),
                            new XElement("x", MMToPixelChartString(75.7f)),
                            new XElement("x", MMToPixelChartString(76.5f)),
                            new XElement("x", MMToPixelChartString(77.3f)),
                            new XElement("x", MMToPixelChartString(78.1f)),
                            new XElement("x", MMToPixelChartString(81f))
                            ));
                        int picket = 20;
                        while (picket <= 121)
                        {
                            addParam.Add(new XElement("picket", picket));
                            picket = picket + 10;
                        }

                        addParam.Add(new XAttribute("final-picket", picket));

                        //var checkingRoad = MainParametersService.GetMainParameterscheckingRoad(1666);

                        var CCCPspeed = new XElement("graphics");
                        var kmElements = new XElement("km");
                        var speedlimit = new XElement("speedlimit");
                        double StrLInd = 5;

                        var CCCPsred1 = new List<double>();

                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        var CCCPsred = new List<float>();

                        var Skewness_SKO = new List<float>();
                        var SSSP_speed = new List<double>();
                        var Speed = new List<float>();
                        var Vpz = -1;
                        var prevPkt = -1;

                        var mainPar = MainParametersService.GetMainParametersFromDB(tripProcess.Trip_id);
                        //SelectedData.First() / (0.002 * (testMinInd - 500))
                        var mainParam = mainPar.Where(o => (lkm.Min() <= o.Km && o.Km <= lkm.Max())).ToList();
                        if (mainParam.Count == 0) continue;

                        //находим максимальную скорость сссп
                        var ve = 250;
                        var kol = (ve / 10) * 10;
                        for (int i = 0; i <= kol; i += 10)
                        {
                            speedlimit.Add(new XElement("text", i,
                                            new XAttribute("y", kol - i),
                                            new XAttribute("x", -12)));
                        }

                        addParam.Add(speedlimit);

                        addParam.Add(new XAttribute("directNaprav", tripProcess.Direction));
                        addParam.Add(new XAttribute("Uchastok", lkm.Min() + " - " + lkm.Max()));
                        addParam.Add(new XAttribute("Put", ""));
                        addParam.Add(new XAttribute("TravelDate", tripProcess.Date_Vrem.ToString("dd.MM.yyyy")));
                        addParam.Add(new XAttribute("CarNumber", tripProcess.Car));

                        var speed = new List<Speed>();
                        var Vgruz = -1;
                        progressBar.Maximum = lkm.Count();





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
                        //------------------------
                        var vPassList = new List<int> { };
                        var vFreigList = new List<int> { };

                        foreach (var kilometer in lkm)
                        {
                            var Cv = new List<float>();

                            //if (StrLInd > 1000) break;

                            progressBar.Value = lkm.IndexOf(kilometer) + 1;

                            speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, "1") as List<Speed>;
                            Vpz = speed.Count > 0 ? speed.Last().Passenger : -1;
                            Vgruz = speed.Count > 0 ? speed.Last().Freight : -1;

                            vPassList.Add(Vpz);
                            vFreigList.Add(Vgruz);


                            var KmWrite = false;
                            //var mainPar = mainParam.Where(o => o.Km == km.Number).ToList();

                            var current = mainParam.Where(o => o.Km == kilometer).ToList();
                            if (current.Count == 0) continue;

                            foreach (var elem in current)
                            {
                                Cv.Add(elem.Cv);
                            }

                            var C_list = LinearCo.Select(o => Math.Abs(o - Cv.Average())).ToList();

                            var min = C_list.Min();
                            var ind = C_list.IndexOf(min);
                            var sssp = LinearSSSP[ind];

                            CCCPsred1.Add(sssp);


                            foreach (var elem in current)
                            {
                                {
                                    if (KmWrite == false)
                                    {
                                        kmElements.Add(new XElement("text", kilometer,
                                                new XAttribute("y", StrLInd),
                                                new XAttribute("x", -(kol + 18))));
                                        KmWrite = true;
                                    }
                                    if (sssp > Vpz)
                                    {
                                        var y = kol;
                                        var h = (widthInPixel / widthImMM);

                                        CCCPspeed.Add(new XElement("CCCPspeedBYpiket",
                                            new XAttribute("x", StrLInd),
                                            new XAttribute("y", (y - sssp).ToString().Replace(",", ".")),
                                            new XAttribute("style", "fill:lightgreen"),
                                            new XAttribute("h", sssp.ToString().Replace(",", "."))
                                            )
                                        );
                                        StrLInd += 10;

                                    }

                                    if (sssp < Vpz)
                                    {
                                        var y = kol;
                                        var h = (widthInPixel / widthImMM);
                                        CCCPspeed.Add(new XElement("CCCPspeedBYpiket",
                                            new XAttribute("x", StrLInd),
                                            new XAttribute("y", (y - sssp).ToString().Replace(",", ".")),
                                            new XAttribute("style", "fill:red"),
                                            new XAttribute("h", sssp.ToString().Replace(",", "."))
                                            )
                                        );
                                        StrLInd += 10;
                                    }

                                    if (sssp == Vpz)
                                    {
                                        var y = kol;
                                        var h = (widthInPixel / widthImMM);

                                        CCCPspeed.Add(new XElement("CCCPspeedBYpiket",
                                            new XAttribute("x", StrLInd),
                                            new XAttribute("y", (y - sssp).ToString().Replace(",", ".")),
                                            new XAttribute("style", "fill: blue"),
                                            new XAttribute("h", sssp.ToString().Replace(",", "."))
                                            )
                                        );
                                        StrLInd = StrLInd + 10;
                                    }


                                    SSSP_speed.Clear();
                                    //prevPkt = pkt;
                                }
                                SSSP_speed.Add(sssp);
                            }
                        }

                        var VP = "";
                        var VF = "";

                        var prev_vp = -1;
                        var prev_vf = -1;

                        var prevlocalInd = -1;
                        for (int i = 0; i < vPassList.Count; i++)
                        {
                            if (prev_vp == -1)
                            {
                                VP += $"{ 5 },{ kol - vPassList[i] } ";
                                VF += $"{ 5 },{ kol - vFreigList[i] } ";
                            }

                            if (prev_vp != -1 && vPassList[i] != prev_vp)
                            {
                                VP += $"{ prevlocalInd * 10 + 5 },{ kol - vPassList[i] } ";
                                VF += $"{ prevlocalInd * 10 + 5 },{ kol - vFreigList[i] } ";
                            }

                            VP += $"{ (i + 1) * 10 + 5 },{ kol - vPassList[i] } ";
                            VF += $"{ (i + 1) * 10 + 5 },{ kol - vFreigList[i] } ";

                            prev_vp = vPassList[i];
                            prev_vf = vFreigList[i];
                            prevlocalInd = (i + 1);
                        }

                        addParam.Add(
                            new XElement("cccpsred", "ср.СССП",

                                new XAttribute("v", kol - CCCPsred1.Average()),
                                new XAttribute("cccp", CCCPsred1.Average().ToString("0")),

                                new XAttribute("dlina", StrLInd)
                            ),
                            new XElement("vpass", "Vпасс",
                                new XAttribute("polylinePass", VP),
                                new XAttribute("v", kol - 80),
                                new XAttribute("vpz", Vpz),
                                new XAttribute("dlina", StrLInd)
                            ),
                            new XElement("vgruz", "Vгруз",
                                new XAttribute("polylineFreig", VF),
                                new XAttribute("v", kol - 70),
                                new XAttribute("vgruz", Vgruz),
                                new XAttribute("dlina", StrLInd)
                            ),
                            new XElement("MaxSpeedLimit", kol)
                            );
                        progressBar.Value = 0;

                        addParam.Add(kmElements);
                        addParam.Add(CCCPspeed);
                        report.Add(addParam);

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