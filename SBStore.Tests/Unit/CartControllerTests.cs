using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SBStore.DataAccess.Repository.IRepository;
using SBStoreWeb.Areas.Customer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using SBStore.Models.ViewModels;
using SBStore.Models;
using System.Linq.Expressions;
using System.Linq;
using SBStore.Utility;

namespace SBStore.Tests.Unit
{
    public class CartControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IShoppingCartRepository> _mockCartRepo;
        private readonly Mock<IAppUserRepository> _mockAppUserRepo;
        private readonly Mock<IProductRepository> _mockProductRepo;
        private readonly Mock<IProductImageRepository> _mockProductImageRepo;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockCartRepo = new Mock<IShoppingCartRepository>();
            _mockAppUserRepo = new Mock<IAppUserRepository>();
            _mockProductRepo = new Mock<IProductRepository>();
            _mockProductImageRepo = new Mock<IProductImageRepository>();

            _mockUow.Setup(u => u.ShoppingCart).Returns(_mockCartRepo.Object);
            _mockUow.Setup(u => u.AppUser).Returns(_mockAppUserRepo.Object);
            _mockUow.Setup(u => u.Product).Returns(_mockProductRepo.Object);
            _mockUow.Setup(u => u.ProductImage).Returns(_mockProductImageRepo.Object);

            _controller = new CartController(_mockUow.Object);


            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
           {
                new Claim(ClaimTypes.NameIdentifier, "user1")
           }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            _controller.TempData = new TempDataDictionary(
                _controller.ControllerContext.HttpContext,
                Mock.Of<ITempDataProvider>()
            );
           
        }
        [Fact]
        public void Index_ShouldReturnViewWithShoppingCartVM()
        {
            // Arrange
            var cartList = new List<ShoppingCart>
            {
                new ShoppingCart { Id = 1, ProductId = 1, Count = 2, Product = new Product { Title = "Test Product", Price = 10 } }
            };

            var productImages = new List<ProductImage>
            {
                new ProductImage { ProductId = 1, ImageUrl = "test.png" }
            };

            _mockCartRepo.Setup(r => r.GetAll(It.IsAny<Expression<Func<ShoppingCart, bool>>>(), "Product"))
                .Returns(cartList);
            _mockProductImageRepo.Setup(r => r.GetAll(It.IsAny<Expression<Func<ProductImage, bool>>>(), It.IsAny<string>()))
                                 .Returns(productImages);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().BeOfType<ShoppingCartVM>();

            var model = (ShoppingCartVM)viewResult.Model;
            model.ShoppingCartList.Should().HaveCount(1);

            model.ShoppingCartList.First().Product.ProductImages.Should().HaveCount(1);

            model.ShoppingCartList.First().Price.Should().Be(10);
            model.OrderHeader.OrderTotal.Should().Be(20);
        }
        [Fact]
        public void Summary_ShouldReturnViewWithShoppingCartVM()
        {
            // Arrange
            var userId = "user1";

            var cartList = new List<ShoppingCart>
            {
                new ShoppingCart
                {
                    Id = 1,
                    ProductId = 1,
                    Count = 2,
                    Product = new Product { Title = "Test Product", Price = 10 }
                }
            };
            var productImages = new List<ProductImage>
            {
                new ProductImage { ProductId = 1, ImageUrl = "test.png" }
            };

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

            _mockCartRepo
                .Setup(r => r.GetAll(It.IsAny<Expression<Func<ShoppingCart, bool>>>(), "Product"))
                .Returns(cartList);

            _mockAppUserRepo
                .Setup(r => r.Get(It.IsAny<Expression<Func<AppUser, bool>>>(), null, false))
                .Returns(user);

            // Act
            var result = _controller.Summary();

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().BeOfType<ShoppingCartVM>();

            var model = (ShoppingCartVM)viewResult.Model;

            model.ShoppingCartList.Should().HaveCount(1);
            model.ShoppingCartList.First().Price.Should().Be(10);
            model.OrderHeader.OrderTotal.Should().Be(20);

            // Sprawdzenie danych użytkownika w OrderHeader
            model.OrderHeader.Name.Should().Be("John Doe");
            model.OrderHeader.PhoneNumber.Should().Be("123456789");
            model.OrderHeader.StreetAddress.Should().Be("Test Street");
            model.OrderHeader.City.Should().Be("Test City");
            model.OrderHeader.State.Should().Be("Test State");
            model.OrderHeader.PostalCode.Should().Be("12345");
        }
        [Fact]
        public void OrderConfirmation_ShouldClearCartAndReturnView()
        {
            // Arrange
            int orderId = 1;
            string userId = "user1";

            var orderHeader = new OrderHeader
            {
                Id = orderId,
                AppUserId = userId,
                PaymentStatus = SD.PaymentStatusDelayedPayment, // skip stripe
            };

            var shoppingCartItems = new List<ShoppingCart>
            {
                new ShoppingCart { Id = 1, AppUserId = userId, ProductId = 1, Count = 2 },
                new ShoppingCart { Id = 2, AppUserId = userId, ProductId = 2, Count = 1 }
            };

            _mockUow.Setup(u => u.OrderHeader.Get(
                    It.IsAny<Expression<Func<OrderHeader, bool>>>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()
                ))
                .Returns(orderHeader);

            _mockCartRepo.Setup(r => r.GetAll(It.IsAny<Expression<Func<ShoppingCart, bool>>>(), null))
                         .Returns(shoppingCartItems);

            // Act
            var result = _controller.OrderConfirmation(orderId);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(orderId);

            _mockCartRepo.Verify(r => r.DeleteRange(shoppingCartItems), Times.Once);
            _mockUow.Verify(u => u.Save(), Times.AtLeastOnce);
        }
    }
}
