using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALARm.Core.Report;

namespace ALARm.Core.AdditionalParameteres
{
    public class Sssp : RdObject
    {
        public string ART = "";
        public string Station = "";
        public int Piket { get; set; }
        public float Gauge { get; set; }
        public float Ner_l { get; set; }
        public float Ner_r { get; set; }
        public float Stright_avg { get; set; }
        public double Lvl { get; set; }
        public float Lvl_avg { get; set; }
        public float Lvl0 { get; set; }

        public float Level_SKO { get; set; }
        public float Level_avg { get; set; }
        public float Gauge_SKO { get; set; }
        public float Gauge_avg { get; set; }
        public float Skewness_SKO { get; set; }
        public float Skewness_avg { get; set; }
        

        public float Drawdown_left_SKO { get; set; }
        public float Drawdown_right_SKO { get; set; }
        public float Drawdown_left_avg { get; set; }
        public float Drawdown_right_avg { get; set; }

        public float Stright_left_SKO { get; set; }
        public float Stright_right_SKO { get; set; }
        public float Stright_left_avg { get; set; }
        public float Stright_right_avg { get; set; }

        public float Drawdown_left { get; set; }
        public float Drawdown_right { get; set; }
        public float Stright_left { get; set; }
        public float Stright_right { get; set; }
        public float SSSP_speed { get; set; }
        public float Sssp_vert { get; set; }
        public float Sssp_gor { get; set; }
        public float Speed { get; set; }
        public float Cv { get; set; }
        public float Cr { get; set; }
        public float Co { get; set; }
    }
}