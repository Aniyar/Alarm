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
using ALARm_Report.controls;
using System.Reflection;

namespace ALARm_Report.Forms
{
    // убрать 22  IntegartedIndicators22 
    public class IntegartedIndicators22 : Report
    {
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod period,
            MetroProgressBar progressBar)
        {
            var filterForm = new FilterForm();
            var filters = new List<Filter>();
            var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
            filters.Add(new FloatFilter() { Name = "Величина индекса", Value = 0.0f });
            filters.Add(new FloatFilter() { Name = "Величина съёма металла, мм", Value = 0.10f });
            filterForm.SetDataSource(filters);
            if (filterForm.ShowDialog() == DialogResult.Cancel)
                return;

            var npch = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId);
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {

                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                //var tripProcesses = RdStructureService.GetAdditionalParametersProcess(period);

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);

                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                string prevDirection = "";
                foreach (var tripProcess in tripProcesses)
                {


                    var tripElement = new XElement("trip",
                          new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        new XAttribute("way", roadName),
                        new XAttribute("npch", "ПЧ:" + (npch != null ? (npch as AdmUnit).Code : "-")),
                         new XAttribute("Period", period.Period + " "),
                        new XAttribute("addinfo", tripProcess.GetProcessTypeName),
                        new XAttribute("car", tripProcess.Car),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("filter1", ((float)(float)filters[0].Value).ToString("0.00")),
                        new XAttribute("filter2", "съём металла за проход = " + ((float)(float)filters[1].Value).ToString("0.00") + " мм")

                        );

                    var trip = RdStructureService.GetTrip(tripProcess.Trip_id);

                    var kilometers = RdStructureService.GetKilometersByTrip(trip);
                    kilometers = kilometers.OrderBy(o => o.Number).ToList();

                    //--------------------------------------------
                    XElement nhaprElement = null;
                    progressBar.Maximum = kilometers.Count;
                    List<Curve> curves = RdStructureService.GetCurvesInTrip(trip.Id) as List<Curve>;

                    //var filter_curves = curves.Where(o => ((float)(float)filters[0].Value <= o.Start_Km && o.Final_Km <= (float)(float)filters[1].Value)).ToList();
                    foreach (var curve in curves)
                    {

                        curve.Elevations = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();
                        curve.Straightenings = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();
                        List<RDCurve> rdcs = RdStructureService.GetRDCurves(curve.Id, trip.Id);



                        XElement xeCurve = new XElement("curve",
                            new XAttribute("priznak", curve.Side[0] == 'Л' ? 'П' : 'Л'));

                        tripElement.Add(xeCurve);

                    }

                    //foreach (var curve in curves)
                    //{
                    //    if (curve.Start_Km * 1000 + curve.Start_M > km * 1000 + m + 1)
                    //    {

                    //        nhaprElement.Add(new XElement("datarow",

                    //            new XAttribute("characteristics", " ")

                    //            );
                    //    }

                    //    nhaprElement.Add(new XElement("datarow",

                    //        new XAttribute("characteristics", "Кривая R = )

                    //        );


                    //}
                    foreach (var kilometer in kilometers)
                    {

                        //if (kilometer.Number != 146) continue;

                        if (!prevDirection.Equals(kilometer.Direction_name + " Путь : " + kilometer.Track_name))
                        {
                            if (nhaprElement != null)
                            {
                                tripElement.Add(nhaprElement);
                            }

                            prevDirection = kilometer.Direction_name + " Путь : " + kilometer.Track_name;
                            nhaprElement = new XElement("nhapr", new XAttribute("name", prevDirection));
                        }
                        List<Speed> settedSpeeds = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem,
                                                                                                  kilometer.Number, MainTrackStructureConst.MtoSpeed,
                                                                                                  tripProcess.DirectionName,
                                                                                                  "1"
                                                                                                  ) as List<Speed>; //toDo trackNumber
                        progressBar.Value = kilometers.IndexOf(kilometer) + 1;

                        //var shortRoughness = AdditionalParametersService.GetShortRoughnessFromText(kilometer.Number);

                        var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(kilometer.Number, tripProcess.Trip_id);
                        if (DBcrossRailProfile == null || DBcrossRailProfile.Count() == 0) continue;

                        var shortRoughness = AdditionalParametersService.GetShortRoughnessFromDBParse(DBcrossRailProfile);



                        var indicators = shortRoughness.GetIntegratedIndicators(settedSpeeds.Count > 0 ? settedSpeeds[0].Passenger : 140);
                        var leftInd = Math.Round((indicators[0][6] * indicators[0][7]) / 100, 2);
                        var rightInd = Math.Round((indicators[1][6] * indicators[1][7]) / 100, 2);

                        int rshp = shortRoughness.GetRshp(settedSpeeds.Count > 0 ? settedSpeeds[0].Passenger : 140, (float)(float)filters[1].Value);

                        if (!((rightInd >= (float)(float)filters[0].Value) || (leftInd >= (float)(float)filters[0].Value)))
                            continue;
                        var side = curves.Select(o => o.Side[0]).ToList().ToString();
                        nhaprElement.Add(new XElement("datarow",
                            new XElement("begin", kilometer.Number + "." + Math.Min(shortRoughness.MetersLeft.Min(), shortRoughness.MetersRight.Min())),
                            new XElement("end", kilometer.Number + "." + Math.Max(shortRoughness.MetersLeft.Max(), shortRoughness.MetersRight.Max())),

                            //new XElement("priznak", ),

                            new XElement("lnitl", Math.Round(indicators[0][4], 2) + "/" + Math.Round(indicators[0][5], 2)),
                            new XElement("lnitm", Math.Round(indicators[0][2], 2) + "/" + Math.Round(indicators[0][3], 2)),
                            new XElement("lnits", Math.Round(indicators[0][0], 2) + "/" + Math.Round(indicators[0][1], 2)),
                            new XElement("lstate", Math.Round((indicators[0][6] * indicators[0][7]) / 1000, 2)),

                            new XElement("rnitl", Math.Round(indicators[1][4], 2) + "/" + Math.Round(indicators[1][5], 2)),
                            new XElement("rnitm", Math.Round(indicators[1][2], 2) + "/" + Math.Round(indicators[1][3], 2)),
                            new XElement("rnits", Math.Round(indicators[1][0], 2) + "/" + Math.Round(indicators[1][1], 2)),
                            new XElement("rstate", Math.Round((indicators[1][6] * indicators[1][7]) / 1000, 2)),

                            new XElement("rshp_count", rshp)
                        ));

                    }
                    if (nhaprElement != null)
                    {
                        tripElement.Add(nhaprElement);
                    }
                    report.Add(tripElement);
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