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
        Task<List<GIAGOITIN>> GetPriceTags();
        Task<List<HUYEN>> GetDistricts();
        Task<List<TINTUYENDUNG>> GetOnlineRecruitments();
        Task<HINHTHUCTRALUONG> GetPaidType(int id);
        Task<KINHNGHIEM> GetExperience(int id);
        Task<TINHTHANHPHO> GetCity(int id);
        Task<LOAICONGVIEC> GetJob(int id);
        Task<NGUOITHUE> GetEmployer(int id);
        Task<TINTUYENDUNG> GetRecruitment(int id);
        Task<TRANGTHAITIN> GetRecruitmentStatus(int id);
        Task<GIAGOITIN> GetPricePacket(int id);
        Task<HUYEN> GetDistrict(int id);
        Task<bool> CreateRecruitment(TINTUYENDUNG recruitment, string[] jobIds, int pricePacketId);
        Task<ListRecruitmentViewModel> GetEmployerRecruitments(int employerId);
        Task<bool> DeleteSpecificRecruitment(int recruitmentId);
        Task<bool> EditRecruitment(EditRecruitmentViewModel model);
    }
}
