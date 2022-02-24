using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class MinorGrid : ComponentBase
    {
        [Parameter]
        public int[] MinorPositions { get; set; }
        [Parameter]
        public float[] MinorPositionsFloat { get; set; }
        [Parameter]
        public int y1 { get; set; }
        [Parameter]
        public int y2 { get; set; }
        [Parameter]
        public string StrokeWidth { get; set; }
        [Parameter]
        public string StrokeDashArray { get; set; }
        [Parameter]
        public string Color { get; set; } = null;
        [Parameter]
        public int Zero { get; set; }


    }
}
