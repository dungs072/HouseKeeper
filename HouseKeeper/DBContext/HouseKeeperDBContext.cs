using HouseKeeper.Models;
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

    }
}
