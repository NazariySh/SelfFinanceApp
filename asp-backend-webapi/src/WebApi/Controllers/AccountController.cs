using Application.Interfaces;
using Shared.Dtos;
using Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/account")]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AppUserForCreationDto user)
        {
            await _accountService.CreateAsync(user);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _accountService.LoginAsync(loginDto);

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("auth/refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenDto tokenDto)
        {
            var token = await _accountService.RefreshTokenAsync(tokenDto);

            return Ok(token);
        }

        [HttpDelete("auth/logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = GetAccountIdentifier();

            await _accountService.LogoutAsync(userId);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAccount([FromBody] AppUserForUpdateDto user)
        {
            var userId = GetAccountIdentifier();

            await _accountService.UpdateAsync(userId, user);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = GetAccountIdentifier();

            await _accountService.DeleteAsync(userId);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAccount()
        {
            var userId = GetAccountIdentifier();

            var account = await _accountService.GetByIdAsync(userId);

            return account is null ? NotFound() : Ok(account);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/register")]
        public async Task<IActionResult> RegisterWithRole(string roleName, [FromBody] AppUserForCreationDto user)
        {
            await _accountService.CreateAsync(user, roleName);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/users")]
        public async Task<IActionResult> GetAllAccounts([FromQuery] PaginationParameters parameters)
        {
            var accounts = await _accountService.GetAllAsync(parameters);

            return Ok(accounts);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/users/{id}")]
        public async Task<IActionResult> GetAccountById(string id)
        {
            var account = await _accountService.GetByIdAsync(id);

            return account is null ? NotFound() : Ok(account);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/users/{id}")]
        public async Task<IActionResult> UpdateAccount(string id, [FromBody] AppUserForUpdateDto user)
        {
            await _accountService.UpdateAsync(id, user);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/users/{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            await _accountService.DeleteAsync(id);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/users/{id}/roles")]
        public async Task<IActionResult> AddAccountRole(string id, string roleName)
        {
            await _accountService.AddRoleAsync(id, roleName);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/users/{id}/roles")]
        public async Task<IActionResult> RemoveAccountRole(string id, string roleName)
        {
            await _accountService.RemoveRoleAsync(id, roleName);

            return Ok();
        }

        private string GetAccountIdentifier()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        }
    }
}
