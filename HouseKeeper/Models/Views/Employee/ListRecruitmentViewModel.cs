using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Employee
{
    public class ListRecruitmentViewModel
    {
        public List<TINTUYENDUNG> Recruitments { get; set; }
        public List<HUYEN> Districts { get; set; }  
        public List<TINHTHANHPHO> Cities { get; set; }
        public NGUOIGIUPVIEC Employee { get; set; }
    }
}
