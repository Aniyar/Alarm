using Microsoft.AspNetCore.Components;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class AlarmButton : ComponentBase
    {
        [Parameter]
        public string Symbol { get; set; }
        [Parameter]
        public string Tooltip { get; set; }
        [Parameter]
        public string Text { get; set; } = "";
        [Parameter]
        public EventCallback OnClick { get; set; }
        [Parameter]
        public string Style { get; set; } = "";
    }
}
