using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    public class NotCheckedKm
    {
        public string Direction_name_code { get; set; }
        public string Track_name { get; set; }
        public int Start_km { get; set; }
        public int Start_m { get; set; }
        public int Final_km { get; set; }
        public int Final_m { get; set; }
        public string Note { get; set; }
    }
}
