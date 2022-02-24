using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class StationTrack: StationObject
    {
        public string Station { get; set; }
        public Int64 Adm_station_id { get; set; }
        public string Park { get; set; }
        public Int64 Stw_park_id { get; set; } = -1;
        public string Track { get; set; }
        public Int64 Adm_track_id { get; set; }
        public string Border { get; set; }
        public string Belong { get; set; }
    }
}
