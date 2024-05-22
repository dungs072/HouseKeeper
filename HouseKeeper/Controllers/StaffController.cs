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
        public async Task<IActionResult> ShowRecruitmentDetail(int recruitmentId, int isPending)
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
                TempData["Error"] = Configs.ModerationConfig.NotFoundNotification;
                return RedirectToAction("ShowRecruitmentAreHandled", model.StaffId);
            }

            if (status == StaffEnum.ModerationStatus.ServerError)
            {
                TempData["Error"] = Configs.ModerationConfig.ServerErrorNotification;
                return RedirectToAction("ShowRecruitmentAreHandled", model.StaffId);
            }

            if (status == StaffEnum.ModerationStatus.IsHandledByOther)
            {
                TempData["Error"] = Configs.ModerationConfig.HandledByOtherNotification;
                return RedirectToAction("ShowRecruitmentAreHandled", model.StaffId);
            }
            model.Recruitment = result.Item2;
            var rejectionsDetailsList = await staffRespository.GetRejectionsDetail(recruitmentId);
            
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
            if (DateTime.Now > model.LastTimeCanEditNotes.AddHours(Configs.ModerationConfig.HoursAllowForEditRejectionNotes) )
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
            return View("RejectionRecruitmentDetail", model);
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
                TempData["Error"] = Configs.ModerationConfig.NotFoundNotification;
                return RedirectToAction("ShowRecruitmentAreHandled", staffId);
            }
            if (result == StaffEnum.ModerationStatus.ServerError)
            {
                TempData["Error"] = Configs.ModerationConfig.ServerErrorNotification;
                return RedirectToAction("ShowRecruitmentDetail", model.RecruitmentId);
            }

            TempData["Success"] = Configs.ModerationConfig.EditSuccessNotification(model.RecruitmentId);
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
                TempData["Error"] = Configs.ModerationConfig.NotFoundNotification;
                return RedirectToAction("ShowRecruitmentAreHandled", model.StaffId);
            }
            if (result == StaffEnum.ModerationStatus.ServerError)
            {
                TempData["Error"] = Configs.ModerationConfig.ServerErrorNotification;
                return RedirectToAction("ShowRecruitmentDetail", model.RecruitmentId);
            }
            TempData["Success"] = Configs.ModerationConfig.RejectSuccessNotification(model.RecruitmentId);
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
                TempData["Error"] = Configs.ModerationConfig.NotFoundNotification;
                return RedirectToAction("ShowRecruitmentAreHandled", staffId);
            }
            if (result == StaffEnum.ModerationStatus.ServerError)
            {
                TempData["Error"] = Configs.ModerationConfig.ServerErrorNotification;
                return RedirectToAction("ShowRecruitmentDetail", recruitmentId);
            }
            TempData["Success"] = Configs.ModerationConfig.AcceptSuccessNotification(recruitmentId);
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
            bool result = await staffRespository.ApproveEmployer(employerId);
            if(result)
            {
                TempData["Success"] = "Approve employer successfully!";
                return RedirectToAction("ShowListEmployers");
            }
            else
            {
                TempData["Error"] = "Approve employer failed!";
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
            bool result = await staffRespository.DisapproveEmployer(model);
            if (result)
            {
                TempData["Success"] = "Approve employer successfully!";
                return RedirectToAction("ShowListEmployers");
            }
            else
            {
                TempData["Error"] = "Approve employer failed!";
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
            bool result = await staffRespository.ApproveEmployee(employeeId);
            if (result)
            {
                TempData["Success"] = "Approve employee successfully!";
                return RedirectToAction("ShowListEmployees");
            }
            else
            {
                TempData["Error"] = "Approve employee failed!";
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
            bool result = await staffRespository.DisapproveEmployee(model);
            if (result)
            {
                TempData["Success"] = "Approve employee successfully!";
                return RedirectToAction("ShowListEmployees");
            }
            else
            {
                TempData["Error"] = "Approve employee failed!";
                return RedirectToAction("ShowEmployeeDetail", model.Employee.EmployeeId);
            }
        }


        // send email
        public async Task<IActionResult> SendEmail()
        {
            if (!CheckCurrentToken())
            {
                TempData["Error"] = "Error. Please dont intrude to other personality";
                return RedirectToAction("Login", "Home");
            }
            try
            {
                MailMessage mail = new MailMessage();

                mail.Subject = "Test send email";
                mail.Body = "<p>Dear {},</p>\r\n<p>You recruitment (with ID: 134) approved successfully</p>\r\n<p>Your recruitment will be displayed on the Website.</p>\r\n<p>Let me know if you need more any information.</p>\r\n<p>Thanks,</p>\r\n<p><b>Sender</b></p>\r\n<p>Website Staff</p>\r\n<p>Email: {} </p>\r\n<p>Cell: {} </p>";
                mail.IsBodyHtml = true;
                mail.From = new MailAddress("noreply@housekeeper.com", "HouseKeeper");
                mail.To.Add(new MailAddress("studywithhuy2002@gmail.com", "Huy02"));
                mail.Priority = MailPriority.High;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("dhuynguyen2002@gmail.com", "yzmg wesw pptc xdae\r\n");
                smtp.EnableSsl = true;
                smtp.Send(mail);
                TempData["Success"] = "Send email successfully!";
                return RedirectToAction("Index");


            }
            catch (Exception e)
            {
                TempData["Error"] = "Send email failed!";
                return RedirectToAction("Index");
            }
        }



    }
}
