﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("GIAGOITIN", Schema = "dbo")]
    public class GIAGOITIN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_GIAGOITIN")]
        public int PricePacketId { get; set; }
        [Column("TENGOI")]
        public string PricePacketName { get; set; }
        [Column("GIA")]
        public decimal Price { get; set; }
        [Column("SOLUONGNGAY")]
        public int NumberDays { get; set; }
        public virtual ICollection<CHITIETGIAGOITIN> PricePacketDetails { get; set; }
    }
}
