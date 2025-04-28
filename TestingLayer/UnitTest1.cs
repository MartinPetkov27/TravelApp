using BusinessLayer;
using DataLayer;
using Microsoft.EntityFrameworkCore;

namespace TestingLayer
{
    public class UnitTest1
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
        public async Task CreateAsync_Should_Add_New_Story()
        {
            // Arrange: in-memory база с уникално име
            var options = new DbContextOptionsBuilder<TravelAppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using var dbContext = new TravelAppDbContext(options);
            var context = new StoryContext(dbContext);

            // Създаваме фиктивен потребител
            var user = new User("Test", "User")
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "historian_user",
                Email = "historian@test.com",
                NormalizedUserName = "HISTORIAN_USER",
                NormalizedEmail = "HISTORIAN@TEST.COM",
                PhoneNumber = "0888888888",
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = "hashed" // фиктивна стойност за тест
            };


            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            // Създаваме история
            var story = new Story
            {
                Title = null,
                Content = "This is a unit test story.",
                Status = Status.Pending,
                User = user,
                UserId = user.Id
            };

            // Act
            await context.CreateAsync(story);

            // Assert
            var created = await dbContext.Stories.FirstOrDefaultAsync(s => s.Title == "Test Story");
            Assert.NotNull(created);
            Assert.Equal("This is a unit test story.", created.Content);
            Assert.Equal(user.Id, created.UserId);
            Assert.Equal(Status.Pending, created.Status);
        }
    }
}

