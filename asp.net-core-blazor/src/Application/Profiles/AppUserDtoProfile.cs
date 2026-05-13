using AutoMapper;
using Shared.Dtos;

namespace Application.Profiles
{
    public class AppUserDtoProfile : Profile
    {
        public AppUserDtoProfile()
        {
            CreateMap<AppUserDto, AppUserForUpdateDto>();
        }
    }
}
