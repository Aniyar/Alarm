using ALARm.Core;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class PointChart : ComponentBase
    {
        [Parameter]
        public double[] Points { get; set; }
        [Parameter]
        public string NominalRail { get; set; }
        [Parameter]
        public string NominalRailRotate { get; set; }
        [Parameter]
        public string NominalRailTranslate { get; set; }
        [Parameter]
        public string DownHill { get; set; }
        [Parameter]
        public string Tilt { get; set; }
        [Parameter]
        public int Width { get; set; }
        [Parameter]
        public int Heigth { get; set; }
        [Parameter]
        public string ViewBox { get; set; }
        [Parameter]
        public string Style { get; set; }
        [Parameter]
        public Side Side { get; set; }
        protected override void OnParametersSet()
        {
            if (Points != null)
            {
                IEnumerable<double> x = Points.Where((value, index) => index % 2 == 0 && value < 500);
                IEnumerable<double> y = Points.Where((value, index) => index % 2 == 1 && value < 500);
                var minX = (int)x.Average() - 25;
                var widthX = Side == Side.Right ? 120 : 100;

                var minY = (int)y.Min() + (Side == Side.Right ? 10 : 0);
                var heigthY = (int)(y.Max() - minY);
                ViewBox = $"{minX} {minY} {widthX} {heigthY}";
            }
        }
    }
    
}
