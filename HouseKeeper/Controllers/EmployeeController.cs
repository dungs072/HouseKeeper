using HouseKeeper.Models.Views.Employee;
using HouseKeeper.Respositories;
using Microsoft.AspNetCore.Mvc;
using HouseKeeper.Models.Views.Recruitments;


namespace HouseKeeper.Controllers
{
    public class EmployeeController:Controller
    {
        private readonly IEmployeeRespository employeeRespository;
        public EmployeeController(IAccountTypeRespository accountTypeRespository, 
                                IEmployeeRespository employeeRespository)
        {
            this.employeeRespository = employeeRespository;
        }
        public async Task<IActionResult> DisplayList(int page)
        {
            try
            {
                Models.Views.Employee.ListRecruitmentViewModel listRecruitmentViewModel = new Models.Views.Employee.ListRecruitmentViewModel();
                listRecruitmentViewModel.Recruitments = await employeeRespository.GetRecruitments(page,"",null,null);
                listRecruitmentViewModel.Cities = await employeeRespository.GetCities();
                listRecruitmentViewModel.Districts = await employeeRespository.GetDistricts();
                return View("IndexEmployee", listRecruitmentViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest("Error occurred while fetching recruitments: " + ex.Message);
            }
        }
        public async Task<ActionResult> LoadMoreItems(int currentPage)
        {
            var items = await employeeRespository.GetRecruitments(currentPage, "", null, null);
            Models.Views.Employee.ListRecruitmentViewModel model = new Models.Views.Employee.ListRecruitmentViewModel();
            model.Recruitments = items;
            return PartialView("ListRecruitmentPartital", model);
        }
        public async Task<ActionResult> JobDetail(int recruitmentId)
        {
            JobDetailViewModel model = new JobDetailViewModel();
            model.Recruitment = await employeeRespository.GetRecruitment(recruitmentId);
            return View(model);
        }
        public async Task<ActionResult> SearchJob(string keyword, int? cityId, int? districtId)
        {
            Models.Views.Employee.ListRecruitmentViewModel listRecruitmentViewModel = new Models.Views.Employee.ListRecruitmentViewModel();
            listRecruitmentViewModel.Recruitments = await employeeRespository.GetRecruitments(1, keyword, cityId, districtId);
            listRecruitmentViewModel.Cities = await employeeRespository.GetCities();
            listRecruitmentViewModel.Districts = await employeeRespository.GetDistricts();
            return View("IndexEmployee", listRecruitmentViewModel);
        }
    }
}
