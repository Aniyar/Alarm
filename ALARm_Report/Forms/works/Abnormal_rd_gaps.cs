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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm_Report.Forms
{
    public class Abnormal_rd_gaps : Report
    {
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
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

                List<Curve> curves = (MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);
                distance.Name = distance.Name.Replace("ПЧ-", "");

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
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);
                        if (!kilometers.Any()) continue;

                        kilometers = kilometers.Where(o => o.Track_id == track_id).ToList();

                        trip.Track_Id = track_id;
                        var lkm = kilometers.Select(o => o.Number).ToList();

                        if (lkm.Count() == 0) continue;

                      
                       


                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();


           

                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        kilometers = kilometers.Where(Km => ((float)(float)filters[0].Value <= Km.Number && Km.Number <= (float)(float)filters[1].Value)).ToList();
                        kilometers = (tripProcess.Travel_Direction == Direction.Reverse ? kilometers.OrderBy(o => o.Number) : kilometers.OrderByDescending(o => o.Number)).ToList();
                        //--------------------------------------------
                        List<Digression> notes = RdStructureService.GetDigressions3and4(tripProcess.Trip_id, distance.Code, new int[] { 3, 4 });
                        var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;

                        CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;



                        XElement tripElem = new XElement("trip",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                            new XAttribute("direction", curvesAdmUnit.Direction),
                           
                            new XAttribute("check", tripProcess.GetProcessTypeName),
                            new XAttribute("track", curvesAdmUnit.Track),
                            new XAttribute("road", road),
                            new XAttribute("distance", distance.Code),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("ps", tripProcess.Car));



                        var speed = new List<Speed>();
                        var temperature = new List<Temperature>();
                        int previousGap = -1;

                        progressBar.Maximum = tripProcesses.Count;

                        //var gaps = AdditionalParametersService.GetGap(tripProcess.Id, (int)tripProcess.Direction);

                        //startKM = gaps[0].Km;
                        //finalKM = gaps[gaps.Count - 1].Km;

                        string lastDirection = String.Empty, lastTrack = String.Empty;
                        int lastPchu = -1, lastPd = -1, lastPdb = -1, lastKm = -1;
                        List<DigressionTotal> totals = new List<DigressionTotal>();
                        DigressionTotal digressionTotal = new DigressionTotal();
                        XElement xeDirection = new XElement("directions");
                        XElement xeTracks = new XElement("tracks");
                        XElement lev = new XElement("lev");
                        int boleeFirst = 0;
                        int boleeSecond = 0;
                        int boleeTherd = 0;
                        int boleeFourth = 0;
                        List<Gap> check_gap_state = AdditionalParametersService.Check_gap_state(tripProcess.Trip_id, template.ID);
                        foreach (var km in kilometers)
                        {
                            //var PRUbyKmMAIN = check_gap_state.Where( o.Km == km.Number).ToList();

                            //данные стыка
                            var gaps = new List<Digression> { };

                            foreach (var gap in check_gap_state.Where(o =>  o.Km == km.Number).ToList())
                            {
                                gap.PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                                gap.FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;

                                gaps.Add(gap.GetDigressions());
                                gaps.Add(gap.GetDigressions3());
                            }

                            foreach (var dig in gaps)
                            {
                                int count = dig.Length / 4;
                                count += dig.Length % 4 > 0 ? 1 : 0;

                                var side = " " + (dig.Threat == (Threat)Threat.Right ? "Зазор правый" : dig.Threat == (Threat)Threat.Left ? "Зазор левый" : "");

                                km.Digressions.Add(
                                    new DigressionMark()
                                    {
                                        Digression = dig.DigName,
                                        NotMoveAlert = false,
                                        Meter = dig.Meter,
                                        finish_meter = dig.Kmetr,
                                        Length = dig.Length,
                                        Value = dig.Velich,
                                        Count = count,
                                        DigName = dig.GetName() + side,
                                        PassengerSpeedLimit = dig.PassengerSpeedLimit,
                                        FreightSpeedLimit = dig.FreightSpeedLimit,
                                        Comment = "",
                                        AllowSpeed = dig.AllowSpeed
                                    });

                            }

                            //var gaps = AdditionalParametersService.RDGetGap(tripProcess.Trip_id, (int)tripProcess.Direction, (int)track_id);
                            var PRUbyKmMAIN = check_gap_state.Where(o => o.Km == km.Number).ToList();
                            //var PRUbyKmMAIN = check_gap_state.ToList();
                            //gaps = gaps.Where(o => o.Km > 128).ToList();
                            //if (gaps == null || gaps.Count == 0) { continue; }
                            //gaps = gaps.Where(o => o.Km > 128).ToList();

                            foreach (var gap in PRUbyKmMAIN)
                            {
                                if (gap.Zazor > 24 && gap.Zazor <= 26) boleeFirst++;
                                if (gap.Zazor > 26 && gap.Zazor <= 30) boleeSecond++;
                                if (gap.Zazor > 30 && gap.Zazor <= 35) boleeTherd++;
                                if (gap.Zazor > 35) boleeFourth++;

                                if (gap.R_zazor > 24 && gap.R_zazor <= 26) boleeFirst++;
                                if (gap.R_zazor > 26 && gap.R_zazor <= 30) boleeSecond++;
                                if (gap.R_zazor > 30 && gap.R_zazor <= 35) boleeTherd++;
                                if (gap.R_zazor > 35) boleeFourth++;
                                if ((previousGap == null) || (previousGap != gap.Km))
                                {
                                    speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, trackName.ToString()) as List<Speed>;
                                    temperature = MainTrackStructureService.GetTemp(tripProcess.Trip_id, track_id, gap.Km) as List<Temperature>;

                                    previousGap = gap.Km;
                                }

                                gap.PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                                gap.FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;
                                var dig = gap.GetDigressions();
                                var dig2 = gap.GetDigressions2();
                               
                                if (gap.Zazor > 24)
                                {

                                    if (!dig.AllowSpeed.Equals("25/25"))
                                        dig.AllowSpeed = "-";

                                    XElement Notes = new XElement("note");
                                    Notes.Add(
                                        new XAttribute("km", gap.Km),
                                        new XAttribute("m", gap.Meter),
                                        new XAttribute("Ots", gap.Threat == Threat.Left ? " Зазор правый" : "Зазор левый "),
                                        new XAttribute("velichina", gap.Zazor.ToString()),//ToDo temperature
                                        new XAttribute("temperature", temperature.Count != 0 ? temperature[0].Kupe.ToString() : " "),//ToDo temperature
                                        new XAttribute("Vpz", speed.Count > 0? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                        new XAttribute("VdopZazor", dig.AllowSpeed),
                                        //new XAttribute("VdopZazor",  dig.AllowSpeed.Equals("25/25") ? dig.AllowSpeed  : "-/-"),

                                        new XAttribute("Primech","")
                                        );
                                    tripElem.Add(Notes);
                                }
                                if (gap.R_zazor > 24)
                                {
                                    if (!dig2.R_AllowSpeed.Equals("25/25"))
                                        dig2.R_AllowSpeed = "-";

                                    XElement Notes = new XElement("note");
                                    Notes.Add(
                                        new XAttribute("km", gap.Km),
                                        new XAttribute("m", gap.Meter),
                                        new XAttribute("Ots", gap.R_threat == Threat.Right ? "Зазор левый " : " Зазор правый"),
                                        new XAttribute("velichina", gap.R_zazor.ToString()),//ToDo temperature
                                        new XAttribute("temperature", temperature.Count != 0 ? temperature[0].Koridor.ToString() : " "),//ToDo temperature
                                        new XAttribute("Vpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                        new XAttribute("VdopZazor", dig2.R_AllowSpeed),

                                        //new XAttribute("VdopZazor", dig2.AllowSpeed.Equals("25/25") ? dig2.R_AllowSpeed  : "-/-"),
                                        new XAttribute("Primech", "")
                                        );
                                    tripElem.Add(Notes);
                                }
                                
                                //if (dig2.R_DigName == DigressionName.Gap || dig.DigName == DigressionName.Gap || dig2.R_DigName == DigressionName.AnomalisticGap || dig.DigName == DigressionName.AnomalisticGap)
                                //{
                                //    lev.Add(Notes);
                                //}

                            }
                     
                        }
                        lev.Add(
                   new XAttribute("total", boleeFirst+ boleeSecond+ boleeTherd+ boleeFourth),
                   new XAttribute("boleeFirst", boleeFirst),
                   new XAttribute("boleeSecond", boleeSecond),
                   new XAttribute("boleeTherd", boleeTherd),
                   new XAttribute("boleeFourth", boleeFourth)
                   );
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
                htReport.Save($@"G:\form\3.Износ рельсов,стыковые зазоры,деформативные характеристики пути\24.Ведомость сверхнормативных стыковых зазоров .html");
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
