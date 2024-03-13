using HouseKeeper.DBContext;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.OutPage;
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
        //userId:login successfully 
        //-2: Phone number and gmail is not registered
        //-3: Password is wrong
        public async Task<int> Login(LoginViewModel model)
        {
            var accounts = await dBContext.Accounts.Where(a => a.PhoneNumber == model.LoginName || a.Gmail == model.LoginName).ToListAsync();
            if(accounts.Count>0)
            {
                if (accounts[0].Password.Trim()==model.Password)
                {
                    return accounts[0].AccountID;
                }
                else
                {
                    return -3;
                }
            }
            else
            {
                return -2;
            }
        }
        //0: is admin
        //1: is employer
        //2: is employee
        public async Task<LoginInfor> GetEmployerOrEmployee(int accountId)
        {
            var employers = await dBContext.Employers.Where(a => a.Account.AccountID == accountId).ToListAsync();
            var loginInfor = new LoginInfor();
            if(employers.Count>0)
            {
                loginInfor.Id = employers[0].EmployerId;
                loginInfor.ViewIndex = 1;
            }
            var employee = await dBContext.Employees.Where(a=>a.Account.AccountID == accountId).ToListAsync();
            if(employee.Count>0)
            {
                loginInfor.Id = employee[0].EmployeeId;
                loginInfor.ViewIndex = 2;
            }
            return loginInfor;
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
