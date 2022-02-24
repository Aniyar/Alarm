using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
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

namespace ALARm_Report.Forms
{
    public class SettingJointGaps : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            //Сделать выбор периода
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(distanceId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel)
                    return;
                admTracksId = choiceForm.admTracksIDs;
            }

            //Выбор км
            var filterForm = new FilterForm();
            var filters = new List<Filter>();
            

            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var mainProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);

                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                foreach (var process in mainProcesses)
                {

                    var List_kms = RdStructureService.GetKilometerTrip(process.Trip_id);

                    filters.Add(new FloatFilter() { Name = "Начальный Км:", Value = List_kms[0] });
                    filters.Add(new FloatFilter() { Name = "Конечный Км:", Value = List_kms[List_kms.Count-1] });

                    filterForm.SetDataSource(filters);
                    if (filterForm.ShowDialog() == DialogResult.Cancel)
                        return;

                    XElement xePages = new XElement("pages");

                    var gaps = RdStructureService.GetGaps(process.Id);

                    List<Gap> check_gap_state = AdditionalParametersService.Check_gap_state(process.Trip_id, template.ID);

                    if (!check_gap_state.Any())
                        continue;

                    foreach (var trackId in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(trackId);

                        xePages.Add(
                            new XAttribute("pc", process.Car),
                            new XAttribute("trip_info", process.Date_Vrem.Date.ToShortDateString()),
                            new XAttribute("trip_date", process.Trip_date),
                            new XAttribute("distance", distance.Code),
                        
                            new XAttribute("road", road),
                            new XAttribute("direction", $"{process.DirectionName}({ process.DirectionCode})"),
                            new XAttribute("track", trackName),
                            new XAttribute("km", (float)(float)filters[0].Value + " - " + (float)(float)filters[1].Value)
                        );

                        XElement xeKms = new XElement("kms");


                        int order = 1, summMeasureLeft = 0, summMeasureRight = 0;
                        double summNominalLeft = 0, summNominalRight = 0;

                        foreach (var data1 in check_gap_state)
                        {
                            var kms = data1.Km;

                            if ((float)(float)filters[0].Value > kms || kms > (float)(float)filters[1].Value) continue;

                            string km_info = $"{process.DirectionName}({ process.DirectionCode}) / Путь: {trackName} / ПЧ: {distance.Code}";

                            

                            xeKms = new XElement("kms",
                                new XAttribute("km_info", km_info),
                                new XAttribute("km", kms));

                            var meters = data1.Meter;

                            XElement xeElements = new XElement("elements",
                                new XAttribute("order", order),
                                new XAttribute("m", meters));

                            var rightGap = data1.R_zazor;

                            if (rightGap != -999)
                            {
                                var Nominal = 9;

                                summNominalRight += Nominal;
                                summMeasureRight += rightGap;
                                xeElements.Add(
                                    new XAttribute("nominal_right", Nominal),
                                    new XAttribute("measure_right", rightGap),
                                    new XAttribute("summ_measure_right", summMeasureRight),
                                    new XAttribute("summ_nominal_right", Math.Round(summNominalRight, 2)),
                                    new XAttribute("moving_right", Math.Round(summMeasureRight - summNominalRight, 2)));
                                order++;
                                xeKms.Add(xeElements);
                                xePages.Add(xeKms);
                            }
                            //else
                            //{
                            //    xeElements.Add(
                            //        new XAttribute("nominal_right", ""),
                            //        new XAttribute("measure_right", ""),
                            //        new XAttribute("summ_measure_right", ""),
                            //        new XAttribute("summ_nominal_right", ""),
                            //        new XAttribute("moving_right", ""));
                            //}

                            //var leftGap = data1.Zazor;

                            //if (leftGap != -999)
                            //{
                            //    var Nominal = 9;

                            //    summNominalLeft += Nominal;
                            //    summMeasureLeft += leftGap;
                            //    xeElements.Add(
                            //        new XAttribute("nominal_left", Nominal),
                            //        new XAttribute("measure_left", leftGap),
                            //        new XAttribute("summ_measure_left", summMeasureLeft),
                            //        new XAttribute("summ_nominal_left", Math.Round(summNominalLeft, 2)),
                            //        new XAttribute("moving_left", Math.Round(summMeasureLeft - summNominalLeft, 2)));
                            //    //order++;
                            //}
                            //else
                            //{
                            //    xeElements.Add(
                            //        new XAttribute("nominal_left", ""),
                            //        new XAttribute("measure_left", ""),
                            //        new XAttribute("summ_measure_left", ""),
                            //        new XAttribute("summ_nominal_left", ""),
                            //        new XAttribute("moving_left", ""));
                            //}
                            
                            
                        }

                        
                        report.Add(xePages);
                    }
                    
                }
                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_SettingJointGaps.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_SettingJointGaps.html");
            }
        }
    }
}