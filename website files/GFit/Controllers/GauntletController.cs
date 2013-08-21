using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gFit.DBLayer;
using gFit.BusinessLayer;
using gFit.Models.Base;
using gFit.Models;
using gFit.Models.Composite;

namespace gFit.Controllers
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


            if (p.IsComplete)
            {
                return RedirectToAction("Status", "Gauntlet", new { id = id });
            }


            if (!p.IsComplete && !p.HasIncompleteSet)
            {
                Set s = SetDBLayer.Instance.GetNewSet(p.Account.Id, p.Gauntlet.Id);
                p.Sets.Add(s);
            }

            if (!p.HasIncompleteSet)
            {
                throw new NullReferenceException();                    
            }

            
            return View(p);
        }


        //id refers to Gauntlet.Id
        //can't call this just [HttpPost]'DoSet' because jqueryMobile won't realize
        //that it redirects to the "Status" action and will think its
        //already on Gauntlet/DoSet/1. This prevents the user from entering 
        //another set once he completes one
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoSetPostBack(int id, int numReps)
        {

            GauntletParticipation p = new GauntletParticipation();
            p.Account = AccountBusinessLayer.GetCurrentAccount();
            p.Gauntlet = GauntletDBLayer.Instance.GetGauntlet(id);
            p.Sets = SetDBLayer.Instance.GetSetsByAccountAndGauntlet(p.Account.Id, p.Gauntlet.Id);

            //they have to do some reps
            if(numReps == 0)
            {
                return RedirectToAction("Status", "Gauntlet", new { id = id});
            }

            

            if(!p.HasIncompleteSet)
            {
                return RedirectToAction("DoSet", "Gauntlet", new {id = id});
            }

            Set s = p.IncompleteSet;
            s.NumReps = Math.Min(numReps,p.RepsLeft);
            s.EndTime = DateTime.UtcNow;
            s.Completed = true;

            SetDBLayer.Instance.StoreCompletedSet(s);

            return RedirectToAction("Status", "Gauntlet", new { id = id });
        }


        public ActionResult DeleteSet(int setId, int gauntletId, int accountId)
        {
            Set s = new Set { Id = setId, GauntletId = gauntletId, AccountId = accountId };

            SetDBLayer.Instance.DeleteSet(s);

            return RedirectToAction("Status", "Gauntlet", new { id = gauntletId });
        }



        [HttpGet]
        public ActionResult DoSetPostBack(int id)
        {
            return RedirectToAction("Status", "Gauntlet", new { id = id });
        }







    }
}
