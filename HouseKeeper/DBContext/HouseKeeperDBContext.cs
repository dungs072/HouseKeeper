using HouseKeeper.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace HouseKeeper.DBContext
{
    public class HouseKeeperDBContext:DbContext
    {
        public HouseKeeperDBContext(DbContextOptions options):base(options) 
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
        public DbSet<TAIKHOAN> Accounts { get; set; }
        public DbSet<LOAITK> AccountTypes { get; set; }
        public DbSet<CHITIETAPPLY> ApplyDetails { get; set; }
        public DbSet<CHITIETCONGVIEC> JobDetails { get; set; }
        public DbSet<CHITIETLOAIGIUPVIEC> HouseWorkDetails { get; set; }
        public DbSet<CHITIETNOICOTHELAMVIEC> WorkplacesDetails { get; set; }
        public DbSet<HINHTHUCTRALUONG> SalaryForms { get; set; }
        public DbSet<KINHNGHIEM> Experiences { get; set; }
        public DbSet<LOAICONGVIEC> Jobs { get; set; }
        public DbSet<NGUOIGIUPVIEC> Employees { get; set; }
        public DbSet<NGUOITHUE> Employers { get; set; }
        public DbSet<TINHTHANHPHO> Cities { get; set; }
        public DbSet<TINTUYENDUNG> Recruitments { get; set; }
        public DbSet<GIAGOITIN> PricePackets { get; set; }
        public DbSet<CHITIETGIAGOITIN> PricePacketDetails { get; set; }
        public DbSet<NHANVIEN> Staffs { get; set; }
        public DbSet<DANHTINH> Identities { get; set; }
        public DbSet<HUYEN> Districts { get; set; } 
        public DbSet<TRANGTHAITIN> RecruitmentStatus { get; set; }
        public DbSet<LICHSUDAUGIA> BidHistories { get; set; }
        public DbSet<LYDOTUCHOI> Rejections { get; set; }
        public DbSet<CHITIETTUCHOI> RejectionDetails { get; set; }
        public DbSet<TRANGTHAIDANHTINH> IdentityStates { get; set; }

    }
}
