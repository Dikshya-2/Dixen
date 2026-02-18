using Dixen.Controllers;
using Dixen.Repo.DTOs;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DixenXUnitTest.ControllerTests
{
    public class PerformerControllerTest
    {
        private readonly Mock<IGRepo<Performer>> _performerRepoMock;
        private readonly PerformerController _controller;
        public PerformerControllerTest()
        {
            _performerRepoMock = new Mock<IGRepo<Performer>>();
            _controller = new PerformerController(_performerRepoMock.Object);
        }
        private Performer GetSamplePerformer(int id = 1)
        {
            return new Performer
            {
                Id = id,
                Name = "Performer " + id,
                Bio = "Bio " + id,
                EventId = 100 + id
            };
        }
        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            _performerRepoMock
               .Setup(r => r.GetAll(It.IsAny<Func<IQueryable<Performer>, IIncludableQueryable<Performer, object>>>()))
               .ReturnsAsync(new List<Performer>
               {
                GetSamplePerformer(),
                GetSamplePerformer(2)
               });


            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsAssignableFrom<List<PerformerDto>>(okResult.Value);
            Assert.Equal(2, value.Count);
            Assert.Equal("Performer 1", value[0].Name);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenPerformerExists()
        {
            // Arrange
            var performer = GetSamplePerformer();
            _performerRepoMock.Setup(r => r.GetById(1, null))
                .ReturnsAsync(performer);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<PerformerDto>(okResult.Value);
            Assert.Equal(1, value.Id);
            Assert.Equal("Performer 1", value.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenPerformerMissing()
        {
            // Arrange
            _performerRepoMock.Setup(r => r.GetById(99, null))
                .ReturnsAsync((Performer)null!);

            // Act
            var result = await _controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new PerformerDto
            {
                Name = "New Performer",
                Bio = "New Bio",
                EventId = 101
            };

            // Make the mock return the performer passed in
            _performerRepoMock.Setup(r => r.Create(It.IsAny<Performer>()))
                .ReturnsAsync((Performer p) => p);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedDto = Assert.IsType<PerformerDto>(createdResult.Value);

            Assert.Equal("New Performer", returnedDto.Name);
            Assert.Equal("New Bio", returnedDto.Bio);
            Assert.Equal(101, returnedDto.EventId);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenPerformerExists()
        {
            // Arrange
            var dto = new PerformerDto { Name = "Updated", Bio = "Updated Bio", EventId = 102 };
            var existingPerformer = GetSamplePerformer();

            _performerRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync(existingPerformer);

            // Make Update return the updated performer
            _performerRepoMock.Setup(r => r.Update(1, It.IsAny<Performer>()))
                .ReturnsAsync((int id, Performer p) => p);

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<PerformerDto>(okResult.Value);

            Assert.Equal("Updated", returnedDto.Name);
            Assert.Equal("Updated Bio", returnedDto.Bio);
            Assert.Equal(102, returnedDto.EventId);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenPerformerMissing()
        {
            // Arrange
            var dto = new PerformerDto { Name = "Missing", Bio = "Missing", EventId = 103 };
            _performerRepoMock.Setup(r => r.GetById(99, null)).ReturnsAsync((Performer)null!);

            // Act
            var result = await _controller.Update(99, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            // Arrange
            _performerRepoMock.Setup(r => r.Delete(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
