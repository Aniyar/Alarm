using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class TripPlan
    {
        public int Start_km { get; set; }
        public int Start_m { get; set; }
        public int Final_km { get; set; }
        public int Final_m { get; set; }
        public bool ItIsCurve { get; set; }
        public int Radius { get; set; }
        public string Side { get; set; }
        public string LenText { get {
                return Math.Round(Final_km - Start_km + (Final_m - Start_m) / 1000.0, 3).ToString().Replace(',', '.');
            } }
        public string TripCharac { get {
                return ItIsCurve ? "Кривая R = " + Radius.ToString() : "Прямая";
            } }
        public string Angle { get {
                double angle = ItIsCurve ? (Math.Atan((17860.0 / Radius) / Math.Abs((Final_km - Start_km) * 1000 + Final_m - Start_m)) * 180) / Math.PI : 0;
                return ItIsCurve ? Side + " " + Math.Round(angle, 1).ToString().Replace(',', '.') : "0";
            } }
    }
}
