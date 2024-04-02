using HouseKeeper.Models.Views.Admin;
using HouseKeeper.Respositories;
using Microsoft.AspNetCore.Mvc;

namespace HouseKeeper.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminRespository adminRespository;
        public AdminController(IAdminRespository adminRespository)
        {
            this.adminRespository = adminRespository;
        }
        public IActionResult Index()
        {
            return View("IndexAdmin");
        }
        public async Task<IActionResult> ShowJobType()
        {
            JobTypeViewModel model = new JobTypeViewModel();
            model.JobTypes = await adminRespository.GetJobTypes();
            return View("JobType", model);
        }
    }
}
