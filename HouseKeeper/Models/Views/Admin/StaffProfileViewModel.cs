using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Admin
{
    public class StaffProfileViewModel
    {
        public NHANVIEN Staff { get; set; }
        public bool isEdit { get; set; }
        public List<TINHTHANHPHO> Cities { get; set; }
        public List<HUYEN> Districts { get; set; }
    }
}
