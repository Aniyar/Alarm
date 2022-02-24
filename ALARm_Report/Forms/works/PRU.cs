using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class PRU : ALARm.Core.Report.GraphicDiagrams

    {

        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
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

            this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                List<Curve> curves = (MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);
                distance.Name = distance.Name.Replace("ПЧ-", "");

                var tripProcess = RdStructureService.GetTripsOnDistance(parentId, period);
                if (tripProcess.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var track_id in admTracksId)
                {
                    foreach (var tripProces in tripProcess)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var trip = RdStructureService.GetTrip(tripProces.Id);
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);
                        if (!kilometers.Any()) continue;
                        ////Выбор километров по проезду-----------------
                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();


                        var lkm = kilometers.Select(o => o.Number).ToList();

                        var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                        //filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        //filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        //filterForm.SetDataSource(filters);
                        //if (filterForm.ShowDialog() == DialogResult.Cancel)
                        //    return;

                        //kilometers = kilometers.Where(Km => ((float)(float)filters[0].Value <= Km.Number && Km.Number <= (float)(float)filters[1].Value)).ToList();
                        //kilometers = (trip.Travel_Direction == Direction.Reverse ? kilometers.OrderBy(o => o.Number) : kilometers.OrderByDescending(o => o.Number)).ToList();
                        //--------------------------------------------


                        XElement tripElem = new XElement("trip",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            //  new XAttribute("direction", kilometers[0].Direction_name),
                            new XAttribute("direction", tripProces.Direction),
                            new XAttribute("directioncode", tripProces.DirectionCode),
                            new XAttribute("check", tripProces.GetProcessTypeName),
                            //new XAttribute("track", kilometers[0].Track_name),
                            new XAttribute("road", road),
                            new XAttribute("distance", distance.Code),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("chief", tripProces.Chief),
                            new XAttribute("ps", tripProces.Car));
                        XElement xeDirection = new XElement("directions");
                        XElement xeTracks = new XElement("tracks");

                        var ListS3 = RdStructureService.GetS3(kilometers.First().Trip.Id, "ПрУ") as List<S3>; //пру
                        if (ListS3.Any())
                        {
                            foreach (var km in kilometers)
                            {
                                var PRUbyKm = ListS3.Where(o => o.Ots == "ПрУ").ToList();
                                //&& o.Km == km.Number
                                km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);


                                foreach (var s3 in PRUbyKm)
                                {
                                    XElement xeNote = new XElement("note",

                                        new XAttribute("km", s3.Km),
                                        new XAttribute("m", s3.Meter),
                                        new XAttribute("Otkl", s3.Otkl),
                                        new XAttribute("len", s3.Len),
                                        new XAttribute("vpz", km.Speeds.Last().Passenger + "/" + km.Speeds.Last().Freight));

                                    xeTracks.Add(xeNote);
                                }

                            }

                            xeDirection.Add(xeTracks);
                            tripElem.Add(xeDirection);
                        }
                            
                            
                        else
                        {

                            {
                                XElement xeNote = new XElement("note",

                                    new XAttribute("km", ""),
                                    new XAttribute("m", ""),
                                    new XAttribute("Otkl", ""),
                                    new XAttribute("len", ""),
                                    new XAttribute("vpz", ""));

                                xeTracks.Add(xeNote);
                            }
                        }

                      
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
    }
}

