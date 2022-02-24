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
using ALARm_Report.controls;
using System.Collections.Generic;
using System.Linq;

namespace ALARm_Report.Forms
{
    public class MissingAntiTheft : Report
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

            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");
                XElement xePages = new XElement("pages");

                var road = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;

                var mainProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;

                foreach (var mainProcess in mainProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        xePages = new XElement("pages",
                                new XAttribute("road", road),
                                new XAttribute("period", period.Period),
                                new XAttribute("type", mainProcess.GetProcessTypeName),
                                new XAttribute("car", mainProcess.Car),
                                new XAttribute("chief", mainProcess.Chief),
                                new XAttribute("ps", mainProcess.Car),
                                 new XAttribute("data", "" + mainProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                                new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief));

                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + ")" + " / Путь: " + trackName + " / ПЧ: " + distance.Code));
                        
                        var index = 1;

                        
                        var trackclasses = new List<TrackClass>();
                        var curves = new List<StCurve>();
                        var speed = new List<Speed>();
                        var pdbSection = new List<PdbSection>();
                        Digression prevDig = null;
                        var sector = "";

                        var kms = RdStructureService.GetKilometerTrip(mainProcess.Trip_id);

                        var addDig = new List<Digression> { };
                        
                        foreach (var Km in kms)
                        {
                            var digressions = RdStructureService.GetAti(mainProcess.Trip_id, Km);
                            addDig.AddRange(digressions);
                        }

                        addDig = addDig.OrderBy(o => o.Km).ThenBy(o => o.Meter).ToList();

                        foreach (var elem in addDig)
                        {
                            if ((prevDig == null) || (prevDig.Km != elem.Km))
                            {
                                trackclasses = (List<TrackClass>)MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, elem.Km, MainTrackStructureConst.MtoTrackClass, track_id);
                                curves = (List<StCurve>)MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, elem.Km, MainTrackStructureConst.MtoStCurve, track_id);
                                sector = MainTrackStructureService.GetSector(track_id, elem.Km, mainProcess.Date_Vrem);
                                sector = sector == null ? "" : sector;
                                speed = MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, elem.Km, MainTrackStructureConst.MtoSpeed, mainProcess.DirectionName, trackName.ToString()) as List<Speed>;
                                pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, elem.Km, MainTrackStructureConst.MtoPdbSection, mainProcess.DirectionName, trackName.ToString()) as List<PdbSection>;
                            }
                            prevDig = elem;

                            var trackclass = trackclasses.Count > 0 ? trackclasses[0].Class_Id : -1;
                            var curve = curves.Count > 0 ? (int)curves[0].Radius : -1;

                            //эпюра есептейміз
                            //Эпюры шпал на путях линий 1 - 3 - го классов должны быть: в прямых участках и в кривых радиусом более 1200 м - 1840 шт./ км, 
                            //радиусом 1200 м и менее - 2000 шт./ км; на путях 4 - 5 - го класса: в прямых и кривых радиусом более 1200 м - 1600 шт./ км, 
                            //радиусом 1200 м и менее - 1840 шт./ км.
                            var epur_bpd = 0;

                            switch (trackclass)
                            {
                                case int clas when clas >= 1 && clas <= 3:
                                    if (curve == -1 || curve > 1200)
                                    {
                                        epur_bpd = 1840;
                                    }
                                    else if (curve != -1 && curve <= 1200)
                                    {
                                        epur_bpd = 2000;
                                    }
                                    else
                                    {
                                        epur_bpd = 0;
                                    }
                                    break;
                                case int clas when clas >= 4 && clas <= 5:
                                    if (curve == -1 || curve > 1200)
                                    {
                                        epur_bpd = 1600;
                                    }
                                    else if (curve != -1 && curve <= 1200)
                                    {
                                        epur_bpd = 1840;
                                    }
                                    else
                                    {
                                        epur_bpd = 0;
                                    }
                                    break;
                                default:
                                    epur_bpd = 0;
                                    break;
                            }

                            var Otsut = -1;

                            if (curve == -1)
                            {
                                if(20 > elem.Cn)
                                {
                                    Otsut = 20 - elem.Cn;
                                }
                            }
                            else if(400 > curve)
                            {
                                if (26 > elem.Cn)
                                {
                                    Otsut = 26 - elem.Cn;
                                }
                            }
                            else if( 400 <= curve && curve < 600)
                            {
                                if (20 > elem.Cn)
                                {
                                    Otsut = 20 - elem.Cn;
                                }
                            }
                            else if( 600 <= curve && curve < 950)
                            {
                                if (14 > elem.Cn)
                                {
                                    Otsut = 14 - elem.Cn;
                                }
                            }
                            if (Otsut == -1) continue;

                            XElement xeElements = new XElement("elements",
                                new XAttribute("n", index),
                                new XAttribute("pchu", pdbSection.Count > 0 ? $"ПЧУ-{pdbSection[0].Pchu}/ПД-{pdbSection[0].Pd}/ПДБ-{pdbSection[0].Pdb}" : "ПЧУ-/ПД-/ПДБ-"),
                                new XAttribute("station", sector),
                                new XAttribute("km", elem.Km),
                                new XAttribute("piket", elem.Meter / 100 + 1),
                                new XAttribute("meter", elem.Meter),
                                new XAttribute("speed", speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "-/-"),
                                new XAttribute("thread", elem.Threat == Threat.Left ? "левая" : elem.Threat == Threat.Right ? "правая" : "не определено"),
                                new XAttribute("threadleft", elem.Threat == Threat.Left ? Otsut.ToString() : ""),
                                new XAttribute("threadright", elem.Threat == Threat.Right ? Otsut.ToString() : ""),
                                new XAttribute("notice", ""));
                            xeTracks.Add(xeElements);

                            index++;
                        }
                        xePages.Add(xeTracks);
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
                htReport.Save(Path.GetTempPath() + "/report_MissingAntiTheft.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_MissingAntiTheft.html");
            }
        }
    }
}
