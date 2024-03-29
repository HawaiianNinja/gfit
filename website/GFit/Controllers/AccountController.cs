﻿#region

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using gFit.DBLayer;
using gFit.Facebook;
using gFit.Filters;
using gFit.Models;

#endregion

namespace gFit.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        /*
         * Notifiaction Strings
         */

        private const string notification_loggedOut = "You have been logged out of gFit.";
        private const string notification_failedLogin = "You must allow gFit access on Facebook.";


        //
        // GET: /Account/Login
        [AllowAnonymous]
        [ActionName("Login")]
        public ActionResult Login()
        {
            var notification = TempData["notification"] as string;

            ViewBag.notification = notification == null ? "" : notification;

            return View();
        }


        //
        // POST: /Account/logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            TempData["notification"] = notification_loggedOut;

            Session.Clear();

            return RedirectToAction("Login");
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

            var loginSucceeded = result.IsSuccessful;


            if (!loginSucceeded)
            {
                TempData["notification"] = notification_failedLogin;

                return RedirectToAction("Login");
            }


            var provider = result.Provider;
            var facebookUid = result.ProviderUserId;
            var username = result.UserName;
            var accessToken = result.ExtraData["access_token"];

            var longLivedToken = FacebookBusinessLayer.GetLongLivedAccessToken(accessToken);


            var account = AccountDBLayer.Instance.GetAccountFromDB(facebookUid);

            if (account == null)
            {
                account = new Account(facebookUid, accessToken);
                account.Provider = provider;
                account.PullDataFromFacebook();

                if (account.Username == null)
                {
                    account.Username = account.Uid;
                }

                AccountDBLayer.Instance.AddAccountToDB(account);

                account = AccountDBLayer.Instance.GetAccountFromDB(facebookUid);

            }


            //log them in with FormsAuthentication 
            FormsAuthentication.SetAuthCookie(facebookUid, true);
            Session["account"] = account;

            var s = new JavaScriptSerializer();

            var accountCookie = new HttpCookie("accountCookie");
            accountCookie.Value = account.GetAsJSON();
            accountCookie.Expires = DateTime.Now.AddMinutes(3600);

            Response.Cookies.Add(accountCookie);


            return RedirectToAction("Index", "Home");
        }

        #region Helpers

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

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

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return
                        "A user name for that e-mail address already exists. Please enter a different e-mail address.";

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
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
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

        #endregion
    }
}