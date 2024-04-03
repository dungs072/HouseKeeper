using HouseKeeper.DBContext;
using HouseKeeper.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace HouseKeeper.Respositories
{
    public class AdminRespository:IAdminRespository
    {
        private readonly HouseKeeperDBContext dBContext;
        public AdminRespository(HouseKeeperDBContext dBContext)
        {

            this.dBContext = dBContext;
        }
        #region Job type
        public async Task<List<LOAICONGVIEC>> GetJobTypes()
        {
            return await dBContext.Jobs.ToListAsync();
        }
        public async Task<bool> AddJobType(string jobName)
        {
            try
            {
                LOAICONGVIEC job = new LOAICONGVIEC();
                job.JobName = jobName;
                dBContext.Jobs.Add(job);
                dBContext.SaveChanges();
                return true;


            }catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> EditJobType(int jobId,string jobName)
        {
            try
            {
                var jobType = await dBContext.Jobs.FindAsync(jobId);
                jobType.JobName = jobName;
                dBContext.Jobs.Update(jobType);
                dBContext.SaveChanges();
                return true;
            }catch (Exception ex)
            {
                return false;
            }
         
        }
        public async Task<bool> DeleteJobType(int jobId)
        {
            try
            {
                var jobType = await dBContext.Jobs.FindAsync(jobId);
                dBContext.Jobs.Remove(jobType);
                dBContext.SaveChanges();
                return true;
            }catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
