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
using PresentationLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    public class PlacesController : Controller
    {
        private readonly PlaceManager placeManager;
        private readonly CountryManager countryManager;
        private readonly TripManager tripManager;

        public PlacesController(PlaceManager placeManager, CountryManager countryManager, TripManager tripManager)
        {
            this.placeManager = placeManager;
            this.countryManager = countryManager;
            this.tripManager = tripManager;
        }

        // GET: Places
        public async Task<IActionResult> Index()
        {
            return View(await placeManager.ReadAllAsync(true, true));
        }

        // GET: Places/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await placeManager.ReadAsync((int)id, true, true);
            if (place == null)
            {
                return NotFound();
            }

            return View(place);
        }

        // GET: Places/Create
        public async Task<IActionResult> CreateFromTrip(CreatePlaceFromTripViewModel viewModel)
        {
            var countries = await countryManager.ReadAllAsync();
            Trip trip = await tripManager.GetIdByName(viewModel.TripName);
            int tripId = trip.Id;
            string selectedCountryName = trip.Countries.Skip(viewModel.CountryIndex).FirstOrDefault()?.Name;

            ViewBag.Countries = countries.Select(c => c.Name).ToList(); // Or .Distinct() if needed
            ViewBag.SelectedCountry = selectedCountryName;

            return View();
        }

        // POST: Places/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Latitude,Longitude,CoutryAlphaCode,Description")] Place place)
        {
            if (ModelState.IsValid)
            {
                await placeManager.CreateAsync(place);
                return RedirectToAction(nameof(Index));
            }
            return View(place);
        }

        [HttpPost]
        [Route("Places/CreateFromBucketList")]
        public async Task<IActionResult> CreateFromBucketList([FromBody] Place place)
        {
            if (place == null || string.IsNullOrEmpty(place.Name) || string.IsNullOrEmpty(place.CoutryAlphaCode))
            {
                return BadRequest("Invalid place data.");
            }

            // Optionally, fetch the Country entity based on CoutryAlphaCode if needed
            place.Country = await countryManager.ReadAsync(place.CoutryAlphaCode);

            await placeManager.CreateAsync(place);

            return Ok(place); // Return the created Place (with ID) to frontend
        }

        // GET: Places/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await placeManager.ReadAsync((int)id, true, true);
            if (place == null)
            {
                return NotFound();
            }
            return View(place);
        }

        // POST: Places/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Latitude,Longitude,CoutryAlphaCode,Description")] Place place)
        {
            if (id != place.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await placeManager.UpdateAsync(place);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaceExists(place.Id))
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
            return View(place);
        }

        // GET: Places/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await placeManager.ReadAsync((int)id, true, true);
            if (place == null)
            {
                return NotFound();
            }

            return View(place);
        }

        // POST: Places/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var place = await placeManager.ReadAsync(id);
            if (place != null)
            {
                await placeManager.DeleteAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PlaceExists(int id)
        {
            return placeManager.ReadAsync(id) is not null;
        }
    }
}
