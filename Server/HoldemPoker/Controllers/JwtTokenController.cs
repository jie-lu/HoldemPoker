using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HoldemPoker.Controllers
{
	[ApiController]
	[Route("jwt-token")]
	public class JwtTokenController : ControllerBase
	{
		private readonly IConfiguration _config;
		private readonly SigningCredentials _signingCreds;
		private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

		public JwtTokenController(IConfiguration config)
		{
			if (config == null) throw new ArgumentNullException(nameof(config));

			_config = config;
			_signingCreds = new SigningCredentials(
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
				SecurityAlgorithms.HmacSha256);
		}

		public class LoginCredentials
		{
			public string UserName { get; set; }
		}

		[AllowAnonymous]
		[HttpPost]
		public IActionResult Create([FromBody] LoginCredentials loginCreds)
		{
			if (loginCreds == null) return BadRequest();

			if (!ValidateLogin(loginCreds))
			{
				return Ok(new { error = "Login failed." });
			}

			var principal = GetPrincipal(loginCreds, JwtBearerDefaults.AuthenticationScheme);
			var token = BuildToken(principal);

			return Ok(new { Name = principal.Identity.Name, Token = token });
		}

		// On a real project, you would use a SignInManager to verify the identity
		// using:
		//  _signInManager.PasswordSignInAsync(user, password, lockoutOnFailure: false);
		// With JWT you would rather avoid that to prevent cookies being set and use: 
		//  _signInManager.UserManager.FindByEmailAsync(email);
		//  _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
		private bool ValidateLogin(LoginCredentials loginCreds)
		{
			return !string.IsNullOrEmpty(loginCreds.UserName);
		}

		// On a real project, you would use the SignInManager 
		// to locate the user by its email and build its ClaimsPrincipal:
		//  var user = await _signInManager.UserManager.FindByEmailAsync(email);
		//  var principal = await _signInManager.CreateUserPrincipalAsync(user)
		private ClaimsPrincipal GetPrincipal(LoginCredentials creds, string authScheme)
		{
			// Here we are just creating a Principal for any user, 
			// using its email and a hardcoded “User” role
			var claims = new List<Claim>
			{
				 new Claim(ClaimTypes.NameIdentifier, creds.UserName),
				 new Claim(ClaimTypes.Name, creds.UserName)
				 //new Claim(ClaimTypes.Email, creds.Email),
				 //new Claim(ClaimTypes.Role, "User"),
			};
			return new ClaimsPrincipal(new ClaimsIdentity(claims, authScheme));
		}

		private string BuildToken(ClaimsPrincipal principal)
		{
			var token = new JwtSecurityToken(_config["Jwt:Issuer"],
			  null,
			  principal.Claims,
			  expires: DateTime.Now.AddDays(3),
			  signingCredentials: _signingCreds);

			return _tokenHandler.WriteToken(token);
		}
	}
}
