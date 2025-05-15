using Application.Dto.Models.Appblazingurls;
using AutoMapper;
using Domain.Entities.Appblazingurls;

namespace Infraestructure.Mappers.Profiles.Appblazingurls;

public class CncVentasProfile : Profile
{
    public CncVentasProfile()
    {

        CreateMap<CncVentas, CncVentasDto>().ReverseMap()
             .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    }
}
