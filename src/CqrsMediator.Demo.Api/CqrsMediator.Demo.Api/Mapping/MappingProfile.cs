
using AutoMapper;

using CqrsMediator.Demo.Api.Dto;

namespace CqrsMediator.Demo.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Dal.Entities.Product, Dto.Product>();
            CreateMap<Dal.Entities.Order, Dto.Order>();
            CreateMap<Dal.Entities.OrderItem, Dto.OrderItem>();
        }
    }
}
