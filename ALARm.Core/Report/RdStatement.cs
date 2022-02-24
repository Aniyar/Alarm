using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    public class RdStatement
    {
        public Int64 Id { get; set; }
        public Int64 Process_id { get; set; }
        public Int64 Track_Id { get; set; }
        public Int64 Distance_id { get; set; }
        public Int64 Pchu_id { get; set; }
        public int Km { get; set; }
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

        public int JointSpeed()
        {
            return Joint - Joint0 - Joint15 - Joint25 - Joint40 - Joint60;
        }
        public int JointlessSpeed()
        {
            return Jointless - Jointless0 - Jointless15 - Jointless25 - Jointless40 - Jointless60;
        }
        public int BallastSpeed()
        {
            return Ballast - Ballast0 - Ballast15 - Ballast25 - Ballast40 - Ballast60;
        }
        public int Surface_railSpeed()
        {
            return Surface_rail - Surface_rail0 - Surface_rail15 - Surface_rail25 - Surface_rail40 - Surface_rail60;
        }
        public int SleeperSpeed()
        {
            return Sleeper - Sleeper0 - Sleeper15 - Sleeper25 - Sleeper40 - Sleeper60;
        }
        public int FasteningSpeed()
        {
            return Fastening - Fastening0 - Fastening15 - Fastening25 - Fastening40 - Fastening60;
        }
    }
}
