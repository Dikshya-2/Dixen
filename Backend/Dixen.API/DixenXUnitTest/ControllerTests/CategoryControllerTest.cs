using Dixen.API.Controllers;
using Dixen.Repo.DTOs.Category;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DixenXUnitTest.ControllerTests
{
    public class CategoryControllerTest
    {
        private readonly Mock<IGRepo<Category>> _categoryRepoMock;
        private readonly Mock<IGRepo<Evnt>> _eventRepoMock;
        private readonly CategoryController _controller;

        public CategoryControllerTest()
        {
            _categoryRepoMock = new Mock<IGRepo<Category>>();
            _eventRepoMock = new Mock<IGRepo<Evnt>>();
            _controller = new CategoryController(_categoryRepoMock.Object, _eventRepoMock.Object);
        }

        private Category GetSampleCategory()
        {
            return new Category
            {
                Id = 1,
                Name = "Music",
                ImageUrl = "music.png",
                Events = new List<Evnt>
                {
                    new Evnt
                    {
                        Id = 1,
                        Title = "Concert",
                        Description = "Live music",
                        ImageUrl = "concert.png",
                        StartTime = DateTime.Now
                    }
                }
            };
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithCategories()
        {
            // Arrange
            var categories = new List<Category> { GetSampleCategory() };

            _categoryRepoMock
                .Setup(r => r.GetAll(It.IsAny<Func<IQueryable<Category>,
                    Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Category, object>>>()))
                .ReturnsAsync(categories);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<CategoryResponse>>(okResult.Value);
            Assert.Single(value);
            Assert.Equal("Music", value.First().Name);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenCategoryExists()
        {
            // Arrange
            var category = GetSampleCategory();
            _categoryRepoMock
                 .Setup(r => r.Find(It.IsAny<Expression<Func<Category, bool>>>(),
                       It.IsAny<Func<IQueryable<Category>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Category, object>>>()))
                 .ReturnsAsync(new List<Category> { category });

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<CategoryResponse>(okResult.Value);
            Assert.Equal("Music", value.Name);
            Assert.Single(value.Events);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenCategoryMissing()
        {
            // Arrange
            _categoryRepoMock
                .Setup(r => r.Find(It.IsAny<Expression<Func<Category, bool>>>(),
                      It.IsAny<Func<IQueryable<Category>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Category, object>>>()))
                .ReturnsAsync(new List<Category>()); 

            // Act
            var result = await _controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsOk_WithCreatedCategory()
        {
            // Arrange
            var dto = new CategoryDto { Name = "Sports" };

            _categoryRepoMock
                .Setup(r => r.Create(It.IsAny<Category>()))
                .ReturnsAsync((Category c) => { c.Id = 2; return c; });

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<CategoryResponse>(okResult.Value);
            Assert.Equal(2, value.Id);
            Assert.Equal("Sports", value.Name);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            // Arrange
            _categoryRepoMock
                .Setup(r => r.Delete(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenCategoryExists()
        {
            // Arrange
            var dto = new CategoryDto { Name = "Updated", ImageUrl = "updated.png" };
            var updatedCategory = new Category { Id = 1, Name = "Updated", ImageUrl = "updated.png" };

            _categoryRepoMock
                .Setup(r => r.Update(1, It.IsAny<Category>()))
                .ReturnsAsync(updatedCategory);

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<CategoryResponse>(okResult.Value);
            Assert.Equal("Updated", value.Name);
            Assert.Equal("updated.png", value.ImageUrl);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenCategoryMissing()
        {
            // Arrange
            var dto = new CategoryDto { Name = "NotExist" };
            _categoryRepoMock
                .Setup(r => r.Update(99, It.IsAny<Category>()))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _controller.Update(99, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetSorted_ReturnsSortedCategories()
        {
            // Arrange
            var cat1 = new Category { Id = 1, Name = "B", Events = new List<Evnt>() };
            var cat2 = new Category { Id = 2, Name = "A", Events = new List<Evnt>() };
            var categories = new List<Category> { cat1, cat2 };

            _categoryRepoMock
                .Setup(r => r.GetAll(It.IsAny<Func<IQueryable<Category>,
                    Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Category, object>>>()))
                .ReturnsAsync(categories);

            // Act
            var result = await _controller.GetSorted(sortBy: "name");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<CategoryResponse>>(okResult.Value);

            // Ascending by Name: A, B
            Assert.Equal("A", value.First().Name);
            Assert.Equal("B", value.Skip(1).First().Name);
        }

        [Fact]
        public async Task Search_ReturnsMatchingCategories()
        {
            // Arrange
            var category = GetSampleCategory();
            _categoryRepoMock
                 .Setup(r => r.Find(It.IsAny<Expression<Func<Category, bool>>>(),
                       It.IsAny<Func<IQueryable<Category>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Category, object>>>()))
                 .ReturnsAsync(new List<Category> { category });
            // Act
            var result = await _controller.Search("Music");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<CategoryResponse>>(okResult.Value);
            Assert.Single(value);
            Assert.Equal("Music", value.First().Name);
        }

    }
}
