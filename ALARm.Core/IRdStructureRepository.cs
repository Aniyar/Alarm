using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;

namespace ALARm.Core
{
    public interface IRdStructureRepository
    {
        List<Int64> GetAdmDirectionIDs(Int64 distance_id);
        List<VideoObject> GetRdObject();
        List<ReportTemplate> GeReportTemplates(int repType);
        List<VideoObject> GetRdObject(int object_id, int km, Int64 file_id);
        List<VideoObject> GetRdObject(string object_id, string trip_files);
        List<string> GetProfileFilePath(long trip_id = -1);
        List<VideoObject> GetRdObjectKm(string km, string trip_files);
        List<VideoObject> GetRdObjectKm(string object_id, string km, string trip_files);
        List<VideoObject> GetVideoObjects(string obj_name, MainParametersProcess process);
        Trips GetTrip(long trip_id);
        object GetTrips(Int64 process_id);
        object GetTrips(List<Int64> directionIDs, int rd_lvl);
        List<RdClasses> GetRdClasses();
        List<Catalog> GetCatalog(int catType);
        void InsertEscort(Escort escort, long trip_id);
        RdClasses GetRdClasses(int class_i);
        List<Kilometer> GetKilometersByTrip(Trips trip);
        List<VideoObjectCount> GetRdObjectCount(Int64 trip_files_id);
        List<VideoObjectCount> GetRdObjectCount(List<Int64> fileIDs);
        object GetAdmDirection(List<Int64> directionIDs);
        List<MainParametersProcess> GetMainParametersProcess();
        List<MainParametersProcess> GetMainParametersProcesses(ReportPeriod period, Int64 distance_id, bool all= true);
        List<MainParametersProcess> GetAdditionalParametersProcess(ReportPeriod process_id);
        MainParametersProcess GetMainParametersProcess(Int64 process_id);
        List<int> GetKilometerTrip(long trip_id,long trackId = -1 );
        List<ReportPeriod> GetReportPeriods(long distanceId);
        List<MainParametersProcess> GetMainParametersProcess(ReportPeriod period, string distanceName);
        List<Digression> GetBallast(MainParametersProcess mainProcess);
        List<Digression> GetDevRail(long trip_id, string dist, string track);
        List<RailFastener> GetRAilSole(long tripId, bool orderBySide, string pch, object trackName);
        List<Digression> GetViolPerpen(int Trip_id, int[] typ, int km);
        List<AdmTrack> GetTracks(long distanceId, ReportPeriod period,long trackId = -1);
        List<Digression> GetDigressions(Int64 processId, string distanceName, int[] typ);
        List<NotCheckedKm> GetDop2(long trip_id, long distId);
        List<Digression> GetDigressions(MainParametersProcess process, int[] typ);
        List<Digression> GetDigSleepers(MainParametersProcess mainProcess, int getDigSleepers);
        List<Digression> GetAti(long Trip_id, int km);
        List<Digression> NoBolt(MainParametersProcess process, Threat typ, int km);
        List<Digression> GetShpal(MainParametersProcess process, int[] typ, int km);
        object GetS3(Int64 processId, string digName = null);

        object GetS3ForKm(Int64 processId, int km);


        object GetS3(Int64 processId, int type, string distanceName);

        object GetS3(Int64 processId, int type);
        object GetBedemost(Int64 processId);
        List<Curve> GetCurves(Int64 processId);
        CurveParams GetCurveParams(long curveId);
        List<Gaps> GetGapsBetweenCoords(long processId, long trackId, double coordStart, double coordFinal);
        List<Gaps> GetGapsAndIsoGaps(long processId, long trackId);
        List<Gaps> GetGaps(Int64 processId);
        object GetRdProfileObjects(long trackId, DateTime date, int type, int start_km, int start_m, int final_km, int final_m);
        object GetRdTables(MainParametersProcess process, int type_id);
        bool CleanTables(int type);

        List<Digression> GetDigressions3and4(Int64 processId, string distanceName, int[] typ);
        List<Digression> TrackDeviations(Int64 processId, string distanceName, int[] typ);
      
        List<CheckSection> CheckVerify(long trip_id, int start, int final);
        List<Digression> DeviationsRailHeadWear(Int64 processId, string distanceName, int[] typ);
        List<RdIrregularity> GetCountByPiket(long Trip_id, int type);
        List<Digression> DerogationsIsostsAndJointless(Int64 processId, string distanceName, int[] typ);
        List<Curve> GetCurvesInTrip(long tripId);
        object SumOfTheDep(long processId);
        List<Digression> DeviationOfPRZH(Int64 processId, string distanceName, int[] typ);
        List<Digression> DeviationsToDangerous(Int64 processId, string distanceName, int[] typ);
        List<Digression> DeviationsRailRailing(Int64 processId, string distanceName, int[] typ);
        List<ControlAdjustmentProtocol> ControlAdjustmentProtocol(Int64 processId);
        List<RDCurve> LoadDataByCurve(List<RDCurve> rdcs, long tripId);
        List<ControlAdjustmentProtocol> GetS3ByTripId(Int64 processId);
        List<Digression> AverageScoreDepartments(Int64 processId, string distanceName, int[] typ);
        List<Digression> PoorKilometers(Int64 processId, string distanceName, int[] typ);
        object GetS3all(Int64 processId, string pch, long trackId = -1);
        object GetDBD(Int64 processId);
        List<Kilometer> GetKilometersByTripId(long trip_id);
        int InsertTrip(Trips trip);
        bool GetButtonState(string name);
        void SetButtonStatus(string name, bool state);
        void CloseTrip();
        Trips GetCurrentTrip();
        List<Fragment> GetTripFragments(long id);
        List<MainParametersProcess> GetProcess(ReportPeriod period, long distanceId, ProcessType processType);
        List<RailFastener> GetBadRailFasteners(long tripId, bool orderBySide, string code, object trackName, int km = -1);
        List<Sleepers> GetSleeper(long trackId, int km, int meter, int start, int final, Threat threat);
        List<Sleepers> GetSleepers();
        SiteInfo GetSiteInfo(long trackId, int startKm, int finalKm);
        List<AdmTrack> GetTracksOnTrip(long tripId);
        List<Trips> GetTripsOnDistance(long distanceId, ReportPeriod period);
        List<Curve> GetCurvesAsTripElems(long trackId, DateTime date, int start_km, int start_m, int final_km, int final_m);
        object GetTripSections(long trackId, DateTime date, int type);
        object GetTripSections(long trackId, DateTime date, int start_km, int start_m, int final_km, int final_m, int type);
        List<Digression> GetAdditional(int km);

