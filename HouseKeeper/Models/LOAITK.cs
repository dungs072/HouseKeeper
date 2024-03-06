using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models
{
    [Table("LOAI_TK", Schema = "dbo")]
    public class LOAITK
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_LOAI")]
        public int AccountTypeId { get; set; }
        [Column("TEN_LOAI")]
        public string TypeName { get; set; }
    }
}
