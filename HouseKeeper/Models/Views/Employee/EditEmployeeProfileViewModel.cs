using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Employee
{
    public class EditEmployeeProfileViewModel
    {
        public NGUOIGIUPVIEC Employee { get; set; }

        public int isIdentityApproved { get; set; }

        public List<TINHTHANHPHO> Cities { get; set; }

        public List<HUYEN> Districts { get; set; }
    }
}
