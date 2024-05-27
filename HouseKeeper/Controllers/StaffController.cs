using HouseKeeper.Configs;
using HouseKeeper.Enum;
using HouseKeeper.IServices;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views;
using HouseKeeper.Models.Views.Staff;
using HouseKeeper.Respositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace HouseKeeper.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffRespository staffRespository;
        private readonly IEmployerRespository employerRespository;
        private readonly IEmployeeRespository employeeRespository;
        private readonly ITokenService tokenService;
        public StaffController(IStaffRespository staffRespository, IEmployerRespository employerRespository,
                            IEmployeeRespository employeeRespository, ITokenService tokenService)
        {
            this.staffRespository = staffRespository;
            this.employerRespository = employerRespository;
            this.employeeRespository = employeeRespository;
            this.tokenService = tokenService;
        }
        private bool CheckCurrentToken()
        {
            string token = Request.Cookies["AuthToken"];
            string role = Request.Cookies["Role"];
            if (token == null) { return false; }
            if (role == null) { return false; }
            if (role != "Staff") { return false; }
            ClaimsPrincipal principal = tokenService.ValidateToken(token);
            return principal != null;
        }

        // Controller get index page for staff
        public ActionResult Index()
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            return View("IndexStaff");
        }

        // Controller get list recruitment not handled by staff
        public async Task<IActionResult> ShowRecruitmentNotHandled()
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            ListRecruitmentsNotHandled model = new ListRecruitmentsNotHandled();
            model.RecruitmentsNotHandled = await staffRespository.GetRecruitmentNotHandled();
            return View("ListRecruitmentsNotHandled", model);
        }

        // Controller get list recruitment are handled by staff
        public async Task<IActionResult> ShowRecruitmentAreHandled()
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            ListRecruitmentsAreHandled model = new ListRecruitmentsAreHandled();
            int staffId = int.Parse(HttpContext.Session.GetString("UserId"));
            model.PendingApprovalRecruitments = await staffRespository.ListRecruitmentAreHandledByStaff(staffId, RecruitmentEnum.RecruitmentStatus.PendingApproval);
            model.DisapprovalRecruitments = await staffRespository.ListRecruitmentAreHandledByStaff(staffId, RecruitmentEnum.RecruitmentStatus.RejectApproval);
            return View("ListRecruitmentsAreHandled", model);
        }



        // Controller get recruitment detail by recruitmentId
        // isPending = 1: show recruitment detail in PendingApprovalRecruitments
        // isPending = 0: show recruitment detail in DisapprovalRecruitments
        public async Task<IActionResult> ShowRecruitmentDetail(int recruitmentId, int isPending = 0)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            RecruitmentModerationViewModel model = new RecruitmentModerationViewModel();
            model.StaffId = int.Parse(HttpContext.Session.GetString("UserId"));
            var result = await staffRespository.GetRecruitment(recruitmentId, model.StaffId);
            var status = result.Item1;

            if (status == StaffEnum.ModerationStatus.NotFound)
            {
                TempData["Error"] = Configs.ModerationConfig.RecruimentNotFoundNotification;
                return RedirectToAction("ShowRecruitmentAreHandled", model.StaffId);
            }

            if (status == StaffEnum.ModerationStatus.ServerError)
            {
                TempData["Error"] = Configs.ModerationConfig.ServerErrorNotification;
                return RedirectToAction("ShowRecruitmentAreHandled", model.StaffId);
            }

            if (status == StaffEnum.ModerationStatus.IsHandledByOther)
            {
                TempData["Error"] = Configs.ModerationConfig.RecruitmentHandledByOtherNotification;
                return RedirectToAction("ShowRecruitmentAreHandled", model.StaffId);
            }

            model.Recruitment = result.Item2;
            var rejectionsDetailsList = await staffRespository.GetRejectionsDetail(recruitmentId);
            isPending = (model.Recruitment.Status.StatusId == (int)RecruitmentEnum.RecruitmentStatus.PendingApproval) ? 1 : 0;
            bool isRejected = (model.Recruitment.Status.StatusId == (int)RecruitmentEnum.RecruitmentStatus.RejectApproval) ? true : false;
            model.RejectionsDetails = new Dictionary<DateTime, List<CHITIETTUCHOI>>();
            foreach (var rejectionDetail in rejectionsDetailsList)
            {
                if (model.RejectionsDetails.ContainsKey(rejectionDetail.Time))
                {
                    model.RejectionsDetails[rejectionDetail.Time].Add(rejectionDetail);
                }
                else
                {
                    model.RejectionsDetails.Add(rejectionDetail.Time, new List<CHITIETTUCHOI> { rejectionDetail });
                }
                if (model.LastTimeCanEditNotes == default(DateTime))
                {
                    model.LastTimeCanEditNotes = rejectionDetail.Time;
                }
                else if (rejectionDetail.Time > model.LastTimeCanEditNotes)
                {
                    model.LastTimeCanEditNotes = rejectionDetail.Time;
                }
            }
            model.NoteIndexCanEdit = 1;
            if (DateTime.Now > model.LastTimeCanEditNotes.AddHours(Configs.ModerationConfig.HoursAllowForEditRejectionNotes) || isRejected == false)
            {
                model.NoteIndexCanEdit = 0;
            }
            
            // get list note can edit
            if (model.NoteIndexCanEdit > 0) {
                model.NoteCanEditList = new List<string?>();
                model.NoteIdCanEditList = new List<int>();
                foreach (var note in model.RejectionsDetails[model.LastTimeCanEditNotes])
                {
                    model.NoteIdCanEditList.Add(note.RejectionDetailId);
                    model.NoteCanEditList.Add(note.Note);
                }
            }

            // set last time can edit notes
            model.LastTimeCanEditNotes = model.LastTimeCanEditNotes.AddHours(Configs.ModerationConfig.HoursAllowForEditRejectionNotes);

            model.Rejections = await staffRespository.GetRejections();
            model.RejectionId = new List<int>();
            for (int i = 0; i < model.Rejections.Count; i++)
            {
                model.RejectionId.Add(model.Rejections[i].RejectionId);
            }
            model.IsSelectedList = Enumerable.Repeat(false, model.Rejections.Count).ToList();
            model.NoteList = Enumerable.Repeat("", model.Rejections.Count).ToList();
            if (isPending == 1)
            {
                return View("PendingRecruitmentDetail", model);
            }

            return View("NotPendingRecruitmentDetail", model);
        }

        // edit note of rejection
        [HttpPost]
        // reject recruitment and change status to Rejected and add reason
        public async Task<IActionResult> EditNotesOfRejection(RecruitmentModerationViewModel model)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            var result = await staffRespository.EditNotesOfRejection(model);
            int staffId = int.Parse(HttpContext.Session.GetString("UserId"));
            if (result == StaffEnum.ModerationStatus.NotFound)
            {
                TempData["Error"] = Configs.ModerationConfig.RecruimentNotFoundNotification;
                return RedirectToAction("ShowRecruitmentAreHandled", staffId);
            }
            if (result == StaffEnum.ModerationStatus.ServerError)
            {
                TempData["Error"] = Configs.ModerationConfig.ServerErrorNotification;
                return RedirectToAction("ShowRecruitmentDetail", model.RecruitmentId);
            }

            TempData["Success"] = Configs.ModerationConfig.EditRecruitmentSuccessNotification(model.RecruitmentId);
            return RedirectToAction("ShowRecruitmentAreHandled", staffId);
        }

        [HttpPost]
        // reject recruitment and change status to Rejected and add reason
        public async Task<IActionResult> RejectRecruitment(RecruitmentModerationViewModel model)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            var result = await staffRespository.RejectRecruitment(model);
            if (result == StaffEnum.ModerationStatus.NotFound)
            {
                TempData["Error"] = Configs.ModerationConfig.RecruimentNotFoundNotification;
                return RedirectToAction("ShowRecruitmentAreHandled", model.StaffId);
            }
            if (result == StaffEnum.ModerationStatus.ServerError)
            {
                TempData["Error"] = Configs.ModerationConfig.ServerErrorNotification;
                return RedirectToAction("ShowRecruitmentDetail", model.RecruitmentId);
            }
            TempData["Success"] = Configs.ModerationConfig.RejectRecruitmentSuccessNotification(model.RecruitmentId);
            return RedirectToAction("ShowRecruitmentAreHandled", model.StaffId);
        }

        // accpect recruitment and change status to Displayed
        public async Task<IActionResult> AcceptRecruitment(int recruitmentId)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            var result = await staffRespository.AcceptRecruitment(recruitmentId);
            int staffId = int.Parse(HttpContext.Session.GetString("UserId"));
            if (result == StaffEnum.ModerationStatus.NotFound)
            {
                TempData["Error"] = Configs.ModerationConfig.RecruimentNotFoundNotification;
                return RedirectToAction("ShowRecruitmentAreHandled", staffId);
            }
            if (result == StaffEnum.ModerationStatus.ServerError)
            {
                TempData["Error"] = Configs.ModerationConfig.ServerErrorNotification;
                return RedirectToAction("ShowRecruitmentDetail", recruitmentId);
            }
            TempData["Success"] = Configs.ModerationConfig.ApproveRecruitmentSuccessNotification(recruitmentId);
            return RedirectToAction("ShowRecruitmentAreHandled", staffId);
        }

        // show profile of staff
        public async Task<IActionResult> Profile()
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int staffId);
            var staff = await staffRespository.GetStaffProfile(staffId);
            if (staff == null)
            {
                TempData["Error"] = "Server error!!!. Can not get staff profile";
                return RedirectToAction("Index");
            }
            StaffProfileViewModel model = new StaffProfileViewModel();
            model.Staff = staff;
            return View("Profile", model);
        }

        public async Task<IActionResult> ChangePassword()
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel();
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
            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["Error"] = "New password and conform password are not match!";
                return View(model);
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int staffId);
            var result = await staffRespository.HasRightPassword(model.CurrentPassword, staffId);
            if (!result)
            {
                TempData["Error"] = "Wrong current password!";
                return View(model);
            }
            var updateResult = await staffRespository.ChangePassword(model.NewPassword, staffId);
            if (!updateResult)
            {
                TempData["Error"] = "Server error!";
                return View(model);
            }
            TempData["Success"] = "Change password successfully!";
            return RedirectToAction("ChangePassword");
        }

        // show list employers
        public async Task<IActionResult> ShowListEmployers(string q = "", int identityStatus=0) 
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            EmployersModerationViewModel model = new EmployersModerationViewModel();
            model.Employers = await employerRespository.GetEmployersWithIdentityStatus(q, identityStatus);
            model.IdentityStatus = await employerRespository.GetIdentityStatus();
            model.QueryInput = q;
            model.currentIdentityStatus = identityStatus;
            return View("ListEmployers", model);
        }

        public async Task<IActionResult> ShowEmployerDetail(int employerId)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            EmployerDetailViewModel model = new EmployerDetailViewModel();
            model.Employer = await employerRespository.GetEmployer(employerId);
            return View("EmployerDetail", model);
        }

        public async Task<IActionResult> ApproveEmployer(int employerId)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int staffId);

            bool result = await staffRespository.ApproveEmployer(employerId, staffId);
            if(result)
            {
                TempData["Success"] = ModerationConfig.ApproveEmployerSuccessNotification(employerId);
                return RedirectToAction("ShowListEmployers");
            }
            else
            {
                TempData["Error"] = ModerationConfig.ServerErrorNotification;
                return RedirectToAction("ShowEmployerDetail", employerId);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> DisapproveEmployer(EmployerDetailViewModel model)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int staffId);
            bool result = await staffRespository.DisapproveEmployer(model, staffId);
            if (result)
            {
                TempData["Success"] = ModerationConfig.DisapproveEmployerSuccessNotification(model.Employer.EmployerId);
                return RedirectToAction("ShowListEmployers");
            }
            else
            {
                TempData["Error"] = ModerationConfig.ServerErrorNotification;
                return RedirectToAction("ShowEmployerDetail", model.Employer.EmployerId);
            }
        }

        // show list employers
        public async Task<IActionResult> ShowListEmployees(string q = "", int identityStatus = 0)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            EmployeesModerationViewModel model = new EmployeesModerationViewModel();
            model.Employees = await employeeRespository.GetEmployeesWithIdentityStatus(q, identityStatus);
            model.IdentityStatus = await employeeRespository.GetIdentityStatus();
            model.QueryInput = q;
            model.currentIdentityStatus = identityStatus;
            return View("ListEmployees", model);
        }

        public async Task<IActionResult> ShowEmployeeDetail(int employeeId)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            EmployeeDetailViewModel model = new EmployeeDetailViewModel();
            model.Employee = await employeeRespository.GetEmployee(employeeId);
            return View("EmployeeDetail", model);
        }

        public async Task<IActionResult> ApproveEmployee(int employeeId)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int staffId);
            bool result = await staffRespository.ApproveEmployee(employeeId, staffId);
            if (result)
            {
                TempData["Success"] = ModerationConfig.ApproveEmployeeSuccessNotification(employeeId);
                return RedirectToAction("ShowListEmployees");
            }
            else
            {
                TempData["Error"] = ModerationConfig.ServerErrorNotification;
                return RedirectToAction("ShowEmployeeDetail", employeeId);
            }

        }

        [HttpPost]
        public async Task<IActionResult> DisapproveEmployee(EmployeeDetailViewModel model)
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            int.TryParse(HttpContext.Session.GetString("UserId"), out int staffId);
            bool result = await staffRespository.DisapproveEmployee(model, staffId);
            if (result)
            {
                TempData["Success"] = ModerationConfig.DisapproveEmployeeSuccessNotification(model.Employee.EmployeeId);
                return RedirectToAction("ShowListEmployees");
            }
            else
            {
                TempData["Error"] = ModerationConfig.ServerErrorNotification;
                return RedirectToAction("ShowEmployeeDetail", model.Employee.EmployeeId);
            }
        }




    }
}
