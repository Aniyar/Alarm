using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    public class GD_NN_new : ALARm.Core.Report.GraphicDiagrams
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
        private float angleRuleWidth = 9.7f;

        private float GetDIstanceFrom1div60(float x)
        {
            return 15 * angleRuleWidth * (1f / x - 1 / 60f);
        }

        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {

            this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();
            this.RdStructureRepository = RdStructureService.GetRepository();
            this.AdmStructureRepository = AdmStructureService.GetRepository();

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

                var tripProcesses = RdStructureService.GetTripsOnDistance(parentId, period);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                int svgIndex = template.Xsl.IndexOf("</svg>");
                template.Xsl = template.Xsl.Insert(svgIndex, RighstSideXslt());
                var svgLength = 0;

                foreach (var trip in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        trip.Track_Id = track_id;
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);

                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var lkm = kilometers.Select(o => o.Number).ToList();

                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        kilometers = kilometers.Where(Km => ((float)(float)filters[0].Value <= Km.Number && Km.Number <= (float)(float)filters[1].Value)).ToList();
                        kilometers = (trip.Travel_Direction == Direction.Reverse ? kilometers.OrderBy(o => o.Number) : kilometers.OrderByDescending(o => o.Number)).ToList();
                        //--------------------------------------------

                        progressBar.Maximum = kilometers.Count;

                        var mainParam = MainParametersService.GetMainParametersFromDBMeter(trip.Id);

                        foreach (var kilometer in kilometers)
                        {
                            //данные
                            var mainParamByKm = mainParam.Where(o => o.Km == kilometer.Number).ToList();
                            if (mainParamByKm.Count() == 0) continue;

                            string
                                drawdownRight = string.Empty,
                                drawdownLeft = string.Empty,
                                gauge = string.Empty,
                                straighteningRight = string.Empty,
                                straighteningLeft = string.Empty,
                                level = string.Empty,
                                NstraighteningLeft = string.Empty,
                                NstraighteningRight = string.Empty;

                            XElement kmlist = new XElement("kmlist");

                            progressBar.Value = kilometers.IndexOf(kilometer) + 1;

                            var outData = (List<OutData>)RdStructureService.GetNextOutDatas(kilometer.Start_Index - 1, kilometer.GetLength() - 1, kilometer.Trip.Id);
                            kilometer.AddDataRange(outData, kilometer);

                            //lvl avg data
                            var Curves = new List<NatureCurves> { };
                            var StrightAvgTrapezoid = kilometer.StrightAvg.GetTrapezoid(new List<double> { }, new List<double> { }, 4, ref Curves);
                            var LevelAvgTrapezoid = kilometer.LevelAvg.GetTrapezoid(new List<double> { }, new List<double> { }, 10, ref Curves);
                            LevelAvgTrapezoid.Add(LevelAvgTrapezoid[LevelAvgTrapezoid.Count - 1]);
                            StrightAvgTrapezoid.Add(StrightAvgTrapezoid[StrightAvgTrapezoid.Count - 1]);

                            //zero line data
                            kilometer.GetZeroLines(outData, trip, MainTrackStructureService.GetRepository());
                            kilometer.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);
                            kilometer.LoadDigresions(RdStructureRepository, MainTrackStructureRepository, trip, AdditionalParam: true);

                            var sector_station = MainTrackStructureService.GetSector(track_id, kilometer.Number, trip.Trip_date);
                            var fragment = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, kilometer.Number, MainTrackStructureConst.Fragments, kilometer.Direction_name, $"{trackName}") as Fragment;
                            var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, kilometer.Number, MainTrackStructureConst.MtoPdbSection, kilometer.Direction_name, $"{trackName}") as List<PdbSection>;

                            int fourStepOgrCoun = 0, otherfourStepOgrCoun = 0;


                            svgLength = kilometer.GetLength() < 1000 ? 1000 : kilometer.GetLength();
                            var xp = (-kilometer.Start_m - svgLength - 50) + (svgLength + 105) - 52;
                            var direction = AdmStructureRepository.GetDirectionByTrack(kilometer.Track_id);

                            XElement addParam = new XElement("addparam",
                                new XAttribute("top-title",
                                    (direction != null ? $"{direction.Name} ({direction.Code} )" : "Неизвестный") + " Путь: " + kilometer.Track_name + " Км:" +
                                    kilometer.Number + " " + (kilometer.PdbSection.Count > 0 ? kilometer.PdbSection[0].ToString() : " ПЧ-/ПЧУ-/ПД-/ПДБ-") + " Уст: " + " " + (kilometer.Speeds.Count > 0 ? $"{kilometer.Speeds.First().Passenger}/{kilometer.Speeds.First().Freight}" : "-/-") + $" Скор:{(int)kilometer.Speed.Average()}"),

                                new XAttribute("right-title",
                                    copyright + ": " + "ПО " + softVersion + "  " +
                                    systemName + ":" + trip.Car + "(" + trip.Chief.Trim() + ") (БПД от " + MainTrackStructureRepository.GetModificationDate() + ") <" + (kilometer.PdbSection.Count > 0 ? kilometer.PdbSection[0].RoadAbbr : "НЕИЗВ") + ">" + "<" + kilometer.Passage_time.ToString("dd.MM.yyyy  HH:mm") + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(trip.Travel_Direction.ToString())) + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(trip.Car_Position.ToString())) + ">" +
                                    "<" + trip.Trip_date.Month + "-" + trip.Trip_date.Year + " " + "контр. Проезд:" + trip.Trip_date.ToString("dd.MM.yyyy  HH:mm") + " " + diagramName + ">"
                                    ),
                                new XAttribute("pre", xp + 30),
                                new XAttribute("prer", xp + 21),
                                new XAttribute("topr", -kilometer.Start_m - svgLength - 45),
                                new XAttribute("topf", xp + 10),
                                new XAttribute("topx", -kilometer.Start_m - svgLength),
                                new XAttribute("topx1", -kilometer.Start_m - svgLength - 30),
                                new XAttribute("topx2", -kilometer.Start_m - svgLength - 15),
                                new XAttribute("fragment", (kilometer.StationSection != null && kilometer.StationSection.Count > 0 ? "Станция: " + kilometer.StationSection[0].Station : (kilometer.Sector != null ? kilometer.Sector.ToString() : "")) + " Км:" + kilometer.Number),
                                new XAttribute("viewbox", $"-20 {-kilometer.Start_m - svgLength - 50} 830 {svgLength + 105}"),
                                new XAttribute("minY", -kilometer.Start_m),
                                new XAttribute("maxY", -kilometer.Final_m),
                                new XAttribute("minYround", -(kilometer.Start_m - kilometer.Start_m % 100)),

                                RightSideChart(trip.Trip_date, kilometer, kilometer.Track_id, new float[] { 151f, 146f, 152.5f, 155f }),

                                new XElement("xgrid",
                                    new XElement("x", 
                                        MMToPixelChartString(LevelPosition - LevelStep), 
                                            new XAttribute("dasharray", "0.5,3"), 
                                            new XAttribute("stroke", "grey"), 
                                            new XAttribute("label", "  –30"), 
                                            new XAttribute("y", MMToPixelChartString(LevelPosition - LevelStep - 0.5f))),



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

                            string downhillLeft = string.Empty;
                            string downhillRight = string.Empty;
                            string treadTiltLeft = string.Empty;
                            string treadTiltRight = string.Empty;

                            for (int index = 0; index < Math.Min(kilometer.meter.Count, mainParamByKm.Count) - 1; index++)
                            {
                                //int metre = -kilometer.meter[index];
                                int metre = -(kilometer.Length - index);

                                drawdownRight += MMToPixelChartString(mainParamByKm[index].Drawdown_right * ProsKoef * 2 + ProsRightPosition) + "," + metre + " ";
                                drawdownLeft += MMToPixelChartString(mainParamByKm[index].Drawdown_left * ProsKoef * 2 + ProsLeftPosition) + "," + metre + " ";

                                gauge += MMToPixelChartString((mainParamByKm[index].Gauge - 1520) * GaugeKoef + GaugePosition) + "," + metre + " ";

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

                            char separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];

                            report.Add(addParam);
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

        private string GetXByDigName(DigName digName)
        {
            float move = 6.6f;
            switch (digName)
            {
                case DigName dn when dn == DigressionName.LongWaveLeft:
                    return MMToPixelChartString(LevelPosition + move);
                case DigName dn when dn == DigressionName.LongWaveRight:
                    return MMToPixelChartString(+move);
                case DigName dn when dn == DigressionName.MiddleWaveLeft:
                    return MMToPixelChartString(+move);
                case DigName dn when dn == DigressionName.MiddleWaveRight:
                    return MMToPixelChartString(+move);
                case DigName dn when dn == DigressionName.ShortWaveLeft:
                    return MMToPixelChartString(+move);
                case DigName dn when dn == DigressionName.ShortWaveRight:
                    return MMToPixelChartString(+move);
            }

            return "-100";
        }
    }
}