        List<Trips> GetTripsByRoad(Int64 road_id, ReportPeriod period, int trip_type);
        List<ReportPeriod> GetTripPeriodsByRoad(long road_id);
        List<OutData> GetNextOutDatas(int meter, int count, long trip_id);
        int GetDistanceBetweenCoordinates(int start_km, int start_m, int final_km, int final_m, long track_id, DateTime trip_date);
        void RunRvoDataInsert();
        long[] RunRvoDataInsert(int km, long fileID, string path);
        ReportTemplate GetReportTemplate(string className);
        void ClearBedemost(long id);
        List<Trips> GetTrips(int count = 10);
        int InsertKilometer(Kilometer km);
        List<DigressionMark> GetDigressionMarks(long trip_id,int km, long track_id, int[] type);
        List<Gap> GetGaps(long tripId, GapSource source, int km);

        void SendEkasuiData(Trips trip, int km);

        List<RdProfile> getRefrenceData(Dictionary<String,Object> p);
        /// <summary>
        /// Өлшеу барысындағы нақты қисықтың параметрлерін қайтарады
        /// </summary>
        /// <param name="curve_id">қисықтың идентификаторы</param>
        /// <param name="trip_id">сапардың идентификаторы</param>
        /// <returns>қисықтың бойындағы әрбір метрдегі нүктелер</returns>
        List<RDCurve> GetRDCurves(long curve_id, long trip_id);
        /// <summary>
        /// вагонға ілесушілер тізімін қайтарады
        /// </summary>
        /// <param name="trip_id">саяхат идетификаторы</param>
        /// <returns>ілесушілер тізімін қайтарады</returns>
        public List<Escort> GetEscorts(long trip_id);
        object GetNextPart(Func<double> last1, Func<int> last2, int n);

        /// <summary>
        /// Тапқан ескертпелерді қайтарады
        /// </summary>
        /// <param name="trip_id">саяхат идентификаторы</param>
        /// <param name="track_id">жол идентификаторы</param>
        /// <param name="km">километр нөмірі</param>
        /// <returns>ескертпелер</returns>
        public List<DigressionMark> GetDigressionMarks(long trip_id, long track_id, int km);
        List<RdProfile> GetRdTablesByKM(MainParametersProcess tripProcess, float stKM, float fnKM);
        public Dictionary<string, object> GetBedemost(long trip_id, long track_id, int km);
        /// <summary>
        ///Ескерту жазбасын түзету
        /// </summary>
        /// <param name="digression"></param>
        /// <param name="action"></param>
        /// <param name="kilometer"></param>
        /// <returns>сәтті болған жағдайда 1, әйтпесе -1</returns>
        public int UpdateDigression(DigressionMark digression, Kilometer kilometer, RdAction action);
        /// <summary>
        /// PU32 үшін километрлерді қайтарады
        /// </summary>
        /// <param name="from">есептің бастапқы күні</param>
        /// <param name="to">есептің аяққы күні</param>
        /// <param name="distance_id">қашықтықтық идентификаторы</param>
        /// <param name="trip_type">саяхат түрі</param>
        /// <returns>қашықтық бойындағы белгілі бір кезеңде тексерілген километрлерді қайтарады</returns>
        public List<Kilometer> GetPU32Kilometers(DateTime from, DateTime to, long distance_id, TripType trip_type);
        List<Trips> GetTripFromFileId(int fileId);
        List<CrosProf> GetNextProfileDatas(int meter, int count, long trip_id);
        List<CrosProf> GetNextProfileDatasByKm(int number, long id);
        //List<Digression> TotalSleeper(long processId);
        public void UpdateGap(Gap gap);
        public int UpdateGapBase(Gap gap, Kilometer kilometer, RdAction action);
        public int InsertCorrection(long trip_id, int track_id, int Number, int coord, int CorrectionValue);
        public List<CorrectionNote> GetCorrectionNotes(long trip_id, int track_id, int Number, int coord, int CorrectionValue);
        public int UpdateDigressionBase(Digression digression, int type, Kilometer kilometer, RdAction action);
        public int UpdateAdditionalBase(Digression digression, Kilometer kilometer, RdAction action);

    }
}
