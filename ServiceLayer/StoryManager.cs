using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using DataLayer;

namespace ServiceLayer
{
    public class StoryManager
    {
        private readonly StoryContext storyContext;

        //Methods for managing the storyContext for CRUD operations in Story
        public StoryManager(StoryContext storyContext)
        { 
            this.storyContext = storyContext;
        }

        public async Task CreateAsync(Story item)
        {
            await storyContext.CreateAsync(item);
        }
        public async Task<Story> ReadAsync(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            return await storyContext.ReadAsync(key, useNavigationalProperties, isReadOnly);
        }
        public async Task<ICollection<Story>> ReadAllAsync(bool useNAvigationalProperties = false, bool isReadOnly = true)
        {
            return await storyContext.ReadAllAsync(useNAvigationalProperties, isReadOnly);
        }
        public async Task UpdateAsync(Story item, bool useNaigationalProperties = false)
        {
            await storyContext.UpdateAsync(item, useNaigationalProperties);
        }
        public async Task DeleteAsync(int key)
        {
            await storyContext.DeleteAsync(key);
        }
    }
}
