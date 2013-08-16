using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gSculpt.Models
{
    public class Gauntlet
    {

        public int Id { get; set; }
        public string Excercise { get; set; }
        public int Reps { get; set; }
        public int Difficulty { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateAssigned { get; set; }


        public Gauntlet() { }






    }
}