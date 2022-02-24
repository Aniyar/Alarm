//using ALARm.Core;
//using ALARm.Core.AdditionalParameteres;
//using ALARm.Core.Report;
//using ALARm.Services;
//using ALARm_Report.controls;
//using MetroFramework.Controls;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity.Infrastructure.Interception;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Xml;
//using System.Xml.Linq;
//using System.Xml.Xsl;

//namespace ALARm_Report.Forms
//{
//    /// <summary>
//    /// График диаграммы сводной по доп параметрам
//    /// </summary>
//    public class fp51 : Report
//    {

//        private int widthInPixel = 622;
//        private float widthImMM = 155;
//        private int xBegin = -11;

//        private float gaspRightPosition = 14f;
//        private float gapsKoef = 10f / 60f;

//        private float zazorKoef = 1;
//        private float MMToPixelChart(float mm)
//        {
//            return widthInPixel / widthImMM * mm + xBegin;
//        }
//        private string MMToPixelChartString(float mm)
//        {
//            return (widthInPixel / widthImMM * mm + xBegin).ToString().Replace(",", ".");
//        }
//        private string MMToPixelChartWidthString(float mm)
//        {
//            return (widthInPixel / widthImMM * mm).ToString().Replace(",", ".");
//        }

//        private readonly int LabelsDivWidthInPixel = 550;
//        private readonly float LabelsDivWidthInMM = 146;
//        private readonly float BottomLabelHeightInMM = 1.6f;
//        private string MMToPixelLabel(float mm)
//        {
//            return (LabelsDivWidthInPixel / LabelsDivWidthInMM * mm).ToString().Replace(",", ".");
//        }

//        private float gaugeKoef = 0.5f;
//        private float gaugePosition = 110f;

//        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
//        {
//            XDocument htReport = new XDocument();

//            using (XmlWriter writer = htReport.CreateWriter())
//            {
//                List<Curve> curves = (MainTrackStructureService.GetCurves(distanceId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
//                XDocument xdReport = new XDocument();
//                XElement report = new XElement("report");
//                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
//                var road = AdmStructureService.GetUnit(AdmStructureConst.AdmDirection, distance.Parent_Id) as AdmUnit;
//                var roadName = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);
//                var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;
//                CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;

//                var tripProcesses = RdStructureService.GetProcess(period, distanceId, ProcessType.VideoProcess);
//                if (tripProcesses.Count == 0)
//                {
//                    MessageBox.Show(Properties.Resources.paramDataMissing);
//                    return;
//                }
//                foreach (var tripProcess in tripProcesses)
//                {
//                    var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
//                    if (kms.Count() == 0) continue;

//                    ////Выбор километров по проезду-----------------
//                    var filterForm = new FilterForm();
//                    var filters = new List<Filter>();

//                    filters.Add(new FloatFilter() { Name = "Начало (км)", Value = kms.Min() });
//                    filters.Add(new FloatFilter() { Name = "Конец (км)", Value = kms.Max() });

//                    filterForm.SetDataSource(filters);
//                    if (filterForm.ShowDialog() == DialogResult.Cancel)
//                        return;

//                    kms = kms.Where(o => ((float)(float)filters[0].Value <= o && o <= (float)(float)filters[1].Value)).ToList();
//                    //--------------------------------------------


//                    progressBar.Maximum = tripProcesses.Count();

//                    progressBar.Value = kms.Count();
//                    XElement addParam = new XElement("addparam",
//                        new XAttribute("date_statement", tripProcess.Date_Vrem.ToString("dd.MM.yyyy")),
//                        new XAttribute("distance", distance.Code),
//                         new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
//                        new XAttribute("km", kms.Min() + "-" + kms.Max()),
//                        new XAttribute("napr", ""),
//                        new XAttribute("periodDate", period.Period),
//                        new XAttribute("ps", tripProcess.Car),
//                        new XAttribute("pch", distance.Code),
//                        new XAttribute("road", roadName),
//                        new XAttribute("check", "" + tripProcess.GetProcessTypeName),
//                        new XAttribute("track", curvesAdmUnit.Track),
//                        new XAttribute("direction", curvesAdmUnit.Direction),
                    
//                        new XAttribute("viewbox", "137 -10 600 300"),
//                        new XAttribute("minY", 0),
//                        new XAttribute("maxY", 1000),
//                        new XAttribute("minYLine", 25),
//                        new XElement("xgrid",
//                        new XAttribute("x0", xBegin),
//                        new XAttribute("x00", MMToPixelChart(5)),
//                        new XAttribute("x000", MMToPixelChart(26)),
//                        new XAttribute("x0000", MMToPixelChart(4.5f)),

//                        new XAttribute("x1", MMToPixelChart(31)),
//                        new XAttribute("l1", MMToPixelChart(33.89f)),
//                        new XAttribute("l2", MMToPixelChart(53.82f)),
//                        new XAttribute("l3", MMToPixelChart(33.39f)),

//                        new XAttribute("x2", MMToPixelChart(57)),
//                        new XAttribute("l4", MMToPixelChart(60.05f)),
//                        new XAttribute("l5", MMToPixelChart(78.74f)),
//                        new XAttribute("l6", MMToPixelChart(59.55f)),

