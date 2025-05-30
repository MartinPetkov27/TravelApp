using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessLayer;
using DataLayer;
using ServiceLayer;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using PresentationLayer.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace PresentationLayer.Controllers
{
    public class CountriesController : Controller
    {
        private readonly CountryManager countryManager;
        private readonly TripManager tripManager;

        public CountriesController(CountryManager countryManager, TripManager tripManager)
        {
            this.countryManager = countryManager;   
            this.tripManager = tripManager;
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
            return View(await countryManager.ReadAllAsync(true, true));
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(string? id, int tripId)
        {
            if (id == null)
            {
                return NotFound();
            }

            Country country = await countryManager.ReadAsync((string)id, true, true);

            if (country == null)
            {
                return NotFound();
            }

            EditTripPlacesByCountryViewModel viewModel = new EditTripPlacesByCountryViewModel();
            viewModel.Country = country;
            viewModel.TripId = tripId;

            return View(viewModel);
        }

        // GET: Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlphaCode,Name,Capital,Currency,Language")] Country country)
        {
            if (ModelState.IsValid)
            {
                await countryManager.CreateAsync(country);
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        [HttpGet]
        public async Task<IActionResult> AddCountryToTrip(int tripId)
        {
            Trip trip = await tripManager.ReadAsync(tripId, false, true);

            ViewBag.TripId = tripId;
            //ViewBag.TripTitle = trip.Title;    
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCountryToTrip(AddCountryToTripViewModel viewModel)
        {
            Trip trip = new Trip();
            if (ModelState.IsValid)
            {
                Country country = await countryManager.GetCountryByName(viewModel.CountryName);
                trip = await tripManager.ReadAsync(viewModel.TripId, true, true);

                trip.Countries.Add(country);
                await tripManager.UpdateAsync(trip, true);

                if (country.Trips == null)
                {
                    country.Trips = new List<Trip>() { trip };
                    countryManager.UpdateAsync(country, true);
                }
                else 
                {
                    country.Trips.Add(trip);
                    countryManager.UpdateAsync(country, true);
                }
                
                return RedirectToAction(nameof(EditTripCountries), new { tripId = viewModel.TripId });
            }
            return View(trip.Id);
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await countryManager.ReadAsync(id, true, true);
            
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> EditTripCountries(int tripId)
        {
            ViewBag.TripId = tripId;
            return View(await countryManager.ReadByTripAsync(tripId, true, true));
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AlphaCode,Name,Capital,Currency,Language")] Country country)
        {
            if (id != country.AlphaCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   await countryManager.UpdateAsync(country, true);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.AlphaCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await countryManager.ReadAsync(id, true, true);
            
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var country = await countryManager.ReadAsync(id);
            if (country != null)
            {
                await countryManager.DeleteAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Countries/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteCountryFromTrip(string id, int tripId)
        {
            Trip trip = await tripManager.ReadAsync(tripId, true, true);

            if (trip != null)
            {
                TempData["TripName"] = trip.Title;
                TempData["TripId"] = trip.Id;
            }
            else
            { TempData["TripId"] = "no title"; TempData["TripId"] = 0; }

            if (id == null)
            {
                return NotFound();
            }

            var country = await countryManager.ReadAsync(id, true, true);

            if (country == null)
            {
                return NotFound();
            }

            DeleteCountryFromTripViewModel viewModel = new DeleteCountryFromTripViewModel();
            viewModel.Country = country;
            viewModel.TripId = tripId;

            return View(viewModel);
        }
        // POST: Countries/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCountryFromTripConfirmed(DeleteCountryFromTripViewModel viewModel)
        {
            var country = await countryManager.GetCountryByName(viewModel.Country.Name);

            var trip = await tripManager.ReadAsync(viewModel.TripId,true, true);

            if (country != null && trip != null)
            {
                trip.Countries.Remove(country);
                await tripManager.UpdateAsync(trip, true);
            }
            
            return RedirectToAction(nameof(EditTripCountries), new { tripId = viewModel.TripId});
        }

        private bool CountryExists(string id)
        {
            return countryManager.ReadAsync(id) is not null;
        }


    }
}
