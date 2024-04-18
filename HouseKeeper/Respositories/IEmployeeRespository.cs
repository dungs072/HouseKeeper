using HouseKeeper.Models.DB;

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
    }
}
