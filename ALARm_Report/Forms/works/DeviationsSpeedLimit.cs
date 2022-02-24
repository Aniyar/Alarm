using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
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
    public class DeviationsSpeedLimit : Report
    {
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetUnit(AdmStructureConst.AdmNod, distance.Parent_Id) as AdmUnit;

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    XElement tripElem = new XElement("trip",
                        new XAttribute("type", tripProcess.GetProcessTypeName),
                        new XAttribute("road", road.Name),
                        new XAttribute("distance", distance.Name),
                        new XAttribute("data", period.Period),
                        new XAttribute("chief", tripProcess.Chief),
                        new XAttribute("car", tripProcess.Car));

                    var ListS3 = (RdStructureService.GetS3(tripProcess.Id, 4) as List<S3>).Where(s => s.Ogp > 0 && s.Ovp > 0);
                    string lastDirection = String.Empty, lastTrack = String.Empty;
                    int lastPchu = -1, lastPd = -1, lastPdb = -1, lastKm = -1;
                    int countUsh = 0, countP = 0, countPrR = 0, countPrL = 0, countUsk = 0, countPrzh2P = 0, countPrzhP = 0, countPrzhPrL = 0, count100K = 0;
                    XElement xeDirection = new XElement("directions");
                    XElement xeTracks = new XElement("tracks");
                    foreach (var s3 in ListS3)
                    {
                        if (s3.Naprav.Equals(lastDirection))
                        { 
                            if (s3.Put.Equals(lastTrack))
                            {
                                if (s3.Pchu == lastPchu)
                                {
                                    if (s3.Pd == lastPd)
                                    {
                                        if (s3.Pdb == lastPdb)
                                        {
                                            if (s3.Km == lastKm)
                                            {
                                                XElement xeNote = new XElement("note",
                                                    new XAttribute("pchu", ""),
                                                    new XAttribute("pd", ""),
                                                    new XAttribute("pdb", ""),
                                                    new XAttribute("km", ""),
                                                    new XAttribute("m", s3.Meter),
                                                    new XAttribute("found_date", s3.TripDateTime.ToString("dd.MM.yyyy_hh:mm")),
                                                    new XAttribute("digression", s3.Ots),
                                                    new XAttribute("count", s3.Kol),
                                                    new XAttribute("deviation", s3.Otkl),
                                                    new XAttribute("len", s3.Len),
                                                    new XAttribute("speed", s3.Uv.ToString() + "/" + s3.Uvg.ToString() + "/" + s3.Uvg.ToString()),
                                                    new XAttribute("limit_speed", s3.Ovp.ToString() + "/" + s3.Ogp.ToString() + "/" + s3.Ogp.ToString()),
                                                    new XAttribute("node", s3.Primech));

                                                switch (s3.Ots)
                                                {
                                                    case "Уш":
                                                        countUsh += 1;
                                                        break;
                                                    case "П":
                                                        countP += 1;
                                                        break;
                                                    case "Пр.п":
                                                        countPrR += 1;
                                                        break;
                                                    case "Пр.л":
                                                        countPrL += 1;
                                                        break;
                                                    case "Анп":
                                                        countUsk += 1;
                                                        break;
                                                    case "прж2П":
                                                        countPrzh2P += 1;
                                                        break;
                                                    case "пржП":
                                                        countPrzhP += 1;
                                                        break;
                                                    case "пржПр.л":
                                                        countPrzhPrL += 1;
                                                        break;
                                                    case "К100":
                                                        count100K += 1;
                                                        break;
                                                }

                                                xeTracks.Add(xeNote);
                                            }
                                            else
                                            {
                                                lastKm = s3.Km;

                                                XElement xeNote = new XElement("note",
                                                    new XAttribute("pchu", ""),
                                                    new XAttribute("pd", ""),
                                                    new XAttribute("pdb", ""),
                                                    new XAttribute("km", s3.Km),
                                                    new XAttribute("m", s3.Meter),
                                                    new XAttribute("found_date", s3.TripDateTime.ToString("dd.MM.yyyy_hh:mm")),
                                                    new XAttribute("digression", s3.Ots),
                                                    new XAttribute("count", s3.Kol),
                                                    new XAttribute("deviation", s3.Otkl),
                                                    new XAttribute("len", s3.Len),
                                                    new XAttribute("speed", s3.Uv.ToString() + "/" + s3.Uvg.ToString() + "/" + s3.Uvg.ToString()),
                                                    new XAttribute("limit_speed", s3.Ovp.ToString() + "/" + s3.Ogp.ToString() + "/" + s3.Ogp.ToString()),
                                                    new XAttribute("node", s3.Primech));

                                                switch (s3.Ots)
                                                {
                                                    case "Уш":
                                                        countUsh += 1;
                                                        break;
                                                    case "П":
                                                        countP += 1;
                                                        break;
                                                    case "Пр.п":
                                                        countPrR += 1;
                                                        break;
                                                    case "Пр.л":
                                                        countPrL += 1;
                                                        break;
                                                    case "Уск":
                                                        countUsk += 1;
                                                        break;
                                                    case "прж2П":
                                                        countPrzh2P += 1;
                                                        break;
                                                    case "пржП":
                                                        countPrzhP += 1;
                                                        break;
                                                    case "пржПр.л":
                                                        countPrzhPrL += 1;
                                                        break;
                                                    case "К100":
                                                        count100K += 1;
                                                        break;
                                                }

                                                xeTracks.Add(xeNote);
                                            }
                                        }
                                        else
                                        {
                                            lastPdb = s3.Pdb;
                                            lastKm = s3.Km;

                                            XElement xeNote = new XElement("note",
                                                new XAttribute("pchu", ""),
                                                new XAttribute("pd", ""),
                                                new XAttribute("pdb", s3.Pdb),
                                                new XAttribute("km", s3.Km),
                                                new XAttribute("m", s3.Meter),
                                                new XAttribute("found_date", s3.TripDateTime.ToString("dd.MM.yyyy_hh:mm")),
                                                new XAttribute("digression", s3.Ots),
                                                new XAttribute("count", s3.Kol),
                                                new XAttribute("deviation", s3.Otkl),
                                                new XAttribute("len", s3.Len),
                                                new XAttribute("speed", s3.Uv.ToString() + "/" + s3.Uvg.ToString() + "/" + s3.Uvg.ToString()),
                                                new XAttribute("limit_speed", s3.Ovp.ToString() + "/" + s3.Ogp.ToString() + "/" + s3.Ogp.ToString()),
                                                new XAttribute("node", s3.Primech));

                                            switch (s3.Ots)
                                            {
                                                case "Уш":
                                                    countUsh += 1;
                                                    break;
                                                case "П":
                                                    countP += 1;
                                                    break;
                                                case "Пр.п":
                                                    countPrR += 1;
                                                    break;
                                                case "Пр.л":
                                                    countPrL += 1;
                                                    break;
                                                case "Уск":
                                                    countUsk += 1;
                                                    break;
                                                case "прж2П":
                                                    countPrzh2P += 1;
                                                    break;
                                                case "пржП":
                                                    countPrzhP += 1;
                                                    break;
                                                case "пржПр.л":
                                                    countPrzhPrL += 1;
                                                    break;
                                                case "К100":
                                                    count100K += 1;
                                                    break;
                                            }

                                            xeTracks.Add(xeNote);
                                        }
                                    }
                                    else
                                    {
                                        lastPd = s3.Pd;
                                        lastPdb = s3.Pdb;
                                        lastKm = s3.Km;

                                        XElement xeNote = new XElement("note",
                                            new XAttribute("pchu", ""),
                                            new XAttribute("pd", s3.Pd),
                                            new XAttribute("pdb", s3.Pdb),
                                            new XAttribute("km", s3.Km),
                                            new XAttribute("m", s3.Meter),
                                            new XAttribute("found_date", s3.TripDateTime.ToString("dd.MM.yyyy_hh:mm")),
                                            new XAttribute("digression", s3.Ots),
                                            new XAttribute("count", s3.Kol),
                                            new XAttribute("deviation", s3.Otkl),
                                            new XAttribute("len", s3.Len),
                                            new XAttribute("speed", s3.Uv.ToString() + "/" + s3.Uvg.ToString() + "/" + s3.Uvg.ToString()),
                                            new XAttribute("limit_speed", s3.Ovp.ToString() + "/" + s3.Ogp.ToString() + "/" + s3.Ogp.ToString()),
                                            new XAttribute("node", s3.Primech));

                                        switch (s3.Ots)
                                        {
                                            case "Уш":
                                                countUsh += 1;
                                                break;
                                            case "П":
                                                countP += 1;
                                                break;
                                            case "Пр.п":
                                                countPrR += 1;
                                                break;
                                            case "Пр.л":
                                                countPrL += 1;
                                                break;
                                            case "Уск":
                                                countUsk += 1;
                                                break;
                                            case "прж2П":
                                                countPrzh2P += 1;
                                                break;
                                            case "пржП":
                                                countPrzhP += 1;
                                                break;
                                            case "пржПр.л":
                                                countPrzhPrL += 1;
                                                break;
                                            case "К100":
                                                count100K += 1;
                                                break;
                                        }

                                        xeTracks.Add(xeNote);
                                    }
                                }
                                else
                                {
                                    lastPchu = s3.Pchu;
                                    lastPd = s3.Pd;
                                    lastPdb = s3.Pdb;
                                    lastKm = s3.Km;

                                    XElement xeNote = new XElement("note",
                                        new XAttribute("pchu", s3.Pchu),
                                        new XAttribute("pd", s3.Pd),
                                        new XAttribute("pdb", s3.Pdb),
                                        new XAttribute("km", s3.Km),
                                        new XAttribute("m", s3.Meter),
                                        new XAttribute("found_date", s3.TripDateTime.ToString("dd.MM.yyyy_hh:mm")),
                                        new XAttribute("digression", s3.Ots),
                                        new XAttribute("count", s3.Kol),
                                        new XAttribute("deviation", s3.Otkl),
                                        new XAttribute("len", s3.Len),
                                        new XAttribute("speed", s3.Uv.ToString() + "/" + s3.Uvg.ToString() + "/" + s3.Uvg.ToString()),
                                        new XAttribute("limit_speed", s3.Ovp.ToString() + "/" + s3.Ogp.ToString() + "/" + s3.Ogp.ToString()),
                                        new XAttribute("node", s3.Primech));

                                    switch (s3.Ots)
                                    {
                                        case "Уш":
                                            countUsh += 1;
                                            break;
                                        case "П":
                                            countP += 1;
                                            break;
                                        case "Пр.п":
                                            countPrR += 1;
                                            break;
                                        case "Пр.л":
                                            countPrL += 1;
                                            break;
                                        case "Уск":
                                            countUsk += 1;
                                            break;
                                        case "прж2П":
                                            countPrzh2P += 1;
                                            break;
                                        case "пржП":
                                            countPrzhP += 1;
                                            break;
                                        case "пржПр.л":
                                            countPrzhPrL += 1;
                                            break;
                                        case "К100":
                                            count100K += 1;
                                            break;
                                    }

                                    xeTracks.Add(xeNote);
                                }
                            }
                            else
                            {
                                if (!lastTrack.Equals(String.Empty))
                                {
                                    xeDirection.Add(xeTracks);
                                }
                                xeTracks = new XElement("tracks",
                                    new XAttribute("track", s3.Put));

                                lastTrack = s3.Put;
                                lastPchu = s3.Pchu;
                                lastPd = s3.Pd;
                                lastPdb = s3.Pdb;
                                lastKm = s3.Km;

                                XElement xeNote = new XElement("note",
                                    new XAttribute("pchu", s3.Pchu),
                                    new XAttribute("pd", s3.Pd),
                                    new XAttribute("pdb", s3.Pdb),
                                    new XAttribute("km", s3.Km),
                                    new XAttribute("m", s3.Meter),
                                    new XAttribute("found_date", s3.TripDateTime.ToString("dd.MM.yyyy_hh:mm")),
                                    new XAttribute("digression", s3.Ots),
                                    new XAttribute("count", s3.Kol),
                                    new XAttribute("deviation", s3.Otkl),
                                    new XAttribute("len", s3.Len),
                                    new XAttribute("speed", s3.Uv.ToString() + "/" + s3.Uvg.ToString() + "/" + s3.Uvg.ToString()),
                                    new XAttribute("limit_speed", s3.Ovp.ToString() + "/" + s3.Ogp.ToString() + "/" + s3.Ogp.ToString()),
                                    new XAttribute("node", s3.Primech));

                                switch (s3.Ots)
                                {
                                    case "Уш":
                                        countUsh += 1;
                                        break;
                                    case "П":
                                        countP += 1;
                                        break;
                                    case "Пр.п":
                                        countPrR += 1;
                                        break;
                                    case "Пр.л":
                                        countPrL += 1;
                                        break;
                                    case "Уск":
                                        countUsk += 1;
                                        break;
                                    case "прж2П":
                                        countPrzh2P += 1;
                                        break;
                                    case "пржП":
                                        countPrzhP += 1;
                                        break;
                                    case "пржПр.л":
                                        countPrzhPrL += 1;
                                        break;
                                    case "К100":
                                        count100K += 1;
                                        break;
                                }

                                xeTracks.Add(xeNote);
                            }
                        }
                        else
                        {
                            if (!lastDirection.Equals(String.Empty))
                            {
                                xeDirection.Add(xeTracks);
                                tripElem.Add(xeDirection);
                            }
                            xeDirection = new XElement("directions",
                                new XAttribute("direction", s3.Direction_full));
                            xeTracks = new XElement("tracks",
                                new XAttribute("track", s3.Put));

                            lastDirection = s3.Naprav;
                            lastTrack = s3.Put;
                            lastPchu = s3.Pchu;
                            lastPd = s3.Pd;
                            lastPdb = s3.Pdb;
                            lastKm = s3.Km;

                            XElement xeNote = new XElement("note",
                                new XAttribute("pchu", s3.Pchu),
                                new XAttribute("pd", s3.Pd),
                                new XAttribute("pdb", s3.Pdb),
                                new XAttribute("km", s3.Km),
                                new XAttribute("m", s3.Meter),
                                new XAttribute("found_date", s3.TripDateTime.ToString("dd.MM.yyyy_hh:mm")),
                                new XAttribute("digression", s3.Ots),
                                new XAttribute("count", s3.Kol),
                                new XAttribute("deviation", s3.Otkl),
                                new XAttribute("len", s3.Len),
                                new XAttribute("speed", s3.Uv.ToString() + "/" + s3.Uvg.ToString() + "/" + s3.Uvg.ToString()),
                                new XAttribute("limit_speed", s3.Ovp.ToString() + "/" + s3.Ogp.ToString() + "/" + s3.Ogp.ToString()),
                                new XAttribute("node", s3.Primech));

                            switch (s3.Ots)
                            {
                                case "Уш":
                                    countUsh += 1;
                                    break;
                                case "П":
                                    countP += 1;
                                    break;
                                case "Пр.п":
                                    countPrR += 1;
                                    break;
                                case "Пр.л":
                                    countPrL += 1;
                                    break;
                                case "Уск":
                                    countUsk += 1;
                                    break;
                                case "прж2П":
                                    countPrzh2P += 1;
                                    break;
                                case "пржП":
                                    countPrzhP += 1;
                                    break;
                                case "пржПр.л":
                                    countPrzhPrL += 1;
                                    break;
                                case "К100":
                                    count100K += 1;
                                    break;
                            }

                            xeTracks.Add(xeNote);
                        }
                    }

                    tripElem.Add(new XAttribute("countDistance", count100K + countP + countPrL + countPrR + countPrzh2P + countPrzhP + countPrzhPrL + countUsh + countUsk),
                        new XAttribute("countUsh", countUsh),
                        new XAttribute("countP", countP),
                        new XAttribute("countPrR", countPrR),
                        new XAttribute("countPrL", countPrL),
                        new XAttribute("countUsk", countUsk),
                        new XAttribute("countPrzh2P", countPrzh2P),
                        new XAttribute("countPrzhP", countPrzhP),
                        new XAttribute("countPrzhPrL", countPrzhPrL),
                        new XAttribute("countK100", count100K));

                    xeDirection.Add(xeTracks);
                    tripElem.Add(xeDirection);
                    report.Add(tripElem);
                }
                xdReport.Add(report);
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_DeviationsSpeedLimit.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_DeviationsSpeedLimit.html");
            }
        }
    }
}
