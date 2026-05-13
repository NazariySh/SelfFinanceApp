using AutoMapper;
using Shared.Dtos;

namespace Application.Profiles
{
    public class FinancialOperationDtoProfile : Profile
    {
        public FinancialOperationDtoProfile()
        {
            CreateMap<FinancialOperationDto, FinancialOperationForUpdateDto>();
        }
    }
}
