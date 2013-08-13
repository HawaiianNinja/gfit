using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Dynamic;
using Facebook;
using gSculpt.Business_Layer;

namespace gSculpt.Models
{
    [Table("ACCOUNTS")]
    public class Account
    {
        
        /*
         * Properties included in DB record
         */
        public string Uid { get; set; }
        public string LongAuthToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set;  }
        public string Username { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }

        

        /*
         * Properties not included in DB record
         */
        public List<BasicFriend> Friends { get; private set; }



        /*
         * -----------------------------------------------------
         * Constructors
         * -----------------------------------------------------
         */

        public Account()
        {
        }


        public Account(string uid, string shortTermAccessToken)
        {

            Uid = uid;
            LongAuthToken = FacebookBusinessLayer.GetLongLivedAccessToken(shortTermAccessToken);
            LongAuthToken = shortTermAccessToken;

        }





        /*
         * -----------------------------------------------------
         * Methods
         * -----------------------------------------------------
         */

       
        //Using the stored LongAuthToken, this will attempt to pull data from facebook about the user
        public bool PullDataFromFacebook()
        {

            if (LongAuthToken == null)
            {
                throw new NullReferenceException("Must have a long access token to pull data from facebook");
            }

            FirstName = FacebookBusinessLayer.GetUserFirstName(LongAuthToken);
            LastName = FacebookBusinessLayer.GetUserLastName(LongAuthToken);
            DOB = FacebookBusinessLayer.GetUserBirthday(LongAuthToken);
            Gender = FacebookBusinessLayer.GetUserGender(LongAuthToken);
            Friends = FacebookBusinessLayer.GetUserFriends(LongAuthToken);


            return true; //TODO: we need to error check that we were able to get data from facebook
        }



        //
        // if 
        public void GenerateRandomUsername()
        {
            Random r = new Random();
            Username = LastName + Convert.ToInt32(r.Next(1, 10000));
        }



        public bool IsValid()
        {

            //for now ensure that DOB is set to something meaningful,
            //in the future this may need to be updated (checking LongAuthToken is valid, etc)
            return (DOB != DateTime.MinValue);
            
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
