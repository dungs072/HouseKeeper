using HouseKeeper.Enum;
using HouseKeeper.Models.Views.Admin;
using HouseKeeper.Respositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        #region Rejection
        public async Task<IActionResult> ShowRejection()
        {
            RejectionViewModel model = new RejectionViewModel();
            model.Rejections = await adminRespository.GetRejections();
            return View("Rejection", model);
        }
        public async Task<IActionResult> AddRejection(string rejectionName)
        {
            var result = await adminRespository.AddRejection(rejectionName);
            if (result)
            {
                TempData["Success"] = "Add rejection successfully!";
            }
            else
            {
                TempData["Error"] = "Server error! Rejection name is duplicate!!";
            }
            return RedirectToAction("ShowRejection");
        }
        public async Task<IActionResult> EditRejection(int rejectionId, string rejectionName)
        {
            var result = await adminRespository.EditRejection(rejectionId, rejectionName);
            if (result)
            {
                TempData["Success"] = "Edit rejection successfully!";
            }
            else
            {
                TempData["Error"] = "Server error! rejection name is duplicate!!";
            }
            return RedirectToAction("ShowRejection");
        }
        public async Task<IActionResult> DeleteRejection(int rejectionId)
        {
            var result = await adminRespository.DeleteRejection(rejectionId);
            if (result)
            {
                TempData["Success"] = "Delete rejection successfully!";
            }
            else
            {
                TempData["Error"] = "Server error!";
            }
            return RedirectToAction("ShowRejection");
        }
        #endregion

        #region PricePacket
        public async Task<IActionResult> ShowPricePackets()
        {
            PricePacketViewModel model = new PricePacketViewModel();
            model.PricePackets = await adminRespository.GetPricePackets();
            return View("PricePacket", model);
        }
        public async Task<IActionResult> AddPricePacket(string pricePacketName, int numberDay, decimal price)
        {
            var result = await adminRespository.AddPricePacket(pricePacketName,price,numberDay);
            if (result)
            {
                TempData["Success"] = "Add price packet successfully!";
            }
            else
            {
                TempData["Error"] = "Server error! price packet name is duplicate!!";
            }
            return RedirectToAction("ShowPricePackets");
        }
        public async Task<IActionResult> EditPricePacket(int pricePacketId, string pricePacketName, int numberDay, decimal price)
        {
            var result = await adminRespository.EditPricePacket(pricePacketId,pricePacketName, price, numberDay);
            if (result)
            {
                TempData["Success"] = "Edit price packet successfully!";
            }
            else
            {
                TempData["Error"] = "Server error! price packet name is duplicate!!";
            }
            return RedirectToAction("ShowPricePackets");
        }
        public async Task<IActionResult> DeletePricePacket(int pricePacketId)
        {
            var result = await adminRespository.DeletePricePacket(pricePacketId);
            if (result)
            {
                TempData["Success"] = "Delete price packet successfully!";
            }
            else
            {
                TempData["Error"] = "Server error!";
            }
            return RedirectToAction("ShowPricePackets");
        }
		#endregion

        
		public ActionResult DrawChart(int selectedYear = 0)
		{
            if (selectedYear == 0) selectedYear = DateTime.Now.Year;

            // Get data points for price packet revenue, bid revenue, total revenue
            var revenueDataPoints = adminRespository.GetRevenueDataPoints(selectedYear).Result;
            
            ViewBag.pricePacketRevenueDataPoints = JsonConvert.SerializeObject(revenueDataPoints[AdminEnum.RevenueType.PricePacket]);
            ViewBag.bidRevenueDataPoints = JsonConvert.SerializeObject(revenueDataPoints[AdminEnum.RevenueType.Bid]);
            ViewBag.totalRevenueDataPoints = JsonConvert.SerializeObject(revenueDataPoints[AdminEnum.RevenueType.Total]);
            ViewBag.SelectedYear = selectedYear;
            ViewBag.StartYear = Configs.StatisticConfig.startYearStatistic;
            ViewBag.EndYear = Configs.StatisticConfig.endYearStatistic;
            return View("RevenueStatistic");
		}
	}
}
