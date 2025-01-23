using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DataLayer
{
    public class BucketListContext : IDb<BucketList, int>
    {
        private TravelAppDbContext dbContext;

        public BucketListContext(TravelAppDbContext dbContext)
        { 
            this.dbContext = dbContext;
        }
        public async Task CreateAsync(BucketList item)
        {
            try
            {
                List<Place> destinations = new List<Place>(item.Destinations.Count);

                foreach (var destination in item.Destinations)
                {
                    Place destinationFromDb = await dbContext.Places.FindAsync(destination.Id);

                    if (destinationFromDb is null)
                    {
                        destinations.Add(destination);
                    }
                    else
                    {
                        destinations.Add(destinationFromDb);
                    }
                }
                item.Destinations = destinations;

                dbContext.BucketLists.Add(item);
                dbContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<BucketList> ReadAsync(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<BucketList> query= dbContext.BucketLists;

                if (useNavigationalProperties)
                {
                    query = query.Include(b => b.Destinations);
                }

                if (isReadOnly)
                { 
                    query.AsNoTrackingWithIdentityResolution();
                }

                return await query.FirstOrDefaultAsync(b => b.Id == key);

            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ICollection<BucketList>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<BucketList> query = dbContext.BucketLists;

                if (useNavigationalProperties)
                {
                    query = query.Include(b => b.Destinations);
                }

                if (isReadOnly)
                {
                    query.AsNoTrackingWithIdentityResolution();
                }

                return await query.ToListAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task UpdateAsync(BucketList item, bool useNavigationalProperties = false)
        {
            try
            {
                BucketList bucketListFromDb = await ReadAsync(item.Id, useNavigationalProperties, false);
                
                bucketListFromDb.Name = item.Name;

                if (useNavigationalProperties)
                { 
                    List<Place> destinations = new List<Place>(item.Destinations.Count);

                    foreach (var destination in item.Destinations)
                    {
                        Place destinationFromDb = await dbContext.Places.FindAsync(destination.Id);

                        if (destinationFromDb is null)
                        {
                            destinations.Add(destination);
                        }
                        else
                        {
                            destinations.Add(destinationFromDb);
                        }
                    }

                    bucketListFromDb.Destinations = destinations;
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(int key)
        {
            try
            {
                BucketList bucketListFromDb = await ReadAsync(key, false, false);

                if (bucketListFromDb is null)
                {
                    throw new ArgumentException("BucketList with id - {0} does not exsist.", key.ToString());
                }

                dbContext.BucketLists.Remove(bucketListFromDb);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
