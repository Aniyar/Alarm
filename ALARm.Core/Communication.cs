using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class Communication : MainTrackObject
    {
        public int Km { get; set; }
        public int Meter { get; set; }
        public int Object_id { get; set; }
        public string Object { get; set; }
    }
}
