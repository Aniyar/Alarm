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
    public class Long_profile_irregularities : Report
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

                     

                        //filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        //filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        kilometers = kilometers.Where(Km => ((float)(float)filters[0].Value <= Km.Number && Km.Number <= (float)(float)filters[1].Value)).ToList();
                        kilometers = (tripProcess.Travel_Direction == Direction.Reverse ? kilometers.OrderBy(o => o.Number) : kilometers.OrderByDescending(o => o.Number)).ToList();
                        //--------------------------------------------

                        string[] subs = tripProcess.DirectionName.Split('(');
                        List<Speed> speeds = MainTrackStructureService.GetSpeeds(tripProcess.Date_Vrem, subs.Any() ? subs.First() : "", trackName.ToString());
                        //speeds = speeds.Where(o => ((float)(float)filters[0].Value <= o.Start_Km && o.Final_Km <= (float)(float)filters[1].Value)).ToList();

                        var OutData = RdStructureService.GetRdTables(tripProcess, 12) as List<RdProfile>;


                        XElement tripElem = new XElement("trip",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("direction", tripProcess.DirectionName),
                            new XAttribute("check", tripProcess.GetProcessTypeName),
                            new XAttribute("track", trackName),
                            new XAttribute("road", road),
                            new XAttribute("distance", distance.Code),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("ps", tripProcess.Car));
                        XElement xeDirection = new XElement("directions");
                        XElement xeTracks = new XElement("tracks");
                        

                        foreach (var item in speeds)
                        {
                            var Filtered_OutData = OutData.Where(o => item.RealStartCoordinate <= o.Realcoord && o.Realcoord <= item.RealFinalCoordinate).ToList();

                            if (Filtered_OutData.Count() == 0) continue;

                            var Filtered_Curves = curves.Where(o => item.RealStartCoordinate <= o.Start_Km && o.Final_Km <= item.RealFinalCoordinate).ToList();
                            if (Filtered_Curves.Count() == 0) continue;
                            //обнуляем участки кривых для рихтовки
                            foreach (var OD in Filtered_OutData)
                            {
                                foreach (var curve in Filtered_Curves)
                                {
                                    if (curve.RealStartCoordinate - 20.0 / 10000.0 <= OD.Realcoord && OD.Realcoord <= curve.RealFinalCoordinate + 20.0 / 10000.0)
                                    {
                                        OD.Stright_avg = 0.0;
                                        OD.Stright_left = 0.0;
                                        OD.Stright_right = 0.0;
                                        OD.Plan = 0.0;
                                    }
                                }
                            }

                            var Speed = item.Lastochka;
                            var l = 100;
                            //Длина интервала усреднения,  (м)
                            if (141 <= Speed && Speed <= 160)
                                l = 100;
                            else if (161 <= Speed && Speed <= 200)
                                l = 120;
                            else if (201 <= Speed && Speed <= 250)
                                l = 150;

                            //Скольз среднее  от l
                            for (int i = 0; i < Filtered_OutData.Count - l; i++)
                            {
                                var Range_L = Filtered_OutData.GetRange(i, l).ToList();

                                var Select_L = Range_L.Select(o => o.Stright_left).ToList();
                                var Avg_L = Select_L.Average();

                                //флуктуация вычисление от рихтовки
                                var prom = Filtered_OutData[i + l / 2].Stright_left - Avg_L;

                                var Select_L_profile = Range_L.Select(o => o.Y).ToList();
                                var Avg_L_profile = Select_L_profile.Average();

                                //флуктуация вычисление от профиля
                                var prom_profile = Filtered_OutData[i + l / 2].Y - Avg_L_profile;

                                Filtered_OutData[i + l / 2].FlukStr = prom;
                                Filtered_OutData[i + l / 2].FlukProfile = prom_profile;
                            }

                            //Вторая производная от Filtered_OutData
                            var Dx = 20.0;
                            var ANepPlan = new List<double> { }; // ускорение на длинных нер в плане 

                            var dlina = 0;
                            var dig = new List<RdProfile> { };

                            var ANepDigressions = new List<ANep> { };

                            for (int i = (int)(2 * Dx); i < Filtered_OutData.Count - (int)(Dx); i++)
                            {
                                //План
                                //var D = Filtered_OutData[i].FlukStr - 2.0 * Filtered_OutData[i - (int)(Dx)].FlukStr + Filtered_OutData[i - 2 * (int)(Dx)].FlukStr;
                                //var Dnep = 0.001 * ((Speed * Speed * D * D) / (Dx * Dx)) / (3.6 * 3.6);

                                var V2 = Math.Pow(Speed / 3.6, 2);
                                var Anep = 0.3 * ((V2 * Filtered_OutData[i].FlukStr + 0.000000001) / 17860.0);

                                if (Math.Abs(Anep) > 0.15)
                                {
                                    dlina += 1;
                                    dig.Add(Filtered_OutData[i]);
                                    ANepPlan.Add(Anep);
                                }
                                else if (dlina > 5)
                                {
                                    ANepDigressions.Add(new ANep
                                    {
                                        Km = dig[dig.Count / 2].Km,
                                        Meter = dig[dig.Count / 2].Meter,
                                        Plan = ANepPlan.Average() * Math.Sign(dig.Select(o => o.FlukStr).ToList()[dig.Count / 2]),
                                        Profile = -999,
                                        Len = dlina,
                                        Ampl = Math.Abs(dig.Select(o => Math.Abs(o.FlukStr)).Max())
                                    });
                                    //Console.WriteLine(
                                    //    $"Plan koord = { dig[dig.Count / 2].Km }.{ dig[dig.Count / 2].Meter } Plan = { ANepPlan.Average() * Math.Sign(dig.Select(o => o.Stright_left).ToList()[dig.Count / 2]) } Len = { dlina } Amp = { Math.Abs(dig.Select(o => o.Stright_left).Max()) * 20 }");
                                    dlina = 0;
                                    dig = new List<RdProfile> { };
                                    ANepPlan = new List<double> { };
                                }
                                else
                                {
                                    dlina = 0;
                                    ANepPlan = new List<double> { };
                                    dig = new List<RdProfile> { };
                                }
                            }

                            ANepPlan = new List<double> { }; // ускорение на длинных нер в профиле 
                            dlina = 0;
                            dig = new List<RdProfile> { };

                            for (int i = (int)(2 * Dx); i < Filtered_OutData.Count - (int)(Dx); i++)
                            {
                                //Профиль
                                var D = Filtered_OutData[i].FlukProfile - 2.0 * Filtered_OutData[i - (int)(Dx)].FlukProfile + Filtered_OutData[i - 2 * (int)(Dx)].FlukProfile;
                                var Dnep = 0.001 * ((Speed * Speed * D * D) / (Dx * Dx)) / (3.6 * 3.6);

                                if (Math.Abs(Dnep) > 0.18)
                                {
                                    dlina += 1;
                                    dig.Add(Filtered_OutData[i]);
                                    ANepPlan.Add(Dnep);
                                }
                                else if (dlina > 5)
                                {
                                    ANepDigressions.Add(new ANep
                                    {
                                        Km = dig[dig.Count / 2].Km,
                                        Meter = dig[dig.Count / 2].Meter,
                                        Plan = -999,
                                        Profile = ANepPlan.Average() * Math.Sign(dig.Select(o => o.FlukProfile).ToList()[dig.Count / 2]),
                                        Len = dlina,
                                        Ampl = Math.Abs(dig.Select(o => Math.Abs(o.FlukProfile)).Max())
                                    });
                                    //Console.WriteLine(
                                    //    $"Profile koord = { dig[dig.Count / 2].Km }.{ dig[dig.Count / 2].Meter } Plan = { ANepPlan.Average() * Math.Sign(dig.Select(o => o.Stright_left).ToList()[dig.Count / 2]) } Len = { dlina } Amp = { Math.Abs(dig.Select(o => o.Y).Average()) }");
                                    dlina = 0;
                                    dig = new List<RdProfile> { };
                                    ANepPlan = new List<double> { };
                                }
                                else
                                {
                                    dlina = 0;
                                    ANepPlan = new List<double> { };
                                    dig = new List<RdProfile> { };
                                }
                            }

                            ANepDigressions = ANepDigressions.OrderBy(o => o.Km + o.Meter / 10000.0).ToList();

                            var kmListDig = ANepDigressions.Select(o => o.Km).Distinct().ToList();

                            foreach (var km in kmListDig)
                            {
                                var tempData = ANepDigressions.Where(o => o.Km == km && o.Plan != -999).ToList();
                                var maxPlan = -999.0;
                                if (tempData.Select(o => Math.Abs(o.Plan)).ToList().Any())
                                    maxPlan = tempData.Select(o => Math.Abs(o.Plan)).ToList().Max();
                                var Plan = tempData.Where(o => Math.Abs(o.Plan) == maxPlan).ToList();

                                foreach (var Item in Plan)
                                {
                                    //баллы План
                                    var Wear = "";
                                    if (0.18 < Math.Abs(Item.Plan) && Math.Abs(Item.Plan) <= 0.25)
                                        Wear = "В";
                                    else if (0.25 < Math.Abs(Item.Plan))
                                        Wear = "П";
                                    //баллы Профиль
                                    if (0.15 < Math.Abs(Item.Profile) && Math.Abs(Item.Profile) <= 0.20)
                                    {
                                        Wear = Wear == "П" ? "П" : Wear == "В" ? "В" : "";
                                    }
                                    else if (0.20 < Math.Abs(Item.Profile))
                                    {
                                        Wear = "П";
                                    }

                                    //XElement Note = new XElement("note",
                                    //  new XAttribute("Km", Item.Km),
                                    //  new XAttribute("m", Item.Meter),
                                    //  new XAttribute("Vust", "Ласт " + Speed),
                                    //  new XAttribute("Maxbump", Item.Ampl.ToString("0")),
                                    //  new XAttribute("Sectionlength", Item.Len),
                                    //  new XAttribute("Plan", Item.Plan == -999 ? "-" : Item.Plan.ToString("0.00")),
                                    //  new XAttribute("Profil", Item.Profile == -999 ? "-" : Item.Profile.ToString("0.00")),
                                    //  new XAttribute("Ball", Wear == "П" ? "50" : "-"),
                                    //  new XAttribute("Wear", Wear));

                                    //xeTracks.Add(Note);
                                }
                                var tempDataProfile = ANepDigressions.Where(o => o.Km == km && o.Profile != -999).ToList();

                                var maxProfile = -999.0;
                                if (tempDataProfile.Select(o => Math.Abs(o.Plan)).ToList().Any())
                                    maxProfile = tempDataProfile.Select(o => Math.Abs(o.Plan)).ToList().Max();
                                var Profile = tempData.Where(o => Math.Abs(o.Plan) == maxProfile).ToList();
                                foreach (var Item in Profile)
                                {
                                    //Нормативы длинных неровностей в продольном профиле
                                    var bal = 0;
                                    var Wear = "";

                                    if (61 <= item.Passenger && item.Passenger <= 100)
                                    {
                                        if (40 <= Item.Ampl && Item.Ampl <= 50)
                                            Wear = Wear == "П" ? "П" : Wear == "В" ? "В" : "";
                                        else if (51 <= Item.Ampl && Item.Ampl <= 60)
                                            Wear = Wear == "П" ? "П" : Wear == "В" ? "В" : "";
                                        else if (60 < Item.Ampl)
                                            Wear = "П";

                                        //балл
                                        if (60 < Item.Ampl)
                                            bal = 50;
                                    }
                                    else if (100 > item.Passenger)
                                    {
                                        if (40 <= Item.Ampl && Item.Ampl <= 50)
                                            Wear = Wear == "П" ? "П" : Wear == "В" ? "В" : "";
                                        else if (51 <= Item.Ampl && Item.Ampl <= 60)
                                            Wear = "П";
                                        else if (60 < Item.Ampl)
                                            Wear = "П";

                                        //балл
                                        if (50 < Item.Ampl)
                                            bal = 50;
                                    }

                                    //XElement Note = new XElement("note",
                                    //  new XAttribute("Km", Item.Km),
                                    //  new XAttribute("m", Item.Meter),
                                    //  new XAttribute("Vust", "Ласт " + Speed),
                                    //  new XAttribute("Maxbump", Item.Ampl.ToString("0")),
                                    //  new XAttribute("Sectionlength", Item.Len),
                                    //  new XAttribute("Plan", Item.Plan == -999 ? "-" : Item.Plan.ToString("0.00")),
                                    //  new XAttribute("Profil", Item.Profile == -999 ? "-" : Item.Profile.ToString("0.00")),
                                    //  new XAttribute("Ball", Wear == "П" ? "50" : "-"),
                                    //  new XAttribute("Wear", Wear));

                                    //xeTracks.Add(Note);

                                    var TempCurve = curves.Where(o => o.RealStartCoordinate <= Item.RealCoord && Item.RealCoord <= o.RealFinalCoordinate).ToList();
                                    if (TempCurve.Any())
                                    {
                                        TempCurve.First().Elevations =
                                            (MainTrackStructureService.GetCurves(
                                                TempCurve.First().Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();
                                        TempCurve.First().Straightenings =
                                            (MainTrackStructureService.GetCurves(
                                                TempCurve.First().Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();

                                    }


                                    XElement xeMain = new XElement("main",
                                      new XAttribute("km", Item.Km),
                                      new XAttribute("m", Item.Meter),
                                      new XAttribute("amp", Item.Ampl.ToString("0")),
                                      new XAttribute("len", Item.Len),
                                      new XAttribute("vpz", item.Passenger + "/" + item.Freight),
                                      new XAttribute("primech", TempCurve.Any() ? $"Кривая R-{ TempCurve.First().Straightenings.First().Radius } м" : ""));

                                    xeTracks.Add(xeMain);

                                }
                            }
                        }


                        xeDirection.Add(xeTracks);
                        tripElem.Add(xeDirection);
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
                htReport.Save($@"G:\form\2.Характеристики положения пути в плане и профиле\20.Ведомость сверхнормативных длинных профильных неровностей.html");
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
public class ANep
{
    public int Km { get; set; }
    public int Meter { get; set; }
    public double Ampl { get; set; }
    public int Len { get; set; }
    public double Plan { get; set; }
    public double Profile { get; set; }
    public double RealCoord { get; set; }
}