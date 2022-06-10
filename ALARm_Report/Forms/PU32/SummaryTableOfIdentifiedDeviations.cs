using ALARm.Core;
using ALARm.Core.Report;
using ALARm.DataAccess;
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
    public class SummaryTableOfIdentifiedDeviations : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
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
                    foreach (var trackId in admTracksId)
                    {

                        var trackName = AdmStructureService.GetTrackName(trackId);
                        var trip = RdStructureService.GetTrip(tripProcess.Id);
                        var kms = RdStructureService.GetKilometersByTrip(trip);
                        kms = kms.Where(o => o.Track_id == trackId).ToList();
                        if (kms.Count() == 0) continue;




                        var lkm = kms.Select(o => o.Number).ToList();

                        if (lkm.Count() == 0) continue;

                        if (kms == null || kms.Count == 0)
                            continue;

                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;

                        //kms = kms.Where(o => ((float)filters[0].Value <= o.Number && o.Number <= (float)filters[1].Value)).Distinct().ToList();

                        //kms = kms.OrderBy(o => o.Number).ToList();

                        List<Curve> curves = (MainTrackStructureService.GetCurves(parentId, MainTrackStructureConst.MtoCurve) as List<Curve>).Where(c => c.Radius <= 1200).OrderBy(c => c.Start_Km * 1000 + c.Start_M).ToList();

                        XElement tripElem = new XElement("trip",
                        new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                        new XAttribute("ps", tripProcess.Car),
                        new XAttribute("uch", roadName),
                        new XAttribute("car", tripProcess.Car),
                        new XAttribute("type", tripProcess.GetProcessTypeName),
                        new XAttribute("pch", distance.Code),
                        new XAttribute("check", "" + tripProcess.GetProcessTypeName),
                        new XAttribute("PeriodMonth", period.PeriodMonthMonth),
                        new XAttribute("PeriodYear", period.PeriodYear),
                        new XAttribute("trip_date", period.Period));

                        progressBar.Maximum = kms.Count;

                        var check_gap_state = AdditionalParametersService.Check_gap_state(tripProcess.Id, template.ID); //стыки
                        var Pu32_gap = check_gap_state.Where(o => o.Otst.Contains("З") || o.Otst.Contains("З?")).ToList();

                        foreach (var km in kms)
                        {
                            km.LoadTrackPasport(MainTrackStructureService.GetRepository(), tripProcess.Date_Vrem);
                            var digressions = RdStructureService.GetDigressionMarks(tripProcess.Id, km.Track_id, km.Number);

                            //var ListS3 = RdStructureService.GetS3ForKm(tripProcess.Id, km.Number) as List<S3>; 
                            var ListS3 = RdStructureService.GetS3all(tripProcess.Trip_id, distance.Code) as List<S3>;
                            ListS3 = ListS3.Where(o => o.Km == km.Number).Distinct().OrderBy(o => o.Meter).ToList();

                            if (ListS3 == null || ListS3.Count == 0) continue;

                            progressBar.Value = kms.IndexOf(km) + 1;

                            //curve.Elevations = (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();
                            ////Выбор километров по проезду-----------------
                            /*var filterForm = new FilterForm();
                            var filters = new List<Filter>();

                            filters.Add(new FloatFilter() { Name = "Начало (км)", Value = ListS3.Select(o => o.Km).Min() });
                            filters.Add(new FloatFilter() { Name = "Конец (км)", Value = ListS3.Select(o => o.Km).Max() });

                            filterForm.SetDataSource(filters);
                            if (filterForm.ShowDialog() == DialogResult.Cancel)
                                return;

                            ListS3 = ListS3.Where(o => ((float)filters[0].Value <= o.Km && o.Km <= (float)filters[1].Value)).ToList();
                           */
                            //var GetBedomost = (RdStructureService.GetBedemost(tripProcess.Trip_id) as List<Bedemost>).Where(o => ((float)filters[0].Value <= o.Km && o.Km <= (float)filters[1].Value)).ToList();
                            //foreach (var item in GetBedomost)
                            //{
                            //    ListS3.Add(
                            //        new S3 { 
                            //            Km=item.Km,
                            //            Meter=item.Metr
                            //        }
                            //        );
                            //}


                            //--------------------------------------------
                            //стыки на километр
                            var kmGap = Pu32_gap.Where(o => o.Km == km.Number).ToList();
                            foreach (var item in kmGap)
                            {
                                var dig = new S3 { };

                                dig.Km = item.Km;
                                dig.Meter = item.Meter;
                                dig.Ots = item.Otst;
                                dig.Otkl = Math.Max(item.Zazor, item.R_zazor);
                                dig.Primech = item.Temp;

                                if (item.Otst == "З?")
                                {
                                    dig.Ovp = -1;
                                    dig.Ogp = -1;
                                }
                                else
                                {
                                    dig.Ovp = int.Parse(item.Vdop.Split('/')[1]);
                                    dig.Ogp = int.Parse(item.Vdop.Split('/')[1]);
                                }

                                dig.Uv = int.Parse(item.Vpz.Split('/')[0]);
                                dig.Uvg = int.Parse(item.Vpz.Split('/')[1]);

                                ListS3.Add(dig);
                            }

                            ListS3 = ListS3.OrderBy(o => o.RealCoordinate).ToList();

                            var temp = ListS3.Where(o => o.Ots != "З" && o.Ots != "З?").ToList();

                            foreach (var s3 in ListS3)
                            {
                                var radius = "-";
                                var vozvihenie = "-";
                                var norm = "-";
                                var ball = "-";

                                var curv = curves.Where(
                                    o => (o.Start_Km * 1000 + o.Start_M) <= (s3.Km * 1000 + s3.Meter) && (o.Final_Km * 1000 + o.Final_M) >= (s3.Km * 1000 + s3.Meter)).ToList();
                                if (curv.Count > 0)
                                {
                                    curv.First().Elevations = (MainTrackStructureService.GetCurves(curv.First().Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(el => el.RealStartCoordinate).ToList();
                                    curv.First().Straightenings = (MainTrackStructureService.GetCurves(curv.First().Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(st => st.RealStartCoordinate).ToList();

                                    radius = curv.First().Straightenings.First().Radius.ToString();
                                    vozvihenie = curv.First().Elevations.First().Lvl.ToString();
                                    norm = curv.First().Straightenings.First().Width.ToString();
                                }

                                if (digressions != null && digressions.Count > 0)
                                {
                                    var digm = digressions.Where(o => o.Km == s3.Km && o.Meter == s3.Meter && o.DigName == s3.Ots).FirstOrDefault();

                                    if (digm != null)
                                    {
                                        ball = digm.GetPoint(km).ToString();
                                        norm = digm.Norma.ToString();
                                        var rad = digm.Radius.ToString();
                                        radius = radius == "-" && rad != "10000" ? rad : radius;
                                    }
                                }

                                int count = 1;
                                switch (s3.Ots)
                                {
                                    case "Пр.п":
                                    case "Пр.л":
                                        count = 1;
                                        break;
                                    case "Суж":
                                        count = s3.Len / 4;
                                        count += s3.Len % 4 > 0 ? 1 : 0;
                                        break;
                                    case "Уш":
                                        count = s3.Len / 4;
                                        count += s3.Len % 4 > 0 ? 1 : 0;
                                        break;
                                    case "У":
                                        count = s3.Len / 10;
                                        count += s3.Len % 10 > 0 ? 1 : 0;
                                        break;
                                    case "П":
                                        count = 1;
                                        break;
                                    case "Р":
                                        count = 1;
                                        break;
                                }



                                if (s3.Ots == "З" || s3.Ots == "З?")
                                {
                                    XElement xeNote = new XElement("Note",
                                        new XAttribute("codDorogi", temp.Any() ? temp.First().Roadcode : "-"),
                                        new XAttribute("codNapr", temp.Any() ? temp.First().Directcode : "-"),
                                        new XAttribute("classput", "1"),

                                        new XAttribute("pch", distance.Code),
                                        new XAttribute("checkDate", temp.Any() ? temp.First().Date.ToString("dd/MM/yyyy") : "-"),
                                        new XAttribute("nomerPS", temp.Any() ? temp.First().Pscode : "-"),
                                        new XAttribute("nomerPuti", temp.Any() ? temp.First().Nput.ToString() : "-"),
                                        new XAttribute("km", s3.Km),
                                        new XAttribute("m", s3.Meter),
                                        new XAttribute("vidOts", s3.Ots),
                                        new XAttribute("norma", "-"),
                                        new XAttribute("velichOts", s3.Otkl),
                                        new XAttribute("len", "-"),
                                        new XAttribute("stepen", "-"),
                                        new XAttribute("ball", s3.Ots == "З" ? 50 : 20),
                                        new XAttribute("count", "-"),
                                        new XAttribute("Vust",/* s3.Us.ToString().Max().ToString() == "-1" ? "-" : s3.Us.ToString()*/"-"),//to doo
                                        new XAttribute("vPass", s3.Uv.ToString() == "-1" ? "-" : s3.Uv.ToString()),
                                        new XAttribute("vGruz", s3.Uvg.ToString() == "-1" ? "-" : s3.Uvg.ToString()),
                                        new XAttribute("Vogr", /*s3.Ogp.ToString().Max().ToString() == "-1" ? "-" : s3.Ogp.ToString()*/"-"),//to doo
                                        new XAttribute("vOgrPass", s3.Ovp.ToString() == "-1" ? "-" : s3.Ovp.ToString() == "0" ? "-" : s3.Ovp.ToString()),
                                        new XAttribute("vOgrGruz", s3.Ogp.ToString() == "-1" ? "-" : s3.Ogp.ToString() == "0" ? "-" : s3.Ogp.ToString()),
                                        new XAttribute("vOgrPorozh", "-"),
                                        new XAttribute("radius", "-"),
                                        new XAttribute("elevation", "-"),
                                        new XAttribute("strelka", "-"),
                                        new XAttribute("primech", s3.Primech)
                                        );

                                    tripElem.Add(xeNote);
                                }
                                else if (s3.Ots == "Анп" || s3.Ots == "?Анп")
                                {
                                    var otkl = "";
                                    if (s3.Primech.Any())
                                    {
                                        try
                                        {
                                            otkl = s3.Primech.Split().First().Split(':').Last();
                                        }
                                        catch
                                        {
                                            otkl = "ошибка при разделении";
                                        }
                                    }

                                    XElement xeNote = new XElement("Note",
                                            new XAttribute("codDorogi", s3.Roadcode),
                                            new XAttribute("codNapr", s3.Directcode == null ? "" : s3.Directcode),
                                            new XAttribute("classput", "1"),

                                            new XAttribute("pch", distance.Code),
                                            new XAttribute("checkDate", s3.Date.ToString("dd/MM/yyyy")),
                                            new XAttribute("nomerPS", s3.Pscode == null ? "" : s3.Pscode),
                                            new XAttribute("nomerPuti", s3.Nput),
                                            new XAttribute("km", s3.Km),
                                            new XAttribute("m", s3.Meter),
                                            new XAttribute("vidOts", s3.Ots),
                                            new XAttribute("norma", norm),
                                            new XAttribute("velichOts", otkl),
                                            new XAttribute("len", s3.Len),
                                            new XAttribute("stepen", "-"),
                                            new XAttribute("ball", s3.Ots == "Анп" ? 50 : 0),
                                            new XAttribute("count", count),
                                            new XAttribute("Vust",/* s3.Us.ToString().Max().ToString() == "-1" ? "-" : s3.Us.ToString()*/"-"),//to doo
                                            new XAttribute("vPass", s3.Uv.ToString() == "-1" ? "-" : s3.Uv.ToString()),
                                            new XAttribute("vGruz", s3.Uvg.ToString() == "-1" ? "-" : s3.Uvg.ToString()),
                                            new XAttribute("Vogr", /*s3.Ogp.ToString().Max().ToString() == "-1" ? "-" : s3.Ogp.ToString()*/"-"),//to doo
                                            new XAttribute("vOgrPass", s3.Ovp.ToString() == "-1" ? "-" : s3.Ovp.ToString() == "0" ? "-" : s3.Ovp.ToString()),
                                            new XAttribute("vOgrGruz", s3.Ogp.ToString() == "-1" ? "-" : s3.Ogp.ToString() == "0" ? "-" : s3.Ogp.ToString()),
                                            new XAttribute("vOgrPorozh", "-"),
                                            new XAttribute("radius", radius),
                                            new XAttribute("elevation", vozvihenie),
                                            new XAttribute("strelka", s3.Primech.Contains("Стр.;") ? 1 : s3.Strelka),
                                            new XAttribute("primech", s3.Primech.Contains("м;") ? "м" : "-")
                                            );

                                    tripElem.Add(xeNote);
                                }
                                else
                                {
                                    XElement xeNote = new XElement("Note",
                                            new XAttribute("codDorogi", s3.Roadcode),
                                            new XAttribute("codNapr", s3.Directcode == null ? "" : s3.Directcode),
                                            new XAttribute("classput", "1"),

                                            new XAttribute("pch", distance.Code),
                                            new XAttribute("checkDate", s3.Date.ToString("dd/MM/yyyy")),
                                            new XAttribute("nomerPS", s3.Pscode == null ? "" : s3.Pscode),
                                            new XAttribute("nomerPuti", s3.Nput),
                                            new XAttribute("km", s3.Km),
                                            new XAttribute("m", s3.Meter),
                                            new XAttribute("vidOts", s3.Ots),
                                            new XAttribute("norma", norm),
                                            new XAttribute("velichOts", s3.Otkl),
                                            new XAttribute("len", s3.Len),
                                            new XAttribute("stepen", s3.Typ.ToString() == "5" ? "-" : s3.Typ.ToString()),
                                            new XAttribute("ball", ball),
                                            new XAttribute("count", count),
                                            new XAttribute("Vust",/* s3.Us.ToString().Max().ToString() == "-1" ? "-" : s3.Us.ToString()*/"-"),//to doo
                                            new XAttribute("vPass", s3.Uv.ToString() == "-1" ? "-" : s3.Uv.ToString()),
                                            new XAttribute("vGruz", s3.Uvg.ToString() == "-1" ? "-" : s3.Uvg.ToString()),
                                            new XAttribute("Vogr", /*s3.Ogp.ToString().Max().ToString() == "-1" ? "-" : s3.Ogp.ToString()*/"-"),//to doo
                                            new XAttribute("vOgrPass", s3.Ovp.ToString() == "-1" ? "-" : s3.Ovp.ToString() == "0" ? "-" : s3.Ovp.ToString()),
                                            new XAttribute("vOgrGruz", s3.Ogp.ToString() == "-1" ? "-" : s3.Ogp.ToString() == "0" ? "-" : s3.Ogp.ToString()),
                                            new XAttribute("vOgrPorozh", "-"),
                                            new XAttribute("radius", radius),
                                            new XAttribute("elevation", vozvihenie),
                                            new XAttribute("strelka", s3.Primech.Contains("Стр.;") ? 1 : s3.Strelka),
                                            new XAttribute("primech", s3.Primech.Contains("м;") ? "м" : "-")
                                            );

                                    tripElem.Add(xeNote);
                                }
                            }

                        }
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
                //htReport.Save($@"G:\form\6.Выходные формы Основные параметры\30.Сводная таблица выявленных отступлений (ФП-1.4).html");
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
