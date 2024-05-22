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
            WrongPassword = -3,
            Locked = -4
        }

        public enum CreateEditAccountResult
        {
            Success = 1,
            PhoneDuplicated = 2,
            GmailDuplicated = 3,
            ServerError = 4,
            CitizenNumberDuplicated = 5,
            FrontImageError = 6,
            BackImageError = 7,
            AvatarImageError = 8,
            NotFound = 9
        }
    }
}
