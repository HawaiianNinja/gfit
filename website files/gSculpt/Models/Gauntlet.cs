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


        public int BasePoints
        {
            get
            {
                return (int)Math.Round((Reps * Difficulty)/100.0)*100;
            }
            set { }
        }

        public string DifficultyRange
        {
            get
            {
                if (BasePoints < 400)
                {
                    return "Easy";
                }
                else if (BasePoints < 700)
                {
                    return "Medium";
                }
                else
                {
                    return "Hard";
                }

            }

            set { }
        }

               


        public Gauntlet() { }



            


    }
}