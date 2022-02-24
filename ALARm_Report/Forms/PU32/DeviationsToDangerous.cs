using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;


namespace ALARm_Report.Forms
{
    public class DeviationsToDangerous : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                List<Curve> curves = (MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
                XDocument xdReport = new XDocument();

                var distance =
                    AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);


                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    List<Digression> notes = RdStructureService.DeviationsToDangerous(tripProcess.Trip_id, distance.Code, new int[] { 3 });
                    var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;

                    CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;

                    var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                    if (kms.Count() == 0) continue;
                    var directName = AdditionalParametersService.DirectName(tripProcess.Id, (int)tripProcess.Direction);

                    XElement tripElem = new XElement("trip",
                        new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                        new XAttribute("direction", curvesAdmUnit.Direction),
                        new XAttribute("km", kms.Min() + "-" + kms.Max()),
                        new XAttribute("track", curvesAdmUnit.Track),
                        new XAttribute("check", tripProcess.GetProcessTypeName),
                        new XAttribute("road", roadName),
                        new XAttribute("distance", distance.Code),
                        new XAttribute("periodDate", period.Period),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("ps", tripProcess.Car));
                   
                    string previousDirectionName = string.Empty;
                    string previousTrackName = string.Empty;
                    string previousPCHUName = string.Empty;
                    string previousPDName = string.Empty;
                    string previousPDBName = string.Empty;

                    XElement directionElement = new XElement("direction");
                    XElement tracklElement = new XElement("track");
                    XElement PCHUElement = new XElement("PCHU");
                    XElement PDElement = new XElement("PD");
                    XElement PDBElement = new XElement("PDB");


                    int directionRecordCount = 0;
                    int trackCount = 0;
                    int PCHUCount = 0;
                    int PDCount = 0;
                    int PDBCount = 0;

                    int totalCount = 0;
                    int constrictionCount = 0;
                    int broadeningCount = 0;
                    int levelCount = 0;
                    int skewnessCount = 0;
                    int drawdownCount = 0;
                    int straighteningCount = 0;


