using HouseKeeper.Models.Views.Employee;
using HouseKeeper.Respositories;
using Microsoft.AspNetCore.Mvc;
using HouseKeeper.Models.Views.Employer;


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
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            JobDetailViewModel model = new JobDetailViewModel();
            model.Recruitment = await employeeRespository.GetRecruitment(recruitmentId);
            var applyDetail = await employeeRespository.GetApplyDetail(recruitmentId, employeeId);
            model.ApplyDetail = applyDetail;
            return View(model);
        }
        public async Task<ActionResult> ApplyJob(int recruitmentId)
        {
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            var result = await employeeRespository.ApplyJob(recruitmentId,employeeId);
            if (result)
            {
                TempData["Success"] = "Apply to this recruitment successfully";
                return RedirectToAction("JobDetail", new { recruitmentId = recruitmentId });
            }
            else
            {
                TempData["Error"] = "Fail to apply to this recruitment";
                return RedirectToAction("JobDetail", new { recruitmentId = recruitmentId });
            }
        }
        public async Task<ActionResult> CancelApplyJob(int recruitmentId,int applyDetailId,bool isList = false)
        {
            var result = await employeeRespository.CancelApplyJob(applyDetailId);
            if (result)
            {
                TempData["Success"] = "Apply to this recruitment successfully";
                if(isList)
                {
                    return RedirectToAction("GetAppliedRecruitment");
                }
                return RedirectToAction("JobDetail", new { recruitmentId = recruitmentId });
            }
            else
            {
                TempData["Error"] = "Fail to apply to this recruitment";
                if (isList)
                {
                    return RedirectToAction("GetAppliedRecruitment");
                }
                return RedirectToAction("JobDetail", new { recruitmentId = recruitmentId });
            }
        }
        public async Task<ActionResult> SearchJob(string keyword, int? cityId, int? districtId)
        {
            Models.Views.Employee.ListRecruitmentViewModel listRecruitmentViewModel = new Models.Views.Employee.ListRecruitmentViewModel();
            listRecruitmentViewModel.Recruitments = await employeeRespository.GetRecruitments(1, keyword, cityId, districtId);
            listRecruitmentViewModel.Cities = await employeeRespository.GetCities();
            listRecruitmentViewModel.Districts = await employeeRespository.GetDistricts();
            return View("IndexEmployee", listRecruitmentViewModel);
        }
        public async Task<ActionResult> GetAppliedRecruitment()
        {
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            ListAppliedRecruitmentViewModel model = new ListAppliedRecruitmentViewModel();
            model.ApplyDetails = await employeeRespository.GetApplyRecruitmentList(employeeId);
            return View("ListAppliedRecruitment", model);
        }
    }
}
