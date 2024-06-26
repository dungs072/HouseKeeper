﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseKeeper.Models.DB
{
    [Table("HUYEN", Schema = "dbo")]
    public class HUYEN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_HUYEN")]
        public int DistrictId { get; set; }
        [Column("TENHUYEN")]
        public string DistrictName { get; set; }
        [ForeignKey("ID_TINH_TP")]
        public virtual TINHTHANHPHO City { get; set; }

        public virtual ICollection<TINTUYENDUNG> Recruitments { get; set; }
        public virtual ICollection<NGUOITHUE> Employers { get; set; }
        public virtual ICollection<NGUOIGIUPVIEC> Employees { get; set; }
        public virtual ICollection<CHITIETNOICOTHELAMVIEC> WorkableDetails { get; set; }
    }
}
