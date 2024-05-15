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
        public string CitizenNumber { get; set; }

        [Column("MATTRUOC")]
        public string FrontImage { get; set; }
        [Column("MATSAU")]
        public string BackImage { get; set; }
    }
}
