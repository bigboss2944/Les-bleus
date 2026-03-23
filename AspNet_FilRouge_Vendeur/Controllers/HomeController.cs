using Microsoft.AspNetCore.Mvc;

namespace AspNet_FilRouge_Vendeur.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Message = "Les bleus, our story.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Please, for any question.";
            return View();
        }
    }
}
