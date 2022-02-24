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
    public class DFi2 : Report

    {
        /// <summary>
        /// Округление до кратному пяти
        /// </summary>
        /// <param name="num">координата в метрах</param>
        /// <returns>вощвращает координату в метрах кратному пяти</returns>
        private int RoundNum(int num)
        {
            int rem = num % 10;
            return rem >= 5 ? (num - rem + 10) : (num - rem);
        }
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            //Сделать выбор периода
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(parentId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel)
                    return;
                admTracksId = choiceForm.admTracksIDs;
            }
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

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var trackId in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(trackId);
                        

                        XElement lev = new XElement("lev");
                        //List<Curve> curves = MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>;
                        List<Curve> curves = RdStructureService.GetCurvesInTrip(tripProcess.Trip_id) as List<Curve>;
                        if (!curves.Any()) continue;
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var min = curves.Select(o => o.Start_Km).Min();
                        var max = curves.Select(o => o.Final_Km).Max();

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = min });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = max });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;
                        //фильтр по выбранным км
                        curves = curves.Where(o => ((float)(float)filters[0].Value <= o.Start_Km && o.Final_Km <= (float)(float)filters[1].Value)).ToList();

                        //var kilometers = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);

                        int iter = 1;

                        XElement tripElem = new XElement("trip",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("road", road),
                            new XAttribute("km", $"{(float)(float)filters[0].Value} - {(float)(float)filters[1].Value}"),
                            new XAttribute("put", trackName),
                            new XAttribute("napr", $"{tripProcess.DirectionName}({tripProcess.DirectionCode})"),
                            new XAttribute("ps", tripProcess.Car),
                            new XAttribute("pch", distance.Code),
                            new XAttribute("period1", period.Period),
                            new XAttribute("period2", period.Period),
                            new XAttribute("chief", tripProcess.Chief),
                           
                        new XAttribute("date_statement", tripProcess.GetProcessTypeName)
                        // new XAttribute("km", "")
                        );

                        foreach (var elem in curves)
                        {
                            List<float> digListL = new List<float>();
                            List<float> digListR = new List<float>();

                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDB(elem, tripProcess.Trip_id);
                            if (DBcrossRailProfile == null || DBcrossRailProfile.Count == 0) continue;

                            var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(DBcrossRailProfile);

                            //var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromText(kilometer);

                            List<Digression> addDigressions = crossRailProfile.GetDigressions();
                            addDigressions = addDigressions.OrderBy(o => o.Meter).ToList();

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
                                            tripProcess.Date_Vrem, elem.Start_Km, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, $"{trackName}") as List<Speed>;

                            if (digListL.Count > 0)
                            {
                                if (digListL.Count != 0 && digListL.Max() > digListR.Max() && digListR.Count != 0 && digListL.Max() > digListR.Max())
                                {
                                    
                                    var Vust = speed.Count > 0 ? speed[0].Passenger : -1;

                                    XElement xeNote = new XElement("Note",
                                        new XAttribute("iter", iter),
                                        new XAttribute("id", elem.Num),
                                        new XAttribute("start_km", elem.Start_Km + "." + elem.Start_M ),
                                        new XAttribute("final_km",  elem.Final_Km + "." + elem.Final_M),
                                        new XAttribute("radius", elem.Radius),
                                        new XAttribute("Vust", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                        new XAttribute("sred1", digListL.Average().ToString("0.0")),
                                        new XAttribute("max1", digListL.Max().ToString("0.0")),
                                        new XAttribute("sred2", digListL.Average().ToString("0.0")),
                                        new XAttribute("max2", digListL.Max().ToString("0.0")),
                                        new XAttribute("intensv", $"{ Math.Abs(digListL.Average() - digListL.Average()) }")
                                        );

                                    lev.Add(xeNote);
                                }

                            }
                            if (digListR.Count > 0)
                            {
                                if (digListL.Max() < digListR.Max())
                                {
                                    var Vust = speed.Count > 0 ? speed[0].Passenger : -1;

                                    XElement xeNote = new XElement("Note",
                                        new XAttribute("iter", iter),
                                        new XAttribute("id", elem.Num),
                                        new XAttribute("start_km", elem.Start_Km + "." + elem.Start_M),
                                        new XAttribute("final_km", elem.Final_Km + "." + elem.Final_M),
                                        new XAttribute("param", elem.Start_Km + "км " + elem.Start_M + "м - " + elem.Final_Km + "км " + elem.Final_M + "м"),
                                        new XAttribute("radius", elem.Radius),
                                        new XAttribute("Vust", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString(): "-/-"),
                                        new XAttribute("sred1", digListR.Average().ToString("0.0")),
                                        new XAttribute("max1", digListR.Max().ToString("0.0")),
                                        new XAttribute("sred2", digListR.Average().ToString("0.0")),
                                        new XAttribute("max2", digListR.Max().ToString("0.0")),
                                        new XAttribute("intensv", $"{ Math.Abs(digListR.Average() - digListR.Average()) }")
                                        );

                                    lev.Add(xeNote);
                                    
                                }
                            }
                            iter++;
                        }
                        tripElem.Add(new XAttribute("trackinfo", $"{tripProcess.DirectionName}({tripProcess.DirectionCode}) Путь: {trackName}")); 
                        tripElem.Add(lev) ;
                        report.Add(tripElem);
                    }
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
