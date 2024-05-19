using HouseKeeper.Models.DB;
using Stripe;

namespace HouseKeeper.Models.Views.Employer
{
    public class ListRecruitmentViewModel
    {
        public List<TINTUYENDUNG> OnlineRecruitments { get; set; }
        public List<TINTUYENDUNG> PendingApprovalRecruitments { get;set; }
        public List<TINTUYENDUNG> DisapprovalRecruitments { get; set; }
        public List<TINTUYENDUNG> HiddenRecruitments { get; set; }
        public List<TINTUYENDUNG> OutDatedRecruitments { get; set; }
        public TAIKHOAN Account { get; set; }
    }
}
