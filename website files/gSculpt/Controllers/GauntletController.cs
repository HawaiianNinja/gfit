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





        public ActionResult Status(int id)
        {
            GauntletParticipation p = new GauntletParticipation();
            p.Account = AccountBusinessLayer.GetCurrentAccount();
            p.Gauntlet = GauntletDBLayer.Instance.GetGauntlet(id);
            p.Sets = SetDBLayer.Instance.GetSetsByAccountAndGauntlet(p.Account.Id, p.Gauntlet.Id);

                       

            return View(p);

        }


        public ActionResult DoSet(int id)
        {

            GauntletParticipation p = new GauntletParticipation();
            p.Account = AccountBusinessLayer.GetCurrentAccount();
            p.Gauntlet = GauntletDBLayer.Instance.GetGauntlet(id);
            p.Sets = SetDBLayer.Instance.GetSetsByAccountAndGauntlet(p.Account.Id, p.Gauntlet.Id);


            if (!p.IsComplete && !p.HasIncompleteSet)
            {
                Set s = SetDBLayer.Instance.GetNewSet(p.Account.Id, p.Gauntlet.Id);
                p.Sets.Add(s);
            }

            
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoSet(int numReps, int gauntletId)
        {


            GauntletParticipation p = new GauntletParticipation();
            p.Account = AccountBusinessLayer.GetCurrentAccount();
            p.Gauntlet = GauntletDBLayer.Instance.GetGauntlet(gauntletId);
            p.Sets = SetDBLayer.Instance.GetSetsByAccountAndGauntlet(p.Account.Id, p.Gauntlet.Id);

            Set s = p.IncompleteSet;
            s.NumReps = numReps;
            s.EndTime = DateTime.UtcNow;
            s.Completed = true;

            SetDBLayer.Instance.StoreCompletedSet(s);

            return RedirectToAction("Status", "Gauntlet", new { id = gauntletId});
        }









    }
}
