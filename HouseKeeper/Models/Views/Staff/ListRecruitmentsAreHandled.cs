using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Staff
{
    public class ListRecruitmentsAreHandled
    {
        public List<TINTUYENDUNG> PendingApprovalRecruitments { get; set; }
        public List<TINTUYENDUNG> DisapprovalRecruitments { get; set; }
    }
}
