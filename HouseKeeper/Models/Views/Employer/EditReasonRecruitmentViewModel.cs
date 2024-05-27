using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Employer
{
    public class EditReasonRecruitmentViewModel
    {
        public int RecruitmentId { get; set; }
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


        public int PaidTypeId { get; set; }
        public int ExperienceId { get; set; }
        public int DistrictId { get; set; }
        public int CityId { get; set; }

        public string JobIds { get; set; }

        public List<LOAICONGVIEC> SelectedJobs { get; set; }

        public Dictionary<DateTime,List<CHITIETTUCHOI>> RejectionDetails { get; set; }

        public NHANVIEN Staff { get; set; }
    }
}
