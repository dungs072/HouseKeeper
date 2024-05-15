namespace HouseKeeper.Enum
{
    public class AccountEnum
    {

        public enum AccountType
        {
            Unknown = 0,
            Admin = 1,
            Employer = 2,
            Employee = 3,
            Staff = 4
        }

        public enum LoginResult
        {
            UserId = 0,
            PhoneAndGmailNotRegistered = -2,
            WrongPassword = -3
        }

        public enum CreateAccountResult
        {
            Success = 1,
            PhoneDuplicated = 2,
            GmailDuplicated = 3,
            ServerError = 4
        }
    }
}
