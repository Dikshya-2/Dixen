using Dixen.API.Controllers;
using Dixen.Repo.DTOs.Organizer;
using Dixen.Repo.DTOs;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;
using Xunit;

namespace DixenXUnitTest.ControllerTests
{
    public class OrganizerControllerTest
    {
        private readonly Mock<IGRepo<Organizer>> _organizerRepoMock;
        private readonly Mock<IGRepo<EventSubmission>> _submissionRepoMock;
        private readonly Mock<IGRepo<Evnt>> _eventRepoMock;
        private readonly Mock<IGRepo<Category>> _categoryRepoMock;
        private readonly Mock<IEventSubmissionRepository> _eventSubmissionRepositoryMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        private readonly OrganizerController _controller;

        public OrganizerControllerTest()
        {
            _organizerRepoMock = new Mock<IGRepo<Organizer>>();
            _submissionRepoMock = new Mock<IGRepo<EventSubmission>>();
            _eventRepoMock = new Mock<IGRepo<Evnt>>();
            _categoryRepoMock = new Mock<IGRepo<Category>>();
            _eventSubmissionRepositoryMock = new Mock<IEventSubmissionRepository>();

            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _controller = new OrganizerController(
                _organizerRepoMock.Object,
                _submissionRepoMock.Object,
                _eventRepoMock.Object,
                _categoryRepoMock.Object,
                _userManagerMock.Object,
                _eventSubmissionRepositoryMock.Object
            );
        }
        private Organizer GetSampleOrganizer()
        {
            return new Organizer
            {
                Id = 1,
                OrganizationName = "Org1",
                ContactEmail = "org@test.com",
                Events = new List<Evnt>
                {
                    new Evnt
                    {
                        Id = 1,
                        Title = "Event1",
                        Description = "Test Description",
                        ImageUrl = "img.png",
                        StartTime = DateTime.UtcNow
                    }
                }
            };
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithOrganizers()
        {
            var organizers = new List<Organizer> { GetSampleOrganizer() };
            _organizerRepoMock
                .Setup(r => r.GetAll(
                    It.IsAny<Func<IQueryable<Organizer>,
                    IIncludableQueryable<Organizer, object>>?>()))
                .ReturnsAsync(organizers);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsAssignableFrom<IEnumerable<OrganizerResponseDto>>(okResult.Value);

            Assert.Single(value);
            Assert.Equal("Org1", value.First().OrganizationName);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenMissing()
        {
            _organizerRepoMock
                .Setup(r => r.GetById(
                    It.IsAny<int>(),
                    It.IsAny<Func<IQueryable<Organizer>,
                    IIncludableQueryable<Organizer, object>>?>()))
                .ReturnsAsync((Organizer?)null);

            var result = await _controller.Get(99);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var dto = new OrganizerCreateUpdateDto
            {
                OrganizationName = "NewOrg",
                ContactEmail = "new@test.com"
            };
            _organizerRepoMock
                .Setup(r => r.Create(It.IsAny<Organizer>()))
                .ReturnsAsync((Organizer o) =>
                {
                    o.Id = 5;
                    return o;
                });

            var result = await _controller.Create(dto);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var value = Assert.IsType<OrganizerResponseDto>(created.Value);
            Assert.Equal(5, value.Id);
            Assert.Equal("NewOrg", value.OrganizationName);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            _organizerRepoMock.Setup(r => r.Delete(1)).ReturnsAsync(true);
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }

      
    }
}
