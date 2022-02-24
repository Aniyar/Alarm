using ALARm.Core;
using ALARm.Core.Report;
using MetroFramework.Controls;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using ALARm.Services;
using System.Collections.Generic;

namespace ALARm_Report.Forms
{
    public class Statistics : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");
                XElement xePages = new XElement("pages");


                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var mainProcesses = RdStructureService.GetMainParametersProcesses(period, distanceId);
                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }


                var nod = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;
                //var road = AdmStructureService.GetUnit(AdmStructureConst.AdmRoad, nod.Parent_Id) as AdmUnit;

                foreach (var mainProcess in mainProcesses)
                {

                    var Check_Total_bolts = AdditionalParametersService.Check_Total_bolts(mainProcess.Trip_id, template.ID);
                    var bolts_check_km = Check_Total_bolts.GroupBy(O => O.Km).ToList().Count;
                    var bolts_All = Check_Total_bolts.Where(O => O.Otst != "").ToList().Count;


                    var bolts_before = 0;
                    var bolts_after = 0;
                    foreach (var bolts in Check_Total_bolts.Where(O => O.Before != "").ToList())
                    {
                        if (bolts.Before != "")
                        {
                            var bolts_before_count = bolts.Before.ToList().Count;
                            bolts_before++;

                        }
                    }
                    foreach (var bolts in Check_Total_bolts.Where(O => O.After != "").ToList())
                    {
                        if (bolts.After != "")
                        {
                            var bolts_after_count = bolts.After.ToList().Count;
                            bolts_after++;

                        }

                    }

                    var Check_Total_overlay = AdditionalParametersService.Check_Total_bolts(mainProcess.Trip_id, template.ID);
                    var overlay_check_km = Check_Total_overlay.GroupBy(O => O.Km).ToList().Count;
                    var overlay_All = Check_Total_overlay.Where(O => O.Otst != "").ToList().Count;


                    //для шпал
                    var Check_Total_sleepers = AdditionalParametersService.Check_sleepers_state(mainProcess.Trip_id, template.ID);
                    var sleepers_check_km = Check_Total_sleepers.GroupBy(O => O.Km).ToList().Count;


                    var sleepers_no_speedlimit = Check_Total_sleepers.Where(O => O.Otst != "").ToList().Where(O => O.Vdop == "").ToList().Count;
                    var sleepers_All = Check_Total_sleepers.Where(O => O.Otst != "").ToList().Count;

                    //



                    //для стыковых зазоров
                    var Check_Total_gaps = AdditionalParametersService.Check_Total_state_digression(mainProcess.Trip_id, template.ID);
                    var gap_check_km = Check_Total_gaps.GroupBy(O => O.Km).ToList().Count;


                    var gap_no_speedlimit = Check_Total_gaps.Where(O => O.Otst != "").ToList().Where(O => O.Vdop == "").ToList().Count;
                    var gap_All = Check_Total_gaps.Where(O => O.Otst != "").ToList().Count;

                    //

                    //  для скрелепний  

                    var Check_Total_fastening = AdditionalParametersService.Check_Total_state_fastening(mainProcess.Trip_id, template.ID);
                    var fastening_check_km = Check_Total_fastening.GroupBy(O => O.Km).ToList().Count;



                    var fastening_no_speedlimit = Check_Total_fastening.Where(O => O.Otst != "").ToList().Where(O => O.Vdop == "").ToList().Count;
                    var fastening_All = Check_Total_fastening.Where(O => O.Otst != "").ToList().Count;

                    //


                    //для маячная метка
                    var Check_Total_threat_id = AdditionalParametersService.Check_Total_threat_id(mainProcess.Trip_id, template.ID);

                    var threat_id_All = Check_Total_threat_id.Where(O => O.Otst != "").ToList().Count;

                    //

                    var statistics = RdStructureService.GetRdTables(mainProcess, 4) as List<RdStatistics>;
                   // var statistics = AdditionalParametersService.Check_Total_state_rails(mainProcess.Trip_id, ID) as List<RdStatistics>;
                    //    var statistics = AdditionalParametersService.Total(mainProcess.Trip_id, template.ID) as List<RdStatistics>;
                    //if (statistics.Count < 1)
                    //{
                    //    continue;
                    //}

                    xePages = new XElement("pages",
                            new XAttribute("road", road),
                            new XAttribute("period", period.Period),
                            new XAttribute("type", mainProcess.GetProcessTypeName),
                            new XAttribute("car", mainProcess.Car),
                            new XAttribute("chief", mainProcess.Chief),
                            new XAttribute("data", " " + mainProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                            new XAttribute("info", mainProcess.Car + " " + mainProcess.Chief)
                            //new XAttribute("length", stat.Len)
                            );

                    foreach (var item in Check_Total_overlay)
                    {
                        //string peroverlay = item.overlay_count > 0 ? ((item.overlay_correct * 100.0) / item.overlay_count).tostring("0.00") + "%" : "0.0%";
                        //string perwoverlay = item.overlay_count > 0 ? ((item.overlay_wrong * 100.0) / item.overlay_count).tostring("0.00") + "%" : "0.0%";
                        //string permoverlay = item.overlay_count > 0 ? ((item.overlay_missint * 100.0) / item.overlay_count).tostring("0.00") + "%" : "0.0%";
                        //string perfoverlay = item.overlay_identify > 0 ? ((item.overlay_false * 100.0) / item.overlay_identify).tostring("0.00") + "%" : "0.0%";

                        //xePages.Add(new XAttribute("countOverlay", overlay_All),
                        //    new XAttribute("countCorrectOverlay", overlay_All));
                            //new XAttribute("percentCorrectOverlay", peroverlay),
                            //new XAttribute("countWrongOverlay", item.Overlay),
                            //new XAttribute("percentWrongOverlay", perwoverlay),
                            //new XAttribute("countMissingOverlay", item.Overlay_missint),
                            //new XAttribute("percentMissingOverlay", permoverlay),
                            //new XAttribute("countOverlayIdentify", overlay_All),
                            //new XAttribute("countFalseOverlayIdentify", item.Overlay_false),
                            //new XAttribute("percentFalseOverlayIdentify", perfoverlay));

                        //report.Add(xePages);
                    }

                    //string perbolt = stat.Boltfree_count > 0 ? ((stat.Boltfree_correct * 100.0) / stat.Boltfree_count).ToString("0.00") + "%" : "0.0%";
                    //string permbolt = stat.Boltfree_count > 0 ? ((stat.Boltfree_missing * 100.0) / stat.Boltfree_count).ToString("0.00") + "%" : "0.0%";
                    //string permbolt1 = stat.Boltfree_count > 0 ? ((stat.Boltfree_as_bolt * 100.0) / stat.Boltfree_count).ToString("0.00") + "%" : "0.0%";
                    //string permbolt2 = stat.Boltfree_count > 0 ? ((stat.Boltfree_as_wrongoverlay * 100.0) / stat.Boltfree_count).ToString("0.00") + "%" : "0.0%";
                    //string permbolt3 = stat.Boltfree_count > 0 ? ((stat.Boltfree_as_missingoverlay * 100.0) / stat.Boltfree_count).ToString("0.00") + "%" : "0.0%";
                    //string perfbolt = bolts_before + bolts_after > 0 ? ((stat.Boltfree_false * 100.0) / bolts_before + bolts_after).ToString("0.00") + "%" : "0.0%";
                    //string perfbolt1 = bolts_before + bolts_after > 0 ? ((stat.Boltfree_false_bolt * 100.0) / bolts_before + bolts_after).ToString("0.00") + "%" : "0.0%";
                    //string perfbolt2 = bolts_before + bolts_after > 0 ? ((stat.Boltfree_false_wrongoverlay * 100.0) / bolts_before + bolts_after).ToString("0.00") + "%" : "0.0%";
                    //string perfbolt3 = bolts_before + bolts_after > 0 ? ((stat.Boltfree_false_missingoverlay * 100.0) / bolts_before + bolts_after).ToString("0.00") + "%" : "0.0%";

                    xePages.Add(new XAttribute("countBoltFree", bolts_before + bolts_after),
                        new XAttribute("countCorrectBoltFree", bolts_before + bolts_after),
                        new XAttribute("percentCorrectBoltFree", "-999"),
                        new XAttribute("countMissingBoltFree", "-999"),
                        new XAttribute("percentMissingyBoltFree", "-999"),
                        new XAttribute("countBoltFreeAsBolt", "-999"),
                        new XAttribute("percentBoltFreeAsBolt", "-999"),
                        new XAttribute("countBoltFreeAsWrongOverlay", "-999"),
                        new XAttribute("percentBoltFreeAsWrongOverlay", "-999"),
                        new XAttribute("countBoltFreeAsMissingOverlay", "-999"),
                        new XAttribute("percentBoltFreeAsMissingOverlay", "-999"),
                        new XAttribute("countBoltFreeIdentify", bolts_before + bolts_after),
                        new XAttribute("countFalseBoltFreeIdentify", "-999"),
                        new XAttribute("percentFalseBoltFreeIdentify", "-999"),
                        new XAttribute("countFalseBoltFreeAsBolt", "-999"),
                        new XAttribute("percentFalseBoltFreeAsBolt", "-999"),
                        new XAttribute("countFalseBoltFreeAsWrongOverlay", "-999"),
                        new XAttribute("percentFalseBoltFreeAsWrongOverlay", "-999"),
                        new XAttribute("countFalseBoltFreeAsMissingOverlay", "-999"),
                        new XAttribute("percentFalseBoltFreeAsMissingOverlay", "-999"));

                    string perjoint = "-999";
                    string perwjoint = "-999";
                    string perwjoint1 = "-999";
                    string permjoint = "-999" ;
                    string permjoint1 = "-999";
                    string perfjoint = "-999";

                    xePages.Add(new XAttribute("countJoint", gap_All),
                        new XAttribute("countCorrectJoint", gap_All),
                        new XAttribute("percentCorrectJoint", perjoint),
                        new XAttribute("countWrongJoint", "-999"),
                        new XAttribute("percentWrongJoint", perwjoint),
                        new XAttribute("countWrongIsoJoint", "-999"),
                        new XAttribute("percentWrongIsoJoint", perwjoint1),
                        new XAttribute("countMissingJoint", "-999"),
                        new XAttribute("percentMissingJoint", permjoint),
                        new XAttribute("countMissingIsoJoint", "-999"),
                        new XAttribute("percentMissingIsoJoint", permjoint1),
                        new XAttribute("countJointIdentify", gap_All),
                        new XAttribute("countFalseJointIdentify", "-999"),
                        new XAttribute("percentFalseJointIdentify", perfjoint));

                    string permarks = "-999";
                    string perwmarks = "-999";
                    string permmarks = "-999";
                    string perfmarks = "-999" ;

                    xePages.Add(new XAttribute("countMarks", threat_id_All),
                        new XAttribute("countCorrectMarks", threat_id_All),
                        new XAttribute("percentCorrectMarks", permarks),
                        new XAttribute("countWrongMarks", "-999"),
                        new XAttribute("percentWrongMarks", perwmarks),
                        new XAttribute("countMissingMarks", "-999"),
                        new XAttribute("percentMissingMarks", permmarks),
                        new XAttribute("countMarksIdentify", threat_id_All),
                        new XAttribute("countFalseMarksIdentify", "-999"),
                        new XAttribute("percentFalseMarksIdentify", perfmarks));

                    string perdefs = "-999";
                    string permdefs = "-999";
                    string perfdefs = "-999";

                    xePages.Add(new XAttribute("countDefects", "-999"),
                        new XAttribute("countCorrectDefects", "-999"),
                        new XAttribute("percentCorrectDefects", perdefs),
                        new XAttribute("countMissingDefects", "-999"),
                        new XAttribute("percentMissingDefects", permdefs),
                        new XAttribute("countMarksDefects", "-999"),
                        new XAttribute("countFalseDefectsIdentify", "-999"),
                        new XAttribute("percentFalseDefectsIdentify", perfdefs));

                    string perfast = "-999";
                    string permfast = "-999";
                    string persleep = "-999";
                    string perwsleep = "-999";
                    string permsleep = "-999";
                    string perfsleep = "-999";
                    string peranti = "-999";
                    string perwanti = "-999";
                    string permanti = "-999";
                    string perfanti = "-999";
                    string perswitch = "-999";
                    string perwswitch = "-999";
                    string permswitch = "-999";
                    string perfswitch = "-999";

                    xePages.Add(new XAttribute("countFastening", fastening_All),
                        new XAttribute("countCorrectFastening", fastening_All),
                        new XAttribute("percentCorrectFastening", perfast),
                        new XAttribute("countMissingFastening", "-999"),
                        new XAttribute("percentMissingFastening", permfast),
                        new XAttribute("countSleeper", sleepers_All),
                        new XAttribute("countCorrectSleeper", sleepers_All),
                        new XAttribute("percentCorrectSleeper", persleep),
                        new XAttribute("countWrongSleeper", "-999"),
                        new XAttribute("percentWrongSleeper", perwsleep),
                        new XAttribute("countMissingSleeper", "-999"),
                        new XAttribute("percentMissingSleeper", permsleep),
                        new XAttribute("countSleeperIdentify", sleepers_All),
                        new XAttribute("countFalseSleeperIdentify", "-999"),
                        new XAttribute("percentFalseSleeperIdentify", perfsleep),
                        new XAttribute("countAntiTheft", "-999"),
                        new XAttribute("countCorrectAntiTheft", "-999"),
                        new XAttribute("percentCorrectAntiTheft", peranti),
                        new XAttribute("countWrongAntiTheft", "-999"),
                        new XAttribute("percentWrongAntiTheft", perwanti),
                        new XAttribute("countMissingAntiTheft", "-999"),
                        new XAttribute("percentMissingAntiTheft", permanti),
                        new XAttribute("countAntiTheftIdentify", "-999"),
                        new XAttribute("countFalseAntiTheftIdentify", "-999"),
                        new XAttribute("percentFalseAntiTheftIdentify", perfanti),
                        new XAttribute("countSwitch", "-999"),
                        new XAttribute("countCorrectSwitch", "-999"),
                        new XAttribute("percentCorrectSwitch", perswitch),
                        new XAttribute("countWrongSwitch", "-999"),
                        new XAttribute("percentWrongSwitch", perwswitch),
                        new XAttribute("countMissingSwitch", "-999"),
                        new XAttribute("percentMissingSwitch", permswitch),
                        new XAttribute("countSwitchIdentify", "-999"),
                        new XAttribute("countFalseSwitchIdentify", "-999"),
                        new XAttribute("percentFalseSwitchIdentify", perfswitch));


                    //foreach (var stat in statistics)
                    //{


                    //    string perbolt = stat.Boltfree_count > 0 ? ((stat.Boltfree_correct * 100.0) / stat.Boltfree_count).ToString("0.00") + "%" : "0.0%";
                    //    string permbolt = stat.Boltfree_count > 0 ? ((stat.Boltfree_missing * 100.0) / stat.Boltfree_count).ToString("0.00") + "%" : "0.0%";
                    //    string permbolt1 = stat.Boltfree_count > 0 ? ((stat.Boltfree_as_bolt * 100.0) / stat.Boltfree_count).ToString("0.00") + "%" : "0.0%";
                    //    string permbolt2 = stat.Boltfree_count > 0 ? ((stat.Boltfree_as_wrongoverlay * 100.0) / stat.Boltfree_count).ToString("0.00") + "%" : "0.0%";
                    //    string permbolt3 = stat.Boltfree_count > 0 ? ((stat.Boltfree_as_missingoverlay * 100.0) / stat.Boltfree_count).ToString("0.00") + "%" : "0.0%";
                    //    string perfbolt = bolts_before + bolts_after > 0 ? ((stat.Boltfree_false * 100.0) / bolts_before + bolts_after).ToString("0.00") + "%" : "0.0%";
                    //    string perfbolt1 = bolts_before + bolts_after > 0 ? ((stat.Boltfree_false_bolt * 100.0) / bolts_before + bolts_after).ToString("0.00") + "%" : "0.0%";
                    //    string perfbolt2 = bolts_before + bolts_after > 0 ? ((stat.Boltfree_false_wrongoverlay * 100.0) / bolts_before + bolts_after).ToString("0.00") + "%" : "0.0%";
                    //    string perfbolt3 = bolts_before + bolts_after > 0 ? ((stat.Boltfree_false_missingoverlay * 100.0) / bolts_before + bolts_after).ToString("0.00") + "%" : "0.0%";

                    //    xePages.Add(new XAttribute("countBoltFree", bolts_before + bolts_after),
                    //        new XAttribute("countCorrectBoltFree", bolts_before + bolts_after),
                    //        new XAttribute("percentCorrectBoltFree", perbolt),
                    //        new XAttribute("countMissingBoltFree", stat.Boltfree_missing),
                    //        new XAttribute("percentMissingyBoltFree", permbolt),
                    //        new XAttribute("countBoltFreeAsBolt", stat.Boltfree_as_bolt),
                    //        new XAttribute("percentBoltFreeAsBolt", permbolt1),
                    //        new XAttribute("countBoltFreeAsWrongOverlay", stat.Boltfree_as_wrongoverlay),
                    //        new XAttribute("percentBoltFreeAsWrongOverlay", permbolt2),
                    //        new XAttribute("countBoltFreeAsMissingOverlay", stat.Boltfree_as_missingoverlay),
                    //        new XAttribute("percentBoltFreeAsMissingOverlay", permbolt3),
                    //        new XAttribute("countBoltFreeIdentify", bolts_before + bolts_after),
                    //        new XAttribute("countFalseBoltFreeIdentify", stat.Boltfree_false),
                    //        new XAttribute("percentFalseBoltFreeIdentify", perfbolt),
                    //        new XAttribute("countFalseBoltFreeAsBolt", stat.Boltfree_false_bolt),
                    //        new XAttribute("percentFalseBoltFreeAsBolt", perfbolt1),
                    //        new XAttribute("countFalseBoltFreeAsWrongOverlay", stat.Boltfree_false_wrongoverlay),
                    //        new XAttribute("percentFalseBoltFreeAsWrongOverlay", perfbolt2),
                    //        new XAttribute("countFalseBoltFreeAsMissingOverlay", stat.Boltfree_false_missingoverlay),
                    //        new XAttribute("percentFalseBoltFreeAsMissingOverlay", perfbolt3));

                    //    string perjoint = gap_All > 0 ? ((gap_All * 100.0) / gap_All).ToString("0.00") + "%" : "0.0%";
                    //    string perwjoint = gap_All > 0 ? ((stat.Joint_wrong * 100.0) / gap_All).ToString("0.00") + "%" : "0.0%";
                    //    string perwjoint1 = gap_All > 0 ? ((stat.Isojoint_wrong * 100.0) / gap_All).ToString("0.00") + "%" : "0.0%";
                    //    string permjoint = gap_All > 0 ? ((stat.Joint_missing * 100.0) / gap_All).ToString("0.00") + "%" : "0.0%";
                    //    string permjoint1 = gap_All > 0 ? ((stat.Isojoint_missing * 100.0) / gap_All).ToString("0.00") + "%" : "0.0%";
                    //    string perfjoint = gap_All > 0 ? ((stat.Joint_false * 100.0) / gap_All).ToString("0.00") + "%" : "0.0%";

                    //    xePages.Add(new XAttribute("countJoint", gap_All),
                    //        new XAttribute("countCorrectJoint", gap_All),
                    //        new XAttribute("percentCorrectJoint", perjoint),
                    //        new XAttribute("countWrongJoint", stat.Joint_wrong),
                    //        new XAttribute("percentWrongJoint", perwjoint),
                    //        new XAttribute("countWrongIsoJoint", stat.Isojoint_wrong),
                    //        new XAttribute("percentWrongIsoJoint", perwjoint1),
                    //        new XAttribute("countMissingJoint", stat.Joint_missing),
                    //        new XAttribute("percentMissingJoint", permjoint),
                    //        new XAttribute("countMissingIsoJoint", stat.Isojoint_missing),
                    //        new XAttribute("percentMissingIsoJoint", permjoint1),
                    //        new XAttribute("countJointIdentify", gap_All),
                    //        new XAttribute("countFalseJointIdentify", stat.Joint_false),
                    //        new XAttribute("percentFalseJointIdentify", perfjoint));

                    //    string permarks = threat_id_All > 0 ? ((threat_id_All * 100.0) / threat_id_All).ToString("0.00") + "%" : "0.0%";
                    //    string perwmarks = threat_id_All > 0 ? ((stat.Marks_wrong * 100.0) / threat_id_All).ToString("0.00") + "%" : "0.0%";
                    //    string permmarks = stat.Marks_count > 0 ? ((stat.Marks_missing * 100.0) / stat.Marks_count).ToString("0.00") + "%" : "0.0%";
                    //    string perfmarks = stat.Marks_identify > 0 ? ((stat.Marks_false * 100.0) / stat.Marks_identify).ToString("0.00") + "%" : "0.0%";

                    //    xePages.Add(new XAttribute("countMarks", threat_id_All),
                    //        new XAttribute("countCorrectMarks", threat_id_All),
                    //        new XAttribute("percentCorrectMarks", permarks),
                    //        new XAttribute("countWrongMarks", stat.Marks_wrong),
                    //        new XAttribute("percentWrongMarks", perwmarks),
                    //        new XAttribute("countMissingMarks", stat.Marks_missing),
                    //        new XAttribute("percentMissingMarks", permmarks),
                    //        new XAttribute("countMarksIdentify", threat_id_All),
                    //        new XAttribute("countFalseMarksIdentify", stat.Marks_false),
                    //        new XAttribute("percentFalseMarksIdentify", perfmarks));

                    //    string perdefs = stat.Defects_count > 0 ? ((stat.Defects_correct * 100.0) / stat.Defects_count).ToString("0.00") + "%" : "0.0%";
                    //    string permdefs = stat.Defects_count > 0 ? ((stat.Defects_missing * 100.0) / stat.Defects_count).ToString("0.00") + "%" : "0.0%";
                    //    string perfdefs = stat.Defects_identify > 0 ? ((stat.Defects_false * 100.0) / stat.Defects_identify).ToString("0.00") + "%" : "0.0%";

                    //    xePages.Add(new XAttribute("countDefects", stat.Defects_count),
                    //        new XAttribute("countCorrectDefects", stat.Defects_correct),
                    //        new XAttribute("percentCorrectDefects", perdefs),
                    //        new XAttribute("countMissingDefects", stat.Defects_missing),
                    //        new XAttribute("percentMissingDefects", permdefs),
                    //        new XAttribute("countMarksDefects", stat.Defects_identify),
                    //        new XAttribute("countFalseDefectsIdentify", stat.Defects_false),
                    //        new XAttribute("percentFalseDefectsIdentify", perfdefs));

                    //    string perfast = fastening_All > 0 ? ((fastening_All * 100.0) / fastening_All).ToString("0.00") + "%" : "0.0%";
                    //    string permfast = fastening_All > 0 ? ((stat.Fastening_missing * 100.0) / fastening_All).ToString("0.00") + "%" : "0.0%";
                    //    string persleep = sleepers_All > 0 ? ((stat.Sleeper_correct * 100.0) / sleepers_All).ToString("0.00") + "%" : "0.0%";
                    //    string perwsleep = sleepers_All > 0 ? ((stat.Sleeper_wrong * 100.0) / sleepers_All).ToString("0.00") + "%" : "0.0%";
                    //    string permsleep = sleepers_All > 0 ? ((stat.Sleeper_missing * 100.0) / sleepers_All).ToString("0.00") + "%" : "0.0%";
                    //    string perfsleep = sleepers_All > 0 ? ((stat.Sleeper_false * 100.0) / sleepers_All).ToString("0.00") + "%" : "0.0%";
                    //    string peranti = stat.Antitheft_count > 0 ? ((stat.Antitheft_correct * 100.0) / stat.Antitheft_count).ToString("0.00") + "%" : "0.0%";
                    //    string perwanti = stat.Antitheft_count > 0 ? ((stat.Antitheft_wrong * 100.0) / stat.Antitheft_count).ToString("0.00") + "%" : "0.0%";
                    //    string permanti = stat.Antitheft_count > 0 ? ((stat.Antitheft_missing * 100.0) / stat.Antitheft_count).ToString("0.00") + "%" : "0.0%";
                    //    string perfanti = stat.Antitheft_identify > 0 ? ((stat.Antitheft_false * 100.0) / stat.Antitheft_identify).ToString("0.00") + "%" : "0.0%";
                    //    string perswitch = stat.Switch_count > 0 ? ((stat.Switch_correct * 100.0) / stat.Switch_count).ToString("0.00") + "%" : "0.0%";
                    //    string perwswitch = stat.Switch_count > 0 ? ((stat.Switch_wrong * 100.0) / stat.Switch_count).ToString("0.00") + "%" : "0.0%";
                    //    string permswitch = stat.Switch_count > 0 ? ((stat.Switch_missing * 100.0) / stat.Switch_count).ToString("0.00") + "%" : "0.0%";
                    //    string perfswitch = stat.Switch_identify > 0 ? ((stat.Switch_false * 100.0) / stat.Switch_identify).ToString("0.00") + "%" : "0.0%";

                    //    xePages.Add(new XAttribute("countFastening", fastening_All),
                    //        new XAttribute("countCorrectFastening", fastening_All),
                    //        new XAttribute("percentCorrectFastening", perfast),
                    //        new XAttribute("countMissingFastening", stat.Fastening_missing),
                    //        new XAttribute("percentMissingFastening", permfast),
                    //        new XAttribute("countSleeper", sleepers_All),
                    //        new XAttribute("countCorrectSleeper", sleepers_All),
                    //        new XAttribute("percentCorrectSleeper", persleep),
                    //        new XAttribute("countWrongSleeper", stat.Sleeper_wrong),
                    //        new XAttribute("percentWrongSleeper", perwsleep),
                    //        new XAttribute("countMissingSleeper", stat.Sleeper_missing),
                    //        new XAttribute("percentMissingSleeper", permsleep),
                    //        new XAttribute("countSleeperIdentify", sleepers_All),
                    //        new XAttribute("countFalseSleeperIdentify", stat.Sleeper_false),
                    //        new XAttribute("percentFalseSleeperIdentify", perfsleep),
                    //        new XAttribute("countAntiTheft", stat.Antitheft_count),
                    //        new XAttribute("countCorrectAntiTheft", stat.Antitheft_correct),
                    //        new XAttribute("percentCorrectAntiTheft", peranti),
                    //        new XAttribute("countWrongAntiTheft", stat.Antitheft_wrong),
                    //        new XAttribute("percentWrongAntiTheft", perwanti),
                    //        new XAttribute("countMissingAntiTheft", stat.Antitheft_missing),
                    //        new XAttribute("percentMissingAntiTheft", permanti),
                    //        new XAttribute("countAntiTheftIdentify", stat.Antitheft_identify),
                    //        new XAttribute("countFalseAntiTheftIdentify", stat.Antitheft_false),
                    //        new XAttribute("percentFalseAntiTheftIdentify", perfanti),
                    //        new XAttribute("countSwitch", stat.Switch_count),
                    //        new XAttribute("countCorrectSwitch", stat.Switch_correct),
                    //        new XAttribute("percentCorrectSwitch", perswitch),
                    //        new XAttribute("countWrongSwitch", stat.Switch_wrong),
                    //        new XAttribute("percentWrongSwitch", perwswitch),
                    //        new XAttribute("countMissingSwitch", stat.Switch_missing),
                    //        new XAttribute("percentMissingSwitch", permswitch),
                    //        new XAttribute("countSwitchIdentify", stat.Switch_identify),
                    //        new XAttribute("countFalseSwitchIdentify", stat.Switch_false),
                    //        new XAttribute("percentFalseSwitchIdentify", perfswitch));


                    //}
                    report.Add(xePages);
                }

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_Statistics.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_Statistics.html");
            }
        }
    }
}
