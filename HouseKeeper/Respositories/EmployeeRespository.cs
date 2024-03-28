using HouseKeeper.DBContext;
using HouseKeeper.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace HouseKeeper.Respositories
{
    public class EmployeeRespository:IEmployeeRespository
    {
        private readonly HouseKeeperDBContext dBContext;
        private string[] status = new string[] { "Pending approval", "Reject approval",
                                                    "Displayed", "Hidden", "Expired" };
        public EmployeeRespository(HouseKeeperDBContext dBContext)
        {

            this.dBContext = dBContext;
        }
        public async Task<List<TINTUYENDUNG>> GetRecruitments(int page)
        {
            int pageSize = 5;
            if (page < 1)
            {
                page = 1;
            }
            var recruitments = await dBContext.Recruitments.Where(a => a.Status == status[2])
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return recruitments;
        }
        public async Task<TINTUYENDUNG> GetRecruitment(int id)
        {
            return await dBContext.Recruitments.FindAsync(id);
        }
    }
}
