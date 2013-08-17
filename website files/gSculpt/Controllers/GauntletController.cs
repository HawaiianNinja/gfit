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

            List<Gauntlet> gfitGauntlet = GauntletDBLayer.Instance.GetTodaysGauntletsFromDB();

            return View();

        }





        public ActionResult Details(int id)
        {

            return View(new Gauntlet());

        }




    }
}
