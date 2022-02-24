using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
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

namespace ALARm_Report.Forms
{
    public class LongitudinalProfileElements : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
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

                var mainProcesses = RdStructureService.GetMainParametersProcesses(period, distanceId);
                if (!mainProcesses.Any())
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                foreach (var process in mainProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        

                        var trackName = AdmStructureService.GetTrackName(track_id);

                        process.TrackID = track_id;
                        process.TrackName = trackName.ToString();

                        //Продольный профиль
                        var raw_rd_profile = RdStructureService.GetRdTables(process, 1) as List<RdProfile>;
                        //Реперные точки
                        var raw_RefPoints = MainTrackStructureService.GetRefPointsByTripIdToDate(track_id, process.Date_Vrem);

                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        var min = raw_RefPoints.Select(o => o.Km).Min();
                        var max = raw_RefPoints.Select(o => o.Km).Max();

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = min });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = max });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;
                        //фильтр
                        raw_RefPoints = raw_RefPoints.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList();

                        var roadName = AdmStructureService.GetRoadName(distanceId, AdmStructureConst.AdmDistance, true);
                      
                        var start = false;
                        var RefPoints = new List<RefPoint> { };
                        var rd_profile = new List<RdProfile> { };
                        RefPoint PREV = null;

                    

                        foreach (var rp in raw_RefPoints)
                        {
                            if (start == true)
                            {
                                var st = raw_rd_profile.Where(o => o.Km * 1000 + o.Meter > rp.Km * 1000 + rp.Meter).ToList();

                                if (st.Count == 0)
                                {
                                    RefPoints.Add(PREV ?? rp);
                                    break;
                                }
                                PREV = rp;
                            }
                            if (start == false)
                            {
                                var st = raw_rd_profile.Where(o => o.Km * 1000 + o.Meter <= rp.Km * 1000 + rp.Meter).ToList();

                                if (st.Count > 0)
                                {
                                    RefPoints.Add(rp);
                                    start = true;
                                }
                            }
                        }
                        if (RefPoints.Count == 1)
                        {
                            RefPoints.Add(raw_RefPoints.Last());
                        }

                        rd_profile = raw_rd_profile.Where(o =>
                                                              RefPoints.First().Km * 1000 + RefPoints.First().Meter <= o.Km * 1000 + o.Meter &&
                                                              RefPoints.Last().Km * 1000 + RefPoints.Last().Meter >= o.Km * 1000 + o.Meter
                                                         ).ToList();
                        RefPoints = raw_RefPoints.Where(o =>
                                                            RefPoints.First().Km * 1000 + RefPoints.First().Meter <= o.Km * 1000 + o.Meter &&
                                                            RefPoints.Last().Km * 1000 + RefPoints.Last().Meter >= o.Km * 1000 + o.Meter
                                                       ).ToList();





                        var startP = rd_profile.IndexOf(rd_profile.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList().First());
                        var finalP = rd_profile.IndexOf(rd_profile.Where(o => ((float)(float)filters[0].Value <= o.Km && o.Km <= (float)(float)filters[1].Value)).ToList().Last());

                        var countP = finalP - startP;

                        var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                        var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                        var road = AdmStructureService.GetUnit(AdmStructureConst.AdmRoad, nod.Parent_Id) as AdmUnit;
                        if (!rd_profile.Any())
                            continue;

                        var track_profile = rd_profile;
                        //var track_profile = rd_profile.Where(r => r.Track_id == trackId).OrderBy(r => r.X).ToList();


                        var trackId = track_id;
                        var tripDate = process.Date_Vrem;

                        List<int> x_RefPoints = new List<int>();
                        List<int> x_track_profile = new List<int>();
                        List<int> xInd = new List<int>();
                        List<double> yr = new List<double>();

                        int j = 0;
                        foreach (var elem in track_profile)
                        {
                            var t = RefPoints.Where(o => o.Km == elem.Km && o.Meter == elem.Meter).ToList();
                            if (t.Count > 0)
                            {
                                x_RefPoints.Add(j + 1);
                                xInd.Add(j);
                                yr.Add(elem.Y);
                            }
                            x_track_profile.Add(j + 1);

                            j++;
                        }

                        XElement xePages = new XElement("pages",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version}"),
                            new XAttribute("tripinfo", process.DirectionName +  " / Путь: " + trackName),
                            new XAttribute("road", roadName),
                            new XAttribute("distance", distance.Code ?? ""),
                            new XAttribute("period", period.Period),
                            new XAttribute("trip_info",  process.GetProcessTypeName ),
                            new XAttribute("chief", process.Chief),
                            new XAttribute("car", process.Car ?? ""),
                            new XAttribute("km"," " + 
                                rd_profile.Select(o => o.Km).ToList().Min() + " - " + rd_profile.Select(o => o.Km).ToList().Max()
                                ),
                            new XAttribute("trip_date", process.Trip_date ?? ""),

                            new XAttribute("direction", process.DirectionName),

                            new XAttribute("track", process.TrackName ?? "")
                        );

                        //ProfileData profileData = new ProfileData(
                        //    rd_profile.Select(p => p.X).ToList(), 
                        //    rd_profile.Select(p => p.Y).ToList()
                        //    );

                        ProfileData profileData = new ProfileData(
                                                                     //profile
                                                                     x_track_profile,
                                                                     track_profile.Select(p => p.Y).ToList(),

                                                                     track_profile.Select(p => p.Deviation).ToList(),

                                                                     //repers
                                                                     RefPoints.Select(p => p.Mark).ToList(),
                                                                     x_RefPoints,

                                                                     yr,
                                                                     xInd,
                                                                     startP,
                                                                     countP
                                                                     );

                        var original = profileData.GraphProfile2();
                        int i = 0;
                        string SlopeDiff = string.Empty;
                        foreach (var profile in profileData.straightProfilesNew)
                        {
                            try
                            {
                                xePages.Add(new XElement("elements",
                                new XAttribute("km", rd_profile[profile.CoordAbs - 1].Km),
                                new XAttribute("meter", rd_profile[profile.CoordAbs - 1].Meter),
                                new XAttribute("mark", (profile.Profile * 100).ToString("0")),
                                new XAttribute("length", profile.Len),
                                new XAttribute("slope", ((profileData.straightProfilesNew[i + 1].Profile - profile.Profile) / profile.Len * 1000).ToString("0.00")),
                                new XAttribute("slopedifference", SlopeDiff == string.Empty ? "-" : SlopeDiff)));
                                SlopeDiff = (profileData.straightProfilesNew[i + 1].Profile - profile.Profile).ToString("0.00");
                                i++;

                            }
                            catch
                            {
                                Console.Error.WriteLine("Ошибка xePages");
                                i++;
                            }
                        }
                        
                        report.Add(xePages);
                    }
                }

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_TripPlanElements.html");

            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_TripPlanElements.html");
            }
        }
    }
}
