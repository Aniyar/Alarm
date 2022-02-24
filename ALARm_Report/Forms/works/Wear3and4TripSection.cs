using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
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
    public class Wear3and4TripSection : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var mainProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                //var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;

                foreach (var process in mainProcesses)
                {
                    var rd_curves_wear_great_13 = (RdStructureService.GetRdTables(process, 9) as List<RDCurve>).Where(r => r.Wear >= 13.0).ToList();

                    if (rd_curves_wear_great_13.Count < 1)
                        continue;

                    XElement xePages = new XElement("pages",
                        new XAttribute("car", process.Car),
                        new XAttribute("date", DateTime.Now.ToString("dd.MM.yyyy")),
                        new XAttribute("trip_date", process.Trip_date),
                        new XAttribute("distance", distance.Code),
                        new XAttribute("direction", process.DirectionName + " (" + process.DirectionCode + ")"),
                        new XAttribute("track", process.TrackName));

                    foreach (var curve_id in rd_curves_wear_great_13.GroupBy(r => r.Curve_id).Select(r => r.Key))
                    {
                        var curve = MainTrackStructureService.GetMtoObject(curve_id, MainTrackStructureConst.MtoCurve) as Curve;
                        var rd_curves = (RdStructureService.GetRdTables(process, 9) as List<RDCurve>).Where(r => r.Curve_id == curve_id).ToList();

                        XElement xeElements = new XElement("elements",
                            new XAttribute("coords", curve.Start_Km + " км " + curve.Start_M + " м - " + curve.Final_Km + " км " + curve.Final_M + " м"),
                            new XAttribute("maxwear", rd_curves.Max(r => r.Wear)),
                            new XAttribute("avgwear", rd_curves.Average(r => r.Wear)),
                            new XAttribute("deviation", rd_curves.Max(r => r.Wear) >= 18.0 ? 4 : 3),
                            new XAttribute("speed", rd_curves.Min(r => r.PassSpeed).ToString() + "\\" + rd_curves.Min(r => r.FreightSpeed).ToString()),
                            new XAttribute("speed2", rd_curves.Min(r => r.FreightBoost)));

                        xePages.Add(xeElements);
                    }

                    report.Add(xePages);
                }

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_Wear3and4TripSection.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_Wear3and4TripSection.html");
            }
        }
    }
}
