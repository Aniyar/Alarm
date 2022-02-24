using System.Collections.Generic;

namespace ALARm.Core
{
    public class AdmRoad : AdmUnit
    {
        public new List<AdmNod> Child { get; set; }
        private new string Chief_fullname { get; set; }
    }
}