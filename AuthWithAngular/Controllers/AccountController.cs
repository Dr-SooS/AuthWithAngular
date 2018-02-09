using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AuthApp.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using System.Net.Http;
using Newtonsoft.Json;

namespace AuthApp.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]/[action]")]
	public class AccountController : Controller
    {
		private readonly SignInManager<User> _signInManager;
		private readonly UserManager<User> _userManager;
		private readonly IConfiguration _configuration;
		private readonly RoleManager<IdentityRole> _roleManager;
		private static readonly HttpClient client = new HttpClient();

		public AccountController(
			UserManager<User> userManager,
			SignInManager<User> signInManager,
			IConfiguration configuration,
			RoleManager<IdentityRole> roleManager
			)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_configuration = configuration;
			_roleManager = roleManager;
		}

		[Authorize(Roles = "admin, user")]
		[HttpGet]
		public async Task<object> Protected()
		{
			//await _userManager.AddToRoleAsync(await _userManager.GetUserAsync(User), "admin");
			return "Protected area";
		}

		[HttpPost]
		public async Task<object> Login([FromBody] LoginDto model)
		{
			var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

			if (result.Succeeded)
			{
				var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
				return await GenerateJwtToken(model.Email, appUser);
			}

			throw new ApplicationException("INVALID_LOGIN_ATTEMPT");
		}

		[HttpPost]
		public async Task<object> Vk([FromBody] VkTokenDto token)
		{
			
			//var user = await client.GetStringAsync($"https://api.vk.com/method/users.get?access_token={accessToken.Token}");

			var user = await _userManager.FindByEmailAsync(token.Email);
			if(user == null)
			{
				user = new User
				{
					Email = token.Email,
					UserName = token.Email
				};
				var result = await _userManager.CreateAsync(user);
				await _userManager.AddToRolesAsync(user, new string[] { "user", "admin" });
			}

			await _signInManager.SignInAsync(user, false);
			return await GenerateJwtToken(token.Email, user);

			throw new ApplicationException("UNKNOWN_ERROR");
		}

		[HttpPost]
		public async Task<object> Register([FromBody] RegisterDto model)
		{
			var user = new User
			{
				UserName = model.Email,
				Email = model.Email
			};
			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(user, false);
				return await GenerateJwtToken(model.Email, user);
			}

			throw new ApplicationException("UNKNOWN_ERROR");
		}

		private async Task<object> GenerateJwtToken(string email, User user)
		{
			var roles = await _userManager.GetRolesAsync(user);
			//var claims = new List<Claim>
			//{
			//	new Claim(JwtRegisteredClaimNames.Sub, email),
			//	new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			//	new Claim(ClaimTypes.NameIdentifier, user.Id),
			//	new Claim(ClaimTypes.Role, roles[1])
			//};

			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
				new Claim(ClaimsIdentity.DefaultRoleClaimType, roles[1])
			};

			var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"])), SecurityAlgorithms.HmacSha256);
			var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

			var token = new JwtSecurityToken(
				_configuration["JwtIssuer"],
				_configuration["JwtIssuer"],
				claims,
				expires: expires,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public class LoginDto
		{
			[Required]
			public string Email { get; set; }

			[Required]
			public string Password { get; set; }

		}

		public class VkTokenDto
		{
			public string Token { get; set; }
			public string Email { get; set; }
		}

		public class RegisterDto
		{
			[Required]
			public string Email { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 6)]
			public string Password { get; set; }
		}
	}
}