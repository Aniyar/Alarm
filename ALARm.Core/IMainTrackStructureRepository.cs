using ALARm.Core.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public interface IMainTrackStructureRepository
    {
        List<Period> GetPeriods(Int64 trackId, int mtoType);
        List<Catalog> GetCatalog(int mtoObjectType);
        Int64 InsertPeriod(Period period);
        bool UpdatePeriod(Period period);
        bool DeletePeriod(Int64 periodId);
        Int64 InsertObject(MainTrackObject section, int motObjectType);
        object GetMtoObject(long mtoObjectId, int mtoObjectType);
        object GetMtoObjects(Int64 periodId, int mtoObjectType);
        bool DeleteMtoObject(Int64 id, int mtoObjectType);
        bool UpdateMtoObject(MainTrackObject section, int mtoObjectType);
        bool GenerateDirectionList(string direction, Int64 trackID, string dirName, DateTime dirListDate);
        object GetCurves(Int64 parentId, int type, Period period = null);
        object GetKMs(DateTime processDateTime, int mtoObjectType, Int64 curveId);
        List<RDCurve> GetRDCurves(Int64 curveId, Int64 mainProcessId);
        NonstandardKm GetNonStandardKm(Int64 id, int mtoObject, int nkm);
        List<AdmTrack> GetSwitchTracks(long id);
        List<RepairProject> GetAcceptRepairProject(Int64 trackId);
        bool SetAcceptRepairProject(Int64 trackId);
        object GetMtoObjectsByCoord(DateTime date, int nkm, int mtoObjectType, string directionCode, string trackNumber);
        object GetMtoObjectsByCoord(DateTime date, int nkm, int mtoObjectType, long track_id, int meter = -1);
        object GetMtoObjectsByCoordSpeeds(DateTime date, int nkm, int mtoObjectType, long track_id, int meter );
        List<DistSection> GetDistSectionByDistId(long distId);
        CoordinateGNSS GetCoordByLen(int start_km, int start_m, int length, long track_Id, DateTime date);
        string GetModificationDate();
        List<Switch> GetFragmentsSwitch(Int64 trackId, Direction direction, double coord = 0.0);
        Switch GetTrackSwitch(long track_Id, long startSwitch_Id);
        List<long> GetCommomTracks(long start_station, long final_station);
        object GetMtoObjectsByCoord(DateTime travelDate, int kilometrNumber, int motObjectType, long direction_id, string trackNumber, int meter = -1);
		object Hole_diam(int km, int meter);
        void InsertRoute(List<Fragment> route, long trip_id);
        List<Kilometer> GetKilometersOfFragment(Fragment fragment, DateTime date, Direction direction, long trip_id);
        string GetSector(long track_id, int nkm, DateTime trip_date);
        List<RefPoint> GetRefPointsByTripIdToDate(long track_id, DateTime date_Vrem);
        int GetDistanceBetween2Coord(int start_km, int start_m, int final_km, int final_m, long track_id, DateTime date);
        List<Temperature> GetTemp(long trip_id, long track_id, int km);
        List<float> GetGaugesByCurve(List<MainParametersProcess> tripProcesses, Curve curve, string track);
        List<Curve> GetCurveByTripIdToDate(MainParametersProcess tripProcess);
        void Pru_write(long track_id, Kilometer kilometer, List<DigressionMark> pru_dig_list);

        void Bpd_write(long track_id, Kilometer kilometer, List<DigressionMark> curve_bpd_list);
        List<Gaps> GetIzoGaps(object trackNumber, long direction_id);
        List<Speed> GetSpeeds(DateTime tripDate, string directionName, string trackNumber);
    }
}
