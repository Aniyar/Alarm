using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using MetroFramework;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class DerogationsIsostsAndJointless : Report
    {
        private string engineer { get; set; } = "Komissia K";
        private string chief { get; set; } = "Komissia K";
        private DateTime from, to;
        private TripType tripType;
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
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
                    List<Digression> notes = RdStructureService.DerogationsIsostsAndJointless(tripProcess.Trip_id, distance.Code, new int[] { 3, 4 });
                    var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;

                    CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;
                    var kilometers = RdStructureService.GetPU32Kilometers(from, to, parentId, tripType); //.GetRange(65,15);
                    if (kilometers.Count == 0)
                        return;

                    //var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                    //if (kms.Count() == 0) continue;


                    XElement tripElem = new XElement("trip",
                        new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        new XAttribute("direction", curvesAdmUnit.Direction),
                        new XAttribute("km", kilometers.Min() + "-" + kilometers.Max()),
                        new XAttribute("track", curvesAdmUnit.Track),
                        new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                        new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                        new XAttribute("road", roadName),
                        new XAttribute("distance", distance.Code),
                        new XAttribute("trip_date", tripProcess.Trip_date),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("ps", tripProcess.Car)
                    );
                   


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
                    //////////////////////
                    ///
                    var byKilometer = new XElement("bykilometer",
                                    new XAttribute("code", kilometers[0].Direction_code),
                                    new XAttribute("track", kilometers[0].Track_name),
                                    new XAttribute("name", kilometers[0].Direction_name),
                                    new XAttribute("pch", distance.Code));
                    var sectionElement = new XElement("section",
                    new XAttribute("name", kilometers[0].Direction_name),
                    new XAttribute("code", kilometers[0].Direction_code),
                    new XAttribute("track", kilometers[0].Track_name)
                    );

                    var sectionTotal = new Total
                    {
                        Code = kilometers[0].Track_name,
                        DirectionCode = kilometers[0].Direction_code,
                        DirectionName = kilometers[0].Direction_name
                    };


                    var pchuElement = new XElement("pchu",
                        new XAttribute("pch", distance.Code),
                        new XAttribute("code", kilometers[0].PchuCode),
                        new XAttribute("chief", kilometers[0].PchuChief)
                        );
                    var pchuTotal = new Total();
                    pchuTotal.Code = kilometers[0].PchuCode;

                    var pdElement = new XElement("pd",
                    new XAttribute("code", kilometers[0].PdCode),
                    new XAttribute("chief", kilometers[0].PdChief)
                    );
                    var pdTotal = new Total();
                    pdTotal.Code = kilometers[0].PdCode;

                    var pdbElement = new XElement("pdb",
                        new XAttribute("code", kilometers[0].PdbCode),
                        new XAttribute("chief", kilometers[0].PdbChief));
                    var pdbTotal = new Total();
                    pdbTotal.Code = kilometers[0].PdbCode;

                    /////////////////////
                    foreach (var km in kilometers)
                    {
                        progressBar.Value += 1;

                        km.LoadTrackPasport(MainTrackStructureService.GetRepository(), km.TripDate);
                        if (!sectionTotal.Code.Equals(km.Track_name) || !sectionTotal.DirectionCode.Equals(km.Direction_code))
                        {
                            PDTotalGenerate(ref pdElement, ref pdTotal, ref pchuElement, ref pchuTotal, km.PdCode, km.PdChief);
                        }
                        if (!pchuTotal.Code.Equals(km.PchuCode))
                        {
                          
                            PDTotalGenerate(ref pdElement, ref pdTotal, ref pchuElement, ref pchuTotal, km.PdCode, km.PdChief);
                        }
                        if (!pdTotal.Code.Equals(km.PdCode))
                        {
                          
                            PDTotalGenerate(ref pdElement, ref pdTotal, ref pchuElement, ref pchuTotal, km.PdCode, km.PdChief);
                        }

                    }

                    foreach (var note in notes)
                    {
                        if (previousDirectionName.Equals(string.Empty))
                            previousDirectionName = note.Direction;

                        if (previousTrackName.Equals(string.Empty))
                            previousTrackName = note.Track;

                        if (previousPCHUName.Equals(string.Empty))
                            previousPCHUName = note.PCHU;

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

                        note.Norma = note.Name.Equals("Уш") ? "Ншк=" + note.Norma : String.Empty;
                      

                        PDBElement.Add(new XElement("NOTE", 
                            new XAttribute("founddate", note.FoundDate),
                            new XAttribute("km", note.Km),
                            new XAttribute("meter", note.Meter),
                            new XAttribute("digression", note.Name),
                            new XAttribute("value", note.Value),
                            new XAttribute("length", note.Length),
                            new XAttribute("count", note.Count),
                            new XAttribute("typ", note.Typ),
                            new XAttribute("fullSpeed", note.FullSpeed),
                            new XAttribute("allowSpeed", note.AllowSpeed.Replace("-1","-")),
                            new XAttribute("norma", note.Norma)
                            ));

                        directionRecordCount += 1;
                        trackCount += 1;
                        PCHUCount += 1;
                        PDCount += 1;
                        PDBCount += 1;
                        switch (note.Name)
                        {
                            case "Пр.п":
                            case "Пр.л":
                                drawdownCount += note.Count;
                                break;
                            case "Суж":
                                note.Count = note.Length / 4 + (note.Length % 4 > 1 ? 1 : 0);
                                constrictionCount += note.Count;

                                break;
                            case "Уш":
                                note.Count = note.Length / 4 + (note.Length % 4 > 1 ? 1 : 0);
                                broadeningCount += note.Count;
                                break;
                            case "У":
                                note.Count = note.Length / 10 + (note.Length % 10 > 1 ? 1 : 0);
                                levelCount += note.Count;
                                break;
                            case "П":
                                skewnessCount += note.Count;
                                break;
                            case "Р":
                                straighteningCount += note.Count;
                                break;
                        }

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
            void PDTotalGenerate(ref XElement pdElement, ref Total pdTotal, ref XElement pchuElement, ref Total pchuTotal, string code, string chief)
            {
                var avgBall1 = (pdTotal.MainParamPointSum + pdTotal.CurvePointSum) / pdTotal.Length;
                var avgBall2 = (pdTotal.MainParamPointSum + pdTotal.CurvePointSum + pdTotal.AddParamPointSum) / pdTotal.Length;

                pdElement.Add(
                    new XAttribute("len", pdTotal.Length % 1 == 0 ? pdTotal.Length.ToString("0", nfi) : pdTotal.Length.ToString("0.000", nfi)),
                    new XAttribute("point", $"{pdTotal.MainParamPointSum + pdTotal.CurvePointSum}/{pdTotal.AddParamPointSum}"),
                    new XAttribute("rating", pdTotal.GetSectorQualitiveRating()),
                    new XAttribute("ratecount", $"Отл - {pdTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {pdTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {pdTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {pdTotal.RatingCounts[3].ToString("0", nfi)}; Средний балл - {avgBall1:0}/{avgBall2:0}"),

                    new XAttribute("avgLine", $" {avgBall2:0}/{pdTotal.GetSectorQualitiveRating().Split('/')[1]}"),


                    new XAttribute("c1", pdTotal.Constriction.ToString()),
                    new XAttribute("c2", pdTotal.Broadening.ToString()),
                    new XAttribute("c3", pdTotal.Level.ToString()),
                    new XAttribute("c4", pdTotal.Sag.ToString()),
                    new XAttribute("c5", pdTotal.Drawdown.ToString()),
                    new XAttribute("c6", pdTotal.Strightening.ToString()),
                    new XAttribute("c7", pdTotal.Common),
                    new XAttribute("c8", pdTotal.FourthOtherAdd));

                pchuTotal += pdTotal;
                pdTotal = new Total();
                pdTotal.Code = code;

                pchuElement.Add(pdElement);

                pdElement = new XElement("pd",
                                new XAttribute("code", code),
                                new XAttribute("chief", chief));
            }
        }

        public override string ToString()
        {
            return "Отступления 2 степени, близкие к 3";
        }

    }

}