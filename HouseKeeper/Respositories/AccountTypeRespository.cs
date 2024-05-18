using HouseKeeper.DBContext;
using HouseKeeper.Enum;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views;
using HouseKeeper.Models.Views.OutPage;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Net.Mail;
using System.Net;
using Stripe;
using System.Text;
using System.Security.Cryptography;

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
                var hashPassword = HashPassword(model.Password);
                if (accounts[0].Password.Trim()==hashPassword)
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
                var hashPassword = HashPassword(account.Password);
                account.Password = hashPassword;
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
                var hashPassword = HashPassword(account.Password);
                account.Password = hashPassword;
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

        public string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            char[] password = new char[length];
            for (int i = 0; i < length; i++)
            {
                password[i] = chars[random.Next(chars.Length)];
            }

            return new string(password);
        }
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public async Task<bool> HandleForgetPassword(ForgetPasswordViewModel model)
        {
            try
            {
                var user = IsEmailExist(model.Gmail);
                if (user != null)
                {
                    string randomPassword = GenerateRandomPassword(6);
                    HandleSendingDataToEmail(model.Gmail, "HouseKeeper - Forget Password", "Your new password is: " + randomPassword);
                    user.Password = HashPassword(randomPassword);
                    dBContext.Accounts.Update(user);
                    await dBContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> SendMessage(ContactViewModel model)
        {
            try
            {
                HandleSendingDataToEmail(model.Gmail, model.Subject + " - "+model.Name, model.Message);
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }
        public void HandleSendingDataToEmail(string toEmail, string subject, string body)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("dungoc1235@gmail.com", "kkacjdpsrwwwiuur");
            smtpClient.EnableSsl = true;
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("dungoc1235@gmail.com");
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = subject;
            mailMessage.Body = body;

            smtpClient.Send(mailMessage);
        }

        public TAIKHOAN IsEmailExist(string email)
        {
            var account = dBContext.Accounts.FirstOrDefault(u => (u.Gmail == email));
            return account;
        }

    }
}
