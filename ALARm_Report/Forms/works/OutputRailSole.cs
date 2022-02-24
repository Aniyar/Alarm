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
using ALARm.Core.AdditionalParameteres;
using System.Linq;
using System.Globalization;

namespace ALARm_Report.Forms
{
    public class OutputRailSole : Report
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

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;

                var mainProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);

                progressBar.Maximum = mainProcesses.Count;

                foreach (var mainProcess in mainProcesses)
                {
                    progressBar.Value = mainProcesses.IndexOf(mainProcess) + 1;

                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        var GetRAilSole = RdStructureService.GetRAilSole(mainProcess.Trip_id, false, distance.Code, trackName);
                        
                        if (/*GetRAilSole.Count < 1 ||*/ GetRAilSole == null) continue;

                        GetRAilSole = GetRAilSole.Where(o => o.Razn > 10 && o.Km >128).ToList();


                        int countSl = 1;
                        int prevM = -1;
                        int prevThreat = -1;
                        var digList = new List<RailFastener>();

                        for (int i = 0; i <= GetRAilSole.Count - 2; i++)
                        {
                            prevM = prevM == -1 ? GetRAilSole[i].Km * 1000 + GetRAilSole[i].Mtr : prevM;
                            prevThreat = prevThreat == -1 ? (int)GetRAilSole[i].Threat : prevThreat;

                            var nextM = GetRAilSole[i + 1].Km * 1000 + GetRAilSole[i + 1].Mtr;
                            var nextThreat = (int)GetRAilSole[i + 1].Threat;


                            if (Math.Abs(prevM - nextM) < 2)
                            {
                                if (prevThreat == nextThreat)
                                {
                                    prevM = nextM;
                                    countSl++;
                                }
                                else
                                {
                                    if (countSl > 3)
                                    {
                                        digList.Add(GetRAilSole[i]);
                                        digList[digList.Count - 1].Count = countSl;

                                        prevM = nextM;
                                        countSl = 1;
                                    }
                                }
                            }
                            else if (countSl > 3)
                            {
                                if (prevThreat != nextThreat)
                                {
                                    digList.Add(GetRAilSole[i]);
                                    digList[digList.Count - 1].Count = countSl;

                                    prevM = nextM;
                                    countSl = 1;
                                }
                                else
                                {
                                    prevM = nextM;
                                    countSl++;
                                }
                            }
                            else
                            {
                                prevM = nextM;
                                countSl = 1;
                            }
                        }

                        if (mainProcess.Id == lastProcess)
                        {
                            XElement xeTracks = new XElement("tracks",
                                new XAttribute("trackinfo", $"{mainProcess.DirectionName}({mainProcess.DirectionCode}) / Путь: {trackName} / ПЧ: { distance.Code }"));

                            RailFastener prev_digression = null;
                            List<Curve> curves = new List<Curve> { };
                            var sector = "";
                            var speed = new List<Speed> { };

                            foreach (var finddeg in GetRAilSole)
                            {
                                if ((prev_digression == null) || (prev_digression.Km != finddeg.Km))
                                {
                                    //string tripplan = finddeg.CurveRadius != 0 ? "кривая R-" + finddeg.CurveRadius.ToString() : "прямой";
                                    //string amount = finddeg.Length.ToString() + " шт";
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
                                    new XAttribute("speed", speed.Count > 0 ? speed[0].ToString() : ""),
                                    new XAttribute("digression", finddeg.Digressions[0].GetName()),
                                    new XAttribute("amount", /*amount*/""),
                                    new XAttribute("thread", finddeg.Threat == Threat.Left ? "левая" : "правая"),
                                    new XAttribute("fastening", finddeg.ToString()),
                                    new XAttribute("tripplan", curves.Count > 0 ? $"кривая R-{curves[0].Radius}" : "прямой"),
                                    new XAttribute("speed2", /*finddeg.AllowSpeed*/""),
                                    new XAttribute("notice", ""),
                                    new XAttribute("fileId", finddeg.file_id),
                                    new XAttribute("Ms", finddeg.Ms),
                                    new XAttribute("fNum", finddeg.Fnum)

                                    );
                                xeTracks.Add(xeElements);
                                index++;

                                prev_digression = finddeg;
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
                                new XAttribute("car", "" + mainProcess.Car),
                                new XAttribute("chief", mainProcess.Chief),
                                new XAttribute("ps", mainProcess.Car),
                                //new XAttribute("data", "" + mainProcess.Date_Vrem.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))),
                                new XAttribute("data", "" + mainProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                                new XAttribute("info", "ПС: " + mainProcess.Car + " " + mainProcess.Chief));

                            XElement xeTracks = new XElement("tracks",
                                new XAttribute("trackinfo", $"{mainProcess.DirectionName}({mainProcess.DirectionCode}) / Путь: {trackName} / ПЧ: { distance.Code }"));

                            RailFastener prev_digression = null;
                            List<Curve> curves = new List<Curve> { };
                            var sector = "";
                            var speed = new List<Speed> { };

                            foreach (var finddeg in digList)
                            {
                                if ((prev_digression == null) || (prev_digression.Km != finddeg.Km))
                                {
                                    //string tripplan = finddeg.CurveRadius != 0 ? "кривая R-" + finddeg.CurveRadius.ToString() : "прямой";
                                    //string amount = finddeg.Length.ToString() + " шт";
                                    sector = MainTrackStructureService.GetSector(track_id, finddeg.Km, mainProcess.Date_Vrem);
                                    speed = MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, finddeg.Km, MainTrackStructureConst.MtoSpeed, mainProcess.DirectionName, trackName.ToString()) as List<Speed>;
                                    curves = MainTrackStructureService.GetMtoObjectsByCoord(mainProcess.Date_Vrem, finddeg.Km, MainTrackStructureConst.MtoCurve, track_id) as List<Curve>;
                                }

                                var curve = curves.Count > 0 ? (int)curves[0].Radius : -1;

                                var ogr = "";

                                switch (finddeg.Count)
                                {
                                    case int c when c == 3:
                                        ogr = "60/60";
                                        break;
                                    case int c when c == 4:
                                        ogr = "40/40    ";
                                        break;
                                    case int c when c == 5:
                                        ogr = "0/0";
                                        break;
                                    default:
                                        ogr = "";
                                        break;
                                }

                                XElement xeElements = new XElement("elements",
                                    new XAttribute("n", index),
                                    new XAttribute("pchu", finddeg.PdbSection),
                                    new XAttribute("station", sector),
                                    new XAttribute("km", finddeg.Km),
                                    new XAttribute("piket", finddeg.Mtr / 100 + 1),
                                    new XAttribute("meter", finddeg.Mtr),
                                    new XAttribute("speed", speed.Count > 0 ? speed[0].ToString() : ""),
                                    new XAttribute("digression", "Выход подошвы рельса из реборд подкладки"),
                                    new XAttribute("amount", countSl),
                                    new XAttribute("thread", finddeg.Threat == Threat.Left ? "левая" : "правая"),
                                    new XAttribute("fastening", /*finddeg.ToString()*/ "КБ-65"),
                                    new XAttribute("tripplan", curves.Count > 0 ? $"кривая R-{curves[0].Radius}" : "прямой"),
                                    new XAttribute("speed2", ogr),
                                    new XAttribute("notice", ""),
                                    new XAttribute("fileId", finddeg.file_id),
                                    new XAttribute("Ms", finddeg.Ms),
                                    new XAttribute("fNum", finddeg.Fnum)
                                    );
                                xeTracks.Add(xeElements);
                                index++;

                                prev_digression = finddeg;
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
                
                htReport.Save(Path.GetTempPath() + "/report_OutputRailSole.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_OutputRailSole.html");
            }
        }
    }
}
