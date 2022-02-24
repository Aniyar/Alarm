using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class DgTitleCol : ComponentBase
    {
        [Parameter]
        public int X { get; set; }
        [Parameter]
        public int Heigth { get; set; }
        [Parameter]
        public int Width { get; set; }
        [Parameter]
        public string Title { get; set; }
        [Parameter]
        public bool ShowValue { get; set; } = false;
        [Parameter]
        public string Value { get; set; }

    }
}
