using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALARm.Core;
using Dapper;
using Npgsql;

namespace ALARm.DataAccess
{
    public class VideoObjectsReposirory : IVideoObjectsRepository
    {
        public List<VideoObject> GetList(long tripId, int km, VideoObjectType type)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string sqlText =
                    "select id, km, ((pt-1) * 100 + mtr) as mtr, oid as objectType from public.rd_video_objects where oid = " +
                    (int) type + " and km = " + km + " and trip_id = " + tripId;
                return db.Query<VideoObject>(sqlText, commandType: CommandType.Text).ToList();

            }
        }
    }
}
