using ALARm.Core;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class Route : ComponentBase
    {
        [Parameter]
        public List<Fragment> Source { get; set; }
       
        [Parameter]
        public List<long> CommonTracks { get; set; }
        [Parameter]
        public List<StationSection> Stations { get; set; }
        [Parameter]
        public bool Short { get; set; } = false;
        [Parameter]
        public bool Editable { get; set; } = true;
        [Parameter]
        public double CurrentCoord { get; set; } = 0.0;
        Fragment CurrentFragment { get; set; }
        bool editDialog = false,   deleteDialog = false;
        [Inject] 
        IMainTrackStructureRepository MainTrackStructureRepository { get; set; }
        [Inject]
        AlarmPP.Web.Services.AppData AppData { get; set; }
        void GetNextTracks(Fragment fragment)
        {
            
            if (fragment.NextFragment == null)
            {
                Toaster.Add("Выберите конечную координату фрагмента", MatToastType.Warning, "Построение маршрута");
                return;
            }
            fragment.NextFragment.AdmTracks = MainTrackStructureRepository.GetSwitchTracks(fragment.NextFragment.Start_Switch_Id);
            var start_switch = MainTrackStructureRepository.GetTrackSwitch(fragment.NextFragment.Track_Id, fragment.NextFragment.Start_Switch_Id);
            fragment.NextFragment.Start_Km = start_switch.Km;
            fragment.NextFragment.Start_M = start_switch.Meter + (int)fragment.Direction ;


            fragment.NextFragment.Start_Switch_Id = start_switch.Id;
            AppData.Trip.Route.Add(fragment.NextFragment);
            //        result.Start_Switch_Id = startSwitch.Id;
            //var result = new Fragment();
            //for (int i = 0; i < switches.Count - 1; i++)
            //{
            //    if (switches[i].RealCoordinate < RealStartCoordinate && RealStartCoordinate < switches[i + 1].RealCoordinate)
            //    {
            //        var startSwitch = travel_Direction == Direction.Direct ? switches[i + 1] : switches[i];
            //        // var finalSwitch = travel_Direction == Direction.Direct ? switches[i] : switches[i + 1];
            //        result.Start_Km = startSwitch.Km;
            //        result.Start_M = startSwitch.Meter + (int)travel_Direction * 1;
            //        result.AdmTracks = mainTrackStructureRepository.GetSwitchTracks(startSwitch.Id);
            //        result.Start_Switch_Id = startSwitch.Id;

            //    }
            //}
            //return result;

            //fragment.NextFragment.Final_Km = 0;
            //fragment.NextFragment.Final_M = 0;
            //fragment.NextFragment.GetNextFragments(MainTrackStructureRepository);
            //var switches = MainTrackStructureRepository.GetFragmentsSwitch(fragment.NextFragment.Track_Id, Direction.Direct, fragment.NextFragment.RealStartCoordinate);

            //foreach (var sw in switches)
            //{
            //    //var fragment = new Fragment { Start_Km = sw.Km, Start_M = sw.Meter };
            //    fragment.NextFragment.AdmTracks = MainTrackStructureRepository.GetSwitchTracks(sw.Id);
            //    //NextFragments.Add(fragment);


            //}
            //AppData.Trip.Route.Add(fragment.NextFragment);

            //foreach (var fr in AppData.Trip.Route)
            //{
            //    fr.Editable = false;
            //}
            //if (TravelDirection == (int)Direction.NotDefined) {
            //    Toaster.Add("Укажите счет километра", MatToastType.Danger, "Построение маршрута");
            //    return;
            //}
            //if (fragment.Track_Id < 0)
            //{
            //    Toaster.Add("Выберите путь", MatToastType.Danger, "Построение маршрута");
            //    return;
            //}
            //var nextFragment = fragment.GetNextFrgament(MainTrackStructureRepository, fragment.Direction);
            //if (nextFragment != null)
            //{
            //   // AppData.Trip.Route[AppData.Trip.Route.Count - 1].Final_Km = nextFragment.Start_Km;
            //   // AppData.Trip.Route[AppData.Trip.Route.Count - 1].Final_M = nextFragment.Start_M;
            //    AppData.Trip.Route[AppData.Trip.Route.Count - 1].Final_Switch_Id = nextFragment.Start_Switch_Id;
            //    AppData.Trip.Route.Add(nextFragment);
            //}
        }
        void DeleteFrom(Fragment fragment,bool include)
        {
            if (fragment == null)
                return;
            var index = AppData.Trip.Route.IndexOf(fragment);
            if (include) index--;
            for (int i = AppData.Trip.Route.Count - 1; i > index; i--)
            {
                AppData.Trip.Route.RemoveAt(i);
            }
            deleteDialog = false;
        }
        void TrackChanged(ChangeEventArgs e, Fragment fragment)
        {
            fragment.Track_Id = long.Parse(e.Value.ToString());
            

            DeleteFrom(fragment, false);
            editDialog = false;
            fragment.GetNextFragments(MainTrackStructureRepository);
            
            if ((fragment.Start_Switch_Id < 0))
                return;
            
                var start_switch = MainTrackStructureRepository.GetTrackSwitch(fragment.Track_Id, fragment.Start_Switch_Id);
                fragment.Start_Km = start_switch.Km;
                fragment.Start_M = start_switch.Meter + (int)fragment.Direction;


                fragment.Start_Switch_Id = start_switch.Id;
            
        }
        void SelectFragmentFinal(ChangeEventArgs e, Fragment fragment)
        {
            try
            {
                
                var args = e.Value.ToString().Split("-");
                fragment.Final_Km = int.Parse(args[1]);
                fragment.Final_M = int.Parse(args[2]);
                var selectedFragment = fragment.NextFragments[int.Parse(args[3])];

                fragment.NextFragment = new Fragment() { 
                    Track_Id = int.Parse(args[0]), 
                    Start_Km = fragment.Final_Km, 
                    Start_M = fragment.Final_M,
                    Start_Switch_Id = selectedFragment.Start_Switch_Id
                };
                
             
            }
            catch (Exception ex)
            {
                Console.WriteLine("Route.SelectFragmentFinal.Error: " + ex.Message);
                Toaster.Add("Ошибка при построении маршрута", MatToastType.Danger, "Построение маршрута");
            }
        }
        void DoFragmentEditable(Fragment fragment)
        {
            CurrentFragment = fragment;
            CurrentFragment.Editable = true;
            editDialog = true;
        }
        void ToFinalStation(Fragment fragment)
        {
            var finalStation = Stations.Where(station => station.Station_Id == AppData.Trip.Final_station).First();
            
            fragment.Final_Km = fragment.Direction == Direction.Direct ? finalStation.Start_Km : finalStation.Final_Km;
            fragment.Final_M =  fragment.Direction == Direction.Direct ? finalStation.Start_M : finalStation.Final_M;
            fragment.Belong_Id = finalStation.Id;
        }
        protected override void OnInitialized()
        {
            if ((AppData.Trip.Start_station > -1) && (AppData.Trip.Final_station > -1))
            {
                CommonTracks = MainTrackStructureRepository.GetCommomTracks(AppData.Trip.Start_station, AppData.Trip.Final_station);
            }
        }
        public void RefreshState()
        {
            this.StateHasChanged();
        }

    }   
  
}
