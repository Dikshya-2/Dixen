using Dixen.Repo.DTOs.Analysis;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Dixen.Repo.Services;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DixenXUnitTest.ServiceTests
{
    public class EventAnalysisServiceTests
    {
        private readonly Mock<IGRepo<Evnt>> _eventRepoMock;
        private readonly Mock<IGRepo<SocialShare>> _socialShareRepoMock;
        private readonly EventAnalysisService _service;

        public EventAnalysisServiceTests()
        {
            _eventRepoMock = new Mock<IGRepo<Evnt>>();
            _socialShareRepoMock = new Mock<IGRepo<SocialShare>>();
            _service = new EventAnalysisService(_eventRepoMock.Object, _socialShareRepoMock.Object);
        }

        [Fact]
        public async Task AnalyzeEventsAsync_ShouldReturnEventSummary()
        {
            // Arrange
            var events = new List<Evnt>
            {
                new Evnt
                {
                    Id = 1,
                    Title = "Event 1",
                    Description = "Description 1",
                    Categories = new List<Category> { new Category { Id = 1, Name = "Conference" } },
                    Bookings = new List<Booking>
                    {
                        new Booking
                        {
                            UserId = "user1",
                            Tickets = new List<Ticket>
                            {
                                new Ticket { Id = 1, Quantity = 2, BookingId=1 },
                                new Ticket { Id = 2, Quantity = 3, BookingId = 1 },
                                
                            }
                        }
                    }
                },
                new Evnt
                {
                    Id = 2,
                    Title = "Event 2",
                    Description = "Description 2",
                    Categories = new List<Category>(), // no category
                    Bookings = new List<Booking>
                    {
                        new Booking
                        {
                            UserId = "user2",
                            Tickets = new List<Ticket>
                            {
                                new Ticket { Id = 3, Quantity = 5 }
                            }
                        }
                    }
                }
            };

            var shares = new List<SocialShare>
            {
                new SocialShare { Id = 1, EventId = 1 },
                new SocialShare { Id = 2, EventId = 1 },
                new SocialShare { Id = 3, EventId = 2 }
            };

            _eventRepoMock.Setup(r => r.GetAllForAnalytics(It.IsAny<Func<IQueryable<Evnt>, IQueryable<Evnt>>>()))
                .ReturnsAsync(events);
            _socialShareRepoMock
                .Setup(r => r.GetAll(It.IsAny<Func<IQueryable<SocialShare>, IIncludableQueryable<SocialShare, object>>?>()))
                .ReturnsAsync(shares);



            // Act
            var result = await _service.AnalyzeEventsAsync();

            // Assert
            Assert.Equal(2, result.Count);

            var first = result.First(r => r.EventId == 1);
            Assert.Equal("Event 1", first.Title);
            Assert.Equal("Conference", first.EventType);
            Assert.Equal(5, first.TicketsSold); 
            Assert.Equal(2, first.SharesCount);

            var second = result.First(r => r.EventId == 2);
            Assert.Equal("Event 2", second.Title);
            Assert.Equal("Unknown", second.EventType); 
            Assert.Equal(5, second.TicketsSold);
            Assert.Equal(1, second.SharesCount);
        }

        [Fact]
        public void CalculateAnalytics_ShouldReturnZeroCorrelation_WhenNoVariation()
        {
            // Arrange
            var data = new List<EventSummaryDto>
            {
                new EventSummaryDto { TicketsSold = 5, SharesCount = 10 },
                new EventSummaryDto { TicketsSold = 5, SharesCount = 10 }
            };

            // Act
            var result = _service.CalculateAnalytics(data);

            // Assert
            Assert.Equal(0, ((dynamic)result).Correlation);
            Assert.False(((dynamic)result).IsEmpty);
        }

        [Fact]
        public void CalculateAnalytics_ShouldReturnCorrelation_WhenDataHasVariation()
        {
            // Arrange
            var data = new List<EventSummaryDto>
            {
                new EventSummaryDto { TicketsSold = 1, SharesCount = 2 },
                new EventSummaryDto { TicketsSold = 2, SharesCount = 3 },
                new EventSummaryDto { TicketsSold = 3, SharesCount = 5 }
            };

            // Act
            var result = _service.CalculateAnalytics(data);

            // Assert
            double correlation = ((dynamic)result).Correlation;
            Assert.InRange(correlation, 0.9, 1.0); 
            Assert.False(((dynamic)result).IsEmpty);
        }

        [Fact]
        public void CalculateAnalytics_ShouldReturnIsEmpty_WhenDataIsNull()
        {
            // Act
            var result = _service.CalculateAnalytics(null);

            // Assert
            Assert.True(((dynamic)result).IsEmpty);
            Assert.Equal(0, ((dynamic)result).Correlation);
        }

        [Fact]
        public void CalculateAnalytics_ShouldReturnIsEmpty_WhenDataIsEmpty()
        {
            // Act
            var result = _service.CalculateAnalytics(new List<EventSummaryDto>());

            // Assert
            Assert.True(((dynamic)result).IsEmpty);
            Assert.Equal(0, ((dynamic)result).Correlation);
        }
    }
}
