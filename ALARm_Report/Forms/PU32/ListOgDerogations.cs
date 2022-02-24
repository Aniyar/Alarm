 using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
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

namespace ALARm_Report.Forms
{
    public class ListOgDerogations : Report
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

                var distance =
                    AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                var videoProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                //var tripProcesses = RdStructureService.GetMainParametersProcesses(period, distance.Name);
                if (videoProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                XElement report = new XElement("report");
                foreach (var tripProcess in videoProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        var longrails = RdStructureService.GetTripSections(track_id, period.StartDate, MainTrackStructureConst.MtoLongRails) as List<LongRails>;

                        if (longrails.Count < 1)
                            continue;
                        foreach (var longrail in longrails)
                        {
                            var Plet = "";
                            var sector = "";
                            var trackName = AdmStructureService.GetTrackName(track_id);
                            var pdbSection = new List<PdbSection>();
                            //List<Gap> Check_ListOgDerogations_state = new List<Gap> { };
                            List<Gap> Check_ListOgDerogations_state = AdditionalParametersService.Check_ListOgDerogations_state(tripProcess.Trip_id, template.ID);
                            //AdditionalParametersService.Check_ListOgDerogations_state(tripProcess.Trip_id, template.ID);
                            //if (Check_ListOgDerogations_state == null || Check_ListOgDerogations_state.Count == 0)
                            {
                                XElement tripElem = new XElement("trip",
                                    new XAttribute("date_statement", tripProcess.Date_Vrem.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))),
                                    new XAttribute("type", tripProcess.GetProcessTypeName), //ToDo
                                    new XAttribute("road", roadName),
                                    new XAttribute("distance", distance.Code),
                                    new XAttribute("periodDate", $"{period.Period}"),
                                    new XAttribute("chief", tripProcess.Chief),
                                    new XAttribute("ps", tripProcess.Car),
                                    new XAttribute("trip_date", tripProcess.Trip_date)
                                    );
                                XElement direction = new XElement("direction",
                                    new XAttribute("name", tripProcess.DirectionName + " (" + tripProcess.DirectionCode + "), Путь: " + trackName + ", ПЧ: " + distance.Code));

                                var GetMaech = AdditionalParametersService.GetMaech(tripProcess.Trip_id, (int)tripProcess.Direction);

                                //if (GetMaech == null || GetMaech.Count() == 0) continue;

                                GetMaech = GetMaech.Where(o => o.Km * 1000 + o.Meter >= longrail.Start_Km * 1000 + longrail.Start_M &&
                                                             o.Km * 1000 + o.Meter <= longrail.Final_Km * 1000 + longrail.Final_M).ToList();

                                var speed = new List<Speed>();
                                var previousGap = 0;
                                var temperature = new List<Temperature>();
                                int iter = 1;

                                foreach (var elem in GetMaech)
                                {
                                    if ((previousGap == null) || (previousGap != elem.Km))
                                    {
                                        temperature = MainTrackStructureService.GetTemp(tripProcess.Trip_id, track_id, elem.Km);
                                        sector = MainTrackStructureService.GetSector(track_id, elem.Km, tripProcess.Date_Vrem);
                                        sector = sector == null ? "" : sector;
                                        speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, elem.Km, MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, trackName.ToString()) as List<Speed>;
                                        pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, elem.Km, MainTrackStructureConst.MtoPdbSection, tripProcess.DirectionName, trackName.ToString()) as List<PdbSection>;
                                    }
                                    var t = (temperature.Count != 0 ? temperature[0].Kupe.ToString() : " ") + "°";

                                    previousGap = elem.Km;

                                    elem.Temperature = temperature.Count != 0 ? temperature[0].Kupe : -999;
                                    elem.track_id = track_id;
                                    elem.Vpz = speed.Count > 0 ? speed[0].Passenger.ToString() + "/" + speed[0].Freight.ToString() : "";
                                    elem.Pdb_section = (pdbSection.Count > 0 ? $"ПЧУ-{pdbSection[0].Pchu}/ПД-{pdbSection[0].Pd}/ПДБ-{pdbSection[0].Pdb}" : "ПЧУ-/ПД-/ПДБ-").ToString();
                                    elem.Movement = elem.Oid == 13 ? "13" : elem.Oid == 14 ? "14" : "";
                                    //elem.Movement = (elem.Oid == 13 ? "отсутствие риски на шпале" : elem.Oid == 14 ? "отсутствие риски на плети" : "");
                                    elem.Km = elem.Km;
                                    elem.Sector = sector;
                                    elem.Y = elem.Y;
                                }

                                var Insert_ListOgDerogations_state = AdditionalParametersService.Insert_ListOgDerogations_state(tripProcess.Trip_id, template.ID, GetMaech);
                                Check_ListOgDerogations_state = AdditionalParametersService.Check_ListOgDerogations_state(tripProcess.Trip_id, template.ID);

                                Check_ListOgDerogations_state = Check_ListOgDerogations_state.OrderBy(o => o.Km).ThenBy(o => o.Meter).ToList();

