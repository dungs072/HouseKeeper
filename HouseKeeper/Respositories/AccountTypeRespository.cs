using HouseKeeper.DBContext;
using HouseKeeper.Enum;
using HouseKeeper.IServices;
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
        private readonly IPasswordService passwordService;
        public AccountTypeRespository(HouseKeeperDBContext dBContext, IPasswordService passwordService)
        {
           
            this.dBContext = dBContext;
            this.passwordService = passwordService;
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
                if (accounts[0].Password.Trim() == passwordService.HashPassword(model.Password) || accounts[0].Password.Trim() == model.Password)
                {
                    return accounts[0].AccountID;
                }
                else
                {
                    return (int)AccountEnum.LoginResult.WrongPassword;
                }
            }
            else
            {
                return (int)AccountEnum.LoginResult.PhoneAndGmailNotRegistered;
            }
        }

        public async Task<LoginInfor> GetEmployerOrEmployee(int accountId)
        {
            var employers = await dBContext.Employers.Where(a => a.Account.AccountID == accountId).ToListAsync();
            var loginInfor = new LoginInfor();
            loginInfor.ViewIndex = (int)AccountEnum.AccountType.Unknown;
            if(employers.Count>0)
            {
                loginInfor.Id = employers[0].EmployerId;
                loginInfor.ViewIndex = (int)AccountEnum.AccountType.Employer;
            }
            var employee = await dBContext.Employees.Where(a=>a.Account.AccountID == accountId).ToListAsync();
            if(employee.Count>0)
            {
                loginInfor.Id = employee[0].EmployeeId;
                loginInfor.ViewIndex = (int)AccountEnum.AccountType.Employee;
            }
            var staff = await dBContext.Staffs.Where(a => a.Account.AccountID == accountId).ToListAsync();
            if(staff.Count>0)
            {
                loginInfor.Id = staff[0].StaffId;
                loginInfor.ViewIndex = (int)AccountEnum.AccountType.Staff;
            }

            if(employee.Count==0&&employers.Count==0&&staff.Count==0)
            {
                loginInfor.ViewIndex = (int)AccountEnum.AccountType.Admin;
                loginInfor.Id = accountId;
            }
            return loginInfor;
        }
        //1: create account succesffully
        //2: Phone number is duplicated
        //3: Gmail is duplicated
        //4: Server error;
        public async Task<int> CreateEmployerAccount(TAIKHOAN account, NGUOITHUE employer, DANHTINH identity)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                await dBContext.Accounts.AddAsync(account);
                await dBContext.Employers.AddAsync(employer);
                await dBContext.Identity.AddAsync(identity);
                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return (int)AccountEnum.CreateAccountResult.Success;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if(ex.InnerException!=null)
                {
                    if (ex.InnerException.Message.Contains("UK_SDT"))
                    {
                        return (int)AccountEnum.CreateAccountResult.PhoneDuplicated;
                    }
                    else if (ex.InnerException.Message.Contains("UK_GMAIL"))
                    {
                        return (int)AccountEnum.CreateAccountResult.GmailDuplicated;
                    }
                }
                return (int)AccountEnum.CreateAccountResult.ServerError;
            }
        }
        public async Task<int> CreateEmployeeAccount(TAIKHOAN account, NGUOIGIUPVIEC employee, DANHTINH identity)
        {
            using var transaction = await dBContext.Database.BeginTransactionAsync();
            try
            {
                await dBContext.Accounts.AddAsync(account);
                await dBContext.Employees.AddAsync(employee);
                await dBContext.Identity.AddAsync(identity);
                await dBContext.SaveChangesAsync();
                transaction.Commit();
                return (int)AccountEnum.CreateAccountResult.Success;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.Message.Contains("UK_SDT"))
                    {
                        return (int)AccountEnum.CreateAccountResult.PhoneDuplicated;
                    }
                    else if (ex.InnerException.Message.Contains("UK_GMAIL"))
                    {
                        return (int)AccountEnum.CreateAccountResult.GmailDuplicated;
                    }
                }
                return (int)AccountEnum.CreateAccountResult.ServerError;
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

        public async Task<List<HUYEN>> GetDistricts()
        {
            return await dBContext.Districts.ToListAsync();
        }

        public async Task<HUYEN> GetDistrict(int id)
        {
            return await dBContext.Districts.FindAsync(id);
        }

        public async Task<TRANGTHAIDANHTINH> GetIdentityState(int id)
        {
            return await dBContext.IdentityStates.FindAsync(id);
        }
    }
}
