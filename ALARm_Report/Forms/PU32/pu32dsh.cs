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
    public class pu32dsh : Report
    {

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

                var road = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        XElement tripElem = new XElement("trip",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                                new XAttribute("date_statement", tripProcess.Date_Vrem.Date.ToShortDateString()),
                                new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                                new XAttribute("road", road),
                                 new XAttribute("direction", tripProcess.DirectionName + " (" + tripProcess.DirectionCode + ")"),
                                new XAttribute("distance", $"{distance.Code}"),
                                new XAttribute("periodDate", period.Period),
                                new XAttribute("chief", tripProcess.Chief),
                                new XAttribute("pch", distance.Code),
                                new XAttribute("ps", tripProcess.Car),
                                new XAttribute("trip_date", tripProcess.Trip_date));
                        XElement lev = new XElement("lev", new XAttribute("napr", $"{tripProcess.DirectionName}({tripProcess.DirectionCode})  Путь: {trackName} "));

                        var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                        var previousKm = -1;
                        var speed = new List<Speed>();

                        var rail_type = new List<RailsSections>();
                        var skreplenie = new List<RailsBrace>();
                        var shpala = new List<CrossTie>();
                        var trackclasses = new List<TrackClass>();
                        var curves = new List<StCurve>();

                        var GetCountByPiket = RdStructureService.GetCountByPiket(tripProcess.Trip_id, MainTrackStructureConst.GetCountByPiket) as List<RdIrregularity>;
                        var Check_sleepers_state = AdditionalParametersService.Check_sleepers_state(tripProcess.Trip_id, template.ID);
                        var Check_Total_state_rails = (List<Digression>)AdditionalParametersService.Check_Total_state_rails(tripProcess.Trip_id, template.ID);

                        GetCountByPiket = GetCountByPiket.Where(o => o.Km >= kms.First() && o.Km <= kms.Last()).ToList();

                        foreach (var elem in GetCountByPiket)
                        {
                            var km = elem.Km;

                            if ((previousKm == -1) || (previousKm != km))
                            {
                                rail_type = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, km, MainTrackStructureConst.MtoRailSection, tripProcess.DirectionName, trackName.ToString()) as List<RailsSections>;
                                shpala = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, km, MainTrackStructureConst.MtoCrossTie, tripProcess.DirectionName, trackName.ToString()) as List<CrossTie>;
                                skreplenie = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, km, MainTrackStructureConst.MtoRailsBrace, tripProcess.DirectionName, trackName.ToString()) as List<RailsBrace>;
                                trackclasses = (List<TrackClass>)MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, km, MainTrackStructureConst.MtoTrackClass, track_id);
                                curves = (List<StCurve>)MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, km, MainTrackStructureConst.MtoStCurve, track_id);

                                speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, km, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, trackName.ToString()) as List<Speed>;
                            }
                            previousKm = km;

                            var trackclass = trackclasses.Count > 0 ? trackclasses[0].Class_Id : -1;
                            var curve = curves.Count > 0 ? (int)curves[0].Radius : -1;

                            //эпюра есептейміз
                            //Эпюры шпал на путях линий 1 - 3 - го классов должны быть: в прямых участках и в кривых радиусом более 1200 м - 1840 шт./ км, 
                            //радиусом 1200 м и менее - 2000 шт./ км; на путях 4 - 5 - го класса: в прямых и кривых радиусом более 1200 м - 1600 шт./ км, 
                            //радиусом 1200 м и менее - 1840 шт./ км.
                            var epur_bpd = 0;

                            switch (trackclass)
                            {
                                case int clas when clas >= 1 && clas <= 3:
                                    if (curve == -1 || curve > 1200)
                                    {
                                        epur_bpd = 1840;
                                    }
                                    else if(curve != -1 && curve <= 1200)
                                    {
                                        epur_bpd = 2000;
                                    }
                                    else
                                    {
                                        epur_bpd = 0;
                                    }
                                    break;
                                case int clas when clas >= 4 && clas <= 5:
                                    if (curve == -1 || curve > 1200)
                                    {
                                        epur_bpd = 1600;
                                    }
                                    else if (curve != -1 && curve <= 1200)
                                    {
                                        epur_bpd = 1840;
                                    }
                                    else
                                    {
                                        epur_bpd = 0;
                                    }
                                    break;
                                default:
                                    epur_bpd = 0;
                                    break;
                            }

                            var kns = Check_Total_state_rails.Where(o => o.Km == elem.Km && o.Meter / 100 + 1 == elem.Piket && o.Vdop != "").ToList();
                            var knsh = Check_sleepers_state.Where(o => o.Km == elem.Km && o.Meter / 100 + 1 == elem.Piket && o.Vdop != "").ToList();
                            string knsh_string = "";

                            string kn_string = "";

                            var ps = new List<int> { };
                            var gr = new List<int> { };

                           
                            foreach (var k in knsh)
                            {
                                //Определение звена 1,2,3,4
                                var ostatok = k.Meter % 100;
                                var zveno = (int)(ostatok / 25.0) + 1;
                                
                                
                                    knsh_string += $"{k.Velich}/{zveno}" + ", ";

                                    ps.Add(int.Parse(k.Vdop.Split('/')[0]));
                                    gr.Add(int.Parse(k.Vdop.Split('/')[1]));
                            }
                            foreach (var kn in kns)
                            {
                                //Определение звена 1,2,3,4
                                var ostatok = kn.Meter % 100;
                                var zveno = (int)(ostatok / 25.0) + 1;

                                kn_string += $"{kn.Velich}/{zveno}" + ", ";
                            }

                            if (knsh_string != "" || kn_string != "")
                            {
                                XElement note = new XElement("note",
                                           new XAttribute("km", $"{elem.Km}.{elem.Piket}"),
                                           new XAttribute("rails", rail_type.Count > 0 ? rail_type[0].Name : "Нет данных"),
                                           new XAttribute("sleepers", shpala.Count > 0 ? shpala[0].Name : "Нет данных"),
                                           new XAttribute("fasteners", skreplenie.Count > 0 ? skreplenie[0].Name : "Нет данных"),
                                           new XAttribute("epur", epur_bpd),
                                           new XAttribute("def_fasteners", elem.Skrep_def),
                                           new XAttribute("def_sleepers", elem.Shpal_def),
                                           new XAttribute("bad_fasteners", elem.Skrep_negod),
                                           new XAttribute("bad_sleepers", elem.Shpal_negod),
                                           new XAttribute("kust_sleepers", knsh_string),
                                           new XAttribute("kust_fasteners", kn_string),
                                           new XAttribute("vpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                           new XAttribute("vdop", ps.Count > 0 ? $"{ps[ps.IndexOf(ps.Min())]}/{gr[ps.IndexOf(ps.Min())]}" : ""));
                                lev.Add(note);
                            }

                       
                        }
                        tripElem.Add(lev);

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
