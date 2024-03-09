using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("CHITIETCONGVIEC", Schema = "dbo")]
    public class CHITIETCONGVIEC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_CTCV")]
        public int JobDetailID { get; set; }

        public virtual NGUOIGIUPVIEC Employee { get; set; }
        public virtual LOAICONGVIEC Job { get; set; }
    }
}
