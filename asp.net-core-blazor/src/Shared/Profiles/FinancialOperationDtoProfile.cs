using AutoMapper;
using Shared.Dtos;

namespace Shared.Profiles
{
    public class FinancialOperationDtoProfile : Profile
    {
        public FinancialOperationDtoProfile()
        {
            CreateMap<FinancialOperationDto, FinancialOperationForUpdateDto>();
        }
    }
}
