using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    public class OutData
    {
        public int id { get; set; }
        public int x { get; set; }
        public int speed { get; set; }
        public int km { get; set; }
        public int meter { get; set; }

        public double RealCoordinate => km + meter / 10000.0;

        public double gauge { get; set; }
        public double x101_kupe { get; set; }
        public double x102_koridor { get; set; }
        public double y101_kupe { get; set; }
        public double y102_koridor { get; set; }
        public double gauge_correction { get; set; }
        public double level { get; set; }
        public double level_correction { get; set; }
        public double stright_left { get; set; }
        public double stright_right { get; set; }
        public double stright_avg { get; set; }
        public double stright_avg_70 { get; set; }
        public double stright_avg_100 { get; set; }
        public double stright_avg_120 { get; set; }
        public double stright_avg_150 { get; set; }
        public double drawdown_left { get; set; }
        public double drawdown_right { get; set; }
        public double val01 { get; set; }
        public int _meters { get; set; }
        public double level_avg { get; set; }
        public double level_avg_70 { get; set; }
        public double level_avg_100 { get; set; }
        public double level_avg_120 { get; set; }
        public double level_avg_150 { get; set; }
        public double drawdown_avg { get; set; }
        public double drawdown_avg_70 { get; set; }
        public double drawdown_avg_100 { get; set; }
        public double drawdown_avg_120 { get; set; }
        public double drawdown_avg_150 { get; set; }
        public double drawdown_left_sko { get; set; }
        public double drawdown_right_sko { get; set; }
        public double level_sko { get; set; }
        public double skewness_pxi { get; set; }
        public double skewness_sko { get; set; }
        public double sssp_before { get; set; }
        public double sssp_speed { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double heigth { get; set; }
        public double level_zero { get; set; }
        public int trip_id { get; set; }
        public int correction { get; set; }
        public double level1 { get; set; }
        public double level2 { get; set; }
        public double level3 { get; set; }
        public double level4 { get; set; }
        public double level5 { get; set; }
        public double stright1 { get; set; }
        public double stright2 { get; set; }
        public double stright3 { get; set; }
        public double stright4 { get; set; }
        public double stright5 { get; set; }

        public override string ToString()
        {
            return $"km={km} meter={meter} coorection={correction} real={RealCoordinate} {id} {x}";
        }

    }
}
