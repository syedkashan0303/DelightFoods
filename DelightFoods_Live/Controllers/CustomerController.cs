using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DelightFoods_Live.Data;
using DelightFoods_Live.Models;
using Microsoft.AspNetCore.Authorization;
using DelightFoods_Live.Utilites;
using Microsoft.AspNetCore.Identity;
using DelightFoods_Live.Models.DTO;

namespace DelightFoods_Live.Controllers
{
	[Authorize(Roles = "Admin")]

	public class CustomerController : Controller
	{
		private readonly ApplicationDbContext _context;

		public CustomerController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{

			var getUsers = await _context.Customers.ToListAsync();

			var usersDTOList = new List<CustomerDTO>();

			if (getUsers != null)
			{
				var getCity = Enum.GetValues(typeof(CityClass)).Cast<CityClass>().ToList();


				var utilities = new MapperClass<CustomerModel, CustomerDTO>();
				foreach (var item in getUsers)
				{
					var usersDTO = utilities.Map(item);

					usersDTO.CityId = item.CityId;
					usersDTO.Mobile = item.Mobile;
					usersDTO.CityName = usersDTO.CityId > 0 ? getCity.FirstOrDefault(x => ((int)x) == usersDTO.CityId).ToString() : "";
					usersDTO.Email = item.Email;
					usersDTOList.Add(usersDTO);
				}
				return View(usersDTOList);
			}

			return View(new List<CustomerDTO>());

			//return View(await _context.Customers.ToListAsync());
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var customerModel = await _context.Customers
				.FirstOrDefaultAsync(m => m.Id == id);
			if (customerModel == null)
			{
				return NotFound();
			}

			return View(customerModel);
		}

		public async Task<IActionResult> DetailByGUID(string? id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}

			var user = _context.Users.FirstOrDefault(x => x.Id == id);
			if (user != null)
			{

				var customerModel = _context.Customers.FirstOrDefault(m => m.UserId == user.Id);
				if (customerModel == null)
				{
					return NotFound();
				}
				return View(customerModel);
			}

			return NotFound();
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Mobile,Email")] CustomerModel customerModel)
		{
			if (ModelState.IsValid)
			{
				_context.Add(customerModel);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(customerModel);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var customerModel = await _context.Customers.FindAsync(id);
			if (customerModel == null)
			{
				return NotFound();
			}
			return View(customerModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Mobile,Email")] CustomerModel customerModel)
		{
			if (id != customerModel.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(customerModel);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CustomerModelExists(customerModel.Id))
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
			return View(customerModel);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var customerModel = await _context.Customers
				.FirstOrDefaultAsync(m => m.Id == id);
			if (customerModel == null)
			{
				return NotFound();
			}

			return View(customerModel);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			var user = _context.Users.Find(id);
			if (user != null)
			{
				var customerModel = _context.Customers.FirstOrDefault(x => x.UserId == user.Id);
				if (customerModel != null)
				{
					_context.Customers.Remove(customerModel);
					_context.SaveChanges();

				}
				_context.Users.Remove(user);
			}

			_context.SaveChanges();
			return RedirectToAction("CustomerList", "ApplicationUserManagment");
		}

		private bool CustomerModelExists(int id)
		{
			return _context.Customers.Any(e => e.Id == id);
		}
	}
}
