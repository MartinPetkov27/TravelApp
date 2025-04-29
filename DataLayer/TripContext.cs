using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class TripContext : IDb<Trip, int>
    {
        private readonly TravelAppDbContext dbContext;

        public TripContext(TravelAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //Create method for the Trip
        public async Task CreateAsync(Trip item)
        {
            try
            {
                if (item.Countries != null && item.Countries.Count > 0)
                {
                    var attachedCountries = new List<Country>(item.Countries.Count);

                    foreach (var country in item.Countries)
                    {
                        var existingCountry = await dbContext.Countries.FindAsync(country.AlphaCode);

                        if (existingCountry != null)
                        {
                            attachedCountries.Add(existingCountry);
                        }
                        else
                        {
                            attachedCountries.Add(country);
                        }
                    }

                    item.Countries = attachedCountries;
                }

                dbContext.Trips.Add(item);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Read method for the Trip
        public async Task<Trip> ReadAsync(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<Trip> query = dbContext.Trips;

                if (useNavigationalProperties)
                {
                    query = query.Include(t => t.User).Include(t => t.Places).Include(t => t.Countries);
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

        //ReadAll method for the Trip
        public async Task<ICollection<Trip>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            try
            {
                IQueryable<Trip> query = dbContext.Trips;
                
                if (useNavigationalProperties)
                {
                    query = query.Include(t => t.User).Include(t => t.Places).Include(t => t.Countries);
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

        //Update method for the Trip
        public async Task UpdateAsync(Trip item, bool useNavigationalProperties = false)
        {
            try
            {
                Trip tripFromDb = await ReadAsync(item.Id, useNavigationalProperties, false);

                tripFromDb.Id = item.Id;
                tripFromDb.Title = item.Title;
                tripFromDb.Description = item.Description;
                tripFromDb.StartingDate = item.StartingDate;
                tripFromDb.EndingDate = item.EndingDate;
                
                if (useNavigationalProperties)
                {
                    #region Places
                    List<Place> places = new List<Place>(item.Places.Count);

                    foreach (var place in item.Places)
                    {
                        Place placeFromDb = await dbContext.Places.FindAsync(place.Id);

                        if (placeFromDb is null)
                        {
                            places.Add(place);
                        }
                        else
                        {
                            places.Add(placeFromDb);
                        }
                    }
                    tripFromDb.Places = places;
                    if (places.Count > 0)
                    {
                        tripFromDb.StartingPlace = places[0];
                        tripFromDb.EndingPlace = places[places.Count - 1];
                    }
                    else
                    {
                        tripFromDb.StartingPlace = null;
                        tripFromDb.EndingPlace = null;
                    }
                    #endregion

                    #region User
                    User userFromDb = await dbContext.Users.FindAsync(item.User.Id);
                    if (userFromDb != null)
                    {
                        tripFromDb.User = userFromDb;
                    }
                    else
                    {
                        tripFromDb.User = item.User;
                    }
                    #endregion

                    #region Countries
                    List<Country> countries = new List<Country>(item.Countries.Count);
                    foreach (var country in item.Countries)
                    {
                        Country countryFromDb = await dbContext.Countries.FindAsync(country.AlphaCode);
                        if (countryFromDb is null)
                        {
                            countries.Add(country);
                        }
                        else
                        {
                            countries.Add(countryFromDb);
                        }
                    }
                    tripFromDb.Countries = countries;
                    #endregion

                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Delete method for the Trip
        public async Task DeleteAsync(int key)
        {
            try
            {
                Trip tripFromDb = await ReadAsync(key, false, false);

                if (tripFromDb == null)
                {
                    throw new ArgumentException("Trip with id - {0} does not exsist.", key.ToString());
                }

                dbContext.Trips.Remove(tripFromDb);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
