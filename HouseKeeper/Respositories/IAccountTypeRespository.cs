using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views;

namespace HouseKeeper.Respositories
{
    public interface IAccountTypeRespository
    {
        Task<List<LOAITK>> GetAccounts();

        Task<int> Login(LoginViewModel model);
        Task<int> GetEmployerOrEmployee(int accountId);

        Task<LOAITK> GetSpecificAccountType(int id);
        Task<TINHTHANHPHO> GetSpecificCity(int id);
        Task<int> CreateEmployerAccount(TAIKHOAN account, NGUOITHUE employer);
        Task<int> CreateEmployeeAccount(TAIKHOAN account, NGUOIGIUPVIEC employee);
        Task<List<TINHTHANHPHO>> GetCities();
    }
}
