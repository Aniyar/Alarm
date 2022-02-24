using ALARm.Core;
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
    public class Pu32dbd : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();

                var distance =
                    AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
            
                var road = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    var ListS3 = RdStructureService.GetDBD(tripProcess.Id) as List<S3>;
                    ListS3 = ListS3.Where(o => o.Typ == 4).ToList();
                    //Участки дист коррекция
                    var dist_section = MainTrackStructureService.GetDistSectionByDistId(distance.Id);
                    foreach (var item in ListS3)
                    {
                        var ds = dist_section.Where(
                            o => item.Km * 1000 + item.Meter >= o.Start_Km * 1000 + o.Start_M && item.Km * 1000 + item.Meter <= o.Final_Km * 1000 + o.Final_M).ToList();

                        item.Pchu = ds.First().Pchu;
                        item.Pd = ds.First().Pd;
                        item.Pdb = ds.First().Pdb;
                    }

                    List<Digression> Check_deviationsinfastening_state = AdditionalParametersService.Check_deviationsinfastening_state(tripProcess.Trip_id, template.ID);
                    Check_deviationsinfastening_state = Check_deviationsinfastening_state.Where(o => o.Km > 128 && o.Vdop != "").ToList();

                    var Check_sleepers_state = AdditionalParametersService.Check_sleepers_state(tripProcess.Trip_id, template.ID);
                    Check_sleepers_state = Check_sleepers_state.Where(o => o.Km > 128 && o.Vdop != "").ToList();

                    var digressions = new List<Digression> { };
                    digressions.AddRange(Check_deviationsinfastening_state);
                    digressions.AddRange(Check_sleepers_state);

                   
                    foreach (var item in ListS3)
                    {
                        digressions.Add(new Digression
                        {
                            DigressionType = DigressionType.Main,
                            Pchu = "" + item.Pd + "" + item.Pchu,
                            
                            Km = item.Km,
                            Meter = item.Meter,
                            Ots = item.Ots,
                            Velich = item.Otkl,
                            Length = item.Len,
                            Count = item.Kol,
                            Typ = item.Typ,
                            Vpz = item.Uv.ToString().Replace("-1", "-") + "/" + item.Uvg.ToString().Replace("-1", "-"),
                            Vdop = item.Ovp.ToString().Replace("-1", "-") + "/" + item.Ogp.ToString().Replace("-1", "-"),
                            Note = item.Primech
                        });
                    }

                    XElement tripElem = new XElement("trip",
                        new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        new XAttribute("road", road),
                        new XAttribute("pch", distance.Code),
                        new XAttribute("ps", tripProcess.Car),
                        new XAttribute("check", tripProcess.GetProcessTypeName),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("info", $"{tripProcess.DirectionName}({tripProcess.DirectionCode}) Путь: 1"),
                        new XAttribute("itogo", $"Всего по пути 1 - {digressions.Count} шт"),
                        new XAttribute("date_statement", period.Period)
                        );


                    XElement lev = new XElement("lev");
                    //XElement xeNote = new XElement("Note");
                    int iter = 0;

                    int constrictionCount = 0;
                    int broadeningCount = 0;
                    int levelCount = 0;
                    int skewnessCount = 0;
                    int drawdownCount = 0;
                    int straighteningCount = 0;
                    int nonstraighteningCount = 0;
                    int knsh = 0;
                    int kns = 0;


                    //кнш
                    //var knsh = Check_sleepers_state.Where(o => o.Km == elem.Km && o.Meter / 100 + 1 == elem.Piket && o.Vdop != "").ToList();
                    //string knsh_string = "";
                    //var ps = new List<int> { };
                    //var gr = new List<int> { };
                    //foreach (var k in knsh)
                    //{
                    //    //Определение звена 1,2,3,4
                    //    var ostatok = k.Meter % 100;
                    //    var zveno = (int)(ostatok / 25.0) + 1;

                    //    knsh_string += $"{k.Velich}/{zveno}" + ", ";

                    //    ps.Add(int.Parse(k.Vdop.Split('/')[0]));
                    //    gr.Add(int.Parse(k.Vdop.Split('/')[1]));
                    //}

                    digressions = digressions.OrderBy(o => o.Km).ThenBy(o => o.Meter).ToList();
                    string lastPchu = "-1", lastPd = "-1";
                    
                    int lastKm = -1;
                    foreach (var item in digressions)

                    {
                        XElement Notes = new XElement("Note");

                        if (item.DigressionType == DigressionType.Additional)
                        {
                            var d = item.Pchu.Split('/');
                            var PCHU = d[0].Replace("ПЧУ-", "").ToString();
                            var PD = d[1].Replace("ПД-", "").ToString();
                            if (PCHU.Equals(lastPchu))
                            {
                                if (PD.Equals(lastPd))
                                {
                                    if (item.Km == lastKm)
                                    {

                                        //xeNote = new XElement("Note",
                                        //    new XAttribute("Pchu", d[0].Replace("ПЧУ-", "")),
                                        //    new XAttribute("PD", d[1].Replace("ПД-", "")),
                                        //    new XAttribute("Km", item.Km),
                                        //    new XAttribute("Pk", item.Meter / 100 + 1),
                                        //    new XAttribute("Otst", item.Ots),
                                        //    new XAttribute("Velichina", item.Kol),
                                        //    new XAttribute("Dlina", "-"),
                                        //    new XAttribute("Stepen", "-"),
                                        //    new XAttribute("Vust", item.Vpz),
                                        //    new XAttribute("Vogr", item.Vdop),
                                        //    new XAttribute("Itogo", ""),
                                        //    new XAttribute("Primech", "")
                                        //    );
                                        //lev.Add(xeNote);
                                        //iter++;
                                        XElement xeNote = new XElement("Note",
                                           new XAttribute("Pchu", ""),
                                           new XAttribute("PD", ""),
                                           new XAttribute("Km", ""),
                                           new XAttribute("Pk", item.Meter / 100 + 1),
                                           new XAttribute("Otst", item.Ots),
                                           new XAttribute("Velichina", item.Kol),
                                           new XAttribute("Dlina", "-"),
                                           new XAttribute("Stepen", "-"),
                                           new XAttribute("Vust", item.Vpz),
                                           new XAttribute("Vogr", item.Vdop),
                                           new XAttribute("Itogo", ""),
                                           new XAttribute("Primech", "")
                                           );
                                        lev.Add(xeNote);
                                        iter++;
                                    }
                                    else
                                    {
                                        lastKm = item.Km;
                                        XElement xeNote = new XElement("Note",
                                          new XAttribute("Pchu", ""),
                                          new XAttribute("PD", ""),
                                          new XAttribute("Km", item.Km),
                                          new XAttribute("Pk", item.Meter / 100 + 1),
                                          new XAttribute("Otst", item.Ots),
                                          new XAttribute("Velichina", item.Kol),
                                          new XAttribute("Dlina", "-"),
                                          new XAttribute("Stepen", "-"),
                                          new XAttribute("Vust", item.Vpz),
                                          new XAttribute("Vogr", item.Vdop),
                                          new XAttribute("Itogo", ""),
                                          new XAttribute("Primech", "")
                                          );
                                        lev.Add(xeNote);
                                        iter++;
                                    }
                                }
                                else

                                {
                                    PD = lastPd.ToString();
                                    lastKm = item.Km;
                                    XElement xeNote = new XElement("Note",
                                        new XAttribute("Pchu", ""),
                                        new XAttribute("PD", PD),
                                        new XAttribute("Km", item.Km),
                                        new XAttribute("Pk", item.Meter / 100 + 1),
                                        new XAttribute("Otst", item.Ots),
                                        new XAttribute("Velichina", item.Kol),
                                        new XAttribute("Dlina", "-"),
                                        new XAttribute("Stepen", "-"),
                                        new XAttribute("Vust", item.Vpz),
                                        new XAttribute("Vogr", item.Vdop),
                                        new XAttribute("Itogo", ""),
                                        new XAttribute("Primech", "")
                                        );
                                    lev.Add(xeNote);
                                    iter++;
                                }
                            }
                            else
                            {
                                lastPchu = PCHU;
                                lastPd = PD;
                                lastKm = item.Km;

                                XElement xeNote = new XElement("Note",
                                      new XAttribute("Pchu", PCHU),
                                      new XAttribute("PD", PD),
                                      new XAttribute("Km", item.Km),
                                      new XAttribute("Pk", item.Meter / 100 + 1),
                                      new XAttribute("Otst", item.Ots),
                                      new XAttribute("Velichina", item.Kol),
                                      new XAttribute("Dlina", "-"),
                                      new XAttribute("Stepen", "-"),
                                      new XAttribute("Vust", item.Vpz),
                                      new XAttribute("Vogr", item.Vdop),
                                      new XAttribute("Itogo", ""),
                                      new XAttribute("Primech", "")
                                      );
                                lev.Add(xeNote);
                                iter++;
                            }
                        }
                        else if (item.DigressionType == DigressionType.Main)
                        {
                            XElement xeNote = new XElement("Note",
                                new XAttribute("Pchu", item.Pchu),
                                  //new XAttribute("PD", item.PD),
                                  new XAttribute("Km", item.Km),
                                new XAttribute("Pk",  (item.Meter / 100 + 1)),
                                new XAttribute("Otst", item.Ots),
                                new XAttribute("Velichina", item.Velich + " мм"),
                                new XAttribute("Dlina", item.Length),
                                new XAttribute("Stepen", item.Typ),
                                new XAttribute("Vust", item.Vpz),
                                new XAttribute("Vogr", item.Vdop),
                                new XAttribute("Itogo", ""),
                                new XAttribute("Primech", item.Note)
                                );
                            lev.Add(xeNote);
                            iter++;

                        }
                        switch (item.Ots)
                        {
                            case "КНШ":
                                knsh += 1;
                                break;
                            case "КНС":
                                kns += 1;
                                break;
                            case "Пр.п":
                            case "Пр.л":
                                drawdownCount += item.Count;
                                break;
                            case "Суж":
                                item.Count = item.Length / 4 + (item.Length % 4 + 1 > 1 ? 1 : 0);
                                constrictionCount += item.Count;

                                break;
                            case "Уш":
                                item.Count = item.Length / 4 + (item.Length % 4 + 1 > 1 ? 1 : 0);
                                broadeningCount += item.Count;
                                break;
                            case "У":
                                item.Count = item.Length / 10 + (item.Length % 10 + 1 > 1 ? 1 : 0);
                                levelCount += item.Count;
                                break;
                            case "П":
                                skewnessCount += item.Count;
                                break;
                            case "Р":
                                straighteningCount += item.Count;
                                break;
                            case "Рнр":
                                nonstraighteningCount += item.Count;
                                break;
                        }
                    }
                    lev.Add(new XAttribute("km", $"{digressions.First().Km + " - " + digressions.Last().Km}"),
                            new XAttribute("put", $"1"),
                            new XAttribute("pch", distance.Code),
                            new XAttribute("kolKm", digressions.Last().Km - digressions.First().Km),
                            new XAttribute("itogoPCH", digressions.Count()));
                    if (knsh  > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", $"КНШ -  {knsh} ")));
                    }
                    if (kns > 0 )
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", $"КНС - {kns} ")));
                    }
                    if (drawdownCount > 0 )
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", $"Пр - {drawdownCount}")));
                    }
                    if (constrictionCount >  0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", $"Суж -  {constrictionCount}")));
                    }
                    if (broadeningCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", "Уш - " + broadeningCount)));
                    }
                    if (levelCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", "У - " + levelCount)));
                    }
                    if (skewnessCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", "П - " + skewnessCount)));
                    }
                    if (straighteningCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", "Р - " + straighteningCount)));
                    }
                    if (nonstraighteningCount > 0)
                    {
                        tripElem.Add(new XElement("total", new XAttribute("final", "Рнр - " + nonstraighteningCount)));
                    }


                    tripElem.Add(
                        new XAttribute("final",
                            $@" В том числе: {(knsh > 0 ? $"КНШ - {knsh}," + '\r' : "")} 
                                             {(kns > 0 ? $"КНС - {kns}," : "")} 
                                             {(drawdownCount > 0 ? $"Пр - {drawdownCount}," : "")}
                                             {(constrictionCount > 0 ? $"Суж - {constrictionCount}," : "")} 
                                             {(broadeningCount > 0 ? $"Уш - {broadeningCount}," : "")} 
                                             {(levelCount > 0 ? $"У - {levelCount}," : "")} 
                                             {(skewnessCount > 0 ? $"П - {skewnessCount}," : "")} 
                                             {(straighteningCount > 0 ? $"Р - {straighteningCount}," : "")}
                                             {(nonstraighteningCount > 0 ? $"Рнр - {nonstraighteningCount}," : "")}")

                        );



                    tripElem.Add(lev);
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
