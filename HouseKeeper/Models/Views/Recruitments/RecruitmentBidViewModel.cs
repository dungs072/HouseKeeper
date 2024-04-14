using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.Views.Recruitments
{
    public class RecruitmentBidViewModel
    {
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public int MaxApplications { get; set; }
        public DateTime PostTime { get; set; }
        public DateTime RecruitDeadlineDate { get; set; }
        public decimal BidPrice { get; set; }
        public string RecruiterName { get; set; }   
    }
}
