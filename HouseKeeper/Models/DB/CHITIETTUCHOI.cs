using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("CHITIETTUCHOI", Schema = "dbo")]
    public class CHITIETTUCHOI
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_CT_TUCHOI")]
        public int RejectionDetailId { get; set; }
        [Column("THOIGIAN")]
        public DateTime Time { get; set; }
        [Column("GHICHU")]
        public string? Note { get; set; }
        [ForeignKey("ID_TINTUYENDUNG")]
        public virtual TINTUYENDUNG Recruitment { get; set; }
        [ForeignKey("ID_LOAITUCHOI")]
        public virtual LYDOTUCHOI Rejection { get; set; }

    }
}
