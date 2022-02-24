using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.DataAccess;
using Autofac;
using System;
using System.Collections.Generic;

namespace ALARm.Services
{
    public class RdStructureService
    {
        static readonly IContainer Container;

        public static List<string> GetProfileFilePath(long trip_id = -1)
        {
            return Container.Resolve<IRdStructureRepository>().GetProfileFilePath(trip_id);
        }

        public static IRdStructureRepository GetRepository()
        {
            return new RdStructureRepository();
        }
        static RdStructureService()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<RdStructureRepository>().As<IRdStructureRepository>();
            Container = builder.Build();
        }

        public static List<Int64> GetAdmDirectionIDs(Int64 distance_id)
        {
            return Container.Resolve<IRdStructureRepository>().GetAdmDirectionIDs(distance_id);
        }

        public static List<Kilometer> GetKilometersByTrip(Trips trip)
        {
            return Container.Resolve<IRdStructureRepository>().GetKilometersByTrip(trip);
        }

        public static List<VideoObject> GetRdObject()
        {
            return Container.Resolve<IRdStructureRepository>().GetRdObject();
        }

        public static List<MainParametersProcess> GetAdditionalParametersProcess(ReportPeriod period)
        {
            return Container.Resolve<IRdStructureRepository>().GetAdditionalParametersProcess(period);
        }

        public static List<int> GetKilometerTrip(long trip_id,long trackId = -1)
        {
            return Container.Resolve<IRdStructureRepository>().GetKilometerTrip(trip_id, trackId);
        }

        public static List<Digression> GetBallast(MainParametersProcess mainProcess)
        {
            return Container.Resolve<IRdStructureRepository>().GetBallast(mainProcess);
        }

        public static List<Digression> GetDevRail(long trip_id, string dist, string track)
        {
            return Container.Resolve<IRdStructureRepository>().GetDevRail(trip_id, dist, track);
        }

        public static List<RailFastener> GetRAilSole(long tripId, bool orderBySide, string pch, object trackName)
        {
            return Container.Resolve<IRdStructureRepository>().GetRAilSole(tripId, orderBySide, pch, trackName);
        }

        public static List<NotCheckedKm> GetDop2(long trip_id, long distId)
        {
            return Container.Resolve<IRdStructureRepository>().GetDop2(trip_id, distId);
        }

        public static List<Digression> GetDigSleepers(MainParametersProcess mainProcess, int getDigSleepers)
        {
            return Container.Resolve<IRdStructureRepository>().GetDigSleepers(mainProcess, getDigSleepers);
        }

   
        public static List<AdmTrack> GetTracks(long distanceId, ReportPeriod period,long trackId = -1)
        {
            return Container.Resolve<IRdStructureRepository>().GetTracks(distanceId, period,trackId);
        }
        public static List<Digression> GetAti(long Trip_id, int km)
        {
            return Container.Resolve<IRdStructureRepository>().GetAti(Trip_id, km);
        }

        public static List<Catalog> GetCatalog(int catType)
        {
            return Container.Resolve<IRdStructureRepository>().GetCatalog(catType);
        }


        public static List<Digression> GetViolPerpen(int Trip_id, int[] typ, int km)
        {
            return Container.Resolve<IRdStructureRepository>().GetViolPerpen(Trip_id, typ, km);
        }
        public static List<Digression> NoBolt(MainParametersProcess process, Threat typ, int km=-1)
        {
            return Container.Resolve<IRdStructureRepository>().NoBolt(process, typ, km);
        }
        public static List<Digression> GetShpal(MainParametersProcess process, int[] typ, int km = -1)
        {
            return Container.Resolve<IRdStructureRepository>().GetShpal(process, typ, km);
        }



        public static object GetReportPeriods(long distanceId)
        {
            return Container.Resolve<IRdStructureRepository>().GetReportPeriods(distanceId);
        }

        public static List<Curve> GetCurves(Int64 processId)
        {
            return Container.Resolve<IRdStructureRepository>().GetCurves(processId);
        }

        public static List<CheckSection> CheckVerify(long trip_id, int start, int final)
        {
            return Container.Resolve<IRdStructureRepository>().CheckVerify(trip_id, start, final);
        }

        public static List<RDCurve> LoadDataByCurve(List<RDCurve> rdcs, long tripId)
        {
            return Container.Resolve<IRdStructureRepository>().LoadDataByCurve(rdcs, tripId);
        }

        public static List<RdIrregularity> GetCountByPiket(long Trip_id, int type)
        {
            return Container.Resolve<IRdStructureRepository>().GetCountByPiket(Trip_id, type);
        }

        public static List<Gaps> GetGapsBetweenCoords(long processId, long trackId, double coordStart, double coordFinal)
        {
            return Container.Resolve<IRdStructureRepository>().GetGapsBetweenCoords(processId, trackId, coordStart, coordFinal);
        }

