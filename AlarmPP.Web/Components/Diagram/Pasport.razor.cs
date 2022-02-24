using ALARm.Core;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class Pasport : ComponentBase
    {
        [Inject]
        public IMainTrackStructureRepository MainTrackStructureRepository { get; set; }
        [Inject]
        public AlarmPP.Web.Services.AppData AppData { get; set; }
        [Parameter]
        public int Y { get; set; }
        [Parameter]
        public int Width { get; set; }
        [Parameter]
        public Kilometer Kilometer { get; set; }
        private string GetArtificialLinePoints(int threat, int start, int final, int x, int Y)
        {
            
            return (!IsBoundary(start) ? $"{x + (int)threat*10 },{ Y + start - (int)Kilometer.Direction* 5 }" : "") + $" { x + (int)threat * 7 },{ Y + start} {x + (int)threat * 7},{ Y + final} " + (!IsBoundary(final) ? $" {x + (int)threat *10},{Y + final + (int)Kilometer.Direction* 5} " : "");
        }
        private bool IsBoundary(int coordinate)
        {
            return ((coordinate == 0) || (coordinate == Kilometer.Length));
        }
    }
}

