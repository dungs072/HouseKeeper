using HouseKeeper.Enum;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Employee;

namespace HouseKeeper.Respositories
{
    public interface IEmployeeRespository
    {
        Task<List<TINTUYENDUNG>> GetRecruitments(int page, string searchKey, int? cityId, int? districtId);
        Task<TINTUYENDUNG> GetRecruitment(int id);
        Task<List<TINHTHANHPHO>> GetCities();
        Task<List<HUYEN>> GetDistricts();
        Task<CHITIETAPPLY> GetApplyDetail(int recruitmentId, int employeeId);
        Task<bool> ApplyJob(int recruitmentId, int employeeId);
        Task<bool> CancelApplyJob(int applyDetailId);
        Task<List<CHITIETAPPLY>> GetApplyRecruitmentList(int employeeId);
        Task<NGUOIGIUPVIEC> GetEmployee(int employeeId);
        Task<List<LOAICONGVIEC>> GetJobsForEmployee(int employeeId);
        Task<List<HUYEN>> GetWorkplacesForEmployee(int employeeId);
        Task<bool> AddJob(int jobId, int employeeId);
        Task<bool> DeleteJob(int jobId, int employeeId);
        Task<bool> AddDistrict(int districtId, int employeeId);
        Task<bool> DeleteDistrict(int districtId, int employeeId);
        Task<bool> EditEmployeeProfile(EditEmployeeProfileViewModel model, int employeeId, IFormFile avatarImage, IFormFile frontImage, IFormFile backImage, AccountEnum.AccountType accountType);

    }
}
