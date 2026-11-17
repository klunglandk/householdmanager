using HouseholdManager.Services;
using Microsoft.AspNetCore.Mvc;
using HouseholdManager.Models;
using Microsoft.AspNetCore.Identity;
using HouseholdManager.Database.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly UserManager<User> _userManager;

        public AuthController(ITokenService tokenService, IRefreshTokenService refreshTokenService, UserManager<User> userManager)
        {
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [EnableRateLimiting("login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (await _userManager.FindByNameAsync(model.UserName) is not { } user) return Unauthorized();
            if (await _userManager.IsLockedOutAsync(user)) return StatusCode(StatusCodes.Status429TooManyRequests);

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                await _userManager.AccessFailedAsync(user);
                return Unauthorized();
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            if (await CreateAccessTokenAsync(user) is not { } token) return Unauthorized();

            var refreshToken = await _refreshTokenService.CreateAsync(user.Id);
            return Ok(new { Token = token, RefreshToken = refreshToken });
        }

        [AllowAnonymous]
        [EnableRateLimiting("login")]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenModel model)
        {
            if (await _refreshTokenService.RotateAsync(model.RefreshToken) is not { } rotated) return Unauthorized();
            if (await _userManager.FindByIdAsync(rotated.UserId) is not { } user) return Unauthorized();
            if (await _userManager.IsLockedOutAsync(user)) return Unauthorized();
            if (await CreateAccessTokenAsync(user) is not { } token) return Unauthorized();

            return Ok(new { Token = token, RefreshToken = rotated.RawToken });
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenModel model)
        {
            await _refreshTokenService.RevokeAsync(model.RefreshToken);
            return NoContent();
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (await _userManager.GetUserAsync(User) is not { } user) return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(error => error.Description));
            }

            await _refreshTokenService.RevokeAllAsync(user.Id);

            if (await CreateAccessTokenAsync(user) is not { } token) return Unauthorized();
            var refreshToken = await _refreshTokenService.CreateAsync(user.Id);
            return Ok(new { Token = token, RefreshToken = refreshToken });
        }

        private async Task<string?> CreateAccessTokenAsync(User user)
        {
            if ((await _userManager.GetRolesAsync(user)).FirstOrDefault() is not { } role) return null;
            return _tokenService.GenerateJwtToken(user.Id, user.UserName ?? string.Empty, role, user.HouseholdId);
        }
    }
}
