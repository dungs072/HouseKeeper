using HouseKeeper.Enum.Staff;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Staff;

namespace HouseKeeper.Respositories
{
    public interface IStaffRespository
    {
        Task<List<Models.DB.TINTUYENDUNG>> GetRecruitmentNotHandled();
        Task<bool> AcceptRecruitment(int recruitmentId);

        Task<Tuple<EnumStaff.ModerationStatus,TINTUYENDUNG>> GetRecruitment(int recruitmentId, int staffId);

        Task<List<LYDOTUCHOI>> GetRejections();

        Task<bool> RejectRecruitment(RecruitmentModerationViewModel model);
        Task<List<TINTUYENDUNG>> ListRecruitmentAreHandledByStaff(int staffId);
    }
}
