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
    public class DeviationsByKm : Report
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
                    new XAttribute("distancetotal", "Итого по ПЧ-25: 20 м"),
                    new XAttribute("info", "№ЦДИ-770/р от 02.08.2018г"));

                XElement xeTracks = new XElement("tracks",
                    new XAttribute("trackinfo", "Шарташ-Устье Аха (20811), Путь: 1, ПЧ: 25"),
                    new XAttribute("tracktotal", "Итого путь-1: 20 м"));

                XElement xeElements = new XElement("elements",
                    new XAttribute("n", "1"),
                    new XAttribute("startkm", "142"),
                    new XAttribute("startm", "101"),
                    new XAttribute("finalkm", "143"),
                    new XAttribute("finalm", "504"),
                    new XAttribute("comment", "скрепления засыпаны щебнем"),
                    new XAttribute("thread", "правая"),
                    new XAttribute("fastening", "ЖБР-65"));
                xeTracks.Add(xeElements);

                xeElements = new XElement("elements",
                    new XAttribute("n", "2"),
                    new XAttribute("startkm", "150"),
                    new XAttribute("startm", "307"),
                    new XAttribute("finalkm", "154"),
                    new XAttribute("finalm", "357"),
                    new XAttribute("comment", "скрепления засыпаны щебнем"),
                    new XAttribute("thread", "левая, правая"),
                    new XAttribute("fastening", "КБ"));
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

                htReport.Save(Path.GetTempPath() + "/report_DeviationsByKm.html");

            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_DeviationsByKm.html");
            }
        }
    }
}
