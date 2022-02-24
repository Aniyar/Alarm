using Microsoft.AspNetCore.Components;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class RuleGrid : ComponentBase
    {
        [Parameter]
        public int Width { get; set; }
        [Parameter]
        public int Height { get; set; }
        int RuleWidth => 30;
    }
}
