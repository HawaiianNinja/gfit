#region

using System.Web.Mvc;
using gFit.BusinessLayer;

#endregion

namespace gFit.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var currentAccount = AccountBusinessLayer.GetCurrentAccount();

            if (currentAccount == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(currentAccount);
        }
    }
}