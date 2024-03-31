using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("LICHSUDAUGIA", Schema = "dbo")]
    public class LICHSUDAUGIA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_LS_DAUGIA")]
        public int BidHistoryId { get; set; }
        [Column("LUONGGIATANG")]
        public decimal IncreasePrice { get; set; }
        [Column("NGAYMUA")]
        public DateTime BuyDate { get; set; }
        [Column("DATHANHTOAN")]
        public bool IsPaid { get; set; }
        [ForeignKey("ID_TINTUYENDUNG")]
        public virtual TINTUYENDUNG Recruitment { get; set; }
    }
}
