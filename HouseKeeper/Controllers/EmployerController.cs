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
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> HandleCreateRecruitment(CreateRecruitmentsViewModel model)
        {
            TINTUYENDUNG recruitment = new TINTUYENDUNG();
            recruitment.MinSalary = model.MinSalary;
            recruitment.MaxSalary = model.MaxSalary;
            recruitment.Age = model.AgeRange;
            recruitment.FullTime = model.IsFulltime;
            recruitment.Note = model.TakeNotes;
            return View();// aadd more here
        }
        public IActionResult ListRecruitment()
        {
            return View();
        }

    }
}
