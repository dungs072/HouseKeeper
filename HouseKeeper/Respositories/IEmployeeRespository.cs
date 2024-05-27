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

        Task<HUYEN> GetDistrict(int id);
        Task<List<HUYEN>> GetDistricts();
        Task<TINHTHANHPHO> GetCity(int cityId);
        Task<CHITIETAPPLY> GetApplyDetail(int recruitmentId, int employeeId);
        Task<bool> ApplyJob(int recruitmentId, int employeeId);
        Task<bool> CancelApplyJob(int applyDetailId);
        Task<List<CHITIETAPPLY>> GetApplyRecruitmentList(int employeeId);
        Task<NGUOIGIUPVIEC> GetEmployee(int employeeId);
        Task<List<NGUOIGIUPVIEC>> GetEmployeesWithIdentityStatus(string q, int identiyStatus);
        Task<List<LOAICONGVIEC>> GetJobsForEmployee(int employeeId);
        Task<List<HUYEN>> GetWorkplacesForEmployee(int employeeId);
        Task<bool> AddJob(int jobId, int employeeId);
        Task<bool> DeleteJob(int jobId, int employeeId);
        Task<bool> AddDistrict(int districtId, int employeeId);
        Task<bool> DeleteDistrict(int districtId, int employeeId);
        Task<AccountEnum.CreateEditAccountResult> EditEmployeeProfile(EditEmployeeProfileViewModel model, int employeeId, IFormFile avatarImage, IFormFile frontImage, IFormFile backImage, AccountEnum.AccountType accountType);
        Task<JobProposalViewModel> GetJobProposals(int employeeId);
        Task<bool> HasRightPassword(string password, int userId);
        Task<bool> ChangePassword(string password, int userId);
        Task<List<TRANGTHAIDANHTINH>> GetIdentityStatus();
        Task<TAIKHOAN> GetAccount(int employeeId);
        Task<NGUOITHUE> GetEmployer(int employerId);
        Task<List<TINTUYENDUNG>> GetRecruitmentsByEmployer(int page, int employerId, RecruitmentEnum.RecruitmentStatus recruitmentStatus);
    }
}
