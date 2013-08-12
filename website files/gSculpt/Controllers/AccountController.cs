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
using gSculpt.Business_Layer;
using gSculpt.Filters;
using gSculpt.Models;

namespace gSculpt.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string facebook_accessToken, string facebook_uid)
        {
            var account = AccountBusinessLayer.GetUserByFbUid(facebook_uid);
            if (account == null)
            {
                account = new Account(facebook_uid, facebook_accessToken);
                // goto fix up page
                AccountBusinessLayer.AddAccount(account);
            }
            
            // check database here
            FormsAuthentication.SetAuthCookie(account.Uid, true);
            return View();
        }

        //
        // POST: /Account/LogOff
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }



        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {

            
            return View();
        }





        //
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
                return View("Error");
            }

            if(!result.ExtraData.Keys.Contains("accesstoken"))
            {
                return View ("No access token");
            }


            
            //give them a unique UserID (facebook/userID)
            //we can leave this like that
            var provider = result.Provider;
            var uniqueUserID = result.ProviderUserId;
            var uniqueID = provider + "/" + uniqueUserID;
            var accessToken = result.ExtraData["accesstoken"];


            var account = AccountBusinessLayer.GetUserByFbUid(result.ProviderUserId);
            if (account == null)
            {
                account = new Account(uniqueUserID, accessToken);
                // goto fix up page
                AccountBusinessLayer.AddAccount(account);
            }

            // check database here
            FormsAuthentication.SetAuthCookie(account.Uid, true);
            return View();




            //log them in with FormsAuthentication 
            FormsAuthentication.SetAuthCookie(uniqueID, false);






            return View();



        }






        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
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

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
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