using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Admin
{
    public class PricePacketViewModel
    {
        public List<GOITIN> PricePackets { get; set; }
        public string QueryInput { get; set; }
    }
}
