﻿using HouseKeeper.Enum;
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
        Task<bool> HasRightPassword(string password, int userId);
        Task<bool> ChangePassword(string password, int userId);
        Task<List<NGUOITHUE>> GetEmployers(string q);
        Task<bool> ApproveEmployer(int employerId, int staffId);
        Task<bool> DisapproveEmployer(EmployerDetailViewModel model, int staffId);
        Task<bool> ApproveEmployee(int employeeId, int staffId);
        Task<bool> DisapproveEmployee(EmployeeDetailViewModel employeeId, int staffId);
    }
}
