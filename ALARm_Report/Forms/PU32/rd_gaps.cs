using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
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
    public class rd_gaps : Report
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
                XElement report = new XElement("report");
                XElement tripElem = new XElement("trip");
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var videoProcesses = //RdStructureService.GetMainParametersProcesses(period, parentId,true);
                                     RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (videoProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                progressBar.Maximum = videoProcesses.Count;
                int iter = 1;
               
                foreach (var tripProcess in videoProcesses)
                {
                    progressBar.Value = videoProcesses.IndexOf(tripProcess) + 1;
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kms = RdStructureService.GetKilometersByTrip(trip);
                        if (!kms.Any())
                        {
                            {
                                MessageBox.Show("Нет отчетных данных по выбранным параметрам");
                                return;
                            }
                            continue;
                        }
                        
                        kms = kms.Where(o => o.Track_id == track_id).ToList();

                        trip.Track_Id = track_id;
                        var lkm = kms.Select(o => o.Number).ToList();

                        if (lkm.Count() == 0) continue;
                      
                        List<Gap> check_gap_state = AdditionalParametersService.Check_gap_state(tripProcess.Trip_id, template.ID);

                        var ttt = tripProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm");

                         tripElem = new XElement("trip",
                            new XAttribute("date_statement", ttt),
                            new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                            new XAttribute("road", road),
                            new XAttribute("distance", distance.Name),
                            new XAttribute("periodDate", period.Period + " "),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("ps", tripProcess.Car),
                            new XAttribute("trip_date", tripProcess.Trip_date)
                        );

                        XElement Direct = new XElement("direction",
                            new XAttribute("name", tripProcess.DirectionName + " (" + tripProcess.DirectionCode + ")" + " / Путь: " + trackName + " / ПЧ: " + distance.Code)
                        );
                        progressBar.Value = videoProcesses.IndexOf(tripProcess) + 1;
                        //check_gap_state = check_gap_state.Where(o => o.Km.Between(710, 720)).ToList();

                        foreach (var gap in check_gap_state)
                        {
                            if (template.ID == 50)
                            {
                                if (gap.Vdop.Length > 0)
                                {
                                    int d = template.ID;
                                }
                                if (gap.Vdop == "" || gap.Vdop == "-/-")
                                {
                                    continue;
                                }

                                XElement Notes = new XElement("Note");
                                Notes.Add(
                                    new XAttribute("n", iter),
                                    new XAttribute("PPP", gap.Pdb_section),
                                    new XAttribute("PeregonStancia", gap.Fragment),
                                    new XAttribute("km", gap.Km),
                                    new XAttribute("piket", gap.Meter / 100 + 1),
                                    new XAttribute("m", gap.Meter),
                                    new XAttribute("Vpz", gap.FullSpeed),
                                    new XAttribute("ZazorR", gap.R_zazor > gap.Zazor ? gap.R_zazor : gap.Zazor),
                                    new XAttribute("nit", gap.R_zazor > gap.Zazor ? "правая" : "левая"),
                                    new XAttribute("T", gap.Temp),
                                    new XAttribute("Vdop", gap.Vdop),
                                    new XAttribute("Primech","" )//ToDo
                                );
                                iter++;
                                Direct.Add(Notes);
                            }
                            else
                            {
                                XElement Notes = new XElement("Note");

                                Notes.Add(
                                    new XAttribute("n", iter),
                                    new XAttribute("PPP", gap.Pdb_section),
                                    new XAttribute("PeregonStancia", gap.Fragment),
                                    new XAttribute("km", gap.Km),
                                    new XAttribute("piket", gap.Meter / 100 + 1),
                                    new XAttribute("m", gap.Meter),
                                    new XAttribute("Vpz", gap.FullSpeed),
                                    new XAttribute("ZazorR", gap.R_zazor == -1 ? "" : gap.R_zazor.ToString()),
                                    new XAttribute("ZazorL", gap.Zazor == -1 ? "" : gap.Zazor.ToString()),

                                    new XAttribute("T", gap.Temp),//ToDo
                                    new XAttribute("Zabeg", gap.Zabeg == -1 ? "" : gap.Zabeg.ToString()),
                                    new XAttribute("Vdop", gap.Vdop),
                                    new XAttribute("Otst", gap.Otst_l.Count()>0 ? gap.Otst_l: gap.Otst_r),
                                    new XAttribute("Primech", ""),//ToDo


                                    new XAttribute("CarPosition", (int)tripProcess.Car_Position),
                                    new XAttribute("repType", (int)RepType.Gaps),
                                    new XAttribute("fileId", gap.File_Id),
                                    new XAttribute("Ms", gap.Ms),
                                    new XAttribute("fNum", gap.Fnum));
                                iter++;
                                Direct.Add(Notes);
                            }
                        }
                        iter = 1;
                        tripElem.Add(Direct);
                        report.Add(tripElem);

                    }
                }
                progressBar.Value = 0;
                xdReport.Add(report);
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                if (template.ID == 50)
                {
                    //htReport.Save(@"\\Desktop-tolegen\sntfi\ogr_gapreport.html");
                    htReport.Save(Path.GetTempPath() + "ogr_gapreport.html");
                }
                else
                {
                    //htReport.Save(@"\\Desktop-tolegen\sntfi\gapreport.html");
                    htReport.Save(Path.GetTempPath() + "gapreport.html");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                //htReport.Save($@"G:\form\4.Выходные формы Видеоконтроля ВСП\21.Ведомость состояния стыковых зазоров.html");
                if (template.ID == 50)
                {
                    //System.Diagnostics.Process.Start(@"http://Desktop-tolegen:5500/ogr_gapreport.html");
                    System.Diagnostics.Process.Start(Path.GetTempPath() + "/ogr_gapreport.html");
                }
                else
                {
                    //System.Diagnostics.Process.Start(@"http://Desktop-tolegen:5500/gapreport.html");
                    System.Diagnostics.Process.Start(Path.GetTempPath() + "/gapreport.html");
                }

            }
        }
        public override string ToString()
        {
            return "Отступления 2 степени, близкие к 3";
        }
    }
}