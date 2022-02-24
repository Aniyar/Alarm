using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class DistanceBetweenTracks : MainTrackObject
    {
        public int Left_m { get; set; }
        public Int64 Left_adm_track_id { get; set; }
        public string Left_track { get; set; }
        public int Right_m { get; set; }
        public Int64 Right_adm_track_id { get; set; }
        public string Right_track { get; set; }
    }
}
