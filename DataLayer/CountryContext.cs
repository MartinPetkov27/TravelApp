using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataLayer
{
    public class CountryContext : IDb<Country, string>
    {
        private readonly TravelAppDbContext dbContext;

        public CountryContext(TravelAppDbContext dbContext)
        { 
            this.dbContext = dbContext;   
        }

        //Create method for the Country
        public async Task CreateAsync(Country item)
        {
            try
            {
                dbContext.Countries.Add(item);  
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            { 
                throw;
            }
        }

        //Read method for the Country
        public async Task<Country> ReadAsync(string key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<Country> query = dbContext.Countries;

                if (isReadOnly)
                {
                    query = query.AsNoTrackingWithIdentityResolution();
                }

                return await query.FirstOrDefaultAsync(c => c.AlphaCode == key);
            }
            catch (Exception)
            {

                throw;
            }
        }

        //ReadAll method for the Country
        public async Task<ICollection<Country>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<Country> query = dbContext.Countries;

                if (isReadOnly)
                {
                    query = query.AsNoTrackingWithIdentityResolution();
                }

                return await query.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Update method for the Country
        public async Task UpdateAsync(Country item, bool useNavigationalProperties = false)
        {
            try
            {
                Country countryFromDb = await ReadAsync(item.AlphaCode, useNavigationalProperties, false);
                
                countryFromDb.AlphaCode = item.AlphaCode;
                countryFromDb.Name = item.Name;
                countryFromDb.Currency = item.Currency;
                countryFromDb.Capital = item.Capital;
                countryFromDb.Language = item.Language;

                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Delete method for the Country
        public async Task DeleteAsync(string key)
        {
            try
            {
                Country countryFromDb = await ReadAsync(key, false, false);

                if (countryFromDb == null)
                {
                    throw new ArgumentException("Country with alpha code - {0} does not exsits", key);
                }
                dbContext.Countries.Remove(countryFromDb);  
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
