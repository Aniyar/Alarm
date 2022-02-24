using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    /// <summary>
    /// Карточка кривой
    /// </summary>
    public class RDCurve : MainTrackObject
    {
        public double FiList { get; set; }
        public double FiList2 { get; set; }
        public float Trapez_level { get; set; }
        public float Trapez_str { get; set; }

        public float Avg_level { get; set; }
        public float Avg_str { get; set; }

        public int Point_level { get; set; }
        public int Point_str { get; set; }

        public int Km { get; set; }
        public int M { get; set; }
        public int X { get; set; }

        public override string ToString()
        {
            return $"{Km} {M} {Trapez_str} {X} {Trapez_level} {FiList2}";
        }

        /// <summary>
        /// радиус
        /// </summary>
        public float Radius { get; set; }
        /// <summary>
        /// уровень
        /// </summary>
        public float Level { get; set; }
        /// <summary>
        /// шаблон
        /// </summary>
        public int Gauge { get; set; }
        public float PassBoost { get; set; }
        public float SapsanBoost { get; set; }
        public float LastochkaBoost { get; set; }
        public float SapsanBoost_anp { get; set; }
        public float LastochkaBoost_anp { get; set; }
        public float PassBoost_anp { get; set; }
        public float FreightBoost_anp { get; set; }
        public float FreightBoost { get; set; }
        public int PassSpeed { get; set; }
        public int FreightSpeed { get; set; }
        public float Broadening { get; set; }
        public float Wear { get; set; }
        public long Curve_id { get; set; }
        public long Process_Id { get; set; }
        public double GetRealCoordinate()
        {
            return Km + M / 10000.0;
        }
        
        public string GetRadiusCoord()
        {
            return X + "," + (Radius * Math.Sign(Trapez_str == 0 ? 1 : Trapez_str)).ToString().Replace(",", ".") + " ";
        }
        public string GetLevelCoord()
        {
            return X + "," + (Level * Math.Sign(Trapez_str == 0 ? 1 : Trapez_str)).ToString().Replace(",", ".") + " ";
        }
        public string GetGaugeCoord()
        {
            return X + "," + (-Gauge).ToString().Replace(",", ".") + " ";
        }
        public string GetLastochkaBoostCoord()
        {
            return X + "," + (-LastochkaBoost).ToString().Replace(",", ".") + " ";
        }
        public string GetPassBoostCoord()
        {
            return X + "," + (-PassBoost).ToString().Replace(",", ".") + " ";
        }
        public string GetFreightBoostCoord()
        {
            return X + "," + (-FreightBoost).ToString().Replace(",", ".") + " ";
        }

        public string GetTrapez_levelCoord()
        {
            return X + "," + Trapez_level.ToString().Replace(",", ".") + " ";
        }

        public string GetTrapez_strCoord()
        {
            return X + "," + Trapez_str.ToString().Replace(",", ".") + " ";
        }

        public string GetDev()
        {
            return X + "," + (Level * Math.Sign(Trapez_str) - Trapez_level).ToString().Replace(",", ".") + " ";
        }
    }
}
