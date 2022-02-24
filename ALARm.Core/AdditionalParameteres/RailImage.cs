using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.AdditionalParameteres
{
    public class RailImage
    {
        public string Base64 { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<VideoObject> Objects {get;set;}
        public List<Gap> Gaps { get; set; }
        public Gap CurrentGap { get; set; }
        public double[] CurrentGapPosition { get; set; }
        public Threat SelectedSide { get; set; }
        public int CursorPositionFromStart { get; set; }
    }
}
