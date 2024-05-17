﻿using HouseKeeper.DBContext;
using HouseKeeper.Enum;
using HouseKeeper.IServices;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Admin;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HouseKeeper.Respositories
{
    public class AdminRespository : IAdminRespository
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

        #region Rejection
        public async Task<List<LYDOTUCHOI>> GetRejections()
        {
            return await dBContext.Rejections.ToListAsync();
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
        public async Task<List<GOITIN>> GetPricePackets()
        {
            return await dBContext.PricePackets.ToListAsync();
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
            return account.Password.Trim() == HashPassword(password.Trim());
        }

        public async Task<bool> ChangePassword(string password, int userId)
        {
            try
            {
                var account = await dBContext.Accounts.FindAsync(userId);
                if (account == null) { return false; }
                account.Password = HashPassword(password);
                dBContext.Accounts.Update(account);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
