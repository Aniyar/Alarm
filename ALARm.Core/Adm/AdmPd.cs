using System.Collections.Generic;

namespace ALARm.Core
{
    public class AdmPd : AdmUnit
    {
        public AdmPchu Parent { get; set; }
        public new List<AdmPdb> Child { get; set; }
        

    }
}