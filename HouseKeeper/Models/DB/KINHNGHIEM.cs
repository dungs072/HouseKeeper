using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HouseKeeper.Models.DB
{
    [Table("KINHNGHIEM", Schema = "dbo")]
    public class KINHNGHIEM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_KINHNGHIEM")]
        public int ExperienceId { get; set; }
        [Column("TEN_KINHNGHIEM")]
        public string ExperienceName { get; set; }
    }
}
