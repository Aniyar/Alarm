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
    public class pu32dris : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            var filterForm = new FilterForm();
            var filters = new List<Filter>();
            filters.Add(new FloatFilter() { Name = "Порог. знач. Вертикального износа  (мм)", Value = 4.0f });
            filterForm.SetDataSource(filters);
            if (filterForm.ShowDialog() == DialogResult.Cancel)
                return;

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
                    XElement tripElem = new XElement("trip",
                        new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        new XAttribute("pch", distance.Code),
                        new XAttribute("check", tripProcess.GetProcessTypeName),
                        new XAttribute("dki", tripProcess.Car),
                        new XAttribute("road", road),
                        new XAttribute("chief",  tripProcess.Chief),
                        new XAttribute("mm", filters[0].Value), // минимальное пороговое значение 10мм
                        new XAttribute("trip_date", period.Period));

                    //var ListS3 = RdStructureService.GetDBD(tripProcess.Id) as List<S3>;

                    XElement lev = new XElement("lev",
                        new XAttribute("napr", tripProcess.DirectionName + " (" + tripProcess.DirectionCode + ")"),
                        new XAttribute("put", "1"),
                        new XAttribute("pch", distance.Code),
                        new XAttribute("trip_date", tripProcess.Date_Vrem.ToShortDateString()));

                    int SideWearLeft = 0;
                    int SideWearRight = 0; 
                    int VertIznosL = 0;
                    int VertIznosR = 0;
                    int gapi = 0;
                    int DownhillLeft = 0;
                    int DownhillRight = 0;
                    int TreadTiltLeft = 0;
                    int TreadTiltRight = 0;


                    var kilometers = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);


                    //var kilometers = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                    //if (kilometers.Count() == 0) continue;

                    //////Выбор километров по проезду-----------------
                    ////var filterForm = new FilterForm();
                    ////var filters = new List<Filter>();
                    //filters.Add(new FloatFilter() { Name = "Порог. знач. Вертикального износа  (мм)", Value = 4.0f });
                    ////filters.Add(new FloatFilter() { Name = "Начало (км)", Value = kilometers.Min() });
                    ////filters.Add(new FloatFilter() { Name = "Конец (км)", Value = kilometers.Max() });

                    //filterForm.SetDataSource(filters);
                    //if (filterForm.ShowDialog() == DialogResult.Cancel)
                    //    return;

                    //kilometers = kilometers.Where(o => ((float)(float)filters[0].Value <= o && o <= (float)(float)filters[1].Value)).ToList();
                    ////--------------------------------------------


                    progressBar.Maximum = kilometers.Count;

                    foreach (var kilometer in kilometers)
                    {
                        progressBar.Value = kilometers.IndexOf(kilometer) + 1;

                        var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer,
                            MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, "1") as List<Speed>;

                        //данные
                        var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(kilometer, tripProcess.Trip_id);
                        if (DBcrossRailProfile == null) continue;

                        var sortedData = DBcrossRailProfile.OrderByDescending(d => d.Meter).ToList();
                        var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(sortedData);

                        List<Digression> addDigressions = crossRailProfile.GetDigressions();
                        addDigressions = addDigressions.Where(o => o.Length > 1).ToList();

                        var gaps = AdditionalParametersService.GetGaps(tripProcess.Trip_id, (int)tripProcess.Direction, kilometer);
                        foreach (var gap in gaps)
                        {
                            gap.PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                            gap.FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;
                            addDigressions.Add(gap.GetDigressions());
                        }

                        addDigressions = addDigressions.OrderByDescending(o => o.Meter).ToList();
                        List<Curve> curves = MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>;

                        foreach (var digression in addDigressions)
                        {
                            if ((digression.DigName == DigressionName.SideWearLeft || digression.DigName == DigressionName.SideWearRight) && 
                                digression.Value >= (float)(float)filters[0].Value) continue;
                            
                            if (digression.DigName == DigressionName.Gap)
                            {
                                gapi += 1;
                                var vdop = digression.AllowSpeed.ToString() == "0/0" ? "0" : digression.AllowSpeed.ToString();

                                int Porog = -1;
                                var Vust = speed.Count > 0 ? speed[0].Passenger : -1;
                                switch (Vust)
                                {
                                    case int Value when Value > 100:
                                        Porog = 24;
                                        break;
                                    case int Value when Value > 60:
                                        Porog = 26;
                                        break;
                                    case int Value when Value > 25:
                                        Porog = 30;
                                        break;
                                    case int Value when Value > 15:
                                        Porog = 35;
                                        break;
                                }
                                XElement xeNote = new XElement("Note",
                                new XAttribute("Km", kilometer- digression.Velich),
                                new XAttribute("M", tripProcess.Direction == Direction.Reverse ? digression.Meter- digression.Length : 1000 - digression.Meter+ digression.Length),
                                new XAttribute("Otst", digression.GetName() + " " + (digression.Threat.ToString() == "1" ? "п." : "л.")),
                                new XAttribute("Velich", digression.Velich),
                                new XAttribute("Dlina", "-"),
                                new XAttribute("Porog", Porog),
                                new XAttribute("Vust", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                new XAttribute("Vdop", vdop),
                                  new XAttribute("itogoPCH", digression.Count)
                                );
                                lev.Add(xeNote);
                            }
                            if (((digression.DigName == DigressionName.DownhillLeft) || (digression.DigName == DigressionName.DownhillRight) ||
                                (digression.DigName == DigressionName.TreadTiltLeft) || (digression.DigName == DigressionName.TreadTiltRight) )&& digression.Length > 0 && (int)digression.Typ == 4)
                            {
                                switch (digression.DigName.Name)
                                {
                                    case string name when name == DigressionName.DownhillLeft.Name:
                                        DownhillLeft += 1;
                                        break;
                                    case string name when name == DigressionName.DownhillRight.Name:
                                        DownhillRight += 1;
                                        break;
                                    case string name when name == DigressionName.TreadTiltLeft.Name:
                                        TreadTiltLeft += 1;
                                        break;
                                    case string name when name == DigressionName.TreadTiltRight.Name:
                                        TreadTiltRight += 1;
                                        break;
                                }

                                XElement xeNote = new XElement("Note",
                                new XAttribute("Km", kilometer),
                                new XAttribute("M", tripProcess.Direction == Direction.Reverse ? digression.Meter - digression.Length : 1000 - digression.Meter + digression.Length),
                                new XAttribute("Otst", digression.GetName()),
                                new XAttribute("Velich", digression.Value > 0 ? ("1/" + ((int)(1 / digression.Value)).ToString()) : "     0" ),
                                new XAttribute("Dlina", digression.Length),
                                new XAttribute("Porog", (int)digression.Typ==3?"1/12": "1/12"),
                                new XAttribute("Vust", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                new XAttribute("Vdop", "-/-"),
                                 new XAttribute("itogoPCH", digression.Count)//to DOO ( сделать итого по пч)
                                );
                                lev.Add(xeNote);
                            }
                            if (((digression.DigName == DigressionName.SideWearLeft) || (digression.DigName == DigressionName.SideWearRight) ||
                                (digression.DigName == DigressionName.VertIznosL) || (digression.DigName == DigressionName.VertIznosR)) && digression.Length > 0)
                            {
                                switch (digression.DigName.Name)
                                {
                                    case string name when name == DigressionName.SideWearLeft.Name:
                                        SideWearLeft += 1;
                                        break;
                                    case string name when name == DigressionName.SideWearRight.Name:
                                        SideWearRight += 1;
                                        break;
                                    case string name when name == DigressionName.VertIznosL.Name:
                                        VertIznosL += 1;
                                        break;
                                    case string name when name == DigressionName.VertIznosR.Name:
                                        VertIznosR += 1;
                                        break;
                                }

                                //боковой износ огр ск
                                var Vogr = -1;
                                var Vust = speed.Count > 0 ? speed[0].Passenger : -1;

                                var Porog = -1;

                                if (digression.DigName == DigressionName.SideWearLeft || digression.DigName == DigressionName.SideWearRight)
                                {
                                    //if ((int)digression.Typ == 2) continue;

                                    foreach (var elem in curves)
                                    {
                                        if (kilometer >= elem.Start_Km && kilometer <= elem.Final_Km)
                                        {
                                            switch (digression.Value)
                                            {
                                                case float Value when Value >= 13.1 && Value <= 15:
                                                    Vogr = 140;
                                                    Porog = 13;
                                                    break;
                                                case float Value when Value >= 15.1 && Value <= 20 && elem.Radius > 350:
                                                    Vogr = 70;
                                                    Porog = 15;
                                                    break;
                                                case float Value when Value >= 15.1 && Value <= 20 && elem.Radius <= 350:
                                                    Vogr = 50;
                                                    Porog = 15;
                                                    break;
                                                case float Value when Value > 20:
                                                    Vogr = 50;
                                                    Porog = 20;
                                                    break;
                                            }

                                        }
                                    }
                                }
                                digression.Count  ++;
                                var vdop = (-1 != Vogr && Vogr < Vust) ? (Vogr + " / " + digression.Typ + "ст") : "-";
                                if (vdop == "-" && ((digression.DigName == DigressionName.SideWearLeft) || (digression.DigName == DigressionName.SideWearRight))) continue;

                                var dn = digression.GetName();

                                XElement xeNote = new XElement("Note",
                                new XAttribute("Km", kilometer),
                                new XAttribute("M", tripProcess.Direction == Direction.Reverse ? digression.Meter : 1000 - digression.Meter),
                                new XAttribute("Otst", dn+""),
                                new XAttribute("Velich", digression.Value.ToString("0.00")),
                                new XAttribute("Dlina", digression.Length),
                                new XAttribute("Porog", ((digression.DigName == DigressionName.VertIznosL) || (digression.DigName == DigressionName.VertIznosR)) ? "4":Porog.ToString()),
                                new XAttribute("Vust", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                new XAttribute("Vdop", vdop),
                                new XAttribute("itogoPCH", digression.Count)//to DOO
                                );
                                lev.Add(xeNote);
                            }
                        }
                    }
                    lev.Add(new XAttribute("vsego", SideWearLeft + SideWearRight + VertIznosL + VertIznosR + DownhillLeft + DownhillRight + TreadTiltLeft + TreadTiltRight + gapi)
                       );

                    //В том числе:
                    if (SideWearLeft > 0 || SideWearRight > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", $"Иб -  {SideWearLeft + SideWearRight}")));
                    }
                    if (VertIznosL > 0 || VertIznosR >0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", $"Ив - {VertIznosL+VertIznosR  } " )));
                    }
                    if (DownhillLeft > 0 || DownhillRight >0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", $"Пу - {DownhillLeft + DownhillRight}" )));
                    }
                    if (TreadTiltLeft > 0 || TreadTiltRight>0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", $"Нпк -  {TreadTiltRight + TreadTiltLeft}")));
                    }
                    if (gapi > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", "З - " + gapi)));
                    }
                   

                    tripElem.Add(
                       new XAttribute("final",
                           $@" В том числе: {(SideWearLeft > 0 ? $"Иб. л - {SideWearLeft}," : "")} {(SideWearRight > 0 ? $"Иб. пр - {SideWearRight}," : "")} {(VertIznosL > 0 ? $"Ив. л - {VertIznosL}," : "")} 
                                            {(VertIznosR > 0 ? $"Ив. пр - {VertIznosR}," : "")} {(DownhillLeft > 0 ? $"Пу. л - {DownhillLeft}," : "")} {(DownhillRight > 0 ? $"Пу. пр - {DownhillRight}," : "")}
                                            {(TreadTiltLeft > 0 ? $"Нпк. л - {TreadTiltLeft}," : "")} { (TreadTiltRight > 0 ? $"Нпк. пр - {TreadTiltRight}," : "")} 
                                            { (gapi > 0 ? $"З - {gapi}," : "")} 
                                            ")
                                            
                       );

                    tripElem.Add(lev);
                    //tripElem.Add(
                    //    new XAttribute("SideWearLeft", "Иб. л - " + SideWearLeft),
                    //    new XAttribute("SideWearRight", "Иб. пр - " + SideWearRight),
                    //    new XAttribute("VertIznosL", "Ив. л - " + VertIznosL),
                    //    new XAttribute("VertIznosR", "Ив. пр - " + VertIznosR),
                    //    new XAttribute("DownhillLeft", "Пу. л - " + DownhillLeft),
                    //    new XAttribute("DownhillRight", "Пу. пр - " + DownhillRight),
                    //    new XAttribute("TreadTiltLeft", "Нпк. л - " + TreadTiltLeft),
                    //    new XAttribute("TreadTiltRight", "Нпк. пр - " + TreadTiltRight),
                    //    new XAttribute("gapi", "З - " + gapi)
                    //    );
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
