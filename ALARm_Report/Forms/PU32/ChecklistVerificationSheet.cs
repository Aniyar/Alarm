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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using ElCurve = ALARm.Core.ElCurve;


namespace ALARm_Report.Forms
{
    public class ChecklistVerificationSheet : Report
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

                var road = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    List<DigressionTotal> totals = new List<DigressionTotal>();
                    DigressionTotal digressionTotal = new DigressionTotal();
                    DigressionTotal shaplontotal = new DigressionTotal();
                    DigressionTotal leveltotal = new DigressionTotal();
                    
                    XElement tripElem = new XElement("trip",
                      new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                      new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                      new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                      new XAttribute("road", road),
                      new XAttribute("distance", distance.Code),
                      new XAttribute("periodDate", period.Period),
                      new XAttribute("chief", tripProcess.Chief),
                      new XAttribute("ps", tripProcess.Car));

                    //if (tripProcess != vibor) continue;
                    leveltotal.Count = 0;
                    shaplontotal.Count = 0;
                    digressionTotal.Count = 0;
                    foreach (var track in admTracksId)
                    {
                        
                        var trackName = AdmStructureService.GetTrackName(track);
                  
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);
                        kilometers = kilometers.Where(ckm => ckm.Track_id == track).ToList();
                        var totalwaytrack = 0;
                        XElement xeDirection = new XElement("directions");
                        XElement xeTracks = new XElement("tracks");
                        foreach (var km in kilometers)
                        {
                            var MtoCheckSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, km.Number, 
                                                    MainTrackStructureConst.MtoCheckSection, tripProcess.DirectionName, trackName.ToString()) as List<CheckSection>;
                           
                            if (MtoCheckSection.Count > 0)
                            {
                                foreach(var sect in MtoCheckSection)
                                {
                                   
                                    var CheckVerifyKm = RdStructureService.CheckVerify(tripProcess.Trip_id, sect.Start_Km * 1000 + sect.Start_M,sect.Final_Km * 1000 + sect.Final_M);

                                    //CheckVerifyKm = CheckVerifyKm.Where(ckm => ckm.Track_Id == track.Id).ToList();

                                    if (CheckVerifyKm.Count <= 0)
                                        continue;

                                    var month = period.Period.Split(' ')[0];

                                    XElement note = new XElement("note",
                                                        
                                                        new XAttribute("put", trackName),
                                                        new XAttribute("km", km.Number),
                                                        new XAttribute("mes", tripProcess.Date_Vrem.ToString("dd.MM.yyyy")),
                                                        new XAttribute("ur1", CheckVerifyKm.Count > 0 ? CheckVerifyKm[0].Trip_mo_level.ToString("0.0") : "нет данных"),
                                                        new XAttribute("ur2", CheckVerifyKm.Count > 0 ? CheckVerifyKm[0].Trip_sko_level.ToString("0.0") : "нет данных"),
                                             

                                                          
                                                        new XAttribute("ur3", MtoCheckSection.Count > 0 ? MtoCheckSection[0].Avg_level.ToString("0.0") : "нет данных"),
                                                        new XAttribute("ur4", MtoCheckSection.Count > 0 ? MtoCheckSection[0].Sko_level.ToString("0.0") : "нет данных"),
                                                         new XAttribute("ur2format", Math.Abs(CheckVerifyKm[0].Trip_mo_level - MtoCheckSection[0].Avg_level)),


                                                        new XAttribute("sh1", CheckVerifyKm.Count > 0 ? CheckVerifyKm[0].Trip_mo_gauge.ToString("0.0") : "нет данных"),
                                                        new XAttribute("sh2", CheckVerifyKm.Count > 0 ? CheckVerifyKm[0].Trip_sko_gauge.ToString("0.0") : "нет данных"),
                                                          
                                                        new XAttribute("sh3", MtoCheckSection.Count > 0 ? MtoCheckSection[0].Avg_width.ToString("0.0") : "нет данных"),
                                                        new XAttribute("sh4", MtoCheckSection.Count > 0 ? MtoCheckSection[0].Sko_width.ToString("0.0") : "нет данных"),
                                                           new XAttribute("sh2format",Math.Abs(CheckVerifyKm[0].Trip_mo_gauge - MtoCheckSection[0].Avg_width))

                                                        );
                              
                                    digressionTotal.Count += 1;
                                    totals.Add(digressionTotal);
                                    if (CheckVerifyKm[0].Trip_sko_gauge < 2.0)
                                    {
                                        shaplontotal.Count += 1;
                                        totals.Add(shaplontotal);
                                    }
                                    if (CheckVerifyKm[0].Trip_sko_level < 2.0)
                                    {
                                        leveltotal.Count += 1;
                                        totals.Add(leveltotal);
                                    }

                                    xeTracks.Add(note);
                                    

                                }
                            }
                        }
                        xeDirection.Add(xeTracks);
                        tripElem.Add(xeDirection);
                        xeTracks.Add(new XAttribute("dircode", tripProcess.DirectionCode));
                        xeTracks.Add(new XAttribute("dirtrack", trackName));

                    }
                  
                    tripElem.Add(new XAttribute("totals", digressionTotal.Count));
                    tripElem.Add(new XAttribute("totalslevel", leveltotal.Count));
                    tripElem.Add(new XAttribute("totalsshablon", shaplontotal.Count));
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
            finally{
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report.html");
            }
        }

        public override string ToString()
        {
            return "Отступления 2 степени, близкие к 3";
        }

    }
}
