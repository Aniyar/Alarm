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
    public class DF310 : Report
    {
        public Direction TravelDirection { get; set; }
        private int prevMetr = -1;
        private float CurrentSm = 0;

        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
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
                var filterForm1 = new controls.FilterForm();
                var filters1 = new List<Filter>();
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                filters1.Add(new FloatFilter() { Name = "Пороговое значение (мм)", Value = 1.0f });

                filterForm1.SetDataSource(filters1);
                if (filterForm1.ShowDialog() == DialogResult.Cancel)
                    return;


                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var trackId in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(trackId);
                        //var kilometers = AdditionalParametersService.GetKilometers(tripProcess.Id, (int)tripProcess.Direction);
                        //var kms = AdditionalParametersService.GetKilometersByTripId(tripProcess.Id);
                        progressBar.Maximum = tripProcesses.Count;
                        var directName = AdditionalParametersService.DirectName(tripProcess.Id, (int)tripProcess.Direction);

                        

                        XElement data = new XElement("data",
                            new XAttribute("napr", $"{tripProcess.DirectionName}({tripProcess.DirectionCode})")
                            );
                        XElement pch = new XElement("Pch",
                            new XAttribute("npch", distance.Code)
                            );
                        XElement put = new XElement("Put",
                            new XAttribute("nput", trackName)
                            );
                        XElement prop = new XElement("Prop");

                        XElement lev = new XElement("lev");
                        List<Curve> curves = MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>;

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
                        curves = curves.Where(o => ((float)(float)filters[0].Value <= o.Start_Km && o.Final_Km <= (float)(float)filters[1].Value)).ToList().OrderBy(o=>o.Start_Km+o.Start_M/10000.0).ToList();

                        int iter = 1;

                        XElement tripElem = new XElement("trip",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("pch", distance.Code),
                            new XAttribute("ps", tripProcess.Car),
                            new XAttribute("km", $"{(float)(float)filters[0].Value} - {(float)(float)filters[1].Value}"),
                            new XAttribute("porog", (float)filters1[0].Value),
                            new XAttribute("road", road),
                            new XAttribute("period", period.Period),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("check", tripProcess.GetProcessTypeName)
                            );

                        foreach (var elem in curves)
                        {
                            List<float> VertIznosL = new List<float>();
                            List<float> VertIznosR = new List<float>();

                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDB(elem, tripProcess.Trip_id);
                            if (DBcrossRailProfile == null || DBcrossRailProfile.Count == 0) continue;

                            var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(DBcrossRailProfile);

                            List<Digression> addDigressions = crossRailProfile.GetDigressions();
                            addDigressions = addDigressions.OrderBy(o => o.Km + o.Meter/10000.0).ToList();

                            foreach (var digression in addDigressions)
                            {
                                if (elem.Side == "Левая" && digression.DigName == DigressionName.VertIznosL)
                                {
                                    //if ((int)digression.Typ == 2) continue;
                                    VertIznosL.Add(digression.Value);
                                }
                                if (elem.Side == "Правая" && digression.DigName == DigressionName.VertIznosR)
                                {
                                    //if ((int)digression.Typ == 2) continue;
                                    VertIznosR.Add(digression.Value);
                                }
                                //var vertIznos = AdditionalParametersService.vertIznos(kilometer);

                                //if (Math.Round(VertIznosR.Max(), 2) < (float)(float)filters[0].Value)
                                //{
                                //    continue;

                                //}
                                //if (Math.Round(VertIznosL.Max(), 2) < (float)(float)filters[0].Value)
                                //{
                                //    continue;
                                //}
                            }
                            var speed = MainTrackStructureService.GetMtoObjectsByCoord(
                                            tripProcess.Date_Vrem, elem.Start_Km, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, $"{trackName}") as List<Speed>;

                            var kord = elem.Start_Km + "км " + elem.Start_M + "м - " + elem.Final_Km + "км " + elem.Final_M + "м";
                            int four = 0;
                            int ten = 0;
                            int therty = 0;
                            foreach (var ob in VertIznosR)
                            {
                                if (ob > 4 && ob < 11)
                                {
                                    four = four + 1;
                                }
                                if (ob > 10 && ob < 14)
                                {
                                    ten = ten + 1;
                                }
                                if (ob > 13)
                                {
                                    therty = therty + 1;
                                }
                            }
                            if (four != 0 || ten != 0 || therty != 0)
                            {
                                prop = new XElement("Prop",
                                       new XAttribute("iD", iter++),
                                       new XAttribute("kM_a", kord),
                                       new XAttribute("rels", "пр."),
                                       new XAttribute("v", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight : "-/-"),
                                       new XAttribute("sred", Math.Round(crossRailProfile.VertIznosR.Where(oo => oo > 0).ToList().Average(), 2)),
                                       new XAttribute("max", Math.Round(crossRailProfile.VertIznosR.Where(oo => oo > 0).ToList().Max(), 2)),
                                       new XAttribute("fourMM", four.ToString() == "0" ? "-" : (four / 4).ToString()),
                                       new XAttribute("tenMM", ten.ToString() == "0" ? "-" : (ten / 4).ToString()),
                                       new XAttribute("thertyMM", therty.ToString() == "0" ? "-" : (therty / 4).ToString())
                                       );
                                if ((float)filters1[0].Value <= Math.Round(crossRailProfile.VertIznosR.Where(oo => oo > 0).ToList().Average(), 2))
                                    put.Add(prop);
                                four = 0;
                                ten = 0;
                                therty = 0;
                            }
                            continue;
                            if (four != 0 || ten != 0 || therty != 0)
                                foreach (var ob in VertIznosL)
                                {
                                    if (ob > 4 && ob < 11)
                                    {
                                        four = four + 1;
                                    }
                                    if (ob > 10 && ob < 14)
                                    {
                                        ten = ten + 1;
                                    }
                                    if (ob > 13)
                                    {
                                        therty = therty + 1;
                                    }
                                }
                            prop = new XElement("Prop",
                                        new XAttribute("iD", iter++),
                                        new XAttribute("kM_a", kord),
                                        new XAttribute("rels", "л."),
                                        new XAttribute("v", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight : "-/-"),
                                        new XAttribute("sred", Math.Round(crossRailProfile.VertIznosL.Where(oo => oo > 0).ToList().Average(), 2)),
                                        new XAttribute("max", Math.Round(crossRailProfile.VertIznosL.Where(oo => oo > 0).ToList().Max(), 2)),
                                        new XAttribute("fourMM", four.ToString() == "0" ? "-" : (four / 4).ToString()),
                                        new XAttribute("tenMM", ten.ToString() == "0" ? "-" : (ten / 4).ToString()),
                                        new XAttribute("thertyMM", therty.ToString() == "0" ? "-" : (therty / 4).ToString())
                                        );
                            if ((float)filters1[0].Value <= Math.Round(crossRailProfile.VertIznosL.Where(oo => oo > 0).ToList().Average(), 2))
                                put.Add(prop);
                            {
                            }
                            continue;
                          
                          
                          
                        }
                       
                        pch.Add(put);
                        data.Add(pch);
                        tripElem.Add(data);
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
