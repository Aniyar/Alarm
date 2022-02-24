using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ALARm.Core;
using Dapper;
using Npgsql;

namespace ALARm.DataAccess
{
    public class ExportImportRepository : IExportImportRepository
    {
        public string ExportQueryReturnString(string text)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<string>(text, commandType: CommandType.Text).FirstOrDefault();
            }
        }

        public bool Execute(string text)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                try
                {
                    db.Execute(text, commandType: CommandType.Text);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public long ImportQueryReturnLong(string text)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<long>(text, commandType: CommandType.Text).FirstOrDefault();
            }
        }

        public List<long> ImportQueryReturnListLong(string text)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                return db.Query<long>(text, commandType: CommandType.Text).ToList();
            }
        }
    }
}
