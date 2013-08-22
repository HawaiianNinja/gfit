#region

using System;
using System.Collections.Generic;
using System.Linq;
using gFit.Models.Base;

#endregion

namespace gFit.Models.Composite
{
    public class GauntletParticipation
    {
        public Gauntlet Gauntlet { get; set; }
        public Account Account { get; set; }
        public List<Set> Sets { get; set; }


        public bool IsStarted
        {
            get { return Sets.Count != 0; }
            set { }
        }

        public bool InProgress
        {
            get { return IsStarted && !IsComplete; }
            set { }
        }

        public bool IsComplete
        {
            get { return TotalRepsCompleted == Gauntlet.Reps; }
            set { }
        }


        public List<Set> SetsDesc
        {
            get { return Sets.OrderByDescending(o => o.StartTime).ToList(); }

            set { }
        }

        public List<Set> SetsAsc
        {
            get { return Sets.OrderBy(o => o.StartTime).ToList(); }
            set { }
        }


        public Set IncompleteSet
        {
            get
            {
                foreach (var set in Sets)
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
            get { return Gauntlet.Reps - TotalRepsCompleted; }
            set { }
        }


        public int TotalRepsCompleted
        {
            get
            {
                var sum = 0;
                foreach (var set in Sets)
                {
                    sum += set.NumReps;
                }
                return sum;
            }
            set { }
        }


        public int TotalSetsCompleted
        {
            get
            {
                var sum = 0;
                foreach (var s in Sets)
                {
                    if (s.Completed)
                        sum++;
                }
                return sum;
            }
            set { }
        }


        public bool HasIncompleteSet
        {
            get
            {
                foreach (var set in Sets)
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


        public bool IsValid
        {
            get
            {
                //check that there aren't too many reps
                if (TotalRepsCompleted > Gauntlet.Reps)
                {
                    return false;
                }

                //check that there isn't more than onne
                //incomplete set
                var numIncompleteSets = 0;
                foreach (var s in Sets)
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
                foreach (var s in Sets)
                {
                    if (!s.IsValid)
                        return false;
                }


                return true;
            }
            set { }
        }


        /*
         * 
         * STATISTIC CLASSES
         * 
         */

        public double AverageRepsPerSet
        {
            get
            {
                if (Sets.Count == 0)
                    return 0;

                return Math.Round((TotalRepsCompleted/Sets.Count)/10.0)*10;
            }
            set { }
        }

        public TimeSpan TotalSetTime
        {
            get
            {
                var totalTime = new TimeSpan();

                foreach (var s in Sets)
                {
                    if (s.Completed)
                    {
                        totalTime.Add(s.EndTime.Subtract(s.StartTime));
                    }
                }
                return totalTime;
            }
            set { }
        }

        public TimeSpan AverageTimePerSet
        {
            get
            {
                if (Sets.Count == 0)
                    return TimeSpan.FromTicks(0);

                return TimeSpan.FromTicks(TotalSetTime.Ticks/Sets.Count);
            }
            set { }
        }
    }
}