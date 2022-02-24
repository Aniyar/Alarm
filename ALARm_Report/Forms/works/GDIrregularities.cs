using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
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
    public class GDIrregularities : Report
    {
        private double y_coef = 1200.0 / 5000.0;
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                var mainProcesses = RdStructureService.GetMainParametersProcesses(period, distanceId);
                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                var road = AdmStructureService.GetUnit(AdmStructureConst.AdmRoad, nod.Parent_Id) as AdmUnit;

                foreach (var processId in mainProcesses.GroupBy(m => m.Id).Select(m => m.Key))
                {
                    var process = mainProcesses.Where(m => m.Id == processId).First();
                    var irregularitiesProfile = ((List<RdIrregularity>)RdStructureService.GetRdTables(process, 0)).Where(i => i.Belong == 1).ToList();
                    var irregularitiesPlan = ((List<RdIrregularity>)RdStructureService.GetRdTables(process, 0)).Where(i => i.Belong == 0).ToList();
                    var profile = RdStructureService.GetRdTables(process, 1) as List<RdProfile>;

                    if (irregularitiesProfile.Count == 0 || irregularitiesPlan.Count == 0 || profile.Count == 0)
                        continue;

                    int startAbsCoord = Math.Min(Math.Min(irregularitiesProfile.Min(i => i.Km * 1000 + i.Meter), irregularitiesPlan.Min(i => i.Km * 1000 + i.Meter)), profile.Min(p => p.X));
                    int finalAbsCoord = Math.Max(Math.Max(irregularitiesProfile.Max(i => i.Km * 1000 + i.Meter), irregularitiesPlan.Max(i => i.Km * 1000 + i.Meter)), profile.Max(p => p.X));

                    while (startAbsCoord < finalAbsCoord)
                    {
                        int absCoords = startAbsCoord + 5000;

                        XElement xePages = new XElement("pages",
                            new XAttribute("distance", distance.Code),
                            new XAttribute("tripdate", process.Trip_date),
                            new XAttribute("car", process.Car),
                            new XAttribute("track", process.TrackName),
                            new XAttribute("direction", process.DirectionName + " (" + process.DirectionCode + ")"));

                        string irregularitiesProfileGraph = "", irregularitiesPlanGraph = "", profileGraph = "", slopeGraph = "";
                        double x_coef_irr_profile = 90.0;
                        if (irregularitiesProfile.Where(i => i.Km * 1000 + i.Meter >= startAbsCoord && i.Km * 1000 + i.Meter <= absCoords).Any())
                            x_coef_irr_profile /= (irregularitiesProfile.Where(i => i.Km * 1000 + i.Meter >= startAbsCoord && i.Km * 1000 + i.Meter <= absCoords).Max(i => Math.Abs(i.Amount)));
                        double x_coef_irr_plan = 90.0;
                        if (irregularitiesPlan.Where(i => i.Km * 1000 + i.Meter >= startAbsCoord && i.Km * 1000 + i.Meter <= absCoords).Any())
                            x_coef_irr_plan /= (irregularitiesPlan.Where(i => i.Km * 1000 + i.Meter >= startAbsCoord && i.Km * 1000 + i.Meter <= absCoords).Max(i => Math.Abs(i.Amount)));
                        double x_coef_profile = 225.0 / 1100.0;
                        double x_coef_slope = 135.0 / 10.0;

                        for (int i = startAbsCoord; i < absCoords; i++)
                        {
                            if (i % 1000 == 0)
                            {
                                XElement xeKms = new XElement("kms",
                                    new XAttribute("y1", (i - startAbsCoord) * y_coef),
                                    new XAttribute("y", (i - startAbsCoord) * y_coef),
                                    new XAttribute("text", i / 1000));

                                xePages.Add(xeKms);
                            }
                            else if (i % 100 == 0)
                            {
                                XElement xeMeters = new XElement("meters",
                                    new XAttribute("y1", (i - startAbsCoord) * y_coef));

                                xePages.Add(xeMeters);
                            }

                            foreach (var irr in irregularitiesProfile.Where(ir => ir.Km * 1000 + ir.Meter == i))
                            {
                                double y = (i - startAbsCoord) * y_coef;
                                double x = irr.Amount * x_coef_irr_profile;
                                irregularitiesProfileGraph += x.ToString().Replace(',', '.') + "," + y.ToString().Replace(',', '.') + " ";
                            }

                            foreach (var irr in irregularitiesPlan.Where(ir => ir.Km * 1000 + ir.Meter == i))
                            {
                                double y = (i - startAbsCoord) * y_coef;
                                double x = irr.Amount * x_coef_irr_plan;
                                irregularitiesPlanGraph += x.ToString().Replace(',', '.') + "," + y.ToString().Replace(',', '.') + " ";
                            }

                            foreach (var prof in profile.Where(p => p.X == i))
                            {
                                double y = (i - startAbsCoord) * y_coef;
                                double x = (prof.Y % 1100.0) * x_coef_profile;
                                profileGraph += x.ToString().Replace(',', '.') + "," + y.ToString().Replace(',', '.') + " ";
                            }
                        }

                        for (int i = startAbsCoord; i < absCoords; i++)
                        {
                            double profile1 = -1, profile2 = -1;

                            foreach (var prof in profile.Where(p => p.X == i))
                            {
                                profile1 = prof.Y;
                            }

                            foreach (var prof in profile.Where(p => p.X == i + 10))
                            {
                                profile2 = prof.Y;
                            }

                            if (profile1 != -1 && profile2 != -1)
                            {
                                double y = (i - startAbsCoord) * y_coef;
                                double x = (profile2 - profile1) * x_coef_slope;
                                slopeGraph += x.ToString().Replace(',', '.') + "," + y.ToString().Replace(',', '.') + " ";
                            }
                        }

                        xePages.Add(new XAttribute("irregularities_plan", irregularitiesPlanGraph),
                            new XAttribute("irregularities_profile", irregularitiesProfileGraph),
                            new XAttribute("profile", profileGraph),
                            new XAttribute("slope", slopeGraph),
                            new XAttribute("irr_p_x1", 30 * x_coef_irr_plan),
                            new XAttribute("irr_p_x2", -30 * x_coef_irr_plan),
                            new XAttribute("irr_p_x3", 50 * x_coef_irr_profile),
                            new XAttribute("irr_p_x4", -50 * x_coef_irr_profile));

                        report.Add(xePages);

                        startAbsCoord += 5000;
                    }
                }

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_GDIrregularities.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_GDIrregularities.html");
            }
        }
    }
}
