using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class Gaps
    {
        public long Id { get; set; }
        public long Process_id { get; set; }
        public long File_id { get; set; }
        public long Track_id { get; set; }
        public int Threat_id { get; set; }
        public int Frame_number { get; set; }
        public int Km { get; set; }
        public int Picket { get; set; }
        public int Meter { get; set; }
        public int Start { get; set; }
        public int Final { get; set; }
        public double Nominal { get; set; }
        public int Length { get; set; }
        public int Step { get; set; }
        public int Measure { get {
                return Math.Abs(Final - Start);
            } }
        public Side Side { get; set; }
        public int Real_meter { get {
                return (Picket - 1) * 100 + Meter;
            } }


        public string Direction_full { get; set; }
        public string Track { get; set; }
		public string IfIso { get; set; } = "";
		public string Primech { get; set; } = "";
    }
    public enum GapSource { Laser = 0, Bitmap = 1 };
}
