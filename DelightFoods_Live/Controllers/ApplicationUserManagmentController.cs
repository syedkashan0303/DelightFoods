using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using Microsoft.EntityFrameworkCore;
using DelightFoods_Live.Models.DTO;
using DelightFoods_Live.Models;
using DelightFoods_Live.Data;

namespace DelightFoods_Live.Controllers
{
	[Authorize(Roles = "Admin")]
	public class ApplicationUserManagmentController : Controller
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ApplicationDbContext _dbContext;


		public ApplicationUserManagmentController(
			RoleManager<IdentityRole> roleManager,
			UserManager<IdentityUser> userManager,
			ApplicationDbContext dbContext)
		{
			_roleManager = roleManager;
			_userManager = userManager;
			_dbContext = dbContext;

		}


		#region Roles

		//List all the roles Created by users
		public IActionResult Index()
		{
			var roles = _roleManager.Roles;
			return View(roles);
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(IdentityRole model)
		{
			//Avoid duplicacy
			if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
			}
			return RedirectToAction("Index");
		}

		#endregion


		#region UserRoleList

		public async Task<IActionResult> UserRoleList()
		{
			var users = await _userManager.Users.ToListAsync();

			var roleMappingList = new List<UserRoleMapping>();

			foreach (var user in users)
			{
				var roleMapping = new UserRoleMapping();
				roleMapping.UserId = user.Id;
				roleMapping.UserName = user.UserName;
				var roles = await _userManager.GetRolesAsync(user);

				if (roles != null && roles.Any())
				{
					foreach (var role in roles)
					{
						roleMapping.RoleName = role;
					}
				}
				roleMappingList.Add(roleMapping);
			}

			return View(roleMappingList);
		}

		#endregion



		#region UserRoleList

		public async Task<IActionResult> CustomerList()
		{
			var users = await _dbContext.ApplicationUsers.ToListAsync();

			var customerLists = new List<ApplicationUser>();

			foreach (var user in users)
			{
				var customer = _dbContext.Customers.FirstOrDefault(c => c.UserId == user.Id);
				if (customer != null)
				{
					var roles = await _userManager.GetRolesAsync(user);
					if (roles != null && roles.Any())
					{
						foreach (var role in roles)
						{
							if (role != "Admin")
							{
								customerLists.Add(user);
							}
						}
					}
				}
			}

			return View(customerLists);
		}

		#endregion


	}
}
