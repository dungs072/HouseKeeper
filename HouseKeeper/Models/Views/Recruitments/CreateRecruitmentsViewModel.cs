using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Recruitments
{
    public class CreateRecruitmentsViewModel
    {
        public bool IsFulltime { get; set; }
        public float MinSalary { get; set; }
        public float MaxSalary { get; set; }
        public string? Gender { get; set; }
        public int NumberVacancies { get; set; }
        public string AgeRange { get; set; }
        public string? TakeNotes { get; set; }

        public DateTime PostTime { get; set; }

        public List<HINHTHUCTRALUONG> PaidTypes { get; set; }
        public List<KINHNGHIEM> Experiences { get; set; }
        public List<TINHTHANHPHO> Cities { get; set; }
        public List<LOAICONGVIEC> jobs { get; set; }

    }
}
