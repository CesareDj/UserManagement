using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        // GET: api/Users
        [HttpGet]
        public Task<List<User>> GetUsersAsync()
        {
            return _userService.GetUsersAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserAsync(int id)
        {
            var user = await _userService.GetUserAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUserAsync(UserDto userDto)
        {
            var user = await _userService.CreateUserAsync(userDto);

            return CreatedAtAction(nameof(GetUserAsync), new { id = user.Id }, user);
        }

        // PUT: api/Users
        [HttpPut]
        public async Task<IActionResult> PutUserAsync(User user)
        {
            var updatedUser = await _userService.UpdateUserAsync(user);

            if (updatedUser == null)
            {
                return BadRequest();
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var user = await _userService.DeleteUserAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}