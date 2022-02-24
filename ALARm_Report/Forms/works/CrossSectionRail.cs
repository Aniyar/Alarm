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
    public class CrossSectionRail : Report
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

                    //if (rd_curves_wear_great_13.Count < 1)
                    //    continue;

                    XElement xePages = new XElement("pages",

                        new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        new XAttribute("car", process.Car),
                        new XAttribute("chief", process.Chief),
                        new XAttribute("ps", process.Car),
                        new XAttribute("check", process.GetProcessTypeName),
                        new XAttribute("period", period.Period),
                        new XAttribute("trip_date", process.Trip_date),
                        new XAttribute("distance", distance.Code),
                        new XAttribute("direction", process.DirectionName + " (" + process.DirectionCode + ")"),
                        new XAttribute("track", $"{process.TrackName}"));

                    report.Add(xePages);
                }

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_CrossSectionRail.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_CrossSectionRail.html");
            }
        }
    }
}
