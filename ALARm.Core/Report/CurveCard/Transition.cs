using System;
using System.Collections.Generic;
using System.Text;

namespace ALARm.Core.Report.CurveCard
{
    public class Transition
    {
        
        

        public Transition(int nOld, double foundAlpa1, double foundBetta1)
        {
            N = nOld;
            Alpha = foundAlpa1;
            Betta = foundBetta1;
        }
        public override string ToString()
        {
            return $"N={N} Alpha={Alpha} Betta={Betta}";
        }
        public int N { get; set; }
        public double Alpha { get; set; }
        public double Betta { get; set; }
    }
}
