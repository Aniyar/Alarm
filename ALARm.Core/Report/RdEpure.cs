using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class RdEpure
    {
        public Int64 Id { get; set; }
        public Int64 Process_id { get; set; }
        public Int64 Track_Id { get; set; }
        public int Km { get; set; }
        public int Piket { get; set; }
        public int Nexus { get; set; }
        public int Epure_actual { get; set; }
        public int Epure_evaluation { get; set; }
        public string Notice { get; set; }
    }
}
