using BusinessLayer;
using DataLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingLayer
{
    public class IntegrationTest
    {
        public static class DbContextMock
        {
            public static TravelAppDbContext GetInMemoryDbContext()
            {
                var options = new DbContextOptionsBuilder<TravelAppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // уникална база за всеки тест
                    .Options;

                return new TravelAppDbContext(options);
            }
        }
        [Fact]
        public async Task CreateAsync_Should_Add_Story_WithSeededHistorian()
        {
            // Arrange: използваме реалната база
            var options = new DbContextOptionsBuilder<TravelAppDbContext>()
                .UseSqlServer("Server=DESKTOP-AUDH7G9\\SQLEXPRESS;Database=TravelApp;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True")
                .Options;

            await using var dbContext = new TravelAppDbContext(options);
            var context = new StoryContext(dbContext);

            // Намираме historian потребителя, създаден от Seeding Layer
            var historian = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "historian");

            Assert.NotNull(historian); // Ако го няма – сийдът не е минал

            var story = new Story
            {
                Title = "Integration Test Story",
                Content = "Story added using seeded historian user.",
                Status = Status.Pending,
                User = historian,
                UserId = historian.Id
            };

            // Act
            await context.CreateAsync(story);

            // Assert
            var saved = await dbContext.Stories.FirstOrDefaultAsync(s => s.Title == "Integration Test Story");
            Assert.NotNull(saved);
            Assert.Equal(Status.Pending, saved.Status);
            Assert.Equal(historian.Id, saved.UserId);
        }

    }
}