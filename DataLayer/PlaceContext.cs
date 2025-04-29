using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class PlaceContext : IDb<Place, int>
    {
        private readonly TravelAppDbContext dbContext;

        public PlaceContext(TravelAppDbContext dbContext)
        { 
            this.dbContext = dbContext;
        }
        public async Task CreateAsync(Place item)
        {
            try
            {
                if (item.Country != null)
                {
                    var attachedCountry = item.Country;

                  
                        var existingCountry = await dbContext.Countries.FindAsync(item.Country.AlphaCode);

                        if (existingCountry != null)
                        {
                            attachedCountry = existingCountry;
                        }
                        else
                        {
                            attachedCountry = item.Country;
                        }

                    item.Country= attachedCountry;
                }


                if (item.Trip != null)
                {
                    var attachedTrip = item.Trip;
                    var existingTrip = await dbContext.Trips.FindAsync(item.Trip.Id);
                    if (existingTrip != null)
                    {
                        attachedTrip = existingTrip;
                    }
                    else
                    {
                        attachedTrip = item.Trip;
                    }
                    item.Trip = attachedTrip;
                }

                dbContext.Places.Add(item);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Place> ReadAsync(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<Place> query = dbContext.Places;

                if (useNavigationalProperties)
                {
                    query = query.Include(p => p.Country).Include(p => p.Trip);
                }

                if (isReadOnly)
                {
                    query = query.AsNoTrackingWithIdentityResolution();
                }

                return await query.FirstOrDefaultAsync(t => t.Id == key);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ICollection<Place>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<Place> query = dbContext.Places;

                if (useNavigationalProperties)
                {
                    query = query.Include(p => p.Country).Include(p => p.Trip);
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
        public async Task UpdateAsync(Place item, bool useNavigationalProperties = false)
        {
            try
            {
                Place placeFromDb = await ReadAsync(item.Id, useNavigationalProperties, false);

                placeFromDb.Id = item.Id;
                placeFromDb.Name = item.Name;
                placeFromDb.Description = item.Description;
                placeFromDb.Longitude = item.Longitude;
                placeFromDb.Latitude = item.Latitude;

                if (useNavigationalProperties)
                {
                    Country countryFromDb = await dbContext.Countries.FindAsync(item.Country.AlphaCode);
                    if (countryFromDb != null)
                    {
                        placeFromDb.Country = countryFromDb;
                    }
                    else
                    {
                        placeFromDb.Country = item.Country;
                    }

                    Trip tripFromDb = await dbContext.Trips.FindAsync(item.Trip.Id);
                    if (tripFromDb != null)
                    {
                        placeFromDb.Trip = tripFromDb;
                    }
                    else
                    { 
                        placeFromDb.Trip = item.Trip;
                    }
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
                Place placeFromDb = await ReadAsync(key);

                if (placeFromDb == null)
                {
                    throw new ArgumentException("Place with id - {0} does not exsist.", key.ToString());
                }

                dbContext.Places.Remove(placeFromDb);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
