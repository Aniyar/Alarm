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
    public class StatementStateGaps : Report
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

                var distance =
                    AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road =
                    AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;

                var tripProcesses = RdStructureService.GetAdditionalParametersProcess(period);
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




                        var directName = AdditionalParametersService.DirectName(tripProcess.Id, (int)tripProcess.Direction);

                        XElement tripElem = new XElement("trip",
                            new XAttribute("date_statement", tripProcess.Date_Vrem.Date.ToShortDateString()),
                            new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                            new XAttribute("road", road.Name),
                            new XAttribute("distance", distance.Name),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("ps", tripProcess.Car),
                            new XAttribute("trip_date", tripProcess.Trip_date)
                        );
                        XElement Direct = new XElement("direction",
                            new XAttribute("name",
                                directName.Count == 0 ? "-/-/-" : directName[0].Name + " / Путь: " + directName[0].Put + " / ПЧ: " + directName[0].Pch)
                        );
                        var gaps = AdditionalParametersService.GetGap(tripProcess.Id, (int)tripProcess.Direction);
                        //var izostyk = AdditionalParametersService.Izostyk(45, 655, '01.04.2020');

                        /// <Flag>
                        /// Егер елемент келесі елемент(metr) қайталанса онда true болады
                        /// </Flag>
                        var Flag = false;


                        foreach (var gap in gaps)
                        {
                            XElement Notes = new XElement("Note");

                            var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, "1") as List<Speed>;
                            var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km, MainTrackStructureConst.MtoPdbSection, tripProcess.DirectionName, "1") as List<PdbSection>;
                            var fragment = "Станция1 - Станция2";
                            var temp = "20°";

                            gap.PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
                            gap.FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;
                            var dig = gap.GetDigressions();

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
                                new XAttribute("Vpz", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                new XAttribute("ZazorR", gap.Thread == 1 ? gap.Zazor.ToString() : "-"),
                                new XAttribute("ZazorL", gap.Thread == 1 ? "-" : gap.Zazor.ToString()),
                                new XAttribute("T", temp),//ToDo
                                new XAttribute("Zabeg", ""),
                                new XAttribute("Vdop", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),

                                new XAttribute("Otst", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.GetName() : ""),// Otst(speed.Count > 0 ? speed[0].ToString() : "-/-", gap.Zazor)),//ToDo
                                new XAttribute("fw", dig.DigName == DigressionName.Gap ? "bold" : "normal"),

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
                                new XAttribute("Zabeg", ""),
                                new XAttribute("Vdop", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),
                                new XAttribute("Otst", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.GetName() : ""),// Otst(speed.Count > 0 ? speed[0].ToString() : "-/-", gap.Zazor)),//ToDo
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
                                new XAttribute("Otst", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.GetName() : ""),
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