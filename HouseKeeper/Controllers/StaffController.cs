using HouseKeeper.Enum.Staff;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Staff;
using HouseKeeper.Respositories;
using Humanizer.Localisation.TimeToClockNotation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HouseKeeper.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffRespository staffRespository;
        public StaffController(IStaffRespository staffRespository)
        {
            this.staffRespository = staffRespository;
        }
        // GET: StaffController
        public ActionResult Index()
        {
            return View("IndexStaff");
        }

        // Controller get list recruitment is pending approval and is waiting for moderation
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
            model.RecruimentsAreHandled = await staffRespository.ListRecruitmentAreHandledByStaff(staffId);
            return View("ListRecruitmentsAreHandled", model);
        }



        // show recruitment detail by recruitmentId
        public async Task<IActionResult> ShowRecruitmentDetail(int recruitmentId)
        {
            RecruitmentModerationViewModel model = new RecruitmentModerationViewModel();
            model.StaffId = int.Parse(HttpContext.Session.GetString("UserId"));
            var result = await staffRespository.GetRecruitment(recruitmentId, model.StaffId);
            var status = result.Item1;
            
            if (status == EnumStaff.ModerationStatus.NotFound)
            {
                TempData["Error"] = "Recruitment not found or is deleted!";
                return RedirectToAction("ShowRecruitmentNotHandled");
            }

            if (status == EnumStaff.ModerationStatus.ServerError)
            {
                TempData["Error"] = "Server Error, Try again!";
                return RedirectToAction("ShowRecruitmentNotHandled");
            }

            if (status == EnumStaff.ModerationStatus.IsHandledByOther)
            {
                TempData["Error"] = "Recruitment is handled by other staff";
                return RedirectToAction("ShowRecruitmentNotHandled");
            }
            model.Recruitment = result.Item2;
            model.Rejections = await staffRespository.GetRejections();
            model.RejectionId = new List<int>();
            for(int i = 0; i < model.Rejections.Count; i++)
            {
                model.RejectionId.Add(model.Rejections[i].RejectionId);
            }
            model.IsSelectedList = Enumerable.Repeat(false, model.Rejections.Count).ToList();
            model.NoteList = Enumerable.Repeat("", model.Rejections.Count).ToList();
            return View("RecruitmentDetail", model);
        }

        [HttpPost]
        // reject recruitment and change status to Rejected and add reason
        public async Task<IActionResult> RejectRecruitment(RecruitmentModerationViewModel model)
        {
            var result = await staffRespository.RejectRecruitment(model);
            if (result)
            {
                TempData["Success"] = "Reject recruitment successfully!";
            }
            else
            {
                TempData["Error"] = "Server error!";
            }
            return RedirectToAction("ShowRecruitmentDetail", model.RecruitmentId);
        }

        // GET: StaffController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // Từ chối tin chưa sửa ID Staff
        // accpect recruitment and change status to Displayed
        public async Task<IActionResult> AcceptRecruitment(int recruitmentId)
        {
            var result = await staffRespository.AcceptRecruitment(recruitmentId);
            if (result)
            {
                TempData["Success"] = "Accept recruitment successfully!";
            }
            else
            {
                TempData["Error"] = "Server error!";
            }
            return RedirectToAction("ShowRecruitmentDetail", recruitmentId);
        }
    }
}
