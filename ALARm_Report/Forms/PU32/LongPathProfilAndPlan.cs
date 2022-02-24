using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm_Report.Forms
{
    public class LongPathProfilAndPlan : GraphicDiagrams
    {
        private new float MMToPixelChart(float mm)
        {
            return widthInPixel / widthImMM * mm + xBegin;
        }
        private new string MMToPixelChartString(float mm)
        {
            return (widthInPixel / widthImMM * mm + xBegin).ToString().Replace(",", ".");
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


            XDocument htReport = new XDocument();

            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(parentId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel)
                    return;
                admTracksId = choiceForm.admTracksIDs;
            }

            List<Curve> curves = (MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
            diagramName = "Длинные неровности";
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);
                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);
                //var tripProcesses = RdStructureService.GetAdditionalParametersProcess(period);

                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;
                CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;

                foreach (var tripProcess in tripProcesses)
                {
                    tripProcess.Direction = Direction.Direct;

                    foreach (var track_id in admTracksId)
                    {
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);

                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();

                        if (kilometers.Count == 0) continue;

                        var trackName = AdmStructureService.GetTrackName(track_id);

                        tripProcess.TrackName = trackName.ToString();
                        tripProcess.TrackID = track_id;

                        var raw_rd_profile = RdStructureService.GetRdTables(tripProcess, 1) as List<RdProfile>;
                        //Реперные точки
                        var raw_RefPoints = MainTrackStructureService.GetRefPointsByTripIdToDate(track_id, tripProcess.Date_Vrem);
                        if (!raw_RefPoints.Any()) continue;
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var min = raw_RefPoints.Select(o => o.Km).Min();
                        var max = raw_RefPoints.Select(o => o.Km).Max();

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = min });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = max });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;
                        //фильтр
                        raw_RefPoints = raw_RefPoints.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList();
                        if (!raw_RefPoints.Any())
                            continue;

                        diagramName = "ГД-ДН";

                        XElement addParam = new XElement("addparam",
                                   new XAttribute("distance", ((AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId)).Code),
                                        new XAttribute("tripdate", tripProcess.Date_Vrem.ToString("dd/MM/yyyy HH:mm")),
                                        new XAttribute("PS", tripProcess.Car),
                                        new XAttribute("roadName", road),
                                        new XAttribute("track", trackName),
                                        new XAttribute("chief", tripProcess.Chief),
                                        new XAttribute("km", filters[0].Value + " - " + filters[1].Value),
                                        new XAttribute("direction", tripProcess.DirectionName + "(" + tripProcess.DirectionCode + ")"),
                             new XAttribute("top-title",

                                $"{tripProcess.DirectionName}({tripProcess.DirectionCode})  Путь:{trackName} ПЧ:{distance.Code}  Км:" /*+ filters[0].Value + " - " + filters[1].Value*/),
                            new XAttribute("right-title", (
                                copyright + ": " + "ПО " + softVersion + "  " +
                                systemName + ":" + "(none" + ") (БПД от " +
                                MainTrackStructureService.GetModificationDate() + ") <" + AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, false) + ">" +
                                    "<" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ">" +
                                         "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.Direction.ToString())) + ">" +
                                               "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.Car_Position.ToString())) + ">" +
                                "<" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ">" +
                                //"<" + ">" +
                                //"<" + ">" +
                                "<" + period.PeriodMonth + "-" + period.PeriodYear + " " + (tripProcess.Trip_Type == TripType.Control ? "контр." : tripProcess.Trip_Type == TripType.Work ? "раб." : "доп.") +
                                    " Проезд:" + diagramName + ">" + " Л: " + 1)),

                            new XAttribute("viewbox", "0 0 600 1000"),
                            new XAttribute("minY", 0),
                            new XAttribute("maxY", 1000),
                            new XAttribute("minYLine", 25),
                            new XElement("xgrid",

                            new XAttribute("x0", xBegin),
                            new XAttribute("x00", MMToPixelChart(2.74f)),
                            new XAttribute("x000", MMToPixelChart(16.44f)),
                            new XAttribute("x0000", MMToPixelChart(30.90f)),

                            new XAttribute("x1", MMToPixelChart(32.64f)),
                            new XAttribute("l1", MMToPixelChart(49.04f)),

                                new XAttribute("x2", MMToPixelChart(65.54f)),
                                new XAttribute("l4", MMToPixelChart(67.03f)),
                                new XAttribute("l5", MMToPixelChart(79.49f)),
                                new XAttribute("l6", MMToPixelChart(92.20f)),

                            new XAttribute("x3", MMToPixelChart(93.69f)),
                            new XAttribute("l7", MMToPixelChart(105.65f)),

                            new XAttribute("x4", MMToPixelChart(117.37f)),
                            //астынгы сызык 
                            new XAttribute("maxY1", MMToPixelChart(239.47f)),
                            new XAttribute("maxY2", MMToPixelChart(245.70f)),
                            new XAttribute("maxY3", MMToPixelChart(251.93f)),
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


                        var kmElements = new XElement("km");
                        string Krivizna = string.Empty;
                        string NerovPlana = string.Empty;
                        string DevData = string.Empty;
                        string SlopeData = string.Empty;
                        string ProfileData = string.Empty;

                        //Продольный профиль------------------------------------------------------------------------


                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                        var start = false;
                        var RefPoints = new List<RefPoint> { };
                        var rd_profile = new List<RdProfile> { };
                        RefPoint PREV = null;

                        foreach (var rp in raw_RefPoints)
                        {
                            if (start == true)
                            {
                                var st = raw_rd_profile.Where(o => o.Km * 1000 + o.Meter > rp.Km * 1000 + rp.Meter).ToList();

                                if (st.Count == 0)
                                {
                                    RefPoints.Add(PREV ?? rp);
                                    break;
                                }
                                PREV = rp;
                            }
                            if (start == false)
                            {
                                var st = raw_rd_profile.Where(o => o.Km * 1000 + o.Meter <= rp.Km * 1000 + rp.Meter).ToList();

                                if (st.Count > 0)
                                {
                                    RefPoints.Add(rp);
                                    start = true;
                                }
                            }
                        }
                        if (RefPoints.Count == 1)
                        {
                            RefPoints.Add(raw_RefPoints.Last());
                        }

                        rd_profile = raw_rd_profile.Where(o =>
                                                              RefPoints.First().Km * 1000 + RefPoints.First().Meter <= o.Km * 1000 + o.Meter &&
                                                              RefPoints.Last().Km * 1000 + RefPoints.Last().Meter >= o.Km * 1000 + o.Meter
                                                         ).ToList();
                        RefPoints = raw_RefPoints.Where(o =>
                                                            RefPoints.First().Km * 1000 + RefPoints.First().Meter <= o.Km * 1000 + o.Meter &&
                                                            RefPoints.Last().Km * 1000 + RefPoints.Last().Meter >= o.Km * 1000 + o.Meter
                                                       ).ToList();

                        var startP = rd_profile.IndexOf(rd_profile.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList().First());
                        var finalP = rd_profile.IndexOf(rd_profile.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList().Last());

                        var countP = finalP - startP;

                        var track_profile = rd_profile;
                        //var track_profile = rd_profile.Where(r => r.Track_id == trackId).OrderBy(r => r.X).ToList();


                        var trackId = track_id;
                        var tripDate = tripProcess.Date_Vrem;

                        List<int> x_RefPoints = new List<int>();
                        List<int> x_track_profile = new List<int>();
                        List<int> xInd = new List<int>();
                        List<double> yr = new List<double>();

                        int j = 0;
                        foreach (var elem in track_profile)
                        {
                            var t = RefPoints.Where(o => o.Km == elem.Km && o.Meter == elem.Meter).ToList();
                            if (t.Count > 0)
                            {
                                x_RefPoints.Add(j + 1);
                                xInd.Add(j);
                                yr.Add(elem.Y);
                            }
                            x_track_profile.Add(j + 1);

                            j++;
                        }
                        var ccccc = track_profile.Last();

                        ProfileData profileData = new ProfileData(
                                                                 //profile
                                                                 x_track_profile,
                                                                 track_profile.Select(p => p.Y).ToList(),

                                                                 track_profile.Select(p => p.Deviation).ToList(),

                                                                 //repers
                                                                 RefPoints.Select(p => p.Mark).ToList(),
                                                                 x_RefPoints,

                                                                 yr,
                                                                 xInd,
                                                                 startP,
                                                                 countP
                                                                 );

                        var original = profileData.GraphProfile();
                        //Линеар на график профиля 0,05
                        profileData.getLinearGraphNew();

                        //----------------------------------------------------------------------------------------------

                        var mainParamRiht = MainParametersService.GetMainParametersFromDBMeter(tripProcess.Trip_id);
                        mainParamRiht = mainParamRiht.Where(o =>
                                                              RefPoints.First().Km * 1000 + RefPoints.First().Meter <= o.Km * 1000 + o.Meter &&
                                                              RefPoints.Last().Km * 1000 + RefPoints.Last().Meter >= o.Km * 1000 + o.Meter
                                                              ).ToList();

                        var mainParameters = RdStructureService.GetRdTablesByKM(
                                                                                tripProcess,
                                                                                RefPoints.First().Km * 1000 + RefPoints.First().Meter,
                                                                                RefPoints.Last().Km * 1000 + RefPoints.Last().Meter) as List<RdProfile>;


                        //определяем входят ли в кривые участки
                        var GetCurvesList = MainTrackStructureService.GetCurveByTripIdToDate(tripProcess) as List<Curve>;
                        for (int jj = 0; jj < mainParameters.Count; jj++)
                        {
                            var current_coord = mainParameters[jj].Km * 1000 + mainParameters[jj].Meter;

                            var curve = GetCurvesList.Where(o => o.Start_coord <= current_coord && current_coord <= o.Final_coord).ToList();

                            mainParameters[jj].IsCurve = curve.Count() > 0 ? true : false;
                        }

                        var testInd = mainParameters.Count();
                        var rectHeig = 950.65 / testInd;
                        var StrLInd = 0.0;
                        var prevKm = -1;


                        //var Data = profileData.ORIGINminusLBOplusLR();


                        var RollAver_nerov = new List<double>();

                        var profile_min = -1;
                        var profile_max = -1;

                        for (int ii = 0; ii < mainParamRiht.Count - 1; ii++)
                        {
                            //КМ
                            if (prevKm != mainParameters[ii].Km)
                            {
                                kmElements.Add(new XElement("text", mainParameters[ii].Km + " км",
                                    new XAttribute("y", -603),
                                    new XAttribute("x", StrLInd))
                                );
                                prevKm = mainParameters[ii].Km;
                            }

                            var nerov = (mainParameters[ii].IsCurve == true ? 0.0 : (mainParamRiht[ii].Stright_left - mainParamRiht[ii].Stright_avg));
                            if (RollAver_nerov.Count() >= 50)
                            {
                                RollAver_nerov.Add(nerov);
                                nerov = (RollAver_nerov.Skip(RollAver_nerov.Count() - 50).Take(50).Average());
                            }
                            else
                            {
                                RollAver_nerov.Add(nerov / 2.5);
                            }

                            NerovPlana += ((mainParameters[ii].IsCurve == true ? 0.0 : (nerov * 25 * 2) * Math.Exp(-Math.Abs(nerov / 2))) + 55.0).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";
                            Krivizna += (mainParamRiht[ii].Stright_avg * 1 + 186.3).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";

                            double grapH = 151.0;
                            double middleH = grapH / 2.0;
                            double y_coef = grapH / 335.0;

                            if (ii < profileData.ORIGINminusLBOplusLRrrr.Count())
                            {
                                //Девиация
                                var calcY_Dev = (y_coef * (profileData.linearPointYNew[ii] - profileData.ORIGINminusLBOplusLRrrr[ii]) * 1000.0);

                                DevData += (calcY_Dev + 307.985657).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";


                                if (ii < profileData.ORIGINminusLBOplusLRrrr.Count() - 100)
                                {
                                    //Уклон
                                    var calcY_slope = (y_coef * (profileData.ORIGINminusLBOplusLRrrr[ii] - profileData.ORIGINminusLBOplusLRrrr[ii + 100]) * 30.0);

                                    SlopeData += (calcY_slope + 412.963226).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";
                                }
                                else
                                {
                                    SlopeData += (0.0 + 412.963226).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";
                                }

                                //Продольный
                                var calcY = (middleH - y_coef * (profileData.ORIGINminusLBOplusLRrrr[ii] - profileData.ORIGINminusLBOplusLRrrr[0]) * 10.0);

                                int prom2 = 0;
                                prom2 = (int)calcY / 151;

                                if (calcY > 151)
                                {
                                    calcY = calcY - 151.0 * prom2;
                                    profile_max = (int)profileData.ORIGINminusLBOplusLRrrr[ii];
                                }
                                else if (calcY < 0)
                                {
                                    calcY = calcY + 151.0 - 151.0 * prom2;
                                    profile_min = (int)profileData.ORIGINminusLBOplusLRrrr[ii];
                                }

                                ProfileData += (calcY + 460).ToString().Replace(",", ".") + "," + StrLInd.ToString().Replace(",", ".") + " ";
                            }
                            StrLInd = StrLInd + rectHeig;
                        }
                        var linesElem = new XElement("lines");

                        linesElem.Add(new XElement("NerovPlana", NerovPlana));
                        linesElem.Add(new XElement("Krivizna", Krivizna));

                        linesElem.Add(new XElement("Dev", DevData));
                        linesElem.Add(new XElement("Slope", SlopeData));
                        linesElem.Add(new XElement("ProfileData", ProfileData));

                        //linesElem.Add(new XElement("Krivizna", OtklOtPryam));

                        addParam.Add(
                            new XAttribute("profile_max", (int)profileData.ORIGINminusLBOplusLRrrr.Max()),
                            new XAttribute("profile_min", (int)profileData.ORIGINminusLBOplusLRrrr.Min()));

                        addParam.Add(linesElem);
                        addParam.Add(kmElements);

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
