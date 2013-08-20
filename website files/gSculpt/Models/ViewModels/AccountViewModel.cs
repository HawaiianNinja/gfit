using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using gFit.Models;

namespace gFit.Models.ViewModels
{
    public class AccountViewModel
    {

        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }


        public AccountViewModel()
        {

        }

        public AccountViewModel(Account a)
        {
            Username = a.Username;
            FirstName = a.FirstName;
            LastName = a.LastName;
            Gender = a.Gender;
            DOB = a.DOB;
        }



    }
}