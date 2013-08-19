using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gSculpt.Models.Base
{
  
    
    public class Set
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int GauntletId { get; set; }
        public int NumReps { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Completed { get; set; }

    }

}