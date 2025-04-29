using BusinessLayer;
using DataLayer;

namespace ServiceLayer
{
    public class BucketListManager
    {
        private readonly BucketListContext bucketListContext;

        //Methods for managing the bucketListContext for CRUD operaatin in BucketList
        public BucketListManager(BucketListContext context)
        {
            bucketListContext = context;
        }

        public async Task CreateAsync(BucketList bucketList)
        {
            await bucketListContext.CreateAsync(bucketList); 
        }

        public async Task<BucketList> ReadAsync(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            return await bucketListContext.ReadAsync(key, useNavigationalProperties, isReadOnly);
        }

        public async Task<ICollection<BucketList>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
        { 
            return await bucketListContext.ReadAllAsync(useNavigationalProperties, isReadOnly);
        }

        public async Task UpdateAsync(BucketList item, bool useNavigationalProperties = false)
        {
            await bucketListContext.UpdateAsync(item, useNavigationalProperties);
        }
        public async Task DeleteAsync(int key)
        {
            await bucketListContext.DeleteAsync(key);
        }
    }
}
