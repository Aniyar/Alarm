namespace ALARm.Core
{
    public class PdbSection : DistSection
    {
        
        public long PdbId { get; set; }
        public string Pdb { get; set; }
        public string Pd { get; set; }
        public string Pchu { get; set; }
        public string RoadAbbr { get; set; }
        public string Chief { get; set; }
        public override string ToString()
        {
            return $"ПЧ-{Distance}/ПЧУ-{Pchu}/ПД-{Pd}/ПДБ-{Pdb}";
        }
        public string Nod { get; set; }

    }
}