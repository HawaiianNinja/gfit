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
        
                

        public string Uid { get; set; }
        public string LongAuthToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set;  }
        public string Username { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastAccessed { get; set; }




        public Account()
        {
        }

        public Account(string uid, string token)
        {

            /*
             * Old Code - should be able to be deleated once this is tested
             *  var client = new FacebookClient(LongAuthToken);
             *  dynamic result = client.Get("me", new { fields = "first_name,last_name,birthday,gender" });
             */

            Uid = uid;
            LongAuthToken = FacebookBusinessLayer.GetLongLivedAccessToken(token);

            FirstName = FacebookBusinessLayer.GetUserFirstName(LongAuthToken);
            LastName = FacebookBusinessLayer.GetUserLastName(LongAuthToken);
            DOB = FacebookBusinessLayer.GetUserBirthday(LongAuthToken);
            Gender = FacebookBusinessLayer.GetUserGender(LongAuthToken);
            Created = DateTime.UtcNow;
            LastAccessed = DateTime.UtcNow;


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
