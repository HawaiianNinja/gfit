﻿using System;
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


        public ActionResult DoSet(int id)
        {

            GauntletParticipation p = new GauntletParticipation();
            p.Account = AccountBusinessLayer.GetCurrentAccount();
            p.Gauntlet = GauntletDBLayer.Instance.GetGauntlet(id);
            p.Sets = SetDBLayer.Instance.GetSetsByAccountAndGauntlet(p.Account.AccountId, p.Gauntlet.Id);


            Set setToDo = new Set();
            if (p.HasIncompleteSet)
            {
                setToDo = p.IncompleteSet;
            }
            else if (!p.IsComplete)
            {
                setToDo = SetDBLayer.Instance.GetNewSet(p.Account.AccountId, p.Gauntlet.Id);
                p.Sets.Add(setToDo);
            }




            return View(p);
        }




    }
}