                                foreach (var elem in Check_ListOgDerogations_state)
                                {
                                    var nextfd = Check_ListOgDerogations_state[Check_ListOgDerogations_state.IndexOf(elem)] == Check_ListOgDerogations_state[Check_ListOgDerogations_state.Count - 1] ? null
                                        : Check_ListOgDerogations_state[Check_ListOgDerogations_state.IndexOf(elem) + 1];

                                    if (nextfd != null)
                                    {
                                        if ((elem.Oid == 13 && nextfd.Oid == 13) || (elem.Oid == 14 && nextfd.Oid == 14)) continue;
                                    }
                                    if (nextfd != null)
                                    {
                                        if (elem.Threat == nextfd.Threat &&
                                            elem.Km == nextfd.Km &&
                                            elem.Meter == nextfd.Meter &&
                                            elem.Fnum == nextfd.Fnum &&
                                            elem.Local_fnum == nextfd.Local_fnum &&
                                            ((elem.Oid == 13 && nextfd.Oid == 14) || (elem.Oid == 14 && nextfd.Oid == 13))
                                            )
                                        {
                                            XElement xeNote = new XElement("Note",
                                              new XAttribute("n", iter),
                                              new XAttribute("PPP", elem.Pdb_section),
                                              new XAttribute("PeregonStancia", sector),
                                              new XAttribute("nomerPletStartFinish", iter == 12 ? $"№1 {longrail.Start_Km}км {longrail.Start_M}м {longrail.Final_Km}км {longrail.Final_M}м" : ""),
                                              new XAttribute("NomerMayatShpal", iter),
                                              new XAttribute("km", elem.Km),
                                              new XAttribute("piket", elem.Meter / 100 + 1),
                                              new XAttribute("m", elem.Meter),
                                              new XAttribute("Vpz", elem.Vpz),
                                              new XAttribute("ZazorR", elem.Threat == Threat.Right ? Math.Abs(elem.Y - nextfd.Y).ToString() : "0"),
                                              new XAttribute("ZazorL", elem.Threat == Threat.Left ? Math.Abs(elem.Y - nextfd.Y).ToString() : "0"),
                                              new XAttribute("T", elem.Temperature),
                                              new XAttribute("Primech", ""),
                                              new XAttribute("fileId", elem.File_Id),
                                              new XAttribute("Ms", elem.Ms),
                                              new XAttribute("fNum", elem.Fnum)
                                  );
                                         
                                            iter++;
                                            direction.Add(xeNote);
                                        }
                                        else if (elem.Oid == 13)
                                        {
                                            XElement xeNote = new XElement("Note",
                                                 new XAttribute("n", iter),
                                                 new XAttribute("PPP", elem.Pdb_section),
                                                 new XAttribute("PeregonStancia", sector),
                                                 new XAttribute("nomerPletStartFinish", iter == 12 ? $"№1 {longrail.Start_Km}км {longrail.Start_M}м {longrail.Final_Km}км {longrail.Final_M}м" : ""),
                                                 new XAttribute("NomerMayatShpal", iter),
                                                 new XAttribute("km", elem.Km),
                                                 new XAttribute("piket", elem.Meter / 100 + 1),
                                                 new XAttribute("m", elem.Meter),
                                                 new XAttribute("Vpz", elem.Vpz),
                                                 new XAttribute("ZazorR", elem.Threat == Threat.Right && elem.Oid == 13  ? "ОРШ" : Math.Abs(elem.Y - nextfd.Y).ToString()),
                                                 new XAttribute("ZazorL", elem.Threat == Threat.Left && elem.Oid == 13 ? "ОРШ" : Math.Abs(elem.Y - nextfd.Y).ToString()),
                                                 new XAttribute("T", elem.Temperature),
                                                 new XAttribute("Primech", elem.Oid == 13 ? "ОРШ" : elem.Oid == 14 ? "ОРП" : ""),
                                                 new XAttribute("fileId", elem.File_Id),
                                                 new XAttribute("Ms", elem.Ms),
                                                 new XAttribute("fNum", elem.Fnum)
                                                 );


                                            iter++;
                                            direction.Add(xeNote);
                                        }
                                        else if ( elem.Oid == 14)
                                        {
                                            XElement xeNote = new XElement("Note",
                                                 new XAttribute("n", iter),
                                                 new XAttribute("PPP", elem.Pdb_section),
                                                 new XAttribute("PeregonStancia", sector),
                                                 new XAttribute("nomerPletStartFinish", iter == 12 ? $"№1 {longrail.Start_Km}км {longrail.Start_M}м {longrail.Final_Km}км {longrail.Final_M}м" : ""),
                                                 new XAttribute("NomerMayatShpal", iter),
                                                 new XAttribute("km", elem.Km),
                                                 new XAttribute("piket", elem.Meter / 100 + 1),
                                                 new XAttribute("m", elem.Meter),
                                                 new XAttribute("Vpz", elem.Vpz),
                                                 new XAttribute("ZazorR", elem.Threat == Threat.Right && elem.Oid == 14 ? "ОРП" : Math.Abs(elem.Y - nextfd.Y).ToString()),
                                                 new XAttribute("ZazorL", elem.Threat == Threat.Left && elem.Oid == 14 ? "ОРП" : Math.Abs(elem.Y - nextfd.Y).ToString()),
                                                 new XAttribute("T", elem.Temperature),
                                                 new XAttribute("Primech", elem.Oid == 13 ? "ОРШ" : elem.Oid == 14 ? "ОРП" : ""),
                                                 new XAttribute("fileId", elem.File_Id),
                                                 new XAttribute("Ms", elem.Ms),
                                                 new XAttribute("fNum", elem.Fnum)
                                                 );


                                            iter++;
                                            direction.Add(xeNote);
                                        }
                                    }

                                }

