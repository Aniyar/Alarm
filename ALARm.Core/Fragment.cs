using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class Fragment : MainTrackObject
    {
        private long _trackId = -1;
        public long Belong_Id { get; set; }
        public Direction Direction => Final_Km ==0 && Final_M == 0 ? Direction.NotDefined : RealStartCoordinate <= RealFinalCoordinate ? Direction.Direct : Direction.Reverse;
        public new long Track_Id
        {
            get
            {
                return _trackId;
            }
            set {
                _trackId = value;

            }
        }
        public int Length { get; set; }
        public long Start_Switch_Id { get; set; } = -1;
        public long Final_Switch_Id { get; set; } = -1;
        public bool Editable { get; set; } = false;

        public Fragment GetNextFrgament(IMainTrackStructureRepository mainTrackStructureRepository, Direction travel_Direction)
        {
            var switches = mainTrackStructureRepository.GetFragmentsSwitch(Track_Id, travel_Direction, RealFinalCoordinate);
            if (switches.Count < 2)
                return null;
            var result = new Fragment();
            for (int i = 0; i < switches.Count - 1; i++)
            {
                if (switches[i].RealCoordinate < RealStartCoordinate && RealStartCoordinate < switches[i + 1].RealCoordinate)
                {
                    var startSwitch = travel_Direction == Direction.Direct ? switches[i + 1] : switches[i];
                   // var finalSwitch = travel_Direction == Direction.Direct ? switches[i] : switches[i + 1];
                    result.Start_Km = startSwitch.Km;
                    result.Start_M = startSwitch.Meter + (int)travel_Direction * 1;
                    result.AdmTracks = mainTrackStructureRepository.GetSwitchTracks(startSwitch.Id);
                    result.Start_Switch_Id = startSwitch.Id;

                }
            }
            return result;
        }
        public List<Fragment> NextFragments { get; private set; } = new List<Fragment>();
        public void GetNextFragments(IMainTrackStructureRepository mainTrackStructureRepository) {

            NextFragments = new List<Fragment>();
            var switches = mainTrackStructureRepository.GetFragmentsSwitch(Track_Id, Direction, RealStartCoordinate);
            
            foreach (var sw in switches)
            {
               var fragment = new Fragment { Start_Km = sw.Km, Start_M = sw.Meter};
               fragment.Start_Switch_Id = sw.Id;
               fragment.AdmTracks = mainTrackStructureRepository.GetSwitchTracks(sw.Id);
               NextFragments.Add(fragment);
            
               
            }
            
           
        }
        

        public string StartStationName { get; set; }
        public string FinalStationName { get; set; }
        
        public override string ToString()
        {
            return $"путь: {Track_Code}; нач.:{RealStartCoordinate}; кон.: {RealFinalCoordinate}; кол.пер.:{AdmTracks?.Count}";
        }
        public List<AdmTrack> AdmTracks { get;set;}
        public List<StationTrack> StationTracks { get; set; }
        public Fragment NextFragment { get; set; }
    }
}
