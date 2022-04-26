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
    public class excess_elevation : Report
    {
        public override void Process(Int64 parentId, ReportTemplate template, ReportPeriod current, MetroProgressBar progressBar)
        {
            var filterForm0 = new FilterForm();
            var filters0 = new List<Filter>();
            filters0.Add(new DoubleFilter() { Name = "Анп меньше (м/с2) ", Value = 0 });

            filterForm0.SetDataSource(filters0);
            if (filterForm0.ShowDialog() == DialogResult.Cancel)
                return;

            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report",
                     new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                     new XAttribute("date_statement", DateTime.Now.Date.ToShortDateString()),
                     new XAttribute("distance", "ПЧ: " + ((AdmUnit)AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId)).Code),
                     new XAttribute("source", "база данных"),
                     new XAttribute("file_travel", "база данных"));
                var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);

                var trips = RdStructureService.GetMainParametersProcesses(current, parentId, true);
                foreach (var trip in trips)
                {
                    List<Curve> curves = RdStructureService.GetCurvesInTrip(trip.Id) as List<Curve>;
                    if (!curves.Any()) continue;
                    var filterForm = new FilterForm();
                    var filters = new List<Filter>();

                    var min = curves.Select(o => o.Start_Km).Min();
                    var max = curves.Select(o => o.Final_Km).Max();

                    filters.Add(new FloatFilter() { Name = "Начало (км)", Value = min });
                    filters.Add(new FloatFilter() { Name = "Конец (км)", Value = max });

                    filterForm.SetDataSource(filters);
                    if (filterForm.ShowDialog() == DialogResult.Cancel)
                        return;
                    //фильтр по выбранным км
                    curves = curves.Where(o => ((float)(float)filters[0].Value <= o.Start_Km && o.Final_Km <= (float)(float)filters[1].Value)).ToList();

                    var curvesAdmUnits = AdmStructureService.GetCurvesAdmUnits(curves[0].Id) as List<CurvesAdmUnits>;
                    CurvesAdmUnits curvesAdmUnit = curvesAdmUnits.Count > 0 ? curvesAdmUnits[0] : null;
                    string site = "Неизвестный";
                    if (curvesAdmUnits.Any())
                    {
                        if (!curvesAdmUnit.StationStart.Equals("Неизвестный") && !curvesAdmUnit.StationFinal.Equals("Неизвестный"))
                        {
                            if (curvesAdmUnit.StationStart.Equals(curvesAdmUnit.StationFinal))
                                site = curvesAdmUnit.StationStart;
                            else
                                site = curvesAdmUnit.StationStart + "-" + curvesAdmUnit.StationFinal;
                        }
                        else if (curvesAdmUnit.StationStart.Equals("Неизвестный") && !curvesAdmUnit.StationFinal.Equals("Неизвестный"))
                        {
                            site = curvesAdmUnit.StationFinal;
                        }
                        else if (!curvesAdmUnit.StationStart.Equals("Неизвестный") && curvesAdmUnit.StationFinal.Equals("Неизвестный"))
                        {
                            site = curvesAdmUnit.StationStart;
                        }
                    }


                    XElement xeCurve = new XElement("curve",
                        new XAttribute("wear", "* - "),//to DO Пороговое значение износа
                        new XAttribute("road", roadName),
                        new XAttribute("chief", trip.Chief),
                        new XAttribute("type", trip.GetProcessTypeName),
                        new XAttribute("period", current.Period),
                        new XAttribute("date_trip", trip.Trip_date),
                        new XAttribute("site", site),
                        new XAttribute("track", curvesAdmUnit != null ? curvesAdmUnit.Track : "-"),
                        new XAttribute("direction", curvesAdmUnit != null ? curvesAdmUnit.Direction : "-"),
                        new XAttribute("km", curves.Min(c => c.Start_Km).ToString() + " - " + curves.Max(c => c.Final_Km).ToString()),
                        new XAttribute("order", curves.IndexOf(curves[0]) + 1),
                        new XAttribute("PC", trip.Car),
                        new XAttribute("side", curves[0].Side.ToLower()[0]));

                    foreach (var curve in curves)
                    {
                        List<RDCurve> rdcs = RdStructureService.GetRDCurves(curve.Id, trip.Id);

                        var LevelPoins = rdcs.Where(o => o.Point_level > 0).ToList();
                        var StrPoins = rdcs.Where(o => o.Point_str > 0).ToList();

                        int lvl = -1, str = -1, lenPerKrivlv = -1;

                        try
                        {
                            //Поиск круговой кивой рихтовки
                            var str_circular = new List<RDCurve> { };

                            for (int strIndex = 1; strIndex < StrPoins.Count - 1; strIndex++)
                            {
                                if (Math.Abs(StrPoins[strIndex].X - StrPoins[strIndex - 1].X) < 13)
                                    continue;

                                var firstDiffX = Math.Abs(Math.Abs(StrPoins[strIndex].Trapez_str) - Math.Abs(StrPoins[strIndex - 1].Trapez_str)) / Math.Abs(StrPoins[strIndex].X - StrPoins[strIndex - 1].X);
                                var secondDiffX = Math.Abs(Math.Abs(StrPoins[strIndex].Trapez_str) - Math.Abs(StrPoins[strIndex + 1].Trapez_str)) / Math.Abs(StrPoins[strIndex].X - StrPoins[strIndex + 1].X);

                                if (5.0 * firstDiffX < secondDiffX || 5.0 * secondDiffX < firstDiffX)
                                {
                                    str_circular.Add(StrPoins[strIndex]);
                                }
                            }

                            if (Math.Abs(Math.Abs(StrPoins[StrPoins.Count - 1].Trapez_str) - Math.Abs(StrPoins[StrPoins.Count - 2].Trapez_str)) / Math.Abs(StrPoins[StrPoins.Count - 1].X - StrPoins[StrPoins.Count - 2].X) < 0.05)
                            {
                                str_circular.Add(StrPoins[StrPoins.Count - 1]);
                            }
                            //Поиск круговой кивой уровень
                            var lvl_circular = new List<RDCurve> { };

                            for (int lvlIndex = 1; lvlIndex < LevelPoins.Count - 1; lvlIndex++)
                            {
                                if (Math.Abs(LevelPoins[lvlIndex].X - LevelPoins[lvlIndex - 1].X) < 6)
                                    continue;

                                var firstDiffX = Math.Abs(Math.Abs(LevelPoins[lvlIndex].Trapez_level) - Math.Abs(LevelPoins[lvlIndex - 1].Trapez_level)) / Math.Abs(LevelPoins[lvlIndex].X - LevelPoins[lvlIndex - 1].X);
                                var secondDiffX = Math.Abs(Math.Abs(LevelPoins[lvlIndex].Trapez_level) - Math.Abs(LevelPoins[lvlIndex + 1].Trapez_level)) / Math.Abs(LevelPoins[lvlIndex].X - LevelPoins[lvlIndex + 1].X);

                                if (5.0 * firstDiffX < secondDiffX || 5.0 * secondDiffX < firstDiffX)
                                {
                                    lvl_circular.Add(LevelPoins[lvlIndex]);
                                }
                            }
                            if (Math.Abs(Math.Abs(LevelPoins[LevelPoins.Count - 1].Trapez_level) - Math.Abs(LevelPoins[LevelPoins.Count - 2].Trapez_level)) / Math.Abs(LevelPoins[LevelPoins.Count - 1].X - LevelPoins[LevelPoins.Count - 2].X) < 0.05)
                            {
                                lvl_circular.Add(LevelPoins[LevelPoins.Count - 1]);
                            }



                            //нижние 2 точки трапеции
                            var start_km = StrPoins.First().Km;
                            var start_m = StrPoins.First().M;
                            var final_km = StrPoins.Last().Km;
                            var final_m = StrPoins.Last().M;

                            var start_lvl_km = LevelPoins.First().Km;
                            var start_lvl_m = LevelPoins.First().M;
                            var final_lvl_km = LevelPoins.Last().Km;
                            var final_lvl_m = LevelPoins.Last().M;


                            //верхние 2 точки трапеции
                            var start_kmc = str_circular.First().Km;
                            var start_mc = str_circular.First().M;
                            var final_kmc = str_circular.Last().Km;
                            var final_mc = str_circular.Last().M;

                            for (int cirrInd = 0; cirrInd < str_circular.Count - 1; cirrInd++)
                            {
                                if (Math.Abs(str_circular[cirrInd].Trapez_str - str_circular[cirrInd + 1].Trapez_str) /
                                    Math.Abs(str_circular[cirrInd].X - str_circular[cirrInd + 1].X) > 0.0065

                                    && (Math.Abs(str_circular[cirrInd].Trapez_str) < 5 || Math.Abs(str_circular[cirrInd + 1].Trapez_str) < 5))
                                {
                                    continue;
                                }
                                else
                                {
                                    //начальный до 1 переходной
                                    var circ1Ind = StrPoins.IndexOf(str_circular[cirrInd]);
                                    for (int iii = circ1Ind - 1; iii >= 0; iii--)
                                    {
                                        if (Math.Abs(StrPoins[iii].Trapez_str) < 3)
                                        {
                                            start_km = StrPoins[iii].Km;
                                            start_m = StrPoins[iii].M;
                                            //var final_km = StrPoins.Last().Km;
                                            //var final_m = StrPoins.Last().M;
                                            break;
                                        }
                                    }
                                    //конечный от 2 переходной
                                    var circ2Ind = StrPoins.IndexOf(str_circular[cirrInd + 1]);
                                    for (int iii = circ2Ind + 1; iii < StrPoins.Count; iii++)
                                    {
                                        if (Math.Abs(StrPoins[iii].Trapez_str) < 3)
                                        {
                                            //start_km = StrPoins[iii].Km;
                                            //start_m = StrPoins[iii].M;
                                            final_km = StrPoins[iii].Km;
                                            final_m = StrPoins[iii].M;
                                            break;
                                        }
                                    }

                                    // круговая верхние точки
                                    start_kmc = str_circular[cirrInd].Km;
                                    start_mc = str_circular[cirrInd].M;

                                    final_kmc = str_circular[cirrInd + 1].Km;
                                    final_mc = str_circular[cirrInd + 1].M;

                                    break;
                                }
                            }

                            var start_lvl_kmc = lvl_circular.First().Km;
                            var start_lvl_mc = lvl_circular.First().M;
                            var final_lvl_kmc = lvl_circular.Last().Km;
                            var final_lvl_mc = lvl_circular.Last().M;

                            for (int cirrInd = 0; cirrInd < lvl_circular.Count - 1; cirrInd++)
                            {
                                if (Math.Abs(lvl_circular[cirrInd].Trapez_level - lvl_circular[cirrInd + 1].Trapez_level) /
                                    Math.Abs(lvl_circular[cirrInd].X - lvl_circular[cirrInd + 1].X) > 0.0065

                                    && (Math.Abs(lvl_circular[cirrInd].Trapez_level) < 5 || Math.Abs(lvl_circular[cirrInd + 1].Trapez_level) < 5))
                                {
                                    continue;
                                }
                                else
                                {
                                    //начальный до 1 переходной
                                    var circ1Ind = LevelPoins.IndexOf(lvl_circular[cirrInd]);
                                    for (int iii = circ1Ind - 1; iii >= 0; iii--)
                                    {
                                        if (Math.Abs(LevelPoins[iii].Trapez_level) < 3)
                                        {
                                            start_lvl_km = LevelPoins[iii].Km;
                                            start_lvl_m = LevelPoins[iii].M;
                                            //var final_km = StrPoins.Last().Km;
                                            //var final_m = StrPoins.Last().M;
                                            break;
                                        }
                                    }
                                    //конечный от 2 переходной
                                    var circ2Ind = LevelPoins.IndexOf(lvl_circular[cirrInd + 1]);
                                    for (int iii = circ2Ind + 1; iii < LevelPoins.Count; iii++)
                                    {
                                        if (Math.Abs(LevelPoins[iii].Trapez_level) < 3)
                                        {
                                            //start_km = StrPoins[iii].Km;
                                            //start_m = StrPoins[iii].M;
                                            final_lvl_km = LevelPoins[iii].Km;
                                            final_lvl_m = LevelPoins[iii].M;
                                            break;
                                        }
                                    }
                                    //круговая верхние точки
                                    start_lvl_kmc = lvl_circular[cirrInd].Km;
                                    start_lvl_mc = lvl_circular[cirrInd].M;

                                    final_lvl_kmc = lvl_circular[cirrInd + 1].Km;
                                    final_lvl_mc = lvl_circular[cirrInd + 1].M;

                                    break;
                                }
                            }

                            var lenPerKriv10000 = ((start_kmc + start_mc / 10000.0) - (final_kmc + final_mc / 10000.0)) * 10000;
                            var lenPerKriv = Math.Abs((int)lenPerKriv10000 % 1000);

                            var lenKriv10000 = ((start_km + start_m / 10000.0) - (final_km + final_m / 10000.0)) * 10000;
                            var lenKriv = Math.Abs((int)lenKriv10000 % 1000);

                            var lenPerKriv10000lv = ((start_lvl_kmc + start_lvl_mc / 10000.0) - (final_lvl_kmc + final_lvl_mc / 10000.0)) * 10000;
                            lenPerKrivlv = Math.Abs((int)lenPerKriv10000lv % 1000);

                            var lenKriv10000lv = ((start_lvl_km + start_lvl_m / 10000.0) - (final_lvl_km + final_lvl_m / 10000.0)) * 10000;
                            var lenKrivlv = Math.Abs((int)lenKriv10000lv % 1000);

                            var d = false;
                            if ((start_km + start_m / 10000.0) > (final_km + final_m / 10000.0))
                                d = true;


                            var razn1 = (int)(((start_km + start_m / 10000.0) - (start_lvl_km + start_lvl_m / 10000.0)) * 10000) % 1000; // start
                            var razn2 = (int)(((final_km + final_m / 10000.0) - (final_lvl_km + final_lvl_m / 10000.0)) * 10000) % 1000; // final
                            var razn3 = lenKriv - lenKrivlv; // общая длина нижних


                            var razn1c = (int)(((start_kmc + start_mc / 10000.0) - (start_lvl_kmc + start_lvl_mc / 10000.0)) * 10000) % 1000; // start
                            var razn2c = (int)(((final_kmc + final_mc / 10000.0) - (final_lvl_kmc + final_lvl_mc / 10000.0)) * 10000) % 1000; // final

                            //Переходные 
                            //1-й
                            var tap_len1 = Math.Round(((start_km + start_m / 10000.0) - (start_kmc + start_mc / 10000.0)) * 10000) % 1000;
                            var tap_len1_lvl = Math.Round(((start_lvl_km + start_lvl_m / 10000.0) - (start_lvl_kmc + start_lvl_mc / 10000.0)) * 10000) % 1000;
                            //2-й
                            var tap_len2 = Math.Round(((final_km + final_m / 10000.0) - (final_kmc + final_mc / 10000.0)) * 10000) % 1000;
                            var tap_len2_lvl = Math.Round(((final_lvl_km + final_lvl_m / 10000.0) - (final_lvl_kmc + final_lvl_mc / 10000.0)) * 10000) % 1000;

                            //Радиус/Уровень (для мин макс сред)
                            var temp_data = rdcs.GetRange((int)Math.Abs(tap_len1_lvl) + 40, Math.Abs(lenPerKrivlv));
                            var temp_data_str = rdcs.Where(o => (start_kmc + start_mc / 10000.0) <= (o.Km + o.M / 10000.0) && (o.Km + o.M / 10000.0) <= (final_kmc + final_mc / 10000.0)).ToList();
                            var temp_data_lvl = rdcs.Where(o => (start_lvl_kmc + start_lvl_mc / 10000.0) <= (o.Km + o.M / 10000.0) && (o.Km + o.M / 10000.0) <= (final_lvl_kmc + final_lvl_mc / 10000.0)).ToList();

                            //Переходные (для макс сред)
                            var transitional_lvl_data = rdcs.GetRange(40, Math.Abs((int)tap_len1_lvl));
                            var transitional_str_data = rdcs.GetRange(40, Math.Abs((int)tap_len1));

                            var transitional_lvl_data2 = rdcs.GetRange((int)Math.Abs(tap_len1_lvl) + 40 + Math.Abs(lenPerKrivlv), Math.Abs((int)tap_len2_lvl));
                            var transitional_str_data2 = rdcs.GetRange((int)Math.Abs(tap_len1) + 40 + Math.Abs(lenPerKriv), Math.Abs((int)tap_len2));

                            Data rdcsData = new Data();

                            //план/ср 1 пер
                            var rad_mid = rdcsData.GetAvgPlan(temp_data_str);
                            var temp1 = (8865.0 / rad_mid) * 4;
                            var perAvg1 = temp1 / Math.Abs(tap_len1);
                            //план/макс 1пер
                            var rad_max = rdcsData.GetMaxPlan(temp_data_str);
                            var temp = (8865.0 / rad_max) * 4;
                            var perMax = temp / Math.Abs(tap_len1);

                            var rad_min = rdcsData.GetMinPlan(temp_data_str);

                            //уровень/ср 1 пер
                            var lvl_mid = rdcsData.GetAvgLevel(temp_data_lvl);
                            var perAvglvl = lvl_mid / Math.Abs(tap_len1_lvl);
                            //уровень/макс 1 пер
                            var lvl_max = rdcsData.GetMaxLevel(temp_data_lvl);
                            var perMaxlvl = lvl_max / Math.Abs(tap_len1_lvl);

                            var lvl_min = rdcsData.GetMinLevel(temp_data_lvl);

                            lvl = lvl_mid;
                            str = rad_mid;

                            List<int> x = new List<int>();
                            List<int> passSpeed = new List<int>();
                            List<int> freightSpeed = new List<int>();
                            List<float> plan = new List<float>();
                            List<float> level = new List<float>();
                            List<float> gauge = new List<float>();
                            List<float> passBoost = new List<float>();
                            List<float> freightBoost = new List<float>();

                            foreach (var rdc in rdcs)
                            {
                                x.Add(rdcs.IndexOf(rdc));
                                plan.Add(rdc.Radius);
                                level.Add(rdc.Level);
                                gauge.Add(rdc.Gauge);
                                passBoost.Add(rdc.PassBoost);
                                freightBoost.Add(rdc.FreightBoost);
                                passSpeed.Add(rdc.PassSpeed);
                                freightSpeed.Add(rdc.FreightSpeed);
                            }

                            var AnpPassMax = (Math.Pow(passSpeed.First(), 2) / (13.0 * rad_mid)) - (0.0061 * lvl_mid);

                            var AnpFreigMax = (Math.Pow(freightSpeed.First(), 2) / (13.0 * rad_mid)) - (0.0061 * lvl_mid);


                            XElement paramCurve = new XElement("param_curve",
                            new XAttribute("curve_num", curve.Num),
                            //план
                            new XAttribute("start_km", start_km),
                            new XAttribute("start_m", start_m),
                            new XAttribute("final_km", final_km),
                            new XAttribute("final_m", final_m),

                            new XAttribute("vozv", lvl_mid),
                            new XAttribute("radius", rad_mid),
                            new XAttribute("Vpz_pass", passSpeed.First()),
                            new XAttribute("Vpz_gruz", freightSpeed.First()),
                            new XAttribute("Anp_pass", AnpPassMax.ToString("0.00").Replace(",", ".")),
                            new XAttribute("Anp_gruz", AnpFreigMax.ToString("0.00").Replace(",", ".")));

                            if (AnpPassMax <= (double)filters0[0].Value)
                            {
                                xeCurve.Add(paramCurve);
                            }

                        }
                        catch
                        {
                            Console.WriteLine("Ошибка при расчете натурной кривой");
                        }                        
                    }
                    report.Add(xeCurve);
                }
                xdReport.Add(report);
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report.html");
                htReport.Save($@"G:\form\2.Характеристики положения пути в плане и профиле\17.Ведомость кривых с избыточным возвышением .html");
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