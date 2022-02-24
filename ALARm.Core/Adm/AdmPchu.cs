using System.Collections.Generic;

namespace ALARm.Core
{
    public class AdmPchu : AdmUnit
    {
        public AdmDistance Parent { get; set; }
        public new List<AdmPd> Child { get; set; }
        

    }
}