//                        new XAttribute("x11", MMToPixelChart(31)),
//                        new XAttribute("x111", MMToPixelChart(31)),

//                        new XAttribute("x3", MMToPixelChart(82)),
//                        new XAttribute("l7", MMToPixelChart(84.72f)),
//                        new XAttribute("l8", MMToPixelChart(102.91f)),
//                        new XAttribute("l9", MMToPixelChart(84.22f)),

//                        new XAttribute("x4", MMToPixelChart(106)),
//                        new XAttribute("l10", MMToPixelChart(108.64f)),
//                        new XAttribute("l11", MMToPixelChart(124.84f)),
//                        new XAttribute("l12", MMToPixelChart(108.15f)),

//                        new XAttribute("x41", MMToPixelChart(128)),
//                        new XAttribute("l13", MMToPixelChart(130.57f)),
//                        new XAttribute("l14", MMToPixelChart(152.25f)),
//                        new XAttribute("l15", MMToPixelChart(130.08f)),

//                        //астынгы сызык
//                        new XAttribute("lineLow", widthInPixel - xBegin),

//                        new XAttribute("ticks", MMToPixelChart(146) - widthInPixel / widthImMM / 4),
//                        new XAttribute("x5", MMToPixelChart(151)),
//                        new XAttribute("picket", MMToPixelChart(146) + (MMToPixelChart(152.5f) - MMToPixelChart(151f)) / 2),
//                        new XAttribute("x6", MMToPixelChart(152.5f)),
//                        new XAttribute("x7", MMToPixelChart(155)),
//                        new XAttribute("x8", MMToPixelChart(151f) + (MMToPixelChart(152.5f) - MMToPixelChart(151f)) / 2),
//                        new XElement("x", MMToPixelChartString(6.5f)),
//                        new XElement("x", MMToPixelChartString(7.75f)),
//                        new XElement("x", MMToPixelChartString(9f)),
//                        new XElement("x", MMToPixelChartString(20.5f)),
//                        new XElement("x", MMToPixelChartString(21.75f)),
//                        new XElement("x", MMToPixelChartString(23f)),
//                        new XElement("x", MMToPixelChartString(32f)),
//                        new XElement("x", MMToPixelChartString(34f)),
//                        new XElement("x", MMToPixelChartString(35f)),
//                        new XElement("x", MMToPixelChartString(37f)),
//                        //НПК лев
//                        new XElement("x", MMToPixelChartString(57.5f)),
//                        new XElement("x", MMToPixelChartString(60f)),
//                        new XElement("x", MMToPixelChartString(61.7f)),
//                        new XElement("x", MMToPixelChartString(62.5f)),
//                        new XElement("x", MMToPixelChartString(63.3f)),
//                        new XElement("x", MMToPixelChartString(64.1f)),
//                        new XElement("x", MMToPixelChartString(67f)),
//                        //НПК пр
//                        new XElement("x", MMToPixelChartString(71.5f)),
//                        new XElement("x", MMToPixelChartString(74f)),
//                        new XElement("x", MMToPixelChartString(75.7f)),
//                        new XElement("x", MMToPixelChartString(76.5f)),
//                        new XElement("x", MMToPixelChartString(77.3f)),
//                        new XElement("x", MMToPixelChartString(78.1f)),
//                        new XElement("x", MMToPixelChartString(81f))
//                        ));
//                    int picket = 20;
//                    while (picket <= 121)
//                    {
//                        addParam.Add(new XElement("picket", picket));
//                        picket = picket + 10;
//                    }

//                    addParam.Add(new XAttribute("final-picket", picket));

//                    //var checkingRoad = MainParametersService.GetMainParameterscheckingRoad(1666);

//                    var CCCPspeedBYpiket = new XElement("graphics");
//                    var kmElements = new XElement("km");
//                    var speedlimit = new XElement("speedlimit");
//                    double StrLInd = 5;

//                    var CCCPsred1 = new List<float>();

//                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    var CCCPsred = new List<float>();

//                    var Skewness_SKO = new List<float>();
//                    var SSSP_speed = new List<float>();
//                    var Speed = new List<float>();
//                    var Vpz = -1;
//                    var prevPkt = -1;

//                    var mainPar = MainParametersService.GetMainParametersFromDB(tripProcess.Trip_id);

//                    mainPar = mainPar.Where(o => (kms.Min() <= o.Km && o.Km <= kms.Max())).ToList();

//                    CCCPsred1.AddRange(mainPar.Select(o => o.SSSP_speed));

//                    //находим максимальную скорость сссп
//                    var ve = CCCPsred1.Max() + 10;
//                    var kol = (int)(ve / 10) * 10;
//                    for (int i = 0; i <= kol; i += 10)
//                    {
//                        speedlimit.Add(new XElement("text", i,
//                                        new XAttribute("y", kol - i),
//                                        new XAttribute("x", -12)));
//                    }

//                    addParam.Add(speedlimit);

