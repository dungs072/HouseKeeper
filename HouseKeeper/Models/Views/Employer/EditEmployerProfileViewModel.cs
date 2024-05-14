using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Employer
{
    public class EditEmployerProfileViewModel
    {
        public NGUOITHUE Employer { get; set; }

        public int isIdentityApproved { get; set; }

        public List<TINHTHANHPHO> Cities { get; set; }

        public List<HUYEN> Districts { get; set; }
    }
}
