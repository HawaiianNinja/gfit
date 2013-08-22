using gFit.BusinessLayer;
using gFit.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace gFit.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {

            Account currentAccount = AccountBusinessLayer.GetCurrentAccount();

            if (currentAccount == null)
            {
                    return RedirectToAction("Login", "Account");
            }

            return View(currentAccount);

        }

    }
}
