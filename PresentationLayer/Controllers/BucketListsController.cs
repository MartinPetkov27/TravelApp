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
using System.Diagnostics.Metrics;

namespace PresentationLayer.Controllers
{
    public class BucketListsController : Controller
    {
        private readonly BucketListManager bucketListManager;
        private readonly CountryManager countryManager;

        public BucketListsController(BucketListManager bucketListManager, CountryManager countryManager)
        {
            this.bucketListManager = bucketListManager;
            this.countryManager = countryManager;
        }

        // GET: BucketLists
        public async Task<IActionResult> Index()
        {
            return View(await bucketListManager.ReadAllAsync(true, true));
        }
        // GET: Trips
        public async Task<IActionResult> IndexAdmin()
        {
            return View(await bucketListManager.ReadAllAsync(true, true));
        }

        // GET: BucketLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BucketList bucketList = await bucketListManager.ReadAsync((int)id, true, true);
            if (bucketList == null)
            {
                return NotFound();
            }

            return View(bucketList);
        }

        // GET: BucketLists/Create
        public async Task<IActionResult> Create()
        {
            var countries = await countryManager.ReadAllAsync();
            ViewBag.Countries = countries.ToList();


            return View();
        }

        // POST: BucketLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Id,Name,UserId")]*/ BucketList bucketList)
        {
            if (ModelState.IsValid)
            {
                await bucketListManager.CreateAsync(bucketList);
                return RedirectToAction(nameof(Index));
            }
            return View(bucketList);
        }

        // GET: BucketLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bucketList = await bucketListManager.ReadAsync((int)id, true, true);
            if (bucketList == null)
            {
                return NotFound();
            }
            return View(bucketList);
        }

        // POST: BucketLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UserId")] BucketList bucketList)
        {
            if (id != bucketList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   await bucketListManager.UpdateAsync(bucketList);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BucketListExists(bucketList.Id))
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
            return View(bucketList);
        }

        // GET: BucketLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bucketList = await bucketListManager.ReadAsync((int)id, true, true);
            if (bucketList == null)
            {
                return NotFound();
            }

            return View(bucketList);
        }

        // POST: BucketLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bucketList = await bucketListManager.ReadAsync((int)id, true, true);
            if (bucketList != null)
            {
                await bucketListManager.DeleteAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private  bool BucketListExists(int id)
        {
            return bucketListManager.ReadAsync(id, true, true)is not null;
        }
    }
}
