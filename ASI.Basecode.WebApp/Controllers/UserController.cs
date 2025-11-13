using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUsersByRole/{role}")]
        [AllowAnonymous]
        public IActionResult GetUsersByRole(string role)
        {
            try
            {
                var users = _userService.GetUsersByRole(role);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get users by role: {ex.Message}" });
            }
        }

        [HttpGet("GetAllUsers")]
        [AllowAnonymous]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get all users: {ex.Message}" });
            }
        }
    }
}