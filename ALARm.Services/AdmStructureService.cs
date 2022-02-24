using System;
using ALARm.Core;
using ALARm.DataAccess;
using Autofac;
using System.IO;
using System.Collections.Generic;

namespace ALARm.Services
{
    public class AdmStructureService
    {
        static readonly IContainer Container;
        static AdmStructureService()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<AdmStructureRepository>().As<IAdmStructureRepository>();
            Container = builder.Build();
        }

        public static bool Delete(Int64 unitId, int unitLevel)
        {
            return Container.Resolve<IAdmStructureRepository>().Delete(unitId, unitLevel);
        }

        public static bool Update(object obj, int unitLevel)
        {
            return Container.Resolve<IAdmStructureRepository>().Update(obj, unitLevel);
        }

        public static object GetUnits(int admLevel, Int64 parentId)
        {
            return Container.Resolve<IAdmStructureRepository>().GetUnits(admLevel, parentId);
           
        }

        public static AdmUnit GetRoadName(long id, long parent_Id)
        {
            throw new NotImplementedException();
        }

        public static List<AdmUnit> GetDistancesRoad(Int64 roadId)
        {
            return Container.Resolve<IAdmStructureRepository>().GetDistancesRoad(roadId);
        }

        public static object GetCurvesAdmUnits(Int64 curveId)
        {
            return Container.Resolve<IAdmStructureRepository>().GetCurvesAdmUnits(curveId);
        }

        public static object GetCatalog(int unitLevel)
        {
            return (Container.Resolve<IAdmStructureRepository>().GetCatalog(unitLevel));
        }

        public static object Insert(object obj)
        {
            ((AdmUnit) obj).Id =  Container.Resolve<IAdmStructureRepository>().Insert(obj);
            return obj;
        }
        /// <summary>
        /// Возвращает администравтивную единицу
        /// </summary>
        /// <param name="admLevel">уровень административной единицы</param>
        /// <param name="id">идентификатор адм единицы</param>
        /// <returns>администравтивную единицу (AdmUnit)</returns>
        public static object GetUnit(int admLevel, Int64 id)
        {
            return Container.Resolve<IAdmStructureRepository>().GetUnit(admLevel, id) as AdmUnit;
        }

        public static object GetTrackName(long track_id)
        {
            return (Container.Resolve<IAdmStructureRepository>().GetTrackName(track_id));
        }

        public static object GetDistancesOfRoad(Int64 roadId)
        {
            return (Container.Resolve<IAdmStructureRepository>().GetDitancesOfRoad(roadId));
        }

        public static object GetPartsOfDistance(Int64 distanceId)
        {
            return (Container.Resolve<IAdmStructureRepository>().GetPartsOfDistance(distanceId));
        }

        public static IAdmStructureRepository GetRepository()
        {
            return new AdmStructureRepository();
        }

        public static StationTrack GetStationTrack(Int64 id, int admlvl)
        {
            return (Container.Resolve<IAdmStructureRepository>().GetStationTrack(id, admlvl));
        }
        
        public static string GetRoadName(Int64 childId, int childLevel, bool fullName)
        {
            return (Container.Resolve<IAdmStructureRepository>().GetRoadName(childId, childLevel, fullName));
        }

        public static string GetDirectionName(string code)
        {
            return (Container.Resolve<IAdmStructureRepository>().GetDirectionName(code));
        }

        public static List<AdmRoad> GetDirectionRoads(long directionId)
        {
            return (Container.Resolve<IAdmStructureRepository>().GetDirectionRoads(directionId));
        }

        public static bool DeleteDirection(long id, long roadid, bool delMode)
        {
            return (Container.Resolve<IAdmStructureRepository>().DeleteDirection(id, roadid, delMode));
        }

        public static List<AdmDirection> GetStationsDirection(long station_id, long road_id)
        {
            return Container.Resolve<IAdmStructureRepository>().GetStationsDirection(station_id, road_id);
        }
        public static AdmDirection GetDirectionIdByTrack(long track_id)
        {
            return Container.Resolve<IAdmStructureRepository>().GetDirectionByTrack(track_id);
        }
        public static object GetUnitsOfRoad(int admLevel, long road_id) {
            return Container.Resolve<IAdmStructureRepository>().GetUnitsOfRoad(admLevel, road_id);
        }

        public static object GetDirectionByTrack(long track_id)
        {
            return (Container.Resolve<IAdmStructureRepository>().GetDirectionByTrack(track_id));
        }
    }
}
