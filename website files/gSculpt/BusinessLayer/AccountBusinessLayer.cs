using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using gSculpt.Models;

namespace gSculpt.BusinessLayer
{
    public static class AccountBusinessLayer
    {
        public static Account GetUserByFbUid(string uid)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["LocalSQLExpress"].ConnectionString;
            Account user;
            using (var con = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("dbo.usp_getAccountByFacebookUid", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@FacebookUid", uid));
                con.Open();
                var rdr = cmd.ExecuteReader();
                if (!rdr.HasRows)
                {
                    return null;
                }
                rdr.Read();
                user = new Account
                           {
                               Username = rdr["username"].ToString(),
                               FirstName = rdr["firstName"].ToString(),
                               LastName = rdr["lastName"].ToString(),
                               DOB = (DateTime)rdr["dob"],
                               Gender = rdr["gender"].ToString(),
                               Uid = rdr["fbUserId"].ToString(),
                               LongTermAuthToken = rdr["fbAuthToken"].ToString(),
                               //Created = (DateTime)rdr["dateCreated"],
                               //LastAccessed = (DateTime)rdr["dateLastAccessed"]
                           };
            }
            return user;
        }

        public static bool AddAccount(Account acc)
        {


            var connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            int result;
            using (var con = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("dbo.usp_addAccount", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@username", acc.Username));
                cmd.Parameters.Add(new SqlParameter("@firstname", acc.FirstName));
                cmd.Parameters.Add(new SqlParameter("@lastname", acc.LastName));
                cmd.Parameters.Add(new SqlParameter("@dob", acc.DOB));
                cmd.Parameters.Add(new SqlParameter("@gender", acc.Gender));
                cmd.Parameters.Add(new SqlParameter("@fbUserId", acc.Uid));
                cmd.Parameters.Add(new SqlParameter("@fbAuthToken", acc.LongTermAuthToken));
                con.Open();
                result = cmd.ExecuteNonQuery();
            }
            return result == 1;
        }
    }
}