using Dixen.Repo.Model.Entities;
using Dixen.Repo.Model.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Model
{
    public class DatabaseContext: IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options){ }
        public DbSet<Evnt> Events { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<Performer> Performers { get; set; }
        public DbSet<EventSubmission> EventSubmissions { get; set; }
        public DbSet<SocialShare> SocialShares { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<EventReview> EventReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Soft Delete Query Filters
            modelBuilder.Entity<Evnt>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Booking>().HasQueryFilter(b => !b.IsDeleted);
            modelBuilder.Entity<Hall>().HasQueryFilter(h => !h.IsDeleted);
            modelBuilder.Entity<EventReview>().HasQueryFilter(er => !er.Event.IsDeleted);
            modelBuilder.Entity<EventSubmission>().HasQueryFilter(es => !es.Event.IsDeleted);
            modelBuilder.Entity<Performer>().HasQueryFilter(p => !p.Event.IsDeleted);
            modelBuilder.Entity<SocialShare>().HasQueryFilter(ss => !ss.Event.IsDeleted);
            modelBuilder.Entity<Ticket>().HasQueryFilter(t => !t.Booking.IsDeleted);
            modelBuilder.Entity<Organizer>().HasQueryFilter(o => !o.IsDeleted);
            #endregion


            #region Relationships
            modelBuilder.Entity<Evnt>()
                .HasOne(e => e.Organizer)
                .WithMany(o => o.Events)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hall>()
                .HasOne(h => h.Event)
                .WithMany(e => e.Halls)
                .HasForeignKey(h => h.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hall>()
                .HasOne(h => h.Venue)
                .WithMany(v => v.Halls)
                .HasForeignKey(h => h.VenueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SocialShare>()
                .HasOne(ss => ss.User)
                .WithMany(u => u.SocialShares)
                .HasForeignKey(ss => ss.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Hall)
                .WithMany()
                .HasForeignKey(b => b.HallId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventSubmission>()
                .HasOne(es => es.Event)
                .WithMany(e => e.Submissions)
                .HasForeignKey(es => es.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventReview>()
                .HasOne(er => er.Event)
                .WithMany(e => e.Reviews)
                .HasForeignKey(er => er.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventReview>()
                .HasOne(er => er.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(er => er.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Performer>()
                .HasOne(p => p.Event)
                .WithMany(e => e.Performers)
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evnt>()
                .HasMany(e => e.Categories)
                .WithMany(c => c.Events)
                .UsingEntity<Dictionary<string, object>>(
                    "EventCategories",
                    j => j.HasOne<Category>()
                          .WithMany()
                          .HasForeignKey("CategoryId")
                          .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Evnt>()
                          .WithMany()
                          .HasForeignKey("EventId")
                          .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("CategoryId", "EventId");
                    });

            #endregion
           
            #region Property Configurations
            modelBuilder.Entity<Ticket>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Type)
                .HasConversion<string>();

            #endregion

            #region Seed Data

             modelBuilder.Entity<ApplicationUser>().HasData(
             new ApplicationUser
             {
                 Id = "9f0bd209-3b56-410c-b4fc-5654161c3925",
                 FullName= "Dikshya Singh",
                 Age =10,
                 Gender ="Female",
                 UserName = "It@gmail.com",
                 Email = "It@gmail.com"
             }
        );

            modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" },
            new IdentityRole { Id = "3", Name = "Organization", NormalizedName = "ORGANIZATION" }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Technology", ImageUrl = "/assets/images/Technology.jpg" },
                new Category { Id = 2, Name = "Music", ImageUrl = "/assets/images/Music.jpg" },
                new Category { Id = 3, Name = "Concert", ImageUrl = "/assets/images/Concert.jpg" },
                new Category { Id = 4, Name = "Comedy", ImageUrl = "/assets/images/" },
                new Category { Id = 5, Name = "Dance", ImageUrl = "/assets/images/threater.avif" },
                new Category { Id = 6, Name = "Art", ImageUrl = "/assets/images/art.jpg" },
                new Category { Id = 7, Name = "Sports", ImageUrl = "/assets/images/sports.avif" },
                new Category { Id = 8, Name = "Education", ImageUrl = "/assets/images/workshop.jpg" }
                );


            modelBuilder.Entity<Organizer>().HasData(
                new Organizer { Id = 1, OrganizationName = "Tech Org", ContactEmail = "contact@tech.org" },
                 new Organizer { Id = 2, OrganizationName = "ArtSchool", ContactEmail = "contact@art.org" }
            );

            modelBuilder.Entity<Evnt>().HasData(
                new Evnt
                {
                    Id = 1,
                    Title = "Tech Future 2025",
                    ImageUrl = "tech_icon.jpg",
                    Description = "Annual tech event",
                    StartTime = new DateTime(2026, 3, 1, 18, 0, 0, DateTimeKind.Utc),
                    OrganizerId = 1
                },
                 new Evnt
                 {
                     Id = 2,
                     Title = "Painting Exibation",
                     ImageUrl = "art_icon.png",
                     Description = "test",
                     StartTime = DateTime.UtcNow.AddDays(30),
                     OrganizerId = 2
                 },
                  new Evnt
                  {
                      Id = 3,
                      Title = "Rock Concert",
                      ImageUrl = "rock_concert.jpg",
                      Description = "test",
                      StartTime = DateTime.UtcNow.AddDays(30),
                      OrganizerId = 2
                  },
                   new Evnt
                   {
                       Id = 4,
                       Title = " Jazz Night",
                       ImageUrl = "images.jpeg",
                       Description = "test",
                       StartTime = DateTime.UtcNow.AddDays(30),
                       OrganizerId = 2
                   }
            );

            modelBuilder.Entity<Hall>().HasData(
                new Hall { Id = 1, Name = "Main Hall", Capacity = 300, EventId = 1, VenueId = 1 },
                new Hall { Id = 2, Name = "Room A", Capacity = 100, EventId = 1, VenueId = 1 }
            );


            modelBuilder.Entity<Performer>().HasData(
                new Performer { Id = 1, Name = "John Doe", Bio = "Tech Speaker", EventId = 1 }
            );

            modelBuilder.Entity<SocialShare>().HasData(
                new SocialShare
                {
                    Id = 1,
                    Platform = "Twitter",
                    SharedAt = DateTime.UtcNow,
                    EventId = 1,
                    UserId = "9f0bd209-3b56-410c-b4fc-5654161c3925"
                },
                  new SocialShare
                  {
                      Id = 2,
                      Platform = "Facebook",
                      SharedAt = DateTime.UtcNow,
                      EventId = 2,
                      UserId = "9f0bd209-3b56-410c-b4fc-5654161c3925",
                  }
            );

            modelBuilder.Entity("EventCategories").HasData(
                new { CategoryId = 1, EventId = 1 },
                new { CategoryId = 3, EventId = 3 },
                new { CategoryId = 3, EventId = 4 },
                new { CategoryId = 2, EventId = 2 } 
            );

            modelBuilder.Entity<EventSubmission>().HasData(
                new EventSubmission
                {
                    Id = 1,
                    SubmittedBy = "Alice",
                    Details = "Wants to present on AI",
                    SubmittedAt = DateTime.UtcNow,
                    EventId = 1,
                    IsApproved = null
                }
            );


            modelBuilder.Entity<Venue>().HasData(
            new Venue
            {
                Id = 1,
                Name = "Bella Center Copenhagen",
                Address = "Center Boulevard 5",
                City = "Copenhagen"
            },
            new Venue
            {
                Id = 2,
                Name = "Royal Arena",
                Address = "Hannemanns Allé 18",
                City = "Copenhagen"
            }
        );
            modelBuilder.Entity<Booking>().HasData(
            new Booking
            {
                Id = 1,
                BookedTime = DateTime.UtcNow,
                UserId = "9f0bd209-3b56-410c-b4fc-5654161c3925",
                EventId = 1,
                HallId = 1
            });

            modelBuilder.Entity<Ticket>().HasData(
            new Ticket { Id = 1, BookingId = 1, Type = TicketType.General, Price = 50, Quantity = 100 },
            new Ticket { Id = 2, BookingId = 1, Type = TicketType.VIP, Price = 120, Quantity = 20 },
            new Ticket { Id = 3, BookingId = 1, Type = TicketType.Student, Price = 30, Quantity = 50 }
             );
            #endregion
        }
    }
    // SOFT DELETE INTERCEPTOR 
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChanges(eventData, result);
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
            InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        private void ApplySoftDelete(DbContext? context)
        {
            if (context == null) return;
            foreach (var entry in context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Deleted))
            {
                var prop = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "IsDeleted");
                if (prop != null)
                {
                    entry.State = EntityState.Modified;
                    prop.CurrentValue = true;
                }
            }
        }
    }

}
