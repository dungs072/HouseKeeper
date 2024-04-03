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

        #region PaidType
        public async Task<IActionResult> ShowPaidType()
        {
            PaidTypeViewModel model = new PaidTypeViewModel();
            model.PaidTypes = await adminRespository.GetPaidTypes();
            return View("PaidType", model);
        }
        public async Task<IActionResult> AddPaidType(string paidTypeName)
        {
            var result = await adminRespository.AddPaidType(paidTypeName);
            if (result)
            {
                TempData["Success"] = "Add paid type successfully!";
            }
            else
            {
                TempData["Error"] = "Server error! Paid type name is duplicate!!";
            }
            return RedirectToAction("ShowPaidType");
        }
        public async Task<IActionResult> EditPaidType(int paidTypeId, string paidTypeName)
        {
            var result = await adminRespository.EditPaidType(paidTypeId, paidTypeName);
            if (result)
            {
                TempData["Success"] = "Edit paid type successfully!";
            }
            else
            {
                TempData["Error"] = "Server error! Paid type name is duplicate!!";
            }
            return RedirectToAction("ShowPaidType");
        }
        public async Task<IActionResult> DeletePaidType(int paidTypeId)
        {
            var result = await adminRespository.DeletePaidType(paidTypeId);
            if (result)
            {
                TempData["Success"] = "Delete paid type successfully!";
            }
            else
            {
                TempData["Error"] = "Server error!";
            }
            return RedirectToAction("ShowPaidType");
        }
        #endregion

        #region Experience
        public async Task<IActionResult> ShowExperience()
        {
            ExperienceViewModel model = new ExperienceViewModel();
            model.Experiences = await adminRespository.GetExperiences();
            return View("Experience", model);
        }
        public async Task<IActionResult> AddExperience(string experienceName)
        {
            var result = await adminRespository.AddExperience(experienceName);
            if (result)
            {
                TempData["Success"] = "Add experience successfully!";
            }
            else
            {
                TempData["Error"] = "Server error! Experience name is duplicate!!";
            }
            return RedirectToAction("ShowExperience");
        }
        public async Task<IActionResult> EditExperience(int experienceId, string experienceName)
        {
            var result = await adminRespository.EditExperience(experienceId, experienceName);
            if (result)
            {
                TempData["Success"] = "Edit experience successfully!";
            }
            else
            {
                TempData["Error"] = "Server error! experience name is duplicate!!";
            }
            return RedirectToAction("ShowExperience");
        }
        public async Task<IActionResult> DeleteExperience(int experienceId)
        {
            var result = await adminRespository.DeleteExperience(experienceId);
            if (result)
            {
                TempData["Success"] = "Delete experience successfully!";
            }
            else
            {
                TempData["Error"] = "Server error!";
            }
            return RedirectToAction("ShowExperience");
        }
        #endregion
    }
}
