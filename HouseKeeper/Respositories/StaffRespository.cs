using HouseKeeper.Contanst;
using HouseKeeper.Enum.Staff;
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
            return await dBContext.Recruitments.Where(a => a.Status.StatusName == RecruitmentsStatus.PendingApproval && a.Staff == null).ToListAsync();
        }

        // Get all recruitment are handled by staff
        public async Task<List<Models.DB.TINTUYENDUNG>> ListRecruitmentAreHandledByStaff(int staffId)
        {
            return await dBContext.Recruitments.Where(a => a.Status.StatusName == RecruitmentsStatus.PendingApproval && a.Staff != null && a.Staff.StaffId == staffId).ToListAsync();
        }

        // Accept recruitment by id and change status to Displayed
        public async Task<EnumStaff.ModerationStatus> AcceptRecruitment(int recruitmentId)
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
                    return EnumStaff.ModerationStatus.NotFound;
                }

                // Associate the new Status object with the Recruitment object
                recruitment.Status = await dBContext.RecruitmentStatus.FindAsync(RecruitmentsStatus.GetStatusId(RecruitmentsStatus.Displayed));

                dBContext.Recruitments.Update(recruitment);
                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return EnumStaff.ModerationStatus.OK;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return EnumStaff.ModerationStatus.ServerError;
            }
        }


        // reject recruitment by id and change status to Rejected and add list reason
        [HttpPost]
        public async Task<EnumStaff.ModerationStatus> RejectRecruitment(RecruitmentModerationViewModel model)
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
                    return EnumStaff.ModerationStatus.NotFound;
                }

                recruitment.Status = await dBContext.RecruitmentStatus.FindAsync(RecruitmentsStatus.GetStatusId(RecruitmentsStatus.RejectApproval));
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
                return EnumStaff.ModerationStatus.OK;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return EnumStaff.ModerationStatus.ServerError;
            }
        }

        // Get recruitment by id
        public async Task<Tuple<EnumStaff.ModerationStatus, TINTUYENDUNG>> GetRecruitment(int recruitmentId, int staffId)
        {
            var recruitment = await dBContext.Recruitments.FindAsync(recruitmentId);
            if (recruitment == null)
            {
                return new Tuple<EnumStaff.ModerationStatus, TINTUYENDUNG>(EnumStaff.ModerationStatus.NotFound, null);
            }
            if(recruitment.Staff != null && recruitment.Staff.StaffId != staffId)
            {
                return new Tuple<EnumStaff.ModerationStatus, TINTUYENDUNG>(EnumStaff.ModerationStatus.IsHandledByOther, null);
            }
            if(recruitment.Staff == null)
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
                    return new Tuple<EnumStaff.ModerationStatus, TINTUYENDUNG>(EnumStaff.ModerationStatus.ServerError, recruitment);
                }
            }
            return new Tuple<EnumStaff.ModerationStatus, TINTUYENDUNG>(EnumStaff.ModerationStatus.OK, recruitment);
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
    }
}
