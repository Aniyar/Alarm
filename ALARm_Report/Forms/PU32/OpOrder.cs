using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
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
    public class OpOrder : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    XElement tripElem = new XElement("trip",
                        new XAttribute("check", tripProcess.GetProcessTypeName),
                        new XAttribute("road", road),
                        new XAttribute("distance", distance.Name),
                        new XAttribute("periodDate", period.Period),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("ps", tripProcess.Car));

                    var ListS3 = RdStructureService.GetS3(tripProcess.Id) as List<S3>;

                    ListS3 = ListS3.Where(o => o.Ots != "ПрУ").ToList();

                    string lastDirection = String.Empty, lastPut = String.Empty;
                    int lastPd = -1, lastKm = -1, lastPiket = -1;
                    XElement xeDirection = new XElement("direction");
                    foreach (var s3 in ListS3)
                    {
                        
                        if (s3.Naprav.Equals(lastDirection))
                        {
                            if (s3.Pd == lastPd)
                            {
                                if (s3.Put.Equals(lastPut))
                                {
                                    if (s3.Km == lastKm)
                                    {
                                        if (s3.Meter / 100 + 1 == lastPiket)
                                        {
                                            string speedLimit = String.Empty;
                                            if (s3.Ovp == -1)
                                                speedLimit = "--/";
                                            else
                                                speedLimit = s3.Ovp.ToString() + "/";
                                            if (s3.Ogp == -1)
                                                speedLimit += "--";
                                            else
                                                speedLimit += s3.Ogp.ToString();

                                            XElement xeNote = new XElement("note",
                                            new XAttribute("NPd", ""),
                                            new XAttribute("NWay", ""),
                                            new XAttribute("km", ""),
                                            new XAttribute("p", ""),
                                            new XAttribute("m", s3.Meter),
                                            new XAttribute("name", s3.Ots),
                                            new XAttribute("Power", s3.Typ),
                                            new XAttribute("Value", s3.Otkl),
                                            new XAttribute("Length", s3.Len),
                                            new XAttribute("SpeedLimit", speedLimit),
                                            new XAttribute("Note", s3.Primech));
                                            xeDirection.Add(xeNote);
                                        }
                                        else
                                        {
                                            lastPiket = s3.Meter / 100 + 1;

                                            string speedLimit = String.Empty;
                                            if (s3.Ovp == -1)
                                                speedLimit = "--/";
                                            else
                                                speedLimit = s3.Ovp.ToString() + "/";
                                            if (s3.Ogp == -1)
                                                speedLimit += "--";
                                            else
                                                speedLimit += s3.Ogp.ToString();

                                            XElement xeNote = new XElement("note",
                                            new XAttribute("NPd", ""),
                                            new XAttribute("NWay", ""),
                                            new XAttribute("km", ""),
                                            new XAttribute("p", lastPiket),
                                            new XAttribute("m", s3.Meter),
                                            new XAttribute("name", s3.Ots),
                                            new XAttribute("Power", s3.Typ),
                                            new XAttribute("Value", s3.Otkl),
                                            new XAttribute("Length", s3.Len),
                                            new XAttribute("SpeedLimit", speedLimit),
                                            new XAttribute("Note", s3.Primech));
                                            xeDirection.Add(xeNote);
                                        }
                                    }
                                    else
                                    {
                                        lastKm = s3.Km;
                                        lastPiket = s3.Meter / 100 + 1;

                                        string speedLimit = String.Empty;
                                        if (s3.Ovp == -1)
                                            speedLimit = "--/";
                                        else
                                            speedLimit = s3.Ovp.ToString() + "/";
                                        if (s3.Ogp == -1)
                                            speedLimit += "--";
                                        else
                                            speedLimit += s3.Ogp.ToString();

                                        XElement xeNote = new XElement("note",
                                        new XAttribute("NPd", ""),
                                        new XAttribute("NWay", ""),
                                        new XAttribute("km", s3.Km),
                                        new XAttribute("p", lastPiket),
                                        new XAttribute("m", s3.Meter),
                                        new XAttribute("name", s3.Ots),
                                        new XAttribute("Power", s3.Typ),
                                        new XAttribute("Value", s3.Otkl),
                                        new XAttribute("Length", s3.Len),
                                        new XAttribute("SpeedLimit", speedLimit),
                                        new XAttribute("Note", s3.Primech));
                                        xeDirection.Add(xeNote);
                                    }
                                }
                                else
                                {
                                    lastPut = s3.Put;
                                    lastKm = s3.Km;
                                    lastPiket = s3.Meter / 100 + 1;

                                    string speedLimit = String.Empty;
                                    if (s3.Ovp == -1)
                                        speedLimit = "--/";
                                    else
                                        speedLimit = s3.Ovp.ToString() + "/";
                                    if (s3.Ogp == -1)
                                        speedLimit += "--";
                                    else
                                        speedLimit += s3.Ogp.ToString();

                                    XElement xeNote = new XElement("note",
                                    new XAttribute("NPd", ""),
                                    new XAttribute("NWay", s3.Put),
                                    new XAttribute("km", s3.Km),
                                    new XAttribute("p", lastPiket),
                                    new XAttribute("m", s3.Meter),
                                    new XAttribute("name", s3.Ots),
                                    new XAttribute("Power", s3.Typ),
                                    new XAttribute("Value", s3.Otkl),
                                    new XAttribute("Length", s3.Len),
                                    new XAttribute("SpeedLimit", speedLimit),
                                    new XAttribute("Note", s3.Primech));
                                    xeDirection.Add(xeNote);
                                }
                            }
                            else
                            {
                                lastPd = s3.Pd;
                                lastPut = s3.Put;
                                lastKm = s3.Km;
                                lastPiket = s3.Meter / 100 + 1;

                                string speedLimit = String.Empty;
                                if (s3.Ovp == -1)
                                    speedLimit = "--/";
                                else
                                    speedLimit = s3.Ovp.ToString() + "/";
                                if (s3.Ogp == -1)
                                    speedLimit += "--";
                                else
                                    speedLimit += s3.Ogp.ToString();

                                XElement xeNote = new XElement("note",
                                new XAttribute("NPd",s3.Pd),
                                new XAttribute("NWay", s3.Put),
                                new XAttribute("km", s3.Km),
                                new XAttribute("p", lastPiket),
                                new XAttribute("m", s3.Meter),
                                new XAttribute("name", s3.Ots),
                                new XAttribute("Power", s3.Typ),
                                new XAttribute("Value", s3.Otkl),
                                new XAttribute("Length", s3.Len),
                                new XAttribute("SpeedLimit", speedLimit),
                                new XAttribute("Note", s3.Primech));
                                xeDirection.Add(xeNote);
                            }
                        }
                        else
                        {
                            if (!lastDirection.Equals(String.Empty))
                                tripElem.Add(xeDirection);
                            xeDirection = new XElement("direction",
                                new XAttribute("TravelDirection", tripProcess.DirectionName + "(" + tripProcess.DirectionCode + ")"),
                                new XAttribute("date", tripProcess.Date_Vrem.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))));

                            lastDirection = s3.Naprav;
                            lastPd = s3.Pd;
                            lastPut = s3.Put;
                            lastKm = s3.Km;
                            lastPiket = s3.Meter / 100 + 1;

                            string speedLimit = String.Empty;
                            if (s3.Ovp == -1)
                                speedLimit = "--/";
                            else
                                speedLimit = s3.Ovp.ToString() + "/";
                            if (s3.Ogp == -1)
                                speedLimit += "--";
                            else
                                speedLimit += s3.Ogp.ToString();

                            XElement xeNote = new XElement("note",
                                new XAttribute("NPd", s3.Pd),
                                new XAttribute("NWay", s3.Put),
                                new XAttribute("km", s3.Km),
                                new XAttribute("p", lastPiket),
                                new XAttribute("m", s3.Meter),
                                new XAttribute("name", s3.Ots),
                                new XAttribute("Power", s3.Typ),
                                new XAttribute("Value", s3.Otkl),
                                new XAttribute("Length", s3.Len),
                                new XAttribute("SpeedLimit", speedLimit),
                                /*new XAttribute("Repetitions", "n"),
                                new XAttribute("TermOfElimination", "n"),
                                new XAttribute("ResponsibleForElimination", "n"),
                                new XAttribute("EliminationDate", "n"),
                                new XAttribute("WhoAccepted", "n"),
                                new XAttribute("PD", "n"),
                                new XAttribute("PCh", "n"),*/
                                new XAttribute("Note", s3.Primech));
                            xeDirection.Add(xeNote);
                        }
                    }
                    tripElem.Add(xeDirection);
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
