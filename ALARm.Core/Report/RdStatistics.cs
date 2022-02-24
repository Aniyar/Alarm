using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    public class RdStatistics
    {
        public Int64 Id { get; set; }
        public Int64 Process_id { get; set; }
        public int Len { get; set; }
        public int Overlay_count { get; set; }
        public int Overlay_correct { get; set; }
        public int Overlay_wrong { get; set; }
        public int Overlay_missint { get; set; }
        public int Overlay_identify { get; set; }
        public int Overlay_false { get; set; }
        public int Boltfree_count { get; set; }
        public int Boltfree_correct { get; set; }
        public int Boltfree_missing { get; set; }
        public int Boltfree_as_bolt { get; set; }
        public int Boltfree_as_wrongoverlay { get; set; }
        public int Boltfree_as_missingoverlay { get; set; }
        public int Boltfree_identify { get; set; }
        public int Boltfree_false { get; set; }
        public int Boltfree_false_bolt { get; set; }
        public int Boltfree_false_wrongoverlay { get; set; }
        public int Boltfree_false_missingoverlay { get; set; }
        public int Joint_count { get; set; }
        public int Joint_correct { get; set; }
        public int Joint_wrong { get; set; }
        public int Isojoint_wrong { get; set; }
        public int Joint_missing { get; set; }
        public int Isojoint_missing { get; set; }
        public int Joint_identify { get; set; }
        public int Joint_false { get; set; }
        public int Marks_count { get; set; }
        public int Marks_correct { get; set; }
        public int Marks_wrong { get; set; }
        public int Marks_missing { get; set; }
        public int Marks_identify { get; set; }
        public int Marks_false { get; set; }
        public int Defects_count { get; set; }
        public int Defects_correct { get; set; }
        public int Defects_missing { get; set; }
        public int Defects_identify { get; set; }
        public int Defects_false { get; set; }
        public int Fastening_count { get; set; }
        public int Fastening_correct { get; set; }
        public int Fastening_missing { get; set; }
        public int Sleeper_count { get; set; }
        public int Sleeper_correct { get; set; }
        public int Sleeper_wrong { get; set; }
        public int Sleeper_missing { get; set; }
        public int Sleeper_identify { get; set; }
        public int Sleeper_false { get; set; }
        public int Antitheft_count { get; set; }
        public int Antitheft_correct { get; set; }
        public int Antitheft_wrong { get; set; }
        public int Antitheft_missing { get; set; }
        public int Antitheft_identify { get; set; }
        public int Antitheft_false { get; set; }
        public int Switch_count { get; set; }
        public int Switch_correct { get; set; }
        public int Switch_wrong { get; set; }
        public int Switch_missing { get; set; }
        public int Switch_identify { get; set; }
        public int Switch_false { get; set; }
    }
}
