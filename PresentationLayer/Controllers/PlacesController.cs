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
        public async Task<IActionResult> Create(string tripName)
        {
            var countries = await countryManager.ReadAllAsync();
            ViewBag.Countries = countries.Select(c => c.Name).ToList(); // Or .Distinct() if needed

            ViewBag.TripName = tripName;
            int tripId = await tripManager.GetIdByName(tripName);
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
