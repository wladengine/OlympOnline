using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OlympOnline.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/
        [HttpGet]
        public ActionResult Index(TestModelClass mdl, string id)
        {
            if (TempData.ContainsKey("TestModelClass") && TempData["TestModelClass"] is TestModelClass)
                mdl = (TestModelClass)TempData["TestModelClass"];

            if (mdl.Some == null && !string.IsNullOrEmpty(id))
                return RedirectToAction("Get", new System.Web.Routing.RouteValueDictionary() { { "id", id } });

            return View("Index", mdl);
        }

        [HttpPost]
        public ActionResult Index_Post(TestModelClass mdl)
        {
            return View("Index", mdl);
        }
    }
}
