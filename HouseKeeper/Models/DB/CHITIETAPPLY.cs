using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("CHITIETAPPLY", Schema = "dbo")]
    public class CHITIETAPPLY
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_CT_APPLY")]
        public int ApplyDetailId { get; set; }
        [Column("THOIGIAN")]
        public DateTime Time { get; set; }
        [ForeignKey("ID_GIUPVIEC")]
        public virtual NGUOIGIUPVIEC Employee { get; set; }
        [ForeignKey("ID_TINTUYENDUNG")]
        public virtual TINTUYENDUNG Recruitment { get; set; }


    }
}
