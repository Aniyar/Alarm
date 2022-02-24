using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALARm.Core;
namespace AlarmPP.Web.Components.Diagram
{
    public partial class VideoObject: ComponentBase
    {
        [Parameter]
        public int Width { get; set; }
        [Parameter]
        public int Height { get; set; }
        int X { get; set; }
        int Y { get; set; }
        [Parameter]
        public ALARm.Core.VideoObject Object { get; set; }
        protected override void OnParametersSet()
        {
            
            if (Object.Threat == Threat.Left)
            {
                X = Object.Y;
                Y = Height - Object.X - Object.W;

            }
            else
            {
                X = Width - Object.Y - Object.H;
                Y = Object.X;
            }

        }
        
    }
}
