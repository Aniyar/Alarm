

using Accord;
using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.DataAccess.Properties;
using Dapper;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ALARm.DataAccess
{
    public class AdditionalParametersRepository : IAdditionalParametersRepository
    {
        private int[,] Filter = null;

        object IAdditionalParametersRepository.Insert_deviationsinballast(long trip_id, int iD, List<RailFastener> digressions)
        {
            
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    foreach (var fastener in digressions)
                    {
                      

                        var txt = $@"INSERT INTO report_deviationsinballast(
	                                        trip_id,
                                            pchu,
                                            station,
	                                        km,
	                                        mtr,
                                            railtype,
                                            vpz,
	                                        otst,
                                            kns,
                                            vdop,
                                            notice,
                                            fnum ,
                                            ms,
	                                        file_id,
                                            threat_id
                                        )
                                        VALUES
	                                       (
                                            '{trip_id}', 
                                            '{fastener.PdbSection}',
                                            '{fastener.Station}',
                                            '{fastener.Km}',
                                            '{fastener.Mtr}',
                                            '{fastener.RailType}',
                                            '{fastener.Vpz}',
                                            '{fastener.Otst}',
                                            '{fastener.Vdop}',
										    '{fastener.Kns}',
                                            '{fastener.Notice}',
                                            '{fastener.Fnum}',
                                            '{fastener.Ms}',
                                            '{fastener.file_id}',
                                            '{fastener.Threat_id}')
                                            ";
                        db.Execute(txt);

                    }
                    return "Удачно записано!";
                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert_deviationsinballast Ошибка записи в БД " + e.Message);
                    return "Ошибка при записи!";
                }

            }
        }
        object IAdditionalParametersRepository.Insert_deviationsinfastening(long trip_id, int iD, List<RailFastener> digList)
        {


            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    foreach (var digression in digList)
                    {
                        var listIS = new List<int> { 8, 9, 10 };
                        var listGAP = new List<int> { 5, 6, 7 };

                        if (listIS.Contains(digression.Oid)) continue;
                        if (listGAP.Contains(digression.Oid)) continue;
                        if (digression.Oid == 47) continue;

                        var txt = $@"INSERT INTO report_deviationsinfastening (
	                                        trip_id,
                                            pchu,
                                            station,
	                                        km,
	                                        mtr,
                                            vpz,
                                            trackclass,
	                                        ots,
                                            fastening,
                                            threat_id,
                                            kol,
                                            tripplan,
                                            norma,
                                            vdop,
                                            notice,
                                            fnum ,
                                            ms,
	                                        file_id
                                        )
                                        VALUES
	                                       (
                                            '{trip_id}', 
                                            '{digression.Pchu}',
                                            '{digression.Station}',
                                            '{digression.Km}',
                                            '{digression.Mtr}',
                                            '{digression.Vpz}',
                                            '{digression.TrackClass}',
                                            '{digression.Ots}',
                                            '{digression.Fastening}',
                                            '{digression.Threat_id}',
                                            '{digression.Velich}',
                                            '{digression.Tripplan}',
                                            '{digression.Norma}',
                                            '{digression.Vdop}',
                                            '{digression.Notice}',
                                            '{digression.Fnum}',
                                            '{digression.Ms}',
                                            '{digression.file_id}')";
                        db.Execute(txt);

                    }
                    return "Удачно записано!";
                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert_deviationsinfastening Ошибка записи в БД " + e.Message);
                    return "Ошибка при записи!";
                }

            }
        }
        public List<Digression> Check_deviationsinfastening_state(long trip_id, int iD)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    var txt = $@"SELECT
	                                *
                                FROM    
	                                report_deviationsinfastening
                                WHERE
	                                trip_id = {trip_id} 
	                                                
                                ORDER BY
	                                km";

                    return db.Query<Digression>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_Total_state_digression error: " + e.Message);

                    return null;
                }

            }
        }


        object IAdditionalParametersRepository.Insert_ListOgDerogations_state(long trip_id, int iD, List<Gap> GetMaech)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    foreach (var elem in GetMaech)
                    {
                        var listIS = new List<int> { 8, 9, 10 };
                        var listGAP = new List<int> { 5, 6, 7 };

                        if (listIS.Contains(elem.Oid)) continue;
                        if (listGAP.Contains(elem.Oid)) continue;
                        if (elem.Oid == 47) continue;

                        var txt = $@"INSERT INTO rd_movement_thread (
	                                        trip_id,
                                            pdb_section,
	                                        km,
	                                        meter,
                                            track_id,
                                            vpz,
                                            threat,
                                            movement,
                                            oid,
                                            sector,
                                            y,
                                            temperature,
                                            fnum,
                                            ms,
	                                        file_id
                                                                        
                                        )
                                        VALUES
	                                       (
                                            '{trip_id}', 
                                            '{elem.Pdb_section}',
                                            '{elem.Km}',
                                            '{elem.Meter}', 
                                            '{elem.track_id}',
                                            '{elem.Vpz}',
                                            '{elem.Threat}',
                                            '{elem.Movement}',
                                            '{elem.Oid}',
                                            '{elem.Sector}',
                                            '{elem.Y}',
										    '{elem.Temperature}',
                                            '{elem.Fnum}',
                                            '{elem.Ms}',
                                            '{elem.File_Id}')";
                        db.Execute(txt);

                    }
                    
                    return "Удачно записано!";
                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert_defshpal Ошибка записи в БД " + e.Message);
                    return "Ошибка при записи!";
                }

            }
        }

        //public List<Digression> Check_Total_balast(long trip_id, int iD)
        //{
        //    using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
        //    {
        //        if (db.State == ConnectionState.Closed)
        //            db.Open();
        //        try
        //        {

        //            var txt = $@"SELECT
	       //                                         * 
        //                                        FROM    
	       //                                         rd_movement_thread
                                                
        //                                        WHERE
	       //                                         trip_id = {trip_id} 
	                                                
        //                                        ORDER BY
	       //                                         km
        //                                        ";

        //            return db.Query<Digression>(txt).ToList();
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("Check_Total_state_digression error: " + e.Message);

        //            return null;
        //        }

        //    }
        //}
        public List<Digression> Check_Total_balast(long trip_id, int iD)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    var txt = $@"SELECT
	                                                * 
                                                FROM    
	                                             report_deviationsinballast
                                                
                                                WHERE
	                                                trip_id = {trip_id} 
	                                                
                                                ORDER BY
	                                                km
                                                ";

                    return db.Query<Digression>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_Total_state_digression error: " + e.Message);

                    return null;
                }

            }
        }


        public List<Gap> Check_ListOgDerogations_state(long trip_id, int iD)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    var txt = $@"SELECT
	                                * 
                                FROM  
                                report_listogderogations
	                               -- rd_movement_thread
                                WHERE
	                                trip_id = {trip_id} 
	                                                
                                ORDER BY
	                                km
                                ";

                    return db.Query<Gap>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("rd_movement_thread error: " + e.Message);

                    return null;
                }

            }
        }
        public List<Digression> Check_Total_threat_id(long trip_id, int iD)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    var txt = $@"SELECT
	                                                * 
                                                FROM    
	                                               report_listogderogations
                                                WHERE
	                                                trip_id = {trip_id} 
	                                                
                                                ORDER BY
	                                                km
                                                ";

                    return db.Query<Digression>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_Total_state_digression error: " + e.Message);

                    return null;
                }

            }
        }
        public List<Digression> Check_Total_bolts(long trip_id, int iD)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    var txt = $@"SELECT
	                                                * 
                                                FROM    
	                                                report_bolts
                                                WHERE
	                                                trip_id = {trip_id} 
	                                                
                                                ORDER BY
	                                                km
                                                ";

                    return db.Query<Digression>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_Total_state_digression error: " + e.Message);

                    return null;
                }

            }
        }

        public List<Digression> Check_Total_state_Jointless(long trip_id, int iD)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    var txt = $@"SELECT
	                                                * 
                                                FROM 
                                                rd_movement_thread
	                                               -- report_listogderogations
                                                WHERE
	                                                trip_id = {trip_id} 
	                                                
                                                ORDER BY
	                                                km
                                                ";

                    return db.Query<Digression>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_Total_state_digression error: " + e.Message);

                    return null;
                }

            }
        }
        public List<Digression> Check_Total_state_rails(long trip_id, int iD)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    var txt = $@"SELECT
	                                                * 
                                                FROM    
	                                                report_deviationsinrails
                                                WHERE
	                                                trip_id = {trip_id} 
	                                                
                                                ORDER BY
	                                                km
                                                ";

                    return db.Query<Digression>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_Total_state_digression error: " + e.Message);

                    return null;
                }

            }
        }

        object IAdditionalParametersRepository.Insert_sleepers(long trip_id, int iD, List<Digression> digList)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    foreach (var finddeg in digList)
                    {
                        var listIS = new List<int> { 8, 9, 10 };
                        var listGAP = new List<int> { 5, 6, 7 };

                        if (listIS.Contains(finddeg.Oid)) continue;
                        if (listGAP.Contains(finddeg.Oid)) continue;
                        if (finddeg.Oid == 47) continue;

                        var txt = $@"INSERT INTO report_deviationsinsleepers (
	                                        trip_id,
                                            pchu,
                                            station,
	                                        km,
	                                        meter,
                                            vpz,
                                            trackclass,
	                                        ots,
                                            fastening,
                                            kol,
                                            railType,
                                            tripplan,
                                            norma,
                                            vdop,
                                            notice,
                                            fnum ,
                                            ms,
	                                        file_id
                                        )
                                        VALUES
	                                       (
                                            '{trip_id}', 
                                            '{finddeg.PCHU}',
                                            '{finddeg.Station}',
                                            '{finddeg.Km}',
                                            '{finddeg.Meter}',
                                            '{finddeg.Vpz}',
                                            '{finddeg.TrackClass}',
                                            '{finddeg.Ots}',
                                            '{finddeg.Fastening}',
                                            '{finddeg.Kol}',
                                            '{finddeg.RailType}',
                                            '{finddeg.Tripplan}',
                                            '{finddeg.Norma}',
                                            '{finddeg.Vdop}',
                                            '{finddeg.Notice}',
                                            '{finddeg.Fnum}',
                                            '{finddeg.Ms}',
                                            '{finddeg.file_id}')";
                        db.Execute(txt);

                    }
                    return "Удачно записано!";
                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert_defshpal Ошибка записи в БД " + e.Message);
                    return "Ошибка при записи!";
                }

            }
        }
        public List<Digression> Check_sleepers_state(long trip_id, int iD)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    var txt = $@"SELECT
	                                                * 
                                                FROM    

	                                                report_deviationsinsleepers
                                                WHERE
	                                                trip_id = {trip_id} 
	                                                




                                                ORDER BY
	                                                km
                                                ";

                    return db.Query<Digression>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_Total_state_digression error: " + e.Message);

                    return null;
                }

            }
        }

        public List<Digression> Check_Total_state_fastening(long trip_id, int iD)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    var txt = $@"SELECT
	                                                * 
                                                FROM    
	                                                report_speedlimfastening
                                                WHERE
	                                                trip_id = {trip_id} 
	                                                
                                                ORDER BY
	                                                km
                                                ";

                    return db.Query<Digression>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_Total_state_digression error: " + e.Message);

                    return null;
                }

            }
        }
        public List<Digression> Check_Total_state_digression(long trip_id, int iD)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    var txt = $@"SELECT
	                                                * 
                                                FROM    
	                                                report_gaps
                                                WHERE
	                                                trip_id = {trip_id} 
	                                                
                                                ORDER BY
	                                                km
                                                ";

                    return db.Query<Digression>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_Total_state_digression error: " + e.Message);

                    return null;
                }

            }
        }


        public List<Digression> Total(long trip_id, int iD)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                    

                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    var txt = $@"SELECT COALESCE
	                                ( Current_data.km_b_fastener, Current_data.km_shpala, Current_data.km_gaps ,Current_data.km_npk_l ) AS km,
                                --Current_data.km_b_fastener,
	                                COALESCE ( Current_data.total_b_fastener, - 999 ) AS total_b_fastener,
                                --Current_data.km_shpala,
	                                COALESCE ( Current_data.total_shpala, - 999 ) AS total_shpala,
	                                COALESCE ( Current_data.total_gaps, - 999 ) AS total_gaps, 
                                    COALESCE ( Current_data.total_npk_l, - 999 ) AS total_npk_l
                                FROM
	                                (
	                                SELECT
		                                * 
	                                FROM
		                                (
		                                SELECT
			                                rbf.km AS km_b_fastener,
			                                COUNT ( rbf.km ) AS total_b_fastener 
		                                FROM
			                                report_badfasteners AS rbf 
		                                WHERE
			                                trip_id = {trip_id} 
		                                GROUP BY
			                                rbf.km 
		                                ORDER BY
			                                rbf.km ASC
		                                ) AS bf
		                                FULL OUTER JOIN (
		                                SELECT
			                                rdf.km AS km_shpala,
			                                COUNT ( rdf.km ) AS total_shpala 
		                                FROM
			                                report_defshpal AS rdf 
		                                WHERE
			                                trip_id = {trip_id} 
		                                GROUP BY
			                                rdf.km 
		                                ORDER BY
			                                rdf.km ASC
		                                ) AS ds ON bf.km_b_fastener = ds.km_shpala
		                                FULL OUTER JOIN (
		                                SELECT
			                                rg.km AS km_gaps,
			                                  COUNT ( rg.zazor_l ) +count(rg.zazor_r ) AS total_gaps  
		                                FROM
			                                report_gaps AS rg 
		                                WHERE
			                                trip_id = {trip_id} 
			                                --AND otst <> '' 
		                                GROUP BY
			                                rg.km 
		                                ORDER BY
			                                rg.km ASC
		                                ) AS rg ON ds.km_shpala = rg.km_gaps 
                                        	FULL OUTER JOIN (
			                                SELECT
				                                pd213.km AS km_npk_l,
				                                COUNT ( pd213.km ) AS total_npk_l
			                                FROM
				                                profiledata_213 AS pd213
		                                --WHERE
				                                --trip_id = 213 
				
			                                GROUP BY
				                                pd213.km 
			                                ORDER BY
			                                pd213.km ASC 
			                                ) AS pd213 ON rg.km_gaps = pd213.km_npk_l
			
	                                ) AS Current_data
                                        ORDER BY
	                                        km ASC";

                    return db.Query<Digression>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_defshpal_state error: " + e.Message);

                    return null;
                }


            }
        }

        public List<Digression> Check_defshpal_state(long trip_id, int iD)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                   
                    var txt = $@"SELECT
	                                * , file_id fileid, meter as Mtr
                                FROM    
	                                report_defshpal
                                WHERE
	                                trip_id = {trip_id} 
	                                               
                                ORDER BY
	                                km, meter
                                ";

                    return db.Query<Digression>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_defshpal_state error: " + e.Message);

                    return null;
                }

            }
        }

        object IAdditionalParametersRepository.Insert_defshpal(long trip_id, int iD, List<Digression> digressions)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    foreach (var finddeg in digressions)
                    {
                        var listIS = new List<int> { 8, 9, 10 };
                        var listGAP = new List<int> { 5, 6, 7 };

                        if (listIS.Contains(finddeg.Oid)) continue;
                        if (listGAP.Contains(finddeg.Oid)) continue;
                        if (finddeg.Oid == 47) continue;

                        var txt = $@"INSERT INTO report_defshpal (
	                                        trip_id,
                                            pchu,
                                            station,
	                                        km,
	                                        meter,
	                                        otst,
                                            fastening,
	                                        meropr,
                                            notice,
                                            fnum ,
                                            ms,
	                                        file_id
                                        )
                                        VALUES
	                                       (
                                            '{trip_id}', 
                                            '{finddeg.Pchu}',
                                            '{finddeg.Station}',
                                            '{finddeg.Km}',
                                            '{finddeg.Meter}',
                                            '{finddeg.Otst}',
                                            '{finddeg.Fastening}',
										    '{finddeg.Meropr}',
                                            '{finddeg.Notice}',
                                            '{finddeg.Fnum}',
                                            '{finddeg.Ms}',
                                            '{finddeg.Fileid}')";
                            db.Execute(txt);
                        
                    }
                    return "Удачно записано!";
                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert_defshpal Ошибка записи в БД " + e.Message);
                    return "Ошибка при записи!";
                }

            }
        }

        public List<Digression> Check_badfastening_state(long trip_id, int templ_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var txt = $@"SELECT
	                                * 
                                FROM
	                                report_badfasteners
                                WHERE
	                                trip_id = {trip_id} 
	                                               
                                ORDER BY
	                                km, mtr
                                ";

                    return db.Query<Digression>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_badfastening_state error: " + e.Message);

                    return null;
                }

            }
        }
        object IAdditionalParametersRepository.Insert_badfastening(long trip_id, int iD, List<RailFastener> badFasteners)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    foreach (var fastener in badFasteners)
                    {
                        

                        var txt = $@"INSERT INTO report_badfasteners (
	                                        trip_id,
	                                        pchu,
	                                        station,
	                                        km,
	                                        mtr,
	                                        otst,
	                                        threat_id,
											fastening,
                                            oid,    
                                            next_oid,
											notice,
	                                        fileid,
	                                        ms,
	                                        fnum 
                                        )
                                        VALUES
	                                       (
                                            '{trip_id}', 
                                            '{fastener.PdbSection}', 
                                            '{fastener.Station}',
                                            '{fastener.Km}',
                                            '{fastener.Mtr}',
                                            '{fastener.Digressions[0].GetName()}',
										    '{fastener.Threat_id}',
										    '{fastener.Fastening}',
                                            '{fastener.Oid}',
                                            '{fastener.Next_oid}',   
                                            '{fastener.Notice}',
                                            '{fastener.Fileid}',
                                            '{fastener.Ms}',
                                            '{fastener.Fnum}')";
                        db.Execute(txt);
                    }
                    return "Удачно записано!";
                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert_badfastening Ошибка записи в БД " + e.Message);
                    return "Ошибка при записи!";
                }

            }
        }


        object IAdditionalParametersRepository.Insert_bolt(long trip_id, int template_id, List<Digression> digressions)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    foreach (var bolt in digressions)
                    {
                        var txt = $@"INSERT INTO report_bolts (
	                                        trip_id,
	                                        pchu,
	                                        station,
	                                        km,
	                                        meter,
	                                        speed,
	                                        overlay,
											fastening,
											before,
											after,
											fullspeed,
											notice,
	                                        fileid,
	                                        ms,
	                                        fnum,
                                            threat,
                                            note
                                        )
                                        VALUES
	                                       (
                                            '{trip_id}', 
                                            '{bolt.PCHU}', 
                                            '{bolt.Station}',
                                            '{bolt.Km}',
                                            '{bolt.Meter}',
                                            '{bolt.Speed}',
                                            '{bolt.Overlay}',
										    '{bolt.Fastening}',
										    '{bolt.Before}',
                                            '{bolt.After}',
											'{bolt.FullSpeed}',
                                            '{bolt.Notice}',
                                            '{bolt.Fileid}',
                                            '{bolt.Ms}',
                                            '{bolt.Fnum}', 
                                            '{bolt.Threat}',
                                            '{bolt.Note}'
                                            )";
                        db.Execute(txt);
                    }
                    return "Удачно записано!";
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка записи в БД " + e.Message);
                    return "Ошибка при записи!";
                }

            }
        }
        public List<Digression> Check_bolt_state(long trip_id, int templ_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<Digression>($@"SELECT
	                                                * 
                                                FROM
	                                                report_bolts 
                                                WHERE
	                                                trip_id = {trip_id} 
	                                                --AND template_id = {templ_id}
                                                ORDER BY
	                                                km, meter ", commandType: CommandType.Text).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_bolt_state error: " + e.Message);

                    return null;
                }

            }
        }



        public List<Gap> Check_gap_state(long trip_id, int templ_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var gaps = db.Query<Gap>($@"SELECT
	                                            * , zazor_r as r_zazor, zazor_l as zazor, m as meter, vpz as fullSpeed
                                            FROM
	                                            report_gaps
                                            WHERE
	                                            trip_id = {trip_id} 
	                                            --AND template_id = {templ_id}
                                            order by km, meter
                                            ", commandType: CommandType.Text).ToList();

                    gaps = gaps.Where(o => Math.Abs(o.Zabeg) < 500).ToList();
                    /*foreach (Gap gap in gaps){
                        gap.R_zazor = (int)(gap.R_zazor / 1.4);
                        gap.Zazor = (int)(gap.Zazor / 1.4);
                        gap.Zabeg = (int)(gap.Zabeg / 1.4);
                    }*/
                    return gaps;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Check_gap_state eroor: " + e.Message);
                    return null;
                }

            }
        }
        public List<Gap> Check_Sleep_gap_state(long trip_id, int templ_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<Gap>($@"SELECT
	                                            *,
		                                        abs(koord - LEAD ( km*1000+meter, 1 ) OVER ( ORDER BY km )) razn
                                            FROM
	                                            (
	                                            SELECT
		                                            pdb_section,
		                                            fragment,
		                                            km*1000+m koord,
		                                            km,
		                                            M AS meter,
		                                            vpz AS fullSpeed,
		                                            zazor_r AS r_zazor,
		                                            zazor_l AS zazor,
		                                            otst_l,
		                                            otst_r,
		                                            LEAD ( otst_l, 1 ) OVER ( ORDER BY ID ) next_otst
	                                            FROM
		                                            report_gaps 
	                                            ORDER BY
		                                            koord
	                                            ) AS DATA 
                                            WHERE
                                            	otst_l = 'СЗ'
	                                            OR
	                                            otst_r = 'СЗ' ", commandType: CommandType.Text).ToList();
                  
                        //СЗ//З?
                }
                catch
                {
                    return null;
                }

            }
        }
        public string Insert_gap(long trip_id, int template_id, List<Digression> gaps)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                int i = 1;
                try
                {
                    var gap_l = gaps.Where(o => o.Threat == Threat.Left).ToList();
                    var gap_r = gaps.Where(o => o.Threat == Threat.Right).ToList();
                   
                    
                    foreach (var gap in gap_l)
                    {

                        if(gap.Km == 700)
                        {
                            
                        }
                        var zabeg = "-999";
                        var Vdop = "";
                        var Ots = "";

                        var r = gap_r.Where(o => o.Km == gap.Km && (o.Meter >= gap.Meter-1 && o.Meter <= gap.Meter + 1)).ToList();
                        if (r.Any())
                        {
                            if (gap.Zazor == -1)
                            {
                                double k = (double)gap.H / (double)r.First().H;
                                gap.Zazor = (int)(r.First().Zazor * k);
                                gap.GetDigressions436();
                                //if (gap.DigName.Name.Equals("З?"))
                                //    gap.DigName.Name = "З?Л";
                                //if (gap.DigName.Name.Equals("З"))
                                //    gap.DigName.Name = "ЗЛ";
                            }
                            if (r.First().Zazor == -1)
                            {
                                double k = (double)r.First().H / (double)gap.H;
                                r.First().Zazor = (int)(gap.Zazor * k);
                                r.First().GetDigressions436();
                                //if (gap.DigName.Name.Equals("З?"))
                                //    gap.DigName.Name = "З?Л";
                                //if (gap.DigName.Name.Equals("З"))
                                //    gap.DigName.Name = "ЗЛ";
                            }

                            zabeg = (gap.Koord - r.First().Koord).ToString();
                            Vdop = gap.Zazor < r.First().Zazor ? r.First().AllowSpeed : gap.AllowSpeed;
                            
                        }

                        var Dig_l = gap.DigName.Name == "неизвестный" ? "" : gap.DigName.Name;
                        var Dig_r = r.First().DigName.Name == "неизвестный" ? "" : r.First().DigName.Name;
                        // var Dig = Ots == "неизвестный" ? "" : Ots;

                        i++;

                        var txt = $@"INSERT INTO report_gaps (
	                                        trip_id,
	                                        pdb_section,
	                                        fragment,
	                                        km,
	                                        piket,
	                                        M,
	                                        vpz,
	                                        zazor_r,
	                                        zazor_l,
                                            temp,
	                                        zabeg,
	                                        vdop,
	                                        otst_l,
                                            otst_r,
	                                        file_id,
	                                        fnum,
	                                        ms,
	                                        r_file_id,
	                                        r_fnum,
	                                        r_ms,
	                                        template_id ,x ,y ,h ,
                                                         x_r ,y_r ,h_r

                                        
                                        )
                                        VALUES
	                                       (
                                            '{trip_id}', 
                                            '{gap.Pdb_section}', 
                                            '{gap.Fragment}',
                                            '{gap.Km}',
                                            '{(gap.Meter / 100 + 1)}',
                                            '{gap.Meter}',
                                            '{gap.FullSpeed}',
                                            '{ (r.Any() ? r.First().Zazor.ToString() : "-999") }',
                                            '{gap.Zazor}',
                                            '{gap.temp}',
                                            '{zabeg}',
                                            '{ (!r.Any() ? gap.AllowSpeed : Vdop)}',
                                            '{ Dig_l }',
                                            '{ Dig_r }',
                                            '{gap.Fileid}',
                                            '{gap.Fnum}',
                                            '{gap.Ms}',
                                            '{ (r.Any() ? r.First().Fileid.ToString() : "-999") }',
                                            '{ (r.Any() ? r.First().Fnum.ToString() : "-999") }',
                                            '{ (r.Any() ? r.First().Ms.ToString() : "-999") }',
                                            '{template_id}',
                                            '{gap.X}',
                                            '{gap.Y}',
                                            '{gap.H}',
                                            '{(r.Any() ? r.First().X.ToString() : "-999")}',
                                            '{(r.Any() ? r.First().Y.ToString() : "-999")}',
                                            '{(r.Any() ? r.First().H.ToString() : "-999")}');";
                        db.Execute(txt);
                    }
                    return "Удачно записано!";
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка записи в БД " + e.Message);
                    return $"Ошибка на {i} записи";
                }

            }
        }
        public List<Gap> GetdefISGap(long id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Gap>(@"SELECT mtr as meter, * FROM public.rd_video_objects
                                    where rd_video_objects.oid=12 and rd_video_objects.trip_id=" + id +
                                    " order by km, meter,fnum" , commandType: CommandType.Text).ToList();
            }
        }
        public List<Gap> GetGaps(Int64 process_id, int direction, int kilometer)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string orderby = direction == 1 ? " asc " : " desc ";
                return db.Query<Gap>(@"select gaps.nkm as km, ((gaps.picket-1)*100 + gaps.meter) as meter, max(final-start) as length,
                files.threat_id as threat from rd_gaps as gaps
                inner join trip_files as files on files.id = gaps.file_id
                inner join trips as trip on trip.id = files.trip_id
                where gaps.process_id = " + process_id + " and gaps.nkm = " + kilometer + @" and (final-start) between 0 and 60
                group by gaps.nkm, gaps.picket, gaps.meter, files.threat_id , trip.direction_id
                order by gaps.nkm " + orderby + ", ((gaps.picket-1)*100 + gaps.meter) " + orderby, commandType: CommandType.Text).ToList();
            }
        }
        public List<Gap> GetMaech(long trip_id, int direction)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string orderby = direction == 1 ? " asc " : " desc ";
                try
                {
                    return db.Query<Gap>($@"
                                            SELECT
	                                            fnum,
	                                            km,
	                                            mtr AS meter,
	                                            local_fnum,
	                                            files.threat_id AS Threat,
	                                            oid, * 
                                            FROM
	                                            PUBLIC.rd_video_objects
	                                            INNER JOIN trip_files AS files ON files.ID = file_id 
                                            WHERE
	                                            oid IN ( 13, 14 ) 
	                                            AND files.trip_id = {trip_id}
                                            ORDER BY
	                                            --fnum,
	                                            km desc,
	                                            mtr desc,
	                                            local_fnum desc,
	                                            files.threat_id desc,
	                                            oid 
                                                --limit 3000
                                            
                                            ", commandType: CommandType.Text).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetMaech error: " + e.Message);
                    return null;
                }
                
            }
        }
        public List<Gap> GetGaps(Int64 trip_id, int kilometer)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                
                return db.Query<Gap>(@"
                select gaps.nkm as km, max(frame_number) as frame_number, ((gaps.picket-1)*100 + gaps.meter) as meter, max(final-start) as length, max(start) as start, 
                    files.threat_id as threat, speed.passenger as passspeed, speed.freight as FreightSpeed, files.id as file_id
                from rd_gaps as gaps
                    inner join trip_files as files on files.id = gaps.file_id
                    inner join trips as trip on trip.id = files.trip_id
                    inner join tpl_period as sp on trip.trip_date between sp.start_date and sp.final_date
				    inner join apr_speed as speed on speed.period_id = sp.id
                where trip.id = " + trip_id + " and gaps.nkm = " + kilometer + @" and (final-start) between 0 and 60
                    group by gaps.nkm, gaps.picket, gaps.meter, files.threat_id , trip.direction_id, files.id, speed.passenger, speed.freight
                    order by gaps.nkm, ((gaps.picket-1)*100 + gaps.meter) ", commandType: CommandType.Text).ToList();
            }
        }
        public List<Gap> GetGap(long process_id, int direction)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string orderby = " asc ";
                return db.Query<Gap>(@"
                select track.code as put, trip.direction_id as direct, gaps.nkm as km, gaps.picket as picket, ((gaps.picket-1)*100 + gaps.meter) as meter, 
max(final-start) as zazor, max(final-start) as Length, max(start) as start,
                files.threat_id as thread
                from rd_gaps as gaps
                inner join trip_files as files on files.id = gaps.file_id
                inner join trips as trip on trip.id = files.trip_id
                inner join adm_track as track on track.adm_direction_id=trip.direction_id
                where gaps.process_id =" + process_id + @" and (final-start) between 0 and 60
                group by track.code, gaps.nkm, gaps.picket, gaps.meter, thread , direct
                order by track.code " + orderby + ", gaps.nkm  " + orderby + ", gaps.picket  " + orderby + ", gaps.meter  " + orderby, commandType: CommandType.Text).ToList();
            }
        }

        public List<Gap> RDGetGap(long process_id, int direction, int side)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<Gap>($@"
						SELECT
	                        	km,
	                            meter,
	                            COALESCE ( MAX ( zazor ),- 999 ) AS zazor,
	                            COALESCE ( MAX ( r_zazor ),- 999 ) AS r_zazor,
	                            trip_id,
	                            MAX ( fnum ) AS fnum,
	                            MAX ( ms ) AS ms,
	                            MAX ( file_id ) AS file_id,
	                            COALESCE ( MAX ( zabeg ),- 999 ) AS zabeg 
                        FROM
	                        (
	                        SELECT DISTINCT COALESCE
		                        ( l.l_km, r.r_km ) AS km,
		                        l.zazor,
		                        r.r_zazor,
		                        COALESCE ( l_meter, r_meter ) AS meter,
		                        COALESCE ( l_ms, r_ms ) AS ms,
		                        COALESCE ( l_fnum, r_fnum ) AS fnum,
		                        COALESCE ( l_file_id, r_file_id ) AS file_id,
		                        COALESCE ( l_trip_id, r_trip_id ) AS trip_id,
		                        l.l_y - r.r_y AS zabeg
	                        FROM
		                        lside_gap l
		                        FULL OUTER JOIN rside_gap r ON l.l_km = r.r_km 
		                        AND r.r_meter = l.l_meter 
		                        AND r.r_trip_id = l.l_trip_id 
	                        WHERE
		                        l.l_trip_id = {process_id}
		                        OR r.r_trip_id = {process_id}
	                        ORDER BY
		                        km,
		                        meter
	                        ) AS report_gaps 
                        where km > 126
                        GROUP BY
	                        km,
	                        meter,
	                        trip_id  ", commandType: CommandType.Text).ToList();
                }
                catch(Exception e)
                {
                    Console.Out.WriteLine("RDGetGap error: " +e.Message);
                    return null;
                }
            }
        }
        public List<Gap> RDGetShpal(long process_id, int direction, int km)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Gap>($@"
                                        SELECT
	                                        * 
                                        FROM
	                                        (
	                                        SELECT
		                                        *,
		                                        LEAD ( koord, 1 ) OVER ( ORDER BY koord ) ncord,
		                                        ABS ( koord - LEAD ( koord, 1 ) OVER ( ORDER BY koord ) ) razn 
	                                        FROM
		                                        (
		                                        SELECT DISTINCT
			                                        km,
			                                        mtr AS meter,
			                                        round(
				                                        mtr * 1000.0 + (
					                                        ( SELECT travel_direction FROM trips WHERE ID = {process_id} ) * 
                                                ( local_fnum * 200.0 ) - ( SELECT car_position FROM trips WHERE ID = {process_id} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
				                                        ) 
			                                        ) AS koord,
			                                        oid 
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
			                                        AND tfile.trip_id = {process_id}
			
			                                        AND km = {km}
		                                        ORDER BY
			                                        koord 
		                                        ) DATA 
	                                        ) data2 
	                                        WHERE razn > 120 ", commandType: CommandType.Text).ToList();
            }
        }
        public List<Gap> RDGetShpalGap(long process_id, int direction, int km, int epur)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Gap>($@"
                                        SELECT
	                                        * 
                                        FROM
	                                        (
	                                        SELECT
		                                        km,
		                                        meter,
		                                        oid,
		                                        threat_id threat,
		                                        razn,
		                                        NAME,
		                                        next_name 
	                                        FROM
		                                        (
		                                        SELECT
			                                        *,
			                                        LEAD ( NAME, 1 ) OVER ( ORDER BY koord ) next_name 
		                                        FROM
			                                        (
			                                        SELECT
				                                        *,
				                                        LEAD ( koord, 1 ) OVER ( ORDER BY koord ) ncord,
				                                        ABS ( ABS ( koord - LEAD ( koord, 1 ) OVER ( ORDER BY koord ) ) - {epur / 2} ) razn,
			                                        CASE
					
					                                        WHEN oid IN ( 5, 6, 7, 8, 9, 10 ) THEN
					                                        'Стык' 
					                                        WHEN oid IN (
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
						                                        ) THEN
						                                        'Шпала' ELSE'не определено' 
				                                        END AS NAME 
				                                        FROM
					                                        (
					                                        SELECT DISTINCT
						                                        km,
						                                        mtr AS meter,
						                                        round(
							                                        mtr * 1000.0 + (
								                                        ( SELECT travel_direction FROM trips WHERE ID = {process_id} ) * ( local_fnum * 200.0 ) - 
                                                                                ( SELECT car_position FROM trips WHERE ID =  {process_id} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
							                                        ) 
						                                        ) AS koord,
						                                        oid,
						                                        threat_id 
					                                        FROM
						                                        RD_VIDEO_OBJECTS AS rvo
						                                        INNER JOIN trip_files AS tfile ON tfile.ID = rvo.file_Id 
					                                        WHERE
						                                        oid IN (
							                                        5,
							                                        6,
							                                        7,
							                                        8,
							                                        9,
							                                        10,
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
						                                        AND tfile.trip_id = {process_id} 
						                                        AND km = {km} 
						                                        AND threat_id = 1 
					                                        ORDER BY
						                                        koord 
					                                        ) DATA 
				                                        ) data2 
			                                        ) data3 
		                                        WHERE
			                                        ( NAME = 'Шпала' AND next_name = 'Стык' ) 
			                                        OR ( NAME = 'Стык' AND next_name = 'Шпала' ) 
		                                        ) dm1
		                                        FULL OUTER JOIN (
		                                        SELECT
			                                        km r_km,
			                                        meter r_meter,
			                                        oid r_oid,
			                                        threat_id r_threat,
			                                        razn r_razn,
			                                        NAME r_name,
			                                        next_name r_next_name 
		                                        FROM
			                                        (
			                                        SELECT
				                                        *,
				                                        LEAD ( NAME, 1 ) OVER ( ORDER BY koord ) next_name 
			                                        FROM
				                                        (
				                                        SELECT
					                                        *,
					                                        LEAD ( koord, 1 ) OVER ( ORDER BY koord ) ncord,
					                                        ABS ( ABS ( koord - LEAD ( koord, 1 ) OVER ( ORDER BY koord ) ) - {epur / 2} ) razn,
				                                        CASE
						
						                                        WHEN oid IN ( 5, 6, 7, 8, 9, 10 ) THEN
						                                        'Стык' 
						                                        WHEN oid IN (
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
							                                        ) THEN
							                                        'Шпала' ELSE'не определено' 
					                                        END AS NAME 
					                                        FROM
						                                        (
						                                        SELECT DISTINCT
							                                        km,
							                                        mtr AS meter,
							                                        round(
								                                        mtr * 1000.0 + (
									                                        ( SELECT travel_direction FROM trips WHERE ID =  {process_id} ) * ( local_fnum * 200.0 ) - 
                                                                                    ( SELECT car_position FROM trips WHERE ID =  {process_id} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
								                                        ) 
							                                        ) AS koord,
							                                        oid,
							                                        threat_id 
						                                        FROM
							                                        RD_VIDEO_OBJECTS AS rvo
							                                        INNER JOIN trip_files AS tfile ON tfile.ID = rvo.file_Id 
						                                        WHERE
							                                        oid IN (
								                                        5,
								                                        6,
								                                        7,
								                                        8,
								                                        9,
								                                        10,
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
							                                        AND tfile.trip_id =  {process_id} 
							                                        AND km = {km} 
							                                        AND threat_id = 2 
						                                        ORDER BY
							                                        koord 
						                                        ) DATA 
					                                        ) data2 
				                                        ) data3 
			                                        WHERE
				                                        ( NAME = 'Шпала' AND next_name = 'Стык' ) 
				                                        OR ( NAME = 'Стык' AND next_name = 'Шпала' ) 
			                                        ) dm2 ON dm2.r_km = dm1.km 
		                                        AND dm2.r_meter = dm1.meter 
	                                        AND dm2.r_NAME = dm1.NAME
                                        ", commandType: CommandType.Text).ToList();
            }
        }

        public List<Gap> GetFusGap(Int64 process_id, int direction)
        {
             using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string orderby = " asc ";
                return db.Query<Gap>(@"
                select track.code as put, trip.direction_id as direct, gaps.nkm as km, gaps.picket as picket, ((gaps.picket-1)*100 + gaps.meter) as meter, 
max(final-start) as zazor, max(final-start) as Length, max(start) as start,
                files.threat_id as thread
                from rd_gaps as gaps
                inner join trip_files as files on files.id = gaps.file_id
                inner join trips as trip on trip.id = files.trip_id
                inner join adm_track as track on track.adm_direction_id=trip.direction_id
                where gaps.process_id =" + process_id + @" and (final-start) between 0 and 60 and (final-start) = 0
                group by track.code, gaps.nkm, gaps.picket, gaps.meter, thread , direct
                order by track.code " + orderby + ", gaps.nkm  " + orderby + ", gaps.picket  " + orderby + ", gaps.meter  " + orderby, commandType: CommandType.Text).ToList();
            }
        }
        public List<Heat> GetHeats(Int64 trip_id, int kilometer)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
           
                return db.Query<Heat>(@"
                select km, case when threat = -1 then meter else nextmeter end as meter,
                      (meter*1000 + start) - (nextmeter*1000+nextstart) as value
                from (
                    select gaps.nkm as km, ((gaps.picket-1)*100 + gaps.meter) as meter, max(final-start) as length, max(start) as start, 
                        case when abs((coalesce(lead((gaps.picket-1)*100 + gaps.meter) OVER(ORDER BY  gaps.nkm, ((gaps.picket-1)*100 + gaps.meter)),-1) - ((gaps.picket-1)*100 + gaps.meter)))<=1 then 
	                    lead((gaps.picket-1)*100 + gaps.meter) OVER(ORDER BY  gaps.nkm, ((gaps.picket-1)*100 + gaps.meter)) ELSE -1 END	as nextmeter,
	                    case when abs((coalesce(lead((gaps.picket-1)*100 + gaps.meter) OVER(ORDER BY  gaps.nkm, ((gaps.picket-1)*100 + gaps.meter)),-1) - ((gaps.picket-1)*100 + gaps.meter)))<=1 then 
	                    coalesce(lead(max(start)) OVER(ORDER BY  gaps.nkm, ((gaps.picket-1)*100 + gaps.meter)),-1) ELSE -1 END as nextstart,
	                    case when abs((coalesce(lead((gaps.picket-1)*100 + gaps.meter) OVER(ORDER BY  gaps.nkm, ((gaps.picket-1)*100 + gaps.meter)),-1) - ((gaps.picket-1)*100 + gaps.meter)))<=1 then 
	                    coalesce(lead(files.threat_id) OVER(ORDER BY  gaps.nkm, ((gaps.picket-1)*100 + gaps.meter)),0) ELSE 0 END  as nextthreat,
                        files.threat_id as threat 
                    from rd_gaps as gaps
                        inner join trip_files as files on files.id = gaps.file_id
                        inner join trips as trip on trip.id = files.trip_id
                    where trip.id = " + trip_id + " and gaps.nkm = " + kilometer + @" and ((final-start) between 0 and 60) 
                        group by gaps.nkm, gaps.picket, gaps.meter, files.threat_id , trip.direction_id
                        order by gaps.nkm, ((gaps.picket-1)*100 + gaps.meter)) as steptable
				 where nextmeter != -1", commandType: CommandType.Text).ToList();
            }

        }
        public List<Gap> DirectName(long process_id, int direction_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<Gap>($@"
                SELECT distinct
                 --frag.id,
                concat(direction.name
                ,' (', direction.code, ' )') as name
                ,direction.code
                --,direction.id as direction_id
                ,track.code as put
                ,distance.code as pch


                FROM public.fragments as frag
                inner join (
                             select id, case when start_km < final_km then 1 else -1 end as direct from fragments
                            ) as frag_dir on frag_dir.id = frag.id
                inner join adm_track as track on track.id = frag.adm_track_id
                inner join adm_direction as direction on direction.id = track.adm_direction_id
                inner join generate_series(frag.start_km, frag.final_km, case when frag.start_km < frag.final_km then 1 else -1 end) as km on true
                inner join trips as trip on trip.id = frag.trip_id 
                left join (
	                        select adm_track_id,km, len, start_date, final_date 
		                       from tpl_nst_km as nest 
                               inner join tpl_period as nest_period on nest_period.id = nest.period_id   
                           ) as nonst on nonst.adm_track_id = frag.adm_track_id and nonst.km = km.km and trip.trip_date between nonst.start_date and nonst.final_date
                left join (
	                        select adm_track_id, km, start_date, final_date 
		                       from tpl_non_ext_km as non 
                               inner join tpl_period as non_period on non_period.id = non.period_id  
                           ) as nonexist on nonexist.adm_track_id = frag.adm_track_id and nonexist.km = km.km and trip.trip_date between nonexist.start_date and nonexist.final_date
					 
                INNER JOIN (
	                SELECT DISTINCT
		                period.adm_track_id,
		                period.start_date,
		                period.final_date,
		                dist.code,
		                dist.NAME,
		                dist.ID,
		                SECTION.start_km,
		                SECTION.final_km 
	                FROM
		                tpl_dist_section
		                AS SECTION INNER JOIN tpl_period AS period ON period.ID = SECTION.period_id
		                INNER JOIN adm_distance AS dist ON dist.ID = SECTION.adm_distance_id 
	                ) AS distance ON distance.adm_track_id = track.ID and km.km BETWEEN distance.start_km and distance.final_km
                    where trip.id = { process_id } and distance.id ={direction_id}", commandType: CommandType.Text).ToList();
            }
        }
		public List<int> GetKilometersByTripId(Int64 process_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<int>($@"select DISTINCT  kilom.number
                                    from kilometers as kilom
                                    inner join adm_track as track on track.id = kilom.track_id
                                    inner join adm_direction as direction on direction.id = track.adm_direction_id
                                    inner join bedemost as bed on bed.naprav = direction.name and kilom.number = bed.kmtrue
                                    inner join trips on trips.id = kilom.trip_id
                                    inner join rd_process process on process.trip_id = trips.id
                                    where process.id = {process_id} 
                                    order by kilom.number ", commandType: CommandType.Text).ToList();
            }
        }
		public List<int> GetKilometersByTripId(long process_id, long trackId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<int>($@"select DISTINCT  kilom.number
                                    from kilometers as kilom
                                    inner join adm_track as track on track.id = kilom.track_id and track.id = {trackId}
                                    inner join adm_direction as direction on direction.id = track.adm_direction_id
                                    inner join bedemost as bed on bed.naprav = direction.name and kilom.number = bed.kmtrue
                                    inner join trips on trips.id = kilom.trip_id
                                    inner join rd_process process on process.trip_id = trips.id
                                    where process.id = {process_id} 
                                    order by kilom.number ", commandType: CommandType.Text).ToList();
            }
        }
        public List<int> GetStationByTripId(long process_id, long trackId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                return db.Query<int>($@"select DISTINCT  station.name
                                    from kilometers as kilom
                                    inner join adm_station as station on id = kilom.track_id and name = {trackId}
                                    inner join adm_direction as direction on direction.id = track.adm_direction_id
                                    inner join bedemost as bed on bed.naprav = direction.name and kilom.number = bed.kmtrue
                                    inner join trips on trips.id = kilom.trip_id
                                    inner join rd_process process on process.trip_id = trips.id
                                    where process.id = {process_id} 
                                    order by kilom.number ", commandType: CommandType.Text).ToList();
            }
        }
        public CrossRailProfile vertIznos(int nkm)
        {
            var crossRailProfile = new CrossRailProfile();
            using (var file = new StreamReader("G:/work_shifrovka/dop/" + nkm.ToString() + "_1.add_dat", Encoding.GetEncoding(1251)))
            {
                crossRailProfile.NKm = nkm;
                string line = "";
                file.ReadLine();
                file.ReadLine();
                file.ReadLine();
                file.ReadLine();
                file.ReadLine();
                line = file.ReadLine();
                crossRailProfile.TravelDirection = line.Equals("Обратный")
                    ? Direction.Reverse
                    : Direction.Direct;
                file.ReadLine();
                file.ReadLine();
                file.ReadLine();


                while ((line = file.ReadLine()) != null) crossRailProfile.ParsevertIznos(line);
            }

            return crossRailProfile;
        }

        public List<int> GetKilometers(Int64 process_id, int direction)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string orderby = direction == 1 ? " asc " : " desc ";
                return db.Query<int>(@" select distinct gaps.nkm from rd_gaps as gaps
                                        inner join trip_files as files on files.id = gaps.file_id
                                        inner join trips as trip on trip.id = files.trip_id
                                        where gaps.process_id = " + process_id + @"
                                        group by gaps.nkm, gaps.picket, gaps.meter, files.threat_id , trip.direction_id
                                        order by gaps.nkm " + orderby , commandType: CommandType.Text).ToList();
            }
        }
        
        public List<CrosProf> GetCrossRailProfileFromDBbyKm_forPPWEB(int nkm, int index, int count, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                        return db.Query<CrosProf>($@"
                            SELECT DISTINCT
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
	                             iz_45_r,
                                 x_big_l,
                                 x_big_r
                                --, imp_left, 
                                --implen_left, 
                                --imp_right, 
                                --implen_right
                            FROM
	                            PUBLIC.profiledata_{trip_id}
                            WHERE
	                            km = {nkm}
	                          --  AND id > {index} 
                         -- 
                          -- ORDER BY
	                        --    id 
                           -- Limit {count}
                ", commandType: CommandType.Text).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetCrossRailProfileFromDBbyKm error: " + e.Message);
                    return new List<CrosProf> { };
                }
               

            }
        }
       
        public List<CrosProf> GetCrossRailProfileFromDBbyKm(int nkm, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<CrosProf>($@"
                            SELECT DISTINCT
	                            meter,
	                            AVG ( pu_l ) pu_l,
	                            AVG ( pu_r ) pu_r,
	                            AVG ( vert_l ) vert_l,
	                            AVG ( vert_r ) vert_r,
	                            AVG ( bok_l ) bok_l,
	                            AVG ( bok_r ) bok_r,
	                            AVG ( npk_l ) npk_l,
	                            AVG ( npk_r ) npk_r,
	                            AVG ( shortwavesleft ) shortwavesleft,
	                            AVG ( shortwavesright ) shortwavesright,
	                            AVG ( mediumwavesleft ) mediumwavesleft,
	                            AVG ( mediumwavesright ) mediumwavesright,
	                            AVG ( longwavesleft ) longwavesleft,
	                            AVG ( longwavesright ) longwavesright,
	                            AVG ( iz_45_l ) iz_45_l,
	                            AVG ( iz_45_r ) iz_45_r 
                             --   AVG ( x_big_l ) x_big_l ,
                               -- AVG ( x_big_r ) x_big_r 
                            FROM
	                            PUBLIC.profiledata_{trip_id}
                            WHERE
	                            km = {nkm}
	                            AND meter > 0 
                            GROUP BY
	                            meter 
                            ORDER BY
	                            meter DESC ", commandType: CommandType.Text).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetCrossRailProfileFromDBbyKm error: " + e.Message);
                    return new List<CrosProf> { };
                }


            }
        }
        public List<CrosProf> GetCrossRailProfileFromDBbyTripId(long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<CrosProf>($@"
                            SELECT DISTINCT
	                            km,
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
	                            iz_45_r,
                                 x_big_l ,
                                 x_big_r ,
                                imp_left,
                                imp_right,
                                implen_left,
                                implen_right
                            FROM
	                            PUBLIC.profiledata_{trip_id}
                            WHERE
	                            km > 0 
	                            AND meter > 0 
                            ORDER BY
	                            km DESC,
	                            meter DESC ", commandType: CommandType.Text).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetCrossRailProfileFromDBbyTripId error: " + e.Message);
                    return null;
                }

            }
        }
        public List<CrosProf> GetGaugeFromDB(int nkm, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<CrosProf>($@"
                            SELECT DISTINCT
	                            *
                            FROM
	                            PUBLIC.outdata_{trip_id}
                            WHERE
	                            km = {nkm} 
                            ORDER BY
	                            meter desc", commandType: CommandType.Text).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetGaugeFromDB error: " + e.Message);
                    return null;
                }

            }
        }

        public List<CrosProf> GetGaugeFromDBkmmter(int nkm, int meter, long trip_id)

        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<CrosProf>($@"
                            SELECT DISTINCT
	                            *
                            FROM
	                            PUBLIC.outdata_{trip_id}
                            WHERE
	                            km = {nkm} AND
                                meter = {meter}
                            ORDER BY
	                            meter desc", commandType: CommandType.Text).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetGaugeFromDB error: " + e.Message);
                    return null;
                }

            }
        }
        public List<CrosProf> GetCrossRailProfileFromDB(Curve curve, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<CrosProf>($@"SELECT
	                                                * 
                                                FROM
	                                                PUBLIC.profiledata_{trip_id} 
                                                WHERE
	                                                km * 1000+meter >= {curve.Start_Km * 1000 + curve.Start_M} 
	                                                AND km * 1000+meter <= {curve.Final_Km * 1000 + curve.Final_M}
                                                ORDER BY
	                                                km,
	                                                meter ", commandType: CommandType.Text).ToList();
                }
                catch(Exception e)
                {
                    Console.WriteLine("GetCrossRailProfileFromDB error: " + e.Message);
                    return null;
                }

            }
        }
        public List<CrosProf> GetCrossRailProfileDFPR3(long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<CrosProf>($@"SELECT
	                                                km,
	                                                (meter-1) / 100+1 AS piket,
	                                                AVG ( pu_l ) AS avg_pu_l,
	                                                SQRT ( VARIANCE( pu_l ) ) AS sko_pu_l,
	                                                AVG ( pu_r ) AS avg_pu_r,
	                                                SQRT ( VARIANCE( pu_r ) ) AS sko_pu_r,
	                                                AVG ( npk_l ) AS avg_npk_l,
	                                                SQRT ( VARIANCE( npk_l ) ) AS sko_npk_l,
	                                                AVG ( npk_r ) AS avg_npk_r,
	                                                SQRT ( VARIANCE( npk_r ) ) AS sko_npk_r 
                                                FROM
	                                                PUBLIC.profiledata_{trip_id} --WHERE km = 4
	                                            	where meter >=0
                                                GROUP BY
	                                                km,
	                                                piket
	
                                                ORDER BY
	                                                km,
	                                                piket ", commandType: CommandType.Text).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetCrossRailProfileDFPR3 error: " + e.Message);
                    return null;
                }

            }
        }
        public List<CrosProf> GetCrossRailProfileDFPR3Radius(long track_id, DateTime date_Vrem, int Km, int meter)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<CrosProf>($@"select st.radius from apr_curve curve
                                            inner join tpl_period period on period.id = curve.period_id
                                            inner join apr_stcurve st on st.curve_id = curve.id
                                            WHERE 
                                            adm_track_id = {track_id} -- track_id
                                            and '{date_Vrem}' BETWEEN period.start_date and period.final_date -- tripdate
                                            and numrange(coordinatetoreal(st.start_km, st.start_m), coordinatetoreal(st.final_km, st.final_m)) && numrange({Km} + (({meter}/100+1)-1)*100/10000.0,{Km} + {meter}/10000.0 + 0099.0) limit 1 -- координаты пикета формула км + нач.м/1000
	                                             ", commandType: CommandType.Text).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetCrossRailProfileDFPR3Radius error: " + e.Message);
                    return null;
                }

            }
        }
        public CrossRailProfile GetCrossRailProfileFromDBParse(List<CrosProf> crosProfs)
        {
            var crossRailProfile = new CrossRailProfile();
            foreach (var elem in crosProfs)
            {
                crossRailProfile.ParseDB(elem);
            }
            return crossRailProfile;
        }
       

        public CrossRailProfile GetCrossRailProfileFromText(int nkm)
        {
            var crossRailProfile = new CrossRailProfile();
            using (var file = new StreamReader(@"G:\work_shifrovka\dop\" + nkm.ToString() + "_1.add_dat", Encoding.ASCII))
            //using (var file = new StreamReader(@"G:\work_shifrovka\dop\km_700_4816.svgpdat", Encoding.ASCII))

            {
                crossRailProfile.NKm = nkm;
                string line = "";
                file.ReadLine();
                file.ReadLine();
                file.ReadLine();
                file.ReadLine();
                file.ReadLine();
                line = file.ReadLine();
                crossRailProfile.TravelDirection = line.Equals("Обратный")
                    ? Direction.Direct
                    : Direction.Reverse;
                file.ReadLine();
                file.ReadLine();
                file.ReadLine();

                
                while ((line = file.ReadLine()) != null) crossRailProfile.ParseTextLine(line);
            }

            return crossRailProfile;
        }
        public ShortRoughness GetShortRoughnessFromDBParse(List<CrosProf> DBcrossRailProfile)
        {
            var result = new ShortRoughness();
            foreach (var elem in DBcrossRailProfile)
            {
                result.ParseDB(elem);
                result.ParseDB2(elem);
            }
            //result.Correct();
            return result;
        }
        public ShortRoughness GetShortRoughnessFromText(int km)
        {
            var result = new ShortRoughness();
            string line;
            //чтение измерительных данных
            using (var file = new StreamReader("G:/work_shifrovka/dop/" + km.ToString() + ".svgpdat", Encoding.GetEncoding(1251)))
            {
                line = file.ReadLine();
                result.Direction = "Обратный";
                result.Car = new CarParameters { ChiefName = "CheffName", CurrentPosition = CarPosition.Boiler };
                //line = file.ReadLine();
                result.TrackNumber = line != null && line.Length <= 3 ? line : line != null && line.Equals(Resources.even) ? "2" : "1";
                result.TravelDirection = line != null && line.Equals(Resources.reverse) ? Direction.Direct : Direction.Reverse;
                //line = file.ReadLine();
                //result.TravelDate = DateTime.ParseExact(line, "dd.mm.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                result.TravelDate = DateTime.Now;
                result.Car.CarNumber = "0001";
                result.KilometrNumber = int.Parse(line ?? throw new InvalidOperationException());
                while ((line = file.ReadLine()) != null) result.Parse(line);
            }
            using (var file = new StreamReader("G:/work_shifrovka/dop/" + km.ToString() + "_2.svgpdat", Encoding.GetEncoding(1251)))
            {
                line = file.ReadLine();
                result.Direction = "Обратный";
                result.Car = new CarParameters { ChiefName = "CheffName", CurrentPosition = CarPosition.Boiler };
                //line = file.ReadLine();
                result.TrackNumber = line != null && line.Length <= 3 ? line : line != null && line.Equals(Resources.even) ? "2" : "1";
                result.TravelDirection = line != null && line.Equals(Resources.reverse) ? Direction.Direct : Direction.Reverse;
                //line = file.ReadLine();
                //result.TravelDate = DateTime.ParseExact(line, "dd.mm.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                result.TravelDate = DateTime.Now;
                result.Car.CarNumber = "0001";
                result.KilometrNumber = int.Parse(line ?? throw new InvalidOperationException());
                while ((line = file.ReadLine()) != null) result.Parse2(line);
            }

            result.Correct();


            return result;
        }

        public Bitmap GetFrame(int frameNumber, Int64 file_id)
        {

            string filePath = GetFilePathById(file_id);
            if (filePath == string.Empty)
                return null;
            
            //filePath = @"D:\common\DATA\IN\2019_04\2019_04_03__19_42_26.s1";
            //filePath = @"C:\Data\2019_04\2019_04_03__19_42_26.s1";
            var leftImageInnerSide = GetBitMap(filePath, frameNumber);
            //filePath = @"D:\common\DATA\IN\2019_04\2019_04_03__19_42_26.s2";
            //filePath = @"C:\DATA\2019_04\2019_04_03__19_42_26.s2";
            var leftImageOuterSide = GetBitMap(filePath, frameNumber);
            if ((leftImageInnerSide == null) || (leftImageOuterSide == null))
                    return null;
            leftImageInnerSide.RotateFlip(RotateFlipType.Rotate90FlipNone);
            leftImageOuterSide.RotateFlip(RotateFlipType.Rotate270FlipNone);
            Bitmap result = new Bitmap(leftImageInnerSide.Width + leftImageOuterSide.Width, leftImageInnerSide.Height);
            Graphics g = Graphics.FromImage(result);
            g.DrawImageUnscaled(leftImageOuterSide, 0, 0);
            g.DrawImageUnscaled(leftImageInnerSide, leftImageOuterSide.Width, 0);
            return result;
        }

        private string GetFilePathById(long file_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.QueryFirst<string>("SELECT file_name as file_path  FROM public.trip_files where id = " + file_id, commandType: CommandType.Text);
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetFilePathById error: " + e.Message);
                    return string.Empty;
                }
            }
        }
        private List<Dictionary<String,object>> getFilesPathById(long fileId)
        {
            var result = new List<Dictionary<String, object>>();
            using (NpgsqlConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    string sqlText =
                    @"select 
                            file_name as fileName, trip_files.id as fId,
                            case when description = 'StykiKupeVneshn' then 0
                                when description = 'StykiKupeVnutr' then 1
                                when description = 'SredCHastSHpaly' then 2
                                when description = 'StykiKoridorVnutr' then 3
                                when description = 'StykiKoridorVneshn' then 4 else 5 end as descId

                                  
                        from 
	                        trip_files 
		                        where trip_id = (select trip_id from trip_files where id = " + fileId + @") 
			                        and right(file_name,4) = (select right(file_name,4) from trip_files where id ="+fileId+@"  ) and description in (
				                        'StykiKupeVneshn','StykiKupeVnutr','SredCHastSHpaly','StykiKoridorVnutr','StykiKoridorVneshn'
			                        ) order by descId
                        ";
                    NpgsqlCommand cmd = new NpgsqlCommand(sqlText, db);
                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var fileName = reader.GetString(reader.GetOrdinal("fileName"));
                            var fId = reader.GetInt64(reader.GetOrdinal("fId"));
                            result.Add(new Dictionary<string, object>() { { "fileName", fileName }, { "fileId", fId} });
                        }
                        
                    }
                    return result;
                }
                catch (Exception e){
                    Console.Error.WriteLine("getFilesPathById error: " + e.Message);
                    return null;
                }
            }
        }
        public Bitmap GetBitMap(String filePath, int frameNumber)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                try
                {
                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();
                    int frameSize = width * height;
                    long position = (long)0 * (long)frameSize + 8;
                    reader.BaseStream.Seek(position, SeekOrigin.Begin);
                    byte[] by = reader.ReadBytes(frameSize);
                    var arr = by.Skip(8).Take(4).ToArray();
                    var km = BitConverter.ToInt32(by.Skip(40).Take(4).ToArray(), 0);
                    var encoderCounter_1 = BitConverter.ToInt32(arr, 0);
                    position = 10 * (long)frameSize + 8;
                    reader.BaseStream.Seek(position, SeekOrigin.Begin);
                    by = reader.ReadBytes(frameSize);
                    
                    var res = Convert2Bitmap(by, width, height);
                    return res;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("GetBitMap error: " + e.Message);
                    return null;
                }
            }
        }
        public List<Bitmap> GetBitMap(long fileId, long ms)
        {
            List<Bitmap> bitMaps = new List<Bitmap>();
            var filePaths = getFilesPathById(fileId);
            filePaths.ForEach(filePath =>
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePath["fileName"] as string, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    try
                    {
                        int width = reader.ReadInt32();
                        int height = reader.ReadInt32();
                        int frameSize = width * height;
                        long position = (long)ms * (long)frameSize + 8;
                        reader.BaseStream.Seek(position, SeekOrigin.Begin);
                        byte[] by = reader.ReadBytes(frameSize);
                        var arr = by.Skip(8).Take(4).ToArray();
                        //Array.Reverse(arr);
                        var encoderCounter_1 = BitConverter.ToInt32(arr, 0);
                        position = (long)(Math.Abs(ms) / 200) * (long)frameSize + 8;
                        reader.BaseStream.Seek(position, SeekOrigin.Begin);
                        by = reader.ReadBytes(frameSize);
                        encoderCounter_1 = BitConverter.ToInt32(by.Skip(8).Take(4).ToArray(), 0);
                        bitMaps.Add( Convert2Bitmap(by, width, height));
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("GetBitmap error: " + e.Message);
                        //return bitMaps;
                    }
                }
            });
            return bitMaps;
        }

        public int[,] getFilter(long fileId, long ms)
        {
            var filePath = getFilesPathById(fileId)[0];
            List<Dictionary<String, Object>> shapes = new List<Dictionary<string, object>>();
            using (BinaryReader reader = new BinaryReader(File.Open(filePath["fileName"] as string, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                try
                {
                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();
                    int frameSize = width * height;
                    long position = (long)0 * (long)frameSize + 8;
                    reader.BaseStream.Seek(position, SeekOrigin.Begin);
                    byte[] by = reader.ReadBytes(frameSize);
                    var arr = by.Skip(8).Take(4).ToArray();
                    var km = BitConverter.ToInt32(by.Skip(40).Take(4).ToArray(), 0);
                    var encoderCounter_1 = BitConverter.ToInt32(arr, 0);
                    position = (long)(Math.Abs(ms - encoderCounter_1) / 200) * (long)frameSize + 8;
                    reader.BaseStream.Seek(position, SeekOrigin.Begin);
                    by = reader.ReadBytes(frameSize);
                    byte[] first = new byte[width];
                    Array.Copy(by, 0, first, 0, width);
                    int sum = 0;
                    for (int i=0; i<width; i++)
                    {
                        sum += first[i];
                    }
                    int firstavg = sum / width;
                    byte[] last = new byte[width];
                    Array.Copy(by, by.Length-width, last, 0, width);
                    sum = 0;
                    for (int i = 0; i < width; i++)
                    {
                        sum += last[i];
                    }
                    int lastavg = sum / width;
                    int[,] matrix = new int[height, width];
                    for (int i=0; i<height; i++)
                    {
                        for (int j=0; j < width; j++)
                        {
                            matrix[i, j] = (firstavg + lastavg) / 2 + (lastavg - firstavg) / height * i;
                        }
                    }
                    Filter = matrix;
                    return matrix;
                }
                catch (Exception e)
                {
                    
                    System.Console.WriteLine("getBitmaps error: " + e.StackTrace);
                    return new int[0, 0];
                }
            }
        }

        private Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            red *= correctionFactor;
            green *= correctionFactor;
            blue *= correctionFactor;
            red = (red > 255 ? 255 : red);
            green = (green > 255 ? 255 : green);
            blue = (blue > 255 ? 255 : blue);
            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        public Dictionary<string,Object> getBitMaps(long fileId, long ms, int fnum, RepType RepType){
            List<Bitmap> bitMaps = new List<Bitmap>();
            var filePaths = getFilesPathById(fileId);
            List<Dictionary<String, Object>> shapes = new List<Dictionary<string, object>>();
            try
            {
                filePaths.ForEach(filePath =>
                {
                    using (BinaryReader reader = new BinaryReader(File.Open(filePath["fileName"] as string, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        try
                        {
                            int width = reader.ReadInt32();
                            int height = reader.ReadInt32();
                            int frameSize = width * height;
                            long position = (long)0 * (long)frameSize + 8;
                            reader.BaseStream.Seek(position, SeekOrigin.Begin);
                            byte[] by = reader.ReadBytes(frameSize);
                            var arr = by.Skip(8).Take(4).ToArray();
                            var km = BitConverter.ToInt32(by.Skip(40).Take(4).ToArray(), 0);
                            var encoderCounter_1 = BitConverter.ToInt32(arr, 0);
                            position = (long)(Math.Abs(ms - encoderCounter_1) / 200) * (long)frameSize + 8;
                            reader.BaseStream.Seek(position, SeekOrigin.Begin);
                            by = reader.ReadBytes(frameSize);

                            encoderCounter_1 = BitConverter.ToInt32(by.Skip(8).Take(4).ToArray(), 0);
                            if (encoderCounter_1 == ms)
                            {
                                Bitmap bitMap = Convert2Bitmap(by, width, height);
                                //if (Filter == null)
                                //{
                                //    byte[] first = new byte[width];
                                //    Array.Copy(by, 0, first, 0, width);
                                //    int sum = 0;
                                //    for (int i = 0; i < width; i++)
                                //    {
                                //        sum += first[i];
                                //    }
                                //    int firstavg = sum / width;
                                //    byte[] last = new byte[width];
                                //    Array.Copy(by, by.Length - width, last, 0, width);
                                //    sum = 0;
                                //    for (int i = 0; i < width; i++)
                                //    {
                                //        sum += last[i];
                                //    }
                                //    int lastavg = sum / width;
                                //    int[,] matrix = new int[height, width];
                                //    for (int i = 0; i < height; i++)
                                //    {
                                //        for (int j = 0; j < width; j++)
                                //        {
                                //            matrix[i, j] = (firstavg + lastavg) / 2 + (lastavg - firstavg) / height * i;
                                //        }
                                //    }
                                //    Filter = matrix;
                                //}
                                for (int row = 0; row < bitMap.Width; row++)
                                {
                                    for (int col = 0; col < bitMap.Height; col++)
                                    {
                                        Color pixel = bitMap.GetPixel(row, col);
                                        if (pixel.GetBrightness() > 120)
                                        {
                                            pixel = ChangeColorBrightness(pixel, 0.80f);
                                            bitMap.SetPixel(row, col, pixel);
                                        }
                                        else
                                        {
                                            pixel = ChangeColorBrightness(pixel, 1.2f);
                                            bitMap.SetPixel(row, col, pixel);
                                        }
                                    }
                                }




                                Graphics gr = Graphics.FromImage(bitMap);
                                using (Pen selPen = new Pen(Color.White))
                                {
                                    var objects = GetObjectsByFrameNumber((long)filePath["fileId"], ms, fnum, RepType);
                                    //var objects = GetObjectsByFrameNumber((long)filePath["fileId"], ms, -1);
                                    var sts = false;
                                    foreach (var vo in objects)
                                    {
                                        selPen.Color = RepType == RepType.Fastener ? Color.Red : (vo.Oid == (int)VideoObjectType.no_bolt ?
                                                                            Color.Red : vo.Oid == (int)VideoObjectType.bolt_M22 || vo.Oid == (int)VideoObjectType.bolt_M24 ?
                                                                            Color.Blue : Color.White);

                                        gr.DrawRectangle(selPen, vo.X - 25, vo.Y - 45, vo.W, vo.H);
                                    }
                                }

                                bitMaps.Add(bitMap);
                            }
                            else
                            {
                                position = (long)fnum * (long)frameSize + 8;
                                reader.BaseStream.Seek(position, SeekOrigin.Begin);
                                by = reader.ReadBytes(frameSize);
                                Bitmap bitMapL = Convert2Bitmap(by, width, height);
                                for (int row = 0; row < bitMapL.Width; row++)
                                {
                                    for (int col = 0; col < bitMapL.Height; col++)
                                    {
                                        Color pixel = bitMapL.GetPixel(row, col);
                                        if (pixel.GetBrightness() > 120)
                                        {
                                            pixel = ChangeColorBrightness(pixel, 0.80f);
                                            bitMapL.SetPixel(row, col, pixel);
                                        }
                                        else
                                        {
                                            pixel = ChangeColorBrightness(pixel, 1.2f);
                                            bitMapL.SetPixel(row, col, pixel);
                                        }
                                    }
                                }
                                Graphics grl = Graphics.FromImage(bitMapL);
                                using (Pen selPen = new Pen(Color.White))
                                {
                                    var objects = GetObjectsByFrameNumber((long)filePath["fileId"], -1, fnum, RepType);
                                    foreach (var vo in objects)
                                    {
                                        selPen.Color = RepType == RepType.Fastener ? Color.Red : (vo.Oid == 2 ? Color.Red : vo.Oid == 0 || vo.Oid == 1 ? Color.Blue : Color.White);

                                        grl.DrawRectangle(selPen, vo.X - 25, vo.Y - 45, vo.W, vo.H);
                                    }
                                }
                                bitMaps.Add(bitMapL);
                            }
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine("getBitmaps error: " + e.StackTrace);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("====>getBitMaps  Ошибка загрузки файла " + e.Message);
            }

            if (RepType == RepType.Gaps)
            {
                var d = new Dictionary<String, Object>
                {
                    { "name", "gap" },
                    { "thread", 1 },
                    { "x7", 10 },
                    { "y7", 10 },
                    { "w7", 10 },
                    { "h7", 20 }
                };
                shapes.Add(d);
            }

            var result = new Dictionary<String, Object>
            {
                { "drawShapes", shapes },
                { "bitMaps", bitMaps },
                { "filePaths",filePaths }
            };
            return result;
        }


        public Bitmap Convert2Bitmap(byte[] DATA, int width, int height)
        {

            var arr = Array.ConvertAll(DATA, Convert.ToInt32);
            var result = ConvertMatrix(arr, height, width).ToBitmap();

            return Crop(result);
        }
        //ToDo почему myImage.Height - 45
        public Bitmap Crop(Image myImage)
        {
            Bitmap croppedBitmap = new Bitmap(myImage);
            croppedBitmap = croppedBitmap.Clone(
                            new Rectangle(25, 45, myImage.Width-50, myImage.Height - 45),
                            System.Drawing.Imaging.PixelFormat.DontCare);
            return croppedBitmap;
        }
        static int[,] ConvertMatrix(int[] flat, int m, int n)
        {
            if (flat.Length != m * n)
            {
                throw new ArgumentException("Invalid length");
            }
            int[,] ret = new int[m, n];
            // BlockCopy uses byte lengths: a double is 8 bytes
            Buffer.BlockCopy(flat, 0, ret, 0, flat.Length * sizeof(Int32));
            return ret;
        }

        public List<VideoObject> GetObjectsByFrameNumber(int frame_Number, Int64 trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string sqlText =
                    @"select distinct rvo.*,cl.obj_name as name, tfile.threat_id as threat from public.rd_video_objects as rvo
                        inner join classes as cl on cl.class_id = rvo.oid
                        inner join trip_files as tfile on tfile.id = rvo.fileId  where fnum = " +
                    frame_Number + " and rvo.trip_id = " + trip_id;
                return db.Query<VideoObject>(sqlText, commandType: CommandType.Text).ToList();

            }
        }

        public List<Gap> GetGapsByFrameNumber(int frame_Number, Int64 trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<Gap>(@"
                select gaps.nkm as km, max(frame_number) as frame_number, ((gaps.picket-1)*100 + gaps.meter) as meter, max(final-start) as length, max(start) as start, 
                    files.threat_id as threat, speed.passenger as passspeed, speed.freight as FreightSpeed, files.id as file_id
                from rd_gaps as gaps
                    inner join trip_files as files on files.id = gaps.file_id
                    inner join trips as trip on trip.id = files.trip_id
                    inner join tpl_period as sp on trip.trip_date between sp.start_date and sp.final_date
				    inner join apr_speed as speed on speed.period_id = sp.id
                where trip.id = " + trip_id + " and gaps.frame_number =" + frame_Number + @"  and (final-start) between 0 and 60
                    group by gaps.nkm, gaps.picket, gaps.meter, files.threat_id , trip.direction_id, files.id, speed.passenger, speed.freight
                    order by gaps.nkm, ((gaps.picket-1)*100 + gaps.meter) ", commandType: CommandType.Text).ToList();
            }
        }
        
        public System.Drawing.Bitmap MatrixToTimage(int[,] matrix)
        {
            
            return matrix.ToBitmap();
        }

        public List<RailFastener> GetRailFasteners(long tripId, int kilometer)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                var videoObjects = db.Query<VideoObject>(
                    $@"select distinct rvo.*, cl.obj_name as name, tfile.threat_id as threat from public.rd_video_objects as rvo
                         inner join classes as cl on cl.class_id = rvo.oid
                         inner join trip_files as tfile on tfile.id = rvo.fileId 
                        where rvo.oid in (
                                            {(int)VideoObjectType.D65}, 
                                            {(int)VideoObjectType.GBR},
                                            {(int)VideoObjectType.KB65},
                                            {(int)VideoObjectType.SKL},
                                            {(int)VideoObjectType.P350},
                                            {(int)VideoObjectType.KD65},
                                            {(int)VideoObjectType.kpp}
                                            ) and rvo.trip_id = {tripId} and rvo.km <= {kilometer}
                        order by rvo.km, threat, rvo.mtr").ToList();
                var result = new List<RailFastener>();
                foreach (var videoObject in videoObjects)
                {
                    var serialized = JsonConvert.SerializeObject(videoObject);
                    switch ((FastenerEnum)videoObject.Oid)
                    {
                        case FastenerEnum.D65:
                            result.Add(JsonConvert.DeserializeObject <D65>(serialized));
                            break;
                        case FastenerEnum.GBR:
                            result.Add(JsonConvert.DeserializeObject<GBR>(serialized));
                            break;
                        case FastenerEnum.KB65:
                            result.Add(JsonConvert.DeserializeObject<KB65>(serialized));
                            break;
                        case FastenerEnum.SKL:
                            result.Add(JsonConvert.DeserializeObject<SKL>(serialized));
                            break;
                        case FastenerEnum.P350:
                            result.Add(JsonConvert.DeserializeObject<SKL>(serialized));
                            break;
                        case FastenerEnum.KD65:
                            result.Add(JsonConvert.DeserializeObject<KD65>(serialized));
                            break;
                        case FastenerEnum.kpp:
                            result.Add(JsonConvert.DeserializeObject<SKL>(serialized));
                            break;
                        default:
                            break;
                    }
                }
                return result;
            }
        }

        public List<VideoObject> GetObjectsByFrameNumber(long fileId, long ms = -1, int fnum = -1, RepType Type = RepType.Undefined)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                var filter = " AND rvo.oid in ";

                switch (Type)
                {
                    case RepType.Undefined:
                        filter = "";
                        break;
                    case RepType.Gaps:
                        filter += $"({(int)VideoObjectType.GapFull})";
                        break;
                    case RepType.Bolt:
                        filter +=  $"(" +
                                   $"{ (int)VideoObjectType.no_bolt}" +
                                   $")";
                        break;
                    case RepType.Fastener:
                        filter +=   $"(" +
                                    $"{(int)VideoObjectType.D65_NoPad}," +
                                    $"{ (int)VideoObjectType.D65_MissingSpike}," +
                                    $"{ (int)VideoObjectType.KB65_NoPad}," +
                                    $"{ (int)VideoObjectType.KB65_MissingClamp}," +
                                    $"{ (int)VideoObjectType.SklNoPad}," +
                                    $"{ (int)VideoObjectType.SklBroken}," +
                                    $"{ (int)VideoObjectType.GBRNoPad}," +
                                    $"{ (int)VideoObjectType.WW}," +
                                    $"{ (int)VideoObjectType.KD65NB}," +
                                    $"{ (int)VideoObjectType.KppNoPad}" +
                                    $")";
                        break;

                    default:
                        filter = "";
                        break;
                }

                return db.Query<VideoObject>(
                    $@"SELECT DISTINCT
	                        rvo.oid,
	                        rvo.fnum,
	                        rvo.km,
	                        rvo.pt,
	                        rvo.mtr,
	                        rvo.x,
	                        rvo.y,
	                        rvo.w,
	                        rvo.h,
	                        rvo.ms,
	                        rvo.file_id,
	                        tf.threat_id AS Threat,
	                        tf.side_id AS Side,
	                        tf.file_name 
                        FROM
	                        rd_video_objects AS rvo
	                        INNER JOIN trip_files tf ON tf.ID = rvo.file_id 
                        WHERE
	                        rvo.file_id = {fileId} 
                            -- AND rvo.ms = {ms} 
	                        AND rvo.fnum = {fnum} 
	                        {filter} --AND rvo.oid = 7 
                        ORDER BY
	                        oid ").ToList();
            }
        }

        public CarPosition GetCarPositionByFile(long fileId)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();


                return db.QueryFirst<CarPosition>(
                    $@"SELECT car_position FROM trips
                    inner join trip_files as file on file.trip_id = trips.id
                     where file.id = {fileId}");
            }
            throw new NotImplementedException();
            
        }

        public List<CrosProf> GetCrossRailProfileDFPR3Radius(int kilometer, long trip_id)
        {
            throw new NotImplementedException();
        }

        public List<Digression> GetImpulsesByKm(int kilometer)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<Digression>($@"SELECT
	                                                    * , threat_id threat, len length
                                                    FROM
	                                                    impulses_ 
                                                    WHERE
	                                                    intensity_RA >=5
	                                                    and km = {kilometer}", commandType: CommandType.Text).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetImpulsesByKm: " + e.Message);

                    return null;
                }

            }
        }

        public List<Digression> GetFullGapsByNN(long km, long trip_id, string query = "")
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var txt = $@"SELECT
	                                                    km,
	                                                    mtr meter,
	                                                    round( koord ) koord,
	                                                    oid,
	                                                    file_id Fileid,
	                                                    fnum,
	                                                    ms,
	                                                    file_name,
	                                                    threat_id threat, x,y,w,h
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
					                                                    ( SELECT travel_direction FROM trips WHERE ID = {trip_id} ) * ( local_fnum * 200.0 ) - ( SELECT car_position FROM trips WHERE ID = {trip_id} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
				                                                    ) AS koord 
			                                                    FROM
				                                                    rd_video_objects rvo
				                                                    INNER JOIN trip_files tf ON tf.ID = rvo.file_id 
			                                                    WHERE
				                                                    file_id in ( {query} )
				                                                    AND oid IN ( {(int)VideoObjectType.GapFull} ) 
				                                                    AND threat_id = {(int)Threat.Left}
			                                                    ORDER BY
				                                                    koord 
			                                                    ) data1 
		                                                    ) data2 
	                                                    WHERE
		                                                    razn > 3200  or razn IS NULL
	                                                    ) gaps_left UNION ALL
                                                    SELECT
	                                                    km,
	                                                    mtr meter,
	                                                    round( koord ) koord,
	                                                    oid,
	                                                    file_id Fileid,
	                                                    fnum,
	                                                    ms,
	                                                    file_name,
	                                                    threat_id threat, x,y,w,h
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
					                                                    ( SELECT travel_direction FROM trips WHERE ID = {trip_id} ) * ( local_fnum * 200.0 ) - ( SELECT car_position FROM trips WHERE ID = {trip_id} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
				                                                    ) AS koord 
			                                                    FROM
				                                                    rd_video_objects rvo
				                                                    INNER JOIN trip_files tf ON tf.ID = rvo.file_id 
			                                                    WHERE
				                                                    file_id in ( {query} ) 
				                                                    AND oid IN ( {(int)VideoObjectType.GapFull} ) 
				                                                    AND threat_id = {(int)Threat.Right}
			                                                    ORDER BY
				                                                    koord 
			                                                    ) data1 
		                                                    ) data2 
	                                                    WHERE
	                                                    razn > 3200  or razn IS NULL
	                                                    ) gaps_right
	
	                                                    ORDER BY koord";
                    var gaps = db.Query<Digression>(txt, commandType: CommandType.Text).ToList();

                    GapProcessing(gaps, km, trip_id);

                    return gaps;
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetImpulsesByKm: " + e.Message);

                    return null;
                }

            }
        }

        private void GapProcessing(List<Digression> gaps, long km, long trip_id)
        {
            try
            {
                foreach (var gap in gaps)
                {
                    var reader = new BinaryReader(File.Open(gap.File_name, FileMode.Open));


                    try
                    {


                        int widthF = reader.ReadInt32();
                        int heightF = reader.ReadInt32();
                        int frameSize = widthF * heightF;
                        long position = gap.Fnum * (long)frameSize + 8;
                        reader.BaseStream.Seek(position, SeekOrigin.Begin);
                        byte[] by = reader.ReadBytes(frameSize);

                        var mtx1 = ConvertMatrix(Array.ConvertAll(by, Convert.ToInt32), heightF, widthF);
                        var frame = mtx1.ToRedBitmap();
                        frame = frame.Clone(new Rectangle(gap.X + (int)(gap.W * 0.4), gap.Y - 2, gap.W - (int)(gap.W * 0.9), gap.H + 4), frame.PixelFormat);
                        var mtx = frame.ToRedMatrix();

                        var width = frame.Width;
                        var height = frame.Height;

                        var param1 = 0;
                        var param2 = 0;

                        double[] graphic = new double[height - (param1 + param2)];
                        double[] graphic_new = new double[height - (param1 + param2)];

                        for (int y = 0; y < height; y++) //heigth
                        {
                            var s1 = 0.0;
                            for (int x = param1; x < width - param2; x++)
                            {
                                s1 += mtx[y, x];
                            }
                            graphic_new[y] = s1;
                        }
                        for (int i = 0; i < height; i++)
                        {
                            graphic[i] = Math.Exp(3 * graphic_new[i] / graphic_new.Average());
                        }

                        var Gap_len = VectorToPoints(graphic, gap);

                        if (Gap_len > gap.H)
                        {
                            if (gap.H - 4 > 0)
                            {
                                gap.Zazor = gap.H - 4;
                            }
                            else
                            {
                                gap.Zazor = gap.H;
                            }
                        }
                        else
                        {
                            if(Gap_len < gap.H - 10)
                                gap.Zazor = gap.H - 10;
                            else
                                gap.Zazor = Gap_len;
                        }
                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        gap.Zazor = -1;
                        Console.WriteLine("GapProcessing: " + e.Message);
                        reader.Close();
                    }
                }
            }            
            catch (Exception e)
            {
                Console.WriteLine("GapProcessing: " + e.Message);
            }
        }

        private int VectorToPoints(double[] vector, Digression gap)
        {
            var center = false;

            var beforeCenter = new List<double> { };
            var afterCenter = new List<double> { };

            for (int i = 0; i < vector.Length; i++)
            {
                if (!center)
                {
                    beforeCenter.Add(vector[i]);
                }
                else
                {
                    afterCenter.Add(vector[i]);
                }

                if (i == vector.Length / 2)
                    center = true;
            }

            var before_value = beforeCenter.Max();

            var before_count = 0;
            for (int b = vector.Length / 2; b > 0; b--)
            {
                if (before_value * 0.9 < vector[b])
                    break;

                before_count++;
            }

            var after_value = afterCenter.Max();

            var after_count = 0;
            for (int a = vector.Length / 2; a < vector.Length; a++)
            {
                if (after_value * 0.9 < vector[a])
                    break;

                after_count++;
            }
            var gap_len = before_count + after_count;


            return gap_len;
        }

        public void Insert_ViolPerpen(Kilometer km, List<RailsBrace> skreplenie, List<Digression> violPerpen)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                int i = 1;
                try
                {
                    var pass_speed = km.PdbSection.Count > 0 ? km.Speeds.First().Passenger : -1;
                    var fr_speed = km.PdbSection.Count > 0 ? km.Speeds.First().Freight : -1;

                    var vdop = $"{pass_speed}/{fr_speed}";

                    foreach (var item in violPerpen)
                    {
                        var txt = $@"INSERT INTO report_violperpen (
	                                        trip_id,
	                                        km,
	                                        meter,
	                                        vdop,
	                                        angle,
	                                        fastener,
	                                        file_id,
	                                        fnum,
	                                        ms                                         
                                        )
                                        VALUES
	                                       (
                                            '{km.Trip.Id}', 
                                            '{item.Km}', 
                                            '{item.Meter}',
                                            '{vdop}',
                                            '{item.Angle.ToString("0.00").Replace(",",".")}',
                                            '{(skreplenie.Any() ? skreplenie.First().Name : "не определено")}',
                                            '{item.file_id}',
                                            '{item.Fnum}',
                                            '{item.Ms}');";
                        db.Execute(txt);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка записи в БД Insert_ViolPerpen" + e.Message);
                }

            }
        }

        public List<Digression> Check_ViolPerpen(long trip_id)
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
       

        public object GetAddParam(long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {

                    var txt = $@"SELECT
	                                * , m meter
                                FROM
	                                s3_additional 
                                WHERE
	                                trip_id = {trip_id}";

                    return db.Query<S3>(txt).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetAddParam error: " + e.Message);

                    return new List<S3>();
                }

            }
        }

        public List<CrosProf> GetCrossRailProfileFromTrip(long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<CrosProf>($@"
                            SELECT DISTINCT
	                            km,
	                            meter,
	                            AVG ( pu_l ) pu_l,
	                            AVG ( pu_r ) pu_r,
	                            AVG ( vert_l ) vert_l,
	                            AVG ( vert_r ) vert_r,
	                            AVG ( bok_l ) bok_l,
	                            AVG ( bok_r ) bok_r,
	                            AVG ( npk_l ) npk_l,
	                            AVG ( npk_r ) npk_r,
	                            AVG ( shortwavesleft ) shortwavesleft,
	                            AVG ( shortwavesright ) shortwavesright,
	                            AVG ( mediumwavesleft ) mediumwavesleft,
	                            AVG ( mediumwavesright ) mediumwavesright,
	                            AVG ( longwavesleft ) longwavesleft,
	                            AVG ( longwavesright ) longwavesright,
	                            AVG ( iz_45_l ) iz_45_l,
	                            AVG ( iz_45_r ) iz_45_r 
                            FROM
	                            PUBLIC.profiledata_{trip_id}
                            WHERE
	                            km > 0 
	                            AND meter > 0 
                            GROUP BY
	                            km,
	                            meter 
                            ORDER BY
	                            km,
	                            meter", commandType: CommandType.Text).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetCrossRailProfileFromTrip error: " + e.Message);
                    return null;
                }

            }
        }
        public List<Digression> Insert_additional_param_state(List<Digression> addDigressions, int nkm)
        {
            int index = 0;
            foreach (var adddig in addDigressions)
            {
                try
                {

                    using (var db = new NpgsqlConnection(Helper.ConnectionString()))
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var txt = $@"INSERT INTO s3_additional (km,
                                                                kmetr,
                                                                meter,
                                                                typ,
                                                                digname,
                                                                direction_num,
                                                                founddate,
                                                                threat,
                                                                r_threat,
                                                                LENGTH,
                                                                LOCATION,
                                                                norma,
                                                                r_digname,
                                                                VALUE,
                                                                COUNT,
                                                                allowspeed,
                                                                primech 
                                                                )
                                                 VALUES ({nkm}, {adddig.Kmetr}, {adddig.Meter}, {adddig.Typ}, '{adddig.DigName}', {adddig.Direction_num}, '{adddig.FoundDate}', '{adddig.Threat}', '{adddig.R_threat}', {adddig.Length}, '{adddig.Location}', '{adddig.Norma}', '{adddig.R_DigName}', {adddig.Value}, {adddig.Count}, '{adddig.AllowSpeed}', '{adddig.Primech}')";
                        db.Execute(txt);
                    }
                    index++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert_additional_param_state error: " + e.Message);
                    return new List<Digression> { };
                }
            }
            return addDigressions;
        }


        

        public List<DataFlow> GetShortRough(long trip_id, int number)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var query = $@"SELECT * FROM testdata_{trip_id} where km = {number}  ORDER BY  meter  ";
                    return db.Query<DataFlow>(query, commandType: CommandType.Text).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetCrossRailProfileFromTrip error: " + e.Message);
                    return new List<DataFlow> { };
                }
            }
        }



        public List<CrosProf> GetCrossRailProfileFromDBbyCurve(Curve curve, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var QUERY = $@"
                            SELECT DISTINCT
	                            km + meter / 10000.0 koord,
	                            km,
	                            meter,
                                (meter - 1) / 100 + 1 as picket,
	                            AVG ( pu_l ) pu_l,
	                            AVG ( pu_r ) pu_r,
	                            AVG ( vert_l ) vert_l,
	                            AVG ( vert_r ) vert_r,
	                            AVG ( bok_l ) bok_l,
	                            AVG ( bok_r ) bok_r,
	                            AVG ( npk_l ) npk_l,
	                            AVG ( npk_r ) npk_r,
	                            AVG ( shortwavesleft ) shortwavesleft,
	                            AVG ( shortwavesright ) shortwavesright,
	                            AVG ( mediumwavesleft ) mediumwavesleft,
	                            AVG ( mediumwavesright ) mediumwavesright,
	                            AVG ( longwavesleft ) longwavesleft,
	                            AVG ( longwavesright ) longwavesright,
	                            AVG ( iz_45_l ) iz_45_l,
	                            AVG ( iz_45_r ) iz_45_r 
                            FROM
	                            PUBLIC.profiledata_{trip_id}
                            WHERE
	                            km + meter / 10000.0 BETWEEN {Math.Min(curve.RealStartCoordinate, curve.RealFinalCoordinate).ToString().Replace(",",".")} 
                                                         and {Math.Max(curve.RealStartCoordinate, curve.RealFinalCoordinate).ToString().Replace(",", ".")}
	                            AND meter > 0 
                            GROUP BY
	                            km,
	                            meter 
                            ORDER BY
	                            km ,
	                            meter ";
                    return db.Query<CrosProf>(QUERY, commandType: CommandType.Text).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetCrossRailProfileFromTrip error: " + e.Message);
                    return new List<CrosProf> { };
                }

            }
        }

        public List<Digression> Insert_additional_param_state_longwawes(List<Digression> impulses)
        {
            int index = 0;
            foreach (var imp in impulses)
            {
                try
                {

                    using (var db = new NpgsqlConnection(Helper.ConnectionString()))
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var txt = $@"INSERT INTO s3_additional (
                                    km,
                                    kmetr,
                                    meter,
                                    typ,
                                    digname,
                                    direction_num,
                                    founddate,
                                    threat,
                                    r_threat,
                                    LENGTH,
                                    LOCATION,
                                    norma,
                                    r_digname,
                                    VALUE,
                                    COUNT,
                                    allowspeed,
                                    primech 
                                    )
                     VALUES ({imp.Km}, {imp.Kmetr}, {imp.Meter}, {3}, '{imp.DigName}', {imp.Direction_num}, '{imp.FoundDate}', '{imp.Threat}', '{imp.R_threat}', {imp.Length}, '{imp.Location}', '{imp.Norma}', '{imp.R_DigName}', {imp.Intensity_ra.ToString("0.00")}, {imp.Count}, '{imp.AllowSpeed}', '{imp.Primech}')";
                        db.Execute(txt);
                    }
                    index++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert_additional_param_state_longwawes error: " + e.Message);
                    return new List<Digression> { };
                }
            }
            return impulses;
        }

        public List<Digression> Insert_additional_param_state_aslan(List<Digression> addDigressions, long trip_id)
        {
            int index = 0;
            foreach (var adddig in addDigressions)
            {
                try
                {

                    using (var db = new NpgsqlConnection(Helper.ConnectionString()))
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var txt = $@"INSERT INTO s3_additional (
                                                                km,
                                                                kmetr,
                                                                meter,
                                                                typ,
                                                                digname,
                                                                direction_num,
                                                                founddate,
                                                                threat,
                                                                r_threat,
                                                                LENGTH,
                                                                LOCATION,
                                                                norma,
                                                                r_digname,
                                                                VALUE,
                                                                COUNT,
                                                                allowspeed,
                                                                primech, 
                                                                trip_id
                                                                )
                                                 VALUES ({adddig.Km}, {adddig.Kmetr}, {adddig.Meter}, {3}, '{adddig.DigName}', {adddig.Direction_num}, '{adddig.FoundDate}', '{adddig.Threat}', '{adddig.R_threat}', {adddig.Length}, '{adddig.Location}', '{adddig.Norma}', '{adddig.R_DigName}', {adddig.Value}, {adddig.Count}, '{adddig.AllowSpeed}', '{adddig.Primech}', {trip_id})";
                        db.Execute(txt);
                    }
                    index++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert_additional_param_state error: " + e.Message);
                    return new List<Digression> { };
                }
            }
            return addDigressions;
        }


        public List<Digression> GetFullGapsByNN(long km, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var txt = $@"SELECT
	                                km,
	                                mtr meter,
	                                round( koord ) koord,
	                                oid,
	                                file_id Fileid,
	                                fnum,
	                                ms,
	                                file_name,
	                                threat_id threat, x,y,w,h
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
					                                ( SELECT travel_direction FROM trips WHERE ID = {trip_id} ) * ( local_fnum * 200.0 ) - ( SELECT car_position FROM trips WHERE ID = {trip_id} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
				                                ) AS koord 
			                                FROM
				                                rd_video_objects rvo
				                                INNER JOIN trip_files tf ON tf.ID = rvo.file_id 
			                                WHERE
				                                km = {km} 
				                                AND oid = {(int)VideoObjectType.GapFull}
				                                AND threat_id = {(int)Threat.Left}
                                                and trip_id = {trip_id}
			                                ORDER BY
				                                koord 
			                                ) data1 
		                                ) data2 
	                                WHERE
		                                razn > 3200  or razn IS NULL
	                                ) gaps_left UNION ALL
                                SELECT
	                                km,
	                                mtr meter,
	                                round( koord ) koord,
	                                oid,
	                                file_id Fileid,
	                                fnum,
	                                ms,
	                                file_name,
	                                threat_id threat, x,y,w,h
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
					                                ( SELECT travel_direction FROM trips WHERE ID = {trip_id} ) * ( local_fnum * 200.0 ) - ( SELECT car_position FROM trips WHERE ID = {trip_id} ) * ( y - ( 320.0 / 2.0 ) ) / 1.2 
				                                ) AS koord 
			                                FROM
				                                rd_video_objects rvo
				                                INNER JOIN trip_files tf ON tf.ID = rvo.file_id 
			                                WHERE
				                                km = {km} 
				                                AND oid = {(int)VideoObjectType.GapFull} 
				                                AND threat_id = {(int)Threat.Right}
                                                and trip_id = {trip_id}
			                                ORDER BY
				                                koord 
			                                ) data1 
		                                ) data2 
	                                WHERE
	                                razn > 3200  or razn IS NULL
	                                ) gaps_right
	
	                                ORDER BY koord";
                    var gaps = db.Query<Digression>(txt, commandType: CommandType.Text).ToList();

                    GapProcessing(gaps, km, trip_id);

                    return gaps;
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetImpulsesByKm: " + e.Message);

                    return null;
                }

            }
        }
    }
}
