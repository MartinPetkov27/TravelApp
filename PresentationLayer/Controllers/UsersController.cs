using BusinessLayer;
using DataLayer;
using PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace PresentationLayer.Controllers
{
	public class UsersController : Controller
	{
		private readonly IdentityContext _context;

		public UsersController(IdentityContext context)
		{
			_context = context;
		}

		// GET: Users
		public async Task<IActionResult> Index()
		{
			return View(await _context.ReadAllUsersAsync());
		}

		// GET: Users/Details/5
		public async Task<IActionResult> Details(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var user = await _context.ReadUserAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			return View(user);
		}

		#region Create
		public IActionResult Create()
		{
			// Get a list of roles for the dropdown
			var roles = Enum.GetValues(typeof(RoleType))
				.Cast<RoleType>()
				.Select(r => new SelectListItem
				{
					Value = r.ToString(),
					Text = r.ToString()
				}).ToList();

			// Pass the roles to the view
			ViewData["Roles"] = roles;
			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("UserName,Password,Email,PhoneNumber")] User user, RoleType role)
		{
			if (ModelState.IsValid)
			{
				// Assign the selected role to the user
				//user.Role = Enum.Parse<Role>(role);

				// Add the user to the context (adjust according to how you are saving your user data)
				await _context.CreateUserAsync(user.UserName, user.PasswordHash, user.Email, user.FirstName, user.LastName, user.PhoneNumber, role);

				// Redirect to the Index or another page after creating the user
				return RedirectToAction(nameof(Index));
			}

			//// If validation fails, pass the roles again to the view
			//var roles = Enum.GetValues(typeof(Role))
			//    .Cast<Role>()
			//    .Select(r => new SelectListItem
			//    {
			//        Value = r.ToString(),
			//        Text = r.ToString()
			//    }).ToList();
			//ViewData["Roles"] = roles;

			return View(user);
		}
		#endregion

		// GET: Users/Edit/5
		public async Task<IActionResult> Edit(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var user = await _context.ReadUserAsync(id);
			var role = await _context.GetUserRoleAsync(id);

            var viewModel = new EditUserViewModel
            {
                User = user,
                Role = role // Store the single role
            };

            return View(viewModel);
        }

		// POST: Users/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,FirstName,LastName,PhoneNumber")] User user)
		{
			if (id != user.Id)
			{
				return NotFound();
			}

			User userFromDb = await _context.ReadUserAsync(id);
			user.Trips = userFromDb.Trips;
			user.BucketLists = userFromDb.BucketLists;
			user.Stories = userFromDb.Stories;

			if (ModelState.IsValid)
			{
				try
				{
					await _context.UpdateUserAsync(user.Id, user.UserName, user.Email, user.FirstName,user.LastName, user.PhoneNumber);
                } 
				catch (DbUpdateConcurrencyException)
				{
					if (!await UserExists(user.Id))
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

			var viewModel = new EditUserViewModel
			{
				User = await _context.ReadUserAsync(id),
				Role = await _context.GetUserRoleAsync(id) // Store the single role
			};

            return View(viewModel);
		}

		// GET: Users/Delete/5
		public async Task<IActionResult> Delete(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var user = await _context.ReadUserAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			return View(user);
		}

		// POST: Users/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			User user = await _context.ReadUserAsync(id);
			await _context.DeleteUserByNameAsync(user.UserName);
			return RedirectToAction(nameof(Index));
		}

		private async Task<bool> UserExists(string id)
		{
			return await _context.ReadUserAsync(id) is not null;
		}
	}
}
