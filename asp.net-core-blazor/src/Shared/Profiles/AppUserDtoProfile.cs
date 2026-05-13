using AutoMapper;
using Shared.Dtos;

namespace Shared.Profiles
{
    public class AppUserDtoProfile : Profile
    {
        public AppUserDtoProfile()
        {
            CreateMap<AppUserDto, AppUserForUpdateDto>();
        }
    }
}
