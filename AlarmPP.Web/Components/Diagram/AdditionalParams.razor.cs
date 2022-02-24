using ALARm.Core;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlarmPP.Web.Services;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class AdditionalParams : ComponentBase
    {
        [Parameter]
        public List<Kilometer> kilometers { get; set; }

    }

}
