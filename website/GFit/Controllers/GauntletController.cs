#region


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using gFit.BusinessLayer;
using gFit.DBLayer;
using gFit.Models.Base;
using gFit.Models.Composite;
using gFit.Models;

#endregion

namespace gFit.Controllers
{
    [Authorize]
    public class GauntletController : Controller
    {
        public ActionResult Index()
        {
            var lp = new List<GauntletParticipation>();
            var lg = GauntletDBLayer.Instance.GetTodaysGauntlets();

            foreach (var g in lg)
            {
                var p = new GauntletParticipation();
                p.Account = AccountBusinessLayer.GetCurrentAccount();
                p.Gauntlet = GauntletDBLayer.Instance.GetGauntlet(g.Id);
                p.Sets = SetDBLayer.Instance.GetSetsByAccountAndGauntlet(p.Account.Id, p.Gauntlet.Id);
                lp.Add(p);
            }


            return View(lp);
        }


        public ActionResult Status(int id)
        {
            Gauntlet g = GauntletDBLayer.Instance.GetGauntlet(id);

            var p = new GauntletParticipation();
            p.Gauntlet = g;
            p.Account = AccountBusinessLayer.GetCurrentAccount();            
            p.Sets = SetDBLayer.Instance.GetSetsByAccountAndGauntlet(p.Account.Id, p.Gauntlet.Id);



            if (p.IsComplete)
            {
                List<Account> accounts = AccountDBLayer.Instance.GetAccountsThatCompletedGauntlet(p.Gauntlet.Id);



                List<GauntletParticipation> gp = new List<GauntletParticipation>();

                foreach (Account a in accounts)
                {
                    var p2 = new GauntletParticipation();
                    p2.Gauntlet = g;
                    p2.Account = a;
                    p2.Sets = SetDBLayer.Instance.GetSetsByAccountAndGauntlet(a.Id, g.Id);

                    gp.Add(p2);

                }

                p.Statistics = new GauntletStatistics { AllParticipations = gp };

            }



            return View(p);
        }


        public ActionResult DoSet(int id)
        {
            var p = new GauntletParticipation();
            p.Account = AccountBusinessLayer.GetCurrentAccount();
            p.Gauntlet = GauntletDBLayer.Instance.GetGauntlet(id);
            p.Sets = SetDBLayer.Instance.GetSetsByAccountAndGauntlet(p.Account.Id, p.Gauntlet.Id);


            if (p.IsComplete)
            {
                return RedirectToAction("Status", "Gauntlet", new {id});
            }


            if (!p.IsComplete && !p.HasIncompleteSet)
            {
                var s = SetDBLayer.Instance.GetNewSet(p.Account.Id, p.Gauntlet.Id);
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
            var p = new GauntletParticipation();
            p.Account = AccountBusinessLayer.GetCurrentAccount();
            p.Gauntlet = GauntletDBLayer.Instance.GetGauntlet(id);
            p.Sets = SetDBLayer.Instance.GetSetsByAccountAndGauntlet(p.Account.Id, p.Gauntlet.Id);

            //they have to do some reps
            if (numReps == 0)
            {
                return RedirectToAction("Status", "Gauntlet", new {id});
            }


            if (!p.HasIncompleteSet)
            {
                return RedirectToAction("DoSet", "Gauntlet", new {id});
            }

            var s = p.IncompleteSet;
            s.NumReps = Math.Min(numReps, p.RepsLeft);
            s.EndTime = DateTime.UtcNow;
            s.Completed = true;

            SetDBLayer.Instance.StoreCompletedSet(s);

            return RedirectToAction("Status", "Gauntlet", new {id});
        }

        public ActionResult DeleteSet(int id, Guid setGuid)
        {
            var s = new Set {Guid = setGuid};

            SetDBLayer.Instance.DeleteSet(s);

            return RedirectToAction("Status", "Gauntlet", new {id});
        }


        [HttpGet]
        public ActionResult DoSetPostBack(int id)
        {
            return RedirectToAction("Status", "Gauntlet", new {id});
        }
    }
}