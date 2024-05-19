using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views
{
    public class ChangePasswordViewModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public TAIKHOAN Account { get; set; }   
    }
}
