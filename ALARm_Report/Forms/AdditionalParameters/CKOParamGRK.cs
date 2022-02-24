using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Globalization;
using System.IO;
using System.Linq;
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
    public class CKOParamGRK : GraphicDiagrams
    {
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

        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
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

            diagramName = "СКО";

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

                

                foreach (var tripProcess in tripProcesses)
                {
                    //Канонический вид
                    //tripProcess.Direction = Direction.Direct;

                    foreach (var track_id in admTracksId)
                    {
                     
                            var trackName = AdmStructureService.GetTrackName(track_id);

                        var kilometers = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                        if (kilometers.Count() == 0) continue;

                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();
                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = kilometers.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = kilometers.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;
                        var trip = RdStructureService.GetTrip(tripProcess.Trip_id);
                        var kilometersnumber = RdStructureService.GetKilometersByTrip(trip);
                        var directName = AdditionalParametersService.DirectName(tripProcess.Id, (int)tripProcess.Direction);
                    
                        foreach (var kilometer in kilometersnumber)
                        {
                            var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer.Number, MainTrackStructureConst.MtoPdbSection, kilometer.Direction_name, $"{trackName}") as List<PdbSection>;

                            //var kilometers = AdditionalParametersService.GetKilometers(tripProcess.Id, (int)tripProcess.Direction);
                            progressBar.Maximum = 1;
                            for (int i = 0; i < 1; i++)
                            {
                                progressBar.Value = i;
                                XElement addParam = new XElement("addparam",



                                        //new XAttribute("top-title",
                                        //        (directName != null ? $"{tripProcess.DirectionName} ({tripProcess.DirectionCode} )" : "Неизвестный") + " Путь: " + kilometer.Track_name + " Км:" +
                                        //        kilometer.Number + " " + (pdbSection.Count > 0 ? pdbSection[0].ToString() : " ПЧ-/ПЧУ-/ПД-/ПДБ-") +
                                        //        " Уст: " + " " + (kilometer.Speeds.Count > 0 ? kilometer.Speeds.Last().ToString() : "-/-/-") + $" Скор:{kilometer.Speed.Average().ToString("0")}"),


                                    new XAttribute("right-title",

                                    copyright + ": " + "ПО " + softVersion + "  " +
                                    systemName + ":" + tripProcess.Car + "(" + tripProcess.Chief + ") (БПД от " +
                                    MainTrackStructureService.GetModificationDate() + ") <" + AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, false) + ">" +
                                    //"<" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.Direction.ToString())) + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.CarPosition.ToString())) + ">" +
                                    "<" + period.PeriodMonth + "-" + period.PeriodYear + " " + "контр. Проезд:" + tripProcess.Date_Vrem.ToShortDateString() + " " 
                                    + tripProcess.Date_Vrem.ToShortTimeString() +" " + diagramName + ">" + " Л: " + (i + 1)
                                    ),
                                
                                    new XAttribute("distance", ((AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId)).Code),
                                    new XAttribute("tripdate", tripProcess.Date_Vrem.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))),
                                    new XAttribute("PS", tripProcess.Car),
                                    new XAttribute("roadName", roadName),
                                    new XAttribute("track", trackName),
                                    new XAttribute("km", kilometers.Min() + " - " + kilometers.Max()),
                                    new XAttribute("direction", tripProcess.DirectionName + "(" + tripProcess.DirectionCode + ")"),

                                        new XAttribute("viewbox", "0 0 600 1000"),
                                        new XAttribute("minY", 0),
                                        new XAttribute("maxY", 1000),
                                        new XAttribute("minYLine", 25),
                                        new XElement("xgrid",

                                        new XAttribute("x0", xBegin),
                                        new XAttribute("x00", MMToPixelChart(5)),
                                        new XAttribute("x000", MMToPixelChart(25)),
                                        new XAttribute("x0000", MMToPixelChart(4.5f)),

                                        new XAttribute("x1", MMToPixelChart(28.50f)),
                                        new XAttribute("l1", MMToPixelChart(33.89f)),
                                        new XAttribute("l2", MMToPixelChart(53.82f)),
                                        new XAttribute("l3", MMToPixelChart(33.39f)),

                                        new XAttribute("x2", MMToPixelChart(57)),
                                        new XAttribute("l4", MMToPixelChart(60.05f)),
                                        new XAttribute("l5", MMToPixelChart(76.25f)),
                                        new XAttribute("l6", MMToPixelChart(59.55f)),

                                        //new XAttribute("x2", MMToPixelChart(57)),
                                        //new XAttribute("l4", MMToPixelChart(60.05f)),
                                        //new XAttribute("l5", MMToPixelChart(78.74f)),
                                        //new XAttribute("l6", MMToPixelChart(59.55f)),

                                        new XAttribute("x11", MMToPixelChart(31)),
                                        new XAttribute("x111", MMToPixelChart(31)),



                                        new XAttribute("x3", MMToPixelChart(79)),
                                        new XAttribute("l7", MMToPixelChart(82.05f)),
                                        new XAttribute("l8", MMToPixelChart(98.25f)),
                                        new XAttribute("l9", MMToPixelChart(81.55f)),

                                        //new XAttribute("x3", MMToPixelChart(82)),
                                        //new XAttribute("l7", MMToPixelChart(84.72f)),
                                        //new XAttribute("l8", MMToPixelChart(102.91f)),
                                        //new XAttribute("l9", MMToPixelChart(84.22f)),

                                        new XAttribute("x4", MMToPixelChart(101)),
                                        new XAttribute("l10", MMToPixelChart(104.05f)),
                                        new XAttribute("l11", MMToPixelChart(120.25f)),
                                        new XAttribute("l12", MMToPixelChart(103.55f)),

                                        new XAttribute("x41", MMToPixelChart(123)),
                                        new XAttribute("l13", MMToPixelChart(126.05f)),
                                        new XAttribute("l14", MMToPixelChart(142.25f)),
                                        new XAttribute("l15", MMToPixelChart(125.55f)),

                                        //астынгы сызык
                                        new XAttribute("lineLow", 575),

                                        new XAttribute("ticks", MMToPixelChart(146) - widthInPixel / widthImMM / 4),
                                        new XAttribute("x5", MMToPixelChart(151)),
                                        new XAttribute("picket", MMToPixelChart(146) + (MMToPixelChart(152.5f) - MMToPixelChart(151f)) / 2),
                                        new XAttribute("x6", MMToPixelChart(152.5f)),
                                        new XAttribute("x7", MMToPixelChart(146)),
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



                                var gapElements = new XElement("graphics");
                                var kmElements = new XElement("km");
                                double StrLInd = 1, StrRInd = 1, LvlInd = 1, DrRInd = 1, DrLInd = 1, TrWInd = 1, testInd = 0;
                                int prevKm = -1;
                                bool check = false;


                            //kilometers = kilometers.Where(km => ((float)filters[0].Value <= km && km <= (float)filters[1].Value)).ToList();



                            var mainParam = MainParametersService.GetMainParametersFromDB(tripProcess.Trip_id);
                            mainParam = mainParam.Where(o => ((float)filters[0].Value <= o.Km && o.Km <= (float)filters[1].Value)).ToList();

                                testInd = mainParam.Count();
                                var rectHeig = 974.9 / testInd;


                                for (int ii = 0; ii < mainParam.Count; ii++)
                                {
                                    if (prevKm != mainParam[ii].Km)
                                    {
                                        kmElements.Add(new XElement("text", mainParam[ii].Km,
                                            new XAttribute("y", 5),
                                            new XAttribute("x", 18 + TrWInd))
                                        );
                                        prevKm = mainParam[ii].Km;
                                    }

                                    gapElements.Add(new XElement("StraighteningLeft",
                                            new XAttribute("x", 9f),
                                            new XAttribute("y", 24 + StrLInd),
                                            new XAttribute("h", rectHeig),
                                            new XAttribute("w",
                                                float.Parse(MMToPixelChartWidthString((float)mainParam[ii].Stright_left_SKO * (21.2f / 10f)), CultureInfo.InvariantCulture) > 80f ?
                                                    80.2f : float.Parse(MMToPixelChartWidthString((float)mainParam[ii].Stright_left_SKO * (21.2f / 10f)), CultureInfo.InvariantCulture))
                                            )
                                        );
                                    StrLInd = StrLInd + rectHeig;

                                    gapElements.Add(new XElement("StraighteningRigth",
                                            new XAttribute("x", 125),
                                            new XAttribute("y", 24 + StrRInd),
                                            new XAttribute("h", rectHeig),
                                            new XAttribute("w",
                                                float.Parse(MMToPixelChartWidthString((float)mainParam[ii].Stright_right_SKO * (21.2f / 10f)), CultureInfo.InvariantCulture) > 80f ?
                                                    80f : float.Parse(MMToPixelChartWidthString((float)mainParam[ii].Stright_right_SKO * (21.2f / 10f)), CultureInfo.InvariantCulture))
                                            )
                                        );
                                    StrRInd = StrRInd + rectHeig;

                                    gapElements.Add(new XElement("Level",
                                            new XAttribute("x", 230),
                                            new XAttribute("y", 24 + LvlInd),
                                            new XAttribute("h", rectHeig),
                                            new XAttribute("w",
                                                float.Parse(MMToPixelChartWidthString(mainParam[ii].Level_SKO * (21.2f / 10f)), CultureInfo.InvariantCulture) > 65f ?
                                                    65f : float.Parse(MMToPixelChartWidthString(mainParam[ii].Level_SKO * (21.2f / 10f)), CultureInfo.InvariantCulture))
                                            )
                                        );
                                    LvlInd = LvlInd + rectHeig;

                                    gapElements.Add(new XElement("DrawdownLeft",
                                            new XAttribute("x", 318.4f),
                                            new XAttribute("y", 24 + DrLInd),
                                            new XAttribute("h", rectHeig),
                                            new XAttribute("w",
                                                float.Parse(MMToPixelChartWidthString((float)mainParam[ii].Drawdown_left_SKO * (21.2f / 10f)), CultureInfo.InvariantCulture) > 65f ?
                                                    65f : float.Parse(MMToPixelChartWidthString((float)mainParam[ii].Drawdown_left_SKO * (21.2f / 10f)), CultureInfo.InvariantCulture))
                                            )
                                        );
                                    DrLInd = DrLInd + rectHeig;

                                    gapElements.Add(new XElement("DrawdoownRigth",
                                            new XAttribute("x", 406.4f),
                                            new XAttribute("y", 24 + DrRInd),
                                            new XAttribute("h", rectHeig),
                                            new XAttribute("w",
                                                float.Parse(MMToPixelChartWidthString(mainParam[ii].Drawdown_right_SKO * (21.2f / 10f)), CultureInfo.InvariantCulture) > 65f ?
                                                    65f : float.Parse(MMToPixelChartWidthString((float)mainParam[ii].Drawdown_right_SKO * (21.2f / 10f)), CultureInfo.InvariantCulture))
                                            )
                                        );
                                    DrRInd = DrRInd + rectHeig;

                                    gapElements.Add(new XElement("TrackWidth",
                                            new XAttribute("x", 494.8f),
                                            new XAttribute("y", 24 + TrWInd),
                                            new XAttribute("h", rectHeig),
                                            new XAttribute("w",
                                                float.Parse(MMToPixelChartWidthString(mainParam[ii].Gauge_SKO * (21.2f / 10f)), CultureInfo.InvariantCulture) > 65f ?
                                                    65f : float.Parse(MMToPixelChartWidthString(mainParam[ii].Gauge_SKO * (21.2f / 10f)), CultureInfo.InvariantCulture))
                                            )
                                        );
                                    TrWInd = TrWInd + rectHeig;
                                }
                                addParam.Add(kmElements);
                                addParam.Add(gapElements);
                                report.Add(addParam);
                            }
                        }
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
