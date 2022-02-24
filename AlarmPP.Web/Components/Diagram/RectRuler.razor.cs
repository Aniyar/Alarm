using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;


namespace AlarmPP.Web.Components.Diagram
{
    public partial class RectRuler : ComponentBase
    {
        [Parameter]
        public RailImage RailImage { get; set; }
        [Inject] AlarmPP.Web.Services.AppData AppData { get; set; }
        [Parameter]
        public int X { get; set; }
        [Parameter]
        public int Width { get; set; }
        [Parameter]
        public Gap Gap { get; set; }
        public string CursorStyle { get; set; } = "cursor:move";
        public string Position { get; set; } = "0";
        double CalcY { get; set; } = -1;
        public string Text { get; set; } = "0";
        double PreviousY { get; set; } = -1;
        int step => 1;
        public int RuleHeaderHeight => 16;
        public int RuleHeaderWidth => 20;
        public int XPosition { get; set; }
        public int LabelY { get; set; }
        int positionFromStart { get; set; }


        async Task OnMouseMove(MouseEventArgs args)
        {
            object[] paramss = new object[] { "svgrailimage" };
            var position = await JSRuntime.InvokeAsync<double[]>("GetElementPosition", paramss);
            var koef = position[1] / RailImage.Height;
            Position = args.ClientY.ToString();
            CalcY = position[3]  +  Gap.Start * koef;
            if (args.Buttons != 1)
            {
                if ((args.ClientY - CalcY <= 2) || (CalcY + Gap.Length * koef - args.ClientY <= 2))
                {
                    CursorStyle = "cursor:row-resize";
                    AppData.RailImage.SelectedSide = (Math.Abs(args.ClientY - CalcY) <= 2) ? Threat.Left : Threat.Right;
                  
                }
                else

                {
                    AppData.RailImage.SelectedSide = Threat.Middle;
                    CursorStyle = "cursor:move";
                    AppData.RailImage.CursorPositionFromStart = Convert.ToInt32((args.ClientY - CalcY) / koef);
                }
            }
            if (args.Buttons == 1)
            {
                AppData.RailImage.CurrentGap = Gap;
                AppData.RailImage.CurrentGapPosition = position;
                if (AppData.RailImage.SelectedSide == Threat.Left)
                {

                    int prevStart = AppData.RailImage.CurrentGap.Start;
                    int currentStart = Convert.ToInt32((args.ClientY - AppData.RailImage.CurrentGapPosition[3]) / koef);
                    if (currentStart < prevStart + AppData.RailImage.CurrentGap.Length)
                    {
                        AppData.RailImage.CurrentGap.Start = currentStart;
                        AppData.RailImage.CurrentGap.Length = AppData.RailImage.CurrentGap.Length + (prevStart - AppData.RailImage.CurrentGap.Start);
                    }
                }
                else if (AppData.RailImage.SelectedSide == Threat.Right)
                {
                    int length = Convert.ToInt32((args.ClientY - CalcY) / koef);
                    if (length > 0)
                        AppData.RailImage.CurrentGap.Length = length;
                }
                else
                {

                    AppData.RailImage.CurrentGap.Start = Convert.ToInt32((args.ClientY - AppData.RailImage.CurrentGapPosition[3]) / koef) - AppData.RailImage.CursorPositionFromStart;
                }

                LabelY = Gap.Length > RuleHeaderHeight ? Gap.Start + (Gap.Length - RuleHeaderHeight) / 2 : Gap.Start - (RuleHeaderHeight - Gap.Length) / 2;
            }
            else
            {
                AppData.RailImage.CurrentGap = null;
            }
        }
        protected override void OnParametersSet()
        {
            LabelY = Gap.Length > RuleHeaderHeight ? Gap.Start + (Gap.Length - RuleHeaderHeight) / 2 : Gap.Start - (RuleHeaderHeight - Gap.Length) / 2;
            if (Gap.Threat == Threat.Right)
            {
                X = Width/2;
                XPosition = Width * 2 - 20;
            }
        }
    }
}
