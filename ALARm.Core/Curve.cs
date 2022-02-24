using System.Collections.Generic;


namespace ALARm.Core
{
    public class Curve : MainTrackObject
    {
        public int Start_coord { get; set; }
        public int Final_coord { get; set; }
        public int Radius { get; set; }
        public int Side_id { get; set; }
        public string Side { get; set; }

        public float Altitude_start { get; set; }
        public float Altitude_final { get; set; }
        //public List<ElCurve> Radiuses { get; set; }
        public List<StCurve> Straightenings { get; set; }
        public List<ElCurve> Elevations { get; set; }
        public object Passspeed { get; set; }
        public object Freightspeed { get; set; }
		public double MaxRadius { get; set; }
        public double MaxLvl { get; set; }
        public int Len { get; set; }
        public int Num { get; set; }
    }
	
	public class CurveParams
    {
        public int Speed_pass { get; set; }
        public int Speed_frei { get; set; }
        public int Limit_speed_pass { get; set; }
        public int Limit_speed_frei { get; set; }
        public string Brace { get; set; }
        public string Fastening { get; set; }
    }

    public class MatchCurveCoords
    {
        public int StAbsCoords { get; set; }
        public int StElDifference { get; set; } = 0;
        public int Lvl { get; set; } = 0;
    }
}