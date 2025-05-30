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

        //Create method for the BucketList
        public async Task CreateAsync(BucketList item)
        {
            try
            {
                if (item.Destinations != null)
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
                }

                dbContext.BucketLists.Add(item);
                dbContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Read method for the BucketList
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

        //ReadAll method for the BucketList
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

        //Update method for the BucketList
        public async Task UpdateAsync(BucketList item, bool useNavigationalProperties = false)
        {
            try
            {
                BucketList bucketListFromDb = await ReadAsync(item.Id, useNavigationalProperties, false);
                
                bucketListFromDb.Name = item.Name;
                bucketListFromDb.Place1 = item.Place1;
                bucketListFromDb.Place2 = item.Place2;
                bucketListFromDb.Place3 = item.Place3;
                bucketListFromDb.Place4 = item.Place4;
                bucketListFromDb.Place5 = item.Place5;
                bucketListFromDb.ImageUrl1 = item.ImageUrl1;
                bucketListFromDb.ImageUrl2  = item.ImageUrl2;
                bucketListFromDb.ImageUrl3 = item.ImageUrl3;
                bucketListFromDb.ImageUrl4 = item.ImageUrl4;
                bucketListFromDb.ImageUrl5 = item.ImageUrl5;

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

        //Delete method for the BucketList
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
