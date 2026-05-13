using Domain.Entities;
using Domain.Models;
using Shared.Models;

namespace Domain.Repository
{
    public interface IAccountRepository
    {
        Task AddAsync(AppUser user, string password, string roleName);
        Task UpdateAsync(AppUser user);
        Task RemoveAsync(AppUser user);
        Task<AppUser?> GetByIdAsync(string id);
        Task<AppUser?> GetByNameAsync(string userName);
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task<PagedList<AppUser>> GetAllAsync(PaginationParameters parameters);
        Task<Token> LoginAsync(AppUser user, string password);
        Task LogoutAsync(AppUser user);
        Task<Token> RefreshTokenAsync(Token token);
        Task AddRoleAsync(AppUser user, string roleName);
        Task RemoveRoleAsync(AppUser user, string roleName);
        Task<IEnumerable<string>> GetRolesAsync(AppUser user);
        Task<Dictionary<string, List<string>>> GetRolesForUsersAsync(IReadOnlyCollection<string> userIds);
    }
}
