﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gFit.BusinessLayer
{
    public static class MathExtensions
    {



        public static double StdDev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }




    }
}