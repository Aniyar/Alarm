using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
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
    public class DeviationOf2CloseTo3 : Report
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
                List<Curve> curves = (MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
                XDocument xdReport = new XDocument();

                var distance =
                    AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);


                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(track_id);

                        List<Digression> notes = RdStructureService.GetDigressions(tripProcess.Trip_id, distance.Code, new int[] { 2 });

                        List<Gap> check_gap_state = AdditionalParametersService.Check_gap_state(tripProcess.Trip_id, template.ID);



                        var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;

                        CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;

                        var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                        if (kms.Count() == 0) continue;
                        var directName = AdditionalParametersService.DirectName(tripProcess.Id, (int)tripProcess.Direction);

                        XElement tripElem = new XElement("trip",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                            new XAttribute("direction", tripProcess.DirectionName),
                            new XAttribute("km", kms.Min() + "-" + kms.Max()),
                            new XAttribute("track", trackName),
                            new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                            new XAttribute("road", roadName),
                            new XAttribute("distance", distance.Code),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("ps", tripProcess.Car)
                        );


                        //notes = notes.Where(o => o.Km >= 129 && o.Km <= 147).ToList();


                        //Участки дист коррекция
                        var dist_section = MainTrackStructureService.GetDistSectionByDistId(distance.Id);

                       

                        foreach (var item in notes)
                        {

                            var ds = dist_section.Where(
                                o => item.Km * 1000 + item.Meter >= o.Start_Km * 1000 + o.Start_M && item.Km * 1000 + item.Meter <= o.Final_Km * 1000 + o.Final_M).ToList();


                            item.PCHU = ds.First().Pchu.ToString();
                            item.PD = ds.First().Pd.ToString();
                            item.PDB = ds.First().Pdb.ToString();
                            //item.Primech = ds.First().P.ToString();
                        }

                        string previousDirectionName = string.Empty;
                        string previousTrackName = string.Empty;
                        string previousPCHUName = string.Empty;
                        string previousPDName = string.Empty;
                        string previousPDBName = string.Empty;

                        XElement directionElement = new XElement("direction");
                        XElement tracklElement = new XElement("track");
                        XElement PCHUElement = new XElement("PCHU");
                        XElement PDElement = new XElement("PD");
                        XElement PDBElement = new XElement("PDB");


                        int directionRecordCount = 0;
                        int trackCount = 0;
                        int PCHUCount = 0;
                        int PDCount = 0;
                        int PDBCount = 0;

                        int totalCount = 0;
                        int constrictionCount = 0;
                        int broadeningCount = 0;
                        int levelCount = 0;
                        int skewnessCount = 0;
                        int drawdownCount = 0;
                        int straighteningCount = 0;


                        var prev_km = -1;
                        var Artificials = new List<ArtificialConstruction> { };

                        foreach (var note in notes)
                        {
                            //Исскуст соорр - мосты
                            if (note.Km != prev_km)
                            {
                                Artificials = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, note.Km, 
                                    MainTrackStructureConst.MtoArtificialConstruction, track_id) as List<ArtificialConstruction>;
                            }
                            var temp_bridge = Artificials.Where(o => o.Start_Km * 1000 + o.Start_M <= note.Km * 1000 + note.Meter &&
                                                                     o.Final_Km * 1000 + o.Final_M >= note.Km * 1000 + note.Meter).ToList();
                            if (temp_bridge.Any())
                            {
                                note.Primech += " мост";
                            }
                            prev_km = note.Km;

                            //стыки
                            // запрос списка Изостыков с БПД
                            var IzoGaps = MainTrackStructureService.GetIzoGaps(trackName, tripProcess.DirectionID);
                            var temp_gap = IzoGaps.Where(o => o.Km * 1000 + o.Meter == note.Km * 1000 + note.Meter).ToList();
                            if (temp_gap.Any())
                            {
                                note.Primech += "из.стык";
                            }
                            

                            if (previousDirectionName.Equals(string.Empty))
                                previousDirectionName = note.Direction;

                            if (previousTrackName.Equals(string.Empty))
                                previousTrackName = note.Track;

                            if (previousPCHUName.Equals(string.Empty))
                                previousPCHUName = note.Pchu;

                            if (previousPDName.Equals(string.Empty))
                                previousPDName = note.PD;

                            if (previousPDBName.Equals(string.Empty))
                                previousPDBName = note.PDB;

                            if (!previousDirectionName.Equals(note.Direction))
                            {
                                directionElement.Add(new XAttribute("recordCount", directionRecordCount));
                                directionElement.Add(new XAttribute("name", previousDirectionName));
                                directionElement.Add(
                                    new XAttribute("direction", tripProcess.DirectionName),
                                      new XAttribute("directioncode", note.Direction));

                                tracklElement.Add(new XAttribute("name", previousTrackName));
                                tracklElement.Add(new XAttribute("recordCount", trackCount));

                                PCHUElement.Add(new XAttribute("number", previousPCHUName));
                                PCHUElement.Add(new XAttribute("recordCount", PCHUCount));

                                PDElement.Add(new XAttribute("number", previousPDName));
                                PDElement.Add(new XAttribute("recordCount", PDCount));

                                PDBElement.Add(new XAttribute("number", previousPDBName));
                                PDBElement.Add(new XAttribute("recordCount", PDBCount));

                                PDElement.Add(PDBElement);
                                PCHUElement.Add(PDElement);
                                tracklElement.Add(PCHUElement);
                                directionElement.Add(tracklElement);
                                tripElem.Add(directionElement);

                                directionRecordCount = 0;
                                trackCount = 0;
                                PCHUCount = 0;
                                PDCount = 0;
                                PDBCount = 0;

                                previousTrackName = string.Empty;
                                previousPCHUName = string.Empty;
                                previousPDName = string.Empty;
                                previousPDBName = string.Empty;

                                directionElement = new XElement("direction");
                                tracklElement = new XElement("track");
                                PCHUElement = new XElement("PCHU");
                                PDElement = new XElement("PD");
                                PDBElement = new XElement("PDB");

                                previousDirectionName = note.Direction;
                            }

                            if (!previousTrackName.Equals(note.Track) && !previousTrackName.Equals(string.Empty))
                            {
                                tracklElement.Add(new XAttribute("name", previousTrackName));
                                tracklElement.Add(new XAttribute("recordCount", trackCount));

                                tracklElement.Add(
                                  new XAttribute("direction", tripProcess.DirectionName),
                                    new XAttribute("directioncode", note.Direction));

                                PCHUElement.Add(new XAttribute("number", previousPCHUName));
                                PCHUElement.Add(new XAttribute("recordCount", PCHUCount));

                                PDElement.Add(new XAttribute("number", previousPDName));
                                PDElement.Add(new XAttribute("recordCount", PDCount));

                                PDBElement.Add(new XAttribute("number", previousPDBName));
                                PDBElement.Add(new XAttribute("recordCount", PDBCount));

                                PDElement.Add(PDBElement);
                                PCHUElement.Add(PDElement);
                                tracklElement.Add(PCHUElement);
                                directionElement.Add(tracklElement);

                                trackCount = 0;
                                PCHUCount = 0;
                                PDCount = 0;
                                PDBCount = 0;

                                previousPCHUName = string.Empty;
                                previousPDName = string.Empty;
                                previousPDBName = string.Empty;

                                tracklElement = new XElement("track");
                                PCHUElement = new XElement("PCHU");
                                PDElement = new XElement("PD");
                                PDBElement = new XElement("PDB");

                                previousTrackName = note.Track;

                            }

                            if (!previousPCHUName.Equals(note.Pchu) && !previousPCHUName.Equals(string.Empty))
                            {
                                PCHUElement.Add(new XAttribute("number", previousPCHUName));
                                PCHUElement.Add(new XAttribute("recordCount", PCHUCount));

                                PCHUElement.Add(
                                  new XAttribute("direction", tripProcess.DirectionName),
                                    new XAttribute("directioncode", note.Direction));

                                PDElement.Add(new XAttribute("number", previousPDName));
                                PDElement.Add(new XAttribute("recordCount", PDCount));

                                PDBElement.Add(new XAttribute("number", previousPDBName));
                                PDBElement.Add(new XAttribute("recordCount", PDBCount));

                                PDElement.Add(PDBElement);
                                PCHUElement.Add(PDElement);
                                tracklElement.Add(PCHUElement);

                                PCHUCount = 0;
                                PDCount = 0;
                                PDBCount = 0;

                                previousPDName = string.Empty;
                                previousPDBName = string.Empty;

                                PCHUElement = new XElement("PCHU");
                                PDElement = new XElement("PD");
                                PDBElement = new XElement("PDB");

                                previousPCHUName = note.PCHU;
                            }

                            if (!previousPDName.Equals(note.PD) && !previousPDName.Equals(string.Empty))
                            {
                                PDElement.Add(new XAttribute("number", previousPDName));
                                PDElement.Add(new XAttribute("recordCount", PDCount));

                                PDBElement.Add(new XAttribute("number", previousPDBName));
                                PDBElement.Add(new XAttribute("recordCount", PDBCount));

                                PDElement.Add(PDBElement);
                                PCHUElement.Add(PDElement);

                                PDCount = 0;
                                PDBCount = 0;


                                previousPDBName = string.Empty;

                                PDBElement = new XElement("PDB");
                                PDElement = new XElement("PD");

                                previousPDName = note.PD;
                            }

                            if (!previousPDBName.Equals(note.PDB) && !previousPDBName.Equals(string.Empty))
                            {
                                PDBElement.Add(new XAttribute("number", previousPDBName));
                                PDBElement.Add(new XAttribute("recordCount", PDBCount));
                                PDElement.Add(PDBElement);

                                PDBCount = 0;

                                PDBElement = new XElement("PDB");

                                previousPDBName = note.PDB;
                            }

                            int count = 0;
                            switch (note.Name)
                            {
                                case "Пр.п":
                                case "Пр.л":
                                    drawdownCount += note.Count;
                                    break;
                                case "Суж":
                                  

                                    continue;
                                case "Уш":

                                    continue;
                                case "У":
                                    note.Count = note.Length / 10 + (note.Length % 10 + 1 > 1 ? 1 : 0);
                                    levelCount += note.Count;
                                    break;
                                case "П":
                                    skewnessCount += note.Count;
                                    break;
                                case "Р":
                                    straighteningCount += note.Count;
                                    break;
                            }


                            PDBElement.Add(new XElement("NOTE",
                                new XAttribute("founddate", note.FoundDate.ToString("dd.MM.yyyy")),
                                new XAttribute("km", note.Km),
                                new XAttribute("meter", note.Meter),
                                new XAttribute("digression", note.Name),
                                new XAttribute("value", note.Value),
                                new XAttribute("length", note.Length),
                                new XAttribute("count", note.Count),
                                new XAttribute("primech", note.Primech)
                                //new XAttribute("primech", ((item.Km == note.Km ? item.Meter <= note.Meter) ? (note.Meter < (item.Length + item.Meter))) ? item.Zazor.ToString() : "стык"),
                                //new XAttribute("primech", (item.Km == note.Km && note.Meter <= item.Meter && item.Meter <= (note.Meter + note.Length)) ? item.Zazor.ToString() : "стык")
                                ));

                            directionRecordCount += 1;
                            trackCount += 1;
                            PCHUCount += 1;
                            PDCount += 1;
                            PDBCount += 1;

                            totalCount += 1;

                        }
                        directionElement.Add(new XAttribute("name", previousDirectionName + previousTrackName));
                        directionElement.Add(new XAttribute("recordCount", directionRecordCount));

                        directionElement.Add(
                           new XAttribute("direction", tripProcess.DirectionName),
                           new XAttribute("directioncode", tripProcess.DirectionCode),
                             new XAttribute("track", previousTrackName));

                        tracklElement.Add(new XAttribute("name", previousTrackName));
                        tracklElement.Add(new XAttribute("recordCount", trackCount));

                        PCHUElement.Add(new XAttribute("number", previousPCHUName));
                        PCHUElement.Add(new XAttribute("recordCount", PCHUCount));

                        PDElement.Add(new XAttribute("number", previousPDName));
                        PDElement.Add(new XAttribute("recordCount", PDCount));

                        PDBElement.Add(new XAttribute("number", previousPDBName));
                        PDBElement.Add(new XAttribute("recordCount", PDBCount));

                        PDElement.Add(PDBElement);
                        PCHUElement.Add(PDElement);
                        tracklElement.Add(PCHUElement);
                        directionElement.Add(tracklElement);
                        tripElem.Add(directionElement);


                        tripElem.Add(new XAttribute("drawdownCount", drawdownCount));
                      
                        tripElem.Add(new XAttribute("levelCount", levelCount));
                        tripElem.Add(new XAttribute("skewnessCount", skewnessCount));
                        tripElem.Add(new XAttribute("straighteningCount", straighteningCount));
                        tracklElement.Add(new XAttribute("totalCount", drawdownCount   + levelCount + skewnessCount + straighteningCount));


                        //В том числе:
                        if (drawdownCount > 0)
                        {
                            tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Пр - " + drawdownCount)));
                        }
                   
                        if (levelCount > 0)
                        {
                            tripElem.Add(new XElement("total", new XAttribute("totalinfo", "У - " + levelCount)));
                        }
                        if (skewnessCount > 0)
                        {
                            tripElem.Add(new XElement("total", new XAttribute("totalinfo", "П - " + skewnessCount)));
                        }
                        if (straighteningCount > 0)
                        {
                            tripElem.Add(new XElement("total", new XAttribute("totalinfo", "Р - " + straighteningCount)));
                        }

                        directionElement.Add(new XAttribute("countDistance", directionRecordCount));
                        directionElement.Add(new XAttribute("distance", distance.Code));
                        



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
                htReport.Save($@"G:\form\6.Выходные формы Основные параметры\Отступления 2 степени, близкие к 3.html");
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