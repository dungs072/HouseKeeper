using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Staff
{
    public class RecruitmentModerationViewModel
    {
        public TINTUYENDUNG? Recruitment { get; set; }
        public List<LYDOTUCHOI>? Rejections { get; set; }
        public Dictionary<DateTime, List<CHITIETTUCHOI>>? RejectionsDetails { get; set; }

        public List<int>? RejectionId { get; set; }
        public List<string>? NoteList { get; set; }
        public List<bool>? IsSelectedList { get; set; }

        public List<int> NoteIdCanEditList { get; set; }
        public List<string?> NoteCanEditList { get; set; }

        public int RecruitmentId { get; set; }
        public int StaffId { get; set; }

        public int NoteIndexCanEdit { get; set; }
        
        public DateTime LastTimeCanEditNotes { get; set; }

        public string getRejectionName(int rejectionId)
        {
            foreach (var rejection in Rejections)
            {
                if (rejection.RejectionId == rejectionId)
                {
                    return rejection.RejectionName;
                }
            }
            return "";
        }


    }
}
