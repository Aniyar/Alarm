using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
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
    public class ProvenPath : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                List<Curve> curves = (MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();
                XDocument xdReport = new XDocument();


                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                var tripProcesses = RdStructureService.GetProcess(period, parentId, ProcessType.VideoProcess);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var dist_section = MainTrackStructureService.GetDistSectionByDistId(distance.Id) as List<DistSection>;

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {

                    List<Digression> notes = RdStructureService.GetDigressions(tripProcess.Trip_id, distance.Code, new int[] { 2 });
                    var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;

                    CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Any() ? curvesAdmUnits[0] : null;

                    var kms = RdStructureService.GetKilometerTrip(tripProcess.Trip_id);
                    if (kms.Count() == 0) continue;

                    XElement tripElem = new XElement("trip",
                        new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                        new XAttribute("direction", curvesAdmUnit.Direction),
                        new XAttribute("km", kms.Min() + "-" + kms.Max()),
                        new XAttribute("track", curvesAdmUnit.Track),
                        new XAttribute("check", tripProcess.GetProcessTypeName),
                        new XAttribute("road", roadName),
                        new XAttribute("distance", distance.Code),
                        new XAttribute("periodDate", period.Period),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("ps", tripProcess.Car));

                    var listBedemost = RdStructureService.GetBedemost(tripProcess.Trip_id) as List<Bedemost>;

                    if (listBedemost == null || listBedemost.Count == 0) continue;
                    List<ProvenPathReport> provenPaths = new List<ProvenPathReport>();
                    List<ProvenPathTotalReport> provenPathTotals = new List<ProvenPathTotalReport>();
                    XElement xeNote = new XElement("note");

                    foreach (Bedemost bedemost in listBedemost)
                    {
                        if (provenPaths.Where(tmp => tmp.Direction.Equals(bedemost.Naprav)).Count() > 0)
                        {
                            if (provenPaths.Where(tmp => tmp.Direction.Equals(bedemost.Naprav) && tmp.Pchu == bedemost.Pchu).Count() > 0)
                            {
                                if (provenPaths.Where(tmp => tmp.Direction.Equals(bedemost.Naprav) && tmp.Pchu == bedemost.Pchu && tmp.Pd == bedemost.Pd).Count() < 1)
                                {
                                    List<Bedemost> temp = listBedemost.Where(tmp => tmp.Naprav.Equals(bedemost.Naprav) && tmp.Pchu == bedemost.Pchu && tmp.Pd == bedemost.Pd).ToList();

                                    ProvenPathReport tempProven = new ProvenPathReport();
                                    tempProven.Direction = bedemost.Naprav;
                                    tempProven.CountDirection = 0;
                                    tempProven.Pchu = bedemost.Pchu;
                                    tempProven.CountPchu = 0;
                                    tempProven.Pd = bedemost.Pd;
                                    tempProven.Track = bedemost.Put;
                                    tempProven.KmAll = 0.0;
                                    tempProven.KmCheck = 0.0;
                                    tempProven.KmNotCheck = 0.0;

                                    //foreach (Bedemost tmepBed in temp)
                                    //{
                                    //    tempProven.KmAll += bedemost.Lkm;
                                    //    tempProven.KmCheck += bedemost.Lkm;
                                    //}

                                    var ld = dist_section.Where(tmp => tmp.Pchu == bedemost.Pchu && tmp.Pd == bedemost.Pd).ToList();

                                    tempProven.KmAll = ld.Last().Final_Km - ld.First().Start_Km;
                                    tempProven.KmCheck = temp.Last().Km - temp.First().Km;
                                    tempProven.KmNotCheck = tempProven.KmAll - tempProven.KmCheck;

                                    provenPaths.Add(tempProven);
                                }
                            }
                            else
                            {
                                List<Bedemost> temp = listBedemost.Where(tmp => tmp.Naprav.Equals(bedemost.Naprav) && tmp.Pchu == bedemost.Pchu && tmp.Pd == bedemost.Pd).ToList();

                                ProvenPathReport tempProven = new ProvenPathReport();
                                tempProven.Direction = bedemost.Naprav;
                                tempProven.CountDirection = 0;
                                tempProven.Pchu = bedemost.Pchu;
                                tempProven.CountPchu = listBedemost.Where(tmp => tmp.Naprav.Equals(bedemost.Naprav) && tmp.Pchu == bedemost.Pchu).GroupBy(tmp => tmp.Pd).Count();
                                tempProven.Pd = bedemost.Pd;
                                tempProven.Track = bedemost.Put;
                                tempProven.KmAll = 0.0;
                                tempProven.KmCheck = 0.0;
                                tempProven.KmNotCheck = 0.0;

                                //foreach (Bedemost tmepBed in temp)
                                //{
                                //    tempProven.KmAll += bedemost.Lkm;
                                //    tempProven.KmCheck += bedemost.Lkm;
                                //}

                                var ld = dist_section.Where(tmp => tmp.Pchu == bedemost.Pchu && tmp.Pd == bedemost.Pd).ToList();

                                tempProven.KmAll = ld.Last().Final_Km - ld.First().Start_Km;
                                tempProven.KmCheck = temp.Last().Km - temp.First().Km;
                                tempProven.KmNotCheck = tempProven.KmAll - tempProven.KmCheck;

                                provenPaths.Add(tempProven);
                            }
                        }
                        else
                        {
                            List<Bedemost> temp = listBedemost.Where(tmp => tmp.Naprav.Equals(bedemost.Naprav) && tmp.Pchu == bedemost.Pchu && tmp.Pd == bedemost.Pd).ToList();

                            ProvenPathReport tempProven = new ProvenPathReport();
                            tempProven.Direction = bedemost.Naprav;
                            tempProven.CountDirection = listBedemost.Where(tmp => tmp.Naprav.Equals(bedemost.Naprav)).GroupBy(tmp => tmp.Pd).Count();
                            tempProven.Pchu = bedemost.Pchu;
                            tempProven.CountPchu = listBedemost.Where(tmp => tmp.Naprav.Equals(bedemost.Naprav) && tmp.Pchu == bedemost.Pchu).GroupBy(tmp => tmp.Pd).Count();
                            tempProven.Pd = bedemost.Pd;
                            tempProven.Track = bedemost.Put;
                            tempProven.KmAll = 0.0;
                            tempProven.KmCheck = 0.0;
                            tempProven.KmNotCheck = 0.0;

                            //foreach (Bedemost tmepBed in temp)
                            //{
                            //    tempProven.KmAll += bedemost.Lkm;
                            //    tempProven.KmCheck += bedemost.Lkm;
                            //}

                            var ld = dist_section.Where(tmp => tmp.Pchu == bedemost.Pchu && tmp.Pd == bedemost.Pd).ToList();

                            tempProven.KmAll = ld.Last().Final_Km - ld.First().Start_Km;
                            tempProven.KmCheck = temp.Last().Km - temp.First().Km;
                            tempProven.KmNotCheck = tempProven.KmAll - tempProven.KmCheck;

                            temp = listBedemost.Where(tmp => tmp.Naprav.Equals(bedemost.Naprav)).ToList();

                            ProvenPathTotalReport tempTotal = new ProvenPathTotalReport();
                            tempTotal.Direction = bedemost.Naprav;
                            tempTotal.DKmAll = 0.0;
                            tempTotal.DKmCheck = 0.0;
                            tempTotal.DKmNotCheck = 0.0;

                            //foreach (Bedemost tmepBed in temp)
                            //{
                            //    tempTotal.DKmAll += bedemost.Lkm;
                            //    tempTotal.DKmCheck += bedemost.Lkm;
                            //}

                            tempTotal.DKmAll = dist_section.Last().RealFinalCoordinate - dist_section.First().RealStartCoordinate;
                            tempTotal.DKmCheck = temp.Sum(o=>o.Lkm);
                            tempTotal.DKmNotCheck = tempTotal.DKmAll - tempTotal.DKmCheck;


                            provenPaths.Add(tempProven);
                            provenPathTotals.Add(tempTotal);
                        }
                    }

                    int tempi = 0;
                    foreach (ProvenPathReport temp in provenPaths)
                    {
                        if (temp.CountDirection != 0 && !temp.Equals(provenPaths.First()))
                        {
                            try
                            {
                                ProvenPathTotalReport tempTotal = provenPathTotals[tempi];

                                double tempCheck = 0.0, tempNotCheck = 0.0;

                                if (tempTotal.DKmAll <= 0.0)
                                {
                                    tempCheck = 0.0;
                                    tempNotCheck = 0.0;
                                }
                                else
                                {
                                    tempCheck = (tempTotal.DKmCheck*100.0) / tempTotal.DKmAll;
                                    tempNotCheck = (tempTotal.DKmNotCheck) * 100.0/ tempTotal.DKmAll;
                                }

                                xeNote = new XElement("note",
                                    new XAttribute("total", tempTotal.Direction),
                                    new XAttribute("countDirection", -1),
                                    new XAttribute("countPchu", -1),
                                    new XAttribute("COUNTallKm", tempTotal.DKmAll.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                    new XAttribute("COUNTcheckKm", tempTotal.DKmCheck.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                    new XAttribute("COUNTcheckPersent", Math.Round(tempCheck, 2)),
                                    new XAttribute("COUNTnotcheckKm", tempTotal.DKmNotCheck.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                                    new XAttribute("COUNTnotcheckPersent", Math.Round(tempNotCheck, 2)));
                            }
                            catch
                            {
                                xeNote = new XElement("note",
                                    new XAttribute("total", "error"),
                                    new XAttribute("countDirection", -1),
                                    new XAttribute("countPchu", -1),
                                    new XAttribute("COUNTallKm", -1.0),
                                    new XAttribute("COUNTcheckKm", -1.0),
                                    new XAttribute("COUNTcheckPersent", -1),
                                    new XAttribute("COUNTnotcheckKm", -1.0),
                                    new XAttribute("COUNTnotcheckPersent", -1));
                            }

                            tempi += 1;
                            tripElem.Add(xeNote);
                        }

                        if (temp.KmAll <= 0.0)
                        {
                            temp.Check = 0.0;
                            temp.NotCheck = 0.0;
                        }
                        else
                        {
                            temp.Check = (temp.KmCheck * 100.0) / temp.KmAll ;
                            temp.NotCheck = (temp.KmNotCheck * 100.0) / temp.KmAll ;
                        }

                        xeNote = new XElement("note",
                            new XAttribute("total", ""),
                            new XAttribute("direction", temp.Direction),
                            new XAttribute("countDirection", temp.CountDirection),
                            new XAttribute("pchu", temp.Pchu),
                            new XAttribute("countPchu", temp.CountPchu),
                            new XAttribute("pd", temp.Pd),
                            new XAttribute("track", temp.Track),
                            new XAttribute("kmAll", temp.KmAll.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("kmCheck", temp.KmCheck.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("check", Math.Round(temp.Check,2)),
                            new XAttribute("kmNotCheck", temp.KmNotCheck.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("notCheck", Math.Round(temp.NotCheck, 2)));

                        tripElem.Add(xeNote);
                    }

                    try
                    {
                        ProvenPathTotalReport tempTotal = provenPathTotals[tempi];

                        double tempCheck = 0.0, tempNotCheck = 0.0;

                        if (tempTotal.DKmAll <= 0.0)
                        {
                            tempCheck = 0.0;
                            tempNotCheck = 0.0;
                        }
                        else
                        {
                            tempCheck = (tempTotal.DKmCheck * 100.0) / tempTotal.DKmAll;
                            tempNotCheck = (tempTotal.DKmNotCheck * 100.0) / tempTotal.DKmAll;
                        }

                        xeNote = new XElement("note",
                            new XAttribute("total", tempTotal.Direction),
                            new XAttribute("countDirection", -1),
                            new XAttribute("countPchu", -1),
                            new XAttribute("COUNTallKm", tempTotal.DKmAll.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("COUNTcheckKm", tempTotal.DKmCheck.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("COUNTcheckPersent", Math.Round(tempCheck, 2)),
                            new XAttribute("COUNTnotcheckKm", tempTotal.DKmNotCheck.ToString("f2", (System.Globalization.CultureInfo.InvariantCulture))),
                            new XAttribute("COUNTnotcheckPersent", Math.Round(tempNotCheck,2)));

                    }
                    catch
                    {
                        xeNote = new XElement("note",
                            new XAttribute("total", "error"),
                            new XAttribute("countDirection", -1),
                            new XAttribute("countPchu", -1),
                            new XAttribute("COUNTallKm", -1.0),
                            new XAttribute("COUNTcheckKm", -1.0),
                            new XAttribute("COUNTcheckPersent", -1),
                            new XAttribute("COUNTnotcheckKm", -1.0),
                            new XAttribute("COUNTnotcheckPersent", -1));
                    }

                    tripElem.Add(xeNote);

                    report.Add(tripElem);
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
    }
}
