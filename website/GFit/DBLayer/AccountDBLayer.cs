﻿#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using gFit.Models;

#endregion

namespace gFit.DBLayer
{
    public class AccountDBLayer : DBLayer
    {
        private static AccountDBLayer instance;


        private AccountDBLayer()
        {
        }

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


        public bool AddAccountToDB(Account account)
        {
            var sqlParameters = new List<SqlParameter>();

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

            return ExecuteNonQuery("dbo.usp_addAccount", sqlParameters) == 1;
        }


        public Account GetAccountFromDB(string uid)
        {
            var sqlParameters = new List<SqlParameter>();

            AddSqlParameter(sqlParameters, "@Uid", uid);

            var dt = GetDataTableFromStoredProcedure("usp_getAccountByOAuthUid", sqlParameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            var a = GetAccountsFromDataTable(dt)[0];

            return a;
        }


        public List<Account> GetAccountsThatCompletedGauntlet(int gauntlet_id)
        {
            var sqlParameters = new List<SqlParameter>();

            AddSqlParameter(sqlParameters, "@gauntlet_id", gauntlet_id);

            var dt = GetDataTableFromStoredProcedure("usp_getAccountsThatCompletedGauntlet", sqlParameters);

            return GetAccountsFromDataTable(dt);

        }





        private List<Account> GetAccountsFromDataTable(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return new List<Account>();
            }


            var list = new List<Account>();

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var a = new Account();

                a.Id = (int) dt.Rows[i]["account_id"];
                a.Username = (string) dt.Rows[i]["username"];

                a.Password = (string) GetColValue(dt.Rows[i], "password");
                a.FirstName = (string) GetColValue(dt.Rows[i], "firstName");
                a.LastName = (string) GetColValue(dt.Rows[i], "lastName");
                a.DOB = Convert.ToDateTime(GetColValue(dt.Rows[i], "dob"));
                a.Gender = (string) GetColValue(dt.Rows[i], "gender");
                a.Provider = (string) GetColValue(dt.Rows[i], "provider_name");
                a.LongTermAuthToken = (string) GetColValue(dt.Rows[i], "authToken");
                a.Uid = (string) GetColValue(dt.Rows[i], "Uid");


                list.Add(a);
            }

            return list;
        }
    }
}