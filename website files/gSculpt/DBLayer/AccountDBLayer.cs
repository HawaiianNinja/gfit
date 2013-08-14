using gSculpt.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace gSculpt.DBLayer
{
    public class AccountDBLayer : DBLayer
    {


        private static AccountDBLayer instance;

        public static AccountDBLayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountDBLayer();
                }
                return instance;
            }
            private set { }
        }




        private AccountDBLayer():base()
        {
        }


        public bool AddAccountToDB(Account account)
        {           
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            //AddSqlParameter(sqlParameters, "@oAuth", 1);
            AddSqlParameter(sqlParameters, "@username", account.Username);
            AddSqlParameter(sqlParameters, "@password", account.Password);
            AddSqlParameter(sqlParameters, "@firstname", account.FirstName);
            AddSqlParameter(sqlParameters, "@lastname", account.LastName);
            AddSqlParameter(sqlParameters, "@dob", account.DOB);
            AddSqlParameter(sqlParameters, "@gender", account.Gender);
            AddSqlParameter(sqlParameters, "@provider_name", account.Provider);
            AddSqlParameter(sqlParameters, "@authToken", account.LongTermAuthToken);
            AddSqlParameter(sqlParameters, "@uid", account.Uid);

            return ExecuteNonQuery("dbo.usp_addAccount", sqlParameters);

        }




        public Account GetAccountFromDB(string uid)
        {

            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            AddSqlParameter(sqlParameters, "@uid", uid);

            DataTable dt = GetDataTableFromStoredProcedure("usp_getAccountByOAuthUid", sqlParameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            Account a = GetAccountsFromDataTable(dt)[0];

            return a;

        }



        private List<Account> GetAccountsFromDataTable(DataTable dt)
        {

            if(dt.Rows.Count == null)
            {
                return null;
            }


            List<Account> list = new List<Account>();

            for(int i = 0; i < dt.Rows.Count; i++)
            {

                Account a = new Account();

                a.AccountId = (int) dt.Rows[i]["account_id"];
                a.Username = (string) dt.Rows[i]["username"];
                a.Password = (string) dt.Rows[i]["password"];
                a.FirstName = (string)dt.Rows[i]["firstName"];
                a.LastName = (string)dt.Rows[i]["lastName"];
                a.DOB = Convert.ToDateTime(dt.Rows[i]["dob"]);
                a.Gender = (string)dt.Rows[i]["gender"];
                a.Provider = (string)dt.Rows[i]["provider_name"];
                a.LongTermAuthToken = (string)dt.Rows[i]["authToken"];
                a.Uid = (string)dt.Rows[i]["uid"];

                list.Add(a);

            }

            return list;

        }

        

    }
}