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
using System.Collections.Generic;
using System.Linq;
using ALARm_Report.controls;
using System.Reflection;

namespace ALARm_Report.Forms
{
    public class EpureSleepers : Report
    {
        public override void Process(long distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
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
            int index = 1;

            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");
                XElement xePages = new XElement("pages");

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var mainProcesses = //RdStructureService.GetAdditionalParametersProcess(period);
                                    RdStructureService.GetMainParametersProcess(period, distance.Code);
             
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
                        var directName = AdditionalParametersService.DirectName(mainProcess.Id, (int)distance.Id);
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        index = 1;

                        xePages = new XElement("pages",
                                    new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                                    new XAttribute("direction", mainProcess.DirectionName),
                                    new XAttribute("road", road),
                                    new XAttribute("period", period.Period),
                                    new XAttribute("type", mainProcess.GetProcessTypeName),
                                    new XAttribute("car", mainProcess.Car),
                                    new XAttribute("chief", mainProcess.Chief),
                                    new XAttribute("ps", mainProcess.Car),
                                     new XAttribute("data", "" + mainProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                                    new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief));

                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", mainProcess.DirectionName + " (" + mainProcess.DirectionCode + "), Путь: " + trackName + ", ПЧ: " + distance.Code));

                        float sum = 0;
                        int StartM = -1;
                        var trackclasses = new List<TrackClass>();
                        var curves = new List<StCurve>();
                        var pdbSection = new List<PdbSection>();
                        var sector = "";
                        var previousKm = -1;
                        //список изостыков
                        var listIS = new List<int> { 8, 9, 10 };

                        var kms = RdStructureService.GetKilometerTrip(mainProcess.Trip_id);

                        foreach (var kmetr in kms)
                        {
                            if (kmetr < 129)
                            {
                                var c = 1;
                                continue;
                            }

                            if ((previousKm == -1) || (previousKm != kmetr))
                            {
                                trackclasses = (List<TrackClass>)MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, kmetr, MainTrackStructureConst.MtoTrackClass, track_id);
                                curves = (List<StCurve>)MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, kmetr, MainTrackStructureConst.MtoStCurve, track_id);
                                sector = MainTrackStructureService.GetSector(track_id, kmetr, mainProcess.Date_Vrem);
                                pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, kmetr, MainTrackStructureConst.MtoPdbSection, 
                                    mainProcess.DirectionName, trackName.ToString()) as List<PdbSection>;
                            }
                            previousKm = kmetr;

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

                            var startM = -1;
                            var RDGetShpal = AdditionalParametersService.RDGetShpalGap(mainProcess.Trip_id, (int)mainProcess.Direction, kmetr, epur_bpd == 2000 ? 520 : epur_bpd == 1840 ? 420 : 0);
                           
