using HouseKeeper.Enum;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Admin;
using HouseKeeper.Models.Views.Staff;
using HouseKeeper.Respositories;
using Humanizer.Localisation.TimeToClockNotation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace HouseKeeper.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffRespository staffRespository;
        public StaffController(IStaffRespository staffRespository)
        {
            this.staffRespository = staffRespository;
        }

        // Controller get index page for staff
        public ActionResult Index()
        {
            return View("IndexStaff");
        }

        // Controller get list recruitment not handled by staff
        public async Task<IActionResult> ShowRecruitmentNotHandled()
        {
            ListRecruitmentsNotHandled model = new ListRecruitmentsNotHandled();
            model.RecruitmentsNotHandled = await staffRespository.GetRecruitmentNotHandled();
            return View("ListRecruitmentsNotHandled", model);
        }

        // Controller get list recruitment are handled by staff
        public async Task<IActionResult> ShowRecruitmentAreHandled()
        {
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
        
    }
}
