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

            return View();

        }





        public ActionResult Details(int id)
        {



            return View(new Gauntlet());

        }




    }
}
