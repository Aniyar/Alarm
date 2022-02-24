using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class RepairProject : MainTrackObject
    {
        public Int64 Adm_track_id { get; set; }
        public int Accept_id { get; set; }
        public string Accept { get; set; }
        public int Type_id { get; set; }
        public string Type { get; set; }
        public DateTime Repair_date { get; set; }
        public string Repair_short_date {
            get {
                return Repair_date.ToString("MMMM yyyy");
            }
        }
        public DateTime Accepted_At { get; set; }
        public int Speed { get; set; }
    }
}
