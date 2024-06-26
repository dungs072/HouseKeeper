﻿using HouseKeeper.Configs;
using HouseKeeper.DBContext;
using HouseKeeper.Enum;
using HouseKeeper.IServices;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Admin;
using HouseKeeper.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace HouseKeeper.Respositories
{
    public class AdminRespository : IAdminRespository
    {
        private readonly HouseKeeperDBContext dBContext;
        private readonly IFirebaseService firebaseService;
        private readonly IPasswordService passwordService;
        public AdminRespository(HouseKeeperDBContext dBContext, IFirebaseService firebaseService, IPasswordService passwordService)
        {

            this.dBContext = dBContext;
            this.firebaseService = firebaseService;
            this.passwordService = passwordService;
        }
        #region Job type
        public async Task<List<LOAICONGVIEC>> GetJobTypes(string search="")
        {
            if(!search.IsNullOrEmpty())
            {
                return await dBContext.Jobs.Where(a=>a.JobName.Contains(search))
                                    .OrderBy(job => job.JobName).ToListAsync();
            }
            return await dBContext.Jobs.OrderBy(job => job.JobName).ToListAsync();
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


            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> EditJobType(int jobId, string jobName)
        {
            try
            {
                var jobType = await dBContext.Jobs.FindAsync(jobId);
                jobType.JobName = jobName;
                dBContext.Jobs.Update(jobType);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region City&District
        public async Task<List<TINHTHANHPHO>> GetCities(string search="")
        {
            if(!search.IsNullOrEmpty())
            {
                return await dBContext.Cities.Where(a=>a.CityName.Contains(search))
                                            .OrderBy(a => a.CityName).ToListAsync();
            }
            return await dBContext.Cities.OrderBy(a=>a.CityName).ToListAsync();
        }

        public async Task<bool> AddCity(string cityName)
        {
            try
            {
                TINHTHANHPHO city = new TINHTHANHPHO();
                city.CityName = cityName;
                await dBContext.Cities.AddAsync(city);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> EditCity(int cityId, string cityName)
        {
            try
            {
                var city = await dBContext.Cities.FindAsync(cityId);
                city.CityName = cityName;
                dBContext.Cities.Update(city);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> DeleteCity(int cityId)
        {
            try
            {
                var city = await dBContext.Cities.FindAsync(cityId);
                dBContext.Cities.Remove(city);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<HUYEN>> GetDistricts(int cityId,string search="")
        {
            if(!search.IsNullOrEmpty())
            {
                return await dBContext.Districts.Where(a => a.City.CityId == cityId&&a.DistrictName.Contains(search))
                                                .OrderBy(a => a.DistrictName).ToListAsync();
            }
            return await dBContext.Districts.Where(a => a.City.CityId == cityId).OrderBy(a=>a.DistrictName).ToListAsync();
        }
        public async Task<bool> AddDistrict(string districtName, int cityId)
        {
            try
            {
                TINHTHANHPHO city = await dBContext.Cities.FindAsync(cityId);
                HUYEN district = new HUYEN();
                district.DistrictName = districtName;
                district.City = city;
                await dBContext.Districts.AddAsync(district);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> EditDistrict(int districtId, string districtName)
        {
            try
            {
                var district = await dBContext.Districts.FindAsync(districtId);
                district.DistrictName = districtName;
                dBContext.Districts.Update(district);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> DeleteDistrict(int districtId)
        {
            try
            {
                var district = await dBContext.Districts.FindAsync(districtId);
                dBContext.Districts.Remove(district);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Paid Type
        public async Task<List<HINHTHUCTRALUONG>> GetPaidTypes(string search="")
        {
            if(!search.IsNullOrEmpty())
            {
                return await dBContext.SalaryForms.Where(a=>a.SalaryFormName.Contains(search))
                                                .OrderBy(a => a.SalaryFormName).ToListAsync();
            }
            return await dBContext.SalaryForms.OrderBy(a=>a.SalaryFormName).ToListAsync();
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
        public async Task<List<KINHNGHIEM>> GetExperiences(string search="")
        {
            if(!search.IsNullOrEmpty())
            {
                return await dBContext.Experiences.Where(a=>a.ExperienceName.Contains(search))
                                                .OrderBy(a => a.ExperienceName).ToListAsync();
            }
            return await dBContext.Experiences.OrderBy(a=>a.ExperienceName).ToListAsync();
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

        #region Rejection
        public async Task<List<LYDOTUCHOI>> GetRejections(string search="")
        {
            if(!search.IsNullOrEmpty())
            {
                return await dBContext.Rejections.Where(a=>a.RejectionName.Contains(search))
                                                .OrderBy(a => a.RejectionName).ToListAsync();
            }
            return await dBContext.Rejections.OrderBy(a=>a.RejectionName).ToListAsync();
        }
        public async Task<bool> AddRejection(string rejectionName)
        {
            try
            {
                LYDOTUCHOI rejection = new LYDOTUCHOI();
                rejection.RejectionName = rejectionName;
                dBContext.Rejections.Add(rejection);
                dBContext.SaveChanges();
                return true;


            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> EditRejection(int rejectionId, string rejectionName)
        {
            try
            {
                var rejection = await dBContext.Rejections.FindAsync(rejectionId);
                rejection.RejectionName = rejectionName;
                dBContext.Rejections.Update(rejection);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> DeleteRejection(int rejectionId)
        {
            try
            {
                var rejection = await dBContext.Rejections.FindAsync(rejectionId);
                dBContext.Rejections.Remove(rejection);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region PricePacket
        public async Task<List<GOITIN>> GetPricePackets(string search="")
        {
            if(!search.IsNullOrEmpty())
            {
                return await dBContext.PricePackets.Where(a=>a.PricePacketName.Contains(search))
                                                    .OrderBy(a => a.PricePacketName).ToListAsync();
            }
            return await dBContext.PricePackets.OrderBy(a=>a.PricePacketName).ToListAsync();
        }
        public async Task<bool> AddPricePacket(string pricePacketName, dynamic price, int numberDays)
        {
            try
            {
                GOITIN pricePacket = new GOITIN();
                pricePacket.PricePacketName = pricePacketName;
                pricePacket.Price = price;
                pricePacket.NumberDays = numberDays;
                dBContext.PricePackets.Add(pricePacket);
                dBContext.SaveChanges();
                return true;


            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> EditPricePacket(int pricePacketId, string pricePacketName, dynamic price, int numberDays)
        {
            try
            {
                var pricePacket = await dBContext.PricePackets.FindAsync(pricePacketId);
                pricePacket.PricePacketName = pricePacketName;
                pricePacket.Price = price;
                pricePacket.NumberDays = numberDays;
                dBContext.PricePackets.Update(pricePacket);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> DeletePricePacket(int pricePacketId)
        {
            try
            {
                var pricePacket = await dBContext.PricePackets.FindAsync(pricePacketId);
                dBContext.PricePackets.Remove(pricePacket);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region RevenueChart
        public async Task<Dictionary<AdminEnum.RevenueType, List<DataPoint>>> GetRevenueDataPoints(int year)
        {
            var pricePacketRevenueInYear = await dBContext.PricePacketDetails.Where(x => x.BuyDate.Year == year && x.HasPaid).ToListAsync();
            var bidRevenueInYear = await dBContext.BidHistories.Where(x => x.BuyDate.Year == year && x.IsPaid).ToListAsync();

            List<decimal> pricePacketRevenueMonthly = new List<decimal>(new decimal[12]);
            List<decimal> bidRevenueMonthly = new List<decimal>(new decimal[12]);

            foreach (var pricePacketRevenue in pricePacketRevenueInYear)
            {
                pricePacketRevenueMonthly[pricePacketRevenue.BuyDate.Month - 1] += pricePacketRevenue.PricePacket.Price;
            }

            foreach (var bidRevenue in bidRevenueInYear)
            {
                bidRevenueMonthly[bidRevenue.BuyDate.Month - 1] += bidRevenue.IncreasePrice;
            }

            Dictionary<AdminEnum.RevenueType, List<DataPoint>> dataPoints = new();
            dataPoints.Add(AdminEnum.RevenueType.PricePacket, new List<DataPoint>());
            dataPoints.Add(AdminEnum.RevenueType.Bid, new List<DataPoint>());
            dataPoints.Add(AdminEnum.RevenueType.Total, new List<DataPoint>());

            for (int i = 0; i < 12; i++)
            {
                dataPoints[AdminEnum.RevenueType.PricePacket].Add(new DataPoint(Configs.StatisticConfig.monthsArray[i], pricePacketRevenueMonthly[i]));
                dataPoints[AdminEnum.RevenueType.Bid].Add(new DataPoint(Configs.StatisticConfig.monthsArray[i], bidRevenueMonthly[i]));
                dataPoints[AdminEnum.RevenueType.Total].Add(new DataPoint(Configs.StatisticConfig.monthsArray[i], pricePacketRevenueMonthly[i] + bidRevenueMonthly[i]));
            }

            return dataPoints;
        }
        #endregion

        public async Task<bool> HasRightPassword(string password, int userId)
        {
            var account = await dBContext.Accounts.FindAsync(userId);
            if (account == null) { return false; }
            return account.Password.Trim() == passwordService.HashPassword(password.Trim());
        }

        public async Task<bool> ChangePassword(string password, int userId)
        {
            try
            {
                var account = await dBContext.Accounts.FindAsync(userId);
                if (account == null) { return false; }
                account.Password = passwordService.HashPassword(password);
                dBContext.Accounts.Update(account);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public async Task<List<NHANVIEN>> GetStaffs(string queryInput)
        {
            if (!string.IsNullOrEmpty(queryInput))
            {
                return await dBContext.Staffs.Where(
                    s => s.FirstName.Contains(queryInput) 
                    || s.LastName.Contains(queryInput)
                    || s.Identity.CitizenNumber.Contains(queryInput)
                    || (s.Account.Gmail != null && s.Account.Gmail.Contains(queryInput)) 
                    || s.Account.PhoneNumber.Contains(queryInput)
                    ).OrderBy(a => a.FirstName + a.LastName).ToListAsync();
            }
            else
            return await dBContext.Staffs.OrderBy(a=>a.FirstName+a.LastName).ToListAsync();
        }

        public async Task<NHANVIEN> GetStaffProfile(int staffId)
        {
            return await dBContext.Staffs.FindAsync(staffId);
        }


        public async Task<AccountEnum.CreateEditAccountResult> AddStaff(StaffProfileViewModel model, IFormFile frontImage, IFormFile backImage, AccountEnum.AccountType staffAccountType)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                if (await dBContext.Accounts.AnyAsync(a => a.PhoneNumber == model.Staff.Account.PhoneNumber)) 
                {
                    return AccountEnum.CreateEditAccountResult.PhoneDuplicated;
                }

                if (await dBContext.Accounts.AnyAsync(a => a.Gmail == model.Staff.Account.Gmail))
                {
                    return AccountEnum.CreateEditAccountResult.GmailDuplicated;
                }

                if (await dBContext.Identity.AnyAsync(i => i.CitizenNumber == model.Staff.Identity.CitizenNumber))
                {
                    return AccountEnum.CreateEditAccountResult.CitizenNumberDuplicated;
                }

                var tempFrontImage = await firebaseService.UploadImage(frontImage, staffAccountType, ImageEnum.ImageType.Identity);
                if (tempFrontImage == null)
                {
                    return AccountEnum.CreateEditAccountResult.FrontImageError;
                }

                var tempBackImage = await firebaseService.UploadImage(backImage, staffAccountType, ImageEnum.ImageType.Identity);
                if (tempBackImage == null)
                {
                    return AccountEnum.CreateEditAccountResult.BackImageError;
                }

                NHANVIEN staff = new();
                staff.FirstName = model.Staff.FirstName.Trim();
                staff.LastName = model.Staff.LastName.Trim();

                staff.Account = new();
                staff.Account.AvatarUrl = DefaultImageUrlConfig.DefaultAvatarStaff;

                staff.Account.PhoneNumber = model.Staff.Account.PhoneNumber;
                if(model.Staff.Account.Gmail!=null)
                {
                    staff.Account.Gmail = model.Staff.Account.Gmail.Trim();
                }
                
                staff.Account.Password = passwordService.HashPassword("123");
                staff.Account.Status = true;
                staff.Account.AccountType = await dBContext.AccountTypes.FindAsync((int)AccountEnum.AccountType.Staff);

                staff.Identity = new();
                staff.Identity.CitizenNumber = model.Staff.Identity.CitizenNumber;
                staff.Identity.FrontImage = tempFrontImage;
                staff.Identity.BackImage = tempBackImage;


                await dBContext.Identity.AddAsync(staff.Identity);
                await dBContext.Accounts.AddAsync(staff.Account);
                await dBContext.Staffs.AddAsync(staff);
                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return AccountEnum.CreateEditAccountResult.Success;
            }
            catch (Exception ex)
            {
                return AccountEnum.CreateEditAccountResult.ServerError;
            }
        }
        public async Task<AccountEnum.CreateEditAccountResult> EditStaff(StaffProfileViewModel model, IFormFile frontImage, IFormFile backImage, AccountEnum.AccountType staffAccountType)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                var staff = await dBContext.Staffs.FindAsync(model.Staff.StaffId);
                model.Staff.Identity.FrontImage = staff.Identity.FrontImage;
                model.Staff.Identity.BackImage = staff.Identity.BackImage;
                var accountWithPhone = await dBContext.Accounts.FirstOrDefaultAsync(a => a.PhoneNumber == model.Staff.Account.PhoneNumber);
                var accountWithGmail = await dBContext.Accounts.FirstOrDefaultAsync(a => a.Gmail == model.Staff.Account.Gmail);
                var identityWithCitizenNumber = await dBContext.Identity.FirstOrDefaultAsync(i => i.CitizenNumber == model.Staff.Identity.CitizenNumber);

                if (accountWithPhone != null && accountWithPhone.AccountID != staff.Account.AccountID)
                {
                    return AccountEnum.CreateEditAccountResult.PhoneDuplicated;
                }

                if (accountWithGmail != null && accountWithGmail.AccountID != staff.Account.AccountID)
                {
                    return AccountEnum.CreateEditAccountResult.GmailDuplicated;
                }

                if (identityWithCitizenNumber != null && identityWithCitizenNumber.CitizenId != staff.Identity.CitizenId)
                {
                    return AccountEnum.CreateEditAccountResult.CitizenNumberDuplicated;
                }
                var tempFrontImage = await firebaseService.UploadImage(frontImage, staffAccountType, ImageEnum.ImageType.Identity);
                if (frontImage != null && tempFrontImage == null)
                {
                    return AccountEnum.CreateEditAccountResult.FrontImageError;
                }

                var tempBackImage = await firebaseService.UploadImage(backImage, staffAccountType, ImageEnum.ImageType.Identity);
                if (backImage != null && tempBackImage == null)
                {
                    return AccountEnum.CreateEditAccountResult.BackImageError;
                }

                staff.FirstName = model.Staff.FirstName;
                staff.LastName = model.Staff.LastName;

                staff.Account.PhoneNumber = model.Staff.Account.PhoneNumber;
                staff.Account.Gmail = model.Staff.Account.Gmail;

                staff.Identity.CitizenNumber = model.Staff.Identity.CitizenNumber;
                staff.Identity.FrontImage = (tempFrontImage == null ? staff.Identity.FrontImage : tempFrontImage) ;
                staff.Identity.BackImage = (tempBackImage == null ? staff.Identity.BackImage : tempBackImage);

                dBContext.Staffs.Update(staff);
                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return AccountEnum.CreateEditAccountResult.Success;
            }
            catch (Exception ex)
            {
                return AccountEnum.CreateEditAccountResult.ServerError;
            }

        }

        public async Task<bool> ToggleStaffAccount(int staffId, bool status)
        {
            try
            {
                var staff = await dBContext.Staffs.FindAsync(staffId);
                staff.Account.Status = status;
                dBContext.Staffs.Update(staff);
                await dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        // get employers
        public async Task<List<NGUOITHUE>> GetEmployers(string queryInput)
        {
            if (!string.IsNullOrEmpty(queryInput))
            {
                return await dBContext.Employers.Where(
                    s => s.FirstName.Contains(queryInput)
                    || s.LastName.Contains(queryInput)
                    || s.Identity.CitizenNumber.Contains(queryInput)
                    || (s.Account.Gmail != null && s.Account.Gmail.Contains(queryInput))
                    || s.Account.PhoneNumber.Contains(queryInput)
                    ).OrderBy(a => a.FirstName + a.LastName).ToListAsync();
            }
            return await dBContext.Employers.OrderBy(a => a.FirstName + a.LastName).ToListAsync();
        }

        public async Task<bool> ToggleEmployerAccount(int employerId, bool status)
        {
            try
            {
                var employer = await dBContext.Employers.FindAsync(employerId);
                employer.Account.Status = status;
                dBContext.Employers.Update(employer);
                await dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<NGUOIGIUPVIEC>> GetEmployees(string q)
        {
            if (!string.IsNullOrEmpty(q))
            {
                return await dBContext.Employees.Where(
                       s => s.FirstName.Contains(q)
                       || s.LastName.Contains(q)
                       || s.Identity.CitizenNumber.Contains(q)
                       || (s.Account.Gmail != null && s.Account.Gmail.Contains(q))
                       || s.Account.PhoneNumber.Contains(q)
                       ).OrderBy(a => a.FirstName + a.LastName).ToListAsync();
            }
            return await dBContext.Employees.OrderBy(a => a.FirstName + a.LastName).ToListAsync();
        }

        public async Task<bool> ToggleEmployeeAccount(int employeeId, bool status)
        {
            try
            {
                var employee = await dBContext.Employees.FindAsync(employeeId);
                employee.Account.Status = status;
                dBContext.Employees.Update(employee);
                await dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

