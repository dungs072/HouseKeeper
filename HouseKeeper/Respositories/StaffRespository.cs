using HouseKeeper.Contanst;
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
        public async Task<List<Models.DB.TINTUYENDUNG>> GetRecruitmentsPendingApproval()
        {
            return await dBContext.Recruitments.Where(a => a.Status.StatusName == RecruitmentsStatus.PendingApproval).ToListAsync();
        }

        // Accept recruitment by id and change status to Displayed
        public async Task<bool> AcceptRecruitment(int recruitmentId)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();

            try
            {
                var recruitment = await dBContext.Recruitments
                    .Include(r => r.Status)  // Ensure that the Status property is loaded
                    .FirstOrDefaultAsync(r => r.RecruitmentId == recruitmentId);

                if (recruitment == null || recruitment.Status == null)
                {
                    // Recruitment not found, or Status not loaded
                    transaction.Rollback();
                    return false;
                }


                // Associate the new Status object with the Recruitment object
                recruitment.Status = await dBContext.RecruitmentStatus.FindAsync(RecruitmentsStatus.GetStatusId(RecruitmentsStatus.Displayed));

                dBContext.Recruitments.Update(recruitment);
                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }


        // reject recruitment by id and change status to Rejected and add list reason
        [HttpPost]
        public async Task<bool> RejectRecruitment(RecruitmentModerationViewModel model)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                var recruitment = await dBContext.Recruitments
                    .Include(r => r.Status)  // Ensure that the Status property is loaded
                    .FirstOrDefaultAsync(r => r.RecruitmentId == model.RecruitmentId);
                recruitment.Staff = await dBContext.Staffs.FindAsync(model.StaffId);
                recruitment.Status = await dBContext.RecruitmentStatus.FindAsync(RecruitmentsStatus.GetStatusId(RecruitmentsStatus.RejectApproval));
                for (int i = 0; i < model.RejectionId.Count; i++)
                {
                    if (model.IsSelectedList[i] == false)
                    {
                        continue;
                    }
                    var rejectionDetail = new CHITIETTUCHOI();
                    rejectionDetail.Recruitment = recruitment;
                    rejectionDetail.Rejection = await dBContext.Rejections.FindAsync(model.RejectionId[i]);
                    rejectionDetail.Time = DateTime.Now;
                    rejectionDetail.Note = model.NoteList[i];
                    await dBContext.RejectionDetails.AddAsync(rejectionDetail);
                }
                dBContext.Recruitments.Update(recruitment);
                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }



        // Get recruitment by id
        public async Task<TINTUYENDUNG> GetRecruitment(int recruitmentId)
        {
            return await dBContext.Recruitments.FindAsync(recruitmentId);
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
    }
}