                    //notes = notes.Where(note => note.Name.Contains("прж")).ToList();
                    foreach (var note in notes)
                    {
                        if (previousDirectionName.Equals(string.Empty))
                            previousDirectionName = note.Direction;

                        if (previousTrackName.Equals(string.Empty))
                            previousTrackName = note.Track;

                        if (previousPCHUName.Equals(string.Empty))
                            previousPCHUName = note.Pchu;

                        if (previousPDName.Equals(string.Empty))
                            previousPDName = note.PD;

                        if (previousPDBName.Equals(string.Empty))
                            previousPDBName = note.PDB;

                        if (!previousDirectionName.Equals(note.Direction))
                        {
                            directionElement.Add(new XAttribute("recordCount", directionRecordCount));
                            directionElement.Add(new XAttribute("name", previousDirectionName));

                            tracklElement.Add(new XAttribute("name", previousTrackName));
                            tracklElement.Add(new XAttribute("recordCount", trackCount));

                            PCHUElement.Add(new XAttribute("number", previousPCHUName));
                            PCHUElement.Add(new XAttribute("recordCount", PCHUCount));

                            PDElement.Add(new XAttribute("number", previousPDName));
                            PDElement.Add(new XAttribute("recordCount", PDCount));

                            PDBElement.Add(new XAttribute("number", previousPDBName));
                            PDBElement.Add(new XAttribute("recordCount", PDBCount));

                            PDElement.Add(PDBElement);
                            PCHUElement.Add(PDElement);
                            tracklElement.Add(PCHUElement);
                            directionElement.Add(tracklElement);
                            tripElem.Add(directionElement);

                            directionRecordCount = 0;
                            trackCount = 0;
                            PCHUCount = 0;
                            PDCount = 0;
                            PDBCount = 0;

                            previousTrackName = string.Empty;
                            previousPCHUName = string.Empty;
                            previousPDName = string.Empty;
                            previousPDBName = string.Empty;

                            directionElement = new XElement("direction");
                            tracklElement = new XElement("track");
                            PCHUElement = new XElement("PCHU");
                            PDElement = new XElement("PD");
                            PDBElement = new XElement("PDB");

                            previousDirectionName = note.Direction;
                        }

                        if (!previousTrackName.Equals(note.Track) && !previousTrackName.Equals(string.Empty))
                        {
                            tracklElement.Add(new XAttribute("name", previousTrackName));
                            tracklElement.Add(new XAttribute("recordCount", trackCount));

                            PCHUElement.Add(new XAttribute("number", previousPCHUName));
                            PCHUElement.Add(new XAttribute("recordCount", PCHUCount));

                            PDElement.Add(new XAttribute("number", previousPDName));
                            PDElement.Add(new XAttribute("recordCount", PDCount));

                            PDBElement.Add(new XAttribute("number", previousPDBName));
                            PDBElement.Add(new XAttribute("recordCount", PDBCount));

                            PDElement.Add(PDBElement);
                            PCHUElement.Add(PDElement);
                            tracklElement.Add(PCHUElement);
                            directionElement.Add(tracklElement);

                            trackCount = 0;
                            PCHUCount = 0;
                            PDCount = 0;
                            PDBCount = 0;

                            previousPCHUName = string.Empty;
                            previousPDName = string.Empty;
                            previousPDBName = string.Empty;

                            tracklElement = new XElement("track");
                            PCHUElement = new XElement("PCHU");
                            PDElement = new XElement("PD");
                            PDBElement = new XElement("PDB");

                            previousTrackName = note.Track;
                            
                        }

                        if (!previousPCHUName.Equals(note.PCHU) && !previousPCHUName.Equals(string.Empty))
                        {
                            PCHUElement.Add(new XAttribute("number", previousPCHUName));
                            PCHUElement.Add(new XAttribute("recordCount", PCHUCount));

                            PDElement.Add(new XAttribute("number", previousPDName));
                            PDElement.Add(new XAttribute("recordCount", PDCount));

                            PDBElement.Add(new XAttribute("number", previousPDBName));
                            PDBElement.Add(new XAttribute("recordCount", PDBCount));

                            PDElement.Add(PDBElement);
                            PCHUElement.Add(PDElement);
                            tracklElement.Add(PCHUElement);

                            PCHUCount = 0;
                            PDCount = 0;
                            PDBCount = 0;

                            previousPDName = string.Empty;
                            previousPDBName = string.Empty;

                            PCHUElement = new XElement("PCHU");
                            PDElement = new XElement("PD");
                            PDBElement = new XElement("PDB");

                            previousPCHUName = note.PCHU;
                        }

                        if (!previousPDName.Equals(note.PD) && !previousPDName.Equals(string.Empty))
                        {
                            PDElement.Add(new XAttribute("number", previousPDName));
                            PDElement.Add(new XAttribute("recordCount", PDCount));

                            PDBElement.Add(new XAttribute("number", previousPDBName));
                            PDBElement.Add(new XAttribute("recordCount", PDBCount));

                            PDElement.Add(PDBElement);
                            PCHUElement.Add(PDElement);

                            PDCount = 0;
                            PDBCount = 0;
                            

                            previousPDBName = string.Empty;

                            PDBElement = new XElement("PDB");
                            PDElement = new XElement("PD");

                            previousPDName = note.PD;
                        }

                        if (!previousPDBName.Equals(note.PDB) && !previousPDBName.Equals(string.Empty))
                        {
                            PDBElement.Add(new XAttribute("number", previousPDBName));
                            PDBElement.Add(new XAttribute("recordCount", PDBCount));
                            PDElement.Add(PDBElement);

                            PDBCount = 0;

                            PDBElement = new XElement("PDB");

                            previousPDBName = note.PDB;
                        }
                        switch (note.Name)
                        {
                            case "Пр.п":
                            case "Пр.л":
                                drawdownCount += note.Count;
                                break;
                            case "Суж":
                                note.Count = note.Length / 4 + (note.Length % 4 + 1 > 1 ? 1 : 0);
                                constrictionCount += note.Count;

                                break;
                            case "Уш":
                                note.Count = note.Length / 4 + (note.Length % 4 + 1 > 1 ? 1 : 0);
                                broadeningCount += note.Count;
                                break;
                            case "У":
                                note.Count = note.Length / 10 + (note.Length % 10 + 1 > 1 ? 1 : 0);
                                levelCount += note.Count;
                                break;
                            case "П":
                                skewnessCount += note.Count;
                                break;
                            case "Р":
                                straighteningCount += note.Count;
                                break;
                        }

                        PDBElement.Add(new XElement("NOTE",
                            new XAttribute("founddate", note.FoundDate.ToString("dd.MM.yyyy")),
                            new XAttribute("km", note.Km),
                            new XAttribute("meter", note.Meter),
                            new XAttribute("digression", note.Name),
                            new XAttribute("value", note.Value),
                            new XAttribute("length", note.Length),
                            new XAttribute("count", note.Count),
                            new XAttribute("typ", note.Typ),
                            new XAttribute("setSpeed", note.FullSpeed.Replace("0/0/0", "-/-/-")),
                            new XAttribute("limitedSpeed", note.AllowSpeed.Replace("-1", "-")),
                            new XAttribute("primech", "")
                            ));

                        directionRecordCount += 1;
                        trackCount += 1;
                        PCHUCount += 1;
                        PDCount += 1;
                        PDBCount += 1;
                        

                        totalCount += 1;

                    }
                    directionElement.Add(new XAttribute("name", previousDirectionName));
                    directionElement.Add(new XAttribute("recordCount", directionRecordCount));

                    tracklElement.Add(new XAttribute("name", previousTrackName));
                    tracklElement.Add(new XAttribute("recordCount", trackCount));

                    PCHUElement.Add(new XAttribute("number", previousPCHUName));
                    PCHUElement.Add(new XAttribute("recordCount", PCHUCount));

                    PDElement.Add(new XAttribute("number", previousPDName));
                    PDElement.Add(new XAttribute("recordCount", PDCount));

                    PDBElement.Add(new XAttribute("number", previousPDBName));
                    PDBElement.Add(new XAttribute("recordCount", PDBCount));

                    PDElement.Add(PDBElement);
                    PCHUElement.Add(PDElement);
                    tracklElement.Add(PCHUElement);
                    directionElement.Add(tracklElement);
                    tripElem.Add(directionElement);

                    
                    tripElem.Add(new XAttribute("drawdownCount", drawdownCount));
                    tripElem.Add(new XAttribute("constrictionCount", constrictionCount));
                    tripElem.Add(new XAttribute("broadeningCount", broadeningCount));
                    tripElem.Add(new XAttribute("levelCount", levelCount));
                    tripElem.Add(new XAttribute("skewnessCount", skewnessCount));
                    tripElem.Add(new XAttribute("straighteningCount", straighteningCount));
                    tripElem.Add(new XAttribute("totalCount", drawdownCount + constrictionCount + broadeningCount + levelCount + skewnessCount + straighteningCount));
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