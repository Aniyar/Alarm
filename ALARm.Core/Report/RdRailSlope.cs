using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class RdRailSlope
    {
        public long Id { get; set; }
        public long Process_id { get; set; }
        public long File_id { get; set; }
        public long Threat_id { get; set; } = 0;
        public long Track_id { get; set; }
        public int Km { get; set; }
        public int Meter { get; set; }
        public double Slope_value { get; set; }

        public int Picket {
            get {
                return Meter / 100 + 1;
            } 
        }
        public string Track { get; set; }
        public string Direction { get; set; }
        public string Threat { get {
                switch (Threat_id)
                {
                    case -1:
                        return "Левая";
                    case 1:
                        return "Правая";
                    default:
                        return "-";
                }
            } }
    }
}
