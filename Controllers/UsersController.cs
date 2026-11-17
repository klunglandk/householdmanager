using HouseholdManager.Database.DataModels;
using HouseholdManager.Models;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IRefreshTokenService _refreshTokenService;

        public UsersController(UserManager<User> userManager, IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserModel model)
        {
            if (model.Role is not ("admin" or "child"))
            {
                return BadRequest("Role must be either 'admin' or 'child'.");
            }

            if (await _userManager.GetUserAsync(User) is not { } admin)
            {
                return Unauthorized();
            }

            var newUser = new User { UserName = model.UserName, HouseholdId = admin.HouseholdId };

            var createResult = await _userManager.CreateAsync(newUser, model.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors.Select(error => error.Description));
            }

            var roleResult = await _userManager.AddToRoleAsync(newUser, model.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(newUser);
                return BadRequest(roleResult.Errors.Select(error => error.Description));
            }

            return Ok(new { newUser.Id, newUser.UserName, model.Role });
        }

        [HttpPost("{userId}/reset-password")]
        public async Task<IActionResult> ResetPassword(string userId, [FromBody] ResetPasswordModel model)
        {
            if (await _userManager.GetUserAsync(User) is not { } admin) return Unauthorized();
            if (await _userManager.FindByIdAsync(userId) is not { } user || user.HouseholdId != admin.HouseholdId) return NotFound();

            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                return BadRequest(removeResult.Errors.Select(error => error.Description));
            }

            var addResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addResult.Succeeded)
            {
                return BadRequest(addResult.Errors.Select(error => error.Description));
            }

            await _refreshTokenService.RevokeAllAsync(user.Id);
            return NoContent();
        }
    }
}