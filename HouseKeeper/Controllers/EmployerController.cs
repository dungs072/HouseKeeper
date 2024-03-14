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
            var result = employerRespository.CreateRecruitment(recruitment,selectedJobs);
            if(result.Result)
            {
                TempData["Success"] = "Create new recruitment successfully!";
            }
            else
            {
                TempData["Error"] = "Server error. Failed to create new recruitment!";
            }
            return View("Recruitment",model);
        }
        public async Task<IActionResult> ListRecruitment()
        {
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employerId);
            ListRecruitmentViewModel model = await employerRespository.GetEmployerRecruitments(employerId);
            return View(model);
        }

    }
}
