using ALARm.Core;
using ALARm.Core.Report;
using MetroFramework.Controls;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using ALARm.Services;
using ALARm.Core.AdditionalParameteres;
using System.Collections.Generic;
using ALARm_Report.controls;
using System.Globalization;
using System.Linq;

namespace ALARm_Report.Forms
{
    public class FasteningUnderRailSole : Report
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

            Int64 lastProcess = -1;
            int index = 1;

            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");
                XElement xePages = new XElement("pages");

                var mainProcesses = RdStructureService.GetProcess(period, distanceId, ProcessType.VideoProcess);
                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                foreach (var mainProcess in mainProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {

                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var trip = RdStructureService.GetTrip(mainProcess.Id);
                        var kms = RdStructureService.GetKilometersByTrip(trip);
                        if (!kms.Any()) continue;

                        kms = kms.Where(o => o.Track_id == track_id).ToList();

                        trip.Track_Id = track_id;
                        var lkm = kms.Select(o => o.Number).ToList();

                        if (lkm.Count() == 0) continue;
                        //var trackName = AdmStructureService.GetTrackName(track_id);

                        //var digressions = RdStructureService.GetDigressions(mainProcess, new int[] { 1004 });
                        //var digressions = RdStructureService.GetRAilSole(mainProcess.Trip_id, false, distance.Code, "");
                        var digressions = RdStructureService.GetRAilSole(mainProcess.Trip_id, false, distance.Code, trackName);

                        if (digressions.Count < 1 || digressions == null)
                        {
                            continue;
                        }

                        if (mainProcess.Id == lastProcess)
                        {
                            XElement xeTracks = new XElement("tracks",
                                new XAttribute("trackinfo", $"{mainProcess.DirectionName}({mainProcess.DirectionCode}) / Путь: {trackName} / ПЧ: { distance.Code }"));

                            foreach (var finddeg in digressions)
                            {
                                //string tripplan = finddeg.CurveRadius != 0 ? "кривая R-" + finddeg.CurveRadius.ToString() : "прямой";

                                XElement xeElements = new XElement("elements",
                                    new XAttribute("n", index),
                                    new XAttribute("pchu", finddeg.PdbSection),
                                    new XAttribute("station", /*finddeg.StationName*/""),
                                    new XAttribute("km", finddeg.Km),
                                    new XAttribute("piket", finddeg.Mtr / 100 + 1),
                                    new XAttribute("meter", finddeg.Mtr),
                                    new XAttribute("digression", /*finddeg.Name*/"Клемма под подошвой рельса"),
                                    new XAttribute("thread", finddeg.Threat == Threat.Left ? "левая" : "правая"),
                                    new XAttribute("fastening", finddeg.ToString()),
                                    new XAttribute("tripplan", ""),
                                    new XAttribute("notice", ""));
                                xeTracks.Add(xeElements);

                                index++;
                            }

                            xePages.Add(xeTracks);
                        }
                        else
                        {
                            if (lastProcess != -1)
                                report.Add(xePages);
                            lastProcess = mainProcess.Id;
                            index = 1;

                            xePages = new XElement("pages",
                                new XAttribute("road", road),
                                new XAttribute("period", period.Period + " "),
                                new XAttribute("type", mainProcess.GetProcessTypeName),
                                new XAttribute("car", mainProcess.Car),
                                new XAttribute("chief", mainProcess.Chief),
                                new XAttribute("ps", mainProcess.Car),
                                new XAttribute("data", "" + mainProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                                new XAttribute("info", "ПС: " + mainProcess.Car + " " + mainProcess.Chief));

                            XElement xeTracks = new XElement("tracks",
                                new XAttribute("trackinfo", $"{mainProcess.DirectionName}({mainProcess.DirectionCode}) / Путь: {trackName} / ПЧ: { distance.Code }"));


                            RailFastener prev_digression = null;
                            List<Curve> curves = new List<Curve> { };
                            var skreplenie = new List<RailsBrace>();
                            var sector = "";
                            var speed = new List<Speed> { };

                            skreplenie = MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, digressions[0].Km, MainTrackStructureConst.MtoRailsBrace, mainProcess.DirectionName, trackName.ToString()) as List<RailsBrace>;


                            //проверка скрепления на паспорте
                            if (skreplenie[0].Name != "ЖБР" || skreplenie[0].Name != "ЖБР-65" || skreplenie[0].Name != "ЖБР-65Ш") 
                            {
                                digressions = new List<RailFastener> { };
                            }

                            foreach (var finddeg in digressions)
                            {
                                if ((prev_digression == null) || (prev_digression.Km != finddeg.Km))
                                {
                                    skreplenie = MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, finddeg.Km, MainTrackStructureConst.MtoRailsBrace, mainProcess.DirectionName, trackName.ToString()) as List<RailsBrace>;
                                    sector = MainTrackStructureService.GetSector(track_id, finddeg.Km, mainProcess.Date_Vrem);
                                    speed = MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, finddeg.Km, MainTrackStructureConst.MtoSpeed, mainProcess.DirectionName, trackName.ToString()) as List<Speed>;
                                    curves = MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, finddeg.Km, MainTrackStructureConst.MtoCurve, track_id) as List<Curve>;
                                }

                                XElement xeElements = new XElement("elements",
                                    new XAttribute("n", index),
                                    new XAttribute("pchu", finddeg.PdbSection),
                                    new XAttribute("station", sector),
                                    new XAttribute("km", finddeg.Km),
                                    new XAttribute("piket", finddeg.Mtr / 100 + 1),
                                    new XAttribute("meter", finddeg.Mtr),
                                    new XAttribute("vpz", speed.Count > 0 ? speed[0].Passenger+"/"+speed[0].Freight : "Нет данных"),
                                    new XAttribute("digression", /*finddeg.Name*/"Клемма под подошвой рельса"),
                                    new XAttribute("thread", finddeg.Threat == Threat.Left ? "левая" : "правая"),
                                    new XAttribute("fastening", skreplenie.Count > 0 ? skreplenie[0].Name : "Нет данных"),
                                    new XAttribute("tripplan", curves.Count > 0 ? $"кривая R-{curves[0].Radius}" : "прямой"),
                                    new XAttribute("notice", ""));
                                xeTracks.Add(xeElements);

                                index++;
                            }
                            xePages.Add(xeTracks);
                        }
                    }
                }
                report.Add(xePages);
                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_FasteningUnderRailSole.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_FasteningUnderRailSole.html");
            }
        }
    }
}