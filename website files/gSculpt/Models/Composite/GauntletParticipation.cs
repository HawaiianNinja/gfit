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



    }
}