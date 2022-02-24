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
    public class AverageScoreDepartments : Report
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
                    var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;
                    CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;

                    var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                    if (kms.Count() == 0) continue;


                    XElement tripElem = new XElement("trip",
                        new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                        new XAttribute("direction", curvesAdmUnit.Direction),
                        new XAttribute("track", curvesAdmUnit.Track),
                        new XAttribute("km", kms.Min() + "-" + kms.Max()),
                        new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                        new XAttribute("road", roadName),
                        new XAttribute("distance", distance.Code),
                        new XAttribute("periodDate", period.Period),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("ps", tripProcess.Car),
                        new XAttribute("trip_date", tripProcess.Trip_date)
                    );

                    List<Digression> notes = RdStructureService.AverageScoreDepartments(tripProcess.Trip_id, distance.Code, new int[] { 3, 4 });



                    //notes = notes.Where(o => o.Km >= 129 && o.Km <= 147).ToList();

                    //Участки дист коррекция
                    var dist_section = MainTrackStructureService.GetDistSectionByDistId(distance.Id);
                    foreach (var item in notes)
                    {
                        var ds = dist_section.ToList();
                       // var ds = dist_section.Where(
                       //o => item.Km * 1000 + item.Meter >= o.Start_Km * 1000 + o.Start_M && item.Km * 1000 + item.Meter <= o.Final_Km * 1000 + o.Final_M).ToList();

                        item.PCHU = ds.First().Pchu.ToString();
                        //item.PD = ds.First().Pd.ToString();
                        //item.PDB = ds.First().Pdb.ToString();
                    }

                    string previousPDName = string.Empty;
                    string previousPDBName = string.Empty;
                    string previousPCHUName = string.Empty;
                    XElement PDElement = new XElement("PD");
                    XElement PDBElement = new XElement("PDB");
                    XElement PCHUElement = new XElement("PCHU");
                    int AllCount = 0;

                    int totalCount = 0;
                    int totalAvg = 0;

                    for (int i = 0; i < notes.Count; i++)
                    {
                        if (i == 0)
                        {
                            PDElement.Add(new XAttribute("number", notes[i].PD));

                            PDElement.Add(new XAttribute("pchu", notes[i].PCHU));
                        }

                        if(i < notes.Count-1)
                        {
                            if (notes[i].PD == notes[i + 1].PD)
                            {
                                PDBElement = new XElement("PDB");
                                PDBElement.Add(
                                    new XAttribute("number", notes[i].PDB),
                                    new XAttribute("pchu", notes[i].PCHU),
                                    new XAttribute("ball", AvgScore(notes[i].AvgBall))
                                    );
                                totalCount++;
                                AllCount++;
                                totalAvg = totalAvg + notes[i].AvgBall;
                                PDElement.Add(PDBElement);
                            }
                            else
                            {
                                PDBElement = new XElement("PDB");
                                PDBElement.Add(
                                    new XAttribute("pchu", notes[i].PCHU),
                                    new XAttribute("number", notes[i].PDB),
                                    new XAttribute("ball", AvgScore(notes[i].AvgBall))
                                    );
                                totalCount++;
                                AllCount++;
                                totalAvg = totalAvg + notes[i].AvgBall;

                                PDElement.Add(PDBElement);
                                PDElement.Add(new XAttribute("recordCount", totalCount));
                                PDElement.Add(new XAttribute("ball", AvgScore(totalAvg / totalCount)));
                                tripElem.Add(PDElement);
                                //создание нового пдб

                                PDElement = new XElement("PD");
                                PDElement.Add(new XAttribute("number", notes[i + 1].PD),
                                     new XAttribute("pchu", notes[i].PCHU));
                                totalCount = 0;
                                totalAvg = 0;
                            }
                        }


                        //последний элемент
                        if (i == notes.Count-1)
                        {
                            if (notes[i].PD == notes[i - 1].PD)
                            {
                                PDBElement = new XElement("PDB");
                                PDBElement.Add(
                                    new XAttribute("number", notes[i].PDB),
                                    new XAttribute("pchu", notes[i].PCHU),
                                new XAttribute("ball", AvgScore(notes[i].AvgBall))
                                    );
                                totalCount++;
                                AllCount++;
                                totalAvg = totalAvg + notes[i].AvgBall;

                                PDElement.Add(PDBElement);
                                PDElement.Add(new XAttribute("recordCount", totalCount));
                                PDElement.Add(new XAttribute("ball", AvgScore(totalAvg / totalCount)));
                                tripElem.Add(PDElement);
                            }
                            else
                            {
                                PDElement = new XElement("PD");
                                PDElement.Add(new XAttribute("number", notes[i].PD),
                                      new XAttribute("pchu", notes[i].PCHU));
                                totalCount = 0;
                                totalAvg = 0;

                                PDBElement = new XElement("PDB");
                                PDBElement.Add(
                                    new XAttribute("number", notes[i].PDB),
                                    new XAttribute("ball", AvgScore(notes[i].AvgBall))
                                    );
                                totalCount++;
                                AllCount++;
                                totalAvg = totalAvg + notes[i].AvgBall;

                                PDElement.Add(PDBElement);
                                PDElement.Add(new XAttribute("recordCount", totalCount));
                                PDElement.Add(new XAttribute("ball", AvgScore(totalAvg / totalCount)));
                                tripElem.Add(PDElement);
                            }
                        }
                    }
                    tripElem.Add(new XAttribute("AllCount", AllCount));
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

        private object AvgScore(int avgBall)
        {
            string avg = String.Empty;
            if (avgBall >= 0 || avgBall <= 25)
            {
                avg = avgBall.ToString() + "/О";
            }
            if (avgBall >= 26 || avgBall <= 80)
            {
                avg = avgBall.ToString() + "/Х";
            }
            if (avgBall >= 81 || avgBall <= 180)
            {
                avg = avgBall.ToString() + "/У";
            }
            if (avgBall > 180)
            {
                avg = avgBall.ToString() + "/Н";
            }
            return avg;
        }

        public override string ToString()
        {
            return "Отступления 2 степени, близкие к 3";
        }
    }
}