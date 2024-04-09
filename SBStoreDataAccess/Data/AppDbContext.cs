using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SBStore.Models;

namespace SBStore.DataAccess.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
           
    }
    public DbSet <Category> Categories { get; set; } //tworzymy tabelę
    public DbSet <Product> Products { get; set; } //tworzymy tabelę
    public DbSet <Company> Companies { get; set; } //tworzymy tabelę
    public DbSet <ShoppingCart> ShoppingCarts { get; set; } //tworzymy tabelę
    public DbSet<AppUser> AppUsers { get; set; } 
    protected override void OnModelCreating(ModelBuilder modelBuilder) //wstrzykujemy dane
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Horror", DisplayOrder = 1 },
            new Category { Id = 2, Name = "Poetry", DisplayOrder = 2 },
            new Category { Id = 3, Name = "Romance", DisplayOrder = 3 }
            );

        modelBuilder.Entity<Company>().HasData(
          new Company { Id = 1, Name = "Technology Solution", StreetAddress = "Tech St" , City = "Tech City", PostalCode="32-232",State= "Ill", PhoneNumber="23322323"},
          new Company { Id = 2, Name = "Biology Solution", StreetAddress = "Bio St", City = "Bio City", PostalCode = "32-233", State = "Maschs", PhoneNumber = "23322323" },
          new Company { Id = 3, Name = "Medical Solution", StreetAddress = "Med St", City = "Med City", PostalCode = "32-255", State = "Cal", PhoneNumber = "23322323" }
          );

        modelBuilder.Entity<Product>().HasData(
             new Product
             {
                 Id = 1,
                 Title = "Destiny",
                 Author = "Billy Spark",
                 Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                 ISBN = "SWD9999001",
                 ListPrice = 99,
                 Price = 90,
                 Price50 = 85,
                 Price100 = 80,
                 CategoryId = 1,
                 ImageURL=""
                 
             },
             new Product
             {
                 Id = 2,
                 Title = "Black Skies",
                 Author = "Nancy Hoover",
                 Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsumsitamet tincidunt. ",
                 ISBN = "CAW777777701",
                 ListPrice = 40,
                 Price = 30,
                 Price50 = 25,
                 Price100 = 20,
                 CategoryId = 1,
                 ImageURL = ""
             },
             new Product
             {
                 Id = 3,
                 Title = "Dark Times",
                 Author = "Julian Button",
                 Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsumsitamet tincidunt. ",
                 ISBN = "RITO5555501",
                 ListPrice = 55,
                 Price = 50,
                 Price50 = 40,
                 Price100 = 35,
                 CategoryId = 1,
                 ImageURL = ""
             },
             new Product
             {
                 Id = 4,
                 Title = "Wolves in the storm",
                 Author = "Abby Muscles",
                 Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsumsitamet tincidunt. ",
                 ISBN = "WS3333333301",
                 ListPrice = 70,
                 Price = 65,
                 Price50 = 60,
                 Price100 = 55,
                 CategoryId = 2,
                 ImageURL = ""
             },
             new Product
             {
                 Id = 5,
                 Title = "Witcher inside the Storm",
                 Author = "Ron Parker",
                 Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsumsitamet tincidunt. ",
                 ISBN = "SOTJ1111111101",
                 ListPrice = 30,
                 Price = 27,
                 Price50 = 25,
                 Price100 = 20,
                 CategoryId = 2,
                 ImageURL = ""
             },
             new Product
             {
                 Id = 6,
                 Title = "Wonderful Times",
                 Author = "Laura Phantom",
                 Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                 ISBN = "FOT000000001",
                 ListPrice = 25,
                 Price = 23,
                 Price50 = 22,
                 Price100 = 20,
                 CategoryId = 3,
                 ImageURL = ""
             }
            );
    }
}
