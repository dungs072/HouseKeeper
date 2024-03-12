using HouseKeeper.DBContext;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.OutPage;
using HouseKeeper.Models.Views.Recruitments;
using Microsoft.EntityFrameworkCore;

namespace HouseKeeper.Respositories
{
    public class EmployerRespository:IEmployerRespository
    {
        private readonly HouseKeeperDBContext dBContext;
        public EmployerRespository(HouseKeeperDBContext dBContext)
        {

            this.dBContext = dBContext;
        }
        public async Task<List<HINHTHUCTRALUONG>> GetPaidTypes()
        {
            return await dBContext.SalaryForms.ToListAsync();
        }
        public async Task<List<KINHNGHIEM>> GetExperiences()
        {
            return await dBContext.Experiences.ToListAsync();
        }
        public async Task<List<TINHTHANHPHO>> GetCities()
        {
            return await dBContext.Cities.ToListAsync();
        }
        public async Task<List<LOAICONGVIEC>> GetJobs()
        {
            return await dBContext.Jobs.ToListAsync();
        }
        public async Task<int>CreateRecruitment(CreateRecruitmentsViewModel model)
        {
            return 1;
        }
    }
}
