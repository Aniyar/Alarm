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
    public class sheetsOfPulseWeldedJoints : Report
    {
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
               // var tripProcesses = RdStructureService.GetAdditionalParametersProcess(period);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                int iter = 1;
                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    XElement tripElem = new XElement("trip",
                        new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                        new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                        new XAttribute("road", road.Name),
                        new XAttribute("distance", distance.Name),
                        new XAttribute("periodDate", period.Period),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("ps", tripProcess.Car),
                        new XAttribute("trip_date", tripProcess.Trip_date)
                    );
                    XElement Direct = new XElement("direction",
                        new XAttribute("name", "Шарташ-Устье Аха (20811), Путь: 1, ПЧ: 25 ")
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
                        var fragment = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km,
                            MainTrackStructureConst.Fragments, tripProcess.DirectionName, "1") as Fragment;

                        var nextGap =
                            gaps.ElementAt(gaps.IndexOf(gap)) == gaps.ElementAt(gaps.Count - 1) ? null : gaps.ElementAt(gaps.IndexOf(gap) + 1);

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
                            new XAttribute("ustSpeed", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                            new XAttribute("otst", ""),//ToDo
                            new XAttribute("kust", ""),//ToDo
                            new XAttribute("nit", gap.Thread == 1 ? "левая" : "правая"),//ToDo
                            new XAttribute("skreplenie", ""),//ToDo
                            new XAttribute("planPuti", Direction.Direct == tripProcess.Direction ? "Прямой" : "Обратный"),//ToDo
                            new XAttribute("shablon", ""),//ToDo
                            new XAttribute("ogrSpeed", ""),//ToDo
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
                            new XAttribute("ustSpeed", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                            new XAttribute("otst", ""),//ToDo
                            new XAttribute("kust", ""),//ToDo
                            new XAttribute("nit", gap.Thread == 1 ? "левая" : "правая"),//ToDo
                            new XAttribute("skreplenie", ""),//ToDo
                            new XAttribute("planPuti", Direction.Direct == tripProcess.Direction ? "Прямой" : "Обратный"),//ToDo
                            new XAttribute("shablon", ""),//ToDo
                            new XAttribute("ogrSpeed", ""),//ToDo
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
                            new XAttribute("ustSpeed", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                            new XAttribute("otst", ""),//ToDo
                            new XAttribute("kust", ""),//ToDo
                            new XAttribute("nit", gap.Thread == 1 ? "левая" : "правая"),//ToDo
                            new XAttribute("skreplenie", ""),//ToDo
                            new XAttribute("planPuti", Direction.Direct == tripProcess.Direction ? "Прямой" : "Обратный"),//ToDo
                            new XAttribute("shablon", ""),//ToDo
                            new XAttribute("ogrSpeed", ""),//ToDo
                            new XAttribute("Primech", "")//ToDo
                            );
                            iter++;
                            Flag = true;
                        }

                        Direct.Add(Notes);

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