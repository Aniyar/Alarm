using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ALARm.Core
{
    public class xmlAndXslFiles
    {
        public static string detailXMLfile => System.IO.Path.GetTempPath() + "rd_vo_detail.xml";
        public static string summaryXMLfile => System.IO.Path.GetTempPath() + "rd_vo_summary.xml";
        public static string fullXMLfile => System.IO.Path.GetTempPath() + "report\\rd_vo_full.xml";
        public static string detailXSLfile => "rd_vo_detail.xsl";
        public static string summaryXSLfile => "rd_vo_summary.xsl"; 
        public static string fullXSLfile => "rd_vo_full.xsl";
        public static string detailFullXSLfile => "rd_vo_detail_full.xsl";
    }
}
