using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SBStore.DataAccess.Repository.IRepository;
using SBStore.Models;
using SBStoreWeb.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SBStore.Tests.Unit
{
    public class CategoryControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ICategoryRepository> _mockCategoryRepo;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockCategoryRepo = new Mock<ICategoryRepository>();

            _mockUow.Setup(u => u.Category).Returns(_mockCategoryRepo.Object);

            _controller = new CategoryController(_mockUow.Object);
        }
        [Fact]
        public void Index_ShouldReturnViewWithCategories()
        {
            // Arrange
            var categories = new List<Category> { new Category { Id = 1, Name = "Horror", DisplayOrder = 1 } };
            _mockCategoryRepo.Setup(r => r.GetAll(null, null)).Returns(categories);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().BeAssignableTo<List<Category>>();
            ((List<Category>)viewResult.Model).Should().HaveCount(1);
        }
        [Fact]
        public void Create_Get_ShouldReturnView()
        {
            // Act
            var result = _controller.Create();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public void Create_Post_ValidModel_ShouldCallAddAndRedirect()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Horror", DisplayOrder = 2 };

            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );

            // Act
            var result = _controller.Create(category);

            // Assert
            _mockCategoryRepo.Verify(r => r.Add(category), Times.Once);
            _mockUow.Verify(u => u.Save(), Times.Once);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("Index");
        }
        [Fact]
        public void Edit_Get_CategoryExists_ShouldReturnView()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Horror", DisplayOrder = 1 };

            _mockCategoryRepo
               .Setup(r => r.Get(
                   It.IsAny<Expression<Func<Category, bool>>>(),
                   null,
                   false))
               .Returns(category);

            // Act
            var result = _controller.Edit(1);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(category);
        }
        [Fact]
        public void Edit_Get_CategoryDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            _mockCategoryRepo
                .Setup(r => r.Get(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    null,
                    false))
                .Returns((Category)null);

            // Act
            var result = _controller.Edit(1);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public void Edit_Post_ValidModel_ShouldUpdateAndRedirect()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Horror", DisplayOrder = 2 };

            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );
            // Act
            var result = _controller.Edit(category);

            // Assert
            _mockCategoryRepo.Verify(r => r.Update(category), Times.Once);
            _mockUow.Verify(u => u.Save(), Times.Once);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("Index");
        }
        [Fact]
        public void DeletePOST_CategoryExists_ShouldDeleteAndRedirect()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Horror", DisplayOrder = 1 };
            _mockCategoryRepo
                .Setup(r => r.Get(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    null,
                    false))
                .Returns(category);

            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );

            // Act
            var result = _controller.DeletePOST(1);

            // Assert
            _mockCategoryRepo.Verify(r => r.Delete(category), Times.Once);
            _mockUow.Verify(u => u.Save(), Times.Once);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("Index");
        }
    }
}
