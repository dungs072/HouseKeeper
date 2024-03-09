using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("LOAICONGVIEC", Schema = "dbo")]
    public class LOAICONGVIEC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_LOAICV")]
        public int JobId { get; set; }
        [Column("TEN_CV")]
        public string JobName { get; set; }
    }
}
