using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OlympOnline.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string lang)
        {
            //if (!Request.IsSecureConnection)
            //    return Redirect("https://olymp.spbu.ru/");

            Guid g;
            if (!Util.CheckAuthCookies(Request.Cookies, out g))
                return RedirectToAction("LogOn", "Account");
            else
                return RedirectToAction("Main", "Applicant");
        }
    }
}
