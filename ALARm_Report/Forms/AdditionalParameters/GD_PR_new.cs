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
    public class GD_PR_new : ALARm.Core.Report.GraphicDiagrams
    {
        private readonly float LevelPosition = 32.5f;

        private readonly float StraighRighttPosition = 62f;

        private readonly float StrightLeftPosition = 71f;

        private readonly float GaugePosition = 100.5f;

        private readonly float ProsRightPosition = 124.5f;

        private readonly float ProsLeftPosition = 138.5f;

        private float NPKLeftPosition = 76.62f;
        private float NPKRightPosition = 112.9f;

        private float PULeftPosition = 3.91f;
        private float PURightPosition = 40.26f;
        private float angleRuleWidth = 26.06f;

        private float GetDIstanceFrom1div60(float x)
        {
            var koef = (angleRuleWidth / (1 / 12f - 1 / 60f));
            var value = 1 / x - 1 / 60f;

            return koef * value;
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

            diagramName = "ПР";
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

                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();

                        if (kilometers.Count == 0)
                            continue;

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

                        foreach (var kilometer in kilometers)
                        {
                            kilometer.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);
                            if (kilometer.NonstandardKms.Any())
                            {
                                kilometer.Final_m = kilometer.NonstandardKms.First().Len;
                            }
                            //данные
                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(kilometer.Number, trip.Id);
                            if (DBcrossRailProfile == null) continue;

                            var sortedData = DBcrossRailProfile.OrderByDescending(d => d.Meter).ToList();
                            var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(sortedData);

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

                            var sector_station = MainTrackStructureService.GetSector(track_id, kilometer.Number, trip.Trip_date);
                            var fragment = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, kilometer.Number, MainTrackStructureConst.Fragments, kilometer.Direction_name, $"{trackName}") as Fragment;
                            var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, kilometer.Number, MainTrackStructureConst.MtoPdbSection, kilometer.Direction_name, $"{trackName}") as List<PdbSection>;

                            //Линий уровня по середине------------------------------------
                            List<string> result = new List<string>();
                            string drawdownRight = string.Empty, drawdownLeft = string.Empty, gauge = string.Empty, zeroGauge = string.Empty,
                                    zeroStraighteningRight = string.Empty, averageStraighteningRight = string.Empty, straighteningRight = string.Empty,
                                    zeroStraighteningLeft = string.Empty, averageStraighteningLeft = string.Empty, straighteningLeft = string.Empty,
                                    level = string.Empty, averageLevel = string.Empty, zeroLevel = string.Empty;

                            int fourStepOgrCoun = 0, otherfourStepOgrCoun = 0;


                            svgLength = kilometer.GetLength() < 1000 ? 1000 : kilometer.GetLength();
                            var xp = (-kilometer.Start_m - svgLength - 50) + (svgLength + 105) - 52;
                            var direction = AdmStructureRepository.GetDirectionByTrack(kilometer.Track_id);

                            XElement addParam = new XElement("addparam",
                                new XAttribute("top-title",
                                    (direction != null ? $"{direction.Name} ({direction.Code} )" : "Неизвестный") + " Путь: " + kilometer.Track_name + " Км:" +
                                            kilometer.Number + " " + (kilometer.PdbSection.Count > 0 ? kilometer.PdbSection[0].ToString() : " ПЧ-/ПЧУ-/ПД-/ПДБ-") + " Уст: " + " " +
                                            (kilometer.Speeds.Count > 0 ? $"{kilometer.Speeds.First().Passenger}/{kilometer.Speeds.First().Freight}" : "-/-") +
                                            $" Скор:{(int)kilometer.Speed.Average()}"),

                                new XAttribute("right-title",
                                    copyright + ": " + "ПО " + softVersion + "  " +
                                    systemName + ":" + trip.Car + "(" + trip.Chief.Trim() + ") (БПД от " + MainTrackStructureRepository.GetModificationDate() + ") <" + (kilometer.PdbSection.Count > 0 ? kilometer.PdbSection[0].RoadAbbr : "НЕИЗВ") + ">" + "<" + kilometer.Passage_time.ToString("dd.MM.yyyy  HH:mm") + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(trip.Travel_Direction.ToString())) + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(trip.Car_Position.ToString())) + ">" +
                                    "<" + trip.Trip_date.Month + "-" + trip.Trip_date.Year + " " + (trip.Trip_Type == TripType.Control ? "контр." : trip.Trip_Type == TripType.Work ? "раб." : "доп.") +
                                    " Проезд:" + trip.Trip_date.ToString("dd.MM.yyyy  HH:mm") + " " + diagramName + ">"
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
                                    //НПК лев
                                    new XElement("x", MMToPixelChartString(NPKLeftPosition)),
                                    new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(30))),
                                    new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(22))),
                                    new XElement("x20", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(20))),
                                    new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(18))),
                                    new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(16))),
                                    new XElement("x", MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(12))),
                                    //НПК прав.
                                    new XElement("x", MMToPixelChartString(NPKRightPosition)),
                                    new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(30))),
                                    new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(22))),
                                    new XElement("x20", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(20))),
                                    new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(18))),
                                    new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(16))),
                                    new XElement("x", MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(12))),
                                    //ПУ лев.
                                    new XElement("x", MMToPixelChartString(PULeftPosition)),
                                    new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(30))),
                                    new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(22))),
                                    new XElement("x20", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(20))),
                                    new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(18))),
                                    new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(16))),
                                    new XElement("x", MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(12))),
                                    //ПУ прав.
                                    new XElement("x", MMToPixelChartString(PURightPosition)),
                                    new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(30))),
                                    new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(22))),
                                    new XElement("x20", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(20))),
                                    new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(18))),
                                    new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(16))),
                                    new XElement("x", MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(12))))
                                );

                            string downhillLeft = string.Empty;
                            string downhillRight = string.Empty;
                            string treadTiltLeft = string.Empty;
                            string treadTiltRight = string.Empty;



                            for (int i = 0; i < kilometer.meter.Count - 1; i++)
                            {
                                int metre = -kilometer.meter[i];

                                var st = crossRailProfile.Meters.Where(o => o == i).ToList();
                                if (st.Any())
                                {
                                    var ind = crossRailProfile.Meters.IndexOf(i);

                                    treadTiltLeft += MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(1 / crossRailProfile.TreadTiltLeft[ind])) + "," + metre + " ";
                                    treadTiltRight += MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(1 / crossRailProfile.TreadTiltRight[ind])) + "," + metre + " ";
                                    //пу
                                    downhillLeft += MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(1 / crossRailProfile.DownhillLeft[ind])) + "," + metre + " ";
                                    downhillRight += MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(1 / crossRailProfile.DownhillRight[ind])) + "," + metre + " ";
                                }
                                else
                                {
                                    treadTiltLeft += MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(1.0f /(3.0f/45.0f))) + "," + metre + " ";
                                    treadTiltRight += MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(1 / (3.0f / 45.0f))) + "," + metre + " ";
                                    //пу
                                    downhillLeft += MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(1 / 0.05f)) + "," + metre + " ";
                                    downhillRight += MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(1 / 0.05f)) + "," + metre + " ";
                                }

                                //нпк
                                //treadTiltLeft += MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(1 / crossRailProfile.TreadTiltLeft[index])) + "," + metre + " ";
                                //treadTiltRight += MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(1 / crossRailProfile.TreadTiltRight[index])) + "," + metre + " ";
                                ////пу
                                //downhillLeft += MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(1 / crossRailProfile.DownhillLeft[index])) + "," + metre + " ";
                                //downhillRight += MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(1 / crossRailProfile.DownhillRight[index])) + "," + metre + " ";


                                //treadTiltLeft += MMToPixelChartString(NPKLeftPosition + GetDIstanceFrom1div60(20)) + "," + metre + " ";
                                //treadTiltRight += MMToPixelChartString(NPKRightPosition + GetDIstanceFrom1div60(12)) + "," + metre + " ";
                                ////пу
                                //downhillLeft += MMToPixelChartString(PULeftPosition + GetDIstanceFrom1div60(60)) + "," + metre + " ";
                                //downhillRight += MMToPixelChartString(PURightPosition + GetDIstanceFrom1div60(30)) + "," + metre + " ";
                            }

                            addParam.Add(new XElement("polyline", new XAttribute("points", treadTiltLeft)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", treadTiltRight)));

                            addParam.Add(new XElement("polyline", new XAttribute("points", downhillLeft)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", downhillRight)));

                            char separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];


                            List<Digression> addDigressions = crossRailProfile.GetDigressions();
                            var dignatur = new List<DigressionMark> { };

                            foreach (var dig in addDigressions)
                            {
                                int count = dig.Length / 4;
                                count += dig.Length % 4 > 0 ? 1 : 0;

                                if (dig.Length < 4 && (int)dig.Typ == 3) continue;

                                if (
                                    dig.DigName == DigressionName.SideWearLeft || dig.DigName == DigressionName.SideWearRight ||
                                    dig.DigName == DigressionName.VertIznosL || dig.DigName == DigressionName.VertIznosR ||
                                    dig.DigName == DigressionName.ReducedWearLeft || dig.DigName == DigressionName.ReducedWearRight ||
                                    dig.DigName == DigressionName.HeadWearLeft || dig.DigName == DigressionName.HeadWearRight)
                                {
                                    continue;
                                }

                                dignatur.Add(
                                    new DigressionMark()
                                    {
                                        Digression = dig.DigName,
                                        NotMoveAlert = false,
                                        Meter = dig.Kmetr,
                                        finish_meter = dig.Kmetr + dig.Length,
                                        Degree = (int)dig.Typ,
                                        Length = dig.Length,
                                        Value = dig.Value,
                                        Count = dig.Count,
                                        DigName = dig.GetName(),
                                        PassengerSpeedLimit = -1,
                                        FreightSpeedLimit = -1,
                                        Comment = "",
                                        Diagram_type = "GD_PR"
                                    });

                                //var picket = kilometer.Pickets.GetPicket(dig.Meter);
                                //if (picket != null)
                                //{
                                //    picket.Digression.Add(Digressions.First());

                                //}
                            }
                            kilometer.Digressions.AddRange(dignatur);
                            kilometer.LoadDigresions(RdStructureRepository, MainTrackStructureRepository, trip, AdditionalParam: true);


                            var digElemenets = new XElement("digressions");
                            List<int> usedTops = new List<int>();
                            List<int> speedmetres = new List<int>();

                            var gmeter = kilometer.Start_m.RoundTo10() + 10;

                            foreach (var picket in kilometer.Pickets)
                            {
                                picket.Digression = picket.Digression.OrderBy(o => o.Meter).ToList();
                                picket.WriteNotesToReport(
                                    kilometer,
                                    speedmetres,
                                    addParam,
                                    digElemenets,
                                    ProsRightPosition,
                                    ProsLeftPosition,
                                    StraighRighttPosition,
                                    StrightLeftPosition,
                                    GaugePosition,
                                    LevelPosition,
                                    this,
                                    ref fourStepOgrCoun,
                                    ref otherfourStepOgrCoun);
                            }

                            //addParam.Add(new XAttribute("common", "Кол. ст. - " + kilometer.GetdigressionsCount));

                            addParam.Add(
                                new XAttribute("speedlimit", "Кол. ст. - " + kilometer.GetdigressionsCount + " Огр: " + kilometer.SpeedLimit)
                                );

                            addParam.Add(digElemenets);
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
                htReport.Save($@"G:\form\5.Графические диаграммы ГД\ГД-ПР - подуклонка рельсов, наклон поверхности катания.html");
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

        private string GetXByDigName(string digName)
        {
            float move = 6.6f;
            switch (digName)
            {
                case string name when name == DigressionName.LongWaveLeft.Name:
                    return MMToPixelChartString(LevelPosition + move);
                case string name when name == DigressionName.LongWaveRight.Name:
                    return MMToPixelChartString(+move);
                case string name when name == DigressionName.MiddleWaveLeft.Name:
                    return MMToPixelChartString(+move);
                case string name when name == DigressionName.MiddleWaveRight.Name:
                    return MMToPixelChartString(+move);
                case string name when name == DigressionName.ShortWaveLeft.Name:
                    return MMToPixelChartString(+move);
                case string name when name == DigressionName.ShortWaveRight.Name:
                    return MMToPixelChartString(+move);
            }

            return "-100";
        }
    }
}
