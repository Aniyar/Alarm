using System;

namespace ALARm.Core
{
    public class DistSection : MainTrackObject {
        public Int64 DistanceId { get; set; }
        public string Road { get; set; }
        public string Distance { get; set; }
        public string note { get; set; }
        
        public int Pchu { get; set; }
        public int Pd { get; set; }
        public int Pdb { get; set; }

        public int Len { get; set; }
    }
}