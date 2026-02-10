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
    public class EventSubmissionRepositoryTest

    {
        private readonly DbContextOptions<DatabaseContext> _options;
        private readonly DatabaseContext _context;
        private readonly EventSubmissionRepository _eventSubmissionRepository;

        public EventSubmissionRepositoryTest() 
        {
            _options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DatabaseContext(_options);  
            _eventSubmissionRepository = new EventSubmissionRepository(_context);
        }

        [Fact]
        public async Task ApproveEventSubmission_ShouldSetIsApprovedToTrue_WhenSubmissionExists()
        {
            // Arrange
            await _context.Database.EnsureDeletedAsync();

            var submission = new EventSubmission
            {
                Id = 1,
                Title = "Test Event",
                SubmittedBy = "Test Org",
                SubmittedAt = DateTime.UtcNow,
                EventId = 1,
                IsApproved = false
            };

            await _context.EventSubmissions.AddAsync(submission);
            await _context.SaveChangesAsync();

            // Act
            await _eventSubmissionRepository.ApproveEventSubmission(1);

            // Assert 
            var result = await _context.EventSubmissions.FindAsync(1);
            Assert.NotNull(result);
            Assert.True(result!.IsApproved);  
            Assert.True(result.IsApproved);      
        }

        [Fact]
        public async Task RejectEventSubmission_ShouldSetIsApprovedToFalse_WhenSubmissionExists()
        {
            // Arrange
            await _context.Database.EnsureDeletedAsync();

            var submission = new EventSubmission
            {
                Id = 1,
                Title = "Test Event",
                SubmittedBy = "Test Org",
                SubmittedAt = DateTime.UtcNow,
                EventId = 1,
                IsApproved = true
            };

            await _context.EventSubmissions.AddAsync(submission);
            await _context.SaveChangesAsync();

            // Act
            await _eventSubmissionRepository.RejectEventSubmission(1);

            // Assert - 
            var result = await _context.EventSubmissions.FindAsync(1);
            Assert.NotNull(result);
            //Assert.True(result);
            Assert.False(result.IsApproved);
        }

        [Fact]
        public async Task ApproveEventSubmission_ShouldDoNothing_WhenSubmissionDoesNotExist()
        {
            // Arrange
            await _context.Database.EnsureDeletedAsync();

            // Act
            await _eventSubmissionRepository.ApproveEventSubmission(999);

            // Assert
            var count = await _context.EventSubmissions.CountAsync();
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task RejectEventSubmission_ShouldDoNothing_WhenSubmissionDoesNotExist()
        {
            // Arrange
            await _context.Database.EnsureDeletedAsync();

            // Act
            await _eventSubmissionRepository.RejectEventSubmission(999);

            // Assert
            var count = await _context.EventSubmissions.CountAsync();
            Assert.Equal(0, count);
        }

    }
}
