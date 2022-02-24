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
    public class SummarySheetForAssessingRailsJoints : Report
    {

        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetUnit(AdmStructureConst.AdmDirection , distance.Parent_Id) as AdmUnit;

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Name);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                var filterForm = new FilterForm();
                var filters = new List<Filter>();
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
                filters.Add(new FloatFilter() { Name = "Пороговое значение для Иб, (мм)", Value = 0 });
                filters.Add(new FloatFilter() { Name = "Пороговое значение для Ив, (мм)", Value = 0 });
                filters.Add(new FloatFilter() { Name = "Пороговое значение для З, (мм)", Value = 0 });

                filterForm.SetDataSource(filters);
                if (filterForm.ShowDialog() == DialogResult.Cancel)
                    return;

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    //var kilometers = AdditionalParametersService.GetKilometers(tripProcess.Id, (int)tripProcess.Direction);
                    var kms = AdditionalParametersService.GetKilometersByTripId(tripProcess.Id);
                    progressBar.Maximum = kms.Count;

                    XElement tripElem = new XElement("trip",
                        new XAttribute("dki", road.Code),
                        new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                        new XAttribute("trip_date", period.Period),
                        new XAttribute("pch", distance.Code),
                        new XAttribute("uch", kms.Min() + " - " + kms.Max()),
                        new XAttribute("napr", road.Name),
                        new XAttribute("nput", ""),
                        new XAttribute("km", kms.Min() +" - "+ kms.Max())
                        );
                    XElement data = new XElement("data",
                        new XAttribute("napr", road.Name)
                        );
                    XElement pch = new XElement("Pch",
                        new XAttribute("npch", distance.Code)
                        );
                    XElement put = new XElement("Put",
                        new XAttribute("nput", "")
                        );
                    XElement prop = new XElement("Prop");
                    
                    foreach (var kilometer in kms)
                    {

                        progressBar.Value = kms.IndexOf(kilometer)+1;

                        var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromText(kilometer);
                         
                        var speed = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, crossRailProfile.NKm,
                            MainTrackStructureConst.MtoSpeed, tripProcess.DirectionName, "1") as List<Speed>;
                        var fragment = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer,
                            MainTrackStructureConst.Fragments, tripProcess.DirectionName, "1") as Fragment;
                        var pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, kilometer,
                            MainTrackStructureConst.MtoPdbSection, tripProcess.DirectionName, "1") as List<PdbSection>;

                        var shortRoughness = AdditionalParametersService.GetShortRoughnessFromText(kilometer);
                        List<Digression> digressions = shortRoughness.GetDigressions();
                        digressions = digressions.OrderBy(o => o.Meter).ToList();

                        
                        foreach (var digression in digressions) 
                        {
                            XElement Note = new XElement("Note");

                            if (digression.Length < 1) continue;

                            float count = digression.Length / 100;
                            if ((digression.DigName == DigressionName.ImpulsLeft) ||
                                (digression.DigName == DigressionName.ImpulsRight))
                            {
                                count = count / 10;
                            }

                            Note.Add(
                                   new XAttribute("Km", kilometer),
                                   new XAttribute("m", digression.Meter < 10 ? "   " : (digression.Meter < 100 ? "  " : "") + digression.Meter),
                                   new XAttribute("Otst", digression.GetName()),
                                   new XAttribute("Velichina", ""),
                                   new XAttribute("Dlina", digression.Length),
                                   new XAttribute("PZnach", ""),
                                   new XAttribute("Stepen", ""),
                                   new XAttribute("Vust", speed.Count > 0 ? speed[0].Passenger.ToString()+"/"+ speed[0].Freight : "-"),
                                   new XAttribute("Vdop", "")
                                   );

                            put.Add(Note);
                        }
                    }
                    pch.Add(put);
                    data.Add(pch);
                    tripElem.Add(data);
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
