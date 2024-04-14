using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Staff;

namespace HouseKeeper.Respositories
{
    public interface IStaffRespository
    {
        Task<List<Models.DB.TINTUYENDUNG>> GetRecruitmentsPendingApproval();
        Task<bool> AcceptRecruitment(int recruitmentId);

        Task<TINTUYENDUNG> GetRecruitment(int recruitmentId);

        Task<List<LYDOTUCHOI>> GetRejections();

        Task<bool> RejectRecruitment(RecruitmentModerationViewModel model);
    }
}
