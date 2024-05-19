using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Admin
{
    public class CityViewModel
    {
        public List<TINHTHANHPHO> Cities { get; set; }
    }
    public class DistrictModel
    {
        public List<HUYEN> Districts { get; set;}
        public int CityId { get; set; }
    }
}
