using HouseKeeper.Enum;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Admin;

namespace HouseKeeper.Respositories
{
    public interface IAdminRespository
    {
        // JobType
        Task<List<LOAICONGVIEC>> GetJobTypes();
        Task<bool> AddJobType(string jobName);
        Task<bool> EditJobType(int jobId, string jobName);
        Task<bool> DeleteJobType(int jobId);
        // PaidType
        Task<List<HINHTHUCTRALUONG>> GetPaidTypes();
        Task<bool> AddPaidType(string paidTypeName);
        Task<bool> EditPaidType(int paidTypeId, string paidTypeName);
        Task<bool> DeletePaidType(int paidTypeId);
        //Experience
        Task<List<KINHNGHIEM>> GetExperiences();
        Task<bool> AddExperience(string experienceName);
        Task<bool> EditExperience(int experienceId, string experienceName);
        Task<bool> DeleteExperience(int experienceId);
        //Rejection
        Task<List<LYDOTUCHOI>> GetRejections();
        Task<bool> AddRejection(string rejectionName);
        Task<bool> EditRejection(int rejectionId, string rejectionName);
        Task<bool> DeleteRejection(int rejectionId);
        //Price packet
        Task<List<GOITIN>> GetPricePackets();
        Task<bool> AddPricePacket(string pricePacketName, dynamic price, int numberDays);
        Task<bool> EditPricePacket(int pricePacketId, string pricePacketName, dynamic price, int numberDays);
        Task<bool> DeletePricePacket(int pricePacketId);
        //Revenue
        Task<Dictionary<AdminEnum.RevenueType, List<DataPoint>>> GetRevenueDataPoints(int year);
    }
}
