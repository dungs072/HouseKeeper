using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("CHITIETNOICOTHELAMVIEC", Schema = "dbo")]
    public class CHITIETNOICOTHELAMVIEC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_CT_NOILAMVIEC")]
        public int WorkPlaceId { get; set; }
        [ForeignKey("ID_HUYEN")]
        public virtual HUYEN District { get; set; }
        [ForeignKey("ID_TINTUYENDUNG")]
        public virtual TINTUYENDUNG Recruitment { get; set; }
    }
}
