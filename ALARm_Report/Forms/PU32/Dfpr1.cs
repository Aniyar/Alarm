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
    public class Dfpr1 : Report
    {

        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(parentId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel)
                    return;
                admTracksId = choiceForm.admTracksIDs;
            }
            using (XmlWriter writer = htReport.CreateWriter())
            {
                List<Curve> curves = (MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);

                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)

                {
                    foreach (var trackId in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(trackId);

                        var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;
                        CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;
                        //var kilometers = AdditionalParametersService.GetKilometers(tripProcess.Id, (int)tripProcess.Direction);
                        var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                        progressBar.Maximum = kms.Count;

                        XElement tripElem = new XElement("trip",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("track", curvesAdmUnit.Track),
                            new XAttribute("road", road),
                            new XAttribute("km", kms.Min().ToString() + "-" + kms.Max().ToString()),
                            new XAttribute("date_statement", period.Period),
                            new XAttribute("trip_date", DateTime.Now.Date.ToShortDateString()),
                            new XAttribute("pch", distance.Code),
                            new XAttribute("type", tripProcess.GetProcessTypeName),
                            new XAttribute("napr", $"{tripProcess.DirectionName}({tripProcess.DirectionCode})"),
                            new XAttribute("nput", $"{tripProcess.TrackName}"),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("ps", $"{tripProcess.Car}")
                            );
                        XElement data = new XElement("data",
                            new XAttribute("napr", road)
                            );
                        XElement pch = new XElement("Pch",
                            new XAttribute("npch", distance.Code)
                            );
                        XElement put = new XElement("Put",
                            new XAttribute("nput", "1")
                            );
                        XElement prop = new XElement("Prop");
                        XElement xeTracks = new XElement("tracks",
                               new XAttribute("trackinfo", $"{tripProcess.DirectionName}({tripProcess.DirectionCode}) Путь: {trackName}"));

                       
                        foreach (var kilometer in kms)
                        {

                        
                            progressBar.Value = kms.IndexOf(kilometer) + 1;

                            //var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromText(kilometer);
                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(kilometer, tripProcess.Trip_id);
                            if (DBcrossRailProfile == null || DBcrossRailProfile.Count == 0) continue;

                            var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(DBcrossRailProfile);


                            var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer,
                                MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, "1") as List<Speed>;

                            int v1 = 0;
                            int v2 = 0;
                            int v3 = 0;
                            int v4 = 0;
                            int v5 = 0;
                            for (int i = 0; i < crossRailProfile.DownhillLeft.Count; i++)
                            {
                                //менее 1/60
                                if (1 / 60.0 > crossRailProfile.DownhillLeft[i])
                                {
                                    v1 += 1;
                                }
                                //от 1/60 до 1/30
                                else if (1 / 60.0 <= crossRailProfile.DownhillLeft[i] && crossRailProfile.DownhillLeft[i] < 1/ 30.0)
                                {
                                    v2 += 1;
                                }
                                //от 1/30 до 1/15
                                else if (1 / 30.0 <= crossRailProfile.DownhillLeft[i] && crossRailProfile.DownhillLeft[i] < 1/ 15.0)
                                {
                                    v3 += 1;
                                }
                                //от 1/15 до 1/12
                                else if (1 / 15.0 <= crossRailProfile.DownhillLeft[i] && crossRailProfile.DownhillLeft[i] < 1/ 12.0)
                                {
                                    v4 += 1;
                                }
                                //более 1/12
                                else if (1 / 12.0 < crossRailProfile.DownhillLeft[i])
                                {
                                    v5 += 1;
                                }

                                //менее 1/60
                                if (1 / 60.0 > crossRailProfile.DownhillRight[i])
                                {
                                    v1 += 1;
                                }
                                //от 1/60 до 1/30
                                else if (1 / 60.0 <= crossRailProfile.DownhillRight[i] && crossRailProfile.DownhillRight[i] < 1 / 30.0)
                                {
                                    v2 += 1;
                                }
                                //от 1/30 до 1/15
                                else if (1 / 30.0 <= crossRailProfile.DownhillRight[i] && crossRailProfile.DownhillRight[i] < 1 / 15.0)
                                {
                                    v3 += 1;
                                }
                                //от 1/15 до 1/12
                                else if (1 / 15.0 <= crossRailProfile.DownhillRight[i] && crossRailProfile.DownhillRight[i] < 1 / 12.0)
                                {
                                    v4 += 1;
                                }
                                //более 1/12
                                else if (1 / 12.0 < crossRailProfile.DownhillRight[i])
                                {
                                    v5 += 1;
                                }
                            }
                            var cn = crossRailProfile.DownhillLeft.Count*2;

                            prop = new XElement("Prop",
                                        new XAttribute("kM_a", kilometer),
                                        new XAttribute("v1", ((v1 * 100.0) / cn).ToString("0")),
                                        new XAttribute("v2", ((v2 * 100.0) / cn).ToString("0")),
                                        new XAttribute("v3", ((v3 * 100.0) / cn).ToString("0")),
                                        new XAttribute("v4", ((v4 * 100.0) / cn).ToString("0")),
                                        new XAttribute("v5", ((v5 * 100.0) / cn).ToString("0")),
                                        new XAttribute("v", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight : "-/-")
                                        );

                       
                            put.Add(prop);
                        }
                        pch.Add(put);
                        pch.Add(new XAttribute("trackinfo", $"{tripProcess.DirectionName}({tripProcess.DirectionCode}) Путь: {trackName}"));
                        data.Add(pch);

                        tripElem.Add(data);
                      
                        report.Add(tripElem);
                    }
                }

                progressBar.Value = 0;

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