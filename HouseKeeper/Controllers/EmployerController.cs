using Microsoft.AspNetCore.Mvc;

namespace HouseKeeper.Controllers
{
    public class EmployerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Recruitment()
        {
            return View();
        }
        public IActionResult ListRecruitment()
        {
            return View();
        }
    }
}
