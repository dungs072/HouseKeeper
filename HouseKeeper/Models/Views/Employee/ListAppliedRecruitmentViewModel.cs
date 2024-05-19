using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Employee
{
    public class ListAppliedRecruitmentViewModel
    {
        public List<CHITIETAPPLY> ApplyDetails { get; set; }
        public TAIKHOAN Account { get; set; }
    }
}
