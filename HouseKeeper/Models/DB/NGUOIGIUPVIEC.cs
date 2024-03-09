using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("NGUOIGIUPVIEC", Schema = "dbo")]
    public class NGUOIGIUPVIEC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_GIUPVIEC")]
        public int EmployeeId { get; set; }
        [Column("HO")]
        public string LastName { get; set; }
        [Column("TEN")]
        public string FirstName { get; set; }
        [Column("NGAYSINH")]
        public DateTime BirthDate { get; set; }

        [ForeignKey("ID_TINH_TP")]
        public virtual TINHTHANHPHO City { get; set; }

        [ForeignKey("ID_TK")]
        public virtual TAIKHOAN Account { get; set; }

        public virtual ICollection<CHITIETCONGVIEC> JobDetails { get; set; }
    }
}
