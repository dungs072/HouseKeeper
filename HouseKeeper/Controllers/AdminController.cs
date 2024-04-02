using Microsoft.AspNetCore.Mvc;

namespace HouseKeeper.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View("IndexAdmin");
        }
    }
}
