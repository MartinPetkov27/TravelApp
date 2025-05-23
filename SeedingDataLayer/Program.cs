﻿using System.Data;
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
                   "Server=DESKTOP-AUDH7G9\\SQLEXPRESS;Database=TravelApp1;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
        static async Task SeedCountries(TravelAppDbContext dbContext)
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

    }
}

