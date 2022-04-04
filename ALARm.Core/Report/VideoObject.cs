using ALARm.Core.Report;
using System;
using System.Collections.Generic;

namespace ALARm.Core
{
    public class VideoObject
    {
        public Int64 Id { get; set; }
        public int Oid { get; set; }
        public int Next_oid { get; set; }
        public int Razn { get; set; }
        public int Fnum { get; set; }
        public int Km { get; set; }
        public int Pt { get; set; }
        public int Mtr { get; set; }
        public int Mm { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
        public int Prb { get; set; }
        public long Ms { get; set; }
        public Int64 Tripid { get; set; }
        public Int64 Fileid { get; set; }
        public Int64 file_id { get; set; }
        public String FilePath { get; set; }
        public Threat Threat { get; set; }
        public int Type_id { get; set; }
        public int[,] Frame { get; set; }
        public string Objname { get; set; }
        public string StationName { get; set; }
        public string PdbName { get; set; }
        public string Name { get; set; }
        public string Speed { get; set; }
        public string PdbSection { get; set; }
        public long TrackId { get; set; }
        public double RealCoordinate => Km + Mtr / 10000.0;
        
    }
    public enum VideoObjectType
    {
        bolt_M24 = 0, //болт M24
        bolt_M22 = 1, //болт М22
        no_bolt = 2, //отст болт

        ws_bot = 3, // шпал
        ws_top = 4, // шпал

        GapRight = 5,//стык
        GapLeft = 6,
        GapFull = 7,

        GapIsoLeft = 8,
        GapIsoRight = 9,//изо. стык
        GapIso = 10,

        MSBOT = 11, //метка на шпале
        MSTOP = 12, //метка на шпале
        MS = 13,//маячная рельс
        MS2 = 14,//маячная скреп

        D65 = 15, //D65
        D65_MissingSpike = 16, //отст гвоздя
        D65_NoPad =17,//отсутствует все

        KB65_NoPad = 18, //отсутствует все
        KB65_MissingClamp = 19, //KB65 отсутствующий болт
        KB65 = 20,  //KB65

        Railbreak = 21, // д трещина
        Railbreak_Stone = 22, //жб трещина
        Railbreak_vikol = 23, //д выкол
        Railbreak_raskol = 24, //д раскол
        
        SklNoPad = 25, //все отсутст
        SklBroken = 26, //дефект
        SKL = 27, //SKL

        GBR = 28, //ЖБР
        GBRNoPad = 29, //все отсутст

        ati = 30, //противоугон

        WW = 31, //gbr и skl без крыльев

        P350MissingClamp = 32, //отсутст 
        P350 = 33, //скреп

        KD65 = 34, //KD65
        KD65NB = 35, //отсутст болта

        KppNoPad = 36, //все отсут
        kpp = 37, //KPP

        CWWI = 38,  //соед. провод

        OIR = 39, //выход из реборд

        GapD_left = 40, //стыки снизу
        GapD_right = 41,
        GapD_full = 42,

        GapD_izo_left = 43, //изостыки снизу
        GapD_izo_rigth = 44,
        GapD_izo = 45,

        Railbreak_midsection = 46, //д по середине 
        Railbreak_Stone_vikol = 47, // жб выкол
        Railbreak_Stone_raskol = 48, //жб раскол
        Railbreak_Stone_midsection = 49, //жб по середине 



     //|--------------------------------------------------|
        
        stuck_styk = 222,    //слип. стык
        BSO1 = 211,          //дефектный соед. стык

        DDO2 = 221,      //Неизвестно
        NO1 = 251,       //Неизвестно
        NBDDO2 = 261,    //Неизвестно
        white = 271,     //Неизвестно
    }
    public class VideoObjectCount : VideoObject
    {
        public long Count { get; set; }
    }

    public class TripFiles
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string File_name { get; set; }
        public long Trip_id { get; set; }
        public int Threat_id { get; set; }
        public int Type_id { get; set; }
        public bool Checked_Status { get; set; }
    }

    public class Escort
    {
        public long Distance_Id { get; set; } = -1;
        public string Distance_Name { get; set; }
        public string FullName { get; set; }
        public bool Saved { get; set; } = false;
         

    }

    public class Trips
    {
        public string Direction_Name { get; set; }
        public string Direction { get; set; }
        public string DirectionCode { get; set; }

        public long Id { get; set; }
        public bool Processed { get; set; }
        public DateTime Trip_date { get; set; }
        public long Direction_id { get; set; } = -1;
        public bool Checked_Status { get; set; }
        public string Car { get; set; }
        public string Chief { get; set; }
        public string Start_station_name { get; set; }
        public string Final_station_name { get; set; }
        public long Start_station { get; set; } = -1;
        public long Final_station { get; set; } = -1;
        public Direction Travel_Direction { get; set; }
        public TripType Trip_Type { get; set; }

        //public bool Rail_profile { get; set; } = false;
        //public bool Longitudinal_profile { get; set; } = false;
        //public bool Short_irregularities { get; set; } = false;
        //public bool Joint_gaps { get; set; } = false;
        //public bool Georadar { get; set; } = false;
        //public bool Dimensions { get; set; } = false;
        //public bool Beacon_marks { get; set; } = false;
        //public bool Embankment { get; set; } = false;
        //public bool Rail_temperature { get; set; } = false;
        //public bool Geolocation { get; set; } = false;
        //public bool Rail_video_monitoring { get; set; } = false;
        //public bool Video_monitoring { get; set; } = false;

        public string GetProcessTypeName
        {
            get
            {
                switch (Trip_Type)
                {
                    case TripType.Work: return "Рабочая";
                    case TripType.Additional: return "Дополнительная";
                    case TripType.Control: return "Контрольная";
                    case TripType.NotDefined: return "Не определено";
                    default: return string.Empty;
                }
            }
        }
        public CarPosition Car_Position { get; set; }
        public long Road_Id { get; set; }
        public double Start_Position { get; set; }
        public long Track_Id { get; set; } = -1;

        public long Trip_id { get; set; }

        public bool Longitudinal_Profile { get; set; } = false;
        public bool Short_Irregularities { get; set; } = false;
        public bool Joint_Gaps { get; set; } = false;
        public bool Georadar { get; set; } = false;
        public bool Rail_Profile { get; set; } = false;
        public bool Dimensions { get; set; } = false;
        public bool Beacon_Marks { get; set; } = false;
        public bool Embankment { get; set; } = false;
        public bool Rail_Temperature { get; set; } = false;
        public bool Geolocation { get; set; } = false;
        public bool Rail_Video_Monitoring { get; set; } = false;
        public bool Video_Monitoring { get; set; } = true;

        public int Start_Km { get; set; }
        public int Start_M { get; set; }
        public List<Fragment> Route;
        public List<Escort> Escort;
        public List<AdmDistance> Distances { get; set; }
        public string ShortInfrom { get {
                return $"{Start_station_name}-{Final_station_name} {Trip_date.ToString("dd.MM.yyyy hh:mm")}";
            }
        }

        public string TrackCode { get; set; }
    }

    public class RdClasses
    {
        public int Id { get; set; }
        public int Class_id { get; set; }
        public string Description { get; set; }
        public string Obj_name { get; set; }
    }
}
