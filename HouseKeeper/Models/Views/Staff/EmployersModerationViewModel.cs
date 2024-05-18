using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Staff
{
    public class EmployersModerationViewModel
    {
        public List<NGUOITHUE> Employers { get; set; }
        public string QueryInput { get; set; }

        public List<TRANGTHAIDANHTINH> IdentityStatus { get; set; }

        public int currentIdentityStatus { get; set; }
    }
}
