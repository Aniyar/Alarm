using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
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
    public class StatementStateGapsSpeedLimit : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var tripProcesses = //RdStructureService.GetAdditionalParametersProcess(period);
                                    RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                int iter = 1;
                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    var directName = AdditionalParametersService.DirectName(tripProcess.Id, (int)tripProcess.Direction);

                    XElement tripElem = new XElement("trip",
                        new XAttribute("date_statement", tripProcess.Date_Vrem.Date),
                        new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                        new XAttribute("road", road),
                        new XAttribute("distance", distance.Name),
                        new XAttribute("periodDate", period.Period),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("ps", tripProcess.Car),
                        new XAttribute("trip_date", tripProcess.Trip_date)
                    );
                    XElement Direct = new XElement("direction",
                        new XAttribute("name",
                            directName.Count == 0 ? "-/-/-" : directName[0].Name + "/Путь: " + directName[0].Put + "/ПЧ: " + directName[0].Pch)
                    );
                    var gaps = AdditionalParametersService.GetGap(tripProcess.Id, (int)tripProcess.Direction);

                    /// <Flag>
                    /// Егер елемент келесі елемент(metr) қайталанса онда true болады
                    /// </Flag>
                    var Flag = false;


                    foreach (var gap in gaps)
                    {
                        XElement Notes = new XElement("Note");

                        var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km,
                            MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, "1") as List<Speed>;
                        var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km,
                            MainTrackStructureConst.MtoPdbSection, tripProcess.DirectionName, "1") as List<PdbSection>;
                        var fragment = "Станция1 - Станция2";
                        var temp = "20°";

                        var nextGap =
                            gaps.ElementAt(gaps.IndexOf(gap)) == gaps.ElementAt(gaps.Count - 1) ? null : gaps.ElementAt(gaps.IndexOf(gap) + 1);

                        gap.PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                        gap.FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;
                        var dig = gap.GetDigressions();

                        if (nextGap != null && (gap.Meter != nextGap.Meter))
                        {
                            if (Flag == true)
                            {
                                Flag = false;
                                continue;
                            }
                            Notes.Add(
                            new XAttribute("n", iter),
                            new XAttribute("PPP", pdbSection.Count > 0 ? pdbSection[0].Pdb : "ПЧУ-/ПД-/ПДБ-"),
                            new XAttribute("PeregonStancia", fragment.ToString()),
                            new XAttribute("km", gap.Km),
                            new XAttribute("piket", gap.Picket),
                            new XAttribute("m", gap.Meter),
                            new XAttribute("Vpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                            new XAttribute("ZazorR", gap.Thread == 1 ? gap.Zazor.ToString() : "-"),
                            new XAttribute("ZazorL", gap.Thread == 1 ? "-" : gap.Zazor.ToString()),
                            new XAttribute("nit", gap.Thread == 1 ? "правая" : "левая"),
                            new XAttribute("T", temp),//ToDo
                            new XAttribute("Zabeg", ""),
                            new XAttribute("Vdop", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),
                            new XAttribute("Otst", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.GetName() : ""),
                            new XAttribute("Primech", "")//ToDo
                            );

                            if (dig.GetName() == "З")
                            {
                                Direct.Add(Notes);
                                iter++;
                            }
                        }
                        if (nextGap == null)
                        {
                            if (Flag == true)
                            {
                                Flag = false;
                                continue;
                            }

                            Notes.Add(
                            new XAttribute("n", iter),
                            new XAttribute("PPP", pdbSection.Count > 0 ? pdbSection[0].Pdb : "ПЧУ-/ПД-/ПДБ-"),
                            new XAttribute("PeregonStancia", fragment.ToString()),
                            new XAttribute("km", gap.Km),
                            new XAttribute("piket", gap.Picket),
                            new XAttribute("m", gap.Meter),
                            new XAttribute("Vpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                            new XAttribute("ZazorR", gap.Thread == 1 ? gap.Zazor.ToString() : "-"),
                            new XAttribute("ZazorL", gap.Thread == 1 ? "-" : gap.Zazor.ToString()),
                            new XAttribute("nit", gap.Thread == 1 ? "правая" : "левая"),
                            new XAttribute("T", temp),//ToDo
                            new XAttribute("Zabeg", ""),
                            new XAttribute("Vdop", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),
                            new XAttribute("Otst", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.GetName() : ""),// Otst(speed.Count > 0 ? speed[0].ToString() : "-/-", gap.Zazor)),//ToDo
                            new XAttribute("Primech", "")//ToDo
                            );

                            if (dig.GetName() == "З")
                            {
                                iter++;
                                Direct.Add(Notes);
                            }
                        }
                        if (nextGap != null && gap.Meter == nextGap.Meter)
                        {
                            Notes.Add(
                            new XAttribute("n", iter),
                            new XAttribute("PPP", pdbSection.Count > 0 ? pdbSection[0].Pdb : "ПЧУ-/ПД-/ПДБ-"),
                            new XAttribute("PeregonStancia", fragment.ToString()),
                            new XAttribute("km", gap.Km),
                            new XAttribute("piket", gap.Picket),
                            new XAttribute("m", gap.Meter),
                            new XAttribute("Vpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                            new XAttribute("ZazorR", gap.Thread == 1 ? gap.Zazor.ToString() : nextGap.Zazor.ToString()),
                            new XAttribute("nit", gap.Thread == 1 ? "правая" : "левая"),
                            new XAttribute("ZazorL", nextGap.Thread == -1 ? nextGap.Zazor.ToString() : gap.Zazor.ToString()),
                            new XAttribute("T", temp),//ToDo
                            new XAttribute("Zabeg", (int)(Math.Abs(gap.Start - nextGap.Start) / 10)),
                            new XAttribute("Vdop", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),
                            new XAttribute("Otst", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.GetName() : ""),
                            new XAttribute("Primech", "")//ToDo
                            );

                            Flag = true;
                            if (dig.GetName() == "З")
                            {
                                Direct.Add(Notes);
                                iter++;
                            }
                        }
                    }
                    iter = 1;
                    tripElem.Add(Direct);
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
        public override string ToString()
        {
            return "Отступления 2 степени, близкие к 3";
        }
    }

}