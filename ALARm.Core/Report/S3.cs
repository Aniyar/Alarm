using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    public class S3 : RdObject
    {
        public string Roadcode { get; set; }        
        public string Directcode { get; set; }        
        public DateTime Date { get; set; }        
        public string Pscode { get; set; }        
        public string Nput { get; set; }        
        public int Nkm { get; set; }        
        public int Nmeter { get; set; }        
        public string Ots { get; set; }        
        public int Otkl { get; set; }        
        public int Len { get; set; }        
        public int Typ { get; set; }        
        public int Kol { get; set; }        
        public int Uv { get; set; }        
        public int Uvg { get; set; }        
        public int Ovp { get; set; }        
        public int Ogp { get; set; }        
        //todo porozh        
        public int Ogrporozh { get; set; }        
        //todo strelka        
        public int Strelka { get; set; }
        //todo promech        
        public string Primech { get; set; } = "";
        public string Piket { get; set; }
        public string Pch { get; set; }
        public string Naprav { get; set; }
        public string Put { get; set; }
        public int Pchu { get; set; }
        public int Pd { get; set; }
        public int Pdb { get; set; }
        public Int64 Process_id { get; set; }
        public long Trip_id { get; set; }
        public long Track_id { get; set; }
        public int Tip_poezdki { get; set; }
        public int Cu { get; set; }
        public int Us { get; set; }
        public DateTime TripDateTime { get; set; }

        public string Direction_full { get; set; }
        public Threat Threat_id { get; set; }
    }

    public class DigressionTotal
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class DigressionTotalTrack
    {
        public long Track_id = 0;
        public int Count = 0;
    }
    public class SumOfTheDep : RdObject
    {
        public new int Km { get; set; }
        public int Fastening { get; set; }
        public int Fastening0 { get; set; }
        public int Fastening15 { get; set; }
        public int Fastening25 { get; set; }
        public int Fastening40 { get; set; }
        public int Fastening60 { get; set; }
        public int Joint { get; set; }
        public int Joint0 { get; set; }
        public int Joint15 { get; set; }
        public int Joint25 { get; set; }
        public int Joint40 { get; set; }
        public int Joint60 { get; set; }
        public int Sleeper { get; set; }
        public int Sleeper0 { get; set; }
        public int Sleeper15 { get; set; }
        public int Sleeper25 { get; set; }
        public int Sleeper40 { get; set; }
        public int Sleeper60 { get; set; }
        public int Surface_rail { get; set; }
        public int Surface_rail0 { get; set; }
        public int Surface_rail15 { get; set; }
        public int Surface_rail25 { get; set; }
        public int Surface_rail40 { get; set; }
        public int Surface_rail60 { get; set; }
        public int Ballast { get; set; }
        public int Ballast0 { get; set; }
        public int Ballast15 { get; set; }
        public int Ballast25 { get; set; }
        public int Ballast40 { get; set; }
        public int Ballast60 { get; set; }
        public int Jointless { get; set; }
        public int Jointless0 { get; set; }
        public int Jointless15 { get; set; }
        public int Jointless25 { get; set; }
        public int Jointless40 { get; set; }
        public int Jointless60 { get; set; }
        public int Other { get; set; }
    }
}
