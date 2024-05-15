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
        [Column("GIOITINH")]
        public string Gender { get; set; }
        [Column("NGAYSINH", TypeName = "date")]
        public DateTime? BirthDate { get; set; }
        [Column("SONHA_DUONG")]
        public string? Address { get; set; }

        [ForeignKey("ID_HUYEN")]
        public virtual HUYEN District { get; set; }

        [ForeignKey("ID_TK")]
        public virtual TAIKHOAN Account { get; set; }
        [ForeignKey("ID_CCCD")]
        public virtual DANHTINH Identity { get; set; }

        [ForeignKey("ID_TRANGTHAIDANHTINH")]
        public virtual TRANGTHAIDANHTINH? IdentityState { get; set; }

        public virtual ICollection<CHITIETCONGVIEC> JobDetails { get; set; }
        public virtual ICollection<CHITIETNOICOTHELAMVIEC> WorkPlacesDetails { get; set; }
    }
}
