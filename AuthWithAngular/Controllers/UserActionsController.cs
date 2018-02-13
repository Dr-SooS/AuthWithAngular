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
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> CurrentUser()
		{
			var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
			var roles = await _userManager.GetRolesAsync(user);
			return Ok(new UserFrontDto { Id = user.Id, Email = user.Email, Blocked = user.Blocked, Role = roles[0] });
		}

		[ActionName("role")]
		[HttpPost("{id}")]
		public async Task<IActionResult> UpadateRole([FromRoute] string id, [FromBody] UserFrontDto userDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != userDto.Id)
			{
				return BadRequest();
			}

			var user = await _context.AuthAppUsers.SingleOrDefaultAsync(m => m.Id == id);

			var roles = await _userManager.GetRolesAsync(user);
			await _userManager.RemoveFromRolesAsync(user, roles.ToArray());
			await _userManager.AddToRoleAsync(user, userDto.Role);

			await _context.SaveChangesAsync();

			roles = await _userManager.GetRolesAsync(user);
			return Ok(new UserFrontDto { Id = user.Id, Email = user.Email, Blocked = user.Blocked, Role = roles[0] });

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