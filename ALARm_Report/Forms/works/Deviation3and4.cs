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
    public class Deviation3and4 : Report
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

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                var npch = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId);

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
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kms = RdStructureService.GetKilometersByTrip(trip);
                        if (!kms.Any()) continue;

                        kms = kms.Where(o => o.Track_id == track_id).ToList();

                        trip.Track_Id = track_id;
                        var lkm = kms.Select(o => o.Number).ToList();

                        if (lkm.Count() == 0) continue;
                        var ListS3 = RdStructureService.GetS3(track_id, 3, distance.Name) as List<S3>;


                        List<Digression> notes = RdStructureService.GetDigressions3and4(tripProcess.Trip_id, distance.Code, new int[] { 3, 4 });
                        var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;

                        CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;

                        //var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                        //if (kms.Count() == 0) continue;




                        //Участки дист коррекция
                        var dist_section = MainTrackStructureService.GetDistSectionByDistId(distance.Id);
                        foreach (var item in notes)
                        {
                            var ds = dist_section.Where(
                                o => item.Km * 1000 + item.Meter >= o.Start_Km * 1000 + o.Start_M && item.Km * 1000 + item.Meter <= o.Final_Km * 1000 + o.Final_M).ToList();

                            item.PCHU = ds.First().Pchu.ToString();
                            item.PD = ds.First().Pd.ToString();
                            item.PDB = ds.First().Pdb.ToString();
                        }

                        XElement tripElem = new XElement("trip",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                            //new XAttribute("direction", curvesAdmUnit.Direction),
                            new XAttribute("km", lkm.Min() + "-" + lkm.Max()),
                            new XAttribute("check", tripProcess.GetProcessTypeName), //ToDo
                            new XAttribute("road", roadName),
                            new XAttribute("track", curvesAdmUnit.Track),
                            new XAttribute("distance", distance.Code),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("ps", tripProcess.Car),
                            new XAttribute("way", trackName),
                            new XAttribute("direction", $"{tripProcess.DirectionName}({tripProcess.DirectionCode})"),
                            new XAttribute("trip_date", tripProcess.Trip_date)
                        //,
                        //new XAttribute("km", notes[0].Km + " - " + notes[notes.Count - 1].Km)

                        );


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
                        int GrCount = 0;
                        int straighteningCount = 0;

                        int stepen3 = 0, stepen4 = 0;
                        var prevKm = -1;

                        foreach (var note in notes)
                        {
                            List<String> All = new List<String>(note.AllowSpeed.Split("/".ToCharArray()));
                            List<String> Full = new List<String>(note.FullSpeed.Split("/".ToCharArray()));

                            note.AllowSpeed = $"{All[0]}/{All[1]}";
                            note.FullSpeed = $"{Full[0]}/{Full[1]}";

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
                            int count = 1;
                            switch (note.Name)
                            {
                                case "Пр.п":
                                case "Пр.л":
                                    count = note.Count;
                                    break;
                                case "Суж":
                                    count = note.Length / 4;
                                    count += note.Length % 4 > 0 ? 1 : 0;
                                    break;
                                case "Уш":
                                    count = note.Length / 4;
                                    count += note.Length % 4 > 0 ? 1 : 0;
                                    break;
                                case "У":
                                    count = note.Length / 10;
                                    count += note.Length % 10 > 0 ? 1 : 0;
                                    break;
                                case "П":
                                    count = note.Count;
                                    break;
                                case "Р":
                                    count = note.Count;
                                    break;
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

                            note.Norma = note.Name.Equals("Уш") ? "Ншк=" + note.Norma : String.Empty;

                            if (prevKm != note.Km)
                            {
                                if (prevKm != -1)
                                {
                                    PDBElement.Add(new XElement("NOTE",
                                    new XAttribute("founddate", ""),
                                    new XAttribute("km", prevKm),
                                    new XAttribute("meter", ""),
                                    new XAttribute("digression", "3 ст: " + stepen3 + ", 4 ст: " + stepen4),
                                    //new XAttribute("digression", "3 ст: " + stepen3 + ", 4 ст: " + stepen4),
                                    new XAttribute("value", ""),
                                    new XAttribute("length", ""),
                                    new XAttribute("count", ""),
                                    new XAttribute("typ", ""),
                                    new XAttribute("fullSpeed", ""),
                                    new XAttribute("allowSpeed", ""),
                                    new XAttribute("norma", "")
                                    ));
                                    stepen3 = 0;
                                    stepen4 = 0;
                                }
                            }

                            if ((int)note.Typ == 3 || (int)note.Typ == 4)
                            {
                                PDBElement.Add(new XElement("NOTE",
                                new XAttribute("founddate", note.FoundDate),
                                new XAttribute("km", note.Km),
                                new XAttribute("meter", note.Meter),
                                new XAttribute("digression", note.Name),
                                new XAttribute("value", note.Value),
                                new XAttribute("length", note.Length + "(" + count + ")"),
                                new XAttribute("count", note.Count),
                                new XAttribute("typ", note.Typ),
                                new XAttribute("fullSpeed", note.FullSpeed.Replace("-1", "-")),
                                new XAttribute("allowSpeed", note.Primech.Contains("гр-/60") ? "-/60" : note.AllowSpeed.Replace("-1", "-")),
                                new XAttribute("norma", note.Primech.Contains("м;") ? "мост" : note.Primech.Contains("Стр") ? "Стр" : note.Primech.Contains("гр-/60") ? "гр" : note.Primech.Replace(";", ""))
                                ));
                            }


                            prevKm = note.Km;
                            if ((int)note.Typ == 3)
                            {
                                stepen3 = stepen3 + count;
                            }
                            if ((int)note.Typ == 4)
                            {
                                stepen4 = stepen4 + count;
                            }

                            directionRecordCount += 1;
                            trackCount += 1;
                            PCHUCount += 1;
                            PDCount += 1;
                            PDBCount += 1;
                            switch (note.Name)
                            {
                                case "Пр.п":
                                case "Пр.л":
                                    drawdownCount += 1;
                                    break;
                              
                                case "Суж":
                                    constrictionCount += 1;
                                    break;
                                case "Уш":
                                    broadeningCount += 1;
                                    break;
                                case "У":
                                    levelCount += 1;
                                    break;
                                case "П":
                                    skewnessCount += 1;
                                    break;
                                case "Р":
                                    straighteningCount += 1;
                                    break;
                            }

                            totalCount += 1;


                        }
                        directionElement.Add(new XAttribute("name", previousDirectionName));
                        directionElement.Add(new XAttribute("recordCount", directionRecordCount));

                        tracklElement.Add(new XAttribute("name", previousTrackName));
                        tracklElement.Add(new XAttribute("recordCount", trackCount));

                        PCHUElement.Add(new XAttribute("number", previousPCHUName));
                        PCHUElement.Add(new XAttribute("recordCount", PCHUCount));

                        PDElement.Add(new XAttribute("number", previousPDName));
                        PDElement.Add(new XAttribute("recordCount", PDCount));

                        PDBElement.Add(new XAttribute("number", previousPDBName));
                        PDBElement.Add(new XAttribute("recordCount", PDBCount));

                        PDBElement.Add(new XElement("NOTE",
                                    new XAttribute("founddate", ""),
                                    new XAttribute("km", prevKm),
                                    new XAttribute("meter", ""),
                                    new XAttribute("digression", "3 ст: " + stepen3 + ", 4 ст: " + stepen4),
                                    new XAttribute("value", ""),
                                    new XAttribute("length", ""),
                                    new XAttribute("count", ""),
                                    new XAttribute("typ", ""),
                                    new XAttribute("fullSpeed", ""),
                                    new XAttribute("allowSpeed", ""),
                                    new XAttribute("norma", "")
                                    ));

                        PDElement.Add(PDBElement);
                        PCHUElement.Add(PDElement);
                        tracklElement.Add(PCHUElement);
                        tracklElement.Add(new XAttribute("trackinfo", $"{tripProcess.DirectionName}({tripProcess.DirectionCode})" + " / Путь: " + $"{trackName}" + " / ПЧ: " + distance.Code));
                        directionElement.Add(tracklElement);
                        tripElem.Add(directionElement);


                        tripElem.Add(new XAttribute("drawdownCount", drawdownCount));
                        tripElem.Add(new XAttribute("constrictionCount", constrictionCount));
                        tripElem.Add(new XAttribute("broadeningCount", broadeningCount));
                        tripElem.Add(new XAttribute("levelCount", levelCount));
                        tripElem.Add(new XAttribute("skewnessCount", skewnessCount));
                        tripElem.Add(new XAttribute("straighteningCount", straighteningCount));
                        tripElem.Add(new XAttribute("GrCount", GrCount));
                        tripElem.Add(new XAttribute("totalCount", drawdownCount + constrictionCount + broadeningCount + levelCount + skewnessCount + straighteningCount ));
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