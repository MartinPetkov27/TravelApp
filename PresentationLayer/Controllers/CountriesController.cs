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
    public class CountriesController : Controller
    {
        private readonly CountryManager countryManager;

        public CountriesController(CountryManager countryManager)
        {
            this.countryManager = countryManager;   
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
            return View(await countryManager.ReadAllAsync(true, true));
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(string? id)
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

            return View(country);
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
                   await countryManager.UpdateAsync(country);
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

        private bool CountryExists(string id)
        {
            return countryManager.ReadAsync(id) is not null;
        }
    }
}
