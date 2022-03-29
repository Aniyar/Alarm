using System;
using System.Collections.Generic;
using System.Linq;
using ALARm.Core;
using Npgsql;
using System.Data;
using Dapper;
using ALARm.Core.Report;
using ALARm.Core.AdditionalParameteres;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Net;
using System.Net.Mail;

namespace ALARm.DataAccess
{
    public class RdStructureRepository : IRdStructureRepository
    {
        //public object TotalSleeper(long processId)
        //{
        //    using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
        //    {


        //        if (db.State == ConnectionState.Closed)
        //            db.Open();
        //        var txt =
        //            @"SELECT km,Count(km) as sleper  FROM report_defshpal
        //            WHERE trip_id = 213
        //            group by km";

        //        return db.Query<Digression>(txt).ToList();
        //    }
        //}


        public List<RailFastener> GetRAilSole(long tripId, bool orderBySide, string pch, object trackName)
        {


            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    pch = pch.Replace("ПЧ-", "");
                    pch = pch.Replace("ДТЖ", "64");

                    var txt = $@"SELECT
	                                *,
	                                round( LEAD ( koord, 1 ) OVER ( ORDER BY km ) ) next_koord,
	                                LEAD ( oid, 1 ) OVER ( ORDER BY km ) next_oid,
	                                round( ABS ( LEAD ( koord, 1 ) OVER ( ORDER BY km ) - koord ) ) razn 
                                FROM
	                                (
	                                SELECT DISTINCT
		                                concat (
			                                'ПЧ-',
			                                COALESCE ( distance.code, '' ),
			                                '/ПЧУ-',
			                                COALESCE ( pchu.code, '' ),
			                                '/ПД-',
			                                COALESCE ( pd.code, '' ),
			                                '/ПДБ-',
			                                COALESCE ( pdb.code, '' ) 
		                                ) AS pdbsection,
		                                frag.adm_track_id AS trackid,
		                                trips.ID,
		                                oid,
		                                fnum,
		                                km,
		                                mtr,
		                                rvo.file_id,
		                                ms,
		                                threat_id AS threat,
		                                mtr * 1000.0 + (
			                                ( SELECT travel_direction FROM trips WHERE ID = 213 ) * ( rvo.local_fnum * 200.0 ) - ( SELECT car_position FROM trips WHERE ID = 213 ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
		                                ) AS koord
	                                FROM
		                                PUBLIC.rd_video_objects AS rvo
		                                INNER JOIN trip_files AS files ON files.ID = rvo.file_id
		                                INNER JOIN trips ON trips.ID = files.trip_id
		                                INNER JOIN fragments AS frag ON frag.trip_id = trips.ID 
		                                AND (
			                                isbelong ( km, mtr, frag.final_km, frag.final_m, frag.start_km, frag.start_m ) 
			                                OR isbelong ( km, mtr, frag.start_km, frag.start_m, frag.final_km, frag.final_m ) 
		                                )
		                                INNER JOIN tpl_pdb_section AS SECTION ON km BETWEEN SECTION.start_km 
		                                AND SECTION.final_km
		                                INNER JOIN tpl_period AS period ON period.ID = SECTION.period_id 
		                                AND trips.trip_date BETWEEN period.start_date 
		                                AND period.final_date 
		                                AND period.adm_track_id = frag.adm_track_id
		                                INNER JOIN adm_pdb AS pdb ON pdb.ID = SECTION.adm_pdb_id
		                                INNER JOIN adm_pd AS pd ON pd.ID = pdb.adm_pd_id
		                                INNER JOIN adm_pchu AS pchu ON pchu.ID = pd.adm_pchu_id
		                                INNER JOIN adm_distance AS distance ON distance.ID = pchu.adm_distance_id 
	                                WHERE
		                                oid = {(int)VideoObjectType.OIR} 
		                                AND trips.ID = {tripId} 
		                                AND distance.code = '{pch}' 
                                        and km > 126
	                                ORDER BY
		                                km,
		                                mtr,
	                                koord 
	                                ) AS DATA";

                    var videoObjects = db.Query<VideoObject>(txt).ToList();
                    var result = new List<RailFastener>();
                    foreach (var videoObject in videoObjects)
                    {
                        var serialized = JsonConvert.SerializeObject(videoObject);
                        RailFastener fastener = null;
                        switch ((VideoObjectType)videoObject.Oid)
                        {
                            case VideoObjectType.OIR:
                                fastener = JsonConvert.DeserializeObject<GBR>(serialized);
                                fastener.AddDigression(new Digression() { DigName = DigressionName.Oir });
                                result.Add(fastener);
                                break;
                            default:
                                break;
                        }
                        
                    }
                    return result;
                }
                catch (Exception e)
                {

                    System.Console.WriteLine("GetRAilSoleError:" + e.Message);
                    return null;
                }
            }
        }

