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

        [ForeignKey("ID_GIUPVIEC")]
        public virtual NGUOIGIUPVIEC Employee { get; set; }
        [ForeignKey("ID_LOAICV")]
        public virtual LOAICONGVIEC Job { get; set; }
    }
}
