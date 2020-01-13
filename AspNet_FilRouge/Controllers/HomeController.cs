using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNet_FilRouge.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Les bleus, our story.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Please, for any question.";

            return View();
        }
    }
}