using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace gSculpt.DBLayer
{
    public abstract class DBLayer
    {



        public static String ConnectionString;
        public static SqlConnection Connection;

        public DBLayer()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["LocalSQLExpress"].ConnectionString;
            Connection = new SqlConnection(ConnectionString);
        }



        private static SqlCommand GetSqlCommandForStoredProcedure(String storedProcedureName, List<SqlParameter> sqlParameters)
        {
            
            var cmd = new SqlCommand(storedProcedureName, Connection);
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter p in sqlParameters)
            {
                cmd.Parameters.Add(p);
            }

            return cmd;

        }




        public static DataTable GetDataTableFromStoredProcedure(String storedProcedureName, List<SqlParameter> sqlParameters)
        {

            Connection.Open();

            SqlCommand cmd = GetSqlCommandForStoredProcedure(storedProcedureName, sqlParameters);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);


            Connection.Close();

            return new DataTable();

        }


        public static bool ExecuteNonQuery(String storedProcedureName, List<SqlParameter> sqlParameters)
        {

            Connection.Open();

            SqlCommand cmd = GetSqlCommandForStoredProcedure(storedProcedureName, sqlParameters);
            cmd.CommandType = CommandType.StoredProcedure;

            int result = cmd.ExecuteNonQuery();



            Connection.Close();

            return result != 0;


        }


        public static void AddSqlParameter(List<SqlParameter> list, string paramName, dynamic value)
        {
            list.Add(new SqlParameter(paramName, value));
        }



    }
}