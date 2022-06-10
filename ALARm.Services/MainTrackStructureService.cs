using ALARm.Core;
using ALARm.Core.Report;
using ALARm.DataAccess;
using Autofac;
using System;
using System.Collections.Generic;

namespace ALARm.Services
{
    public class MainTrackStructureService
    {
        static readonly IContainer Container;
        public static IMainTrackStructureRepository GetRepository()
        {
            return new MainTrackStructureRepository();
        }
        static MainTrackStructureService()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MainTrackStructureRepository>().As<IMainTrackStructureRepository>();
            Container = builder.Build();
        }
        

        public static bool DeletePeriod(Int64 periodId)
        {
            return Container.Resolve<IMainTrackStructureRepository>().DeletePeriod(periodId);
        }

        public static bool UpdatePeriod(Period period)
        {
            return Container.Resolve<IMainTrackStructureRepository>().UpdatePeriod(period);
        }

        public static List<Period> GetPeriods(Int64 trackId, int mtoType)
        {
            return (Container.Resolve<IMainTrackStructureRepository>().GetPeriods(trackId, mtoType));
        }

        public static List<Catalog> GetCatalog(int mtoType)
        {
            return (Container.Resolve<IMainTrackStructureRepository>().GetCatalog(mtoType));
        }

        public static List<DistSection> GetDistSectionByDistId(long DistId)
        {
            return (Container.Resolve<IMainTrackStructureRepository>().GetDistSectionByDistId(DistId));
        }

        public static CoordinateGNSS GetCoordByLen(int start_Km, int start_M, int length, long track_Id, DateTime date)
        {
            return (Container.Resolve<IMainTrackStructureRepository>().GetCoordByLen(start_Km, start_M, length, track_Id, date));
        }


       
        public static object GetMtoObjectsByCoord(DateTime travelDate, int kilometrNumber, int motObjectType, string direction, string trackNumber)
        {
            return (Container.Resolve<IMainTrackStructureRepository>().GetMtoObjectsByCoord(travelDate, kilometrNumber, motObjectType, direction, trackNumber));
        }

        //public static object GetMtoObjectsByCoord(DateTime travelDate, int kilometrNumber, int СдЕЛАТЬ ВЫБОРКУ ПО адм.ТРАК АЙДИ, string direction, string trackNumber)
        //{
        //    return (Container.Resolve<IMainTrackStructureRepository>().GetMtoObjectsByCoord(travelDate, kilometrNumber, motObjectType, direction, trackNumber));
        //}
        public static List<RefPoint> GetRefPointsByTripIdToDate(long track_id, DateTime date_Vrem)
        {
            return (Container.Resolve<IMainTrackStructureRepository>().GetRefPointsByTripIdToDate(track_id, date_Vrem));
        }

        public static object GetMtoObjectsByCoord(DateTime travelDate, int kilometrNumber, int motObjectType, long direction_id, string trackNumber, int meter = -1)
        {
            return (Container.Resolve<IMainTrackStructureRepository>().GetMtoObjectsByCoord(travelDate, kilometrNumber, motObjectType, direction_id, trackNumber, meter));
        }
        public static object GetCurves(Int64 parentId, int type, Period period = null)
        {
            return (Container.Resolve<IMainTrackStructureRepository>().GetCurves(parentId,type,period));
        }

        public static Period InsertPeriod(Period period)
        {
            period.Id =  Container.Resolve<IMainTrackStructureRepository>().InsertPeriod(period);
            return period;
        }

        public static MainTrackObject InsertObject(MainTrackObject section, int motObjectType)
        {
            section.Id = Container.Resolve<IMainTrackStructureRepository>().InsertObject(section, motObjectType);
            return section;
        }

		public static object GetMtoObject(long mtoObjectId, int mtoObjectType)
        {
            return (Container.Resolve<IMainTrackStructureRepository>().GetMtoObject(mtoObjectId, mtoObjectType));
        }
        public static object GetMtoObjects(Int64 periodId, int mtoObjectType)
        {
            return (Container.Resolve<IMainTrackStructureRepository>().GetMtoObjects(periodId, mtoObjectType));
        }

        public static List<Speed> GetSpeeds(DateTime TripDate, string DirectionName, string TrackNumber)
        {
            return (Container.Resolve<IMainTrackStructureRepository>().GetSpeeds(TripDate, DirectionName, TrackNumber));
        }

