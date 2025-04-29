using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using DataLayer;

namespace ServiceLayer
{
    public class PlaceManager
    {
        private readonly PlaceContext placeContext;

        //Methods for managing the placeContext for CRUD operations in Place 
        public PlaceManager(PlaceContext placeContext)
        { 
            this.placeContext = placeContext;
        }

        public async Task CreateAsync(Place item)
        {
            await placeContext.CreateAsync(item);
        }
        public async Task<Place> ReadAsync(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            return await placeContext.ReadAsync(key, useNavigationalProperties, isReadOnly);
        }
        public async Task<ICollection<Place>> ReadAllAsync(bool useNAvigationalProperties = false, bool isReadOnly = true)
        {
            return await placeContext.ReadAllAsync(useNAvigationalProperties, isReadOnly);
        }
        public async Task UpdateAsync(Place item, bool useNaigationalProperties = false) 
        {
            await placeContext.UpdateAsync(item, useNaigationalProperties);
        }
        public async Task DeleteAsync(int key) 
        { 
            await placeContext.DeleteAsync(key);
        }
    }
}
