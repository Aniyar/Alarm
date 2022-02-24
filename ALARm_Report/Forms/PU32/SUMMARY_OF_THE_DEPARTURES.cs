using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.Services;
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
    public class SUMMARY_OF_THE_DEPARTURES : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();

                var distance =AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                // var tripProcesses = RdStructureService.GetAdditionalParametersProcess(period);
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
                        new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                        new XAttribute("road", road),
                        new XAttribute("distance", distance.Name),
                        new XAttribute("periodDate", period.Period),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("ps", tripProcess.Car),
                        new XAttribute("date_statement", tripProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                         new XAttribute("info", tripProcess.Car + " " + " " + " " + " " + " " + tripProcess.Chief)
                        );


                    //  для скрелепний  

                    //var Check_Total_fastening = AdditionalParametersService.Check_Total_state_fastening(tripProcess.Trip_id, template.ID);
                    List<Digression> Check_Total_fastening = AdditionalParametersService.Check_deviationsinfastening_state(tripProcess.Trip_id, template.ID);
                    var fastening_check_km = Check_Total_fastening.GroupBy(O => O.Km).ToList().Count;

                    var fastening_vdop_0 = 0;
                    var fastening_vdop_15 = 0;
                    var fastening_vdop_25 = 0;
                    var fastening_vdop_40 = 0;
                    var fastening_vdop_60 = 0;

                    foreach (var otst in Check_Total_fastening.Where(O => O.Otst != "").ToList())
                    {
                        if (otst.Vdop != "")
                        {
                            var speed = otst.Vdop.Split('/');

                            if ((Int32.Parse(speed[0]) == 0 && Int32.Parse(speed[1]) == 0) || (Int32.Parse(speed[0]) == 0 || Int32.Parse(speed[1]) == 0))
                                fastening_vdop_0++;
                            if ((Int32.Parse(speed[0]) == 15 && Int32.Parse(speed[1]) == 15) || (Int32.Parse(speed[0]) == 15 || Int32.Parse(speed[1]) == 15))
                                fastening_vdop_15++;
                            if ((Int32.Parse(speed[0]) == 25 && Int32.Parse(speed[1]) == 25) || (Int32.Parse(speed[0]) == 25 || Int32.Parse(speed[1]) == 25))
                                fastening_vdop_25++;
                            if ((Int32.Parse(speed[0]) == 40 && Int32.Parse(speed[1]) == 40) || (Int32.Parse(speed[0]) == 40 || Int32.Parse(speed[1]) == 40))
                                fastening_vdop_40++;
                            if ((Int32.Parse(speed[0]) >= 60 && Int32.Parse(speed[1]) >= 60) || (Int32.Parse(speed[0]) >= 60 || Int32.Parse(speed[1]) >= 60))
                                fastening_vdop_60++;
                        }
                    }

                    var fastening_no_speedlimit = Check_Total_fastening.Where(O => O.Otst != "").ToList().Where(O => O.Vdop == "").ToList().Count;
                    var fastening_All = Check_Total_fastening.Where(O => O.Otst != "").ToList().Count;

                    //

                    //для Отступления в содержании рельсовых стыков //To doo
                    List<Digression> Check_Total_gaps = AdditionalParametersService.Check_sleepers_state(tripProcess.Trip_id, template.ID);
                    //var Check_Total_gaps = AdditionalParametersService.Check_Total_state_digression(tripProcess.Trip_id, template.ID);
                    var gap_check_km = Check_Total_gaps.GroupBy(O => O.Km).ToList().Count;

                    var gap_vdop_0 = 0;
                    var gap_vdop_15 = 0;
                    var gap_vdop_25= 0;
                    var gap_vdop_40 = 0;
                    var gap_vdop_60 = 0;

                    foreach (var otst in Check_Total_gaps.Where(O => O.Otst != "").ToList())
                    {
                        if (otst.Vdop != "")
                        {
                           var speed = otst.Vdop.Split('/');

                            if ((Int32.Parse(speed[0]) == 0 && Int32.Parse(speed[1]) == 0) || (Int32.Parse(speed[0]) == 0 || Int32.Parse(speed[1]) == 0))
                                gap_vdop_0++;
                            if ((Int32.Parse(speed[0]) == 15 && Int32.Parse(speed[1]) == 15) || (Int32.Parse(speed[0]) == 15 || Int32.Parse(speed[1]) == 15))
                                gap_vdop_15++;
                            if ((Int32.Parse(speed[0]) == 25 && Int32.Parse(speed[1]) == 25) || (Int32.Parse(speed[0]) == 25 || Int32.Parse(speed[1]) == 25))
                                gap_vdop_25++;
                            if ((Int32.Parse(speed[0]) == 40 && Int32.Parse(speed[1]) == 40) || (Int32.Parse(speed[0]) == 40 || Int32.Parse(speed[1]) == 40))
                                gap_vdop_40++;
                            if ((Int32.Parse(speed[0]) >= 60 && Int32.Parse(speed[1]) >= 60) || (Int32.Parse(speed[0]) >= 60 || Int32.Parse(speed[1]) >= 60))
                                gap_vdop_60++;
                        }
                    }

                    //var gap_no_speedlimit = Check_Total_gaps.Where(O => O.Otst != "").ToList().Where(O => O.Vdop == "").ToList().Count;
                    //var gap_All = Check_Total_gaps.Where(O => O.Otst != "").ToList().Count;
                    var gap_no_speedlimit = 0;
                    var gap_All = 0;

                    //


                    //для шпал
                    var Check_Total_sleepers = AdditionalParametersService.Check_sleepers_state(tripProcess.Trip_id, template.ID);
                    var sleepers_check_km = Check_Total_sleepers.GroupBy(O => O.Km).ToList().Count;
                    //report_defshpal
                    var sleepers_vdop_0 = 0;
                    var sleepers_vdop_15 = 0;
                    var sleepers_vdop_25 = 0;
                    var sleepers_vdop_40 = 0;
                    var sleepers_vdop_60 = 0;

                    foreach (var otst in Check_Total_sleepers.Where(O => O.Otst != "").ToList())
                    {
                        if (otst.Vdop != "")
                        {
                            var speed = otst.Vdop.Split('/');

                            if ((Int32.Parse(speed[0]) == 0 && Int32.Parse(speed[1]) == 0) || (Int32.Parse(speed[0]) == 0 || Int32.Parse(speed[1]) == 0))
                                sleepers_vdop_0++;
                            if ((Int32.Parse(speed[0]) == 15 && Int32.Parse(speed[1]) == 15) || (Int32.Parse(speed[0]) == 15 || Int32.Parse(speed[1]) == 15))
                                sleepers_vdop_15++;
                            if ((Int32.Parse(speed[0]) == 25 && Int32.Parse(speed[1]) == 25) || (Int32.Parse(speed[0]) == 25 || Int32.Parse(speed[1]) == 25))
                                sleepers_vdop_25++;
                            if ((Int32.Parse(speed[0]) == 40 && Int32.Parse(speed[1]) == 40) || (Int32.Parse(speed[0]) == 40 || Int32.Parse(speed[1]) == 40))
                                sleepers_vdop_40++;
                            if ((Int32.Parse(speed[0]) >= 60 && Int32.Parse(speed[1]) >= 60) || (Int32.Parse(speed[0]) >= 60 || Int32.Parse(speed[1]) >= 60))
                                sleepers_vdop_60++;
                        }
                    }

                    var sleepers_no_speedlimit = Check_Total_sleepers.Where(O => O.Otst != "").ToList().Where(O => O.Vdop == "").ToList().Count;
                    var sleepers_All = Check_Total_sleepers.Where(O => O.Otst != "").ToList().Count;

                    //

                    //для рельсов//to doo
                    var Check_Total_ralils= AdditionalParametersService.Check_Total_state_rails(tripProcess.Trip_id, template.ID);
                    var ralils_check_km = Check_Total_gaps.GroupBy(O => O.Km).ToList().Count;

                    var ralils_vdop_0 = 0;
                    var ralils_vdop_15 = 0;
                    var ralils_vdop_25 = 0;
                    var ralils_vdop_40 = 0;
                    var ralils_vdop_60 = 0;

                    foreach (var otst in Check_Total_gaps.Where(O => O.Otst != "").ToList())
                    {
                        if (otst.Vdop != "")
                        {
                            var speed = otst.Vdop.Split('/');

                            if ((Int32.Parse(speed[0]) == 0 && Int32.Parse(speed[1]) == 0) || (Int32.Parse(speed[0]) == 0 || Int32.Parse(speed[1]) == 0))
                                ralils_vdop_0++;
                            if ((Int32.Parse(speed[0]) == 15 && Int32.Parse(speed[1]) == 15) || (Int32.Parse(speed[0]) == 15 || Int32.Parse(speed[1]) == 15))
                                ralils_vdop_15++;
                            if ((Int32.Parse(speed[0]) == 25 && Int32.Parse(speed[1]) == 25) || (Int32.Parse(speed[0]) == 25 || Int32.Parse(speed[1]) == 25))
                                ralils_vdop_25++;
                            if ((Int32.Parse(speed[0]) == 40 && Int32.Parse(speed[1]) == 40) || (Int32.Parse(speed[0]) == 40 || Int32.Parse(speed[1]) == 40))
                                ralils_vdop_40++;
                            if ((Int32.Parse(speed[0]) >= 60 && Int32.Parse(speed[1]) >= 60) || (Int32.Parse(speed[0]) >= 60 || Int32.Parse(speed[1]) >= 60))
                                ralils_vdop_60++;
                        }
                    }


                    var ralils_no_speedlimit = Check_Total_gaps.Where(O => O.Otst != "").ToList().Where(O => O.Vdop == "").ToList().Count;
                    var ralils_All = Check_Total_gaps.Where(O => O.Otst != "").ToList().Count;
                    //var ralils_no_speedlimit = 0;
                    //var ralils_All = 0;

                    //
                    //для баласта //to do
                    var Check_Total_balast = AdditionalParametersService.Check_Total_balast(tripProcess.Trip_id, template.ID);
                    var ballast_check_km = Check_Total_balast.GroupBy(O => O.Km).ToList().Count;

                    var ballast_vdop_0 = 0;
                    var ballast_vdop_15 = 0;
                    var ballast_vdop_25 = 0;
                    var ballast_vdop_40 = 0;
                    var ballast_vdop_60 = 0;

                    foreach (var otst in Check_Total_balast.Where(O => O.Otst != "").ToList())
                    {
                        if (otst.Vdop != "" && otst.Vdop != null)
                        {
                            var speed = otst.Vdop.Split('/');

                            if ((Int32.Parse(speed[0]) == 0 && Int32.Parse(speed[1]) == 0) || (Int32.Parse(speed[0]) == 0 || Int32.Parse(speed[1]) == 0))
                                ballast_vdop_0++;
                            if ((Int32.Parse(speed[0]) == 15 && Int32.Parse(speed[1]) == 15) || (Int32.Parse(speed[0]) == 15 || Int32.Parse(speed[1]) == 15))
                                ballast_vdop_15++;
                            if ((Int32.Parse(speed[0]) == 25 && Int32.Parse(speed[1]) == 25) || (Int32.Parse(speed[0]) == 25 || Int32.Parse(speed[1]) == 25))
                                ballast_vdop_25++;
                            if ((Int32.Parse(speed[0]) == 40 && Int32.Parse(speed[1]) == 40) || (Int32.Parse(speed[0]) == 40 || Int32.Parse(speed[1]) == 40))
                                ballast_vdop_40++;
                            if ((Int32.Parse(speed[0]) >= 60 && Int32.Parse(speed[1]) >= 60) || (Int32.Parse(speed[0]) >= 60 || Int32.Parse(speed[1]) >= 60))
                                ballast_vdop_60++;
                        }
                    }

                    var ballast_no_speedlimit = Check_Total_balast.Where(O => O.Otst != "").ToList().Where(O => O.Vdop == "").ToList().Count;
                    var ballast_All = Check_Total_balast.Where(O => O.Otst != "").ToList().Count;



                    //

                    //для бестыкого пути
               
                    //var Check_Total_Jointless = AdditionalParametersService.Check_Total_state_Jointless(tripProcess.Trip_id, template.ID);
                    //var Jointless_check_km = Check_Total_Jointless.GroupBy(O => O.Km).ToList().Count;
                    //var Jointless_All = Check_Total_Jointless.Where(O => O.Otst != "").ToList().Count;


                    //Отступления в содержании бесстыкового пути //to do
                    List<Gap> Check_ListOgDerogations_state = AdditionalParametersService.Check_ListOgDerogations_state(tripProcess.Trip_id, template.ID);
                    //var Check_ListOgDerogations_state = AdditionalParametersService.Check_Total_balast(tripProcess.Trip_id, template.ID);
                    var ListOgDerogations_check_km = Check_ListOgDerogations_state.GroupBy(O => O.Km).ToList().Count;

                    var ListOgDerogations_vdop_0 = 0;
                    var ListOgDerogations_vdop_15 = 0;
                    var ListOgDerogations_vdop_25 = 0;
                    var ListOgDerogations_vdop_40 = 0;
                    var ListOgDerogations_vdop_60 = 0;

                    foreach (var otst in Check_ListOgDerogations_state.Where(O => O.Otst != "").ToList())
                    {
                        if (otst.Vdop != ""&& otst.Vdop != null)
                        {
                            var speed = otst.Vdop.Split('/');

                            if ((Int32.Parse(speed[0]) == 0 && Int32.Parse(speed[1]) == 0) || (Int32.Parse(speed[0]) == 0 || Int32.Parse(speed[1]) == 0))
                                ListOgDerogations_vdop_0++;
                            if ((Int32.Parse(speed[0]) == 15 && Int32.Parse(speed[1]) == 15) || (Int32.Parse(speed[0]) == 15 || Int32.Parse(speed[1]) == 15))
                                ListOgDerogations_vdop_15++;
                            if ((Int32.Parse(speed[0]) == 25 && Int32.Parse(speed[1]) == 25) || (Int32.Parse(speed[0]) == 25 || Int32.Parse(speed[1]) == 25))
                                ListOgDerogations_vdop_25++;
                            if ((Int32.Parse(speed[0]) == 40 && Int32.Parse(speed[1]) == 40) || (Int32.Parse(speed[0]) == 40 || Int32.Parse(speed[1]) == 40))
                                ListOgDerogations_vdop_40++;
                            if ((Int32.Parse(speed[0]) >= 60 && Int32.Parse(speed[1]) >= 60) || (Int32.Parse(speed[0]) >= 60 || Int32.Parse(speed[1]) >= 60))
                                ListOgDerogations_vdop_60++;
                        }
                    }

                    var ListOgDerogations_no_speedlimit = Check_ListOgDerogations_state.Where(O => O.Otst != "").ToList().Where(O => O.Vdop == "").ToList().Count;
                    var ListOgDerogations_All = Check_ListOgDerogations_state.Where(O => O.Otst != "").ToList().Count;





                    //
                    XElement xeNote = new XElement("Prop",
                                new XAttribute("pch", "ПЧ-"+distance.Code),
                                new XAttribute("checkKM", gap_check_km+ fastening_check_km+ sleepers_check_km+ ralils_check_km+ ballast_check_km+ ListOgDerogations_check_km),



                                new XAttribute("skrep0", fastening_vdop_0),
                                new XAttribute("skrep15", fastening_vdop_15),
                                new XAttribute("skrep25", fastening_vdop_25),
                                new XAttribute("skrep40", fastening_vdop_40),
                                new XAttribute("skrep60", fastening_vdop_60),
                                new XAttribute("skrepBezOgr", fastening_no_speedlimit),
                                new XAttribute("skrepAll", fastening_All),




                                new XAttribute("gap0", gap_vdop_0),
                                new XAttribute("gap15", gap_vdop_15),
                                new XAttribute("gap25", gap_vdop_25),
                                new XAttribute("gap40", gap_vdop_40),
                                new XAttribute("gap60", gap_vdop_60),
                                new XAttribute("gapBezOgr", gap_no_speedlimit),
                                new XAttribute("gapAll", gap_All),



                                new XAttribute("shpal0", sleepers_vdop_0),
                                new XAttribute("shpal15", sleepers_vdop_15),
                                new XAttribute("shpal25", sleepers_vdop_25),
                                new XAttribute("shpal40", sleepers_vdop_40),
                                new XAttribute("shpal60", sleepers_vdop_60),
                                new XAttribute("shpalBezOgr", sleepers_no_speedlimit),
                                new XAttribute("shpalAll", sleepers_All),

                                new XAttribute("rail0", ralils_vdop_0),
                                new XAttribute("rail15", ralils_vdop_15),
                                new XAttribute("rail25", ralils_vdop_25),
                                new XAttribute("rail40", ralils_vdop_40),
                                new XAttribute("rail60", ralils_vdop_60),
                                new XAttribute("railBezOgr", ralils_no_speedlimit),
                                new XAttribute("railAll", ralils_All),

                                new XAttribute("ballast0", ballast_vdop_0),
                                new XAttribute("ballast15", ballast_vdop_15),
                                new XAttribute("ballast25", ballast_vdop_25),
                                new XAttribute("ballast40", ballast_vdop_40),
                                new XAttribute("ballast60", ballast_vdop_60),
                                new XAttribute("ballastBezOgr", ballast_no_speedlimit),
                                new XAttribute("ballastAll", ballast_All),

                                new XAttribute("bezstyk0", ListOgDerogations_vdop_0),
                                new XAttribute("bezstyk15", ListOgDerogations_vdop_15),
                                new XAttribute("bezstyk25", ListOgDerogations_vdop_25),
                                new XAttribute("bezstyk40", ListOgDerogations_vdop_40),
                                new XAttribute("bezstyk60", ListOgDerogations_vdop_60),
                                new XAttribute("bezstykBezOgr", ListOgDerogations_no_speedlimit),
                                new XAttribute("bezstykAll", ListOgDerogations_All),


                                new XAttribute("Prochie",/* elem.Other*/""),
                                new XAttribute("Itogo", fastening_All + gap_All + sleepers_All + ralils_All + ballast_All + ListOgDerogations_All),
                                new XAttribute("allOgrSpeed",
                                (fastening_vdop_0 + fastening_vdop_15 + fastening_vdop_25 + fastening_vdop_40 + fastening_vdop_60) +
                                (gap_vdop_0 + gap_vdop_15 + gap_vdop_25 + gap_vdop_40 + gap_vdop_60) +
                                (sleepers_vdop_0 + sleepers_vdop_15 + sleepers_vdop_25 + sleepers_vdop_40 + sleepers_vdop_60) +
                                (ralils_vdop_0 + ralils_vdop_15 + ralils_vdop_25 + ralils_vdop_40 + ralils_vdop_60) +
                                (ballast_vdop_0 + ballast_vdop_15 + ballast_vdop_25 + ballast_vdop_40 + ballast_vdop_60)+
                                (ListOgDerogations_vdop_0+ ListOgDerogations_vdop_15+ ListOgDerogations_vdop_40+ ListOgDerogations_vdop_60))
                                );
                    tripElem.Add(xeNote);

                    //var SumOfTheDep = RdStructureService.SumOfTheDep(tripProcess.Id) as List<SumOfTheDep>;

                    //foreach (var elem in Check_Total_gaps)
                    //{


                        XElement itogo = new XElement("Itogo",
                                new XAttribute("pch", distance.Code),
                                new XAttribute("checkKM", gap_check_km + fastening_check_km + sleepers_check_km + ralils_check_km + ballast_check_km + ListOgDerogations_check_km),

                                new XAttribute("skrep0", fastening_vdop_0),
                                new XAttribute("skrep15", fastening_vdop_15),
                                new XAttribute("skrep25", fastening_vdop_25),
                                new XAttribute("skrep40", fastening_vdop_40),
                                new XAttribute("skrep60", fastening_vdop_60),
                                new XAttribute("skrepBezOgr", fastening_no_speedlimit),
                                new XAttribute("skrepAll", fastening_All),




                                new XAttribute("gap0", gap_vdop_0),
                                new XAttribute("gap15", gap_vdop_15),
                                new XAttribute("gap25", gap_vdop_25),
                                new XAttribute("gap40", gap_vdop_40),
                                new XAttribute("gap60", gap_vdop_60),
                                new XAttribute("gapBezOgr", gap_no_speedlimit),
                                new XAttribute("gapAll", gap_All),



                                new XAttribute("shpal0", sleepers_vdop_0),
                                new XAttribute("shpal15", sleepers_vdop_15),
                                new XAttribute("shpal25", sleepers_vdop_25),
                                new XAttribute("shpal40", sleepers_vdop_40),
                                new XAttribute("shpal60", sleepers_vdop_60),
                                new XAttribute("shpalBezOgr", sleepers_no_speedlimit),
                                new XAttribute("shpalAll", sleepers_All),

                                new XAttribute("rail0", ralils_vdop_0),
                                new XAttribute("rail15", ralils_vdop_15),
                                new XAttribute("rail25", ralils_vdop_25),
                                new XAttribute("rail40", ralils_vdop_40),
                                new XAttribute("rail60", ralils_vdop_60),
                                new XAttribute("railBezOgr", ralils_no_speedlimit),
                                new XAttribute("railAll", ralils_All),

                                new XAttribute("ballast0", ballast_vdop_0),
                                new XAttribute("ballast15", ballast_vdop_15),
                                new XAttribute("ballast25", ballast_vdop_25),
                                new XAttribute("ballast40", ballast_vdop_40),
                                new XAttribute("ballast60", ballast_vdop_60),
                                new XAttribute("ballastBezOgr", ballast_no_speedlimit),
                                new XAttribute("ballastAll", ballast_All),

                                new XAttribute("bezstyk0", ListOgDerogations_vdop_0),
                                new XAttribute("bezstyk15", ListOgDerogations_vdop_15),
                                new XAttribute("bezstyk25", ListOgDerogations_vdop_25),
                                new XAttribute("bezstyk40", ListOgDerogations_vdop_40),
                                new XAttribute("bezstyk60", ListOgDerogations_vdop_60),
                                new XAttribute("bezstykBezOgr", ListOgDerogations_no_speedlimit),
                                new XAttribute("bezstykAll", ListOgDerogations_All),


                                new XAttribute("Prochie",/* elem.Other*/""),
                                new XAttribute("Itogo", fastening_All + gap_All + sleepers_All + ralils_All + ballast_All+ ListOgDerogations_All),
                                new XAttribute("allOgrSpeed",
                                (fastening_vdop_0 + fastening_vdop_15 + fastening_vdop_25 + fastening_vdop_40 + fastening_vdop_60) +
                                (gap_vdop_0 + gap_vdop_15 + gap_vdop_25 + gap_vdop_40 + gap_vdop_60) +
                                (sleepers_vdop_0 + sleepers_vdop_15 + sleepers_vdop_25 + sleepers_vdop_40 + sleepers_vdop_60) +
                                (ralils_vdop_0 + ralils_vdop_15 + ralils_vdop_25 + ralils_vdop_40 + ralils_vdop_60) +
                                (ballast_vdop_0 + ballast_vdop_15 + ballast_vdop_25 + ballast_vdop_40 + ballast_vdop_60)+
                                (ListOgDerogations_vdop_0 + ListOgDerogations_vdop_15 + ListOgDerogations_vdop_40 + ListOgDerogations_vdop_60))
                                );
                    tripElem.Add(itogo);
                    //}
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
