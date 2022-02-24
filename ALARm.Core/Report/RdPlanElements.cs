using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class RdPlanElements
    {
        public Int64 Id { get; set; }
        public Int64 Process_id { get; set; }
        public Int64 Track_Id { get; set; }
        public int Start_Km { get; set; }
        public int Start_M { get; set; }
        public int Final_Km { get; set; }
        public int Final_M { get; set; }
        public double Angle { get; set; }
        public int Len { get; set; }
        public bool Is_curve { get; set; }
        public int Side_id { get; set; }
        public int Radius { get; set; }
        public int Digression { get; set; }

        public string TrackName { get; set; }
    }
}
