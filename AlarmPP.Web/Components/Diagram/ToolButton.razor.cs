using ALARm.Core;
using AlarmPP.Web.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class ToolButton : ComponentBase
    {
        [Inject]
        public IRdStructureRepository RdStructureRepository { get; set; }
        [Parameter]
        public bool Pressed { get; set; }
        [Parameter]
        public string Tooltip { get; set; }
        [Parameter]
        public ShowButtons ShowButton { get; set; }
        [Parameter]
        public string IconClass { get; set; }
        [Parameter]
        public string IconCode { get; set; }
        public void SetShowStatus(ShowButtons buttonName)
        {
            
            switch (buttonName)
            {
                case ShowButtons.Signal:
                    AppData.ShowSignals = !AppData.ShowSignals;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Signal.ToString(), AppData.ShowSignals);
                    return;
                case ShowButtons.Event:
                    AppData.ShowEvents = !AppData.ShowEvents;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Event.ToString(), AppData.ShowEvents);
                    return;
                case ShowButtons.Digression:
                    AppData.ShowDigressions = !AppData.ShowDigressions;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Digression.ToString(), AppData.ShowDigressions);
                    return;
                case ShowButtons.ZeroLines:
                    AppData.ShowZeroLines = !AppData.ShowZeroLines;
                    RdStructureRepository.SetButtonStatus(ShowButtons.ZeroLines.ToString(), AppData.ShowZeroLines);
                    return;
                case ShowButtons.Pasport:
                    AppData.ShowPasport = !AppData.ShowPasport;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Pasport.ToString(), AppData.ShowPasport);
                    return;
                case ShowButtons.Video:
                    AppData.ShowVideo = !AppData.ShowVideo;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Video.ToString(), AppData.ShowVideo);
                    StateHasChanged();
                    return;

            }
            //await ProtectedSessionStore.SetAsync(statusComponentName, status);
        }
    }
}
