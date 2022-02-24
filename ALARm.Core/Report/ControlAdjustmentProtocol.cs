using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    public class ControlAdjustmentProtocol : RdObject
    {
        public int original_id { get; set; }
        public string Direction { get; set; }
        public string Track { get; set; }
        public int Pchu { get; set; }
        public int Pd { get; set; }
        public string FoundDate { get; set; }
        public int Km { get; set; }
        public int Meter { get; set; }
        public string NAME { get; set; }
        public int VALUE { get; set; }
        public int LENGTH { get; set; }
        public int COUNT { get; set; }
        public int Typ { get; set; }
        public int state_id { get; set; }
        public string comment { get; set; }
        public string editor { get; set; }
    }
}

