using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using DataLayer;

namespace ServiceLayer
{
    public class TripManager
    {
        private readonly TripContext tripContext;

        public TripManager(TripContext tripContext)
        {
            this.tripContext = tripContext;
        }

        public async Task CreateAsync(Trip item)
        {
            await tripContext.CreateAsync(item);
        }
        public async Task<Trip> ReadAsync(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            return await tripContext.ReadAsync(key, useNavigationalProperties, isReadOnly);
        }
        public async Task<Trip> GetIdByName(string title)
        {
            ICollection<Trip> trips = await tripContext.ReadAllAsync(false, true);
            return trips.FirstOrDefault(t => t.Title == title);
        }
        public async Task<ICollection<Trip>> ReadAllAsync(bool useNAvigationalProperties = false, bool isReadOnly = true)
        {
            return await tripContext.ReadAllAsync(useNAvigationalProperties, isReadOnly);
        }
        public async Task UpdateAsync(Trip item, bool useNaigationalProperties = false)
        {
            await tripContext.UpdateAsync(item, useNaigationalProperties);
        }
        public async Task DeleteAsync(int key)
        {
            await tripContext.DeleteAsync(key);
        }
    }
}
