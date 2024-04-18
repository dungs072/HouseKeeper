using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("TRANGTHAIDANHTINH", Schema = "dbo")]
    public class TRANGTHAIDANHTINH
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_TRANGTHAIDANHTINH")]
        public int IdentityStateId { get; set; }
        [Column("TEN_TRANGTHAI")]
        public string IdentityStateName { get; set; }
    }
}
