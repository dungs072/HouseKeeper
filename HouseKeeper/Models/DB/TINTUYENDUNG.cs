using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("TINTUYENDUNG", Schema = "dbo")]
    public class TINTUYENDUNG
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_TINTUYENDUNG")]
        public int RecruitmentId { get; set; }
        [Column("FULLTIME")]
        public bool FullTime { get; set; }
        [Column("MINLUONG")]
        public bool MinSalary { get; set; }
        [Column("MAXLUONG")]
        public bool MaxSalary { get; set; }
        [Column("GIOITINH")]
        public string? Gender { get; set; }
        [Column("SOLUONGTUYENDUNG")]
        public int MaxApplications { get; set; }
        [Column("DOTUOI")]
        public string Age { get; set; }
        [Column("GHICHU")]
        public string? Note { get; set; }
        [Column("THOIGIANDANG")]
        public DateTime PostTime { get; set; }
        [Column("HANTUYENDUNG")]
        public DateTime? RecruitDeadlineDate { get; set; }
        [Column("TRANGTHAI")]
        public bool Status { get; set; }
        [Column("DAUGIA")]
        public bool BidPrice { get; set; }

        [ForeignKey("ID_HTTL")]
        public virtual HINHTHUCTRALUONG SalaryForm { get; set; }
        [ForeignKey("ID_KINHNGHIEM")]
        public virtual KINHNGHIEM Experience { get; set; }
        [ForeignKey("ID_TINHTHANHPHO")]
        public virtual TINHTHANHPHO City { get; set; }
        [ForeignKey("ID_NGUOITHUE")]
        public virtual NGUOITHUE Employer { get; set; }


        public virtual ICollection<CHITIETAPPLY> ApplyDetails { get; set; }
        public virtual ICollection<CHITIETGIAGOITIN> PricePacketDetail { get; set; }
    }
}
