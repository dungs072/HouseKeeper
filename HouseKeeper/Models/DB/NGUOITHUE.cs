using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("NGUOITHUE", Schema = "dbo")]
    public class NGUOITHUE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_NGUOITHUE")]
        public int EmployerId { get; set; }
        [Column("HO")]
        public string LastName { get; set; }
        [Column("TEN")]
        public string FirstName { get; set; }
        [Column("SONHA_DUONG")]
        public string? Address { get; set; }

        [ForeignKey("ID_TK")]
        public virtual TAIKHOAN Account { get; set; }
        [ForeignKey("ID_CCCD")]
        public virtual DANHTINH Identity { get; set; }
        [ForeignKey("ID_HUYEN")]
        public virtual HUYEN District { get; set; }
        [ForeignKey("ID_TRANGTHAIDANHTINH")]
        public virtual TRANGTHAIDANHTINH IdentityState { get; set; }
    }
}
