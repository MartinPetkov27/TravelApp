using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer
{
    public class IdentityManager
    {
        private readonly IdentityContext identityContext;

        public IdentityManager(IdentityContext context)
        {
            identityContext = context;
        }

        public async Task<Tuple<IdentityResult, User>> CreateUserAsync(string username, string password, string email, string firstName, string lastName, string phoneNumber, RoleType role)
        {
             return await identityContext.CreateUserAsync(username, password, email, firstName, lastName, phoneNumber, role);
        }

        public async Task<User> LogInUserAsync(string username, string password)
        {
            return await identityContext.LogInUserAsync(username, password);
        }

        public async Task<User> ReadAsync(string key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            return await identityContext.ReadUserAsync(key, useNavigationalProperties);
        }

        public async Task<IEnumerable<User>> ReadAllUsersAsync(bool useNavigationalProperties = false)
        {
            return await identityContext.ReadAllUsersAsync(useNavigationalProperties);
        }

        public async Task UpdateUserAsync(string id, string username, string email, string firstName, string lastName, string phoneNumber)
        {
            await identityContext.UpdateUserAsync(id, username, email, firstName, lastName, phoneNumber);
        }

        public async Task DeleteUserByNameAsync(string username)
        {
            await identityContext.DeleteUserByNameAsync(username);
        }

        public async Task<User> FindUserByNameAsync(string username)
        {
            return await identityContext.FindUserByNameAsync(username);
        }
    }
}
