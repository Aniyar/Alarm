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
    }

}
