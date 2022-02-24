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
    public class UnfitnessSectional : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();

            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                XElement xePages = new XElement("pages",
                    new XAttribute("direction", "Свердловская ЖД"),
                    new XAttribute("period", "Сентябрь 2019"),
                    new XAttribute("type", "рабочая/контрольная/дополнительная проверка"),
                    new XAttribute("car", "ДКИ/ПС-503"),
                    new XAttribute("data", "Проезд: 13.09.2019 07:29"),
                    new XAttribute("info", "№ЦДИ-770/р от 02.08.2018г"));

                XElement xeTracks = new XElement("tracks",
                    new XAttribute("trackinfo", "Шарташ-Устье Аха (20811), Путь: 1, ПЧ: 25"));

                XElement xeElements = new XElement("elements",
                    new XAttribute("n", "1"),
                    new XAttribute("pchu", "ПЧУ-2/ПД-3/ПДБ-1"),
                    new XAttribute("station", "Егоршино-Талый Ключ"),
                    new XAttribute("km", "142"),
                    new XAttribute("piket", "2"),
                    new XAttribute("meter", "105"),
                    new XAttribute("speed", "80/80"),
                    new XAttribute("digression", "КНШ"),
                    new XAttribute("size", "4 шт"),
                    new XAttribute("railtype", "Р65"),
                    new XAttribute("fastening", "Д65"),
                    new XAttribute("trackplan", "прямой"),
                    new XAttribute("template", ""),
                    new XAttribute("addspeed", "60/40"),
                    new XAttribute("notice", ""));
                xeTracks.Add(xeElements);

                xeElements = new XElement("elements",
                    new XAttribute("n", "2"),
                    new XAttribute("pchu", "ПЧУ-2/ПД-3/ПДБ-1"),
                    new XAttribute("station", "Егоршино-Талый Ключ"),
                    new XAttribute("km", "150"),
                    new XAttribute("piket", "4"),
                    new XAttribute("meter", "351"),
                    new XAttribute("speed", "70/60"),
                    new XAttribute("digression", "КНШ"),
                    new XAttribute("size", "5 шт"),
                    new XAttribute("railtype", "Р65"),
                    new XAttribute("fastening", "КБ"),
                    new XAttribute("trackplan", "кривая R-700"),
                    new XAttribute("template", ""),
                    new XAttribute("addspeed", "40/25"),
                    new XAttribute("notice", ""));
                xeTracks.Add(xeElements);

                xePages.Add(xeTracks);

                report.Add(xePages);

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {

                htReport.Save(Path.GetTempPath() + "/report_UnfitnessSectional.html");

            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_UnfitnessSectional.html");
            }
        }
    }
}
