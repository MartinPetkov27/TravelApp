using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class StoryContext : IDb<Story, int>
    {
        private readonly TravelAppDbContext dbContext;

        public StoryContext(TravelAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //Create method for the Story
        public async Task CreateAsync(Story item)
        {
            try
            {
                dbContext.Stories.Add(item);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Read method for the Story
        public async Task<Story> ReadAsync(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<Story> query = dbContext.Stories;

                if(useNavigationalProperties)
                {
                    query = query.Include(s => s.User);
                }

                if (isReadOnly)
                { 
                    query = query.AsNoTrackingWithIdentityResolution();
                }

                return await query.FirstOrDefaultAsync(s => s.Id == key);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //ReadAll method for the Story
        public async Task<ICollection<Story>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<Story> query = dbContext.Stories;

                if (useNavigationalProperties)
                { 
                    query.Include(s => s.User); 
                }

                if (isReadOnly)
                {
                    query = query.AsNoTrackingWithIdentityResolution();
                }

                return await query.ToArrayAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Update method for the Story
        public async Task UpdateAsync(Story item, bool useNavigationalProperties = false)
        {
            try
            {
                Story storyFromDb = await ReadAsync(item.Id, useNavigationalProperties, false);

                storyFromDb.Id = item.Id;   
                storyFromDb.Title = item.Title;
                storyFromDb.Content = item.Content;
                storyFromDb.Status = item.Status;

                if (useNavigationalProperties)
                {
                    User userFromDb = await dbContext.Users.FindAsync(item.User.Id);
                    if (userFromDb != null)
                    {
                        storyFromDb.User = userFromDb;
                    }
                    else
                    { 
                        storyFromDb.User = item.User;
                    }
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            { 
                throw;
            }
        }

        //Delete method for the Story
        public async Task DeleteAsync(int key)
        {
            try
            {
                Story storyFromDb = await ReadAsync(key);

                if (storyFromDb == null)
                {
                    throw new ArgumentException("Story with id - {0} does not exsist.", key.ToString());
                }

                dbContext.Stories.Remove(storyFromDb);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        } 
    }
}
