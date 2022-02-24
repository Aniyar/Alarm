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
    public class Dfz2LET : Report
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
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        var directName = AdditionalParametersService.DirectName(tripProcess.Id, (int)tripProcess.Direction);
                        var gaps = AdditionalParametersService.GetFusGap(tripProcess.Id, (int)tripProcess.Direction);
                      //  if (gaps == null || gaps.Count == 0) { continue; }
                        gaps = gaps.Where(o => o.Km > 128).ToList();


                        XElement tripElem = new XElement("trip",
                               new XAttribute("direction", tripProcess.DirectionName + " (" + tripProcess.DirectionCode + ")"),

                            new XAttribute("nput", trackName),
                            new XAttribute("uchastok", $"{road}"),//todo
                            new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                            new XAttribute("road", road),

                            new XAttribute("pch", distance.Code),


                            new XAttribute("date_statement", tripProcess.Date_Vrem.Date.ToShortDateString()),
                      
                           
                            new XAttribute("distance", distance.Name),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("ps", tripProcess.Car),
                            new XAttribute("trip_date", tripProcess.Trip_date)
                        );
                        XElement lev = new XElement("lev");

                        /// <summary>
                        /// Егер елемент келесі елемент(metr) қайталанса онда true болады
                        /// </summary>
                        var Flagl = false;
                        var Flagr = false;
                        int mtr = 0;
                        int kol = 0;
                        int nach_meter = 0;
                        int StartM = -1;
                        int StartKM = -1;
                        int FinalKM = -1;
                        var speed = new List<Speed>();
                        var temperature = new List<Temperature>();
                        int previousGap = -1;

                        
                        foreach (var gap in gaps)
                        {
                            //XElement Winter = new XElement("Winter");
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

                            var nextGap =
                                gaps.ElementAt(gaps.IndexOf(gap)) == gaps.ElementAt(gaps.Count - 1) ? null : gaps.ElementAt(gaps.IndexOf(gap) + 1);

                            var Next_dig = nextGap.GetDigressions();
                            var Next_dig2 = nextGap.GetDigressions2();

                            if (dig.DigName == Next_dig.DigName)
                            {
                                Flagl = true;
                                StartKM = dig.Kmetr;
                                StartM = dig.Meter;
                                mtr = mtr + Math.Abs(gap.Meter - nextGap.Meter);
                            }
                            //Note.Add(
                            //        new XAttribute("ldlina", mtr),
                            //        new XAttribute("lt", (temperature.Count != 0 ? temperature[0].Koridor.ToString() : " ") + "°"),
                            //        new XAttribute("lNzazor", kol),
                            //        new XAttribute("lm", dig.Meter),
                            //        new XAttribute("lkm", dig.Kmetr),
                            //        new XAttribute("rkm", "-"),
                            //        new XAttribute("rm", "-"),
                            //        new XAttribute("rdlina", "-"),
                            //        new XAttribute("rt", "-"),
                            //        new XAttribute("rNzazor", "-")
                            //    );

                            //if (gap.Thread == -1 && nextGap.Thread == -1)
                            //{
                            //    if (gap.Km == nextGap.Km)
                            //    {
                            //        if (Math.Abs(gap.Meter - nextGap.Meter) >= 25)
                            //        {
                            //            if (km == -1) nach_meter = gap.Meter;
                            //            mtr = mtr + Math.Abs(gap.Meter - nextGap.Meter);
                            //            kol = kol + 1;
                            //            Flagl = true;
                            //            km += 1;
                            //            startm = startm == -1 ? gap.Meter : startm;
                            //        }
                            //        else
                            //        {
                            //            if (mtr != 0 && kol >= 3 && Flagl == true)
                            //            {
                            //                Note.Add(
                            //                    new XAttribute("ldlina", mtr),
                            //                    new XAttribute("lt", "20"),
                            //                    new XAttribute("lNzazor", kol),
                            //                    new XAttribute("lm", nach_meter),
                            //                    new XAttribute("lkm", gap.Km),
                            //                    new XAttribute("rkm", "-"),
                            //                    new XAttribute("rm", "-"),
                            //                    new XAttribute("rdlina", "-"),
                            //                    new XAttribute("rt", "-"),
                            //                    new XAttribute("rNzazor", "-")
                            //                );
                            //                mtr = 0;
                            //                kol = 0;
                            //                Flagl = false;
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (mtr != 0 && kol >= 3 && Flagl == true)
                            //        {
                            //            finishm = finishm == -1 ? gap.Meter : finishm;

                            //            Note.Add(
                            //                new XAttribute("ldlina", Math.Abs(finishm - startm)),
                            //                new XAttribute("lt", "20"),
                            //                new XAttribute("lNzazor", kol),
                            //                new XAttribute("lm", nach_meter),
                            //                new XAttribute("lkm", gap.Km),
                            //                new XAttribute("rkm", "-"),
                            //                new XAttribute("rm", "-"),
                            //                new XAttribute("rdlina", "-"),
                            //                new XAttribute("rt", "-"),
                            //                new XAttribute("rNzazor", "-")
                            //            );
                            //            mtr = 0;
                            //            kol = 0;
                            //            Flagl = false;

                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    if (mtr != 0 && kol >= 3 && Flagl == true)
                            //    {
                            //        Note.Add(
                            //            new XAttribute("ldlina", mtr),
                            //            new XAttribute("lt", "20"),
                            //            new XAttribute("lNzazor", kol),
                            //            new XAttribute("lm", nach_meter),
                            //            new XAttribute("lkm", gap.Km),
                            //            new XAttribute("rkm", "-"),
                            //            new XAttribute("rm", "-"),
                            //            new XAttribute("rdlina", "-"),
                            //            new XAttribute("rt", "-"),
                            //            new XAttribute("rNzazor", "-")
                            //        );
                            //        mtr = 0;
                            //        kol = 0;
                            //        Flagl = false;
                            //    }
                            //}
                            //-------------------------------------------------------------------------------------------
                            //if (gap.Thread == 1 && nextGap.Thread == 1)
                            //{
                            //    if (gap.Km == nextGap.Km)
                            //    {
                            //        if (Math.Abs(gap.Meter - nextGap.Meter) >= 25)
                            //        {
                            //            Note.Add(
                            //                new XAttribute("rm", gap.Meter),
                            //                new XAttribute("rkm", gap.Km),
                            //                   new XAttribute("lkm", "-"),
                            //                   new XAttribute("lm", "-"),
                            //                   new XAttribute("ldlina", "-"),
                            //                   new XAttribute("lt", "-"),
                            //                   new XAttribute("lNzazor", "-")
                            //                );
                            //            mtr = mtr + Math.Abs(gap.Meter - nextGap.Meter);
                            //            kol = kol + 1;
                            //            Flagr = true;
                            //        }
                            //        else
                            //        {
                            //            if (mtr != 0 && kol >= 3 && Flagr == true)
                            //            {
                            //                Note.Add(
                            //                new XAttribute("rdlina", mtr),
                            //                new XAttribute("rt", "20"),
                            //                new XAttribute("rNzazor", kol)
                            //                );
                            //                mtr = 0;
                            //                kol = 0;
                            //                Flagr = false;
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (mtr != 0 && kol >= 3 && Flagr == true)
                            //        {
                            //            Note.Add(
                            //            new XAttribute("rdlina", mtr),
                            //            new XAttribute("rt", "20"),
                            //            new XAttribute("rNzazor", kol)
                            //            );
                            //            mtr = 0;
                            //            kol = 0;
                            //            Flagr = false;
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    if (mtr != 0 && kol >= 3 && Flagr == true)
                            //    {
                            //        Note.Add(
                            //        new XAttribute("rdlina", mtr),
                            //        new XAttribute("rt", "20"),
                            //        new XAttribute("rNzazor", kol)
                            //        );
                            //        mtr = 0;
                            //        kol = 0;
                            //        Flagr = false;
                            //    }
                            //}
                            //{
                            //    mtr = mtr + Math.Abs(gap.Meter - nextGap.Meter);
                            //    Flag = true; 
                            //}
                            //if (Flag == false)
                            //{



                            //if (nextGap != null && (gap.Meter != nextGap.Meter))
                            //{
                            //    if (Flag == true)
                            //    {
                            //        Flag = false;
                            //        continue;
                            //    }
                            //    if (gap.Thread == -1)
                            //    {
                            //        Note.Add(
                            //            new XAttribute("lkm", gap.Km),
                            //            new XAttribute("lm", gap.Meter),
                            //            new XAttribute("lvelich", gap.Zazor),
                            //            new XAttribute("lt", "20"),
                            //            new XAttribute("lvpz", speed.Count > 0 ? speed[0].Passenger.ToString() : "-"),
                            //            new XAttribute("lvdp", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),

                            //            new XAttribute("rkm", "-"),
                            //            new XAttribute("rm", "-"),
                            //            new XAttribute("rvelich", "-"),
                            //            new XAttribute("rt", "-"),
                            //            new XAttribute("rvpz", "-"),
                            //            new XAttribute("rvdp", "-")
                            //        );
                            //    }
                            //    if (gap.Thread == 1)
                            //    {
                            //        Note.Add(
                            //            new XAttribute("lkm", "-"),
                            //            new XAttribute("lm", "-"),
                            //            new XAttribute("lvelich", "-"),
                            //            new XAttribute("lt", "-"),
                            //            new XAttribute("lvpz", "-"),
                            //            new XAttribute("lvdp", "-"),

                            //            new XAttribute("rkm", gap.Km),
                            //            new XAttribute("rm", gap.Meter),
                            //            new XAttribute("rvelich", gap.Zazor),
                            //            new XAttribute("rt", "20"),
                            //            new XAttribute("rvpz", speed.Count > 0 ? speed[0].Passenger.ToString() : "-"),
                            //            new XAttribute("rvdp", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : "")
                            //        );
                            //    }
                            //    if (dig.GetName() == "З")
                            //    {
                            //        Winter.Add(Note);
                            //        iter++;
                            //    }
                            //}
                            //if (nextGap == null)
                            //{
                            //    if (Flag == true)
                            //    {
                            //        Flag = false;
                            //        continue;
                            //    }

                            //    if (gap.Thread == -1)
                            //    {
                            //        Note.Add(
                            //            new XAttribute("lkm", gap.Km),
                            //            new XAttribute("lm", gap.Meter),
                            //            new XAttribute("lvelich", gap.Zazor),
                            //            new XAttribute("lt", "20"),
                            //            new XAttribute("lvpz", speed.Count > 0 ? speed[0].Passenger.ToString() : "-"),
                            //            new XAttribute("lvdp", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),

                            //            new XAttribute("rkm", "-"),
                            //            new XAttribute("rm", "-"),
                            //            new XAttribute("rvelich", "-"),
                            //            new XAttribute("rt", "-"),
                            //            new XAttribute("rvpz", "-"),
                            //            new XAttribute("rvdp", "-")
                            //        );
                            //    }
                            //    if (gap.Thread == 1)
                            //    {
                            //        Note.Add(
                            //            new XAttribute("lkm", "-"),
                            //            new XAttribute("lm", "-"),
                            //            new XAttribute("lvelich", "-"),
                            //            new XAttribute("lt", "-"),
                            //            new XAttribute("lvpz", "-"),
                            //            new XAttribute("lvdp", "-"),

                            //            new XAttribute("rkm", gap.Km),
                            //            new XAttribute("rm", gap.Meter),
                            //            new XAttribute("rvelich", gap.Zazor),
                            //            new XAttribute("rt", "20"),
                            //            new XAttribute("rvpz", speed.Count > 0 ? speed[0].Passenger.ToString() : "-"),
                            //            new XAttribute("rvdp", (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : "")
                            //        );
                            //    }
                            //    if (dig.GetName() == "З")
                            //    {
                            //        Winter.Add(Note);
                            //        iter++;
                            //    }
                            //}

                            //if (nextGap != null && gap.Meter == nextGap.Meter)
                            //{
                            //    Note.Add(
                            //        new XAttribute("lkm", gap.Thread == -1 ? gap.Km : nextGap.Km),
                            //        new XAttribute("lm", gap.Meter),
                            //        new XAttribute("lvelich", gap.Thread == -1 ? gap.Zazor : nextGap.Zazor),
                            //        new XAttribute("lt", "20"),
                            //        new XAttribute("lcount", speed.Count > 0 ? speed[0].Passenger.ToString() : "-"),
                            //        //new XAttribute("lvdp",
                            //        //    (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : ""),

                            //        new XAttribute("rkm", nextGap.Thread == 1 ? nextGap.Km : gap.Km),
                            //        new XAttribute("rm", nextGap.Thread == 1 ? nextGap.Meter : gap.Meter),
                            //        new XAttribute("rvelich", nextGap.Thread == 1 ? nextGap.Zazor : gap.Zazor),
                            //        new XAttribute("rt", "20"),
                            //        new XAttribute("rcount", speed.Count > 0 ? speed[0].Passenger.ToString() : "-")
                            //        //new XAttribute("rvdp",
                            //        //    (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.AllowSpeed : "")
                            //    );
                            //    if (dig.GetName() == "З")
                            //    {
                            //        Winter.Add(Note);
                            //        iter++;
                            //    }
                            //    Flag = true;
                            //}
                            //Winter.Add(Note);
                            Summer.Add(Note);
                            //tripElem.Add(Winter);
                            tripElem.Add(Summer);

                        }
                        tripElem.Add(new XAttribute("km", gaps.Count > 0 ? (StartKM + " - " + FinalKM) : "0"));
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