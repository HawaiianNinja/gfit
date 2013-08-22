using DotNetOpenAuth.AspNet;
using gFit.DBLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;



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


        private string RedirectUri = "";



        public string ProviderName
        {
            get { return "FACEBOOK"; }
        }




        private const string baseUrl = "https://www.facebook.com/dialog/oauth?client_id=";
        public const string graphApiToken = "https://graph.facebook.com/oauth/access_token?";
        public const string graphApiMe = "https://graph.facebook.com/me?";

        public List<string> Scope = new List<string>();




        /*
         * 
         * 
         * CONSTRUCTORs
         * 
         * 
         */

        public FacebookScopedClient()
        {
        }






        /*
         * 
         * 
         * Interface Methods
         * 
         * 
         */

        public void RequestAuthentication(System.Web.HttpContextBase context, Uri returnUrl)
        {

            Scope.Add("user_birthday");


            RedirectUri = HttpUtility.UrlEncode(returnUrl.ToString());

            string url = baseUrl + AppID + "&redirect_uri=" + RedirectUri + "&" + BuildScopeString();


            context.Response.Redirect(url);
        }


        public AuthenticationResult VerifyAuthentication(System.Web.HttpContextBase context)
        {
           
            

            //string rawUrl = context.Request.Url.OriginalString;
            ////From this we need to remove code portion
            //rawUrl = Regex.Replace(rawUrl, "&code=[^&]*", "");
            //rawUrl = Regex.Replace(rawUrl, ":80", "");
            //RedirectUri = rawUrl;


            //IDictionary<string, string> userData = GetUserData(code, rawUrl);


            string code = context.Request.QueryString["code"];
            IDictionary<string, string> userData = GetUserData(code);

            
            if (userData == null || userData.Count == 0)
            {
                LogDBLayer.Instance.AddToLog("Redirect Uri: " + RedirectUri);
                return new AuthenticationResult(false, ProviderName, null, null, userData);
            }

            string id = userData["id"];
            string username = userData["username"];
            userData.Remove("id");
            userData.Remove("username");

            AuthenticationResult result = new AuthenticationResult(true, ProviderName, id, username, userData);
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

            string s = "";

            if(Scope.Count != 0)
            {
                s = "scope=";
             

                foreach (string i in Scope)
                {
                    s += i +",";
                }
            }
            

            return s.Remove(s.Length-1, 1);
        }


        private IDictionary<string, string> GetUserData(string accessCode)
        {



            string tokenUrl = graphApiToken + "client_id=" + AppID + "&redirect_uri=" + RedirectUri + "&client_secret=" + AppSecret + "&code=" + accessCode;

            LogDBLayer.Instance.AddToLog("tokenUrl: " + tokenUrl);


            string token = GetHTML(tokenUrl);

            Dictionary<string, string> userData = new Dictionary<string,string>();


            if (token == null || token == "")
            {
                LogDBLayer.Instance.AddToLog("Redirect URI from GetUserData: " + RedirectUri);                
                LogDBLayer.Instance.AddToLog("tokenUrl: " + tokenUrl);                
                return userData;

            }


            string fields = "id,username";
            string accessToken = token.Substring("access_token=", "&");
            string dataUrl = graphApiMe + "fields=" + fields + "&access_token=" + token.Substring("access_token=", "&");

            string data = GetHTML(dataUrl);

            // this dictionary must contains
            userData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            userData.Add("access_token", accessToken);

            return userData;
        }



        

        

        


        private static string GetHTML(string URL)
        {
            string connectionString = URL;

            try
            {
                System.Net.HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(connectionString);
                myRequest.Credentials = CredentialCache.DefaultCredentials;
                //// Get the response
                WebResponse webResponse = myRequest.GetResponse();
                Stream respStream = webResponse.GetResponseStream();
                ////
                StreamReader ioStream = new StreamReader(respStream);
                string pageContent = ioStream.ReadToEnd();
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
                int iStart = str.IndexOf(StartString) + StartString.Length;
                int iEnd = str.IndexOf(EndString, iStart);
                return str.Substring(iStart, (iEnd - iStart));
            }
            return null;
        }
    }

}