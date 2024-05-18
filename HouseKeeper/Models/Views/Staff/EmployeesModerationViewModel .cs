using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Staff
{
    public class EmployeesModerationViewModel
    {
        public List<NGUOIGIUPVIEC> Employees { get; set; }
        public string QueryInput { get; set; }

        public List<TRANGTHAIDANHTINH> IdentityStatus { get; set; }

        public int currentIdentityStatus { get; set; }
    }
}
