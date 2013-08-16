﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Facebook;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using gSculpt.BusinessLayer;
using gSculpt.Filters;
using gSculpt.Models;
using gSculpt.Facebook;
using gSculpt.DBLayer;

namespace gSculpt.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {


        /*
         * Notifiaction String
         */

        private const string notification_loggedOut = "You have been logged out of gFit.";
        private const string notification_failedLogin = "You must allow gFit access on Facebook.";







        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {

            string notification = TempData["notification"] as string;

            ViewBag.notification = notification == null ? "" : notification;

            return View();
        }


        //
        // POST: /Account/LogOff
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            TempData["notification"] = notification_loggedOut;

            string urlAction = Url.Action("Login");

            return Login();
        }

        

        // after this is over, it will send them to AuthenticationCallback
        // GET: /Account/OAuthLogin
        [AllowAnonymous]
        public void LoginWithProvider(string provider = "Facebook")
        {
            //Set this to the action of our callback method
            
            OAuthWebSecurity.RequestAuthentication(provider, Url.Action("AuthenticationCallback"));

        }


        [AllowAnonymous]
        public ActionResult AuthenticationCallback()
        {

            var result = OAuthWebSecurity.VerifyAuthentication();

            bool loginSucceeded = result.IsSuccessful;
            
            /*
             * Do some login validation
             */

            if (!loginSucceeded)
            {
                TempData["notification"] = notification_failedLogin;
                RedirectToAction("Login");
            }

                        
            //give them a unique UserID (facebook/userID)
            //we can leave this like that
            var provider = result.Provider;
            var facebookUid = result.ProviderUserId;
            var username = result.UserName;

            var accessToken = result.ExtraData["access_token"];
            

            //get a long lived access token
            var longLivedToken = FacebookBusinessLayer.GetLongLivedAccessToken(accessToken);



            var account = AccountDBLayer.Instance.GetAccountFromDB(facebookUid);

            if (account == null)
            {
                account = new Account(facebookUid, accessToken);
                account.Provider = provider;
                account.PullDataFromFacebook();
                AccountDBLayer.Instance.AddAccountToDB(account);
            }



            //log them in with FormsAuthentication 
            FormsAuthentication.SetAuthCookie(facebookUid, false);
            Session["account"] = account;


            RedirectToAction("Home");

            return View();

        }




        //
        // POST: /Account/Disassociate
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider = "Facebook", string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }
        */



        //
        // GET: /Account/Settings
        // We can use this to manage account settings 
        [HttpGet]
        [Authorize]
        public ActionResult Settings()
        {


            return View();

        }



        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
