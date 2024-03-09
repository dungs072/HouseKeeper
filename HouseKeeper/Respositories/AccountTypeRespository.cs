using HouseKeeper.DBContext;
using HouseKeeper.Models.DB;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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
        //1: create account succesffully
        //2: Phone number is duplicated
        //3: Gmail is duplicated
        //4: Server error;
        public async Task<int> CreateEmployerAccount(TAIKHOAN account, NGUOITHUE employer)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                await dBContext.Accounts.AddAsync(account);
                await dBContext.Employers.AddAsync(employer);
                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return 1;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if(ex.InnerException!=null)
                {
                    if (ex.InnerException.Message.Contains("UK_SDT"))
                    {
                        return 2;
                    }
                    else if (ex.InnerException.Message.Contains("UK_GMAIL"))
                    {
                        return 3;
                    }
                }
                return 4;
            }
        }
        public async Task<int> CreateEmployeeAccount(TAIKHOAN account, NGUOIGIUPVIEC employee)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                await dBContext.Accounts.AddAsync(account);
                await dBContext.Employees.AddAsync(employee);
                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return 1;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.Message.Contains("UK_SDT"))
                    {
                        return 2;
                    }
                    else if (ex.InnerException.Message.Contains("UK_GMAIL"))
                    {
                        return 3;
                    }
                }
                return 4;
            }
        }

        public async Task<LOAITK> GetSpecificAccountType(int id)
        {
            return await dBContext.AccountTypes.FindAsync(id);   
        }
        public async Task<TINHTHANHPHO> GetSpecificCity(int id)
        {
            return await dBContext.Cities.FindAsync(id);
        }
        public async Task<List<TINHTHANHPHO>> GetCities()
        {
            return await dBContext.Cities.ToListAsync();
        }
    }
}
