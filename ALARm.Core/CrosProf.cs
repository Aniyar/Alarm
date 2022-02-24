using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALARm.Core.Report;

namespace ALARm.Core.AdditionalParameteres
{
    public class CrosProf : RdObject
    {
 
        public int Kmetr { get; set; }
        public int Meter { get; set; }
        public string DigName { get; set; }
        public int Typ { get; set; }
        public int Count { get; set; }
        public int Length { get; set; }
        public string FullSpeed { get; set; }
    
        public string Start_ { get; set; }
        public string Final_ { get; set; }
        public string Rail_type { get; set; }
        public string Brace_type { get; set; }

        public float Stright_left { get; set; }
        public float Pu_l { get; set; }
        public float Pu_r { get; set; }
        public float Vert_l { get; set; }
        public float Vert_r { get; set; }
        public float Bok_l { get; set; }
        public float Bok_r { get; set; }
        public float Npk_l { get; set; }
        public float Npk_r { get; set; }
        public float Shortwavesleft { get; set; }
        public float Shortwavesright { get; set; }
        public float Mediumwavesleft { get; set; }
        public float Mediumwavesright { get; set; }
        public float Longwavesleft { get; set; }
        public float Longwavesright { get; set; }
        public float Iz_45_l { get; set; }
        public float Iz_45_r { get; set; }

        public float Avg_pu_l { get; set; }
        public float Sko_pu_l { get; set; }
        public float Avg_pu_r { get; set; }
        public float Sko_pu_r { get; set; }
        public float Avg_npk_l { get; set; }
        public float Sko_npk_l { get; set; }
        public float Avg_npk_r { get; set; }
        public float Sko_npk_r { get; set; }
        public float Gauge { get; set; }
        public int Radius { get; set; }
        public int Piket { get; set; }

    }
}