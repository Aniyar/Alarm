using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class RdStatisticRoughnessImpulse
    {
        public long Process_id { get; set; }
        public long Track_id { get; set; }
        public string Track { get; set; }
        public string Direction { get; set; }
        public int Km { get; set; }
        public int Meter { get; set; }
        public double Right_Value { get; set; }
        public double Left_Value { get; set; }
    }

    public class RdIntegralSurfaceRails
    {
        public long Process_id { get; set; }
        public long Track_id { get; set; }
        public string Track { get; set; }
        public string Direction { get; set; }
        public int Km { get; set; }
        public int Meter { get; set; }
        public double Right_Value { get; set; }
        public double Left_Value { get; set; }
    }
}
