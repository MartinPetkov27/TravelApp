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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    public class StoriesController : Controller
    {
        private readonly StoryManager storyManager;
        private readonly UserManager<User> userManager;

        public StoriesController(StoryManager storyManager, UserManager<User> userManager)
        {
            this.storyManager = storyManager;
            this.userManager = userManager;
        }

        // GET: Stories
        public async Task<IActionResult> Index()
        {
            return View(await storyManager.ReadAllAsync(true, true));
        }
        // GET: Stories
        public async Task<IActionResult> IndexAdmin()
        {
            return View(await storyManager.ReadAllAsync(true, true));
        }
        // GET: Stories
        public async Task<IActionResult> IndexAdminPending()
        {
            return View(await storyManager.ReadAllAsync(true, true));
        }

        // GET: Stories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Story story = await storyManager.ReadAsync((int)id, true, true);
            if (story == null)
            {
                return NotFound();
            }

            return View(story);
        }
        // GET: Stories/Read/5
        public async Task<IActionResult> Read(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Story story = await storyManager.ReadAsync((int)id, true, true);
            if (story == null)
            {
                return NotFound();
            }

            return View(story);
        }
        // GET: Stories/Details/5
        public async Task<IActionResult> Examine(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Story story = await storyManager.ReadAsync((int)id, true, true);
            if (story == null)
            {
                return NotFound();
            }

            return View(story);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var story = await storyManager.ReadAsync(id, true, true);
            if (story == null)
            {
                return NotFound();
            }

            story.Status = Status.Approved;
            await storyManager.UpdateAsync(story);

            return RedirectToAction("IndexAdminPending");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Discard(int id)
        {
            var story = await storyManager.ReadAsync(id, true, true);
            if (story == null)
            {
                return NotFound();
            }

            story.Status = Status.Pending; // or whatever you define
            await storyManager.UpdateAsync(story);

            return RedirectToAction("IndexAdminPending");
        }

        // GET: Stories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Title, string Content, List<string> ImageUrls, string MapsUrl)
        {
            // Assuming you have the current User loaded (you'll need their UserId and User)
            var user = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Story newStory = new Story
            {
                Title = Title,
                Content = Content,
                ImageUrls = ImageUrls,
                MapsUrl = MapsUrl, // or set it to a default value
                User = user,
                UserId = user.Id,
                Status = Status.Pending // or whatever default you want
            };

            await storyManager.CreateAsync(newStory);
            return RedirectToAction(nameof(Index));
        }

        // GET: Stories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var story = await storyManager.ReadAsync((int)id, true, true);
            if (story == null)
            {
                return NotFound();
            }
            return View(story);
        }

        // POST: Stories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Status,UserId")] Story story)
        {
            if (id != story.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await storyManager.UpdateAsync(story);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoryExists(story.Id))
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
            return View(story);
        }

        // GET: Stories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var story = await storyManager.ReadAsync((int)id, true, true);

            if (story == null)
            {
                return NotFound();
            }

            return View(story);
        }

        // POST: Stories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var story = await storyManager.ReadAsync((int)id, true, true);
            if (story != null)
            {
                await storyManager.DeleteAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StoryExists(int id)
        {
            return storyManager.ReadAsync(id, true, true) is not null;
        }
    }
}
