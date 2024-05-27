using HouseKeeper.Models.Views.Employee;
using HouseKeeper.Respositories;
using Microsoft.AspNetCore.Mvc;
using HouseKeeper.Models.Views.Employer;
using HouseKeeper.Enum;
using static HouseKeeper.Enum.AccountEnum;
using HouseKeeper.Models.Views;
using HouseKeeper.IServices;
using System.Runtime.InteropServices;
using HouseKeeper.Configs;
using System.Security.Claims;


namespace HouseKeeper.Controllers
{
    public class EmployeeController:Controller
    {
        private readonly IEmployeeRespository employeeRespository;
        private readonly ITokenService tokenService;
        private AccountEnum.AccountType accountType = AccountEnum.AccountType.Employee;
        public EmployeeController(IAccountTypeRespository accountTypeRespository, 
                                IEmployeeRespository employeeRespository,ITokenService tokenService)
        {
            this.employeeRespository = employeeRespository;
            this.tokenService = tokenService;
        }
        private bool CheckCurrentToken()
        {
            string token = Request.Cookies["AuthToken"];
            string role = Request.Cookies["Role"];
            if (token == null) { return false; }
            if (role == null) { return false; }
            if (role != "Employee") { return false; }
            ClaimsPrincipal principal = tokenService.ValidateToken(token);
            return principal != null;
        }
        public async Task<IActionResult> DisplayList(int page)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            try
            {
                Models.Views.Employee.ListRecruitmentViewModel listRecruitmentViewModel = new Models.Views.Employee.ListRecruitmentViewModel();
                listRecruitmentViewModel.Recruitments = await employeeRespository.GetRecruitments(page,"",null,null);
                listRecruitmentViewModel.Cities = await employeeRespository.GetCities();
                listRecruitmentViewModel.Districts = await employeeRespository.GetDistricts();
                int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
                listRecruitmentViewModel.Employee = await employeeRespository.GetEmployee(employeeId);
                return View("IndexEmployee", listRecruitmentViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest("Error occurred while fetching recruitments: " + ex.Message);
            }
        }
        public async Task<ActionResult> LoadMoreItems(int currentPage)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            var items = await employeeRespository.GetRecruitments(currentPage, "", null, null);
            Models.Views.Employee.ListRecruitmentViewModel model = new Models.Views.Employee.ListRecruitmentViewModel();
            model.Recruitments = items;
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            model.Employee = await employeeRespository.GetEmployee(employeeId);
            return PartialView("ListRecruitmentPartital", model);
        }
        public async Task<ActionResult> JobDetail(int recruitmentId)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            JobDetailViewModel model = new JobDetailViewModel();
            model.Recruitment = await employeeRespository.GetRecruitment(recruitmentId);
            var applyDetail = await employeeRespository.GetApplyDetail(recruitmentId, employeeId);
            model.ApplyDetail = applyDetail;
            model.Account = await employeeRespository.GetAccount(employeeId);
            return View(model);
        }
        public async Task<ActionResult> ApplyJob(int recruitmentId)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
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
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            var result = await employeeRespository.CancelApplyJob(applyDetailId);
            if (result)
            {
                TempData["Success"] = "Cancel to apply to this recruitment successfully";
                if(isList)
                {
                    return RedirectToAction("GetAppliedRecruitment");
                }
                return RedirectToAction("JobDetail", new { recruitmentId = recruitmentId });
            }
            else
            {
                TempData["Error"] = "Fail to cancel your application to this recruitment";
                if (isList)
                {
                    return RedirectToAction("GetAppliedRecruitment");
                }
                return RedirectToAction("JobDetail", new { recruitmentId = recruitmentId });
            }
        }
        public async Task<ActionResult> SearchJob(string keyword, int? cityId, int? districtId)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            Models.Views.Employee.ListRecruitmentViewModel listRecruitmentViewModel = new Models.Views.Employee.ListRecruitmentViewModel();
            listRecruitmentViewModel.Recruitments = await employeeRespository.GetRecruitments(1, keyword, cityId, districtId);
            listRecruitmentViewModel.Cities = await employeeRespository.GetCities();
            listRecruitmentViewModel.Districts = await employeeRespository.GetDistricts();
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            listRecruitmentViewModel.Employee = await employeeRespository.GetEmployee(employeeId);
            return View("IndexEmployee", listRecruitmentViewModel);
        }
        public async Task<ActionResult> GetAppliedRecruitment()
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            ListAppliedRecruitmentViewModel model = new ListAppliedRecruitmentViewModel();
            model.ApplyDetails = await employeeRespository.GetApplyRecruitmentList(employeeId);
            model.Account = await employeeRespository.GetAccount(employeeId);
            return View("ListAppliedRecruitment", model);
        }
        public async Task<IActionResult> Profile()
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            EmployeeProfileViewModel model = new EmployeeProfileViewModel();
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            model.Employee = await employeeRespository.GetEmployee(employeeId);
            model.Districts = await employeeRespository.GetWorkplacesForEmployee(employeeId);
            model.Jobs = await employeeRespository.GetJobsForEmployee(employeeId);
            return View(model);
        }

        public async Task<IActionResult> AddJob(int jobId)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            var result = await employeeRespository.AddJob(jobId, employeeId);
            if(result)
            {
                TempData["Success"] = "Add job successfully";
                return RedirectToAction("Profile");
            }
            else
            {
                TempData["Error"] = "Server error!!!. Add job failed";
                return RedirectToAction("Profile");
            }
        }
        public async Task<IActionResult> DeleteJob(int jobId)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            var result = await employeeRespository.DeleteJob(jobId, employeeId);
            if (result)
            {
                TempData["Success"] = "Delete job successfully";
                return RedirectToAction("Profile");
            }
            else
            {
                TempData["Error"] = "Server error!!!. Delete job failed";
                return RedirectToAction("Profile");
            }
        }
        public async Task<IActionResult> AddDistrict(int districtId)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            var result = await employeeRespository.AddDistrict(districtId, employeeId);
            if (result)
            {
                TempData["Success"] = "Add workplace successfully";
                return RedirectToAction("Profile");
            }
            else
            {
                TempData["Error"] = "Server error!!!. Add workplace failed";
                return RedirectToAction("Profile");
            }
        }
        public async Task<IActionResult> DeleteDistrict(int districtId)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            var result = await employeeRespository.DeleteDistrict(districtId, employeeId);
            if (result)
            {
                TempData["Success"] = "Delete district successfully";
                return RedirectToAction("Profile");
            }
            else
            {
                TempData["Error"] = "Server error!!!. Delete district failed";
                return RedirectToAction("Profile");
            }
        }

        public async Task<IActionResult> EditProfile()
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            EditEmployeeProfileViewModel model = new EditEmployeeProfileViewModel();
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            model.Employee = await employeeRespository.GetEmployee(employeeId);
            model.Cities = await employeeRespository.GetCities();
            model.Districts = await employeeRespository.GetDistricts();
            model.isIdentityApproved = (model.Employee.IdentityState != null && model.Employee.IdentityState.IdentityStateId == (int)IdentityEnum.IdentiyStatus.Approve) ? 1 : 0;
            
            return View("EditProfile", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditEmployeeProfileViewModel model, IFormFile avatarImage, IFormFile frontImage, IFormFile backImage)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            model.Employee.EmployeeId = employeeId;

            var result = await employeeRespository.EditEmployeeProfile(model, employeeId, avatarImage, frontImage, backImage, accountType);
            if (result == AccountEnum.CreateEditAccountResult.Success)
            {
                TempData["Success"] = CreateEditAccountConfig.EditProfileSuccess;
                return RedirectToAction("Profile");
            }
            switch (result)
            {
                case AccountEnum.CreateEditAccountResult.PhoneDuplicated:
                    TempData["Error"] = CreateEditAccountConfig.PhoneDuplicated;
                    break;
                case AccountEnum.CreateEditAccountResult.GmailDuplicated:
                    TempData["Error"] = CreateEditAccountConfig.GmailDuplicated;
                    break;
                case AccountEnum.CreateEditAccountResult.CitizenNumberDuplicated:
                    TempData["Error"] = CreateEditAccountConfig.CitizenNumberDuplicated;
                    break;
                case AccountEnum.CreateEditAccountResult.ServerError:
                    TempData["Error"] = CreateEditAccountConfig.ServerError;
                    break;
                case AccountEnum.CreateEditAccountResult.FrontImageError:
                    TempData["Error"] = CreateEditAccountConfig.FrontImageError;
                    break;
                case AccountEnum.CreateEditAccountResult.BackImageError:
                    TempData["Error"] = CreateEditAccountConfig.BackImageError;
                    break;
            }

            model.Cities = await employeeRespository.GetCities();
            model.Districts = await employeeRespository.GetDistricts();
            var district = await employeeRespository.GetDistrict(model.Employee.District.DistrictId);
            model.Employee.District.City = await employeeRespository.GetCity(district.City.CityId);
            return View(model);
        }
        public async Task<IActionResult> ShowProposalJob()
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            JobProposalViewModel model = new JobProposalViewModel();
            model = await employeeRespository.GetJobProposals(employeeId);
            model.Account = await employeeRespository.GetAccount(employeeId);
            return View("JobProposal", model);
        }
        public async Task<IActionResult> ChangePassword()
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            model.Account = await employeeRespository.GetAccount(employeeId);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            model.Account = await employeeRespository.GetAccount(employeeId);
            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["Error"] = "New password and conform password are not match!";
                return View(model);
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employerId);
            var result = await employeeRespository.HasRightPassword(model.CurrentPassword, employerId);
            if (!result)
            {
                TempData["Error"] = "Wrong current password!";
                return View(model);
            }
            var updateResult = await employeeRespository.ChangePassword(model.NewPassword, employerId);
            if (!updateResult)
            {
                TempData["Error"] = "Server error!";
                return View(model);
            }
            TempData["Success"] = "Change password successfully!";
            return RedirectToAction("ChangePassword");
        }

        public async Task<IActionResult> EmployerProfile(int employerId)
        {
            EmployerProfileViewModel model = new EmployerProfileViewModel();
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employeeId);
            model.Employee = await employeeRespository.GetEmployee(employeeId);
            model.Employer = await employeeRespository.GetEmployer(employerId);
            return View(model);
        }

    }
}
