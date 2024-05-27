using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("TAIKHOAN", Schema = "dbo")]
    public class TAIKHOAN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_TK")]
        public int AccountID { get; set; }
        [Column("SDT")]
        public string PhoneNumber { get; set; }
        [Column("GMAIL")]
        public string Gmail { get; set; }
        [Column("MATKHAU")]
        public string Password { get; set; }
        [Column("HINHDAIDIEN")]
        public string? AvatarUrl { get; set; }
        [Column("TRANGTHAI")]
        public bool Status { get; set; }
        [ForeignKey("ID_LOAI")]
        public virtual LOAITK AccountType { get; set; }

    }
}
