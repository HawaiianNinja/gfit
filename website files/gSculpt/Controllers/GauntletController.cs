using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gSculpt.Models;

namespace gSculpt.Controllers
{
    public class GauntletController : Controller
    {
        
        
        
        public ActionResult Index()
        {

            Gauntlet g = new Gauntlet
            {
                Date = DateTime.Today,
                Activity = "Pullups",
                Difficulty = 10,
                Reps = 100
            };

            List<Gauntlet> l = new List<Gauntlet>();
            l.Add(g);
            l.Add(g);


            return View(l);

        }




        public ActionResult Details(int id)
        {



            return View(new Gauntlet());

        }




    }
}
