namespace HouseKeeper.Configs
{
    public class FirebaseConfig
    {
        public string ApiKey { get; set; }
        public string Bucket { get; set; }
        public string AuthEmail { get; set; }
        public string AuthPassword { get; set; }

        public string EmployeeFolder { get; set; }
        public string EmployerFolder { get; set; }
        public string StaffFolder { get; set; }
        public string IdentityFolder { get; set; }
        public string AvatarFolder { get; set; }
    }
}
