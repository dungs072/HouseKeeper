using HouseKeeper.DBContext;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.OutPage;
using HouseKeeper.Models.Views.Recruitments;
using Microsoft.EntityFrameworkCore;

namespace HouseKeeper.Respositories
{
    public class EmployerRespository:IEmployerRespository
    {
        private readonly HouseKeeperDBContext dBContext;
        private string[] status = new string[] { "Pending approval", "Reject approval", 
                                                    "Displayed", "Hidden", "Expired" };
        public EmployerRespository(HouseKeeperDBContext dBContext)
        {

            this.dBContext = dBContext;
        }
        public async Task<List<HINHTHUCTRALUONG>> GetPaidTypes()
        {
            return await dBContext.SalaryForms.ToListAsync();
        }
        public async Task<List<KINHNGHIEM>> GetExperiences()
        {
            return await dBContext.Experiences.ToListAsync();
        }
        public async Task<List<TINHTHANHPHO>> GetCities()
        {
            return await dBContext.Cities.ToListAsync();
        }
        public async Task<List<LOAICONGVIEC>> GetJobs()
        {
            return await dBContext.Jobs.ToListAsync();
        }
        public async Task<List<GIAGOITIN>> GetPriceTags()
        {
            return await dBContext.PricePackets.ToListAsync();
        }
        public async Task<HINHTHUCTRALUONG> GetPaidType(int id)
        {
            return await dBContext.SalaryForms.FindAsync(id);
        }
        public async Task<KINHNGHIEM> GetExperience(int id)
        {
            return await dBContext.Experiences.FindAsync(id);
        }
        public async Task<TINHTHANHPHO> GetCity(int id)
        {
            return await dBContext.Cities.FindAsync(id);
        }
        public async Task<LOAICONGVIEC> GetJob(int id)
        {
            return await dBContext.Jobs.FindAsync(id);
        }
        public async Task<NGUOITHUE> GetEmployer(int id)
        {
            return await dBContext.Employers.FindAsync(id);
        }
        public async Task<GIAGOITIN> GetPricePacket(int id)
        {
            return await dBContext.PricePackets.FindAsync(id);
        }
        public async Task<bool>CreateRecruitment(TINTUYENDUNG recruitment, string[] jobIds, int pricePacketId)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                recruitment.Status = status[0];
           
                await dBContext.Recruitments.AddAsync(recruitment);
                List<CHITIETLOAIGIUPVIEC> housekeepingTypes = new List<CHITIETLOAIGIUPVIEC>();
                CHITIETGIAGOITIN packetDetail = new CHITIETGIAGOITIN();
                packetDetail.Recruitment = recruitment;
                packetDetail.PricePacket = await GetPricePacket(pricePacketId);
                packetDetail.BuyDate = DateTime.Now;
                await dBContext.PricePacketDetails.AddAsync(packetDetail);
                foreach(var jobId in jobIds)
                {
                    var houseKeepingType = new CHITIETLOAIGIUPVIEC();
                    houseKeepingType.Recruitment = recruitment;
                    houseKeepingType.Job = await GetJob(int.Parse(jobId));
                    housekeepingTypes.Add(houseKeepingType);
                }
                await dBContext.HouseWorkDetails.AddRangeAsync(housekeepingTypes);
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
        
        public async Task<ListRecruitmentViewModel> GetEmployerRecruitments(int employerId)
        {
            ListRecruitmentViewModel model = new ListRecruitmentViewModel();
            var recruitments = await dBContext.Recruitments.Where(a=>a.Employer.EmployerId==employerId).ToListAsync();
            model.OnlineRecruitments = new List<TINTUYENDUNG>();
            model.PendingApprovalRecruitments = new List<TINTUYENDUNG>();
            model.OutDatedRecruitments = new List<TINTUYENDUNG>();
            model.HiddenRecruitments = new List<TINTUYENDUNG>();
            model.DisapprovalRecruitments = new List<TINTUYENDUNG>();
            foreach(var recruitment in recruitments)
            {
                if (recruitment.Status == status[0])
                {
                    model.PendingApprovalRecruitments.Add(recruitment);
                }
                else if (recruitment.Status == status[1])
                {
                    model.DisapprovalRecruitments.Add(recruitment);
                }
                else if (recruitment.Status == status[2])
                {
                    model.OnlineRecruitments.Add(recruitment);
                }
                else if (recruitment.Status == status[3])
                {
                    model.HiddenRecruitments.Add(recruitment);
                }
                else if (recruitment.Status == status[4])
                {
                    model.OutDatedRecruitments.Add(recruitment);
                }
            }

            return model;
        }
    }
}
