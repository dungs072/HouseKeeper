using HouseKeeper.Models;

namespace HouseKeeper.Respositories
{
    public interface IAccountTypeRespository
    {
        Task<List<LOAITK>> GetAccounts();
    }
}
