using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Shared.Dtos;
using Shared.Models;
using Domain.Repository;
using Domain.Models;

namespace Application.Services
{
    public class AccountService(
        IAccountRepository accountRepository,
        IValidatorService validatorService,
        IMapper mapper)
        : IAccountService
    {
        private readonly IAccountRepository _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        private readonly IValidatorService _validatorService = validatorService ?? throw new ArgumentNullException(nameof(validatorService));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task CreateAsync(AppUserForCreationDto userDto, string roleName = "User")
        {
            ArgumentNullException.ThrowIfNull(userDto);

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Role name cannot be empty", nameof(roleName));
            }

            await _validatorService.ValidateAndThrowAsync(userDto);

            var newUser = _mapper.Map<AppUser>(userDto);

            await _accountRepository.AddAsync(newUser, userDto.Password, roleName);
        }

        public async Task UpdateAsync(string id, AppUserForUpdateDto userDto)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(id));
            }

            ArgumentNullException.ThrowIfNull(userDto);

            if (id != userDto.Id)
            {
                throw new ArgumentException("User Id does not match", nameof(id));
            }

            await _validatorService.ValidateAndThrowAsync(userDto);

            var user = await _accountRepository.GetByIdAsync(id) ?? throw new AccountNotFoundException("User not found");

            if (user.UserName == userDto.UserName && user.Email == userDto.Email)
            {
                throw new AccountAlreadyExistException("No changes to update!");
            }

            var userForUpdate = _mapper.Map(userDto, user);

            await _accountRepository.UpdateAsync(userForUpdate);
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(id));
            }

            var user = await _accountRepository.GetByIdAsync(id) ?? throw new AccountNotFoundException("User not found");

            await _accountRepository.RemoveAsync(user);
        }

        public async Task<AppUserDto?> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(id));
            }

            var user = await _accountRepository.GetByIdAsync(id);

            AppUserDto? userDto = null;

            if (user != null)
            {
                userDto = _mapper.Map<AppUserDto>(user);

                var roles = await _accountRepository.GetRolesAsync(user);

                userDto.Roles = roles.ToList();
            }

            return userDto;
        }

        public async Task<AppUserDto?> GetByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("User name cannot be empty", nameof(userName));
            }

            var user = await _accountRepository.GetByNameAsync(userName);

            AppUserDto? userDto = null;

            if (user != null)
            {
                userDto = _mapper.Map<AppUserDto>(user);

                var roles = await _accountRepository.GetRolesAsync(user);

                userDto.Roles = roles.ToList();
            }

            return userDto;
        }

        public async Task<IEnumerable<AppUserDto>> GetAllAsync()
        {
            var users = await _accountRepository.GetAllAsync();

            var userDtos = _mapper.Map<IEnumerable<AppUserDto>>(users);

            foreach (var user in users)
            {
                var roles = await _accountRepository.GetRolesAsync(user);

                var userDto = userDtos.FirstOrDefault(x => x.Id == user.Id);

                if (userDto != null)
                {
                    userDto.Roles = roles.ToList();
                }
            }

            return userDtos;
        }

        public async Task<PagedList<AppUserDto>> GetAllAsync(PaginationParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var pagedUsers = await _accountRepository.GetAllAsync(parameters);

            var userDtos = _mapper.Map<List<AppUserDto>>(pagedUsers.Items);

            var userIds = pagedUsers.Items.Select(u => u.Id).ToList();
            var rolesByUser = await _accountRepository.GetRolesForUsersAsync(userIds);

            foreach (var userDto in userDtos)
            {
                userDto.Roles = rolesByUser.TryGetValue(userDto.Id, out var roles)
                    ? roles
                    : new List<string>();
            }

            return new PagedList<AppUserDto>(userDtos, pagedUsers.PageNumber, pagedUsers.PageSize, pagedUsers.TotalCount);
        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            ArgumentNullException.ThrowIfNull(loginDto);

            var user = await _accountRepository.GetByNameAsync(loginDto.UserName) ?? throw new AccountNotFoundException("User not found");

            var token = await _accountRepository.LoginAsync(user, loginDto.Password);

            return _mapper.Map<TokenDto>(token);
        }

        public async Task LogoutAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(id));
            }

            var user = await _accountRepository.GetByIdAsync(id) ?? throw new AccountNotFoundException("User not found");

            await _accountRepository.LogoutAsync(user);
        }

        public async Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto)
        {
            ArgumentNullException.ThrowIfNull(tokenDto);

            var token = _mapper.Map<Token>(tokenDto);

            var newToken = await _accountRepository.RefreshTokenAsync(token);

            return _mapper.Map<TokenDto>(newToken);
        }

        public async Task AddRoleAsync(string id, string roleName)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(id));
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Role name cannot be empty", nameof(roleName));
            }

            var user = await _accountRepository.GetByIdAsync(id) ?? throw new AccountNotFoundException("User not found");

            await _accountRepository.AddRoleAsync(user, roleName);
        }

        public async Task RemoveRoleAsync(string id, string roleName)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(id));
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Role name cannot be empty", nameof(roleName));
            }

            var user = await _accountRepository.GetByIdAsync(id) ?? throw new AccountNotFoundException("User not found");

            await _accountRepository.RemoveRoleAsync(user, roleName);
        }
    }
}
