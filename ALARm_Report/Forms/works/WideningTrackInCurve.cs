using ALARm.Core;
using ALARm.Core.Report;
using MetroFramework.Controls;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using ALARm.Services;
using System.Collections.Generic;
using System.Linq;
using ALARm_Report.controls;
using System.Reflection;

namespace ALARm_Report.Forms
{
    public class WideningTrackInCurve : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            var filterForm = new FilterForm();
            var filters = new List<Filter>();
            filters.Add(new FloatFilter() { Name = "Порог. знач. Отжатие(мм)", Value = 0 });

            filterForm.SetDataSource(filters);
            if (filterForm.ShowDialog() == DialogResult.Cancel)
                return;
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(distanceId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel)
                    return;
                admTracksId = choiceForm.admTracksIDs;
            }

            Int64 lastProcess = -1;
            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                List<Curve> curves = (MainTrackStructureService.GetCurves(distanceId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();

                var filterForm1 = new FilterForm();
                var filters1 = new List<Filter>();
                var roadName = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);

                var min = curves.Select(o => o.Start_Km).Min();
                var max = curves.Select(o => o.Final_Km).Max();

                filters1.Add(new FloatFilter() { Name = "Начало (км)", Value = min });
                filters1.Add(new FloatFilter() { Name = "Конец (км)", Value = max });

                filterForm1.SetDataSource(filters1);
                if (filterForm1.ShowDialog() == DialogResult.Cancel)
                    return;
                //фильтр по выбранным км
                curves = curves.Where(o => ((float)filters1[0].Value <= o.Start_Km && o.Final_Km <= (float)filters1[1].Value)).ToList();


                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");
                XElement xePages = new XElement("pages");

                var road = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        if (tripProcesses.Count == 0 || curves.Count < 1)
                        {
                            MessageBox.Show(Properties.Resources.paramDataMissing);
                            return;
                        }

                        var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;
                        CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;
                        string site = "Неизвестный";
                        if (curvesAdmUnits.Any())
                        {
                            if (!curvesAdmUnit.StationStart.Equals("Неизвестный") && !curvesAdmUnit.StationFinal.Equals("Неизвестный"))
                            {
                                site = curvesAdmUnit.StationStart + "-" + curvesAdmUnit.StationFinal;
                            }
                            else if (curvesAdmUnit.StationStart.Equals("Неизвестный") && !curvesAdmUnit.StationFinal.Equals("Неизвестный"))
                            {
                                site = curvesAdmUnit.StationFinal;
                            }
                            else if (!curvesAdmUnit.StationStart.Equals("Неизвестный") && curvesAdmUnit.StationFinal.Equals("Неизвестный"))
                            {
                                site = curvesAdmUnit.StationStart;
                            }
                        }
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        xePages.Add(new XAttribute("distance", ((AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId)).Code),
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("km", curves.Min(c => c.Start_Km).ToString() + "-" + curves.Max(c => c.Final_Km).ToString()),
                            new XAttribute("site", site),
                            new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("trackinfo", tripProcess.DirectionName + " (" + tripProcess.DirectionCode + "), Путь: " + trackName + ", ПЧ: " + distance.Code),
                            new XAttribute("track", curvesAdmUnit.Track),
                            new XAttribute("car", tripProcesses[0].Car),
                            new XAttribute("check", tripProcesses[0].GetProcessTypeName),
                            new XAttribute("road", road),
                            new XAttribute("chief", tripProcesses[0].Chief),
                            new XAttribute("direction", curvesAdmUnit.Direction)
                            );
                     
                           
                        int i = 1;
                        foreach (var curve in curves)
                        {

                            //доп параметры
                            var addParams = AdditionalParametersService.GetCrossRailProfileFromDB(curve, tripProcesses[0].Trip_id);

                            if (addParams.Any())
                            {
                                var cskjdbfvjsd = 0;
                            }
                            List<RDCurve> rdcs =
                                MainTrackStructureService.GetMtoObjects(curve.Id, MainTrackStructureConst.MtoRdCurve) as List<RDCurve>;

                            CurveParams curveParams =
                                RdStructureService.GetCurveParams(curve.Id) as CurveParams;

                            curve.Straightenings =
                                (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.Start_Km * 1000 + st.Start_M).ToList();
                            var shpala = MainTrackStructureService.GetMtoObjectsByCoord(tripProcesses[0].Date_Vrem, curve.Start_Km, MainTrackStructureConst.MtoCrossTie, tripProcesses[0].DirectionName, curvesAdmUnit.Track) as List<CrossTie>;
                            var Gauge = MainTrackStructureService.GetGaugesByCurve(tripProcesses, curve, curvesAdmUnit.Track);
                            var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcesses[0].Date_Vrem, curve.Start_Km, MainTrackStructureConst.MtoSpeed, tripProcesses[0].DirectionName, curvesAdmUnit.Track) as List<Speed>;

                            var lim = curve.Straightenings.Any() ? curve.Straightenings.Min(s => s.Wear) : 0.0;

                            //отжатие todo
                            var new_lim = new List<double> { };
                            if (Gauge.Count() > 0)
                            {
                                for (int ii = 1; ii < Gauge.Count; ii++)
                                {
                                    new_lim.Add(Math.Abs(Gauge[i] - Gauge[i - 1]));
                                }
                            }
                            if ((float)(float)filters[0].Value > (new_lim.Count > 0 ? new_lim.Max() : 0)) continue;

                            XElement xeElements = new XElement("elements",
                                new XAttribute("order", i),

                                new XAttribute("startkm", curve.Start_Km),
                                new XAttribute("startm", curve.Start_M),
                                new XAttribute("finalkm", curve.Final_Km),
                                new XAttribute("finalm", curve.Final_M),
                                new XAttribute("Vpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),

                                new XAttribute("radius", Convert.ToInt32(rdcs.Any() ? rdcs.Average(r => 17860 / (Math.Abs(r.Radius) + 1)) : curve.Straightenings.Average(s => s.Radius))),
                                new XAttribute("radiusmin", Convert.ToInt32(rdcs.Any() ? rdcs.Min(r => 17860 / (Math.Abs(r.Radius) + 1)) : curve.Straightenings.Min(s => s.Radius))),

                                new XAttribute("widthmax", Gauge.Count > 0 ? Gauge.Max().ToString("0.0") : "нет данных"),
                                new XAttribute("width", Gauge.Count > 0 ? Gauge.Average().ToString("0.0") : "нет данных"),

                                new XAttribute("wearmax", addParams.Count > 0 ? addParams.Max(r => curve.Side_id == (int)Side.Left ? r.Bok_l : r.Bok_r).ToString("0.0") : "нет данных"),
                                new XAttribute("wear", addParams.Count > 0 ? addParams.Average(r => curve.Side_id == (int)Side.Left ? r.Bok_l : r.Bok_r).ToString("0.0") : "нет данных"),

                                new XAttribute("broadeningmax", Gauge.Count > 0 ? Math.Abs(Gauge.Max() - 1520).ToString("0.0") : "нет данных"),
                                new XAttribute("broadening", Gauge.Count > 0 ? Math.Abs(Gauge.Average() - 1520).ToString("0.0") : "нет данных"),

                                new XAttribute("brace", shpala.Count > 0 ? shpala[0].Name : "нет данных"),
                                new XAttribute("fastening", curveParams.Fastening),

                                new XAttribute("limit", new_lim.Count > 0 ? new_lim.Max().ToString("0.00") : "нет данных"));

                            xePages.Add(xeElements);

                            i++;
                        }
                    }
                }

                report.Add(xePages);
                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_WideningTrackInCurve.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_WideningTrackInCurve.html");
            }
        }
    }
}
