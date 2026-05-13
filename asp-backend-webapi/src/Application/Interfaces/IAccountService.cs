using Shared.Dtos;
using Shared.Models;

namespace Application.Interfaces
{
    public interface IAccountService
    {
        Task CreateAsync(AppUserForCreationDto userDto, string roleName = "User");
        Task UpdateAsync(string id, AppUserForUpdateDto userDto);
        Task DeleteAsync(string id);
        Task<AppUserDto?> GetByIdAsync(string id);
        Task<AppUserDto?> GetByNameAsync(string userName);
        Task<IEnumerable<AppUserDto>> GetAllAsync();
        Task<PagedList<AppUserDto>> GetAllAsync(PaginationParameters parameters);
        Task<TokenDto> LoginAsync(LoginDto userDto);
        Task LogoutAsync(string id);
        Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto);
        Task AddRoleAsync(string id, string roleName);
        Task RemoveRoleAsync(string id, string roleName);
    }
}
