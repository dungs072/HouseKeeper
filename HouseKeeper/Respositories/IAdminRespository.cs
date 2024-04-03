using HouseKeeper.Models.DB;

namespace HouseKeeper.Respositories
{
    public interface IAdminRespository
    {
        Task<List<LOAICONGVIEC>> GetJobTypes();
        Task<bool> AddJobType(string jobName);
        Task<bool> EditJobType(int jobId, string jobName);
        Task<bool> DeleteJobType(int jobId);
    }
}
