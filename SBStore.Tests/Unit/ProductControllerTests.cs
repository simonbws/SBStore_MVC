using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SBStore.DataAccess.Repository.IRepository;
using SBStore.Models;
using SBStore.Models.ViewModels;
using SBStoreWeb.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SBStore.Tests.Unit
{
    
    public class ProductControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IProductRepository> _mockProductRepo;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private  ProductController _controller;

        public ProductControllerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockProductRepo = new Mock<IProductRepository>();
            _mockEnv = new Mock<IWebHostEnvironment>();

            _mockUow.Setup(u => u.Product).Returns(_mockProductRepo.Object);
            _controller = new ProductController(_mockUow.Object, _mockEnv.Object);
        }
        [Fact]
        public void Index_ShouldReturnViewWithListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Title = "Book A", ISBN = "ISBN-A", Author = "Author A", ListPrice = 10, Price = 9, Price50 = 8, Price100 = 7, CategoryId = 1 },
                new Product { Id = 2, Title = "Book B", ISBN = "ISBN-B", Author = "Author B", ListPrice = 20, Price = 18, Price50 = 17, Price100 = 15, CategoryId = 2 }
            };

            _mockProductRepo
                .Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>(), "Category"))
                .Returns(products);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull("Index should return ViewResult");

            viewResult.Model.Should().BeAssignableTo<List<Product>>();
            var modelList = (List<Product>)viewResult.Model;
            modelList.Should().HaveCount(2);
            modelList.First().Title.Should().Be("Book A");

            _mockProductRepo.Verify(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>(), "Category"), Times.Once);
        }
        [Fact]
        public void Upsert_Get_ShouldReturnCreateView_WhenIdIsNull()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Cat1" },
                new Category { Id = 2, Name = "Cat2" }
            };

            _mockUow.Setup(u => u.Category.GetAll(null, null))
                    .Returns(categories);

            // Act
            var result = _controller.Upsert(null);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();

            viewResult.Model.Should().BeOfType<ProductViewModel>();
            var model = (ProductViewModel)viewResult.Model;

            model.Product.Should().NotBeNull();
            model.Product.Id.Should().Be(0); // new product

            model.CategoryList.Should().HaveCount(2);

            // verify repo call
            _mockUow.Verify(u => u.Category.GetAll(null, null), Times.Once);
        }
        [Fact]
        public void Upsert_Get_ShouldReturnEditView_WhenIdIsProvided()
        {
            // Arrange
            int id = 1;

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Cat1" }
            };

            var product = new Product
            {
                Id = 1,
                Title = "Test Product",
                ProductImages = new List<ProductImage>
                {
                    new ProductImage { Id = 10, ProductId = 1, ImageUrl = "a.png" }
                }
            };
            _mockUow.Setup(u => u.Category.GetAll(null, null))
           .Returns(categories);

            _mockUow.Setup(u => u.Product.Get(
            It.IsAny<Expression<Func<Product, bool>>>(),
            "ProductImages",
            false))
            .Returns(product);

            // Act
            var result = _controller.Upsert(id);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();

            viewResult.Model.Should().BeOfType<ProductViewModel>();
            var model = (ProductViewModel)viewResult.Model;

            model.Product.Id.Should().Be(1);
            model.Product.Title.Should().Be("Test Product");

            model.Product.ProductImages.Should().HaveCount(1);

            model.CategoryList.Should().HaveCount(1);

            _mockUow.Verify(u => u.Product.Get(
                It.IsAny<Expression<Func<Product, bool>>>(),
                "ProductImages",
                false), Times.Once);

            _mockUow.Verify(u => u.Category.GetAll(null, null), Times.Once);
        }
        [Fact]
        public void Upsert_Post_ShouldAddProductAndRedirect_WhenModelIsValid()
        {
            // Arrange
            var productViewModel = new ProductViewModel
            {
                Product = new Product { Id = 0, Title = "New Product" }
            };
            List<IFormFile> files = null; // brak plików

            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(e => e.WebRootPath).Returns("C:\\FakeWebRoot");
            _controller = new ProductController(_mockUow.Object, mockEnv.Object);

            _controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>()
            );

            // Act
            var result = _controller.Upsert(productViewModel, files);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("Index");

            // Verify
            _mockUow.Verify(u => u.Product.Add(productViewModel.Product), Times.Once);
            _mockUow.Verify(u => u.Save(), Times.Once);
        }

    }
}
