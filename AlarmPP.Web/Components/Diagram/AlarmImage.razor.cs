using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Drawing;

using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class AlarmImage : ComponentBase
    {
        [Parameter]
        public Bitmap Bitmap { get; set; }
        [Parameter]
        public string Image { get; set; }
        private string Base64 { get; set; }
        protected override void OnParametersSet()
        {
            Base64 = Image;
        }

    }
}
