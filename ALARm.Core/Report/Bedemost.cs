using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    public class Bedemost
    {
        public long Id { get; set; }
        public string Pch { get; set; }
        public string Naprav { get; set; }
        public string Put { get; set; }
        public int Pchu { get; set; }
        public int Pd { get; set; }
        public int Pdb { get; set; }
        public int Km { get; set; }
        public int Kmtrue { get; set; }
        public int Tip_poezdki { get; set; }
        public long Process_id { get; set; }
        public double Lkm { get; set; }
        public int Ball { get; set; }
        public string Otsenka { get; set; }
        public int Type1 { get; set; }
    }
}
