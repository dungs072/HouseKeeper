using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.OutPage;
using HouseKeeper.Models.Views.Recruitments;
using HouseKeeper.Respositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HouseKeeper.Controllers
{
    public class EmployerController : Controller
    {
        private readonly IEmployerRespository employerRespository;
        public EmployerController(IEmployerRespository employerRespository)
        {
            this.employerRespository = employerRespository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Recruitment()
        {
            CreateRecruitmentsViewModel model = new CreateRecruitmentsViewModel();
            List<HINHTHUCTRALUONG> paidTypes = await employerRespository.GetPaidTypes();
            List<KINHNGHIEM> experiences = await employerRespository.GetExperiences();
            List<TINHTHANHPHO> cities = await employerRespository.GetCities();
            List<LOAICONGVIEC> jobs = await employerRespository.GetJobs();
            model.PaidTypes = paidTypes;
            model.Experiences = experiences;
            model.Cities = cities;
            model.jobs = jobs;
            model.MaxSalary = 25;
            model.NumberVacancies = 1;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> HandleCreateRecruitment(CreateRecruitmentsViewModel model)
        {
            var value0 = Request.Form["SelectedJobs"];
           // string[] selectedJobs = value0.ToString().Split(',');  
            var value1 = Request.Form["isFullTime"];
            bool isFullTime = value1 == "on";
            var value2 = Request.Form["min-age"];
            var value3 = Request.Form["max-age"];
            var value4 = Request.Form["PaidTypeId"];
            var value5 = Request.Form["ExperienceId"];
            var value6 = Request.Form["CityId"];
            var value7 = Request.Form["Gender"];
            TINTUYENDUNG recruitment = new TINTUYENDUNG();
            recruitment.MinSalary = model.MinSalary*1000;
            recruitment.MaxSalary = model.MaxSalary*1000;
            recruitment.Age = value2+"-"+value3;
            recruitment.Note = model.TakeNotes;
            recruitment.FullTime = isFullTime;
            recruitment.MaxApplications = model.NumberVacancies;
            recruitment.SalaryForm = await employerRespository.GetPaidType(int.Parse(value4));
            recruitment.Experience = await employerRespository.GetExperience(int.Parse(value5));
            recruitment.City = await employerRespository.GetCity(int.Parse(value6));
            if(value7=="Null")
            {
                recruitment.Gender = null;
            }
            else
            {
                recruitment.Gender = value7;
            }
            recruitment.PostTime = DateTime.Now;
            recruitment.RecruitDeadlineDate = null;
            int.TryParse(HttpContext.Session.GetString("UserId"),out int employerId);
            recruitment.Employer = await employerRespository.GetEmployer(employerId);
            PriceTagViewModel priceModel = new PriceTagViewModel();
            priceModel.Recruitment = recruitment;
            priceModel.JobIds = value0;
           
            //var result = employerRespository.CreateRecruitment(recruitment,selectedJobs);
            //if(result.Result)
            //{
            //    TempData["Success"] = "Please choose your price you want!";
            //}
            //else
            //{
            //    TempData["Error"] = "Server error. Failed to create new recruitment!";
            //}
            string priceTagViewModelJson = JsonConvert.SerializeObject(priceModel);
            HttpContext.Session.SetString("PriceTagViewModel",priceTagViewModelJson);
            TempData["Success"] = "Please choose your price you want!";
            return RedirectToAction("PriceTag");
        }
        public async Task<IActionResult> PriceTag()
        {
            string priceTagViewModelJson = HttpContext.Session.GetString("PriceTagViewModel") as string;
            if (!string.IsNullOrEmpty(priceTagViewModelJson))
            {
                PriceTagViewModel model = JsonConvert.DeserializeObject<PriceTagViewModel>(priceTagViewModelJson);
                if (model == null)
                {
                    TempData["Error"] = "Server error!";
                    return RedirectToAction("Index", "Home");
                }
                model.PriceTags = await employerRespository.GetPriceTags();
                TempData["Success"] = "Please set your price to display above other recruitments!";
                return View(model);
            }
            TempData["Error"] = "Server error!";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> BidPrice(int pricePacketId)
        {
            string priceTagViewModelJson = HttpContext.Session.GetString("PriceTagViewModel") as string;
            if (!string.IsNullOrEmpty(priceTagViewModelJson))
            {
                PriceTagViewModel model = JsonConvert.DeserializeObject<PriceTagViewModel>(priceTagViewModelJson);
                if (model == null)
                {
                    TempData["Error"] = "Server error!";
                    return RedirectToAction("Index", "Home");
                }
                model.PricePacketId = pricePacketId;
                return View(model);
            }
            TempData["Error"] = "Server error!";
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> HandleSendBidPrice(PriceTagViewModel tempModel)
        {
            string priceTagViewModelJson = HttpContext.Session.GetString("PriceTagViewModel") as string;
            if (!string.IsNullOrEmpty(priceTagViewModelJson))
            {
                PriceTagViewModel model = JsonConvert.DeserializeObject<PriceTagViewModel>(priceTagViewModelJson);
                if (model == null)
                {
                    TempData["Error"] = "Server error!";
                    return RedirectToAction("Index", "Home");
                }
                model.Recruitment.BidPrice = tempModel.BidPrice;
                TempData["Success"] = "Please finish your payment";
                return View("CheckOut",model);
            }
            TempData["Error"] = "Server error!";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> CheckOut()
        {
            string priceTagViewModelJson = HttpContext.Session.GetString("PriceTagViewModel") as string;
            if (!string.IsNullOrEmpty(priceTagViewModelJson))
            {
                PriceTagViewModel model = JsonConvert.DeserializeObject<PriceTagViewModel>(priceTagViewModelJson);
                if (model == null)
                {
                    TempData["Error"] = "Server error!";
                    return RedirectToAction("Index", "Home");
                }
                string[] selectedJobs = model.JobIds.ToString().Split(',');
                var state = await employerRespository.CreateRecruitment(model.Recruitment, selectedJobs);
                if(state)
                {
                    TempData["Success"] = "Create new recruitment successfully";
                    return RedirectToAction("ListRecruitment");
                }
                else
                {
                    TempData["Error"] = "Fail to create new recruitment";
                    return View("CheckOut", model);
                }
            }
            TempData["Error"] = "Server error!";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> ListRecruitment()
        {
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employerId);
            ListRecruitmentViewModel model = await employerRespository.GetEmployerRecruitments(employerId);
            return View(model);
        }


    }
}
