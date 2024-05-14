using HouseKeeper.Enum;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Staff;

namespace HouseKeeper.Respositories
{
    public interface IStaffRespository
    {
        Task<List<Models.DB.TINTUYENDUNG>> GetRecruitmentNotHandled();
        Task<StaffEnum.ModerationStatus> AcceptRecruitment(int recruitmentId);
        Task<Tuple<StaffEnum.ModerationStatus, TINTUYENDUNG>> GetRecruitment(int recruitmentId, int staffId);
        Task<List<LYDOTUCHOI>> GetRejections();
        Task<StaffEnum.ModerationStatus> RejectRecruitment(RecruitmentModerationViewModel model);
        Task<List<TINTUYENDUNG>> ListRecruitmentAreHandledByStaff(int staffId, RecruitmentEnum.RecruitmentStatus recruitmentStatus);
        Task<List<CHITIETTUCHOI>?> GetRejectionsDetail(int recruitmentId);
        Task<StaffEnum.ModerationStatus> EditNotesOfRejection(RecruitmentModerationViewModel model);
        Task<NHANVIEN> GetStaffProfile(int staffId);
    }
}
