namespace Application.Contracts.Commons;

public interface IGMapper
{
    T Map<T>(object source);

    List<T> Map<T>(List<object> source);

    IQueryable<T> ProjectTo<T>(IQueryable<object> source);


    T Map<T>(object source, T entidad);
}
