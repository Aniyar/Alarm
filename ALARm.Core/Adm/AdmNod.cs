using System.Collections.Generic;

namespace ALARm.Core
{
    public class AdmNod : AdmUnit
    {
        public AdmRoad Parent { get; set; }
        public new List<AdmDistance> Child { get; set; }
        private new string Chief_fullname { get; set; }


    }
}