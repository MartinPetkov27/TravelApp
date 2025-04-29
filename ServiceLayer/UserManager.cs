using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using DataLayer;

namespace ServiceLayer
{
    public class UserManager
    {
        private readonly UserContext userContext;

        //Methods for managing the userContext for CRUD operations in User
        public UserManager(UserContext userContext)
        {
            this.userContext = userContext;
        }

        public async Task CreateAsync(User item)
        {
            await userContext.CreateAsync(item);
        }
        public async Task<User> ReadAsync(string key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            return await userContext.ReadAsync(key, useNavigationalProperties, isReadOnly);
        }
        public async Task<ICollection<User>> ReadAllAsync(bool useNAvigationalProperties = false, bool isReadOnly = true)
        {
            return await userContext.ReadAllAsync(useNAvigationalProperties, isReadOnly);
        }
        public async Task UpdateAsync(User item, bool useNaigationalProperties = false)
        {
            await userContext.UpdateAsync(item, useNaigationalProperties);
        }
        public async Task DeleteAsync(string key)
        {
            await userContext.DeleteAsync(key);
        }
    }
}