        public static List<Curve> GetCurvesInTrip(long tripId)
        {
            return Container.Resolve<IRdStructureRepository>().GetCurvesInTrip(tripId);
        }

        public static List<Gaps> GetGapsAndIsoGaps(long processId, long trackId)
        {
            return Container.Resolve<IRdStructureRepository>().GetGapsAndIsoGaps(processId, trackId);
        }
        public static List<Gaps> GetGaps(Int64 processId)
        {
            return Container.Resolve<IRdStructureRepository>().GetGaps(processId);
        }

        public static List<ReportTemplate> GetReportTemplates(int repType)
        {
            return Container.Resolve<IRdStructureRepository>().GeReportTemplates(repType);
        }

        public static object SumOfTheDep(long processId)
        {
            return Container.Resolve<IRdStructureRepository>().SumOfTheDep(processId);
        }
        public static List<VideoObject> GetRdObject(int object_id, int km, Int64 file_id)
        {
            return Container.Resolve<IRdStructureRepository>().GetRdObject(object_id, km, file_id);
        }



        public static List<VideoObject> GetRdObject(string object_id, string trip_files)
        {
            return Container.Resolve<IRdStructureRepository>().GetRdObject(object_id, trip_files);
        }

        public static List<VideoObject> GetVideoObjects(string obj_name, MainParametersProcess process)
        {
            return Container.Resolve<IRdStructureRepository>().GetVideoObjects(obj_name, process);
        }

        public static object GetNextOutDatas(int v1, int v2, long trip_id)
        {
            return Container.Resolve<IRdStructureRepository>().GetNextOutDatas(v1, v2, trip_id);
        }

        public static List<VideoObjectCount> GetRdObjectCount(Int64 trip_files_id)
        {
            return Container.Resolve<IRdStructureRepository>().GetRdObjectCount(trip_files_id);
        }



        public static List<VideoObjectCount> GetRdObjectCount(List<Int64> fileIDs)
        {
            return Container.Resolve<IRdStructureRepository>().GetRdObjectCount(fileIDs);
        }

        public static List<VideoObject> GetRdObjectKm(string km, string trip_files)
        {
            return Container.Resolve<IRdStructureRepository>().GetRdObjectKm(km, trip_files);
        }

        public static List<Digression> PoorKilometers(Int64 processId, string distanceName, int[] typ)
        {
            return Container.Resolve<IRdStructureRepository>().PoorKilometers(processId, distanceName, typ);
        }

        public static List<Digression> AverageScoreDepartments(Int64 processId, string distanceName, int[] typ)
        {
            return Container.Resolve<IRdStructureRepository>().AverageScoreDepartments(processId, distanceName, typ);
        }


        public static List<Digression> DeviationsRailRailing(Int64 processId, string distanceName, int[] typ)
        {
            return Container.Resolve<IRdStructureRepository>().DeviationsRailRailing(processId, distanceName, typ);
        }

        public static List<ControlAdjustmentProtocol> ControlAdjustmentProtocol(Int64 processId)
        {
            return Container.Resolve<IRdStructureRepository>().ControlAdjustmentProtocol(processId);
        }
        public static List<ControlAdjustmentProtocol> GetS3ByTripId(Int64 processId)
        {
            return Container.Resolve<IRdStructureRepository>().GetS3ByTripId(processId);
        }

        public static List<Digression> DeviationsToDangerous(Int64 processId, string distanceName, int[] typ)
        {
            return Container.Resolve<IRdStructureRepository>().DeviationsToDangerous(processId, distanceName, typ);
        }

        public static List<Digression> DeviationOfPRZH(Int64 processId, string distanceName, int[] typ)
        {
            return Container.Resolve<IRdStructureRepository>().DeviationOfPRZH(processId, distanceName, typ);
        }

        public static List<Digression> DerogationsIsostsAndJointless(Int64 processId, string distanceName, int[] typ)
        {
            return Container.Resolve<IRdStructureRepository>().DerogationsIsostsAndJointless(processId, distanceName, typ);
        }

        public static List<Digression> DeviationsRailHeadWear(Int64 processId, string distanceName, int[] typ)
        {
            return Container.Resolve<IRdStructureRepository>().DeviationsRailHeadWear(processId, distanceName, typ);
        }

        public static List<Digression> TrackDeviations(Int64 processId, string distanceName, int[] typ)
        {
            return Container.Resolve<IRdStructureRepository>().TrackDeviations(processId, distanceName, typ);
        }

        public static List<VideoObject> GetRdObjectKm(string object_id, string km, string trip_files)
        {
            return Container.Resolve<IRdStructureRepository>().GetRdObjectKm(object_id, km, trip_files);
        }

