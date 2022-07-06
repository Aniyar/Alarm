    namespace ALARm.Core
{
    ///
    public static class MainTrackStructureConst
    {
        //Изо стыки с БПД
        public const int GetIzoGaps = 42;
        //негодные шпалы
        public const int GetDigSleepers = 41;
        //Количество негодностей по пикету
        public const int GetCountByPiket = 40;
        //участки дистанций
        public const int MtoDistSection = 0;
        //тип шпал (ағаш, бетон95 жылға дейін, 95 жылдан кейін)
        public const int MtoCrossTie = 1;
        //нестандартные километры (ұзындығы 1000 метрге тең емес километрлер)
        public const int MtoNonStandard = 2;
        // класс пути
        public const int MtoTrackClass = 3;
        //скрепление
        public const int MtoRailsBrace = 4;
        //ширина колеи екі рельстің арақашықтығы (1520 мм)
        public const int MtoNormaWidth = 5;
        //установленный скорость (скорости по приказу) - бекітілген жылдамдық 
        public const int MtoSpeed = 6;
        public const int MtoTemperature = 60;
        public const int MtoTempSpeed = 7;
        //прямый участкий с возвышением одной рельсоыой нити
        public const int MtoElevation = 8;
        // участки подразделений 
        public const int MtoPdbSection = 9;
        //распределение раздельных пунктов
        public const int MtoStationSection = 10;
        //кривые участки қисық жолдар
        public const int MtoCurve = 11;
        
        //кривой участок уровень
        public const int MtoElCurve = 12;
        //рихтовочная нить
        public const int MtoStraighteningThread = 13;
        //искусстывенные сооружений - мост пен туннель
        public const int MtoArtificialConstruction = 14;
        //стрелочный перевод
        public const int MtoSwitch = 15;
        //бесстыковой путь
        public const int MtoLongRails = 16;
        //рель 
        public const int MtoRailSection = 17;
        //несуществующий км - жоқ километр
        public const int MtoNonExtKm = 18;

        public const int MtoProfileObject = 19;
        //карточка кривой
        public const int MtoRdCurve = 20;
        //изостык - изолацияланға қосылған жер
        public const int MtoChamJoint = 21;
        //отметки 
        public const int MtoProfmarks = 22;
        //
        public const int MtoTraffic = 23;
        //категория пути level beretin jerler
        public const int MtoWaycat = 24;
        //реперные точки
        public const int MtoRefPoint = 25;
        //
        public const int MtoRepairProject = 26;
        //
        public const int MtoRFID = 27;
        //контрольныйе участки
        public const int MtoCheckSection = 28;
        //рихтовка кривой - қисық жолдың ирелендеуі
        public const int MtoStCurve = 29;
        //расстояние между путями
        public const int MtoDistanceBetweenTracks = 30;

        public const int MtoCommunication = 31;


        public const int MtoCoordinateGNSS = 32;
        //дефект земляного полотна
        public const int MtoDefectsEarth = 33;
        //
        public const int MtoDeep = 35;
        //
        public const int MtoBallastType = 36;
        public const int MtoDimension = 37;
        public const int MtoFragment = 28;
        //кривые участки по пасспорту қисық жолдар
        public const int MtoCurveBPD = 38;


        public const int CatSide = 1000;
        public const int MtoSwitchMark = 1001;
        public const int MtoSwitchPoint = 1002;
        public const int MtoSwitchDir = 1003;
        public const int MtoSwitchStation = 1004;
        public const int MtoAcceptType = 1005;

        public const int Fragments = 2000;
        
    }

    public enum Side { NotDefined = 0, Right = 1, Left = 2 }
    public enum Threat { Left = 1, Middle = 0, Right = 2 ,None =3}
    public enum SwitchDirection { Direct = -1, NotDefined = 0, Reverse = 1 }
}