using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace DataLayer
{
    public class UserContext : IDb<User, int>
    {
        private readonly TravelAppDbContext dbContext;

        public UserContext(TravelAppDbContext dbContext)
        { 
            this.dbContext = dbContext; 
        }
        public async Task CreateAsync(User item)
        {
            try
            {
                #region Check for the Trips navigational property
                
                List<Trip> trips = new List<Trip>(item.Trips.Count);

                foreach (var trip in item.Trips)
                {
                    Trip tripFromDb = await dbContext.Trips.FindAsync(trip.Id);

                    if (tripFromDb is null)
                    {
                        trips.Add(trip);
                    }
                    else
                    {
                        trips.Add(tripFromDb);
                    }
                }
                item.Trips = trips;
                #endregion

                #region Check for the BucketLists navigational property
                List<BucketList> bucketLists= new List<BucketList>(item.BucketLists.Count);

                foreach (var bucketList in item.BucketLists)
                {
                    BucketList bucketListFromDb = await dbContext.BucketLists.FindAsync(bucketList.Id);

                    if (bucketListFromDb is null)
                    {
                        bucketLists.Add(bucketList);
                    }
                    else
                    {
                        bucketLists.Add(bucketListFromDb);
                    }
                }
                item.BucketLists = bucketLists;
                #endregion

                #region Check for the Stories navigational property 
                List<Story> stories = new List<Story>(item.Stories.Count);

                foreach (var story in item.Stories)
                {
                    Story storyFromDb = await dbContext.Stories.FindAsync(story.Id);

                    if (storyFromDb is null)
                    {
                        stories.Add(story);
                    }
                    else
                    {
                        stories.Add(storyFromDb);
                    }
                }
                item.Stories = stories;
                #endregion

                dbContext.Users.Add(item);
                dbContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<User> ReadAsync(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<User> query = dbContext.Users;

                if (useNavigationalProperties)
                { 
                    query = query.Include(u => u.Trips).Include(u => u.BucketLists).Include(u => u.Stories);
                }

                if (isReadOnly) { query = query.AsNoTrackingWithIdentityResolution(); }

                return await query.FirstOrDefaultAsync(u => u.Id == key);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ICollection<User>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<User> query = dbContext.Users;

                if (useNavigationalProperties)
                {
                    query = query.Include(u => u.Trips).Include(u => u.BucketLists).Include(u => u.Stories);
                }

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
        public async Task UpdateAsync(User item, bool useNavigationalProperties = false)
        {
            User userFromDb = await ReadAsync(item.Id, useNavigationalProperties, false);

            userFromDb.Id = item.Id;
            userFromDb.Username = item.Username;
            userFromDb.Password = item.Password;
            userFromDb.Email = item.Email;
            userFromDb.FirstName = item.FirstName;  
            userFromDb.LastName = item.LastName;
            userFromDb.Role = item.Role;

            if (useNavigationalProperties)
            {
                #region Check for the Trips navigational property

                List<Trip> trips = new List<Trip>(item.Trips.Count);

                foreach (var trip in item.Trips)
                {
                    Trip tripFromDb = await dbContext.Trips.FindAsync(trip.Id);

                    if (tripFromDb is null)
                    {
                        trips.Add(trip);
                    }
                    else
                    {
                        trips.Add(tripFromDb);
                    }
                }
                item.Trips = trips;
                #endregion

                #region Check for the BucketLists navigational property
                List<BucketList> bucketLists = new List<BucketList>(item.BucketLists.Count);

                foreach (var bucketList in item.BucketLists)
                {
                    BucketList bucketListFromDb = await dbContext.BucketLists.FindAsync(bucketList.Id);

                    if (bucketListFromDb is null)
                    {
                        bucketLists.Add(bucketList);
                    }
                    else
                    {
                        bucketLists.Add(bucketListFromDb);
                    }
                }
                item.BucketLists = bucketLists;
                #endregion

                #region Check for the Stories navigational property 
                List<Story> stories = new List<Story>(item.Stories.Count);

                foreach (var story in item.Stories)
                {
                    Story storyFromDb = await dbContext.Stories.FindAsync(story.Id);

                    if (storyFromDb is null)
                    {
                        stories.Add(story);
                    }
                    else
                    {
                        stories.Add(storyFromDb);
                    }
                }
                item.Stories = stories;
                #endregion
            }

            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(int key)
        {
            try
            {
                User userFromDb = await ReadAsync(key, false, false);

                if (userFromDb is null)
                {
                    throw new ArgumentException("User with id - {0} does not exsist.", key.ToString());
                }

                dbContext.Remove(userFromDb);
                await dbContext.SaveChangesAsync(); 
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
