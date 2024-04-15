﻿using HouseKeeper.DBContext;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.OutPage;
using HouseKeeper.Models.Views.Employer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseKeeper.Respositories
{
    public class EmployerRespository : IEmployerRespository
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
        public async Task<List<HUYEN>> GetDistricts()
        {
            return await dBContext.Districts.ToListAsync();
        }
        public async Task<List<TINTUYENDUNG>> GetOnlineRecruitments()
        {
            return await dBContext.Recruitments.Where(a => a.Status.StatusName == status[2]).
                        OrderByDescending(a=>a.BidPrice).Take(10).ToListAsync();
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
        public async Task<TINTUYENDUNG> GetRecruitment(int id)
        {
            return await dBContext.Recruitments.FindAsync(id);
        }
        public async Task<TRANGTHAITIN> GetRecruitmentStatus(int id)
        {
            return await dBContext.RecruitmentStatus.FindAsync(id);
        }
        public async Task<HUYEN> GetDistrict(int id)
        {
            return await dBContext.Districts.FindAsync(id);
        }
        public async Task<bool> CreateRecruitment(TINTUYENDUNG recruitment, string[] jobIds, int pricePacketId)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
               
                List<CHITIETLOAIGIUPVIEC> housekeepingTypes = new List<CHITIETLOAIGIUPVIEC>();
                CHITIETGIAGOITIN packetDetail = new CHITIETGIAGOITIN();
                packetDetail.Recruitment = recruitment;
                packetDetail.PricePacket = await GetPricePacket(pricePacketId);
                packetDetail.BuyDate = DateTime.Now;
                //packetDetail.HasPaid = true;

                recruitment.Status = await GetRecruitmentStatus(1);
                recruitment.RecruitDeadlineDate = DateTime.Now.AddDays(packetDetail.PricePacket.NumberDays);
                await dBContext.Recruitments.AddAsync(recruitment);
                await dBContext.PricePacketDetails.AddAsync(packetDetail);
                LICHSUDAUGIA bidHistory = new LICHSUDAUGIA();
                bidHistory.BuyDate = DateTime.Now;
                bidHistory.IncreasePrice = recruitment.BidPrice*1000;
                //bidHistory.IsPaid = true;
                bidHistory.Recruitment = recruitment;
                await dBContext.BidHistories.AddAsync(bidHistory);
                foreach (var jobId in jobIds)
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
        #region ListRecruitment
        public async Task<ListRecruitmentViewModel> GetEmployerRecruitments(int employerId)
        {
            ListRecruitmentViewModel model = new ListRecruitmentViewModel();
            var originalRecruitments = await dBContext.Recruitments.Where(a => a.Status.StatusName == status[2])
                                        .OrderByDescending(a=>a.BidPrice).ToListAsync();
            var recruitments = await dBContext.Recruitments.Where(a => a.Employer.EmployerId == employerId).
                                    OrderByDescending(a => a.PostTime).ToListAsync();
            model.OnlineRecruitments = new List<TINTUYENDUNG>();
            model.PendingApprovalRecruitments = new List<TINTUYENDUNG>();
            model.OutDatedRecruitments = new List<TINTUYENDUNG>();
            model.HiddenRecruitments = new List<TINTUYENDUNG>();
            model.DisapprovalRecruitments = new List<TINTUYENDUNG>();
            foreach (var recruitment in recruitments)
            {
                if (recruitment.Status.StatusName == status[0])
                {
                    model.PendingApprovalRecruitments.Add(recruitment);
                }
                else if (recruitment.Status.StatusName == status[1])
                {
                    model.DisapprovalRecruitments.Add(recruitment);
                }
                else if (recruitment.Status.StatusName == status[2])
                {
                    model.OnlineRecruitments.Add(recruitment);
                }
                else if (recruitment.Status.StatusName == status[3])
                {
                    model.HiddenRecruitments.Add(recruitment);
                }
                else if (recruitment.Status.StatusName == status[4])
                {
                    model.OutDatedRecruitments.Add(recruitment);
                }
            }
            int count = 1;
            // fix here
            if(model.OnlineRecruitments.Count>0)
            {
                foreach (var recruitment in originalRecruitments)
                {
                    foreach(var onlineRecruitment in model.OnlineRecruitments)
                    {
                        if(recruitment==onlineRecruitment)
                        {
                            onlineRecruitment.Ranked = count;
                            break;
                        }
                        
                    }
                    count++;
                }
            }
           

            return model;
        }
        public async Task<bool> DeleteSpecificRecruitment(int recruitmentId)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                var recruitment = await dBContext.Recruitments.FindAsync(recruitmentId);
                var pricesDetails = recruitment.PricePacketDetail.ToList();
                var jobDetails = recruitment.HouseworkDetails.ToList();
                var bidHistories = recruitment.BidHistories.ToList();
                recruitment.PricePacketDetail = null;
                recruitment.HouseworkDetails = null;
                // need to return money to user
                dBContext.PricePacketDetails.RemoveRange(pricesDetails);
                dBContext.HouseWorkDetails.RemoveRange(jobDetails);
                dBContext.BidHistories.RemoveRange(bidHistories);

                dBContext.Recruitments.Remove(recruitment);
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

        public async Task<bool> EditRecruitment(EditRecruitmentViewModel model)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                TINTUYENDUNG recruitment = await dBContext.Recruitments.FindAsync(model.RecruitmentId);
                if (recruitment == null)
                {
                    return false;
                }
                recruitment.Age = model.AgeRange;
                //recruitment.City = await dBContext.Cities.FindAsync(model.CityId);
                recruitment.Experience = await dBContext.Experiences.FindAsync(model.ExperienceId);
                recruitment.SalaryForm = await dBContext.SalaryForms.FindAsync(model.PaidTypeId);
                recruitment.District = await dBContext.Districts.FindAsync(model.DistrictId);
                recruitment.Note = model.TakeNotes;
                recruitment.FullTime = model.IsFulltime;
                recruitment.Gender = model.Gender;
                recruitment.MaxApplications = model.NumberVacancies;
                recruitment.MinSalary = model.MinSalary;
                recruitment.MaxSalary = model.MaxSalary;
                recruitment.Address = model.Address;
                dBContext.Recruitments.Update(recruitment);
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
        // remember to date out for recruitment
        public async Task<bool> HideRecruitment(int recruitmentId)
        {
            try
            {
                TINTUYENDUNG recruitment = await dBContext.Recruitments.FindAsync(recruitmentId);
                var statuses = await dBContext.RecruitmentStatus.ToListAsync();
                foreach (var statuss in statuses)
                {
                    if (statuss.StatusName == status[3])
                    {
                        recruitment.Status = statuss;
                        break;
                    }
                }
                dBContext.Recruitments.Update(recruitment);
                await dBContext.SaveChangesAsync();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
           
        }
        public async Task<bool> UnHideRecruitment(int recruitmentId)
        {
            try
            {
                TINTUYENDUNG recruitment = await dBContext.Recruitments.FindAsync(recruitmentId);
                var statuses = await dBContext.RecruitmentStatus.ToListAsync();
                foreach (var statuss in statuses)
                {
                    if (statuss.StatusName == status[2])
                    {
                        recruitment.Status = statuss;
                        break;
                    }
                }
                dBContext.Recruitments.Update(recruitment);
                await dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> AddBidPrice(int recruitmentId, decimal price)
        {
            try
            {
                var recruitment = await dBContext.Recruitments.FindAsync(recruitmentId);
                recruitment.BidPrice += price;
                dBContext.Recruitments.Update(recruitment);
                await dBContext.SaveChangesAsync();
                return true;

            }
            catch(Exception ex)
            {
                return false;
            }
        }
        #endregion
    }

}
