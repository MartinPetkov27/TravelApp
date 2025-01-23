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
    public class TripsController : Controller
    {
            private readonly TripManager tripManager;

            public TripsController(TripManager tripManager)
            {
                this.tripManager = tripManager;
            }

            // GET: Trips
            public async Task<IActionResult> Index()
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

            // GET: Trips/Create
            public IActionResult Create()
            {
                return View();
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

                var trip = await tripManager.ReadAsync((int)id, true, true);
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
                var trip = await tripManager.ReadAsync(id);
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
        }
}
