using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class MainParameters
    {
        public string Direction { get; set; }
        public string TrackNumber { get; set; }
        public int KilometrNumber { get; set; }
        public int Legnth { get; set; }
        public CarParameters Car { get; set; }
        public Direction TravelDirection { get; set; }
        public DateTime TravelDate { get; set; }
        public List<float> Gauge { get; set; }
        public List<double> GaugeNorm { get; set; }
        public List<double> ZeroGauge { get; set; }
        public List<double> DrawdoownRigth { get; set; }
        public List<double> DrawdownLeft { get; set; }
        public List<double> StraighteningRigth { get; set; }
        public List<double> ZeroStraightening { get; set; }
        public List<double> StraighteningLeft { get; set; }
        public List<float> NerovPlana { get; set; }
        public List<float> Krivizna { get; set; }
        public List<double> AverageStraightening { get; set; }
        public List<double> Level { get; set; }
		public List<double> Perekos { get; set; }
        public List<double> CCPspeed { get; set; }
        public List<double> AverageLevel { get; set; }
        public List<double> ZeroLevel { get; set; }
        public List<double> StraighteningSide { get; set; }
        public List<int> Meters { get; set; }
        public List<ArtificialConstruction> ArtificialConstructions { get; set; }
        public List<Curve> Curves { get; set; }
        public List<CrossTie> CrossTies { get; set; }
        public List<Switch> Switches { get; set; }
        public List<float> TrackWidth { get; set; } //ширина кольи

        public List<float> Strl { get; set; }
        public List<float> Strr { get; set; }
        public List<float> LVL { get; set; }
		public List<float> CCCPspeedBYpiket { get; set; }
        public List<float> CCCPspeedBYpiketGARB { get; set; }
        
        public void ParseSKOvedomost(string line)
        {
            line = Regex.Replace(line, @"\s+", " ");
            var parameters = line.Split(new char[] { ' ', '\t' });
            {
                DrawdownLeft.Add(double.Parse(parameters[23], CultureInfo.InvariantCulture.NumberFormat));
                DrawdoownRigth.Add(double.Parse(parameters[24], CultureInfo.InvariantCulture.NumberFormat));
                StraighteningLeft.Add(double.Parse(parameters[30], CultureInfo.InvariantCulture.NumberFormat));
                StraighteningRigth.Add(double.Parse(parameters[31], CultureInfo.InvariantCulture.NumberFormat));
                Gauge.Add(float.Parse(parameters[32], CultureInfo.InvariantCulture.NumberFormat));
                Perekos.Add(double.Parse(parameters[26], CultureInfo.InvariantCulture.NumberFormat));
                CCPspeed.Add(double.Parse(parameters[29], CultureInfo.InvariantCulture.NumberFormat));
            }
        }
        public List<float> LevelNew { get; set; }
        public List<float> DrL { get; set; }
        public List<float> DrR { get; set; }
        public List<float> TrW { get; set; }

        public void Parse(string line)
        {
            line = Regex.Replace(line, @"\s+", " ");
            var parameters = line.Split(new char[] { ' ', '\t' });
            {
                var z_angle = double.Parse(parameters[10], CultureInfo.InvariantCulture.NumberFormat) * (int)TravelDirection;
                Meters.Add((int.Parse(parameters[2]) - 1) * 100 + int.Parse(parameters[3]));
                Gauge.Add(float.Parse(parameters[6], CultureInfo.InvariantCulture.NumberFormat));
                DrawdoownRigth.Add(float.Parse(parameters[4], CultureInfo.InvariantCulture.NumberFormat) * MainParametersConst.koef_dropdown * (int)TravelDirection);
                DrawdownLeft.Add(float.Parse(parameters[5], CultureInfo.InvariantCulture.NumberFormat) * MainParametersConst.koef_dropdown * (int)TravelDirection);
                StraighteningRigth.Add(z_angle + (double.Parse(parameters[8], CultureInfo.InvariantCulture.NumberFormat) * (int)TravelDirection - z_angle) * MainParametersConst.koef_straightening);
                StraighteningLeft.Add(z_angle + (double.Parse(parameters[7], CultureInfo.InvariantCulture.NumberFormat) * (int)TravelDirection - z_angle) * MainParametersConst.koef_straightening);
                Level.Add(double.Parse(parameters[9], CultureInfo.InvariantCulture.NumberFormat) * (int)TravelDirection);
                AverageLevel.Add(double.Parse(parameters[11], CultureInfo.InvariantCulture.NumberFormat) * (int)TravelDirection);
                AverageStraightening.Add(z_angle);
            }
        }
        public MainParameters()
        {
            Meters = new List<int>();
            Gauge = new List<float>();
            GaugeNorm = new List<double>();
            ZeroGauge = new List<double>();
            DrawdownLeft = new List<double>();
            DrawdoownRigth = new List<double>();
            StraighteningLeft = new List<double>();
            StraighteningRigth = new List<double>();
            AverageStraightening = new List<double>();
            ZeroStraightening = new List<double>();
            Level = new List<double>();
			Perekos = new List<double>();
            CCPspeed = new List<double>();
            AverageLevel = new List<double>();
            ZeroLevel = new List<double>();
            ArtificialConstructions = new List<ArtificialConstruction>();
            StraighteningSide = new List<double>();
            Legnth = 1000;

			CCCPspeedBYpiket = new List<float>();
            CCCPspeedBYpiketGARB = new List<float>();
            StraighteningLeft = new List<double>();
            NerovPlana = new List<float>();
            Krivizna = new List<float>();
            StraighteningRigth = new List<double>();
            Level = new List<double>();
            DrawdownLeft = new List<double>();
            DrawdoownRigth = new List<double>();
            TrackWidth = new List<float>();

            Strl = new List<float>();
            Strr = new List<float>();
            LVL = new List<float>();
            LevelNew = new List<float>();
            DrL = new List<float>();
            DrR = new List<float>();
            TrW = new List<float>();
        }
        public int GetStraighteningAverageDirect(int meter)
        {
            int result = 1;
            if (meter < 0)
                return result;
            if (!(AverageStraightening is null))
            {
                int index = Meters.IndexOf(meter);
                result = AverageStraightening[index >0 ? index :0] < 0 ? -1 : 1;
            }
            return result;
        }
        int prevPiket = -1;
        string sumStraigLeftPIKET = string.Empty;

        public void ParseSKO(string line)
        {
            line = Regex.Replace(line, @"\s+", " ");
            var parameters = line.Split(new char[] { ' ', '\t' });
            {
                if (Convert.ToInt32(parameters[2]) != prevPiket)
                {
					if (CCCPspeedBYpiketGARB.Count() > 0)
                    {
                        CCCPspeedBYpiket.Add(CCCPspeedBYpiketGARB.Average());
                        CCCPspeedBYpiketGARB.Clear();
                    }
                    if (Strl.Count() > 0)
                    {
                        StraighteningLeft.Add(Strl.Average());
                        Strl.Clear();
                    }
                    if (Strr.Count() > 0)
                    {
                        StraighteningRigth.Add(Strr.Average());
                        Strr.Clear();
                    }
                    if (LVL.Count() > 0)
                    {
                        LevelNew.Add(LVL.Average());
                        LVL.Clear();
                    }
                    if (DrL.Count() > 0)
                    {
                        DrawdownLeft.Add(DrL.Average());
                        DrL.Clear();
                    }
                    if (DrR.Count() > 0)
                    {
                        DrawdoownRigth.Add(DrR.Average());
                        DrR.Clear();
                    }
                    if (TrW.Count() > 0)
                    {
                        TrackWidth.Add(TrW.Average());
                        TrW.Clear();
                    }
                    prevPiket = Convert.ToInt32(parameters[2]);

                }

                if (Convert.ToInt32(parameters[2]) == prevPiket)
                {
                    //prevPiket = Convert.ToInt32(parameters[2]);
                    CCCPspeedBYpiketGARB.Add(float.Parse(parameters[29], CultureInfo.InvariantCulture.NumberFormat));
                    Strl.Add(float.Parse(parameters[30], CultureInfo.InvariantCulture.NumberFormat));
                    Strr.Add(float.Parse(parameters[31], CultureInfo.InvariantCulture.NumberFormat));
                    LVL.Add(float.Parse(parameters[26], CultureInfo.InvariantCulture.NumberFormat));
                    DrL.Add(float.Parse(parameters[24], CultureInfo.InvariantCulture.NumberFormat));
                    DrR.Add(float.Parse(parameters[25], CultureInfo.InvariantCulture.NumberFormat));
                    TrW.Add(float.Parse(parameters[32], CultureInfo.InvariantCulture.NumberFormat));
                }
                //StraighteningLeft.Add(double.Parse(parameters[30], CultureInfo.InvariantCulture.NumberFormat));
                //StraighteningRigth.Add(double.Parse(parameters[31], CultureInfo.InvariantCulture.NumberFormat));
                //Level.Add(double.Parse(parameters[25], CultureInfo.InvariantCulture.NumberFormat));
                //DrawdownLeft.Add(double.Parse(parameters[23], CultureInfo.InvariantCulture.NumberFormat));
                //DrawdoownRigth.Add(double.Parse(parameters[24], CultureInfo.InvariantCulture.NumberFormat));
                // TrackWidth.Add(double.Parse(parameters[8]));
            }
        }
        public void ParseDevPlan(string line)
        {
            line = Regex.Replace(line, @"\s+", " ");
            var parameters = line.Split(new char[] { ' ', '\t' });
            {

                if (Convert.ToInt32(parameters[2]) != prevPiket)
                {
                    if (Strl.Count() != 0)
                    {
                        NerovPlana.Add(Strl.Average() - Strr.Average());
                    }
                    if (Strr.Count() != 0)
                    {
                        Krivizna.Add((Strl.Average() + Strr.Average()) / 2);
                    }

                    Strl.Clear();
                    Strr.Clear();
                    prevPiket = Convert.ToInt32(parameters[2]);
                }

                if (Convert.ToInt32(parameters[2]) == prevPiket)
                {
                    //prevPiket = Convert.ToInt32(parameters[2]);
                    Strl.Add(float.Parse(parameters[30], CultureInfo.InvariantCulture.NumberFormat));
                    Strr.Add(float.Parse(parameters[31], CultureInfo.InvariantCulture.NumberFormat));
                }
            }
        }
    }
}
