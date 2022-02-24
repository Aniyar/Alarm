using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework;
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
using ElCurve = ALARm.Core.ElCurve;


namespace ALARm_Report.Forms
{
    public class Dfz2 : Report
    {
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
                int iter = 1;
                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {

                        var directName = AdditionalParametersService.DirectName(tripProcess.Id, (int)tripProcess.Direction);

                    var trackName = AdmStructureService.GetTrackName(track_id);

                    var gaps = AdditionalParametersService.RDGetGap(tripProcess.Trip_id, (int)tripProcess.Direction, 1);
                    if (gaps == null || gaps.Count == 0) { continue; }
                        gaps = gaps.Where(o => o.Km > 128).ToList();


                        XElement tripElem = new XElement("trip",
                        new XAttribute("direction", tripProcess.DirectionName + " (" + tripProcess.DirectionCode + ")"),
                        new XAttribute("date_statement", $"{ period.Period }    { tripProcess.GetProcessTypeName }"),
                        new XAttribute("nput", trackName),
                        new XAttribute("uchastok", $"{road}"),//todo
                        new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                        new XAttribute("road", road),
                        new XAttribute("periodDate", period.Period),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("ps", tripProcess.Car),
                        new XAttribute("pch", distance.Code),
                        new XAttribute("napr", directName.Count > 0 ? directName[0].Name.ToString() : ""),
                        new XAttribute("put", directName.Count > 0 ? directName[0].Put.ToString() : ""),
                        new XAttribute("trip_date", tripProcess.Trip_date)

                    );
                    XElement lev = new XElement("lev");

                    /// <summary>
                    /// Егер елемент келесі елемент(metr) қайталанса онда true болады
                    /// </summary>
                    var Flag = false;
                    int t = -2;

                    var speed = new List<Speed>();
                    var temperature = new List<Temperature>();
                    int previousGap = -1;
                    int PrevPut = -1, startKM = 0, finalKM = 0;
                        startKM = gaps[0].Km;
                     finalKM = gaps[gaps.Count - 1].Km;
                        foreach (var gap in gaps)
                    {
                        XElement Winter = new XElement("Winter");
                        XElement Summer = new XElement("Summer");
                        XElement Note = new XElement("Note");

                        //var hole_diameter = MainTrackStructureService.Diam(gap.Km, gap.Meter) as List<int>;
                        //if (hole_diameter.Count > 0 && hole_diameter.Min() != 36)
                        //{
                        //    continue;
                        //}
                        gap.R_zazor = gap.R_zazor < 0 ? 0 : (int)Math.Round(gap.R_zazor / 1.5) + 2;
                        gap.Zazor = gap.Zazor < 0 ? 0 : (int)Math.Round(gap.Zazor / 1.5) + 2;
                        gap.Zabeg = (int)Math.Round(gap.Zabeg / 1.5);

                        if ((previousGap == null) || (previousGap != gap.Km))
                        {
                            speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, "1") as List<Speed>;
                            temperature = MainTrackStructureService.GetTemp(tripProcess.Trip_id, 1, gap.Km) as List<Temperature>;

                            previousGap = gap.Km;
                        }
                        if (temperature.Count > 0)
                        {
                            if (temperature[0].Koridor > 0 || temperature[0].Koridor > 0)
                                continue;
                        }
                        gap.PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                        gap.FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;

                        var dig = gap.GetDigressions();
                        var dig2 = gap.GetDigressions2();

                        Note.Add(
                                    new XAttribute("lkm", gap.Km),
                                    new XAttribute("lm", gap.Meter),
                                    new XAttribute("lvelich", gap.Zazor),
                                    new XAttribute("lt", t),
                                    new XAttribute("lvpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                    new XAttribute("lvdp", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),

                                    new XAttribute("rkm", gap.Km),
                                    new XAttribute("rm", gap.Meter),
                                    new XAttribute("rvelich", gap.R_zazor),
                                    new XAttribute("rt", t),
                                    new XAttribute("rvpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                    new XAttribute("rvdp", (dig2.R_DigName == DigressionName.Gap || dig2.R_DigName == DigressionName.FusingGap || dig2.R_DigName == DigressionName.AnomalisticGap) ? dig2.R_AllowSpeed : "")
                                );
                        if (dig2.R_DigName == DigressionName.Gap || dig.DigName == DigressionName.Gap)
                        {
                            Winter.Add(Note);
                            iter++;
                        }

                           

                            Summer.Add(Note);
                            
                            tripElem.Add(Winter);
                        tripElem.Add(Summer);
                    }
                        tripElem.Add(new XAttribute("km", gaps.Count > 0 ? (startKM + " - " + finalKM) : "0"));
                        iter = 1;
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
        public override string ToString()
        {
            return "Отступления 2 степени, близкие к 3";
        }
    }

}