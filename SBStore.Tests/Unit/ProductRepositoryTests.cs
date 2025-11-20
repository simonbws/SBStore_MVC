using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SBStore.DataAccess.Data;
using SBStore.DataAccess.Repository;
using SBStore.Models;

namespace SBStore.Tests.Unit
{
    public class ProductRepositoryTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var context = new AppDbContext(options);
            // Seed danych testowych
            context.Products.Add(new Product
            {
                Id = 1,
                Title = "Test Product 1",
                Description = "Test description",
                ISBN = "ISBN1",
                Author = "Author1",
                ListPrice = 50,
                Price = 45,
                Price50 = 40,
                Price100 = 35,
                CategoryId = 1
            });
            context.SaveChanges();

            return context;
        }
        [Fact]
        public void GetAll_ShouldReturnAllProducts()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repo = new ProductRepository(context);

            // act
            var products = repo.GetAll(p => true);

            // Assert
            products.Should().HaveCount(1);
            products.Should().ContainSingle(p => p.Title == "Test Product 1");

        }
        [Fact]
        public void Get_ShouldReturnSingleProduct()
        {
            // arrange
            var context = GetInMemoryDbContext();
            var repo = new ProductRepository(context);
            // act
            var product = repo.Get(p => p.Id == 1);
            // Assert
            product.Should().NotBeNull();
            product.Title.Should().Be("Test Product 1");

        }
        [Fact]
        public void GetAll_WithIncludes_ShouldReturnWithCategory()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Categories.Add(new Category
            {
                Id = 1,
                Name = "Horror",
                DisplayOrder = 1
            });
            context.SaveChanges();
            var repo = new ProductRepository(context);

            // Act
            var products = repo.GetAll(p => true, includeProperties: "Category");

            // Assert
            products.Should().HaveCount(1);
            products.First().Category.Should().NotBeNull();
            products.First().Category.Name.Should().Be("Horror");
        }
        [Fact]
        public void Add_ShouldInsertProduct()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repo = new ProductRepository(context);

            var newProduct = new Product
            {
                Id = 2,
                Title = "New Product",
                Description = "New product description",
                ISBN = "ISBN2",
                Author = "Author2",
                ListPrice = 60,
                Price = 55,
                Price50 = 50,
                Price100 = 45,
                CategoryId = 1
            };

            // Act
            repo.Add(newProduct);
            context.SaveChanges(); // EF zapisuje do InMemory

            // Assert
            var productFromDb = context.Products.Find(2);
            productFromDb.Should().NotBeNull();
            productFromDb.Title.Should().Be("New Product");
            productFromDb.Description.Should().Be("New product description");
        }
        [Fact]
        public void Add_ShouldFail_WhenTitleIsNull()
        {
            var context = GetInMemoryDbContext();
            var repo = new ProductRepository(context);

            var invalidProduct = new Product
            {
                Id = 10,
                Title = null,   // <-- powinno rzuciæ
                Description = "desc",
                ISBN = "ISBN",
                Author = "Author",
                ListPrice = 10,
                Price = 9,
                Price50 = 8,
                Price100 = 7,
                CategoryId = 1
            };
            repo.Add(invalidProduct);

            // Act + Assert
            Action act = () => context.SaveChanges();
            act.Should().Throw<DbUpdateException>();
        }
            [Fact]
        public void Update_ShouldModifyExistingProduct()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repo = new ProductRepository(context);

            var existingProduct = context.Products.Find(1);
            existingProduct.Title = "Updated Title";
            existingProduct.Description = "Updated description";
            existingProduct.ISBN = existingProduct.ISBN; // kopiujemy stare wartoœci
            existingProduct.Author = existingProduct.Author;
            existingProduct.ListPrice = existingProduct.ListPrice;
            existingProduct.Price = existingProduct.Price;
            existingProduct.Price50 = existingProduct.Price50;
            existingProduct.Price100 = existingProduct.Price100;
            existingProduct.CategoryId = existingProduct.CategoryId;

            // Act
            repo.Update(existingProduct);
            context.SaveChanges();

            // Assert
            var productFromDb = context.Products.Find(1);
            productFromDb.Title.Should().Be("Updated Title");
            productFromDb.Description.Should().Be("Updated description");
        }
        [Fact]
        public void Delete_ShouldRemoveProduct()
        {
            // arrange
            var context = GetInMemoryDbContext();
            var repo = new ProductRepository(context);

            var product = context.Products.Find(1);

            // act
            repo.Delete(product);
            context.SaveChanges();

            // Assert
            var deleted = context.Products.Find(1);
            deleted.Should().BeNull();
        }
        [Fact]
        public void DeleteRange_ShouldRemoveMultipleProducts()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repo = new ProductRepository(context);

            context.Products.Add(new Product
            {
                Id = 2,
                Title = "P2",
                Description = "D2",
                ISBN = "I2",
                Author = "A2",
                ListPrice = 10,
                Price = 9,
                Price50 = 8,
                Price100 = 7,
                CategoryId = 1
            });
            context.Products.Add(new Product
            {
                Id = 3,
                Title = "P3",
                Description = "D3",
                ISBN = "I3",
                Author = "A3",
                ListPrice = 20,
                Price = 19,
                Price50 = 18,
                Price100 = 17,
                CategoryId = 1
            });
            context.SaveChanges();

            var twoProducts = context.Products.Where(p => p.Id == 2 || p.Id == 3);

            // Act
            repo.DeleteRange(twoProducts);
            context.SaveChanges();

            // Assert
            context.Products.Find(2).Should().BeNull();
            context.Products.Find(3).Should().BeNull();
        }
    }

}