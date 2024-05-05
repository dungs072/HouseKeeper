using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("GOITIN", Schema = "dbo")]
    public class GOITIN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_GOITIN")]
        public int PricePacketId { get; set; }
        [Column("TENGOI")]
        public string PricePacketName { get; set; }
        [Column("GIA")]
        public decimal Price { get; set; }
        [Column("SOLUONGNGAY")]
        public int NumberDays { get; set; }
        public virtual ICollection<HOADON> PricePacketDetails { get; set; }
    }
}
