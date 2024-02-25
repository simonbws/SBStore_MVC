using Microsoft.EntityFrameworkCore;

namespace SBStoreWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
               
        }
    }
}
