using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
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
    public class ProvenPathDoNot : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                var tripProcesses = RdStructureService.GetProcess(period, parentId, ProcessType.VideoProcess);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    XElement tripElem = new XElement("trip",
                        new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        new XAttribute("check", tripProcess.GetProcessTypeName),
                        new XAttribute("road", roadName),
                        new XAttribute("distance", distance.Code),
                        new XAttribute("periodDate", period.Period),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("car", tripProcess.Car));

                    var NotCheckedKm = RdStructureService.GetDop2(tripProcess.Trip_id, distance.Id) as List<NotCheckedKm>;
                    if (NotCheckedKm == null || NotCheckedKm.Count == 0)
                    {


                        var Note = new XElement("Note",
                            new XAttribute("Direction", "-"),
                            new XAttribute("Way", "-"),
                            new XAttribute("Start", "-"),
                            new XAttribute("Final", "-"),
                            new XAttribute("Note", "-"),
                            new XAttribute("carrr", "-")
                            );
                        tripElem.Add(Note);


                    }
                    else 
                    {
                        foreach (var elem in NotCheckedKm)
                        {


                            // var p = Math.Round(num % (int)num, 2);
                            var StartKm = (elem.Start_km + elem.Start_m / 1000.0);
                            var FinalKm = (elem.Final_km + elem.Final_m / 1000.0);
                            var Note = new XElement("Note",
                                new XAttribute("Direction", elem.Direction_name_code),
                                new XAttribute("Way", elem.Track_name),
                                new XAttribute("Start", StartKm),
                                new XAttribute("Final", FinalKm),
                                new XAttribute("Note", elem.Note),
                                new XAttribute("carrr", Math.Abs(elem.Final_km * 1000 + elem.Final_m - elem.Start_km * 1000 - elem.Start_m))
                                );
                            tripElem.Add(Note);
                        }
                    }
                    
                    report.Add(tripElem);
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
