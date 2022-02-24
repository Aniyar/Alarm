using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Xsl;
using ALARm.Core.AdditionalParameteres;
using System;
using System.Reflection;

namespace ALARm_Report.Forms
{
    public class pu32kompl : ALARm.Core.Report.GraphicDiagrams
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();
            this.RdStructureRepository = RdStructureService.GetRepository();
            this.AdmStructureRepository = AdmStructureService.GetRepository();

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
           
                var tripProcesses = RdStructureService.GetTripsOnDistance(parentId, period);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);
                    foreach (var track_id in admTracksId)
                    {
                        //Участки дист коррекция
                        var listBedemost = MainTrackStructureService.GetDistSectionByDistId(distance.Id);
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip).OrderBy(o=>o.Number).ToList();
                        var cn = kilometers.Count;
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var GetCountByPiket = RdStructureService.GetCountByPiket(tripProcess.Id, MainTrackStructureConst.GetCountByPiket) as List<RdIrregularity>;

                        XElement tripElem = new XElement("trip",
                              new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        new XAttribute("pch", distance.Code),
                        new XAttribute("check", tripProcess.GetProcessTypeName),
                     
                        new XAttribute("road", road),
                    
                        new XAttribute("trip_date", period.Period),
                       // new XAttribute("Info", $"{tripProcess.DirectionName}({tripProcess.DirectionCode}), Путь:{trackName}, {period.Period}"),
                            new XAttribute("car", tripProcess.Car),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("Count", cn + 2)
                        );                  

                        //пч
                        var CountPch = listBedemost.Count();
                        var Pch = new XElement("Pch",
                                   // new XAttribute("PchCode", $"ПЧ-{distance.Code}({distance.Id})"),
                                    new XAttribute("PchCode", $"{tripProcess.Direction}({tripProcess.DirectionCode})  Путь: {trackName} "),
                                    new XAttribute("Count", cn)
                                    );

                        var DistincPd = listBedemost.Select(o => o.Pd).Distinct().ToList();

                        XElement Pages = new XElement("pages");
                        //foreach (listBedemost in kilometers)
                        //{ }
                        foreach (var Pd in DistincPd)
                        {
                            var ListPd = listBedemost.Where(o => o.Pd == Pd).ToList();
                            //пд
                            var PD = new XElement("Pd", 
                                new XAttribute("PdCode", $"ПД-{ListPd.First().Pd}")
                                );
                            var CountPd = 0;

                            for (int i = ListPd.First().Start_Km; i <= ListPd.Last().Final_Km; i++)
                            {
                                    var km = kilometers.Where(o => o.Number == i).ToList();
                                    if (km.Count == 0) continue;
                                    var kilometer = km.First();
                                    kilometer.LoadTrackPasport(MainTrackStructureRepository, tripProcess.Trip_date);
                                kilometer.LoadDigresions(RdStructureRepository, MainTrackStructureRepository, tripProcess);

                                var dig = kilometer.Digressions.Where(o => (int)o.Digression != -1).ToList();
                                var dig34 = dig.Where(o => o.Degree == 3 || o.Degree == 4).ToList();

                                var shpala = GetCountByPiket.Where(o => o.Km == kilometer.Number).ToList();
                                var shpala_def = shpala.Select(o => o.Shpal_def).Sum();
                                var shpala_negod = shpala.Select(o => o.Shpal_negod).Sum();

                                
                                //foreach ( km in kilometers)
                                { }
                                    var KM = new XElement("Km",
                                            new XAttribute("KmCode", kilometer.Number),
                                            new XAttribute("Count", 1),
                                            new XAttribute("Peregon",
                                                                kilometer.StationSection != null && kilometer.StationSection.Count > 0 ? 
                                                                    "Станция: " + kilometer.StationSection[0].Station : 
                                                                    (kilometer.Sector != null ? kilometer.Sector.ToString() : "")),

                                            new XAttribute("Vust", kilometer.Speeds.Count > 0 ? kilometer.Speeds.Last().Passenger+"/"+ kilometer.Speeds.Last().Freight : "-/-"),
                                            new XAttribute("Vdop", /*kilometer.Speeplimit*/ "-/-"),
                                            new XAttribute("digression", dig.Count),
                                            new XAttribute("digression3and4", dig34.Count),
                                            new XAttribute("SSSP", CalcSSSP(kilometer.Number,tripProcess.Id)),
                                            new XAttribute("Gabarit", "-/-"),
                                            new XAttribute("MESHPUT", "-/-"),
                                            new XAttribute("KrivVogr", "-/-"),
                                            new XAttribute("Rast2and3", "-/-"),
                                            new XAttribute("PizmaRIGHT", "-/-"),
                                            new XAttribute("PizmaLEFT", "-/-"),
                                            new XAttribute("Uklon", "-/-"),
                                            new XAttribute("SHIRINANASIP", "-/-"),
                                            new XAttribute("STYKSVERHNORMA", "-/-"),
                                            new XAttribute("PODUKLON", "-/-"),
                                            new XAttribute("BOKIZNOS", "-/-"),
                                            new XAttribute("IMPNEROV", "-/-"),
                                            new XAttribute("KORNEROV", "0,00/0,00"),
                                            new XAttribute("SOSTSHPAL", $"{shpala_def}/{shpala_negod}"),
                                            new XAttribute("SOSTBALAST", "-/-"),
                                            new XAttribute("GODPOSLEKAPREM", "-/-"),
                                            new XAttribute("TONAG", "-/-")
                                            );
                                PD.Add(KM);

                                CountPd++;
                                //if(km in kms)
                            }

                            PD.Add(new XAttribute("Count", CountPd));
                            Pch.Add(PD);
                        }
                        tripElem.Add(Pch);
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

        private object CalcSSSP(int kilometer, long Trip_id)
        {
            //sssp calculate------------
            double[] Со = { 0.40, 0.44, 0.51, 0.59, 0.68, 0.80, 1.0, 1.28, 1.55, 1.85, 2.25, 2.75 };
            int[] SSSP = { 250, 220, 200, 180, 160, 140, 120, 100, 80, 60, 40, 15 };

            var LinearSSSP = new List<double> { };
            var LinearCo = new List<double> { };

            for (int x = 0; x < SSSP.Count() - 1; x++)
            {
                var k = (SSSP[x + 1] - SSSP[x]) / (Со[x + 1] - Со[x]);
                var d = (Со[x + 1] - Со[x]) / 50.0;

                for (int y = 0; y < 50; y++)
                {
                    var SSSP_linear = SSSP[x] + k * y * d;

                    var Co_linear = Со[x] + y * d;

                    LinearSSSP.Add(SSSP_linear);
                    LinearCo.Add(Co_linear);
                }
            }
            //---------------------------------

            var mainPar = MainParametersService.GetMainParametersSKOvedomDB(kilometer, Trip_id);
            if (mainPar.Count == 0) return "";

            var Skewness_SKO = new List<float>();
            var Drawdown_left_SKO = new List<float>();
            var Drawdown_right_SKO = new List<float>();
            var Gauge = new List<float>();
            var Stright_left = new List<float>();
            var Stright_right = new List<float>();
            var SSSP_speed = new List<float>();
            var Cv = new List<float>();
            var Cr = new List<float>();
            var Co = new List<float>();

            foreach (var elem in mainPar)
            {
                Skewness_SKO.Add(elem.Skewness_SKO);
                Drawdown_left_SKO.Add(elem.Drawdown_left_SKO);
                Drawdown_right_SKO.Add(elem.Drawdown_right_SKO);
                Gauge.Add(elem.Gauge);
                Stright_left.Add(elem.Stright_left);
                Stright_right.Add(elem.Stright_right);
                Cv.Add(elem.Cv);
                Cr.Add(elem.Cr);
                Co.Add(elem.Co);
            }

            var calc_c0 = Cv.Average() * 0.615 + Cr.Average() * 0.39;
            var C_list = LinearCo.Select(o => Math.Abs(o - calc_c0)).ToList();

            var min = C_list.Min();
            var ind = C_list.IndexOf(min);

            var sssp = LinearSSSP[ind];

            return sssp.ToString("0");
        }
    }
}
