using Application.Contracts.Commons;
using Domain.Entities.BaseModel;
using Infraestructure.Persistence.Contexts.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;

namespace Infraestructure.Persistence.FactoryRepo;

public class BuildRepository<TDbContext> : IRepository<TDbContext> where TDbContext : BaseContext
{

    public TDbContext? _dbContext { get; set; }
    public IDbContextTransaction? _transaction { get; set; }
    public IDbContextFactory<TDbContext> _dbfactory { get; set; }

    private readonly ISecurity _accesor;

    public BuildRepository(IDbContextFactory<TDbContext> dbfactory, ISecurity accesor)
    {
        _dbfactory = dbfactory;
        _accesor = accesor;
    }

    public IDbContextFactory<TDbContext> DbFactory()
    {
        return _dbfactory!;
    }

    public IQueryable<T> Get<T>(Expression<Func<T, bool>> filter = null!,
                                  Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null!) where T : class
    {

        var query = _dbContext!.Set<T>().AsQueryable();


        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            return orderBy(query);
        }
        else
        {
            return query;
        }
    }

    public async Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : AuditableBaseEntity
    {
        if (_transaction == null)
            _transaction = await _dbContext!.Database.BeginTransactionAsync();

        entity.UsuarioCreacion = _accesor.GetCurrentUserName();

        entity.FechaCreacion = DateTime.Now;

        await _dbContext!.Set<TEntity>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return entity!;
    }

    public async Task<List<TEntity>> AddRangeAsync<TEntity>(List<TEntity> entity) where TEntity : AuditableBaseEntity
    {

        if (_transaction == null)
            _transaction = await _dbContext!.Database.BeginTransactionAsync();

        var vUsuario = _accesor.GetCurrentUserName();
        var vDateNow = DateTime.Now;

        entity.ForEach(x =>
        {
            x.UsuarioCreacion = vUsuario;
            x.FechaCreacion = vDateNow;
        });

        _dbContext!.AddRange(entity);
        await _dbContext.SaveChangesAsync();

        return entity!;
    }

    public async Task<List<TEntity>> RemoveRangeAsync<TEntity>(List<TEntity> entity) where TEntity : AuditableBaseEntity
    {

        if (_transaction == null)
            _transaction = await _dbContext!.Database.BeginTransactionAsync();

        _dbContext!.Set<TEntity>().RemoveRange(entity);
        await _dbContext.SaveChangesAsync();
        return entity!;
    }

    public async Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : AuditableBaseEntity
    {
        try
        {
            if (_transaction == null)
                _transaction = await _dbContext!.Database.BeginTransactionAsync();

            entity.FechaModificacion = DateTime.Now;
            entity.UsuarioModificacion = _accesor.GetCurrentUserName();

            _dbContext!.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Exception("Otro usuario esta realizando cambios en el mismo registro, actualize los datos de la vista, verifique los datos y vuelva a intentar, entidad(" + entity.GetType().Name + ")");
        }

        return entity!;
    }


    public async Task<List<TEntity>> UpdateAsyncRange<TEntity>(List<TEntity> entity) where TEntity : AuditableBaseEntity
    {
        try
        {
            if (_transaction == null)
                _transaction = await _dbContext!.Database.BeginTransactionAsync();

            var date = DateTime.Now;
            var user = _accesor.GetCurrentUserName();

            entity.ForEach(x =>
            {
                x.FechaModificacion = date;
                x.UsuarioModificacion = user;
            });

            _dbContext!.UpdateRange(entity);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Exception("Otro usuario esta realizando cambios en el mismo registro, actualize los datos de la vista, verifique los datos y vuelva a intentar, entidad(" + entity.GetType().Name + ")");
        }

        return entity!;
    }


    public async Task<TEntity> RemoveAsync<TEntity>(TEntity entity) where TEntity : AuditableBaseEntity
    {

        if (_transaction == null)
            _transaction = await _dbContext!.Database.BeginTransactionAsync();

        _dbContext!.Set<TEntity>().Remove(entity);
        await _dbContext.SaveChangesAsync();
        return entity!;
    }


    public async Task CommitAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("la transaccion no se inicio correctamente.");
        }

        try
        {
            await _transaction.CommitAsync();
        }
        catch (Exception)
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackAsync()
    {

        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await DisposeTransactionAsync();
        }
    }

    public virtual IQueryable<TEntity> ExecuteFuncion<TEntity>(params object[] param) where TEntity : FunctionBaseTable
    {
        return _dbContext!.GetFunction<TEntity>(param);
    }

    public DbSet<TEntity> Entity<TEntity>() where TEntity : AuditableBaseEntity
    {
        return _dbContext!.Set<TEntity>();
    }

    public async Task<int> InsertMerge<TEntity>(IEnumerable<TEntity> entities, string[] keyColumns) where TEntity : AuditableBaseEntity
    {
        if (_transaction == null)
            _transaction = await _dbContext!.Database.BeginTransactionAsync();

        var vUsuario = _accesor.GetCurrentUserName();

        return await _dbContext!.InsertMergeAsync(entities, keyColumns, vUsuario!);

    }

    public async Task<List<T>> InsertBulk<T>(List<T> entities) where T : AuditableBaseEntity
    {
        if (_transaction == null)
            _transaction = await _dbContext!.Database.BeginTransactionAsync();

        var vUsuario = _accesor.GetCurrentUserName();

        return await _dbContext!.InsertBulk(entities, vUsuario!);
    }





    public async ValueTask DisposeAsync()
    {
        // Si queda una transacción sin confirmar, hacer rollback
        if (_transaction != null)
        {
            await RollbackAsync();
        }

        // Liberar recursos del contexto
        if (_dbContext != null)
        {
            await _dbContext.DisposeAsync();
            _dbContext = null;
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

}
