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
using System.Net.Http.Headers;
using System.Text.Json;


namespace PresentationLayer.Controllers
{
   
    public class TripsController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly UserManager userManagerService;
        private readonly TripManager tripManager;
        private readonly GeoNamesService geoNamesService;
        private readonly CountryManager countryManager;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration config;
        //private readonly TripGeneratorController tripGeneratorController;

        public TripsController(TripManager tripManager, GeoNamesService geoNamesService, CountryManager countryManager, UserManager<User> userManager, /*TripGeneratorController tripGeneratorController,*/ IHttpClientFactory httpClientFactory, IConfiguration config, UserManager userManagerService)
        {
            this.tripManager = tripManager;
            this.geoNamesService = geoNamesService;
            this.countryManager = countryManager;
            this.userManager = userManager;
            //this.tripGeneratorController = tripGeneratorController;
            this.httpClientFactory = httpClientFactory;
            this.config = config;
            this.userManagerService = userManagerService;
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
        public async Task<IActionResult> CreateTripSelectCities(string Title, string CountryName)
        {
            var countries = await countryManager.ReadAllAsync(false);
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
                    Countries = new List<Country>(),
                    CountryIndexMap = new Dictionary<int, string>(),
                    User = user,
                    //StartingDate = trip.Places.OrderBy(p => p.StartingDate).FirstOrDefault().StartingDate,
                    //EndingDate = trip.Places.OrderBy(p => p.EndingDate).LastOrDefault().EndingDate
                };
                trip.Countries.Add(null); // or new Country() if you prefer
                trip.Countries[0] = country;
                trip.CountryIndexMap.Add(1, country.Name);
                trip.UserId = user.Id;
                trip.User = user;
                await tripManager.CreateAsync(trip);

                country.Trips = new List<Trip>() { trip };
                await countryManager.UpdateAsync(country, true);
            }

            //var lastEndingDate = trip.Places.OrderByDescending(p => p.EndingDate).FirstOrDefault()?.EndingDate;

            //if (lastEndingDate.HasValue)
            //{
            //    ViewBag.LastEndingDate = lastEndingDate.Value.AddDays(1).ToString("yyyy-MM-dd"); // Allow next place to start one day later
            //}
            //else
            //{
            //    ViewBag.LastEndingDate = DateTime.Today.ToString("yyyy-MM-dd"); // fallback if no places yet
            //}

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
                    trip.Countries.ToList();
                    trip.Countries.Add(null);
                    trip.Countries[trip.Countries.Count-1] = country;
                    
                    //trip.CountryIndexMap.Add(trip.Countries.Count, country.Name);
                    
                    await tripManager.UpdateAsync(trip, true);
                }
            }

            return RedirectToAction("CreateTripSelectCities", new { Title = title, CountryName = trip.Countries.First().Name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDescription(int tripId, string description)
        {
            Trip trip = await tripManager.ReadAsync(tripId, true);
            if (trip == null)
                return NotFound();

            trip.Description = description;
            await tripManager.UpdateAsync(trip, true);

            return View("Index", await tripManager.ReadAllAsync(true, true));
        }


        //public IActionResult GenerateTrip(string continent, string targetGroup, int duration, string criteria)
        //{
        //    // TEMPORARY: Just return the input values as JSON so we know it's working
        //    var result = new
        //    {
        //        Continent = continent,
        //        TargetGroup = targetGroup,
        //        Duration = duration,
        //        Criteria = criteria
        //    };

        //    return Json(result);
        //}

        //[HttpPost("generate")]
        //public async Task<IActionResult> GenerateTrip([FromBody] TripCriteria criteria)
        //{
        //metoda shte ide w tripManagera i shte bude vikan tuk

        //return await tripGeneratorController.GenerateTrip(criteria);

        //            var prompt = $@"
        //Generate a simple trip plan for the following:
        //- Continent: {criteria.Continent}
        //- Target Group: {criteria.TargetGroup}
        //- Duration: {criteria.Duration} days
        //- Theme: {criteria.Criteria}

        //Return only a JSON array like this:
        //[
        //  {{
        //    ""country"": ""Italy"",
        //    ""places"": [""Rome"", ""Florence"", ""Venice""]
        //  }},
        //  {{
        //    ""country"": ""France"",
        //    ""places"": [""Paris"", ""Nice""]
        //  }}
        //]";

        //            using var http = new HttpClient();
        //            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your-openai-api-key");

        //            var requestBody = new
        //            {
        //                model = "gpt-4",
        //                messages = new[]
        //                {
        //            new { role = "user", content = prompt }
        //        },
        //                temperature = 0.7
        //            };

        //            var response = await http.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);

        //            if (!response.IsSuccessStatusCode)
        //                return StatusCode((int)response.StatusCode, "OpenAI API call failed");

        //            var resultJson = await response.Content.ReadAsStringAsync();

        //            // Extract assistant's message
        //            var result = JsonDocument.Parse(resultJson);
        //            var content = result.RootElement
        //                .GetProperty("choices")[0]
        //                .GetProperty("message")
        //                .GetProperty("content")
        //                .GetString();

        //            // Try to parse what the AI returned as actual JSON
        //            try
        //            {
        //                var jsonElement = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(content);
        //                return Ok(jsonElement); // ✅ Return as parsed JSON array
        //            }
        //            catch
        //            {
        //                return Ok(new { rawResponse = content }); // 🧪 fallback: return plain text
        //            }
        //}


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
                        await tripManager.UpdateAsync(trip, true);
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
                return tripManager.ReadAsync(id, false) is not null;
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
