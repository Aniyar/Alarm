using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class RdIrregularity
    {
        public Int64 Id { get; set; }
        public Int64 Process_id { get; set; }
        public Int64 Track_Id { get; set; }
        public int Km { get; set; }
        public int Piket { get; set; }

        public int Skrep_def { get; set; }
        public int Skrep_negod { get; set; }
        public int Shpal_def { get; set; }
        public int Shpal_negod { get; set; }
        public string Otst { get; set; }
        public string Kns { get; set; }
        public string Vdop { get; set; }

        public int Meter { get; set; }
        public int Amount { get; set; }
        public double Slope_tap { get; set; }
        public int Belong { get; set; }

        public string TrackName { get; set; }
        public string Direction { get; set; }

        public bool IsBridge { get; set; } 
        public bool IsSwitch { get; set; }
        public bool IsStation { get; set; }
    }
}
