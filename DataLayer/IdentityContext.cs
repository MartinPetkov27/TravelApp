using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using BusinessLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DataLayer
{
    public class IdentityContext
    {
        private readonly UserManager<User> userManager;
        private readonly TravelAppDbContext context;

        public IdentityContext(TravelAppDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        #region Seeding Data

        //public async Task SeedDataAsync(string adminPass, string adminEmail)
        //{
        //    int userRoles = await context.UserRoles.CountAsync();

        //    if (userRoles == 0)
        //    {
        //        await ConfigureAdminAccountAsync(adminPass, adminEmail);
        //    }
        //}

        //private async Task ConfigureAdminAccountAsync(string password, string email)
        //{
        //    User adminIdentityUser = await context.Users.FirstAsync();

        //    if (adminIdentityUser != null)
        //    {
        //        await userManager.AddToRoleAsync(adminIdentityUser, RoleType.Admin.ToString());
        //        await userManager.AddPasswordAsync(adminIdentityUser, password);
        //        await userManager.SetEmailAsync(adminIdentityUser, email);
        //    }
        //}

        #endregion

        #region CRUD Operations

        public async Task<Tuple<IdentityResult, User>> CreateUserAsync(string username, string password, string email, string firstName, string lastName, string phoneNumber, RoleType role)
        {
            try
            {
                User user = new User
                {
                    UserName = username,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                };

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    return new Tuple<IdentityResult, User>(result, user);
                }
                if (role == RoleType.Administrator)
                {
                    await userManager.AddToRoleAsync(user, RoleType.Administrator.ToString());
                }
                if (role == RoleType.Historian)
                {
                    await userManager.AddToRoleAsync(user, RoleType.Historian.ToString());
                }
                else
                {
                    await userManager.AddToRoleAsync(user, RoleType.User.ToString());
                }

                return new Tuple<IdentityResult, User>(IdentityResult.Success, user);
            }
            catch (Exception ex)
            {
                IdentityResult result = IdentityResult.Failed(new IdentityError() { Code = "Registration", Description = ex.Message });
                return new Tuple<IdentityResult, User>(result, null);
            }
        }

        public async Task<User> LogInUserAsync(string username, string password)
        {
            try
            {
                User user = await userManager.FindByNameAsync(username);

                if (user == null)
                {
                    return null;
                }

                IdentityResult result = await userManager.PasswordValidators[0].ValidateAsync(userManager, user, password);

                if (result.Succeeded)
                {
                    return await context.Users.FindAsync(user.Id);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> ReadUserAsync(string key, bool useNavigationalProperties = false)
        {
            try
            {
                if (!useNavigationalProperties)
                {
                    // If you want to use the API
                    return await userManager.FindByIdAsync(key);
                }

                return await context.Users.Include(u => u.Trips).Include(u => u.BucketLists).Include(u => u.Stories).SingleOrDefaultAsync(u => u.Id == key);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<User>> ReadAllUsersAsync(bool useNavigationalProperties = false)
        {
            try
            {
                if (!useNavigationalProperties)
                {
                    // If you want to use the API
                    return await context.Users.ToListAsync();
                }

                return await context.Users.Include(u => u.Trips).Include(u => u.BucketLists).Include(u => u.Stories).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateUserAsync(string id, string username, string firstName, string lastName, string phoneNumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(username))
                {
                    User user = await context.Users.FindAsync(id);
                    user.UserName = username;
                    user.FirstName = firstName;
                    user.LastName = lastName;
                    user.PhoneNumber = phoneNumber;
                    await userManager.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteUserByNameAsync(string username)
        {
            try
            {
                User user = await FindUserByNameAsync(username);

                if (user == null)
                {
                    throw new InvalidOperationException("User not found for deletion!");
                }

                await userManager.DeleteAsync(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> FindUserByNameAsync(string username)
        {
            try
            {
                // Identity return Null if there is no user!
                return await userManager.FindByNameAsync(username);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
