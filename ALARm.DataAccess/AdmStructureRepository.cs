using ALARm.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using Npgsql;
using ALARm.Core.Report;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ALARm.DataAccess
{
    public class AdmStructureRepository : IAdmStructureRepository
    {
        public bool Delete(Int64 unitId, int unitLevel)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                var admUnitTableName = String.Empty;
                switch(unitLevel)
                {
                    case AdmStructureConst.AdmRoad:
                        admUnitTableName = "ADM_ROAD";
                        break;
                    case AdmStructureConst.AdmNod:
                        admUnitTableName = "ADM_NOD";
                        break;
                    case AdmStructureConst.AdmDistance:
                        admUnitTableName = "ADM_DISTANCE";
                        break;
                    case AdmStructureConst.AdmPchu:
                        admUnitTableName = "ADM_PCHU";
                        break;
                    case AdmStructureConst.AdmPd:
                        admUnitTableName = "ADM_PD";
                        break;
                    case AdmStructureConst.AdmPdb:
                        admUnitTableName = "ADM_PDB";
                        break;
                    case AdmStructureConst.AdmDirection:
                        admUnitTableName = "ADM_DIRECTION";
                        break;
                    case AdmStructureConst.AdmTrack:
                        admUnitTableName = "ADM_TRACK";
                        break;
                    case AdmStructureConst.AdmStation:
                        admUnitTableName = "ADM_STATION";
                        break;
                    case AdmStructureConst.AdmPark:
                        admUnitTableName = "stw_park";
                        break;
                    case AdmStructureConst.AdmStationObject:
                        admUnitTableName = "stw_object";
                        break;
                    case AdmStructureConst.AdmStationTrack:
                        return 0 != db.Execute("Delete from adm_track where id in " +
                            "(select adm_track_id from stw_track where id=@parentID); " +
                            "Delete from stw_track where ID=@parentID;", new { parentID = unitId }, commandType: CommandType.Text);
                    case AdmStructureConst.AdmParkTrack:
                        return 0 != db.Execute("Delete from adm_track where id in " +
                            "(select adm_track_id from stw_park_track where id=@parentID); " +
                            "Delete from stw_park_track where ID=@parentID;", new { parentID = unitId }, commandType: CommandType.Text);
                }
                if (admUnitTableName.Equals(String.Empty))
                    return false;
                int result = db.Execute("Delete from "+ admUnitTableName + " where ID=@parentID", new { parentID = unitId }, commandType: CommandType.Text);
                return result != 0;
            }
        }

        public bool DeleteDirection(long id, long roadid, bool delMode)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                int result = 0;
                
                if (delMode) //true - delete all
                {
                    result = db.Execute("delete from road_direction where direction_id=@id", new { id }, commandType: CommandType.Text);
                    if (result != 0)
                    {
                        result = db.Execute("delete from adm_direction where id=@id", new { id }, commandType: CommandType.Text);
                    }
                    return result != 0;
                }
                else //false - delete one
                {
                    result = db.Execute("delete from road_direction where direction_id=@id and road_id=@roadid", new { id, roadid }, commandType: CommandType.Text);
                    return result != 0;
                }
            }
        }

        public List<Catalog> GetCatalog(int admObjectType)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string catalogTableName = string.Empty;
                switch (admObjectType)
                {
                    case AdmStructureConst.AdmStation:
                        catalogTableName = "CAT_STATION_TYPE";
                        break;
                    case AdmStructureConst.AdmStationObject:
                        catalogTableName = "cat_stw_object_type";
                        break;
                    case AdmStructureConst.AdmStationTrack:
                    case AdmStructureConst.AdmParkTrack:
                        catalogTableName = "cat_track_type";
                        break;
                    case AdmStructureConst.AdmPark:
                        catalogTableName = "cat_park_type";
                        break;
                }
                return catalogTableName.Equals(string.Empty) ? new List<Catalog>() : db.Query<Catalog>("Select * from " + catalogTableName + "", commandType: CommandType.Text).ToList();

            }
        }

        public object GetDitancesOfRoad(Int64 roadId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<AdmDistance>("Select ad.ID, (ad.NAME) as NAME from ADM_DISTANCE as ad " +
                    "INNER JOIN ADM_NOD as an on an.ID = ad.ADM_NOD_ID " +
                    " where an.ADM_ROAD_ID=" + roadId.ToString() + "  order by ad.ID", commandType: CommandType.Text).ToList();
            }
        }

        public object GetPartsOfDistance(Int64 distanceId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                
                return db.Query<AdmPdb>("Select ap.ID, ('ПЧУ-' || apc.CODE || '/ПД-' || apd.CODE || '/ПДБ-' || ap.CODE) as NAME from ADM_PDB as ap " +
                    "INNER JOIN ADM_PD as apd on apd.ID = ap.ADM_PD_ID " +
                    "INNER JOIN ADM_PCHU as apc on apc.ID = apd.ADM_PCHU_ID " +
                    " where apc.ADM_DISTANCE_ID=" + distanceId.ToString() + "  order by apc.ID, apd.ID, ap.ID", commandType: CommandType.Text).ToList();
            }
        }

        public object GetUnits(int admLevel, Int64 parentId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                String sqltext = String.Empty;
                switch (admLevel)
                {
                    case AdmStructureConst.AdmRoad:
                        sqltext = "Select * from ADM_ROAD order by ID";
                        break;
                    case AdmStructureConst.AdmNod:
                        sqltext = "Select * from ADM_NOD where ADM_ROAD_ID=" + parentId.ToString() + "  order by ID";
                        break;
                    case AdmStructureConst.AdmDistance:
                        if (parentId == -1)
                            sqltext = "Select * from ADM_DISTANCE order by ID";
                        else
                            sqltext = "Select * from ADM_DISTANCE where ADM_NOD_ID=" + parentId.ToString() + "  order by ID";
                        break;
                    case AdmStructureConst.AdmPchu:
                        sqltext = "Select * from ADM_PCHU where ADM_DISTANCE_ID=" + parentId.ToString() + "  order by ID";
                        break;
                    case AdmStructureConst.AdmPd:
                        sqltext = "Select * from ADM_PD where ADM_PCHU_ID=" + parentId.ToString() + "  order by ID";
                        break;
                    case AdmStructureConst.AdmPdb:
                        sqltext = "Select * from ADM_PDB where ADM_PD_ID=" + parentId.ToString() + "  order by ID";
                        break;
                    case AdmStructureConst.AdmDirection:
                        if (parentId < 1)
                        {
                            sqltext = "Select * from ADM_DIRECTION  order by ID";
                        }
                        else
                        {
                            sqltext = @"Select direction.*from ADM_DIRECTION direction
                                inner join road_direction rdlink on rdlink.direction_id = direction.id and rdlink.road_id=" + parentId.ToString() + " order by direction.id";
                        }
                        break;
                    case AdmStructureConst.AdmTrack:
                        sqltext = $@"select track.id, track.code, track.adm_direction_id as parent_id, (min(section.start_km * 1000 + section.start_m) / 1000 || ' км ' || min(section.start_km * 1000 + section.start_m) % 1000 || ' м - ' || max(section.final_km * 1000 + section.final_m) / 1000 || ' км ' || max(section.final_km * 1000 + section.final_m) % 1000 || ' м') as border, 
                                    concat(coalesce(direction.code, station.code),'-', coalesce(direction.name, coalesce(station.name, concat(pstation.name,park.name)))) as belong
                                    from adm_track track 
                                    left join tpl_period period on period.adm_track_id = track.id and period.mto_type = 0 
                                    and period.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = track.id and tpl_period.mto_type = 0) 
                                    left join tpl_dist_section section on section.period_id = period.id 
                                    left join adm_direction as direction on direction.id = track.adm_direction_id
					                left join stw_track as st on st.adm_track_id = track.id
					                left join adm_station as station on station.id = st.adm_station_id
					                left join stw_park_track as stp on stp.adm_track_id = track.id
					                left join stw_park as park on park.id = stp.stw_park_id
				                    left join adm_station as pstation on pstation.id = park.adm_station_id
                                    where track.ADM_DIRECTION_ID= {parentId} group by track.id, direction.code, station.code, direction.name, station.name, pstation.name,park.name, pstation.code  order by track.ID";
                        return db.Query<AdmTrack>(sqltext, commandType: CommandType.Text).ToList();
                    case AdmStructureConst.AdmStation:
                        if (parentId == 0)
                        {
                            sqltext = "Select * from ADM_STATION order by ID";
                            return db.Query<Station>(sqltext, commandType: CommandType.Text).ToList();
                        }
                        else
                        {
                            sqltext = "Select ast.*, st.NAME as object_type from ADM_STATION as ast " +
                                "INNER JOIN CAT_STATION_TYPE as st on st.ID = ast.TYPE_ID " +
                                "where ast.ADM_NOD_ID=" + parentId.ToString() + "  order by ID";
                            return db.Query<Station>(sqltext, commandType: CommandType.Text).ToList();
                        }
                    case AdmStructureConst.AdmPark:
                        sqltext = @"Select stwp.*, parktype.NAME as object_type from stw_park as stwp 
                            INNER JOIN cat_park_type as parktype on parktype.ID = stwp.TYPE_ID
                            where stwp.adm_station_id=" + parentId.ToString() + " order by ID";
                        return db.Query<Park>(sqltext, commandType: CommandType.Text).ToList();
                    case AdmStructureConst.AdmStationObject:
                        sqltext = @"Select stwo.*, csot.NAME as object_type from stw_object stwo  
                            INNER JOIN cat_stw_object_type as csot on csot.ID = stwo.TYPE_ID
                            where stwo.stw_park_id=" + parentId.ToString() + " order by ID";
                        return db.Query<StationObject>(sqltext, commandType: CommandType.Text).ToList();
                    case AdmStructureConst.AdmStationTrack:
                        sqltext = @"Select stwt.*, trackt.NAME as object_type, station.name as Station, track.code as Code, track.id as Adm_track_id,
                            (min(section.start_km * 1000 + section.start_m) / 1000 || ' км ' || min(section.start_km * 1000 + section.start_m) % 1000 || ' м - ' || max(section.final_km * 1000 + section.final_m) / 1000 || ' км ' || max(section.final_km * 1000 + section.final_m) % 1000 || ' м') as border 
                            , concat (station.code,' ', station.name) as belong
                            from stw_track as stwt 
                            INNER JOIN cat_track_type as trackt on trackt.ID = stwt.TYPE_ID
                            inner join adm_track as track on track.id = stwt.adm_track_id
                            inner join adm_station as station on stwt.adm_station_id = station.id
                            left join tpl_period period on period.adm_track_id = track.id and period.mto_type = 9
                                and period.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = track.id and tpl_period.mto_type = 9)
                            left join tpl_pdb_section section on section.period_id = period.id
                            where stwt.adm_station_id=" + parentId.ToString() + " group by stwt.id, trackt.id, station.id, track.id, station.code, station.name order by stwt.ID";
                        return db.Query<StationTrack>(sqltext, commandType: CommandType.Text).ToList();
                    case AdmStructureConst.AdmParkTrack:
                        sqltext = @"Select stwt.*, trackt.NAME as object_type, park.name as Park, track.code as Code, track.id as Adm_track_id,
                            (min(section.start_km * 1000 + section.start_m) / 1000 || ' км ' || min(section.start_km * 1000 + section.start_m) % 1000 || ' м - ' || max(section.final_km * 1000 + section.final_m) / 1000 || ' км ' || max(section.final_km * 1000 + section.final_m) % 1000 || ' м') as border 
                            , concat (station.code,' ', park.name) as belong
                            from stw_park_track as stwt 
                            INNER JOIN cat_track_type as trackt on trackt.ID = stwt.TYPE_ID
                            inner join adm_track as track on track.id = stwt.adm_track_id
                            left join stw_park as park on stwt.stw_park_id = park.id
                            left join adm_station as station on station.id = park.adm_station_id
                            left join tpl_period period on period.adm_track_id = track.id and period.mto_type = 9
                                and period.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = track.id and tpl_period.mto_type = 9)
                            left join tpl_pdb_section section on section.period_id = period.id
                            where stwt.stw_park_id=" + parentId.ToString() + " group by stwt.id, trackt.id, park.id, track.id, station.code, park.name order by ID";
                        return db.Query<StationTrack>(sqltext, commandType: CommandType.Text).ToList();
                    case AdmStructureConst.AdmStationSection:
                        sqltext = @"Select distinct tss.*, ast.NAME as station from adm_station as ast 
                            INNER JOIN CAT_STATION_TYPE as st on st.ID = ast.TYPE_ID
                            INNER JOIN tpl_station_section as tss on tss.station_id = ast.id
                            INNER JOIN tpl_period as tp on tp.id = tss.period_id
                            INNER JOIN adm_track as track on track.id = tp.adm_track_id
                            where track.id = " + parentId + " and now() between tp.start_date and tp.final_date order by ast.name ";
                        return db.Query<StationSection>(sqltext, commandType: CommandType.Text).ToList();


                }
                if (sqltext.Equals(String.Empty))
                    return null;
                return db.Query<AdmUnit>(sqltext, commandType: CommandType.Text).ToList();
            }
        }

        public Int64 Insert(object obj)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                if (obj is AdmRoad)
                {
                    return (Int64) db.ExecuteScalar("insertroad",
                        new {code = (obj as AdmRoad).Code, rname = (obj as AdmRoad).Name, abbr = (obj as AdmRoad).Abbr},
                        commandType: CommandType.StoredProcedure);
                }
                else if (obj is AdmNod)
                {
                    return (Int64) db.ExecuteScalar("insertnod",
                        new
                        {
                            code = (obj as AdmNod).Code, rname = (obj as AdmNod).Name,
                            roadid = (obj as AdmNod).Parent_Id
                        }, commandType: CommandType.StoredProcedure);
                }
                else if (obj is AdmDistance)
                {
                    return (Int64) db.ExecuteScalar("insertdistance",
                        new
                        {
                            code = (obj as AdmDistance).Code, rname = (obj as AdmDistance).Name,
                            nodid = (obj as AdmDistance).Parent_Id
                        }, commandType: CommandType.StoredProcedure);
                }
                else if (obj is AdmPchu)
                {
                    return (Int64) db.ExecuteScalar("insertpchu",
                        new
                        {
                            code = (obj as AdmPchu).Code, chiefname = (obj as AdmPchu).Name,
                            distanceid = (obj as AdmPchu).Parent_Id
                        }, commandType: CommandType.StoredProcedure);
                }
                else if (obj is AdmPd)
                {
                    return (Int64) db.ExecuteScalar("insertpd",
                        new
                        {
                            code = (obj as AdmPd).Code, chiefname = (obj as AdmPd).Name,
                            pchuid = (obj as AdmPd).Parent_Id
                        }, commandType: CommandType.StoredProcedure);
                }
                else if (obj is AdmPdb)
                {
                    return (Int64) db.ExecuteScalar("insertpdb",
                        new
                        {
                            code = (obj as AdmPdb).Code, chiefname = (obj as AdmPdb).Name,
                            pdid = (obj as AdmPdb).Parent_Id
                        }, commandType: CommandType.StoredProcedure);
                }
                else if (obj is AdmDirection)
                {
                    return (Int64) db.ExecuteScalar("insertdirection",
                        new { dcode = (obj as AdmDirection).Code, dname = (obj as AdmDirection).Name, roadid = (obj as AdmDirection).Parent_Id },
                        commandType: CommandType.StoredProcedure);
                }
                else if (obj is AdmTrack)
                {
                    return (Int64) db.ExecuteScalar("inserttrack",
                        new {code = (obj as AdmTrack).Code, directionid = (obj as AdmTrack).Parent_Id},
                        commandType: CommandType.StoredProcedure);
                }
                else if (obj is Station)
                {
                    return (Int64) db.ExecuteScalar("insertstation",
                        new
                        {
                            code = (obj as Station).Code, nodid = (obj as Station).Parent_Id,
                            sname = (obj as Station).Name, typeid = (obj as Station).Type_id
                        }, commandType: CommandType.StoredProcedure);
                }
                else if (obj is Park)
                {
                    return (Int64) db.ExecuteScalar("insertpark",
                        new
                        {
                            pname = (obj as Park).Name, stationid = (obj as Park).Parent_Id,
                            typeid = (obj as Park).Type_id
                        }, commandType: CommandType.StoredProcedure);
                }
                else if (obj is StationTrack)
                {
                    if (((StationTrack)obj).Stw_park_id == -1)
                        return (Int64)db.ExecuteScalar("insertstationtrack",
                            new {
                                tname = (obj as StationTrack).Code,
                                stationid = (obj as StationTrack).Adm_station_id,
                                typeid = (obj as StationTrack).Type_id
                            }, commandType: CommandType.StoredProcedure);
                    else
                        return (Int64)db.ExecuteScalar("insertparktrack",
                            new {
                                tname = (obj as StationTrack).Code,
                                parkid = (obj as StationTrack).Stw_park_id,
                                typeid = (obj as StationTrack).Type_id
                            }, commandType: CommandType.StoredProcedure);
                }
                else if (obj is StationObject)
                {
                    return (Int64) db.ExecuteScalar("insertstationobject",
                        new
                        {
                            oname = (obj as StationObject).Name, parkid = (obj as StationObject).Parent_Id,
                            typeid = (obj as StationObject).Type_id
                        }, commandType: CommandType.StoredProcedure);
                }

                return -1;
            }
        }

        public bool Update(object obj, int unitLevel)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                int result;
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (unitLevel)
                {
                    case AdmStructureConst.AdmRoad:
                        result = db.Execute("UPDATE ADM_ROAD SET CODE=@code, NAME=@name, ABBR=@abbr WHERE ID=@id", new { code = ((AdmUnit)obj).Code, name = ((AdmUnit)obj).Name, id = ((AdmUnit)obj).Id, abbr= ((AdmUnit)obj).Abbr  }, commandType: CommandType.Text);
                        return result != 0;
                    case AdmStructureConst.AdmNod:
                        result = db.Execute("UPDATE ADM_NOD SET CODE=@code, NAME=@name WHERE ID=@id", new { code = ((AdmUnit)obj).Code, name = ((AdmUnit)obj).Name, id = ((AdmUnit)obj).Id }, commandType: CommandType.Text);
                        return result != 0;
                    case AdmStructureConst.AdmDistance:
                        result = db.Execute("UPDATE ADM_DISTANCE SET CODE=@code, NAME=@name WHERE ID=@id", new { code = ((AdmUnit)obj).Code, name = ((AdmUnit)obj).Name, id = ((AdmUnit)obj).Id }, commandType: CommandType.Text);
                        return result != 0;
                    case AdmStructureConst.AdmPchu:
                        result = db.Execute("UPDATE ADM_PCHU SET CODE=@code, CHIEF_FULLNAME=@name WHERE ID=@id", new { code = ((AdmUnit)obj).Code, name = ((AdmUnit)obj).Name, id = ((AdmUnit)obj).Id }, commandType: CommandType.Text);
                        return result != 0;
                    case AdmStructureConst.AdmPd:
                        result = db.Execute("UPDATE ADM_PD SET CODE=@code, CHIEF_FULLNAME=@name WHERE ID=@id", new { code = ((AdmUnit)obj).Code, name = ((AdmUnit)obj).Name, id = ((AdmUnit)obj).Id }, commandType: CommandType.Text);
                        return result != 0;
                    case AdmStructureConst.AdmPdb:
                        result = db.Execute("UPDATE ADM_PDB SET CODE=@code, CHIEF_FULLNAME=@name WHERE ID=@id", new { code = ((AdmUnit)obj).Code, name = ((AdmUnit)obj).Name, id = ((AdmUnit)obj).Id }, commandType: CommandType.Text);
                        return result != 0;
                    case AdmStructureConst.AdmDirection:
                        result = db.Execute("UPDATE ADM_DIRECTION SET CODE=@code, NAME=@name WHERE ID=@id", new { code = ((AdmUnit)obj).Code, name = ((AdmUnit)obj).Name, id = ((AdmUnit)obj).Id }, commandType: CommandType.Text);
                        return result != 0;
                    case AdmStructureConst.AdmTrack:
                        result = db.Execute("UPDATE ADM_TRACK SET CODE=@code WHERE ID=@id", new { code = ((AdmUnit)obj).Code, id = ((AdmUnit)obj).Id }, commandType: CommandType.Text);
                        return result != 0;
                    case AdmStructureConst.AdmStation:
                        result = db.Execute("UPDATE ADM_STATION SET TYPE_ID=@typeId, CODE=@code, NAME=@name WHERE ID=@id", new { typeId = ((StationObject)obj).Type_id, code = ((StationObject)obj).Code, name = ((StationObject)obj).Name, id = ((StationObject)obj).Id }, commandType: CommandType.Text);
                        return result != 0;
                    case AdmStructureConst.AdmPark:
                        result = db.Execute("UPDATE stw_park SET TYPE_ID=@typeId, NAME=@name WHERE ID=@id", new { typeId = ((Park)obj).Type_id, name = ((Park)obj).Name, id = ((Park)obj).Id }, commandType: CommandType.Text);
                        return result != 0;
                    case AdmStructureConst.AdmStationObject:
                        result = db.Execute("UPDATE stw_object SET TYPE_ID=@typeId, NAME=@name WHERE ID=@id", new { typeId = ((StationObject)obj).Type_id, name = ((StationObject)obj).Name, id = ((StationObject)obj).Id }, commandType: CommandType.Text);
                        return result != 0;
                    case AdmStructureConst.AdmStationTrack:
                        result = db.Execute(@"UPDATE stw_track SET TYPE_ID=@typeId where id=@id;
                                                update adm_track set CODE=@code WHERE ID=@trackid;", new { typeId = ((StationTrack)obj).Type_id, code = ((StationTrack)obj).Code, id = ((StationTrack)obj).Id, trackid = ((StationTrack)obj).Adm_track_id }, commandType: CommandType.Text);
                        return result != 0;
                    case AdmStructureConst.AdmParkTrack:
                        result = db.Execute(@"UPDATE stw_park_track SET TYPE_ID=@typeId where id=@id;
                                                update adm_track set CODE=@code WHERE ID=@trackid;", new { typeId = ((StationTrack)obj).Type_id, code = ((StationTrack)obj).Code, id = ((StationTrack)obj).Id, trackid = ((StationTrack)obj).Adm_track_id }, commandType: CommandType.Text);
                        return result != 0;
                    default:
                        return false;
                }
            }
        }

        public object GetUnit(int admLevel, Int64 id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string sql = string.Empty;
                switch (admLevel)
                {
                    case AdmStructureConst.AdmRoad:
                        sql = "Select *  from ADM_ROAD as child ";
                        break;
                    case AdmStructureConst.AdmNod:
                        sql = "Select child.*, parent.id as parent_id from ADM_NOD as child inner join ADM_ROAD as parent on child.adm_road_id = parent.id ";
                        break;
                    case AdmStructureConst.AdmDistance:
                        sql = "Select child.*, parent.id as parent_id from ADM_DISTANCE as child inner join ADM_NOD as parent on child.adm_nod_id = parent.id ";
                        break;
                    case AdmStructureConst.AdmDirection:
                        sql = "Select *  from ADM_DIRECTION as child ";
                        break;
					case AdmStructureConst.AdmTrack:
                        sql = "select child.*, parent.id as parent_id from adm_track as child inner join adm_direction as parent on child.adm_direction_id = parent.id ";
                        break;
                }
                var result= db.Query<AdmUnit>( sql + " where child.id =" + id.ToString(), commandType: CommandType.Text).ToList();
             
                return (result.Count > 0) ? result[0] : null;
            }
        }

        public object GetCurvesAdmUnits(Int64 curveId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<CurvesAdmUnits>(@"select admt.code as Track, (admd.name || ' (' || admd.code || ')') as Direction, coalesce(station.name, 'Неизвестный') as StationStart, coalesce(station2.name, 'Неизвестный') as StationFinal,
                    (stationSection.axis_km * 1000 + stationSection.axis_m - aprc.start_km * 1000 - aprc.start_m) as st1, (stationSection2.axis_km * 1000 + stationSection2.axis_m - aprc.final_km * 1000 - aprc.final_m) as st2
                    from adm_direction as admd 
                    inner join adm_track admt on admt.adm_direction_id = admd.id 
                    inner join tpl_period tplp on tplp.adm_track_id = admt.id 
                    inner join apr_curve aprc on aprc.period_id = tplp.id 
                    inner join tpl_period stationPeriod on stationPeriod.adm_track_id = admt.id 
                    left join tpl_station_section stationSection on stationSection.period_id = stationPeriod.id and (stationSection.axis_km * 1000 + stationSection.axis_m - aprc.start_km * 1000 - aprc.start_m) < 1000
                    left join adm_station station on station.id = stationSection.station_id 
                    left join tpl_station_section stationSection2 on stationSection2.period_id = stationPeriod.id and (stationSection2.axis_km * 1000 + stationSection2.axis_m - aprc.final_km * 1000 - aprc.final_m) > -1000
                    left join adm_station station2 on station2.id = stationSection2.station_id
                    where aprc.id=" + curveId.ToString() + " order by aprc.id, st1, st2 limit 1", commandType: CommandType.Text).ToList();
            }
        }

		public StationTrack GetStationTrack(Int64 id, int admlvl)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            { 
                if (db.State == ConnectionState.Closed)
                    db.Open();
                if (admlvl == AdmStructureConst.AdmStationTrack)
                {
                    List<StationTrack> result = db.Query<StationTrack>(@"Select stwt.*, trackt.NAME as object_type, track.code as Code, track.id as Adm_track_id from stw_track as stwt 
                    INNER JOIN cat_track_type as trackt on trackt.ID = stwt.TYPE_ID
                    inner join adm_track as track on track.id = stwt.adm_track_id
                    where stwt.id=" + id.ToString() + " order by ID", commandType: CommandType.Text).ToList();
                    return (result.Count > 0) ? result[0] : null;
                }
                else if (admlvl == AdmStructureConst.AdmParkTrack)
                {
                    List<StationTrack> result = db.Query<StationTrack>(@"Select stwt.*, trackt.NAME as object_type, track.code as Code, track.id as Adm_track_id from stw_park_track as stwt 
                    INNER JOIN cat_track_type as trackt on trackt.ID = stwt.TYPE_ID
                    inner join adm_track as track on track.id = stwt.adm_track_id
                    where stwt.id=" + id.ToString() + " order by ID", commandType: CommandType.Text).ToList();
                    return (result.Count > 0) ? result[0] : null;
                }
                else
                {
                    return null;
                }
            }
        }
        public List<AdmUnit> GetDistancesRoad(Int64 roadId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<AdmUnit>("Select distance.* from ADM_DISTANCE distance " +
                    "inner join adm_nod nod on nod.id = distance.adm_nod_id " +
                    "inner join adm_road road on road.id = nod.adm_road_id and road.id = " + roadId.ToString(), commandType: CommandType.Text).ToList();
            }
        }
        
        public string GetRoadName(Int64 childId, int childLevel, bool fullName)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string childName = "";
                    switch (childLevel)
                    {
                        case AdmStructureConst.AdmNod:
                            childName = "nod";
                            break;
                        case AdmStructureConst.AdmDistance:
                            childName = "distance";
                            break;
                        case AdmStructureConst.AdmPchu:
                            childName = "pchu";
                            break;
                        case AdmStructureConst.AdmPd:
                            childName = "pd";
                            break;
                        case AdmStructureConst.AdmPdb:
                            childName = "pdb";
                            break;
                    }
                    try
                    {
                        string roadName = db.QueryFirst<string>(@"
                        Select distinct road.abbr from Adm_Road as road
                        Inner join adm_nod as nod on nod.adm_road_id = road.id
                        Inner join adm_distance as distance on distance.adm_nod_id = nod.id
                        left join adm_pchu as pchu on pchu.adm_distance_id = distance.id
                        left join adm_pd as pd on pd.adm_pchu_id = pchu.id
                        left join adm_pdb as pdb on pdb.adm_pd_id = pd.id where " + childName + ".id =" + childId);
                        if (fullName)
                            return roadName;
                        roadName = roadName.ToUpper();
                        var words = roadName.Split('-');
                        roadName = words.Length > 1 ? words[0][0] + "-" + Core.Helper.GetShortForm(words[1]) : Core.Helper.GetShortForm(words[0]);

                        return roadName;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("getRoadName error:" + e.Message);
                        return "";
                    }                   
                }           
        }
        object IAdmStructureRepository.GetTrackName(long track_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    string track_name = db.QueryFirst<string>($@"
                        SELECT
	                        code AS NAME 
                        FROM
	                        adm_track 
                        WHERE
	                        ID = { track_id }");
                    return track_name;
                }
                catch(Exception e)
                {
                    Console.WriteLine("GetTrackName eroor: " + e.Message);
                    return "";
                }
            }
        }

        public string GetDirectionName(string code)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                string name = db.Query<string>("select name from adm_direction where code=@code", new { code }, commandType: CommandType.Text).FirstOrDefault();

                return name;
            }
        }

        public string GetFragmentName(int childId, int childLevel)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string childName = "";
                switch (childLevel)
                {
                    case AdmStructureConst.AdmNod:
                        childName = "nod";
                        break;
                    case AdmStructureConst.AdmDistance:
                        childName = "distance";
                        break;
                    case AdmStructureConst.AdmPchu:
                        childName = "pchu";
                        break;
                    case AdmStructureConst.AdmPd:
                        childName = "pd";
                        break;
                    case AdmStructureConst.AdmPdb:
                        childName = "pdb";
                        break;
                }
                string roadName = db.QueryFirst<string>(@"
                    Select distinct road.name from Adm_Road as road
                    Inner join adm_nod as nod on nod.adm_road_id = road.id
                    Inner join adm_distance as distance on distance.adm_nod_id = nod.id
                    Inner join adm_pchu as pchu on pchu.adm_distance_id = distance.id
                    Inner join adm_pd as pd on pd.adm_pchu_id = pchu.id
                    Inner join adm_pdb as pdb on pdb.adm_pd_id = pd.id where " + childName + ".id =" + childId).ToUpper();
                var words = roadName.Split('-');
                roadName = words.Length > 1 ? words[0][0] + "-" + Core.Helper.GetShortForm(words[1]) : Core.Helper.GetShortForm(words[0]);
                return roadName;
            }
        }

        public List<AdmDirection> GetDirectionsOfRoad(long road_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<AdmDirection>(@"Select ad.* from adm_direction as ad
                    INNER JOIN road_direction as ard on ard.direction_id = ad.id
                     where ard.road_id =" + road_id.ToString() + "  order by ad.ID", commandType: CommandType.Text).ToList();
            }
        }
        public List<AdmRoad> GetDirectionRoads(long directionId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<AdmRoad>(@"select road.* from adm_road road
                    inner join road_direction rdlink on rdlink.road_id = road.id
                    where rdlink.direction_id=@directionId", new { directionId }, commandType: CommandType.Text).ToList();
            }
        }
		public List<AdmDirection> GetStationsDirection(long station_id, long road_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<AdmDirection>($@"select direction.id, direction.code, (direction.name || ' (' || direction.code || ')') as name from adm_direction direction
                    inner join adm_track track on track.adm_direction_id = direction.id
                    inner join tpl_period ssperiod on ssperiod.adm_track_id = track.id and ssperiod.mto_type = 10
                        and is_newest_period(ssperiod.id, ssperiod.mto_type, ssperiod.adm_track_id)
                    inner join tpl_station_section ssection on ssection.period_id = ssperiod.id and ssection.station_id = {station_id}
                    inner join road_direction rd on rd.direction_id = direction.id and rd.road_id = {road_id}
                    group by direction.id", commandType: CommandType.Text).ToList();
            }
        }

        public AdmDirection GetDirectionByTrack(long track_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.QueryFirst<AdmDirection>(@"Select ad.* from adm_direction as ad
                    INNER JOIN adm_track as atr on atr.adm_direction_id = ad.id
                     where atr.id =" + track_id.ToString(), commandType: CommandType.Text);
            }
        }
        public object GetUnitsOfRoad(int admLevel, long road_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                String sqltext = String.Empty;
                switch (admLevel)
                {
                   
                    case AdmStructureConst.AdmStation:
                        
                            sqltext = $@"Select st.* from ADM_STATION as st
                            inner join adm_nod as nod on nod.id = st.adm_nod_id
                            inner join adm_road as road on road.id = nod.adm_road_id
                            where road.id = {road_id} order by st.name";
                            return db.Query<Station>(sqltext, commandType: CommandType.Text).ToList();
                       



                }
                if (sqltext.Equals(String.Empty))
                    return null;
                return db.Query<AdmUnit>(sqltext, commandType: CommandType.Text).ToList();
            }
        }

        
    }
}
