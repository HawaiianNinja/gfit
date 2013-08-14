using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using gSculpt.Models;
using gSculpt.Facebook;

namespace gSculpt
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            //OAuthWebSecurity.RegisterFacebookClient(
            //    appId: "296587703816145",
            //    appSecret: "5ddf5f4419c40cb6528160ffdaa56623");

            OAuthWebSecurity.RegisterClient(new FacebookScopedClient(), "Facebook", null);
            
            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
