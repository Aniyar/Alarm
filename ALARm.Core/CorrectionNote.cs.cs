using System;
using System.Collections.Generic;
using System.Text;

namespace ALARm.Core
{
    public class CorrectionNote
    {
        public int Type { get; set; }
        public int Km { get; set; }
        public int Meter { get; set; }
        public string CorrectionValue { get; set; }

        public int coord { get; set; }

    }
}
