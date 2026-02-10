using Dixen.Repo.Model;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DixenXUnitTest.Repositorie
{
    public class GenericRepoTest
    {
        private DatabaseContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;
            return new DatabaseContext(options);
        }

        [Fact]
        public async Task Create_AddsEntityAndSaves()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repo = new GRepo<Category>(context);  

            var category = new Category { Id = 1, Name = "Test" }; 

            // Act
            var result = await repo.Create(category);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
            Assert.Equal(1, result.Id);

            var savedCategory = await context.Categories.FindAsync(1);
            Assert.NotNull(savedCategory);
            Assert.Equal("Test", savedCategory.Name);
        }

        [Fact]
        public async Task GetById_ReturnsEntity()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var category = new Category { Id = 1, Name = "Test" };  
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            var repo = new GRepo<Category>(context);  

            // Act
            var result = await repo.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task Delete_RemovesEntity()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var category = new Category { Id = 1, Name = "DeleteMe" };  
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            var repo = new GRepo<Category>(context);

            // Act
            var deleted = await repo.Delete(1);  

            // Assert
            Assert.True(deleted);
            var remaining = await context.Categories.FindAsync(1);
            Assert.Null(remaining);
        }

        [Fact]
        public async Task GetAll_ReturnsAllEntities()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            await context.Categories.AddRangeAsync(
                new Category { Id = 1, Name = "Cat1" },  
                new Category { Id = 2, Name = "Cat2" }
            );
            await context.SaveChangesAsync();

            var repo = new GRepo<Category>(context);  

            // Act
            var result = await repo.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Cat1");
            Assert.Contains(result, c => c.Name == "Cat2");
        }
    }
}
