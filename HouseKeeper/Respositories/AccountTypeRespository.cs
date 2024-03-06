using HouseKeeper.DBContext;
using HouseKeeper.Models;
using Microsoft.EntityFrameworkCore;

namespace HouseKeeper.Respositories
{
    public class AccountTypeRespository : IAccountTypeRespository
    {
        private readonly HouseKeeperDBContext dBContext;
        public AccountTypeRespository(HouseKeeperDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public async Task<List<LOAITK>> GetAccounts()
        {
            return await dBContext.AccountTypes.ToListAsync();
        }

    }
}
