using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.OutPage;
using HouseKeeper.Models.Views.Recruitments;
using HouseKeeper.Respositories;
using Microsoft.AspNetCore.Mvc;

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
            string[] selectedJobs = value0.ToString().Split(',');  
            var value1 = Request.Form["isFullTime"];
            bool isFullTime = value1 == "on";
            TINTUYENDUNG recruitment = new TINTUYENDUNG();
            recruitment.MinSalary = model.MinSalary;
            recruitment.MaxSalary = model.MaxSalary;
            recruitment.Age = model.AgeRange;
            recruitment.Note = model.TakeNotes;
            recruitment.FullTime = isFullTime;
            recruitment.MaxApplications = model.NumberVacancies;
            recruitment.SalaryForm = await employerRespository.GetPaidType(model.PaidTypeId);
            recruitment.Experience = await employerRespository.GetExperience(model.ExperienceId);
            recruitment.City = await employerRespository.GetCity(model.CityId);
            var result = employerRespository.CreateRecruitment(recruitment,selectedJobs);
            if(result.Result)
            {
                TempData["Success"] = "Create new recruitment successfully!";
            }
            else
            {
                TempData["Error"] = "Server error. Failed to create new recruitment!";
            }
            return View();// aadd more here
        }
        public IActionResult ListRecruitment()
        {
            return View();
        }

    }
}
