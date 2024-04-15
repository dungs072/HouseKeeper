using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Employer
{
    public class CreateRecruitmentsViewModel
    {
        public bool IsFulltime { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public string? Gender { get; set; }
        public int NumberVacancies { get; set; }
        public string AgeRange { get; set; }
        public string? TakeNotes { get; set; }
        public string Address { get; set; }

        public DateTime PostTime { get; set; }

        public List<HINHTHUCTRALUONG> PaidTypes { get; set; }
        public List<KINHNGHIEM> Experiences { get; set; }
        public List<TINHTHANHPHO> Cities { get; set; }
        public List<HUYEN> Districts { get; set; }
        public List<LOAICONGVIEC> jobs { get; set; }

        
    }
}
