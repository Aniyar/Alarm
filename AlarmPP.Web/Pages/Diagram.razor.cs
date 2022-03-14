using ALARm.Core;
using AlarmPP.Web.Components.Diagram;
using AlarmPP.Web.Services;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AlarmPP.Web.Pages
{

    public partial class Diagram : ComponentBase
    {
        [Inject]
        public IRdStructureRepository RdStructureRepository { get; set; }
        
        [Parameter]
        public MatTheme MainTheme { get; set; } = new MatTheme()
        {
            Primary = MatThemeColors.BlueGrey._500.Value,
            Secondary = MatThemeColors.BlueGrey._500.Value
        };

        private TrackPanel TrackPanel;
        public List<Kilometer> Kilometers { get; set; }
        public MatMenu dropMenu { get; set; }
        public ForwardRef dropMenuRef { get; set; } = new ForwardRef();
        public TripChooserDialog chooserDialog;
        public void OnDropMenuClick(MouseEventArgs e)
        {
            this.dropMenu.OpenAsync();
        }

        public MatMenu printMenu { get; set; }
        public ForwardRef printMenuRef { get; set; } = new ForwardRef();
        public void OnPrintMenuClick(MouseEventArgs e)
        {
            this.printMenu.OpenAsync();
        }

        bool FinishProcessingDialog { get; set ;} = false;
        public MatMenu digFilterMenu { get; set; }
        public ForwardRef digFilterMenuRef { get; set; } = new ForwardRef();
        public void OnDigFilterMenuClick(MouseEventArgs e)
        {
            this.digFilterMenu.OpenAsync();
        }

        public MatMenu digFilterMenu2 { get; set; }
        public ForwardRef digFilterMenuRef2 { get; set; } = new ForwardRef();
        public void OnDigFilterMenuClick2(MouseEventArgs e)
        {
            this.digFilterMenu2.OpenAsync();
        }

        public bool selectRegion { get; set; } = false;
        public bool selectRect { get; set; } = false;
        protected override void OnInitialized()
        {

            AppData.ShowSignals = RdStructureRepository.GetButtonState(ShowButtons.Signal.ToString());
            AppData.ShowEvents = RdStructureRepository.GetButtonState(ShowButtons.Event.ToString());
            AppData.ShowMainParams = RdStructureRepository.GetButtonState(ShowButtons.MainParams.ToString());
            AppData.ShowDigressions = RdStructureRepository.GetButtonState(ShowButtons.Digression.ToString());
            AppData.ShowZeroLines = RdStructureRepository.GetButtonState(ShowButtons.ZeroLines.ToString());
            AppData.ShowPasport = RdStructureRepository.GetButtonState(ShowButtons.Pasport.ToString());
            AppData.Show1DegreeDigressions = RdStructureRepository.GetButtonState(ShowButtons.FirstDegreeDigression.ToString());
            AppData.Show2DegreeDigressions = RdStructureRepository.GetButtonState(ShowButtons.SecondDegreeDigression.ToString());
            AppData.Show3DegreeDigressions = RdStructureRepository.GetButtonState(ShowButtons.ThirdDegreeDigressions.ToString());
            AppData.ShowCloseToDangerous = RdStructureRepository.GetButtonState(ShowButtons.CloseToDangerous.ToString());
            AppData.ShowGapsCloseToDangerous = RdStructureRepository.GetButtonState(ShowButtons.GapCloseToDangerous.ToString());
            AppData.ShowDangerousForEmtyWagon = RdStructureRepository.GetButtonState(ShowButtons.DangerousForEmtyWagon.ToString());
            AppData.ShowOthersDigressions = RdStructureRepository.GetButtonState(ShowButtons.OthersDigressions.ToString());
            AppData.ShowDangerousDigressions = RdStructureRepository.GetButtonState(ShowButtons.DangerousDigression.ToString());
            AppData.ShowJoints = RdStructureRepository.GetButtonState(ShowButtons.Joints.ToString());
            AppData.ShowRailProfile = RdStructureRepository.GetButtonState(ShowButtons.RailProfile.ToString());
            AppData.ShowGaps = RdStructureRepository.GetButtonState(ShowButtons.Gaps.ToString());
            AppData.ShowBolts = RdStructureRepository.GetButtonState(ShowButtons.Bolts.ToString());
            AppData.ShowFasteners = RdStructureRepository.GetButtonState(ShowButtons.Fasteners.ToString());
            AppData.ShowPerShpals = RdStructureRepository.GetButtonState(ShowButtons.PerShpals.ToString());
            AppData.ShowDefShpals = RdStructureRepository.GetButtonState(ShowButtons.DefShpals.ToString());
            if (AppData.Show1DegreeDigressions || AppData.Show2DegreeDigressions || AppData.ShowGapsCloseToDangerous ||
                AppData.Show3DegreeDigressions || AppData.ShowCloseToDangerous || AppData.ShowDangerousDigressions ||
                AppData.ShowDangerousForEmtyWagon || AppData.ShowOthersDigressions || AppData.ShowGaps || AppData.ShowBolts ||
                AppData.ShowFasteners || AppData.ShowPerShpals || AppData.ShowDefShpals)
            {
                AppData.DigressionChecked = true;
            }
            // additional
            AppData.ShowPU = RdStructureRepository.GetButtonState(ShowButtons.PU.ToString());
            AppData.ShowNPK = RdStructureRepository.GetButtonState(ShowButtons.NPK.ToString());
            AppData.ShowIznosPriv = RdStructureRepository.GetButtonState(ShowButtons.IznosPriv.ToString());
            AppData.ShowIznosBok = RdStructureRepository.GetButtonState(ShowButtons.IznosBok.ToString());
            AppData.ShowIznosVert = RdStructureRepository.GetButtonState(ShowButtons.IznosVert.ToString());
            AppData.ShowLongWaves = RdStructureRepository.GetButtonState(ShowButtons.LongWaves.ToString());
            AppData.ShowMediumWaves = RdStructureRepository.GetButtonState(ShowButtons.MediumWaves.ToString());
            AppData.ShowShortWaves = RdStructureRepository.GetButtonState(ShowButtons.ShortWaves.ToString());
            if (AppData.ShowPU || AppData.ShowNPK || AppData.ShowIznosPriv ||
                AppData.ShowIznosVert || AppData.ShowLongWaves || AppData.ShowMediumWaves ||
                AppData.ShowShortWaves)
            {
                AppData.AddDigressionChecked = true;
            }
            //bykilometer
            AppData.ByKilometerChecked = RdStructureRepository.GetButtonState(ShowButtons.ByKilometer.ToString());

        }
        private void Refresh()
        {
            StateHasChanged();
            TrackPanel.Refresh();
        }

        public void SetShowStatus(ShowButtons buttonName)
        {
            switch (buttonName)
            {
                case ShowButtons.Correction:
                    AppData.ShowCorrection = !AppData.ShowCorrection;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Correction.ToString(), AppData.ShowCorrection);
                    break;
                case ShowButtons.ByKilometer:
                    AppData.ByKilometerChecked = !AppData.ByKilometerChecked;
                    AppData.ShowDangerousDigressions = false;
                    AppData.ShowCloseToDangerous = false;
                    AppData.Show3DegreeDigressions = false;
                    AppData.Show2DegreeDigressions = false;
                    AppData.Show1DegreeDigressions = false;
                    AppData.ShowGaps = false;
                    AppData.ShowGapsCloseToDangerous = false;
                    AppData.ShowBolts = false;
                    AppData.ShowFasteners = false;
                    AppData.ShowDefShpals = false;
                    AppData.ShowPerShpals = false;
                    RdStructureRepository.SetButtonStatus(ShowButtons.ByKilometer.ToString(), AppData.ByKilometerChecked);
                    if (AppData.ByKilometerChecked)
                    {
                        AppData.DigressionChecked = false;
                        AppData.ShowDigressions = false;
                    }
                    else
                    {
                        AppData.ByKilometerChecked = false;
                    }
                    break;
                case ShowButtons.Signal:
                    AppData.ShowSignals = !AppData.ShowSignals;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Signal.ToString(), AppData.ShowSignals);
                    break;
                case ShowButtons.Event:
                    AppData.ShowEvents = !AppData.ShowEvents;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Event.ToString(), AppData.ShowEvents);
                    break;
                case ShowButtons.ZeroLines:
                    AppData.ShowZeroLines = !AppData.ShowZeroLines;
                    RdStructureRepository.SetButtonStatus(ShowButtons.ZeroLines.ToString(), AppData.ShowZeroLines);
                    break;
                case ShowButtons.MainParams:
                    AppData.ShowMainParams = !AppData.ShowMainParams;
                    RdStructureRepository.SetButtonStatus(ShowButtons.MainParams.ToString(), AppData.ShowMainParams);
                    break;
                case ShowButtons.Pasport:
                    AppData.ShowPasport = !AppData.ShowPasport;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Pasport.ToString(), AppData.ShowPasport);
                    break; ;
                case ShowButtons.Digression:
                    AppData.ShowDigressions = !AppData.ShowDigressions;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Digression.ToString(), AppData.ShowDigressions);
                    break;
                case ShowButtons.DangerousDigression:
                    AppData.ShowDangerousDigressions = !AppData.ShowDangerousDigressions;
                    RdStructureRepository.SetButtonStatus(ShowButtons.DangerousDigression.ToString(), AppData.ShowDangerousDigressions);
                    AppData.ShowGaps = false;
                    AppData.ShowGapsCloseToDangerous = false;
                    AppData.ShowBolts = false;
                    AppData.ShowFasteners = false;
                    AppData.ShowDefShpals = false;
                    AppData.ShowPerShpals = false;
                    if (AppData.ShowDangerousDigressions) {
                        AppData.DigressionChecked = true;
                        AppData.ShowDigressions = true;
                    }
                    else
                    {
                        AppData.DigressionChecked = false;
                    }
                        
                    break;
                case ShowButtons.DangerousForEmtyWagon:
                    AppData.ShowDangerousForEmtyWagon = !AppData.ShowDangerousForEmtyWagon;
                    RdStructureRepository.SetButtonStatus(ShowButtons.DangerousForEmtyWagon.ToString(), AppData.ShowDangerousForEmtyWagon);
                    if (AppData.ShowDangerousForEmtyWagon)
                        AppData.DigressionChecked = true;
                    else
                        AppData.DigressionChecked = false;
                    break;
                case ShowButtons.ThirdDegreeDigressions:
                    AppData.Show3DegreeDigressions = !AppData.Show3DegreeDigressions;
                    AppData.ShowGaps = false;
                    AppData.ShowGapsCloseToDangerous = false;
                    AppData.ShowBolts = false;
                    AppData.ShowFasteners = false;
                    AppData.ShowDefShpals = false;
                    AppData.ShowPerShpals = false;
                    RdStructureRepository.SetButtonStatus(ShowButtons.ThirdDegreeDigressions.ToString(), AppData.Show3DegreeDigressions);
                    if (AppData.Show3DegreeDigressions)
                    {
                        AppData.DigressionChecked = true;
                        AppData.ShowDigressions = true;
                    }
                    else
                    {
                        AppData.DigressionChecked = false;
                    }
                    break;
                case ShowButtons.CloseToDangerous:
                    AppData.ShowCloseToDangerous = !AppData.ShowCloseToDangerous;
                    RdStructureRepository.SetButtonStatus(ShowButtons.CloseToDangerous.ToString(), AppData.ShowCloseToDangerous);
                    AppData.ShowGaps = false;
                    AppData.ShowGapsCloseToDangerous = false;
                    AppData.ShowBolts = false;
                    AppData.ShowFasteners = false;
                    AppData.ShowDefShpals = false;
                    AppData.ShowPerShpals = false;
                    if (AppData.ShowCloseToDangerous)
                    {
                        
                        AppData.DigressionChecked = true;
                    }
                    else
                    {
                        AppData.DigressionChecked = false;
                    }
                       
                    break;
                case ShowButtons.GapCloseToDangerous:

                    AppData.ShowGapsCloseToDangerous = !AppData.ShowGapsCloseToDangerous;
                    RdStructureRepository.SetButtonStatus(ShowButtons.GapCloseToDangerous.ToString(), AppData.ShowGapsCloseToDangerous);
                    AppData.ShowDangerousDigressions = false;
                    AppData.ShowCloseToDangerous = false;
                    AppData.Show3DegreeDigressions = false;
                    AppData.Show2DegreeDigressions = false;
                    AppData.Show1DegreeDigressions = false;
                    AppData.ShowBolts = false;
                    AppData.ShowFasteners = false;
                    AppData.ShowDefShpals = false;
                    AppData.ShowPerShpals = false;
                    if (AppData.ShowGapsCloseToDangerous)
                    {
                        
                        AppData.DigressionChecked = true;
                    }
                    else
                    {
                        AppData.DigressionChecked = false;
                    }
                        
                    break;
                case ShowButtons.FirstDegreeDigression:
                    AppData.Show1DegreeDigressions = !AppData.Show1DegreeDigressions;
                    RdStructureRepository.SetButtonStatus(ShowButtons.FirstDegreeDigression.ToString(), AppData.Show1DegreeDigressions);
                    AppData.ShowGaps = false;
                    AppData.ShowBolts = false;
                    AppData.ShowFasteners = false;
                    AppData.ShowDefShpals = false;
                    AppData.ShowPerShpals = false;
                    AppData.ShowGapsCloseToDangerous = false;
                    if (AppData.Show1DegreeDigressions)
                    {
                        AppData.DigressionChecked = true;
                        AppData.ShowDigressions = true;

                    }
                    else
                    {
                        AppData.DigressionChecked = false;
                    }
                        
                    break;
                case ShowButtons.SecondDegreeDigression:
                    AppData.Show2DegreeDigressions = !AppData.Show2DegreeDigressions;
                    RdStructureRepository.SetButtonStatus(ShowButtons.SecondDegreeDigression.ToString(), AppData.Show2DegreeDigressions);
                    AppData.ShowGaps = false;
                    AppData.ShowBolts = false;
                    AppData.ShowFasteners = false;
                    AppData.ShowDefShpals = false;
                    AppData.ShowPerShpals = false;
                    AppData.ShowGapsCloseToDangerous = false;
                    if (AppData.Show2DegreeDigressions)
                    {
                        AppData.DigressionChecked = true;
                        AppData.ShowDigressions = true;
                    }
                    else
                    {
                        AppData.DigressionChecked = false;
                    }
                    break;

                case ShowButtons.OthersDigressions:
                    AppData.ShowOthersDigressions = !AppData.ShowOthersDigressions;
                    RdStructureRepository.SetButtonStatus(ShowButtons.OthersDigressions.ToString(), AppData.ShowOthersDigressions);
                    if (AppData.ShowOthersDigressions)
                        AppData.DigressionChecked = true;
                    else
                        AppData.DigressionChecked = false;
                    break;
                case ShowButtons.ExcludedByOerator:
                    AppData.ShowExcludedByOerator = !AppData.ShowExcludedByOerator;
                    RdStructureRepository.SetButtonStatus(ShowButtons.ExcludedByOerator.ToString(), AppData.ShowExcludedByOerator);
                    if (AppData.ShowExcludedByOerator)
                        AppData.DigressionChecked = true;
                    else
                        AppData.DigressionChecked = false;
                    break;
                case ShowButtons.ExcludedOnSwitch:
                    AppData.ShowExcludedOnSwitch = !AppData.ShowExcludedOnSwitch;
                    RdStructureRepository.SetButtonStatus(ShowButtons.ExcludedOnSwitch.ToString(), AppData.ShowExcludedOnSwitch);
                    if (AppData.ShowExcludedOnSwitch)
                        AppData.DigressionChecked = true;
                    else
                        AppData.DigressionChecked = false;
                    break;
                case ShowButtons.NotTakenOnRating:
                    AppData.ShowNotTakenOnRating = !AppData.ShowNotTakenOnRating;
                    RdStructureRepository.SetButtonStatus(ShowButtons.NotTakenOnRating.ToString(), AppData.ShowNotTakenOnRating);
                    if (AppData.ShowNotTakenOnRating)
                        AppData.DigressionChecked = true;
                    else
                        AppData.DigressionChecked = false;
                    break;
                case ShowButtons.Joints:
                    AppData.ShowJoints = !AppData.ShowJoints;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Joints.ToString(), AppData.ShowJoints);
                    break;
                case ShowButtons.RailProfile:
                    AppData.ShowRailProfile = !AppData.ShowRailProfile;
                    RdStructureRepository.SetButtonStatus(ShowButtons.RailProfile.ToString(), AppData.ShowRailProfile);
                    break;
                case ShowButtons.Gaps:
                    AppData.ShowGaps = !AppData.ShowGaps;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Gaps.ToString(), AppData.ShowGaps);
                    AppData.ShowDangerousDigressions = false;
                    AppData.ShowCloseToDangerous = false;
                    AppData.Show3DegreeDigressions = false;
                    AppData.Show2DegreeDigressions = false;
                    AppData.Show1DegreeDigressions = false;
                    AppData.ShowBolts = false;
                    AppData.ShowFasteners = false;
                    AppData.ShowDefShpals = false;
                    AppData.ShowPerShpals = false;
                    if (AppData.ShowGaps)
                    {
                        AppData.DigressionChecked = true;
                    }
                    else
                    {
                        AppData.DigressionChecked = false;
                    }
                    break;
                case ShowButtons.Bolts:
                    AppData.ShowBolts = !AppData.ShowBolts;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Bolts.ToString(), AppData.ShowBolts);
                    AppData.ShowDangerousDigressions = false;
                    AppData.ShowCloseToDangerous = false;
                    AppData.Show3DegreeDigressions = false;
                    AppData.Show2DegreeDigressions = false;
                    AppData.Show1DegreeDigressions = false;
                    AppData.ShowGaps = false;
                    AppData.ShowGapsCloseToDangerous = false;
                    AppData.ShowFasteners = false;
                    AppData.ShowDefShpals = false;
                    AppData.ShowPerShpals = false;
                    if (AppData.ShowBolts)
                    {
                        AppData.DigressionChecked = true;
                    }
                    else
                    {
                        AppData.DigressionChecked = false;
                    }
                    break;
                case ShowButtons.Fasteners:
                    AppData.ShowFasteners = !AppData.ShowFasteners;
                    RdStructureRepository.SetButtonStatus(ShowButtons.Fasteners.ToString(), AppData.ShowFasteners);
                    AppData.ShowDangerousDigressions = false;
                    AppData.ShowCloseToDangerous = false;
                    AppData.Show3DegreeDigressions = false;
                    AppData.Show2DegreeDigressions = false;
                    AppData.Show1DegreeDigressions = false;
                    AppData.ShowGaps = false;
                    AppData.ShowGapsCloseToDangerous = false;
                    AppData.ShowBolts = false;
                    AppData.ShowDefShpals = false;
                    AppData.ShowPerShpals = false;
                    if (AppData.ShowFasteners)
                    {
                        AppData.DigressionChecked = true;
                    }
                    else
                    {
                        AppData.DigressionChecked = false;
                    }
                    break;
                case ShowButtons.PerShpals:
                    AppData.ShowPerShpals = !AppData.ShowPerShpals;
                    RdStructureRepository.SetButtonStatus(ShowButtons.PerShpals.ToString(), AppData.ShowPerShpals);
                    AppData.ShowDangerousDigressions = false;
                    AppData.ShowCloseToDangerous = false;
                    AppData.Show3DegreeDigressions = false;
                    AppData.Show2DegreeDigressions = false;
                    AppData.Show1DegreeDigressions = false;
                    AppData.ShowGaps = false;
                    AppData.ShowGapsCloseToDangerous = false;
                    AppData.ShowFasteners = false;
                    AppData.ShowDefShpals = false;
                    AppData.ShowBolts = false;
                    if (AppData.ShowPerShpals)
                    {
                        AppData.DigressionChecked = true;
                        
                    }
                    else
                    {
                        AppData.DigressionChecked = false;
                    }
                    break;
                case ShowButtons.DefShpals:
                    AppData.ShowDefShpals = !AppData.ShowDefShpals;
                    RdStructureRepository.SetButtonStatus(ShowButtons.DefShpals.ToString(), AppData.ShowDefShpals);
                    AppData.ShowDangerousDigressions = false;
                    AppData.ShowCloseToDangerous = false;
                    AppData.Show3DegreeDigressions = false;
                    AppData.Show2DegreeDigressions = false;
                    AppData.Show1DegreeDigressions = false;
                    AppData.ShowGaps = false;
                    AppData.ShowGapsCloseToDangerous = false;
                    AppData.ShowFasteners = false;
                    AppData.ShowBolts = false;
                    AppData.ShowPerShpals = false;
                    if (AppData.ShowDefShpals)
                    {
                        AppData.DigressionChecked = true;
                        
                    }
                    else
                    {
                        AppData.DigressionChecked = false;
                    }
                    break;
                case ShowButtons.PU:
                    AppData.ShowPU = !AppData.ShowPU;
                    if (AppData.ShowPU)
                        AppData.AddDigressionChecked = true;
                    else
                        AppData.AddDigressionChecked = false;
                    break;
                case ShowButtons.NPK:
                    AppData.ShowNPK = !AppData.ShowNPK;
                    RdStructureRepository.SetButtonStatus(ShowButtons.NPK.ToString(), AppData.ShowNPK);
                    if (AppData.ShowNPK)
                        AppData.AddDigressionChecked = true;
                    else
                        AppData.AddDigressionChecked = false;
                    break;
                case ShowButtons.LongWaves:
                    AppData.ShowLongWaves = !AppData.ShowLongWaves;
                    if (AppData.ShowLongWaves)
                        AppData.AddDigressionChecked = true;
                    else
                        AppData.AddDigressionChecked = false;
                    break;
                case ShowButtons.MediumWaves:
                    AppData.ShowMediumWaves = !AppData.ShowMediumWaves;
                    if (AppData.ShowMediumWaves)
                        AppData.AddDigressionChecked = true;
                    else
                        AppData.AddDigressionChecked = false;
                    break;
                case ShowButtons.ShortWaves:
                    AppData.ShowShortWaves = !AppData.ShowShortWaves;
                    if (AppData.ShowShortWaves)
                        AppData.AddDigressionChecked = true;
                    else
                        AppData.AddDigressionChecked = false;
                    break;
                case ShowButtons.IznosBok:
                    AppData.ShowIznosBok = !AppData.ShowIznosBok;
                    if (AppData.ShowIznosBok)
                        AppData.AddDigressionChecked = true;
                    else
                        AppData.AddDigressionChecked = false;
                    break;
                case ShowButtons.IznosPriv:
                    AppData.ShowIznosPriv = !AppData.ShowIznosPriv;
                    if (AppData.ShowIznosPriv)
                        AppData.AddDigressionChecked = true;
                    else
                        AppData.AddDigressionChecked = false;
                    break;
                case ShowButtons.IznosVert:
                    AppData.ShowIznosVert = !AppData.ShowIznosVert;
                    if (AppData.ShowIznosVert)
                        AppData.AddDigressionChecked = true;
                    else
                        AppData.AddDigressionChecked = false;
                    break;
            }

            Refresh();
        }

        public void FinishProcessing()
        {
            try
            {
                if (AppData.RdStructureRepository.FinishProcessing(AppData.Trip.Id) > 0)
                {
                    //Toaster.Add($"Постобработка успешно завершена", MatBlazor.MatToastType.Success);
                    FinishProcessingDialog = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Не уадлость завершить редактирование из за ошибки: " + e.Message);
            }

        }
        public async Task OpenDialog()
        {
            AppData.IsDialogOpen = true;
            StateHasChanged();
        }

        public async Task PrintCurrentKm()
        {
            await TrackPanel.PrintCurrentKm();
        }

        public async Task PrintRegion()
        {
            await TrackPanel.PrintRegion();
        }
    }
}
