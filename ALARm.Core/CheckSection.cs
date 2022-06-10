using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class CheckSection : MainTrackObject
    {
        public long Track_Id { get; set; }
        public double Avg_width { get; set; }
        public double Avg_level { get; set; }
        public double Sko_width { get; set; }
        public double Sko_level { get; set; }
        public double Trip_mo_level { get; set; }
        public double Trip_sko_level { get; set; }
        public double Trip_mo_gauge { get; set; }
        public double Trip_sko_gauge { get; set; }
    }
}
