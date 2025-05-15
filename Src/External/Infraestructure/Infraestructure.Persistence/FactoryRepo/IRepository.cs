using Domain.Entities.BaseModel;
using Infraestructure.Persistence.Contexts.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace Infraestructure.Persistence.FactoryRepo;

public interface IRepository<TDbContext> : IAsyncDisposable where TDbContext : BaseContext 
{

    public TDbContext? _dbContext { get; set; }
    public IDbContextTransaction? _transaction { get; set; }

    public IDbContextFactory<TDbContext> DbFactory();

    public IQueryable<T> Get<T>(Expression<Func<T, bool>> filter = null!,
                                  Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null!) where T : class  ;

    public Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : AuditableBaseEntity;

    public Task<List<TEntity>> AddRangeAsync<TEntity>(List<TEntity> entity) where TEntity : AuditableBaseEntity;

    public Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : AuditableBaseEntity;

    public Task<List<TEntity>> UpdateAsyncRange<TEntity>(List<TEntity> entity) where TEntity : AuditableBaseEntity;

    public Task<TEntity> RemoveAsync<TEntity>(TEntity entity) where TEntity : AuditableBaseEntity;

    Task<List<TEntity>> RemoveRangeAsync<TEntity>(List<TEntity> entity) where TEntity : AuditableBaseEntity;

    public Task CommitAsync();

    public Task RollbackAsync();

    public IQueryable<TEntity> ExecuteFuncion<TEntity>(params object[] param) where TEntity : FunctionBaseTable;

    public Task<int> InsertMerge<TEntity>(IEnumerable<TEntity> entities, string[] keyColumns ) where TEntity : AuditableBaseEntity;

    public Task<List<T>> InsertBulk<T>(List<T> entities) where T : AuditableBaseEntity;

    public DbSet<TEntity> Entity<TEntity>() where TEntity : AuditableBaseEntity;

}
