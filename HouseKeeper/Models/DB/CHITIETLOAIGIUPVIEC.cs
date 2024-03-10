using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("CHITIETLOAIGIUPVIEC", Schema = "dbo")]
    public class CHITIETLOAIGIUPVIEC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_CTGV")]
        public int HouseworkDetailId { get; set; }
        [ForeignKey("ID_LOAICV")]
        public virtual LOAICONGVIEC Job { get; set; }
        [ForeignKey("ID_TINTUYENDUNG")]
        public virtual TINTUYENDUNG Recruitment { get; set; }
    }
}
