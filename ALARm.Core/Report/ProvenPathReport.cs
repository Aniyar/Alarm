using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    public class ProvenPathReport
    {
        public string Direction { get; set; }
        public int CountDirection { get; set; }
        public int Pchu { get; set; }
        public int CountPchu { get; set; }
        public int Pd { get; set; }
        public string Track { get; set; }
        public double KmAll { get; set; }
        public double KmCheck { get; set; }
        public double KmNotCheck { get; set; }
        public double Check { get; set; }
        public double NotCheck { get; set; }
    }

    public class ProvenPathTotalReport
    {
        public string Direction { get; set; }
        public double DKmAll { get; set; }
        public double DKmCheck { get; set; }
        public double DKmNotCheck { get; set; }
    }
}
