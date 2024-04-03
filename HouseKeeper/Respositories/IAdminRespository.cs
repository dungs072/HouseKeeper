using HouseKeeper.Models.DB;

namespace HouseKeeper.Respositories
{
    public interface IAdminRespository
    {
        // JobType
        Task<List<LOAICONGVIEC>> GetJobTypes();
        Task<bool> AddJobType(string jobName);
        Task<bool> EditJobType(int jobId, string jobName);
        Task<bool> DeleteJobType(int jobId);
        // PaidType
        Task<List<HINHTHUCTRALUONG>> GetPaidTypes();
        Task<bool> AddPaidType(string paidTypeName);
        Task<bool> EditPaidType(int paidTypeId, string paidTypeName);
        Task<bool> DeletePaidType(int paidTypeId);
        //Experience
        Task<List<KINHNGHIEM>> GetExperiences();
        Task<bool> AddExperience(string experienceName);
        Task<bool> EditExperience(int experienceId, string experienceName);
        Task<bool> DeleteExperience(int experienceId);
    }
}