        public List<Int64> GetAdmDirectionIDs(Int64 distance_id)
        {
            System.Console.WriteLine(Helper.ConnectionString());
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Int64>("Select admd.id from adm_direction admd " +
                    "inner join adm_track admt on admt.adm_direction_id = admd.id " +
                    "inner join tpl_period tplp on tplp.adm_track_id = admt.id " +
                    "inner join tpl_dist_section tplds on tplds.period_id = tplp.id " +
                    "where tplds.adm_distance_id = " + distance_id.ToString(), commandType: CommandType.Text).ToList();
            }
        }

        public List<Catalog> GetCatalog(int catType)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string catalogTableName = string.Empty;
                switch (catType)
                {
                    case RdStructureConst.ReportCatalog:
                        catalogTableName = "cat_report";
                        break;


                }
                return catalogTableName.Equals(string.Empty) ? new List<Catalog>() : db.Query<Catalog>("Select * from " + catalogTableName, commandType: CommandType.Text).ToList();
            }
        }

        public List<ReportTemplate> GeReportTemplates(int repType)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<ReportTemplate>("Select * from CAT_REPORT_TEMPLATE where rep_type = " + repType.ToString() + " order by name ", commandType: CommandType.Text)
                    .ToList();
            }
        }

        public List<RdClasses> GetRdClasses()
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                String sqltext = "SELECT * FROM classes ORDER BY class_id";
                return db.Query<RdClasses>(sqltext, commandType: CommandType.Text).ToList();
            }
        }
        public object GetS3all(Int64 processId, string pch, long trackId = -1)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                var query = $@"select 
	                                road.code as roadcode, 
	                                direct.code as directcode, 
	                                trips.trip_date as date, 
	                                trips.car as pscode, 
	                                s3.put as nput, 
	                                s3.km, 
	                                s3.meter, 
	                                s3.*
                                from 
	                                s3
                                inner join trips on trips.id = s3.trip_id
                                inner join adm_direction direct on direct.id = trips.direction_id
                                inner join adm_distance dist on dist.code = s3.pch
                                inner join adm_nod nod on nod.id = dist.adm_nod_id
                                inner join adm_road road on road.id = nod.adm_road_id
                                where s3.trip_id = {processId} and s3.pch = '{pch}'  	AND ots <> 'Пси' OR (s3.pch= '{pch}' AND ots ='Пси' AND ovp > -1  ) {(trackId > -1 ? $" AND s3.track_id = {trackId}" : "")}
                
                                order by roadcode, directcode, date, pscode, nput, km, meter";
                return db.Query<S3>(query, commandType: CommandType.Text).ToList();
            }
        }
		public object SumOfTheDep(long processId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                //if (db.State == ConnectionState.Closed)
                //    db.Open();
                //return db.Query<SumOfTheDep>(
                //    @"SELECT * FROM rd_statement
                //        inner join rd_process process on process.id = rd_statement.process_id
                //        where process.id = @process_id ",
                //    new { process_id = processId }, commandType: CommandType.Text).ToList();

                if (db.State == ConnectionState.Closed)
                    db.Open();
                var txt =
                    @"SELECT * FROM rd_statement
                        inner join rd_process process on process.id = rd_statement.process_id
                        where process.id = @process_id";
                return db.Query<SumOfTheDep>(txt).ToList();
            }
        }
        public object GetDBD(Int64 processId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<S3>(
                    @"SELECT
	                    s3.put AS nput,
	                    s3.pd,
	                    s3.pchu,
	                    s3.km,
	                    ( s3.meter / 100+1 ) AS piket,
	                    s3.* 
                    FROM
	                    s3
	                    inner join trips on trips.id = s3.trip_id 
                    WHERE
	                    trips.ID = @process_id 
                    ORDER BY
	                    nput,
	                    pd,
	                    pchu,
	                    km,
	                    piket",
                    new { process_id = processId }, commandType: CommandType.Text).ToList();
            }
        }

        public RdClasses GetRdClasses(int class_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                String sqltext = "SELECT * FROM classes WHERE class_id=" + class_id.ToString();
                return db.Query<RdClasses>(sqltext, commandType: CommandType.Text).Single();
            }
        }

        public List<VideoObject> GetRdObject()
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                String sqltext = "SELECT * FROM rd_video_objects ORDER BY km, oid";
                return db.Query<VideoObject>(sqltext, commandType: CommandType.Text).ToList();
            }
        }
        public List<int> GetKilometerTrip(long Trip_id, long trackId = -1)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<int>($@"select
	                                            DISTINCT kilom.num
                                            from
	                                            kilometers as kilom
                                            where
	                                            trip_id = { Trip_id } {(trackId > -1 ? $" AND track_id = {trackId}" : "")}
                                            order by
	                                            kilom.num ").ToList();
        }
                catch(Exception e)
                {
                    Console.WriteLine("GetKilometerTripError:" + e.Message);
                    return null;
                }
            }
        }

        public static List<OutData> GetNextOutDatasByTripIdAndKms(long trip_id, int startKm, int finalKm)
        {
            try
            {
                using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
                {

                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    return db.Query<OutData>($@"SELECT
	                                                * 
                                                FROM
	                                                outdata_{trip_id} 
                                                WHERE
	                                                ( km BETWEEN {startKm} AND {finalKm} ) 
                                                ORDER BY
                                                ID ").ToList();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error GetNextOutDatasByTripId {e.Message}");
                return null;
            }
        }

        public List<VideoObject> GetRdObject(int object_id, int km, Int64 file_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                String sqltext = "SELECT * FROM rd_video_objects WHERE oid=" + object_id.ToString() + " AND km=" + km.ToString() + " AND fileid=" + file_id.ToString();
                return db.Query<VideoObject>(sqltext, commandType: CommandType.Text).ToList();
            }
        }

        public List<VideoObject> GetRdObject(string object_id, string trip_files)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                String sqltext = "SELECT * FROM rd_video_objects WHERE (" + object_id + ") AND (" + trip_files + ") ORDER BY oid";
                return db.Query<VideoObject>(sqltext, commandType: CommandType.Text).ToList();
            }
        }

       public List<VideoObject> GetVideoObjects(string obj_name, MainParametersProcess process)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<VideoObject>(@"select vidobj.*, classes.description as objname, ('ПЧУ-' || coalesce(pchu.CODE, 'неизвестный') || '/ПД-' || coalesce(pd.CODE, 'неизвестный') || '/ПДБ-' || coalesce(pdb.CODE, 'неизвестный')) as pdbname, coalesce(station.name, 'неизвестный') as stationname, (coalesce(to_char(speed.passenger, '999'), '-') || '/' || coalesce(to_char(speed.freight, 'FM999'), '-')) as speed
                    from rd_video_objects as vidobj
                    inner join classes on classes.class_id = vidobj.oid and position(classes.obj_name in @obj_name) != 0
                    inner join rd_process on rd_process.trip_id = vidobj.trip_id and rd_process.id = @process_id
                    inner join adm_track as track on track.id = @track_id
                    left join tpl_period as stationperiod on stationperiod.adm_track_id = @track_id and stationperiod.mto_type = 10 
                        and stationperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 10)
                    left join tpl_station_section as stationsection on stationsection.period_id = stationperiod.id
                        and abs(stationsection.axis_km * 1000 + stationsection.axis_m - vidobj.km * 1000 - vidobj.pt * 100 - vidobj.mtr) = (select min(abs(tpl_station_section.axis_km * 1000 + tpl_station_section.axis_m - vidobj.km * 1000 - vidobj.pt * 100 - vidobj.mtr)) from tpl_station_section where tpl_station_section.period_id = stationperiod.id)
                    left join adm_station as station on station.id = stationsection.station_id
                    left join tpl_period as pdbperiod on pdbperiod.adm_track_id = track.id and pdbperiod.mto_type = 9 
                        and pdbperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = track.id and tpl_period.mto_type = 9)
                    left join tpl_pdb_section as pdbsection on pdbsection.period_id = pdbperiod.id 
                        and ((vidobj.km * 1000 + vidobj.pt * 100 + vidobj.mtr) between (pdbsection.start_km * 1000 + pdbsection.start_m) and (pdbsection.final_km * 1000 + pdbsection.final_m))
                    left join adm_pdb as pdb on pdb.id = pdbsection.adm_pdb_id
                    left join adm_pd as pd on pd.id = pdb.adm_pd_id
                    left join adm_pchu as pchu on pchu.id = pd.adm_pchu_id
                    left join tpl_period as speedperiod on speedperiod.adm_track_id = @track_id and speedperiod.mto_type = 6
                        and speedperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 6)
					left join apr_speed as speed on speed.period_id = speedperiod.id
						and ((vidobj.km * 1000 + vidobj.pt * 100 + vidobj.mtr) between (speed.start_km * 1000 + speed.start_m) and (speed.final_km * 1000 + speed.final_m))
                    where vidobj.track_id = @track_id", 
                    new { obj_name, process_id = process.Id, track_id = process.TrackID}, commandType: CommandType.Text).ToList();
            }
        }

        public List<VideoObjectCount> GetRdObjectCount(Int64 trip_files_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                String sqltext = "SELECT km, count(oid) FROM rd_video_objects WHERE fileid=" + trip_files_id.ToString() + " GROUP BY km ORDER BY km";
                return db.Query<VideoObjectCount>(sqltext, commandType: CommandType.Text).ToList();
            }
        }

        public List<VideoObjectCount> GetRdObjectCount(List<Int64> fileIDs)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                string idtext = "(";
                foreach (Int64 fileID in fileIDs)
                    idtext += fileID.ToString() + ", ";
                idtext = idtext.TrimEnd(new char[] { ',', ' ' });
                idtext += ")";
                if (db.State == ConnectionState.Closed)
                    db.Open();
                String sqltext = "SELECT km, count(oid) FROM rd_video_objects WHERE fileid in " + idtext + " GROUP BY km ORDER BY km";
                return db.Query<VideoObjectCount>(sqltext, commandType: CommandType.Text).ToList();
            }
        }

        public List<VideoObject> GetRdObjectKm(string km, string trip_files)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string sqltext = "SELECT * FROM rd_video_objects WHERE (" + km + ") AND (" + trip_files + ") ORDER BY km";
                return db.Query<VideoObject>(sqltext, commandType: CommandType.Text).ToList();
            }
        }

        public List<Kilometer> GetKilometersByTrip(Trips trip)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string sqltext = @"
                select distinct direction.id as direction_id,
                direction.code AS direction_code,
                concat(direction.name, '(', direction.code, ')') as direction_name, 
                track.id as track_id,track.code as track_name, kilom.*, kilom.num as number from kilometers as kilom
                LEFT join rd_process as rdp on rdp.trip_id = kilom.trip_id
                inner join adm_track as track on track.id = kilom.track_id
                inner join adm_direction as direction on direction.id = track.adm_direction_id
                where kilom.trip_id = " + trip.Id + " and num > 0 order by direction.id, track.id, kilom.id";
               
                var result = db.Query<Kilometer>(sqltext).ToList();
                foreach(var km in result)
                {
                    km.Trip = trip;
                }
                return result;
            }
        }


        public List<VideoObject> GetRdObjectKm(string object_id, string km, string trip_files)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                String sqltext = "SELECT * FROM rd_video_objects WHERE (" + object_id + ") AND (" + km + ") AND (" + trip_files + ") ORDER BY km";
                return db.Query<VideoObject>(sqltext, commandType: CommandType.Text).ToList();
            }
        }

        public object GetTrips(Int64 process_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Trips>(@"select trips.*, (direction.name || ' (' || direction.code || ')') as direction from trips
                    inner join adm_direction direction on direction.id = trips.direction_id
                    inner join rd_process process on process.trip_id = trips.id and process.id = " + process_id.ToString(), commandType: CommandType.Text).ToList();
            }
        }
        public List<Trips> GetTripsByRoad(Int64 road_id, ReportPeriod period, int trip_type)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                String sqltext = @"Select direction.name as direction_name,  s_station.name as start_station_name, f_station.name as final_station_name, trip.*  from trips as trip
                        inner join adm_direction as direction on direction.id = trip.direction_id
                        inner join road_direction as road on road.direction_id = direction.id
                        left join adm_station as s_station on s_station.id = trip.start_station
                        left join adm_station as f_station on f_station.id = trip.final_station
                        where road.road_id = @road_id and trip.trip_date between @start and @final order by trip.trip_date desc";
                var trips = db.Query<Trips>(sqltext, new { road_id = road_id, start = period.StartDate, final = period.FinishDate }, commandType: CommandType.Text).ToList();
                for (int i = 0; i< trips.Count; i++)
                {
                    var fr = GetTripFragments(trips[i].Id);
                        if (fr.Count>0)
                    {
                        trips[i].TrackCode = fr[0].Track_Code;
                    }
                }
                return trips;
            }
        }
        public object GetTrips(List<Int64> directionIDs, int rd_lvl)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                if (rd_lvl == 0)
                {
                    String sqltext = "SELECT trips.*, false as Checked_Status FROM trips WHERE direction_id in (";
                    foreach (Int64 i in directionIDs)
                    {
                        sqltext += i.ToString() + ", ";
                    }
                    sqltext = sqltext.Remove(sqltext.Length - 2);
                    sqltext += ")";
                    return db.Query<Trips>(sqltext, commandType: CommandType.Text).ToList();
                }
                else
                {
                    String sqltext = "SELECT trip_files.*, false as Checked_Status FROM trip_files "
                        + "inner join trips on trips.id = trip_files.trip_id "
                        + "WHERE direction_id in (";
                    foreach (Int64 i in directionIDs)
                    {
                        sqltext += i.ToString() + ", ";
                    }
                    sqltext = sqltext.Remove(sqltext.Length - 2);
                    sqltext += ")";
                    return db.Query<TripFiles>(sqltext, commandType: CommandType.Text).ToList();
                }
            }
        }

        public object GetAdmDirection(List<Int64> directionIDs)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                if (directionIDs.Count > 0)
                {
                    String sqltext = "SELECT * FROM adm_direction WHERE id in (";
                    foreach (Int64 i in directionIDs)
                    {
                        sqltext += i.ToString() + ", ";
                    }
                    sqltext = sqltext.Remove(sqltext.Length - 2);
                    sqltext += ") order by id";
                    return db.Query<AdmDirection>(sqltext, commandType: CommandType.Text).ToList();
                }
                else
                    return null;
            }
        }

        public List<MainParametersProcess> GetMainParametersProcess()
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<MainParametersProcess>("select * from rd_process order by id", commandType: CommandType.Text).ToList();
            }
        }

        public MainParametersProcess GetMainParametersProcess(Int64 process_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<MainParametersProcess>($@"select distinct rdp.*, trip.car_position as carposition, direction.name as directionname, direction.code as directioncode, trip.travel_direction as direction, coalesce(trip.chief, 'неизвестный') as chief,  coalesce(trip.car, 'неизвестный') as car, coalesce(to_char(trip.trip_date, 'DD.MM.YYYY'), 'неизвестный') as trip_date
                    from rd_process as rdp
                    inner join trips as trip on trip.id = rdp.trip_id
                    inner join adm_direction as direction on direction.id = trip.direction_id
                    where rdp.id = {process_id}", commandType: CommandType.Text).ToList()[0];
            }
        }

        public List<ReportPeriod> GetReportPeriods(long distanceId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<ReportPeriod>(
                     $@"Select distinct date_part('year', trip.trip_date) as periodYear, date_part('month', trip.trip_date) as periodMonth  
                    from trips as trip
                    inner join adm_direction direction on direction.id = trip.direction_id
                    inner join adm_track track on track.adm_direction_id = direction.id
                    inner join tpl_period dperiod on dperiod.adm_track_id = track.id
                    inner join tpl_dist_section dsection on dsection.period_id = dperiod.id
                    where dsection.adm_distance_id = {distanceId}
                    order by periodYear, periodMonth DESC",
                    commandType: CommandType.Text).ToList();
            }
        }
        public List<ReportPeriod> GetTripPeriodsByRoad(long road_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<ReportPeriod>(
                     @"Select distinct date_part('year', trip.trip_date) as periodYear, date_part('month', trip.trip_date) as periodMonth  from trips as trip
                        inner join adm_direction as direction on direction.id = trip.direction_id
                        inner join adm_track as track on track.adm_direction_id = direction.id
                        inner join tpl_period as dist_period on dist_period.adm_track_id = track.id
                        inner join tpl_dist_section as dsection on dsection.period_id = dist_period.id
                        inner join adm_distance as distance on distance.id = dsection.adm_distance_id
                        inner join adm_nod as nod on nod.id = distance.adm_nod_id
                        inner join adm_road as road on road.id = nod.adm_road_id
                        inner join adm_station as s_station on s_station.id = trip.start_station
                        inner join adm_station as f_station on f_station.id = trip.final_station
                        where road.id = " + road_id + " order by  periodYear, periodMonth DESC",
                    commandType: CommandType.Text).ToList();
            }
        }
        public List<MainParametersProcess> GetMainParametersProcesses(ReportPeriod period, Int64 distance_id, bool all = false)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                
                return db.Query<MainParametersProcess>(
                    @"
                SELECT DISTINCT
	                trip.ID AS trip_id,
	                COALESCE ( trip.chief, 'неизвестный' ) AS chief,
	                COALESCE ( trip.car, 'неизвестный' ) AS car,
	                COALESCE ( to_char( trip.trip_date, 'DD.MM.YYYY' ), 'неизвестный' ) AS trip_date,
                    trip.trip_date date_vrem,
	                trip.*,
                    adm_direction.NAME ||'(' ||adm_direction.code ||')'  DirectionName
                FROM
	                trips AS trip
	                INNER JOIN fragments AS frag ON frag.trip_id = trip.
	                ID INNER JOIN tpl_period AS period ON period.adm_track_id = frag.adm_track_id 
	                AND trip.trip_date BETWEEN period.start_date 
	                AND period.final_date
	                INNER JOIN tpl_dist_section AS distance ON distance.period_id = period.ID 
	                AND (
		                (
			                coordinatetoreal ( frag.start_km, frag.start_m ) < coordinatetoreal ( frag.final_km, frag.final_m ) 
			                AND numrange ( frag.start_km, frag.final_km ) && numrange ( distance.start_km, distance.final_km ) 
		                ) 
		                OR (
			                coordinatetoreal ( frag.start_km, frag.start_m ) > coordinatetoreal ( frag.final_km, frag.final_m ) 
			                AND numrange ( frag.final_km, frag.start_km ) && numrange ( distance.start_km, distance.final_km ) 
		                ) 
	                )
	                INNER JOIN adm_distance ON adm_distance.ID = distance.adm_distance_id
                    INNER JOIN adm_direction ON adm_direction.ID = trip.direction_id
                    where trip.trip_date between @startDate and @finishDate and distance.adm_distance_id = @distance order by trip.id desc  " + (all ? "" : "limit 1"),
                    new { distance = distance_id, startDate = period.StartDate, finishDate = period.FinishDate }, commandType: CommandType.Text).ToList();
            }
        }
        
        public List<MainParametersProcess> GetMainParametersProcess(ReportPeriod period, string distanceName)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                distanceName = distanceName.Replace("ПЧ-", "");
                distanceName = distanceName.Replace("ДТЖ", "64");
                return db.Query<MainParametersProcess>(
                    @"SELECT DISTINCT
	                    dir.ID AS DirectionID,
	                    dir.code AS DirectionCode,
	                    dir.NAME AS directionname,
                        --dir.NAME || '(' || dir.code || ')' AS directionname,
	                    trip.travel_direction AS direction,
	                    trip.ID AS trip_id,
	                    COALESCE ( trip.chief, 'неизвестный' ) AS chief,
	                    COALESCE ( trip.car, 'неизвестный' ) AS car,
	                    COALESCE ( to_char( trip.trip_date, 'dd.MM.yyyy hh:mm:ss' ), 'неизвестный' ) AS trip_date,
	                    trip.trip_date AS date_vrem,
            	        trip.car_position as carposition,
	                    trip.* 
                    FROM
	                    trips AS trip
	                    INNER JOIN kilometers AS km ON km.trip_id = trip.
	                    ID INNER JOIN tpl_period AS period ON period.adm_track_id = km.track_id 
	                    AND trip.trip_date BETWEEN period.start_date 
	                    AND period.final_date
	                    INNER JOIN tpl_dist_section AS distance ON distance.period_id = period.
	                    ID INNER JOIN adm_distance ON adm_distance.ID = distance.adm_distance_id
	                    INNER JOIN adm_direction AS dir ON dir.ID = trip.direction_id

                    where trip.trip_date between @startDate and @finishDate and adm_distance.code = @distName",
                    new { distName = distanceName, startDate = period.StartDate, finishDate = period.FinishDate }, commandType: CommandType.Text).ToList();
            }
        }

        public List<MainParametersProcess> GetAdditionalParametersProcess(ReportPeriod period)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<MainParametersProcess>(
                    @"select distinct rdp.*, dir.id as DirectionID,dir.code as DirectionCode,
                    trip.car_position as carposition, dir.name as directionname, trip.travel_direction as direction, 
                    coalesce(trip.chief, 'неизвестный') as chief,  coalesce(trip.car, 'неизвестный') as car, 
                    coalesce(to_char(trip.trip_date, 'DD.MM.YYYY'), 'неизвестный') as trip_date 
                    from rd_process as rdp
                    LEFT JOIN trips as trip on trip.id = rdp.trip_id
                    INNER join adm_direction as dir on dir.id = trip.direction_id
                    where rdp.date_vrem between @startDate and @finishDate and (rdp.process_type = 100 or rdp.process_type = 0)",
                    new {  startDate = period.StartDate, finishDate = period.FinishDate }, commandType: CommandType.Text).ToList();
            }
        }

        public List<Digression> GetDigressions(Int64 processId, string distanceName, int[] typ)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed) 
                    db.Open();
                distanceName = distanceName == "ДТЖ" ? "64" : distanceName;
                return db.Query<Digression>(
                    @"Select naprav as direction, put as track, pchu, pd, pdb, 
                                                trip_date as FoundDate, 
                                                km, meter, ots as name, ots as digression, otkl as value, len as length, kol as count, typ from s3 
                    INNER JOIN trips as trips on trips.id = s3.trip_id
                    where is2to3 and s3.trip_id = @process_id and s3.pch = @distance_Name and s3.typ in ( " + string.Join(",", typ) + ") " + @"
                    order by direction, track, pchu, pd, pdb, km, meter",
                    new { process_id = processId, distance_Name = distanceName }, commandType: CommandType.Text).ToList();
            }
        }
        public List<Digression> PoorKilometers(Int64 processId, string distanceName, int[] typ)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Digression>(
                    @"Select bedemost.pch,  bedemost.naprav as direction, bedemost.put as track, bedemost.km as kmetr, s3.meter as mtr, s3.ovp, s3.ovp,
                    CONCAT(CAST (uv as text) ,'/' , CAST (uvg  as text ), '/', CAST (uvg  as text )) as allowSpeed, 
                    CONCAT(CAST (ovp as text) ,'/' , CAST (ogp  as text ), '/', CAST (ogp  as text )) as FullSpeed, 
                    lkm, bedemost.primech,
                    s3.otkl, s3.len, s3.ots, s3.ots as digression
                    from bedemost 
                    INNER JOIN trips trip on trip.id = bedemost.trip_id
                    INNER join s3 on s3.trip_id = trip.id
                    where ball > 180 
                    and bedemost.kmtrue = s3.km and bedemost.trip_id = @process_id and bedemost.pch =  @distance_Name
                    and (s3.ovp <> -1 or s3.ogp <> -1)  
                    order by direction, track, kmetr, mtr",
                    new { process_id = processId, distance_Name = distanceName }, commandType: CommandType.Text).ToList();
            }
        }
        public List<Digression> GetDigressions3and4(Int64 processId, string distanceName, int[] typ)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                distanceName = distanceName == "ДТЖ" ? "64" : distanceName;
                return db.Query<Digression>(
                    @"Select distinct  naprav as direction, put as track, pchu, pd, pdb, trip_date as FoundDate, km, meter, ots as name, ots as digression, otkl as value, len as length, kol as count, typ, coalesce(norma.norma_width,1520) as norma,
                    CONCAT(CAST (coalesce( speed.passenger,-1) as text) ,'/' , CAST (coalesce(speed.freight,-1)  as text ), '/', CAST (coalesce(empty_freight,-1)  as text) ) as fullSpeed, 
                    CONCAT(CAST (s3.ovp as text) ,'/' , CAST (s3.ogp  as text ), '/', CAST (s3.ogp  as text) ) as allowSpeed, typ, s3.pch, primech 
                    from s3 
                    INNER JOIN trips as trips on trips.id = s3.trip_id
                    --INNER JOIN adm_distance as distance on distance.name = s3.pch
                    INNER JOIN adm_track as track on track.code = s3.put
                    INNER JOIN adm_direction as direction on direction.name = s3.naprav and direction.id = track.adm_direction_id

                    Left JOIN tpl_period as speed_period on speed_period.adm_track_id = track.id and trip_date between speed_period.start_date and speed_period.final_Date and speed_period.mto_type = 6
                    Left JOIN apr_speed as speed on speed.period_id = speed_period.id and isbelong(s3.km,s3.meter, speed.start_km, speed.start_m, speed.final_km, speed.final_m)
                    Left JOIN tpl_period as norma_period on norma_period.adm_track_id = track.id and trip_date between norma_period.start_date and norma_period.final_Date and norma_period.mto_type = 5
                    Left JOIN apr_norma_width as norma on norma.period_id = norma_period.id and isbelong(s3.km,s3.meter, norma.start_km, norma.start_m, norma.final_km, norma.final_m)
                    where s3.trip_id = @process_id and s3.pch = @distance_Name and s3.typ in ( " + string.Join(",", typ) + ") " + @"
                    order by direction, track, pchu, pd, pdb, km, meter",
                    new { process_id = processId, distance_Name = distanceName }, commandType: CommandType.Text).ToList();
            }
        }
        public List<Digression> GetAti(long Trip_id, int Km)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                var elem_list = new List<Digression> { };

                var left = db.Query<Digression>(
                    $@"SELECT
	                        * 
                        FROM
	                        (
	                        SELECT
		                        koord,
		                        km,
		                        mtr AS meter, oid, description,
		                        ABS ( koord - LEAD ( koord, 1 ) OVER ( ORDER BY koord ) ) razn, 
                                threat_id threat
	                        FROM
		                        (
		                        SELECT
			                        round(
				                        mtr * 1000.0 + (
					                        ( SELECT travel_direction FROM trips WHERE ID = {Trip_id} ) * ( local_fnum * 200.0 ) - 
                                                ( SELECT car_position FROM trips WHERE ID = {Trip_id} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
				                        ) 
			                        ) AS koord,
			                        * 
		                        FROM
			                        RD_VIDEO_OBJECTS AS rvo
			                        INNER JOIN trip_files AS tfile ON tfile.ID = rvo.file_Id 
		                        WHERE
			                        oid = 30
			                        AND tfile.trip_id = {Trip_id} 
			                        AND km = {Km} 
			                        AND threat_id = 1 
		                        ) DATA1 
	                        ) DATA2 
                        WHERE
	                        razn > 500").ToList();

                int cn = 1; 
                for(int i=0; i<=left.Count()-2; i++)
                {
                    if(left[i].Km == left[i+1].Km && Math.Abs(left[i].Meter - left[i + 1].Meter) < 11)
                    {
                        cn++;
                    }
                    else
                    {
                        if(cn > 10)
                        {
                            left[i].Cn = cn;
                            elem_list.Add(left[i]);
                            cn = 1;
                        }
                    }
                    if(i == left.Count() - 2)
                    {
                        if (cn > 10)
                        {
                            left[i].Cn = cn;
                            elem_list.Add(left[i]);
                            cn = 1;
                        }
                    }
                }
                var right = db.Query<Digression>(
                    $@"SELECT
	                        * 
                        FROM
	                        (
	                        SELECT
		                        koord,
		                        km,
		                        mtr AS meter, oid, description,
		                        ABS ( koord - LEAD ( koord, 1 ) OVER ( ORDER BY koord ) ) razn, threat_id threat
	                        FROM
		                        (
		                        SELECT
			                        round(
				                        mtr * 1000.0 + (
					                        ( SELECT travel_direction FROM trips WHERE ID = {Trip_id} ) * ( local_fnum * 200.0 ) - 
                                                ( SELECT car_position FROM trips WHERE ID = {Trip_id} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
				                        ) 
			                        ) AS koord,
			                        * 
		                        FROM
			                        RD_VIDEO_OBJECTS AS rvo
			                        INNER JOIN trip_files AS tfile ON tfile.ID = rvo.file_Id 
		                        WHERE
			                        oid = 30 
			                        AND tfile.trip_id = {Trip_id} 
			                        AND km = {Km} 
			                        AND threat_id = 2 
		                        ) DATA1 
	                        ) DATA2 
                        WHERE
	                        razn > 500").ToList();
                cn = 1;
                for (int i = 0; i <= right.Count() - 2; i++)
                {
                    if (right[i].Km == right[i + 1].Km && Math.Abs(right[i].Meter - right[i + 1].Meter) < 11)
                    {
                        cn++;
                    }
                    else
                    {
                        if (cn > 10)
                        {
                            right[i].Cn = cn;
                            elem_list.Add(right[i]);
                            cn = 1;
                        }
                    }
                    if (i == right.Count() - 2)
                    {
                        if (cn > 10)
                        {
                            right[i].Cn = cn;
                            elem_list.Add(right[i]);
                            cn = 1;
                        }
                    }
                }

                return elem_list;

            }
        }
        public List<Digression> GetDevRail(long trip_id, string dist, string track)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Digression>(
                    $@" SELECT DISTINCT
	                        km,
	                        mtr AS meter,
	                        threat_id threat,
	                        oid,
                        CASE
		                    WHEN oid IN ( 5, 6, 7, 8, 9, 10 ) THEN
		                    'Стык' 
		                    WHEN oid IN ( 18, 19, 20,25,26,27,28,29,32,33,36,37 ) THEN
		                    'ЖБ' 
		                    WHEN oid IN ( 15, 16, 17,34,35 ) THEN
		                    'Дерев' 
		                    ELSE'не определено' 
		                END as name
		
	                    FROM
		                    rd_video_objects rvo
		                    INNER JOIN trip_files tf ON tf.ID = rvo.file_id 
		                    WHERE
		                    oid IN ( 5, 6, 7, 8, 9, 10, 18, 19, 20,25,26,27,28,29,32,33,36,37, 15, 16, 17,34,35 ) AND
		                    trip_id = {trip_id} 
	                    ORDER BY
		                    km,
		                    mtr ").ToList();
            }
        }

        public List<Digression> GetDigressions(MainParametersProcess process, int[] typ)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var txtx = $@"select s3.naprav as direction, s3.put as track, s3.pchu, s3.pd, s3.pdb, s3.km, s3.meter, s3.primech, s3.ots as name, s3.ots as digression, s3.otkl as value, s3.len as length, s3.kol as count, s3.typ, coalesce(station.Name, 'неизвестный') as stationname, coalesce(bracetype.name, 'неизвестный') as bracetype, coalesce(threadside.name, 'Правая') as threadside,
                    (coalesce(to_char(s3.uv, '999'), '-') || '/' || coalesce(to_char(s3.uvg, 'FM999'), '-')) as fullspeed, (coalesce(to_char(s3.ovp, '999'), '-') || '/' || coalesce(to_char(s3.ogp, 'FM999'), '-')) as allowspeed, coalesce(stcurve.radius, 0) as curveradius, coalesce(to_char(stcurve.width, '9999'), to_char(norma.norma_width, '9999'), '') as norma,
                    coalesce(railtype.name, 'неизвестный') as railtype, coalesce(trackclassname.name, 'Первый') as trackclass
                    from s3
                    left join tpl_period as stationperiod on stationperiod.adm_track_id = @track_id and stationperiod.mto_type = 10 
                        and stationperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 10)
                    left join tpl_station_section as stationsection on stationsection.period_id = stationperiod.id
                        and abs(stationsection.axis_km * 1000 + stationsection.axis_m - s3.km * 1000 - s3.meter) = (select min(abs(tpl_station_section.axis_km * 1000 + tpl_station_section.axis_m - s3.km * 1000 - s3.meter)) from tpl_station_section where tpl_station_section.period_id = stationperiod.id)
                    left join adm_station as station on station.id = stationsection.station_id
                    left join tpl_period as braceperiod on braceperiod.adm_track_id = @track_id and braceperiod.mto_type = 4
                        and braceperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 4)
					left join apr_rails_braces as brace on brace.period_id = braceperiod.id
						and ((s3.km * 1000 + s3.meter) between (brace.start_km * 1000 + brace.start_m) and (brace.final_km * 1000 + brace.final_m))
                    left join cat_brace_type as bracetype on bracetype.id = brace.brace_type_id
                    left join tpl_period as curveperiod on curveperiod.adm_track_id = @track_id and curveperiod.mto_type = 11
                        and curveperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 11)
                    left join apr_curve as curve on curve.period_id = curveperiod.id
                        and ((s3.km * 1000 + s3.meter) between (curve.start_km * 1000 + curve.start_m) and (curve.final_km * 1000 + curve.final_m))
                    left join apr_stcurve as stcurve on stcurve.curve_id = curve.id
                        and ((s3.km * 1000 + s3.meter) between (stcurve.start_km * 1000 + stcurve.start_m) and (stcurve.final_km * 1000 + stcurve.final_m))
                    left join tpl_period as normaperiod on normaperiod.adm_track_id = @track_id and normaperiod.mto_type = 5
                        and normaperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 5)
                    left join apr_norma_width as norma on norma.period_id = normaperiod.id
                        and ((s3.km * 1000 + s3.meter) between (norma.start_km * 1000 + norma.start_m) and (norma.final_km * 1000 + norma.final_m))
                    left join tpl_period as threadperiod on threadperiod.adm_track_id = @track_id and threadperiod.mto_type = 13
                        and threadperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 13)
                    left join apr_straightening_thread as thread on thread.period_id = threadperiod.id
                        and ((s3.km * 1000 + s3.meter) between (thread.start_km * 1000 + thread.start_m) and (thread.final_km * 1000 + thread.final_m))
                    left join cat_side as threadside on threadside.id = thread.side_id
                    left join tpl_period as railsperiod on railsperiod.adm_track_id = @track_id and railsperiod.mto_type = 17
                        and railsperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 17)
					left join apr_rails_sections as railsection on railsection.period_id = railsperiod.id
						and ((s3.km * 1000 + s3.meter) between (railsection.start_km * 1000 + railsection.start_m) and (railsection.final_km * 1000 + railsection.final_m))
                    left join cat_rails_type as railtype on railtype.id = railsection.type_id
                    left join tpl_period as classperiod on classperiod.adm_track_id = @track_id and classperiod.mto_type = 3
                        and classperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 3)
					left join apr_trackclass as trackclass on trackclass.period_id = classperiod.id
						and ((s3.km * 1000 + s3.meter) between (trackclass.start_km * 1000 + trackclass.start_m) and (trackclass.final_km * 1000 + trackclass.final_m))
                    left join cat_trackclass as trackclassname on trackclassname.id = trackclass.class_id
                    where  s3.pch = @distance and s3.put = @track and s3.typ in (" + string.Join(",", typ) + ")";
                    return db.Query<Digression>(txtx,new {distance = process.DistanceName, track = process.TrackName, track_id = process.TrackID}, commandType: CommandType.Text).ToList();
                   
                }
                
                catch (Exception e)
                {
                    Console.WriteLine("GetShpalError:" + e.Message);
                    return null;
                }
                //           return db.Query<Digression>(

                //               @"select s3.naprav as direction, s3.put as track, s3.pchu, s3.pd, s3.pdb, s3.km, s3.meter, s3.primech, s3.ots as name, s3.ots as digression, s3.otkl as value, s3.len as length, s3.kol as count, s3.typ, coalesce(station.Name, 'неизвестный') as stationname, coalesce(bracetype.name, 'неизвестный') as bracetype, coalesce(threadside.name, 'Правая') as threadside,
                //               (coalesce(to_char(s3.uv, '999'), '-') || '/' || coalesce(to_char(s3.uvg, 'FM999'), '-')) as fullspeed, (coalesce(to_char(s3.ovp, '999'), '-') || '/' || coalesce(to_char(s3.ogp, 'FM999'), '-')) as allowspeed, coalesce(stcurve.radius, 0) as curveradius, coalesce(to_char(stcurve.width, '9999'), to_char(norma.norma_width, '9999'), '') as norma,
                //               coalesce(railtype.name, 'неизвестный') as railtype, coalesce(trackclassname.name, 'Первый') as trackclass
                //               from s3
                //               left join tpl_period as stationperiod on stationperiod.adm_track_id = @track_id and stationperiod.mto_type = 10 
                //                   and stationperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 10)
                //               left join tpl_station_section as stationsection on stationsection.period_id = stationperiod.id
                //                   and abs(stationsection.axis_km * 1000 + stationsection.axis_m - s3.km * 1000 - s3.meter) = (select min(abs(tpl_station_section.axis_km * 1000 + tpl_station_section.axis_m - s3.km * 1000 - s3.meter)) from tpl_station_section where tpl_station_section.period_id = stationperiod.id)
                //               left join adm_station as station on station.id = stationsection.station_id
                //               left join tpl_period as braceperiod on braceperiod.adm_track_id = @track_id and braceperiod.mto_type = 4
                //                   and braceperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 4)
                //left join apr_rails_braces as brace on brace.period_id = braceperiod.id
                //	and ((s3.km * 1000 + s3.meter) between (brace.start_km * 1000 + brace.start_m) and (brace.final_km * 1000 + brace.final_m))
                //               left join cat_brace_type as bracetype on bracetype.id = brace.brace_type_id
                //               left join tpl_period as curveperiod on curveperiod.adm_track_id = @track_id and curveperiod.mto_type = 11
                //                   and curveperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 11)
                //               left join apr_curve as curve on curve.period_id = curveperiod.id
                //                   and ((s3.km * 1000 + s3.meter) between (curve.start_km * 1000 + curve.start_m) and (curve.final_km * 1000 + curve.final_m))
                //               left join apr_stcurve as stcurve on stcurve.curve_id = curve.id
                //                   and ((s3.km * 1000 + s3.meter) between (stcurve.start_km * 1000 + stcurve.start_m) and (stcurve.final_km * 1000 + stcurve.final_m))
                //               left join tpl_period as normaperiod on normaperiod.adm_track_id = @track_id and normaperiod.mto_type = 5
                //                   and normaperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 5)
                //               left join apr_norma_width as norma on norma.period_id = normaperiod.id
                //                   and ((s3.km * 1000 + s3.meter) between (norma.start_km * 1000 + norma.start_m) and (norma.final_km * 1000 + norma.final_m))
                //               left join tpl_period as threadperiod on threadperiod.adm_track_id = @track_id and threadperiod.mto_type = 13
                //                   and threadperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 13)
                //               left join apr_straightening_thread as thread on thread.period_id = threadperiod.id
                //                   and ((s3.km * 1000 + s3.meter) between (thread.start_km * 1000 + thread.start_m) and (thread.final_km * 1000 + thread.final_m))
                //               left join cat_side as threadside on threadside.id = thread.side_id
                //               left join tpl_period as railsperiod on railsperiod.adm_track_id = @track_id and railsperiod.mto_type = 17
                //                   and railsperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 17)
                //left join apr_rails_sections as railsection on railsection.period_id = railsperiod.id
                //	and ((s3.km * 1000 + s3.meter) between (railsection.start_km * 1000 + railsection.start_m) and (railsection.final_km * 1000 + railsection.final_m))
                //               left join cat_rails_type as railtype on railtype.id = railsection.type_id
                //               left join tpl_period as classperiod on classperiod.adm_track_id = @track_id and classperiod.mto_type = 3
                //                   and classperiod.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = @track_id and tpl_period.mto_type = 3)
                //left join apr_trackclass as trackclass on trackclass.period_id = classperiod.id
                //	and ((s3.km * 1000 + s3.meter) between (trackclass.start_km * 1000 + trackclass.start_m) and (trackclass.final_km * 1000 + trackclass.final_m))
                //               left join cat_trackclass as trackclassname on trackclassname.id = trackclass.class_id
                //               where  s3.pch = @distance and s3.put = @track and s3.typ in (" + string.Join(",", typ) + ")",
                //               new { distance = process.DistanceName, track = process.TrackName, track_id = process.TrackID }, commandType: CommandType.Text).ToList();
            }
        }
        public List<Digression> GetViolPerpen(int Trip_id, int[] typ, int km)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<Digression>($@"
                                                SELECT
	                                                * 
                                                FROM
	                                                (
	                                                SELECT
		                                                km_l km,
		                                                meter_l meter,
		                                                koord_l,
		                                                koord_r ,
		                                                ABS ( atan( ( koord_l - koord_r ) / 1500.0 ) * ( 180 / 3.14 ) ) AS angle,
		                                                fnum,
		                                                ms,
		                                                file_id 
	                                                FROM
		                                                (
		                                                SELECT
			                                                * 
		                                                FROM
			                                                (
			                                                SELECT
				                                                * ,
				                                                km km_l,
				                                                meter meter_l,
				                                                koord koord_l 
			                                                FROM
				                                                (
				                                                SELECT
					                                                *,
					                                                round( ABS ( LEAD ( koord, 1 ) OVER ( ORDER BY km ) - koord ) ) razn 
				                                                FROM
					                                                (
					                                                SELECT
						                                                km,
						                                                mtr AS meter,
						                                                mtr * 1000.0 + (
							                                                ( SELECT travel_direction FROM trips WHERE ID = {Trip_id}  ) * ( local_fnum * 200.0 ) - ( SELECT car_position FROM trips WHERE ID = {Trip_id}  ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
						                                                ) AS koord,
						                                                oid,
						                                                threat_id,
						                                                fnum,
						                                                ms,
						                                                file_id 
					                                                FROM
						                                                RD_VIDEO_OBJECTS AS rvo
						                                                INNER JOIN trip_files AS tfile ON tfile.ID = rvo.file_Id 
					                                                WHERE
						                                                oid IN (
							                                                15,
							                                                16,
							                                                17,
							                                                18,
							                                                19,
							                                                20,
							                                                25,
							                                                26,
							                                                27,
							                                                28,
							                                                29,
							                                                31,
							                                                32,
							                                                33,
							                                                34,
							                                                35,
							                                                36,
							                                                37,
							                                                39 
						                                                ) 
						                                                AND tfile.trip_id = {Trip_id}  
						                                                AND km = {km} 
						                                                AND threat_id = 1 
					                                                ORDER BY
						                                                koord 
					                                                ) data1 
				                                                ) data2 
			                                                WHERE
				                                                razn > 400 
			                                                ) left_fast
			                                                FULL OUTER JOIN (
			                                                SELECT
				                                                * ,
				                                                koord koord_r 
			                                                FROM
				                                                (
				                                                SELECT
					                                                *,
					                                                round( ABS ( LEAD ( koord, 1 ) OVER ( ORDER BY km ) - koord ) ) razn 
				                                                FROM
					                                                (
					                                                SELECT
						                                                km,
						                                                mtr AS meter,
						                                                mtr * 1000.0 + (
							                                                ( SELECT travel_direction FROM trips WHERE ID = {Trip_id}  ) * ( local_fnum * 200.0 ) - ( SELECT car_position FROM trips WHERE ID = {Trip_id}  ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
						                                                ) AS koord,
						                                                oid,
						                                                threat_id 
					                                                FROM
						                                                RD_VIDEO_OBJECTS AS rvo
						                                                INNER JOIN trip_files AS tfile ON tfile.ID = rvo.file_Id 
					                                                WHERE
						                                                oid IN (
							                                                15,
							                                                16,
							                                                17,
							                                                18,
							                                                19,
							                                                20,
							                                                25,
							                                                26,
							                                                27,
							                                                28,
							                                                29,
							                                                31,
							                                                32,
							                                                33,
							                                                34,
							                                                35,
							                                                36,
							                                                37,
							                                                39 
						                                                ) 
						                                                AND tfile.trip_id = {Trip_id} 
						                                                AND km = {km} 
						                                                AND threat_id = 2 
					                                                ORDER BY
						                                                koord 
					                                                ) data1 
				                                                ) data2 
			                                                WHERE
				                                                razn > 400 
			                                                ) right_fast ON right_fast.km = left_fast.km_l 
			                                                AND right_fast.meter = left_fast.meter_l 
		                                                WHERE
			                                                koord_l >= koord_r - 200 
			                                                AND koord_l <= koord_r + 200 
		                                                ) data1 
	                                                ) data1 
                                                WHERE
	                                                angle >7 ",
                    commandType: CommandType.Text).ToList();
                }
                catch(Exception e)
                {
                    Console.Error.WriteLine("GetViolPerpenError:" +e.Message);
                    return null;
                }
                
            }
        }
        public List<Digression> NoBolt(MainParametersProcess process, Threat typ, int km)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var query = $@"SELECT
	                                    * 
                                    FROM
	                                    (
	                                    SELECT
		                                    *,
		                                    round( ABS ( LEAD ( koord, 1 ) OVER ( ORDER BY km ) - koord ) ) razn 
	                                    FROM
		                                    (
		                                    SELECT
			                                    km,
			                                    mtr meter,
			                                    round( koord ) koord,
			                                    oid,
			                                    file_id Fileid,
			                                    fnum,
			                                    ms 
		                                    FROM
			                                    (
			                                    SELECT
				                                    * 
			                                    FROM
				                                    (
				                                    SELECT
					                                    *,
					                                    round( ABS ( LEAD ( koord, 1 ) OVER ( ORDER BY km ) - koord ) ) razn 
				                                    FROM
					                                    (
					                                    SELECT
						                                    *,
						                                    mtr * 1000.0 + (
							                                    ( SELECT travel_direction FROM trips WHERE ID = {process.Trip_id} ) * ( local_fnum * 200.0 ) - ( SELECT car_position FROM trips WHERE ID = {process.Trip_id} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
						                                    ) AS koord 
					                                    FROM
						                                    rd_video_objects rvo
						                                    INNER JOIN trip_files tf ON tf.ID = rvo.file_id 
					                                    WHERE
						                                    km = {km} 
						                                    AND oid = {(int)VideoObjectType.GapFull} 
						                                    AND threat_id = {(int)typ} 
                                                            AND trip_id ={process.Trip_id}
					                                    ORDER BY
						                                    koord 
					                                    ) data1 
				                                    ) data2 
			                                    WHERE
				                                    razn > 50 
			                                    ) gaps UNION ALL
		                                    SELECT
			                                    km,
			                                    mtr meter,
			                                    round( koord ) koord,
			                                    oid,
			                                    file_id Fileid,
			                                    fnum,
			                                    ms 
		                                    FROM
			                                    (
			                                    SELECT
				                                    * 
			                                    FROM
				                                    (
				                                    SELECT
					                                    *,
					                                    round( ABS ( LEAD ( koord, 1 ) OVER ( ORDER BY km ) - koord ) ) razn 
				                                    FROM
					                                    (
					                                    SELECT
						                                    *,
						                                    mtr * 1000.0 + (
							                                    ( SELECT travel_direction FROM trips WHERE ID = {process.Trip_id} ) * ( local_fnum * 200.0 ) - ( SELECT car_position FROM trips WHERE ID = {process.Trip_id} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
						                                    ) AS koord 
					                                    FROM
						                                    rd_video_objects rvo
						                                    INNER JOIN trip_files tf ON tf.ID = rvo.file_id 
					                                    WHERE
						                                    km = {km}  
						                                    AND oid IN ( {(int)VideoObjectType.bolt_M24}, {(int)VideoObjectType.bolt_M22} ) 
						                                    AND threat_id = {(int)typ} 
                                                            AND trip_id ={process.Trip_id}
					                                    ORDER BY
						                                    koord 
					                                    ) data1 
				                                    ) data2 
			                                    WHERE
				                                    razn > 60 
				                                    AND h > 20 
				                                    AND h < 100 
				                                    AND y > 5 
				                                    AND x > 0 
				                                    AND w < 100 
			                                    ) bolts UNION ALL
		                                    SELECT
			                                    km,
			                                    mtr meter,
			                                    round( koord ) koord,
			                                    oid,
			                                    file_id Fileid,
			                                    fnum,
			                                    ms 
		                                    FROM
			                                    (
			                                    SELECT
				                                    * 
			                                    FROM
				                                    (
				                                    SELECT
					                                    *,
					                                    round( ABS ( LEAD ( koord, 1 ) OVER ( ORDER BY km ) - koord ) ) razn 
				                                    FROM
					                                    (
					                                    SELECT
						                                    *,
						                                    mtr * 1000.0 + (
							                                    ( SELECT travel_direction FROM trips WHERE ID = {process.Trip_id} ) * ( local_fnum * 200.0 ) - ( SELECT car_position FROM trips WHERE ID = {process.Trip_id} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
						                                    ) AS koord 
					                                    FROM
						                                    rd_video_objects rvo
						                                    INNER JOIN trip_files tf ON tf.ID = rvo.file_id 
					                                    WHERE
						                                    km = {km}  
						                                    AND oid = {(int)VideoObjectType.no_bolt}  
						                                    AND threat_id = {(int)typ}  
                                                            AND trip_id ={process.Trip_id}
					                                    ORDER BY
						                                    koord 
					                                    ) data1 
				                                    ) data2 
			                                    WHERE
				                                    razn > 50 
				                                    AND h > 20 
			                                    ) NoBolts 
		                                    ORDER BY
			                                    koord 
		                                    ) DATA 
	                                    ) DATA 
                                    WHERE
	                                    razn > 20000 
	                                    OR razn < 500 ";

                    var DB_obj = db.Query<Digression>(query, commandType: CommandType.Text).ToList();

                    var gaps = DB_obj.Where(o => o.Oid == (int)VideoObjectType.GapFull).ToList();

                    var AbsBoltList = new List<Digression> { };
                    

                    foreach (var gap in gaps)
                    {
                        var Before_gap = new List<Digression> { };
                        var After_gap = new List<Digression> { };
                        var Find_gap = false;

                        var Selected_region = DB_obj.Where(o => o.Meter >= gap.Meter - 1 && o.Meter <= gap.Meter + 1).ToList();

                        foreach (var item in Selected_region)
                        {
                            if(item.Oid == (int)VideoObjectType.GapFull)
                                Find_gap = true;

                            if (Find_gap == false)
                            {
                                Before_gap.Add(item);
                            }
                            else
                            {
                                After_gap.Add(item);
                            }
                        }

                        if(After_gap.Count > 4)
                        {
                            After_gap = After_gap.GetRange(0, 4).ToList();
                        }
                        if(Before_gap.Where(o=>o.Razn > 300).Any())
                        {
                            Before_gap.Clear();
                            After_gap.Clear();
                            Find_gap = false;
                            continue;
                        }
                        if (After_gap.Where(o => o.Razn > 300).Any())
                        {
                            Before_gap.Clear();
                            After_gap.Clear();
                            Find_gap = false;
                            continue;
                        }


                        var Before = Before_gap.Where(o => o.Oid == (int)VideoObjectType.no_bolt).ToList();
                        var After = After_gap.Where(o => o.Oid == (int)VideoObjectType.no_bolt).ToList();

                        var cc = Math.Max(Before_gap.Count, After_gap.Count) * 2;

                        string overlay = $"{ (cc >= 6 ? 6 : cc) } отверстия";



                        if (Before.Count == 0 && After.Count == 0)
                        {
                            Before_gap.Clear();
                            After_gap.Clear();
                            Find_gap = false;

                            continue;
                        }
                        Before_gap.Clear();
                        After_gap.Clear();
                        Find_gap = false;

                        var sss = Before.Count == 2 ? "25/25" : After.Count == 2 ? "25/25" : "";

                        if(sss != "")
                        {

                        }



                        AbsBoltList.Add(new Digression()
                        {
                            Km = gap.Km,
                            Meter = gap.Meter,
                            Overlay = overlay,
                            Before = Before.Count.ToString(),
                            After = After.Count.ToString(),
                            FullSpeed = Before.Count == 2 ? "25/25" : After.Count == 2 ? "25/25" : "",
                            Threat = typ,
                            //Note = list.Contains(gap_oid) ? "изолирующий" : "",
                            Fileid = Before.Count> After.Count ? Before.First().Fileid : After.First().Fileid,
                            Ms = Before.Count > After.Count ? Before.First().Ms : After.First().Ms,
                            Fnum = Before.Count > After.Count ? Before.First().Fnum : After.First().Fnum
                        });                        
                    }
                    return AbsBoltList;
                }
                catch(Exception e)
                {
                    Console.WriteLine("NoBoltError:" + e.Message);
                    return null;
                }

            }
        }
        public List<Digression> GetShpal(MainParametersProcess process, int[] typ, int km)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var query = $@"SELECT
	                                    km,
	                                    mtr AS meter,
	                                    oid,
	                                    file_id AS fileId,
	                                    ms,
	                                    fnum 
                                    FROM
	                                    (
	                                    SELECT
		                                    *,
		                                    round( ABS ( LEAD ( koord, 1 ) OVER ( ORDER BY km ) - koord ) ) razn 
	                                    FROM
		                                    (
		                                    SELECT
			                                    *,
			                                    mtr * 1000.0 + (
				                                    ( SELECT travel_direction FROM trips WHERE ID = {process.Trip_id} ) * ( local_fnum * 200.0 ) - ( SELECT car_position FROM trips WHERE ID = {process.Trip_id} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
			                                    ) AS koord 
		                                    FROM
			                                    rd_video_objects rvo
			                                    INNER JOIN trip_files tf ON tf.ID = rvo.file_id 
		                                    WHERE
			                                    km = {km} 
			                                    AND oid in ({(int)VideoObjectType.Railbreak_vikol}
                                                ,{(int)VideoObjectType.Railbreak_Stone_midsection}
                                                ,{(int)VideoObjectType.Railbreak_Stone_raskol}) 
		                                    ORDER BY
			                                    koord 
		                                    ) data1 
	                                    ) data2 
                                    WHERE
	                                    razn > 300 
	                                    AND y > 20";
                    return db.Query<Digression>(query, new { trip_id = process.Trip_id }, commandType: CommandType.Text).ToList();
                }
                catch(Exception e)
                {
                    Console.WriteLine("GetShpalError:" + e.Message);
                    return null;
                }
            }
        }


        

        public List<Digression> TrackDeviations(Int64 processId, string distanceName, int[] typ)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Digression>(
                    @"Select naprav as direction, put as track, pchu, pd, pdb, trip.trip_date as FoundDate, km, meter, ots as name, otkl as value, len as length, kol as count, typ, primech from s3 
                    INNER JOIN trips as trip on trip.id = s3.trip_id
                    where s3.trip_id = @process_id and s3.pch = @distance_Name --and s3.typ in ( " + string.Join(",", typ) + ") " + @"
                    order by direction, track, pchu, pd, pdb, km, meter",
                    new { process_id = processId, distance_Name = distanceName }, commandType: CommandType.Text).ToList();
            }
        }

        public List<Digression> DeviationsRailHeadWear(Int64 processId, string distanceName, int[] typ)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Digression>(
                    @"Select naprav as direction, put as track, pchu, pd, pdb, rdp.date_vrem as FoundDate, km, meter, ots as name, otkl as value, len as length, kol as count, typ, primech from s3 
                    INNER JOIN rd_process as rdp on rdp.id = s3.process_id
                    where s3.process_id = @process_id and s3.pch = @distance_Name and s3.typ in ( " + string.Join(",", typ) + ") " + @"
                    order by direction, track, pchu, pd, pdb, km, meter ",
                    new { process_id = processId, distance_Name = distanceName }, commandType: CommandType.Text).ToList();
            }
        }
        public List<Digression> DerogationsIsostsAndJointless(Int64 processId, string distanceName, int[] typ)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Digression>(
                    @"Select distinct  naprav as direction, put as track, pchu, pd, pdb, trip.trip_date as FoundDate, km, meter, ots as name, otkl as value, len as length, kol as count, typ, coalesce(norma.norma_width,-1),
                    CONCAT(CAST (coalesce(speed.passenger,-1) as text) ,'/' , CAST (coalesce(speed.freight,-1)  as text ), '/', CAST (coalesce(empty_freight,-1)  as text) ) as fullSpeed, 
                    CONCAT(CAST (s3.ovp as text) ,'/' , CAST (s3.ogp  as text ), '/', CAST (s3.ogp  as text) ) as allowSpeed, typ, norma.norma_width as norma from s3 
                    INNER JOIN trips as trip on trip.id = s3.trip_id
                    INNER JOIN adm_distance as distance on distance.name = s3.pch
                    INNER JOIN adm_track as track on track.code = s3.put
                    INNER JOIN adm_direction as direction on direction.name = s3.naprav and direction.id = track.adm_direction_id

                    Left JOIN tpl_period as speed_period on speed_period.adm_track_id = track.id and trip.trip_date between speed_period.start_date and speed_period.final_Date
                    INNER JOIN apr_speed as speed on speed.period_id = speed_period.id and isbelong(s3.km,s3.meter, speed.start_km, speed.start_m, speed.final_km, speed.final_m)
                    Left JOIN tpl_period as norma_period on norma_period.adm_track_id = track.id and trip.trip_date between norma_period.start_date and norma_period.final_Date
                    INNER JOIN apr_norma_width as norma on norma.period_id = norma_period.id and isbelong(s3.km,s3.meter, norma.start_km, norma.start_m, norma.final_km, norma.final_m)
                    where s3.trip_id = @process_id and s3.pch = @distance_Name and s3.typ in ( " + string.Join(",", typ) + ") " + @"
                    order by direction, track, pchu, pd, pdb, km, meter",
                    new { process_id = processId, distance_Name = distanceName }, commandType: CommandType.Text).ToList();
            }
        }
        public List<Digression> DeviationOfPRZH(Int64 processId, string distanceName, int[] typ)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Digression>(
                    @"Select naprav as direction, put as track, pchu, pd, pdb, trip.trip_date as FoundDate, km, meter, ots as name, otkl as value, len as length, kol as count, typ from s3 
                    INNER JOIN trips as trip on trip.id = s3.trip_id
                    where s3.trip_id = @process_id and s3.pch = @distance_Name --and s3.typ in ( " + string.Join(",", typ) + ") " + @"
                    order by direction, track, pchu, pd, pdb, km, meter ",
                    new { process_id = processId, distance_Name = distanceName }, commandType: CommandType.Text).ToList();
            }
        }
        public List<Digression> DeviationsToDangerous(Int64 processId, string distanceName, int[] typ)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                ////Console.Out.WriteLine("processId === > "+processId);
                ////Console.Out.WriteLine("distanceName === > " + distanceName);

                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Digression>(
                    @"Select 
	                    naprav as direction, 
	                    put as track, 
	                    pchu, 
	                    pd, 
	                    pdb, 
	                    trip_date as FoundDate, 
	                    km, 
	                    meter, 
	                    ots as name, 
	                    otkl as value, 
	                    len as length, 
	                    kol as count, 
	                    typ,
	                    CONCAT(CAST (coalesce(s3.uv,-1) as text) ,'/' , CAST (coalesce(s3.uvg,-1)  as text ), '/', CAST (coalesce(s3.uvg,-1)  as text) ) as fullSpeed, 
	                    CONCAT(CAST (s3.ovp as text) ,'/' , CAST (s3.ogp  as text ), '/', CAST (s3.ogp  as text) ) as allowSpeed
                    from 
	                    s3 
                    INNER JOIN 
	                    trips as trips on trips.id = s3.trip_id
                    where 
                        s3.trip_id = @process_id and s3.pch = @distance_Name and s3.ots = 'Уш' and s3.is2to3
                    order by 
	                    direction, 
	                    track, 
	                    pchu, 
	                    pd, 
	                    pdb, 
	                    km,
	                    meter",
                    new { process_id = processId, distance_Name = distanceName }, commandType: CommandType.Text).ToList();
            }
        }
        public List<Digression> DeviationsRailRailing(Int64 processId, string distanceName, int[] typ)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Digression>(
                    @"Select distinct  naprav as direction, put as track, pchu, pd, pdb, rdp.date_vrem as FoundDate, km, meter, ots as name, otkl as value, len as length, kol as count, typ, coalesce(norma.norma_width,-1),
                    CONCAT(CAST (coalesce(speed.passenger,-1) as text) ,'/' , CAST (coalesce(speed.freight,-1)  as text ), '/', CAST (coalesce(empty_freight,-1)  as text) ) as fullSpeed, 
                    CONCAT(CAST (s3.ovp as text) ,'/' , CAST (s3.ogp  as text ), '/', CAST (s3.ogp  as text) ) as allowSpeed, typ, norma.norma_width as norma from s3 
                    INNER JOIN rd_process as rdp on rdp.id = s3.process_id
                    INNER JOIN adm_distance as distance on distance.name = s3.pch
                    INNER JOIN adm_track as track on track.code = s3.put
                    INNER JOIN adm_direction as direction on direction.name = s3.naprav and direction.id = track.adm_direction_id

                    Left JOIN tpl_period as speed_period on speed_period.adm_track_id = track.id and rdp.date_vrem between speed_period.start_date and speed_period.final_Date
                    INNER JOIN apr_speed as speed on speed.period_id = speed_period.id and isbelong(s3.km,s3.meter, speed.start_km, speed.start_m, speed.final_km, speed.final_m)
                    Left JOIN tpl_period as norma_period on norma_period.adm_track_id = track.id and rdp.date_vrem between norma_period.start_date and norma_period.final_Date
                    INNER JOIN apr_norma_width as norma on norma.period_id = norma_period.id and isbelong(s3.km,s3.meter, norma.start_km, norma.start_m, norma.final_km, norma.final_m)
                    where s3.process_id = @process_id and s3.pch = @distance_Name and s3.typ in ( " + string.Join(",", typ) + ") " + @"
                    order by direction, track, pchu, pd, pdb, km, meter",
                    new { process_id = processId, distance_Name = distanceName }, commandType: CommandType.Text).ToList();
            }
        }
        public List<ControlAdjustmentProtocol> GetS3ByTripId(long processId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<ControlAdjustmentProtocol>(
                    $@"SELECT
	                        s3.ID original_id,
	                        naprav AS direction,
	                        put AS track,
	                        pchu,
	                        pd,
	                        pdb,
	                        trip.trip_date AS FoundDate,
	                        km,
	                        meter,
	                        ots AS NAME,
	                        otkl AS VALUE,
	                        len AS LENGTH,
	                        kol AS COUNT,
                            uv,
                            uvg,
	                        ovp, 
	                        ogp,
	                        typ
                        FROM
	                        s3
	                        INNER JOIN trips trip ON trip.ID = s3.trip_id 
                        WHERE
	                        s3.trip_id = {processId} 
                        ORDER BY
	                        direction,
	                        track,
	                        pchu,
	                        pd,
	                        pdb,
	                        km,
	                        meter",
                    new { process_id = processId }, commandType: CommandType.Text).ToList();
            }
        }
        public List<ControlAdjustmentProtocol> ControlAdjustmentProtocol(Int64 processId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<ControlAdjustmentProtocol>(
                    $@"SELECT 
	                        original_id,
	                        naprav AS direction,
	                        put AS track,
	                        pchu,
	                        pd,
	                        pdb,
	                        km,
	                        meter,
	                        ots AS NAME,
	                        otkl AS VALUE,
	                        len AS LENGTH,
	                        kol AS COUNT,
                            uv,
                            uvg,
	                        ovp,
	                        ogp,
	                        typ,
	                        state_id,
	                        comment,
                            editor
                        FROM
	                        s3_history 
                        WHERE
	                        trip_id = {processId}",
                    new { process_id = processId }, commandType: CommandType.Text).ToList();
            }
        }
        public List<Digression> AverageScoreDepartments(Int64 tripid, string distanceName, int[] typ)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Digression>(
                    $@"SELECT
	                        --pch,
	                        pd,
	                        pdb,
	                        round( AVG ( ball ) ) avgBall 
                        FROM
	                        bedemost
	                        INNER JOIN trips AS trip ON trip.ID = bedemost.trip_id 
                        WHERE
	                        bedemost.trip_id = {tripid} 
	                        AND bedemost.pch = '{distanceName}' 
                        GROUP BY
	                        --pch,
	                        pd,
	                        pdb",
                    new { process_id = tripid, distance_Name = distanceName }, commandType: CommandType.Text).ToList();
            }
        }
        
        public object GetS3(Int64 trip_id, string digressionName = null)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                //Закоментил для работы с ПУ-32 КОМП нужны были все степени
      //          if (db.State == ConnectionState.Closed)
      //              db.Open();
      //          return db.Query<S3>(
      //              $@"select direction.code as Directcode, s3.*, trips.trip_date as TripDateTime from s3 
      //                  inner join adm_track as track on track.id = s3.track_id
						//inner join adm_direction as direction on direction.id = track.adm_direction_id
      //                  inner join trips on s3.trip_id = trips.id 
      //                  where trips.id = {trip_id} and s3.typ in (3, 4) 
      //                  order by naprav, pd, put, km, meter", commandType: CommandType.Text).ToList();

                if (db.State == ConnectionState.Closed)
                    db.Open();
                
                return db.Query<S3>(
                    $@"select direction.code as Directcode, s3.*, trips.trip_date as TripDateTime from s3 
                        inner join adm_track as track on track.id = s3.track_id
						inner join adm_direction as direction on direction.id = track.adm_direction_id
                        inner join trips on s3.trip_id = trips.id 
                        where trips.id = {trip_id}{digressionName?.Replace(digressionName, $" and s3.ots = '{digressionName}' ")}
                        order by naprav, pd, put, km, meter", commandType: CommandType.Text).ToList();
            }
        }

        public object GetS3ForKm(Int64 trip_id, int km)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<S3>(
                    $@"select direction.code as Directcode, s3.*, trips.trip_date as TripDateTime from s3 
                        inner join adm_track as track on track.id = s3.track_id
						inner join adm_direction as direction on direction.id = track.adm_direction_id
                        inner join trips on s3.trip_id = trips.id 
                        where trips.id = {trip_id} and s3.typ in (3, 4)  and s3.km = { km} 
                        order by naprav, pd, put, km, meter", commandType: CommandType.Text).ToList();
            }
        }

        public object GetS3(Int64 processId, int type)
        {
            Console.Out.WriteLine("processId === >"+ processId);

            Console.Out.WriteLine("type === >" + type);

            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<S3>(
                    $@"SELECT DISTINCT ON
	                (
		                s3.naprav,
		                s3.put,
		                s3.pchu,
		                s3.pd,
		                s3.pdb,
		                s3.put,
		                s3.km,
		                s3.meter 
	                ) meter,
	                s3.ID,
	                s3.pch,
	                s3.naprav,
	                s3.put,
	                s3.pchu,
	                s3.pd,
	                s3.pdb,
	                s3.put,
	                s3.km,
	                s3.trip_id,
	                s3.ots,
	                s3.kol,
	                s3.otkl,
	                s3.len,
	                s3.primech,
	                s3.tip_poezdki,
	                s3.cu,
	                s3.us,
	                s3.p1,
	                s3.ur,
	                s3.pr,
	                s3.r1,
	                s3.r2,
	                s3.bas,
	                s3.typ,
	                s3.uv,
	                s3.uvg,
	                s3.ovp,
	                s3.ogp,
	                s3.is2to3,
	                s3.track_id,
	                s3.onswitch,
	                direction.code AS Directcode,
	                trip.trip_date AS TripDateTime,
	                ( COALESCE ( direction.NAME, '-' ) || '(' || COALESCE ( direction.code, '-' ) || ')' ) AS Direction_Full 
                FROM
	                s3
	                INNER JOIN adm_track AS track ON track.ID = s3.track_id
	                INNER JOIN trips trip ON trip.ID = s3.trip_id
	                LEFT JOIN adm_direction direction ON direction.NAME = s3.naprav 
                WHERE
	                trip.ID = @process_id 
	                AND s3.typ = @type_ 
	                AND NOT ( s3.onswitch ) 
                ORDER BY
	                s3.naprav,
	                s3.put,
	                s3.pchu,
	                s3.pd,
	                s3.pdb,
	                s3.put,
	                s3.km,
	                s3.meter",new { process_id = processId, type_ = type }, commandType: CommandType.Text).ToList();
            }
        }
        public object GetS3(Int64 processId, int type, string distanceName)
        {
            Console.Out.WriteLine("processId === >" + processId);

            Console.Out.WriteLine("type === >" + type);

            distanceName = distanceName == "ДТЖ" ? "64" : distanceName;

            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                var query = $@"SELECT 
                    s3.meter,
	                s3.ID,
	                s3.pch,
	                coalesce(s3.naprav,' ') as naprav,
	                s3.put,
	                s3.pchu,
	                s3.pd,
	                s3.pdb,
	                s3.put,
	                s3.km,
	                s3.trip_id,
	                s3.ots,
	                s3.kol,
	                s3.otkl,
	                s3.len,
	                s3.primech,
	                s3.tip_poezdki,
	                s3.cu,
	                s3.us,
	                s3.p1,
	                s3.ur,
	                s3.pr,
	                s3.r1,
	                s3.r2,
	                s3.bas,
	                s3.typ,
	                s3.uv,
	                s3.uvg,
	                s3.ovp,
	                s3.ogp,
	                s3.is2to3,
	                s3.track_id,
	              s3.onswitch,
	                direction.code AS Directcode,
	                trip.trip_date AS TripDateTime,
	                ( COALESCE ( direction.NAME, '-' ) || '(' || COALESCE ( direction.code, '-' ) || ')' ) AS Direction_Full 
                FROM
	                s3
	                INNER JOIN adm_track AS track ON track.ID = s3.track_id
	                INNER JOIN trips trip ON trip.ID = s3.trip_id
	                LEFT JOIN adm_direction direction ON direction.NAME = s3.naprav 
                WHERE
	                trip.ID = {processId}
	                AND s3.typ = {type} 
                    AND s3.pch = '{distanceName}'
	             --AND NOT ( s3.onswitch ) 
                ORDER BY
	                s3.naprav,
	                s3.put,
	                s3.pchu,
	                s3.pd,
	                s3.pdb,
	                s3.put,
	                s3.km,
	                s3.meter";
                return db.Query<S3>(query
                    , new { process_id = processId, type_ = type, distName = distanceName}, commandType: CommandType.Text).ToList();
            }
        }

        public object GetBedemost(Int64 trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Bedemost>(
                    $@"select bed.*, fdbroad+fdconstrict+fdskew+fddrawdown+fdstright+fdlevel type1  
                       from bedemost bed 
                       where bed.trip_id = {trip_id} 
                       order by naprav, pchu, pd, put, kmtrue", commandType: CommandType.Text).ToList();
            }
        }

        public List<Curve> GetCurves(Int64 processId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Curve>(
                    @"select apr_curve.*, min(curve.passspeed) as passspeed, min(curve.freightspeed) as freightspeed from apr_curve 
                        inner join rd_curve curve on curve.curve_id = apr_curve.id and curve.process_id = @process_id
                        group by (apr_curve.id)",
                    new { process_id = processId }, commandType: CommandType.Text).ToList();
            }
        }

        /// <summary>
        /// Взять стыки между двумя координатами
        /// </summary>
        /// <param name="processId">ID процесса</param>
        /// <param name="trackId">ID пути</param>
        /// <param name="coordStart">Координата начала (Km * 10000 + Meter + Mm / 1000.0)</param>
        /// <param name="coordFinal">Координата конца (Km * 10000 + Meter + Mm / 1000.0)</param>
        /// <returns>Лист стыков</returns>
        public List<Gaps> GetGapsBetweenCoords(long processId, long trackId, double coordStart, double coordFinal)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Gaps>($@"select gaps.* from rd_gaps gaps
                    where gaps.track_id = {trackId} and gaps.process_id = {processId}
                    and gaps.nkm * 10000 + (gaps.picket - 1) * 100 + gaps.meter + ((gaps.start + gaps.final) / 2) / 1000.0 between {coordStart} and {coordFinal}").ToList();
            }
        }

        /// <summary>
        /// Взять стыки и изостыки пути
        /// </summary>
        /// <param name="processId">ID процесса</param>
        /// <param name="trackId">ID пути</param>
        /// <returns>Лист стыков и изостыков</returns>
        public List<Gaps> GetGapsAndIsoGaps(long processId, long trackId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Gaps>($@"select files.threat_id, gaps.nkm, gaps.picket, gaps.meter, gaps.start, gaps.final, '' as ifiso from rd_gaps gaps
                    inner join trip_files files on files.id = gaps.file_id
                    where gaps.track_id = {trackId} and gaps.process_id = {processId}
                    union all
                    select files.threat_id, rvo.km as nkm, rvo.pt as picket, (rvo.pt - 1) * 100 + rvo.mtr as meter, least(firstCoord.mm, secondCoord.mm) as start, greatest(firstCoord.mm, secondCoord.mm) as final, 'изолирующий' as ifiso from rd_video_objects rvo
                    inner join trip_files files on files.id = rvo.fileid
                    inner join trips on trips.id = rvo.trip_id
                    inner join getrealcoords_videoobjects(rvo.km, rvo.pt, rvo.mtr, rvo.x, rvo.mm, files.coef_camera, trips.travel_direction) firstCoord on true
                    inner join getrealcoords_videoobjects(rvo.km, rvo.pt, rvo.mtr, rvo.x + rvo.h, rvo.mm, files.coef_camera, trips.travel_direction) secondCoord on true
                    where rvo.track_id = {trackId} and rvo.oid = {(int)VideoObjectType.GapIso}
                    order by nkm, picket, meter, start").ToList();
            }
        }
        public List<Gaps> GetGaps(Int64 processId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Gaps>(
                    $@"select gaps.*, files.threat_id, (direction.name || ' (' || direction.code || ')') as direction_full, track.code as track from rd_gaps gaps
                        inner join trip_files files on files.id = gaps.file_id
                        inner join adm_track track on track.id = gaps.track_id
                        inner join adm_direction direction on direction.id = track.adm_direction_id
                        where gaps.process_id={processId}
                        order by gaps.nkm, gaps.picket, gaps.meter", commandType: CommandType.Text).ToList();
            }
        }
        public List<Digression> GetDigSleepers(MainParametersProcess mainProcess, int getDigSleepers)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (getDigSleepers)
                {
                    case 41:
                        return db.Query<Digression>(
                                $@"SELECT DISTINCT
	                                    km,
	                                    mtr AS meter,
	                                    oid 
                                    FROM
	                                    rd_video_objects rvo
	                                    INNER JOIN trip_files tf ON tf.ID = rvo.file_id 
                                    WHERE
	                                    oid IN ( 48 ) 
	                                    AND trip_id = {mainProcess.Trip_id} 
                                    ORDER BY
	                                    km,
	                                    mtr", commandType: CommandType.Text).ToList();
                    default:
                        return null;
                }
            }
        }

        public object GetRdProfileObjects(long trackId, DateTime date, int type, int start_km, int start_m, int final_km, int final_m)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                Console.Out.WriteLine("trackId "+trackId);
                Console.Out.WriteLine("date " + date);
                Console.Out.WriteLine("start_km " + start_km);
                Console.Out.WriteLine("start_m " + start_m);
                Console.Out.WriteLine("final_km " + final_km);
                Console.Out.WriteLine("final_m " + final_m);
                String dbDate = date.ToString("yyyy'-'MM'-'dd");
                Console.Out.WriteLine("dbDate "+dbDate);
                
                switch (type)
                {
                    case 0:
                        return db.Query<Curve>($@"select curves.*, max(stcurve.radius) as maxradius, coalesce(max(abs(elcurve.lvl)), 0) as maxlvl from apr_curve curves
                            inner join apr_stcurve stcurve on stcurve.curve_id = curves.id
                            left join apr_elcurve elcurve on elcurve.curve_id = curves.id
                            inner join tpl_period curveperiod on curveperiod.id = curves.period_id and '{dbDate}' between curveperiod.start_date and curveperiod.final_date
                            inner join adm_track track on track.id = curveperiod.adm_track_id and track.id = {trackId}
                            where numrange(coordinatetoreal(curves.start_km,curves.start_m), coordinatetoreal(curves.final_km,curves.final_m)) && 
                                { (start_km+start_m/10000.0< final_km+final_m/10000.0 ? $"numrange(coordinatetoreal({start_km},{start_m}),coordinatetoreal({final_km},{final_m}))" : $"numrange(coordinatetoreal({final_km},{final_m}),coordinatetoreal({start_km},{start_m}))")}
                            group by curves.id, stcurve.id, elcurve.id", commandType: CommandType.Text).ToList();
                    case 1:
                        return db.Query<ArtificialConstruction>($@"select bridges.* from apr_artificial_construction bridges
                            inner join tpl_period bridgeperiod on bridgeperiod.id = bridges.period_id and '{dbDate}' between bridgeperiod.start_date and bridgeperiod.final_date
							inner join adm_track track on track.id = bridgeperiod.adm_track_id and track.id = {trackId} 
                            where isbelong(bridges.km, bridges.meter, {start_km}, {start_m}, {final_km}, {final_m})
                            and bridges.type_id = 1",
                            commandType: CommandType.Text).ToList();
                    case 2:
                        return db.Query<Switch>($@"select switches.* from tpl_switch switches
                            inner join tpl_period switchperiod on switchperiod.id = switches.period_id and '{dbDate}' between switchperiod.start_date and switchperiod.final_date
                            inner join adm_track track on track.id = switchperiod.adm_track_id and track.id = {trackId}
                            where isbelong(switches.km, switches.meter, {start_km}, {start_m}, {final_km}, {final_m})",
                            commandType: CommandType.Text).ToList();
                    case 3:
                        var query = $@"
                            SELECT * FROM (
                                select section.*,
                                LEAD ( station.NAME, -1 ) OVER ( ORDER BY station.NAME ) as prevstation,
	                            station.NAME AS Station ,
	                            LEAD ( station.NAME, 1 ) OVER ( ORDER BY station.NAME ) as nextstation

                                from tpl_station_section section
                                inner join adm_station station on station.id = section.station_id
                                inner join tpl_period stationperiod on stationperiod.id = section.period_id and '{dbDate}' between stationperiod.start_date and stationperiod.final_date
                                inner join adm_track track on track.id = stationperiod.adm_track_id and track.id = {trackId}
                            ) section
                            where (section.start_km * 1000 + section.start_m < {final_km * 1000 + final_m}) and (section.final_km * 1000 + section.final_m > {start_km * 1000 + start_m})";
                        return db.Query<StationSection>(query, commandType: CommandType.Text).ToList();
                    default:
                        return null;
                }
            }
        }
        List<RdProfile> IRdStructureRepository.GetRdTablesByKM(MainParametersProcess tripProcess, float stKM, float fnKM)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var query = $@"SELECT
	                                    o.km,
	                                    o.meter,
	                                    o.latitude,
	                                    o.longitude,
	                                    o.heigth,
                                        o.gauge,
	                                    o.level,
	                                    COALESCE ( rd.jgps_sats, - 999 ) jgps_sats,
	                                    is_station ( {tripProcess.TrackID}, '{tripProcess.Date_Vrem}', o.km, o.meter ) AS isstation 
                                    FROM
	                                    outdata_{tripProcess.Trip_id} o
	                                    FULL OUTER JOIN ( SELECT km, ( piket - 1 ) * 100 + metr AS meter, jgps_sats FROM raw_data_228_20211016_144703_3ec1ce7d30 ) rd ON rd.km = o.km 
	                                    AND rd.meter = o.meter 
                                    WHERE
	                                    o.km*1000+o.meter BETWEEN {stKM} 
	                                    AND {fnKM} 
                                    ORDER BY
	                                    o.km,
	                                    o.meter";

                    return db.Query<RdProfile>(query).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"GetRdTablesByKM " + e.Message);
                    return null;
                }
            }
        }
        
        public object GetRdTables(MainParametersProcess process, int type_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                switch (type_id)
                {
                    case 10:
                        return db.Query<RdProfile>($@"SELECT
	                                                    adm_track.code AS track,
	                                                    km,
	                                                    meter,
	                                                    latitude,
	                                                    longitude,
	                                                    outdata_{process.Trip_id}.heigth,
	                                                    is_artificial_construction ( adm_track.ID, '26.10.2020', km, meter ) AS isbridge,
	                                                    is_station ( adm_track.ID, '26.10.2020', km, meter ) AS isstation,
	                                                    is_switch ( adm_track.ID, '26.10.2020', km, meter ) AS isswitch 
                                                    FROM
	                                                    outdata_{process.Trip_id}
	                                                    INNER JOIN trips trips ON trips.ID = outdata_{process.Trip_id}.trip_id
	                                                    INNER JOIN adm_track adm_track ON adm_track.adm_direction_id = trips.direction_id 
                                                    WHERE
	                                                    trip_id = {process.Trip_id} and adm_track.code ='1' and km>0
                                                    ORDER BY
	                                                    km,
	                                                    meter
	                                                     LIMIT 3000  ", commandType: CommandType.Text).ToList();
                    case 11:
                        return db.Query<RdProfile>($@"SELECT
	                                                    o.km,
	                                                    o.meter,
	                                                    o.latitude,
	                                                    o.longitude,
	                                                    o.heigth,
	                                                    rd.jgps_sats 
                                                    FROM
	                                                    outdata_{process.Trip_id} o
	                                                    FULL OUTER JOIN (
	                                                    SELECT
		                                                    km,
		                                                    ( piket - 1 ) * 100 + metr AS meter,
		                                                    jgps_sats 
	                                                    FROM
		                                                    raw_data_{process.Trip_id} 
	                                                    ORDER BY
		                                                    km DESC,
		                                                    ( piket - 1 ) * 100 + metr DESC 
	                                                    ) rd ON rd.km = o.km 
	                                                    AND rd.meter = o.meter 
                                                    WHERE
	                                                    o.km > 0 
                                                    ORDER BY
	                                                    o.km DESC,
	                                                    o.meter DESC  ", commandType: CommandType.Text).ToList();
                    case 12:
                        //Ведомость ускорений, вызванных длинными неровностями
                        return db.Query<RdProfile>($@"SELECT ROW_NUMBER
	                                                        ( ) OVER ( ) x, * 
                                                        FROM
	                                                        (
	                                                        SELECT
		                                                        km,
		                                                        meter M,
		                                                        meter meter,
		                                                        km + meter / 10000.0 realcoord,
		                                                        km * 1000+meter koord,
		                                                        heigth y,
		                                                        stright_avg,
                                                                stright_left,
		                                                        stright_right,
		                                                        ( stright_left + stright_right ) / 2.0 plan
	                                                        FROM
		                                                        outdata_{process.Id} 
	                                                        WHERE
		                                                        km > 0 
	                                                        ORDER BY
	                                                        realcoord 
	                                                        ) DATA", commandType: CommandType.Text).ToList();
                    case 0: //RdIrregularity

                        return db.Query<RdIrregularity>($@"select irregularity.*, track.code as trackname, (direction.name || '(' || direction.code || ')') as Direction,
                            is_artificial_construction(irregularity.track_id, '{process.Date_Vrem:dd-MM-yyyy}', irregularity.km, irregularity.meter) as isbridge, is_station(irregularity.track_id, '{process.Date_Vrem:dd-MM-yyyy}', irregularity.km, irregularity.meter) as isstation, is_switch(irregularity.track_id, '{process.Date_Vrem:dd-MM-yyyy}', irregularity.km, irregularity.meter) as isswitch
                            from rd_irregularity as irregularity
                            inner join adm_track track on track.id = irregularity.track_id
                            inner join adm_direction direction on direction.id = track.adm_direction_id
                            where irregularity.process_id = {process.Id}", commandType: CommandType.Text).ToList();
                    case 1: //RdProfile
                        Console.Out.WriteLine("the trip id ==> "+process.Id);
                        var query = $@"
                        SELECT ROW_NUMBER
	                        ( ) OVER ( ) x,* 
                        FROM
	                        (SELECT 
                                id,
	                            km,
	                            meter M,
                                meter meter,
	                            km * 1000+meter koord,
	                            heigth y,
                                km + meter / 10000.0 realcord 
                            FROM
	                            outdata_{process.Trip_id} 
                            WHERE
                               km > 0
	                           --AND id>=122928 
	                           --AND km < 30 Mine
	                           ORDER BY km + meter / 10000.0 
                          --km > 0 
                          --ORDER BY
	                        --km + meter/10000.0 
                            ) data  ";
                        return db.Query<RdProfile>(query, commandType: CommandType.Text).ToList();

                    case 2: //RdStatisticRoughnessImpulse
                        return db.Query<RdStatisticRoughnessImpulse>(@"select impulse.*, track.code as Track, (direction.name || '(' || direction.code || ')') as Direction from rd_statistic_roughness_impulse impulse
                            inner join adm_track track on track.id = impulse.track_id
                            inner join adm_direction direction on direction.id = track.adm_direction_id
                            inner join rd_process process on process.id = impulse.process_id
                            where process.id = @mainProcessId", new { mainProcessId = process.Id }, commandType: CommandType.Text).ToList();
                    case 3: //RdIntegralSurfaceRails
                        return db.Query<RdIntegralSurfaceRails>(@"select impulse.*, track.code as Track, (direction.name || '(' || direction.code || ')') as Direction from rd_integral_surface_rails impulse
                            inner join adm_track track on track.id = impulse.track_id
                            inner join adm_direction direction on direction.id = track.adm_direction_id
                            inner join rd_process process on process.id = impulse.process_id
                            where process.id = @mainProcessId", new { mainProcessId = process.Id }, commandType: CommandType.Text).ToList();
                    case 4: //RdStatistics
                        var txt = @"select statistics.* from rd_statistics as statistics
                            where statistics.process_id = @process_id";
                        return db.Query<RdStatistics>(txt).ToList();
                        //return db.Query<RdStatistics>(@"select statistics.* from rd_statistics as statistics
                        //    where statistics.process_id = @process_id",
                        //    new { process_id = process.Id }, commandType: CommandType.Text).ToList();
                    case 5: //RdEpure
                        return db.Query<RdEpure>(@"select epure.* from rd_epure as epure
                            where epure.process_id = @process_id and epure.track_id = @track_id",
                            new { process_id = process.Id, track_id = process.TrackID }, commandType: CommandType.Text).ToList();
                    case 6: //RdRailSlope ПУ
                        return db.Query<RdRailSlope>($@"select slope.*, track.code as Track, (direction.name || '(' || direction.code || ')') as Direction, files.threat_id from rd_rail_slope as slope
                            inner join trip_files files on files.id = slope.file_id
                            inner join adm_track track on track.id = slope.track_id
                            inner join adm_direction direction on direction.id = track.adm_direction_id
                            where slope.process_id = {process.Id}",
                            commandType: CommandType.Text).ToList();
                    case 7: //RdSurfaceSlope НПК
                        return db.Query<RdSurfaceSlope>($@"select slope.*, track.code as Track, (direction.name || '(' || direction.code || ')') as Direction, files.threat_id from rd_surface_slope as slope
                            inner join trip_files files on files.id = slope.file_id
                            inner join adm_track track on track.id = slope.track_id
                            inner join adm_direction direction on direction.id = track.adm_direction_id
                            where slope.process_id = {process.Id}",
                            commandType: CommandType.Text).ToList();
                    case 8: //RdMovementThread
                        return db.Query<RdMovementThread>($@"select movement.*, track.code as Track, (direction.name || '(' || direction.code || ')') as Direction, files.threat_id from rd_movement_thread as movement
                            inner join trip_files files on files.id = movement.file_id
                            inner join adm_track track on track.id = movement.track_id
                            inner join adm_direction direction on direction.id = track.adm_direction_id
                            where movement.trip_id = {process.Id}",
                            commandType: CommandType.Text).ToList();
                    case 9: //RdCurve
                        return db.Query<RDCurve>($@"select rdcurve.* from rd_curve as rdcurve
                            where rdcurve.trip_id = {process.Trip_id}",
                            commandType: CommandType.Text).ToList();
                    default:
                        return null;
                }
            }
        }

        public bool CleanTables(int type)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                switch (type)
                {
                    case 0:
                        return db.Execute(@"delete from station_possible_paths where true",
                            new { type }, commandType: CommandType.Text) != 0;
                    default:
                        return false;
                }
            }
        }

        public List<Kilometer> GetKilometersByTripId(long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                var maintrackrep = new MainTrackStructureRepository();
                string sqltext = @"
                select distinct direction.id as direction_id, concat(direction.name, '(', direction.code, ')') as direction_name, track.id as track_id,track.code as track_name, kilom.*,  bed.ball as point from kilometers as kilom
                inner join adm_track as track on track.id = kilom.track_id
                inner join adm_direction as direction on direction.id = track.adm_direction_id
                left join bedemost as bed on bed.naprav = direction.name and kilom.number = bed.kmtrue
                where kilom.trip_id = " + trip_id + $" order by direction.id, track.id, kilom.id";
                var result = db.Query<Kilometer>(sqltext).ToList();
                var trip = GetTrip(trip_id);
                foreach (var kilomerter in result)
                {
                    kilomerter.Trip = trip;
                    kilomerter.LoadPasport(maintrackrep);
                }
                return result;
                
            }
        }

        public bool GetButtonState(string name)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string sqltext = @"
                SELECT pressed
	                FROM public.pp_diagram_button_state where name = '" + name + "'";
                try
                {
                    return db.QueryFirst<bool>(sqltext);
                }
                catch
                {
                    db.Execute("INSERT INTO public.pp_diagram_button_state(name, pressed) VALUES(@bname, false) ", new { bname = name }, commandType: CommandType.Text);
                }
                return false;
            }
        }

        public void SetButtonStatus(string name, bool status)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                db.Execute(@"
                UPDATE public.pp_diagram_button_state
	            SET pressed=@state
	            WHERE name = @bname", new { state = status, bname = name }, commandType: CommandType.Text);

            }
        }

        public int InsertTrip(Trips trip)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                var trip_id = db.QueryFirst<int>(@"
                    INSERT INTO trips(
                        direction_id, car, chief, travel_direction, car_position, start_station, final_station, trip_type, trip_date, 
                        current, start_position, track_id, rail_profile, longitudinal_profile, short_irregularities, joint_gaps, georadar, 
                        dimensions, beacon_marks, embankment, rail_temperature, geolocation, rail_video_monitoring, video_monitoring, road_id, processed)

                    VALUES(@direction_id, (select value from PARAMETER WHERE name = 'car' limit 1), @chief, @travel_direction, @car_position, @start_station, @final_station, @trip_type, @trip_date, 
                        true, @start_position, @track_id, @rail_profile, @longitudinal_profile, @short_irregularities, @joint_gaps, @georadar, 
                        @dimensions, @beacon_marks, @embankment, @rail_temperature, @geolocation, @rail_video_monitoring, @video_monitoring, @road_id, @processed) RETURNING id", new
                {
                    direction_id = trip.Direction_id,
                    car = trip.Car,
                    chief = trip.Chief,
                    travel_direction = (int)trip.Travel_Direction,
                    car_position = (int)trip.Car_Position,
                    start_station = trip.Start_station,
                    final_station = trip.Final_station,
                    trip_type = (int)trip.Trip_Type,
                    trip_date = DateTime.Now,
                    start_position = trip.Start_Position,
                    track_id = trip.Track_Id,
                    rail_profile = trip.Rail_Profile,
                    longitudinal_profile = trip.Longitudinal_Profile,
                    short_irregularities = trip.Short_Irregularities,
                    joint_gaps = trip.Joint_Gaps,
                    georadar = trip.Georadar,
                    dimensions = trip.Dimensions,
                    beacon_marks = trip.Beacon_Marks,
                    embankment = trip.Embankment,
                    rail_temperature = trip.Rail_Temperature,
                    geolocation = trip.Geolocation,
                    rail_video_monitoring = trip.Rail_Video_Monitoring,
                    video_monitoring = trip.Video_Monitoring,
                    road_id = trip.Road_Id,
                    processed = false
                }, commandType: CommandType.Text); ;

                db.Execute(@"CREATE TABLE public.outdata_" + trip_id + @"
                (
                    id serial,
                    speed smallint NOT NULL,
                    km smallint ,
                    meter smallint ,
                    gauge real ,
                    x101_kupe real ,
                    x102_koridor real,
                    y101_kupe real ,
                    y102_koridor real,
                    gauge_correction real,
                    level real,
                    level_correction real,
                    stright_left real ,
                    stright_right real ,
                    stright_avg real ,
                    stright_avg_70 real ,
                    stright_avg_100 real ,
                    stright_avg_120 real ,
                    stright_avg_150 real,
                    drawdown_left real ,
                    drawdown_right real ,
                    _meters integer ,
                    level_avg real ,
                    level_avg_70 real ,
                    level_avg_100 real ,
                    level_avg_120 real ,
                    level_avg_150 real ,
                    drawdown_avg real ,
                    drawdown_avg_70 real ,
                    drawdown_avg_100 real ,
                    drawdown_avg_120 real ,
                    drawdown_avg_150 real ,
                    drawdown_left_SKO real ,
                    drawdown_right_SKO real ,
                    level_SKO real ,
                    skewness_PXI real ,
                    skewness_SKO real ,
                    SSSP_before real ,
                    SSSP_speed real ,
                    latitude real ,
                    longitude real ,
                    heigth real ,
                    level_zero real ,
                    enc_on_meter_begin double precision,
                    val01 real,
                    val02 real,
                    val03 real,
                    val04 real,
                    val05 real,
                    val06 real,
                    val07 real,
                    val08 real,
                    val09 real,
                    val10 real,
                    level1 real,
                    level2 real,
                    level3 real,
                    level4 real,
                    level5 real,
                    stright1 real,
                    stright2 real,
                    stright3 real,
                    stright4 real,
                    stright5 real,
                    trip_id integer,
                    rail_temp_kupe smallint,
                    rail_temp_koridor smallint,
                    ambient_temp smallint,
                    accelerometer_y_axis real,
                    correction smallint NOT NULL DEFAULT 0,
                    CONSTRAINT outdata_" + trip_id + @"_fkey PRIMARY KEY (id),
                    CONSTRAINT trip_" + trip_id + @"fkey FOREIGN KEY (trip_id)
                        REFERENCES public.trips (id) MATCH SIMPLE
                        ON UPDATE NO ACTION
                        ON DELETE CASCADE
                )
                ");

                foreach (var fragment in trip.Route)
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
                foreach(var escort in trip.Escort)
                {
                    db.Execute($@"
                        INSERT INTO public.escort(
                            trip_id, distance_id, fullname
                        )
                        VALUES ({trip_id}, {escort.Distance_Id}, '{escort.FullName}');
                    ");
                }
                return trip_id;
            }
        }
        public Trips GetCurrentTrip()
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                string sqltext = @"
                SELECT trip.*, direction.name as direction_name FROM trips as trip 
                INNER JOIN adm_direction as direction on direction.id = trip.direction_id
                WHERE
                  trip.id = 242
                  --current = true 
                  order by id desc limit 1";
                
                try
                {
                    var result = db.QueryFirst<Trips>(sqltext);
                    result.Escort = db.Query<Escort>($@"
                        Select escort.*, distance.name Distance_Name, true as saved from escort
                        inner join adm_distance distance on distance.id = escort.distance_id where trip_id= {result.Id}").ToList();

                    result.Distances = db.Query<AdmDistance>($@"select dist.* from adm_distance dist
                        inner join adm_nod nod on nod.id = dist.adm_nod_id
                        where nod.adm_road_id ={result.Road_Id}").ToList();
                    return result;
                }
                catch (Exception e)
                {
                    System.Console.Error.WriteLine("Ошибка при получении текущей поездки: " + e.Message);
                }
                return null;
            }
        }

        public void CloseTrip(long tripId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                db.Execute(@"
                    UPDATE trips
	                SET current=false
	                WHERE id = @trip_id", new { trip_id = tripId }, commandType: CommandType.Text);
            }
        }

        public List<Fragment> GetTripFragments(long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Fragment>(
                    @"select fragments.*, fragments.adm_track_id as track_id, tr.code as track_code  from fragments 
                        inner join adm_track as tr on tr.id = fragments.adm_track_id
                        where trip_id = @tripid 
                        order by id",
                    new { tripid = trip_id }, commandType: CommandType.Text).ToList();
            }
        }
        public List<MainParametersProcess> GetProcess(ReportPeriod period, long distanceId, ProcessType processType)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<MainParametersProcess>(
                        $@"select distinct trip.id as id, trip.id as Trip_id, trip.trip_date as Date_Vrem, trip.car_position as carposition, dir.NAME as directionname, dir.code as DirectionCode,  trip.travel_direction as direction, 
                            coalesce(trip.chief, 'неизвестный') as chief,  coalesce(trip.car, 'неизвестный') as car, 
                            coalesce(to_char(trip.trip_date, 'DD.MM.YYYY'), 'неизвестный') as trip_date 
                        from trips as trip 
                        INNER join adm_direction as dir on dir.id = trip.direction_id
                        inner join adm_track as track on track.adm_direction_id = dir.id
                        inner join tpl_period as period on period.adm_track_id = track.id and period.mto_type = {MainTrackStructureConst.MtoDistSection}
                            and period.start_date = (select max(start_date) from tpl_period where tpl_period.adm_track_id = track.id and tpl_period.mto_type = {MainTrackStructureConst.MtoDistSection})
                        inner join tpl_dist_section as section on section.period_id = period.id and section.adm_distance_id = {distanceId} 
                        where trip.trip_date between @startDate and @finishDate ",
                    new { startDate = period.StartDate, finishDate = period.FinishDate }, commandType: CommandType.Text).ToList();

                
            }
        }
        /// <summary>
        /// Вернуть негодные скрепления:
        /// - Д65 - отсутствие 2 или более основных костилей; (TODO - учет внутренную сторону рельса)
        /// - ЖБР - остуствие гибкой клеммы
        /// - КВ65 - отсутствие клеммы
        /// - СКЛ - сломанная гибкая клемма
        /// </summary>
        /// <param name="tripId"></param>
        /// <returns></returns>
        public List<RailFastener> GetBadRailFasteners(long tripId, bool orderBySide, string pch, object trackName, int km = -1)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var txt = $@"SELECT
	                                *,
	                                round( LEAD ( koord, 1 ) OVER ( ORDER BY km ) ) next_koord,
	                                LEAD ( oid, 1 ) OVER ( ORDER BY km ) next_oid,
	                                round( ABS ( LEAD ( koord, 1 ) OVER ( ORDER BY km ) - koord ) ) razn 
                                FROM
	                                (
	                                SELECT DISTINCT
		                                concat (
			                                'ПЧ-',
			                                COALESCE ( distance.code, '' ),
			                                '/ПЧУ-',
			                                COALESCE ( pchu.code, '' ),
			                                '/ПД-',
			                                COALESCE ( pd.code, '' ),
			                                '/ПДБ-',
			                                COALESCE ( pdb.code, '' ) 
		                                ) AS pdbsection,
		                                frag.adm_track_id AS trackid,
		                                trips.ID,
		                                oid,
		                                fnum,
		                                km,
		                                mtr,
		                                rvo.file_id fileid,
		                                ms,
		                                threat_id AS threat,
                                        type_id,x,
		                                mtr * 1000.0 + (
			                                ( SELECT travel_direction FROM trips WHERE ID = {tripId} ) * ( rvo.local_fnum * 200.0 ) - 
                                            ( SELECT car_position FROM trips     WHERE ID = {tripId} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
		                                ) AS koord
	                                FROM
		                                PUBLIC.rd_video_objects AS rvo
		                                INNER JOIN trip_files AS files ON files.ID = rvo.file_id
		                                INNER JOIN trips ON trips.ID = files.trip_id
		                                INNER JOIN fragments AS frag ON frag.trip_id = trips.ID 
		                                AND (
			                                isbelong ( km, mtr, frag.final_km, frag.final_m, frag.start_km, frag.start_m ) 
			                                OR isbelong ( km, mtr, frag.start_km, frag.start_m, frag.final_km, frag.final_m ) 
		                                )
		                                INNER JOIN tpl_pdb_section AS SECTION ON km BETWEEN SECTION.start_km 
		                                AND SECTION.final_km
		                                INNER JOIN tpl_period AS period ON period.ID = SECTION.period_id 
		                                AND trips.trip_date BETWEEN period.start_date 
		                                AND period.final_date 
		                                AND period.adm_track_id = frag.adm_track_id
		                                INNER JOIN adm_pdb AS pdb ON pdb.ID = SECTION.adm_pdb_id
		                                INNER JOIN adm_pd AS pd ON pd.ID = pdb.adm_pd_id
		                                INNER JOIN adm_pchu AS pchu ON pchu.ID = pd.adm_pchu_id
		                                INNER JOIN adm_distance AS distance ON distance.ID = pchu.adm_distance_id 
	                                WHERE
		                                oid IN ({(int)VideoObjectType.D65_NoPad}, 
                                               {(int)VideoObjectType.D65_MissingSpike},
                                               {(int)VideoObjectType.KB65_NoPad},
                                               {(int)VideoObjectType.KB65_MissingClamp},
                                               {(int)VideoObjectType.SklNoPad},
                                               {(int)VideoObjectType.SklBroken},
                                               {(int)VideoObjectType.GBRNoPad},
                                               {(int)VideoObjectType.WW},
                                               {(int)VideoObjectType.P350MissingClamp},
                                               {(int)VideoObjectType.KD65NB},
                                               {(int)VideoObjectType.KppNoPad}) 
		                                AND trips.ID = {tripId} 
		                                AND distance.code = '{pch}' 
                                        {(km == -1 ? "" : $"and km={km}")}
		                                and h > 15 
		                                AND w > 15 
		                                AND x > 24 
		                                AND (y > 44 and y < 290)  
	                                ORDER BY
		                                km,
		                                mtr,
	                                koord 
	                                ) AS DATA";
                    var videoObjects = db.Query<VideoObject>(txt).ToList();
                    
                    var result = new List<RailFastener>();
                    foreach (var videoObject in videoObjects)
                    {
                        //сортировка правой стороны
                        if (videoObject.Threat == Threat.Right && videoObject.Type_id == 1 && videoObject.X > 400)
                        {
                            continue;
                        }

                        var serialized = JsonConvert.SerializeObject(videoObject);
                        RailFastener fastener = null;
                        switch ((VideoObjectType)videoObject.Oid)
                        {
                            case VideoObjectType.D65_NoPad:
                                fastener = JsonConvert.DeserializeObject<D65>(serialized);
                                fastener.AddDigression(new Digression() { DigName = DigressionName.D65_NoPad }); 
                                break;
                            case VideoObjectType.D65_MissingSpike:
                                fastener = JsonConvert.DeserializeObject<D65>(serialized);
                                fastener.AddDigression(new Digression() { DigName = DigressionName.Missing2OrMoreMainSpikes }); //отсутствие двух или более пришивочных костылей
                                break;
                            case VideoObjectType.KB65_NoPad:
                                fastener = JsonConvert.DeserializeObject<KB65>(serialized);
                                fastener.AddDigression(new Digression() { DigName = DigressionName.KB65_NoPad }); 
                                break;
                            case VideoObjectType.KB65_MissingClamp:
                                fastener = JsonConvert.DeserializeObject<KB65>(serialized);
                                fastener.AddDigression(new Digression() { DigName = DigressionName.KB65_MissingClamp }); 
                                break;
                            case VideoObjectType.SklNoPad:
                                fastener = JsonConvert.DeserializeObject<D65>(serialized);
                                fastener.AddDigression(new Digression() { DigName = DigressionName.SklNoPad });
                                break;
                            case VideoObjectType.SklBroken:
                                fastener = JsonConvert.DeserializeObject<D65>(serialized);
                                fastener.AddDigression(new Digression() { DigName = DigressionName.BrokenArsClamp });
                                break;
                            case VideoObjectType.GBRNoPad:
                                fastener = JsonConvert.DeserializeObject<GBR>(serialized);
                                fastener.AddDigression(new Digression() { DigName = DigressionName.GBRNoPad });
                                break;
                            case VideoObjectType.WW:
                                fastener = JsonConvert.DeserializeObject<GBR>(serialized);
                                fastener.AddDigression(new Digression() { DigName = DigressionName.WW });
                                break;
                            case VideoObjectType.P350MissingClamp:
                                fastener = JsonConvert.DeserializeObject<GBR>(serialized);
                                fastener.AddDigression(new Digression() { DigName = DigressionName.P350MissingClamp });
                                break;
                            case VideoObjectType.KD65NB:
                                fastener = JsonConvert.DeserializeObject<KD65>(serialized);
                                fastener.AddDigression(new Digression() { DigName = DigressionName.KD65NB });
                                break;
                            case VideoObjectType.KppNoPad:
                                fastener = JsonConvert.DeserializeObject<GBR>(serialized);
                                fastener.AddDigression(new Digression() { DigName = DigressionName.KppNoPad });
                                break;
                            default:
                                break;
                        }
                        result.Add(fastener);
                    }

                    result = result.Where(o => o.Razn > 260).ToList();

                    return result;
                }
                catch (Exception e) {

                    System.Console.WriteLine("GetBadRailFastenersEroor: "+ e.Message);
                    return new List<RailFastener>(); }
                
            }
        }
        public List<RailFastener> GetDigressions(long tripId, bool orderBySide)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var videoObjects = db.Query<VideoObject>(
                        $@"SELECT distinct concat('ПЧ-',coalesce(distance.code,''), '/ПЧУ-', coalesce(pchu.code,''), '/ПД-', coalesce(pd.code,''), '/ПДБ-', coalesce(pdb.code,'') ) as pdbsection,  frag.adm_track_id as trackid, trips.id, oid,fnum,concat(files.description, files.file_name) as filepath, km,  mtr, rvo.file_id, threat_id as threat,  
                    (max(x) - min(x)) as x, (max(y) - min(y)) as y  FROM public.rd_video_objects as rvo
                    inner join trip_files as files on files.id = rvo.file_id
					inner join trips on trips.id = files.trip_id
					inner join fragments as frag on frag.trip_id = trips.id and  (isbelong(km,mtr, frag.final_km, frag.final_m,frag.start_km, frag.start_m) or isbelong(km,mtr,frag.start_km, frag.start_m, frag.final_km, frag.final_m))
					inner join tpl_pdb_section as section on km between section.start_km and section.final_km
					inner join tpl_period as period on period.id = section.period_id and trips.trip_date between period.start_date and period.final_date and period.adm_track_id = frag.adm_track_id
					inner join adm_pdb as pdb on pdb.id = section.adm_pdb_id
					inner join adm_pd as pd on pd.id = pdb.adm_pd_id
					inner join adm_pchu as pchu on pchu.id = pd.adm_pchu_id
					inner join adm_distance as distance on distance.id = pchu.adm_distance_id
                    where oid in (16,17,18,19,25,26,29,32,35,36) and trips.id={tripId}
                    group by trips.id,  frag.adm_track_id, distance.code, pchu.code, pd.code, pdb.code, fnum, km, mtr, file_id, filepath, threat_id, oid
                    Having count(fnum)=2 and (max(x) - min(x)) < 100 and (max(y) - min(y)) < 10
                    union all
                    Select  concat('ПЧ-',coalesce(distance.code,''), '/ПЧУ-', coalesce(pchu.code,''), '/ПД-', coalesce(pd.code,''), '/ПДБ-', coalesce(pdb.code,'') ) as pdbsection,  frag1.adm_track_id as trackid, trips1.id, oid, fnum, concat(files1.description, files1.file_name) as filepath, km,  mtr, rvo1.file_id, threat_id as threat, x, y
                    FROM public.rd_video_objects as rvo1
                    inner join trip_files as files1 on files1.id = rvo1.file_id
					inner join trips as trips1 on trips1.id = files1.trip_id
					inner join fragments as frag1 on frag1.trip_id = trips1.id and  (isbelong(km,mtr, frag1.final_km, frag1.final_m,frag1.start_km, frag1.start_m) or isbelong(km,mtr,frag1.start_km, frag1.start_m, frag1.final_km, frag1.final_m))
					inner join tpl_pdb_section as section1 on km between section1.start_km and section1.final_km
					inner join tpl_period as period1 on period1.id = section1.period_id and trips1.trip_date between period1.start_date and period1.final_date and period1.adm_track_id = frag1.adm_track_id
					inner join adm_pdb as pdb on pdb.id = section1.adm_pdb_id
					inner join adm_pd as pd on pd.id = pdb.adm_pd_id
					inner join adm_pchu as pchu on pchu.id = pd.adm_pchu_id
					inner join adm_distance as distance on distance.id = pchu.adm_distance_id
                    where rvo1.oid in (16,17,18,19,25,26,29,32,35,36)  and trips1.id={tripId} " +
                        (orderBySide ? "  order by file_id, km, mtr" : " order by km, mtr, file_id")).ToList();
                    var result = new List<RailFastener>();
                    foreach (var videoObject in videoObjects)
                    {
                        var serialized = JsonConvert.SerializeObject(videoObject);
                        RailFastener fastener = null;
                        switch ((VideoObjectType)videoObject.Oid)
                        {
                            case VideoObjectType.GBRNoPad:
                                fastener = JsonConvert.DeserializeObject<GBR>(serialized);
                                fastener.AddDigression(new Digression() { DigName = DigressionName.BrokenArsClamp });
                                break;
                            default:
                                break;
                        }
                        result.Add(fastener);
                    }
                    return result;
                }
                catch (Exception e)
                {

                    System.Console.WriteLine("GetDigressionsError:" +e.Message);
                    return new List<RailFastener>();
                }

            }
        }

        public List<Sleepers> GetSleeper(long trackId, int km, int meter, int start, int final, Threat threat)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                var videoObjects = db.Query<VideoObject>($@"select rvo.oid, concat(files.description, files.file_name) as filepath, realcoord.km as km, realcoord.mtr as mtr, realcoord.mm as mm,
                    rvo.fileid, files.threat_id as threat
                    from rd_video_objects rvo
                    inner join trip_files files on files.id = rvo.fileid
                    inner join trips on trips.id = rvo.trip_id
                    inner join getrealcoords_videoobjects(rvo.km, rvo.pt, rvo.mtr, rvo.x, rvo.mm, files.coef_camera, trips.travel_direction) as realcoord on true
                    where oid in ({VideoObjectType.GBR}, {VideoObjectType.kpp}, {VideoObjectType.SKL}, {VideoObjectType.D65}, {VideoObjectType.KB65}, {VideoObjectType.KppNoPad}, {VideoObjectType.SklBroken},  {VideoObjectType.SklBroken})
                        and rvo.track_id = {trackId} and files.threat_id = {(int)threat} and rvo.km = {km} and (rvo.pt - 1) * 100 + rvo.mtr = {meter} and realcoord.mm between {start} and {final}
                    group by realcoord.km, realcoord.mtr, realcoord.mm, rvo.fileid, filepath, threat, rvo.oid
                    order by realcoord.km, realcoord.mtr, realcoord.mm, rvo.fileid").ToList();
                var result = new List<Sleepers>();
                foreach (var videoObject in videoObjects)
                {
                    result.Add(new Sleepers {
                        Km = videoObject.Km,
                        Mtr = videoObject.Mtr,
                        Mm = videoObject.Mm,
                        Threat = videoObject.Threat
                    });
                }
                return result;
            }
        }

        public List<Sleepers> GetSleepers()
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                var videoObjects = db.Query<VideoObject>($@"select rvo.oid, concat(files.description, files.file_name) as filepath, realcoord.km as km, realcoord.mtr as mtr, realcoord.mm as mm,
                    rvo.fileid, files.threat_id as threat
                    from rd_video_objects rvo
                    inner join trip_files files on files.id = rvo.fileid
                    inner join trips on trips.id = rvo.trip_id
                    inner join getrealcoords_videoobjects(rvo.km, rvo.pt, rvo.mtr, rvo.x, rvo.mm, files.coef_camera, trips.travel_direction) as realcoord on true
                    where oid in ({(int)VideoObjectType.GBR}, {(int)VideoObjectType.kpp}, {(int)VideoObjectType.SKL}, {(int)VideoObjectType.D65}, {(int)VideoObjectType.KB65_MissingClamp}, {(int)VideoObjectType.KB65}, {(int)VideoObjectType.kpp}, {(int)VideoObjectType.SklBroken}, {(int)VideoObjectType.KppNoPad})
                    group by realcoord.km, realcoord.mtr, realcoord.mm, rvo.fileid, filepath, threat, rvo.oid
                    order by realcoord.km, realcoord.mtr, realcoord.mm, rvo.fileid").ToList();
                var result = new List<Sleepers>();
                foreach (var videoObject in videoObjects)
                {
                    result.Add(new Sleepers {
                        Km = videoObject.Km,
                        Mtr = videoObject.Mtr,
                        Mm = videoObject.Mm,
                        Threat = videoObject.Threat
                    });
                }
                return result;
            }
        }
		
		public CurveParams GetCurveParams(long curveId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<CurveParams>(@"select coalesce(btype.name, 'нет данных') as fastening, coalesce(ctype.name, 'нет данных') as brace from apr_curve curve
                    inner join tpl_period cperiod on cperiod.id = curve.period_id
                    left join tpl_period bperiod on bperiod.adm_track_id = cperiod.adm_track_id
                    left join apr_rails_braces braces on braces.period_id = bperiod.id and isbelong(curve.start_km, curve.start_m, braces.start_km, braces.start_m, braces.final_km, braces.final_m) and isbelong(curve.final_km, curve.final_m, braces.start_km, braces.start_m, braces.final_km, braces.final_m)
                    left join cat_brace_type btype on btype.id = braces.brace_type_id
                    left join apr_crosstie crosstie on crosstie.period_id = bperiod.id and isbelong(curve.start_km, curve.start_m, crosstie.start_km, crosstie.start_m, crosstie.final_km, crosstie.final_m) and isbelong(curve.final_km, curve.final_m, crosstie.start_km, crosstie.start_m, crosstie.final_km, crosstie.final_m)
                    left join cat_crosstie_type ctype on ctype.id = crosstie.crosstie_type_id
                    where curve.id = @curveId", new { curveId }, commandType: CommandType.Text).FirstOrDefault();
            }
        }

        public SiteInfo GetSiteInfo(long trackId, int startKm, int finalKm)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<SiteInfo>(@"select coalesce(station.name, 'Неизвестный') as StationStart, coalesce(station2.name, 'Неизвестный') as StationFinal, (stationSection.axis_km - @start_km) as st1, (stationSection2.axis_km - @final_km) as st2
                    from adm_track track
                    inner join tpl_period stationPeriod on stationPeriod.adm_track_id = track.id 
                    left join tpl_station_section stationSection on stationSection.period_id = stationPeriod.id and (stationSection.axis_km - @start_km) < 1
                    left join adm_station station on station.id = stationSection.station_id 
                    left join tpl_station_section stationSection2 on stationSection2.period_id = stationPeriod.id and (stationSection2.axis_km - @final_km) > -1
                    left join adm_station station2 on station2.id = stationSection2.station_id
                    where track.id=@track_id order by track.id, st1, st2 limit 1", new { track_id = trackId, start_km = startKm, final_km = finalKm }, commandType: CommandType.Text).FirstOrDefault();
            }
        }

        public List<AdmTrack> GetTracksOnTrip(long tripId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<AdmTrack>($@"select track.*, False as Accept, coalesce(track.adm_direction_id, -1) as Parent_id from adm_track track
                    inner join trips on trips.id = {tripId}
                    inner join adm_direction direction on direction.id = track.adm_direction_id and direction.id = trips.direction_id
                    order by track.code", commandType: CommandType.Text).ToList();
            }
        }
        public List<AdmTrack> GetTracks (long distanceId, ReportPeriod period,long trackId= -1)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                var tracks =  db.Query<AdmTrack>($@"
                        SELECT
	                        track.*,
	                        FALSE AS Accept,
	                        COALESCE ( track.adm_direction_id, - 1 ) AS Parent_id,
	                        ad.NAME || ' (' || ad.code || ')' direction 
                        FROM
	                        tpl_dist_section
	                        INNER JOIN tpl_period period ON period.ID = tpl_dist_section.period_id
	                        INNER JOIN adm_track track ON track.ID = period.adm_track_id
	                        left JOIN adm_direction ad ON ad.ID = track.adm_direction_id 
                        WHERE
	                        adm_distance_id = {distanceId}  
	                        AND daterange ( period.start_date, period.final_date ) && daterange ( '{period.StartDate:dd.MM.yyyy}', '{period.FinishDate:dd.MM.yyyy}' ) 
                        ORDER BY
	                        track.ID", commandType: CommandType.Text).ToList();

                return tracks;
            }
        }

        public List<Trips> GetTripsOnDistance(long distanceId, ReportPeriod period)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {

                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<Trips>($@"SELECT
	                    trips.*,
	                    direction.NAME AS direction,
	                    direction.code AS directioncode
                        
                    FROM
	                    trips
	                    INNER JOIN adm_direction direction ON direction.ID = trips.direction_id
	                    INNER JOIN adm_track track ON track.adm_direction_id = direction.
	                    ID INNER JOIN tpl_period dperiod ON dperiod.adm_track_id = track.
	                    ID INNER JOIN tpl_dist_section dsection ON dsection.period_id = dperiod.ID 
                    where dsection.adm_distance_id = {distanceId} and trips.trip_date between '{period.StartDate:dd-MM-yyyy}' and '{period.FinishDate:dd-MM-yyyy}'
                    group by trips.id, direction.id
                    order by trips.trip_date DESC
                	", commandType: CommandType.Text).ToList();


                //убрал direction code для работы с ФПО
                //if (db.State == ConnectionState.Closed)
                //    db.Open();

                //return db.Query<Trips>($@"select trips.*, (direction.name ) as direction from trips
                //    inner join adm_direction direction on direction.id = trips.direction_id
                //    inner join adm_track track on track.adm_direction_id = direction.id
                //    inner join tpl_period dperiod on dperiod.adm_track_id = track.id
                //    inner join tpl_dist_section dsection on dsection.period_id = dperiod.id
                //    where dsection.adm_distance_id = {distanceId} and trips.trip_date between '{period.StartDate.ToString("dd-MM-yyyy")}' and '{period.FinishDate.ToString("dd-MM-yyyy")}'
                //    group by trips.id, direction.id
                //    order by trips.trip_date", commandType: CommandType.Text).ToList();
            }
        }

        public List<Curve> GetCurvesAsTripElems(long trackId, DateTime date, int start_km, int start_m, int final_km, int final_m)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<Curve>($@"select curve.*, side.name as side from apr_curve curve
                    inner join cat_side side on side.id = curve.side_id
                    inner join tpl_period cperiod on cperiod.id = curve.period_id and ('{date:dd-MM-yyyy}' between cperiod.start_date and cperiod.final_date)
                    inner join adm_track track on track.id = cperiod.adm_track_id and track.id = {trackId}
                    where (curve.start_km * 1000 + curve.start_m between {start_km * 1000 + start_m} and {final_km * 1000 + final_m})
                        and (curve.final_km * 1000 + curve.final_m between {start_km * 1000 + start_m} and {final_km * 1000 + final_m})
                    order by curve.start_km * 1000 + curve.start_m, curve.final_km * 1000 + curve.final_m", commandType: CommandType.Text).ToList();
            }
        }
        public object GetTripSections(long trackId, DateTime date, int type)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                switch (type)
                {
                    case MainTrackStructureConst.MtoLongRails:
                        return db.Query<LongRails>($@"select longrails.* from apr_long_rails longrails
                            inner join tpl_period lperiod on lperiod.id = longrails.period_id and '{date:dd-MM-yyyy}' between lperiod.start_date and lperiod.final_date
                            inner join cat_longrails catrails on catrails.id = longrails.type_id and catrails.id = 3
                            --where lperiod.adm_track_id = {trackId} --limit 2",
                            commandType: CommandType.Text).ToList();
                    default:
                        return null;
                }
            }
        }

        public List<RdIrregularity> GetCountByPiket(long Trip_id, int type)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                switch (type)
                {
                    case MainTrackStructureConst.GetCountByPiket:
                        return db.Query<RdIrregularity>($@"SELECT COALESCE
	                                                        ( skrep_def.km, skrep_negod.km, shpal_def.km, shpal_negod.km, limit_fast.km ) km,
	                                                        COALESCE ( skrep_def.pt, skrep_negod.pt, shpal_def.pt, shpal_negod.pt, limit_fast.mtr / 100+1 ) piket,
	                                                        skrep_def.cn skrep_def,
	                                                        skrep_negod.cn skrep_negod,
	                                                        shpal_def.cn shpal_def,
	                                                        shpal_negod.cn shpal_negod,
	                                                        otst,
	                                                        kns,
	                                                        vdop,
	                                                        COALESCE ( skrep_def.trip_id, skrep_negod.trip_id, shpal_def.trip_id, shpal_negod.trip_id ) trip_id 
                                                        FROM
	                                                        skrep_def
	                                                        FULL OUTER JOIN skrep_negod ON skrep_def.km = skrep_negod.km 
	                                                        AND skrep_def.pt = skrep_negod.pt 
	                                                        AND skrep_def.trip_id = skrep_negod.trip_id
	                                                        FULL OUTER JOIN shpal_def ON skrep_def.km = shpal_def.km 
	                                                        AND skrep_def.pt = shpal_def.pt 
	                                                        AND skrep_def.trip_id = shpal_def.trip_id
	                                                        FULL OUTER JOIN shpal_negod ON skrep_def.km = shpal_negod.km 
	                                                        AND skrep_def.pt = shpal_negod.pt 
	                                                        AND skrep_def.trip_id = shpal_negod.trip_id
	                                                        FULL OUTER JOIN report_speedlimfastening limit_fast ON skrep_def.km = limit_fast.km 
	                                                        AND skrep_def.pt = limit_fast.mtr / 100+1 
	                                                        AND skrep_def.trip_id = limit_fast.trip_id 
                                                        WHERE
	                                                        COALESCE ( skrep_def.trip_id, skrep_negod.trip_id, shpal_def.trip_id, shpal_negod.trip_id, limit_fast.trip_id ) = {Trip_id} 
                                                        ORDER BY
	                                                        km,
	                                                        piket",
                            commandType: CommandType.Text).ToList();
                    default:
                        return null;
                }
            }
        }

        public object GetTripSections(long trackId, DateTime date, int start_km, int start_m, int final_km, int final_m, int type)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                
                switch (type)
                {
                    case MainTrackStructureConst.MtoRailsBrace:
                        return db.Query<RailsBrace>($@"select braces.*, btype.name as brace_type from apr_rails_braces braces
                            inner join cat_brace_type btype on btype.id = braces.brace_type_id
                            inner join tpl_period speriod on speriod.id = braces.period_id and '{date:dd-MM-yyyy}' between speriod.start_date and speriod.final_date
                            inner join adm_track track on track.id = speriod.adm_track_id and track.id = {trackId}
                            where (braces.start_km * 1000 + braces.start_m < {final_km * 1000 + final_m}) and (braces.final_km * 1000 + braces.final_m > {start_km * 1000 + start_m})",
                            commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoRailSection:
                        return db.Query<RailsSections>($@"select rsection.*, rtype.name as type from apr_rails_sections rsection
                            inner join cat_rails_type rtype on rtype.id = rsection.type_id
                            inner join tpl_period speriod on speriod.id = rsection.period_id and '{date:dd-MM-yyyy}' between speriod.start_date and speriod.final_date
                            inner join adm_track track on track.id = speriod.adm_track_id and track.id = {trackId}
                            where (rsection.start_km * 1000 + rsection.start_m < {final_km * 1000 + final_m}) and (rsection.final_km * 1000 + rsection.final_m > {start_km * 1000 + start_m})",
                            commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoSpeed:
                        return db.Query<Speed>($@"select speed.* from apr_speed speed
                            inner join tpl_period speriod on speriod.id = speed.period_id and '{date:dd-MM-yyyy}' between speriod.start_date and speriod.final_date
                            inner join adm_track track on track.id = speriod.adm_track_id and track.id = {trackId}
                            where (speed.start_km * 1000 + speed.start_m < {final_km * 1000 + final_m}) and (speed.final_km * 1000 + speed.final_m > {start_km * 1000 + start_m})",
                            commandType: CommandType.Text).ToList();
                    case MainTrackStructureConst.MtoCurve:
                        return db.Query<Curve>($@"select curve.*, max(stcurve.radius) as maxradius from apr_curve curve
                            inner join apr_stcurve stcurve on stcurve.curve_id = curve.id
                            inner join tpl_period speriod on speriod.id = curve.period_id and '{date:dd-MM-yyyy}' between speriod.start_date and speriod.final_date
                            inner join adm_track track on track.id = speriod.adm_track_id and track.id = {trackId}
                            where (curve.start_km * 1000 + curve.start_m < {final_km * 1000 + final_m}) and (curve.final_km * 1000 + curve.final_m > {start_km * 1000 + start_m})
                            group by curve.id, stcurve.id",
                            commandType: CommandType.Text).ToList();
                    default:
                        return null;
                }
            }
        }

        public Trips GetTrip(long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Trips>($@"select * from trips
                    where trips.id = {trip_id}", commandType: CommandType.Text).First();
            }
        }

        public int FinishProcessing(long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    db.Execute($@"UPDATE trips SET processed = true WHERE id = {trip_id}", commandType: CommandType.Text);
                    return 1;
                }
                catch (Exception e)
                {
                    return -1;
                }
            }
        }
        public List<Kilometer> GetBedemostKilometers()
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var res= db.Query<Kilometer>($@"select km as number, pch as pchcode, pchu as pchucode, pd as pdcode, pdb as pdbcode, ots_iv_st as speedlim, primech as primech, put as track_name, rating as Rating_bedomost from bedemost", commandType: CommandType.Text).ToList();
                    return res;
                }
                catch (Exception e)
                {
                    return null;
                }
            }

        }
        public List<OutData> GetNextOutDatas(int meter, int count, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {

                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<OutData>($@"
                select 
                    ROW_NUMBER( ) OVER ( ) x,
                    outdata.id, speed, km, meter, gauge, x101_kupe, x102_koridor, y101_kupe, y102_koridor, gauge_correction, level*trip.car_position as level, 
	                level_correction, stright_left*trip.car_position as stright_left, stright_right*trip.car_position as stright_right, stright_avg*trip.car_position as stright_avg, 
	                stright_avg_70*trip.car_position stright_avg_70, stright_avg_100*trip.car_position stright_avg_100, stright_avg_120*trip.car_position as stright_avg_120, 
	                stright_avg_150*trip.car_position as stright_avg_150, drawdown_left, drawdown_right, _meters, level_avg*trip.car_position as level_avg, level_avg_70*trip.car_position as level_avg_70, 
	                level_avg_100*trip.car_position as level_avg_70, level_avg_120 *trip.car_position as level_avg_120, level_avg_150*trip.car_position as level_avg_150, drawdown_avg, drawdown_avg_70, drawdown_avg_100, 		   drawdown_avg_120, drawdown_avg_150, drawdown_left_sko, drawdown_right_sko, level_sko, skewness_pxi, skewness_sko, sssp_before, sssp_speed, latitude, longitude, heigth, level_zero*trip.car_position as level_zero, enc_on_meter_begin, val01, val02, val03, val04, val05, val06, val07, val08, val09, val10, level1, level2, level3, level4, level5, stright1, stright2, stright3, stright4, stright5, trip_id, rail_temp_kupe, rail_temp_koridor, ambient_temp, accelerometer_y_axis, correction 
                from outdata_{trip_id} outdata
                inner join trips trip on trip.id = outdata.trip_id
                where _meters > {meter}  order by id limit {count}
                ").ToList();
                //return db.Query<OutData>($@"select * from outdata_49_20200625_201801 where id > {meter}  order by id limit {count}").ToList();
            }
        }

        public int GetDistanceBetweenCoordinates(int start_km, int start_m, int final_km, int final_m, long track_id, DateTime trip_date)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.QueryFirst<int>($@"select * from getDistanceFrom({start_km},{start_m},{final_km}, {final_m}, {track_id}, {trip_date}");
            }

        }
        public ReportTemplate GetReportTemplate(string className)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                var result = db.QueryFirst<ReportTemplate>($"Select * from CAT_REPORT_TEMPLATE where classname = '{className}'");
                return result;
            }
        }

        public void ClearBedemost(long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                db.Execute($"Delete from s3 where trip_id = '{trip_id}'");
                db.Execute($"Delete from bedemost where trip_id = '{trip_id}'");
          

            }
        }
        public List<Trips> GetTrips(int count = 10)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Trips>($@"SELECT
	                                        trips.*,
	                                        start_st.NAME AS Start_station_name,
	                                        final_st.NAME AS Final_station_name,
	                                        direction.NAME AS direction,
	                                        direction.code AS directioncode 
                                        FROM
	                                        trips
	                                        INNER JOIN adm_station AS start_st ON start_st.ID = start_station
	                                        INNER JOIN adm_station AS final_st ON final_st.ID = final_station
	                                        INNER JOIN adm_direction direction ON direction.ID = trips.direction_id 
                                        ORDER BY
	                                        trip_date DESC limit {count}", commandType: CommandType.Text).ToList();
            }
        }
        public int InsertKilometer(Kilometer km)
        {

            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (km._Meters.Count == 0)
                    return -1;
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    db.Execute($"Delete from kilometers where track_id = {km.Track_id} and num = {km.Number}");
                    km.Id =  db.QueryFirst<int>($@"
                    INSERT INTO kilometers(trip_id, direction_id, length, track_id, passage_time, start, final, start_index, final_index, num, correctionvalue, correctionmeter, correctiontype)
	                VALUES (@Trip_id, @Direction_id, @Length, @Track_Id, @Ptime, @Start_m, @Final_m, @Si, @Fi,@Num, {km.CorrectionValue}, {km.CorrectionMeter}, {(int)km.CorrectionType}) RETURNING id",
                    new { Trip_Id = km.Trip.Id, Direction_id = km.Trip.Direction_id, Length = km.GetLength(), Track_Id = km.Track_id, Ptime = km.Passage_time,
                        Start_m = km.Start_m, Final_m = km.Final_m, Si = km._Meters[0], Fi = km._Meters[km._Meters.Count - 1], Num = km.Number
                    }

                    );
                    return km.Id;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("InsertKilometerError:" +e.Message);
                    return -1;
                }

            }
        }

        public List<Digression> GetAdditional(int km)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    return db.Query<Digression>($@"
                    SELECT *
                    FROM s3_additional 
                    WHERE
	                    km = {km}
                    ORDER BY
	                    s3_additional.km,
	                    s3_additional.meter").ToList();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("getAdditionalError:" + e.StackTrace);
                    return null;
                }
            }
        }

        public List<DigressionMark> GetDigressionMarks(long trip_id, int km, long track_id, int[] type)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                
                   

                string sql = $@" 
                    SELECT distinct
                        s3.id,
	                    s3.km,
                        s3.ots as digname,
	                    s3.meter,
	                    s3.typ AS DEGREE,
	                    s3.len AS LENGTH,
	                    s3.otkl AS VALUE,
	                    s3.kol AS COUNT,
                        s3.onswitch,
                        s3.track_id as trackId,
                        s3.trip_id as tripId,
	                    replace(concat ( s3.ovp, '/', s3.ogp ), '-1', '-') AS LimitSpeed,
                        s3.ovp as passengerspeedlimit,
                        s3.ogp as freightspeedlimit, 
                        s3.isadditional as IsAdditional, 
                        s3.fileid as FileId, 
                        s3.primech as comment,
                        s3.ms as Ms, 
                        s3.fnum as FNum, s3.reptype as RepType, s3.carposition as CarPosition
                    FROM
	                    s3 
                    WHERE
	                    trip_id = {trip_id} and s3.typ in ({string.Join(",", type)}) and s3.km = {km} and s3.onSwitch = false AND s3.ots NOT LIKE'%Крив%'
                    ORDER BY
	                    s3.km,
	                    s3.meter";

                //Console.Out.WriteLine("SQL == > "+sql);
                return db.Query<DigressionMark>(sql, commandType: CommandType.Text).ToList();
            }
        }

        public List<Gap> GetGaps(long tripId, GapSource source, int km)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string sql ="";
                switch (source)
                {
                    case GapSource.Laser:
                        sql = $"select * from surfacegap_153 where km={km}";
                        break;
                    default:
                        break;

                }
                return db.Query<Gap>(sql, commandType: CommandType.Text).ToList();
            }
        }

        public List<RdProfile> getRefrenceData(Dictionary<String, Object> p)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                try
                {
                    //track id haitken kunde hoyu kerek jane 60 km di alip tastagan teris startKm jane finalKm nen !!!!
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    string tripDate = (string)p["tripDate"];
                    long trackId = (long)p["trackId"];
                    int startKm = (int)p["startKm"];
                    int finalKm = (int)p["finalKm"];

                    string sql = $@" 
                            select 
                                rpoint.km as Km,
                                rpoint.meter as M,
                                rpoint.mark as Y

                            from apr_ref_point rpoint 
                                inner join tpl_period as period on period.id = rpoint.period_id and '{tripDate}' between period.start_date and period.final_date
                                order by km ";

                    //Console.Out.WriteLine("SQL == > "+sql);
                    return db.Query<RdProfile>(sql, commandType: CommandType.Text).ToList();

                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("getRefrenceDataError:" + e.StackTrace);
                    return null;
                }
            }
        }
        public void RunRvoDataInsert()
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string sql = "Select * from rd_rvo_kilometer where processed = 0  order by id desc";
                var kms = db.Query<RvoKilometer>(sql, commandType: CommandType.Text).ToList();
                foreach (var km in kms)
                {
                    
                    try
                    {
                        if (db.Execute("COPY rd_video_objects (oid, fnum, km, pt, mtr, x, y, w, h, prb, ms, file_id, local_fnum, km_index) FROM '" + km.File_Path + "' DELIMITER ',' ") > 0) ;

                        db.Execute($"Update rd_rvo_kilometer set processed = 1 where id = {km.Id}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("RunRvoDataInsert1Error:" + e.Message);
                        try
                        {
                            if (db.Execute("COPY rd_video_objects (oid, fnum, km, pt, mtr, x, y, w, h, prb, ms, file_id, local_fnum) FROM '" + @"G:\00000153\" + km.File_Path.Split('\\')[km.File_Path.Split('\\').Length - 1] + "' DELIMITER ',' ") > 0) ;

                            db.Execute($"Update rd_rvo_kilometer set processed = 1 where id = {km.Id}");
                        }
                        catch(Exception e1) {
                            Console.WriteLine("RunRvoDataInsert2Error:" + e1.Message);
                            db.Execute($"Update rd_rvo_kilometer set processed = 1 where id = {km.Id}");
                        }
                    }
                }
            }
        }

        public long[] RunRvoDataInsert(int km, long fileID, string filePath)
        {

            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    string sql =   "COPY rd_video_objects (oid, fnum, km, pt, mtr, x, y, w, h, prb, ms, file_id, local_fnum, km_index) FROM '" + filePath + "' DELIMITER ',' ";
                    if (db.Execute("COPY rd_video_objects (oid, fnum, km, pt, mtr, x, y, w, h, prb, ms, file_id, local_fnum, km_index) FROM '" + filePath + "' DELIMITER ',' ") > 0) 
                        db.Execute($"Update rd_rvo_kilometer set processed = 1 where file_id = {fileID} and km = {km}");
                        List<dynamic> res = db.Query($@"
                        select distinct rdkm.file_id, group_id from rd_rvo_kilometer rdkm
                        left join trip_files f on f.id = rdkm.file_id
                        where km = {km} and trip_id = (select trip_id from trip_files where id = {fileID}) and processed = 1 and group_id = (select group_id from rd_rvo_kilometer where km = 147 and file_id = {fileID})
                        ").ToList();
                    var result = new long[res.Count+1];
                    if (res.Count == 0)
                        return null;
                    result[0] = res[0].group_id;
                    foreach(var d in res)
                    {
                        result[res.IndexOf(d) + 1] = d.file_id;
                    }
                    return result;
                        
                                     
                }
                catch (Exception e)
                {
                    Console.WriteLine("RunRvoDataInsert1Error:" + e.Message);
                    return null;
                }
            }
        }

        public void SendEkasuiData(Trips trip, int km)
        {

            MainTrackStructureRepository MainTrackStructureService = new MainTrackStructureRepository();
            RdStructureRepository RdStructureService = new RdStructureRepository();
            List<S3> s3list = RdStructureService.GetS3ForKm(trip.Id, km) as List<S3>;
            XDocument gParamaters = new XDocument();
            XElement gOutFile = new XElement("OutFile", new XAttribute("nsiver", DateTime.Now.ToString("dd.MM.yyyy")));

            bool s3Exists = s3list.Count > 0;

            if (s3Exists)
            {
                foreach (S3 s3 in s3list)
                {
                    string defectID = String.Empty;
                    switch (s3.Ots)
                    {
                        case "Р":
                        case "Р3м":
                        case "Р м":
                            defectID = "090004000303";
                            break;
                        case "Р.н":
                        case "Рн3м":
                        case "Рн м":
                            defectID = "090004015377";
                            break;
                        case "Уш":
                            defectID = "090004012176";
                            break;
                        case "Суж":
                            defectID = "090004012174";
                            break;
                        case "ОШК":
                            defectID = "090004000294";
                            break;
                        case "УУ":
                            defectID = "090004015317";
                            break;
                        case "Пр.п":
                        case "Пр.п3м":
                        case "Пр.п м":
                            defectID = "090004012681";
                            break;
                        case "Пр.л":
                        case "Пр.л3м":
                        case "Пр.л м":
                            defectID = "090004012166";
                            break;
                        case "У":
                        case "У3м":
                        case "У м":
                            defectID = "090004012169";
                            break;
                        case "П":
                        case "П3м":
                        case "П м":
                            defectID = "090004012168";
                            break;
                        //устр.кривых
                        case "Анп":
                            defectID = "090004007837";
                            break;
                        case "Укл":
                            defectID = "090004012154";
                            break;
                        case "Пси":
                            defectID = "090004007838";
                            break;
                        //сочетания
                        case "Ш10":
                            defectID = "090004007820";
                            break;
                    }

                    var sector = MainTrackStructureService.GetSector(s3.Track_id, s3.Km, trip.Trip_date);
                    var station_section = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, s3.Km, MainTrackStructureConst.MtoStationSection, s3.Track_id) as List<StationSection>;
                    var pdb_section = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, s3.Km, MainTrackStructureConst.MtoPdbSection, s3.Track_id) as List<PdbSection>;
                    XElement gInsident = new XElement("Incident",
                        new XAttribute("REC", s3list.IndexOf(s3)),
                        new XAttribute("kod_otstup", !defectID.Equals(String.Empty) ? defectID : "n"),
                        new XAttribute("kod_napr", s3.Directcode),
                        new XAttribute("kod_st", station_section.Count > 0 ? station_section[0].Station : "n"),
                        new XAttribute("nzhs", pdb_section.Count > 0 ? pdb_section[0].Nod : "n"),
                        new XAttribute("npch", s3.Pch),
                        new XAttribute("nput", s3.Put),
                        new XAttribute("date", trip.Trip_date.ToString("dd.MM.yyyy")),
                        new XAttribute("time", trip.Trip_date.ToString("hh.mm.ss")),
                        new XAttribute("temp_vozd", "20"),
                        new XAttribute("temp_rels", "30"),
                        new XAttribute("nomer_mdk", "ALARMDK"),
                        new XAttribute("avtor", trip.Chief.Trim()),
                        new XAttribute("km", s3.Km),
                        new XAttribute("pk", s3.Meter / 100 + 1),
                        new XAttribute("metr", s3.Meter % 100),
                        new XAttribute("dlina_ots", s3.Len),
                        new XAttribute("velich_ots", s3.Otkl),
                        new XAttribute("stepen_ots", s3.Ots),
                        new XAttribute("kol_ots", s3.Kol),
                        new XAttribute("speed_ogr", s3.Ovp != -1 ? s3.Ovp : 0)

                        );

                    gOutFile.Add(gInsident);

                }


                gParamaters.Add(gOutFile);
                string fileName = $@"G:\sntfi\ekasui\G24543_{trip.Trip_date:ddMMyyyy}_{km}.xml";
                gParamaters.Save(fileName);
                if (File.Exists(fileName))
                    try
                    {

                        // Credentials
                        var credentials = new NetworkCredential("berikboltt@gmail.com", "@2019Byerikbol");

                        // Mail message
                        var message
                            = new MailMessage()
                            {
                                From = new MailAddress("someEmail@gmail.com"),
                                Subject = "Test email."

                            };
                        //message.To.Add(new MailAddress("baliev-rus@mail.ru"));
                        //message.To.Add(new MailAddress("mussin_m@s.kz"));
                        message.To.Add(new MailAddress("aset.durmagambet@gmail.com"));
                        // Add subject.
                        message.Subject = "Данные ЕК АСУИ";

                        // Add HTML body with CID embeded image.
                        string cid = "image001@gembox.com";
                        message.Body = " для тестирования АСУ ";

                        // Add image as inline attachment.
                        //message.Attachments.Add(new Attachment("%#gembox-email-logo.png%") { ContentId = cid });

                        // Add file as attachment.
                        message.Attachments.Add(new Attachment(fileName));

                        // Smtp client
                        var client = new SmtpClient()
                        {
                            Port = 587,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Host = "smtp.gmail.com",
                            EnableSsl = true,
                            Credentials = credentials
                        };

                        // Send it...         
                        client.Send(message);
                    }
                    catch (Exception)
                    {
                        Console.Error.WriteLine("Error in sending email");
                        
                        
                    }

            }

        }
        public List<RDCurve> GetRDCurves(long curve_id, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<RDCurve>($@"
                            SELECT *,
                            ROW_NUMBER ( ) OVER ( ORDER BY rdc.km, rdc.M ) AS x
                            FROM

                            (SELECT DISTINCT
	                            trip_id AS process_id,
	                            radius,
	                            LEVEL,
	                            gauge,
	                            passboost,
	                            freightboost,
	                            passboost_anp,
	                            freightboost_anp,
	                            passspeed,
	                            freightspeed,
	                            km,
	                            M,
	
	                            point_level,
	                            point_str,
	                            trapez_level,
	                            trapez_str,
	                            avg_level,
	                            avg_str 
                            FROM
	                            RD_CURVE AS rdc 
                            WHERE
	                            rdc.curve_id = {curve_id} and trip_id = {trip_id}
                            ORDER BY
	                            rdc.km,
	                            rdc.M) rdc").ToList();
            }
        }

        public List<string> GetProfileFilePath(long trip_id = -1)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<String>($@"
                      select file_name from trip_files where trip_id in ( {(trip_id> -1 ? trip_id.ToString() : "select id from trips where current order by id desc limit")} and description in ('Vnutr. profil, koridor_Caliblovany', 'Vnutr. profil, kupe_Caliblovany') order by id desc, threat_id asc limit 2").ToList();
            }      
        }

        public List<Curve> GetCurvesInTrip(long tripId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                var curves =  db.Query<Curve>($@"
                        SELECT
	                        cs.NAME AS Side,
	                        acu.* ,
	                        row_number() over(ORDER BY acu.id) num
                        FROM
	                        APR_CURVE AS acu
	                        INNER JOIN CAT_SIDE AS cs ON cs.ID = acu.SIDE_ID 
                        WHERE
	                        acu.ID IN ( SELECT DISTINCT curve_id FROM rd_curve WHERE trip_id = {tripId} ) 
                        ORDER BY
	                        acu.start_km,
	                        acu.start_m ").ToList();

                foreach (var curve in curves)
                {
                    curve.Straightenings = db.Query<StCurve>($@"
                             select stcurve.*, 
                                getcoordbylen(stcurve.start_km, stcurve.start_m, stcurve.transition_1, period.adm_track_id, ( SELECT trip_date FROM trips WHERE ID = {tripId} )) as firsttransitionend,
                                getcoordbylen(stcurve.final_km, stcurve.final_m, -stcurve.transition_2, period.adm_track_id, ( SELECT trip_date FROM trips WHERE ID = {tripId} )) as secondtransitionstart
                             from apr_stcurve stcurve
                                inner join apr_curve curve on curve.id = stcurve.curve_id
                                inner join tpl_period period on period.id = curve.period_id
                             where curve.id = {curve.Id}
                                order by stcurve.start_km * 10000 + stcurve.start_m").ToList();

                    curve.Elevations = db.Query<ElCurve>($@"
                            select elcurve.*,
                                getcoordbylen(elcurve.start_km, elcurve.start_m, elcurve.transition_1, period.adm_track_id, ( SELECT trip_date FROM trips WHERE ID = {tripId} )) as firsttransitionend,
                                getcoordbylen(elcurve.final_km, elcurve.final_m, -elcurve.transition_2, period.adm_track_id, ( SELECT trip_date FROM trips WHERE ID = {tripId} )) as secondtransitionstart
                             from apr_elcurve elcurve
                                inner join apr_curve curve on curve.id = elcurve.curve_id
                                inner join tpl_period period on period.id = curve.period_id
                                where curve.id = {curve.Id}
                                order by elcurve.start_km * 10000 + elcurve.start_m").ToList();
                }
                return curves;
            }
        }

        public void CloseTrip()
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                 db.Execute(@"
                    UPDATE trips
 	                SET current=false
                    WHERE current", commandType: CommandType.Text);
             }
        }
        public List<CheckSection> CheckVerify(long trip_id, int start, int final)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<CheckSection>($@"SELECT
	                                                    km,
	                                                    AVG ( LEVEL ) AS trip_mo_level,
	                                                    STDDEV_POP ( LEVEL ) AS trip_sko_level,
	                                                    AVG ( gauge ) AS trip_mo_gauge,
	                                                    STDDEV_POP ( gauge ) AS trip_sko_gauge 
                                                    FROM
	                                                    (SELECT
	                                                    km,
	                                                    meter,
	                                                    LEVEL,
	                                                    gauge 
                                                    FROM
	                                                    outdata_{trip_id} 
                                                    WHERE
	                                                    km * 1000+meter >= {start} 
	                                                    AND km * 1000+meter <= {final} 
                                                    ORDER BY
	                                                    km * 1000+meter) data
                                                    GROUP BY
	                                                    km").ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public void InsertEscort(Escort escort, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                db.Execute($@"
                        INSERT INTO public.escort(
                            trip_id, distance_id, fullname
                        )
                        VALUES ({trip_id}, {escort.Distance_Id}, '{escort.FullName}');
                    ");
            }
        }
        public List<Escort> GetEscorts(long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Escort>($@"
                Select escort.*, distance.name Distance_Name, true as saved from escort
                inner join adm_distance distance on distance.id = escort.distance_id where trip_id= {trip_id}").ToList();
            }
        }
        /// <summary>
        /// Тапқан ескертпелерді қайтарады
        /// </summary>
        /// <param name="trip_id">саяхат идентификаторы</param>
        /// <param name="track_id">жол идентификаторы</param>
        /// <param name="km">километр нөмірі</param>
        /// <returns>ескертпелер</returns>
        public List<DigressionMark> GetDigressionMarks(long trip_id, long track_id, int km)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<DigressionMark>($@"
                SELECT DISTINCT  
	 
                min(id) as id, track_id as trackid, trip_id as tripId, km, meter, typ as degree, len length, otkl as value, kol as count, ots as digname, ots as digression, ovp as passengerspeedlimit, ogp as freightspeedlimit, 
                uv as passengerspeedallow, uvg as freightspeedallow, is2to3, isequalto3, isequalto4, onswitch, islong, primech as comment,
	            CASE
                WHEN primech='Натурная кривая' THEN ots
                ELSE ''
                END 
                AS alert
                from s3 where trip_id = {trip_id} and track_id = {track_id} and km = {km}   and 
	            typ > 1
                GROUP BY track_id, trip_id, km, meter, typ, len, otkl, kol, ots, ovp, ogp, uv, uvg, is2to3, isequalto3, isequalto4, onswitch, islong, comment
                ORDER BY
	                meter").ToList();
            }
        }
        //хочу вставить новую переменную для изменения примечания после коретировки
        //,newbedomost
        /// <summary>
        /// Километрдің қорытынды бағасын қайтарады
        /// </summary>
        /// <param name="trip_id">саяхат идентификаторы</param>
        /// <param name="track_id">жолдың идентификаторы</param>
        /// <param name="km">км нөмірі</param>
        /// <returns>КМ шектеуі, бағысы (баллмен)</returns>
        public Dictionary<string, object> GetBedemost(long trip_id, long track_id, int km)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                var row = db.ExecuteReader($"select distinct ots_iv_st as lim, ball from bedemost where trip_id = {trip_id} and track_id = {track_id} and km = {km}");
                while (row.Read())
                {
                    result.Add("limit", row.GetValue(0));
                    result.Add("ball", row.GetValue(1));
                }
                
            }
            return result;
        }
        public string GetPrimech(DigressionMark digression)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                NpgsqlTransaction transaction = (NpgsqlTransaction)db.BeginTransaction();
                var command = new NpgsqlCommand();
                command.Connection = (NpgsqlConnection)db;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = $@"SELECT primech
                    FROM bedemost WHERE bedemost.trip_id = {digression.TripId} AND bedemost.put = '{digression.TrackName}' AND bedemost.km = {digression.Km}";
                    return (string)command.ExecuteScalar() ?? "";
                }
                catch (Exception e)
                {
                    return "";
                }
                finally
                {
                    db.Close();
                }
            }
        }
                /// <summary>
                ///Ескерту жазбасын түзету
                /// </summary>
                /// <param name="digression"></param>
                /// <param name="action"></param>
                /// <returns>сәтті болған жағдайда 1, әйтпесе -1</returns>
       public int UpdateDigression(DigressionMark digression, Kilometer kilometer, RdAction action)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                NpgsqlTransaction transaction = (NpgsqlTransaction)db.BeginTransaction();
                var command = new NpgsqlCommand();
                command.Connection = (NpgsqlConnection)db;
                command.Transaction = transaction;
                try
                {
                    command.CommandText =$@"
                    INSERT INTO s3_history(
	                   original_id, pch, naprav, put, pchu, pd, pdb, km, meter, trip_id, ots, kol, otkl, len, primech, tip_poezdki, cu, us, p1, p2, ur, pr, r1, r2, bas, typ, uv, uvg, ovp, ogp, is2to3, track_id, onswitch, isequalto4, distance_id, isequalto3, state_id, editor, comment)
                    SELECT
                       id, pch, naprav, put, pchu, pd, pdb, km, meter, trip_id, ots, kol, otkl, len, primech, tip_poezdki, cu, us, p1, p2, ur, pr, r1, r2, bas, typ, uv, uvg, ovp, ogp, is2to3, track_id, onswitch, isequalto4, distance_id, isequalto3, {(int)action}, '{digression.Editor}', '{digression.EditReason}'
                    FROM s3 WHERE s3.id = {digression.ID}
                    ";
                    /// ,'{digression.NewBedemostComment}',newBedemostComment
                    command.ExecuteNonQuery();
                    var prevPoint = kilometer.Point;
                    if (action == RdAction.Delete)
                    {
                        command.CommandText = $"DELETE FROM s3 WHERE id = {digression.ID}";
                        command.ExecuteNonQuery();
                        kilometer.Digressions.Remove(digression);
                    } else
                    {
                        command.CommandText = $@"
                        UPDATE s3
                            SET typ = {digression.Degree}, otkl = {digression.Value}, len = {digression.Length}, ovp = {digression.PassengerSpeedLimit}, ogp = {digression.FreightSpeedLimit}
                        WHERE 
                            id = {digression.ID}
                        ";
                        command.ExecuteNonQuery();
                    }
                    kilometer.CalcPoint();

                    if (prevPoint != kilometer.Point)
                    {
                        command.CommandText= $@"
                        INSERT INTO bedemost_history(
                            original_id, km, ball, ots_iv_st, primech)
                        SELECT
                            id, km, ball, ots_iv_st, primech from bedemost
                        WHERE 
                            km = {digression.Km} and track_id = {digression.TrackId} and trip_id = {digression.TripId}
                        ";
                        command.ExecuteNonQuery();
                        command.CommandText = $@"
                        UPDATE bedemost
                            SET ball = {kilometer.Point}, ots_iv_st = '{digression.LimitSpeedToString()}', primech = '{digression.NewBedemostComment}'
                        WHERE 
                            km = {digression.Km} and track_id = {digression.TrackId} and trip_id = {digression.TripId}
                        ";
                        command.ExecuteNonQuery();
                    }
                    //SET ball = { kilometer.Point }, ots_iv_st = '{digression.LimitSpeedToString()}, primech = {digression.NewBedemostComment}'
                    transaction.Commit();
                    return 1;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine($"UpdateDigression error: {e.Message}");
                    return -1;
                    
                }
                finally
                {
                    db.Close();
                }

             
            }
            return -1;
        }

        public int InsertCorrection(long trip_id, int track_id, int Number, int coord, int CorrectionValue)
        //public int InsertCorrection(long trip_id, int track_id, int Number, int coord, string CorrectionType, int CorrectionValue)
        //public int InsertCorrection(trip.Id, Track_id, Number, coord, $"{coord} Привязка координат: {(CorrectionType == CorrectionType.Manual ? "РП" : "АП")} коррекция; начальной привязки на {CorrectionValue}   метр")
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                NpgsqlTransaction transaction = (NpgsqlTransaction)db.BeginTransaction();
                var command = new NpgsqlCommand();
                command.Connection = (NpgsqlConnection)db;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = $@"
                    INSERT INTO s3_correction(
	                 trip_id,track_id,km,coord,correctionvalue)
                    VALUES
                        ({trip_id} ,{track_id}, {Number}, {coord}, {CorrectionValue} ) ";
                    command.ExecuteNonQuery();
                    //var prevPoint = kilometer.Point;

                    transaction.Commit();
                    return 1;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine($"UpdateDigression error: {e.Message}");
                    return -1;

                }
                finally
                {
                    db.Close();
                }


            }
        }
        public List<CorrectionNote> GetCorrectionNotes(long trip_id, int Track_id, int Number, int coord, int CorrectionValue)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                //return db.Query<CorrectionNote>($@"select  * from s3_correction where km={Number} ").ToList();
                return db.Query<CorrectionNote>($@"select distinct km,trip_id,track_id,correctionvalue,coord from s3_correction where km={Number} GROUP BY  km,trip_id,track_id,correctionvalue,coord ").ToList();
            }
        }
        public List<Digression> Check_direction_name(long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    var txt = $@"SELECT
	                                * ,file_id fileid
                                FROM    
	                                report_violperpen
                                WHERE
	                                trip_id = {trip_id} 
	                                                
                                ORDER BY
	                                km";

                    return db.Query<Digression>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_ViolPerpen error: " + e.Message);

                    return null;
                }

            }
        }

        public List<NotCheckedKm> GetDop2(long trip_id, long distId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<NotCheckedKm>($@"
                SELECT
	                * 
                FROM
	                not_checked_km 
                WHERE
	                trip_id = {trip_id} 
	                AND distance_id = {distId}").ToList();
            }
        }

        public List<Digression> GetBallast(MainParametersProcess mainProcess)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)

                    db.Open();
                //if (ballast_{ mainProcess.Trip_id}.any())
                 
                return db.Query<Digression>($@" SELECT
	                                                COALESCE ( LEFT_data.km, right_data.km ) km,
	                                                COALESCE ( LEFT_data.meter, right_data.meter ) meter,
	                                                COALESCE ( LEFT_data.diff, -999 ) LEFT_data,
	                                                COALESCE ( right_data.diff, -999 ) right_data 
                                                FROM
	                                                ( SELECT * FROM ballast_213 WHERE threat_id = 1 ) LEFT_data
	                                                FULL OUTER JOIN ( SELECT * FROM ballast_213 WHERE threat_id = 2 ) right_data ON LEFT_data.km = right_data.km 
	                                                AND LEFT_data.meter = right_data.meter 
                                                ORDER BY
	                                                km,
	                                                meter").ToList();
            

                //return db.Query<Digression>($@" SELECT
                //                                 COALESCE ( LEFT_data.km, right_data.km ) km,
                //                                 COALESCE ( LEFT_data.meter, right_data.meter ) meter,
                //                                 COALESCE ( LEFT_data.diff, -999 ) LEFT_data,
                //                                 COALESCE ( right_data.diff, -999 ) right_data 
                //                                FROM
                //                                 ( SELECT * FROM ballast_{mainProcess.Trip_id} WHERE threat_id = 1 ) LEFT_data
                //                                 FULL OUTER JOIN ( SELECT * FROM ballast_{mainProcess.Trip_id} WHERE threat_id = 2 ) right_data ON LEFT_data.km = right_data.km 
                //                                 AND LEFT_data.meter = right_data.meter 
                //                                ORDER BY
                //                                 km,
                //                                 meter").ToList();

            }
        }
        /// <summary>
        /// PU32 үшін километрлерді қайтарады
        /// </summary>
        /// <param name="from">есептің бастапқы күні</param>
        /// <param name="to">есептің аяққы күні</param>
        /// <param name="distance_id">қашықтықтық идентификаторы</param>
        /// <param name="trip_type">саяхат түрі</param>
        /// <returns>қашықтық бойындағы белгілі бір кезеңде тексерілген километрлерді қайтарады</returns>
        public List<Kilometer> GetPU32Kilometers(DateTime from, DateTime to, long distance_id, TripType trip_type)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                var result = new List<Kilometer>();
                if (db.State == ConnectionState.Closed)
                    db.Open();


                var trips = db.Query<Trips>($@"select * from trips where trip_date between '{from}' and '{to}'
	                    and trip_type= {(int)trip_type} ").ToList();
                
                foreach (var trip in trips)
                {
                    var kilometers = db.Query<Kilometer>($@"
                    Select distinct  
                        coalesce(corrections.corrcount,0) as corrcount, adn.code as direction_code, adn.name as direction_name, track.code as track_name, pchu.code as pchucode,pchu.chief_fullname pchuchief, 
                        pd.code as pdcode, pd.chief_fullname pdchief, pdb.code as pdbcode, pdb.chief_fullname pdbchief, bed.km number, bed.lkm as lkm, bed.primech, track.id as track_id, rdp.id as trip_id, rdp.trip_date as TripDate,
                        (hist.id is not null) as corrected, bed.fdbroad, bed.fdconstrict, bed.fdskew, bed.fddrawdown, bed.fdstright, bed.fdlevel
                    FROM public.bedemost as bed

                    inner join trips as rdp on rdp.id = bed.trip_id
                    inner join adm_track as track on track.id = bed.track_id
                    inner join adm_direction as adn on adn.id = track.adm_direction_id
                    inner join tpl_period as period on period.adm_track_id = track.id and rdp.trip_date between period.start_date and period.final_date
                    inner join tpl_pdb_section as tps on tps.period_id = period.id and bed.kmtrue between tps.start_km and tps.final_km
                    inner join adm_pdb as pdb on pdb.id = tps.adm_pdb_id and pdb.code = cast(bed.pdb as text)
                    inner join adm_pd as pd on pd.id = pdb.adm_pd_id and pd.code = cast(bed.pd as text)
                    inner join adm_pchu as pchu on pchu.id = pd.adm_pchu_id  and pchu.code = cast(bed.pchu as text)
                    inner join adm_distance as distance on distance.id = pchu.adm_distance_id and distance.code = bed.pch
                    left join bedemost_history hist on hist.original_id = bed.id
                    left join (select count(*) corrcount, km from s3_history where trip_id = {trip.Id} GROUP BY km) as corrections on corrections.km = bed.km

                    where KMTRUE <> 0 and bed.trip_id = {trip.Id}  and distance.id = {distance_id}

                   group by adn.code,adn.name, track.id, rdp.id, track.code, pchu.code, pchu.chief_fullname, pd.code, pd.chief_fullname,  
                            pdb.code, pdb.chief_fullname, bed.km, bed.lkm, bed.primech, hist.id,
                            bed.fdbroad, bed.fdconstrict, bed.fdskew, bed.fddrawdown, bed.fdstright, bed.fdlevel, corrections.corrcount
                   order by adn.code, track.code, pchu.code, pd.code, bed.km").ToList();

                    foreach (var km in kilometers)
                    {
                        km.Trip = trip;
                        result.Add(km);
                    }
                }
                return result;
            }
            
        }

        public int InsertRating(int km, string rating, string put)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                NpgsqlTransaction transaction = (NpgsqlTransaction)db.BeginTransaction();
                var command = new NpgsqlCommand();
                command.Connection = (NpgsqlConnection)db;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = $@"
                    UPDATE bedemost
                    SET rating = '{rating}' WHERE km = {km} AND put = '{put}'";
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    return 1;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine($"UpdateDigression error: {e.Message}");
                    return -1;

                }
                finally
                {
                    db.Close();
                }
            }
        }

        public List<Trips> GetTripFromFileId(int fileId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Trips>($@"SELECT
	                                        trips.*,
	                                        start_st.NAME AS Start_station_name,
	                                        final_st.NAME AS Final_station_name,
	                                        direction.NAME AS direction,
	                                        direction.code AS directioncode 
                                        FROM
	                                        trips
	                                        INNER JOIN adm_station AS start_st ON start_st.ID = start_station
	                                        INNER JOIN adm_station AS final_st ON final_st.ID = final_station
	                                        INNER JOIN adm_direction direction ON direction.ID = trips.direction_id 
                                        WHERE
	                                        trips.id in (SELECT trip_id FROM trip_files WHERE id = {fileId})
                                        ORDER BY
	                                        trip_date ", commandType: CommandType.Text).ToList();
            }
        }

        public List<CrosProf> GetNextProfileDatas(int meter, int count, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<CrosProf>(
                    $@"SELECT 
                            ID,
	                        meter,
	                        pu_l,
	                        pu_r,
	                        vert_l,
	                        vert_r,
	                        bok_l,
	                        bok_r,
	                        npk_l,
	                        npk_r,
                            shortwavesleft,
                            shortwavesright,
                            mediumwavesleft,
                            mediumwavesright,
                            longwavesleft,
                            longwavesright,
	                        iz_45_l,
	                        iz_45_r 
                        FROM
	                        PUBLIC.profiledata_{trip_id}
                        WHERE
	                        ID > {meter} 
                        ORDER BY
                        ID").ToList();

                }
                catch (Exception ex)
                {
                    //Console.WriteLine("GetNextProfileDatas " + ex.Message);
                    return new List<CrosProf> { };
                }
                
            }
        }

        public List<CrosProf> GetNextProfileDatasByKm(int km, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<CrosProf>(
                    $@"SELECT 
                            ID,
	                        meter,
	                        pu_l,
	                        pu_r,
	                        vert_l,
	                        vert_r,
	                        bok_l,
	                        bok_r,
	                        npk_l,
	                        npk_r,
	                        iz_45_l,
	                        iz_45_r 
                        FROM
	                        PUBLIC.profiledata_{trip_id}
                        WHERE
	                        km = {km} 
                        ORDER BY
                        ID").ToList();
            }
        }

        public void UpdateGap(Gap gap)
        {
            var PassSpeed = 80;
            var AllowSpeed = "";
            var DigName = DigressionName.Undefined;
            var Zazor = Math.Max(gap.Zazor, gap.R_zazor);
            if (61 <= PassSpeed && PassSpeed <= 100)
            {
                switch (Zazor)
                {
                    case var value when 25 <= value && value <= 30:
                        AllowSpeed = "";
                        DigName = DigressionName.GapSimbol;
                        break;
                    case var value when 30 < value && value <= 35:
                        AllowSpeed = "25/25";
                        DigName = DigressionName.Gap;
                        break;
                    case var value when 35 < value:
                        AllowSpeed = "0/0";
                        DigName = DigressionName.Gap;
                        break;
                }
            }
            else if (101 <= PassSpeed && PassSpeed <= 200)
            {
                switch (Zazor)
                {
                    case var value when 27 <= value && value <= 30:
                        AllowSpeed = "";
                        DigName = DigressionName.GapSimbol;
                        break;
                    case var value when 30 < value && value <= 35:
                        AllowSpeed = "25/25";
                        DigName = DigressionName.Gap;
                        break;
                    case var value when 35 < value:
                        AllowSpeed = "0/0";
                        DigName = DigressionName.Gap;
                        break;
                }
            }

            var query = $@"UPDATE report_gaps 
                            SET zazor_l = {gap.Zazor},
                            zazor_r = {gap.R_zazor} ,
                            zabeg = {gap.Zabeg},
                            otst = '{(DigName.Name == DigressionName.Undefined.Name ? "" : DigName.Name)}',
                            vdop = '{AllowSpeed}'
                        WHERE
	                        ID = {gap.id}";
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                db.Execute(query);
            }
        }
        public int UpdateGapBase(Gap dGap, Kilometer kilometer, RdAction action)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                NpgsqlTransaction transaction = (NpgsqlTransaction)db.BeginTransaction();
                var command = new NpgsqlCommand();
                command.Connection = (NpgsqlConnection)db;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = $@"
                    INSERT INTO report_gaps_history(
	                   original_id, trip_id, modi_date, user_id, pdb_section, fragment, km, piket, m, vpz, zazor_r, zazor_l, temp, zabeg, vdop, otst_l, otst_r, primech, file_id, fnum,ms,r_file_id,r_fnum,r_ms,template_id,r_t,x,y,h,x_r,y_r,h_r,editor,editreason, state_id)
                    SELECT
                       id, trip_id, CURRENT_TIMESTAMP, user_id, pdb_section, fragment, km, piket, m, vpz, zazor_r, zazor_l, temp, zabeg, vdop, otst_l, otst_r, primech, file_id, fnum,ms,r_file_id,r_fnum,r_ms,template_id,r_t,x,y,h,x_r,y_r,h_r,'{dGap.Editor}', '{dGap.EditReason}', {(int)action}
                    FROM report_gaps WHERE report_gaps.id = {dGap.id}
                    ";
                    command.ExecuteNonQuery();
                    var prevPoint = kilometer.Point;
                    if (action == RdAction.Delete)
                    {
                        command.CommandText = $"DELETE FROM report_gaps WHERE report_gaps.id = {dGap.id}";
                        command.ExecuteNonQuery();
                        kilometer.Gaps.Remove(dGap);
                    }
                    else
                    {
                        command.CommandText = $@"
                        UPDATE report_gaps
                            SET zazor_r = {dGap.R_zazor}, zazor_l = {dGap.Zazor}, zabeg = {dGap.Zabeg}, otst_l = '{dGap.Otst}', otst_r = '{dGap.Otst}', vdop = '{dGap.AllowSpeed}', modi_date=CURRENT_TIMESTAMP
                        WHERE 
                            id = {dGap.id}
                        ";
                        command.ExecuteNonQuery();
                    }
                    kilometer.CalcPoint();
                    transaction.Commit();
                    return 1;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine($"UpdateGap error: {e.Message}");
                    return -1;

                }
                finally
                {
                    db.Close();
                }
            }
            return -1;
        }

        public int UpdateAdditionalBase(Digression digression, Kilometer kilometer, RdAction action)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                NpgsqlTransaction transaction = (NpgsqlTransaction)db.BeginTransaction();
                var command_hist = new NpgsqlCommand();
                command_hist.Connection = (NpgsqlConnection)db;
                command_hist.Transaction = transaction;
                var command_edit = new NpgsqlCommand();
                command_edit.Connection = (NpgsqlConnection)db;
                command_edit.Transaction = transaction;
                var command_del = new NpgsqlCommand();
                command_del.Connection = (NpgsqlConnection)db;
                command_del.Transaction = transaction;
                command_hist.CommandText = $@"
                                INSERT INTO s3_additional_history(original_id, km, meter, typ, digname,direction_num,founddate,threat,r_threat,length,location,norma,r_digname,value,count,allowspeed,primech,modi_date,editor,editreason,action)
                                SELECT
                                   id, km, meter, typ, digname,direction_num,founddate,threat,r_threat,length,location,norma,r_digname,value,count,allowspeed,primech,CURRENT_TIMESTAMP,'{digression.Editor}', '{digression.EditReason}', {(int)action}
                                FROM s3_additional WHERE s3_additional.id = {digression.Id} AND s3_additional.km = {digression.Km}
                                ";
                command_edit.CommandText = $@"
                                UPDATE s3_additional
                                    SET length = {digression.Length}, count = {digression.Count}, allowspeed = '{digression.AllowSpeed}', modi_date=CURRENT_TIMESTAMP
                                WHERE 
                                    id = {digression.Id} AND s3_additional.km = {digression.Km}
                                ";
                command_del.CommandText = $"DELETE FROM s3_additional WHERE s3_additional.id = {digression.Id} AND s3_additional.km = {digression.Km}";
                command_hist.ExecuteNonQuery();
                try
                {
                    if (action == RdAction.Delete)
                    {
                        command_del.ExecuteNonQuery();
                        kilometer.AdditionalDigressions.Remove(digression);
                    }
                    else
                    {
                        command_edit.ExecuteNonQuery();
                    }
                    kilometer.CalcPoint();
                    transaction.Commit();
                    return 1;
                }
                catch (Exception e)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"UpdateDigression error: {e.Message}");
                        return -1;

                    }
                finally
                {
                    db.Close();
                }
            }

        }
        public List<long> GetFileID(int trip_id, int num)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<long>($@"SELECT id FROM trip_files WHERE km_num = {num} AND trip_id = {trip_id}").ToList();
            }

        }
        public void SetFileID(int num, long fileId, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                NpgsqlTransaction transaction = (NpgsqlTransaction)db.BeginTransaction();
                var command = new NpgsqlCommand();
                command.Connection = (NpgsqlConnection)db;
                command.Transaction = transaction;
                command.CommandText = $@"SELECT file_name FROM trip_files WHERE id = {fileId}";
                try
                {
                    string filename = (string)command.ExecuteScalar() ?? "";
                    string extension = Path.GetExtension(filename).Split('_')[1];
                    filename = Path.GetFileNameWithoutExtension(filename);
                    var query = $@"UPDATE trip_files 
                            SET km_num = {num}
	                        WHERE file_name LIKE '%{extension}' AND trip_id = {trip_id}";
                    db.Execute(query);
                }
                catch (Exception  e){
                    transaction.Rollback();
                    Console.WriteLine($"UpdateDigression error: {e.Message}");
                }
                finally
                {
                    db.Close();
                }
            }

        }
        public int UpdateDigressionBase(Digression digression, int type, Kilometer kilometer, RdAction action)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                NpgsqlTransaction transaction = (NpgsqlTransaction)db.BeginTransaction();
                var command_hist = new NpgsqlCommand();
                command_hist.Connection = (NpgsqlConnection)db;
                command_hist.Transaction = transaction;
                var command_edit = new NpgsqlCommand();
                command_edit.Connection = (NpgsqlConnection)db;
                command_edit.Transaction = transaction;
                var command_del = new NpgsqlCommand();
                command_del.Connection = (NpgsqlConnection)db;
                command_del.Transaction = transaction;
                try
                {
                    switch (type)
                    {
                        case 2:
                            command_hist.CommandText = $@"
                                INSERT INTO report_bolts_history(original_id,trip_id,modi_date,pchu,station,km,piket,meter,before,after,speed,notice,fnum,ms,fullspeed,overlay,fileid,fastening,threat,note,editor,editreason,action)
                                SELECT
                                   id, trip_id, CURRENT_TIMESTAMP, pchu,station,km,piket,meter,before,after,speed,notice,fnum,ms,fullspeed,overlay,fileid,fastening,threat,note,'{digression.Editor}', '{digression.EditReason}', {(int)action}
                                FROM report_bolts WHERE report_bolts.id = {digression.Id}
                                ";
                            command_edit.CommandText = $@"
                                UPDATE report_bolts
                                    SET before = {digression.Before}, after = {digression.After}, overlay = '{digression.Overlay}', fullspeed = '{digression.FullSpeed}', modi_date=CURRENT_TIMESTAMP
                                WHERE 
                                    id = {digression.Id}
                                ";
                            command_del.CommandText = $"DELETE FROM report_bolts WHERE report_bolts.id = {digression.Id}";
                            break;
                        case 3:
                            command_hist.CommandText = $@"
                                INSERT INTO report_badfasteners_history(state,trip_id,modi_date,user_id,pchu,station,km,mtr,otst,threat_id,fastening,notice,fnum,ms,fileid,original_id,editor,edit_reason,action)
                                SELECT
                                    state,trip_id,CURRENT_TIMESTAMP,user_id,pchu,station,km,mtr,otst,threat_id,fastening,notice,fnum,ms,fileid,id,'{digression.Editor}', '{digression.EditReason}', {(int)action}
                                FROM report_badfasteners WHERE report_badfasteners.id = {digression.Id}
                                ";
                            command_edit.CommandText = $@"
                                UPDATE report_badfasteners
                                    SET otst = '{digression.Otst}', threat_id = '{digression.Threat_id}', fastening = '{digression.Fastening}', modi_date=CURRENT_TIMESTAMP
                                WHERE 
                                    id = {digression.Id}
                                ";
                            command_del.CommandText = $"DELETE FROM report_badfasteners WHERE report_badfasteners.id = {digression.Id}";
                            break;
                        case 4:
                            command_hist.CommandText = $@"
                                INSERT INTO report_violperpen_history(trip_id,km,meter,vdop,angle,fastener,file_id,fnum,ms,original_id,editor,edit_reason,modi_date,action)
                                SELECT
                                    trip_id,km,meter,vdop,angle,fastener,file_id,fnum,ms,id,'{digression.Editor}', '{digression.EditReason}', CURRENT_TIMESTAMP, {(int)action}
                                FROM report_violperpen WHERE report_violperpen.id = {digression.Id}
                                ";
                            command_edit.CommandText = $@"
                                UPDATE report_violperpen
                                    SET angle = '{digression.Angle}', fastener = '{digression.Fastener}'
                                WHERE 
                                    id = {digression.Id}
                                ";
                            command_del.CommandText = $"DELETE FROM report_violperpen WHERE report_violperpen.id = {digression.Id}";
                            break;
                        case 5:
                            command_hist.CommandText = $@"
                                INSERT INTO report_defshpal_history(state,trip_id,modi_date,user_id,pchu,station,km,meter,otst,fastening,meropr,notice,fnum,ms,file_id,original_id,editor,edit_reason,action)
                                SELECT
                                    state,trip_id,CURRENT_TIMESTAMP,user_id,pchu,station,km,meter,otst,fastening,meropr,notice,fnum,ms,file_id,id,'{digression.Editor}', '{digression.EditReason}',{(int)action}
                                FROM report_defshpal WHERE report_defshpal.id = {digression.Id}
                                ";
                            command_edit.CommandText = $@"
                                UPDATE report_defshpal
                                    SET otst = '{digression.Otst}', fastening = '{digression.Fastening}', meropr = '{digression.Meropr}', notice = '{digression.Notice}', modi_date = CURRENT_TIMESTAMP 
                                WHERE 
                                    id = {digression.Id}
                                ";
                            command_del.CommandText = $"DELETE FROM report_defshpal WHERE report_defshpal.id = {digression.Id}";
                            break;
                    }
                    command_hist.ExecuteNonQuery();
                    var prevPoint = kilometer.Point;
                    if (action == RdAction.Delete)
                    {
                        command_del.ExecuteNonQuery();
                        switch (type)
                        {
                            case 2:
                                kilometer.Bolts.Remove(digression);
                                break;
                            case 3:
                                kilometer.Fasteners.Remove(digression);
                                break;
                            case 4:
                                kilometer.PerShpals.Remove(digression);
                                break;
                            case 5:
                                kilometer.DefShpals.Remove(digression);
                                break;
                        }
                    }
                    else
                    {
                        command_edit.ExecuteNonQuery();
                    }
                    kilometer.CalcPoint();
                    transaction.Commit();
                    return 1;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine($"UpdateDigression error: {e.Message}");
                    return -1;

                }
                finally
                {
                    db.Close();
                }
            }
        }

        public object GetNextPart(Func<double> last1, Func<int> last2, int n)
        {
            throw new NotImplementedException();
        }

        public List<RDCurve> LoadDataByCurve(List<RDCurve> rdcs, long tripId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var query = $@" SELECT
	                                    * , 
                                        stright_right as radius
                                    FROM
	                                    outdata_{tripId}
                                    WHERE
	                                    km + meter / 10000.0 BETWEEN {rdcs.First().GetRealCoordinate().ToString().Replace(",",".")} 
	                                                             AND {rdcs.Last().GetRealCoordinate().ToString().Replace(",", ".")} 
                                    ORDER BY
	                                    km + meter / 10000.0";

                    return db.Query<RDCurve>(query).ToList();
                }
                catch (Exception ex)
                {
                    return new List<RDCurve> { };
                }

            }
        }
    }
    
}
