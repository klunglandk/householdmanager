using HouseholdManager.Database.DataModels;
using HouseholdManager.Dtos;
using HouseholdManager.Infrastructure;
using HouseholdManager.Models;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<AuthTokensDto>> Login([FromBody] LoginModel model)
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
            return Ok(new AuthTokensDto { Token = token, RefreshToken = refreshToken });
        }

        [AllowAnonymous]
        [EnableRateLimiting("login")]
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<AuthTokensDto>> Refresh([FromBody] RefreshTokenModel model)
        {
            if (await _refreshTokenService.RotateAsync(model.RefreshToken) is not { } rotated) return Unauthorized();
            if (await _userManager.FindByIdAsync(rotated.UserId) is not { } user) return Unauthorized();
            if (await _userManager.IsLockedOutAsync(user)) return Unauthorized();
            if (await CreateAccessTokenAsync(user) is not { } token) return Unauthorized();

            return Ok(new AuthTokensDto { Token = token, RefreshToken = rotated.RawToken });
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenModel model)
        {
            await _refreshTokenService.RevokeAsync(model.RefreshToken);
            return NoContent();
        }

        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserDto>> Me()
        {
            if (await _userManager.GetUserAsync(User) is not { } user) return Unauthorized();

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;
            return Ok(UserDto.Map(user, role));
        }

        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthTokensDto>> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (await _userManager.GetUserAsync(User) is not { } user) return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return this.IdentityProblem(result);
            }

            await _refreshTokenService.RevokeAllAsync(user.Id);

            if (await CreateAccessTokenAsync(user) is not { } token) return Unauthorized();
            var refreshToken = await _refreshTokenService.CreateAsync(user.Id);
            return Ok(new AuthTokensDto { Token = token, RefreshToken = refreshToken });
        }

        private async Task<string?> CreateAccessTokenAsync(User user)
        {
            if ((await _userManager.GetRolesAsync(user)).FirstOrDefault() is not { } role) return null;
            return _tokenService.GenerateJwtToken(user.Id, user.UserName ?? string.Empty, role, user.HouseholdId);
        }
    }
}
