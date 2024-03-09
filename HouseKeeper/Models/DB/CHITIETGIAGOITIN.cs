using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("CHITIETGIAGOITIN", Schema = "dbo")]
    public class CHITIETGIAGOITIN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_CTGGT")]
        public int PricePacketDetailId { get; set; }
        [Column("NGAYMUA")]
        public DateTime BuyDate { get; set; }
        [Column("DATHANHTOAN")]
        public bool HasPaid { get; set; }

        public virtual GIAGOITIN PricePacket { get; set; }
        public virtual TINTUYENDUNG Recruitment { get; set; }
    }
}