//                    addParam.Add(new XAttribute("directNaprav", tripProcess.Direction));
//                    addParam.Add(new XAttribute("Uchastok", kms.Min() + " - " + kms.Max()));
//                    addParam.Add(new XAttribute("Put", ""));
//                    addParam.Add(new XAttribute("TravelDate", tripProcess.Date_Vrem.ToString("dd.MM.yyyy")));
//                    addParam.Add(new XAttribute("CarNumber", tripProcess.Car));
                    
//                    var speed = new List<Speed>();
//                    var Vgruz = -1;
//                    progressBar.Maximum = kms.Count();

//                    foreach (var kilometer in kms)
//                    {
//                        if (StrLInd > 1000) break;

//                        progressBar.Value = kms.IndexOf(kilometer) + 1;

//                        speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, "1") as List<Speed>;
//                        Vpz = speed.Count > 0 ? speed[0].Passenger : -1 ;
//                        Vgruz = speed.Count > 0 ? speed[0].Freight : -1;

//                        var KmWrite = false;


//                        var current = mainPar.Where(o => o.Km == kilometer).ToList();
//                        if (current.Count == 0) continue;

//                        foreach (var elem in current)
//                        {
//                            //var pkt = elem.Piket;
//                            //prevPkt = prevPkt == -1 ? pkt : prevPkt;

//                            //if (pkt != prevPkt)
//                            {
//                                if (KmWrite == false)
//                                {
//                                    kmElements.Add(new XElement("text", kilometer,
//                                            new XAttribute("y", StrLInd ),
//                                            new XAttribute("x", -(kol + 18))));
//                                    KmWrite = true;
//                                }
//                                if (elem.SSSP_speed > Vpz)
//                                {
//                                    var y = kol;
//                                    var h = (widthInPixel / widthImMM);

//                                    CCCPspeedBYpiket.Add(new XElement("CCCPspeedBYpiket",
//                                        new XAttribute("x", StrLInd),
//                                        new XAttribute("y", (y - elem.SSSP_speed).ToString().Replace(",", ".")),
//                                        new XAttribute("style", "fill:lightgreen"),
//                                        new XAttribute("h", elem.SSSP_speed.ToString().Replace(",", "."))
//                                        )
//                                    );
//                                    StrLInd += 10;

//                                }

//                                if (elem.SSSP_speed < Vpz)
//                                {
//                                    var y = kol;
//                                    var h = (widthInPixel / widthImMM);
//                                    CCCPspeedBYpiket.Add(new XElement("CCCPspeedBYpiket",
//                                        new XAttribute("x", StrLInd),
//                                        new XAttribute("y", (y - elem.SSSP_speed).ToString().Replace(",", ".")),
//                                        new XAttribute("style", "fill:red"),
//                                        new XAttribute("h", elem.SSSP_speed.ToString().Replace(",", "."))
//                                        )
//                                    );
//                                    StrLInd += 10;
//                                }

//                                if (elem.SSSP_speed == Vpz)
//                                {
//                                    var y = kol;
//                                    var h = (widthInPixel / widthImMM);

//                                    CCCPspeedBYpiket.Add(new XElement("CCCPspeedBYpiket",
//                                        new XAttribute("x", StrLInd),
//                                        new XAttribute("y", (y - elem.SSSP_speed).ToString().Replace(",", ".")),
//                                        new XAttribute("style", "fill: blue"),
//                                        new XAttribute("h", elem.SSSP_speed.ToString().Replace(",", "."))
//                                        )
//                                    );
//                                    StrLInd = StrLInd + 10;
//                                }


//                                SSSP_speed.Clear();
//                                //prevPkt = pkt;
//                            }
//                            SSSP_speed.Add(elem.SSSP_speed);
//                        }
//                    }
//                    addParam.Add(
//                        new XElement("cccpsred", "ср.СССП",
                        
//                            new XAttribute("v", kol - CCCPsred1.Average()),
//                            new XAttribute("cccp",  CCCPsred1.Average().ToString("0")),
                  
//                            new XAttribute("dlina", StrLInd)
//                        ),
//                        new XElement("vpass", "Vпасс",
//                            new XAttribute("v", kol - 80),
//                            new XAttribute("vpz", Vpz),
//                            new XAttribute("dlina", StrLInd)
//                        ),
//                        new XElement("vgruz", "Vгруз",
//                            new XAttribute("v", kol - 70),
//                            new XAttribute("vgruz", Vgruz),
//                            new XAttribute("dlina", StrLInd)
//                        ),
//                        new XElement("MaxSpeedLimit", kol)
//                        );
//                    progressBar.Value = 0;

//                    addParam.Add(kmElements);
//                    addParam.Add(CCCPspeedBYpiket);
//                    report.Add(addParam);

//                }
//                xdReport.Add(report);

//                XslCompiledTransform transform = new XslCompiledTransform();
//                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
//                transform.Transform(xdReport.CreateReader(), writer);
//            }
//            try
//            {
//                htReport.Save(Path.GetTempPath() + "/report.html");
//            }
//            catch
//            {
//                MessageBox.Show("Ошибка сохранения файлы");
//            }
//            finally
//            {
//                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report.html");
//            }
//        }
//    }
//}