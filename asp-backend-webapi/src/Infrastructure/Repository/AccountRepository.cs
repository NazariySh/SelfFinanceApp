using Domain.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Domain.Models;
using Shared.Models;
using Application.Interfaces;

namespace Infrastructure.Repository
{
    public class AccountRepository(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IJwtTokenService jwtTokenService,
        IDbContextFactory<FinancialDbContext> dbContextFactory)
        : IAccountRepository
    {
        private readonly UserManager<AppUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly RoleManager<IdentityRole> _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        private readonly IDbContextFactory<FinancialDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

        public async Task AddAsync(AppUser user, string password, string roleName)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be empty", nameof(password));
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Role name cannot be empty", nameof(roleName));
            }

            if (string.IsNullOrEmpty(user.UserName))
            {
                throw new ArgumentException("Username cannot be empty", nameof(user));
            }

            var existingUser = await GetByNameAsync(user.UserName);

            if (existingUser != null)
            {
                throw new AccountAlreadyExistException("User already exists");
            }

            var result = await _userManager.CreateAsync(user, password);

            ThrowIfNotSucceeded(result, "Failed to create user");

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);

            ThrowIfNotSucceeded(roleResult, $"Failed to add role '{roleName}'");
        }

        public async Task UpdateAsync(AppUser user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var result = await _userManager.UpdateAsync(user);

            ThrowIfNotSucceeded(result, "Failed to update user");
        }

        public async Task RemoveAsync(AppUser user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var result = await _userManager.DeleteAsync(user);

            ThrowIfNotSucceeded(result, "Failed to delete user");
        }

        public Task<AppUser?> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Id cannot be empty", nameof(id));
            }

            return _userManager.FindByIdAsync(id);
        }

        public Task<AppUser?> GetByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Username cannot be empty", nameof(userName));
            }

            return _userManager.FindByNameAsync(userName);
        }

        public async Task<IEnumerable<AppUser>> GetAllAsync()
        {
            return await _userManager.Users
                        .AsNoTracking()
                        .ToListAsync();
        }

        public async Task<PagedList<AppUser>> GetAllAsync(PaginationParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var query = _userManager.Users
                .AsNoTracking()
                .OrderBy(x => x.UserName)
                .ThenBy(x => x.Id);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedList<AppUser>(items, parameters.PageNumber, parameters.PageSize, totalCount);
        }

        public async Task<Dictionary<string, List<string>>> GetRolesForUsersAsync(IReadOnlyCollection<string> userIds)
        {
            ArgumentNullException.ThrowIfNull(userIds);

            if (userIds.Count == 0)
            {
                return new Dictionary<string, List<string>>();
            }

            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var rows = await context.UserRoles
                .AsNoTracking()
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(context.Roles.AsNoTracking(),
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new { ur.UserId, RoleName = r.Name })
                .ToListAsync();

            return rows
                .GroupBy(x => x.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.RoleName ?? string.Empty).Where(name => !string.IsNullOrEmpty(name)).ToList());
        }

        public async Task<Token> LoginAsync(AppUser user, string password)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be empty", nameof(password));
            }

            bool checkForPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!checkForPassword)
            {
                throw new AccountNotFoundException("Invalid username/password");
            }

            return await CreateTokenAsync(user, populateExp: true);
        }

        public async Task LogoutAsync(AppUser user)
        {
            ArgumentNullException.ThrowIfNull(user);

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = default;

            var result = await _userManager.UpdateAsync(user);

            ThrowIfNotSucceeded(result, "Failed to update user");
        }

        public async Task<Token> RefreshTokenAsync(Token token)
        {
            ArgumentNullException.ThrowIfNull(token);

            if (string.IsNullOrEmpty(token.AccessToken) || string.IsNullOrEmpty(token.RefreshToken?.Token))
            {
                throw new SecurityTokenException("Invalid refresh token!");
            }

            var principal = _jwtTokenService.GetPrincipalFromAccessToken(token.AccessToken);

            var userName = principal?.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                throw new SecurityTokenException("Invalid refresh token!");
            }

            var user = await GetByNameAsync(userName) ?? throw new AccountNotFoundException("User not found");

            if (!string.Equals(user.RefreshToken, token.RefreshToken.Token, StringComparison.Ordinal)
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new SecurityTokenException("Invalid refresh token!");
            }

            return await CreateTokenAsync(user, populateExp: false);
        }

        public async Task AddRoleAsync(AppUser user, string roleName)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Role name cannot be empty", nameof(roleName));
            }

            var role = await _roleManager.FindByNameAsync(roleName);

            if (role != null)
            {
                if (await _userManager.IsInRoleAsync(user, roleName))
                {
                    throw new UserAlreadyHasRoleException($"User '{user.UserName}' already has the '{roleName}' role!");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, roleName);

                ThrowIfNotSucceeded(roleResult, $"Failed to add role '{roleName}'");
            }
            else
            {
                throw new RoleNotFoundException("Role not found!");
            }
        }

        public async Task RemoveRoleAsync(AppUser user, string roleName)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Role name cannot be empty", nameof(roleName));
            }

            var role = await _roleManager.FindByNameAsync(roleName);

            if (role != null)
            {
                if (!await _userManager.IsInRoleAsync(user, roleName))
                {
                    throw new RoleNotFoundException($"User '{user.UserName}' does not have the '{roleName}' role.");
                }

                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Count == 1)
                {
                    throw new InvalidOperationException($"User '{user.UserName}' cannot delete their only role '{roles[0]}'.");
                }

                var roleResult = await _userManager.RemoveFromRoleAsync(user, roleName);

                ThrowIfNotSucceeded(roleResult, $"Failed to remove role '{roleName}'");
            }
            else
            {
                throw new RoleNotFoundException("Role not found!");
            }
        }

        public async Task<IEnumerable<string>> GetRolesAsync(AppUser user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return await _userManager.GetRolesAsync(user);
        }

        private async Task<Token> CreateTokenAsync(AppUser user, bool populateExp)
        {
            var claims = await GetClaimsAsync(user);

            var token = _jwtTokenService.CreateToken(claims);

            if (token.RefreshToken != null)
            {
                user.RefreshToken = token.RefreshToken.Token;

                if (populateExp)
                {
                    user.RefreshTokenExpiryTime = token.RefreshToken.ExpiryTime;
                }

                var updateResult = await _userManager.UpdateAsync(user);

                ThrowIfNotSucceeded(updateResult, "Failed to persist refresh token");
            }

            return token;
        }

        private static void ThrowIfNotSucceeded(IdentityResult result, string fallbackMessage)
        {
            if (result.Succeeded)
            {
                return;
            }

            var details = result.Errors.Select(e => $"{e.Code}: {e.Description}");
            var message = string.Join("; ", details);

            throw new InvalidOperationException(string.IsNullOrEmpty(message) ? fallbackMessage : $"{fallbackMessage}. {message}");
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync(AppUser user)
        {
            if (user.UserName is null || user.Email is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email)
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
