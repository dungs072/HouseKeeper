using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("LYDOTUCHOI", Schema = "dbo")]
    public class LYDOTUCHOI
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_LOAITUCHOI")]
        public int RejectionId { get; set; }
        [Column("LYDO")]
        public string RejectionName { get; set; }
        public virtual ICollection<CHITIETTUCHOI> RejectionDetails { get; set; }
    }
}
