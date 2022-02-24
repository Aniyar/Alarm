using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ALARm.Core;
using ALARm.Services;
using Microsoft.VisualBasic.FileIO;

namespace ALARm
{
    public static class ImportAndExport
    {
        internal static bool Export(MetroFramework.Controls.MetroProgressBar progressBar, long admRoadId, DateTime date)
        {
            string roadName = "";
            if (admRoadId != -1)
                roadName = ExportImportService.ExportQueryReturnString($@"select name from adm_road where id = {admRoadId}");
            else
                roadName = "all";
            string dirName = System.IO.Directory.GetCurrentDirectory() + "\\export_ALARm\\" + Transliter.cyrToLat(roadName) + "_" + date.ToString("dd_MM_yyyy");
            DirectoryInfo dirInfo = new DirectoryInfo(dirName);

            if (!dirInfo.Exists)
                dirInfo.Create();

            progressBar.Value = 0;
            progressBar.Step = 100 / 50;
            bool result = true;
            if (admRoadId != -1)
            {
                // ADM_STRUCTURE
                result &= ExportImportService.Execute($@"COPY (SELECT * FROM ADM_ROAD where id = {admRoadId}) TO '{dirName + "\\adm_road.csv"}' with CSV delimiter ',' header;");

                result &= ExportImportService.Execute($@"COPY (SELECT * FROM adm_nod where adm_road_id = {admRoadId}) TO '{dirName + "\\adm_nod.csv"}' with CSV delimiter ',' header;");
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT distance.* FROM adm_distance distance
                    inner join adm_nod nod on nod.id = distance.adm_nod_id
                    where nod.adm_road_id = {admRoadId}) TO '{dirName + "\\adm_distance.csv"}' with CSV delimiter ',' header;");
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT pchu.* FROM adm_pchu pchu
                    inner join adm_distance distance on distance.id = pchu.adm_distance_id
                    inner join adm_nod nod on nod.id = distance.adm_nod_id
                    where nod.adm_road_id = {admRoadId}) TO '{dirName + "\\adm_pchu.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT pd.* FROM adm_pd pd
                    inner join adm_pchu pchu on pchu.id = pd.adm_pchu_id
                    inner join adm_distance distance on distance.id = pchu.adm_distance_id
                    inner join adm_nod nod on nod.id = distance.adm_nod_id
                    where nod.adm_road_id = {admRoadId}) TO '{dirName + "\\adm_pd.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT pdb.* FROM adm_pdb pdb
                    inner join adm_pd pd on pd.id = pdb.adm_pd_id
                    inner join adm_pchu pchu on pchu.id = pd.adm_pchu_id
                    inner join adm_distance distance on distance.id = pchu.adm_distance_id
                    inner join adm_nod nod on nod.id = distance.adm_nod_id
                    where nod.adm_road_id = {admRoadId}) TO '{dirName + "\\adm_pdb.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT rd.* from road_direction rd
                    where rd.road_id = {admRoadId}) TO '{dirName + "\\road_direction.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT direction.* from adm_direction direction
                    inner join road_direction rd on rd.direction_id = direction.id
                    where rd.road_id = {admRoadId}) TO '{dirName + "\\adm_direction.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT track.* from adm_track track
                    where track.adm_direction_id in (select direction.id from adm_direction direction
                        inner join road_direction rd on rd.road_id = {admRoadId} and rd.direction_id = direction.id)
                    or track.id in (select strack.adm_track_id from stw_track strack
                        inner join adm_nod nod on nod.adm_road_id = {admRoadId}
                        inner join adm_station station on station.adm_nod_id = nod.id and station.id = strack.adm_station_id)
                    or track.id in (select strack.adm_track_id from stw_park_track strack
                        inner join adm_nod nod on nod.adm_road_id = {admRoadId}
                        inner join adm_station station on station.adm_nod_id = nod.id 
                        inner join stw_park park on park.adm_station_id = station.id and park.id = strack.stw_park_id)
                    ) TO '{dirName + "\\adm_track.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT station.* FROM adm_station station
                    inner join adm_nod nod on nod.id = station.adm_nod_id
                    where nod.adm_road_id = {admRoadId}) TO '{dirName + "\\adm_station.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT park.* FROM stw_park park
                    inner join adm_station station on station.id = park.adm_station_id
                    inner join adm_nod nod on nod.id = station.adm_nod_id
                    where nod.adm_road_id = {admRoadId}) TO '{dirName + "\\stw_park.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT sobject.* from stw_object sobject
                    inner join stw_park park on park.id = sobject.stw_park_id
                    inner join adm_station station on station.id = park.adm_station_id
                    inner join adm_nod nod on nod.id = station.adm_nod_id
                    where nod.adm_road_id = {admRoadId}) TO '{dirName + "\\stw_object.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT strack.* FROM stw_track strack
                    inner join adm_station station on station.id = strack.adm_station_id
                    inner join adm_nod nod on nod.id = station.adm_nod_id
                    where nod.adm_road_id = {admRoadId}) TO '{dirName + "\\stw_track.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT strack.* FROM stw_park_track strack
                    inner join stw_park park on park.id = strack.stw_park_id
                    inner join adm_station station on station.id = park.adm_station_id
                    inner join adm_nod nod on nod.id = station.adm_nod_id
                    where nod.adm_road_id = {admRoadId}) TO '{dirName + "\\stw_park_track.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                //PERIOD
                result &= ExportImportService.Execute($@"COPY (SELECT tpl_period.* from tpl_period
                    where (tpl_period.adm_track_id in (select track.id from adm_track track
                        inner join road_direction rd on rd.road_id = {admRoadId} and rd.direction_id = track.adm_direction_id)
                    or tpl_period.adm_track_id in (select strack.adm_track_id from stw_track strack
                        inner join adm_nod nod on nod.adm_road_id = {admRoadId}
                        inner join adm_station station on station.adm_nod_id = nod.id and station.id = strack.adm_station_id)
                    or tpl_period.adm_track_id in (select strack.adm_track_id from stw_park_track strack
                        inner join adm_nod nod on nod.adm_road_id = {admRoadId}
                        inner join adm_station station on station.adm_nod_id = nod.id 
                        inner join stw_park park on park.adm_station_id = station.id and park.id = strack.stw_park_id))
                    and ('{date.ToString("dd-MM-yyyy")}' between tpl_period.start_date and tpl_period.final_date)
                    ) TO '{dirName + "\\tpl_period.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                //MTO STRUCTURE
                List<string> mtoStructure = new List<string> { "apr_artificial_construction", "apr_ballast", "apr_cham_joint", "apr_communication", "apr_coordinate_gnss", "apr_crosstie", "apr_curve", "apr_defects_earth", "apr_dimension",
                    "apr_elevation", "apr_long_rails", "apr_norma_width", "apr_profmarks", "apr_rails_braces", "apr_rails_sections", "apr_ref_point", "apr_rfid", "apr_speed", "apr_straightening_thread", "apr_tempspeed", "apr_trackclass",
                    "apr_traffic", "apr_traffic", "tpl_check_sections", "tpl_deep", "tpl_dist_section", "tpl_distance_between_tracks", "tpl_non_ext_km", "tpl_nst_km", "tpl_pdb_section", "tpl_profile_object", "tpl_station_section", "tpl_switch"
                };

                foreach (var nameOfStructure in mtoStructure)
                {
                    result &= ExportImportService.Execute($@"COPY (select struct.* from {nameOfStructure} struct
                        inner join tpl_period tpl_period on tpl_period.id = struct.period_id
                        where (tpl_period.adm_track_id in (select track.id from adm_track track
                            inner join road_direction rd on rd.road_id = {admRoadId} and rd.direction_id = track.adm_direction_id)
                        or tpl_period.adm_track_id in (select strack.adm_track_id from stw_track strack
                            inner join adm_nod nod on nod.adm_road_id = {admRoadId}
                            inner join adm_station station on station.adm_nod_id = nod.id and station.id = strack.adm_station_id)
                        or tpl_period.adm_track_id in (select strack.adm_track_id from stw_park_track strack
                            inner join adm_nod nod on nod.adm_road_id = {admRoadId}
                            inner join adm_station station on station.adm_nod_id = nod.id 
                            inner join stw_park park on park.adm_station_id = station.id and park.id = strack.stw_park_id))
                        and ('{date.ToString("dd-MM-yyyy")}' between tpl_period.start_date and tpl_period.final_date)
                        ) TO '{$@"{dirName}\\{nameOfStructure}.csv"}' with CSV delimiter ',' header;");
                    
                    progressBar.PerformStep();
                }

                result &= ExportImportService.Execute($@"COPY (select elcurve.* from apr_elcurve elcurve
                    inner join apr_curve curve on curve.id = elcurve.curve_id
                    inner join tpl_period tpl_period on tpl_period.id = curve.period_id
                    where (tpl_period.adm_track_id in (select track.id from adm_track track
                        inner join road_direction rd on rd.road_id = {admRoadId} and rd.direction_id = track.adm_direction_id)
                    or tpl_period.adm_track_id in (select strack.adm_track_id from stw_track strack
                        inner join adm_nod nod on nod.adm_road_id = {admRoadId}
                        inner join adm_station station on station.adm_nod_id = nod.id and station.id = strack.adm_station_id)
                    or tpl_period.adm_track_id in (select strack.adm_track_id from stw_park_track strack
                        inner join adm_nod nod on nod.adm_road_id = {admRoadId}
                        inner join adm_station station on station.adm_nod_id = nod.id 
                        inner join stw_park park on park.adm_station_id = station.id and park.id = strack.stw_park_id))
                    and ('{date.ToString("dd-MM-yyyy")}' between tpl_period.start_date and tpl_period.final_date)
                    ) TO '{$@"{dirName}\\apr_elcurve.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (select stcurve.* from apr_stcurve stcurve
                    inner join apr_curve curve on curve.id = stcurve.curve_id
                    inner join tpl_period tpl_period on tpl_period.id = curve.period_id
                    where (tpl_period.adm_track_id in (select track.id from adm_track track
                        inner join road_direction rd on rd.road_id = {admRoadId} and rd.direction_id = track.adm_direction_id)
                    or tpl_period.adm_track_id in (select strack.adm_track_id from stw_track strack
                        inner join adm_nod nod on nod.adm_road_id = {admRoadId}
                        inner join adm_station station on station.adm_nod_id = nod.id and station.id = strack.adm_station_id)
                    or tpl_period.adm_track_id in (select strack.adm_track_id from stw_park_track strack
                        inner join adm_nod nod on nod.adm_road_id = {admRoadId}
                        inner join adm_station station on station.adm_nod_id = nod.id 
                        inner join stw_park park on park.adm_station_id = station.id and park.id = strack.stw_park_id))
                    and ('{date.ToString("dd-MM-yyyy")}' between tpl_period.start_date and tpl_period.final_date)
                    ) TO '{$@"{dirName}\\apr_stcurve.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (select repair.* from repair_project repair 
                        where repair.adm_track_id in (select track.id from adm_track track
                            inner join road_direction rd on rd.road_id = {admRoadId} and rd.direction_id = track.adm_direction_id)
                        or repair.adm_track_id in (select strack.adm_track_id from stw_track strack
                            inner join adm_nod nod on nod.adm_road_id = {admRoadId}
                            inner join adm_station station on station.adm_nod_id = nod.id and station.id = strack.adm_station_id)
                        or repair.adm_track_id in (select strack.adm_track_id from stw_park_track strack
                            inner join adm_nod nod on nod.adm_road_id = {admRoadId}
                            inner join adm_station station on station.adm_nod_id = nod.id 
                            inner join stw_park park on park.adm_station_id = station.id and park.id = strack.stw_park_id)
                        ) TO '{$@"{dirName}\\repair_project.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();
            }
            else
            {
                // ADM_STRUCTURE
                result &= ExportImportService.Execute($@"COPY (SELECT * FROM ADM_ROAD) TO '{dirName + "\\adm_road.csv"}' with CSV delimiter ',' header;");
                

                result &= ExportImportService.Execute($@"COPY (SELECT * FROM adm_nod where adm_road_id in (select id from adm_road)) TO '{dirName + "\\adm_nod.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT distance.* FROM adm_distance distance
                    inner join adm_nod nod on nod.id = distance.adm_nod_id
                    where nod.adm_road_id in (select id from adm_road)) TO '{dirName + "\\adm_distance.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT pchu.* FROM adm_pchu pchu
                    inner join adm_distance distance on distance.id = pchu.adm_distance_id
                    inner join adm_nod nod on nod.id = distance.adm_nod_id
                    where nod.adm_road_id in (select id from adm_road)) TO '{dirName + "\\adm_pchu.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT pd.* FROM adm_pd pd
                    inner join adm_pchu pchu on pchu.id = pd.adm_pchu_id
                    inner join adm_distance distance on distance.id = pchu.adm_distance_id
                    inner join adm_nod nod on nod.id = distance.adm_nod_id
                    where nod.adm_road_id in (select id from adm_road)) TO '{dirName + "\\adm_pd.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT pdb.* FROM adm_pdb pdb
                    inner join adm_pd pd on pd.id = pdb.adm_pd_id
                    inner join adm_pchu pchu on pchu.id = pd.adm_pchu_id
                    inner join adm_distance distance on distance.id = pchu.adm_distance_id
                    inner join adm_nod nod on nod.id = distance.adm_nod_id
                    where nod.adm_road_id in (select id from adm_road)) TO '{dirName + "\\adm_pdb.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT rd.* from road_direction rd
                    where rd.road_id in (select id from adm_road)) TO '{dirName + "\\road_direction.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT direction.* from adm_direction direction
                    inner join road_direction rd on rd.direction_id = direction.id
                    where rd.road_id in (select id from adm_road)) TO '{dirName + "\\adm_direction.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT track.* from adm_track track
                    where track.adm_direction_id in (select direction.id from adm_direction direction
                        inner join road_direction rd on rd.road_id in (select id from adm_road) and rd.direction_id = direction.id)
                    or track.id in (select strack.adm_track_id from stw_track strack
                        inner join adm_nod nod on nod.adm_road_id in (select id from adm_road)
                        inner join adm_station station on station.adm_nod_id = nod.id and station.id = strack.adm_station_id)
                    or track.id in (select strack.adm_track_id from stw_park_track strack
                        inner join adm_nod nod on nod.adm_road_id in (select id from adm_road)
                        inner join adm_station station on station.adm_nod_id = nod.id 
                        inner join stw_park park on park.adm_station_id = station.id and park.id = strack.stw_park_id)
                    ) TO '{dirName + "\\adm_track.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT station.* FROM adm_station station
                    inner join adm_nod nod on nod.id = station.adm_nod_id
                    where nod.adm_road_id in (select id from adm_road)) TO '{dirName + "\\adm_station.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT park.* FROM stw_park park
                    inner join adm_station station on station.id = park.adm_station_id
                    inner join adm_nod nod on nod.id = station.adm_nod_id
                    where nod.adm_road_id in (select id from adm_road)) TO '{dirName + "\\stw_park.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT sobject.* from stw_object sobject
                    inner join stw_park park on park.id = sobject.stw_park_id
                    inner join adm_station station on station.id = park.adm_station_id
                    inner join adm_nod nod on nod.id = station.adm_nod_id
                    where nod.adm_road_id in (select id from adm_road)) TO '{dirName + "\\stw_object.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT strack.* FROM stw_track strack
                    inner join adm_station station on station.id = strack.adm_station_id
                    inner join adm_nod nod on nod.id = station.adm_nod_id
                    where nod.adm_road_id in (select id from adm_road)) TO '{dirName + "\\stw_track.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (SELECT strack.* FROM stw_park_track strack
                    inner join stw_park park on park.id = strack.stw_park_id
                    inner join adm_station station on station.id = park.adm_station_id
                    inner join adm_nod nod on nod.id = station.adm_nod_id
                    where nod.adm_road_id in (select id from adm_road)) TO '{dirName + "\\stw_park_track.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                //PERIOD
                result &= ExportImportService.Execute($@"COPY (SELECT tpl_period.* from tpl_period
                    where (tpl_period.adm_track_id in (select track.id from adm_track track
                        inner join road_direction rd on rd.road_id in (select id from adm_road) and rd.direction_id = track.adm_direction_id)
                    or tpl_period.adm_track_id in (select strack.adm_track_id from stw_track strack
                        inner join adm_nod nod on nod.adm_road_id in (select id from adm_road)
                        inner join adm_station station on station.adm_nod_id = nod.id and station.id = strack.adm_station_id)
                    or tpl_period.adm_track_id in (select strack.adm_track_id from stw_park_track strack
                        inner join adm_nod nod on nod.adm_road_id in (select id from adm_road)
                        inner join adm_station station on station.adm_nod_id = nod.id 
                        inner join stw_park park on park.adm_station_id = station.id and park.id = strack.stw_park_id))
                    and ('{date.ToString("dd-MM-yyyy")}' between tpl_period.start_date and tpl_period.final_date)
                    ) TO '{dirName + "\\tpl_period.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                //MTO STRUCTURE
                List<string> mtoStructure = new List<string> { "apr_artificial_construction", "apr_ballast", "apr_cham_joint", "apr_communication", "apr_coordinate_gnss", "apr_crosstie", "apr_curve", "apr_defects_earth", "apr_dimension",
                    "apr_elevation", "apr_long_rails", "apr_norma_width", "apr_profmarks", "apr_rails_braces", "apr_rails_sections", "apr_ref_point", "apr_rfid", "apr_speed", "apr_straightening_thread", "apr_tempspeed", "apr_trackclass",
                    "apr_traffic", "apr_traffic", "tpl_check_sections", "tpl_deep", "tpl_dist_section", "tpl_distance_between_tracks", "tpl_non_ext_km", "tpl_nst_km", "tpl_pdb_section", "tpl_profile_object", "tpl_station_section", "tpl_switch"
                };

                foreach (var nameOfStructure in mtoStructure)
                {
                    result &= ExportImportService.Execute($@"COPY (select struct.* from {nameOfStructure} struct
                        inner join tpl_period tpl_period on tpl_period.id = struct.period_id
                        where (tpl_period.adm_track_id in (select track.id from adm_track track
                            inner join road_direction rd on rd.road_id in (select id from adm_road) and rd.direction_id = track.adm_direction_id)
                        or tpl_period.adm_track_id in (select strack.adm_track_id from stw_track strack
                            inner join adm_nod nod on nod.adm_road_id in (select id from adm_road)
                            inner join adm_station station on station.adm_nod_id = nod.id and station.id = strack.adm_station_id)
                        or tpl_period.adm_track_id in (select strack.adm_track_id from stw_park_track strack
                            inner join adm_nod nod on nod.adm_road_id in (select id from adm_road)
                            inner join adm_station station on station.adm_nod_id = nod.id 
                            inner join stw_park park on park.adm_station_id = station.id and park.id = strack.stw_park_id))
                        and ('{date.ToString("dd-MM-yyyy")}' between tpl_period.start_date and tpl_period.final_date)
                        ) TO '{$@"{dirName}\\{nameOfStructure}.csv"}' with CSV delimiter ',' header;");
                    
                    progressBar.PerformStep();
                }

                result &= ExportImportService.Execute($@"COPY (select elcurve.* from apr_elcurve elcurve
                    inner join apr_curve curve on curve.id = elcurve.curve_id
                    inner join tpl_period tpl_period on tpl_period.id = curve.period_id
                    where (tpl_period.adm_track_id in (select track.id from adm_track track
                        inner join road_direction rd on rd.road_id in (select id from adm_road) and rd.direction_id = track.adm_direction_id)
                    or tpl_period.adm_track_id in (select strack.adm_track_id from stw_track strack
                        inner join adm_nod nod on nod.adm_road_id in (select id from adm_road)
                        inner join adm_station station on station.adm_nod_id = nod.id and station.id = strack.adm_station_id)
                    or tpl_period.adm_track_id in (select strack.adm_track_id from stw_park_track strack
                        inner join adm_nod nod on nod.adm_road_id in (select id from adm_road)
                        inner join adm_station station on station.adm_nod_id = nod.id 
                        inner join stw_park park on park.adm_station_id = station.id and park.id = strack.stw_park_id))
                    and ('{date.ToString("dd-MM-yyyy")}' between tpl_period.start_date and tpl_period.final_date)
                    ) TO '{$@"{dirName}\\apr_elcurve.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (select stcurve.* from apr_stcurve stcurve
                    inner join apr_curve curve on curve.id = stcurve.curve_id
                    inner join tpl_period tpl_period on tpl_period.id = curve.period_id
                    where (tpl_period.adm_track_id in (select track.id from adm_track track
                        inner join road_direction rd on rd.road_id in (select id from adm_road) and rd.direction_id = track.adm_direction_id)
                    or tpl_period.adm_track_id in (select strack.adm_track_id from stw_track strack
                        inner join adm_nod nod on nod.adm_road_id in (select id from adm_road)
                        inner join adm_station station on station.adm_nod_id = nod.id and station.id = strack.adm_station_id)
                    or tpl_period.adm_track_id in (select strack.adm_track_id from stw_park_track strack
                        inner join adm_nod nod on nod.adm_road_id in (select id from adm_road)
                        inner join adm_station station on station.adm_nod_id = nod.id 
                        inner join stw_park park on park.adm_station_id = station.id and park.id = strack.stw_park_id))
                    and ('{date.ToString("dd-MM-yyyy")}' between tpl_period.start_date and tpl_period.final_date)
                    ) TO '{$@"{dirName}\\apr_stcurve.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();

                result &= ExportImportService.Execute($@"COPY (select repair.* from repair_project repair 
                        where repair.adm_track_id in (select track.id from adm_track track
                            inner join road_direction rd on rd.road_id in (select id from adm_road) and rd.direction_id = track.adm_direction_id)
                        or repair.adm_track_id in (select strack.adm_track_id from stw_track strack
                            inner join adm_nod nod on nod.adm_road_id in (select id from adm_road)
                            inner join adm_station station on station.adm_nod_id = nod.id and station.id = strack.adm_station_id)
                        or repair.adm_track_id in (select strack.adm_track_id from stw_park_track strack
                            inner join adm_nod nod on nod.adm_road_id in (select id from adm_road)
                            inner join adm_station station on station.adm_nod_id = nod.id 
                            inner join stw_park park on park.adm_station_id = station.id and park.id = strack.stw_park_id)
                        ) TO '{$@"{dirName}\\repair_project.csv"}' with CSV delimiter ',' header;");
                
                progressBar.PerformStep();
            }

            return result;
        }

        internal static bool Import(FileInfo[] fileInfo, MetroFramework.Controls.MetroProgressBar progressBar, int import_type, long road_id, long nod_id, Period period)
        {
            string dirName = System.IO.Directory.GetCurrentDirectory() + "\\logs";
            DirectoryInfo dirInfo = new DirectoryInfo(dirName);

            if (!dirInfo.Exists)
                dirInfo.Create();

            string logFile = System.IO.Directory.GetCurrentDirectory() + "\\logs\\import_log.txt";

            using (StreamWriter outputFile = new StreamWriter(logFile, false))
            {
                outputFile.WriteLine(DateTime.Now);
                outputFile.WriteLine(fileInfo.First().DirectoryName);
                outputFile.WriteLine();
            }

            List<ImportListDirTrackID> listIDs = new List<ImportListDirTrackID>();
            List<ImportListStationID> listStationIDs = new List<ImportListStationID>();
            List<ImportListCurveID> listCurveIDs = new List<ImportListCurveID>();
            bool curveAdd = false;
            ImportListCurveID listCurveID = new ImportListCurveID();
            List<ImportListElCurve> listElCurves = new List<ImportListElCurve>();
            List<ImportListStCurve> listStCurves = new List<ImportListStCurve>();
            List<ImportListPDBID> listPDBIDs = new List<ImportListPDBID>();
            List<ImportListStationSection> listStationSections = new List<ImportListStationSection>();

            progressBar.Value = 0;
            progressBar.Step = 100 / 20;

            if (fileInfo.Where(tmp => tmp.Name == "KV_PUTGL.xml").Count() > 0)
            {
                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_STAN.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;

                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string name = z_row.Attributes["NAME"].Value;
                                string code = z_row.Attributes["ST_KOD"].Value;
                                string stantiontype = z_row.Attributes["STAN_TIP"].Value;
                                int typeid = 0;

                                if (stantiontype == "Станция")
                                    typeid = 3;
                                else if (stantiontype == "Разьезд")
                                    typeid = 4;
                                else if (stantiontype == "Блокпост")
                                    typeid = 5;

                                ImportListStationID listStationID = new ImportListStationID();
                                listStationID.Station = name;
                                listStationID.Code = code;
                                listStationID.OldStationID = int.Parse(z_row.Attributes["STAN_ID"].Value);
                                listStationID.NewStationID = ExportImportService.ImportQueryReturnLong("insert into ADM_STATION (TYPE_ID, CODE, NAME, ADM_NOD_ID) values (" + typeid.ToString() + ", " + code + ", \'" + name + "\', " + nod_id + ") returning ID");
                                listStationIDs.Add(listStationID);
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_PUTGL.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string directionName = z_row.Attributes["NAME"].Value;
                                string directionCode = z_row.Attributes["NHAPR"].Value;
                                string trackCode = z_row.Attributes["NPUT"].Value;
                                if (z_row.Attributes["H_PUT_TYPE"].Value == "Главный")
                                {
                                    if (listIDs.Where(tmp => tmp.OldDirectionID == int.Parse(z_row.Attributes["UP_ID"].Value)).Count() < 1)
                                    {
                                        Int64 directionID = ExportImportService.ImportQueryReturnLong("insert into ADM_DIRECTION (CODE, NAME) values (\'" + directionCode + "\', \'" + directionName + "\') returning ID");
                                        Int64 trackID = ExportImportService.ImportQueryReturnLong("insert into ADM_TRACK (CODE, ADM_DIRECTION_ID) values (\'" + trackCode + "\', \'" + directionID.ToString() + "\') returning ID");
                                        ExportImportService.Execute($@"insert into road_direction (road_id, direction_id) values({road_id}, {directionID})");
                                        ImportListDirTrackID listID = new ImportListDirTrackID();
                                        listID.NewDirectionID = directionID;
                                        listID.NewTrackID = trackID;
                                        listID.OldDirectionID = int.Parse(z_row.Attributes["UP_ID"].Value);
                                        listID.OldTrackID = int.Parse(z_row.Attributes["PUTGL_ID"].Value);
                                        listIDs.Add(listID);
                                    }
                                    else
                                    {
                                        Int64 directionID = listIDs.Where(tmp => tmp.OldDirectionID == int.Parse(z_row.Attributes["UP_ID"].Value)).First().NewDirectionID;
                                        Int64 trackID = ExportImportService.ImportQueryReturnLong("insert into ADM_TRACK (CODE, ADM_DIRECTION_ID) values (\'" + trackCode + "\', \'" + directionID.ToString() + "\') returning ID");
                                        ImportListDirTrackID listID = new ImportListDirTrackID();
                                        listID.NewDirectionID = directionID;
                                        listID.NewTrackID = trackID;
                                        listID.OldDirectionID = int.Parse(z_row.Attributes["UP_ID"].Value);
                                        listID.OldTrackID = int.Parse(z_row.Attributes["PUTGL_ID"].Value);
                                        listIDs.Add(listID);
                                    }
                                }
                                else
                                {
                                    if (!directionName.Contains("/"))
                                    {
                                        if (listStationIDs.Where(tmp => tmp.Station == directionName && directionCode.Contains(tmp.Code.ToString())).Count() > 0)
                                        {
                                            Int64 stationID = listStationIDs.Where(tmp => tmp.Station == directionName && directionCode.Contains(tmp.Code.ToString())).First().NewStationID;
                                            Int64 trackID = ExportImportService.ImportQueryReturnLong("insert into adm_track (code) values (\'" + trackCode + "\') returning ID");
                                            ExportImportService.Execute("insert into stw_track (adm_station_id, adm_track_id, type_id) values (" + stationID.ToString() + ", " + trackID.ToString() + ", 8)");
                                            ImportListDirTrackID listID = new ImportListDirTrackID();
                                            listID.NewDirectionID = stationID;
                                            listID.NewTrackID = trackID;
                                            listID.OldDirectionID = int.Parse(z_row.Attributes["UP_ID"].Value);
                                            listID.OldTrackID = int.Parse(z_row.Attributes["PUTGL_ID"].Value);
                                            listIDs.Add(listID);
                                        }
                                        else
                                        {
                                            int typeid = 3;
                                            ImportListStationID listStationID = new ImportListStationID();

                                            foreach (FileInfo tempFile in fileInfo.Where(tmp => tmp.Name == "KV_STR.xml"))
                                            {
                                                XmlDocument tempXmlDocument = new XmlDocument();
                                                tempXmlDocument.Load(tempFile.FullName);
                                                XmlElement tempXmlElement = tempXmlDocument.DocumentElement;
                                                foreach (XmlNode tempXmlNode in tempXmlElement)
                                                {
                                                    if (tempXmlNode.Name == "rs:data")
                                                    {
                                                        foreach (XmlNode tempz_row in tempXmlNode.ChildNodes)
                                                        {
                                                            if (tempz_row.Attributes["ST_KOD"].Value.Contains(directionCode))
                                                            {
                                                                listStationID.OldStationID = int.Parse(tempz_row.Attributes["STAN_ID"].Value);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            listStationID.Station = directionName;
                                            listStationID.Code = directionCode;
                                            listStationID.NewStationID = ExportImportService.ImportQueryReturnLong("insert into ADM_STATION (TYPE_ID, CODE, NAME, ADM_NOD_ID) values (" + typeid.ToString() + ", " + directionCode + ", \'" + directionName + "\', " + nod_id + ") returning ID");
                                            listStationIDs.Add(listStationID);

                                            Int64 trackID = ExportImportService.ImportQueryReturnLong("insert into adm_track (code) values (\'" + trackCode + "\') returning ID");
                                            ExportImportService.Execute("insert into stw_track (adm_station_id, adm_track_id, type_id) values (" + listStationID.NewStationID.ToString() + ", " + trackID.ToString() + ", 8)");
                                            ImportListDirTrackID listID = new ImportListDirTrackID();
                                            listID.NewDirectionID = listStationID.NewStationID;
                                            listID.NewTrackID = trackID;
                                            listID.OldDirectionID = int.Parse(z_row.Attributes["UP_ID"].Value);
                                            listID.OldTrackID = int.Parse(z_row.Attributes["PUTGL_ID"].Value);
                                            listIDs.Add(listID);
                                        }
                                    }
                                    else
                                    {
                                        if (listIDs.Where(tmp => tmp.OldDirectionID == int.Parse(z_row.Attributes["UP_ID"].Value)).Count() < 1)
                                        {
                                            string[] stations = directionName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                                            string stationID = listStationIDs.Where(tmp => tmp.Station.Contains(stations[0]) && tmp.Code.Contains(directionCode.Remove(6))).First().NewStationID.ToString();

                                            ImportListStationID listStationID = new ImportListStationID();
                                            listStationID.Station = stations[1];
                                            listStationID.Code = directionCode;
                                            listStationID.OldStationID = int.Parse(z_row.Attributes["UP_ID"].Value);
                                            listStationID.NewStationID = ExportImportService.ImportQueryReturnLong("insert into stw_park (TYPE_ID, adm_station_id, name) values (3, " + stationID + ", \'" + stations[1] + "\') returning ID");
                                            listStationIDs.Add(listStationID);

                                            Int64 trackID = ExportImportService.ImportQueryReturnLong("insert into adm_track (code) values (\'" + trackCode + "\') returning ID");
                                            ExportImportService.Execute("insert into stw_park_track (stw_park_id, adm_track_id, type_id) values (" + listStationID.NewStationID.ToString() + ", " + trackID.ToString() + ", 8)");

                                            ImportListDirTrackID listID = new ImportListDirTrackID();
                                            listID.NewDirectionID = listStationID.NewStationID;
                                            listID.NewTrackID = trackID;
                                            listID.OldDirectionID = int.Parse(z_row.Attributes["UP_ID"].Value);
                                            listID.OldTrackID = int.Parse(z_row.Attributes["PUTGL_ID"].Value);
                                            listIDs.Add(listID);
                                        }
                                        else
                                        {
                                            string[] stations = directionName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                                            string parkID = listStationIDs.Where(tmp => tmp.Station == stations[1] && tmp.Code.Contains(directionCode)).First().NewStationID.ToString();

                                            Int64 trackID = ExportImportService.ImportQueryReturnLong("insert into adm_track (code) values (\'" + trackCode + "\') returning ID");
                                            ExportImportService.Execute("insert into stw_park_track (stw_park_id, adm_track_id, type_id) values (" + parkID + ", " + trackID.ToString() + ", 8)");

                                            ImportListDirTrackID listID = new ImportListDirTrackID();
                                            listID.NewDirectionID = Int64.Parse(parkID);
                                            listID.NewTrackID = trackID;
                                            listID.OldDirectionID = int.Parse(z_row.Attributes["UP_ID"].Value);
                                            listID.OldTrackID = int.Parse(z_row.Attributes["PUTGL_ID"].Value);
                                            listIDs.Add(listID);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_KMNEST.xml"))
                {
                    using (StreamWriter outputFile = new StreamWriter(logFile, true))
                    {
                        outputFile.WriteLine("KV_KMNEST.xml");
                    }

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string km = z_row.Attributes["KM"].Value;
                                string length = z_row.Attributes["L"].Value;
                                if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                {
                                    try
                                    {
                                        Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                        if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 2").Count() > 0)
                                        {
                                            Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 2");
                                            ExportImportService.Execute("insert into TPL_NST_KM (PERIOD_ID, KM, LEN) values (" + periodID.ToString() + ", " + km + ", " + length + ")");
                                        }
                                        else
                                        {
                                            Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 2) returning ID");
                                            ExportImportService.Execute("insert into TPL_NST_KM (PERIOD_ID, KM, LEN) values (" + periodID.ToString() + ", " + km + ", " + length + ")");
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        using (StreamWriter outputFile = new StreamWriter(logFile, true))
                                        {
                                            outputFile.WriteLine(z_row.Attributes["PUTGL_ID"].Value + ", km: " + z_row.Attributes["KM"].Value + ", len: " + z_row.Attributes["L"].Value);
                                            outputFile.WriteLine(e.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamWriter outputFile = new StreamWriter(logFile, true))
                                    {
                                        outputFile.WriteLine(z_row.Attributes["PUTGL_ID"].Value + ", km: " + z_row.Attributes["KM"].Value + ", len: " + z_row.Attributes["L"].Value);
                                        outputFile.WriteLine("Путь по ID не найден");
                                    }
                                }
                            }
                        }
                    }

                    using (StreamWriter outputFile = new StreamWriter(logFile, true))
                    {
                        outputFile.WriteLine();
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_NESYKM.xml"))
                {
                    using (StreamWriter outputFile = new StreamWriter(logFile, true))
                    {
                        outputFile.WriteLine("KV_NESYKM.xml");
                    }

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string km = z_row.Attributes["KM"].Value;
                                if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                {
                                    try
                                    {
                                        Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                        if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 18").Count() > 0)
                                        {
                                            Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 18");
                                            ExportImportService.Execute("insert into TPL_NON_EXT_KM (PERIOD_ID, KM) values (" + periodID.ToString() + ", " + km + ")");
                                        }
                                        else
                                        {
                                            Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 18) returning ID");
                                            ExportImportService.Execute("insert into TPL_NON_EXT_KM (PERIOD_ID, KM) values (" + periodID.ToString() + ", " + km + ")");
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        using (StreamWriter outputFile = new StreamWriter(logFile, true))
                                        {
                                            outputFile.WriteLine(z_row.Attributes["PUTGL_ID"].Value + ", km: " + z_row.Attributes["KM"].Value);
                                            outputFile.WriteLine(e.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    using (StreamWriter outputFile = new StreamWriter(logFile, true))
                                    {
                                        outputFile.WriteLine(z_row.Attributes["PUTGL_ID"].Value + ", km: " + z_row.Attributes["KM"].Value);
                                        outputFile.WriteLine("Путь по ID не найден");
                                    }
                                }
                            }
                        }
                    }

                    using (StreamWriter outputFile = new StreamWriter(logFile, true))
                    {
                        outputFile.WriteLine();
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_VUS.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string start_km = z_row.Attributes["KMN"].Value;
                                string start_m = z_row.Attributes["MN"].Value;
                                string final_km = z_row.Attributes["KMK"].Value;
                                string final_m = z_row.Attributes["MK"].Value;
                                string passenger = z_row.Attributes["VPAS"].Value;
                                string freight = z_row.Attributes["VGR"].Value;
                                string empty = z_row.Attributes["VPOR"].Value;
                                if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                {
                                    Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                    if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 6").Count() > 0)
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 6");
                                        ExportImportService.Execute("insert into APR_SPEED (sapsan, lastochka, PERIOD_ID, START_KM, START_M, FINAL_KM, FINAL_M, PASSENGER, FREIGHT, EMPTY_FREIGHT) values (0, 0, " + periodID.ToString() + ", " + start_km + "," + start_m + ", " + final_km + "," + final_m + ", " + passenger + "," + freight + ", " + empty + ")");
                                    }
                                    else
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 6) returning ID");
                                        ExportImportService.Execute("insert into APR_SPEED (sapsan, lastochka, PERIOD_ID, START_KM, START_M, FINAL_KM, FINAL_M, PASSENGER, FREIGHT, EMPTY_FREIGHT) values (0, 0, " + periodID.ToString() + ", " + start_km + "," + start_m + ", " + final_km + "," + final_m + ", " + passenger + "," + freight + ", " + empty + ")");
                                    }
                                }
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_MOST.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string start_km = z_row.Attributes["KMN"].Value;
                                string start_m = z_row.Attributes["MN"].Value;
                                string final_km = z_row.Attributes["KMK"].Value;
                                string final_m = z_row.Attributes["MK"].Value;
                                int len = Int32.Parse(final_km) * 1000 + Int32.Parse(final_m) - Int32.Parse(start_km) * 1000 - Int32.Parse(start_m);
                                string typeid = "6";

                                if (z_row.Attributes["H_ID_TYPE"].Value != "6" && z_row.Attributes["H_ID_TYPE"].Value != "0")
                                    switch (z_row.Attributes["TYPE"].Value)
                                    {
                                        case "переезд":
                                            typeid = "5";
                                            break;
                                        case "безбалластный":
                                            typeid = "1";
                                            break;
                                        case "балластный":
                                            typeid = "2";
                                            break;
                                        case "туннель":
                                            typeid = "3";
                                            break;
                                        case "путепров.":
                                            typeid = "4";
                                            break;
                                    }

                                if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                {
                                    Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                    if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 14").Count() > 0)
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 14");
                                        ExportImportService.Execute("insert into APR_ARTIFICIAL_CONSTRUCTION (PERIOD_ID, km, meter, len, TYPE_ID) values (" + periodID.ToString() + ", " + start_km + "," + start_m + ", " + len.ToString() + ", " + typeid + ")");
                                    }
                                    else
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 14) returning ID");
                                        ExportImportService.Execute("insert into APR_ARTIFICIAL_CONSTRUCTION (PERIOD_ID, km, meter, len, TYPE_ID) values (" + periodID.ToString() + ", " + start_km + "," + start_m + ", " + len.ToString() + ", " + typeid + ")"); ;
                                    }
                                }
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_NSK.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string start_km = z_row.Attributes["KMN"].Value;
                                string start_m = z_row.Attributes["MN"].Value;
                                string final_km = z_row.Attributes["KMK"].Value;
                                string final_m = z_row.Attributes["MK"].Value;
                                string width = z_row.Attributes["NORMA"].Value;
                                if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                {
                                    Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                    if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 5").Count() > 0)
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 5");
                                        ExportImportService.Execute("insert into APR_NORMA_WIDTH (PERIOD_ID, START_KM, START_M, FINAL_KM, FINAL_M, NORMA_WIDTH) values (" + periodID.ToString() + ", " + start_km + "," + start_m + ", " + final_km + "," + final_m + ", " + width + ")");
                                    }
                                    else
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 5) returning ID");
                                        ExportImportService.Execute("insert into APR_NORMA_WIDTH (PERIOD_ID, START_KM, START_M, FINAL_KM, FINAL_M, NORMA_WIDTH) values (" + periodID.ToString() + ", " + start_km + "," + start_m + ", " + final_km + "," + final_m + ", " + width + ")");
                                    }
                                }
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_STR.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string km = z_row.Attributes["KM"].Value;
                                string m = z_row.Attributes["M"].Value;
                                string num = z_row.Attributes["NOM"].Value;
                                string dirID, markID, sideID;
                                if (z_row.Attributes["POSH"].Value == "пш")
                                    dirID = "-1";
                                else if (z_row.Attributes["POSH"].Value == "прш")
                                    dirID = "1";
                                else dirID = "0";

                                int hidtype = -1;
                                if (!int.TryParse(z_row.Attributes["H_ID_TYPE"].Value, out hidtype))
                                    markID = "1";
                                else
                                {
                                    if (hidtype == 4)
                                        markID = "5";
                                    else if (hidtype == 3)
                                        markID = "4";
                                    else if (hidtype == 2)
                                        markID = "3";
                                    else if (hidtype == 1)
                                        markID = "2";
                                    else if (hidtype == 5)
                                        markID = "6";
                                    else if (hidtype == 13)
                                        markID = "14";
                                    else if (hidtype == 14)
                                        markID = "15";
                                    else markID = "1";
                                }

                                if (z_row.Attributes["OTB"].Value == "влево")
                                    sideID = "2";
                                else if (z_row.Attributes["OTB"].Value == "вправо")
                                    sideID = "1";
                                else sideID = "0";

                                if (listStationIDs.Where(tmp => tmp.OldStationID == int.Parse(z_row.Attributes["STAN_ID"].Value)).Count() > 0)
                                {
                                    Int64 stationId = listStationIDs.Where(tmp => tmp.OldStationID == int.Parse(z_row.Attributes["STAN_ID"].Value)).First().NewStationID;

                                    if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                    {
                                        Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                        if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 15").Count() > 0)
                                        {
                                            Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 15");
                                            ExportImportService.Execute("insert into TPL_SWITCH (PERIOD_ID, KM, METER, NUM, MARK_ID, DIR_ID, SIDE_ID, POINT_ID, station_id) values (" + periodID.ToString() + ", " + km + ", " + m + ", \'" + num + "\', " + markID + ", " + dirID + ", " + sideID + ", 35, " + stationId.ToString() + ")");
                                        }
                                        else
                                        {
                                            Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 15) returning ID");
                                            ExportImportService.Execute("insert into TPL_SWITCH (PERIOD_ID, KM, METER, NUM, MARK_ID, DIR_ID, SIDE_ID, POINT_ID, station_id) values (" + periodID.ToString() + ", " + km + ", " + m + ", \'" + num + "\', " + markID + ", " + dirID + ", " + sideID + ", 35, " + stationId.ToString() + ")");
                                        }

                                        if (listStationSections.Where(l => l.OldTrackID.Equals(z_row.Attributes["PUTGL_ID"].Value) && l.OldStationID.Equals(z_row.Attributes["STAN_ID"].Value)).Count() > 0)
                                        {
                                            ImportListStationSection listStationSection = listStationSections.Where(l => l.OldTrackID.Equals(z_row.Attributes["PUTGL_ID"].Value) && l.OldStationID.Equals(z_row.Attributes["STAN_ID"].Value)).First();
                                            int pos = Int32.Parse(km) * 1000 + Int32.Parse(m);

                                            if (pos < listStationSection.Start_pos)
                                            {
                                                listStationSection.Start_pos = pos;
                                                listStationSection.Axis_pos = (listStationSection.Start_pos + listStationSection.Final_pos) / 2;
                                            }
                                            else if (pos > listStationSection.Final_pos)
                                            {
                                                listStationSection.Final_pos = pos;
                                                listStationSection.Axis_pos = (listStationSection.Start_pos + listStationSection.Final_pos) / 2;
                                            }
                                        }
                                        else
                                        {
                                            ImportListStationSection listStationSection = new ImportListStationSection {
                                                OldTrackID = z_row.Attributes["PUTGL_ID"].Value,
                                                OldStationID = z_row.Attributes["STAN_ID"].Value,
                                                Start_pos = Int32.Parse(km) * 1000 + Int32.Parse(m),
                                                Axis_pos = Int32.Parse(km) * 1000 + Int32.Parse(m),
                                                Final_pos = Int32.Parse(km) * 1000 + Int32.Parse(m)
                                            };
                                            listStationSections.Add(listStationSection);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (string trackId in listStationSections.GroupBy(l => l.OldTrackID).Select(l => l.Key))
                {
                    foreach (var stationSection in listStationSections.Where(l => l.OldTrackID.Equals(trackId)))
                    {


                        foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_STANKM.xml"))
                        {
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.Load(file.FullName);
                            XmlElement xmlElement = xmlDocument.DocumentElement;

                            foreach (XmlNode xmlNode in xmlElement)
                            {
                                if (xmlNode.Name == "rs:data")
                                {
                                    bool inserted = false;

                                    foreach (XmlNode z_row in xmlNode.ChildNodes)
                                    {
                                        if (z_row.Attributes["STAN_ID"].Value.Equals(stationSection.OldStationID) && z_row.Attributes["PUTGL_ID"].Value.Equals(stationSection.OldTrackID))
                                        {
                                            long trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(trackId)).First().NewTrackID;
                                            long stationID = listStationIDs.Where(tmp => tmp.OldStationID.ToString() == stationSection.OldStationID).First().NewStationID;
                                            int axisKm = Int32.Parse(z_row.Attributes["KM"].Value);
                                            int axisM = Int32.Parse(z_row.Attributes["M"].Value);
                                            int startKm = stationSection.Start_pos / 1000, startM = stationSection.Start_pos % 1000, finalKm = stationSection.Final_pos / 1000, finalM = stationSection.Final_pos % 1000;

                                            if (stationSection.Start_pos >= axisKm * 1000 + axisM)
                                            {
                                                startKm = (axisKm * 1000 + axisM - 1) / 1000;
                                                startM = (axisKm * 1000 + axisM - 1) % 1000;
                                            }

                                            if (stationSection.Final_pos <= axisKm * 1000 + axisM)
                                            {
                                                finalKm = (axisKm * 1000 + axisM + 1) / 1000;
                                                finalM = (axisKm * 1000 + axisM + 1) % 1000;
                                            }

                                            if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 10").Count() > 0)
                                            {
                                                Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 10");
                                                ExportImportService.Execute("insert into TPL_STATION_SECTION (STATION_ID, PERIOD_ID, START_KM, START_M, FINAL_KM, FINAL_M, AXIS_KM, AXIS_M, POINT_ID) values (" + stationID.ToString() + ", " + periodID.ToString() + ", " + startKm.ToString() + ", " + startM.ToString() + ", " + finalKm.ToString() + ", " + finalM.ToString() + ", " + axisKm.ToString() + ", " + axisM.ToString() + ", 35)");
                                            }
                                            else
                                            {
                                                Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 10) returning ID");
                                                ExportImportService.Execute("insert into TPL_STATION_SECTION (STATION_ID, PERIOD_ID, START_KM, START_M, FINAL_KM, FINAL_M, AXIS_KM, AXIS_M, POINT_ID) values (" + stationID.ToString() + ", " + periodID.ToString() + ", " + startKm.ToString() + ", " + startM.ToString() + ", " + finalKm.ToString() + ", " + finalM.ToString() + ", " + axisKm.ToString() + ", " + axisM.ToString() + ", 35)");
                                            }

                                            inserted = true;
                                            break;
                                        }
                                    }

                                    if (!inserted)
                                    {
                                        long trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(trackId)).First().NewTrackID;
                                        long stationID = listStationIDs.Where(tmp => tmp.OldStationID.ToString() == stationSection.OldStationID).First().NewStationID;

                                        int startKm = stationSection.Start_pos / 1000, startM = stationSection.Start_pos % 1000, finalKm = stationSection.Final_pos / 1000, finalM = stationSection.Final_pos % 1000;
                                        int axisKm = stationSection.Axis_pos / 1000;
                                        int axisM = stationSection.Axis_pos % 1000;

                                        if (stationSection.Start_pos == stationSection.Final_pos)
                                        {
                                            startKm = (stationSection.Start_pos - 1) / 1000;
                                            startM = (stationSection.Start_pos - 1) % 1000;
                                            finalKm = (stationSection.Final_pos + 1) / 1000;
                                            finalM = (stationSection.Final_pos + 1) % 1000;
                                        }

                                        if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 10").Count() > 0)
                                        {
                                            Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 10");
                                            ExportImportService.Execute("insert into TPL_STATION_SECTION (STATION_ID, PERIOD_ID, START_KM, START_M, FINAL_KM, FINAL_M, AXIS_KM, AXIS_M, POINT_ID) values (" + stationID.ToString() + ", " + periodID.ToString() + ", " + startKm.ToString() + ", " + startM.ToString() + ", " + finalKm.ToString() + ", " + finalM.ToString() + ", " + axisKm.ToString() + ", " + axisM.ToString() + ", 35)");
                                        }
                                        else
                                        {
                                            Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 10) returning ID");
                                            ExportImportService.Execute("insert into TPL_STATION_SECTION (STATION_ID, PERIOD_ID, START_KM, START_M, FINAL_KM, FINAL_M, AXIS_KM, AXIS_M, POINT_ID) values (" + stationID.ToString() + ", " + periodID.ToString() + ", " + startKm.ToString() + ", " + startM.ToString() + ", " + finalKm.ToString() + ", " + finalM.ToString() + ", " + axisKm.ToString() + ", " + axisM.ToString() + ", 35)");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                listStationIDs.Clear();
                listStationSections.Clear();

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_P_KRIV.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                if (curveAdd)
                                {
                                    listCurveID.FinalKM = z_row.Attributes["KMK"].Value;
                                    listCurveID.FinalM = z_row.Attributes["MK"].Value;

                                    if (z_row.Attributes["L2"].Value != "0")
                                    {
                                        listCurveIDs.Add(listCurveID);
                                        curveAdd = false;
                                    }
                                }
                                else
                                {
                                    listCurveID = new ImportListCurveID();
                                    listCurveID.StartKM = z_row.Attributes["KMN"].Value;
                                    listCurveID.StartM = z_row.Attributes["MN"].Value;
                                    listCurveID.FinalKM = z_row.Attributes["KMK"].Value;
                                    listCurveID.FinalM = z_row.Attributes["MK"].Value;
                                    listCurveID.OldTrackID = z_row.Attributes["PUTGL_ID"].Value;
                                    if (z_row.Attributes["H_BOOL_KRIV"].Value == "лев")
                                        listCurveID.SideID = "2";
                                    else if (z_row.Attributes["H_BOOL_KRIV"].Value == "прав")
                                        listCurveID.SideID = "1";
                                    else listCurveID.SideID = "0";

                                    if (z_row.Attributes["L2"].Value == "0")
                                    {
                                        curveAdd = true;
                                    }
                                    else
                                    {
                                        listCurveIDs.Add(listCurveID);
                                        curveAdd = false;
                                    }
                                }

                                ImportListStCurve listStCurve = new ImportListStCurve();
                                listStCurve.StartKM = z_row.Attributes["KMN"].Value;
                                listStCurve.StartM = z_row.Attributes["MN"].Value;
                                listStCurve.FinalKM = z_row.Attributes["KMK"].Value;
                                listStCurve.FinalM = z_row.Attributes["MK"].Value;
                                listStCurve.T1 = z_row.Attributes["L1"].Value;
                                listStCurve.T2 = z_row.Attributes["L2"].Value;
                                listStCurve.Wear = z_row.Attributes["IZNOS"].Value;
                                listStCurve.Width = z_row.Attributes["SHIR"].Value;
                                listStCurve.Radius = z_row.Attributes["R"].Value;
                                listStCurve.OldTrackID = z_row.Attributes["PUTGL_ID"].Value;

                                listStCurves.Add(listStCurve);
                            }
                        }
                    }
                }

                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_U_KRIV.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                ImportListElCurve listElCurve = new ImportListElCurve();
                                listElCurve.StartKM = z_row.Attributes["KMN"].Value;
                                listElCurve.StartM = z_row.Attributes["MN"].Value;
                                listElCurve.FinalKM = z_row.Attributes["KMK"].Value;
                                listElCurve.FinalM = z_row.Attributes["MK"].Value;
                                listElCurve.T1 = z_row.Attributes["LU1"].Value;
                                listElCurve.T2 = z_row.Attributes["LU2"].Value;
                                listElCurve.Lvl = z_row.Attributes["VOZV"].Value;
                                listElCurve.OldTrackID = z_row.Attributes["PUTGL_ID"].Value;

                                listElCurves.Add(listElCurve);
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (ImportListCurveID listCurve in listCurveIDs)
                {
                    if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(listCurve.OldTrackID)).Count() > 0)
                    {
                        Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(listCurve.OldTrackID)).First().NewTrackID;
                        if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 11").Count() > 0)
                        {
                            Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 11");
                            listCurve.CurveID = ExportImportService.ImportQueryReturnLong("insert into apr_curve (PERIOD_ID, start_km, start_m, final_km, final_m, side_id) values (" + periodID.ToString() + ", " + listCurve.StartKM + ", " + listCurve.StartM + ", " + listCurve.FinalKM + ", " + listCurve.FinalM + ", " + listCurve.SideID + ") returning ID");
                        }
                        else
                        {
                            Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 11) returning ID");
                            listCurve.CurveID = ExportImportService.ImportQueryReturnLong("insert into apr_curve (PERIOD_ID, start_km, start_m, final_km, final_m, side_id) values (" + periodID.ToString() + ", " + listCurve.StartKM + ", " + listCurve.StartM + ", " + listCurve.FinalKM + ", " + listCurve.FinalM + ", " + listCurve.SideID + ") returning ID");
                        }
                    }
                }

                foreach (ImportListStCurve listSt in listStCurves)
                {
                    int startCurve = int.Parse(listSt.StartKM) * 1000 + int.Parse(listSt.StartM);
                    int finalCurve = int.Parse(listSt.FinalKM) * 1000 + int.Parse(listSt.FinalM);

                    if (listCurveIDs.Where(tmp => tmp.OldTrackID == listSt.OldTrackID && startCurve >= int.Parse(tmp.StartKM) * 1000 + int.Parse(tmp.StartM) && finalCurve <= int.Parse(tmp.FinalKM) * 1000 + int.Parse(tmp.FinalM)).Count() > 0)
                    {
                        Int64 curveID = listCurveIDs.Where(tmp => tmp.OldTrackID == listSt.OldTrackID && startCurve >= int.Parse(tmp.StartKM) * 1000 + int.Parse(tmp.StartM) && finalCurve <= int.Parse(tmp.FinalKM) * 1000 + int.Parse(tmp.FinalM)).FirstOrDefault().CurveID;
                        if (curveID != 0)
                            ExportImportService.Execute("insert into apr_stcurve (curve_id, start_km, start_m, final_km, final_m, radius, transition_1, transition_2, width, wear) values ("
                                + curveID.ToString() + ", " + listSt.StartKM + ", " + listSt.StartM + ", " + listSt.FinalKM + ", " + listSt.FinalM + ", " + listSt.Radius + ", "
                                + listSt.T1 + ", " + listSt.T2 + ", " + listSt.Width + ", " + listSt.Wear + ")");
                    }
                }

                foreach (ImportListElCurve listEl in listElCurves)
                {
                    int startCurve = int.Parse(listEl.StartKM) * 1000 + int.Parse(listEl.StartM);
                    int finalCurve = int.Parse(listEl.FinalKM) * 1000 + int.Parse(listEl.FinalM);

                    if (listCurveIDs.Where(tmp => tmp.OldTrackID == listEl.OldTrackID && startCurve >= int.Parse(tmp.StartKM) * 1000 + int.Parse(tmp.StartM) && finalCurve <= int.Parse(tmp.FinalKM) * 1000 + int.Parse(tmp.FinalM)).Count() > 0)
                    {
                        Int64 curveID = listCurveIDs.Where(tmp => tmp.OldTrackID == listEl.OldTrackID && startCurve >= int.Parse(tmp.StartKM) * 1000 + int.Parse(tmp.StartM) && finalCurve <= int.Parse(tmp.FinalKM) * 1000 + int.Parse(tmp.FinalM)).FirstOrDefault().CurveID;
                        if (curveID != 0)
                            ExportImportService.Execute("insert into apr_elcurve (curve_id, start_km, start_m, final_km, final_m, lvl, transition_1, transition_2) values ("
                                + curveID.ToString() + ", " + listEl.StartKM + ", " + listEl.StartM + ", " + listEl.FinalKM + ", " + listEl.FinalM + ", " + listEl.Lvl + ", "
                                + listEl.T1 + ", " + listEl.T2 + ")");
                    }
                }

                listCurveIDs.Clear();
                listStCurves.Clear();
                listElCurves.Clear();

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_PODR.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;

                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                ImportListPDBID listPDBID = new ImportListPDBID();
                                listPDBID.FIO = z_row.Attributes["FIO_PODR"] != null ? z_row.Attributes["FIO_PODR"].Value : "";
                                listPDBID.PDB = z_row.Attributes["NPDB"].Value;
                                listPDBID.PD = z_row.Attributes["NPD"].Value;
                                listPDBID.PCHU = z_row.Attributes["MEX"].Value;
                                listPDBID.Distance = z_row.Attributes["NPCH"].Value;
                                listPDBID.OldTrackID = z_row.Attributes["PUTGL_ID"].Value;

                                if (listPDBIDs.Where(tmp => tmp.Distance == listPDBID.Distance).Count() < 1)
                                {
                                    listPDBID.DistanceID = ExportImportService.ImportQueryReturnLong("insert into ADM_DISTANCE (CODE, NAME, ADM_NOD_ID) values (\'" + listPDBID.Distance + "\', \'" + listPDBID.Distance + "\', " + nod_id.ToString() + ") returning ID");
                                }
                                else
                                {
                                    listPDBID.DistanceID = listPDBIDs.Where(tmp => tmp.Distance == listPDBID.Distance).FirstOrDefault().DistanceID;
                                }

                                if (listPDBID.PD != "0")
                                {
                                    listPDBID.PCHUID = listPDBIDs.Where(tmp => tmp.PCHU == listPDBID.PCHU && tmp.Distance == listPDBID.Distance).FirstOrDefault().PCHUID;

                                    if (listPDBID.PDB != "0")
                                    {
                                        listPDBID.PDID = listPDBIDs.Where(tmp => tmp.PD == listPDBID.PD && tmp.PCHU == listPDBID.PCHU && tmp.Distance == listPDBID.Distance).FirstOrDefault().PDID;

                                        if (listPDBIDs.Where(tmp => tmp.PDB == listPDBID.PDB && tmp.FIO == listPDBID.FIO && tmp.PD == listPDBID.PD && tmp.PCHU == listPDBID.PCHU && tmp.Distance == listPDBID.Distance).Count() < 1)
                                        {
                                            listPDBID.PDBID = ExportImportService.ImportQueryReturnLong("insert into ADM_PDB (CODE, CHIEF_FULLNAME, ADM_PD_ID) values (\'" + listPDBID.PDB + "\', \'" + listPDBID.FIO + "\', " + listPDBID.PDID.ToString() + ") returning ID");
                                            listPDBID.Start = int.Parse(z_row.Attributes["KMN"].Value) * 1000 + int.Parse(z_row.Attributes["MN"].Value);
                                            listPDBID.Final = int.Parse(z_row.Attributes["KMK"].Value) * 1000 + int.Parse(z_row.Attributes["MK"].Value);
                                            listPDBIDs.Add(listPDBID);
                                        }
                                        else
                                        {
                                            listPDBID.PDBID = listPDBIDs.Where(tmp => tmp.PDB == listPDBID.PDB && tmp.FIO == listPDBID.FIO && tmp.PD == listPDBID.PD && tmp.PCHU == listPDBID.PCHU && tmp.Distance == listPDBID.Distance).FirstOrDefault().PDBID;
                                            listPDBID.Start = int.Parse(z_row.Attributes["KMN"].Value) * 1000 + int.Parse(z_row.Attributes["MN"].Value);
                                            listPDBID.Final = int.Parse(z_row.Attributes["KMK"].Value) * 1000 + int.Parse(z_row.Attributes["MK"].Value);
                                            listPDBIDs.Add(listPDBID);
                                        }

                                        if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(listPDBID.OldTrackID)).Count() > 0)
                                        {
                                            Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(listPDBID.OldTrackID)).First().NewTrackID;
                                            if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 9").Count() > 0)
                                            {
                                                Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 9");
                                                ExportImportService.Execute("insert into TPL_PDB_SECTION (ADM_PDB_ID, PERIOD_ID, START_KM, START_M, FINAL_KM, FINAL_M) values (" + listPDBID.PDBID.ToString() + ", " + periodID.ToString() + ", " + z_row.Attributes["KMN"].Value + ", " + z_row.Attributes["MN"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ")");
                                            }
                                            else
                                            {
                                                Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 9) returning ID");
                                                ExportImportService.Execute("insert into TPL_PDB_SECTION (ADM_PDB_ID, PERIOD_ID, START_KM, START_M, FINAL_KM, FINAL_M) values (" + listPDBID.PDBID.ToString() + ", " + periodID.ToString() + ", " + z_row.Attributes["KMN"].Value + ", " + z_row.Attributes["MN"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ")");
                                            }
                                        }
                                    }
                                    else if (listPDBIDs.Where(tmp => tmp.PD == listPDBID.PD && tmp.PCHU == listPDBID.PCHU && tmp.Distance == listPDBID.Distance).Count() < 1)
                                    {
                                        listPDBID.PDID = ExportImportService.ImportQueryReturnLong("insert into ADM_PD (CODE, CHIEF_FULLNAME, ADM_PCHU_ID) values (\'" + listPDBID.PD + "\', \'" + listPDBID.FIO + "\', " + listPDBID.PCHUID.ToString() + ") returning ID");

                                        listPDBID.PDBID = -1;
                                        listPDBID.Start = 0;
                                        listPDBID.Final = 0;
                                        listPDBIDs.Add(listPDBID);
                                    }
                                }
                                else if (listPDBIDs.Where(tmp => tmp.PCHU == listPDBID.PCHU && tmp.Distance == listPDBID.Distance).Count() < 1)
                                {
                                    listPDBID.PCHUID = ExportImportService.ImportQueryReturnLong("insert into ADM_PCHU (CODE, CHIEF_FULLNAME, ADM_DISTANCE_ID) values (\'" + listPDBID.PCHU + "\', \'" + listPDBID.FIO + "\', " + listPDBID.DistanceID.ToString() + ") returning ID");

                                    listPDBID.PDID = -1;
                                    listPDBID.PDBID = -1;
                                    listPDBID.Start = 0;
                                    listPDBID.Final = 0;
                                    listPDBIDs.Add(listPDBID);
                                }
                            }

                            List<string> trackIDs = new List<string>();

                            foreach (ImportListPDBID itemList in listPDBIDs)
                            {
                                if (!trackIDs.Contains(itemList.OldTrackID))
                                {
                                    trackIDs.Add(itemList.OldTrackID);
                                }
                            }

                            foreach (string trackID in trackIDs)
                            {
                                List<ImportListPDBID> tempList = listPDBIDs.Where(tmp => tmp.OldTrackID == trackID && tmp.Start != 0 && tmp.Final != 0).OrderBy(tmp => tmp.Start).ThenBy(tmp => tmp.Final).ToList();

                                List<Int64> distIDs = new List<Int64>();

                                foreach (ImportListPDBID itemList in tempList)
                                {
                                    if (!distIDs.Contains(itemList.DistanceID))
                                    {
                                        distIDs.Add(itemList.DistanceID);
                                    }
                                }

                                foreach (Int64 distID in distIDs)
                                {
                                    List<ImportListPDBID> temp2List = tempList.Where(tmp => tmp.DistanceID == distID).OrderBy(tmp => tmp.Start).ThenBy(tmp => tmp.Final).ToList();

                                    if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(trackID)).Count() > 0 && temp2List.Count > 0)
                                    {
                                        Int64 newTrackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(trackID)).First().NewTrackID;
                                        if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + newTrackID.ToString() + " and MTO_TYPE = 0").Count() > 0)
                                        {
                                            Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + newTrackID.ToString() + " and MTO_TYPE = 0");
                                            ExportImportService.Execute("insert into tpl_dist_section (adm_distance_id, PERIOD_ID, START_KM, START_M, FINAL_KM, FINAL_M) values (" + distID.ToString() + ", " + periodID.ToString() + ", " + (temp2List.First().Start / 1000).ToString() + ", " + (temp2List.First().Start % 1000).ToString() + ", " + (temp2List.Last().Final / 1000).ToString() + ", " + (temp2List.Last().Final % 1000).ToString() + ")");
                                        }
                                        else
                                        {
                                            Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + newTrackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 0) returning ID");
                                            ExportImportService.Execute("insert into tpl_dist_section (adm_distance_id, PERIOD_ID, START_KM, START_M, FINAL_KM, FINAL_M) values (" + distID.ToString() + ", " + periodID.ToString() + ", " + (temp2List.First().Start / 1000).ToString() + ", " + (temp2List.First().Start % 1000).ToString() + ", " + (temp2List.Last().Final / 1000).ToString() + ", " + (temp2List.Last().Final % 1000).ToString() + ")");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                listPDBIDs.Clear();

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_UPS.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string objectID;
                                if (z_row.Attributes["VID"].Value == "Уравн. приб")
                                    objectID = "2";
                                else if (z_row.Attributes["VID"].Value == "Уравн. стык")
                                    objectID = "1";
                                else objectID = "1";
                                if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                {
                                    Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                    if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 21").Count() > 0)
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 21");
                                        ExportImportService.Execute("insert into apr_cham_joint (PERIOD_ID, start_km, start_m, final_km, final_m, type_id) values (" + periodID.ToString() + ", " + z_row.Attributes["KMN"].Value + ", " + z_row.Attributes["MN"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ", " + objectID + ")");
                                    }
                                    else
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 21) returning ID");
                                        ExportImportService.Execute("insert into apr_cham_joint (PERIOD_ID, start_km, start_m, final_km, final_m, type_id) values (" + periodID.ToString() + ", " + z_row.Attributes["KMN"].Value + ", " + z_row.Attributes["MN"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ", " + objectID + ")");
                                    }
                                }
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_UVN.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string levelID;
                                if (z_row.Attributes["ID_NIT"].Value == "1")
                                    levelID = "6";
                                else if (z_row.Attributes["ID_NIT"].Value == "-1")
                                    levelID = "-6";
                                else levelID = "0";
                                if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                {
                                    Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                    if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 8").Count() > 0)
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 8");
                                        ExportImportService.Execute("insert into apr_elevation (PERIOD_ID, level_id, START_KM, START_M, FINAL_KM, FINAL_M) values (" + periodID.ToString() + ", " + levelID + ", " + z_row.Attributes["KMN"].Value + ", " + z_row.Attributes["MN"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ")");
                                    }
                                    else
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 8) returning ID");
                                        ExportImportService.Execute("insert into apr_elevation (PERIOD_ID, level_id, START_KM, START_M, FINAL_KM, FINAL_M) values (" + periodID.ToString() + ", " + levelID + ", " + z_row.Attributes["KMN"].Value + ", " + z_row.Attributes["MN"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ")");
                                    }
                                }
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_GB.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string crosstieID;
                                if (z_row.Attributes["GOD"].Value == "0")
                                    crosstieID = "1";
                                else if (z_row.Attributes["GOD"].Value == "1")
                                    crosstieID = "2";
                                else crosstieID = "-1";
                                if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                {
                                    Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                    if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 1").Count() > 0)
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 1");
                                        ExportImportService.Execute("insert into APR_CROSSTIE (PERIOD_ID, CROSSTIE_TYPE_ID, START_KM, START_M, FINAL_KM, FINAL_M) values (" + periodID.ToString() + ", " + crosstieID + ", " + z_row.Attributes["KMN"].Value + ", " + z_row.Attributes["MN"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ")");
                                    }
                                    else
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 1) returning ID");
                                        ExportImportService.Execute("insert into APR_CROSSTIE (PERIOD_ID, CROSSTIE_TYPE_ID, START_KM, START_M, FINAL_KM, FINAL_M) values (" + periodID.ToString() + ", " + crosstieID + ", " + z_row.Attributes["KMN"].Value + ", " + z_row.Attributes["MN"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ")");
                                    }
                                }
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_Commun.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string objectID;
                                if (z_row.Attributes["NAME"].Value.ToLower() == "энергокабель")
                                    objectID = "85";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "нефтепровод")
                                    objectID = "86";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "кабель связи")
                                    objectID = "87";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "водопровод")
                                    objectID = "88";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "канализация")
                                    objectID = "89";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "турбопровод")
                                    objectID = "90";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "теплотрасса")
                                    objectID = "91";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "теплосеть")
                                    objectID = "92";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "кабельная линия")
                                    objectID = "93";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "влэп")
                                    objectID = "94";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "магистральный кабель")
                                    objectID = "95";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "путепровод")
                                    objectID = "96";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "трасса гидрозолоудаления")
                                    objectID = "97";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "клэп")
                                    objectID = "98";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "коллектор канализационный")
                                    objectID = "99";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "золопровод")
                                    objectID = "100";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "паропровод")
                                    objectID = "101";
                                else if (z_row.Attributes["NAME"].Value.ToLower() == "газопровод")
                                    objectID = "102";
                                else objectID = "35";
                                if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                {
                                    Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                    if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 31").Count() > 0)
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 31");
                                        ExportImportService.Execute("insert into apr_communication (PERIOD_ID, OBJECT_ID, KM, METER) values (" + periodID.ToString() + ", " + objectID + ", " + z_row.Attributes["KM"].Value + ", " + z_row.Attributes["M"].Value + ")");
                                    }
                                    else
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 31) returning ID");
                                        ExportImportService.Execute("insert into apr_communication (PERIOD_ID, OBJECT_ID, KM, METER) values (" + periodID.ToString() + ", " + objectID + ", " + z_row.Attributes["KM"].Value + ", " + z_row.Attributes["M"].Value + ")");
                                    }
                                }
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_IsoJoint.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                {
                                    Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                    if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 19").Count() > 0)
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 19");
                                        ExportImportService.Execute("insert into TPL_PROFILE_OBJECT (PERIOD_ID, OBJECT_ID, SIDE_ID, KM, METER) values (" + periodID.ToString() + ", 26, 0, " + z_row.Attributes["KM"].Value + ", " + z_row.Attributes["M"].Value + ")");
                                    }
                                    else
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 19) returning ID");
                                        ExportImportService.Execute("insert into TPL_PROFILE_OBJECT (PERIOD_ID, OBJECT_ID, SIDE_ID, KM, METER) values (" + periodID.ToString() + ", 26, 0, " + z_row.Attributes["KM"].Value + ", " + z_row.Attributes["M"].Value + ")");
                                    }
                                }
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_KU.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                {
                                    Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                    if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 28").Count() > 0)
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 28");
                                        ExportImportService.Execute("insert into tpl_check_sections (PERIOD_ID, start_km, start_m, final_km, final_m, avg_width, avg_level, sko_width, sko_level) values (" + periodID.ToString() + ", " + z_row.Attributes["KMN"].Value + ", " + z_row.Attributes["MN"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ", " + z_row.Attributes["MO_S"].Value + ", " + z_row.Attributes["MO_U"].Value + ", " + z_row.Attributes["SKO_S"].Value + ", " + z_row.Attributes["SKO_U"].Value + ")");
                                    }
                                    else
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 28) returning ID");
                                        ExportImportService.Execute("insert into tpl_check_sections (PERIOD_ID, start_km, start_m, final_km, final_m, avg_width, avg_level, sko_width, sko_level) values (" + periodID.ToString() + ", " + z_row.Attributes["KMN"].Value + ", " + z_row.Attributes["MN"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ", " + z_row.Attributes["MO_S"].Value + ", " + z_row.Attributes["MO_U"].Value + ", " + z_row.Attributes["SKO_S"].Value + ", " + z_row.Attributes["SKO_U"].Value + ")");
                                    }
                                }
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_DefEarth.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string defEarth = z_row.Attributes["NAME"].Value.ToLower();
                                int defEarthID = 0;

                                switch (defEarth)
                                {
                                    case "выпирание грунтов в выемке":
                                        defEarthID = 1;
                                        break;
                                    case "карсты":
                                        defEarthID = 2;
                                        break;
                                    case "осадка из-за выпора грунтов":
                                        defEarthID = 3;
                                        break;
                                    case "осадка из-за сползания насыпи":
                                        defEarthID = 4;
                                        break;
                                    case "расползание насыпи":
                                        defEarthID = 5;
                                        break;
                                    case "сплыв откоса выемки":
                                        defEarthID = 6;
                                        break;
                                    case "вывал":
                                        defEarthID = 7;
                                        break;
                                    case "оползень":
                                        defEarthID = 8;
                                        break;
                                    case "оползень, осадка":
                                        defEarthID = 9;
                                        break;
                                    case "оползень, сплыв":
                                        defEarthID = 10;
                                        break;
                                    case "сплыв":
                                        defEarthID = 11;
                                        break;
                                    case "сплыв, оползень":
                                        defEarthID = 12;
                                        break;
                                }

                                if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                {
                                    Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                    if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 33").Count() > 0)
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 33");
                                        ExportImportService.Execute("insert into apr_defects_earth (PERIOD_ID, start_km, start_m, final_km, final_m, type_id) values (" + periodID.ToString() + ", " + z_row.Attributes["KM"].Value + ", " + z_row.Attributes["M"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ", " + defEarthID.ToString() + ")");
                                    }
                                    else
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 33) returning ID");
                                        ExportImportService.Execute("insert into apr_defects_earth (PERIOD_ID, start_km, start_m, final_km, final_m, type_id) values (" + periodID.ToString() + ", " + z_row.Attributes["KM"].Value + ", " + z_row.Attributes["M"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ", " + defEarthID.ToString() + ")");
                                    }
                                }
                            }
                        }
                    }
                }

                progressBar.PerformStep();
                foreach (FileInfo file in fileInfo.Where(tmp => tmp.Name == "KV_RIL.xml"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file.FullName);
                    XmlElement xmlElement = xmlDocument.DocumentElement;
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        if (xmlNode.Name == "rs:data")
                        {
                            foreach (XmlNode z_row in xmlNode.ChildNodes)
                            {
                                string sideID = "2";
                                if (listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).Count() > 0)
                                {
                                    Int64 trackID = listIDs.Where(tmp => tmp.OldTrackID == int.Parse(z_row.Attributes["PUTGL_ID"].Value)).First().NewTrackID;
                                    if (ExportImportService.ImportQueryReturnListLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 13").Count() > 0)
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("select ID from TPL_PERIOD where ADM_TRACK_ID = " + trackID.ToString() + " and MTO_TYPE = 13");
                                        ExportImportService.Execute("insert into APR_STRAIGHTENING_THREAD (PERIOD_ID, SIDE_ID, START_KM, START_M, FINAL_KM, FINAL_M) values (" + periodID.ToString() + ", " + sideID + ", " + z_row.Attributes["KMN"].Value + ", " + z_row.Attributes["MN"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ")");
                                    }
                                    else
                                    {
                                        Int64 periodID = ExportImportService.ImportQueryReturnLong("insert into TPL_PERIOD (ADM_TRACK_ID, START_DATE, FINAL_DATE, MTO_TYPE) values (" + trackID.ToString() + ", \'" + period.Start_Date.ToString("yyyy-MM-dd") + "\', \'" + period.Final_Date.ToString("yyyy-MM-dd") + "\', 13) returning ID");
                                        ExportImportService.Execute("insert into APR_STRAIGHTENING_THREAD (PERIOD_ID, SIDE_ID, START_KM, START_M, FINAL_KM, FINAL_M) values (" + periodID.ToString() + ", " + sideID + ", " + z_row.Attributes["KMN"].Value + ", " + z_row.Attributes["MN"].Value + ", " + z_row.Attributes["KMK"].Value + ", " + z_row.Attributes["MK"].Value + ")");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            /*
                    case "KV_PKO":
                    case "KV_RPERIOD":
            */
            return true;
        }

        internal static bool Import(FileInfo[] fileInfo, MetroFramework.Controls.MetroProgressBar progressBar, int import_type)
        {
            progressBar.Value = 0;
            progressBar.Step = 100 / 50;
            bool result = true;
            List<ImportListId> roadIds = new List<ImportListId>();
            List<ImportListId> tempIds = new List<ImportListId>();
            List<ImportListId> temp2Ids = new List<ImportListId>();
            List<ImportListId> distanceIds = new List<ImportListId>();
            List<ImportListId> pdbIds = new List<ImportListId>();
            List<ImportListId> stationIds = new List<ImportListId>();
            List<ImportListId> directionIds = new List<ImportListId>();
            List<ImportListId> trackIds = new List<ImportListId>();
            List<ImportListId> periodIds = new List<ImportListId>();

            if (fileInfo.Any(tmp => tmp.Name == "adm_road.csv"))
            {
                //ADM STRUCTURE
                using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "adm_road.csv").FullName))
                {
                    csvParser.CommentTokens = new string[] { "#" };
                    csvParser.SetDelimiters(new string[] { "," });
                    csvParser.HasFieldsEnclosedInQuotes = true;

                    string[] fields = csvParser.ReadFields();
                    int id = fields.ToList().IndexOf("id"), code = fields.ToList().IndexOf("code"), name = fields.ToList().IndexOf("name");

                    while (!csvParser.EndOfData)
                    {
                        fields = csvParser.ReadFields();
                        string Id = fields[id];
                        string Code = fields[code];
                        string Name = fields[name];

                        long newRoadId = ExportImportService.ImportQueryReturnLong($@"insert into adm_road (code, name) values ('{Code}', '{Name}') returning coalesce(id, -1)");

                        if (newRoadId > 0)
                            roadIds.Add(new ImportListId {
                                NewId = newRoadId,
                                OldId = long.Parse(Id)
                            });
                    }
                }

                if (fileInfo.Any(tmp => tmp.Name == "adm_nod.csv") && roadIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "adm_nod.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), code = fields.ToList().IndexOf("code"), name = fields.ToList().IndexOf("name"), roadid = fields.ToList().IndexOf("adm_road_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string Code = fields[code];
                            string Name = fields[name];
                            string RoadId = fields[roadid];

                            if (roadIds.Any(r => r.OldId.Equals(long.Parse(RoadId))))
                            {
                                long newId = ExportImportService.ImportQueryReturnLong($@"insert into adm_nod (code, name, adm_road_id) values ('{Code}', '{Name}', {roadIds.First(r => r.OldId.Equals(long.Parse(RoadId))).NewId}) returning coalesce(id, -1)");

                                if (newId > 0)
                                    tempIds.Add(new ImportListId {
                                        NewId = newId,
                                        OldId = long.Parse(Id)
                                    });
                            }
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "adm_distance.csv") && tempIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "adm_distance.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), code = fields.ToList().IndexOf("code"), name = fields.ToList().IndexOf("name"), nodid = fields.ToList().IndexOf("adm_nod_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string Code = fields[code];
                            string Name = fields[name];
                            string NodId = fields[nodid];

                            if (tempIds.Any(r => r.OldId.Equals(long.Parse(NodId))))
                            {
                                long newId = ExportImportService.ImportQueryReturnLong($@"insert into adm_distance (code, name, adm_nod_id) values ('{Code}', '{Name}', {tempIds.First(r => r.OldId.Equals(long.Parse(NodId))).NewId}) returning coalesce(id, -1)");

                                if (newId > 0)
                                    distanceIds.Add(new ImportListId {
                                        NewId = newId,
                                        OldId = long.Parse(Id)
                                    });
                            }
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "adm_station.csv") && tempIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "adm_station.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), code = fields.ToList().IndexOf("code"), name = fields.ToList().IndexOf("name"), nodid = fields.ToList().IndexOf("adm_nod_id"), typeid = fields.ToList().IndexOf("type_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string TypeId = fields[typeid];
                            string NodId = fields[nodid];
                            string Code = fields[code];
                            string Name = fields[name];

                            if (tempIds.Any(r => r.OldId.Equals(long.Parse(NodId))))
                            {
                                long newId = ExportImportService.ImportQueryReturnLong($@"insert into adm_station (code, name, adm_nod_id, type_id) values ('{Code}', '{Name}', {tempIds.First(r => r.OldId.Equals(long.Parse(NodId))).NewId}, {TypeId}) returning coalesce(id, -1)");

                                if (newId > 0)
                                    stationIds.Add(new ImportListId {
                                        NewId = newId,
                                        OldId = long.Parse(Id)
                                    });
                            }
                        }
                    }
                tempIds.Clear();
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "adm_pchu.csv") && distanceIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "adm_pchu.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), code = fields.ToList().IndexOf("code"), name = fields.ToList().IndexOf("chief_fullname"), distanceid = fields.ToList().IndexOf("adm_distance_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string DistanceId = fields[distanceid];
                            string Code = fields[code];
                            string Name = fields[name];

                            if (distanceIds.Any(r => r.OldId.Equals(long.Parse(DistanceId))))
                            {
                                long newId = ExportImportService.ImportQueryReturnLong($@"insert into adm_pchu (code, chief_fullname, adm_distance_id) values ('{Code}', '{Name}', {distanceIds.First(r => r.OldId.Equals(long.Parse(DistanceId))).NewId}) returning coalesce(id, -1)");

                                if (newId > 0)
                                    tempIds.Add(new ImportListId {
                                        NewId = newId,
                                        OldId = long.Parse(Id)
                                    });
                            }
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "adm_pd.csv") && tempIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "adm_pd.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), code = fields.ToList().IndexOf("code"), name = fields.ToList().IndexOf("chief_fullname"), pchuid = fields.ToList().IndexOf("adm_pchu_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PchuId = fields[pchuid];
                            string Code = fields[code];
                            string Name = fields[name];

                            if (tempIds.Any(r => r.OldId.Equals(long.Parse(PchuId))))
                            {
                                long newId = ExportImportService.ImportQueryReturnLong($@"insert into adm_pd (code, chief_fullname, adm_pchu_id) values ('{Code}', '{Name}', {tempIds.First(r => r.OldId.Equals(long.Parse(PchuId))).NewId}) returning coalesce(id, -1)");

                                if (newId > 0)
                                    temp2Ids.Add(new ImportListId {
                                        NewId = newId,
                                        OldId = long.Parse(Id)
                                    });
                            }
                        }
                    }
                tempIds.Clear();
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "adm_pdb.csv") && temp2Ids.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "adm_pdb.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), code = fields.ToList().IndexOf("code"), name = fields.ToList().IndexOf("chief_fullname"), pdid = fields.ToList().IndexOf("adm_pd_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PdId = fields[pdid];
                            string Code = fields[code];
                            string Name = fields[name];

                            if (temp2Ids.Any(r => r.OldId.Equals(long.Parse(PdId))))
                            {
                                long newId = ExportImportService.ImportQueryReturnLong($@"insert into adm_pdb (code, chief_fullname, adm_pd_id) values ('{Code}', '{Name}', {temp2Ids.First(r => r.OldId.Equals(long.Parse(PdId))).NewId}) returning coalesce(id, -1)");

                                if (newId > 0)
                                    pdbIds.Add(new ImportListId {
                                        NewId = newId,
                                        OldId = long.Parse(Id)
                                    });
                            }
                        }
                    }
                temp2Ids.Clear();
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "stw_park.csv") && stationIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "stw_park.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), typeid = fields.ToList().IndexOf("type_id"), name = fields.ToList().IndexOf("name"), stationid = fields.ToList().IndexOf("adm_station_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string TypeId = fields[typeid];
                            string StationId = fields[stationid];
                            string Name = fields[name];

                            if (stationIds.Any(r => r.OldId.Equals(long.Parse(StationId))))
                            {
                                long newId = ExportImportService.ImportQueryReturnLong($@"insert into stw_park (name, adm_station_id, type_id) values ('{Name}', {stationIds.First(r => r.OldId.Equals(long.Parse(StationId))).NewId}, {TypeId}) returning coalesce(id, -1)");

                                if (newId > 0)
                                    tempIds.Add(new ImportListId {
                                        NewId = newId,
                                        OldId = long.Parse(Id)
                                    });
                            }
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "stw_object.csv") && tempIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "stw_object.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), typeid = fields.ToList().IndexOf("type_id"), name = fields.ToList().IndexOf("name"), parkid = fields.ToList().IndexOf("stw_park_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string Name = fields[name];
                            string ParkId = fields[parkid];
                            string TypeId = fields[typeid];
                            
                            if (tempIds.Any(r => r.OldId.Equals(long.Parse(ParkId))))
                                ExportImportService.Execute($@"insert into stw_object (name, stw_park_id, type_id) values ('{Name}', {tempIds.First(r => r.OldId.Equals(long.Parse(ParkId))).NewId}, {TypeId}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "adm_direction.csv"))
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "adm_direction.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), code = fields.ToList().IndexOf("code"), name = fields.ToList().IndexOf("name");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string Code = fields[code];
                            string Name = fields[name];

                            long newId = ExportImportService.ImportQueryReturnLong($@"insert into adm_direction (code, name) values ('{Code}', '{Name}') returning coalesce(id, -1)");

                            if (newId > 0)
                                directionIds.Add(new ImportListId {
                                    NewId = newId,
                                    OldId = long.Parse(Id)
                                });
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "road_direction.csv") && roadIds.Any() && directionIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "road_direction.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), roadid = fields.ToList().IndexOf("road_id"), directionid = fields.ToList().IndexOf("direction_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string RoadId = fields[roadid];
                            string DirectionId = fields[directionid];

                            if (roadIds.Any(r => r.OldId.Equals(long.Parse(RoadId))) && directionIds.Any(d => d.OldId.Equals(long.Parse(DirectionId))))
                            {
                                ExportImportService.Execute($@"insert into road_direction (road_id, direction_id) values ({roadIds.First(r => r.OldId.Equals(long.Parse(RoadId))).NewId}, {directionIds.First(d => d.OldId.Equals(long.Parse(DirectionId))).NewId}) returning coalesce(id, -1)");

                                if (!temp2Ids.Any(t => t.OldId.Equals(long.Parse(DirectionId))))
                                    temp2Ids.Add(new ImportListId {
                                        NewId = directionIds.First(d => d.OldId.Equals(long.Parse(DirectionId))).NewId,
                                        OldId = long.Parse(DirectionId)
                                    });
                            }
                        }
                    }
                directionIds.Clear();
                directionIds.AddRange(temp2Ids);
                temp2Ids.Clear();
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "adm_track.csv"))
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "adm_track.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), code = fields.ToList().IndexOf("code"), directionid = fields.ToList().IndexOf("adm_direction_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string Code = fields[code];
                            string DirectionId = fields[directionid];

                            if (!string.IsNullOrEmpty(DirectionId))
                            {
                                if (directionIds.Any(r => r.OldId.Equals(long.Parse(DirectionId))))
                                {
                                    long newId = ExportImportService.ImportQueryReturnLong($@"insert into adm_track (code, adm_direction_id) values ('{Code}', {directionIds.First(r => r.OldId.Equals(long.Parse(DirectionId))).NewId}) returning coalesce(id, -1)");

                                    if (newId > 0)
                                        trackIds.Add(new ImportListId {
                                            NewId = newId,
                                            OldId = long.Parse(Id)
                                        });
                                }
                            }
                            else
                            {
                                long newId = ExportImportService.ImportQueryReturnLong($@"insert into adm_track (code, adm_direction_id) values ('{Code}', null) returning coalesce(id, -1)");

                                if (newId > 0)
                                    temp2Ids.Add(new ImportListId {
                                        NewId = newId,
                                        OldId = long.Parse(Id)
                                    });
                            }
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "stw_track.csv") && temp2Ids.Any() && stationIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "stw_track.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), trackid = fields.ToList().IndexOf("adm_track_id"), stationid = fields.ToList().IndexOf("adm_station_id"), typeid = fields.ToList().IndexOf("type_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string StationId = fields[stationid];
                            string TypeId = fields[typeid];
                            string TrackId = fields[trackid];

                            if (stationIds.Any(r => r.OldId.Equals(long.Parse(StationId))) && temp2Ids.Any(r => r.OldId.Equals(long.Parse(TrackId))))
                            {
                                ExportImportService.Execute($@"insert into stw_track (adm_station_id, adm_track_id, type_id) values ({stationIds.First(r => r.OldId.Equals(long.Parse(StationId))).NewId}, {temp2Ids.First(r => r.OldId.Equals(long.Parse(TrackId))).NewId}, {TypeId}) returning coalesce(id, -1)");

                                if (!trackIds.Any(r => r.OldId.Equals(long.Parse(TrackId))))
                                    trackIds.Add(new ImportListId {
                                        NewId = temp2Ids.First(r => r.OldId.Equals(long.Parse(TrackId))).NewId,
                                        OldId = long.Parse(TrackId)
                                    });
                            }
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "stw_park_track.csv") && temp2Ids.Any() && tempIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "stw_park_track.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), parkid = fields.ToList().IndexOf("stw_park_id"), typeid = fields.ToList().IndexOf("type_id"), trackid = fields.ToList().IndexOf("adm_track_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string ParkId = fields[parkid];
                            string TypeId = fields[typeid];
                            string TrackId = fields[trackid];

                            if (tempIds.Any(r => r.OldId.Equals(long.Parse(ParkId))) && temp2Ids.Any(r => r.OldId.Equals(long.Parse(TrackId))))
                            {
                                ExportImportService.Execute($@"insert into stw_park_track (stw_park_id, adm_track_id, type_id) values ({tempIds.First(r => r.OldId.Equals(long.Parse(ParkId))).NewId}, {temp2Ids.First(r => r.OldId.Equals(long.Parse(TrackId))).NewId}, {TypeId}) returning coalesce(id, -1)");

                                if (!trackIds.Any(r => r.OldId.Equals(long.Parse(TrackId))))
                                    trackIds.Add(new ImportListId {
                                        NewId = temp2Ids.First(r => r.OldId.Equals(long.Parse(TrackId))).NewId,
                                        OldId = long.Parse(TrackId)
                                    });
                            }
                        }
                    }
                tempIds.Clear();
                temp2Ids.Clear();
                roadIds.Clear();
                directionIds.Clear();
                progressBar.PerformStep();

                //PERIOD
                if (fileInfo.Any(tmp => tmp.Name == "tpl_period.csv") && trackIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "tpl_period.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), startdate = fields.ToList().IndexOf("start_date"), finaldate = fields.ToList().IndexOf("final_date"), trackid = fields.ToList().IndexOf("adm_track_id"), changedate = fields.ToList().IndexOf("change_date"), comment = fields.ToList().IndexOf("comment"), mtotype = fields.ToList().IndexOf("mto_type");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string TrackId = fields[trackid];
                            string StartDate = fields[startdate];
                            string FinalDate = fields[finaldate];
                            string ChangeDate = fields[changedate];
                            string Comment = fields[comment];
                            string MtoType = fields[mtotype];

                            if (trackIds.Any(r => r.OldId.Equals(long.Parse(TrackId))))
                            {
                                long newId = ExportImportService.ImportQueryReturnLong($@"insert into tpl_period (adm_track_id, start_date, final_date, change_date, comment, mto_type) values ({trackIds.First(r => r.OldId.Equals(long.Parse(TrackId))).NewId}, '{StartDate}', '{FinalDate}', {(string.IsNullOrEmpty(ChangeDate) ? "null" : "\'" + ChangeDate + "\'")}, {(string.IsNullOrEmpty(Comment) ? "null" : "\'" + Comment + "\'")}, {MtoType}) returning coalesce(id, -1)");

                                if (newId > 0)
                                    periodIds.Add(new ImportListId {
                                        NewId = newId,
                                        OldId = long.Parse(Id)
                                    });
                            }
                        }
                    }
                progressBar.PerformStep();

                //MTO STRUCTURE
                if (fileInfo.Any(tmp => tmp.Name == "apr_artificial_construction.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_artificial_construction.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), typeid = fields.ToList().IndexOf("type_id"), km = fields.ToList().IndexOf("km"), meter = fields.ToList().IndexOf("meter"), len = fields.ToList().IndexOf("len");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string Km = fields[km];
                            string Meter = fields[meter];
                            string TypeId = fields[typeid];
                            string Len = fields[len];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_artificial_construction (period_id, type_id, km, meter, len) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {TypeId}, {Km}, {Meter}, {Len}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_ballast.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_ballast.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), ballast = fields.ToList().IndexOf("ballast"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string Ballast = fields[ballast];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_ballast (period_id, start_km, start_m, final_km, final_m, ballast) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {Ballast}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_cham_joint.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_cham_joint.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), typeid = fields.ToList().IndexOf("type_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string TypeId = fields[typeid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_cham_joint (period_id, start_km, start_m, final_km, final_m, type_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {TypeId}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_communication.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_communication.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), objectid = fields.ToList().IndexOf("object_id"),
                            km = fields.ToList().IndexOf("km"), meter = fields.ToList().IndexOf("meter");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string Km = fields[km];
                            string Meter = fields[meter];
                            string ObjectId = fields[objectid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_communication (period_id, object_id, km, meter) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {ObjectId}, {Km}, {Meter}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_coordinate_gnss.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_coordinate_gnss.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"),
                            km = fields.ToList().IndexOf("km"), meter = fields.ToList().IndexOf("meter"),
                            alti = fields.ToList().IndexOf("altitude"), longti = fields.ToList().IndexOf("longtitude"), lati = fields.ToList().IndexOf("latitude"),
                            coord = fields.ToList().IndexOf("exact_coordinate"), height = fields.ToList().IndexOf("exact_height");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string Km = fields[km];
                            string Meter = fields[meter];
                            string Latitude = fields[lati];
                            string Longtitude = fields[longti];
                            string Altitude = fields[alti];
                            string Coordinate = fields[coord];
                            string Height = fields[height];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_coordinate_gnss (period_id, km, meter, latitude, longtitude, altitude, exact_coordinate, exact_height) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {Km}, {Meter}, {Latitude}, {Longtitude}, {Altitude}, {Coordinate}, {Height}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_crosstie.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_crosstie.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), typeid = fields.ToList().IndexOf("crosstie_type_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string TypeId = fields[typeid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_crosstie (period_id, start_km, start_m, final_km, final_m, crosstie_type_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {TypeId}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_curve.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_curve.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), sideid = fields.ToList().IndexOf("side_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string SideId = fields[sideid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                            {
                                long newId = ExportImportService.ImportQueryReturnLong($@"insert into apr_curve (period_id, start_km, start_m, final_km, final_m, side_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {SideId}) returning coalesce(id, -1)");

                                if (newId > 0)
                                {
                                    tempIds.Add(new ImportListId {
                                        NewId = newId,
                                        OldId = long.Parse(Id)
                                    });
                                }
                            }
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_defects_earth.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_defects_earth.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), typeid = fields.ToList().IndexOf("type_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string TypeId = fields[typeid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_defects_earth (period_id, start_km, start_m, final_km, final_m, type_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {TypeId}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_dimension.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_dimension.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), typeid = fields.ToList().IndexOf("type_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string TypeId = fields[typeid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_dimension (period_id, start_km, start_m, final_km, final_m, type_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {TypeId}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_elcurve.csv") && tempIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_elcurve.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), curveid = fields.ToList().IndexOf("curve_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m"),
                            lvl = fields.ToList().IndexOf("lvl"), t1 = fields.ToList().IndexOf("transition_1"), t2 = fields.ToList().IndexOf("transition_2");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string CurveId = fields[curveid];
                            string Lvl = fields[lvl];
                            string Tran1 = fields[t1];
                            string Tran2 = fields[t2];

                            if (tempIds.Any(r => r.OldId.Equals(long.Parse(CurveId))))
                                ExportImportService.Execute($@"insert into apr_elcurve (curve_id, start_km, start_m, final_km, final_m, lvl, transition_1, transition_2) values ({tempIds.First(r => r.OldId.Equals(long.Parse(CurveId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {Lvl}, {Tran1}, {Tran2}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_elevation.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_elevation.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), lvlid = fields.ToList().IndexOf("level_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string LevelId = fields[lvlid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_elevation (period_id, start_km, start_m, final_km, final_m, level_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {LevelId}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_long_rails.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_long_rails.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), typeid = fields.ToList().IndexOf("type_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string TypeId = fields[typeid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_long_rails (period_id, start_km, start_m, final_km, final_m, type_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {TypeId}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_norma_width.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_norma_width.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), norma = fields.ToList().IndexOf("norma_width"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string Norma = fields[norma];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_norma_width (period_id, start_km, start_m, final_km, final_m, norma_width) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {Norma}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_profmarks.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_profmarks.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), profil = fields.ToList().IndexOf("profil"),
                            km = fields.ToList().IndexOf("km"), meter = fields.ToList().IndexOf("meter");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string Km = fields[km];
                            string Meter = fields[meter];
                            string Profil = fields[4];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_profmarks (period_id, profil, km, meter) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {Profil}, {Km}, {Meter}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_rails_braces.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_rails_braces.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), typeid = fields.ToList().IndexOf("brace_type_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string TypeId = fields[typeid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_rails_braces (period_id, start_km, start_m, final_km, final_m, brace_type_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {TypeId}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_rails_sections.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_rails_sections.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), typeid = fields.ToList().IndexOf("type_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string TypeId = fields[typeid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_rails_sections (period_id, start_km, start_m, final_km, final_m, type_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {TypeId}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_ref_point.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_ref_point.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), mark = fields.ToList().IndexOf("mark"),
                            km = fields.ToList().IndexOf("km"), meter = fields.ToList().IndexOf("meter");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string Km = fields[km];
                            string Meter = fields[meter];
                            string Mark = fields[mark];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_ref_point (period_id, mark, km, meter) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {Mark}, {Km}, {Meter}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_rfid.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_rfid.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), mark = fields.ToList().IndexOf("mark"),
                            km = fields.ToList().IndexOf("km"), meter = fields.ToList().IndexOf("meter"), len = fields.ToList().IndexOf("len");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string Km = fields[km];
                            string Meter = fields[meter];
                            string Mark = fields[mark];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_rfid (period_id, mark, km, meter) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, '{Mark}', {Km}, {Meter}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_speed.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_speed.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), sapsan = fields.ToList().IndexOf("sapsan"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m"),
                            last = fields.ToList().IndexOf("lastochka"), pass = fields.ToList().IndexOf("passenger"), frei = fields.ToList().IndexOf("freight"), emfrei = fields.ToList().IndexOf("empty_freight");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string Sapsan = fields[sapsan];
                            string Lastochka = fields[last];
                            string Pass = fields[pass];
                            string Frei = fields[frei];
                            string EmFrei = fields[emfrei];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_speed (period_id, start_km, start_m, final_km, final_m, sapsan, lastochka, passenger, freight, empty_freight) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {Sapsan}, {Lastochka}, {Pass}, {Frei}, {EmFrei}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_stcurve.csv") && tempIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_stcurve.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), curveid = fields.ToList().IndexOf("curve_id"), wear = fields.ToList().IndexOf("wear"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m"),
                            radius = fields.ToList().IndexOf("radius"), t1 = fields.ToList().IndexOf("transition_1"), t2 = fields.ToList().IndexOf("transition_2"), width = fields.ToList().IndexOf("width");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string CurveId = fields[curveid];
                            string Radius = fields[radius];
                            string Tran1 = fields[t1];
                            string Tran2 = fields[t2];
                            string Width = fields[width];
                            string Wear = fields[wear];

                            if (tempIds.Any(r => r.OldId.Equals(long.Parse(CurveId))))
                                ExportImportService.Execute($@"insert into apr_stcurve (curve_id, start_km, start_m, final_km, final_m, radius, transition_1, transition_2, width, wear) values ({tempIds.First(r => r.OldId.Equals(long.Parse(CurveId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {Radius}, {Tran1}, {Tran2}, {Width}, {Wear}) returning coalesce(id, -1)");
                        }
                    }
                tempIds.Clear();
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_straightening_thread.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_straightening_thread.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), sideid = fields.ToList().IndexOf("side_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string SideId = fields[sideid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_straightening_thread (period_id, start_km, start_m, final_km, final_m, side_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {SideId}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_tempspeed.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_tempspeed.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m"),
                            reason = fields.ToList().IndexOf("reason_id"), pass = fields.ToList().IndexOf("passenger"), frei = fields.ToList().IndexOf("freight"), emfrei = fields.ToList().IndexOf("empty_freight");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string ReasonId = fields[reason];
                            string Pass = fields[pass];
                            string Frei = fields[frei];
                            string EmFrei = fields[emfrei];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_tempspeed (period_id, start_km, start_m, final_km, final_m, reason_id, passenger, freight, empty_freight) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {ReasonId}, {Pass}, {Frei}, {EmFrei}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_trackclass.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_trackclass.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), classid = fields.ToList().IndexOf("class_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string ClassId = fields[classid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_trackclass (period_id, start_km, start_m, final_km, final_m, class_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {ClassId}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_traffic.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_traffic.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), traffic = fields.ToList().IndexOf("traffic"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string Traffic = fields[traffic];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_traffic (period_id, start_km, start_m, final_km, final_m, traffic) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {Traffic}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "apr_waycat.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "apr_waycat.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), typeid = fields.ToList().IndexOf("type_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string TypeId = fields[typeid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into apr_waycat (period_id, start_km, start_m, final_km, final_m, type_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {TypeId}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "repair_project.csv") && trackIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "repair_project.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), trackid = fields.ToList().IndexOf("adm_track_id"), typeid = fields.ToList().IndexOf("type_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m"),
                            accept = fields.ToList().IndexOf("accept_id"), date = fields.ToList().IndexOf("repair_date");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string TrackId = fields[trackid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string TypeId = fields[typeid];
                            string AcceptId = fields[accept];
                            string RepairDate = fields[date];

                            if (trackIds.Any(r => r.OldId.Equals(long.Parse(TrackId))))
                                ExportImportService.Execute($@"insert into repair_project (adm_track_id, start_km, start_m, final_km, final_m, type_id, accept_id, repair_date) values ({trackIds.First(r => r.OldId.Equals(long.Parse(TrackId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {TypeId}, {AcceptId}, '{RepairDate}') returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "tpl_check_sections.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "tpl_check_sections.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m"),
                            aw = fields.ToList().IndexOf("avg_width"), al = fields.ToList().IndexOf("avg_level"), sw = fields.ToList().IndexOf("sko_width"), sl = fields.ToList().IndexOf("sko_level");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string AvgW = fields[aw];
                            string AvgL = fields[al];
                            string SkoW = fields[sw];
                            string SkoL = fields[sl];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into tpl_check_sections (period_id, start_km, start_m, final_km, final_m, avg_width, avg_level, sko_width, sko_level) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {AvgW}, {AvgL}, {SkoW}, {SkoL}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "tpl_deep.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "tpl_deep.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into tpl_deep (period_id, start_km, start_m, final_km, final_m) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "tpl_dist_section.csv") && periodIds.Any() && distanceIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "tpl_dist_section.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), distid = fields.ToList().IndexOf("adm_distance_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string DistanceId = fields[distid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))) && distanceIds.Any(r => r.OldId.Equals(long.Parse(DistanceId))))
                                ExportImportService.Execute($@"insert into tpl_dist_section (period_id, start_km, start_m, final_km, final_m, adm_distance_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {distanceIds.First(r => r.OldId.Equals(long.Parse(DistanceId))).NewId}) returning coalesce(id, -1)");
                        }
                    }
                distanceIds.Clear();
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "tpl_distance_between_tracks.csv") && periodIds.Any() && trackIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "tpl_distance_between_tracks.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), leftid = fields.ToList().IndexOf("left_adm_track_id"), rightid = fields.ToList().IndexOf("right_adm_track_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m"),
                            lm = fields.ToList().IndexOf("left_m"), rm = fields.ToList().IndexOf("right_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string LeftM = fields[lm];
                            string LeftTrack = fields[leftid];
                            string RightM = fields[rm];
                            string RightTrack = fields[rightid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))) && trackIds.Any(r => r.OldId.Equals(long.Parse(LeftTrack))) && trackIds.Any(r => r.OldId.Equals(long.Parse(RightTrack))))
                                ExportImportService.Execute($@"insert into tpl_distance_between_tracks (period_id, start_km, start_m, final_km, final_m, left_m, right_m, left_adm_track_id, right_adm_track_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {LeftM}, {RightM}, {trackIds.First(r => r.OldId.Equals(long.Parse(LeftTrack))).NewId}, {trackIds.First(r => r.OldId.Equals(long.Parse(RightTrack))).NewId}) returning coalesce(id, -1)");
                        }
                    }
                trackIds.Clear();
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "tpl_non_ext_km.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "tpl_non_ext_km.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"),
                            km = fields.ToList().IndexOf("km");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string Km = fields[km];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into tpl_non_ext_km (period_id, km) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {Km}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "tpl_nst_km.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "tpl_nst_km.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"),
                            km = fields.ToList().IndexOf("km"), len = fields.ToList().IndexOf("len");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string Km = fields[km];
                            string Len = fields[len];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into tpl_nst_km (period_id, km, len) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {Km}, {Len}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "tpl_pdb_section.csv") && periodIds.Any() && pdbIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "tpl_pdb_section.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), pdbid = fields.ToList().IndexOf("adm_pdb_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string PdbId = fields[pdbid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))) && pdbIds.Any(r => r.OldId.Equals(long.Parse(PdbId))))
                                ExportImportService.Execute($@"insert into tpl_pdb_section (period_id, start_km, start_m, final_km, final_m, adm_pdb_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {pdbIds.First(r => r.OldId.Equals(long.Parse(PdbId))).NewId}) returning coalesce(id, -1)");
                        }
                    }
                pdbIds.Clear();
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "tpl_profile_object.csv") && periodIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "tpl_profile_object.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), objid = fields.ToList().IndexOf("object_id"),
                            km = fields.ToList().IndexOf("km"), meter = fields.ToList().IndexOf("meter"), sid = fields.ToList().IndexOf("side_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string Km = fields[km];
                            string Meter = fields[meter];
                            string ObjectId = fields[objid];
                            string SideId = fields[sid];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))))
                                ExportImportService.Execute($@"insert into tpl_profile_object (period_id, object_id, side_id, km, meter) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {ObjectId}, {SideId}, {Km}, {Meter}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "tpl_station_section.csv") && periodIds.Any() && stationIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "tpl_station_section.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), stationid = fields.ToList().IndexOf("station_id"),
                            startkm = fields.ToList().IndexOf("start_km"), startm = fields.ToList().IndexOf("start_m"), finalkm = fields.ToList().IndexOf("final_km"), finalm = fields.ToList().IndexOf("final_m"),
                            axiskm = fields.ToList().IndexOf("axis_km"), axism = fields.ToList().IndexOf("axis_m"), pointid = fields.ToList().IndexOf("point_id");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string StartKm = fields[startkm];
                            string StartM = fields[startm];
                            string FinalKm = fields[finalkm];
                            string FinalM = fields[finalm];
                            string StationId = fields[stationid];
                            string PointId = fields[pointid];
                            string AxisKm = fields[axiskm];
                            string AxisM = fields[axism];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))) && stationIds.Any(r => r.OldId.Equals(long.Parse(StationId))))
                                ExportImportService.Execute($@"insert into tpl_station_section (period_id, start_km, start_m, final_km, final_m, axis_km, axis_m, point_id, station_id) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {StartKm}, {StartM}, {FinalKm}, {FinalM}, {AxisKm}, {AxisM}, {PointId}, {stationIds.First(r => r.OldId.Equals(long.Parse(StationId))).NewId}) returning coalesce(id, -1)");
                        }
                    }
                progressBar.PerformStep();

                if (fileInfo.Any(tmp => tmp.Name == "tpl_switch.csv") && periodIds.Any() && stationIds.Any())
                    using (TextFieldParser csvParser = new TextFieldParser(fileInfo.FirstOrDefault(f => f.Name == "tpl_switch.csv").FullName))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        string[] fields = csvParser.ReadFields();
                        int id = fields.ToList().IndexOf("id"), periodid = fields.ToList().IndexOf("period_id"), stationid = fields.ToList().IndexOf("station_id"),
                            km = fields.ToList().IndexOf("km"), meter = fields.ToList().IndexOf("meter"), sid = fields.ToList().IndexOf("side_id"),
                            pid = fields.ToList().IndexOf("point_id"), mid = fields.ToList().IndexOf("mark_id"), did = fields.ToList().IndexOf("dir_id"), num = fields.ToList().IndexOf("num");

                        while (!csvParser.EndOfData)
                        {
                            fields = csvParser.ReadFields();
                            string Id = fields[id];
                            string PeriodId = fields[periodid];
                            string Km = fields[km];
                            string Meter = fields[meter];
                            string StationId = fields[stationid];
                            string SideId = fields[sid];
                            string PointId = fields[pid];
                            string MarkId = fields[mid];
                            string DirId = fields[did];
                            string Num = fields[num];

                            if (periodIds.Any(r => r.OldId.Equals(long.Parse(PeriodId))) && stationIds.Any(r => r.OldId.Equals(long.Parse(StationId))))
                                ExportImportService.Execute($@"insert into tpl_switch (period_id, station_id, point_id, side_id, km, meter, mark_id, dir_id, num) values ({periodIds.First(r => r.OldId.Equals(long.Parse(PeriodId))).NewId}, {stationIds.First(r => r.OldId.Equals(long.Parse(StationId))).NewId}, {PointId}, {SideId}, {Km}, {Meter}, {MarkId}, {DirId}, '{Num}') returning coalesce(id, -1)");
                        }
                    }
                stationIds.Clear();
                periodIds.Clear();
                progressBar.PerformStep();
            }
            else
            {
                result = false;
            }

            return result;
        }
    }
}
