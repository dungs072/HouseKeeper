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
        // City&Districts
        Task<List<TINHTHANHPHO>> GetCities();
        Task<bool> AddCity(string cityName);
        Task<bool> EditCity(int cityId, string cityName);
        Task<bool> DeleteCity(int cityId);
        

        Task<List<HUYEN>> GetDistricts(int cityId);
        Task<bool> AddDistrict(string districtName, int cityId);
        Task<bool> EditDistrict(int districtId, string districtName);
        Task<bool> DeleteDistrict(int districtId);
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
        Task<bool> HasRightPassword(string password, int userId);
        Task<bool> ChangePassword(string password, int userId);
        //Staff
        Task<List<NHANVIEN>> GetStaffs(string queryInput);
        Task<NHANVIEN> GetStaffProfile(int staffId);
        Task<bool> ToggleStaffAccount(int staffId, bool status);
        Task<AccountEnum.CreateAccountResult> AddStaff(StaffProfileViewModel model, IFormFile frontImage, IFormFile backImage, AccountEnum.AccountType staffAccountType);
        Task<AccountEnum.CreateAccountResult> EditStaff(StaffProfileViewModel model, IFormFile frontImage, IFormFile backImage, AccountEnum.AccountType staffAccountType);
        //Employer
        Task<List<NGUOITHUE>> GetEmployers(string queryInput);
        Task<bool> ToggleEmployerAccount(int employerId, bool status);
        Task<List<NGUOIGIUPVIEC>> GetEmployees(string q);
        Task<bool> ToggleEmployeeAccount(int employeeId, bool status);
        //Employee
    }
}
