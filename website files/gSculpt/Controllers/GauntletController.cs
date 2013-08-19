using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gSculpt.DBLayer;
using gSculpt.BusinessLayer;
using gSculpt.Models.Base;
using gSculpt.Models;
using gSculpt.Models.Composite;

namespace gSculpt.Controllers
{
    [Authorize]
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


        public ActionResult TakeGauntlet(int id)
        {

            GauntletParticipation p = new GauntletParticipation();
            p.Account = AccountBusinessLayer.GetCurrentAccount();
            p.Gauntlet = GauntletDBLayer.Instance.GetGauntlet(id);

                


            Set s = SetDBLayer.Instance.GetNewSet(p.Account.AccountId, p.Gauntlet.Id);

            


            return View();
        }




    }
}
