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
        [Column("DIACHI")]
        public string Address { get; set; }

        [ForeignKey("ID_TK")]
        public virtual TAIKHOAN Account { get; set; }
    }
}
