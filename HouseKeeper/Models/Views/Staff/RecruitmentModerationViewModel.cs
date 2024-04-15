using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Staff
{
    public class RecruitmentModerationViewModel
    {
        public TINTUYENDUNG? Recruitment { get; set; }
        public List<LYDOTUCHOI>? Rejections { get; set; }

        public List<int>? RejectionId { get; set; }
        public List<string>? NoteList { get; set; }
        public List<bool>? IsSelectedList { get; set; }

        public int RecruitmentId { get; set; }
        public int StaffId { get; set; }
    }
}
