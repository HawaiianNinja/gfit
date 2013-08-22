using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gFit.Models.Base
{
  
    
    public class Set
    {
        public Guid Guid { get; set; }
        public int AccountId { get; set; }
        public int GauntletId { get; set; }
        public int NumReps { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Completed { get; set; }



        public TimeSpan TotalTime
        {
            get
            {
                return EndTime.Subtract(StartTime);
            }
            set {}
        }
        

        public double RepsPerMinute
        {
            get
            {


                if (TotalTime.TotalSeconds == 0)
                {
                    return 0;
                }

                if (IsValid)
                {
                    return Math.Ceiling(NumReps * (60 / TotalTime.TotalSeconds));
                }
                return 0.0;
            }
            set { }
        }



        public bool IsValid
        {
            get
            {
                //if its completed, end time must be set
                if (Completed && EndTime == DateTime.MinValue)
                    return false;

                //end time must be later than start time
                //(unless Completed is false, then EndTime is MinValue
                if (Completed && EndTime <= StartTime)
                    return false;

                //if the set is complete, is has to have some reps
                if (Completed && NumReps == 0)
                    return false;

                return true;
            }
            set { }
        }




    }

}