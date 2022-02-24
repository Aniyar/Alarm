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
    public class PRandNPKsiteCharacteristics : Report
    {
        private string ValueToFractionStr(double a)
        {
            int b = Convert.ToInt32(1 / a);
            return "1/" + b.ToString();
        }
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

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var mainProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (!mainProcesses.Any())
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                foreach (var process in mainProcesses)
                {
                    foreach (var trackId in admTracksId)
                    {
                        var trackName = AdmStructureService.GetTrackName(trackId);

                        var kms = RdStructureService.GetKilometerTrip(process.Trip_id);
                        progressBar.Maximum = kms.Count;

                        XElement xePages = new XElement("pages",

                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("road", road),
                            new XAttribute("distance", distance.Code),
                            new XAttribute("period", period.Period),
                            new XAttribute("chek",  process.GetProcessTypeName),
                            new XAttribute("car", process.Car),
                             new XAttribute("track_info", $"{process.DirectionName}({process.DirectionCode}) Путь:{trackName}"),
                        new XAttribute("chief", process.Chief));

                        XElement xeTracks = new XElement("tracks",
                            new XAttribute("trackinfo", $"{process.DirectionName}({process.DirectionCode}) Путь:{trackName}"));

                        foreach (var kilometer in kms)
                        {
                            progressBar.Value = kms.IndexOf(kilometer) + 1;

                            var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(kilometer, process.Trip_id);
                            if (DBcrossRailProfile == null || DBcrossRailProfile.Count == 0) continue;

                            var start = kilometer * 1000 + DBcrossRailProfile.First().Meter;
                            var final = kilometer * 1000 + DBcrossRailProfile.Last().Meter;

                            var rail_sections = RdStructureService.GetTripSections(trackId, process.Date_Vrem, start / 1000, start % 1000, final / 1000, final % 1000, MainTrackStructureConst.MtoRailSection) as List<RailsSections>;
                            bool sectionExists = rail_sections.Any();
                            var rail_braces = RdStructureService.GetTripSections(trackId, process.Date_Vrem, start / 1000, start % 1000, final / 1000, final % 1000, MainTrackStructureConst.MtoRailsBrace) as List<RailsBrace>;
                            bool bracesExists = rail_braces.Any();

                            List<int> absCoords = new List<int>();
                            if (sectionExists)
                            {
                                rail_sections.Where(s => s.Start_Km * 1000 + s.Start_M >= start).ToList().ForEach(s => absCoords.Add(s.Start_Km * 1000 + s.Start_M));
                                rail_sections.Where(s => s.Final_Km * 1000 + s.Final_M <= final).ToList().ForEach(s => absCoords.Add(s.Final_Km * 1000 + s.Final_M));
                            }
                            if (bracesExists)
                            {
                                rail_braces.Where(s => s.Start_Km * 1000 + s.Start_M >= start).ToList().ForEach(s => absCoords.Add(s.Start_Km * 1000 + s.Start_M));
                                rail_braces.Where(s => s.Final_Km * 1000 + s.Final_M <= final).ToList().ForEach(s => absCoords.Add(s.Final_Km * 1000 + s.Final_M));
                            }
                            if (!absCoords.Contains(start))
                                absCoords.Add(start);
                            if (!absCoords.Contains(final))
                                absCoords.Add(final);

                            var absCoordsOrder = absCoords.OrderBy(a => a).ToList();

                            for (int i = 0; i < absCoordsOrder.Count() - 1; i++)
                            {
                                start = absCoordsOrder[i];
                                final = absCoordsOrder[i + 1];
                                var railSection = sectionExists ? rail_sections.Where(s => s.Start_Km * 1000 + s.Start_M <= start && s.Final_Km * 1000 + s.Final_M >= final).Any() ? rail_sections.Where(s => s.Start_Km * 1000 + s.Start_M <= start && s.Final_Km * 1000 + s.Final_M >= final).First() : null : null;
                                var braceSection = bracesExists ? rail_braces.Where(s => s.Start_Km * 1000 + s.Start_M <= start && s.Final_Km * 1000 + s.Final_M >= final).Any() ? rail_braces.Where(s => s.Start_Km * 1000 + s.Start_M <= start && s.Final_Km * 1000 + s.Final_M >= final).First() : null : null;
                                var speed_sections = RdStructureService.GetTripSections(trackId, process.Date_Vrem, start / 1000, start % 1000, final / 1000, final % 1000, MainTrackStructureConst.MtoSpeed) as List<Speed>;
                                int speed = -1;
                                if (speed_sections.Any())
                                    speed = speed_sections.Max(s => s.Passenger);


                                var temp_rail_slopes = DBcrossRailProfile.Where(s => kilometer * 1000 + s.Meter >= start && kilometer * 1000 + s.Meter <= final).ToList();
                                var temp_surface_slopes = DBcrossRailProfile.Where(s => kilometer * 1000 + s.Meter >= start && kilometer * 1000 + s.Meter <= final).ToList();

                                xeTracks.Add(new XElement("elements",
                                    new XAttribute("start", (start / 1000.0).ToString().Replace(',', '.')),
                                    new XAttribute("final", (final / 1000.0).ToString().Replace(',', '.')),
                                    new XAttribute("rails", railSection is null ? "-" : railSection.Type),
                                    new XAttribute("fastening", braceSection is null ? "-" : braceSection.Brace_Type),
                                    new XAttribute("railside", "л."),
                                    new XAttribute("PRmedium", temp_rail_slopes.Any() ? ValueToFractionStr(temp_rail_slopes.Average(s => s.Pu_l)) : "-"),
                                    new XAttribute("PRmin", temp_rail_slopes.Any() ? ValueToFractionStr(temp_rail_slopes.Min(s => s.Pu_l)) : "-"),
                                    new XAttribute("PRmax", temp_rail_slopes.Any() ? ValueToFractionStr(temp_rail_slopes.Max(s => s.Pu_l)) : "-"),
                                    new XAttribute("PRlen160", temp_rail_slopes.Any() ? (temp_rail_slopes.Count(s => s.Pu_l < 1.0 / 60.0)/4).ToString() : "-"),
                                    new XAttribute("PRlen120", temp_rail_slopes.Any() ? (temp_rail_slopes.Count(s => s.Pu_l > 1.0 / 12.0)/4).ToString() : "-"),
                                    new XAttribute("NPKmedium", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Average(s => s.Npk_l)) : "-"),
                                    new XAttribute("NPKmin", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Min(s => s.Npk_l)) : "-"),
                                    new XAttribute("NPKmax", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Max(s => s.Npk_l)) : "-"),
                                    new XAttribute("NPKlen160", temp_surface_slopes.Any() ? (temp_surface_slopes.Count(s => s.Npk_l < 1.0 / 60.0)/4).ToString() : "-"),
                                    new XAttribute("NPKlen120", temp_surface_slopes.Any() ? (temp_surface_slopes.Count(s => s.Npk_l > 1.0 / 12.0)/4).ToString() : "-"),
                                    new XAttribute("speed", speed == -1 ? "нет данных" : speed_sections.Count > 0 ? speed_sections[0].Passenger.ToString() + "/" + speed_sections[0].Freight : "-/-")
                                    ));

                                xeTracks.Add(new XElement("elements",
                                    new XAttribute("start", (start / 1000.0).ToString().Replace(',', '.')),
                                    new XAttribute("final", (final / 1000.0).ToString().Replace(',', '.')),
                                    new XAttribute("rails", railSection is null ? "-" : railSection.Type),
                                    new XAttribute("fastening", braceSection is null ? "-" : braceSection.Brace_Type),
                                    new XAttribute("railside", "пр."),
                                    new XAttribute("PRmedium", temp_rail_slopes.Any() ? ValueToFractionStr(temp_rail_slopes.Average(s => s.Pu_r)) : "-"),
                                    new XAttribute("PRmin", temp_rail_slopes.Any() ? ValueToFractionStr(temp_rail_slopes.Min(s => s.Pu_r)) : "-"),
                                    new XAttribute("PRmax", temp_rail_slopes.Any() ? ValueToFractionStr(temp_rail_slopes.Max(s => s.Pu_r)) : "-"),
                                    new XAttribute("PRlen160", temp_rail_slopes.Any() ? (temp_rail_slopes.Count(s => s.Pu_r < 1.0 / 60.0) / 4).ToString() : "-"),
                                    new XAttribute("PRlen120", temp_rail_slopes.Any() ? (temp_rail_slopes.Count(s => s.Pu_r > 1.0 / 12.0) / 4).ToString() : "-"),
                                    new XAttribute("NPKmedium", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Average(s => s.Npk_r)) : "-"),
                                    new XAttribute("NPKmin", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Min(s => s.Npk_r)) : "-"),
                                    new XAttribute("NPKmax", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Max(s => s.Npk_r)) : "-"),
                                    new XAttribute("NPKlen160", temp_surface_slopes.Any() ? (temp_surface_slopes.Count(s => s.Npk_r < 1.0 / 60.0)/4).ToString() : "-"),
                                    new XAttribute("NPKlen120", temp_surface_slopes.Any() ? (temp_surface_slopes.Count(s => s.Npk_r > 1.0 / 12.0)/4).ToString() : "-"),
                                    new XAttribute("speed", speed == -1 ? "нет данных" : speed_sections.Count > 0 ? speed_sections[0].Passenger.ToString() + "/" + speed_sections[0].Freight : "-/-")));
                            }
                        }
                        
                        xePages.Add(xeTracks);
                        report.Add(xePages);

                        //ПР
                        //var rail_slopes = (RdStructureService.GetRdTables(process, 6) as List<RdRailSlope>).OrderBy(s => s.Km * 1000 + s.Meter);
                        //var railExists = rail_slopes.Any();
                        ////НПК
                        //var surface_slopes = (RdStructureService.GetRdTables(process, 7) as List<RdSurfaceSlope>).OrderBy(s => s.Km * 1000 + s.Meter);
                        //var surfaceExists = surface_slopes.Any();
                        ////var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                        //var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                        //var road = AdmStructureService.GetUnit(AdmStructureConst.AdmRoad, nod.Parent_Id) as AdmUnit;

                        //if (!rail_slopes.Any() && !surface_slopes.Any())
                        //    continue;

                        //XElement xePages = new XElement("pages",
                        //    new XAttribute("road", road),
                        //    new XAttribute("distance", distance.Code),
                        //    new XAttribute("dateandtype", period.Period + " " + process.GetProcessTypeName),
                        //    new XAttribute("car", process.Car),
                        //    new XAttribute("chief", process.Chief));

                        //List<long> trackIDs = new List<long>();
                        //if (railExists)
                        //    foreach (var trackId in rail_slopes.GroupBy(s => s.Track_id).Select(s => s.Key).ToList())
                        //        if (!trackIDs.Contains(trackId))
                        //            trackIDs.Add(trackId);
                        //if (surfaceExists)
                        //    foreach (var trackId in surface_slopes.GroupBy(s => s.Track_id).Select(s => s.Key).ToList())
                        //        if (!trackIDs.Contains(trackId))
                        //            trackIDs.Add(trackId);

                        //foreach (var trackId in trackIDs)
                        //{
                        //    if (!admTracksId.Contains(trackId))
                        //    {
                        //        continue;
                        //    }

                        //    int start = -1, final = -1;
                        //    string trackInfo;
                        //    if (railExists)
                        //    {
                        //        trackInfo = rail_slopes.Where(s => s.Track_id == trackId).First().Direction + ". Путь - " + rail_slopes.Where(s => s.Track_id == trackId).First().Track;
                        //        start = rail_slopes.Where(s => s.Track_id == trackId).Min(s => s.Km * 1000 + s.Meter);
                        //        final = rail_slopes.Where(s => s.Track_id == trackId).Max(s => s.Km * 1000 + s.Meter);
                        //        if (surfaceExists)
                        //        {
                        //            start = Math.Min(start, surface_slopes.Where(s => s.Track_id == trackId).Min(s => s.Km * 1000 + s.Meter));
                        //            final = Math.Max(final, surface_slopes.Where(s => s.Track_id == trackId).Max(s => s.Km * 1000 + s.Meter));
                        //        }
                        //    }
                        //    else
                        //    {
                        //        trackInfo = surface_slopes.Where(s => s.Track_id == trackId).First().Direction + ". Путь - " + surface_slopes.Where(s => s.Track_id == trackId).First().Track;
                        //        start = surface_slopes.Where(s => s.Track_id == trackId).Min(s => s.Km * 1000 + s.Meter);
                        //        final = surface_slopes.Where(s => s.Track_id == trackId).Max(s => s.Km * 1000 + s.Meter);
                        //    }

                        //    XElement xeTracks = new XElement("tracks",
                        //        new XAttribute("trackinfo", trackInfo));

                        //    var rail_sections = RdStructureService.GetTripSections(trackId, process.Date_Vrem, start / 1000, start % 1000, final / 1000, final % 1000, MainTrackStructureConst.MtoRailSection) as List<RailsSections>;
                        //    bool sectionExists = rail_sections.Any();
                        //    var rail_braces = RdStructureService.GetTripSections(trackId, process.Date_Vrem, start / 1000, start % 1000, final / 1000, final % 1000, MainTrackStructureConst.MtoRailsBrace) as List<RailsBrace>;
                        //    bool bracesExists = rail_braces.Any();

                        //    List<int> absCoords = new List<int>();
                        //    if (sectionExists)
                        //    {
                        //        rail_sections.Where(s => s.Start_Km * 1000 + s.Start_M >= start).ToList().ForEach(s => absCoords.Add(s.Start_Km * 1000 + s.Start_M));
                        //        rail_sections.Where(s => s.Final_Km * 1000 + s.Final_M <= final).ToList().ForEach(s => absCoords.Add(s.Final_Km * 1000 + s.Final_M));
                        //    }
                        //    if (bracesExists)
                        //    {
                        //        rail_braces.Where(s => s.Start_Km * 1000 + s.Start_M >= start).ToList().ForEach(s => absCoords.Add(s.Start_Km * 1000 + s.Start_M));
                        //        rail_braces.Where(s => s.Final_Km * 1000 + s.Final_M <= final).ToList().ForEach(s => absCoords.Add(s.Final_Km * 1000 + s.Final_M));
                        //    }
                        //    if (!absCoords.Contains(start))
                        //        absCoords.Add(start);
                        //    if (!absCoords.Contains(final))
                        //        absCoords.Add(final);

                        //    var absCoordsOrder = absCoords.OrderBy(a => a).ToList();

                        //    for (int i = 0; i < absCoordsOrder.Count() - 1; i++)
                        //    {
                        //        start = absCoordsOrder[i];
                        //        final = absCoordsOrder[i + 1];
                        //        var railSection = sectionExists ? rail_sections.Where(s => s.Start_Km * 1000 + s.Start_M <= start && s.Final_Km * 1000 + s.Final_M >= final).Any() ? rail_sections.Where(s => s.Start_Km * 1000 + s.Start_M <= start && s.Final_Km * 1000 + s.Final_M >= final).First() : null : null;
                        //        var braceSection = bracesExists ? rail_braces.Where(s => s.Start_Km * 1000 + s.Start_M <= start && s.Final_Km * 1000 + s.Final_M >= final).Any() ? rail_braces.Where(s => s.Start_Km * 1000 + s.Start_M <= start && s.Final_Km * 1000 + s.Final_M >= final).First() : null : null;
                        //        var speed_sections = RdStructureService.GetTripSections(trackId, process.Date_Vrem, start / 1000, start % 1000, final / 1000, final % 1000, MainTrackStructureConst.MtoSpeed) as List<Speed>;
                        //        int speed = -1;
                        //        if (speed_sections.Any())
                        //            speed = speed_sections.Max(s => s.Passenger);


                        //        if (railExists)
                        //        {
                        //            if (rail_slopes.Where(s => s.Track_id == trackId && s.Threat_id == -1).Any())
                        //            {
                        //                var temp_rail_slopes = rail_slopes.Where(s => s.Km * 1000 + s.Meter >= start && s.Km * 1000 + s.Meter <= final && s.Track_id == trackId && s.Threat_id == -1).ToList();
                        //                var temp_surface_slopes = surfaceExists ? surface_slopes.Where(s => s.Km * 1000 + s.Meter >= start && s.Km * 1000 + s.Meter <= final && s.Track_id == trackId && s.Threat_id == -1).ToList() : new List<RdSurfaceSlope>();

                        //                xeTracks.Add(new XElement("elements",
                        //                    new XAttribute("start", (start / 1000.0).ToString().Replace(',', '.')),
                        //                    new XAttribute("final", (final / 1000.0).ToString().Replace(',', '.')),
                        //                    new XAttribute("rails", railSection is null ? "-" : railSection.Type),
                        //                    new XAttribute("fastening", braceSection is null ? "-" : braceSection.Brace_Type),
                        //                    new XAttribute("railside", "л."),
                        //                    new XAttribute("PRmedium", temp_rail_slopes.Any() ? ValueToFractionStr(temp_rail_slopes.Average(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("PRmin", temp_rail_slopes.Any() ? ValueToFractionStr(temp_rail_slopes.Min(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("PRmax", temp_rail_slopes.Any() ? ValueToFractionStr(temp_rail_slopes.Max(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("PRlen160", temp_rail_slopes.Any() ? temp_rail_slopes.Count(s => s.Slope_value < 1.0 / 60.0).ToString() : "-"),
                        //                    new XAttribute("PRlen120", temp_rail_slopes.Any() ? temp_rail_slopes.Count(s => s.Slope_value > 1.0 / 12.0).ToString() : "-"),
                        //                    new XAttribute("NPKmedium", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Average(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("NPKmin", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Min(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("NPKmax", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Max(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("NPKlen160", temp_surface_slopes.Any() ? temp_surface_slopes.Count(s => s.Slope_value < 1.0 / 60.0).ToString() : "-"),
                        //                    new XAttribute("NPKlen120", temp_surface_slopes.Any() ? temp_surface_slopes.Count(s => s.Slope_value > 1.0 / 12.0).ToString() : "-"),
                        //                    new XAttribute("speed", speed == -1 ? "нет данных" : speed.ToString())));
                        //            }
                        //            if (rail_slopes.Where(s => s.Track_id == trackId && s.Threat_id == 1).Any())
                        //            {
                        //                var temp_rail_slopes = rail_slopes.Where(s => s.Km * 1000 + s.Meter >= start && s.Km * 1000 + s.Meter <= final && s.Track_id == trackId && s.Threat_id == 1).ToList();
                        //                var temp_surface_slopes = surfaceExists ? surface_slopes.Where(s => s.Km * 1000 + s.Meter >= start && s.Km * 1000 + s.Meter <= final && s.Track_id == trackId && s.Threat_id == 1).ToList() : new List<RdSurfaceSlope>();

                        //                xeTracks.Add(new XElement("elements",
                        //                    new XAttribute("start", (start / 1000.0).ToString().Replace(',', '.')),
                        //                    new XAttribute("final", (final / 1000.0).ToString().Replace(',', '.')),
                        //                    new XAttribute("rails", railSection is null ? "-" : railSection.Type),
                        //                    new XAttribute("fastening", braceSection is null ? "-" : braceSection.Brace_Type),
                        //                    new XAttribute("railside", "пр."),
                        //                    new XAttribute("PRmedium", temp_rail_slopes.Any() ? ValueToFractionStr(temp_rail_slopes.Average(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("PRmin", temp_rail_slopes.Any() ? ValueToFractionStr(temp_rail_slopes.Min(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("PRmax", temp_rail_slopes.Any() ? ValueToFractionStr(temp_rail_slopes.Max(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("PRlen160", temp_rail_slopes.Any() ? temp_rail_slopes.Count(s => s.Slope_value < 1.0 / 60.0).ToString() : "-"),
                        //                    new XAttribute("PRlen120", temp_rail_slopes.Any() ? temp_rail_slopes.Count(s => s.Slope_value > 1.0 / 12.0).ToString() : "-"),
                        //                    new XAttribute("NPKmedium", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Average(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("NPKmin", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Min(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("NPKmax", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Max(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("NPKlen160", temp_surface_slopes.Any() ? temp_surface_slopes.Count(s => s.Slope_value < 1.0 / 60.0).ToString() : "-"),
                        //                    new XAttribute("NPKlen120", temp_surface_slopes.Any() ? temp_surface_slopes.Count(s => s.Slope_value > 1.0 / 12.0).ToString() : "-"),
                        //                    new XAttribute("speed", speed == -1 ? "нет данных" : speed.ToString())));
                        //            }
                        //        }
                        //        else
                        //        {
                        //            if (surface_slopes.Where(s => s.Track_id == trackId && s.Threat_id == -1).Any())
                        //            {
                        //                var temp_surface_slopes = surface_slopes.Where(s => s.Km * 1000 + s.Meter >= start && s.Km * 1000 + s.Meter <= final && s.Track_id == trackId && s.Threat_id == -1).ToList();

                        //                xeTracks.Add(new XElement("elements",
                        //                    new XAttribute("start", (start / 1000.0).ToString().Replace(',', '.')),
                        //                    new XAttribute("final", (final / 1000.0).ToString().Replace(',', '.')),
                        //                    new XAttribute("rails", railSection is null ? "-" : railSection.Type),
                        //                    new XAttribute("fastening", braceSection is null ? "-" : braceSection.Brace_Type),
                        //                    new XAttribute("railside", "л."),
                        //                    new XAttribute("PRmedium", "-"),
                        //                    new XAttribute("PRmin", "-"),
                        //                    new XAttribute("PRmax", "-"),
                        //                    new XAttribute("PRlen160", "-"),
                        //                    new XAttribute("PRlen120", "-"),
                        //                    new XAttribute("NPKmedium", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Average(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("NPKmin", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Min(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("NPKmax", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Max(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("NPKlen160", temp_surface_slopes.Any() ? temp_surface_slopes.Count(s => s.Slope_value < 1.0 / 60.0).ToString() : "-"),
                        //                    new XAttribute("NPKlen120", temp_surface_slopes.Any() ? temp_surface_slopes.Count(s => s.Slope_value > 1.0 / 12.0).ToString() : "-"),
                        //                    new XAttribute("speed", speed == -1 ? "нет данных" : speed.ToString())));
                        //            }
                        //            if (surface_slopes.Where(s => s.Track_id == trackId && s.Threat_id == 1).Any())
                        //            {
                        //                var temp_surface_slopes = surface_slopes.Where(s => s.Km * 1000 + s.Meter >= start && s.Km * 1000 + s.Meter <= final && s.Track_id == trackId && s.Threat_id == 1).ToList();

                        //                xeTracks.Add(new XElement("elements",
                        //                    new XAttribute("start", (start / 1000.0).ToString().Replace(',', '.')),
                        //                    new XAttribute("final", (final / 1000.0).ToString().Replace(',', '.')),
                        //                    new XAttribute("rails", railSection is null ? "-" : railSection.Type),
                        //                    new XAttribute("fastening", braceSection is null ? "-" : braceSection.Brace_Type),
                        //                    new XAttribute("railside", "пр."),
                        //                    new XAttribute("PRmedium", "-"),
                        //                    new XAttribute("PRmin", "-"),
                        //                    new XAttribute("PRmax", "-"),
                        //                    new XAttribute("PRlen160", "-"),
                        //                    new XAttribute("PRlen120", "-"),
                        //                    new XAttribute("NPKmedium", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Average(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("NPKmin", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Min(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("NPKmax", temp_surface_slopes.Any() ? ValueToFractionStr(temp_surface_slopes.Max(s => s.Slope_value)) : "-"),
                        //                    new XAttribute("NPKlen160", temp_surface_slopes.Any() ? temp_surface_slopes.Count(s => s.Slope_value < 1.0 / 60.0).ToString() : "-"),
                        //                    new XAttribute("NPKlen120", temp_surface_slopes.Any() ? temp_surface_slopes.Count(s => s.Slope_value > 1.0 / 12.0).ToString() : "-"),
                        //                    new XAttribute("speed", speed == -1 ? "нет данных" : speed.ToString())));
                        //            }
                        //        }
                        //    }
                        //    xePages.Add(xeTracks);
                        //}

                        //report.Add(xePages);
                    }
                }

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {

                htReport.Save(Path.GetTempPath() + "/report_PRandNPKsiteCharacteristics.html");

            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_PRandNPKsiteCharacteristics.html");
            }
        }
    }
}
