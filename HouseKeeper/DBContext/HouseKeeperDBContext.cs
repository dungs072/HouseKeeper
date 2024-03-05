using Microsoft.EntityFrameworkCore;

namespace HouseKeeper.DBContext
{
    public class HouseKeeperDBContext:DbContext
    {
        public HouseKeeperDBContext(DbContextOptions options):base(options) 
        {

        }
        
    }
}
