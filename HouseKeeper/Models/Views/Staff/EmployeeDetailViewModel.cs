using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Staff
{
    public class EmployeeDetailViewModel
    {
        public NGUOIGIUPVIEC Employee { get; set; }
        public string DisapprovalReason { get; set; }
    }
}
