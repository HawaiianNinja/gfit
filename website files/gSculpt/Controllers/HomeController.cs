using gSculpt.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace gSculpt.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {

            Account currentAccount = Session["account"] as Account;


            if (currentAccount == null)
            {

                HttpCookie accountCookie = new HttpCookie("accountCookie");
                accountCookie = Request.Cookies["accountCookie"];

                try
                {
                    currentAccount = JsonConvert.DeserializeObject<Account>(accountCookie.Value);
                }
                catch (Exception e)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (currentAccount == null)
                {
                    return RedirectToAction("Login", "Account");
                }
            }



            return View(currentAccount);

        }

    }
}
