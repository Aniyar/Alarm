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
    public class Dfi1 : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                var filterForm = new FilterForm();
                var filters = new List<Filter>();
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                filters.Add(new FloatFilter() { Name = "Порог. знач. бокового износа (мм)", Value = 13.0f });

                filterForm.SetDataSource(filters);
                if (filterForm.ShowDialog() == DialogResult.Cancel)
                    return;

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    var directName = AdditionalParametersService.DirectName(tripProcess.Trip_id, (int)distance.Id);
                    

                    float max = 17.5f, sred = 12.6f;

                    XElement lev = new XElement("lev");
                    List<Curve> curves = MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>;

                    var filterForm1 = new FilterForm();
                    var filters1 = new List<Filter>();

                    var min1 = curves.Select(o => o.Start_Km).Min();
                    var max1 = curves.Select(o => o.Final_Km).Max();

                    filters1.Add(new FloatFilter() { Name = "Начало (км)", Value = min1 });
                    filters1.Add(new FloatFilter() { Name = "Конец (км)", Value = max1 });

                    filterForm1.SetDataSource(filters1);
                    if (filterForm1.ShowDialog() == DialogResult.Cancel)
                        return;

                    //фильтр по выбранным км
                    curves = curves.Where(o => ((float)filters1[0].Value <= o.Start_Km && o.Final_Km <= (float)filters1[1].Value)).ToList();

                    //var kilometers = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);

                    XElement tripElem = new XElement("trip",
                                new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                                new XAttribute("road", road),
                                new XAttribute("napr", directName.Count == 0 ? "" : directName[0].Name),
                                new XAttribute("ps", tripProcess.Car),
                                new XAttribute("date_statement", tripProcess.GetProcessTypeName),
                                new XAttribute("pch", directName.Count == 0 ? -1 : directName[0].Pch),
                                new XAttribute("put", directName.Count == 0 ? "-1" : directName[0].Put),
                                new XAttribute("chief", tripProcess.Chief),
                                new XAttribute("period", period.Period)
                                // new XAttribute("km", "")
                                );

                    foreach (var elem in curves)
                    {

                        elem.Straightenings = (MainTrackStructureService.GetCurves(elem.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();

                        //elem.Radiuses =
                        //(MainTrackStructureService.GetCurves(elem.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(radius => radius.Start_Km * 1000 + radius.Start_M).ToList();

                        //foreach (var kilometer in kilometers)
                        {
                            //if (kilometer >= elem.Start_Km && kilometer <= elem.Final_Km)
                            {
                                List<float> digListL = new List<float>();
                                List<float> digListR = new List<float>();
                             //    var DBcrossRailProfileRadius = AdditionalParametersService.GetCrossRailProfileDFPR3Radius(track_id, process.Date_Vrem, elem.Km, elem.Meter);
                                var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDB(elem, tripProcess.Trip_id);
                                if (DBcrossRailProfile == null || DBcrossRailProfile.Count == 0) continue;

                                var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(DBcrossRailProfile);

                                //var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromText(kilometer);

                                List<Digression> addDigressions = crossRailProfile.GetDigressions();
                              //  addDigressions = addDigressions.OrderBy(o => o.Meter).ToList();

                                //foreach (var digression in addDigressions)
                                {
                                    //if (elem.Side == "Левая" /*&& digression.DigName == DigressionName.SideWearLeft*/)
                                    {
                                        //if ((int)digression.Typ == 2) continue;
                                        //digListL.Add(digression.Value);
                                        digListL.AddRange(crossRailProfile.VertIznosL);
                                    }
                                    //if (elem.Side == "Правая" /*&& digression.DigName == DigressionName.SideWearRight*/)
                                    {
                                        //if ((int)digression.Typ == 2) continue;
                                        //digListR.Add(digression.Value);
                                        digListR.AddRange(crossRailProfile.VertIznosR);
                                    }
                                }

                                var speed = MainTrackStructureService.GetMtoObjectsByCoord(
                                            tripProcess.Date_Vrem, elem.Start_Km, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, $"1") as List<Speed>;

                               
                                var Vogr = -1;
                                if (digListL.Count > 0 )
                                {
                                    //if(digListL.Count!=0 && digListL.Max() > digListR.Max() && digListR.Count != 0 && digListL.Max() > digListR.Max())
                                    {
                                        var step = digListL.Max() > 13 && digListL.Max() <= 15 ? 3 : digListL.Max() > 15 ? 4 : -1;

                                        var Vust = speed.Count > 0 ? speed[0].Passenger : -1;
                                        Vust = Vogr >= Vust ? -1 : Vogr;

                                        XElement xeNote = new XElement("Note",
                                         new XAttribute("param", elem.Start_Km + "км " + elem.Start_M + "м - " + elem.Final_Km + "км " + elem.Final_M + "м"),
                                            new XAttribute("max", digListL.Max().ToString("0.0")),
                                            new XAttribute("sred", digListL.Average().ToString("0.0")),
                                            new XAttribute("Stepen", step),
                                            new XAttribute("radius", elem.Straightenings.Count > 0 ? elem.Straightenings[0].Radius.ToString() : ""),
                                            new XAttribute("Vust", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() + "/" + speed[0].Freight.ToString() : "-/-/-"),
                                              new XAttribute("Vogr", Vogr == -1 ? "" : speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight : "-/-")
                                            );
                                        //if (Vust != -1)
                                        {
                                            if (digListL.Max() >= (float)(float)filters[0].Value)
                                                lev.Add(xeNote);
                                        }
                                    }
                                    
                                }
                                if (digListR.Count > 0)
                                {
                                    //if (digListL.Max() < digListR.Max())
                                    {
                                        var step = digListR.Max() > 13 && digListR.Max() <= 15 ? 3 : digListR.Max() > 15 ? 4 : -1;

                                        var Vust = speed.Count > 0 ? speed[0].Passenger : -1;
                                        Vust = Vogr >= Vust ? -1 : Vogr;

                                        XElement xeNote = new XElement("Note",
                                         new XAttribute("param", elem.Start_Km + "км " + elem.Start_M + "м - " + elem.Final_Km + "км " + elem.Final_M + "м"),
                                            new XAttribute("max", digListR.Max().ToString("0.0")),
                                            new XAttribute("sred", digListR.Average().ToString("0.0")),
                                            new XAttribute("Stepen", step),
                                            new XAttribute("Vust", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() + "/" + speed[0].Freight.ToString() : "-/-/-"),
                                           new XAttribute("Vogr", Vogr == -1 ? "" : speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight : "-/-")
                                            );
                                        //if (Vust != -1)
                                        {
                                            if (digListR.Max() >= (float)(float)filters[0].Value)
                                                lev.Add(xeNote);
                                        }
                                    }
                                }
                            }
                        }                        
                    }
                    tripElem.Add(lev);
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
