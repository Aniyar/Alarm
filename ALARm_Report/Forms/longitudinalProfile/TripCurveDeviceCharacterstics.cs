using ALARm.Core;
using ALARm.Core.Report;
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
    public class TripCurveDeviceCharacterstics : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                XElement xePages = new XElement("pages",
                    new XAttribute("DKI", "..."),
                    new XAttribute("reportdate", "18.07.2012"),
                    new XAttribute("date", "29.05.2012"),
                    new XAttribute("distance", "53;6"),
                    new XAttribute("site", "БББ - ЗЗЗ"),
                    new XAttribute("direction", "ХХХХХ"),
                    new XAttribute("track", "2"),
                    new XAttribute("km", "381 - 502"));

                XElement xeElements = new XElement("elements",
                    new XAttribute("side", "л"),
                    new XAttribute("startkm", "423"),
                    new XAttribute("startm", "26"),
                    new XAttribute("finalkm", "423"),
                    new XAttribute("finalm", "749"),
                    new XAttribute("angle", "32.0"),
                    new XAttribute("circle_len", "436"),
                    new XAttribute("circle_min", "987"),
                    new XAttribute("circle_max", "1152"),
                    new XAttribute("circle_mid", "1039"),
                    new XAttribute("tap1_len", "184"),
                    new XAttribute("tap1_max", "0.35"),
                    new XAttribute("tap1_mid", "0.22"),
                    new XAttribute("tap2_len", "104"),
                    new XAttribute("tap2_max", "0.50"),
                    new XAttribute("tap2_mid", "0.39"),
                    new XAttribute("pint", "0.88"),
                    new XAttribute("anp", "0.60\\0.67"),
                    new XAttribute("v1", "120"),
                    new XAttribute("v2", "125"),
                    new XAttribute("v3", "160"),
                    new XAttribute("startlvl", "-14"),
                    new XAttribute("finallvl", "12"),
                    new XAttribute("circle_len_lvl", "463"),
                    new XAttribute("circle_min_lvl", "70"),
                    new XAttribute("circle_max_lvl", "82"),
                    new XAttribute("circle_mid_lvl", "76"),
                    new XAttribute("tap1_len_lvl", "131"),
                    new XAttribute("tap1_max_lvl", "0.63"),
                    new XAttribute("tap1_mid_lvl", "0.52"),
                    new XAttribute("tap2_len_lvl", "103"),
                    new XAttribute("tap2_max_lvl", "0.80"),
                    new XAttribute("tap2_mid_lvl", "0.63"),
                    new XAttribute("psi", "0.23"),
                    new XAttribute("v4", "80"),
                    new XAttribute("v5", "160"),
                    new XAttribute("v6", "125"));
                xePages.Add(xeElements);

                xeElements = new XElement("elements",
                    new XAttribute("side", "п"),
                    new XAttribute("startkm", "424"),
                    new XAttribute("startm", "246"),
                    new XAttribute("finalkm", "424"),
                    new XAttribute("finalm", "767"),
                    new XAttribute("angle", "21.1"),
                    new XAttribute("circle_len", "402"),
                    new XAttribute("circle_min", "1211"),
                    new XAttribute("circle_max", "1291"),
                    new XAttribute("circle_mid", "1258"),
                    new XAttribute("tap1_len", "119"),
                    new XAttribute("tap1_max", "0.39"),
                    new XAttribute("tap1_mid", "0.32"),
                    new XAttribute("tap2_len", "-"),
                    new XAttribute("tap2_max", "-"),
                    new XAttribute("tap2_mid", "-"),
                    new XAttribute("pint", "0.87"),
                    new XAttribute("anp", "0.42\\0.46"),
                    new XAttribute("v1", "120"),
                    new XAttribute("v2", "138"),
                    new XAttribute("v3", "172"),
                    new XAttribute("startlvl", "30"),
                    new XAttribute("finallvl", "0"),
                    new XAttribute("circle_len_lvl", "406"),
                    new XAttribute("circle_min_lvl", "71"),
                    new XAttribute("circle_max_lvl", "80"),
                    new XAttribute("circle_mid_lvl", "76"),
                    new XAttribute("tap1_len_lvl", "145"),
                    new XAttribute("tap1_max_lvl", "0.63"),
                    new XAttribute("tap1_mid_lvl", "0.47"),
                    new XAttribute("tap2_len_lvl", "-"),
                    new XAttribute("tap2_max_lvl", "-"),
                    new XAttribute("tap2_mid_lvl", "-"),
                    new XAttribute("psi", "0.18"),
                    new XAttribute("v4", "80"),
                    new XAttribute("v5", "180"),
                    new XAttribute("v6", "135"));
                xePages.Add(xeElements);

                report.Add(xePages);

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try{
                htReport.Save(Path.GetTempPath() + "/report_TripCurveDeviceCharacterstics.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_TripCurveDeviceCharacterstics.html");
            }
        }
    }
}
