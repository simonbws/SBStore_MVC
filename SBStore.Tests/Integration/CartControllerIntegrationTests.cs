using Microsoft.EntityFrameworkCore;
using SBStore.DataAccess.Data;
using SBStore.DataAccess.Repository.IRepository;
using SBStore.DataAccess.Repository;
using SBStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBStoreWeb.Areas.Customer.Controllers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FluentAssertions;
using SBStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace SBStore.Tests.Integration
{
    public class CartControllerIntegrationTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
           .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
           .Options;

            return new AppDbContext(options);
        }
        [Fact]
        public void Index_ShouldReturnShoppingCartVM_WithProducts()
        {
            // Arrange
            var context = GetDbContext();
            
            var category = new Category { Id = 1, Name = "TestCategory" };
            context.Categories.Add(category);
            
            var user = new AppUser { Id = "user1", Name = "John Doe" };
            context.AppUsers.Add(user);

            
            var product = new Product
            {
                Id = 1,
                Title = "Test Product",
                Description = "Test Description",
                ISBN = "1234567890",
                Author = "Test Author",
                ListPrice = 100,
                Price = 90,
                Price50 = 85,
                Price100 = 80,
                CategoryId = category.Id,
                Category = category
            };
            context.Products.Add(product);
            var cartItem = new ShoppingCart { Id = 1, AppUserId = user.Id, ProductId = product.Id, Count = 2 };
            context.ShoppingCarts.Add(cartItem);
            context.SaveChanges();

            IUnitOfWork uow = new UnitOfWork(context);

            var controller = new CartController(uow);
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }, "mock"));

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userClaims }
            };

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = result as Microsoft.AspNetCore.Mvc.ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ShoppingCartVM;
            model.Should().NotBeNull();
            model.ShoppingCartList.Should().HaveCount(1);
            model.ShoppingCartList.First().Product.Title.Should().Be("Test Product");
            model.ShoppingCartList.First().Count.Should().Be(2);
            model.OrderHeader.OrderTotal.Should().Be(180); // 90 * 2
        }
        [Fact]
        public void Summary_ShouldReturnShoppingCartVM_WithUserAndCartData()
        {
            // Arrange
            var context = GetDbContext();
            var category = new Category { Id = 1, Name = "TestCategory" };
            context.Categories.Add(category);

            var product = new Product
            {
                Id = 1,
                Title = "Test Product",
                Description = "Test Description",
                ISBN = "1234567890",
                Author = "Test Author",
                ListPrice = 100,
                Price = 90,
                Price50 = 85,
                Price100 = 80,
                CategoryId = category.Id,
                Category = category
            };
            context.Products.Add(product);

            var user = new AppUser
            {
                Id = "user1",
                Name = "John Doe",
                PhoneNumber = "123456789",
                StreetAddress = "Test Street",
                City = "Test City",
                State = "Test State",
                PostalCode = "12345"
            };
            context.AppUsers.Add(user);

            var cartItem = new ShoppingCart
            {
                Id = 1,
                AppUserId = user.Id,
                ProductId = product.Id,
                Count = 2
            };
            context.ShoppingCarts.Add(cartItem);

            context.SaveChanges();

            IUnitOfWork uow = new UnitOfWork(context);
            var controller = new CartController(uow);

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userClaims }
            };

            // Act
            var result = controller.Summary();

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ShoppingCartVM;
            model.Should().NotBeNull();

            model.ShoppingCartList.Should().HaveCount(1);
            var cart = model.ShoppingCartList.First();
            cart.Product.Title.Should().Be("Test Product");
            cart.Count.Should().Be(2);
            cart.Price.Should().Be(90);

            model.OrderHeader.OrderTotal.Should().Be(180);

            model.OrderHeader.Name.Should().Be("John Doe");
            model.OrderHeader.PhoneNumber.Should().Be("123456789");
            model.OrderHeader.StreetAddress.Should().Be("Test Street");
            model.OrderHeader.City.Should().Be("Test City");
            model.OrderHeader.State.Should().Be("Test State");
            model.OrderHeader.PostalCode.Should().Be("12345");
        }
    }
}
