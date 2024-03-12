using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Recruitments;
using Microsoft.EntityFrameworkCore;

namespace HouseKeeper.Respositories
{
    public interface IEmployerRespository
    {
        Task<List<HINHTHUCTRALUONG>> GetPaidTypes();
        Task<List<KINHNGHIEM>> GetExperiences();
        Task<List<TINHTHANHPHO>> GetCities();
        Task<List<LOAICONGVIEC>> GetJobs();
        Task<int> CreateRecruitment(CreateRecruitmentsViewModel model);
    }
}
