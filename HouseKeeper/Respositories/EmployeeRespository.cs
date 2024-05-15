using HouseKeeper.DBContext;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Employee;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HouseKeeper.Respositories
{
    public class EmployeeRespository:IEmployeeRespository
    {
        private readonly HouseKeeperDBContext dBContext;
        private string[] status = new string[] { "Pending approval", "Reject approval",
                                                    "Displayed", "Hidden", "Expired" };
        public EmployeeRespository(HouseKeeperDBContext dBContext)
        {

            this.dBContext = dBContext;
        }
        public async Task<List<TINTUYENDUNG>> GetRecruitments(int page, string searchKey, int? cityId, int? districtId)
        {
            int pageSize = 5;
            if (page < 1)
            {
                page = 1;
            }

            var recruitments = await dBContext.Recruitments.Where(a => a.Status.StatusName == status[2]).OrderByDescending(a=>a.BidPrice).ToListAsync();

            if(!string.IsNullOrEmpty(searchKey))
            {
                recruitments = recruitments
                                .Where(a => a.HouseworkDetails != null && a.HouseworkDetails.Any(b =>
                                    b.Job != null && searchKey.Contains(b.Job.JobName)))
                                .ToList();
                if (cityId!=null)
                {
                    recruitments = recruitments.Where(a=>a.District.City.CityId==cityId).ToList();
                    if(districtId!=null)
                    {
                        recruitments = recruitments.Where(a=>a.District.DistrictId==districtId).ToList();
                    }
                }
            }
            else
            {
                if (cityId != null)
                {
                    recruitments = recruitments.Where(a => a.District.City.CityId == cityId).ToList();
                    if (districtId != null)
                    {
                        recruitments = recruitments.Where(a => a.District.DistrictId == districtId).ToList();
                    }
                }
            }
            recruitments = recruitments.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return recruitments;
        }
        public async Task<TINTUYENDUNG> GetRecruitment(int id)
        {
            return await dBContext.Recruitments.FindAsync(id);
        }
        public async Task<List<TINHTHANHPHO>> GetCities()
        {
            return await dBContext.Cities.ToListAsync();
        }
        public async Task<List<HUYEN>> GetDistricts()
        {
            return await dBContext.Districts.ToListAsync();
        }
        public async Task<CHITIETAPPLY> GetApplyDetail(int recruitmentId, int employeeId)
        {
            var t =  await dBContext.ApplyDetails.Where(a => a.Recruitment.RecruitmentId == recruitmentId && a.Employee.EmployeeId == employeeId).ToListAsync();
            if(t.Count > 0)
            {
                return t[0];
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> ApplyJob(int recruitmentId, int employeeId)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                CHITIETAPPLY applyDetail = new CHITIETAPPLY();
                var recruitment = await dBContext.Recruitments.FindAsync(recruitmentId);
                var employee = await dBContext.Employees.FindAsync(employeeId);
                applyDetail.Recruitment = recruitment;
                applyDetail.Employee = employee;
                applyDetail.Time = DateTime.Now;
                await dBContext.AddAsync(applyDetail);
                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return true;
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }
        public async Task<bool> CancelApplyJob(int applyDetailId)
        {
            try
            {
                var applyDetail = await dBContext.ApplyDetails.FindAsync(applyDetailId);
                dBContext.ApplyDetails.Remove(applyDetail);
                await dBContext.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
        public async Task<List<CHITIETAPPLY>> GetApplyRecruitmentList(int employeeId)
        {
            return await dBContext.ApplyDetails.Where(a => a.Employee.EmployeeId == employeeId).
                                                    OrderByDescending(a => a.Time).ToListAsync();
        }
        public async Task<NGUOIGIUPVIEC> GetEmployee(int employeeId)
        {
            return await dBContext.Employees.FindAsync(employeeId);
        }
        public async Task<List<LOAICONGVIEC>> GetJobsForEmployee(int employeeId)
        {
            var jobs = await dBContext.Jobs.ToListAsync();
            var jobDetails = await dBContext.JobDetails.Where(a => a.Employee.EmployeeId == employeeId).ToListAsync();
            foreach (var jobDetail in jobDetails)
            {
                jobs.Remove(jobDetail.Job);
            }
            return jobs;
        }
        public async Task<List<HUYEN>> GetWorkplacesForEmployee(int employeeId)
        {
            var workPlaces = await dBContext.Districts.ToListAsync();
            var workPlacesDetails = await dBContext.WorkplacesDetails.Where(a => a.Employee.EmployeeId == employeeId).ToListAsync();
            foreach(var workPlaceDetail in workPlacesDetails)
            {
                workPlaces.Remove(workPlaceDetail.District);
            }
            return workPlaces;
        }

        public async Task<bool> AddJob(int jobId, int employeeId)
        {
            try
            {
                var jobDetail = new CHITIETCONGVIEC();
                jobDetail.Job = await dBContext.Jobs.FindAsync(jobId);
                jobDetail.Employee = await dBContext.Employees.FindAsync(employeeId);
                await dBContext.AddAsync(jobDetail);
                await dBContext.SaveChangesAsync();
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }
        public async Task<bool> DeleteJob(int jobId, int employeeId)
        {
            try
            {
                var jobDetail = await dBContext.JobDetails.Where(a => a.Job.JobId == jobId && a.Employee.EmployeeId == employeeId).
                                ToListAsync();
                if (jobDetail.Count == 0) { return false; }
                dBContext.JobDetails.Remove(jobDetail[0]);
                await dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<bool> AddDistrict(int districtId, int employeeId)
        {
            try
            {
                var workplaceDetail = new CHITIETNOICOTHELAMVIEC();
                workplaceDetail.District = await dBContext.Districts.FindAsync(districtId);
                workplaceDetail.Employee = await dBContext.Employees.FindAsync(employeeId);
                await dBContext.AddAsync(workplaceDetail);
                await dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<bool> DeleteDistrict(int districtId, int employeeId)
        {
            try
            {
                var districtDetail = await dBContext.WorkplacesDetails.Where(a => a.District.DistrictId == districtId && a.Employee.EmployeeId == employeeId).
                                ToListAsync();
                if (districtDetail.Count == 0) { return false; }
                dBContext.WorkplacesDetails.Remove(districtDetail[0]);
                await dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<JobProposalViewModel> GetJobProposals(int employeeId)
        {
            var jobDetails = await dBContext.JobDetails.Where(a => a.Employee.EmployeeId== employeeId).ToListAsync();
            var workableDetails = await dBContext.WorkplacesDetails.Where(a=>a.Employee.EmployeeId==employeeId).ToListAsync();
            JobProposalViewModel jobProposalViewModel = new JobProposalViewModel();
            jobProposalViewModel.recruitments = new List<TINTUYENDUNG>();
            var houseWorkDetails = await dBContext.HouseWorkDetails.Where(a => a.Recruitment.Status.StatusName == status[2]).ToListAsync();
            foreach(var houseWorkDetail in houseWorkDetails)
            {
                foreach(var jobDetail in jobDetails)
                {
                    if(jobDetail.Job==houseWorkDetail.Job)
                    {
                        if(!jobProposalViewModel.recruitments.Contains(houseWorkDetail.Recruitment))
                        {
                            jobProposalViewModel.recruitments.Add(houseWorkDetail.Recruitment);
                            
                        }
                        break;
                    }
                }
               
            }
            for(int i =jobProposalViewModel.recruitments.Count-1;i>=0;i--)
            {
                bool flag = false;
                foreach (var workableDetail in workableDetails)
                {
                    if (workableDetail.District.City == jobProposalViewModel.recruitments[i].District.City)
                    {
                        flag = true;
                    }
                }
                if(!flag)
                {
                    jobProposalViewModel.recruitments.RemoveAt(i);
                }
            }
            return jobProposalViewModel;

        }

    }
}
