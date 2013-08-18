using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gSculpt.Models;
using gSculpt.DBLayer;

namespace gSculpt.Controllers
{
    public class GauntletController : Controller
    {
        
        
        public ActionResult Index()
        {

            List<Gauntlet> lg = GauntletDBLayer.Instance.GetTodaysGauntlets();

            return View(lg);

        }





        public ActionResult Details(int id)
        {

            Gauntlet g = GauntletDBLayer.Instance.GetGauntlet(id);

            return View(g);

        }




    }
}
