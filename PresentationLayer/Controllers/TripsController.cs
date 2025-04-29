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
using Newtonsoft.Json;
using static PresentationLayer.Models.GeoNamesCountryDTO;
using System.Net.Http;
using PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace PresentationLayer.Controllers
{
    public class TripsController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly TripManager tripManager;
        private readonly GeoNamesService geoNamesService;
        private readonly CountryManager countryManager;

        public TripsController(TripManager tripManager, GeoNamesService geoNamesService, CountryManager countryManager, UserManager<User> userManager)
        {
            this.tripManager = tripManager;
            this.geoNamesService = geoNamesService;
            this.countryManager = countryManager;
            this.userManager = userManager;
        }

        // GET: Trips
        public async Task<IActionResult> Index()
        {
            return View(await tripManager.ReadAllAsync(true, true));
        }
        // GET: Trips
        public async Task<IActionResult> IndexAdmin()
        {
            return View(await tripManager.ReadAllAsync(true, true));
        }
        // GET: Trips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await tripManager.ReadAsync((int)id, true, true);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View("CreateTripStart", new CreateTripStartViewModel());
        }

        //For the page for choosing starting country
        [HttpGet]
        public async Task<IActionResult> CreateTripStart(CreateTripStartViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel); 

            return RedirectToAction("CreateTripSelectCities", new { countryName = viewModel.CountryName, title = viewModel.Title });
        }

        //Funtions available : add place, add country, generate trip 
        [HttpGet]
        public async Task<IActionResult> CreateTripSelectCities(string Title, string CountryName )
        {
            var countries = await countryManager.ReadAllAsync();
            ViewBag.Countries = countries.Select(c => c.Name).ToList();

            var trip = (await tripManager.ReadAllAsync(true, true))
            .FirstOrDefault(t => t.Title == Title);

            if (trip == null)
            {
                // If trip doesn't exist yet, you can optionally create it
                var country = await countryManager.GetCountryByName(CountryName);

                if (country == null) return NotFound();

                var user = await userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                trip = new Trip
                {
                    Title = Title,
                    Countries = new List<Country> { country },
                    User = user
                };
                await tripManager.CreateAsync(trip);
            }

            return View(trip);
        }

        //For adding countries
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCountryToTrip(string title, string nextCountry)
        {
            var trip = (await tripManager.ReadAllAsync(true, true))
                .FirstOrDefault(t => t.Title == title);

            if (trip == null)
            {
                return NotFound("Trip not found");
            }

            if (!string.IsNullOrEmpty(nextCountry))
            {
                var country = await countryManager.GetCountryByName(nextCountry);

                if (country != null && !trip.Countries.Any(c => c.AlphaCode == country.AlphaCode))
                {
                    trip.Countries.Add(country);

                    await tripManager.UpdateAsync(trip, true);
                }
            }

            return RedirectToAction("CreateTripSelectCities", new { Title = title, CountryName = trip.Countries.First().Name });
        }

        // POST: Trips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,StartingDate,EndingDate,StaritingPlaceId,EndingPlaceId,UserId")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                await tripManager.CreateAsync(trip);
                return RedirectToAction(nameof(Index));
            }
            return View(trip);
        }

        // GET: Trips/Edit/5
        public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var trip = await tripManager.ReadAsync((int)id, true, true);
                if (trip == null)
                {
                    return NotFound();
                }
                return View(trip);
            }

            // POST: Trips/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,StartingDate,EndingDate,StaritingPlaceId,EndingPlaceId,UserId")] Trip trip)
            {
                if (id != trip.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        await tripManager.UpdateAsync(trip);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TripExists(trip.Id))
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
                return View(trip);
            }

            // GET: Trips/Delete/5
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var trip = await tripManager.ReadAsync((int)id, false, true);
                if (trip == null)
                {
                    return NotFound();
                }

                return View(trip);
            }

            // POST: Trips/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var trip = await tripManager.ReadAsync(id, false, true);
                if (trip != null)
                {
                    await tripManager.DeleteAsync(id);
                }

                return RedirectToAction(nameof(Index));
            }

            private bool TripExists(int id)
            {
                return tripManager.ReadAsync(id) is not null;
            }

            [HttpGet]
            public async Task<IActionResult> GetCountries()
            {
                using (var httpClient = new HttpClient())
                {
                var response = await httpClient.GetStringAsync("https://raw.githubusercontent.com/johan/world.geo.json/master/countries.geo.json");
                return Content(response, "application/json");
                }
            }

            [HttpGet]
            public async Task<IActionResult> FetchCountry(string alpha2Code)
            {
                var country = await geoNamesService.GetCountryInfoAsync(alpha2Code);
                if (country == null)
                    return NotFound();

                return Json(country);
            }
    }
}
