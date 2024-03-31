using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("TRANGTHAITIN", Schema = "dbo")]
    public class TRANGTHAITIN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_TRANGTHAI_TIN")]
        public int StatusId { get; set; }
        [Column("TEN_TRANGTHAI")]
        public string StatusName { get; set; }
    }
}
