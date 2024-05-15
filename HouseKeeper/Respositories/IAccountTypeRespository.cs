using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.OutPage;

namespace HouseKeeper.Respositories
{
    public interface IAccountTypeRespository
    {
        Task<List<LOAITK>> GetAccounts();

        Task<int> Login(LoginViewModel model);
        Task<LoginInfor> GetEmployerOrEmployee(int accountId);
        Task<LOAITK> GetSpecificAccountType(int id);
        Task<TINHTHANHPHO> GetSpecificCity(int id);
        Task<int> CreateEmployerAccount(TAIKHOAN account, NGUOITHUE employer, DANHTINH identity);
        Task<int> CreateEmployeeAccount(TAIKHOAN account, NGUOIGIUPVIEC employee, DANHTINH identity);
        Task<List<TINHTHANHPHO>> GetCities();
        Task<List<HUYEN>> GetDistricts();
        Task<HUYEN> GetDistrict(int id);
        Task<TRANGTHAIDANHTINH> GetIdentityState(int id);
    }
}
