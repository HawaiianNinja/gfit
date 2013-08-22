using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace gFit.DBLayer
{
    public abstract class DBLayer
    {

        public static String ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;        

        public DBLayer()
        {
        }

        private SqlCommand GetSqlCommandForStoredProcedure(String storedProcedureName, List<SqlParameter> sqlParameters, SqlConnection connection)
        {

            var cmd = new SqlCommand(storedProcedureName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter p in sqlParameters)
            {
                cmd.Parameters.Add(p);
            }

            return cmd;
        }

        public DataTable GetDataTableFromStoredProcedure(String storedProcedureName, List<SqlParameter> sqlParameters)
        {
            SqlConnection connection;
            DataTable dt = new DataTable();
            using (connection = new SqlConnection(ConnectionString))
            {
                
                SqlCommand cmd = GetSqlCommandForStoredProcedure(storedProcedureName, sqlParameters, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                connection.Close();
            }
            return dt;
        }

        public int ExecuteNonQuery(String storedProcedureName, List<SqlParameter> sqlParameters)
        {
            SqlConnection connection;
            int result = 0;
            using (connection = new SqlConnection(ConnectionString))
            {                

                SqlCommand cmd = GetSqlCommandForStoredProcedure(storedProcedureName, sqlParameters, connection);
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