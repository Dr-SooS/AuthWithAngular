using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthApp.Models;
using AuthWithAngular.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthWithAngular.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly ApplicationContext _context;

		private readonly UserManager<User> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public UsersController(
			ApplicationContext context,
			UserManager<User> userManager,
			RoleManager<IdentityRole> roleManager)
        {
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
        }

		// GET: api/Users
		[HttpGet]
        public async Task<IEnumerable<UserFrontDto>> GetAuthAppUsers()
        {
			List<UserFrontDto> users = new List<UserFrontDto>();
			foreach (var user in _context.AuthAppUsers)
			{
				var roles = await _userManager.GetRolesAsync(user);
				users.Add(new UserFrontDto { Id = user.Id, Email = user.Email, Blocked = user.Blocked, Role = roles[0] });
			}
			return users;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.AuthAppUsers.SingleOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

			var roles = await _userManager.GetRolesAsync(user);

			return Ok(new UserFrontDto { Id = user.Id, Email = user.Email, Blocked = user.Blocked, Role = roles[0] });
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] string id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AuthAppUsers.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.AuthAppUsers.SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            _context.AuthAppUsers.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        private bool UserExists(string id)
        {
            return _context.AuthAppUsers.Any(e => e.Id == id);
        }
    }
}