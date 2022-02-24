using ALARm.Core;
using ALARm.Core.Report;
using ALARm.Services;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    public class OsnovParam : GraphicDiagrams
    {
        private float LevelPosition = 32.5f;
        private float LevelStep = 7.5f;
        private float LevelKoef = 0.25f;

        private float StraighRighttPosition = 62f;
        private float StrightStep = 15f;
        private float StrightKoef = 0.5f;
        private float GaugeKoef = 0.5f;
        private float ProsKoef = 0.5f;

        private float StrightLeftPosition = 71f;

        private float GaugePosition = 100.5f;

        private float ProsRightPosition = 124.5f;

        private float ProsLeftPosition = 138.5f;

       


        public override void Process(long parentId, ReportTemplate template, ReportPeriod period,
            MetroProgressBar progressBar)
        {
            diagramName = "Дубликат";
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                float koef = 3.5f / 0.6f;
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");
                var tripProcesses = RdStructureService.GetAdditionalParametersProcess(period);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }
                int svgIndex = template.Xsl.IndexOf("</svg>");
                template.Xsl = template.Xsl.Insert(svgIndex, righstSideXslt());
                foreach (var tripProcess in tripProcesses)
                {
                    var trip = RdStructureService.GetTrip(tripProcess.Id);
                    var kilometers = RdStructureService.GetKilometersByTrip(trip);
                    progressBar.Maximum = kilometers.Count;
                    foreach (var kilometer in kilometers)
                    {
                        progressBar.Value = kilometers.IndexOf(kilometer) + 1;
                        var xml = XDocument.Load("xmlgraph/" + kilometer.Number + ".xml");
                        List<float> prosLeft = xml.Root.Element("prosleft").Elements("pr").Select(x => (float)Convert.ToDouble(x.Value)).ToList();
                        List<float> prosRight = xml.Root.Element("prosright").Elements("pr").Select(x => (float)Convert.ToDouble(x.Value)).ToList();
                        List<float> fsh0 = xml.Root.Element("fsh0").Elements("sh0").Select(x => (float)Convert.ToDouble(x.Value)).ToList();
                        List<float> fsh = xml.Root.Element("fsh").Elements("sh").Select(x => (float)Convert.ToDouble(x.Value)).ToList();
                        List<float> fsr_rh1 = xml.Root.Element("fsr_rh1").Elements("rh").Select(x => (float)Convert.ToDouble(x.Value)).ToList();
                        List<float> frih1 = xml.Root.Element("frih1").Elements("rh").Select(x => (float)Convert.ToDouble(x.Value)).ToList();
                        List<float> frih10 = xml.Root.Element("frih10").Elements("rh").Select(x => (float)Convert.ToDouble(x.Value)).ToList();
                        List<float> fsr_rh2 = xml.Root.Element("fsr_rh2").Elements("rh").Select(x => (float)Convert.ToDouble(x.Value)).ToList();
                        List<float> frih2 = xml.Root.Element("frih2").Elements("rh").Select(x => (float)Convert.ToDouble(x.Value)).ToList();
                        List<float> frih20 = xml.Root.Element("frih20").Elements("rh").Select(x => (float)Convert.ToDouble(x.Value)).ToList();
                        List<float> fsr_urb = xml.Root.Element("fsr_urb").Elements("urb").Select(x => (float)Convert.ToDouble(x.Value)).ToList();
                        List<float> furb = xml.Root.Element("furb").Elements("urb").Select(x => (float)Convert.ToDouble(x.Value)).ToList();
                        List<float> furb0 = xml.Root.Element("furb0").Elements("urb").Select(x => (float)Convert.ToDouble(x.Value)).ToList();
                        List<float> piketspeed = xml.Root.Elements("piketspeed").Select(x => (float)Convert.ToDouble(x.Value)).ToList();

                        List<AlertNote> notes = (from note in xml.Root.Element("notes").Elements("note")
                                                 select new AlertNote()
                                                 {
                                                     Note = (string)note.Attribute("note"),
                                                     Top = (int)note.Attribute("top"),
                                                     Left = (int)note.Attribute("left"),
                                                     FontStyle = (int)note.Attribute("fstyle")
                                                 }).ToList();
                        tripProcess.Direction = (Direction)int.Parse(xml.Root.Attribute("axisinv").Value);
                        string drawdownRight = string.Empty, drawdownLeft = string.Empty, gauge = string.Empty, zeroGauge = string.Empty,
                                zeroStraighteningRight = string.Empty, averageStraighteningRight = string.Empty, straighteningRight = string.Empty,
                                zeroStraighteningLeft = string.Empty, averageStraighteningLeft = string.Empty, straighteningLeft = string.Empty,
                                level = string.Empty, averageLevel = string.Empty, zeroLevel = string.Empty;
                        
             


                        var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer.Number,
                            MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, "1") as List<Speed>;
                        var fragment = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer.Number,
                            MainTrackStructureConst.Fragments, tripProcess.DirectionName, "1") as Fragment;
                        var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer.Number,
                            MainTrackStructureConst.MtoPdbSection, tripProcess.DirectionName, "1") as List<PdbSection>;
                        XElement addParam = new XElement("addparam",
                            new XAttribute("top-title",

                                kilometer.Direction_name + " Путь: " + kilometer.Track_name + " Км:" + kilometer.Number + " " + (pdbSection.Count > 0 ? pdbSection[0].Pdb : " ПЧ-/ПЧУ-/ПД-/ПДБ-") + " Уст: " + " " + (speed.Count > 0 ? speed[0].ToString() : "-/-/-") + " Скор:58"),
                            new XAttribute("right-title",
                                copyright + ": " + "ПО " + softVersion + "  " +
                                systemName + ":" + tripProcess.Car + "(" + tripProcess.Chief + ") (БПД от " +
                                MainTrackStructureService.GetModificationDate() + ") <" + AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, false) + ">" +
                                "<" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ">" +
                                "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.Direction.ToString())) + ">" +
                                "<" + Helper.GetShortFormInNormalString(Helper.GetResourceName(tripProcess.CarPosition.ToString())) + ">" +
                                "<" + period.PeriodMonth + "-" + period.PeriodYear + " " + "контр. Проезд:" + tripProcess.Date_Vrem.ToShortDateString() + " " + tripProcess.Date_Vrem.ToShortTimeString() +
                                " " + diagramName + ">" + " Л: " + (kilometers.IndexOf(kilometer) + 1)
                                ),
                            new XAttribute("fragment", fragment.ToString() + "Км:" + kilometer.Number),
                            new XAttribute("viewbox", "0 0 770 1015"),
                            new XAttribute("minY", 0),
                            new XAttribute("maxY", 1000),
                            RightSideChart(tripProcess.Date_Vrem, kilometer.Number, tripProcess.Direction, kilometer.Trip.Direction_id,
                                kilometer.Track_name,
                                new float[] { 151f, 146f, 152.5f, 155f }),
                            new XElement("xgrid",
                                new XElement("x", MMToPixelChartString(LevelPosition - LevelStep ), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "  –30"), new XAttribute("y", MMToPixelChartString(LevelPosition - LevelStep-0.5f))),
                                new XElement("x", MMToPixelChartString(LevelPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(LevelPosition - 0.5f))),
                                new XElement("x", MMToPixelChartString(LevelPosition + LevelStep), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"),  new XAttribute("label", "    30"), new XAttribute("y", MMToPixelChartString(LevelPosition + LevelStep - 0.5f))),
                                new XElement("x", MMToPixelChartString(StraighRighttPosition - StrightStep), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "  –30"), new XAttribute("y", MMToPixelChartString(StraighRighttPosition - StrightStep - 0.5f))),
                                new XElement("x", MMToPixelChartString(StraighRighttPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(StraighRighttPosition - 1f))),
                                new XElement("x", MMToPixelChartString(StraighRighttPosition + StrightStep/10f), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "      3"), new XAttribute("y", MMToPixelChartString(StraighRighttPosition + StrightStep / 10f + 0.2f))),
                                new XElement("x", MMToPixelChartString(StrightLeftPosition - StrightStep /10f), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "    –3"), new XAttribute("y", MMToPixelChartString(StrightLeftPosition - StrightStep / 10f - 1f))),
                                new XElement("x", MMToPixelChartString(StrightLeftPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(StrightLeftPosition + 0.2f))),
                                new XElement("x", MMToPixelChartString(StrightLeftPosition + StrightStep), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "    30"), new XAttribute("y", MMToPixelChartString(StrightLeftPosition + StrightStep - 0.5f))),

                                new XElement("x", MMToPixelChartString(GaugePosition - 10 * GaugeKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey")),
                                new XElement("x", MMToPixelChartString(GaugePosition - 8 * GaugeKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "1512"), new XAttribute("y", MMToPixelChartString(GaugePosition - 8 * GaugeKoef - 0.5f))),
                                new XElement("x", MMToPixelChartString(GaugePosition - 4 * GaugeKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey")),
                                new XElement("x", MMToPixelChartString(GaugePosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "1520"), new XAttribute("y", MMToPixelChartString(GaugePosition - 0.5f))),
                                new XElement("x", MMToPixelChartString(GaugePosition + 8 * GaugeKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "1528"), new XAttribute("y", MMToPixelChartString(GaugePosition + 8 * GaugeKoef - 0.5f))),
                                new XElement("x", MMToPixelChartString(GaugePosition + 16 * GaugeKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "1536"), new XAttribute("y", MMToPixelChartString(GaugePosition + 16 * GaugeKoef - 0.5f))),
                                new XElement("x", MMToPixelChartString(GaugePosition + 22 * GaugeKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "1542"), new XAttribute("y", MMToPixelChartString(GaugePosition + 22 * GaugeKoef - 0.5f))),
                                new XElement("x", MMToPixelChartString(GaugePosition + 26 * GaugeKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey")),
                                new XElement("x", MMToPixelChartString(GaugePosition + 28 * GaugeKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "1548"), new XAttribute("y", MMToPixelChartString(GaugePosition + 28 * GaugeKoef - 0.5f))),

                                new XElement("x", MMToPixelChartString(ProsRightPosition - 10 * ProsKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "  –10"), new XAttribute("y", MMToPixelChartString(ProsRightPosition - 10 * ProsKoef - 0.5f))),
                                new XElement("x", MMToPixelChartString(ProsRightPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(ProsRightPosition - 0.5f))),
                                new XElement("x", MMToPixelChartString(ProsRightPosition + 10 * ProsKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "    10"), new XAttribute("y", MMToPixelChartString(ProsRightPosition + 10 * ProsKoef - 0.5f))),

                                new XElement("x", MMToPixelChartString(ProsLeftPosition - 10 * ProsKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "  –10"), new XAttribute("y", MMToPixelChartString(ProsLeftPosition - 10 * ProsKoef - 0.5f))),
                                new XElement("x", MMToPixelChartString(ProsLeftPosition), new XAttribute("dasharray", "3,3"), new XAttribute("stroke", "black"), new XAttribute("label", "      0"), new XAttribute("y", MMToPixelChartString(ProsLeftPosition - 0.5f))),
                                new XElement("x", MMToPixelChartString(ProsLeftPosition + 10 * ProsKoef), new XAttribute("dasharray", "0.5,3"), new XAttribute("stroke", "grey"), new XAttribute("label", "    10"), new XAttribute("y", MMToPixelChartString(ProsLeftPosition + 10 * ProsKoef - 0.5f)))

                                ));


                        for (int index = 0; index < prosRight.Count; index++)
                        {
                            int metre = tripProcess.Direction == Direction.Reverse ? 1000 - index : index;
                            drawdownRight+=MMToPixelChartString(prosRight[index] * ProsKoef +  ProsRightPosition) + "," + metre + " ";
                            drawdownLeft += MMToPixelChartString(prosLeft[index] * ProsKoef + ProsLeftPosition) + "," + metre + " ";
                            gauge += MMToPixelChartString((fsh[index] - fsh0[index]) * GaugeKoef + GaugePosition) + "," + metre + " ";
                            zeroGauge += MMToPixelChartString((fsh0[index] - fsh0[index]) * GaugeKoef +GaugePosition) + "," + metre + " ";

                            zeroStraighteningRight += MMToPixelChartString(frih20[index] * StrightKoef + StraighRighttPosition) + "," + metre + " ";
                            averageStraighteningRight += MMToPixelChartString(fsr_rh2[index] * StrightKoef + StraighRighttPosition) + "," + metre + " ";
                            straighteningRight += MMToPixelChartString(frih2[index] * StrightKoef + StraighRighttPosition) + "," + metre + " ";

                            zeroStraighteningLeft += MMToPixelChartString(frih10[index] * StrightKoef + StrightLeftPosition) + "," + metre + " ";
                            averageStraighteningLeft += MMToPixelChartString(fsr_rh1[index] * StrightKoef + StrightLeftPosition) + "," + metre + " ";
                            straighteningLeft += MMToPixelChartString(frih1[index] * StrightKoef + StrightLeftPosition) + "," + metre + " "; 

                            level += MMToPixelChartString(furb[index] * LevelKoef + LevelPosition) + "," + metre + " "; 
                            averageLevel += MMToPixelChartString(fsr_urb[index] * LevelKoef + LevelPosition) + "," + metre + " ";
                            zeroLevel +=MMToPixelChartString(furb0[index] * LevelKoef + LevelPosition) + "," + metre + " ";
                        }

                        addParam.Add(new XElement("polyline", new XAttribute("points", drawdownRight)));
                        addParam.Add(new XElement("polyline", new XAttribute("points", drawdownLeft)));
                        addParam.Add(new XElement("polyline", new XAttribute("points", gauge)));
                        addParam.Add(new XElement("polyline", new XAttribute("points", zeroGauge)));
                        addParam.Add(new XElement("polyline", new XAttribute("points", zeroStraighteningRight)));
                        addParam.Add(new XElement("polyline", new XAttribute("points", straighteningRight)));
                        addParam.Add(new XElement("polyline", new XAttribute("points", zeroStraighteningLeft)));
                        addParam.Add(new XElement("polyline", new XAttribute("points", averageStraighteningLeft)));
                        addParam.Add(new XElement("polyline", new XAttribute("points", straighteningLeft)));
                        addParam.Add(new XElement("polyline", new XAttribute("points", level)));
                        addParam.Add(new XElement("polyline", new XAttribute("points", averageLevel)));
                        addParam.Add(new XElement("polyline", new XAttribute("points", zeroLevel)));

                        char separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
                        List<TrackObject> strights = (from stright in xml.Root.Element("straightening").Elements("stright")
                            select new TrackObject()
                            {
                                Meter = double.Parse(((string)stright.Attribute("meter")).Replace(',', separator)) * 100,
                                Value = (int)stright.Attribute("type")
                            }).ToList();


               
                       


                        var digElemenets = new XElement("digressions");
                        List<int> usedTops = new List<int>();
                        foreach (var note in notes)
                        {
                            if (note.Note.Contains("Уст.ск:"))
                            {
                              continue;
                            }


                            var axisinv = xml.Root.Attribute("axisinv").Value.ToString();
                            var noteTypes = note.Note.Split(' ');

                            if (noteTypes.Length > 3)
                            {
                                string noteType = note.Note.Split(' ')[1];
                                int.Parse(noteTypes[0]);
                                bool isMarkNote = true;
                                float markPosition = 0;
                                switch (noteType)
                                {

                                    case "Пр.п":
                                        markPosition = ProsRightPosition;
                                        break;
                                    case "Пр.л":
                                        markPosition = ProsLeftPosition;
                                        break;
                                    case "Р":
                                        markPosition = StrightLeftPosition;
                                        break;
                                    case "У":
                                    case "П":
                                        markPosition = LevelPosition;
                                        break;
                                    case "Уш":
                                    case "Суж":
                                        markPosition = GaugePosition;
                                        break;
                                    default:
                                        isMarkNote = false;
                                        break;
                                }

                                if (isMarkNote)
                                {

                                    int strightStart = int.Parse(noteTypes[0]);
                                    float prevPosition = markPosition;
                                    int index;

                                    if (markPosition == StrightLeftPosition)
                                    {
                                       
                                        for (index = int.Parse(noteTypes[0]) ;
                                            index != (int.Parse(noteTypes[0]) + (int.Parse(noteTypes[4])/2) *(int)tripProcess.Direction);
                                            index=index + (int)tripProcess.Direction)
                                        {

                                            if ((markPosition == StrightLeftPosition) ||
                                                (markPosition == StraighRighttPosition))
                                            {
                                                List<TrackObject> localStrights = (from stright in strights
                                                    where Math.Abs(stright.Meter - index) < 4
                                                    select stright).ToList();
                                                prevPosition = markPosition;
                                                markPosition = localStrights.Count > 0 && localStrights[0].Value == 1
                                                    ? StraighRighttPosition
                                                    : StrightLeftPosition;
                                                if (((markPosition != prevPosition) && (index - strightStart) > 1) ||
                                                    (Math.Abs(index - (int.Parse(noteTypes[0]) + (int.Parse(noteTypes[4])/2) * (int)tripProcess.Direction))<2))
                                                {
                                                    digElemenets.Add(new XElement("line",
                                                        new XAttribute("y1", strightStart),
                                                        new XAttribute("y2", index),
                                                        new XAttribute("x", MMToPixelChartString(markPosition)),
                                                        new XAttribute("w",
                                                            MMToPixelChartWidthString(noteTypes[2].Equals("4") ? 4 :
                                                                noteTypes[2].Equals("3") ? 2 : 1))

                                                    ));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {

                                        digElemenets.Add(new XElement("line",
                                                new XAttribute("y1", strightStart + (-1)*(int)tripProcess.Direction * ((markPosition == ProsLeftPosition) || (markPosition == ProsRightPosition) ? int.Parse(noteTypes[4])*0.3 : 0)),
                                                new XAttribute("y2",
                                                    (int.Parse(noteTypes[0]) +  int.Parse(noteTypes[4]) * (int)tripProcess.Direction) ),
                                            new XAttribute("x", MMToPixelChartString(markPosition)),
                                            new XAttribute("w",
                                                MMToPixelChartWidthString(noteTypes[2].Equals("4") ? 4 :
                                                    noteTypes[2].Equals("3") ? 2 : 1))

                                    ));
                                    }
                                }
                            }
                            var top = note.Note.Split(' ')[0];
                            note.Note = note.Note.Replace(" ", "   ");

                            
                            int topint;
                            bool success = int.TryParse(top, out topint);
                            if (success)
                                note.Top = topint;
                            note.Top = note.Top - note.Top % 8;
                            int plus = note.Top;
                            int minus = note.Top;
                            while (usedTops.IndexOf(minus) != -1)
                            {
                                minus = minus - 8;
                            }

                            while (usedTops.IndexOf(plus) != -1)
                            {
                                plus = plus + 8;
                            }

                            note.Top = Math.Abs(note.Top - minus) < Math.Abs(note.Top - plus) ? minus : plus;

                            if (minus < 0)
                                note.Top = plus;
                            if (plus > 992)
                                note.Top = minus;

                            usedTops.Add(note.Top);
                                if (tripProcess.Direction == Direction.Direct)
                                    note.Top = 1000 - note.Top;
                                if (note.Top < 100)
                                    note.Note = "  " + note.Note;
                                if (note.Top < 10)
                                    note.Note = "  " + note.Note;
                                digElemenets.Add(new XElement("dig",

                                    new XAttribute("top", note.Top),
                                    new XAttribute("x", -20),
                                    new XAttribute("note", note.Note),
                                    new XAttribute("fw", note.FontStyle == 0 ? "normal" : "bold")
                                ));
                                if (note.Note.Contains("R"))
                                {
                                    digElemenets.Add(new XElement("rect",

                                        new XAttribute("top", note.Top-9),
                                        new XAttribute("x", -22)
                                    ));

                                }
                        }


                        addParam.Add(digElemenets);
                        report.Add(addParam);
                        //break;

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

        private string GetXByDigName(string digName)
        {
            float move = 6.6f;
            switch (digName)
            {
                case string name when name == DigressionName.LongWaveLeft.Name:
                    return MMToPixelChartString(LevelPosition + move);
                case string name when name == DigressionName.LongWaveRight.Name:
                    return MMToPixelChartString( + move);
                case string name when name == DigressionName.MiddleWaveLeft.Name:
                    return MMToPixelChartString( + move);
                case string name when name == DigressionName.MiddleWaveRight.Name:
                    return MMToPixelChartString( + move);
                case string name when name == DigressionName.ShortWaveLeft.Name:
                    return MMToPixelChartString( + move);
                case string name when name == DigressionName.ShortWaveRight.Name:
                    return MMToPixelChartString( + move);
            }

            return "-100";
        }
    }
}
