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
    public class FligfhtHaulageStatement : Report
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

                var filterForm = new FilterForm();
                var filters = new List<Filter>();
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                filters.Add(new FloatFilter() { Name = "Годовая амплита температуры рельсов", Value = 80 });

                filterForm.SetDataSource(filters);
                if (filterForm.ShowDialog() == DialogResult.Cancel)
                    return;

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        //var directName = AdditionalParametersService.DirectName(tripProcess.Id, (int)tripProcess.Direction);

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
                                tripProcess.DirectionName + " (" + tripProcess.DirectionCode + "), Путь: " + trackName + ", ПЧ: " + distance.Code)
                        );

                        var idZazor = 1;
                        int iter = 0;
                        var nominal = 6.0;

                        var sumZazorL = 0; //todo nomanal zazor
                        var sumZazorR = 0; //todo nomanal zazor

                        var sumNominalZazorL = 0.0;//todo nomanal zazor
                        var sumNominalZazorR = 0.0;//todo nomanal zazor

                        List<Gap> check_gap_state = AdditionalParametersService.Check_gap_state(tripProcess.Trip_id, template.ID);

                        int prev_km = -1;

                        foreach (var item in check_gap_state)
                        {
                            try
                            {
                                List<String> s = new List<String>(item.Temp.Split("°".ToCharArray()));
                                item.Temperature = Int32.Parse(s.First());
                            }
                            catch
                            {
                                item.Temperature = -999;
                            }

                            item.RailLen = 25; //todo
                            item.TempByRegion = int.Parse(filters[0].Value.ToString());

                            var n = item.GetNominalGapValueByTemp();
                            nominal = n;

                            if (item.Km != prev_km)
                            {
                                idZazor = 0;
                                sumZazorL = 0;
                                sumZazorR = 0;

                                sumNominalZazorL = 0;
                                sumNominalZazorR = 0;
                            }
                            prev_km = item.Km;

                            iter++;
                            idZazor++;

                            sumZazorL += item.Zazor;
                            sumZazorR += item.R_zazor;
                            sumNominalZazorL += nominal;

                            XElement Notes = new XElement("Note");

                            Notes.Add(
                                new XAttribute("n", iter),
                                new XAttribute("km", item.Km),
                                new XAttribute("piket", item.Meter / 100 + 1),
                                new XAttribute("m", item.Meter),
                                new XAttribute("styk", idZazor),
                                new XAttribute("nominalZazor", nominal),

                                new XAttribute("zazorL", item.Zazor),
                                new XAttribute("napolZazorL", sumZazorL),
                                new XAttribute("napolNominalZazorL", sumNominalZazorL),
                                new XAttribute("paredvRelsL", (double)(sumZazorL - sumNominalZazorL)),

                                new XAttribute("zazorR", item.R_zazor),
                                new XAttribute("napolZazorR", sumZazorR),
                                new XAttribute("napolNominalZazorR", sumNominalZazorL),
                                new XAttribute("paredvRelsR", (double)(sumZazorR - sumNominalZazorL))
                            );                          
                            Direct.Add(Notes);
                        }
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