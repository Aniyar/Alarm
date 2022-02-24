using System;
using System.Collections.Generic;
using System.IO;

namespace ALARm.Core
{
    public interface IAdmStructureRepository
    {
        object GetCurvesAdmUnits(Int64 curveId);
        object GetUnit(int admLevel, Int64 id);
        object GetUnits(int admLevel, Int64 parentId);
        List<AdmUnit> GetDistancesRoad(Int64 roadId);
        Int64 Insert(object obj);
        bool Update(object obj, int unitLevel);
        bool Delete(Int64 unitId, int unitLevel);
        object GetDitancesOfRoad(Int64 roadId);
        object GetPartsOfDistance(Int64 distanceId);
        List<Catalog> GetCatalog(int mtoObjectType);
        StationTrack GetStationTrack(Int64 id, int admlvl);
		string GetRoadName(Int64 childId, int childLevel, bool fullName);
        string GetDirectionName(string code);
        List<AdmRoad> GetDirectionRoads(long directionId);
        List<AdmDirection> GetDirectionsOfRoad(long road_id);
        bool DeleteDirection(long id, long roadid, bool delMode);
		List<AdmDirection> GetStationsDirection(long station_id, long road_id);
        AdmDirection GetDirectionByTrack(long track_id);
        object GetUnitsOfRoad(int admLevel, long road_id);
        object GetTrackName(long track_id);
    }
}
