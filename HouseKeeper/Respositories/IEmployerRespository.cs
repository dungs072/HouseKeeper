using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Employer;
using Microsoft.EntityFrameworkCore;

namespace HouseKeeper.Respositories
{
    public interface IEmployerRespository
    {
        Task<List<HINHTHUCTRALUONG>> GetPaidTypes();
        Task<List<KINHNGHIEM>> GetExperiences();
        Task<List<TINHTHANHPHO>> GetCities();
        Task<List<LOAICONGVIEC>> GetJobs();
        Task<List<GOITIN>> GetPriceTags();
        Task<List<HUYEN>> GetDistricts();
        Task<List<TINTUYENDUNG>> GetOnlineRecruitments();
        Task<HINHTHUCTRALUONG> GetPaidType(int id);
        Task<KINHNGHIEM> GetExperience(int id);
        Task<TINHTHANHPHO> GetCity(int id);
        Task<LOAICONGVIEC> GetJob(int id);
        Task<NGUOITHUE> GetEmployer(int id);
        Task<TINTUYENDUNG> GetRecruitment(int id);
        Task<TRANGTHAITIN> GetRecruitmentStatus(int id);
        Task<GOITIN> GetPricePacket(int id);
        Task<HUYEN> GetDistrict(int id);
        Task<Dictionary<DateTime, List<CHITIETTUCHOI>>> GetRejectionDetails(int recruitmentId);
        Task<bool> CreateRecruitment(TINTUYENDUNG recruitment, string[] jobIds, int pricePacketId);
        Task<ListRecruitmentViewModel> GetEmployerRecruitments(int employerId);
        Task<int> DeleteSpecificRecruitment(int recruitmentId);
        Task<int> GetAmountMoneyForRecruitment(int recruitmentId);
        Task<bool> EditRecruitment(EditRecruitmentViewModel model);
        Task<bool> HideRecruitment(int recruitmentId);
        Task<bool> UnHideRecruitment(int recruitmentId);
        Task<bool> AddBidPrice(int recruitmentId, decimal price);
        Task<bool> ExtendDeadLine(int recruitmentId, int pricePacketId);

        Task<ListCandidatesViewModel> GetSuitableCandidates(int employerId);
        Task<NGUOIGIUPVIEC> GetDetailCandidate(int employeeId);
    }
}
