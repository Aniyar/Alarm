using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
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
    public class GraphDeviationsGaps : Report
    {

        private double CalcCoefY(double a, double b)
        {
            double c = Math.Max(a, b) - Math.Max(a, b) % 15 + 15;
            return 670 / c;
        }
        private int ValueY(double a, double b)
        {
            double c = Math.Max(a, b) - Math.Max(a, b) % 15 + 15;
            return Convert.ToInt32(c);
        }
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(distanceId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel) return;
                admTracksId = choiceForm.admTracksIDs;
            }
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);
                var mainProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (mainProcesses.Count() == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                foreach (var process in mainProcesses)
                {
                    List<Gap> check_gap_state = AdditionalParametersService.Check_gap_state(process.Trip_id, template.ID);

                    if (!check_gap_state.Any())
                        continue;

                    foreach (var kms in check_gap_state.GroupBy(g => g.Km).Select(g => g.Key))
                    {
                        foreach (var track_id in admTracksId)
                        {
                            XElement xePages = new XElement("pages");
                            var trackName = AdmStructureService.GetTrackName(track_id);
                            var leftGaps = check_gap_state.Where(g => g.Km == kms && g.Zazor != -999).ToList();
                            var rightGaps = check_gap_state.Where(g => g.Km == kms && g.R_zazor != -999).ToList();
                            int summMeasure = 0, tempSummMeasure = 0, order = 1, tempRealMeter = 0;
                            double summNominal = 0, tempSummNominal = 0;
                            string gapsline = "", nominalgapsline = "";

                            double coefX = 940.0 / Math.Max(leftGaps.Count, rightGaps.Count);

                            xePages.Add(
                                   new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),

                                new XAttribute("direction", process.DirectionName + " (" + process.DirectionCode + ")"),
                                new XAttribute("track", trackName),
                                   new XAttribute("period", period.Period),
                                new XAttribute("road", road),
                                new XAttribute("DKI", process.Car),
                                new XAttribute("km", kms),
                                new XAttribute("distance", distance.Code),
                                new XAttribute("tripdate", process.Trip_date));

                            foreach (var gap in leftGaps)
                            {
                                if (tempRealMeter == 0)
                                {
                                    tempRealMeter = gap.Meter;
                                    tempSummMeasure += gap.Zazor;
                                    tempSummNominal += gap.Nominal;
                                }
                                else
                                {
                                    if (Math.Abs(gap.Meter - tempRealMeter) > 100)
                                    {
                                        tempRealMeter = gap.Meter;
                                        summMeasure = Math.Max(summMeasure, tempSummMeasure);
                                        summNominal = Math.Max(summNominal, tempSummNominal);
                                    }
                                    else
                                    {
                                        tempRealMeter = gap.Meter;
                                        tempSummMeasure += gap.Zazor;
                                        tempSummNominal += gap.Nominal;
                                    }
                                }
                            }

                            summMeasure = Math.Max(summMeasure, tempSummMeasure);
                            summNominal = Math.Max(summNominal, tempSummNominal);
                            int valueY = ValueY(summMeasure, summNominal);

                            for (int i = 0; i < 15; i++)
                            {
                                xePages.Add(new XElement("texts_left",
                                    new XAttribute("x", 40),
                                    new XAttribute("y", 670 - i * 40),
                                    new XAttribute("text", valueY / 15 * (i + 1))));
                            }

                            double coefY = CalcCoefY(summMeasure, summNominal);
                            summMeasure = 0;
                            summNominal = 0;
                            tempRealMeter = 0;


                            foreach (var gap in leftGaps)
                            {
                                if (tempRealMeter == 0)
                                {
                                    summMeasure += gap.Zazor;
                                    summNominal += gap.Nominal;
                                    tempRealMeter = gap.Meter;
                                }
                                else
                                {
                                    if (Math.Abs(gap.Meter - tempRealMeter) > 100)
                                    {
                                        tempRealMeter = gap.Meter;
                                        summMeasure = gap.Zazor;
                                        summNominal = gap.Nominal;
                                        xePages.Add(new XElement("gapsline_left",
                                            new XAttribute("gapsline", gapsline),
                                            new XAttribute("nominalgapsline", nominalgapsline)));
                                        gapsline = "";
                                        nominalgapsline = "";
                                        order = 1;
                                    }
                                    else
                                    {
                                        tempRealMeter = gap.Meter;
                                        summMeasure += gap.Zazor;
                                        summNominal += gap.Nominal;
                                    }
                                }

                                double coordsY = 670 - summMeasure * coefY;
                                double coordsX = 60 + (leftGaps.IndexOf(gap) + 1) * coefX - coefX / 2;

                                gapsline += coordsX.ToString().Replace(',', '.') + "," + coordsY.ToString().Replace(',', '.') + " ";

                                xePages.Add(
                                    new XElement("points",
                                        new XAttribute("x", coordsX),
                                        new XAttribute("y", coordsY)),

                                    //new XElement("lines_left",
                                    //    new XAttribute("x", coordsX  + coefX / 2)),

                                    new XElement("texts_bot",
                                        new XAttribute("x", coordsX),
                                        new XAttribute("y", 682),
                                        new XAttribute("text", order))
                                    );

                                //if (order == 1)
                                //{
                                //   xePages.Add(new XElement("texts_left",
                                //       new XAttribute("x", coordsX - 5),
                                //       new XAttribute("y", 683),
                                //       new XAttribute("text", gap.Meter)));
                                //}

                                coordsY = 670 - summNominal * coefY;

                                nominalgapsline += coordsX.ToString().Replace(',', '.') + "," + coordsY.ToString().Replace(',', '.') + " ";
                                order++;
                            }

                            xePages.Add(new XElement("gapsline_left",
                                new XAttribute("gapsline", gapsline),
                                new XAttribute("nominalgapsline", nominalgapsline)));

                            order = 1;
                            summMeasure = 0;
                            summNominal = 0;
                            gapsline = "";
                            nominalgapsline = "";
                            tempRealMeter = 0;
                            tempSummMeasure = 0;
                            tempSummNominal = 0;

                            foreach (var gap in rightGaps)
                            {
                                if (gap.R_zazor == -999) continue;

                                if (tempRealMeter == 0)
                                {
                                    tempRealMeter = gap.Meter;
                                    tempSummMeasure += gap.R_zazor;
                                    tempSummNominal += gap.Nominal;
                                }
                                else
                                {
                                    if (Math.Abs(gap.Meter - tempRealMeter) > 100)
                                    {
                                        tempRealMeter = gap.Meter;
                                        summMeasure = Math.Max(summMeasure, tempSummMeasure);
                                        summNominal = Math.Max(summNominal, tempSummNominal);
                                    }
                                    else
                                    {
                                        tempRealMeter = gap.Meter;
                                        tempSummMeasure += gap.R_zazor;
                                        tempSummNominal += gap.Nominal;
                                    }
                                }
                            }

                            summMeasure = Math.Max(summMeasure, tempSummMeasure);
                            summNominal = Math.Max(summNominal, tempSummNominal);
                            valueY = ValueY(summMeasure, summNominal);

                            for (int i = 0; i < 43; i++)
                            {
                                xePages.Add(new XElement("texts_right",
                                    new XAttribute("x", 40),
                                    new XAttribute("y", 632 - i * 40),
                                    new XAttribute("text", valueY / 15 * (i + 1))));
                            }

                            coefY = CalcCoefY(summMeasure, summNominal);
                            summMeasure = 0;
                            summNominal = 0;

                            foreach (var gap in rightGaps)
                            {
                                if (gap.R_zazor == -999) continue;

                                if (tempRealMeter == 0)
                                {
                                    summMeasure += gap.R_zazor;
                                    summNominal += gap.Nominal;
                                    tempRealMeter = gap.Meter;
                                }
                                else
                                {
                                    if (Math.Abs(gap.Meter - tempRealMeter) > 100)
                                    {
                                        tempRealMeter = gap.Meter;
                                        summMeasure = gap.R_zazor;
                                        summNominal = gap.Nominal;
                                        xePages.Add(new XElement("gapsline_right",
                                            new XAttribute("gapsline", gapsline),
                                            new XAttribute("nominalgapsline", nominalgapsline)));
                                        gapsline = "";
                                        nominalgapsline = "";
                                        order = 1;
                                    }
                                    else
                                    {
                                        tempRealMeter = gap.Meter;
                                        summMeasure += gap.R_zazor;
                                        summNominal += gap.Nominal;
                                    }
                                }

                                double coordsY = 600 - summMeasure * coefY;
                                double coordsX = (rightGaps.IndexOf(gap) + 1) * coefX - coefX / 2;

                                gapsline += coordsX.ToString().Replace(',', '.') + "," + coordsY.ToString().Replace(',', '.') + " ";

                                xePages.Add(new XElement("gaps_right",
                                    new XAttribute("x", coordsX + 60),
                                    new XAttribute("y", coordsY + 70 - 8.5 / 2)),
                                    new XElement("lines_right",
                                    new XAttribute("x", coordsX + 60 + coefX / 2)),
                                    new XElement("texts_right",
                                    new XAttribute("x", coordsX + 60 - 5),
                                    new XAttribute("y", 676),
                                    new XAttribute("text", order)));

                                if (order == 1)
                                {
                                    xePages.Add(new XElement("texts_right",
                                        new XAttribute("x", coordsX + 60 - 5),
                                        new XAttribute("y", 683),
                                        new XAttribute("text", gap.Meter)));
                                }

                                coordsY = 600 - summNominal * coefY;

                                nominalgapsline += coordsX.ToString().Replace(',', '.') + "," + coordsY.ToString().Replace(',', '.') + " ";
                                order++;
                            }

                            xePages.Add(new XElement("gapsline_right",
                                new XAttribute("gapsline", gapsline),
                                new XAttribute("nominalgapsline", nominalgapsline)));

                            report.Add(xePages);
                        }
                    }
                }

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_GraphDeviationsGaps.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_GraphDeviationsGaps.html");
            }
        }
    }
}
