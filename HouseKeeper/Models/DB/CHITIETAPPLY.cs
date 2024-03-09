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
        public DateTime Time { get; set; }
        public string State { get; set; }
        public virtual NGUOIGIUPVIEC Employee { get; set; }
        public virtual TINTUYENDUNG Recruitment { get; set; }


    }
}
