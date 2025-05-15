using Application.Dto.Models.Appsgp;
using AutoMapper;
using Domain.Entities.Appsgp;

namespace Infraestructure.Mappers.Profiles.Appsgp;

public class CarCarteraProfile : Profile
{
    public CarCarteraProfile()
    {
        CreateMap<CarCartera, CarCarteraDto>().ReverseMap()
         .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
