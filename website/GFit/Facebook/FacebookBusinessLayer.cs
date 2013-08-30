#region

using System;
using System.Collections.Generic;
using System.Dynamic;
using Facebook;
using gFit.Models;
using Microsoft.CSharp.RuntimeBinder;
using gFit.DBLayer;

#endregion

namespace gFit.Facebook
{
    public static class FacebookBusinessLayer
    {
        private const string AppID = "296587703816145";
        private const string AppSecret = "5ddf5f4419c40cb6528160ffdaa56623";


        //private string something = OAuthWebSecurity.Get


        private static dynamic ReturnFacebookQuery(string queryFields, string accessToken)
        {
            var client = new FacebookClient(accessToken);
            dynamic result = client.Get("me", new {fields = queryFields});
            return result;
        }


        public static string GetUserFirstName(string accessToken)
        {
            var queryResult = ReturnFacebookQuery("first_name", accessToken);
            return queryResult.first_name;
        }


        public static string GetUserLastName(string accessToken)
        {
            var queryResult = ReturnFacebookQuery("last_name", accessToken);
            return queryResult.last_name;
        }

        public static string GetUserId(string accessToken)
        {
            var queryResult = ReturnFacebookQuery("id", accessToken);
            return queryResult.id;
        }


        public static DateTime GetUserBirthday(string accessToken)
        {
            var queryResult = ReturnFacebookQuery("birthday", accessToken);

            DateTime birthday = Convert.ToDateTime(queryResult.birthday);

            return birthday;
        }

        public static string GetUsername(string accessToken)
        {
            var queryResult = ReturnFacebookQuery("username", accessToken);
            return queryResult.username;
        }


        public static string GetUserGender(string accessToken)
        {
            var queryResult = ReturnFacebookQuery("gender", accessToken);

            if (queryResult.gender != "male" && queryResult.gender != "female")
            {
                queryResult.gender = "male";
            }

            return queryResult.gender;
        }


        public static List<BasicFriend> GetUserFriends(string accessToken)
        {
            var queryResult = ReturnFacebookQuery("friends", accessToken);

            var friends = new List<BasicFriend>();


            try
            {
                foreach (var friend in (JsonArray)queryResult.friends["data"])
                {
                    friends.Add(new BasicFriend
                                    {
                                        Id = (string)(((JsonObject)friend)["id"]),
                                        Name = (string)(((JsonObject)friend)["name"])
                                    });
                }
            }
            catch (RuntimeBinderException e)
            {
                LogDBLayer.Instance.AddToLog("Accesstoken: \"" + accessToken + "\" has no friends.\n" + e.StackTrace);
            }
            
            return friends;
        }


        public static string GetLongLivedAccessToken(string shortLivedToken)
        {
            dynamic result = new ExpandoObject();
            try
            {
                var api = new FacebookClient
                              {
                                  AccessToken = shortLivedToken,
                                  AppId = AppID,
                                  AppSecret = AppSecret
                              };
                dynamic parameters = new ExpandoObject();
                parameters.grant_type = "fb_exchange_token";
                parameters.fb_exchange_token = shortLivedToken;
                parameters.client_id = AppID;
                parameters.client_secret = AppSecret;
                result = api.Get("oauth/access_token", parameters);
            }
            catch (FacebookOAuthException err)
            {
                result.error = "Error";
                result.message = err.Message;
            }
            catch (Exception err)
            {
                result.error = "Error";
                result.message = err.Message;
            }
            return result.access_token as string;
        }
    }
}