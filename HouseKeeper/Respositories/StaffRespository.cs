using HouseKeeper.Enum;
using HouseKeeper.IServices;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Staff;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HouseKeeper.Respositories
{
    public class StaffRespository : IStaffRespository
    {
        private readonly DBContext.HouseKeeperDBContext dBContext;
        public StaffRespository(DBContext.HouseKeeperDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        // Get all recruitment is pending approval
        public async Task<List<Models.DB.TINTUYENDUNG>> GetRecruitmentNotHandled()
        {
            return await dBContext.Recruitments.Where(a => a.Status.StatusId == (int)RecruitmentEnum.RecruitmentStatus.PendingApproval && a.Staff == null).ToListAsync();
        }

        // Get all recruitment are handled by staff
        public async Task<List<Models.DB.TINTUYENDUNG>> ListRecruitmentAreHandledByStaff(int staffId, RecruitmentEnum.RecruitmentStatus recruitmentStatus)
        {
            return await dBContext.Recruitments.Where(a => a.Status.StatusId == (int)recruitmentStatus && a.Staff != null && a.Staff.StaffId == staffId).ToListAsync();
        }

        // Accept recruitment by id and change status to Displayed
        public async Task<StaffEnum.ModerationStatus> AcceptRecruitment(int recruitmentId)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();

            try
            {
                var recruitment = await dBContext.Recruitments
                    .Include(r => r.Status)  // Ensure that the Status property is loaded
                    .FirstOrDefaultAsync(r => r.RecruitmentId == recruitmentId);

                if (recruitment == null)
                {
                    // Recruitment not found, or Status not loaded
                    transaction.Rollback();
                    return StaffEnum.ModerationStatus.NotFound;
                }

                // Associate the new Status object with the Recruitment object
                recruitment.Status = await dBContext.RecruitmentStatus.FindAsync((int)RecruitmentEnum.RecruitmentStatus.Displayed);
                var PricePacketList = await dBContext.PricePacketDetails.Where(x => x.BuyDate >= recruitment.PostTime && x.Recruitment.RecruitmentId == recruitmentId && x.HasPaid == true).ToListAsync();
                var currentTime = DateTime.Now;
                recruitment.RecruitDeadlineDate = currentTime;
                foreach (var pricePacket in PricePacketList)
                {
                    recruitment.RecruitDeadlineDate = recruitment.RecruitDeadlineDate.AddDays(pricePacket.PricePacket.NumberDays);
                }
                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return StaffEnum.ModerationStatus.OK;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StaffEnum.ModerationStatus.ServerError;
            }
        }


        // reject recruitment by id and change status to Rejected and add list reason
        [HttpPost]
        public async Task<StaffEnum.ModerationStatus> RejectRecruitment(RecruitmentModerationViewModel model)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                var recruitment = await dBContext.Recruitments
                    .Include(r => r.Status)  // Ensure that the Status property is loaded
                    .FirstOrDefaultAsync(r => r.RecruitmentId == model.RecruitmentId);

                if (recruitment == null)
                {
                    transaction.Rollback();
                    return StaffEnum.ModerationStatus.NotFound;
                }

                recruitment.Status = await dBContext.RecruitmentStatus.FindAsync((int)RecruitmentEnum.RecruitmentStatus.RejectApproval);
                var currentTime = DateTime.Now;
                for (int i = 0; i < model.RejectionId.Count; i++)
                {
                    if (model.IsSelectedList[i] == false)
                    {
                        continue;
                    }
                    var rejectionDetail = new CHITIETTUCHOI();
                    rejectionDetail.Recruitment = recruitment;
                    rejectionDetail.Rejection = await dBContext.Rejections.FindAsync(model.RejectionId[i]);
                    rejectionDetail.Time = currentTime;
                    rejectionDetail.Note = model.NoteList[i];
                    await dBContext.RejectionDetails.AddAsync(rejectionDetail);
                }
                dBContext.Recruitments.Update(recruitment);
                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return StaffEnum.ModerationStatus.OK;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StaffEnum.ModerationStatus.ServerError;
            }
        }

        // Get recruitment by id
        public async Task<Tuple<StaffEnum.ModerationStatus, TINTUYENDUNG>> GetRecruitment(int recruitmentId, int staffId)
        {
            var recruitment = await dBContext.Recruitments.FindAsync(recruitmentId);
            if (recruitment == null)
            {
                return new Tuple<StaffEnum.ModerationStatus, TINTUYENDUNG>(StaffEnum.ModerationStatus.NotFound, null);
            }
            if (recruitment.Staff != null && recruitment.Staff.StaffId != staffId)
            {
                return new Tuple<StaffEnum.ModerationStatus, TINTUYENDUNG>(StaffEnum.ModerationStatus.IsHandledByOther, null);
            }
            if (recruitment.Staff == null)
            {
                using var transaction = await dBContext.Database.BeginTransactionAsync();
                try
                {
                    recruitment.Staff = await dBContext.Staffs.FindAsync(staffId);
                    dBContext.Recruitments.Update(recruitment);
                    await dBContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Tuple<StaffEnum.ModerationStatus, TINTUYENDUNG>(StaffEnum.ModerationStatus.ServerError, recruitment);
                }
            }
            return new Tuple<StaffEnum.ModerationStatus, TINTUYENDUNG>(StaffEnum.ModerationStatus.OK, recruitment);
        }

        // Get all LYDOTUCHOI
        public async Task<List<LYDOTUCHOI>> GetRejections()
        {
            return await dBContext.Rejections.ToListAsync();
        }

        // get CHITIETTUCHOI table
        public async Task<List<CHITIETTUCHOI>> GetRejectionDetails()
        {
            return await dBContext.RejectionDetails.ToListAsync();
        }

        // get LYDOTUCHOI by id
        public async Task<LYDOTUCHOI> GetRejection(int rejectionId)
        {
            return await dBContext.Rejections.FindAsync(rejectionId);
        }

        // get CHITIETTUCHOI by recruitmentId
        public async Task<List<CHITIETTUCHOI>?> GetRejectionsDetail(int recruitmentId)
        {
            return await dBContext.RejectionDetails.Where(a => a.Recruitment.RecruitmentId == recruitmentId).ToListAsync();
        }

        // Edit note of rejection
        public async Task<StaffEnum.ModerationStatus> EditNotesOfRejection(RecruitmentModerationViewModel model)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                var recruitment = await dBContext.Recruitments
                    .Include(r => r.Status)  // Ensure that the Status property is loaded
                    .FirstOrDefaultAsync(r => r.RecruitmentId == model.RecruitmentId);
                if (recruitment == null)
                {
                    return StaffEnum.ModerationStatus.NotFound;

                }
                for (int i = 0; i < model.NoteIdCanEditList.Count; i++)
                {
                    var rejectionDetail = await dBContext.RejectionDetails.FindAsync(model.NoteIdCanEditList[i]);
                    rejectionDetail.Note = model.NoteCanEditList[i];
                }
                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return StaffEnum.ModerationStatus.OK;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StaffEnum.ModerationStatus.ServerError;
            }
        }

        // get staff profile by id
        public async Task<NHANVIEN> GetStaffProfile(int staffId)
        {
            return await dBContext.Staffs.FindAsync(staffId);
        }

        public async Task<bool> HasRightPassword(string password, int userId)
        {
            var staff = await dBContext.Staffs.FindAsync(userId);
            if (staff == null) { return false; }
            return staff.Account.Password.Trim() == password.Trim();
        }

        public async Task<bool> ChangePassword(string password, int userId)
        {
            try
            {
                var staff = await dBContext.Staffs.FindAsync(userId);
                var account = staff.Account;
                account.Password = password;
                dBContext.Accounts.Update(account);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
    }
}
