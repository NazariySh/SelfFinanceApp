using AutoMapper;
using Domain.Models;
using Shared.Dtos;

namespace Application.Profiles
{
    public class TokenProfile : Profile
    {
        public TokenProfile()
        {
            CreateMap<TokenDto, Token>();
            CreateMap<Token, TokenDto>();

            CreateMap<RefreshTokenDto, RefreshToken>();
            CreateMap<RefreshToken, RefreshTokenDto>();
        }
    }
}
