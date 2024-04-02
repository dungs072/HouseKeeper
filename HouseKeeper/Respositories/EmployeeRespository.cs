﻿using HouseKeeper.DBContext;
using HouseKeeper.Models.DB;
using Microsoft.EntityFrameworkCore;

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

            var recruitments = await dBContext.Recruitments.Where(a => a.Status.StatusName == status[2]).ToListAsync();

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

    }
}
