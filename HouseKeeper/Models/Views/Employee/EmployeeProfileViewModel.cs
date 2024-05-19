using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Employee
{
    public class EmployeeProfileViewModel
    {
        public NGUOIGIUPVIEC Employee { get; set; }
        public List<LOAICONGVIEC> Jobs { get; set; }
        public List<HUYEN> Districts { get; set; }
        public TAIKHOAN Account { get; set; }
    }
}
