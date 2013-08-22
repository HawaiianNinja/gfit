#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

#endregion

namespace gFit.DBLayer
{
    public abstract class DBLayer
    {
        public static String ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        private SqlCommand GetSqlCommandForStoredProcedure(String storedProcedureName, List<SqlParameter> sqlParameters,
                                                           SqlConnection connection)
        {
            var cmd = new SqlCommand(storedProcedureName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (var p in sqlParameters)
            {
                cmd.Parameters.Add(p);
            }

            return cmd;
        }

        public DataTable GetDataTableFromStoredProcedure(String storedProcedureName, List<SqlParameter> sqlParameters)
        {
            SqlConnection connection;
            var dt = new DataTable();
            using (connection = new SqlConnection(ConnectionString))
            {
                var cmd = GetSqlCommandForStoredProcedure(storedProcedureName, sqlParameters, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                connection.Open();
                var reader = cmd.ExecuteReader();
                dt.Load(reader);
                connection.Close();
            }
            return dt;
        }

        public int ExecuteNonQuery(String storedProcedureName, List<SqlParameter> sqlParameters)
        {
            SqlConnection connection;
            var result = 0;
            using (connection = new SqlConnection(ConnectionString))
            {
                var cmd = GetSqlCommandForStoredProcedure(storedProcedureName, sqlParameters, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                connection.Open();
                result = cmd.ExecuteNonQuery();
                connection.Close();
            }
            return result;
        }

        public void AddSqlParameter(List<SqlParameter> list, string paramName, dynamic value)
        {
            list.Add(new SqlParameter(paramName, value));
        }

        public Object GetColValue(DataRow row, string colName)
        {
            if (row.IsNull(colName))
            {
                return null;
            }
            return row[colName];
        }
    }
}