using ALARm.Core.Report;
using ALARm.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using Npgsql;
using System.IO;
using System.Globalization;

namespace ALARm.DataAccess
{
    public class MainTrackStructureRepository : IMainTrackStructureRepository
    {

        public bool DeleteMtoObject(Int64 id, int mtoObjectType)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                string tableName = string.Empty;
                switch (mtoObjectType)
                {
                    case MainTrackStructureConst.MtoDistSection:
                        tableName = "TPL_DIST_SECTION";
                        break;
                    case MainTrackStructureConst.MtoCrossTie:
                        tableName = "APR_CROSSTIE";
                        break;
                    case MainTrackStructureConst.MtoNonStandard:
                        tableName = "TPL_NST_KM";
                        break;
                    case MainTrackStructureConst.MtoTrackClass:
                        tableName = "APR_TRACKCLASS";
                        break;
                    case MainTrackStructureConst.MtoRailsBrace:
                        tableName = "APR_RAILS_BRACES";
                        break;
                    case MainTrackStructureConst.MtoNormaWidth:
                        tableName = "apr_norma_width";
                        break;
                    case MainTrackStructureConst.MtoSpeed:
                        tableName = "APR_SPEED";
                        break;
                    case MainTrackStructureConst.MtoTempSpeed:
                        tableName = "APR_TEMPSPEED";
                        break;
                    case MainTrackStructureConst.MtoElevation:
                        tableName = "APR_ELEVATION";
                        break;
                    case MainTrackStructureConst.MtoPdbSection:
                        tableName = "TPL_PDB_SECTION";
                        break;
                    case MainTrackStructureConst.MtoStationSection:
                        tableName = "TPL_STATION_SECTION";
                        break;
                    case MainTrackStructureConst.MtoCurve:
                        tableName = "APR_CURVE";
                        break;
                    case MainTrackStructureConst.MtoElCurve:
                        tableName = "APR_ELCURVE";
                        break;
                    case MainTrackStructureConst.MtoStCurve:
                        tableName = "APR_STCURVE";
                        break;
                    case MainTrackStructureConst.MtoStraighteningThread:
                        tableName = "APR_STRAIGHTENING_THREAD";
                        break;
                    case MainTrackStructureConst.MtoArtificialConstruction:
                        tableName = "APR_ARTIFICIAL_CONSTRUCTION";
                        break;
                    case MainTrackStructureConst.MtoSwitch:
                        tableName = "TPL_SWITCH";
                        break;
                    case MainTrackStructureConst.MtoLongRails:
                        tableName = "APR_LONG_RAILS";
                        break;
                    case MainTrackStructureConst.MtoRailSection:
                        tableName = "APR_RAILS_SECTIONS";
                        break;
                    case MainTrackStructureConst.MtoNonExtKm:
                        tableName = "TPL_NON_EXT_KM";
                        break;
                    case MainTrackStructureConst.MtoProfileObject:
                        tableName = "tpl_profile_object";
                        break;
                    case MainTrackStructureConst.MtoChamJoint:
                        tableName = "apr_cham_joint";
                        break;
                    case MainTrackStructureConst.MtoProfmarks:
                        tableName = "apr_profmarks";
                        break;
                    case MainTrackStructureConst.MtoTraffic:
                        tableName = "apr_traffic";
                        break;
                    case MainTrackStructureConst.MtoWaycat:
                        tableName = "apr_waycat";
                        break;
                    case MainTrackStructureConst.MtoRefPoint:
                        tableName = "apr_ref_point";
                        break;
                    case MainTrackStructureConst.MtoRepairProject:
                        tableName = "repair_project";
                        break;
                    case MainTrackStructureConst.MtoRFID:
                        tableName = "apr_rfid";
                        break;
                    case MainTrackStructureConst.MtoCheckSection:
                        tableName = "tpl_check_sections";
                        break;
                    case MainTrackStructureConst.MtoCommunication:
                        tableName = "apr_communication";
                        break;
                    case MainTrackStructureConst.MtoCoordinateGNSS:
                        tableName = "apr_coordinate_gnss";
                        break;
                    case MainTrackStructureConst.MtoDefectsEarth:
                        tableName = "apr_defects_earth";
                        break;
                    case MainTrackStructureConst.MtoDistanceBetweenTracks:
                        tableName = "tpl_distance_between_tracks";
                        break;
                    case MainTrackStructureConst.Fragments:
                        tableName = "fragments";
                        break;
                    case MainTrackStructureConst.MtoDeep:
                        tableName = "tpl_deep";
                        break;
                    case MainTrackStructureConst.MtoBallastType:
                        tableName = "apr_ballast";
                        break;
                    case MainTrackStructureConst.MtoDimension:
                        tableName = "apr_dimension";
                        break;
                }
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return (tableName != string.Empty) && (db.Execute("Delete from " + tableName + " where ID=@obID", new { obID = id }, commandType: CommandType.Text) != 0);
            }
        }

        public static object GetKilometersOfFragment(object fragment, DateTime today, object travel_Direction, object id)
        {
            throw new NotImplementedException();
        }

        public bool DeletePeriod(Int64 periodId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                int result = db.Execute("Delete from TPL_PERIOD where ID=@TrackID", new { TrackID = periodId }, commandType: CommandType.Text);
                return result != 0;
            }

        }

        public List<Catalog> GetCatalog(int mtoObjectType)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string catalogTableName = string.Empty;
                switch (mtoObjectType) {
                    case MainTrackStructureConst.MtoCrossTie:
                        catalogTableName = "CAT_CROSSTIE_TYPE";
                        break;
                    case MainTrackStructureConst.MtoTrackClass:
                        catalogTableName = "CAT_TRACKCLASS";
                        break;
                    case MainTrackStructureConst.MtoRailsBrace:
                        catalogTableName = "CAT_BRACE_TYPE";
                        break;
                    case MainTrackStructureConst.MtoTempSpeed:
                        catalogTableName = "CAT_TEMPSPEED_REASON";
                        break;
                    case MainTrackStructureConst.MtoElevation:
                        catalogTableName = "CAT_NORMA_LEVEL";
                        break;
                    case MainTrackStructureConst.MtoStationSection:
                        return db.Query<Catalog>("Select * from CAT_POINT_OBJECT_TYPE where ID<9 or id=23 or id=107 OR ID=35", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.CatSide:
                        catalogTableName = "CAT_SIDE";
                        break;
                    case MainTrackStructureConst.MtoArtificialConstruction:
                        catalogTableName = "CAT_ARTIFICIAL_CONSTRUCTION";
                        break;
                    case MainTrackStructureConst.MtoSwitchMark:
                        catalogTableName = "CAT_CROSS_MARK";
                        break;
                    case MainTrackStructureConst.MtoSwitchPoint:
                        return db.Query<Catalog>("Select * from CAT_POINT_OBJECT_TYPE where (ID>=103 and ID<=105) or id=108 or ID=35", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoSwitchDir:
                        catalogTableName = "CAT_SWITCH_DIR";
                        break;
                    case MainTrackStructureConst.MtoRailSection:
                        catalogTableName = "CAT_RAILS_TYPE";
                        break;
                    case MainTrackStructureConst.MtoProfileObject:
                        return db.Query<Catalog>("Select * from CAT_POINT_OBJECT_TYPE where ID=26", commandType: CommandType.Text).ToList();
                    /*catalogTableName = "cat_point_object_type";
                    break;*/
                    case MainTrackStructureConst.MtoWaycat:
                        catalogTableName = "cat_waycat";
                        break;
                    case MainTrackStructureConst.MtoRepairProject:
                        catalogTableName = "cat_repair_type";
                        break;
                    case MainTrackStructureConst.MtoAcceptType:
                        catalogTableName = "cat_accept_type";
                        break;
                    case MainTrackStructureConst.MtoLongRails:
                        catalogTableName = "cat_longrails";
                        break;
                    case MainTrackStructureConst.MtoChamJoint:
                        catalogTableName = "cat_cham_joint";
                        break;
                    case MainTrackStructureConst.MtoDefectsEarth:
                        catalogTableName = "cat_defects_earth";
                        break;
                    case MainTrackStructureConst.MtoCommunication:
                        return db.Query<Catalog>("Select * from CAT_POINT_OBJECT_TYPE where (ID>=85 and ID<=102) or ID=35", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoDimension:
                        return db.Query<Catalog>("Select * from CAT_POINT_OBJECT_TYPE where (ID>=106 and ID<=107)", commandType: CommandType.Text).ToList();
                }
                return catalogTableName.Equals(string.Empty) ? new List<Catalog>() : db.Query<Catalog>("Select * from " + catalogTableName, commandType: CommandType.Text).ToList();

            }
        }

        public object GetMtoObjects(Int64 periodId, int mtoObjectType)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (mtoObjectType) {
                    case MainTrackStructureConst.MtoDistSection:
                        return db.Query<DistSection>("Select ar.NAME as road, (ad.NAME) as distance, tsd.* from TPL_DIST_SECTION as tsd " +
                            "INNER JOIN ADM_DISTANCE as ad on ad.ID = tsd.ADM_DISTANCE_ID " +
                            "INNER JOIN ADM_NOD as an on an.ID = ad.ADM_NOD_ID " +
                            "INNER JOIN ADM_ROAD as ar on ar.ID = an.ADM_ROAD_ID " +
                            "where PERIOD_ID=" + periodId + " order by tsd.start_km * 1000 + tsd.start_m, tsd.final_km * 1000 + tsd.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoPdbSection:
                        return db.Query<PdbSection>("Select ar.NAME as road, (ad.NAME) as distance, ('ПЧУ-' || apc.CODE || '/ПД-' || apd.CODE || '/ПДБ-' || ap.CODE) as Pdb, tps.* from TPL_PDB_SECTION as tps " +
                            "INNER JOIN  ADM_PDB as ap on ap.ID = tps.ADM_PDB_ID " +
                            "INNER JOIN ADM_PD as apd on apd.ID = ap.ADM_PD_ID " +
                            "INNER JOIN ADM_PCHU as apc on apc.ID = apd.ADM_PCHU_ID " +
                            "INNER JOIN ADM_DISTANCE as ad on ad.ID = apc.ADM_DISTANCE_ID " +
                            "INNER JOIN ADM_NOD as an on an.ID = ad.ADM_NOD_ID " +
                            "INNER JOIN ADM_ROAD as ar on ar.ID = an.ADM_ROAD_ID " +
                            "where PERIOD_ID=" + periodId + " order by tps.start_km * 1000 + tps.start_m, tps.final_km * 1000 + tps.final_m", commandType: CommandType.Text).ToList();
                        //station.type_id as point_id
                    case MainTrackStructureConst.MtoStationSection:
                        return db.Query<StationSection>("Select ar.NAME as road, an.NAME as nod, ast.NAME as station, " +
                            "cpot.NAME as point, tss.* from TPL_STATION_SECTION as tss " +
                            "INNER JOIN ADM_STATION as ast on ast.ID = tss.STATION_ID " +
                            "INNER JOIN ADM_NOD as an on an.ID = ast.ADM_NOD_ID " +
                            "INNER JOIN ADM_ROAD as ar on ar.ID = an.ADM_ROAD_ID " +
                            "INNER JOIN CAT_POINT_OBJECT_TYPE as cpot on cpot.ID = tss.POINT_ID " +
                            "where PERIOD_ID=" + periodId + " order by tss.start_km * 1000 + tss.start_m, tss.final_km * 1000 + tss.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoCrossTie:
                        return db.Query<CrossTie>("Select cct.NAME as CrossTie_type, ac.* from APR_CROSSTIE as ac " +
                           "INNER JOIN CAT_CROSSTIE_TYPE as cct on cct.ID = ac.CROSSTIE_TYPE_ID " +
                           "where ac.PERIOD_ID=" + periodId + " order by ac.start_km * 1000 + ac.start_m, ac.final_km * 1000 + ac.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoTrackClass:
                        return db.Query<TrackClass>("Select ctc.NAME as Class_Type, atc.* from APR_TRACKCLASS as atc " +
                           "INNER JOIN CAT_TRACKCLASS as ctc on ctc.ID = atc.CLASS_ID " +
                            "where atc.PERIOD_ID=" + periodId + " order by atc.start_km * 1000 + atc.start_m, atc.final_km * 1000 + atc.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoRailsBrace:
                        return db.Query<RailsBrace>("Select cbt.NAME as Brace_Type, arb.* from APR_RAILS_BRACES as arb " +
                           "INNER JOIN CAT_BRACE_TYPE as cbt on cbt.ID = arb.BRACE_TYPE_ID " +
                           "where arb.PERIOD_ID=" + periodId + " order by arb.start_km * 1000 + arb.start_m, arb.final_km * 1000 + arb.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoNormaWidth:
                        return db.Query<NormaWidth>("Select anw.* from APR_NORMA_WIDTH as anw " +
                           "where anw.PERIOD_ID=" + periodId + " order by anw.start_km * 1000 + anw.start_m, anw.final_km * 1000 + anw.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoSpeed:
                        return db.Query<Speed>("Select * from APR_SPEED " +
                           "where PERIOD_ID=" + periodId + " order by start_km * 1000 + start_m, final_km * 1000 + final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoTempSpeed:
                        return db.Query<TempSpeed>("Select ctr.NAME as Reason, ats.* from APR_TEMPSPEED as ats " +
                            "INNER JOIN CAT_TEMPSPEED_REASON as ctr on ctr.ID = ats.REASON_ID " +
                           "where ats.PERIOD_ID=" + periodId, commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoElevation:
                        return db.Query<Elevation>("Select cnl.NAME as Side, ae.* from APR_ELEVATION as ae " +
                            "INNER JOIN CAT_NORMA_LEVEL as cnl on cnl.ID = ae.LEVEL_ID " +
                           "where ae.PERIOD_ID=" + periodId + " order by ae.start_km * 1000 + ae.start_m, ae.final_km * 1000 + ae.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoCurve:
                        return db.Query<Curve>("Select cs.NAME as Side, acu.* from APR_CURVE as acu " +
                            "INNER JOIN CAT_SIDE as cs on cs.ID = acu.SIDE_ID " +
                           "where acu.PERIOD_ID=" + periodId + " order by acu.start_km * 1000 + acu.start_m, acu.final_km * 1000 + acu.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoElCurve:
                        return db.Query<ElCurve>("Select aec.* from APR_ELCURVE as aec " +
                            "where aec.CURVE_ID=" + periodId + " order by aec.start_km * 1000 + aec.start_m, aec.final_km * 1000 + aec.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoStCurve:
                        return db.Query<StCurve>("Select stcurve.* from apr_stcurve as stcurve " +
                            "where stcurve.CURVE_ID=" + periodId + " order by stcurve.start_km * 1000 + stcurve.start_m, stcurve.final_km * 1000 + stcurve.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoStraighteningThread:
                        return db.Query<StraighteningThread>("Select cat.NAME as side, cst.* from APR_STRAIGHTENING_THREAD as cst " +
                           "INNER JOIN CAT_SIDE as cat on cat.ID = cst.SIDE_ID " +
                           "where cst.PERIOD_ID=" + periodId + " order by cst.start_km * 1000 + cst.start_m, cst.final_km * 1000 + cst.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoArtificialConstruction:
                        return db.Query<ArtificialConstruction>($@"Select cat.NAME as type, aac.*, startcoords.km as start_km, startcoords.meter as start_m, finalcoords.km as final_km, finalcoords.meter as final_m from APR_ARTIFICIAL_CONSTRUCTION as aac 
                           INNER JOIN CAT_ARTIFICIAL_CONSTRUCTION as cat on cat.ID = aac.TYPE_ID
                           inner join tpl_period aperiod on aperiod.id = aac.period_id
                           inner join gettablecoordbylen(aac.km, aac.meter, aac.len / -2, aperiod.adm_track_id, aperiod.start_date) as startcoords on true
                           inner join gettablecoordbylen(aac.km, aac.meter, aac.len / 2, aperiod.adm_track_id, aperiod.start_date) as finalcoords on true
                           where aac.PERIOD_ID={periodId} order by aac.km * 1000 + aac.meter", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoSwitch:
                        return db.Query<Switch>("Select station.name as Station, cat.NAME as dir, smark.MARK as mark, cpot.NAME as point,  " +
                           "sside.NAME as side, smark.LEN as legnth, tsw.* from TPL_SWITCH as tsw " +
                           "INNER JOIN CAT_SWITCH_DIR as cat on cat.ID = tsw.DIR_ID " +
                           "INNER JOIN CAT_POINT_OBJECT_TYPE as cpot on cpot.ID = tsw.POINT_ID " +
                           "INNER JOIN CAT_CROSS_MARK as smark on smark.ID = tsw.MARK_ID " +
                           "INNER JOIN CAT_SIDE as sside on sside.ID = tsw.SIDE_ID " +
                           "inner join adm_station as station on station.id = tsw.station_id " +
                           "where tsw.PERIOD_ID=" + periodId + " order by tsw.km * 1000 + tsw.meter", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoLongRails:
                        return db.Query<LongRails>("Select cl.name as Type, acu.* from APR_LONG_RAILS as acu " +
                            "inner join cat_longrails as cl on cl.id = acu.type_id " +
                            "where acu.PERIOD_ID=" + periodId + " order by acu.start_km * 1000 + acu.start_m, acu.final_km * 1000 + acu.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoRailSection:
                        return db.Query<RailsSections>("Select cbt.NAME as Type, arb.* from APR_RAILS_SECTIONS as arb " +
                           "INNER JOIN CAT_RAILS_TYPE as cbt on cbt.ID = arb.TYPE_ID " +
                           "where arb.PERIOD_ID=" + periodId + " order by arb.start_km * 1000 + arb.start_m, arb.final_km * 1000 + arb.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoNonExtKm:
                        return db.Query<NonExtKm>("Select * from TPL_NON_EXT_KM where PERIOD_ID=" + periodId + " order by km", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoProfileObject:
                        return db.Query<ProfileObject>("SELECT cs.NAME as Side, cpot.NAME as Object_type, tpo.* FROM TPL_PROFILE_OBJECT as tpo " +
                            "INNER JOIN CAT_POINT_OBJECT_TYPE as cpot on cpot.ID = tpo.OBJECT_ID " +
                            "INNER JOIN CAT_SIDE as cs on cs.ID = tpo.SIDE_ID " +
                            "WHERE tpo.PERIOD_ID=" + periodId + " order by tpo.km * 1000 + tpo.meter", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoNonStandard:
                        return db.Query<NonstandardKm>("Select * from TPL_NST_KM where PERIOD_ID=" + periodId + " order by km", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoChamJoint:
                        return db.Query<ChamJoint>(@"Select chamjoint.*, catalog.name as type from apr_cham_joint chamjoint
                            inner join cat_cham_joint catalog on catalog.id = chamjoint.type_id
                            where chamjoint.period_id=" + periodId + " order by chamjoint.start_km * 1000 + chamjoint.start_m, chamjoint.final_km * 1000 + chamjoint.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoProfmarks:
                        return db.Query<Profmarks>(@"Select * from apr_profmarks
                            where PERIOD_ID=" + periodId + " order by km * 1000 + meter", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoTraffic:
                        return db.Query<MtoTraffic>(@"Select * from apr_traffic 
                            where PERIOD_ID=" + periodId + " order by start_km * 1000 + start_m, final_km * 1000 + final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoWaycat:
                        return db.Query<Waycat>(@"Select waycat.*, catalog.name as type from apr_waycat waycat 
                           INNER JOIN cat_waycat catalog on catalog.ID = waycat.TYPE_ID
                           where waycat.PERIOD_ID=" + periodId + " order by waycat.start_km * 1000 + waycat.start_m, waycat.final_km * 1000 + waycat.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoRefPoint:
                        return db.Query<RefPoint>(@"Select * from apr_ref_point
                            where PERIOD_ID=" + periodId + " order by km * 1000 + meter", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoRepairProject:
                        return db.Query<RepairProject>(@"Select repair.*, catalog.name as type, accept.name as Accept from repair_project repair
                            inner join cat_repair_type catalog on catalog.id = repair.type_id
                            inner join cat_accept_type accept on accept.id = repair.accept_id
                            where repair.adm_track_id=" + periodId + " order by repair.start_km * 1000 + repair.start_m, repair.final_km * 1000 + repair.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoRFID:
                        return db.Query<Rfid>(@"select * from apr_rfid
                            where period_id=" + periodId + " order by km * 1000 + meter", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoCheckSection:
                        return db.Query<CheckSection>(@"select * from tpl_check_sections
                            where period_id=" + periodId + " order by start_km * 1000 + start_m, final_km * 1000 + final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoCommunication:
                        return db.Query<Communication>(@"select commun.*, objtype.name as object from apr_communication commun
                            inner join cat_point_object_type objtype on objtype.id = commun.object_id
                            where commun.period_id=" + periodId + " order by commun.km * 1000 + commun.meter", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoCoordinateGNSS:
                        return db.Query<CoordinateGNSS>(@"select * from apr_coordinate_gnss
                            where period_id=" + periodId + " order by km * 1000 + meter", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoDefectsEarth:
                        return db.Query<DefectsEarth>(@"select defects.*, objtype.name as type from apr_defects_earth defects
                            inner join cat_defects_earth objtype on objtype.id = defects.type_id
                            where defects.period_id=" + periodId + " order by defects.start_km * 1000 + defects.start_m, defects.final_km * 1000 + defects.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoDistanceBetweenTracks:
                        return db.Query<DistanceBetweenTracks>(@"select distance.*, leftt.code as left_track, rightt.code as right_track from tpl_distance_between_tracks distance
                            inner join adm_track leftt on leftt.id = distance.left_adm_track_id
                            inner join adm_track rightt on rightt.id = distance.right_adm_track_id
                            where distance.period_id=" + periodId + " order by distance.start_km * 1000 + distance.start_m, distance.final_km * 1000 + distance.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoDeep:
                        return db.Query<Deep>(@"select * from tpl_deep
                            where period_id=" + periodId + " order by start_km * 1000 + start_m, final_km * 1000 + final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoBallastType:
                        return db.Query<BallastType>($@"select * from apr_ballast
                            where period_id= { periodId } order by start_km * 1000 + start_m, final_km * 1000 + final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoDimension:
                        return db.Query<Dimension>($@"select dimension.*, objtype.name as type from apr_dimension dimension
                            inner join cat_point_object_type objtype on objtype.id = dimension.type_id
                            where dimension.period_id={periodId} order by dimension.start_km * 1000 + dimension.start_m, dimension.final_km * 1000 + dimension.final_m", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoRdCurve:
                        return db.Query<RDCurve>($@"SELECT * from rd_curve
                                                    WHERE curve_id = {periodId}
                                                    ORDER BY km, m,curve_id", commandType: CommandType.Text).ToList();
                    default: return new List<MainTrackObject>();
                }
            }
        }

        public object GetCurves(Int64 parentId, int type, Period period = null)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (type)
                {
                    case MainTrackStructureConst.MtoCurve:
                        return db.Query<Curve>($@"select cs.name as side, aprc.*, tplp.start_date, tplp.final_date,   admt.id as track_id, admt.code as track_code, max(stcurve.radius) as radius, coalesce(speed.passenger, -1) as passspeed
                            from apr_curve aprc 
                            inner join apr_stcurve stcurve on stcurve.curve_id = aprc.id
                            inner join cat_side cs on cs.id = aprc.side_id 
                            inner join tpl_period tplp on tplp.id = aprc.period_id 
                            inner join adm_track admt on admt.id = tplp.adm_track_id 
                            left join tpl_period speedperiod on speedperiod.adm_track_id = admt.id and speedperiod.mto_type = 6 and is_newest_period(speedperiod.id, speedperiod.mto_type, admt.id)
                            left join apr_speed speed on speed.period_id = speedperiod.id and
                                isbelong(aprc.start_km, aprc.start_m, speed.start_km, speed.start_m, speed.final_km, speed.final_m)
                            inner join tpl_period section_period on admt.id = section_period.adm_track_id
                            inner join tpl_dist_section section on section_period.id = section.period_id and 
                                (isbelong(aprc.start_km, aprc.start_m, section.start_km, section.start_m, section.final_km, section.final_m) or 
                                isbelong(aprc.final_km, aprc.final_m, section.start_km, section.start_m, section.final_km, section.final_m))  
                            where { (period != null ? $" daterange(tplp.start_date, tplp.final_date) && daterange({period.Start_Date},{period.Final_Date}) and " : "") } section.adm_distance_id = {parentId}
                            group by aprc.id, cs.id, speed.id, admt.id, admt.code, tplp.start_date, tplp.final_date
                            order by aprc.id", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoElCurve:
                        return db.Query<ElCurve>("Select aec.* from APR_ELCURVE as aec " +
                                                 "where aec.CURVE_ID=" + parentId, commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoStCurve:
                        return db.Query<StCurve>(@"select * from apr_stcurve
                            where curve_id=" + parentId.ToString(), commandType: CommandType.Text).ToList();
                    default: return new List<MainTrackObject>();
                }
            }
        }

        /// <summary>
        /// Объект по координате, дате и пути
        /// </summary>
        /// <param name="date">Дата актуальная</param>
        /// <param name="coord">Координата (km * 10000 + meter)</param>
        /// <param name="mtoObjectType">Тип объекта</param>
        /// <param name="trackId">ID пути</param>
        /// <returns>Лист объектов которые обхватывают координату (включительно)</returns>
        public object GetMtoObjectsByCoord(DateTime date, int coord, int mtoObjectType, long trackId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (mtoObjectType)
                {
                    //Класс пути (один объект)
                    case MainTrackStructureConst.MtoTrackClass:
                        return db.Query<TrackClass>($@"select trackclass.*, classtype.name as class_type from apr_trackclass trackclass
                            inner join tpl_period tperiod on tperiod.id = trackclass.period_id and ('{date.ToString("dd-MM-yyyy")}' between tperiod.start_date and tperiod.final_date)
                            inner join adm_track track on track.id = tperiod.adm_track_id and track.id = {trackId}
                            inner join cat_trackclass classtype on classtype.id = trackclass.class_id
                            where {coord} between trackclass.start_km * 10000 + trackclass.start_m and trackclass.final_km * 10000 + trackclass.final_m").ToList();
                    //Секция ПДБ (один объект)
                    case MainTrackStructureConst.MtoPdbSection:
                        return db.Query<PdbSection>($@"select pdb.*, ('ПЧУ-' || adm_pchu.CODE || '/ПД-' || adm_pd.CODE || '/ПДБ-' || adm_pdb.CODE) as Pdb from tpl_pdb_section pdb
                            inner join tpl_period tperiod on tperiod.id = pdb.period_id and ('{date.ToString("dd-MM-yyyy")}' between tperiod.start_date and tperiod.final_date)
                            inner join adm_track track on track.id = tperiod.adm_track_id and track.id = {trackId}
                            inner join adm_pdb on adm_pdb.id = pdb.adm_pdb_id
                            inner join adm_pd on adm_pd.id = adm_pdb.adm_pd_id
                            inner join adm_pchu on adm_pchu.id = adm_pd.adm_pchu_id
                            where {coord} between pdb.start_km * 10000 + pdb.start_m and pdb.final_km * 10000 + pdb.final_m").ToList();

                    case MainTrackStructureConst.MtoStCurve:

                        return db.Query<StCurve>(@"Select cs.NAME as Side, stcurve.* from apr_stcurve as stcurve
                            INNER JOIN APR_CURVE as acu on acu.id = stcurve.curve_id
                            INNER JOIN CAT_SIDE as cs on cs.ID = acu.SIDE_ID 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = acu.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.id = @trackId 
                            and  @nkm between stcurve.start_km  and stcurve.final_km ",
                            new { travelDate = date, trackId = trackId, nkm = coord }).ToList();
                    case MainTrackStructureConst.MtoNormaWidth:
                        return db.Query<NormaWidth>(@"Select aac.* from APR_NORMA_WIDTH as aac 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = aac.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.id = @trackId 
                            and aac.START_KM <= @nkm and aac.FINAL_KM >= @nkm ", new { travelDate = date, trackId = trackId, nkm = coord }).ToList();
                    //Секция станции
                    //Один объект если координата внутри станции или станция неизвестна
                    //Два объекта если координата на перегоне (между двумя станциями)
                    case MainTrackStructureConst.MtoStationSection:
                        List<StationSection> stationSections = db.Query<StationSection>($@"select ssection.*, station.type_id as point_id, station.name as station from tpl_station_section ssection
                            inner join tpl_period tperiod on tperiod.id = ssection.period_id and ('{date.ToString("dd-MM-yyyy")}' between tperiod.start_date and tperiod.final_date)
                            inner join adm_track track on track.id = tperiod.adm_track_id and track.id = {trackId}
                            inner join adm_station station on station.id = ssection.station_id
                            order by axis_km, axis_m").ToList();

                        if (stationSections.Where(s => s.Start_Km * 10000 + s.Start_M <= coord && s.Final_Km * 10000 + s.Final_M >= coord).Any())
                            return stationSections.Where(s => s.Start_Km * 10000 + s.Start_M <= coord && s.Final_Km * 10000 + s.Final_M >= coord).ToList();
                        else
                        {
                            List<StationSection> stationSections1 = new List<StationSection>();

                            if (stationSections.Where(s => s.Final_Km * 10000 + s.Final_M <= coord).Any())
                                stationSections1.Add(stationSections.Where(s => s.Final_Km * 10000 + s.Final_M <= coord).Last());

                            if (stationSections.Where(s => s.Start_Km * 10000 + s.Start_M >= coord).Any())
                                stationSections1.Add(stationSections.Where(s => s.Start_Km * 10000 + s.Start_M >= coord).First());

                            if (!stationSections1.Any())
                                stationSections1.Add(new StationSection { Station = "Неизвестный" });

                            return stationSections1;
                        }

                    default: return new List<MainTrackObject>();
                }
            }
        }

        /// <summary>
        /// Объект по координатам, дате и пути
        /// </summary>
        /// <param name="date">Дата актуальная</param>
        /// <param name="coordStart">Координата начала (km * 10000 + meter)</param>
        /// <param name="coordFinal">Координата конца (km * 10000 + meter)</param>
        /// <param name="mtoObjectType">Тип объекта</param>
        /// <param name="trackId">ID пути</param>
        /// <returns>Лист объектов которые обхватывают координаты (включительно)</returns>
        public object GetMtoObjectsByCoord(DateTime date, int coordStart, int coordFinal, int mtoObjectType, long trackId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (mtoObjectType)
                {
                    //Установленные скорости (лист объектов)
                    case MainTrackStructureConst.MtoSpeed:
                        return db.Query<Speed>($@"select speed.* from apr_speed speed
                            inner join tpl_period tperiod on tperiod.id = speed.period_id and ('{date.ToString("dd-MM-yyyy")}' between tperiod.start_date and tperiod.final_date)
                            inner join adm_track track on track.id = tperiod.adm_track_id and track.id = {trackId}
                            where speed.start_km * 10000 + speed.start_m <= {coordFinal} and speed.final_km * 10000 + speed.final_m >= {coordStart}
                            order by speed.start_km * 10000 + speed.start_m").ToList();
                    //Класс пути (лист объектов)
                    case MainTrackStructureConst.MtoTrackClass:
                        return db.Query<TrackClass>($@"select trackclass.*, classtype.name as class_type from apr_trackclass trackclass
                            inner join tpl_period tperiod on tperiod.id = trackclass.period_id and ('{date.ToString("dd-MM-yyyy")}' between tperiod.start_date and tperiod.final_date)
                            inner join adm_track track on track.id = tperiod.adm_track_id and track.id = {trackId}
                            inner join cat_trackclass classtype on classtype.id = trackclass.class_id
                            where trackclass.start_km * 10000 + trackclass.start_m <= {coordFinal} and trackclass.final_km * 10000 + trackclass.final_m >= {coordStart}
                            order by trackclass.start_km * 10000 + trackclass.start_m").ToList();
                    //Промежуточные скрепления (лист объектов)
                    case MainTrackStructureConst.MtoRailsBrace:
                        return db.Query<RailsBrace>($@"select railbrace.*, brace.name as Brace_Type from apr_rails_braces railbrace
                            inner join tpl_period tperiod on tperiod.id = railbrace.period_id and ('{date.ToString("dd-MM-yyyy")}' between tperiod.start_date and tperiod.final_date)
                            inner join adm_track track on track.id = tperiod.adm_track_id and track.id = {trackId}
                            inner join cat_brace_type brace on brace.id = railbrace.brace_type_id
                            where railbrace.start_km * 10000 + railbrace.start_m <= {coordFinal} and railbrace.final_km * 10000 + railbrace.final_m >= {coordStart}
                            order by railbrace.start_km * 10000 + railbrace.start_m").ToList();
                    //Тип рельсов (лист объектов)
                    case MainTrackStructureConst.MtoRailSection:
                        return db.Query<RailsSections>($@"select railsection.*, rails.name as type from apr_rails_sections railsection
                            inner join tpl_period tperiod on tperiod.id = railsection.period_id and ('{date.ToString("dd-MM-yyyy")}' between tperiod.start_date and tperiod.final_date)
                            inner join adm_track track on track.id = tperiod.adm_track_id and track.id = {trackId}
                            inner join cat_rails_type rails on rails.id = railsection.type_id
                            where railsection.start_km * 10000 + railsection.start_m <= {coordFinal} and railsection.final_km * 10000 + railsection.final_m >= {coordStart}
                            order by railsection.start_km * 10000 + railsection.start_m").ToList();
                    //Кривые участки пути (лист объектов)
                    case MainTrackStructureConst.MtoCurve:
                        List<Curve> curves = db.Query<Curve>($@"select curve.*, side.name as side from apr_curve curve
                            inner join tpl_period tperiod on tperiod.id = curve.period_id and ('{date.ToString("dd-MM-yyyy")}' between tperiod.start_date and tperiod.final_date)
                            inner join adm_track track on track.id = tperiod.adm_track_id and track.id = {trackId}
                            inner join cat_side side on side.id = curve.side_id
                            where curve.start_km * 10000 + curve.start_m <= {coordFinal} and curve.final_km * 10000 + curve.final_m >= {coordStart}
                            order by curve.start_km * 10000 + curve.start_m").ToList();

                        foreach (var curve in curves)
                        {
                            curve.Straightenings = db.Query<StCurve>($@"select stcurve.* from apr_stcurve stcurve
                                inner join apr_curve curve on curve.id = stcurve.curve_id
                                where curve.id = {curve.Id}
                                order by stcurve.start_km * 10000 + stcurve.start_m").ToList();

                            curve.Elevations = db.Query<ElCurve>($@"select elcurve.* from apr_elcurve elcurve
                                inner join apr_curve curve on curve.id = elcurve.curve_id
                                where curve.id = {curve.Id}
                                order by elcurve.start_km * 10000 + elcurve.start_m").ToList();
                        }

                        return curves;
                    //Кривые участки пути по рихтовке (лист объектов)
                    case MainTrackStructureConst.MtoStCurve:
                        return db.Query<StCurve>($@"select stcurve.* from apr_stcurve stcurve
                            inner join apr_curve curve on curve.id = stcurve.curve_id
                            inner join tpl_period tperiod on tperiod.id = curve.period_id and ('{date.ToString("dd-MM-yyyy")}' between tperiod.start_date and tperiod.final_date)
                            inner join adm_track track on track.id = tperiod.adm_track_id and track.id = {trackId}
                            where curve.start_km * 10000 + curve.start_m <= {coordFinal} and curve.final_km * 10000 + curve.final_m >= {coordStart}
                            order by curve.start_km * 10000 + curve.start_m").ToList();
                    //Шаблон пути
                    case MainTrackStructureConst.MtoNormaWidth:
                        return db.Query<NormaWidth>($@"select normawidth.start_km, normawidth.start_m, normawidth.final_km, normawidth.final_m, normawidth.norma_width from apr_norma_width normawidth
                            inner join tpl_period tperiod on tperiod.id = normawidth.period_id and ('{date.ToString("dd-MM-yyyy")}' between tperiod.start_date and tperiod.final_date)
                            inner join adm_track track on track.id = tperiod.adm_track_id and track.id = {trackId}
                            where normawidth.start_km * 10000 + normawidth.start_m <= {coordFinal} and normawidth.final_km * 10000 + normawidth.final_m >= {coordStart}
                            union all
                            select stcurve.start_km, stcurve.start_m, stcurve.final_km, stcurve.final_m, stcurve.width as norma_width from apr_stcurve stcurve
                            inner join apr_curve curve on curve.id = stcurve.curve_id
                            inner join tpl_period tperiod on tperiod.id = curve.period_id and ('{date.ToString("dd-MM-yyyy")}' between tperiod.start_date and tperiod.final_date)
                            inner join adm_track track on track.id = tperiod.adm_track_id and track.id = {trackId}
                            where curve.start_km * 10000 + curve.start_m <= {coordFinal} and curve.final_km * 10000 + curve.final_m >= {coordStart}
                            order by start_km * 10000 + start_m").ToList();

                    default: return new List<MainTrackObject>();
                }
            }
        }
        public object GetMtoObjectsByCoord(DateTime date, int nkm, int mtoObjectType, string directionCode, string trackNumber)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (nkm == 715)
                {
                    //check
                }
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (mtoObjectType)
                {
                    case MainTrackStructureConst.MtoDistSection:

                    case MainTrackStructureConst.MtoCurve:
                        return db.Query<Curve>(@"Select cs.NAME as Side, acu.* from APR_CURVE as acu 
                            INNER JOIN CAT_SIDE as cs on cs.ID = acu.SIDE_ID 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = acu.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum and adn.NAME = @directCode 
                            and acu.START_KM <= @ncurkm and acu.FINAL_KM >= @ncurkm order by acu.START_KM, acu.START_M", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = directionCode }).ToList();
                    case MainTrackStructureConst.MtoNonStandard:
                        return db.Query<NonstandardKm>("Select tnk.* from TPL_NST_KM as tnk " +
                            "INNER JOIN TPL_PERIOD as tp on tp.ID = tnk.PERIOD_ID " +
                            "INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID " +
                            "INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID " +
                            "WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE " +
                            "and atr.CODE = @trackNum and adn.NAME = @directCode " +
                            "and tnk.KM = " + nkm, new { travelDate = date, trackNum = trackNumber, directCode = directionCode }).ToList();
                    case MainTrackStructureConst.MtoStraighteningThread:
                        return db.Query<StraighteningThread>("Select cst.* from APR_STRAIGHTENING_THREAD as cst " +
                            "INNER JOIN TPL_PERIOD as tp on tp.ID = cst.PERIOD_ID " +
                            "INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID " +
                            "INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID " +
                            "WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE " +
                            "and atr.CODE = @trackNum and adn.NAME = @directCode " +
                            "and cst.START_KM <= @ncurkm and cst.FINAL_KM >= @ncurkm ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = directionCode }).ToList();
                    case MainTrackStructureConst.MtoArtificialConstruction:
                        return db.Query<ArtificialConstruction>(@"Select aac.* from APR_ARTIFICIAL_CONSTRUCTION as aac 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = aac.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            inner join gettablecoordbylen(aac.km, aac.meter, aac.len / -2, tp.adm_track_id, @travelDate) as startcoords on true
                            inner join gettablecoordbylen(aac.km, aac.meter, aac.len / 2, tp.adm_track_id, @travelDate) as finalcoords on true
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum and adn.NAME = @directCode 
                            and startcoords.km <= @ncurkm and finalcoords.km >= @ncurkm ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = directionCode }).ToList();
                    //шпалы
                    case MainTrackStructureConst.MtoCrossTie:
                        
                        var txt = $@"SELECT
	                                    aac.* ,cct.name
                                    FROM
	                                    APR_CROSSTIE AS aac
	                                    INNER JOIN cat_crosstie_type cct ON cct.ID = aac.crosstie_type_id
	                                    INNER JOIN TPL_PERIOD AS tp ON tp.ID = aac.PERIOD_ID
	                                    INNER JOIN ADM_TRACK AS atr ON atr.ID = tp.ADM_TRACK_ID
	                                    INNER JOIN ADM_DIRECTION AS adn ON adn.ID = atr.ADM_DIRECTION_ID 
                                    WHERE 
                                        ('{date.Date.ToShortDateString()}' BETWEEN tp.START_DATE and tp.FINAL_DATE )
                                        and atr.CODE = '{trackNumber}' and adn.NAME = '{directionCode}'
                                        and aac.START_KM <= {nkm} and aac.FINAL_KM >= {nkm} ";
                        return db.Query<CrossTie>(txt).ToList();
                    //скрепления
                    case MainTrackStructureConst.MtoRailsBrace:
                        directionCode = "00002";
                        try
                        {
                            directionCode = new List<String>(directionCode.Split("(".ToCharArray())).First();
                        }
                        catch(Exception e)
                        {

                        }
                        var txt2 = $@"SELECT
	                                    arb.* ,cbt.name
                                    FROM
	                                    apr_rails_braces arb
	                                    INNER JOIN CAT_BRACE_TYPE AS cbt ON cbt.ID = arb.brace_type_id
	                                    INNER JOIN TPL_PERIOD AS tp ON tp.ID = arb.PERIOD_ID
	                                    INNER JOIN ADM_TRACK AS atr ON atr.ID = tp.ADM_TRACK_ID
	                                    INNER JOIN ADM_DIRECTION AS adn ON adn.ID = atr.ADM_DIRECTION_ID 
                                    WHERE 
		                                ('{date.Date.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))}' BETWEEN tp.START_DATE and tp.FINAL_DATE )
		                                and atr.CODE = '{trackNumber}' and adn.NAME = '{directionCode}'
		                                and arb.START_KM <= {nkm} and arb.FINAL_KM >= {nkm} ";
                        return db.Query<RailsBrace>(txt2).ToList();
                    //тип рельса
                    case MainTrackStructureConst.MtoRailSection:
                        directionCode = "00002";
                        //directionCode = directionCode.Split('(').Any() ? directionCode.Split('(')[0] : directionCode;
                       
                        var txt3 = $@"SELECT
	                                    ars.* ,
	                                    crt.NAME 
                                    FROM
	                                    apr_rails_sections ars
	                                    INNER JOIN cat_rails_type crt ON crt.ID = ars.type_id
	                                    INNER JOIN TPL_PERIOD AS tp ON tp.ID = ars.PERIOD_ID
	                                    INNER JOIN ADM_TRACK AS atr ON atr.ID = tp.ADM_TRACK_ID
	                                    INNER JOIN ADM_DIRECTION AS adn ON adn.ID = atr.ADM_DIRECTION_ID 
                                    WHERE
	                                    ( '{date.Date.ToString("g", CultureInfo.CreateSpecificCulture("fr-BE"))}' BETWEEN tp.START_DATE AND tp.FINAL_DATE ) 
	                                    AND atr.CODE = '{trackNumber}' 
	                                    AND adn.NAME = '{directionCode}' 
	                                    AND ars.START_KM <= { nkm } 
	                                    AND ars.FINAL_KM >= { nkm } ";
                        return db.Query<RailsSections>(txt3).ToList();

                    case MainTrackStructureConst.MtoLongRails:
                        return db.Query<LongRails>(@"Select acu.* from APR_LONG_RAILS as acu 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = acu.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum and adn.NAME = @directCode 
                            and acu.START_KM <= @ncurkm and acu.FINAL_KM >= @ncurkm ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = directionCode }).ToList();
                    case MainTrackStructureConst.MtoSwitch:
                        return db.Query<Switch>(@"Select tsw.*, ccm.len as length from TPL_SWITCH as tsw 
                            INNER JOIN cat_cross_mark as ccm on tsw.mark_id = ccm.id
                            INNER JOIN TPL_PERIOD as tp on tp.ID = tsw.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum 
                            and adn.name = @directCode
                            and tsw.km = @ncurkm ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = directionCode }).ToList();
                    case MainTrackStructureConst.MtoProfileObject:
                        return db.Query<Switch>(@"Select tpo.* from tpl_profile_object as tpo 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = tpo.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum 
                            and adn.name = @directCode
                            and tpo.km = @ncurkm ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = directionCode }).ToList();

                    //case MainTrackStructureConst.Primech:
                    //    return db.Query<Speed>(@"Select aps.* from APR_SPEED as aps 
                    //        INNER JOIN TPL_PERIOD as tp on tp.ID = aps.PERIOD_ID 
                    //        INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                    //        INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                    //        WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                    //        and atr.CODE = @trackNum 
                    //        and adn.NAME = @directCode 
                    //        and @ncurkm between aps.start_km and aps.final_km ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = directionCode }).ToList();
                        



                    case MainTrackStructureConst.MtoSpeed:
                        return db.Query<Speed>(@"Select aps.* from APR_SPEED as aps 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = aps.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum 
                            and adn.NAME = @directCode 
                            and @ncurkm between aps.start_km and aps.final_km ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = directionCode }).ToList();
                    case MainTrackStructureConst.MtoCheckSection:
                        return db.Query<CheckSection>(@"Select tcs.* from tpl_check_sections as tcs 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = tcs.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum 
                            and adn.NAME = @directCode 
                            and @ncurkm between tcs.start_km and tcs.final_km ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = directionCode }).ToList();
                    case MainTrackStructureConst.MtoPdbSection:
                        return db.Query<PdbSection>(@"Select ap.ID, adc.code as distance, apc.CODE as pchu, apd.CODE as pd, ap.CODE as pdb, ap.chief_fullname as chief from ADM_PDB as ap 
                            INNER JOIN ADM_PD as apd on apd.ID = ap.ADM_PD_ID 
                            INNER JOIN ADM_PCHU as apc on apc.ID = apd.ADM_PCHU_ID 
                            INNER JOIN ADM_DISTANCE as adc on adc.ID = apc.ADM_DISTANCE_ID 
                            INNER JOIN tpl_pdb_section as tps on tps.adm_pdb_id = ap.id 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = tps.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum and adn.NAME = @directCode 
                            and @ncurkm between tps.start_km and tps.final_km 
                            ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = directionCode }).ToList();
                    
                    case MainTrackStructureConst.MtoTemperature:
                    default: 
                        return new List<MainTrackObject>();
                }
            }
        }

        public object GetMtoObjectsByCoordSpeeds(DateTime date, int nkm, int mtoObjectType,  long track_id,int meter)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (mtoObjectType)
                {
                    case MainTrackStructureConst.MtoSpeed:
                        var txt3 = $@"Select aps.*from APR_SPEED as aps
                            INNER JOIN TPL_PERIOD as tp on tp.ID = aps.PERIOD_ID
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID
                            WHERE '{date}' BETWEEN tp.START_DATE and tp.FINAL_DATE
                            and atr.CODE = '{track_id}'
                            and {nkm} between aps.start_km and aps.final_km
                            and {meter} between aps.start_m and aps.final_m ";

                return db.Query<Speed>(txt3).ToList();
                 
                
                    default:
                        return new List<MainTrackObject>();
                }
            }
        }
        public List<Temperature> GetTemp(long trip_id, long track_id, int km)
        {
            try
            {
                using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    return db.Query<Temperature>($@"
                            SELECT DISTINCT
	                            ROUND(AVG(rail_temp_kupe)) as kupe,
	                            ROUND(AVG(rail_temp_koridor)) as koridor
                            FROM
	                            outdata_{trip_id} 
                            WHERE
	                            km = {km} ").ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetTempError: " + e.Message);
                return null;
            }
        }

        public List<Gaps> GetIzoGaps(object trackNumber, long direction_id)
        {
            try
            {
                using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var query = $@"
                            SELECT
	                            --cs.NAME AS Side,
	                            cpot.NAME AS Primech,
	                            tpo.* ,
	                            tperiod.* ,
	                            track.* 
                            FROM
	                            TPL_PROFILE_OBJECT AS tpo
	                            INNER JOIN CAT_POINT_OBJECT_TYPE AS cpot ON cpot.ID = tpo.OBJECT_ID
	                            INNER JOIN CAT_SIDE AS cs ON cs.ID = tpo.SIDE_ID
	                            INNER JOIN tpl_period tperiod ON tperiod.ID = tpo.period_id
	                            INNER JOIN adm_track track ON track.ID = tperiod.adm_track_id 
                            WHERE
	                            track.code = '{trackNumber}' 
	                            AND adm_direction_id = {direction_id} 
                            ORDER BY
	                            tpo.km * 1000 + tpo.meter  ";
                    return db.Query<Gaps>(query).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetIzoGaps Error: " + e.Message);
                return null;
            }
        }

        public List<float> GetGaugesByCurve(List<MainParametersProcess> tripProcesses, Curve curve, string track)
        {
            try
            {
                using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var Glist =  db.Query<float> ($@"
                            SELECT DISTINCT
	                            gauge as gaugeValue
                            FROM
	                            outdata_{tripProcesses[0].Trip_id}
                            WHERE
                                km * 1000+meter >= {curve.Start_Km * 1000 + curve.Start_M} 
	                            AND km * 1000+meter <= {curve.Final_Km * 1000 + curve.Final_M}").ToList();
                    return Glist;
                }
            }
            catch
            {
                return null;
            }
        }

        public List<Period> GetPeriods(Int64 trackId, int mtoType)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Period>("Select * from TPL_PERIOD where ADM_TRACK_ID=" + trackId + " and MTO_TYPE=" + mtoType, commandType: CommandType.Text).ToList();
            }
        }

        public Int64 InsertObject(MainTrackObject mtoobject, int mtoObjectType)
        {
            using (var db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string procedure;
                var dp = new DynamicParameters();
                dp.Add("periodid", mtoobject.Period_Id);
                dp.Add("startkm", mtoobject.Start_Km);
                dp.Add("startm", mtoobject.Start_M);
                dp.Add("finalkm", mtoobject.Final_Km);
                dp.Add("finalm", mtoobject.Final_M);
                switch (mtoObjectType) {
                    case MainTrackStructureConst.MtoDistSection:
                        procedure = "insertsection";
                        dp.Add("distanceid", ((DistSection)mtoobject).DistanceId);
                        break;
                    case MainTrackStructureConst.MtoCrossTie:
                        procedure = "insertcrosstie";
                        dp.Add("crosstietypeid", ((CrossTie)mtoobject).Crosstie_type_id);
                        break;
                    case MainTrackStructureConst.MtoTrackClass:
                        procedure = "inserttrackclass";
                        dp.Add("classid", ((TrackClass)mtoobject).Class_Id);
                        break;
                    case MainTrackStructureConst.MtoRailsBrace:
                        procedure = "insertrailbrace";
                        dp.Add("bracetypeid", ((RailsBrace)mtoobject).Brace_Type_Id);
                        break;
                    case MainTrackStructureConst.MtoNormaWidth:
                        procedure = "insertnormawidth";
                        dp.Add("width", ((NormaWidth)mtoobject).Norma_Width);
                        break;
                    case MainTrackStructureConst.MtoSpeed:
                        procedure = "insertspeed";
                        dp.Add("passenger", ((Speed)mtoobject).Passenger);
                        dp.Add("freight", ((Speed)mtoobject).Freight);
                        dp.Add("empty_freight", ((Speed)mtoobject).Empty_Freight);
                        dp.Add("sapsan", ((Speed)mtoobject).Sapsan);
                        dp.Add("lastochka", ((Speed)mtoobject).Lastochka);
                        break;
                    case MainTrackStructureConst.MtoTempSpeed:
                        procedure = "inserttempspeed";
                        dp.Add("passenger", ((TempSpeed)mtoobject).Passenger);
                        dp.Add("freight", (mtoobject as TempSpeed).Freight);
                        dp.Add("empty_freight", ((TempSpeed)mtoobject).Empty_Freight);
                        dp.Add("reasonid", ((TempSpeed)mtoobject).Reason_Id);
                        break;
                    case MainTrackStructureConst.MtoElevation:
                        procedure = "insertelevation";
                        dp.Add("levelid", ((Elevation)mtoobject).Level_Id);
                        break;
                    case MainTrackStructureConst.MtoPdbSection:
                        procedure = "insertpdbsection";
                        dp.Add("pdbid", ((PdbSection)mtoobject).PdbId);
                        break;
                    case MainTrackStructureConst.MtoStationSection:
                        procedure = "insertstationsection";
                        dp.Add("stationid", ((StationSection)mtoobject).Station_Id);
                        dp.Add("axiskm", ((StationSection)mtoobject).Axis_Km);
                        dp.Add("axism", ((StationSection)mtoobject).Axis_M);
                        dp.Add("pointid", ((StationSection)mtoobject).Point_Id);

                        break;
                    case MainTrackStructureConst.MtoCurve:
                        procedure = "insertcurve";
                        dp.Add("sideid", ((Curve)mtoobject).Side_id);
                        break;
                    case MainTrackStructureConst.MtoElCurve:
                        procedure = "insertelcurve";
                        dp = new DynamicParameters();
                        dp.Add("curveid", ((ElCurve)mtoobject).Period_Id);
                        dp.Add("startkm", mtoobject.Start_Km);
                        dp.Add("startm", mtoobject.Start_M);
                        dp.Add("finalkm", mtoobject.Final_Km);
                        dp.Add("finalm", mtoobject.Final_M);
                        dp.Add("transition1", ((ElCurve)mtoobject).Transition_1);
                        dp.Add("transition2", ((ElCurve)mtoobject).Transition_2);
                        dp.Add("lvl", ((ElCurve)mtoobject).Lvl);
                        break;
                    case MainTrackStructureConst.MtoStCurve:
                        procedure = "insertstcurve";
                        dp = new DynamicParameters();
                        dp.Add("curveid", ((StCurve)mtoobject).Period_Id);
                        dp.Add("radius", ((StCurve)mtoobject).Radius);
                        dp.Add("startkm", mtoobject.Start_Km);
                        dp.Add("startm", mtoobject.Start_M);
                        dp.Add("finalkm", mtoobject.Final_Km);
                        dp.Add("finalm", mtoobject.Final_M);
                        dp.Add("tran1", ((StCurve)mtoobject).Transition_1);
                        dp.Add("tran2", ((StCurve)mtoobject).Transition_2);
                        dp.Add("wear", ((StCurve)mtoobject).Wear);
                        dp.Add("width", ((StCurve)mtoobject).Width);
                        break;
                    case MainTrackStructureConst.MtoStraighteningThread:
                        procedure = "insertstraightening";
                        dp.Add("sideid", ((StraighteningThread)mtoobject).Side_Id);
                        break;
                    case MainTrackStructureConst.MtoArtificialConstruction:
                        procedure = "insertartificialconstruction";
                        dp = new DynamicParameters();
                        dp.Add("periodid", mtoobject.Period_Id);
                        dp.Add("km", ((ArtificialConstruction)mtoobject).Km);
                        dp.Add("meter", ((ArtificialConstruction)mtoobject).Meter);
                        dp.Add("len", ((ArtificialConstruction)mtoobject).Len);
                        dp.Add("typeid", ((ArtificialConstruction)mtoobject).Type_Id);
                        break;
                    case MainTrackStructureConst.MtoSwitch:
                        procedure = "insertswitch";
                        dp = new DynamicParameters();
                        dp.Add("periodid", ((Switch)mtoobject).Period_Id);
                        dp.Add("stationid", ((Switch)mtoobject).Station_Id);
                        dp.Add("pointid", ((Switch)mtoobject).Point_Id);
                        dp.Add("sideid", ((Switch)mtoobject).Side_Id);
                        dp.Add("markid", ((Switch)mtoobject).Mark_Id);
                        dp.Add("dirid", ((Switch)mtoobject).Dir_Id);
                        dp.Add("km", ((Switch)mtoobject).Km);
                        dp.Add("mr", ((Switch)mtoobject).Meter);
                        dp.Add("num", ((Switch)mtoobject).Num);
                        break;
                    case MainTrackStructureConst.MtoLongRails:
                        procedure = "insertlongrails";
                        dp.Add("typeid", ((LongRails)mtoobject).Type_id);
                        break;
                    case MainTrackStructureConst.MtoRailSection:
                        procedure = "insertrailssections";
                        dp.Add("typeid", ((RailsSections)mtoobject).Type_Id);
                        break;
                    case MainTrackStructureConst.MtoNonStandard:
                        procedure = "insertnonstandardkm";
                        dp = new DynamicParameters();
                        dp.Add("periodid", ((NonstandardKm)mtoobject).Period_Id);
                        dp.Add("km", ((NonstandardKm)mtoobject).Km);
                        dp.Add("len", ((NonstandardKm)mtoobject).Len);
                        break;
                    case MainTrackStructureConst.MtoNonExtKm:
                        procedure = "insertnonexistkm";
                        dp = new DynamicParameters();
                        dp.Add("periodid", ((NonExtKm)mtoobject).Period_Id);
                        dp.Add("km", ((NonExtKm)mtoobject).Km);
                        break;
                    case MainTrackStructureConst.MtoProfileObject:
                        procedure = "insertprofileobject";
                        dp = new DynamicParameters();
                        dp.Add("periodid", ((ProfileObject)mtoobject).Period_Id);
                        dp.Add("sideid", ((ProfileObject)mtoobject).Side_id);
                        dp.Add("objectid", ((ProfileObject)mtoobject).Object_id);
                        dp.Add("km", ((ProfileObject)mtoobject).Km);
                        dp.Add("meter", ((ProfileObject)mtoobject).Meter);
                        break;
                    case MainTrackStructureConst.MtoChamJoint:
                        procedure = "insertchamjoint";
                        dp.Add("typeid", ((ChamJoint)mtoobject).Type_id);
                        break;
                    case MainTrackStructureConst.MtoProfmarks:
                        procedure = "insertprofmarks";
                        dp = new DynamicParameters();
                        dp.Add("periodid", ((Profmarks)mtoobject).Period_Id);
                        dp.Add("km", ((Profmarks)mtoobject).Km);
                        dp.Add("meter", ((Profmarks)mtoobject).Meter);
                        dp.Add("profil", ((Profmarks)mtoobject).Profil);
                        break;
                    case MainTrackStructureConst.MtoTraffic:
                        procedure = "inserttraffic";
                        dp.Add("traffic", ((MtoTraffic)mtoobject).Traffic);
                        break;
                    case MainTrackStructureConst.MtoWaycat:
                        procedure = "insertwaycat";
                        dp.Add("typeid", ((Waycat)mtoobject).Type_id);
                        break;
                    case MainTrackStructureConst.MtoRefPoint:
                        procedure = "insertrefpoint";
                        dp = new DynamicParameters();
                        dp.Add("periodid", ((RefPoint)mtoobject).Period_Id);
                        dp.Add("km", ((RefPoint)mtoobject).Km);
                        dp.Add("meter", ((RefPoint)mtoobject).Meter);
                        dp.Add("mark", ((RefPoint)mtoobject).Mark);
                        break;
                    case MainTrackStructureConst.MtoRepairProject:
                        procedure = "insertrepairproject";
                        dp = new DynamicParameters();
                        dp.Add("trackid", ((RepairProject)mtoobject).Adm_track_id);
                        dp.Add("startkm", mtoobject.Start_Km);
                        dp.Add("startm", mtoobject.Start_M);
                        dp.Add("finalkm", mtoobject.Final_Km);
                        dp.Add("finalm", mtoobject.Final_M);
                        dp.Add("typeid", ((RepairProject)mtoobject).Type_id);
                        dp.Add("acceptid", ((RepairProject)mtoobject).Accept_id);
                        dp.Add("speed", ((RepairProject)mtoobject).Speed);
                        dp.Add("repairdate", ((RepairProject)mtoobject).Repair_date.Date);
                        break;
                    case MainTrackStructureConst.MtoRFID:
                        procedure = "insertrfid";
                        dp = new DynamicParameters();
                        dp.Add("periodid", ((Rfid)mtoobject).Period_Id);
                        dp.Add("km_", ((Rfid)mtoobject).Km);
                        dp.Add("meter_", ((Rfid)mtoobject).Meter);
                        dp.Add("mark_", ((Rfid)mtoobject).Mark);
                        break;
                    case MainTrackStructureConst.MtoCheckSection:
                        procedure = "insertchecksections";
                        dp.Add("avgwidth", ((CheckSection)mtoobject).Avg_width);
                        dp.Add("avglevel", ((CheckSection)mtoobject).Avg_level);
                        dp.Add("skowidth", ((CheckSection)mtoobject).Sko_width);
                        dp.Add("skolevel", ((CheckSection)mtoobject).Sko_level);
                        break;
                    case MainTrackStructureConst.MtoCommunication:
                        procedure = "insertcommunication";
                        dp = new DynamicParameters();
                        dp.Add("periodid", ((Communication)mtoobject).Period_Id);
                        dp.Add("km", ((Communication)mtoobject).Km);
                        dp.Add("meter", ((Communication)mtoobject).Meter);
                        dp.Add("objectid", ((Communication)mtoobject).Object_id);
                        break;
                    case MainTrackStructureConst.MtoCoordinateGNSS:
                        procedure = "insertcoordinategnss";
                        dp = new DynamicParameters();
                        dp.Add("periodid", ((CoordinateGNSS)mtoobject).Period_Id);
                        dp.Add("km", ((CoordinateGNSS)mtoobject).Km);
                        dp.Add("meter", ((CoordinateGNSS)mtoobject).Meter);
                        dp.Add("latitude", ((CoordinateGNSS)mtoobject).Latitude);
                        dp.Add("longtitude", ((CoordinateGNSS)mtoobject).Longtitude);
                        dp.Add("altitude", ((CoordinateGNSS)mtoobject).Altitude);
                        dp.Add("coordinate", ((CoordinateGNSS)mtoobject).Exact_coordinate);
                        dp.Add("height", ((CoordinateGNSS)mtoobject).Exact_height);
                        break;
                    case MainTrackStructureConst.MtoDefectsEarth:
                        procedure = "insertdefectsearth";
                        dp.Add("typeid", ((DefectsEarth)mtoobject).Type_id);
                        break;
                    case MainTrackStructureConst.MtoDistanceBetweenTracks:
                        procedure = "insertdisbetweentracks";
                        dp.Add("leftm", ((DistanceBetweenTracks)mtoobject).Left_m);
                        dp.Add("lefttrack", ((DistanceBetweenTracks)mtoobject).Left_adm_track_id);
                        dp.Add("rightm", ((DistanceBetweenTracks)mtoobject).Right_m);
                        dp.Add("righttrack", ((DistanceBetweenTracks)mtoobject).Right_adm_track_id);
                        break;
                    case MainTrackStructureConst.MtoDeep:
                        procedure = "insertdeep";
                        break;
                    case MainTrackStructureConst.MtoBallastType:
                        procedure = "insertballast";
                        dp.Add("ball", ((BallastType)mtoobject).Ballast);
                        break;
                    case MainTrackStructureConst.MtoDimension:
                        procedure = "insertdimension";
                        dp.Add("typeid", ((Dimension)mtoobject).Type_id);
                        break;
                    default: return -1;
                }
                return (Int64)db.ExecuteScalar(procedure, dp, commandType: CommandType.StoredProcedure);
            }
        }

        public bool UpdateMtoObject(MainTrackObject mtobject, int mtoObjectType)
        {
            using (var db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                int result;
                switch (mtoObjectType)
                {
                    case MainTrackStructureConst.MtoDistSection:
                        result = db.Execute("UPDATE TPL_DIST_SECTION SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, ADM_DISTANCE_ID=@admdisid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, admdisid = ((DistSection)mtobject).DistanceId, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoCrossTie:
                        result = db.Execute("UPDATE APR_CROSSTIE SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, CROSSTIE_TYPE_ID=@crossid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, crossid = ((CrossTie)mtobject).Crosstie_type_id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoTrackClass:
                        result = db.Execute("UPDATE APR_TRACKCLASS SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, CLASS_ID=@classid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, classid = ((TrackClass)mtobject).Class_Id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoRailsBrace:
                        result = db.Execute("UPDATE APR_RAILS_BRACES SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, BRACE_TYPE_ID=@braceid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, braceid = ((RailsBrace)mtobject).Brace_Type_Id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoNormaWidth:
                        result = db.Execute("UPDATE APR_NORMA_WIDTH SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, NORMA_WIDTH=@normawidth WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, normawidth = ((NormaWidth)mtobject).Norma_Width, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoSpeed:
                        result = db.Execute("UPDATE APR_SPEED SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, PASSENGER=@passenger, FREIGHT=@freight, EMPTY_FREIGHT=@empty_freight, sapsan=@sapsan, lastochka=@lastochka WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, passenger = ((Speed)mtobject).Passenger, freight = ((Speed)mtobject).Freight, empty_freight = ((Speed)mtobject).Empty_Freight, sapsan = ((Speed)mtobject).Sapsan, lastochka = ((Speed)mtobject).Lastochka, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoTempSpeed:
                        result = db.Execute("UPDATE APR_TEMPSPEED SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, PASSENGER=@passenger, FREIGHT=@freight, EMPTY_FREIGHT=@empty_freight, REASON_ID=@reasonid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, passenger = ((TempSpeed)mtobject).Passenger, freight = ((TempSpeed)mtobject).Freight, empty_freight = ((TempSpeed)mtobject).Empty_Freight, reasonid = ((TempSpeed)mtobject).Reason_Id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoElevation:
                        result = db.Execute("UPDATE APR_ELEVATION SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, LEVEL_ID=@levelid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, levelid = ((Elevation)mtobject).Level_Id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoPdbSection:
                        result = db.Execute("UPDATE TPL_PDB_SECTION SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, ADM_PDB_ID=@admid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, admid = ((PdbSection)mtobject).PdbId, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoStationSection:
                        result = db.Execute("UPDATE TPL_STATION_SECTION SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, POINT_ID=@pointid, STATION_ID=@stationid, AXIS_KM=@akm, AXIS_M=@am WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, pointid = ((StationSection)mtobject).Point_Id, stationid = ((StationSection)mtobject).Station_Id, akm = ((StationSection)mtobject).Axis_Km, am = ((StationSection)mtobject).Axis_M, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoCurve:
                        result = db.Execute("UPDATE APR_CURVE SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, SIDE_ID=@sideid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, sideid = ((Curve)mtobject).Side_id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoElCurve:
                        result = db.Execute("UPDATE APR_ELCURVE SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, LVL=@lvl, transition_1=@tran1, transition_2=@tran2 WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, lvl = ((ElCurve)mtobject).Lvl, tran1 = ((ElCurve)mtobject).Transition_1, tran2 = ((ElCurve)mtobject).Transition_2, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoStCurve:
                        result = db.Execute("UPDATE APR_STCURVE SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, RADIUS=@radius, WEAR=@wear, transition_1=@tran1, transition_2=@tran2, WIDTH=@width WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, radius = ((StCurve)mtobject).Radius, wear = ((StCurve)mtobject).Wear, tran1 = ((StCurve)mtobject).Transition_1, tran2 = ((StCurve)mtobject).Transition_2, width = ((StCurve)mtobject).Width, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoStraighteningThread:
                        result = db.Execute("UPDATE APR_STRAIGHTENING_THREAD SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, SIDE_ID=@sideid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, sideid = ((StraighteningThread)mtobject).Side_Id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoArtificialConstruction:
                        result = db.Execute("UPDATE APR_ARTIFICIAL_CONSTRUCTION SET km=@km, meter=@m, len=@len, TYPE_ID=@typeid WHERE ID=@id",
                            new { km = ((ArtificialConstruction)mtobject).Km, m = ((ArtificialConstruction)mtobject).Meter, len = ((ArtificialConstruction)mtobject).Len, typeid = ((ArtificialConstruction)mtobject).Type_Id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoSwitch:
                        result = db.Execute("UPDATE TPL_SWITCH SET station_id=@stationid, POINT_ID=@pointid, SIDE_ID=@sideid, MARK_ID=@markid, DIR_ID=@dirid, KM=@km, METER=@m, NUM=@num WHERE ID=@id",
                            new { pointid = ((Switch)mtobject).Point_Id, stationid = ((Switch)mtobject).Station_Id, sideid = ((Switch)mtobject).Side_Id, markid = ((Switch)mtobject).Mark_Id, dirid = ((Switch)mtobject).Dir_Id, km = ((Switch)mtobject).Km, m = ((Switch)mtobject).Meter, num = ((Switch)mtobject).Num, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoLongRails:
                        result = db.Execute("UPDATE APR_LONG_RAILS SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, type_id=@typeid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, typeid = ((LongRails)mtobject).Type_id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoRailSection:
                        result = db.Execute("UPDATE APR_RAILS_SECTIONS SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, TYPE_ID=@typeid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, typeid = ((RailsSections)mtobject).Type_Id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoNonStandard:
                        result = db.Execute("UPDATE TPL_NST_KM SET KM=@km, LEN=@len WHERE ID=@nskmId",
                            new { km = ((NonstandardKm)mtobject).Km, len = ((NonstandardKm)mtobject).Len, nskmId = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoNonExtKm:
                        result = db.Execute("UPDATE TPL_NON_EXT_KM SET KM=@km WHERE ID=@id",
                            new { km = ((NonExtKm)mtobject).Km, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoProfileObject:
                        result = db.Execute("UPDATE TPL_PROFILE_OBJECT SET OBJECT_ID=@objectId, SIDE_ID=@sideId, KM=@km, METER=@m WHERE ID=@id",
                            new { objectId = ((ProfileObject)mtobject).Object_id, sideId = ((ProfileObject)mtobject).Side_id, km = ((ProfileObject)mtobject).Km, m = ((ProfileObject)mtobject).Meter, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoChamJoint:
                        result = db.Execute("UPDATE apr_cham_joint SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, type_id=@type WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, type = ((ChamJoint)mtobject).Type_id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoProfmarks:
                        result = db.Execute("UPDATE apr_profmarks SET km=@km, meter=@meter, profil=@profile WHERE ID=@id",
                            new { km = ((Profmarks)mtobject).Km, meter = ((Profmarks)mtobject).Meter, profile = ((Profmarks)mtobject).Profil, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoTraffic:
                        result = db.Execute("UPDATE apr_traffic SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, traffic=@traffic WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, traffic = ((MtoTraffic)mtobject).Traffic, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoWaycat:
                        result = db.Execute("UPDATE apr_waycat SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, type_id=@typeid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, typeid = ((Waycat)mtobject).Type_id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoRefPoint:
                        result = db.Execute("UPDATE apr_ref_point SET km=@km, meter=@meter, mark=@mark WHERE ID=@id",
                            new { km = ((RefPoint)mtobject).Km, meter = ((RefPoint)mtobject).Meter, mark = ((RefPoint)mtobject).Mark, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoRepairProject:
                        result = db.Execute($"UPDATE repair_project SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, accept_id=@accept, type_id=@type, speed=@speed, repair_date=@repairdate, accepted_at ={((((RepairProject)mtobject).Accept_id == 1) ? "now()" : "null")} WHERE ID=@id",

                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, accept = ((RepairProject)mtobject).Accept_id, type = ((RepairProject)mtobject).Type_id, speed = ((RepairProject)mtobject).Speed, repairdate = ((RepairProject)mtobject).Repair_date.Date, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoRFID:
                        result = db.Execute("update apr_rfid set km=@km, meter=@meter, abscoord=@abscoord, altitude=@altitude, latitude=@latitude, longtitude=@longtitude, mark=@mark where id=@id",
                            new { id = mtobject.Id, km = ((Rfid)mtobject).Km, meter = ((Rfid)mtobject).Meter, mark = ((Rfid)mtobject).Mark },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoCheckSection:
                        result = db.Execute("update tpl_check_sections set START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, avg_width=@avgw, avg_level=@avgl, sko_width=@skow, sko_level=@skol where id=@id",
                            new { id = mtobject.Id, skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, avgw = ((CheckSection)mtobject).Avg_width, avgl = ((CheckSection)mtobject).Avg_level, skow = ((CheckSection)mtobject).Sko_width, skol = ((CheckSection)mtobject).Sko_level },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoCommunication:
                        result = db.Execute("UPDATE apr_communication SET km=@km, meter=@meter, object_id=@object_id WHERE ID=@id",
                            new { km = ((Communication)mtobject).Km, meter = ((Communication)mtobject).Meter, object_id = ((Communication)mtobject).Object_id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoCoordinateGNSS:
                        result = db.Execute("UPDATE apr_coordinate_gnss SET km=@km, meter=@meter, latitude=@latitude, longtitude=@longtitude, altitude=@altitude, exact_coordinate=@coord, exact_height=@height WHERE ID=@id",
                            new { km = ((CoordinateGNSS)mtobject).Km, meter = ((CoordinateGNSS)mtobject).Meter, latitude = ((CoordinateGNSS)mtobject).Latitude, longtitude = ((CoordinateGNSS)mtobject).Longtitude, altitude = ((CoordinateGNSS)mtobject).Altitude,
                                coord = ((CoordinateGNSS)mtobject).Exact_coordinate, height = ((CoordinateGNSS)mtobject).Exact_height, id = mtobject.Id }, commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoDefectsEarth:
                        result = db.Execute("UPDATE apr_defects_earth SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, type_id=@typeid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, typeid = ((DefectsEarth)mtobject).Type_id, id = mtobject.Id },
                            commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoDistanceBetweenTracks:
                        result = db.Execute("UPDATE tpl_distance_between_tracks SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm, left_m=@leftm, left_adm_track_id=@leftid, right_m=@rightm, right_adm_track_id=@rightid WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, leftm = ((DistanceBetweenTracks)mtobject).Left_m, leftid = ((DistanceBetweenTracks)mtobject).Left_adm_track_id,
                                rightm = ((DistanceBetweenTracks)mtobject).Right_m, rightid = ((DistanceBetweenTracks)mtobject).Right_adm_track_id, id = mtobject.Id }, commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoDeep:
                        result = db.Execute("UPDATE tpl_deep SET START_KM=@skm, START_M=@sm, FINAL_KM=@fkm, FINAL_M=@fm WHERE ID=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, id = mtobject.Id }, commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoBallastType:
                        result = db.Execute("update apr_ballast set start_km=@skm, start_m=@sm, final_km=@fkm, final_m=@fm, ballast=@ball where id=@id",
                            new { skm = mtobject.Start_Km, sm = mtobject.Start_M, fkm = mtobject.Final_Km, fm = mtobject.Final_M, ball = ((BallastType)mtobject).Ballast }, commandType: CommandType.Text);
                        break;
                    case MainTrackStructureConst.MtoDimension:
                        result = db.Execute($@"update apr_dimension set start_km={mtobject.Start_Km}, start_m={mtobject.Start_M}, final_km={mtobject.Final_Km}, final_m={mtobject.Final_M},
                            type_id={((Dimension)mtobject).Type_id} where id={mtobject.Id}", commandType: CommandType.Text);
                        break;
                    default:
                        result = 0;
                        break;
                }
                return result != 0;
            }
        }

        public Int64 InsertPeriod(Period period)
        {
            using (var db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("insertperiod", db))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var startdate = new NpgsqlParameter("startdate", NpgsqlTypes.NpgsqlDbType.Date);
                    startdate.Value = period.Start_Date.Date;
                    var finaldate = new NpgsqlParameter("finaldate", NpgsqlTypes.NpgsqlDbType.Date);
                    finaldate.Value = period.Final_Date.Date;
                    var trackid = new NpgsqlParameter("trackid", period.Track_Id);
                    var mtotype = new NpgsqlParameter("mtotype", period.Mto_Type);

                    cmd.Parameters.Add(startdate);
                    cmd.Parameters.Add(finaldate);
                    cmd.Parameters.Add(trackid);
                    cmd.Parameters.Add(mtotype);
                    // ReSharper disable once PossibleNullReferenceException
                    return (Int64)cmd.ExecuteScalar();
                }
            }
        }

        public bool UpdatePeriod(Period period)
        {
            using (var db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                int result = db.Execute("UPDATE TPL_PERIOD SET START_DATE=@start_data, FINAL_DATE=@final_data WHERE ID=@periodId", new { start_data = period.Start_Date, final_data = period.Final_Date, periodId = period.Id }, commandType: CommandType.Text);
                return result != 0;
            }
        }

        public bool GenerateDirectionList(string direction, Int64 trackID, string dirName, DateTime dirListDate)
        {
            using (var db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using (var textWriter = new StreamWriter(dirName + "\\" + direction + "_prm.txt", true, System.Text.Encoding.GetEncoding(1251)))
                {
                    List<DLWidthNorma> dlWidthNorma = db.Query<DLWidthNorma>("SELECT admt.CODE as ADM_TRACK_CODE, ad.CODE as ADM_DISTANCE_CODE, cats.S_NAME as side, anw.START_KM, anw.START_M, anw.FINAL_KM, anw.FINAL_M, anw.NORMA_WIDTH as width FROM APR_NORMA_WIDTH as anw " +
                        "INNER JOIN TPL_PERIOD as tplp on tplp.ID = anw.PERIOD_ID " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "INNER JOIN TPL_DIST_SECTION as tds on ( " +
                        "(anw.START_KM > tds.START_KM and anw.FINAL_KM < tds.FINAL_KM) or " +
                        "(anw.START_KM = tds.START_KM and anw.START_M >= tds.START_M and anw.FINAL_KM < tds.FINAL_KM) or " +
                        "(anw.FINAL_KM = tds.FINAL_KM and anw.FINAL_M <= tds.FINAL_M and anw.START_KM > tds.START_KM) or " +
                        "(anw.START_KM = tds.START_KM and anw.FINAL_KM = tds.FINAL_KM and anw.START_M >= tds.START_M and anw.FINAL_M <= tds.FINAL_M)) " +
                        "INNER JOIN ADM_DISTANCE as ad on ad.ID = tds.ADM_DISTANCE_ID " +
                        "INNER JOIN APR_STRAIGHTENING_THREAD as ast on (ast.START_KM = anw.START_KM AND ast.START_M = anw.START_M AND ast.FINAL_KM = anw.FINAL_KM AND ast.FINAL_M = anw.FINAL_M) " +
                        "INNER JOIN CAT_SIDE as cats on cats.ID = ast.SIDE_ID " +
                        "WHERE anw.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ") AND " +
                        "ast.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ") AND " +
                        "tds.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ")", commandType: CommandType.Text).ToList();

                    foreach (DLWidthNorma dlWN in dlWidthNorma.OrderBy(u => u.Start_Km).ThenBy(u => u.Start_M).ThenBy(u => u.Final_Km).ThenBy(u => u.Final_M))
                    {
                        textWriter.WriteLine($"{dlWN.Adm_Distance_Code}{dlWN.Start_Km,7}{dlWN.Start_M,7}{dlWN.Final_Km,7}{dlWN.Final_M,7}{dlWN.Width,7}");
                        textWriter.WriteLine(dlWN.Adm_Track_Code.ToString());
                        textWriter.WriteLine(dlWN.Side.ToLower());
                    }
                }

                using (var textWriter = new StreamWriter(dirName + "\\" + direction + "_prpuch.txt", true, System.Text.Encoding.GetEncoding(1251)))
                {
                    List<DLElevation> dlElevations = db.Query<DLElevation>("SELECT ad.CODE as ADM_DISTANCE_CODE, admt.CODE as ADM_TRACK_CODE, cnl.NAME as LEVEL, ae.START_KM, ae.START_M, ae.FINAL_KM, ae.FINAL_M from APR_ELEVATION as ae " +
                        "INNER JOIN CAT_NORMA_LEVEL as cnl on cnl.ID = ae.LEVEL_ID " +
                        "INNER JOIN TPL_PERIOD as tplp on tplp.ID = ae.PERIOD_ID " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "INNER JOIN TPL_DIST_SECTION as tds on ( " +
                        "(ae.START_KM > tds.START_KM and ae.FINAL_KM < tds.FINAL_KM) or " +
                        "(ae.START_KM = tds.START_KM and ae.START_M >= tds.START_M and ae.FINAL_KM < tds.FINAL_KM) or " +
                        "(ae.FINAL_KM = tds.FINAL_KM and ae.FINAL_M <= tds.FINAL_M and ae.START_KM > tds.START_KM) or " +
                        "(ae.START_KM = tds.START_KM and ae.FINAL_KM = tds.FINAL_KM and ae.START_M >= tds.START_M and ae.FINAL_M <= tds.FINAL_M)) " +
                        "INNER JOIN ADM_DISTANCE as ad on ad.ID = tds.ADM_DISTANCE_ID " +
                        "WHERE tds.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ") AND " +
                        "ae.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ")", commandType: CommandType.Text).ToList();

                    foreach (DLElevation dlE in dlElevations.OrderBy(u => u.Start_Km).ThenBy(u => u.Start_M).ThenBy(u => u.Final_Km).ThenBy(u => u.Final_M))
                    {
                        textWriter.WriteLine($"{dlE.Adm_Distance_Code}{dlE.Start_Km,7}{dlE.Start_M,7}{dlE.Final_Km,7}{dlE.Final_M,7}");
                        textWriter.WriteLine(dlE.Adm_Track_Code.ToString());
                        textWriter.WriteLine(dlE.Level[0]);
                    }
                }

                List<DLElCurve> dlElCurves = db.Query<DLElCurve>("SELECT ad.CODE as ADM_DISTANCE_CODE, cs.S_NAME as SIDE, admt.CODE as ADM_TRACK_CODE, aec.START_KM, aec.START_M, aec.FINAL_KM, aec.FINAL_M, aec.LVL_START_KM, aec.LVL_START_M, aec.LVL_FINAL_KM, aec.LVL_FINAL_M, aec.RADIUS, aec.WEAR, aec.WIDTH, aec.LVL from APR_ELCURVE as aec " +
                    "INNER JOIN APR_CURVE as acu on acu.ID = aec.CURVE_ID " +
                    "INNER JOIN CAT_SIDE as cs on cs.ID = acu.SIDE_ID " +
                    "INNER JOIN TPL_PERIOD as tplp on tplp.ID = acu.PERIOD_ID " +
                    "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                    "INNER JOIN TPL_DIST_SECTION as tds on ( " +
                    "(aec.START_KM > tds.START_KM and aec.FINAL_KM < tds.FINAL_KM) or " +
                    "(aec.START_KM = tds.START_KM and aec.START_M >= tds.START_M and aec.FINAL_KM < tds.FINAL_KM) or " +
                    "(aec.FINAL_KM = tds.FINAL_KM and aec.FINAL_M <= tds.FINAL_M and aec.START_KM > tds.START_KM) or " +
                    "(aec.START_KM = tds.START_KM and aec.FINAL_KM = tds.FINAL_KM and aec.START_M >= tds.START_M and aec.FINAL_M <= tds.FINAL_M)) " +
                    "INNER JOIN ADM_DISTANCE as ad on ad.ID = tds.ADM_DISTANCE_ID " +
                    "WHERE tds.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                    "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                    "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                    "AND admt.ID = " + trackID.ToString() + ") AND " +
                    "acu.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                    "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                    "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                    "AND admt.ID = " + trackID.ToString() + ")", commandType: CommandType.Text).ToList();

                using (var textWriter = new StreamWriter(dirName + "\\" + direction + "_krv.txt", true, System.Text.Encoding.GetEncoding(1251)))
                {
                    foreach (DLElCurve dlEC in dlElCurves.OrderBy(u => u.Start_Km).ThenBy(u => u.Start_M).ThenBy(u => u.Final_Km).ThenBy(u => u.Final_M))
                    {
                        int d1 = (dlEC.Lvl_Start_Km - dlEC.Start_Km) * 1000 + (dlEC.Lvl_Start_M - dlEC.Start_M);
                        int d2 = (dlEC.Final_Km - dlEC.Lvl_Final_Km) * 1000 + (dlEC.Final_M - dlEC.Lvl_Final_M);
                        textWriter.WriteLine($"{dlEC.Adm_Distance_Code}{dlEC.Start_Km,7}{dlEC.Start_M,7}{dlEC.Final_Km,7}{dlEC.Final_M,7}{d1,7}{d2,7}{dlEC.Radius,7}{dlEC.Width,7}{dlEC.Lvl,7}{dlEC.Wear,7}");
                        textWriter.WriteLine(dlEC.Adm_Track_Code.ToString());
                        textWriter.WriteLine(dlEC.Side.ToLower());
                    }
                }

                foreach (DLElCurve dlEC in dlElCurves.OrderBy(u => u.Start_Km).ThenBy(u => u.Start_M).ThenBy(u => u.Final_Km).ThenBy(u => u.Final_M))
                {
                    using (var textWriter = new StreamWriter(dirName + "\\" + direction + "_put" + dlEC.Adm_Track_Code.ToString() + "_krv.txt", true, System.Text.Encoding.GetEncoding(1251)))
                    {
                        int d1 = (dlEC.Lvl_Start_Km - dlEC.Start_Km) * 1000 + (dlEC.Lvl_Start_M - dlEC.Start_M);
                        int d2 = (dlEC.Final_Km - dlEC.Lvl_Final_Km) * 1000 + (dlEC.Final_M - dlEC.Lvl_Final_M);
                        textWriter.WriteLine($"{dlEC.Adm_Distance_Code}{dlEC.Start_Km,7}{dlEC.Start_M,7}{dlEC.Final_Km,7}{dlEC.Final_M,7}{d1,7}{d2,7}{dlEC.Radius,7}{dlEC.Width,7}{dlEC.Lvl,7}{dlEC.Wear,7}");
                        textWriter.WriteLine(dlEC.Adm_Track_Code.ToString());
                        textWriter.WriteLine(dlEC.Side.ToLower());
                    }
                }

                dlElCurves = null;

                using (var textWriter = new StreamWriter(dirName + "\\" + direction + "_str.txt", true, System.Text.Encoding.GetEncoding(1251)))
                {
                    List<DLSwitch> dlSwitches = db.Query<DLSwitch>("SELECT adms.CODE as ADM_STATION_CODE, ad.CODE as ADM_DISTANCE_CODE, admt.CODE as ADM_TRACK_CODE, ccm.ITYPE, ccm.LEN, tsw.KM as START_KM, tsw.Meter as START_M, tsw.DIR_ID, tsw.SIDE_ID, tsw.NUM from TPL_SWITCH as tsw " +
                        "INNER JOIN TPL_PERIOD as tplp on tplp.ID = tsw.PERIOD_ID " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "INNER JOIN TPL_DIST_SECTION as tds on ( " +
                        "(tsw.KM > tds.START_KM and tsw.KM < tds.FINAL_KM) or " +
                        "(tsw.KM = tds.START_KM and tsw.Meter >= tds.START_M and tsw.KM < tds.FINAL_KM) or " +
                        "(tsw.KM = tds.FINAL_KM and tsw.Meter <= tds.FINAL_M and tsw.KM > tds.START_KM) or " +
                        "(tsw.KM = tds.START_KM and tsw.KM = tds.FINAL_KM and tsw.Meter >= tds.START_M and tsw.Meter <= tds.FINAL_M)) " +
                        "INNER JOIN ADM_DISTANCE as ad on ad.ID = tds.ADM_DISTANCE_ID " +
                        "INNER JOIN ADM_STATION as adms on adms.ID = tsw.STATION_ID " +
                        "INNER JOIN CAT_CROSS_MARK as ccm on ccm.ID = tsw.MARK_ID " +
                        "WHERE tsw.MARK_ID between 3 and 10 AND " +
                        "tds.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ") AND " +
                        "tsw.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ")", commandType: CommandType.Text).ToList();

                    foreach (DLSwitch dlS in dlSwitches.OrderBy(u => u.Start_Km))
                    {
                        int skm, sm, fkm, fm, dir, type, side = -1;
                        switch (dlS.Dir_Id)
                        {
                            case -1:
                                dir = 0;
                                break;
                            case 1:
                                dir = 1;
                                break;
                            case 0:
                            default:
                                dir = -1;
                                break;
                        }
                        if (dlS.Itype == 4)
                            type = 3;
                        else if (dlS.Itype == 3)
                            type = 5;
                        else
                            type = -1;
                        //switch (dlS.Side_id)
                        //{
                        //    case 0:
                        //        side = 1;
                        //        break;
                        //    case 1:
                        //        side = 0;
                        //        break;
                        //    case 2:
                        //        side = 2;
                        //        break;
                        //    default:
                        //        side = -1;
                        //        break;
                        //}
                        if (dir == 0)
                        {
                            fm = dlS.Start_M + 3;
                            if (fm >= 1000)
                            {
                                fkm = dlS.Start_Km + 1;
                                fm -= 1000;
                            }
                            else
                                fkm = dlS.Start_Km;
                            sm = dlS.Start_M - dlS.Len + 1;
                            if (sm < 0)
                            {
                                skm = dlS.Start_Km - 1;
                                sm += 1000;
                            }
                            else
                                skm = dlS.Start_Km;
                        }
                        else if (dir == 1)
                        {
                            fm = dlS.Start_M + dlS.Len - 1;
                            if (fm >= 1000)
                            {
                                fkm = dlS.Start_Km + 1;
                                fm -= 1000;
                            }
                            else
                                fkm = dlS.Start_Km;
                            sm = dlS.Start_M - 3;
                            if (sm < 0)
                            {
                                skm = dlS.Start_Km - 1;
                                sm += 1000;
                            }
                            else
                                skm = dlS.Start_Km;
                        }
                        else
                        {
                            skm = -1;
                            sm = -1;
                            fkm = -1;
                            fm = -1;
                        }

                        textWriter.WriteLine($"{dlS.Adm_Distance_Code}{skm,7}{sm,7}{fkm,7}{fm,7}{type,7}{dir,7}{side,7}");
                        textWriter.WriteLine(dlS.Adm_Track_Code.ToString());
                        textWriter.WriteLine(dlS.Num);
                        textWriter.WriteLine(dlS.Adm_Station_Code.ToString());
                    }
                }

                using (var textWriter = new StreamWriter(dirName + "\\" + direction + "_str2.txt", true, System.Text.Encoding.GetEncoding(1251)))
                {
                    List<DLSwitch> dlSwitches = db.Query<DLSwitch>("SELECT crt.NAME as RAILS_TYPE, adms.CODE as ADM_STATION_CODE, ad.CODE as ADM_DISTANCE_CODE, admt.CODE as ADM_TRACK_CODE, ccm.ITYPE, tsw.KM as START_KM, tsw.Meter as START_M, tsw.SIDE_ID, tsw.NUM from TPL_SWITCH as tsw " +
                        "INNER JOIN TPL_PERIOD as tplp on tplp.ID = tsw.PERIOD_ID " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "INNER JOIN APR_RAILS_SECTIONS as ars on ( " +
                        "(tsw.KM > ars.START_KM and tsw.KM < ars.FINAL_KM) or " +
                        "(tsw.KM = ars.START_KM and tsw.Meter >= ars.START_M and tsw.KM < ars.FINAL_KM) or " +
                        "(tsw.KM = ars.FINAL_KM and tsw.Meter <= ars.FINAL_M and tsw.KM > ars.START_KM) or " +
                        "(tsw.KM = ars.START_KM and tsw.KM = ars.FINAL_KM and tsw.Meter >= ars.START_M and tsw.Meter <= ars.FINAL_M)) " +
                        "INNER JOIN CAT_RAILS_TYPE as crt on crt.ID = ars.TYPE_ID " +
                        "INNER JOIN TPL_DIST_SECTION as tds on ( " +
                        "(tsw.KM > tds.START_KM and tsw.KM < tds.FINAL_KM) or " +
                        "(tsw.KM = tds.START_KM and tsw.Meter >= tds.START_M and tsw.KM < tds.FINAL_KM) or " +
                        "(tsw.KM = tds.FINAL_KM and tsw.Meter <= tds.FINAL_M and tsw.KM > tds.START_KM) or " +
                        "(tsw.KM = tds.START_KM and tsw.KM = tds.FINAL_KM and tsw.Meter >= tds.START_M and tsw.Meter <= tds.FINAL_M)) " +
                        "INNER JOIN ADM_DISTANCE as ad on ad.ID = tds.ADM_DISTANCE_ID " +
                        "INNER JOIN ADM_STATION as adms on adms.ID = tsw.STATION_ID " +
                        "INNER JOIN CAT_CROSS_MARK as ccm on ccm.ID = tsw.MARK_ID " +
                        "WHERE tsw.MARK_ID between 11 and 13 AND " +
                        "tds.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ") AND " +
                        "ars.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ") AND " +
                        "tsw.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ")", commandType: CommandType.Text).ToList();

                    foreach (DLSwitch dlS in dlSwitches.OrderBy(u => u.Start_Km))
                    {
                        int skm, sm, fkm, fm, type, side = -1;
                        if (dlS.Itype == 11)
                            type = 3;
                        else if (dlS.Itype == 12)
                            type = 2;
                        else if (dlS.Itype == 10)
                            type = 1;
                        else
                            type = -1;
                        //switch (dlS.Side_id)
                        //{
                        //    case 0:
                        //        side = 1;
                        //        break;
                        //   case 1:
                        //        side = 0;
                        //        break;
                        //    case 2:
                        //        side = 2;
                        //        break;
                        //    default:
                        //        side = -1;
                        //        break;
                        //}
                        fm = dlS.Start_M + 10;
                        if (fm >= 1000)
                        {
                            fkm = dlS.Start_Km + 1;
                            fm -= 1000;
                        }
                        else
                            fkm = dlS.Start_Km;
                        sm = dlS.Start_M - 10;
                        if (sm < 0)
                        {
                            skm = dlS.Start_Km - 1;
                            sm += 1000;
                        }
                        else
                            skm = dlS.Start_Km;

                        textWriter.WriteLine($"{dlS.Adm_Distance_Code}{skm,7}{sm,7}{fkm,7}{fm,7}{type,7}{side,7}");
                        textWriter.WriteLine(dlS.Adm_Track_Code.ToString());
                        textWriter.WriteLine(dlS.Num);
                        textWriter.WriteLine("г");
                        textWriter.WriteLine(dlS.Rails_Type.ToUpper());
                        textWriter.WriteLine(dlS.Adm_Station_Code.ToString());
                    }
                }



                using (var textWriter = new StreamWriter(dirName + "\\" + direction + "_nes.txt", true, System.Text.Encoding.GetEncoding(1251)))
                {
                    List<DLNonExtKm> dlNonExtKms = db.Query<DLNonExtKm>("SELECT ad.CODE as ADM_DISTANCE_CODE, admt.CODE as ADM_TRACK_CODE, nekm.KM from TPL_NON_EXT_KM as nekm " +
                        "INNER JOIN TPL_DIST_SECTION as tds on (nekm.KM between tds.START_KM AND tds.FINAL_KM) " +
                        "INNER JOIN ADM_DISTANCE as ad on ad.ID = tds.ADM_DISTANCE_ID " +
                        "INNER JOIN TPL_PERIOD as tplp on tplp.ID = nekm.PERIOD_ID " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE tds.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ") AND " +
                        "nekm.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ")", commandType: CommandType.Text).ToList();

                    foreach (DLNonExtKm dl in dlNonExtKms.OrderBy(u => u.Km))
                    {
                        textWriter.WriteLine($"{dl.Adm_Distance_Code}{dl.Km,7}");
                        textWriter.WriteLine(dl.Adm_Track_Code.ToString());
                    }
                }

                using (var textWriter = new StreamWriter(dirName + "\\" + direction + "_nst.txt", true, System.Text.Encoding.GetEncoding(1251)))
                {
                    List<DLNstKm> dlNstKms = db.Query<DLNstKm>("SELECT ad.CODE as ADM_DISTANCE_CODE, admt.CODE as ADM_TRACK_CODE, nekm.KM, nekm.LEN from TPL_NST_KM as nekm " +
                        "INNER JOIN TPL_DIST_SECTION as tds on (nekm.KM between tds.START_KM AND tds.FINAL_KM) " +
                        "INNER JOIN ADM_DISTANCE as ad on ad.ID = tds.ADM_DISTANCE_ID " +
                        "INNER JOIN TPL_PERIOD as tplp on tplp.ID = nekm.PERIOD_ID " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE tds.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ") AND " +
                        "nekm.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ")", commandType: CommandType.Text).ToList();

                    foreach (DLNstKm dl in dlNstKms.OrderBy(u => u.Km))
                    {
                        textWriter.WriteLine($"{dl.Adm_Distance_Code}{dl.Km,7}{dl.Len,7}");
                        textWriter.WriteLine(dl.Adm_Track_Code.ToString());
                    }
                }

                using (var textWriter = new StreamWriter(dirName + "\\" + direction + "_rsp.txt", true, System.Text.Encoding.GetEncoding(1251)))
                {
                    List<DLStationSection> dlStationSections = db.Query<DLStationSection>("SELECT ad.CODE as ADM_DISTANCE_CODE, admt.CODE as ADM_TRACK_CODE, ast.NAME as STATION_NAME, ast.CODE as STATION_CODE, cpot.NAME as POINT, " +
                        "tss.START_KM, tss.START_M, tss.FINAL_KM, tss.FINAL_M, tss.AXIS_KM, tss.AXIS_M, tss.POINT_ID FROM TPL_STATION_SECTION as tss " +
                        "INNER JOIN TPL_DIST_SECTION as tds on ( " +
                        "(tss.START_KM > tds.START_KM and tss.FINAL_KM < tds.FINAL_KM) or " +
                        "(tss.START_KM = tds.START_KM and tss.START_M >= tds.START_M and tss.FINAL_KM < tds.FINAL_KM) or " +
                        "(tss.FINAL_KM = tds.FINAL_KM and tss.FINAL_M <= tds.FINAL_M and tss.START_KM > tds.START_KM) or " +
                        "(tss.START_KM = tds.START_KM and tss.FINAL_KM = tds.FINAL_KM and tss.START_M >= tds.START_M and tss.FINAL_M <= tds.FINAL_M)) " +
                        "INNER JOIN ADM_DISTANCE as ad on ad.ID = tds.ADM_DISTANCE_ID " +
                        "INNER JOIN ADM_STATION as ast on ast.ID = tss.STATION_ID " +
                        "INNER JOIN TPL_PERIOD as tplp on tplp.ID = tss.PERIOD_ID " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "INNER JOIN CAT_POINT_OBJECT_TYPE as cpot on cpot.ID = tss.POINT_ID " +
                        "WHERE tds.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ") AND " +
                        "tss.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ")", commandType: CommandType.Text).ToList();

                    foreach (DLStationSection dl in dlStationSections.OrderBy(u => u.Start_Km).ThenBy(u => u.Start_M).ThenBy(u => u.Final_Km).ThenBy(u => u.Final_M))
                    {
                        textWriter.WriteLine($"{dl.Adm_Distance_Code}{dl.Start_Km,7}{dl.Start_M,7}{dl.Final_Km,7}{dl.Final_M,7}{-1,9}{-1,9}{dl.Axis_Km,9}{dl.Axis_M,9}");
                        textWriter.WriteLine(dl.Station_Name.TrimEnd(' '));
                        textWriter.WriteLine(dl.Point.TrimEnd(' '));
                        textWriter.WriteLine(dl.Adm_Track_Code.ToString());
                    }
                }

                using (var textWriter = new StreamWriter(dirName + "\\" + direction + "_adm.txt", true, System.Text.Encoding.GetEncoding(1251)))
                {
                    List<DLPdbSection> dlPdbSections = db.Query<DLPdbSection>("SELECT apc.CODE as MEX, apd.CODE as PD_CODE, apd.CHIEF_FULLNAME as PD_NAME, ad.NAME as DISTANCE_NAME, ap.CODE as CODE, admt.CODE as ADM_TRACK_CODE, ad.CODE as ADM_DISTANCE_CODE, " +
                        "tps.START_KM, tps.FINAL_KM FROM TPL_PDB_SECTION as tps " +
                        "INNER JOIN ADM_PDB as ap on ap.ID = tps.ADM_PDB_ID " +
                        "INNER JOIN ADM_PD as apd on apd.ID = ap.ADM_PD_ID " +
                        "INNER JOIN ADM_PCHU as apc on apc.ID = apd.ADM_PCHU_ID " +
                        "INNER JOIN ADM_DISTANCE as ad on ad.ID = apc.ADM_DISTANCE_ID " +
                        "INNER JOIN ADM_NOD as an on an.ID = ad.ADM_NOD_ID " +
                        "INNER JOIN ADM_ROAD as ar on ar.ID = an.ADM_ROAD_ID " +
                        "INNER JOIN TPL_PERIOD as tplp on tplp.ID = tps.PERIOD_ID " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE tps.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ")", commandType: CommandType.Text).ToList();

                    foreach (DLPdbSection dl in dlPdbSections.OrderBy(u => u.Start_Km).ThenBy(u => u.Final_Km))
                    {
                        textWriter.WriteLine($"{dl.Adm_Distance_Code}{dl.Mex.TrimEnd(' '),5}{dl.Pd_Code.TrimEnd(' '),5}{dl.Code.TrimEnd(' '),5}{dl.Start_Km,10}{dl.Final_Km,10}");
                        textWriter.WriteLine("no");
                        textWriter.WriteLine("no");
                        textWriter.WriteLine(dl.Distance_Name.TrimEnd(' '));
                        textWriter.WriteLine(dl.Pd_Name.TrimEnd(' '));
                        textWriter.WriteLine(dl.Adm_Track_Code.ToString());
                    }
                }

                using (var textWriter = new StreamWriter(dirName + "\\" + direction + "_skr.txt", true, System.Text.Encoding.GetEncoding(1251)))
                {
                    List<DLSpeed> dlSpeeds = db.Query<DLSpeed>("SELECT ad.CODE as ADM_DISTANCE_CODE, admt.CODE as ADM_TRACK_CODE, asp.START_KM, asp.START_M, asp.FINAL_KM, asp.FINAL_M, asp.PASSENGER, asp.FREIGHT from APR_SPEED as asp " +
                        "INNER JOIN TPL_DIST_SECTION as tds on ( " +
                        "(asp.START_KM > tds.START_KM and asp.FINAL_KM < tds.FINAL_KM) or " +
                        "(asp.START_KM = tds.START_KM and asp.START_M >= tds.START_M and asp.FINAL_KM < tds.FINAL_KM) or " +
                        "(asp.FINAL_KM = tds.FINAL_KM and asp.FINAL_M <= tds.FINAL_M and asp.START_KM > tds.START_KM) or " +
                        "(asp.START_KM = tds.START_KM and asp.FINAL_KM = tds.FINAL_KM and asp.START_M >= tds.START_M and asp.FINAL_M <= tds.FINAL_M)) " +
                        "INNER JOIN ADM_DISTANCE as ad on ad.ID = tds.ADM_DISTANCE_ID " +
                        "INNER JOIN TPL_PERIOD as tplp on tplp.ID = asp.PERIOD_ID " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE tds.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ") AND " +
                        "asp.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ")", commandType: CommandType.Text).ToList();

                    foreach (DLSpeed dl in dlSpeeds.OrderBy(u => u.Start_Km).ThenBy(u => u.Start_M).ThenBy(u => u.Final_Km).ThenBy(u => u.Final_M))
                    {
                        textWriter.WriteLine($"{dl.Adm_Distance_Code}{dl.Start_Km,7}{dl.Start_M,7}{dl.Final_Km,7}{dl.Final_M,7}{dl.Passenger,9}{dl.Freight,9}");
                        textWriter.WriteLine("п");
                        textWriter.WriteLine(dl.Adm_Track_Code.ToString());
                    }
                }

                using (var textWriter = new StreamWriter(dirName + "\\" + direction + "_mst.txt", true, System.Text.Encoding.GetEncoding(1251)))
                {
                    List<DLArtificialConstruction> dlArtificialConstructions = db.Query<DLArtificialConstruction>("SELECT ad.CODE as ADM_DISTANCE_CODE, admt.CODE as ADM_TRACK_CODE, catac.NAME as TYPE, aac.START_KM, aac.START_M, aac.FINAL_KM, aac.FINAL_M from APR_ARTIFICIAL_CONSTRUCTION as aac " +
                        "INNER JOIN TPL_DIST_SECTION as tds on ( " +
                        "(aac.START_KM > tds.START_KM and aac.FINAL_KM < tds.FINAL_KM) or " +
                        "(aac.START_KM = tds.START_KM and aac.START_M >= tds.START_M and aac.FINAL_KM < tds.FINAL_KM) or " +
                        "(aac.FINAL_KM = tds.FINAL_KM and aac.FINAL_M <= tds.FINAL_M and aac.START_KM > tds.START_KM) or " +
                        "(aac.START_KM = tds.START_KM and aac.FINAL_KM = tds.FINAL_KM and aac.START_M >= tds.START_M and aac.FINAL_M <= tds.FINAL_M)) " +
                        "INNER JOIN ADM_DISTANCE as ad on ad.ID = tds.ADM_DISTANCE_ID " +
                        "INNER JOIN TPL_PERIOD as tplp on tplp.ID = aac.PERIOD_ID " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "INNER JOIN CAT_ARTIFICIAL_CONSTRUCTION as catac on catac.ID = aac.TYPE_ID " +
                        "WHERE tds.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ") AND " +
                        "aac.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ")", commandType: CommandType.Text).ToList();

                    foreach (DLArtificialConstruction dl in dlArtificialConstructions.OrderBy(u => u.Start_Km).ThenBy(u => u.Start_M).ThenBy(u => u.Final_Km).ThenBy(u => u.Final_M))
                    {
                        dl.Length = (dl.Final_Km - dl.Start_Km) * 1000 + dl.Final_M - dl.Start_M;
                        textWriter.WriteLine($"{dl.Adm_Distance_Code}{dl.Start_Km,7}{dl.Start_M,7}{dl.Final_Km,7}{dl.Final_M,7}{dl.Length,9}");
                        textWriter.WriteLine(dl.Type.ToLower()[0]);
                        textWriter.WriteLine(dl.Adm_Track_Code.ToString());
                    }
                }

                using (var textWriter = new StreamWriter(dirName + "\\" + direction + "_shp.txt", true, System.Text.Encoding.GetEncoding(1251)))
                {
                    List<DLCrosstie> dlCrossties = db.Query<DLCrosstie>("SELECT ad.CODE as ADM_DISTANCE_CODE, admt.CODE as ADM_TRACK_CODE, ac.START_KM, ac.START_M, ac.FINAL_KM, ac.FINAL_M, ac.CROSSTIE_TYPE_ID from APR_CROSSTIE as ac " +
                        "INNER JOIN TPL_DIST_SECTION as tds on ( " +
                        "(ac.START_KM > tds.START_KM and ac.FINAL_KM < tds.FINAL_KM) or " +
                        "(ac.START_KM = tds.START_KM and ac.START_M >= tds.START_M and ac.FINAL_KM < tds.FINAL_KM) or " +
                        "(ac.FINAL_KM = tds.FINAL_KM and ac.FINAL_M <= tds.FINAL_M and ac.START_KM > tds.START_KM) or " +
                        "(ac.START_KM = tds.START_KM and ac.FINAL_KM = tds.FINAL_KM and ac.START_M >= tds.START_M and ac.FINAL_M <= tds.FINAL_M)) " +
                        "INNER JOIN ADM_DISTANCE as ad on ad.ID = tds.ADM_DISTANCE_ID " +
                        "INNER JOIN TPL_PERIOD as tplp on tplp.ID = ac.PERIOD_ID " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE tds.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ") AND " +
                        "ac.PERIOD_ID in (SELECT tplp.ID FROM TPL_PERIOD as tplp " +
                        "INNER JOIN ADM_TRACK as admt on admt.ID = tplp.ADM_TRACK_ID " +
                        "WHERE \'" + dirListDate.ToShortDateString() + "\' between tplp.START_DATE AND tplp.FINAL_DATE " +
                        "AND admt.ID = " + trackID.ToString() + ")", commandType: CommandType.Text).ToList();

                    foreach (DLCrosstie dl in dlCrossties.OrderBy(u => u.Start_Km).ThenBy(u => u.Start_M).ThenBy(u => u.Final_Km).ThenBy(u => u.Final_M))
                    {
                        int god;
                        if (dl.Crosstie_Type_Id == 1)
                            god = 0;
                        else if (dl.Crosstie_Type_Id == 2)
                            god = 1;
                        else
                            god = -1;
                        textWriter.WriteLine($"{dl.Adm_Distance_Code}{dl.Start_Km,7}{dl.Start_M,7}{dl.Final_Km,7}{dl.Final_M,7}{god,9}");
                        textWriter.WriteLine(dl.Adm_Track_Code.ToString());
                    }
                }

                return true;
            }
        }

        public object GetKMs(DateTime processDateTime, int mtoObjectType, Int64 curveId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (mtoObjectType)
                {
                    case MainTrackStructureConst.MtoNonExtKm:
                        return db.Query<NonExtKm>("select tplnek.* from tpl_non_ext_km tplnek " +
                            "inner join tpl_period tplp on tplp.id = tplnek.period_id " +
                            "inner join adm_track admt on admt.id = tplp.adm_track_id " +
                            "where \'" + processDateTime.ToShortDateString().ToString() + "\' between tplp.start_date and tplp.final_date " +
                            "and admt.id in (select admt.id from adm_track admt " +
                            "inner join tpl_period tplp on tplp.adm_track_id = admt.id " +
                            "inner join apr_curve aprc on aprc.period_id = tplp.id " +
                            "where aprc.id = " + curveId.ToString() + ")", commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoNonStandard:
                        return db.Query<NonstandardKm>("select tplnk.* from tpl_nst_km tplnk " +
                            "inner join tpl_period tplp on tplp.id = tplnk.period_id " +
                            "inner join adm_track admt on admt.id = tplp.adm_track_id " +
                            "where \'" + processDateTime.ToShortDateString().ToString() + "\' between tplp.start_date and tplp.final_date " +
                            "and admt.id in (select admt.id from adm_track admt " +
                            "inner join tpl_period tplp on tplp.adm_track_id = admt.id " +
                            "inner join apr_curve aprc on aprc.period_id = tplp.id " +
                            "where aprc.id = " + curveId.ToString() + ")", commandType: CommandType.Text).ToList();
                    default: return new List<MainTrackObject>();
                }
            }
        }

        public List<RDCurve> GetRDCurves(Int64 curveId, Int64 mainProcessId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<RDCurve>("select * from rd_curve " +
                    "where process_id = " + mainProcessId.ToString() + " and " +
                    "curve_id = " + curveId.ToString(), commandType: CommandType.Text).ToList();
            }
        }

        public object Hole_diam(int km, int meter)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<int>(@"SELECT hole_diameter
					FROM public.cat_rails_type as rail
					inner join apr_rails_sections sec on sec.type_id = rail.id
					where ( " + km.ToString() + " between start_km and final_km) and (" + meter.ToString() + " between start_m and final_m)", commandType: CommandType.Text).ToList();
            }
        }

        public NonstandardKm GetNonStandardKm(Int64 id, int mtoObject, int nkm)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                List<NonstandardKm> result = new List<NonstandardKm>();
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (mtoObject)
                {

                    case MainTrackStructureConst.MtoCurve:
                        result = db.Query<NonstandardKm>(@"SELECT nst.* FROM public.tpl_nst_km as nst
                            INNER JOIN tpl_period as nst_period on nst_period.id = nst.period_id
                            INNER JOIN tpl_period as curve_period on curve_period.adm_track_id = nst_period.adm_track_id  and curve_period.mto_type = " + mtoObject + @"
                            INNER JOIN apr_curve as curve on curve.period_id = curve_period.id
                            where curve.id = " + id + " and nst.km = " + nkm, commandType: CommandType.Text).ToList();
                        break;

                }

                return result.Count > 0 ? result[0] : null;
            }
        }

        public string GetModificationDate()
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    return db.QueryFirst<DateTime>("select changed_on from adm_apr_tpl_log order by log_id desc limit 1").ToString("dd.MM.yyyy");
                }
                catch (Exception e)
                {
                    return "нет данных";
                }
            }
        }

        public List<RepairProject> GetAcceptRepairProject(Int64 trackId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<RepairProject>(@"Select repair.*, catalog.name as type, accept.name as Accept from repair_project repair
                    inner join cat_repair_type catalog on catalog.id = repair.type_id
                    inner join cat_accept_type accept on accept.id = repair.accept_id and accept.id = 1
                    where adm_track_id=" + trackId.ToString() +
                    " order by repair.start_km * 1000 + repair.start_m, repair.final_km * 1000 + repair.final_m, repair.repair_date", commandType: CommandType.Text).ToList();
            }
        }

        public bool SetAcceptRepairProject(Int64 trackId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                int result = db.Execute("update repair_project set accept_id = 1 where adm_track_id=" + trackId.ToString(), commandType: CommandType.Text);

                return result != 0;
            }
        }

        public List<Switch> GetFragmentsSwitch(Int64 trackId, Direction direction, double coord = 0.0)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                var filter = coord > 0 ? $" and coordinatetoreal(switch.km, switch.meter) {(direction == Direction.Direct ? ">" : "<")} {coord.ToString().Replace(",",".")}" : "";
                var order = (direction == Direction.Direct ? "asc" : "desc");
                var sqlText = $@"
                select 
                    switch.*, (switch.num || ' (' || station.name || ' ' || station.code || ')') as num
                from tpl_switch switch
                    inner join tpl_period period on period.id = switch.period_id
                    inner join adm_station station on station.id = switch.station_id
                where period.adm_track_id= { trackId } {filter}
                      
                 order by switch.km  {order}, switch.meter {order}";
                return db.Query<Switch>(sqlText, commandType: CommandType.Text).ToList();
            }
        }

        public List<AdmTrack> GetSwitchTracks(long switch_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {

                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<AdmTrack>($@"
                    select track.id, track.code, 
                        concat(coalesce(direction.code, station.code),'-', coalesce(direction.name, coalesce(station.name, concat(pstation.name,park.name)))) as belong, 
                        track.adm_direction_id as parent_id from tpl_switch as tps
                    inner join adm_station as fstation on fstation.id = tps.station_id
                    inner join tpl_period as tp on tp.id = tps.period_id and now() between tp.start_date and tp.final_date
                    inner join adm_track as track on track.id = tp.adm_track_id
                    left join adm_direction as direction on direction.id = track.adm_direction_id
					left join stw_track as st on st.adm_track_id = track.id
					left join adm_station as station on station.id = st.adm_station_id
					left join stw_park_track as stp on stp.adm_track_id = track.id
					left join stw_park as park on park.id = stp.stw_park_id
				    left join adm_station as pstation on pstation.id = park.adm_station_id
                    where tps.num = (select num from tpl_switch where id = {switch_id})
                        and tps.station_id = (select station_id from tpl_switch where id = {switch_id})").ToList();
            }
        }

        public Switch GetTrackSwitch(long track_Id, long switch_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {

                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.QueryFirst<Switch>($@"
                    select tsw.* from tpl_switch as tsw 
                    inner join tpl_period as tp on tp.id = tsw.period_id
                    inner join adm_track as track on track.id = tp.adm_track_id
                    
                    where tsw.num = (select num from tpl_switch where id = {switch_id})
                        and tsw.station_id = (select station_id from tpl_switch where id = {switch_id})
                        and track.id = {track_Id}");
            }
        }

        public List<long> GetCommomTracks(long start_station, long final_station)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {

                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<long>($@"
                    select distinct
                    track.id from adm_track as track 
                    inner join tpl_period as tp on tp.adm_track_id = track.id 
	                    and now() between tp.start_date and tp.final_date
                    inner join tpl_station_section as tss on tss.period_id = tp.id and tss.station_id = {start_station}
                    inner join tpl_station_section as tss2 on tss2.period_id = tp.id and tss2.station_id = {final_station}").ToList();
            }
        }

        public object GetMtoObjectsByCoord(DateTime date, int nkm, int mtoObjectType, long direction_id, string trackNumber, int meter = -1)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (mtoObjectType)
                {
                    case MainTrackStructureConst.MtoDistSection:

                    case MainTrackStructureConst.MtoCurve:
                        return db.Query<Curve>(@"Select cs.NAME as Side, acu.* from APR_CURVE as acu 
                            INNER JOIN CAT_SIDE as cs on cs.ID = acu.SIDE_ID 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = acu.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum and adn.id = @directCode 
                            and acu.START_KM <= @ncurkm and acu.FINAL_KM >= @ncurkm order by acu.START_KM, acu.START_M", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = direction_id }).ToList();
                    case MainTrackStructureConst.MtoNonStandard:
                        return db.Query<NonstandardKm>("Select tnk.* from TPL_NST_KM as tnk " +
                            "INNER JOIN TPL_PERIOD as tp on tp.ID = tnk.PERIOD_ID " +
                            "INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID " +
                            "INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID " +
                            "WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE " +
                            "and atr.CODE = @trackNum and adn.id = @directCode " +
                            "and tnk.KM = " + nkm, new { travelDate = date, trackNum = trackNumber, directCode = direction_id }).ToList();
                    case MainTrackStructureConst.MtoStraighteningThread:
                        return db.Query<StraighteningThread>("Select cst.* from APR_STRAIGHTENING_THREAD as cst " +
                            "INNER JOIN TPL_PERIOD as tp on tp.ID = cst.PERIOD_ID " +
                            "INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID " +
                            "INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID " +
                            "WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE " +
                            "and atr.CODE = @trackNum and adn.id = @directCode " +
                            "and cst.START_KM <= @ncurkm and cst.FINAL_KM >= @ncurkm ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = direction_id }).ToList();
                    case MainTrackStructureConst.MtoArtificialConstruction:
                        return db.Query<ArtificialConstruction>(@"Select aac.* from APR_ARTIFICIAL_CONSTRUCTION as aac 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = aac.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            inner join gettablecoordbylen(aac.km, aac.meter, aac.len / -2, tp.adm_track_id, @travelDate) as startcoords on true
                            inner join gettablecoordbylen(aac.km, aac.meter, aac.len / 2, tp.adm_track_id, @travelDate) as finalcoords on true
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum and adn.id = @directCode 
                            and startcoords.km <= @ncurkm and finalcoords.km >= @ncurkm ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = direction_id }).ToList();
                    case MainTrackStructureConst.MtoCrossTie:
                        return db.Query<CrossTie>(@"Select aac.* from APR_CROSSTIE as aac 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = aac.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum and adn.id = @directCode 
                            and aac.START_KM <= @ncurkm and aac.FINAL_KM >= @ncurkm ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = direction_id }).ToList();
                    case MainTrackStructureConst.MtoLongRails: //бесстык путь
                        return db.Query<LongRails>(@"Select acu.* from APR_LONG_RAILS as acu 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = acu.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum and adn.id = @directCode 
                            and acu.START_KM <= @ncurkm and acu.FINAL_KM >= @ncurkm ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = direction_id }).ToList();
                    case MainTrackStructureConst.MtoSwitch:
                        return db.Query<Switch>($@"Select tsw.*, ccm.len as length from TPL_SWITCH as tsw 
                            INNER JOIN cat_cross_mark as ccm on tsw.mark_id = ccm.id
                            INNER JOIN TPL_PERIOD as tp on tp.ID = tsw.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum 
                            and adn.id = @directCode
                            and {(meter < 0 ? " tsw.km = @ncurkm" : " @ncurkm between (tsw.km+tsw.meter/10000.0) and (tsw.km+(tsw.meter+ccm.len)/10000.0) ") }", new { ncurkm = (meter < 0 ? nkm : nkm + meter / 10000.0), travelDate = date, trackNum = trackNumber, directCode = direction_id }).ToList();
                    case MainTrackStructureConst.MtoProfileObject:
                        return db.Query<Switch>(@"Select tpo.* from tpl_profile_object as tpo 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = tpo.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum 
                            and adn.id = @directCode
                            and tpo.km = @ncurkm ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = direction_id }).ToList();
                    case MainTrackStructureConst.MtoSpeed:
                        return db.Query<Speed>(@"Select aps.* from APR_SPEED as aps 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = aps.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum 
                            and adn.id = @directCode 
                            and @ncurkm between aps.start_km and aps.final_km ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = direction_id }).ToList();
                    case MainTrackStructureConst.MtoCheckSection:
                        return db.Query<CheckSection>(@"Select tcs.* from tpl_check_sections as tcs 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = tcs.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum 
                            and adn.id = @directCode 
                            and @ncurkm between tcs.start_km and tcs.final_km ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = direction_id }).ToList();
                    case MainTrackStructureConst.MtoPdbSection:
                        return db.Query<PdbSection>(@"Select ap.ID, adc.code as distance, apc.CODE as pchu, apd.CODE as pd, ap.CODE as pdb, ap.chief_fullname as chief from ADM_PDB as ap 
                            INNER JOIN ADM_PD as apd on apd.ID = ap.ADM_PD_ID 
                            INNER JOIN ADM_PCHU as apc on apc.ID = apd.ADM_PCHU_ID 
                            INNER JOIN ADM_DISTANCE as adc on adc.ID = apc.ADM_DISTANCE_ID 
                            INNER JOIN tpl_pdb_section as tps on tps.adm_pdb_id = ap.id 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = tps.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum and adn.id = @directCode 
                            and @ncurkm between tps.start_km and tps.final_km 
                            ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = direction_id }).ToList();

                    case MainTrackStructureConst.MtoStCurve:

                        return db.Query<StCurve>(@"Select cs.NAME as Side, stcurve.* from apr_stcurve as stcurve
                            INNER JOIN APR_CURVE as acu on acu.id = stcurve.curve_id
                            INNER JOIN CAT_SIDE as cs on cs.ID = acu.SIDE_ID 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = acu.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @tracknum and adn.id = @dir_id 
                            and  @coord between (stcurve.start_km + stcurve.start_m/10000.0) and (stcurve.final_km + stcurve.final_m/10000.0)",
                            new { travelDate = date, tracknum = trackNumber, dir_id = direction_id, coord = (nkm + meter / 10000.0) }).ToList();
                    case MainTrackStructureConst.MtoNormaWidth:
                        return db.Query<NormaWidth>(@"Select aac.* from APR_NORMA_WIDTH as aac 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = aac.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN ADM_DIRECTION as adn on adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.CODE = @trackNum and adn.id = @directCode 
                            and aac.START_KM <= @ncurkm and aac.FINAL_KM >= @ncurkm ", new { ncurkm = nkm, travelDate = date, trackNum = trackNumber, directCode = direction_id }).ToList();

                    default: return new List<MainTrackObject>();
                }
            }
        }

        public object GetMtoObjectsByCoord(DateTime date, int nkm, int mtoObjectType, long track_id, int meter = -1)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (mtoObjectType)
                {
                    //Возвышение 6мм прямой
                    case MainTrackStructureConst.MtoElevation:
                        var query = $@"  SELECT
	                                        cnl.NAME AS Side,
	                                        ae.* 
                                        FROM
	                                        APR_ELEVATION AS ae
	                                        INNER JOIN CAT_NORMA_LEVEL AS cnl ON cnl.ID = ae.LEVEL_ID
	                                        INNER JOIN TPL_PERIOD AS tp ON tp.ID = ae.PERIOD_ID
	                                        INNER JOIN ADM_TRACK AS atr ON atr.ID = tp.ADM_TRACK_ID 
                                        WHERE
	                                        '{date:dd-MM-yyyy}' BETWEEN tp.START_DATE 
	                                        AND tp.FINAL_DATE 
	                                        AND atr.ID = {track_id} 
	                                        AND ae.START_KM <= {nkm} 
	                                        AND ae.FINAL_KM >= {nkm} 
                                        ORDER BY
	                                        ae.START_KM,
	                                        ae.START_M";
                        return db.Query<Elevation>(query).ToList();

                    case MainTrackStructureConst.MtoDistSection:
                    case MainTrackStructureConst.MtoCurve:
                        var curves = db.Query<Curve>(@"Select cs.NAME as Side, acu.*, curve.radius 
                            from APR_CURVE as acu 
                            INNER JOIN CAT_SIDE as cs on cs.ID = acu.SIDE_ID 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = acu.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID
	                        INNER JOIN apr_stcurve as curve ON curve.curve_id = acu.id
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.id = @trackId 
                            and acu.START_KM <= @ncurkm and acu.FINAL_KM >= @ncurkm order by acu.START_KM, acu.START_M", new { ncurkm = nkm, travelDate = date, trackId = track_id }).ToList();
                        foreach (var curve in curves)
                        {
                            curve.Straightenings = db.Query<StCurve>($@"
                             select stcurve.*, 
                                getcoordbylen(stcurve.start_km, stcurve.start_m, stcurve.transition_1, period.adm_track_id, '{date:dd.MM.yyyy}') as firsttransitionend,
                                getcoordbylen(stcurve.final_km, stcurve.final_m, -stcurve.transition_2, period.adm_track_id, '{date:dd.MM.yyyy}') as secondtransitionstart
                             from apr_stcurve stcurve
                                inner join apr_curve curve on curve.id = stcurve.curve_id
                                inner join tpl_period period on period.id = curve.period_id
                             where curve.id = {curve.Id}
                                order by stcurve.start_km * 10000 + stcurve.start_m").ToList();

                            curve.Elevations = db.Query<ElCurve>($@"
                            select elcurve.*,
                                getcoordbylen(elcurve.start_km, elcurve.start_m, elcurve.transition_1, period.adm_track_id, '{date.ToString("dd.MM.yyyy")}') as firsttransitionend,
                                getcoordbylen(elcurve.final_km, elcurve.final_m, -elcurve.transition_2, period.adm_track_id, '{date.ToString("dd.MM.yyyy")}') as secondtransitionstart
                             from apr_elcurve elcurve
                                inner join apr_curve curve on curve.id = elcurve.curve_id
                                inner join tpl_period period on period.id = curve.period_id
                                where curve.id = {curve.Id}
                                order by elcurve.start_km * 10000 + elcurve.start_m").ToList();
                        }
                        return curves;

                    //Класс пути (один объект)
                    case MainTrackStructureConst.MtoTrackClass:
                        var ttt = $@"select trackclass.*, classtype.name as class_type from apr_trackclass trackclass
                                    inner join tpl_period tperiod on tperiod.id = trackclass.period_id and ('{date.ToString("dd-MM-yyyy")}' between tperiod.start_date and tperiod.final_date)
                                    inner join adm_track track on track.id = tperiod.adm_track_id and track.id = {track_id}
                                    inner join cat_trackclass classtype on classtype.id = trackclass.class_id
                                    where {nkm} between trackclass.start_km and trackclass.final_km ";
                        return db.Query<TrackClass>(ttt).ToList();
                    case MainTrackStructureConst.MtoNonStandard:
                        return db.Query<NonstandardKm>("Select tnk.* from TPL_NST_KM as tnk " +
                            "INNER JOIN TPL_PERIOD as tp on tp.ID = tnk.PERIOD_ID " +
                            "INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID " +
                            "WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE " +
                            "and atr.id = @trackId " +
                            "and tnk.KM = " + nkm, new { travelDate = date, trackId = track_id }).ToList();
                    case MainTrackStructureConst.MtoStraighteningThread:
                        return db.Query<StraighteningThread>("Select cst.* from APR_STRAIGHTENING_THREAD as cst " +
                            "INNER JOIN TPL_PERIOD as tp on tp.ID = cst.PERIOD_ID " +
                            "INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID " +
                            "WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE " +
                            "and atr.id = @trackId " +
                            "and cst.START_KM <= @ncurkm and cst.FINAL_KM >= @ncurkm ", new { ncurkm = nkm, travelDate = date, trackId = track_id }).ToList();
                    case MainTrackStructureConst.MtoArtificialConstruction:
                        return db.Query<ArtificialConstruction>(@"Select aac.*, startcoords.km as start_km, startcoords.meter as start_m, finalcoords.km as final_km, finalcoords.meter as final_m, startentrance.km as entrance_start_km, startentrance.meter as entrance_start_m, finalentrance.km as entrance_final_km, finalentrance.meter as entrance_final_m  from APR_ARTIFICIAL_CONSTRUCTION as aac 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = aac.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            inner join gettablecoordbylen(aac.km, aac.meter, aac.len / -2, tp.adm_track_id, @travelDate) as startcoords on true
                            inner join gettablecoordbylen(aac.km, aac.meter, aac.len / 2, tp.adm_track_id, @travelDate) as finalcoords on true
                            inner join gettablecoordbylen(startcoords.km, startcoords.meter, -case WHEN aac.len between 25 and 100 then 200 WHEN aac.len > 100 then 500 else 0 END, tp.adm_track_id, @travelDate) as startentrance on true
							inner join gettablecoordbylen(finalcoords.km, finalcoords.meter, case WHEN aac.len between 25 and 100 then 200 WHEN aac.len > 100 then 500 else 0 END, tp.adm_track_id, @travelDate) as finalentrance on true
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.id = @trackId 
                            and startentrance.km <= @ncurkm and finalentrance.km >= @ncurkm ", new { ncurkm = nkm, travelDate = date, trackId = track_id }).ToList();
                    case MainTrackStructureConst.MtoCrossTie:
                        return db.Query<CrossTie>(@"Select aac.* from APR_CROSSTIE as aac 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = aac.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.id = @trackId 
                            and aac.START_KM <= @ncurkm and aac.FINAL_KM >= @ncurkm ", new { ncurkm = nkm, travelDate = date, trackId = track_id }).ToList();
                    case MainTrackStructureConst.MtoLongRails:
                        return db.Query<LongRails>(@"Select acu.* from APR_LONG_RAILS as acu 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = acu.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.id = @trackId 
                            and acu.START_KM <= @ncurkm and acu.FINAL_KM >= @ncurkm ", new { ncurkm = nkm, travelDate = date, trackId = track_id }).ToList();
                    case MainTrackStructureConst.MtoSwitch:
                        var sqll = $@"
                            SELECT * FROM
                            (SELECT 
	                            tsw.*, ccm.len AS length, ccm.mark,
	                            case when dir_id = {(int)SwitchDirection.Reverse} then tsw.km else minus.km END as start_km,
	                            case when dir_id = {(int)SwitchDirection.Reverse} then tsw.meter else minus.meter END as start_m,
	                            case when dir_id = {(int)SwitchDirection.Reverse} then plus.km else tsw.km END as final_km,
	                            case when dir_id = {(int)SwitchDirection.Reverse} then plus.meter else tsw.meter END as final_m
                            FROM TPL_SWITCH AS tsw 

                            INNER JOIN cat_cross_mark as ccm on tsw.mark_id = ccm.id
                            INNER JOIN TPL_PERIOD as tp on tp.ID = tsw.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            INNER JOIN gettablecoordbylen(tsw.km, tsw.meter, -ccm.len, tp.adm_track_id, '{date:dd-MM-yyyy}') as minus on true
                            INNER JOIN gettablecoordbylen(tsw.km, tsw.meter, ccm.len, tp.adm_track_id, '{date:dd-MM-yyyy}') as plus on true

                            WHERE 
	                            '{date:dd-MM-yyyy}' BETWEEN tp.START_DATE and tp.FINAL_DATE and 
	                            atr.id = {track_id} and  {nkm} in (plus.km,  tsw.km, minus.km)) AS sw 
                            WHERE sw.start_km = {nkm} or sw.final_km = {nkm}";

                        return db.Query<Switch>(sqll).ToList();

                    //case MainTrackStructureConst.MtoProfileObject:
                    case MainTrackStructureConst.MtoProfileObject:
                        var sql = $@"
                                    Select tpo.* from tpl_profile_object as tpo 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = tpo.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            WHERE '{date:dd-MM-yyyy}' BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.id = {track_id}
                            and tpo.km = {nkm}
                                    ";
                        return db.Query<ProfileObject>(sql).ToList();



                    //return db.Query<Switch>(@"Select tpo.* from tpl_profile_object as tpo 
                    //    INNER JOIN TPL_PERIOD as tp on tp.ID = tpo.PERIOD_ID 
                    //    INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                    //    WHERE '{date:dd-MM-yyyy}' BETWEEN tp.START_DATE and tp.FINAL_DATE 
                    //    and atr.id = @trackId 
                    //    and tpo.km = @ncurkm ", new { ncurkm = nkm, travelDate = date, trackId = track_id }).ToList();
                    case MainTrackStructureConst.MtoSpeed:

                        return db.Query<Speed>($@"Select aps.* from APR_SPEED as aps 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = aps.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.id = @trackId and
                            {(meter > 0 ? $"isbelong({nkm}, {meter}, aps.start_km, aps.start_m, aps.final_km, aps.final_m)" : $"{nkm} between aps.start_km and aps.final_km order by aps.start_km, aps.start_m") }
                            ", new { travelDate = date, trackId = track_id }).ToList();
                    case MainTrackStructureConst.MtoCheckSection:
                        return db.Query<CheckSection>(@"Select tcs.* from tpl_check_sections as tcs 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = tcs.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.id = @trackId 
                            and @ncurkm between tcs.start_km and tcs.final_km ", new { ncurkm = nkm, travelDate = date, trackId = track_id }).ToList();
                    case MainTrackStructureConst.MtoPdbSection:
                        return db.Query<PdbSection>(@"Select ap.ID, road.name as road, road.abbr as roadAbbr, adn.code as nod, adc.code as distance, apc.CODE as pchu, apd.CODE as pd, ap.CODE as pdb, ap.chief_fullname as chief from ADM_PDB as ap 
                            INNER JOIN ADM_PD as apd on apd.ID = ap.ADM_PD_ID 
                            INNER JOIN ADM_PCHU as apc on apc.ID = apd.ADM_PCHU_ID 
                            INNER JOIN ADM_DISTANCE as adc on adc.ID = apc.ADM_DISTANCE_ID 
                            INNER JOIN ADM_NOD as adn on adn.ID = adc.ADM_NOD_ID
                            INNER JOIN ADM_ROAD as road on road.id = adn.adm_road_id
                            INNER JOIN tpl_pdb_section as tps on tps.adm_pdb_id = ap.id 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = tps.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.id = @trackId 
                            and @ncurkm between tps.start_km and tps.final_km 
                            ", new { ncurkm = nkm, travelDate = date, trackId = track_id }).ToList();
                    case MainTrackStructureConst.MtoStCurve:
                        return db.Query<StCurve>(@"Select cs.NAME as Side, stcurve.* from apr_stcurve as stcurve
                            INNER JOIN APR_CURVE as acu on acu.id = stcurve.curve_id
                            INNER JOIN CAT_SIDE as cs on cs.ID = acu.SIDE_ID 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = acu.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.id = @trackId 
                            and  @coord between (stcurve.start_km + stcurve.start_m/10000.0) and (stcurve.final_km + stcurve.final_m/10000.0)",
                            new { travelDate = date, trackId = track_id, coord = (nkm + meter / 10000.0) }).ToList();
                    case MainTrackStructureConst.MtoNormaWidth:
                        return db.Query<NormaWidth>(@"Select aac.* from APR_NORMA_WIDTH as aac 
                            INNER JOIN TPL_PERIOD as tp on tp.ID = aac.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.id = @trackId 
                            and aac.START_KM <= @ncurkm and aac.FINAL_KM >= @ncurkm ", new { ncurkm = nkm, travelDate = date, trackId = track_id }).ToList();
                    case MainTrackStructureConst.MtoStationSection:
                        return db.Query<StationSection>(@"Select nod.id as nod_id, road.name as road, nod.code as nod, station.name as station, tss.* from adm_station as station
                            INNER JOIN tpl_station_section as tss on tss.station_id = station.id 
							INNER JOIN adm_nod as nod on nod.id = station.adm_nod_id
							INNER JOIN adm_road as road on road.id = nod.adm_road_id
                            INNER JOIN TPL_PERIOD as tp on tp.ID = tss.PERIOD_ID 
                            INNER JOIN ADM_TRACK as atr on atr.ID = tp.ADM_TRACK_ID 
                            WHERE @travelDate BETWEEN tp.START_DATE and tp.FINAL_DATE 
                            and atr.id = @trackId 
                            and @ncurkm between tss.start_km and tss.final_km
                            ", new { ncurkm = nkm, travelDate = date, trackId = track_id }).ToList();
                    case MainTrackStructureConst.MtoRepairProject:
                        return db.Query<RepairProject>($@"
                            select * from repair_project 
                            where 
	                            adm_track_id = {track_id} 
	                            and ('{date:dd-MM-yyyy}' BETWEEN repair_date and accepted_at or ('{date:dd-MM-yyyy}'> repair_date and accepted_at is null ))
	                            and {nkm} BETWEEN start_km and final_km
                            ").ToList();

                    default: return new List<MainTrackObject>();
                }
            }
        }

        public void InsertRoute(List<Fragment> route, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                db.Execute($@"delete from fragments where trip_id = {trip_id}");
                foreach (var fragment in route)
                {
                    db.Execute(@"
                        INSERT INTO public.fragments(
	                        trip_id, adm_track_id, start_km, start_m, final_km, final_m, start_switch_id, final_switch_id, belong_id)
	                        VALUES (@tripid, @admtrackid, @startkm, @startm, @finalkm, @finalm, @ssid, @fsid, @belong);", new
                    {
                        tripid = trip_id,
                        admtrackid = fragment.Track_Id,
                        startkm = fragment.Start_Km,
                        startm = fragment.Start_M,
                        finalkm = fragment.Final_Km,
                        finalm = fragment.Final_M,
                        ssid = fragment.Start_Switch_Id,
                        fsid = fragment.Final_Switch_Id,
                        belong = fragment.Belong_Id

                    });
                }
            }
        }
        public object GetMtoObject(long mtoObjectId, int mtoObjectType)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (mtoObjectType)
                {
                    case MainTrackStructureConst.MtoCurve:
                        return db.Query<Curve>($@"select * from apr_curve curve
                            where curve.id = {mtoObjectId}", commandType: CommandType.Text).First();
                    default: return new MainTrackObject();
                }
            }
        }

        public List<Kilometer> GetKilometersOfFragment(Fragment fragment, DateTime date, Direction direction, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                var sqltext = $@"
                    SELECT
	                    kms.kms AS NUMBER,
	                    COALESCE (km.final, tnk.len, 1000 ) AS final_m,
	                    COALESCE (abs(km.start-km.final), tnk.len, 1000 ) AS LENGTH, { fragment.Track_Id } AS track_id, { fragment.Id } AS fragment_id,
	                    track.code AS track_name,
                        km.start_index,
	                    km.final_index,
                        start_index is not null as IsPrinted,
                        bed.ball as point,
                        COALESCE(km.id,0) as id,
                        concat(direction.name, '(', direction.code, ')') as direction_name,
                        direction.code as direction_code
                    FROM
	                    generate_series ( { fragment.Start_Km }, { fragment.Final_Km }, { (int)direction } ) AS kms
	                    INNER JOIN adm_track AS track ON track.ID = { fragment.Track_Id }
                        inner join adm_direction as direction on direction.id = track.adm_direction_id

                        LEFT JOIN kilometers AS km ON km.track_id = track.ID 
	                        AND km.trip_id = {trip_id} 
	                        AND km.num = kms.kms
                        LEFT JOIN bedemost AS bed ON km.track_id = bed.track_id AND km.num = bed.km and bed.trip_id = km.trip_id
	                    LEFT JOIN (
	                    SELECT
		                    km,
		                    len 
	                    FROM
		                    tpl_nst_km
		                    INNER JOIN tpl_period AS tp ON tp.ID = tpl_nst_km.period_id 
	                    WHERE
		                    tp.adm_track_id = { fragment.Track_Id } 
	                    ) AS tnk ON tnk.km = kms.kms 
                    WHERE
	                    kms.kms NOT IN (
	                    SELECT
		                    km 
	                    FROM
		                    tpl_non_ext_km AS nonext
		                    INNER JOIN tpl_period AS tp ON tp.ID = nonext.period_id 
	                    WHERE
		                    tp.adm_track_id = { fragment.Track_Id } 
	                    ) 
                    
                    ORDER BY
	                    kms.kms { (direction == Direction.Direct ? "asc" : "desc") } ";
                return db.Query<Kilometer>(sqltext).ToList();
            }
        }
        public string GetSector(long track_id, int nkm, DateTime trip_date)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

               

                var sqltext = $@"
                    select concat(start.name,'-', final.name) from 
                        (select tss.station_id, tss.axis_km, 
                            coalesce(LEAD(tss.axis_km,1) OVER (ORDER BY axis_km, axis_m), tss.final_km) as next_axis_km,
                            coalesce(LEAD(tss.station_id,1) OVER (ORDER BY axis_km, axis_m), tss.station_id) as next_station_id			   
			             from tpl_station_section as tss
                         inner join tpl_period as period on period.id = tss.period_id and period.adm_track_id = {track_id} and '{trip_date.ToString("dd.MM.yyyy")}' between period.start_date and period.final_date 
                            order by axis_km, axis_m) 
                    as sector
                    inner join adm_station as start on start.id = sector.station_id
                    inner join adm_station as final on final.id = sector.next_station_id
                    where {nkm} between axis_km and next_axis_km";
                try
                {
                    return db.Query<string>(sqltext).Last();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetSectorError: " + e.Message);
                    return null;
                }
            }
        }
        public int GetDistanceBetween2Coord(int start_km, int start_m, int final_km, int final_m, long track_id, DateTime date)
        {
           
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                var sqltext = $@"
                    select * from getdistancefrom({start_km}, {start_m}, {final_km}, {final_m}, {track_id}, '{date.ToString("dd.MM.yyyy")}')";
                try
                {
                    return db.QueryFirst<int>(sqltext);
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetDistanceBetween2CoordError: " + e.Message);
                    return -1;
                }
            }
        }

        public CoordinateGNSS GetCoordByLen(int start_km, int start_m, int length, long track_id, DateTime date)
        {
            if (length == 0)
                return new CoordinateGNSS() { Km = start_km, Meter = start_m };
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                var sqltext = $@"
                    select * from gettablecoordbylen({start_km}, {start_m}, {length},  {track_id}, '{date.ToString("dd.MM.yyyy")}')";
                try
                {
                    return db.QueryFirst<CoordinateGNSS>(sqltext);
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetCoordByLenError: " +e.Message);
                    return null;
                }
            }
        }

        public List<Curve> GetCurveByTripIdToDate(MainParametersProcess tripProcess)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var query = $@"SELECT
	                                cs.NAME AS side,
	                                aprc.*,
	                                tplp.start_date,
	                                tplp.final_date,
	                                admt.ID AS track_id,
	                                admt.code AS track_code,
	                                MAX ( stcurve.radius ) AS radius,
	                                COALESCE ( speed.passenger, - 1 ) AS passspeed ,
                                    aprc.start_km*1000+aprc.start_m  start_coord,
	                                aprc.final_km*1000+aprc.final_m  final_coord
                                FROM
	                                apr_curve aprc
	                                INNER JOIN apr_stcurve stcurve ON stcurve.curve_id = aprc.
	                                ID INNER JOIN cat_side cs ON cs.ID = aprc.side_id
	                                INNER JOIN tpl_period tplp ON tplp.ID = aprc.period_id
	                                INNER JOIN adm_track admt ON admt.ID = tplp.adm_track_id
	                                LEFT JOIN tpl_period speedperiod ON speedperiod.adm_track_id = admt.ID 
	                                AND speedperiod.mto_type = 6 
	                                AND is_newest_period ( speedperiod.ID, speedperiod.mto_type, admt.ID )
	                                LEFT JOIN apr_speed speed ON speed.period_id = speedperiod.ID 
	                                AND isbelong ( aprc.start_km, aprc.start_m, speed.start_km, speed.start_m, speed.final_km, speed.final_m )
	                                INNER JOIN tpl_period section_period ON admt.ID = section_period.adm_track_id
	                                INNER JOIN tpl_dist_section SECTION ON section_period.ID = SECTION.period_id 
	                                AND (
		                                isbelong ( aprc.start_km, aprc.start_m, SECTION.start_km, SECTION.start_m, SECTION.final_km, SECTION.final_m ) 
		                                OR isbelong ( aprc.final_km, aprc.final_m, SECTION.start_km, SECTION.start_m, SECTION.final_km, SECTION.final_m ) 
	                                ) 
                                WHERE
	                                admt.ID = {tripProcess.TrackID} 
	                                AND '{tripProcess.Date_Vrem}' BETWEEN tplp.start_date 
	                                AND tplp.final_date 
                                GROUP BY
	                                aprc.ID,
	                                cs.ID,
	                                speed.ID,
	                                admt.ID,
	                                admt.code,
	                                tplp.start_date,
	                                tplp.final_date 
                                ORDER BY
	                                aprc.ID";

                    return db.Query<Curve>(query).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"GetCurvesList " + e.Message);
                    return null;
                }
            }
        }

        public List<RefPoint> GetRefPointsByTripIdToDate(long track_id, DateTime date_Vrem)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var query =  $@"SELECT
	                                    * 
                                    FROM
	                                    apr_ref_point 
	                                    INNER JOIN tpl_period period ON period.ID = apr_ref_point.period_id
	                                    INNER JOIN adm_track track ON track.ID = period.adm_track_id 
                                    WHERE
                                        adm_track_id = {track_id}
                                        AND ('{date_Vrem}' BETWEEN period.start_date and period.final_date)
                                    ORDER BY
	                                    km + meter/10000.0;";

                    return db.Query<RefPoint>(query).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"GetCurvesList " + e.Message);
                    return null;
                }
            }
        }

        public List<DistSection> GetDistSectionByDistId(long distId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var query = $@"SELECT
	                                ar.NAME AS road,
	                                ad.ID AS distanceId,
	                                ad.NAME AS distanceName,
	                                ad.code AS distanceCode,
	                                ( 'ПЧУ-' || apc.CODE || '/ПД-' || apd.CODE || '/ПДБ-' || ap.CODE ) AS note,
	                                apc.CODE pchu,
	                                apd.CODE pd,
	                                ap.CODE pdb,
	                                tps.*,
	                                ( final_km * 1000+final_m ) - ( start_km * 1000+start_m ) len 
                                FROM
	                                TPL_PDB_SECTION AS tps
	                                INNER JOIN ADM_PDB AS ap ON ap.ID = tps.ADM_PDB_ID
	                                INNER JOIN ADM_PD AS apd ON apd.ID = ap.ADM_PD_ID
	                                INNER JOIN ADM_PCHU AS apc ON apc.ID = apd.ADM_PCHU_ID
	                                INNER JOIN ADM_DISTANCE AS ad ON ad.ID = apc.ADM_DISTANCE_ID
	                                INNER JOIN ADM_NOD AS an ON an.ID = ad.ADM_NOD_ID
	                                INNER JOIN ADM_ROAD AS ar ON ar.ID = an.ADM_ROAD_ID 
                                WHERE
	                                ad.ID = {distId} 
                                ORDER BY
	                                tps.start_km * 1000 + tps.start_m,
	                                tps.final_km * 1000 + tps.final_m";
                    return db.Query<DistSection>(query).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"GetDistSectionByDistId " + e.Message);
                    return null;
                }
            }
        }

        public void Pru_write(long track_id, Kilometer kilometer, List<DigressionMark> pru_dig_list)
        {
            try
            {
                using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    foreach (var item in pru_dig_list)
                    {
                        if (item.DigName == DigressionName.SpeedUpNear.Name || item.DigName == DigressionName.SpeedUp.Name)
                        {
                            //АНП
                            var query = $@"
                                        INSERT INTO s3(km, meter, trip_id, ots, len, track_id, isadditional, typ, primech, uv, uvg, ovp, ogp, pch, naprav, put)
	                                    VALUES ('{kilometer.Number}', '{item.Meter}', '{kilometer.Trip.Id}', '{item.DigName}', '{item.Length}', '{kilometer.Track_id}', 0, 2, '{item.Comment}',
                                                '{item.PassengerSpeedAllow}', '{item.FreightSpeedAllow}', '{item.PassengerSpeedLimit}', '{item.FreightSpeedLimit}', '{item.Pch}', '{item.DirectionName}', '{item.TrackName}')";
                            db.Execute(query);
                        }
                        else if (item.DigName == DigressionName.Pru.Name)
                        {
                            //ПРУ
                            db.Execute($@"
                                INSERT INTO s3(km, meter, trip_id, ots, kol, otkl, len, track_id, isadditional, typ, pch, naprav, put)
	                            VALUES ('{kilometer.Number}', '{item.Meter}', '{kilometer.Trip.Id}', '{item.DigName}', '{item.Count}', '{item.Value}', '{item.Length}', '{kilometer.Track_id}', 0,2, '{item.Pch}', '{item.DirectionName}', '{item.TrackName}')");
                        }
                        else
                        {
                            //Записать натурную кривую
                            var query = $@"
                                INSERT INTO s3(km, meter, trip_id, ots, primech, track_id, isadditional, typ )
	                            VALUES ('{kilometer.Number}', '{item.Meter}', '{kilometer.Trip.Id}', '{item.Alert}', 'Натурная кривая', '{kilometer.Track_id}', 0,2)";
                            db.Execute(query);
                        }

                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error Pru_write " + e.Message);
            }
        }

        public void Bpd_write(long track_id, Kilometer kilometer, List<DigressionMark> curve_bpd_list)
        {
            try
            {
                using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    foreach (var item in curve_bpd_list)
                    {
                        
                            //Записать Паспортная  кривую
                            var query = $@"
                                INSERT INTO s3(km, meter, trip_id, ots, primech, track_id, isadditional, typ )
	                            VALUES ('{kilometer.Number}', '{item.Meter}', '{kilometer.Trip.Id}', '{item.Alert}', 'Паспортная кривая', '{kilometer.Track_id}', 0,2 )";
                           db.Execute(query);
                        
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Bpd_write " + e.Message);
            }
        }

        public List<Speed> GetSpeeds(DateTime tripDate, string directionName, string trackNumber)
        {
            try
            {
                using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var speeds = db.Query<Speed>($@"
                            SELECT
	                            aps.* ,
	                            atr.CODE,
	                            adn.NAME 
                            FROM
	                            APR_SPEED AS aps
	                            INNER JOIN TPL_PERIOD AS tp ON tp.ID = aps.PERIOD_ID
	                            INNER JOIN ADM_TRACK AS atr ON atr.ID = tp.ADM_TRACK_ID
	                            INNER JOIN ADM_DIRECTION AS adn ON adn.ID = atr.ADM_DIRECTION_ID 
                            WHERE
	                            '{tripDate}' BETWEEN tp.START_DATE 
	                            AND tp.FINAL_DATE 
	                            AND atr.CODE = '{trackNumber}' 
	                            AND adn.NAME = '{directionName}' 
                                AND lastochka > 60 
                            ORDER BY
	                            start_km,
	                            start_m").ToList();
                    return speeds;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}