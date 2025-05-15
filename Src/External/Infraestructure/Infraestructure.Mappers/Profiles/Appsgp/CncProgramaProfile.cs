using Application.Dto.Models.Appsgp;
using AutoMapper;
using Domain.Entities.Appsgp;

namespace Infraestructure.Mappers.Profiles.Appsgp;

public class CncProgramaProfile: Profile
{
    public CncProgramaProfile()
    {
        CreateMap<CncPrograma, CncProgramaDto>().ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<CncPrograma, CncProgramaCarteraDto>().ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    }
}


