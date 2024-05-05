using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("HOADON", Schema = "dbo")]
    public class HOADON
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_HOADON")]
        public int PricePacketDetailId { get; set; }
        [Column("NGAYMUA")]
        public DateTime BuyDate { get; set; }
        [Column("DATHANHTOAN")]
        public bool HasPaid { get; set; }

        [ForeignKey("ID_GOITIN")]
        public virtual GOITIN PricePacket { get; set; }
        [ForeignKey("ID_TINTUYENDUNG")]
        public virtual TINTUYENDUNG Recruitment { get; set; }
    }
}
