using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gFit.Models.Composite
{
    public class GauntletStatistics
    {


        public List<GauntletParticipation> AllParticipations { get; set; }



        public GauntletStatistics() { }



        public double AverageRepsPerSet
        {
            get
            {
                double sum = 0;

                foreach (GauntletParticipation gp in AllParticipations)
                {
                    sum += gp.AverageRepsPerSet;
                }

                return sum / AllParticipations.Count;
            }
            set { }
        }




        public double AverageNumOfSets
        {
            get
            {
                double sum = 0;

                foreach (GauntletParticipation gp in AllParticipations)
                {
                    sum += gp.Sets.Count;
                }

                return sum / AllParticipations.Count;
            }
            set { }
        }


        public TimeSpan MaxTotalTime
        {
            get
            {
                return AllParticipations.Max(o => o.TotalSetTime);
            }
            set { }
        }


        public TimeSpan MinTotalTime
        {
            get
            {
                return AllParticipations.Min(o => o.TotalSetTime);
            }
            set { }
        }



        public TimeSpan AvgTotalTime
        {
            get
            {

                Int64 totalTicks = 0;

                totalTicks = AllParticipations.Sum(o => o.TotalSetTime.Ticks);

                return TimeSpan.FromTicks(totalTicks / AllParticipations.Count);

            }
            set { }
        }


    }
}