using HouseholdManager.Database.DataModels;
using HouseholdManager.Dtos;
using HouseholdManager.Infrastructure;
using HouseholdManager.Models;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IRefreshTokenService _refreshTokenService;

        public UsersController(UserManager<User> userManager, IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _refreshTokenService = refreshTokenService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            if (await _userManager.GetUserAsync(User) is not { } admin) return Unauthorized();

            var users = await _userManager.Users
                .Where(user => user.HouseholdId == admin.HouseholdId)
                .OrderBy(user => user.UserName)
                .ToListAsync();

            var result = new List<UserDto>();
            foreach (var user in users)
            {
                result.Add(UserDto.Map(user, await GetRoleAsync(user)));
            }

            return Ok(result);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> Get(string userId)
        {
            if (await FindHouseholdUserAsync(userId) is not { } user) return NotFound();
            return Ok(UserDto.Map(user, await GetRoleAsync(user)));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserModel model)
        {
            if (model.Role is not ("admin" or "child"))
            {
                ModelState.AddModelError(nameof(model.Role), "Role must be either 'admin' or 'child'.");
                return ValidationProblem(ModelState);
            }

            if (await _userManager.GetUserAsync(User) is not { } admin)
            {
                return Unauthorized();
            }

            var newUser = new User { UserName = model.UserName, HouseholdId = admin.HouseholdId };

            var createResult = await _userManager.CreateAsync(newUser, model.Password);
            if (!createResult.Succeeded)
            {
                return this.IdentityProblem(createResult);
            }

            var roleResult = await _userManager.AddToRoleAsync(newUser, model.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(newUser);
                return this.IdentityProblem(roleResult);
            }

            return CreatedAtAction(nameof(Get), new { userId = newUser.Id }, UserDto.Map(newUser, model.Role));
        }

        [HttpPost("{userId}/reset-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword(string userId, [FromBody] ResetPasswordModel model)
        {
            if (await FindHouseholdUserAsync(userId) is not { } user) return NotFound();

            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                return this.IdentityProblem(removeResult);
            }

            var addResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addResult.Succeeded)
            {
                return this.IdentityProblem(addResult);
            }

            await _refreshTokenService.RevokeAllAsync(user.Id);
            return NoContent();
        }

        [HttpPost("{userId}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(string userId)
        {
            if (await FindHouseholdUserAsync(userId) is not { } user) return NotFound();
            if (IsCurrentUser(user)) return this.BadRequestProblem("You cannot deactivate yourself.");

            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            if (!result.Succeeded)
            {
                return this.IdentityProblem(result);
            }

            await _refreshTokenService.RevokeAllAsync(user.Id);
            return NoContent();
        }

        [HttpPost("{userId}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activate(string userId)
        {
            if (await FindHouseholdUserAsync(userId) is not { } user) return NotFound();

            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            if (!result.Succeeded)
            {
                return this.IdentityProblem(result);
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            return NoContent();
        }

        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string userId)
        {
            if (await FindHouseholdUserAsync(userId, includeChores: true) is not { } user) return NotFound();
            if (IsCurrentUser(user)) return this.BadRequestProblem("You cannot delete yourself.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return this.IdentityProblem(result);
            }

            return NoContent();
        }

        private async Task<User?> FindHouseholdUserAsync(string userId, bool includeChores = false)
        {
            if (await _userManager.GetUserAsync(User) is not { } admin) return null;

            var query = _userManager.Users.Where(user => user.Id == userId && user.HouseholdId == admin.HouseholdId);
            if (includeChores) query = query.Include(user => user.Chores);
            return await query.FirstOrDefaultAsync();
        }

        private bool IsCurrentUser(User user)
        {
            return user.Id == _userManager.GetUserId(User);
        }

        private async Task<string> GetRoleAsync(User user)
        {
            return (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;
        }
    }
}
