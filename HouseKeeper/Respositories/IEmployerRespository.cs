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
        Task<HINHTHUCTRALUONG> GetPaidType(int id);
        Task<KINHNGHIEM> GetExperience(int id);
        Task<TINHTHANHPHO> GetCity(int id);
        Task<LOAICONGVIEC> GetJob(int id);
        Task<NGUOITHUE> GetEmployer(int id);
        Task<bool> CreateRecruitment(TINTUYENDUNG recruitment, string[] jobIds);
    }
}
