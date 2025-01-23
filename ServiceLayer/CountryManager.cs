using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using DataLayer;

namespace ServiceLayer
{
    public class CountryManager
    {
        private readonly CountryContext countryContext;

        public CountryManager(CountryContext countryContext)
        { 
            this.countryContext = countryContext;
        }

        public async Task CreateAsync(Country item)
        {
            await countryContext.CreateAsync(item);
        }

        public async Task<Country> ReadAsync(string key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            return await countryContext.ReadAsync(key, useNavigationalProperties, isReadOnly);
        }

        public async Task<ICollection<Country>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            return await countryContext.ReadAllAsync(useNavigationalProperties, isReadOnly);
        }

        public async Task UpdateAsync(Country item, bool useNavigationalProperties = false)
        {
            await countryContext.UpdateAsync(item, useNavigationalProperties);
        }

        public async Task DeleteAsync(string key)
        {
            await countryContext.DeleteAsync(key);
        }
    }
}
