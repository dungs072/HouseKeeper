using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.OutPage
{
    public class SignUpViewModels
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string Gmail { get; set; }
        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }
        public bool IsEmployer { get; set; }
        public bool IsEmployee { get; set; }
        public string Address { get; set; }
        public DateTime BirthDate { get; set; }
        public List<TINHTHANHPHO> Cities { get; set; }
    }

}
