using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthWithAngular.Models;
using AuthApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthWithAngular.Controllers
{
    [Produces("application/json")]
	[Route("api/[controller]/[action]")]
	public class UserActionsController : Controller
    {
		private readonly ApplicationContext _context;

		private readonly UserManager<User> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public UserActionsController(
			ApplicationContext context,
			UserManager<User> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
		}

		[ActionName("blocked")]
		[HttpPost("{id}")]
		public async Task<IActionResult> ChangeBlockedState([FromRoute] string id, [FromQuery(Name = "new_state")] bool newState)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var user = await _context.AuthAppUsers.SingleOrDefaultAsync(m => m.Id == id);
			user.Blocked = newState;
			_context.SaveChanges();

			var roles = await _userManager.GetRolesAsync(user);

			return Ok(new UserFrontDto { Id = user.Id, Email = user.Email, Blocked = user.Blocked, Role = roles[0] });
		}
	}
}