using Application.Dto.Commons;
using Domain.Entities.BaseModel;
using System.Linq.Expressions;

namespace Application.Contracts.Commons;

public interface IBaseGenericCrud
{
    Task<ResponseAction> SaveEntity<TEntity, TDto>(TDto classdto, bool validate = false) where TEntity : AuditableBaseEntity where TDto :class;
    Task<ResponseAction> UpdateEntity<TEntity, TDto>(TDto classdto, int id, bool validate = false) where TEntity : AuditableBaseEntity where TDto : class;
    Task<ResponseAction> DeleteEntity<TEntity>(int id) where TEntity : AuditableBaseEntity;
    Task<ResponseAction> GetWhereEntity<TEntity, TDto>(Expression<Func<TEntity, bool>> filter = null!, 
                                                                    Func<IQueryable<TEntity>, 
                                                                        IOrderedQueryable<TEntity>> orderBy = null!) where TEntity : AuditableBaseEntity where TDto : class;

    Task<ResponseAction> GetById<TEntity, TDto>(int id) where TEntity : AuditableBaseEntity where TDto : class;

}