                                direction.Add(
                                    new XAttribute("nomerPletStartFinish", $"№1 {longrail.Start_Km}км {longrail.Start_M}м {longrail.Final_Km}км {longrail.Final_M}м "),
                                    new XAttribute("nomerPletStartFinishCount", iter));

                                tripElem.Add(direction);
                                report.Add(tripElem);
                            }
                            //else
                            //{
                            //    XElement tripElem = new XElement("trip",
                            //        new XAttribute("date_statement", tripProcess.Date_Vrem.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))),
                            //        new XAttribute("type", tripProcess.GetProcessTypeName), //ToDo
                            //        new XAttribute("road", roadName),
                            //        new XAttribute("distance", distance.Code),
                            //        new XAttribute("periodDate", $"{period.Period}   "),
                            //        new XAttribute("chief", tripProcess.Chief),
                            //        new XAttribute("ps", tripProcess.Car),
                            //        new XAttribute("trip_date", tripProcess.Trip_date)
                            //        );
                            //    XElement direction = new XElement("direction",
                            //        new XAttribute("name", tripProcess.DirectionName + " (" + tripProcess.DirectionCode + "), Путь: " + trackName + ", ПЧ: " + distance.Code));

                            //    var it = 1;

                            //    //Check_ListOgDerogations_state = Check_ListOgDerogations_state.Where(
                            //    //    o => o.Km * 1000 + o.Meter >= longrail.Start_Km * 1000 + longrail.Start_M &&
                            //    //    o.Km * 1000 + o.Meter <= longrail.Final_Km * 1000 + longrail.Final_M).ToList();

                            //    //foreach (var elem in Check_ListOgDerogations_state)
                            //    //{
                            //    //    XElement xeNote = new XElement("Note",
                            //    //    new XAttribute("n", it),
                            //    //    new XAttribute("PPP", elem.Pdb_section),
                            //    //    new XAttribute("PeregonStancia", elem.Sector),
                            //    //    new XAttribute("nomerPletStartFinish", Plet),
                            //    //    new XAttribute("NomerMayatShpal", it),
                            //    //    new XAttribute("km", elem.Km),
                            //    //    new XAttribute("piket", elem.Meter / 100 + 1),
                            //    //    new XAttribute("m", elem.Meter),
                            //    //    new XAttribute("Vpz", elem.Vpz),
                            //    //    new XAttribute("ZazorR", elem.Threat == Threat.Right ? "0" : "-"),
                            //    //    new XAttribute("ZazorL", elem.Threat == Threat.Left ? "0" : "-"),
                            //    //    new XAttribute("T", elem.Temperature),
                            //    //    new XAttribute("Primech", elem.Oid == 13 ? "ОРШ" : elem.Oid == 14 ? "ОРП" : ""),
                            //    //    new XAttribute("fileId", elem.File_Id),
                            //    //    new XAttribute("Ms", elem.Ms),
                            //    //    new XAttribute("fNum", elem.Fnum)
                            //    //    );
                            //    //    it++;
                            //    //    direction.Add(xeNote);
                            //    //}
                            //    //direction.Add(
                            //    //    new XAttribute("nomerPletStartFinish", $"№1 {longrail.Start_Km}км {longrail.Start_M}м {longrail.Final_Km}км {longrail.Final_M}м "),
                            //    //    new XAttribute("nomerPletStartFinishCount", Check_ListOgDerogations_state.Count()));

                            //    tripElem.Add(direction);
                            //    report.Add(tripElem);
                            //}
                        }
                    }
                }
                xdReport.Add(report);
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            //try
            //{
            //    htReport.Save(@"\\DESKTOP-EMAFC5J\sntfi\ListOgDerogations.html");
            //}
            //catch
            //{
            //    MessageBox.Show("Ошибка сохранения файлы");
            //}
            //finally
            //{
            //    System.Diagnostics.Process.Start(@"http://DESKTOP-EMAFC5J:5500/ListOgDerogations.html");
            //}
            try
            {
                htReport.Save(Path.GetTempPath() + "/ListOgDerogations.html");
            }
            catch (Exception e)
            {
                Console.WriteLine("Insert_defshpal Ошибка записи в БД " + e.Message);
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/ListOgDerogations.html");
            }
        }
    }
}
