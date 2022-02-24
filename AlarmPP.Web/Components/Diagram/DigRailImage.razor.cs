using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using AlarmPP.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class DigRailImage : ComponentBase
    {
        [Parameter]
        public RailImage RailImage { get; set; }
        
        string MainStyle { get; set; } = "float:left; height:100%;";
        string SvgStyle { get; set; } = "height:calc(100% - (24px));";
        double PreviousClientY { get; set; }
        void ExpandClick()
        {
            
            MainStyle = $"position:absolute; width:55%;top:45px;margin-left:auto;margin-right:auto;left:0px;right:0px";
            SvgStyle = "width:100%;";
        }
        void RestoreClick()
        {
            MainStyle = "float:left; height:100%";
            SvgStyle = "height:calc(100% - (24px));";
        }
        void OnMouseMove(MouseEventArgs args)
        {
           
            if (AppData.RailImage.CurrentGap != null)
            {
                var koef = AppData.RailImage.CurrentGapPosition[1] / AppData.RailImage.Height;
                var CalcY = AppData.RailImage.CurrentGapPosition[3] + AppData.RailImage.CurrentGap.Start * koef;
                if (args.Buttons == 1)
                {
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
                    else if ((AppData.RailImage.SelectedSide == Threat.Right))
                    {
                        int length = Convert.ToInt32((args.ClientY - CalcY) / koef);
                        if (length > 0)
                            AppData.RailImage.CurrentGap.Length = length;
                    }
                    else
                    {

                        AppData.RailImage.CurrentGap.Start = Convert.ToInt32((args.ClientY - AppData.RailImage.CurrentGapPosition[3]) / koef) - AppData.RailImage.CursorPositionFromStart;
                    }
                }
                else
                    AppData.RailImage.CurrentGap = null;
            }
        }
    }
}
