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

        #region Paid Type
        public async Task<List<HINHTHUCTRALUONG>> GetPaidTypes()
        {
            return await dBContext.SalaryForms.ToListAsync();
        }
        public async Task<bool> AddPaidType(string paidTypeName)
        {
            try
            {
                HINHTHUCTRALUONG paidType = new HINHTHUCTRALUONG();
                paidType.SalaryFormName = paidTypeName;
                dBContext.SalaryForms.Add(paidType);
                dBContext.SaveChanges();
                return true;


            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> EditPaidType(int paidTypeId, string paidTypeName)
        {
            try
            {
                var paidType = await dBContext.SalaryForms.FindAsync(paidTypeId);
                paidType.SalaryFormName = paidTypeName;
                dBContext.SalaryForms.Update(paidType);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> DeletePaidType(int paidTypeId)
        {
            try
            {
                var paidType = await dBContext.SalaryForms.FindAsync(paidTypeId);
                dBContext.SalaryForms.Remove(paidType);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Experience
        public async Task<List<KINHNGHIEM>> GetExperiences()
        {
            return await dBContext.Experiences.ToListAsync();
        }
        public async Task<bool> AddExperience(string experienceName)
        {
            try
            {
                KINHNGHIEM exp = new KINHNGHIEM();
                exp.ExperienceName = experienceName;
                dBContext.Experiences.Add(exp);
                dBContext.SaveChanges();
                return true;


            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> EditExperience(int experienceId, string experienceName)
        {
            try
            {
                var exp = await dBContext.Experiences.FindAsync(experienceId);
                exp.ExperienceName = experienceName;
                dBContext.Experiences.Update(exp);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> DeleteExperience(int experienceId)
        {
            try
            {
                var exp = await dBContext.Experiences.FindAsync(experienceId);
                dBContext.Experiences.Remove(exp);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
