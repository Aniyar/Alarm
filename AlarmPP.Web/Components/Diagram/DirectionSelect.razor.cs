using ALARm.Core;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class DirectionSelect : ComponentBase
    {
        [Parameter]
        public Fragment Fragment { get; set; }
        [Inject]
        public IMainTrackStructureRepository MainTrackStructureRepository { get; set; }
        [CascadingParameter]
        public Route _Parent
        {
            get; set;
        }

        void ChangeDirect(Direction direct) {
            if (Fragment == null)
                return;
            Fragment.Final_Km = Fragment.Start_Km;
            Fragment.Final_M = Fragment.Start_M + (int)direct;
            Fragment.GetNextFragments(MainTrackStructureRepository);
            _Parent.RefreshState();
            
        }
    }
}
