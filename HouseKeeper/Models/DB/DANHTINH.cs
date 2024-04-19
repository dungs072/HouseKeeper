using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("DANHTINH", Schema = "dbo")]
    public class DANHTINH
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_CCCD")]
        public int CitizenId {  get; set; }
        [Column("CCCD")]
        public int CitizenNumber { get; set; }
        [Column("MATTRUOC")]
        public int FrontImage { get; set; }
        [Column("MATSAU")]
        public int BackImage { get; set; }
        [ForeignKey("ID_TRANGTHAIDANHTINH")]
        public virtual TRANGTHAIDANHTINH IdentityState { get; set; }
    }
}
