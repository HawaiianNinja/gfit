using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using gFit.Models;
using Newtonsoft.Json;

namespace gFit.BusinessLayer
{
    public static class AccountBusinessLayer
    {


        public static Account GetCurrentAccount()
        {
            Account currentAccount = HttpContext.Current.Session["account"] as Account;


            if (currentAccount == null)
            {

                HttpCookie accountCookie = new HttpCookie("accountCookie");
                accountCookie = HttpContext.Current.Request.Cookies["accountCookie"];

                try
                {
                    currentAccount = JsonConvert.DeserializeObject<Account>(accountCookie.Value);
                }
                catch (Exception e)
                {
                    return null;
                }
                                
            }

            return currentAccount;

        }


    }
}