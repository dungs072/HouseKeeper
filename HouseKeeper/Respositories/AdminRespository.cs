using HouseKeeper.DBContext;
using HouseKeeper.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace HouseKeeper.Respositories
{
    public class AdminRespository:IAdminRespository
    {
        private readonly HouseKeeperDBContext dBContext;
        public AdminRespository(HouseKeeperDBContext dBContext)
        {

            this.dBContext = dBContext;
        }
        public async Task<List<LOAICONGVIEC>> GetJobTypes()
        {
            return await dBContext.Jobs.ToListAsync();
        }
    }
}
