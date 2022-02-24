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
    public class ControlAdjustmentProtocol : Report
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

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                var tripProcesses = RdStructureService.GetProcess(period, parentId, ProcessType.VideoProcess);
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



                        XElement tripElem = new XElement("trip",
                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                            new XAttribute("check", trip.GetProcessTypeName), //ToDo
                            new XAttribute("road", roadName),
                            new XAttribute("track", trackName),
                            new XAttribute("code", tripProcess.DirectionCode),
                            new XAttribute("direction", tripProcess.DirectionName),
                            new XAttribute("distance", distance.Code),
                            new XAttribute("periodDate", period.Period),
                            new XAttribute("chief", trip.Chief),
                            new XAttribute("ps", trip.Car)
                        );

                        var ControlAdjustmentProtocol = RdStructureService.ControlAdjustmentProtocol(trip.Id);
                        var S3ByTripId = RdStructureService.GetS3ByTripId(trip.Id);
                        var note = new XElement("NOTES");

                        foreach (var elem in ControlAdjustmentProtocol)
                        {
                            var st = S3ByTripId.Where(o => o.original_id == elem.original_id).ToList();
                            var stdeleted = S3ByTripId.Where(o => o.original_id != elem.original_id).ToList();
                            if (st.Count() > 0)
                            {
                                note.Add(new XElement("NOTE",

                                    new XAttribute("Km", elem.Km),
                                    new XAttribute("Mtr", elem.Meter),
                                    new XAttribute("vidcorect", "Изменение оценки"),
                                    //new XAttribute("CorrectType", elem.state_id),
                                    new XAttribute("Comment", elem.comment),
                                    new XAttribute("Otst", elem.NAME),

                                    new XAttribute("old_Stepen", st[0].Typ),
                                    new XAttribute("old_value", st[0].VALUE),
                                    new XAttribute("old_length", st[0].LENGTH),
                                    new XAttribute("old_count", st[0].COUNT),
                                    new XAttribute("old_strelka", ""),
                                    new XAttribute("old_most", ""),

                                    new XAttribute("Stepen", elem.Typ),
                                    new XAttribute("value", elem.VALUE),
                                    new XAttribute("length", elem.LENGTH),
                                    new XAttribute("count", elem.COUNT),
                                    new XAttribute("strelka", ""),
                                    new XAttribute("most", "")
                                    ));
                            }

                            if (stdeleted.Count() > 0)
                            {
                                note.Add(new XElement("NOTE",

                                       new XAttribute("Km", elem.Km),
                                    new XAttribute("Mtr", elem.Meter),

                                    new XAttribute("vidcorect", "Удаление"),

                                    //new XAttribute("CorrectType", elem.state_id),
                                    new XAttribute("Comment", elem.comment),
                                    new XAttribute("Otst", elem.NAME),

                                    new XAttribute("old_Stepen", "-"),
                                    new XAttribute("old_value", "-"),
                                    new XAttribute("old_length", "-"),
                                    new XAttribute("old_count", "-"),
                                    new XAttribute("old_strelka", "-"),
                                    new XAttribute("old_most", "-"),

                                    new XAttribute("Stepen", elem.Typ),
                                    new XAttribute("value", elem.VALUE),
                                    new XAttribute("length", elem.LENGTH),
                                    new XAttribute("count", elem.COUNT),
                                    new XAttribute("strelka", ""),
                                    new XAttribute("most", "")
                                    ));
                            }

                        }
                        tripElem.Add(note);
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