using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using AlarmPP.Web.Services;
using BlazorContextMenu;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace AlarmPP.Web.Components.Diagram
{
    public partial class ByKilometer : ComponentBase
    {

        [Parameter]
        public List<Kilometer> Kilometers { get; set; }
        [Parameter]
        public  List<Kilometer> BedKilometers { get; set; }
        public int CurrentRow { get; set; } = 0;

        public async Task GoToMark(int yposition, int rowIndex)
        {
            CurrentRow = rowIndex;
            AppData.SliderYPosition = yposition;
            AppData.SliderXPosition = Math.Round(AppData.SliderYPosition / 10);
            AppData.SliderCenterXPosition = AppData.SliderXPosition + 25;
            object[] paramss = new object[] { AppData.SliderYPosition - 200 };
            await JSRuntime.InvokeVoidAsync("ScrollMainSvg", paramss);
        }
    }

}
