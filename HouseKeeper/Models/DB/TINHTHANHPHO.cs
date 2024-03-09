using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("TINHTHANHPHO", Schema = "dbo")]
    public class TINHTHANHPHO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_TINH_TP")]
        public int CityId { get; set; }
        [Column("TEN_TINH_TP")]
        public string CityName { get; set; }
    }
}
