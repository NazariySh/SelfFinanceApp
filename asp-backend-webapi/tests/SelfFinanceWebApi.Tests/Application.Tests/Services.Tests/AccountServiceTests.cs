using Application.Interfaces;
using Application.Profiles;
using Application.Services;
using Application.Validation;
using Shared.Dtos;
using Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SelfFinanceWebApi.Tests.Factory;
using SelfFinanceWebApi.Tests.Interfaces;
using Application.Validation.Common;

namespace SelfFinanceWebApi.Tests.Application.Tests.Services.Tests
{
    public class AccountServiceTests
    {
        private readonly IAccountServiceFactory _accountServiceFactory;

        public AccountServiceTests()
        {
            var serviceProvider = ConfigureServices();

            _accountServiceFactory = serviceProvider.GetRequiredService<IAccountServiceFactory>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowValidationException_WhenUserDtoIsInvalid()
        {
            var accountService = _accountServiceFactory.CreateAccountService("CreateAsyncAccountService1");

            var userDto = new AppUserForCreationDto
            {
                UserName = "test",
                Email = "test",
                Password = "test",
                ConfirmPassword = "test"
            };

            await Assert.ThrowsAsync<ValidationException>(() => accountService.CreateAsync(userDto));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowAccountAlreadyExistException_WhenUserAlreadyExists()
        {
            var accountService = _accountServiceFactory.CreateAccountService("CreateAsyncAccountService2");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            await Assert.ThrowsAsync<AccountAlreadyExistException>(() => accountService.CreateAsync(userDto));
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateUser_WhenUserDtoIsValid()
        {
            var accountService = _accountServiceFactory.CreateAccountService("CreateAsyncAccountService4");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var user = await accountService.GetByNameAsync(userDto.UserName);

            Assert.NotNull(user);
            Assert.Equal(userDto.UserName, user.UserName);
            Assert.Equal(userDto.Email, user.Email);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenUserDtoIsInvalid()
        {
            var accountService = _accountServiceFactory.CreateAccountService("UpdateAsyncAccountService1");

            var userDto = new AppUserForUpdateDto
            {
                Id = "1",
                UserName = "test",
                Email = "test"
            };

            await Assert.ThrowsAsync<ValidationException>(() => accountService.UpdateAsync("1", userDto));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowAccountNotFoundException_WhenUserDoesNotExist()
        {
            var accountService = _accountServiceFactory.CreateAccountService("UpdateAsyncAccountService2");

            var userDto = new AppUserForUpdateDto
            {
                Id = "1",
                UserName = "Anna",
                Email = "anna@gmail.com"
            };

            await Assert.ThrowsAsync<AccountNotFoundException>(() => accountService.UpdateAsync("1", userDto));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser_WhenUserDtoIsValid()
        {
            var accountService = _accountServiceFactory.CreateAccountService("UpdateAsyncAccountService3");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var user = await accountService.GetByNameAsync(userDto.UserName);

            var userForUpdate = new AppUserForUpdateDto
            {
                Id = user!.Id,
                UserName = "Anna456",
                Email = "anna5467@gmail.com"
            };

            await accountService.UpdateAsync(user!.Id, userForUpdate);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowAccountNotFoundException_WhenUserDoesNotExist()
        {
            var accountService = _accountServiceFactory.CreateAccountService("DeleteAsyncAccountService1");

            await Assert.ThrowsAsync<AccountNotFoundException>(() => accountService.DeleteAsync("1"));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteUser_WhenUserExists()
        {
            var accountService = _accountServiceFactory.CreateAccountService("DeleteAsyncAccountService2");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var user = await accountService.GetByNameAsync(userDto.UserName);

            await accountService.DeleteAsync(user!.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var accountService = _accountServiceFactory.CreateAccountService("GetByIdAsyncAccountService1");

            var user = await accountService.GetByIdAsync("1");

            Assert.Null(user);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            var accountService = _accountServiceFactory.CreateAccountService("GetByIdAsyncAccountService2");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var user = await accountService.GetByNameAsync(userDto.UserName);

            var userById = await accountService.GetByIdAsync(user!.Id);

            Assert.NotNull(userById);
            Assert.Equal(user!.UserName, userById!.UserName);
            Assert.Equal(user!.Email, userById!.Email);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var accountService = _accountServiceFactory.CreateAccountService("GetByNameAsyncAccountService1");

            var user = await accountService.GetByNameAsync("Anna");

            Assert.Null(user);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnUser_WhenUserExists()
        {
            var accountService = _accountServiceFactory.CreateAccountService("GetByNameAsyncAccountService2");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var userByName = await accountService.GetByNameAsync(userDto.UserName);

            Assert.NotNull(userByName);
            Assert.Equal(userDto.UserName, userByName!.UserName);
            Assert.Equal(userDto.Email, userByName!.Email);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            var accountService = _accountServiceFactory.CreateAccountService("GetAllAsyncAccountService1");

            var users = await accountService.GetAllAsync();

            Assert.Empty(users);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnUsers_WhenUsersExist()
        {
            var accountService = _accountServiceFactory.CreateAccountService("GetAllAsyncAccountService2");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var users = await accountService.GetAllAsync();

            Assert.NotEmpty(users);
            Assert.Single(users);
        }

        [Fact]
        public async Task SetAccountRoleAsync_ShouldThrowAccountNotFoundException_WhenUserDoesNotExist()
        {
            var accountService = _accountServiceFactory.CreateAccountService("SetAccountRoleAsyncAccountService1");

            await Assert.ThrowsAsync<AccountNotFoundException>(() => accountService.AddRoleAsync("1", "User"));
        }

        [Fact]
        public async Task SetAccountRole_ShouldThrowRoleNotFoundException_WhenRoleNameIsInvalid()
        {
            var accountService = _accountServiceFactory.CreateAccountService("SetAccountRoleAsyncAccountService2");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var user = await accountService.GetByNameAsync(userDto.UserName);

            await Assert.ThrowsAsync<RoleNotFoundException>(() => accountService.AddRoleAsync(user!.Id, "InvalidRole"));
        }

        [Fact]
        public async Task SetAccountRole_ShouldThrowUserAlreadyHasRoleException_WhenUserAlreadyHasRole()
        {
            var accountService = _accountServiceFactory.CreateAccountService("SetAccountRoleAsyncAccountService3");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var user = await accountService.GetByNameAsync(userDto.UserName);

            await accountService.AddRoleAsync(user!.Id, "Admin");

            await Assert.ThrowsAsync<UserAlreadyHasRoleException>(() => accountService.AddRoleAsync(user!.Id, "Admin"));
        }

        [Fact]
        public async Task SetAccountRoleAsync_ShouldSetRole_WhenUserExists()
        {
            var accountService = _accountServiceFactory.CreateAccountService("SetAccountRoleAsyncAccountService4");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var user = await accountService.GetByNameAsync(userDto.UserName);

            await accountService.AddRoleAsync(user!.Id, "Admin");
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowAccountNotFoundException_WhenUserDoesNotExist()
        {
            var accountService = _accountServiceFactory.CreateAccountService("LoginAsyncAccountService1");

            var loginDto = new LoginDto
            {
                UserName = "Dude",
                Password = "duDe@3456"
            };

            await Assert.ThrowsAsync<AccountNotFoundException>(() => accountService.LoginAsync(loginDto));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowAccountNotFoundException_WhenPasswordIsInvalid()
        {
            var accountService = _accountServiceFactory.CreateAccountService("LoginAsyncAccountService2");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var loginDto = new LoginDto
            {
                UserName = "Anna",
                Password = "invalidPassword"
            };

            await Assert.ThrowsAsync<AccountNotFoundException>(() => accountService.LoginAsync(loginDto));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenUserExists()
        {
            var accountService = _accountServiceFactory.CreateAccountService("LoginAsyncAccountService4");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var loginDto = new LoginDto
            {
                UserName = userDto.UserName,
                Password = userDto.Password
            };

            var token = await accountService.LoginAsync(loginDto);

            Assert.NotNull(token);
            Assert.NotNull(token.AccessToken);
            Assert.NotNull(token.RefreshToken);
        }

        [Fact]
        public async Task LogoutAsync_ShouldThrowAccountNotFoundException_WhenUserDoesNotExist()
        {
            var accountService = _accountServiceFactory.CreateAccountService("LogoutAsyncAccountService1");

            await Assert.ThrowsAsync<AccountNotFoundException>(() => accountService.LogoutAsync("1"));
        }

        [Fact]
        public async Task LogoutAsync_ShouldRemoveRefreshToken_WhenUserExists()
        {
            var accountService = _accountServiceFactory.CreateAccountService("LogoutAsyncAccountService2");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var user = await accountService.GetByNameAsync(userDto.UserName);

            await accountService.LogoutAsync(user!.Id);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowSecurityTokenMalformedException_WhenRefreshTokenIsInvalid()
        {
            var accountService = _accountServiceFactory.CreateAccountService("RefreshTokenAsyncAccountService1");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var user = await accountService.GetByNameAsync(userDto.UserName);

            var tokenModel = new TokenDto
            {
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c\r\n",
                RefreshToken = new RefreshTokenDto
                {
                    Token = "invalidRefreshToken"
                }
            };

            await Assert.ThrowsAsync<SecurityTokenMalformedException>(() => accountService.RefreshTokenAsync(tokenModel));
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnToken_WhenRefreshTokenIsValid()
        {
            var accountService = _accountServiceFactory.CreateAccountService("RefreshTokenAsyncAccountService2");

            var userDto = new AppUserForCreationDto
            {
                UserName = "Anna",
                Email = "anna@gmail.com",
                Password = "annaG!456",
                ConfirmPassword = "annaG!456"
            };

            await accountService.CreateAsync(userDto);

            var token = await accountService.LoginAsync(new LoginDto
            {
                UserName = userDto.UserName,
                Password = userDto.Password
            });

            var refreshedToken = await accountService.RefreshTokenAsync(token);

            Assert.NotNull(refreshedToken);
            Assert.NotNull(refreshedToken.AccessToken);
            Assert.NotNull(refreshedToken.RefreshToken);
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AppUserProfile>();
                cfg.AddProfile<TokenProfile>();
            });

            services.AddKeyedSingleton<AbstractValidator<string>, UserNameValidator>(nameof(UserNameValidator));
            services.AddKeyedSingleton<AbstractValidator<string>, PasswordValidator>(nameof(PasswordValidator));

            services.AddSingleton<AbstractValidator<AppUserForCreationDto>, AppUserForCreationDtoValidator>();
            services.AddSingleton<AbstractValidator<AppUserForUpdateDto>, AppUserForUpdateDtoValidator>();

            services.AddScoped<IValidatorService, ValidatorService>();

            services.AddScoped<IAccountRepositoryFactory, AccountRepositoryFactory>();

            services.AddScoped<IIdentityInMemoryFactory, IdentityInMemoryFactory>();

            services.AddScoped<IAccountServiceFactory, AccountServiceFactory>();

            return services.BuildServiceProvider();
        }
    }
}
