using ASI.Basecode.Data.Data.Entities;
using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> GetById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Create([FromBody] User user)
        {
            if (user == null) return BadRequest();

            user.Id = Guid.NewGuid().ToString();
            await _userService.CreateUserAsync(user);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(string id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.Id) return BadRequest();

            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null) return NotFound();

            await _userService.UpdateUserAsync(updatedUser);
            return NoContent();
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(string id)
        {
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null) return NotFound();

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
