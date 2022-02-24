using System.Collections.Generic;

namespace ALARm.Core
{
    public class AdmUnit
    {
        public AdmUnit() { }
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Chief_fullname { get; set; }
        public string Abbr { get; set; }
        public List<AdmUnit> Child;
        public long Parent_Id { get; set; } = -1;
    }
}