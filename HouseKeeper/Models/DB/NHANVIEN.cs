using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("NHANVIEN", Schema = "dbo")]
    public class NHANVIEN
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_NHANVIEN")]
        public int StaffId { get; set; }
        [Column("HO")]
        public string LastName { get; set; }
        [Column("TEN")]
        public string FirstName { get; set; }
        [ForeignKey("ID_TK")]
        public virtual TAIKHOAN Account { get; set; }
        [ForeignKey("ID_CCCD")]
        public virtual DANHTINH Identity { get; set; }
    }
}
