using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using MetroFramework;
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
using ElCurve = ALARm.Core.ElCurve;


namespace ALARm_Report.Forms
{
    public class EscortScreen : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                var tripProcesses = RdStructureService.GetMainParametersProcesses(period, parentId, true);
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
                        new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                        new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                        new XAttribute("road", roadName),
                        new XAttribute("distance", distance.Code),
                        new XAttribute("periodDate", period.Period),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("ps", tripProcess.Car)
                    );
                    List<Escort> escorts = RdStructureService.GetEscorts(tripProcess.Trip_id);
                    string distanceName = string.Empty;
                    StringBuilder escortFullNames = new StringBuilder();
                    int escortCount = 0;
                    foreach (var escort in escorts)
                    {
                        if ((escort.Distance_Name != distanceName) && (distanceName != string.Empty))
                        {
                            tripElem.Add(new XElement("escort",
                                new XAttribute("distance", distance.Code),
                                new XAttribute("fullname", escortFullNames.ToString()),
                                new XAttribute("count", escortCount)
                                ));
                            escortCount = 0;
                            escortFullNames = new StringBuilder();
                            distanceName = escort.Distance_Name;
                        }
                        escortCount++;
                        escortFullNames.Append($"{escort.FullName};  ");
                        if (escorts.IndexOf(escort) == escorts.Count() - 1)
                        {
                            distanceName = escort.Distance_Name;
                            tripElem.Add(new XElement("escort",
                                new XAttribute("distance", distance.Code),
                                new XAttribute("fullname", escortFullNames.ToString()),
                                new XAttribute("count", escortCount)
                                ));
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
            finally{
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report.html");
            }
        }

        public override string ToString()
        {
            return "Отступления 2 степени, близкие к 3";
        }

    }
}