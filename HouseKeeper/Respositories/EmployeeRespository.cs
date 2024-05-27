using Azure;
using HouseKeeper.DBContext;
using HouseKeeper.Enum;
using HouseKeeper.IServices;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Employee;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace HouseKeeper.Respositories
{
    public class EmployeeRespository:IEmployeeRespository
    {
        private readonly HouseKeeperDBContext dBContext;
        private readonly IFirebaseService _firebaseService;
        private readonly IPasswordService _passwordService;
        private string[] status = new string[] { "Pending approval", "Reject approval",
                                                    "Displayed", "Hidden", "Expired" };
        public EmployeeRespository(HouseKeeperDBContext dBContext, IFirebaseService firebaseService, IPasswordService passwordService)
        {

            this.dBContext = dBContext;
            _firebaseService = firebaseService;
            _passwordService = passwordService;
        }
        public async Task<List<TINTUYENDUNG>> GetRecruitments(int page, string searchKey, int? cityId, int? districtId)
        {
            int pageSize = 5;
            if (page < 1)
            {
                page = 1;
            }

            var recruitments = await dBContext.Recruitments.Where(a => a.Status.StatusName == status[2])
                                                        .OrderByDescending(a=>a.BidPrice).ToListAsync();

            if(!string.IsNullOrEmpty(searchKey))
            {
                recruitments = recruitments.Where(a => a.HouseWorkName.ToLower().Contains(searchKey.Trim().ToLower())).ToList();
                                //.Where(a => a.HouseworkDetails != null && a.HouseworkDetails.Any(b =>
                                //    b.Job != null && b.Job.JobName.Contains(searchKey.Trim())))
                                //.ToList();
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

        public async Task<HUYEN> GetDistrict(int districtId)
        {
            return await dBContext.Districts.FindAsync(districtId);
        }

        public async Task<List<HUYEN>> GetDistricts()
        {
            return await dBContext.Districts.ToListAsync();
        }

        public async Task<TINHTHANHPHO> GetCity(int cityId)
        {
            return await dBContext.Cities.FindAsync(cityId);
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
        public async Task<NGUOITHUE> GetEmployer(int employerId)
        {
            return await dBContext.Employers.FindAsync(employerId);
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

        public async Task<List<NGUOIGIUPVIEC>> GetEmployeesWithIdentityStatus(string q, int status)
        {
            // 1 1
            if (!q.IsNullOrEmpty() && status != 0)
            {
                return await dBContext.Employees.Where(a => a.IdentityState.IdentityStateId == (int)status
                                                        && (a.FirstName.Contains(q)
                                                            || a.LastName.Contains(q)
                                                            || a.Identity.CitizenNumber.Contains(q)
                                                            || a.Account.PhoneNumber.Contains(q)
                                                            || (a.Account.Gmail != null && a.Account.Gmail.Contains(q)))
                                                            ).ToListAsync();

            }
            // 1 0
            if (!q.IsNullOrEmpty() && status == 0)
            {
                return await dBContext.Employees.Where(a => a.FirstName.Contains(q)
                                                            || a.LastName.Contains(q)
                                                            || a.Identity.CitizenNumber.Contains(q)
                                                            || a.Account.PhoneNumber.Contains(q)
                                                            || (a.Account.Gmail != null && a.Account.Gmail.Contains(q))
                                                            ).ToListAsync();
            }

            // 0 1
            if (q.IsNullOrEmpty() && status != 0)
            {
                return await dBContext.Employees.Where(a => a.IdentityState.IdentityStateId == (int)status).ToListAsync();
            }

            // 0 0
            return await dBContext.Employees.ToListAsync();
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
        public async Task<AccountEnum.CreateEditAccountResult> EditEmployeeProfile(EditEmployeeProfileViewModel model, int employeeId, IFormFile avatarImage, IFormFile frontImage, IFormFile backImage, AccountEnum.AccountType accountType)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                var employee = await dBContext.Employees.FindAsync(employeeId);

                if (employee == null)
                {
                    return AccountEnum.CreateEditAccountResult.NotFound;
                }
                var accountWithPhone = await dBContext.Accounts.FirstOrDefaultAsync(a => a.PhoneNumber == model.Employee.Account.PhoneNumber);
                var accountWithGmail = await dBContext.Accounts.FirstOrDefaultAsync(a => a.Gmail == model.Employee.Account.Gmail);
                var identityWithCitizenNumber = await dBContext.Identity.FirstOrDefaultAsync(i => i.CitizenNumber == model.Employee.Identity.CitizenNumber);

                if (accountWithPhone != null && accountWithPhone.AccountID != employee.Account.AccountID)
                {
                    return AccountEnum.CreateEditAccountResult.PhoneDuplicated;
                }

                if (accountWithGmail != null && accountWithGmail.AccountID != employee.Account.AccountID)
                {
                    return AccountEnum.CreateEditAccountResult.GmailDuplicated;
                }

                if (identityWithCitizenNumber != null && identityWithCitizenNumber.CitizenId != employee.Identity.CitizenId)
                {
                    return AccountEnum.CreateEditAccountResult.CitizenNumberDuplicated;
                }


                employee.FirstName = model.Employee.FirstName.Trim();
                employee.LastName = model.Employee.LastName.Trim();
                employee.BirthDate = model.Employee.BirthDate;
                employee.Gender = model.Employee.Gender;
                employee.District = await dBContext.Districts.FindAsync(model.Employee.District.DistrictId);
                if(model.Employee.Address!=null)
                {
                    employee.Address = model.Employee.Address.Trim();
                }
               

                employee.Identity.CitizenNumber = model.Employee.Identity.CitizenNumber;

                var tempfrontImage = await _firebaseService.UploadImage(frontImage, accountType, ImageEnum.ImageType.Identity);
                var tempbackImage = await _firebaseService.UploadImage(backImage, accountType, ImageEnum.ImageType.Identity);
                employee.Identity.FrontImage = (tempfrontImage == null) ? employee.Identity.FrontImage : tempfrontImage;
                employee.Identity.BackImage = (tempbackImage == null) ? employee.Identity.BackImage : tempbackImage;

                var tempAvatar = await _firebaseService.UploadImage(avatarImage, accountType, ImageEnum.ImageType.Avatar);
                employee.Account.AvatarUrl = (tempAvatar == null) ? employee.Account.AvatarUrl : tempAvatar;
                employee.Account.Gmail = model.Employee.Account.Gmail;
                employee.Account.PhoneNumber = model.Employee.Account.PhoneNumber;

                if (employee.IdentityState == null || employee.IdentityState.IdentityStateId == (int)IdentityEnum.IdentiyStatus.Disapprove)
                {
                    employee.IdentityState = await dBContext.IdentityStates.FindAsync((int)IdentityEnum.IdentiyStatus.Waiting);
                }

                dBContext.Employees.Update(employee);

                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return AccountEnum.CreateEditAccountResult.Success;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return AccountEnum.CreateEditAccountResult.ServerError;
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
            for (int i = jobProposalViewModel.recruitments.Count - 1; i >= 0; i--)
            {
                if (jobProposalViewModel.recruitments[i].ApplyDetails.FirstOrDefault(a=>a.Employee.EmployeeId==employeeId)!=null)
                {
                    jobProposalViewModel.recruitments.RemoveAt(i);
                }
            }
            return jobProposalViewModel;

        }
        public async Task<bool> HasRightPassword(string password, int userId)
        {
            var employer = await dBContext.Employees.FindAsync(userId);
            if (employer == null) { return false; }
            return employer.Account.Password.Trim() == _passwordService.HashPassword(password.Trim());
        }

        public async Task<bool> ChangePassword(string password, int userId)
        {
            try
            {
                var employer = await dBContext.Employees.FindAsync(userId);
                var account = employer.Account;
                account.Password = _passwordService.HashPassword(password);
                dBContext.Accounts.Update(account);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public async Task<List<TRANGTHAIDANHTINH>> GetIdentityStatus()
        {
            return await dBContext.IdentityStates.ToListAsync();
        }
        public async Task<TAIKHOAN> GetAccount(int employeeId)
        {
            var employee = await GetEmployee(employeeId);
            return employee.Account;
        }

        public async Task<List<TINTUYENDUNG>> GetRecruitmentsByEmployer(int page, int employerId, RecruitmentEnum.RecruitmentStatus recruitmentStatus)
        {
            int pageSize = 5;
            if (page < 1)
            {
                page = 1;
            }

            var recruitments = await dBContext.Recruitments.Where(a => a.Employer.EmployerId == employerId && a.Status.StatusId == (int)recruitmentStatus).
                                                            OrderByDescending(a => a.BidPrice).ToListAsync();
            recruitments = recruitments.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return recruitments;
        }
    }
}
