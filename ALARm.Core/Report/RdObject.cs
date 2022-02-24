using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    public class RdObject
    {
        public long ID { get; }
        public int Meter { get; set; }
        public int Picket { get; set; }
        public int Km { get; set; }
        public long track_id { get; set; }
        public long Ms { get; set; }
        public long Ms2 { get; set; }
        public double RealCoordinate => Km + Meter / 10000.0;

    }
}