        public static List<Digression> GetDigressions3and4(Int64 processId, string distanceName, int[] typ)
        {
            return Container.Resolve<IRdStructureRepository>().GetDigressions3and4(processId, distanceName, typ);
        }

        public static Trips GetTrip(long trip_id)
        {
            return Container.Resolve<IRdStructureRepository>().GetTrip(trip_id);
        }
        public static object GetTrips(Int64 process_id)
        {
            return Container.Resolve<IRdStructureRepository>().GetTrips(process_id);
        }

        public static object GetTrips(List<Int64> directionIDs, int rd_lvl)
        {
            return Container.Resolve<IRdStructureRepository>().GetTrips(directionIDs, rd_lvl);
        }

        public static List<Digression> GetDigressions(Int64 processId, string distanceName, int[] typ)
        {
            return Container.Resolve<IRdStructureRepository>().GetDigressions(processId, distanceName, typ);
        }

        public static List<Digression> GetDigressions(MainParametersProcess process, int[] typ)
        {
            return Container.Resolve<IRdStructureRepository>().GetDigressions(process, typ);
        }

        public static List<RdClasses> GetRdClasses()
        {
            return Container.Resolve<IRdStructureRepository>().GetRdClasses();
        }

        public static RdClasses GetRdClasses(int class_id)
        {
            return Container.Resolve<IRdStructureRepository>().GetRdClasses(class_id);
        }

        public static object GetAdmDirection(List<Int64> directionIDs)
        {
            return Container.Resolve<IRdStructureRepository>().GetAdmDirection(directionIDs);
        }

        public static List<MainParametersProcess> GetMainParametersProcess()
        {
            return Container.Resolve<IRdStructureRepository>().GetMainParametersProcess();
        }

        public static List<RdProfile> GetRdTablesByKM(MainParametersProcess tripProcess, float stKM, float fnKM)
        {
            return Container.Resolve<IRdStructureRepository>().GetRdTablesByKM(tripProcess, stKM, fnKM);
        }

        public static MainParametersProcess GetMainParametersProcess(Int64 process_id)
        {
            return Container.Resolve<IRdStructureRepository>().GetMainParametersProcess(process_id);
        }

        public static List<MainParametersProcess> GetMainParametersProcesses(ReportPeriod period, Int64 distance_id, bool all = false)
        {
            return Container.Resolve<IRdStructureRepository>().GetMainParametersProcesses(period, distance_id, all);
        }

        public static List<MainParametersProcess> GetMainParametersProcess(ReportPeriod period, string distanceName)
        {
            return Container.Resolve<IRdStructureRepository>().GetMainParametersProcess(period, distanceName);
        }

        public static object GetS3(Int64 processId, string digName = null)
        {
            return Container.Resolve<IRdStructureRepository>().GetS3(processId, digName);
        }
        public static object GetS3all(Int64 processId, string pch, long trackId = -1)
        {
            return Container.Resolve<IRdStructureRepository>().GetS3all(processId, pch, trackId);
        }


        public static object GetS3ForKm(Int64 processId, int km)
        {
            return Container.Resolve<IRdStructureRepository>().GetS3ForKm(processId, km);
        }


        public static object GetS3(Int64 processId, int type, string distanceName)
        {
            return Container.Resolve<IRdStructureRepository>().GetS3(processId, type, distanceName);
        }

        public static object GetS3(Int64 processId, int type)
        {
            return Container.Resolve<IRdStructureRepository>().GetS3(processId, type);
        }
        public static object GetDBD(Int64 processId)
        {
            return Container.Resolve<IRdStructureRepository>().GetDBD(processId);
        }

        public static object GetBedemost(Int64 processId)
        {
            return Container.Resolve<IRdStructureRepository>().GetBedemost(processId);
        }

        public static object GetRdProfileObjects(long trackId, DateTime date, int type, int start_km, int start_m, int final_km, int final_m)
        {
            return Container.Resolve<IRdStructureRepository>().GetRdProfileObjects(trackId, date, type, start_km, start_m, final_km, final_m);
        }

        public static object GetRdTables(MainParametersProcess process, int type_id)
        {
            return Container.Resolve<IRdStructureRepository>().GetRdTables(process, type_id);
        }

        public static bool CleanTables(int type)
        {
            return Container.Resolve<IRdStructureRepository>().CleanTables(type);
        }
        public static List<MainParametersProcess> GetProcess(ReportPeriod period, long distanceId, ProcessType processType)
        {
            return Container.Resolve<IRdStructureRepository>().GetProcess(period, distanceId, processType);
        }
        public static List<RailFastener> GetBadRailFasteners(long tripId, bool orderBySide, string code, object trackName, int km = -1)
        {
            return Container.Resolve<IRdStructureRepository>().GetBadRailFasteners(tripId, orderBySide, code, trackName, km);
        }

