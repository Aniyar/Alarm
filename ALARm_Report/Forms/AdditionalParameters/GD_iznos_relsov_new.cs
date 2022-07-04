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
    public class GD_iznos_relsov_new : ALARm.Core.Report.GraphicDiagrams
    {
        private float angleRuleWidth = 9.7f;

        private float izBokl = 7;
        private float izBokR = 23.991f;
        private float izverL = 43.39f;
        private float izverR = 60.83f;
        private float izPRL = 74.77f;
        private float izPRR = 93.44f;
        private float iz45L = 112.15443f;
        private float iz45R = 130.844f;

        private float iznosKoefBok = 11.8f / 20f;
        private float iznosKoefVert = 7.8f / 13f;
        private float iznosKoefPriv= 9.8f / 16f;
        private float iznosKoef45= 10.8f / 18f;

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

            diagramName = "Износ рельсов";
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
                        if (kilometers.Count==0) continue;

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
                            //kilometer.LoadDigresions(RdStructureRepository, MainTrackStructureRepository, trip, AdditionalParam: true);

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

                            var direction = AdmStructureRepository.GetDirectionByTrack(kilometer.Track_id);

                            svgLength = kilometer.GetLength() < 1000 ? 1000 : kilometer.GetLength();
                            var xp = (-kilometer.Start_m - svgLength - 50) + (svgLength + 105) - 52;

                            XElement addParam = new XElement("addparam",
                                new XAttribute("top-title",
                                    (direction != null ? $"{direction.Name} ({direction.Code} )" : "Неизвестный") + " Путь: " + kilometer.Track_name + " Км:" +
                                    kilometer.Number + " " + (kilometer.PdbSection.Count > 0 ? kilometer.PdbSection[0].ToString() : " ПЧ-/ПЧУ-/ПД-/ПДБ-") + " Уст: " + " " +
                                            (kilometer.Speeds.Count > 0 ? $"{kilometer.Speeds.First().Passenger}/{kilometer.Speeds.First().Freight}" : "-/-")  + $" Скор:{(int)kilometer.Speed.Average()}"),

                                new XAttribute("right-title",
                                    copyright + ": " + "ПО " + softVersion + "  " +
                                    systemName + ":" + trip.Car + "(" + trip.Chief.Trim() + ") (БПД от " + MainTrackStructureRepository.GetModificationDate() + ") <" + (kilometer.PdbSection.Count > 0 ? kilometer.PdbSection[0].RoadAbbr : "НЕИЗВ") + ">" + "<" + kilometer.Passage_time.ToString("dd.MM.yyyy  HH:mm") + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(trip.Travel_Direction.ToString())) + ">" +
                                    "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(trip.Car_Position.ToString())) + ">" +
                                    "<" + trip.Trip_date.Month + "-" + trip.Trip_date.Year +
                                            " " + (trip.Trip_Type == TripType.Control ? "контр." : trip.Trip_Type == TripType.Work ? "раб." : "доп.") +
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
                                    new XAttribute("x0", MMToPixelChart(izBokl)),
                                    new XAttribute("x1", MMToPixelChart(izBokR)),
                                    new XAttribute("x2", MMToPixelChart(izPRL)),
                                    new XAttribute("x3", MMToPixelChart(izPRR)),
                                    new XAttribute("x31", MMToPixelChart(izverL)),
                                    new XAttribute("x32", MMToPixelChart(izverR)),
                                    new XAttribute("iz45R", MMToPixelChart(iz45R)),
                                    new XAttribute("iz45L", MMToPixelChart(iz45L)),

                                    new XElement("x", MMToPixelChartString(izBokl + iznosKoefBok * 8f)),
                                    new XElement("x", MMToPixelChartString(izBokl + iznosKoefBok * 13f)),
                                    new XElement("x", MMToPixelChartString(izBokl + iznosKoefBok * 20f)),
                                    new XElement("x", MMToPixelChartString(izBokR + iznosKoefBok * 8f)),
                                    new XElement("x", MMToPixelChartString(izBokR + iznosKoefBok * 13f)),
                                    new XElement("x", MMToPixelChartString(izBokR + iznosKoefBok * 20f)),

                                    new XElement("x", MMToPixelChartString(izverL + iznosKoefVert * 4f)),
                                    new XElement("x", MMToPixelChartString(izverL + iznosKoefVert * 13f)),
                                    new XElement("x", MMToPixelChartString(izverR + iznosKoefVert * 4f)),
                                    new XElement("x", MMToPixelChartString(izverR + iznosKoefVert * 13f)),

                                    new XElement("x", MMToPixelChartString(izPRL + iznosKoefPriv * 8f)),
                                    new XElement("x", MMToPixelChartString(izPRL + iznosKoefPriv * 16f)),
                                    new XElement("x", MMToPixelChartString(izPRR + iznosKoefPriv * 8f)),
                                    new XElement("x", MMToPixelChartString(izPRR + iznosKoefPriv * 16f)),

                                    new XElement("x", MMToPixelChartString(iz45L + iznosKoef45 * 8f)),
                                    new XElement("x", MMToPixelChartString(iz45L + iznosKoef45 * 15f)),
                                    new XElement("x", MMToPixelChartString(iz45L + iznosKoef45 * 18f)),
                                    new XElement("x", MMToPixelChartString(iz45R + iznosKoef45 * 8f)),
                                    new XElement("x", MMToPixelChartString(iz45R + iznosKoef45 * 15f)),
                                    new XElement("x", MMToPixelChartString(iz45R + iznosKoef45 * 18f))

                                ));

                            string downhillLeft = string.Empty;
                            string downhillRight = string.Empty;
                            string treadTiltLeft = string.Empty;
                            string treadTiltRight = string.Empty;

                            string sideWearLeft = string.Empty;
                            string sideWearRight = string.Empty;
                            string VertIznosL = string.Empty;
                            string VertIznosR = string.Empty;
                            string ReducedWearLeft = string.Empty;
                            string ReducedWearRight = string.Empty;
                            string HeadWearLeft = string.Empty;
                            string HeadWearRight = string.Empty;

                            for (int i = 0; i < kilometer.meter.Count - 1; i++)
                            {
                                try
                                {
                                    int metre = -kilometer.meter[i];

                                    var st = crossRailProfile.Meters.Where(o => o == i).ToList();
                                    if (st.Any())
                                    {
                                        var index = crossRailProfile.Meters.IndexOf(i);

                                        //бок износ
                                        sideWearLeft += MMToPixelChartString(izBokl + iznosKoefBok * (crossRailProfile.SideWearLeft[index] < 0 ? 0.0 : crossRailProfile.SideWearLeft[index])) + "," + metre + " ";
                                        sideWearRight += MMToPixelChartString(izBokR + iznosKoefBok * (crossRailProfile.SideWearRight[index] < 0 ? 0.0 : crossRailProfile.SideWearRight[index])) + "," + metre + " ";
                                        //верт износ 
                                        VertIznosL += MMToPixelChartString(izverL + iznosKoefVert * (crossRailProfile.VertIznosL[index] < 0 ? 0.0 : crossRailProfile.VertIznosL[index])) + "," + metre + " ";
                                        VertIznosR += MMToPixelChartString(izverR + iznosKoefVert * (crossRailProfile.VertIznosR[index] < 0 ? 0.0 : crossRailProfile.VertIznosR[index])) + "," + metre + " ";
                                        //прив износ
                                        ReducedWearLeft += MMToPixelChartString(izPRL + iznosKoefPriv * (crossRailProfile.ReducedWearLeft[index] < 0 ? 0.0 : crossRailProfile.ReducedWearLeft[index])) + "," + metre + " ";
                                        ReducedWearRight += MMToPixelChartString(izPRR + iznosKoefPriv * (crossRailProfile.ReducedWearRight[index] < 0 ? 0.0 : crossRailProfile.ReducedWearRight[index])) + "," + metre + " ";
                                        //износ головки рельса 45 град
                                        HeadWearLeft += MMToPixelChartString(iz45L + iznosKoef45 * (crossRailProfile.HeadWearLeft[index] < 0 ? 0.0 : crossRailProfile.HeadWearLeft[index])) + "," + metre + " ";
                                        HeadWearRight += MMToPixelChartString(iz45R + iznosKoef45 * (crossRailProfile.HeadWearRight[index] < 0 ? 0.0 : crossRailProfile.HeadWearRight[index])) + "," + metre + " ";

                                    }
                                    else
                                    {
                                        //бок износ
                                        sideWearLeft += MMToPixelChartString(izBokl) + "," + metre + " ";
                                        sideWearRight += MMToPixelChartString(izBokR) + "," + metre + " ";
                                        //верт износ 
                                        VertIznosL += MMToPixelChartString(izverL) + "," + metre + " ";
                                        VertIznosR += MMToPixelChartString(izverR) + "," + metre + " ";
                                        //прив износ
                                        ReducedWearLeft += MMToPixelChartString(izPRL) + "," + metre + " ";
                                        ReducedWearRight += MMToPixelChartString(izPRR) + "," + metre + " ";
                                        //износ головки рельса 45 град
                                        HeadWearLeft += MMToPixelChartString(iz45L) + "," + metre + " ";
                                        HeadWearRight += MMToPixelChartString(iz45R) + "," + metre + " ";
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Рисование линий А4 ошибка " + e.Message);
                                }
                            }

                            addParam.Add(new XElement("polyline", new XAttribute("points", sideWearLeft)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", sideWearRight)));

                            addParam.Add(new XElement("polyline", new XAttribute("points", VertIznosL)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", VertIznosR)));

                            addParam.Add(new XElement("polyline", new XAttribute("points", ReducedWearLeft)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", ReducedWearRight)));

                            addParam.Add(new XElement("polyline", new XAttribute("points", HeadWearLeft)));
                            addParam.Add(new XElement("polyline", new XAttribute("points", HeadWearRight)));

                            

                            List<Digression> addDigressions = crossRailProfile.GetDigressions();
                            // заполняет таблицу с доп параметрами
                            if (addDigressions != null && addDigressions.Count != 0)
                            { 
                             var Insert_additional_param_state = AdditionalParametersService.Insert_additional_param_state(addDigressions, kilometer.Number);
                            }

                            var dignatur = new List<DigressionMark> { };

                            foreach (var dig in addDigressions)
                            {
                                int count = dig.Length / 4;
                                count += dig.Length % 4 > 0 ? 1 : 0;

                                //if (count < 4) continue;
                                if (dig.DigName == DigressionName.TreadTiltLeft || dig.DigName == DigressionName.TreadTiltRight || 
                                     dig.DigName == DigressionName.DownhillLeft || dig.DigName == DigressionName.DownhillRight ) continue;

                                dignatur.Add(
                                        new DigressionMark()
                                        {
                                            Digression = dig.DigName,
                                            NotMoveAlert = false,
                                            Meter = dig.Meter,
                                            finish_meter = dig.Kmetr,
                                            Degree = (int)dig.Typ,
                                            Length = dig.Length,
                                            Value = dig.Value,
                                            Count = count,
                                            DigName = dig.GetName(),
                                            PassengerSpeedLimit = -1,
                                            FreightSpeedLimit = -1,
                                            Comment = "",
                                            Diagram_type = "Iznos_relsov"
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

                                picket.WriteNotesToReport(
                                    kilometer,
                                    speedmetres,
                                    addParam,
                                    digElemenets,
                                    0,
                                    0 - 2,
                                    0 + 0.72f,
                                    0 - 2.5f,
                                    0,
                                    0,
                                    this,
                                    ref fourStepOgrCoun,
                                    ref otherfourStepOgrCoun);
                            }
                            addParam.Add( new XAttribute("speedlimit", "Кол. ст. - " + kilometer.GetdigressionsCount + " Огр: " + kilometer.SpeedLimit));

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
                //htReport.Save($@"G:\form\5.Графические диаграммы ГД\ГД-И - износов рельса.html");
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
