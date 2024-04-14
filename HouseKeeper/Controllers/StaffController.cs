using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Recruitments;
using HouseKeeper.Models.Views.Staff;
using HouseKeeper.Respositories;
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

        // Controller get list recruitment is pending approval
        public async Task<IActionResult> ShowRecruitmentPendingApproval()
        {
            ListRecruitmentViewModel model = new ListRecruitmentViewModel();
            model.PendingApprovalRecruitments = await staffRespository.GetRecruitmentsPendingApproval();
            return View("ListRecruitments", model);
        }

        // show recruitment detail by recruitmentId
        public async Task<IActionResult> ShowRecruitmentDetail(int recruitmentId)
        {
            RecruitmentModerationViewModel model = new RecruitmentModerationViewModel();
            model.StaffId = int.Parse(HttpContext.Session.GetString("UserId"));
            model.Recruitment = await staffRespository.GetRecruitment(recruitmentId);
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
            return RedirectToAction("ShowRecruitmentPendingApproval");
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
            return RedirectToAction("ShowRecruitmentPendingApproval");
        }
    }
}
