using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
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

namespace ALARm_Report.Forms
{
    public class ChartShortRoughness : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(distanceId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel)
                    return;
                admTracksId = choiceForm.admTracksIDs;
            }

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

                foreach (var process in mainProcesses)
                {
                    //var impulse = RdStructureService.GetRdTables(process, 2) as List<RdStatisticRoughnessImpulse>;
                    //var surface = RdStructureService.GetRdTables(process, 3) as List<RdIntegralSurfaceRails>;
                    var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                    //var road = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                    var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);
                    var trip = RdStructureService.GetTrip(process.Trip_id);
                    List<Kilometer> kilometers = RdStructureService.GetKilometersByTrip(trip);

                    kilometers = kilometers.ToList();

                    kilometers = kilometers.OrderBy(o => o.Number).ToList();

                    //if (impulse.Count < 1 || surface.Count < 1)
                    //{
                    //    continue;
                    //}

                    progressBar.Maximum = kilometers.Count;

                    foreach (var trackId in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(trackId);

                        //if (surface.Where(s => s.Track_id == trackId).Count() < 1)
                        //    continue;

                        XElement xePages = new XElement("pages");

                        //var site = RdStructureService.GetSiteInfo(trackId, 
                        //                                          impulse.Where(i => i.Track_id == trackId).Min(i => i.Km), 
                        //                                          impulse.Where(i => i.Track_id == trackId).Max(i => i.Km)
                        //                                          );

                        xePages.Add(
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("distance", ((AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId)).Code),
                            new XAttribute("Dname", $"{process.DirectionName} Путь:{trackName} "),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("DKI", process.Car),
                            new XAttribute("chief", process.Chief),
                            new XAttribute("road", road),
                            new XAttribute("site", "site.StationStart" + " - " + "site.StationFinal"),
                            new XAttribute("track", trackName),
                            new XAttribute("km", kilometers.Min(o => o.Number).ToString() + " - " + kilometers.Max(o => o.Number).ToString()),
                            new XAttribute("check", process.GetProcessTypeName),
                            new XAttribute("direction", process.DirectionName)
                            );

                        double polyX = 970.0, 
                               polyY = 600.0;

                        int allKm =   kilometers.Count()-1,
                            lastKm =  kilometers.Min(o => o.Number),
                            firstKm = kilometers.Min(o => o.Number);

                        double kmCoef = polyX / allKm;
                        double yCoef = polyY / 3;
                        string integral = "", maxdeeprough = "";

                        foreach (var kms in kilometers)
                        {
                            progressBar.Value = kilometers.IndexOf(kms) + 1;

                            //if ((kms.Number - lastKm) * kmCoef >= 150.0)
                            {
                                XElement xeLines = new XElement("lines",
                                    new XAttribute("x1", (kms.Number - firstKm) * kmCoef+30),
                                    new XAttribute("x2", (kms.Number - firstKm) * kmCoef+30),
                                    new XAttribute("y1", "45"),
                                    new XAttribute("y2", "650"));

                                XElement xeTexts = new XElement("texts",
                                    new XAttribute("x", (kms.Number - firstKm) * kmCoef+30),
                                    new XAttribute("y", 0),
                                    new XAttribute("text", kms.Number.ToString()));

                                xePages.Add(xeTexts);
                                xePages.Add(xeLines);

                                lastKm = kms.Number;
                            }

                            double km = (kms.Number - firstKm) * kmCoef;

                            //Интегральные показатели состояния поверхности катания рельсов (Ф.ДП1)
                            List<Speed> settedSpeeds = MainTrackStructureService.GetMtoObjectsByCoord(process.Date_Vrem,
                                                                                                      kms.Number, 
                                                                                                      MainTrackStructureConst.MtoSpeed,
                                                                                                      process.DirectionName,
                                                                                                      trackName.ToString()
                                                                                                      ) as List<Speed>; //toDo trackNumber
                            
                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(kms.Number, process.Trip_id);
                            if (DBcrossRailProfile == null || DBcrossRailProfile.Count() == 0) continue;

                            var shortRoughness = AdditionalParametersService.GetShortRoughnessFromDBParse(DBcrossRailProfile);



                            var indicators = shortRoughness.GetIntegratedIndicators(settedSpeeds.Count > 0 ? settedSpeeds[0].Passenger : 140);
                            var leftInd = Math.Round((indicators[0][6] * indicators[0][7]) / 100, 2);
                            var rightInd = Math.Round((indicators[1][6] * indicators[1][7]) / 100, 2);

                            int rshp = shortRoughness.GetRshp(settedSpeeds.Count > 0 ? settedSpeeds[0].Passenger : 140, 0.1f);

                            //if (!((rightInd >= (float)(float)filters[0].Value) || (leftInd >= (float)(float)filters[0].Value)))
                            //    continue;

                            var imp = Math.Max(Math.Round((indicators[0][6] * indicators[0][7]) / 1000, 2), Math.Round((indicators[1][6] * indicators[1][7]) / 1000, 2)) * yCoef;
                            //double valueImpulse = (impulse.Where(i => i.Track_id == trackId && i.Km == kms.Number).Max(i => Math.Max(i.Right_Value, i.Left_Value)) - 3) * (-1) * yCoef;
                            double valueSurface = 600 - imp;

                            maxdeeprough += km.ToString("f2", System.Globalization.CultureInfo.InvariantCulture) + "," + valueSurface.ToString("f2", System.Globalization.CultureInfo.InvariantCulture) + " ";
                            integral += km.ToString("f2", System.Globalization.CultureInfo.InvariantCulture) + "," + (600 - (1 + Math.Sin(km)* Math.Sin(km)) * imp).ToString().Replace(",", ".") + " ";
                        }

                        progressBar.Value = 0;

                        xePages.Add(new XAttribute("integral", integral),
                            new XAttribute("maxdeeprough", maxdeeprough));

                        report.Add(xePages);
                    }
                }

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_ChartShortRoughness.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_ChartShortRoughness.html");
            }
        }
    }
}
