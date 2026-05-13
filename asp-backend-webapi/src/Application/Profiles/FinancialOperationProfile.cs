using AutoMapper;
using Shared.Dtos;
using Domain.Entities;

namespace Application.Profiles
{
    public class FinancialOperationProfile : Profile
    {
        public FinancialOperationProfile()
        {
            CreateMap<FinancialOperation, FinancialOperationDto>();

            CreateMap<FinancialOperationForCreationDto, FinancialOperation>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<FinancialOperationForUpdateDto, FinancialOperation>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
        }
    }
}
