using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Recruitments
{
    public class ListRecruitmentViewModel
    {
        public List<TINTUYENDUNG> OnlineRecruitments { get; set; }
        public List<TINTUYENDUNG> PendingApprovalRecruitments { get;set; }
        public List<TINTUYENDUNG> DisapprovalRecruitments { get; set; }
        public List<TINTUYENDUNG> HiddenRecruitments { get; set; }
        public List<TINTUYENDUNG> OutDatedRecruitments { get; set; }
    }
}
