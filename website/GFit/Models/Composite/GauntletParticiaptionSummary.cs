using gFit.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gFit.Models.Composite
{
    public class GauntletParticiaptionSummary
    {

        public Account Account { get; set; }
        public Gauntlet Gauntlet { get; set; }

        public TimeSpan TotalTime { get; set; }
        public TimeSpan AverageTimePerSet { get; set; }
        public TimeSpan AverageTimePerRep { get; set; }

        public double AverageRepsPerSet { get; set; }
        



    }
}