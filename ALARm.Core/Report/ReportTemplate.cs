using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    public class ReportTemplate
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Xsl { get; set; }
        public string Xml { get; set; }
        public string ClassName { get; set; }
        public int Rep_type { get; set; }
    }
}
