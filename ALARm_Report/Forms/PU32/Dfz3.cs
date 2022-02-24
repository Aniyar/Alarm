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
    public class Dfz3 : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            //Сделать выбор периода
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(parentId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel) return;
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
                        var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                        if (kms.Count() == 0) continue;

                        var trackName = AdmStructureService.GetTrackName(track_id);

                        var directName = AdditionalParametersService.DirectName(tripProcess.Id, (int)tripProcess.Direction);
                        var gaps = AdditionalParametersService.RDGetGap(tripProcess.Trip_id, (int)tripProcess.Direction, 1);
                        if (gaps == null || gaps.Count == 0)
                        {
                            continue;
                        }

                        XElement tripElem = new XElement("trip",
                            new XAttribute("direction", tripProcess.DirectionName + " (" + tripProcess.DirectionCode + ")"),
                            new XAttribute("date_statement", tripProcess.Date_Vrem.Date.ToShortDateString()), 
                            new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                            new XAttribute("road", road), 
                            new XAttribute("track", trackName), 
                            new XAttribute("distance", distance.Code), 
                            new XAttribute("periodDate", period.Period), 
                            new XAttribute("chief", tripProcess.Chief), 
                            new XAttribute("ps", tripProcess.Car),
                            new XAttribute("km", kms.Min().ToString() + "-" + kms.Max().ToString()),
                            new XAttribute("trip_date", tripProcess.Trip_date)
                            );

                        XElement lev = new XElement("lev");

                        int boleeFirst = 0;
                        int boleeSecond = 0;
                        int boleeTherd = 0;
                        int boleeFourth = 0;

                        /// <Flag>
                        /// Егер елемент келесі елемент(metr) қайталанса онда true болады
                        /// </Flag>
                        var Flag = false;

                        var speed = new List<Speed>();
                        var temperature = new List<Temperature>();
                        int previousGap = -1;

                        foreach (var gap in gaps)
                        {
                            XElement Notes = new XElement("Note");

                            gap.R_zazor = gap.R_zazor == -999 ? -999 : gap.R_zazor == -1 ? 0 : (int)Math.Round(gap.R_zazor / 1.5);
                            gap.Zazor = gap.Zazor == -999 ? -999 : gap.Zazor == -1 ? 0 : (int)Math.Round(gap.Zazor / 1.5);
                            gap.Zabeg = gap.Zabeg == -999 ? -999 : (int)Math.Round(gap.Zabeg / 1.5);

                            if (gap.R_zazor > 50 || gap.Zazor > 50) 
                            {
                                continue;
                            }

                            if ((previousGap == null) || (previousGap != gap.Km))
                            {
                                speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, "1") as List<Speed>;
                                temperature = MainTrackStructureService.GetTemp(tripProcess.Trip_id, 1, gap.Km) as List<Temperature>;

                                previousGap = gap.Km;
                            }
                            gap.PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                            gap.FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;

                            var dig = gap.GetDigressions();
                            var dig2 = gap.GetDigressions2();

                            Notes.Add(
                                new XAttribute("lkm", gap.Km), 
                                new XAttribute("lm", gap.Meter), 
                                new XAttribute("lvelich", gap.Zazor == -999 ? "" : gap.Zazor.ToString()), 
                                new XAttribute("lt", (temperature.Count != 0 ? temperature[0].Koridor.ToString() : " ") + "°"), 
                                new XAttribute("lvpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"), 
                                new XAttribute("lvdp", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),

                                new XAttribute("rkm", gap.Km), 
                                new XAttribute("rm", gap.Meter), 
                                new XAttribute("rvelich", gap.R_zazor == -999 ? "" : gap.R_zazor.ToString()), 
                                new XAttribute("rt", (temperature.Count != 0 ? temperature[0].Kupe.ToString() : " ") + "°"), 
                                new XAttribute("rvpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"), 
                                new XAttribute("rvdp", (dig2.R_DigName == DigressionName.Gap || dig2.R_DigName == DigressionName.FusingGap || dig2.R_DigName == DigressionName.AnomalisticGap) ? dig2.R_AllowSpeed : "")
                                );

                            if (gap.Zazor > 24 && gap.Zazor <= 26) boleeFirst++;
                            if (gap.Zazor > 26 && gap.Zazor <= 30) boleeSecond++;
                            if (gap.Zazor > 30 && gap.Zazor <= 35) boleeTherd++;
                            if (gap.Zazor > 35) boleeFourth++;

                            if (gap.R_zazor > 24 && gap.R_zazor <= 26) boleeFirst++;
                            if (gap.R_zazor > 26 && gap.R_zazor <= 30) boleeSecond++;
                            if (gap.R_zazor > 30 && gap.R_zazor <= 35) boleeTherd++;
                            if (gap.R_zazor > 35) boleeFourth++;

                            if (dig2.R_DigName == DigressionName.Gap || dig.DigName == DigressionName.Gap || dig2.R_DigName == DigressionName.AnomalisticGap || dig.DigName == DigressionName.AnomalisticGap)
                            {
                                lev.Add(Notes);
                            }

                        }
                        lev.Add(
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