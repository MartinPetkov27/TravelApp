using System;
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
using System.Globalization;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics.Metrics;

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
        public async Task<IActionResult>EditTripPlaces(int tripId, string countryName)
        {
            Trip trip = await tripManager.ReadAsync(tripId, true, true);

            EditTripPlacesViewModel viewModel = new EditTripPlacesViewModel();
            viewModel.Trip = trip;
            viewModel.Country = await countryManager.GetCountryByName(countryName);
            viewModel.Places = trip.Places.Where(p => p.Country.Name == countryName).ToList();

            return View(viewModel);
        }

        // GET: Places/Details/5
        public async Task<IActionResult> Details(int? id, int tripId, string countryName)
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

            ViewBag.TripId = tripId;
            ViewBag.CountryName = countryName;

            return View(place);
        }

        //For adding place to a current trip
        // GET: Places/Create
        public async Task<IActionResult> CreateFromTrip(string tripName, int countryIndex)
        {
            var countries = await countryManager.ReadAllAsync(false);

            Trip trip = await tripManager.GetTripByNameAsync(tripName);
            int tripId = trip.Id;

            string? selectedCountryName = trip.Countries.Skip(countryIndex).FirstOrDefault()?.Name;
            string? selectedCountryAlphaCode = trip.Countries.Skip(countryIndex).FirstOrDefault()?.AlphaCode;

            ViewBag.Countries = countries.Select(c => c.Name).ToList(); // Or .Distinct() if needed
            ViewBag.TripName = tripName;
            ViewBag.SelectedCountry = selectedCountryName;
            ViewBag.SelectedCountryAlphaCode = selectedCountryAlphaCode;
           
            DateTime? lastEndingDate = trip.Places
                .Where(p => p.EndingDate.HasValue)
                .OrderByDescending(p => p.EndingDate)
                .FirstOrDefault()?.EndingDate;

            if (lastEndingDate == null)
            {
                lastEndingDate = DateTime.Today;
            }

            ViewBag.LastEndingDate = lastEndingDate.Value.AddDays(1).ToShortDateString();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromTrip(Place place, string TripName)
        {
            if (!ModelState.IsValid)
            {
                return View(place); // If something is wrong, re-show form
            }

            Console.WriteLine("StartingDate: " + place.StartingDate);
            Console.WriteLine("EndingDate: " + place.EndingDate);

            // Find the trip
            Trip trip = await tripManager.GetTripByNameAsync(TripName);

            if (trip == null)
            {
                return NotFound("Trip not found");
            }

            // Find country (you can improve this if needed)
            var country = await countryManager.GetCountryByAlphaCode(place.CoutryAlphaCode);

            if (country == null)
            {
                return NotFound("Country not found");
            }

            //double latitude = Convert.ToDouble(place.Latitude, CultureInfo.InvariantCulture);
            //double longitude = Convert.ToDouble(place.Longitude, CultureInfo.InvariantCulture);

            ////Check for the real country of the selected place
            //var actualCountryName = await countryManager.GetCountryNameFromCoordinates(latitude, longitude);

            //if (actualCountryName == null || !actualCountryName.Equals(country.Name, StringComparison.OrdinalIgnoreCase))
            //{
            //    TempData["CountryMismatch"] = $"The coordinates belong to {actualCountryName}, but you selected {country.Name}.";
            //    return View("CreateNoSuccess");
            //}

            // Set navigation properties manually
            place.TripId = trip.Id;
            place.Trip = trip;
            place.Country = country;
            place.CoutryAlphaCode = country.AlphaCode;

            // Save the place
            await placeManager.CreateAsync(place);

            // Redirect somewhere, maybe back to trip page?
            //return RedirectToAction("Details", "Trips", new { id = trip.Id });
            ViewBag.TripId = trip.Id;
            ViewBag.TripName = trip.Title;

            //update trip ending and lasting date.

                return RedirectToAction("CreateSuccess", new
                {
                    tripName = TripName,
                    countryName = country.Name,
                    tripId = trip.Id
                });
        }

        #region CreateFromEditTrip
        //For adding place to a current trip
        // GET: Places/Create
        public async Task<IActionResult> CreateFromEditTrip(string tripName, string countryName)
        {
            var countries = await countryManager.ReadAllAsync(false);

            Trip trip = await tripManager.GetTripByNameAsync(tripName);
            int tripId = trip.Id;

            string? selectedCountryName = countryName;
            Country country = await countryManager.GetCountryByName(selectedCountryName);
            string? selectedCountryAlphaCode = country.AlphaCode;

            ViewBag.Countries = countries.Select(c => c.Name).ToList(); // Or .Distinct() if needed
            ViewBag.TripName = tripName;
            ViewBag.SelectedCountry = selectedCountryName;
            ViewBag.SelectedCountryAlphaCode = selectedCountryAlphaCode;

            DateTime? lastEndingDate = trip.Places
                .Where(p => p.EndingDate.HasValue)
                .OrderByDescending(p => p.EndingDate)
                .FirstOrDefault()?.EndingDate;

            if (lastEndingDate == null)
            {
                lastEndingDate = DateTime.Today;
            }

            ViewBag.LastEndingDate = lastEndingDate.Value.ToShortDateString();

            return View("CreateFromEditTrip");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromEditTrip(Place place, string TripName)
        {
            if (!ModelState.IsValid)
            {
                return View(place); // If something is wrong, re-show form
            }

            Console.WriteLine("StartingDate: " + place.StartingDate);
            Console.WriteLine("EndingDate: " + place.EndingDate);

            // Find the trip
            Trip trip = await tripManager.GetTripByNameAsync(TripName);

            if (trip == null)
            {
                return NotFound("Trip not found");
            }

            // Find country (you can improve this if needed)
            var country = await countryManager.GetCountryByAlphaCode(place.CoutryAlphaCode);

            if (country == null)
            {
                return NotFound("Country not found");
            }

            //double latitude = Convert.ToDouble(place.Latitude, CultureInfo.InvariantCulture);
            //double longitude = Convert.ToDouble(place.Longitude, CultureInfo.InvariantCulture);

            ////Check for the real country of the selected place
            //var actualCountryName = await countryManager.GetCountryNameFromCoordinates(latitude, longitude);

            //if (actualCountryName == null || !actualCountryName.Equals(country.Name, StringComparison.OrdinalIgnoreCase))
            //{
            //    TempData["CountryMismatch"] = $"The coordinates belong to {actualCountryName}, but you selected {country.Name}.";
            //    return View("CreateNoSuccess");
            //}

            // Set navigation properties manually
            place.TripId = trip.Id;
            place.Trip = trip;
            place.Country = country;
            place.CoutryAlphaCode = country.AlphaCode;

            // Save the place
            await placeManager.CreateAsync(place);

            // Redirect somewhere, maybe back to trip page?
            //return RedirectToAction("Details", "Trips", new { id = trip.Id });
            ViewBag.TripId = trip.Id;
            ViewBag.TripName = trip.Title;

            //update trip ending and lasting date.

            return RedirectToAction("EditTripPlaces", new
            {
                tripId = trip.Id,
                countryName = country.Name
            });
        }
        [HttpGet]
        public IActionResult CreateSuccessEditTrip(string tripName, string countryName, int tripId)
        {
            ViewBag.TripName = tripName;
            ViewBag.CountryName = countryName;
            ViewBag.TripId = tripId;

            return View();
        }
        #endregion
        [HttpGet]
        public IActionResult CreateSuccess(string tripName, string countryName, int tripId)
        {
            ViewBag.TripName = tripName;
            ViewBag.CountryName = countryName;
            ViewBag.TripId = tripId;

            return View();
        }
       
        [HttpGet]
        public IActionResult CreateNoSuccess(string tripName, string countryName, int tripId)
        {
            ViewBag.TripName = tripName;
            ViewBag.CountryName = countryName;
            ViewBag.TripId = tripId;

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
        public async Task<IActionResult> Delete(int? id, int tripId, string countryName)
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

            ViewBag.TripId = tripId;
            ViewBag.CountryName = countryName;

            return View(place);
        }

        // POST: Places/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int tripId, string countryName)
        {
            var place = await placeManager.ReadAsync(id, false);
            if (place != null)
            {
                await placeManager.DeleteAsync(id);
            }

            return RedirectToAction("EditTripPlaces", new
            {
                tripId = tripId,
                countryName = countryName
            });
        }

        private bool PlaceExists(int id)
        {
            return placeManager.ReadAsync(id, false) is not null;
        }
    }
}
