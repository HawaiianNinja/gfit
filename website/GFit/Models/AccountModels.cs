#region

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Script.Serialization;
using gFit.Facebook;

#endregion

namespace gFit.Models
{
    public class Account
    {
        /*
         * Properties included in DB record
         */


        //properties of account
        public Account()
        {
        }


        public Account(string uid, string shortTermAccessToken)
        {
            Uid = uid;
            LongTermAuthToken = FacebookBusinessLayer.GetLongLivedAccessToken(shortTermAccessToken);
            LongTermAuthToken = shortTermAccessToken;
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }


        //properties of provider
        public string Provider { get; set; }
        public string LongTermAuthToken { get; set; }
        public string Uid { get; set; }


        /*
         * Properties not included in DB record
         */
        public List<BasicFriend> Friends { get; private set; }


        /*
         * -----------------------------------------------------
         * Constructors
         * -----------------------------------------------------
         */


        /*
         * -----------------------------------------------------
         * Methods
         * -----------------------------------------------------
         */


        //Using the stored LongAuthToken, this will attempt to pull data from facebook about the user
        public bool PullDataFromFacebook()
        {
            if (LongTermAuthToken == null)
            {
                throw new NullReferenceException("Must have a long access token to pull data from facebook");
            }

            Username = FacebookBusinessLayer.GetUsername(LongTermAuthToken);
            FirstName = FacebookBusinessLayer.GetUserFirstName(LongTermAuthToken);
            LastName = FacebookBusinessLayer.GetUserLastName(LongTermAuthToken);
            DOB = FacebookBusinessLayer.GetUserBirthday(LongTermAuthToken);
            Gender = FacebookBusinessLayer.GetUserGender(LongTermAuthToken);
            Friends = FacebookBusinessLayer.GetUserFriends(LongTermAuthToken);


            return true; //TODO: we need to error check that we were able to get data from facebook
        }


        public bool IsValid()
        {
            //for now ensure that DOB is set to something meaningful,
            //in the future this may need to be updated (checking LongAuthToken is valid, etc)
            return (DOB != DateTime.MinValue);
        }


        public string GetAsJSON()
        {
            var jss = new JavaScriptSerializer();

            return jss.Serialize(this);
        }
    }

    public class BasicFriend
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }


    public class AccountContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
    }
}