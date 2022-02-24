using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlarmPP.Web.Components.Diagram;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ALARm.Core;
using MatBlazor;
using ALARm.Core.Report;
using AlarmPP.Web.Services;
using RabbitMQ.Client;
using System.Text;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class TripChooserDialog : ComponentBase
    {
        [Inject]
        public IAdmStructureRepository AdmStructureRepository { get; set; }
        [Inject]
        public IRdStructureRepository RdStructureRepository { get; set; }
        [Inject]
        public IMainTrackStructureRepository MainTrackStructureRepository { get; set; }
        [Inject]
        public IAdditionalParametersRepository AdditionalParametersRepository { get; set; }
        [Inject] 
        public OnlineModeData OnlineModeData { get; set; }

        [Parameter]
        public List<Kilometer> Kilometers { get; set; }
        [Parameter]
        public EventCallback OnlineModeStarted { get; set; }
        public long CurrentRoad { get; set; } = -1;

        [Parameter]
        public MatTheme Theme { get; set; }
      
        private long CurrentTrip { get; set; } = -1;
        private long FinalMainTrackId { get; set; } = -1;
        private List<StationSection> AdmStations { get; set; }
      
        private int TravelDirection { get; set; } = 0;
        private int CarPosition { get; set; } = 0;
        private double Coordinate { get; set; } = 0.0;
        private List<AdmTrack> AdmTracks { get; set; } = null;
        private List<StationTrack> StartStationTracks { get; set; } = null;
        private List<StationTrack> FinalStationTracks { get; set; } = null;
        
        public int CurrentTripType { get; set; } = -1;
        public bool OkButtonDisabled { get; set; } = true;
        public List<ReportPeriod> ReportPeriods { get; set; }
        public string CurrentPeriod { get; set; } = "";
        private List<Trips> ControlTrips { get; set; } = null;
        private List<Trips> WorkTrips { get; set; } = null;

        private bool IsHavingActiveTrip = false;
        private bool OnlineModeActive = false;
        private List<Trips> AdditionalTrips { get; set; } = null;
        public int ActiveTabIndex = 1;
        private List<long> commonTracks { get; set; }
        private bool Loading { get; set; } = false;
        public void MouseOver(MouseEventArgs args)
        {
            OnlineModeActive = true;
        }
        public void MouseOut(MouseEventArgs args)
        {
            OnlineModeActive = false;
        }
        public void ActiveChanged()
        {
            if (OnlineModeActive)
            {
                AppData.Trip = RdStructureRepository.GetCurrentTrip();
                IsHavingActiveTrip = AppData.Trip != null;
                if (AppData.Trip == null)
                    AppData.Trip = new Trips();
            }
        }
        private string status = "empty";
        bool startDialog = false;

        
        void OkClick()
        {
            AppData.WorkMode = WorkMode.Postprocessing;
            JSRuntime.InvokeVoidAsync("loader", true);
            AppData.LoadingText = "Идет получение детальных данных по километрам";
            Loading = false;
            StateHasChanged();
            _ = SetLabelById("status-label", "Идет получение детальных данных по километрам");

            AppData.Trip.Route = RdStructureRepository.GetTripFragments(AppData.Trip.Id);
            AppData.MainTrackStructureRepository = MainTrackStructureRepository;
            AppData.AdditionalParametersRepository = AdditionalParametersRepository;
            // AppData.Kilometers = (AppData.Trip.Travel_Direction == Direction.Direct ? Kilometers.OrderBy(km => km.Number) : Kilometers.OrderByDescending(km=>km.Number)).ToList();
            _ = SetLabelById("status-label", "Загрузка данных по основным параметрам ...");
            AppData.RdStructureRepository = RdStructureRepository;
            AppData.MainTrackStructureRepository = MainTrackStructureRepository;
            AppData.AdditionalParametersRepository = AdditionalParametersRepository;
            AppData.ReloadKilometers();
            foreach (var kilometer in AppData.Kilometers)
            {
                //_ = SetLabelById("status-label", "Загрузка данных по попречному профилю рельса для " + kilometer.Number + " км ...");
                //kilometer.CrossRailProfile = AdditionalParametersRepository.GetCrossRailProfileFromText(kilometer.Number);
                //kilometer.CalcRailProfileLines(AppData.Trip);
                //_ = SetLabelById("status-label", "Загрузка данных по стыкам для " + kilometer.Number + " км...");
                //kilometer.Gaps = AdditionalParametersRepository.GetGaps(AppData.Trip.Id, kilometer.Number);
                //kilometer.Heats = AdditionalParametersRepository.GetHeats(AppData.Trip.Id, kilometer.Number);
                //var profileDataList = RdStructureRepository.GetNextProfileDatasByKm(kilometer.Number, AppData.Trip.Id);
                //kilometer.CrossRailProfile.ParseDBList(profileDataList, kilometer);
                var DBcrossRailProfile = AdditionalParametersRepository.GetCrossRailProfileFromDBbyKm(kilometer.Number, AppData.Trip.Id);
                if (DBcrossRailProfile == null) continue;

                var sortedData = DBcrossRailProfile.OrderByDescending(d => d.Meter).ToList();
                kilometer.CrossRailProfile = AdditionalParametersRepository.GetCrossRailProfileFromDBParse(sortedData);
            }
            /*var profileDatas = RdStructureRepository.GetNextProfileDatas(AppData.ProfileMeter, 100, AppData.Trip.Id);

            foreach (var profileData in profileDatas)
            {
                foreach (var km in AppData.Kilometers)
                {
                    if (km.Meters.Count > km.CrossRailProfile.Meters.Count)
                    {
                        km.CrossRailProfile.ParseDB(profileData, km);
                    }
                }

            }*/

            AppData.IsDialogOpen = false;
            OnlineModeStarted.InvokeAsync(true);
            JSRuntime.InvokeVoidAsync("loader", false);
        }
        void LoadTrips(string period)
        {
            CurrentPeriod = period;
            ControlTrips = null;
            if ((!string.IsNullOrEmpty(CurrentPeriod)) && (CurrentRoad != -1))
            {
                var reportPeriod = new ReportPeriod();
                reportPeriod.PeriodMonth = Convert.ToInt32(CurrentPeriod.Split("-")[0]);
                reportPeriod.PeriodYear = Convert.ToInt32(CurrentPeriod.Split("-")[1]);
                
                ControlTrips = RdStructureRepository.GetTripsByRoad(CurrentRoad, reportPeriod, (int)TripType.Control);
                AdditionalTrips = ControlTrips;

                status = "Поездки текущего периода успешно загружены";
            }
            else
                status = "Выберите период";
            OkButtonDisabled = true;
        }
        void LoadTripType(string tripTypeId)
        {
            if ((!string.IsNullOrEmpty(CurrentPeriod)) && (CurrentRoad != -1))
            {
                var reportPeriod = new ReportPeriod();

                ControlTrips = AdditionalTrips.Where(o=>(int)o.Trip_Type == int.Parse(tripTypeId)).ToList();

                status = "Поездки текущего Типа проверки успешно загружены";
            }
            else
                status = "Выберите Тип проверки";
            OkButtonDisabled = true;
        }
        void LoadStations(long track_id)
        {
            AdmStations = AdmStations.Union(AdmStructureRepository.GetUnits(AdmStructureConst.AdmStationSection, track_id) as List<StationSection>).Distinct().ToList();
        }
        void OnlineOkClick(bool continuetrip)
        {
            if (continuetrip)
            {
                AppData.WorkMode = WorkMode.Online;
                AppData.IsDialogOpen = false;
                IsHavingActiveTrip = false;


                OnlineModeSelected(true);
                return;
            }

            var showError = AppData.Trip.Chief == null || CurrentTripType < 0 || TravelDirection == (int)Direction.NotDefined || AppData.Trip.Road_Id < 0 || AppData.Trip.Direction_id < 0 || AppData.Trip.Start_station < 0 || AppData.Trip.Final_station < 0 || CarPosition == (int)ALARm.Core.CarPosition.NotDefined || AppData.Trip.Route.Count == 0;
            foreach (var escort in AppData.Trip.Escort)
            {
                if ((escort.Distance_Id < 0) || (escort.FullName == null) || (escort.FullName.Equals(String.Empty)))
                    showError = true;
            }
            if (showError)
            {
                Toaster.Add("Пожалуйста, проверьте заполненность всех полей", MatToastType.Danger, "Создание поездки");
                return;
            }
            AppData.Trip.Start_Position = AppData.Trip.Route[0].Start_Km + AppData.Trip.Route[0].Start_M / 10000.0;
            AppData.Trip.Trip_Type = (TripType)CurrentTripType;
            AppData.Trip.Travel_Direction = (Direction)TravelDirection;
            AppData.Trip.Car_Position = (ALARm.Core.CarPosition)CarPosition;
            AppData.Trip.Id = RdStructureRepository.InsertTrip(AppData.Trip);
            AppData.Trip.Trip_date = DateTime.Now;
            AppData.WorkMode = WorkMode.Online;
            AppData.IsDialogOpen = false;
            OnlineModeSelected();
            
        }
        void LoadDirections(long road_id)
        {
            
            AppData.Trip.Road_Id = road_id;
            AdmDirections = AdmStructureRepository.GetDirectionsOfRoad(road_id);
            AppData.Trip.Distances = AdmStructureRepository.GetDitancesOfRoad(road_id) as List<AdmDistance>;
            LoadTracks(-1);
        }
        void OnlineModeSelected(bool continious = false)
        {
            Toaster.Add("Поexали", MatToastType.Success, "Поездка в режиме онлайн");

            AppData.Trip.Route = RdStructureRepository.GetTripFragments(AppData.Trip.Id);
            AppData.Trip.Route[0].AdmTracks = AdmStructureRepository.GetUnits(AdmStructureConst.AdmTrack, AppData.Trip.Direction_id) as List<AdmTrack>;
            AppData.Trip.Route[0].StationTracks = AdmStructureRepository.GetUnits(AdmStructureConst.AdmStationTrack, AppData.Trip.Start_station) as List<StationTrack>;
            var startStationParks = AdmStructureRepository.GetUnits(AdmStructureConst.AdmPark, AppData.Trip.Start_station) as List<Park>;
            foreach (var park in startStationParks)
            {
                AppData.Trip.Route[0].StationTracks = AppData.Trip.Route[0].StationTracks.Union(AdmStructureRepository.GetUnits(AdmStructureConst.AdmParkTrack, park.Id) as List<StationTrack>).ToList();
            }
            for (int i = 1; i < AppData.Trip.Route.Count; i++)
            {
                AppData.Trip.Route[i].AdmTracks = MainTrackStructureRepository.GetSwitchTracks(AppData.Trip.Route[i].Start_Switch_Id);
            }
            OnlineModeData.MainTrackStructureRepository = MainTrackStructureRepository;
            AppData.MainTrackStructureRepository = MainTrackStructureRepository;
            AppData.RdStructureRepository = RdStructureRepository;
            AppData.AdditionalParametersRepository = AdditionalParametersRepository;


            AppData.ReloadKilometers();

            //if (Helper.SendMessageFromRabbitMQ("desktop", AppData.Trip.Id, 3) == SocketState.Abortively)
            //    Toaster.Add("localhost Не удалось запустить онлайн обработку видеопотока. Проверьте подключение к сети!!!", MatToastType.Warning, "ALARmDK");
            //if (Helper.SendMessageFromRabbitMQ("mycomputer", AppData.Trip.Id, 1) == SocketState.Abortively)
            //    Toaster.Add("mycomputer Не удалось запустить онлайн обработку видеопотока. Проверьте подключение к сети!!!", MatToastType.Warning, "ALARmDK");

            //if (Helper.SendMessageFromSocket("localhost", 11000, $"start {AppData.Trip.Id} {3}") == SocketState.Abortively)
            //    Toaster.Add("Не удалось запустить онлайн обработку видеопотока. Проверьте подключение к сети!!!", MatToastType.Warning, "ALARmDK");
            //if (Helper.SendMessageFromSocket("mycomputer", 11000, $"start {AppData.Trip.Id} {1}") == SocketState.Abortively)
            //    Toaster.Add("Не удалось запустить онлайн обработку видеопотока. Проверьте подключение к сети!!!", MatToastType.Warning, "ALARmDK");
            OnlineModeStarted.InvokeAsync(null);
        }


        void LoadTracks(long direction_id)
        {
            AppData.Trip.Direction_id = direction_id;
            AppData.Trip.Route = new List<Fragment>() {
                new Fragment { AdmTracks =AdmStructureRepository.GetUnits(AdmStructureConst.AdmTrack, direction_id) as List<AdmTrack> }
            };
            if (AppData.Trip.Route[0].AdmTracks != null)
            {
                AdmStations = new List<StationSection>();
                foreach (var admTrack in AppData.Trip.Route[0].AdmTracks)
                {
                    LoadStations(admTrack.Id);
                }
            }
        }
        void SelectStartStation(long station_id)
        {
            AppData.Trip.Start_station = station_id;
          
            if (station_id > -1) {
                var station = AdmStations.Where(station => station.Station_Id == station_id).First();
                AppData.Trip.Start_station_name = station.Station;
                AppData.Trip.Route[0].Start_Km = station.Start_Km;
                AppData.Trip.Route[0].Start_M= station.Start_M;
                AppData.Trip.Travel_Direction = (Direction)TravelDirection;
                if (AppData.Trip.Travel_Direction == Direction.Direct)
                {
                    AppData.Trip.Route[0].Final_Km = station.Start_Km;
                    AppData.Trip.Route[0].Final_M = station.Start_M + 1;
                } else

                {
                    AppData.Trip.Route[0].Final_Km = station.Start_Km;
                    AppData.Trip.Route[0].Final_M = station.Start_M - 1;
                }

                AppData.Trip.Route[0].Belong_Id = station_id;
                AppData.Trip.Route[0].StationTracks = AdmStructureRepository.GetUnits(AdmStructureConst.AdmStationTrack, station_id) as List<StationTrack>;
                if (AppData.Trip.Route.Count > 1)
                    AppData.Trip.Route.RemoveRange(1, AppData.Trip.Route.Count - 1);
                var startStationParks = AdmStructureRepository.GetUnits(AdmStructureConst.AdmPark, station_id) as List<Park>;
                foreach (var park in startStationParks)
                {
                    AppData.Trip.Route[0].StationTracks = AppData.Trip.Route[0].StationTracks.Union(AdmStructureRepository.GetUnits(AdmStructureConst.AdmParkTrack, park.Id) as List<StationTrack>).ToList();
                }
                if (AppData.Trip.Final_station > -1)
                {
                    commonTracks = MainTrackStructureRepository.GetCommomTracks(AppData.Trip.Start_station, AppData.Trip.Final_station);
                    DetermineTravelDirecion();
                }
            }

        }
        void LoadStationTracks(long station_id)
        {
            AdmStructureRepository.GetUnits(AdmStructureConst.AdmStationTrack, station_id);
        }
        
        void SelectFinalStation(long station_id)
        {
            AppData.Trip.Final_station = station_id;
            
            if (AppData.Trip.Start_station > -1)
            {
                var station = AdmStations.Where(station => station.Station_Id == station_id).First();
                AppData.Trip.Final_station_name = station.Station;
                commonTracks = MainTrackStructureRepository.GetCommomTracks(AppData.Trip.Start_station, AppData.Trip.Final_station);
                DetermineTravelDirecion();
            }

        }

        private void DetermineTravelDirecion()
        {
            var startStation = AdmStations.Where(station => station.Station_Id == AppData.Trip.Start_station).First();
            var finalStation = AdmStations.Where(station => station.Station_Id == AppData.Trip.Final_station).First();
            TravelDirection = startStation.RealCoordinate() < finalStation.RealCoordinate() ? (int)Direction.Direct : (int)Direction.Reverse;
        }

        public async Task SetLabelById(string id, string label)
        {
            object[] paramss = new object[] { id, label };
            await JSRuntime.InvokeVoidAsync("SetLabelById", paramss);
        }
        void LoadPeriods(long road_id)
        {
            CurrentPeriod = string.Empty;
            ReportPeriods = null;
            ControlTrips = null;
            AdditionalTrips = null;
            CurrentRoad = road_id;
            

            if (road_id > -1)
            {
                ReportPeriods = RdStructureRepository.GetTripPeriodsByRoad(CurrentRoad);
                if ((ReportPeriods == null) || (!ReportPeriods.Any()))
                    status = "По выбранной дороге нет отчетных данных";
                else
                    status = "Периоды усешно заружены";
            }
            else
                status = "Выберите дорогу";
            OkButtonDisabled = true;
        }
        private List<AdmUnit> AdmRoads { get; set; }
        private List<AdmDirection> AdmDirections { get; set; }
        protected override void OnInitialized()
        {
            AdmRoads = AdmStructureRepository.GetUnits(AdmStructureConst.AdmRoad, -1) as List<AdmUnit>;
            status = "Список дорог успешно загружен";
            AppData.OnChange += StateHasChanged;
            AppData.Trip.Travel_Direction = Direction.NotDefined;
        }
        public async Task OnTripSelect()
        {
            object[] paramss = new object[] { "mdc-dialog__surface", "max-width", "max-content" };
            await JSRuntime.InvokeVoidAsync("ChangeStyleOfElementByClassName", paramss);
        }
        private void SelectTripItem(long id, TripType tripType) 
        {
            CurrentTrip = id;
            Trips currentTrip = ControlTrips.Where(trip => trip.Id == id).First();
            AppData.Trip = currentTrip;
            status = "Выбрана " + (tripType == TripType.Control ? "контрольная" : "дополнительная") + " поездка: " + currentTrip.Start_station_name + "-" + currentTrip.Final_station_name + " " + currentTrip.Trip_date.ToShortDateString();
            OkButtonDisabled = false;

            AppData.Trip.Direction_id = currentTrip.Direction_id;
            AppData.Trip.Route = new List<Fragment>() {
                new Fragment { AdmTracks =AdmStructureRepository.GetUnits(AdmStructureConst.AdmTrack, currentTrip.Direction_id) as List<AdmTrack> }
            };
            if (AppData.Trip.Route[0].AdmTracks != null)
            {
                
            }
        }
        private void NewTrip()
        {

            RdStructureRepository.CloseTrip();
            AppData.Trip = new Trips();
            IsHavingActiveTrip = false;
            Refresh();


        }
        private void Refresh()
        {
            StateHasChanged();
        }
       
    }
    
}
