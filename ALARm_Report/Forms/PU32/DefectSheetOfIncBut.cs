using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class DefectSheetOfIncBut : Report
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
                    var directName = AdditionalParametersService.DirectName(tripProcess.Id, (int)tripProcess.Direction);

                    XElement tripElem = new XElement("trip",
                        new XAttribute("date_statement", tripProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
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
                    //var defISgaps = AdditionalParametersService.GetdefISGap(tripProcess.Trip_id);



                    /// <Flag>
                    ///// Егер елемент келесі елемент(metr) қайталанса онда true болады
                    /// </Flag>
                    var Flag = false;
                    var defISstat = false;

                    int t = -1;
                    foreach (var gap in gaps)
                    {
                        if (t < 0)
                        {
                            if ((int)gap.Zazor < 3 && (int)gap.Zazor > 18)
                            {
                                continue;
                            }
                        }
                        //foreach (var defIS in defISgaps)
                        //{
                        //    if (gap.Km == defIS.Km && gap.Meter == defIS.Meter) defISstat = true;
                        //}
                        if (defISstat == true)
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
                                new XAttribute("T", temp),//ToDo
                                new XAttribute("Zabeg", ""),
                                new XAttribute("Vdop", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),
                                new XAttribute("PlanPuti", Direction.Direct == tripProcess.Direction ? "Обратный" : "Прямой"),//ToDo
                                new XAttribute("Primech", "")//ToDo
                                );
                                iter++;
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
                                new XAttribute("T", temp),//ToDo
                                new XAttribute("Zabeg", ""),//ToDo
                                new XAttribute("PlanPuti", Direction.Direct == tripProcess.Direction ? "Обратный" : "Прямой"),//ToDo
                                new XAttribute("Vdop", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),
                                new XAttribute("Primech", "")//ToDo
                                );
                                iter++;
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
                                new XAttribute("ZazorL", nextGap.Thread == -1 ? nextGap.Zazor.ToString() : gap.Zazor.ToString()),
                                new XAttribute("T", temp),//ToDo
                                new XAttribute("Zabeg", (int)(Math.Abs(gap.Start - nextGap.Start) / 10)),
                                new XAttribute("Vdop", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),
                                new XAttribute("PlanPuti", Direction.Direct == tripProcess.Direction ? "Обратный" : "Прямой"),//ToDo
                                new XAttribute("Primech", "")//ToDo
                                );
                                iter++;
                                Flag = true;
                            }

                            Direct.Add(Notes);
                        }

                    }



                    //var gaps = AdditionalParametersService.GetGap(tripProcess.Id, (int)tripProcess.Direction);
                    //var defISgaps = AdditionalParametersService.GetdefISGap(tripProcess.Trip_id);



                    ///// <Flag>
                    ///// Егер елемент келесі елемент(metr) қайталанса онда true болады
                    ///// </Flag>
                    //var Flag = false;
                    //var defISstat = false;

                    //int t = -1;
                    //foreach (var gap in gaps)
                    //{
                    //    if (t < 0)
                    //    {
                    //        if ((int)gap.Zazor < 3 && (int)gap.Zazor > 18)
                    //        {
                    //            continue;
                    //        }
                    //    }
                    //    foreach (var defIS in defISgaps)
                    //    {
                    //        if (gap.Km == defIS.Km && gap.Meter == defIS.Meter) defISstat=true;
                    //    }
                    //    if (defISstat == true)
                    //    {
                    //        XElement Notes = new XElement("Note");

                    //        var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km,
                    //            MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, "1") as List<Speed>;
                    //        var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km,
                    //            MainTrackStructureConst.MtoPdbSection, tripProcess.DirectionName, "1") as List<PdbSection>;
                    //        var fragment = "Станция1 - Станция2";
                    //        var temp = "20°";

                    //        var nextGap =
                    //            gaps.ElementAt(gaps.IndexOf(gap)) == gaps.ElementAt(gaps.Count - 1) ? null : gaps.ElementAt(gaps.IndexOf(gap) + 1);

                    //        gap.PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                    //        gap.FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;
                    //        var dig = gap.GetDigressions();

                    //        if (nextGap != null && (gap.Meter != nextGap.Meter))
                    //        {
                    //            if (Flag == true)
                    //            {
                    //                Flag = false;
                    //                continue;
                    //            }

                    //            Notes.Add(
                    //            new XAttribute("n", iter),
                    //            new XAttribute("PPP", pdbSection.Count > 0 ? pdbSection[0].Pdb : "ПЧУ-/ПД-/ПДБ-"),
                    //            new XAttribute("PeregonStancia", fragment.ToString()),
                    //            new XAttribute("km", gap.Km),
                    //            new XAttribute("piket", gap.Picket),
                    //            new XAttribute("m", gap.Meter),
                    //            new XAttribute("Vpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                    //            new XAttribute("ZazorR", gap.Thread == 1 ? gap.Zazor.ToString() : "-"),
                    //            new XAttribute("ZazorL", gap.Thread == 1 ? "-" : gap.Zazor.ToString()),
                    //            new XAttribute("T", temp),//ToDo
                    //            new XAttribute("Zabeg", ""),
                    //            new XAttribute("Vdop", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),
                    //            new XAttribute("PlanPuti", Direction.Direct == tripProcess.Direction ? "Обратный" : "Прямой"),//ToDo
                    //            new XAttribute("Primech", "")//ToDo
                    //            );
                    //            iter++;
                    //        }
                    //        if (nextGap == null)
                    //        {
                    //            if (Flag == true)
                    //            {
                    //                Flag = false;
                    //                continue;
                    //            }

                    //            Notes.Add(
                    //            new XAttribute("n", iter),
                    //            new XAttribute("PPP", pdbSection.Count > 0 ? pdbSection[0].Pdb : "ПЧУ-/ПД-/ПДБ-"),
                    //            new XAttribute("PeregonStancia", fragment.ToString()),
                    //            new XAttribute("km", gap.Km),
                    //            new XAttribute("piket", gap.Picket),
                    //            new XAttribute("m", gap.Meter),
                    //            new XAttribute("Vpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                    //            new XAttribute("ZazorR", gap.Thread == 1 ? gap.Zazor.ToString() : "-"),
                    //            new XAttribute("ZazorL", gap.Thread == 1 ? "-" : gap.Zazor.ToString()),
                    //            new XAttribute("T", temp),//ToDo
                    //            new XAttribute("Zabeg", ""),//ToDo
                    //            new XAttribute("PlanPuti", Direction.Direct == tripProcess.Direction ? "Обратный" : "Прямой"),//ToDo
                    //            new XAttribute("Vdop", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),
                    //            new XAttribute("Primech", "")//ToDo
                    //            );
                    //            iter++;
                    //        }
                    //        if (nextGap != null && gap.Meter == nextGap.Meter)
                    //        {
                    //            Notes.Add(
                    //            new XAttribute("n", iter),
                    //            new XAttribute("PPP", pdbSection.Count > 0 ? pdbSection[0].Pdb : "ПЧУ-/ПД-/ПДБ-"),
                    //            new XAttribute("PeregonStancia", fragment.ToString()),
                    //            new XAttribute("km", gap.Km),
                    //            new XAttribute("piket", gap.Picket),
                    //            new XAttribute("m", gap.Meter),
                    //            new XAttribute("Vpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                    //            new XAttribute("ZazorR", gap.Thread == 1 ? gap.Zazor.ToString() : nextGap.Zazor.ToString()),
                    //            new XAttribute("ZazorL", nextGap.Thread == -1 ? nextGap.Zazor.ToString() : gap.Zazor.ToString()),
                    //            new XAttribute("T", temp),//ToDo
                    //            new XAttribute("Zabeg", (int)(Math.Abs(gap.Start - nextGap.Start) / 10)),
                    //            new XAttribute("Vdop", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),
                    //            new XAttribute("PlanPuti", Direction.Direct == tripProcess.Direction ? "Обратный" : "Прямой"),//ToDo
                    //            new XAttribute("Primech", "")//ToDo
                    //            );
                    //            iter++;
                    //            Flag = true;
                    //        }

                    //        Direct.Add(Notes);
                    //    }

                    //}
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
        public string Vdop(int number1)
        {
            if ((number1 > 24) && (number1 <= 26))
            {
                return "100 км/ч";
            }
            if ((number1 > 26) && (number1 <= 30))
            {
                return "60 км/ч";
            }
            if ((number1 > 30) && (number1 <= 35))
            {
                return "25 км/ч";
            }
            if (number1 > 35)
            {
                return "0/0";
                //return "движение закрывается";
            }
            return string.Empty;
        }
        private string Otst(int vPass, int number1, int number2 = -1)
        {
            if (vPass == -1)
            {
                return string.Empty;
            }
            if (((number1 > 24) && (number1 <= 26) || (number2 > 24) && (number2 <= 26)) && vPass > 100)
            {
                return "АРЗ";
            }
            if (((number1 > 26) && (number1 <= 30) || (number2 > 26) && (number2 <= 30)) && vPass > 60)
            {
                return "АРЗ";
            }
            if (((number1 > 30) && (number1 <= 35) || (number2 > 30) && (number2 <= 35)) && vPass > 25)
            {
                return "АРЗ";
            }
            if ((number1 == 0) || (number2 == 0))
            {
                return "СЗ";
            }
            if (Vdop(Math.Max(number1, number2)) != string.Empty)
            {
                return "З";
            }
            return string.Empty;
        }

    }

}