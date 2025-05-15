using Application.Contracts.Commons;
using AutoMapper;

namespace Infraestructure.Mappers.Implements;

public class GMapper : IGMapper
{
    private readonly IMapper _mapper;

    public GMapper(IMapper mapper)
    {
        _mapper = mapper;
    }

    public T Map<T>(object source)
    {
        return _mapper.Map<T>(source);
    }

    public List<T> Map<T>(List<object> source)
    {
        return _mapper.Map<List<T>>(source);
    }

    public T Map<T>(object source, T entidad)
    {
        return _mapper.Map(source, entidad);
    }

    public IQueryable<T> ProjectTo<T>(IQueryable<object> source)
    {
        return _mapper.ProjectTo<T>(source);
    }

}