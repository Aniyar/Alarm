using System;

namespace ALARm.Core
{
    public class Speed : MainTrackObject
    {
        public int Passenger { get; set; }
        public int Freight { get; set; }
        public int Empty_Freight { get; set; }
        public int Sapsan { get; set; }
        public int Lastochka { get; set; }
		public override string ToString()
        {
            return Passenger + "/" + Freight;
        }
    }
}