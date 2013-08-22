#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using DotNetOpenAuth.AspNet;
using Newtonsoft.Json;
using gFit.DBLayer;

#endregion

namespace gFit.Facebook
{
    public class FacebookScopedClient : IAuthenticationClient
    {
        /*
         * 
         * 
         * Properties
         * 
         * 
         */

        private const string AppID = "296587703816145";
        private const string AppSecret = "5ddf5f4419c40cb6528160ffdaa56623";


        private const string baseUrl = "https://www.facebook.com/dialog/oauth?client_id=";
        public const string graphApiToken = "https://graph.facebook.com/oauth/access_token?";
        public const string graphApiMe = "https://graph.facebook.com/me?";
        private string RedirectUri = "";

        public List<string> Scope = new List<string>();

        public string ProviderName
        {
            get { return "FACEBOOK"; }
        }


        /*
         * 
         * 
         * CONSTRUCTORs
         * 
         * 
         */


        /*
         * 
         * 
         * Interface Methods
         * 
         * 
         */

        public void RequestAuthentication(HttpContextBase context, Uri returnUrl)
        {
            Scope.Add("user_birthday");


            RedirectUri = HttpUtility.UrlEncode(returnUrl.ToString());

            var url = baseUrl + AppID + "&redirect_uri=" + RedirectUri + "&" + BuildScopeString();


            context.Response.Redirect(url);
        }


        public AuthenticationResult VerifyAuthentication(HttpContextBase context)
        {
            //string rawUrl = context.Request.Url.OriginalString;
            ////From this we need to remove code portion
            //rawUrl = Regex.Replace(rawUrl, "&code=[^&]*", "");
            //rawUrl = Regex.Replace(rawUrl, ":80", "");
            //RedirectUri = rawUrl;


            //IDictionary<string, string> userData = GetUserData(code, rawUrl);


            var code = context.Request.QueryString["code"];
            var userData = GetUserData(code);


            if (userData == null || userData.Count == 0)
            {
                LogDBLayer.Instance.AddToLog("Redirect Uri: " + RedirectUri);
                return new AuthenticationResult(false, ProviderName, null, null, userData);
            }

            var id = userData["id"];
            var username = userData["username"];
            userData.Remove("id");
            userData.Remove("username");

            var result = new AuthenticationResult(true, ProviderName, id, username, userData);
            return result;
        }


        /*
         * 
         * 
         * Methods
         * 
         * 
         */

        private string BuildScopeString()
        {
            var s = "";

            if (Scope.Count != 0)
            {
                s = "scope=";


                foreach (var i in Scope)
                {
                    s += i + ",";
                }
            }


            return s.Remove(s.Length - 1, 1);
        }


        private IDictionary<string, string> GetUserData(string accessCode)
        {
            var tokenUrl = graphApiToken + "client_id=" + AppID + "&redirect_uri=" + RedirectUri + "&client_secret=" +
                           AppSecret + "&code=" + accessCode;

            LogDBLayer.Instance.AddToLog("tokenUrl: " + tokenUrl);


            var token = GetHTML(tokenUrl);

            var userData = new Dictionary<string, string>();


            if (token == null || token == "")
            {
                LogDBLayer.Instance.AddToLog("Redirect URI from GetUserData: " + RedirectUri);
                LogDBLayer.Instance.AddToLog("tokenUrl: " + tokenUrl);
                return userData;
            }


            var fields = "id,username";
            var accessToken = token.Substring("access_token=", "&");
            var dataUrl = graphApiMe + "fields=" + fields + "&access_token=" + token.Substring("access_token=", "&");

            var data = GetHTML(dataUrl);

            // this dictionary must contains
            userData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            userData.Add("access_token", accessToken);

            return userData;
        }


        private static string GetHTML(string URL)
        {
            var connectionString = URL;

            try
            {
                var myRequest = (HttpWebRequest) WebRequest.Create(connectionString);
                myRequest.Credentials = CredentialCache.DefaultCredentials;
                //// Get the response
                var webResponse = myRequest.GetResponse();
                var respStream = webResponse.GetResponseStream();
                ////
                var ioStream = new StreamReader(respStream);
                var pageContent = ioStream.ReadToEnd();
                //// Close streams
                ioStream.Close();
                respStream.Close();
                return pageContent;
            }
            catch (Exception)
            {
            }
            return null;
        }
    }


    public static class String
    {
        public static string Substring(this string str, string StartString, string EndString)
        {
            if (str.Contains(StartString))
            {
                var iStart = str.IndexOf(StartString) + StartString.Length;
                var iEnd = str.IndexOf(EndString, iStart);
                return str.Substring(iStart, (iEnd - iStart));
            }
            return null;
        }
    }
}