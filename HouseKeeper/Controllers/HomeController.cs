using HouseKeeper.Models;
using HouseKeeper.Respositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HouseKeeper.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccountTypeRespository accountTypeRespository;
        public HomeController(ILogger<HomeController> logger, 
                IAccountTypeRespository accountTypeRespository)
        {
            _logger = logger;
            this.accountTypeRespository = accountTypeRespository;
        }

        public IActionResult Index()
        {
            List<LOAITK> accountTypes = accountTypeRespository.GetAccounts().Result;
            return View(accountTypes);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
