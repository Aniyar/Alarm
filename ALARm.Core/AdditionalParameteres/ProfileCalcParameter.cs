using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.AdditionalParameteres
{
    public class ProfileCalcParameter
    {
        public double DownParam { get; set; } = -1;
        public double Radius { get; set; } = -1;
        public double LittleRadius { get; set; } = -1;
        public double AngleG { get; set; } = -1;
        public double HeadCoef { get; set; } = 0;
        public double TopSideCoef { get; set; } = 0;
        public double BottomSideCoef { get; set; } = 0;
        public double DistBigRad { get; set; } = 0;
        public double DistLitRad { get; set; } = 0;
        public double DistParam { get; set; } = 0;
        public ProfileCalcParameter(double downParam, double radius, double l_radius, double angle_g, double coefHead, double coefSideBot, double coefSideTop, double dist_big_rad, double dist_lit_rad, double dist_param)
        {
            DownParam = downParam;
            Radius = radius;
            LittleRadius = l_radius;
            AngleG = angle_g;
            HeadCoef = coefHead;
            TopSideCoef = coefSideTop;
            BottomSideCoef = coefSideBot;
            DistLitRad = dist_lit_rad;
            DistBigRad = dist_big_rad;
            DistParam = dist_param;
        }
    }
}