        public static List<Sleepers> GetSleeper(long trackId, int km, int meter, int start, int final, Threat threat)
        {
            return Container.Resolve<IRdStructureRepository>().GetSleeper(trackId, km, meter, start, final, threat);
        }

        public static List<Sleepers> GetSleepers()
        {
            return Container.Resolve<IRdStructureRepository>().GetSleepers();
        }
        public static CurveParams GetCurveParams(long curveId)
        {
            return Container.Resolve<IRdStructureRepository>().GetCurveParams(curveId);
        }

        public static SiteInfo GetSiteInfo(long trackId, int startKm, int finalKm)
        {
            return Container.Resolve<IRdStructureRepository>().GetSiteInfo(trackId, startKm, finalKm);
        }

        public static List<AdmTrack> GetTracksOnTrip(long tripId)
        {
            return Container.Resolve<IRdStructureRepository>().GetTracksOnTrip(tripId);
        }

        public static List<Trips> GetTripsOnDistance(long distanceId, ReportPeriod period)
        {
            return Container.Resolve<IRdStructureRepository>().GetTripsOnDistance(distanceId, period);
        }

        public static List<Curve> GetCurvesAsTripElems(long trackId, DateTime date, int start_km, int start_m, int final_km, int final_m)
        {
            return Container.Resolve<IRdStructureRepository>().GetCurvesAsTripElems(trackId, date, start_km, start_m, final_km, final_m);
        }

        public static object GetTripSections(long trackId, DateTime date, int type)
        {
            return Container.Resolve<IRdStructureRepository>().GetTripSections(trackId, date, type);
        }

        public static object GetTripSections(long trackId, DateTime date, int start_km, int start_m, int final_km, int final_m, int type)
        {
            return Container.Resolve<IRdStructureRepository>().GetTripSections(trackId, date, start_km, start_m, final_km, final_m, type);
        }
        public static List<Trips> GetTrips(int count = 20)
        {
            return Container.Resolve<IRdStructureRepository>().GetTrips(count);
        }
        public static List<Trips> GetTripFromFileId(int fileId)
        {
            return Container.Resolve<IRdStructureRepository>().GetTripFromFileId(fileId);
        }
        public static void RunRvoDataInsert()
        {
            Container.Resolve<IRdStructureRepository>().RunRvoDataInsert();
        }
        public static List<RdProfile> getRefrenceData(Dictionary<String, Object> p)
        {
            return Container.Resolve<IRdStructureRepository>().getRefrenceData(p);
        }
        public void SendEkasuiData(Trips trip, int km)
        {
            Container.Resolve<IRdStructureRepository>().SendEkasuiData(trip, km);
        }
        public static List<RDCurve> GetRDCurves(long curve_id, long trip_id)
        {
            return Container.Resolve<IRdStructureRepository>().GetRDCurves(curve_id, trip_id);
        }
        public static List<Escort> GetEscorts(long trip_id)
        {
            return Container.Resolve<IRdStructureRepository>().GetEscorts(trip_id);
        }
        /// <summary>
        /// Тапқан ескертпелерді қайтарады
        /// </summary>
        /// <param name="trip_id">саяхат идентификаторы</param>
        /// <param name="track_id">жол идентификаторы</param>
        /// <param name="km">километр нөмірі</param>
        /// <returns>ескертпелер</returns>
        public static List<DigressionMark> GetDigressionMarks(long trip_id, long track_id, int km)
        {
            return Container.Resolve<IRdStructureRepository>().GetDigressionMarks(trip_id, track_id, km);
        }
        public List<Kilometer> GetKilometersByTripId(long trip_id)
        {
            return Container.Resolve<IRdStructureRepository>().GetKilometersByTripId(trip_id);
        }
        /// <summary>
        ///Ескерту жазбасын түзету
        /// </summary>
        /// <param name="digression"></param>
        /// <param name="action"></param>
        /// <returns>сәтті болған жағдайда 1, әйтпесе -1</returns>
        public static int UpdateDigression(DigressionMark digression, Kilometer kilometer, RdAction action)
        {
            return Container.Resolve<IRdStructureRepository>().UpdateDigression(digression, kilometer, action);
        }
        /// <summary>
        /// PU32 үшін километрлерді қайтарады
        /// </summary>
        /// <param name="from">есептің бастапқы күні</param>
        /// <param name="to">есептің аяққы күні</param>
        /// <param name="distance_id">қашықтықтық идентификаторы</param>
        /// <param name="trip_type">саяхат түрі</param>
        /// <returns>қашықтық бойындағы белгілі бір кезеңде тексерілген километрлерді қайтарады</returns>
        public static List<Kilometer> GetPU32Kilometers(DateTime from, DateTime to, long distance_id, TripType trip_type)
        {
            return Container.Resolve<IRdStructureRepository>().GetPU32Kilometers(from, to, distance_id, trip_type);
        }
    }
}
