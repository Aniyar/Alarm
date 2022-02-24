using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm_Report.Forms
{
    public class GD_NN : GraphicDiagrams
    {
        private float LevelPosition = 32.5f;
        private float LevelStep = 7.5f;
        private float LevelKoef = 0.25f;

        private float StraighRighttPosition = 62f;
        private float StrightStep = 15f;
        private float StrightKoef = 0.5f;

        private float GaugeKoef = 0.5f;
        private float ProsKoef = 0.5f;

        private float StrightLeftPosition = 71f;

        private float GaugePosition = 100.5f;

        private float ProsRightPosition = 124.5f;

        private float ProsLeftPosition = 138.5f;
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

            diagramName = "ГД-НН";
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                int svgIndex = template.Xsl.IndexOf("</svg>");
                template.Xsl = template.Xsl.Insert(svgIndex, righstSideXslt());

                foreach (var tripProcess in tripProcesses)
                {
                    tripProcess.Direction = Direction.Direct;

                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        tripProcess.TrackName = trackName.ToString(); 

                        var kilometers = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);

                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();
                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = kilometers.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = kilometers.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        kilometers = kilometers.Where(Km => ((float)(float)filters[0].Value <= Km && Km <= (float)(float)filters[1].Value)).ToList();

                        kilometers = (tripProcess.Direction == Direction.Reverse ? kilometers.OrderBy(km => km) : kilometers.OrderByDescending(km => km)).ToList();
                        progressBar.Maximum = kilometers.Count;

                        var mainParam = MainParametersService.GetMainParametersFromDBMeter(tripProcess.Trip_id);

                        foreach (var kilometer in kilometers)
                        {
                            var mainParamByKm = mainParam.Where(o => o.Km == kilometer).ToList();

                            progressBar.Value = kilometers.IndexOf(kilometer) + 1;

                            string 
                                drawdownRight = string.Empty, 
                                drawdownLeft = string.Empty, 
                                gauge = string.Empty, 
                                straighteningRight = string.Empty,
                                straighteningLeft = string.Empty,
                                level = string.Empty, 
                                NstraighteningLeft = string.Empty,
                                NstraighteningRight = string.Empty;

                            var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, $"{trackName}") as List<Speed>;
                            var sector = MainTrackStructureService.GetSector(track_id, kilometer, tripProcess.Date_Vrem);
                            var fragment = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.Fragments, tripProcess.DirectionName, $"{trackName}") as Fragment;
                            var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer, MainTrackStructureConst.MtoPdbSection, tripProcess.DirectionName, $"{trackName}") as List<PdbSection>;

                            XElement addParam = new XElement("addparam",
                                new XAttribute("top-title",

                                            $"{tripProcess.DirectionName}({tripProcess.DirectionCode})  Путь:{trackName}  Км:" + kilometer +

                                            $"  {(pdbSection.Count > 0 ? pdbSection[0].ToString() : "ПЧ-/ПЧУ-/ПД-/ПДБ-")}" +

                                            $"  Уст:{(speed.Count > 0 ? speed[0].ToString() : "-/-/-")}" + "  Скор:58"),

                                new XAttribute("right-title",
                                        copyright + ": " + "ПО " + softVersion + "  " +
                                        systemName + ":" + tripProcess.Car + "(" + tripProcess.Chief + ") (БПД от " +
                                        MainTrackStructureService.GetModificationDate() + ") <" + AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, false) + ">" +
                                        "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.Direction.ToString())) + ">" +
                                        "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.CarPosition.ToString())) + ">" +
                                        "<" + period.PeriodMonth + "-" + period.PeriodYear + " " + "контр. Проезд:" + tripProcess.Date_Vrem.ToShortDateString() + " " + tripProcess.Date_Vrem.ToShortTimeString() +
                                        " " + diagramName + ">" + " Л: " + (kilometers.IndexOf(kilometer) + 1)
                                    ),
                                new XAttribute("fragment", $"{sector}  Км:{kilometer}"),
                                new XAttribute("viewbox", "0 0 770 1015"),
                                new XAttribute("minY", 0),
                                new XAttribute("maxY", 1000),

                                RightSideChart(tripProcess.Date_Vrem, kilometer, Direction.Direct, tripProcess.DirectionID, trackName.ToString(), new float[] { 151f, 146f, 152.5f, 155f, -760 }),

                                new XElement("xgrid",
                                    new XElement("x", MMToPixelChartString(LevelPosition - LevelStep), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "  –30"), new XAttribute("y", MMToPixelChartString(LevelPosition - LevelStep - 0.5f))),
                                    new XElement("x", MMToPixelChartString(LevelPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(LevelPosition - 0.5f))),
                                    new XElement("x", MMToPixelChartString(LevelPosition + LevelStep), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "    30"), new XAttribute("y", MMToPixelChartString(LevelPosition + LevelStep - 0.5f))),
                                    new XElement("x", MMToPixelChartString(StraighRighttPosition - StrightStep), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "  –30"), new XAttribute("y", MMToPixelChartString(StraighRighttPosition - StrightStep - 0.5f))),
                                    new XElement("x", MMToPixelChartString(StraighRighttPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(StraighRighttPosition - 1f))),
                                    new XElement("x", MMToPixelChartString(StraighRighttPosition + StrightStep / 10f), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "      3"), new XAttribute("y", MMToPixelChartString(StraighRighttPosition + StrightStep / 10f + 0.2f))),
                                    new XElement("x", MMToPixelChartString(StrightLeftPosition - StrightStep / 10f), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "    –3"), new XAttribute("y", MMToPixelChartString(StrightLeftPosition - StrightStep / 10f - 1f))),
                                    new XElement("x", MMToPixelChartString(StrightLeftPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(StrightLeftPosition + 0.2f))),
                                    new XElement("x", MMToPixelChartString(StrightLeftPosition + StrightStep), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "    30"), new XAttribute("y", MMToPixelChartString(StrightLeftPosition + StrightStep - 0.5f))),

                                    new XElement("x", MMToPixelChartString(GaugePosition - 10 * GaugeKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey")),
                                    new XElement("x", MMToPixelChartString(GaugePosition - 8 * GaugeKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "1512"), new XAttribute("y", MMToPixelChartString(GaugePosition - 8 * GaugeKoef - 0.5f))),
                                    new XElement("x", MMToPixelChartString(GaugePosition - 4 * GaugeKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey")),
                                    new XElement("x", MMToPixelChartString(GaugePosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "1520"), new XAttribute("y", MMToPixelChartString(GaugePosition - 0.5f))),
                                    new XElement("x", MMToPixelChartString(GaugePosition + 8 * GaugeKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "1528"), new XAttribute("y", MMToPixelChartString(GaugePosition + 8 * GaugeKoef - 0.5f))),
                                    new XElement("x", MMToPixelChartString(GaugePosition + 16 * GaugeKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "1536"), new XAttribute("y", MMToPixelChartString(GaugePosition + 16 * GaugeKoef - 0.5f))),
                                   
                                    new XElement("x", MMToPixelChartString(ProsRightPosition - 10 * ProsKoef * 2), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "  –20"), new XAttribute("y", MMToPixelChartString(ProsRightPosition - 10 * ProsKoef * 2 - 0.5f))),
                                    new XElement("x", MMToPixelChartString(ProsRightPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(ProsRightPosition - 0.5f))),
                                    new XElement("x", MMToPixelChartString(ProsRightPosition + 10 * ProsKoef * 2), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "    20"), new XAttribute("y", MMToPixelChartString(ProsRightPosition + 10 * ProsKoef * 2 - 0.5f))),

                                    new XElement("x", MMToPixelChartString(ProsLeftPosition - 10 * ProsKoef * 2), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "  –20"), new XAttribute("y", MMToPixelChartString(ProsLeftPosition - 10 * ProsKoef * 2 - 0.5f))),
                                    new XElement("x", MMToPixelChartString(ProsLeftPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(ProsLeftPosition - 0.5f))),
                                    new XElement("x", MMToPixelChartString(ProsLeftPosition + 10 * ProsKoef * 2), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "    20"), new XAttribute("y", MMToPixelChartString(ProsLeftPosition + 10 * ProsKoef * 2 - 0.5f)))
                                    ));

                            for (int index = 0; index < mainParamByKm.Count; index++)
                            {
                                int metre = tripProcess.Direction == Direction.Reverse ? 1000 - index : index;

                                drawdownRight += MMToPixelChartString(mainParamByKm[index].Drawdown_right * ProsKoef * 2 + ProsRightPosition) + "," + metre + " ";
                                drawdownLeft += MMToPixelChartString(mainParamByKm[index].Drawdown_left * ProsKoef * 2 + ProsLeftPosition) + "," + metre + " ";

                                gauge += MMToPixelChartString((mainParamByKm[index].Gauge-1520) * GaugeKoef + GaugePosition) + "," + metre + " ";

                                straighteningRight += MMToPixelChartString(mainParamByKm[index].Stright_right * StrightKoef + StraighRighttPosition) + "," + metre + " ";

                                straighteningLeft += MMToPixelChartString(mainParamByKm[index].Stright_left * StrightKoef + StrightLeftPosition) + "," + metre + " ";

                                level += MMToPixelChartString(mainParamByKm[index].Lvl * LevelKoef + LevelPosition) + "," + metre + " ";

                                NstraighteningLeft +=
                                    (float.Parse(MMToPixelChartString(mainParamByKm[index].Ner_l * StrightKoef * 4 + StrightLeftPosition), CultureInfo.InvariantCulture) - 315.6661).ToString().Replace(",", ".") + "," + metre + " ";

                                NstraighteningRight +=
                                    (float.Parse(MMToPixelChartString(mainParamByKm[index].Ner_r * StrightKoef * 4 + StraighRighttPosition), CultureInfo.InvariantCulture) - 369.05).ToString().Replace(",", ".") + "," + metre + " ";
                            }
                            addParam.Add(new XElement("polyline", new XAttribute("points", drawdownRight)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", drawdownLeft)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", gauge)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", straighteningRight)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", straighteningLeft)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", level)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", NstraighteningLeft)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", NstraighteningRight)));

                            report.Add(addParam);
                        }
                    }
                }

                progressBar.Value = 0;

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
