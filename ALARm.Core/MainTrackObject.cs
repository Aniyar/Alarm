using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class MainTrackObject
    { 
        public Int64 Id { get; set; }
        public int Start_Km { get; set; }
        public int Start_M { get; set; }
        public int Final_Km { get; set; }
        public int Final_M { get; set; }
        private long DirectionId { get; set; }
        public long Period_Id { get; set; }
        public long Track_Id { get; set; }
        public string Track_Code { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime Final_Date { get; set; }

        public double RealStartCoordinate => Start_Km + Start_M / 10000.0;
        public double RealFinalCoordinate => Final_Km + Final_M / 10000.0;

        public double CurveFinalKord { get; set; } = -999;

    }
}
    
