using gSculpt.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gSculpt.Models.Composite
{
    public class GauntletParticipation
    {

        public Gauntlet Gauntlet { get; set; }
        public Account Account { get; set; }
        public List<Set> Sets { get; set; }


        public List<Set> SetsDesc
        {
            get
            {
                return Sets.OrderByDescending(o => o.StartTime).ToList<Set>();
            }

            set { }
        }

        public List<Set> SetsAsc
        {
            get
            {
                return Sets.OrderBy(o => o.StartTime).ToList<Set>();
            }
            set { }
        }


        public Set IncompleteSet
        {
            get
            {
                foreach (Set set in Sets)
                {
                    if (!set.Completed)
                    {

                        return set;
                    }
                }

                return null;
            }

            set { }
        }



        public int RepsLeft
        {
            get
            {
                return Gauntlet.Reps - TotalReps;
            }
            set { }
        }


        public int TotalReps
        {
            get
            {
                int sum = 0;
                foreach(Set set in Sets)
                {
                    sum += set.NumReps;
                }
                return sum;
            }
            set { }
        }



        public bool HasIncompleteSet
        {
            get
            {
                foreach (Set set in Sets)
                {
                    if (!set.Completed)
                    {
                        return true;
                    }
                }
                return false;
            }
            set { }
        }
        

        public bool IsComplete
        {
            get
            {
                return TotalReps == Gauntlet.Reps;
            }
            set { }
        }


        public bool IsValid
        {
            get
            {
                //check that there aren't too many reps
                if (TotalReps > Gauntlet.Reps)
                {
                    return false;
                }

                //check that there isn't more than onne
                //incomplete set
                int numIncompleteSets = 0;
                foreach (Set s in Sets)
                {
                    if (!s.Completed)
                        numIncompleteSets++;
                }
                if (numIncompleteSets > 1)
                {
                    return false;
                }

                //check that if GauntletParticipation is complete
                //it can't have any incomplete sets
                if (IsComplete && HasIncompleteSet)
                    return false;

                //check that each set is valid
                foreach (Set s in Sets)
                {
                    if (!s.IsValid)
                        return false;
                }


                return true;

            }
            set { }
        }


        


    }
}