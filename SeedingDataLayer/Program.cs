using System.Data;
using System;
using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PresentationLayer.Models;
using ServiceLayer;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using static System.Net.WebRequestMethods;



namespace SeedingDataConsoleApp
{
    class Program
    {

        static async Task Main(string[] args)
        {
            try
            {
                IdentityOptions options = new IdentityOptions();
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;

                DbContextOptionsBuilder builder = new DbContextOptionsBuilder();
                builder.UseSqlServer(
                   "Server=Segotep\\SQLEXPRESS;Database=TravelApp1;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
                    //"Server=III-PC\\SQLEXPRESS;Database=MVCProjectTemplateDb;Trusted_Connection=True;"
                    );

                TravelAppDbContext dbContext = new TravelAppDbContext(builder.Options);
                
                UserManager<User> userManager = new UserManager<User>(
                    new UserStore<User>(dbContext), Options.Create(options),
                    new PasswordHasher<User>(), new List<IUserValidator<User>>() { new UserValidator<User>() },
                    new List<IPasswordValidator<User>>() { new PasswordValidator<User>() }, new UpperInvariantLookupNormalizer(),
                    new IdentityErrorDescriber(), new ServiceCollection().BuildServiceProvider(),
                    new Logger<UserManager<User>>(new LoggerFactory())
                    );

                IdentityContext identityContext = new IdentityContext(dbContext, userManager);
                StoryContext storyContext = new StoryContext(dbContext);
                
                await ClearTablesAsync(dbContext);

                dbContext.Roles.Add(new IdentityRole("Administrator") { NormalizedName = "ADMINISTRATOR" });
                dbContext.Roles.Add(new IdentityRole("User") { NormalizedName = "USER" });
                dbContext.Roles.Add(new IdentityRole("Historian") { NormalizedName = "HISTORIAN" });
                await dbContext.SaveChangesAsync();

                Tuple<IdentityResult, User> result = await identityContext.CreateUserAsync("admin", "admin", "admincho@abv.bg", "Admin", "Adminov", "0888888888", RoleType.Administrator);
                Tuple<IdentityResult, User> result1 = await identityContext.CreateUserAsync("user", "user", "usercho@abv.bg", "User", "Userov", "0888888888", RoleType.User);
                Tuple<IdentityResult, User> result2 = await identityContext.CreateUserAsync("historian", "historian", "historiancho@abv.bg", "Historian", "Historianov", "0888888888", RoleType.Historian);

                //Seeding Countries
                await SeedCountries(dbContext);

                Console.WriteLine("Roles added successfully!");

                if (result.Item1.Succeeded)
                {
                    Console.WriteLine("Admin account added successfully!");
                }
                else
                {
                    Console.WriteLine("Admin account failed to be created!");
                }
                if (result1.Item1.Succeeded)
                {
                    Console.WriteLine("User account added successfully!");
                }
                else
                {
                    Console.WriteLine("User account failed to be created!");
                }
                if (result2.Item1.Succeeded)
                {
                    Console.WriteLine("Historian account added successfully!");
                }
                else
                {
                    Console.WriteLine("Historian account failed to be created!");
                }

                //Seeding Stories
                await SeedStories(dbContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                Console.WriteLine("Top-level error:");
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                if (ex.InnerException?.InnerException != null)
                    Console.WriteLine("Deeper Inner Exception: " + ex.InnerException.InnerException.Message);
            }
            finally
            {

            }
        }
        public static async Task ClearTablesAsync(TravelAppDbContext dbContext)
        {
            // Delete all records from the tables
            dbContext.Roles.RemoveRange(dbContext.Roles);
            dbContext.Users.RemoveRange(dbContext.Users);
            dbContext.UserRoles.RemoveRange(dbContext.UserRoles);

            // Optionally, for many-to-many or other related tables, clear those too
            await dbContext.SaveChangesAsync();
        }
        
        public static async Task SeedCountries(TravelAppDbContext dbContext)
        {
            var httpClient = new HttpClient();
            var geoNamesService = new GeoNamesService(httpClient);

            Console.WriteLine("About to call GeoNamesService...");
            var countries = await geoNamesService.GetAllCountriesAsync();
            Console.WriteLine("After calling GeoNamesService");

            var validCountries = countries
            .Where(c => !string.IsNullOrWhiteSpace(c.AlphaCode)
                && !string.IsNullOrWhiteSpace(c.Name)
                && c.AlphaCode.Length == 3
                && c.Name.Length <= 100)
            .ToList();

            foreach (var country in validCountries)
            {
                if (!dbContext.Countries.Any(c => c.AlphaCode.ToLower() == country.AlphaCode.ToLower()))
                {
                    dbContext.Countries.Add(country);
                }
            }

            try
            {
                await dbContext.SaveChangesAsync();
                Console.WriteLine("Countries saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving countries:");
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner exception: " + ex.InnerException.Message);
            }
        }
        public static async Task SeedStories(TravelAppDbContext dbContext)
        {
            StoryContext storyContext = new StoryContext(dbContext);
            //Retrieve a user(e.g., historian)
            User historian = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "historian");
            if (historian == null)
            {
                Console.WriteLine("Historian user not found!");
                return;
            }

            Story story1 = new Story
            {

                Title = "Veliki Preslav",
                Content = "This is some content about Veliki Preslav.",
                ImageUrls = new List<string> { "https://complexzdravec.com/wp-content/uploads/veliki-preslav.webp", "https://www.business-register.bg/media/k2/items/cache/a975f7fffe3f534e70a3234554906c80_XL.jpg", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR3FWAfw3KbQnkTKA5xLjj6coVZ4hc13jtbvA&s" },
                MapsUrl = "https://maps.app.goo.gl/c9yf9W6A7R8UqPRN9",
                Status = Status.Approved,
                UserId = historian.Id,
                User = historian
            };

            Story story2 = new Story
            {
                Title = "Tsarevets Fortress",
                Content = "Tsarevets is a medieval stronghold located on a hill.",
                ImageUrls = new List<string> { "https://static.pochivka.bg/sights.bgstay.com/images/00/822/547ef4b17d249.jpg", "https://tickets.museumvt.com/uploads/museum/locations/yw0vvwlb40m/images/Tsarevets_3.jpg", "https://www.tsarevets.eu/assets/images/index-meta.jpg" },
                MapsUrl = "https://maps.app.goo.gl/1KxhxLrYL5DrztBf9",
                Status = Status.Approved,
                UserId = historian.Id,
                User = historian
            };

            Story story3 = new Story
            {
                Title = "Pliska",
                Content = "Pliska was the first capital of Bulgaria.",
                ImageUrls = new List<string> { "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ-jnShHPHtitadCeIheNepJYjee0wSvyqTNw&s", "https://dynamic-media-cdn.tripadvisor.com/media/photo-o/0f/bb/93/09/caption.jpg?w=1200&h=1200&s=1", "https://archaeologyinbulgaria.wordpress.com/wp-content/uploads/2015/05/great-basilica-41.jpg" },
                MapsUrl = "https://maps.app.goo.gl/nmzEunzSKG7cMGnD8",
                Status = Status.Approved,
                UserId = historian.Id,
                User = historian
            };

            Console.WriteLine($"Story: Title={story1.Title}, UserId={story1.UserId}, UserName={story1.User.UserName},Content={story1.Content},FirstImage={story1.ImageUrls.FirstOrDefault()}, MapsUrl = {story1.MapsUrl}");

            dbContext.Stories.Add(story1);
            dbContext.Stories.Add(story2);
            dbContext.Stories.Add(story3);

            try
            {
                await dbContext.SaveChangesAsync();
                Console.WriteLine("Story created successfully.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Full Exception:");
                Console.WriteLine(ex.ToString());
            }

            //try
            //{
            //    await storyContext.CreateAsync(story2);
            //    Console.WriteLine("Story created successfully.");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error creating story: {ex.Message}");
            //}

            //try
            //{
            //    await storyContext.CreateAsync(story3);
            //    Console.WriteLine("Story created successfully.");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error creating story: {ex.Message}");
            //}

            //Console.WriteLine("Stories seeded successfully.");
        }
    }
}

