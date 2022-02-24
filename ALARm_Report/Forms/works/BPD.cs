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
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm_Report.Forms
{
    public class BPD : Report
    {
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
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
                foreach (var track_id in admTracksId)
                {
                    foreach (var tripProcess in tripProcesses)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        var trip = RdStructureService.GetTrip(tripProcess.Trip_id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);


                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        //List<Curve> curves = RdStructureService.GetCurvesInTrip(tripProcess.Trip_id) as List<Curve>;
                        var lkm = kilometers.Select(o => o.Number).ToList();

                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        kilometers = kilometers.Where(Km => ((float)(float)filters[0].Value <= Km.Number && Km.Number <= (float)(float)filters[1].Value)).ToList();
                        kilometers = (tripProcess.Travel_Direction == Direction.Reverse ? kilometers.OrderBy(o => o.Number) : kilometers.OrderByDescending(o => o.Number)).ToList();
                        //--------------------------------------------

                        var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;
                        CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;

                        List<Digression> notes = RdStructureService.GetDigressions3and4(tripProcess.Trip_id, distance.Code, new int[] { 3, 4 });
                        int iter = 1;
                        XElement xeTracks = new XElement("tracks");
                        XElement xeTracksFact = new XElement("fact");

                        XElement tripElem = new XElement("trip",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                            new XAttribute("direction", tripProcess.DirectionName),
                            new XAttribute("directioncode", tripProcess.DirectionCode),
                            new XAttribute("check", tripProcess.GetProcessTypeName),
                            new XAttribute("track", curvesAdmUnit.Track),
                            new XAttribute("road", road),
                            new XAttribute("distance", distance.Code),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("ps", tripProcess.Car));

                        // выводить если не соотвествует
                        var filter_curves = curves.Where(o => ((float)(float)filters[0].Value <= o.Start_Km && o.Final_Km <= (float)(float)filters[1].Value)).ToList();
                        foreach (var curve in filter_curves)
                        {

                           
                    


                            curve.Elevations = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();
                            curve.Straightenings = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();
                            List<RDCurve> rdcs = RdStructureService.GetRDCurves(curve.Id, trip.Id);
                            if (rdcs.Any())
                            {
                                try
                                {
                                    var LevelPoins = rdcs.Where(o => o.Point_level > 0).ToList();
                                    var StrPoins = rdcs.Where(o => o.Point_str > 0).ToList();

                                    if (LevelPoins.Count > 4)
                                    {
                                        if (LevelPoins[0].Level < 15.0 && LevelPoins[1].Level < 15.0)
                                        {
                                            LevelPoins.RemoveAt(0);
                                            StrPoins.RemoveAt(0);
                                        }
                                        else if (LevelPoins[3].Level < 15.0 && LevelPoins[4].Level < 15.0)
                                        {
                                            LevelPoins.RemoveAt(4);
                                            if (StrPoins.Count > 4)
                                                StrPoins.RemoveAt(4);
                                        }
                                    }

                                    var LevelMax = rdcs.Select(o => o.Trapez_level).ToList();
                                    var StrMax = rdcs.Select(o => o.Trapez_str).ToList();

                                    var lenPerKriv10000 = ((StrPoins[1].Km + StrPoins[1].M / 10000.0) - (StrPoins[2].Km + StrPoins[2].M / 10000.0)) * 10000;
                                    var lenPerKriv = Math.Abs((int)lenPerKriv10000 % 1000);

                                    var lenKriv10000 = ((StrPoins[0].Km + StrPoins[0].M / 10000.0) - (StrPoins[3].Km + StrPoins[3].M / 10000.0)) * 10000;
                                    var lenKriv = Math.Abs((int)lenKriv10000 % 1000);

                                    var lenPerKriv10000lv = ((LevelPoins[1].Km + LevelPoins[1].M / 10000.0) - (LevelPoins[2].Km + LevelPoins[2].M / 10000.0)) * 10000;
                                    var lenPerKrivlv = Math.Abs((int)lenPerKriv10000lv % 1000);

                                    var lenKriv10000lv = ((LevelPoins[0].Km + LevelPoins[0].M / 10000.0) - (LevelPoins[3].Km + LevelPoins[3].M / 10000.0)) * 10000;
                                    var lenKrivlv = Math.Abs((int)lenKriv10000lv % 1000);

                                    var d = false;
                                    if ((StrPoins[0].Km + StrPoins[0].M / 10000.0) > (StrPoins[3].Km + StrPoins[3].M / 10000.0))
                                        d = true;

                                    //нижние 2 точки трапеции
                                    var start_km = d ? StrPoins[3].Km : StrPoins.First().Km;
                                    var start_m = d ? StrPoins[3].M : StrPoins.First().M;
                                    var final_km = d ? StrPoins.First().Km : StrPoins[3].Km;
                                    var final_m = d ? StrPoins.First().M : StrPoins[3].M;

                                    var start_lvl_km = d ? LevelPoins[3].Km : LevelPoins.First().Km;
                                    var start_lvl_m = d ? LevelPoins[3].M : LevelPoins.First().M;
                                    var final_lvl_km = d ? LevelPoins.First().Km : LevelPoins[3].Km;
                                    var final_lvl_m = d ? LevelPoins.First().M : LevelPoins[3].M;

                                    var razn1 = (int)(((start_km + start_m / 10000.0) - (start_lvl_km + start_lvl_m / 10000.0)) * 10000) % 1000; // start
                                    var razn2 = (int)(((final_km + final_m / 10000.0) - (final_lvl_km + final_lvl_m / 10000.0)) * 10000) % 1000; // final
                                    var razn3 = lenKriv - lenKrivlv; // общая длина нижних

                                    //верхние 2 точки трапеции
                                    var start_kmc = d ? StrPoins[2].Km : StrPoins[1].Km;
                                    var start_mc = d ? StrPoins[2].M : StrPoins[1].M;
                                    var final_kmc = d ? StrPoins[1].Km : StrPoins[2].Km;
                                    var final_mc = d ? StrPoins[1].M : StrPoins[2].M;

                                    var start_lvl_kmc = d ? LevelPoins[2].Km : LevelPoins[1].Km;
                                    var start_lvl_mc = d ? LevelPoins[2].M : LevelPoins[1].M;
                                    var final_lvl_kmc = d ? LevelPoins[1].Km : LevelPoins[2].Km;
                                    var final_lvl_mc = d ? LevelPoins[1].M : LevelPoins[2].M;

                                    var razn1c = (int)(((start_kmc + start_mc / 10000.0) - (start_lvl_kmc + start_lvl_mc / 10000.0)) * 10000) % 1000; // start
                                    var razn2c = (int)(((final_kmc + final_mc / 10000.0) - (final_lvl_kmc + final_lvl_mc / 10000.0)) * 10000) % 1000; // final

                                    //Переходные 
                                    //1-й
                                    var tap_len1 = Math.Round(((start_km + start_m / 10000.0) - (start_kmc + start_mc / 10000.0)) * 10000) % 1000;
                                    var tap_len1_lvl = Math.Round(((start_lvl_km + start_lvl_m / 10000.0) - (start_lvl_kmc + start_lvl_mc / 10000.0)) * 10000) % 1000;
                                    //2-й
                                    var tap_len2 = Math.Round(((final_km + final_m / 10000.0) - (final_kmc + final_mc / 10000.0)) * 10000) % 1000;
                                    var tap_len2_lvl = Math.Round(((final_lvl_km + final_lvl_m / 10000.0) - (final_lvl_kmc + final_lvl_mc / 10000.0)) * 10000) % 1000;

                                    //Радиус/Уровень (для мин макс сред)
                                    var temp_data = rdcs.GetRange((int)Math.Abs(tap_len1_lvl) + 40, Math.Abs(lenPerKrivlv));
                                    var temp_data_str = rdcs.Where(o => (start_kmc + start_mc / 10000.0) <= (o.Km + o.M / 10000.0) && (o.Km + o.M / 10000.0) <= (final_kmc + final_mc / 10000.0)).ToList();
                                    var temp_data_lvl = rdcs.Where(o => (start_lvl_kmc + start_lvl_mc / 10000.0) <= (o.Km + o.M / 10000.0) && (o.Km + o.M / 10000.0) <= (final_lvl_kmc + final_lvl_mc / 10000.0)).ToList();

                                    //Переходные (для макс сред)
                                    var transitional_lvl_data = rdcs.GetRange(40, Math.Abs((int)tap_len1_lvl));
                                    var transitional_str_data = rdcs.GetRange(40, Math.Abs((int)tap_len1));

                                    var transitional_lvl_data2 = rdcs.GetRange((int)Math.Abs(tap_len1_lvl) + 40 + Math.Abs(lenPerKrivlv), Math.Abs((int)tap_len2_lvl));
                                    var transitional_str_data2 = rdcs.GetRange((int)Math.Abs(tap_len1) + 40 + Math.Abs(lenPerKriv), Math.Abs((int)tap_len2));

                                    var radius = 17860 / (Math.Abs(temp_data_str.Average(fact => fact.Radius)) + 0.00000001);
                                    //данные бокового износа
                                    var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyCurve(curve, trip.Id);

                                    var bok_izl = new List<float> { };
                                    var bok_izr = new List<float> { };

                                    var side_bok = false;

                                    if (DBcrossRailProfile.Any())
                                    {
                                        bok_izl.AddRange(DBcrossRailProfile.Select(o => o.Bok_l).ToList());
                                        bok_izr.AddRange(DBcrossRailProfile.Select(o => o.Bok_r).ToList());

                                        if (bok_izl.Average() > bok_izr.Average())
                                            side_bok = true;
                                    }

                                    var iznos = side_bok ? bok_izl : bok_izr;

                                    var bok_iz_graph = "";
                                    var indx = LevelPoins.Any() ? rdcs.IndexOf(LevelPoins.First()) : 1;
                                    foreach (var item in iznos)
                                    {
                                        bok_iz_graph += $"{indx}, {(-item).ToString("0.00000").Replace(",", ".")} ";
                                        indx++;
                                    }
                                   
                                    var razMFinal = Math.Abs(start_lvl_m - curve.Final_M);
                                    var razKMFinal = Math.Abs(final_lvl_m - curve.Start_Km);
                                    
                                    var razElevetion = Math.Abs(Convert.ToInt32(temp_data_lvl.Select(o => Math.Abs(o.Level)).Average()) - curve.Elevations.First().Lvl);
                                    var razWear = Math.Abs(iznos.Where(o => o > 0).Max() - curve.Straightenings.First().Wear);
                                        XElement xeNote = new XElement("note",
                                           new XAttribute("n", iter++),

                                           new XAttribute("startkm", $"{curve.Start_Km}.{curve.Start_M}"),
                                           new XAttribute("finalkm", $"{curve.Final_Km}.{curve.Final_M}"),

                                           new XAttribute("Radius", curve.Radius),
                                           new XAttribute("Elevation", curve.Elevations.First().Lvl),
                                           new XAttribute("Wear", curve.Straightenings.First().Wear), //износ по пасспорту
                                         
                                           new XAttribute("factstartkm",  $"{start_lvl_km}.{start_lvl_m}"),
                                           new XAttribute("factfinalkm", $"{final_lvl_km}.{final_lvl_m}"),
                                           new XAttribute("factRadius", radius.ToString("0")),
                                           new XAttribute("factElevation", Convert.ToInt32(temp_data_lvl.Select(o => Math.Abs(o.Level)).Average())),
                                           new XAttribute("factWear", iznos.Any() ? iznos.Where(o => o > 0).Max().ToString("0") : ""), // натурный износ
                                           new XAttribute("razElevetion", razElevetion),
                                           new XAttribute("razWear", razWear),
                                           new XAttribute("razMFinal", razMFinal),
                                           new XAttribute("razKMFinal", razKMFinal)
                                           );
                                     
                                       
                                        xeTracks.Add(xeNote);

                                }
                                catch
                                {

                                }


                            }
                        }
                   
                        tripElem.Add(xeTracks);
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
