using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using gSculpt.Models;
using System.Dynamic;

namespace gSculpt.Business_Layer
{
    public static class FacebookBusinessLayer
    {

        private const string AppID = "296587703816145";
        private const string AppSecret = "5ddf5f4419c40cb6528160ffdaa56623";



        private static dynamic ReturnFacebookQuery(string queryFields, string accessToken)
        {
            var client = new FacebookClient(accessToken);
            dynamic result = client.Get("me", new { fields = queryFields });
            return result;
        }


        public static string GetUserFirstName(string accessToken)
        {

            dynamic queryResult = ReturnFacebookQuery("first_name", accessToken);
            return queryResult.first_name;

        }


        public static string GetUserLastName(string accessToken)
        {

            dynamic queryResult = ReturnFacebookQuery("last_name", accessToken);
            return queryResult.first_name;

        }

        public static string GetUserId(string accessToken)
        {

            dynamic queryResult = ReturnFacebookQuery("id", accessToken);
            return queryResult.id;

        }


        
        public static DateTime GetUserBirthday(string accessToken)
        {            
            dynamic queryResult = ReturnFacebookQuery("birthday", accessToken);

            DateTime birthday = Convert.ToDateTime(queryResult.birthday);

            return birthday;
        }

        public static string GetUserGender(string accessToken)
        {
            dynamic queryResult = ReturnFacebookQuery("gender", accessToken);

            if (queryResult.gender != "male" && queryResult.gender != "female")
            {
                queryResult.gender = "male";
            }

            return queryResult.gender;

        }


        public static List<BasicFriend> GetUserFriends(string accessToken)
        {
            dynamic queryResult = ReturnFacebookQuery("friends", accessToken);
            
            var friends = new List<BasicFriend>();
            
            foreach (var friend in (JsonArray)queryResult.friends["data"])
                friends.Add(new BasicFriend
                {
                    Id = (string)(((JsonObject)friend)["id"]),
                    Name = (string)(((JsonObject)friend)["name"])
                });
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