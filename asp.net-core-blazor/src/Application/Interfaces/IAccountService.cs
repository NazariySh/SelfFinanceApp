using Shared.Dtos;
using Shared.Models;

namespace Application.Interfaces
{
    public interface IAccountService
    {
        Task CreateAsync(AppUserForCreationDto userDto, CancellationToken cancellationToken = default);
        Task CreateAsync(AppUserForCreationDto userDto, string roleName, CancellationToken cancellationToken = default);
        Task UpdateAsync(AppUserForUpdateDto userDto, CancellationToken cancellationToken = default);
        Task UpdateAsync(string id, AppUserForUpdateDto userDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<AppUserDto?> GetAsync(CancellationToken cancellationToken = default);
        Task<AppUserDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedList<AppUserDto>> GetAllAsync(PaginationParameters parameters, CancellationToken cancellationToken = default);
        Task AddRoleAsync(string id, string roleName, CancellationToken cancellationToken = default);
        Task RemoveRoleAsync(string id, string roleName, CancellationToken cancellationToken = default);
    }
}
