using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class CoordinateGNSS : MainTrackObject
    {
        public int Km { get; set; }
        public int Meter { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
        public double Altitude { get; set; }
        public double Exact_coordinate { get; set; }
        public double Exact_height { get; set; }
    }
}
