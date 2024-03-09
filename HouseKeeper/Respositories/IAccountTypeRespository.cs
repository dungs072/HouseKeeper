using HouseKeeper.Models.DB;

namespace HouseKeeper.Respositories
{
    public interface IAccountTypeRespository
    {
        Task<List<LOAITK>> GetAccounts();
        Task<LOAITK> GetSpecificAccountType(int id);
        Task<TINHTHANHPHO> GetSpecificCity(int id);
        Task<int> CreateEmployerAccount(TAIKHOAN account, NGUOITHUE employer);
        Task<int> CreateEmployeeAccount(TAIKHOAN account, NGUOIGIUPVIEC employee);
        Task<List<TINHTHANHPHO>> GetCities();
    }
}
