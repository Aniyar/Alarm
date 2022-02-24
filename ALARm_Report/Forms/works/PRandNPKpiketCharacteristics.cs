using ALARm.Core;
using ALARm.Core.Report;
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
    public class PRandNPKpiketCharacteristics : Report
    {

        private string ValueToFractionStr(double a)
        {
            int b = Convert.ToInt32(1 / a);
            return "1/" + b.ToString();
        }
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
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var mainProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (!mainProcesses.Any())
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                foreach (var process in mainProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var kms = RdStructureService.GetKilometerTrip(process.Trip_id);
                        progressBar.Maximum = kms.Count;

                        XElement xePages = new XElement("pages",
                                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                                            new XAttribute("road", road),
                                            new XAttribute("distance", distance.Code),
                                            new XAttribute("period", period.Period),
                                            new XAttribute("check",  process.GetProcessTypeName),
                                            new XAttribute("put", "" + process.TrackName),
                                            
                                            new XAttribute("car", process.Car),
                                            new XAttribute("chief", process.Chief));

                        XElement xeTracks = new XElement("tracks",
                                new XAttribute("trackinfo", $"{process.DirectionName}({process.DirectionCode}) Путь: {trackName}"));
                        var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileDFPR3(process.Trip_id);
                        List<Curve> curves = RdStructureService.GetCurvesInTrip(process.Trip_id);

                         if (DBcrossRailProfile == null || DBcrossRailProfile.Count == 0) continue;

                        var filterForm1 = new FilterForm();
                        var filters1 = new List<Filter>();

                        var min1 = DBcrossRailProfile.Select(o=>o.Km).Min();
                        var max1 = DBcrossRailProfile.Select(o => o.Km).Max();

                        filters1.Add(new FloatFilter() { Name = "Начало (км)", Value = min1 });
                        filters1.Add(new FloatFilter() { Name = "Конец (км)", Value = max1 });

                        filterForm1.SetDataSource(filters1);
                        if (filterForm1.ShowDialog() == DialogResult.Cancel)
                            return;

                        //фильтр по выбранным км
                        DBcrossRailProfile = DBcrossRailProfile.Where(o => ((float)filters1[0].Value <= o.Km && o.Km <= (float)filters1[1].Value)).ToList();


                        foreach (var elem in DBcrossRailProfile)
                        {
                            var realPiketCoord = elem.Km + (elem.Picket * 100 + 1) / 10000.0;
                            var filter_curves = curves.Where(o => o.RealStartCoordinate <= realPiketCoord &&
                                                                         realPiketCoord <= o.RealFinalCoordinate).ToList();

                            foreach (var curve in filter_curves)
                            {
                                curve.Elevations = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();
                                curve.Straightenings = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();
                            }
                            var DBcrossRailProfileRadius = AdditionalParametersService.GetCrossRailProfileDFPR3Radius(track_id, process.Date_Vrem,elem.Km,elem.Meter);
                            var speed = MainTrackStructureService.GetMtoObjectsByCoord(process.Date_Vrem, elem.Km,
                                MainTrackStructureConst.MtoSpeed, process.DirectionName, "1") as List<Speed>;
                            

                            xeTracks.Add(new XElement("elements",
                                                new XAttribute("km", elem.Km),
                                                new XAttribute("piket", elem.Piket),
                                                new XAttribute("railside", "л."),
                                                new XAttribute("radius", filter_curves.Any() ? filter_curves.First().Straightenings.First().Radius.ToString() : ""),
                                                new XAttribute("PRmedium", elem.Avg_pu_l.ToString("0.###").Replace(',', '.') + "(" + ValueToFractionStr(elem.Avg_pu_l) + ")"),
                                                new XAttribute("PRSKO", elem.Sko_pu_l.ToString("0.####").Replace(',', '.')),
                                                new XAttribute("NPKmedium", elem.Avg_npk_l.ToString("0.###").Replace(',', '.') + "(" + ValueToFractionStr(elem.Avg_npk_l) + ")"),
                                                new XAttribute("NPKSKO", elem.Sko_npk_l.ToString("0.####").Replace(',', '.')),
                                                new XAttribute("speed", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight : "-/-")));
                            xeTracks.Add(new XElement("elements",
                                                new XAttribute("km", elem.Km),
                                                new XAttribute("piket", elem.Piket),
                                                new XAttribute("railside", "пр."),
                                                new XAttribute("radius", filter_curves.Any() ? filter_curves.First().Straightenings.First().Radius.ToString() : ""),
                                                new XAttribute("PRmedium", elem.Avg_pu_r.ToString("0.###").Replace(',', '.') + "(" + ValueToFractionStr(elem.Avg_pu_r) + ")"),
                                                new XAttribute("PRSKO", elem.Sko_pu_r.ToString("0.####").Replace(',', '.')),
                                                new XAttribute("NPKmedium", elem.Avg_npk_r.ToString("0.###").Replace(',', '.') + "(" + ValueToFractionStr(elem.Avg_npk_r) + ")"),
                                                new XAttribute("NPKSKO", elem.Sko_npk_r.ToString("0.####").Replace(',', '.')),
                                                new XAttribute("speed", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight : "-/-")));

                        }
                        xePages.Add(xeTracks);
                        report.Add(xePages);
                    }
                
                }

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {

                htReport.Save(Path.GetTempPath() + "/report_PRandNPKpiketCharacteristics.html");

            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_PRandNPKpiketCharacteristics.html");
            }
        }
    }
}
