#region

using System;
using System.Web;
using Newtonsoft.Json;
using gFit.Models;

#endregion

namespace gFit.BusinessLayer
{
    public static class AccountBusinessLayer
    {
        public static Account GetCurrentAccount()
        {
            var currentAccount = HttpContext.Current.Session["account"] as Account;


            if (currentAccount == null)
            {
                var accountCookie = new HttpCookie("accountCookie");
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