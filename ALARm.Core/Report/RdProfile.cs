using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class RdProfile
    {
        public Int64 Id { get; set; }
        public Int64 Process_id { get; set; }
        public Int64 Track_id { get; set; }
        public string Track { get; set; }
        public int Km { get; set; }
        public int Meter { get; set; }
        public int Koord { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Heigth { get; set; }

        public double Level { get; set; }
        public double Gauge { get; set; }

        public bool IsCurve { get; set; }
        public bool IsBridge { get; set; }
        public bool IsSwitch { get; set; }
        public bool IsStation { get; set; }

        public int X { get; set; }
        public double Y { get; set; }
        public double Deviation { get; set; }

        public string Direction { get; set; }
        public DateTime tripDate { get; set; }
        public int M { get; set; }

        public int Jgps_sats { get; set; }

        public double Irregularities_in_plan { get; set; }
        public double Profile_irregularities { get; set; }
        public double NerPlana { get; set; }

        public double Stright_avg { get; set; }
        public double Stright_left { get; set; }
        public double FlukStr { get; set; }
        public double FlukProfile { get; set; }
        public double Stright_right { get; set; }
        public double Plan { get; set; }
        public double Realcoord { get; set; }
        public double GetRealCoordinate()
        {
            Realcoord = Km + Meter / 10000.0;
            return Realcoord;
        }

    }
}
