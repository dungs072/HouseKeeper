using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("HINHTHUCTRALUONG", Schema = "dbo")]
    public class HINHTHUCTRALUONG
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_HTTL")]
        public int SalaryFormId { get; set; }
        [Column("TENHINHTHUC")]
        public string SalaryFormName { get; set; }
    }
}
