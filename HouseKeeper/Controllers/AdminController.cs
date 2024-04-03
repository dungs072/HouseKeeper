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
        #region JobType
        public async Task<IActionResult> ShowJobType()
        {
            JobTypeViewModel model = new JobTypeViewModel();
            model.JobTypes = await adminRespository.GetJobTypes();
            return View("JobType", model);
        }
        public async Task<IActionResult> AddJobType(string jobName)
        {
            var result = await adminRespository.AddJobType(jobName);
            if (result)
            {
                TempData["Success"] = "Add job type successfully!";
            }
            else
            {
                TempData["Error"] = "Server error! Job type name is duplicate!!";
            }
            return RedirectToAction("ShowJobType");
        }
        public async Task<IActionResult> EditJobType(int jobId,string jobName)
        {
            var result = await adminRespository.EditJobType(jobId, jobName);
            if(result)
            {
                TempData["Success"] = "Edit job type successfully!";
            }
            else
            {
                TempData["Error"] = "Server error! Job type name is duplicate!!";
            }
            return RedirectToAction("ShowJobType");
        }
        public async Task<IActionResult> DeleteJobType(int jobId)
        {
            var result = await adminRespository.DeleteJobType(jobId);
            if(result)
            {
                TempData["Success"] = "Delete job type successfully!";
            }
            else
            {
                TempData["Error"] = "Server error!";
            }
            return RedirectToAction("ShowJobType");
        }
        #endregion
    }
}