        public static bool DeleteMtoObject(Int64 id, int mtoObjectType)
        {
            return Container.Resolve<IMainTrackStructureRepository>().DeleteMtoObject(id, mtoObjectType);
        }

        public static bool UpdateMtoObject(MainTrackObject section, int mtoObjectType)
        {
            return Container.Resolve<IMainTrackStructureRepository>().UpdateMtoObject(section, mtoObjectType);
        }

        public static object GetPeriods(Int64 trackId, object mtoNonStandard)
        {
            throw new NotImplementedException();
        }

		public static object Diam(int km, int meter)
        {
            return Container.Resolve<IMainTrackStructureRepository>().Hole_diam(km, meter);
        }
        public static bool GenerateDirectionList(string direction, Int64 trackID, string dirName, DateTime dirListDate)
        {
            return Container.Resolve<IMainTrackStructureRepository>().GenerateDirectionList(direction, trackID, dirName, dirListDate);
        }

        public static List<Gaps> GetIzoGaps(object trackNumber, long direction_id)
        {
            return Container.Resolve<IMainTrackStructureRepository>().GetIzoGaps(trackNumber, direction_id);
        }

        public static List<float> GetGaugesByCurve(List<MainParametersProcess> tripProcesses, Curve curve, string track)
        {
            return Container.Resolve<IMainTrackStructureRepository>().GetGaugesByCurve(tripProcesses, curve, track);
        }

        public static string GetModificationDate()
        {
            return Container.Resolve<IMainTrackStructureRepository>().GetModificationDate();
        }

        public static List<Temperature> GetTemp(long trip_id, long track_id, int km)
        {
            return Container.Resolve<IMainTrackStructureRepository>().GetTemp(trip_id, track_id, km);
        }

        public static object GetKMs(DateTime processDateTime, int mtoObjectType, Int64 curveId)
        {
            return Container.Resolve<IMainTrackStructureRepository>().GetKMs(processDateTime, mtoObjectType, curveId);
        }

        public static List<RDCurve> GetRDCurves(Int64 curveId, Int64 mainProcessId)
        {
            return Container.Resolve<IMainTrackStructureRepository>().GetRDCurves(curveId, mainProcessId);
        }

        public static NonstandardKm GetNonStandardKm(Int64 id, int mtoObject, int nkm)
        {
            return Container.Resolve<IMainTrackStructureRepository>().GetNonStandardKm(id, mtoObject, nkm);
        }

        public static List<RepairProject> GetAcceptRepairProject(Int64 trackId)
        {
            return Container.Resolve<IMainTrackStructureRepository>().GetAcceptRepairProject(trackId);
        }

        public static bool SetAcceptRepairProject(Int64 trackId)
        {
            return Container.Resolve<IMainTrackStructureRepository>().SetAcceptRepairProject(trackId);
        }
        public static object GetMtoObjectsByCoord(DateTime date, int nkm, int mtoObjectType, long track_id, int meter = -1)
        { 
            return Container.Resolve<IMainTrackStructureRepository>().GetMtoObjectsByCoord(date, nkm, mtoObjectType, track_id);
        }
        public static string GetSector(long track_id, int nkm, DateTime trip_date)
        {
            return Container.Resolve<IMainTrackStructureRepository>().GetSector(track_id, nkm, trip_date);
        }
        public static int GetDistanceBetween2Coord(int start_km, int start_m, int final_km, int final_m, long track_id, DateTime date)
        {
            return Container.Resolve<IMainTrackStructureRepository>().GetDistanceBetween2Coord(start_km, start_m, final_km, final_m, track_id, date);
        }

        public static List<Curve> GetCurveByTripIdToDate(MainParametersProcess tripProcess)
        {
            return Container.Resolve<IMainTrackStructureRepository>().GetCurveByTripIdToDate(tripProcess);
        }

        public static void Pru_write(long track_id, Kilometer kilometer, List<DigressionMark> pru_dig_list)
        {
             Container.Resolve<IMainTrackStructureRepository>().Pru_write(track_id, kilometer, pru_dig_list);
        }
        public static void Bpd_write(long track_id, Kilometer kilometer, List<DigressionMark> curve_bpd_list)
        {
            Container.Resolve<IMainTrackStructureRepository>().Bpd_write(track_id, kilometer, curve_bpd_list);
        }
    }
}
