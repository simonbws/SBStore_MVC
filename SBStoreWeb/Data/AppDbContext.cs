using Microsoft.EntityFrameworkCore;
using SBStoreWeb.Models;

namespace SBStoreWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
               
        }

        public DbSet <Category> Categories { get; set; } //tworzymy tabelę

        protected override void OnModelCreating(ModelBuilder modelBuilder) //wstrzykujemy dane
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Horror", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Poetry", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Romance", DisplayOrder = 3 }
                );
        }
    }
}
