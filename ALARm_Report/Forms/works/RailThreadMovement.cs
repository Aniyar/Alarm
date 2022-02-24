using ALARm.Core;
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
    public class RailThreadMovement : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            long firstTripId = -1, secondTripId = -1;
            List<long> firstAdmTracksIDs = new List<long>(), secondAdmTracksIDs = new List<long>(), admTracksIDs = new List<long>();
            using (var compareForm = new CompareTripsForm())
            {
                compareForm.SetTripsDataSource(distanceId, period);
                compareForm.ShowDialog();
                if (compareForm.dialogResult != DialogResult.OK)
                {
                    return;
                }

                firstTripId = compareForm.firstTripId;
                secondTripId = compareForm.secondTripId;
                firstAdmTracksIDs.AddRange(compareForm.firstAdmTracksIDs);
                admTracksIDs.AddRange(compareForm.firstAdmTracksIDs);
                secondAdmTracksIDs.AddRange(compareForm.secondAdmTracksIDs);
                foreach (var trackId in secondAdmTracksIDs)
                    if (!admTracksIDs.Contains(trackId))
                        admTracksIDs.Add(trackId);
            }

            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                var mainProcessesFirst = RdStructureService.GetMainParametersProcesses(period, distanceId).Where(m => m.Trip_id == firstTripId).ToList();
                var tripFirst = RdStructureService.GetTrip(firstTripId);
                var mainProcessesSecond = RdStructureService.GetMainParametersProcesses(period, distanceId).Where(m => m.Trip_id == secondTripId).ToList();
                var secondTrip = RdStructureService.GetTrip(secondTripId);
                if (mainProcessesFirst.Count == 0 || mainProcessesSecond.Count == 0 || admTracksIDs.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                var road = AdmStructureService.GetUnit(AdmStructureConst.AdmRoad, nod.Parent_Id) as AdmUnit;

                foreach (var trackId in admTracksIDs)
                {
                    var longrails = RdStructureService.GetTripSections(4711, period.StartDate, MainTrackStructureConst.MtoLongRails) as List<LongRails>;

                    if (longrails.Count < 1)
                        continue;

                    var track = AdmStructureService.GetUnit(AdmStructureConst.AdmTrack, trackId) as AdmUnit;
                    var direction = AdmStructureService.GetUnit(AdmStructureConst.AdmDirection, track.Parent_Id) as AdmUnit;

                    XElement xePages = new XElement("pages",
                        new XAttribute("road", road.Name),
                        new XAttribute("direction", direction.Name + " (" + direction.Code + ")"),
                        new XAttribute("car", tripFirst.Car),
                        
                        new XAttribute("trip_info", period.Period + " " + mainProcessesFirst[0].GetProcessTypeName),
                        new XAttribute("distance", distance.Code),
                        new XAttribute("track", track.Code),
                        new XAttribute("firstdate", tripFirst.Trip_date.ToString("dd.MM.yyyy")),
                        new XAttribute("seconddate", secondTrip.Trip_date.ToString("dd.MM.yyyy")));

                    var firstMovementThreads = new List<RdMovementThread>();
                    var secondMovementThreads = new List<RdMovementThread>();

                    foreach (var process in mainProcessesFirst)
                    {
                        var temp = (RdStructureService.GetRdTables(process, 8) as List<RdMovementThread>).Where(m => m.Track_id == trackId).ToList();
                        foreach (var temp1 in temp)
                            if (!firstMovementThreads.Any(m => m.Id == temp1.Id))
                                firstMovementThreads.Add(temp1);
                    }

                    foreach (var process in mainProcessesSecond)
                    {
                        var temp = (RdStructureService.GetRdTables(process, 8) as List<RdMovementThread>).Where(m => m.Track_id == trackId).ToList();
                        foreach (var temp1 in temp)
                            if (!secondMovementThreads.Any(m => m.Id == temp1.Id))
                                secondMovementThreads.Add(temp1);
                    }

                    var firstMovementThreadsLeft = firstMovementThreads.Distinct().OrderBy(m => m.Km * 1000 + m.Meter).Where(m => m.Threat_id == 2).ToList();
                    var firstMovementThreadsRight = firstMovementThreads.Distinct().OrderBy(m => m.Km * 1000 + m.Meter).Where(m => m.Threat_id == 1).ToList();
                    var secondMovementThreadsLeft = secondMovementThreads.Distinct().OrderBy(m => m.Km * 1000 + m.Meter).Where(m => m.Threat_id == 2).ToList();
                    var secondMovementThreadsRight = secondMovementThreads.Distinct().OrderBy(m => m.Km * 1000 + m.Meter).Where(m => m.Threat_id == 1).ToList();

                    int i = 1;
                    foreach (var longrail in longrails)
                    {
                        int count = Math.Min(Math.Min(firstMovementThreadsLeft.Count, firstMovementThreadsRight.Count), Math.Min(secondMovementThreadsLeft.Count, secondMovementThreadsRight.Count));
                        //int count = firstMovementThreadsLeft.Count+firstMovementThreadsRight.Count+secondMovementThreadsLeft.Count+secondMovementThreadsRight.Count;
                        XElement xeThongs = new XElement("thongs",
                            new XAttribute("count", count),
                            new XAttribute("number", i),
                            new XAttribute("start", longrail.Start_Km + " км " + longrail.Start_M + " м"),
                            new XAttribute("final", longrail.Final_Km + " км " + longrail.Final_M + " м"));

                        int k = 1;
                        for (int j = 0; j < count; j++)
                        {
                            XElement xeElements = new XElement("elements",
                                new XAttribute("number", k),
                                new XAttribute("left1", firstMovementThreadsLeft.Count > 0 ? firstMovementThreadsLeft[j].Movement.ToString() : "" ),
                                new XAttribute("right1", firstMovementThreadsRight.Count > 0 ? firstMovementThreadsRight[j].Movement.ToString() : ""),
                                new XAttribute("t1", firstMovementThreadsLeft.Count > 0 ? firstMovementThreadsLeft[j].Temperature.ToString() : ""),
                                new XAttribute("left2", secondMovementThreadsLeft.Count > 0 ? secondMovementThreadsLeft[j].Movement.ToString() : ""),
                                new XAttribute("right2", secondMovementThreadsRight.Count > 0 ? secondMovementThreadsRight[j].Movement.ToString() : ""),
                                new XAttribute("t2", secondMovementThreadsLeft.Count > 0 ? secondMovementThreadsLeft[j].Temperature.ToString() : ""));

                            xeThongs.Add(xeElements);
                            k++;
                        }

                        xePages.Add(xeThongs);
                        i++;
                    }

                    report.Add(xePages);
                }

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_RailThreadMovement.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_RailThreadMovement.html");
            }
        }
    }
}
