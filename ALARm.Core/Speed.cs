using System;

namespace ALARm.Core
{
    public class Speed : MainTrackObject
    {
        public int Reason_id { get; set; }
        public int Passenger { get; set; }
        public int Freight { get; set; }
        public DateTime Repair_date { get; set; }
        public int Empty_Freight { get; set; }
        public int Sapsan { get; set; }
        public int Lastochka { get; set; }
        public int Strig { get; set; }
        public string Reason { get; set; }
        public override string ToString()
        {
            return Passenger + "/" + Freight;
        }
    }
}