using Dixen.API.Controllers;
using Dixen.Repo.DTOs.Rating;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DixenXUnitTest.ControllerTests
{
    public class EventReviewControllerTest
    {
        private readonly Mock<IGRepo<EventReview>> _reviewRepoMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly EventReviewController _controller;
        public EventReviewControllerTest()
        {
            _reviewRepoMock = new Mock<IGRepo<EventReview>>();
            _userManagerMock = MockUserManager(new List<ApplicationUser>
            {
                new ApplicationUser { Id = "user1", FullName = "Test User", Age = 25 }
            });
            _controller = new EventReviewController(_reviewRepoMock.Object, _userManagerMock.Object);
        }

        private static Mock<UserManager<ApplicationUser>> MockUserManager(List<ApplicationUser> users)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
            mgr.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync((string id) => users.FirstOrDefault(u => u.Id == id));
            return mgr;
        }

        private EventReviewDto GetSampleDto()
        {
            return new EventReviewDto
            {
                EventId = 1,
                UserId = "user1",
                Rating = 5,
                Comment = "Excellent"
            };
        }
        private EventReview GetSampleReview()
        {
            return new EventReview
            {
                Id = 1,
                EventId = 1,
                UserId = "user1",
                Rating = 5,
                Comment = "Excellent"
            };
        }
        [Fact]
        public async Task AddReview_ShouldAddNewReview_WhenNoExistingReview()
        {
            // Arrange
            var dto = GetSampleDto();
            _reviewRepoMock.Setup(r => r.GetAll(It.IsAny<Func<IQueryable<EventReview>,
                Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EventReview, object>>>()))
                .ReturnsAsync(new List<EventReview>());

            _reviewRepoMock.Setup(r => r.Create(It.IsAny<EventReview>()))
                .ReturnsAsync((EventReview r) => r);

            // Act
            var result = await _controller.AddReview(dto);
            // Assert
            Assert.IsType<OkResult>(result);
            _reviewRepoMock.Verify(r => r.Create(It.IsAny<EventReview>()), Times.Once);
        }

        [Fact]
        public async Task AddReview_ShouldUpdateExistingReview_WhenReviewExists()
        {
            // Arrange
            var dto = GetSampleDto();
            var existing = GetSampleReview();

            _reviewRepoMock.Setup(r => r.GetAll(It.IsAny<Func<IQueryable<EventReview>,
                Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EventReview, object>>>()))
                .ReturnsAsync(new List<EventReview> { existing });

            _reviewRepoMock.Setup(r => r.Update(existing.Id, existing))
                .ReturnsAsync(existing);

            // Act
            var result = await _controller.AddReview(dto);
            // Assert
            Assert.IsType<OkResult>(result);
            _reviewRepoMock.Verify(r => r.Update(existing.Id, existing), Times.Once);
        }

        [Fact]
        public async Task GetReviews_ReturnsListOfReviews_ForEvent()
        {
            // Arrange
            var review = GetSampleReview();
            _reviewRepoMock.Setup(r => r.GetAll(It.IsAny<Func<IQueryable<EventReview>,
                Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EventReview, object>>>()))
                .ReturnsAsync(new List<EventReview> { review });

            // Act
            var result = await _controller.GetReviews(1);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var reviews = Assert.IsAssignableFrom<IEnumerable<EventReviewDto>>(okResult.Value);
            Assert.Single(reviews);
            Assert.Equal(5, reviews.First().Rating);
        }

        [Fact]
        public async Task GetAverageRating_ReturnsCorrectAverage()
        {
            // Arrange
            var reviews = new List<EventReview>
            {
                new EventReview { EventId = 1, Rating = 4 },
                new EventReview { EventId = 1, Rating = 6 }
            };

            _reviewRepoMock.Setup(r => r.GetAll(It.IsAny<Func<IQueryable<EventReview>,
                Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EventReview, object>>>()))
                .ReturnsAsync(reviews);

            // Act
            var result = await _controller.GetAverageRating(1);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var average = Assert.IsType<double>(okResult.Value);
            Assert.Equal(5, average);
        }

        [Fact]
        public async Task GetAverageRating_ReturnsZero_WhenNoReviews()
        {
            // Arrange
            _reviewRepoMock.Setup(r => r.GetAll(It.IsAny<Func<IQueryable<EventReview>,
                Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<EventReview, object>>>()))
                .ReturnsAsync(new List<EventReview>());

            // Act
            var result = await _controller.GetAverageRating(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var average = Assert.IsType<double>(okResult.Value);
            Assert.Equal(0, average);
        }
    }
}
