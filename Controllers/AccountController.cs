using ASI.Basecode.Data.Data.Entities;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ASI.Basecode.WebApp.Models;
using System;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly SignInManager _signInManager;

        public AccountController(IUserService userService, SignInManager signInManager)
        {
            _userService = userService;
            _signInManager = signInManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Username and password are required.");

            var user = await _userService.AuthenticateUserAsync(model.UserId, model.Password);
            if (user == null)
                return Unauthorized("Invalid username or password.");

            await _signInManager.SignInAsync(user, isPersistent: true);

            return Ok(new
            {
                loginResult = 0, // success
                message = "Login successful",
                access_token = "", // no JWT for now
                expires_in = 3600, // 1 hour
                userData = new
                {
                    id = user.Id,
                    userId = user.Username, // matches frontend
                    role = user.Role
                }
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Username and password are required.");

            // Check if user already exists
            var existingUser = await _userService.GetUserByUsernameAsync(model.UserId);
            if (existingUser != null)
                return BadRequest("Username already exists.");

            var user = new User
            {
                Id = System.Guid.NewGuid().ToString(),
                Username = model.UserId,
                Password = model.Password, // TODO: hash password in production!
                Role = "user"
            };

            await _userService.CreateUserAsync(user);

            return Ok(new
            {
                id = user.Id,
                username = user.Username,
                role = user.Role
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logged out successfully");
        }
    }
}