                            for(int i = 0; i <= RDGetShpal.Count - 2; i++)
                            {
                                startM = startM == -1 ? RDGetShpal[i].Meter : startM;

                                if(RDGetShpal[i].Meter == RDGetShpal[i + 1].Meter)
                                {
                                    //left
                                    if(RDGetShpal[i].Razn > RDGetShpal[i + 1].Razn)
                                    {
                                        if ((RDGetShpal[i + 1].Razn + RDGetShpal[i].Razn) * 1.4 < 300) continue;

                                        XElement xeElements = new XElement("elements",
                                            new XAttribute("n", index),
                                            new XAttribute("pchu", pdbSection.Count > 0 ? $"ПЧУ-{pdbSection[0].Pchu}/ПД-{pdbSection[0].Pd}/ПДБ-{pdbSection[0].Pdb}" : "Нет данных"),
                                            new XAttribute("station", sector),
                                            new XAttribute("km", RDGetShpal[i + 1].Km),
                                            new XAttribute("piket", RDGetShpal[i + 1].Meter / 100 + 1),
                                            new XAttribute("meter", RDGetShpal[i + 1].Meter),
                                            new XAttribute("thread", (Threat)RDGetShpal[i + 1].Threat == Threat.Left ? "левая" : (Threat)RDGetShpal[i + 1].Threat == Threat.Right ? "правая" : "неизвестно"),
                                            new XAttribute("joint", RDGetShpal[i + 1].Razn ),
                                            new XAttribute("joint1", ((RDGetShpal[i + 1].Razn + RDGetShpal[i].Razn)*1.4).ToString("0")),
                                            new XAttribute("betweendistance", epur_bpd == 2000 ? 520 : epur_bpd == 1840 ? 420 : 0),
                                            new XAttribute("notice", listIS.Contains(RDGetShpal[i + 1].Oid) ? "ис" :""));

                                        if((Threat)RDGetShpal[i + 1].Threat == Threat.Left || (Threat)RDGetShpal[i + 1].Threat == Threat.Right)
                                        {
                                            xeTracks.Add(xeElements);
                                            index++;
                                        }
                                        
                                    }
                                    else
                                    {
                                        if ((RDGetShpal[i].Razn + RDGetShpal[i + 1].Razn) *1.4 < 300) continue;

                                        XElement xeElements = new XElement("elements",
                                            new XAttribute("n", index),
                                            new XAttribute("pchu", pdbSection.Count > 0 ? $"ПЧУ-{pdbSection[0].Pchu}/ПД-{pdbSection[0].Pd}/ПДБ-{pdbSection[0].Pdb}" : "Нет данных"),
                                            new XAttribute("station", sector),
                                            new XAttribute("km", RDGetShpal[i].Km),
                                            new XAttribute("piket", RDGetShpal[i].Meter / 100 + 1),
                                            new XAttribute("meter", RDGetShpal[i].Meter),
                                            new XAttribute("thread", (Threat)RDGetShpal[i].Threat == Threat.Left ? "левая" : (Threat)RDGetShpal[i].Threat == Threat.Right ? "правая" : "неизвестно"),
                                            new XAttribute("joint", RDGetShpal[i].Razn ),
                                            new XAttribute("joint1", ((RDGetShpal[i].Razn + RDGetShpal[i+1].Razn) * 1.4).ToString("0")),
                                            new XAttribute("betweendistance", epur_bpd == 2000 ? 520 : epur_bpd == 1840 ? 420 : 0),
                                            new XAttribute("notice", listIS.Contains(RDGetShpal[i].Oid) ? "ис" : ""));
                                        if ((Threat)RDGetShpal[i].Threat == Threat.Left || (Threat)RDGetShpal[i].Threat == Threat.Right)
                                        {
                                            xeTracks.Add(xeElements);
                                            index++;
                                        }
                                    }
                                    //right
                                    if (RDGetShpal[i].R_razn > RDGetShpal[i + 1].R_razn)
                                    {
                                        if ((RDGetShpal[i + 1].R_razn + RDGetShpal[i].R_razn) * 1.4 < 300) continue;

                                        XElement xeElements = new XElement("elements",
                                            new XAttribute("n", index),
                                            new XAttribute("pchu", pdbSection.Count > 0 ? $"ПЧУ-{pdbSection[0].Pchu}/ПД-{pdbSection[0].Pd}/ПДБ-{pdbSection[0].Pdb}" : "Нет данных"),
                                            new XAttribute("station", sector),
                                            new XAttribute("km", RDGetShpal[i + 1].R_km),
                                            new XAttribute("piket", RDGetShpal[i + 1].R_meter / 100 + 1),
                                            new XAttribute("meter", RDGetShpal[i + 1].R_meter),
                                            new XAttribute("thread", (Threat)RDGetShpal[i + 1].R_threat == Threat.Left ? "левая" : (Threat)RDGetShpal[i + 1].R_threat == Threat.Right ? "правая" : "неизвестно"),
                                            new XAttribute("joint", RDGetShpal[i + 1].R_razn ),
                                            new XAttribute("joint1", ((RDGetShpal[i + 1].R_razn + RDGetShpal[i].R_razn) * 1.4).ToString("0")),
                                            new XAttribute("betweendistance", epur_bpd == 2000 ? 520 : epur_bpd == 1840 ? 420 : 0),
                                            new XAttribute("notice", listIS.Contains(RDGetShpal[i + 1].R_oid) ? "ис" : ""));
                                        if ((Threat)RDGetShpal[i + 1].R_threat == Threat.Left || (Threat)RDGetShpal[i + 1].R_threat == Threat.Right)
                                        {
                                            xeTracks.Add(xeElements);
                                            index++;
                                        }
                                    }
                                    else
                                    {

                                        if ((RDGetShpal[i].R_razn + RDGetShpal[i + 1].R_razn) * 1.4 < 300) continue;

                                        XElement xeElements = new XElement("elements",
                                            new XAttribute("n", index),
                                            new XAttribute("pchu", pdbSection.Count > 0 ? $"ПЧУ-{pdbSection[0].Pchu}/ПД-{pdbSection[0].Pd}/ПДБ-{pdbSection[0].Pdb}" : "Нет данных"),
                                            new XAttribute("station", sector),
                                            new XAttribute("km", RDGetShpal[i].R_km),
                                            new XAttribute("piket", RDGetShpal[i].R_meter / 100 + 1),
                                            new XAttribute("meter", RDGetShpal[i].R_meter),
                                            new XAttribute("thread", (Threat)RDGetShpal[i].R_threat == Threat.Left ? "левая" : (Threat)RDGetShpal[i].R_threat == Threat.Right ? "правая" : "неизвестно"),
                                            new XAttribute("joint", RDGetShpal[i].R_razn ),
                                            new XAttribute("joint1", ((RDGetShpal[i].R_razn + RDGetShpal[i+1].R_razn) * 1.4).ToString("0")),
                                            new XAttribute("betweendistance", epur_bpd == 2000 ? 520 : epur_bpd == 1840 ? 420 : 0),
                                            new XAttribute("notice", listIS.Contains(RDGetShpal[i].R_oid) ? "ис" : ""));
                                        if ((Threat)RDGetShpal[i].R_threat == Threat.Left || (Threat)RDGetShpal[i].R_threat == Threat.Right)
                                        {
                                            xeTracks.Add(xeElements);
                                            index++;
                                        }
                                    }
                                }

                            }
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
                htReport.Save(Path.GetTempPath() + "/report_EpureSleepers.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_EpureSleepers.html");
            }
        }
    }
}
