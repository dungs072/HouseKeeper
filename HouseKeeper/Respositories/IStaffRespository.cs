using HouseKeeper.Enum.Staff;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Staff;

namespace HouseKeeper.Respositories
{
    public interface IStaffRespository
    {
        Task<List<Models.DB.TINTUYENDUNG>> GetRecruitmentNotHandled();
        Task<EnumStaff.ModerationStatus> AcceptRecruitment(int recruitmentId);

        Task<Tuple<EnumStaff.ModerationStatus, TINTUYENDUNG>> GetRecruitment(int recruitmentId, int staffId);

        Task<List<LYDOTUCHOI>> GetRejections();

        Task<EnumStaff.ModerationStatus> RejectRecruitment(RecruitmentModerationViewModel model);
        Task<List<TINTUYENDUNG>> ListRecruitmentAreHandledByStaff(int staffId);
        Task<List<CHITIETTUCHOI>?> GetRejectionsDetail(int recruitmentId);
    }
}
