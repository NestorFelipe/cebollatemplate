using Application.Dto.Commons;
using Application.Dto.Enums;
using Domain.Entities.BaseModel;
using Infraestructure.Persistence.Contexts.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infraestructure.Persistence.FactoryRepo;

public abstract class BaseCrudAbstract<Tclass, TDbContext> : BaseService<Tclass, TDbContext> where Tclass : class where TDbContext : BaseContext
{
    public async Task<ResponseAction> SaveEntity<TEntity, TDto>(TDto classdto, bool validate = false) where TEntity : AuditableBaseEntity where TDto : class
    {
        var vResult = new ResponseAction();
        TEntity vData;
        try
        {

            if (validate)
            {
                var vValidacion = await _Valid!.RunAsync(classdto);
                if (!_Valid.IsValid)
                    return new ResultAction().Entidad(null).MessageError(vValidacion).Result();
            }


            vData = _Mapper!.Map<TEntity>(classdto!);

            await using (_repo)
            {
                var vSaveAction = await _repo!.AddAsync(vData);
                vResult = new ResultAction().Entidad(vSaveAction).MessageSucces("El registro se guardó correctamente.").Result();

                if (vResult.Estado == State.Success)
                {
                    vResult.Objeto = _Mapper!.Map<TDto>(vSaveAction);
                    vResult.Id = vSaveAction.Id;
                    await _repo!.CommitAsync();
                }
            }
        }
        catch (Exception e)
        {
            vResult = new ResultAction().Entidad(null)
                .MessageError(_Logger!.Register(e))
                .Result();
        }

        return vResult;
    }

    public async Task<ResponseAction> DeleteEntity<TEntity>(int id) where TEntity : AuditableBaseEntity
    {
        var vResult = new ResponseAction();
        try
        {
            await using (_repo)
            {
                var vQuery = await _repo!.Entity<TEntity>().FindAsync(id);

                vResult = new ResultAction().Entidad(vQuery)
                            .MessageError($"No se encontro el registro base con el Id {id}").Result();

                if (vResult.Estado != State.Success) return vResult;

                var vDeleteAction = await _repo!.RemoveAsync(vQuery!);

                vResult = new ResultAction()
                    .Entidad(vDeleteAction).MessageSucces("El registro se eliminó correctamente.").Result();

                if (vResult.Estado == State.Success)
                {
                    vResult.Objeto = null;
                    vResult.Id = id;
                    await _repo!.CommitAsync();
                }
            }
        }
        catch (Exception e)
        {
            vResult = new ResultAction().Entidad(null)
                                .MessageError(_Logger!.Register(e))
                                .Result();
        }

        return vResult;
    }

    public async Task<ResponseAction> UpdateEntity<TEntity, TDto>(TDto classdto, int id, bool validate = false) where TEntity : AuditableBaseEntity where TDto : class
    {
        var vResult = new ResponseAction();
        TEntity vData;
        try
        {

            if (validate)
            {
                var vValidacion = await _Valid!.RunAsync(classdto);
                if (!_Valid.IsValid)
                    return new ResultAction().Entidad(null).MessageError(vValidacion).Result();
            }

            await using (_repo)
            {
                var vQuery = await _repo!.Entity<TEntity>().FindAsync(id);
                vResult = new ResultAction().Entidad(vQuery).MessageError($"No se encontro el registro base con el Id {id}").Result();

                if (vResult.Estado != State.Success) return vResult;

                vData = _Mapper!.Map(classdto!, vQuery)!;

                var vSaveAction = await _repo!.UpdateAsync(vData!);

                vResult = new ResultAction().Entidad(vSaveAction).MessageSucces("El registro se actulizó correctamente.").Result();

                if (vResult.Estado == State.Success)
                {
                    vResult.Objeto = _Mapper!.Map<TDto>(vSaveAction);
                    vResult.Id = id;
                    await _repo!.CommitAsync();
                }
                else
                {
                    await _repo!.RollbackAsync();
                }
            }
        }
        catch (Exception e)
        {
            vResult = new ResultAction().Entidad(null)
                            .MessageError(_Logger!.Register(e))
                            .Result();
        }

        return vResult;
    }

    public async Task<ResponseAction> GetWhereEntity<TEntity, TDto>(Expression<Func<TEntity, bool>> filter = null!, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!)
      where TEntity : AuditableBaseEntity
      where TDto : class
    {

        var vResult = new ResponseAction();
        try
        {
            await using (_repo)
            {
                var vQuery = await _repo!.Get(filter, orderBy).ToListAsync();
                vResult = new ResultAction().ListaEntidad(vQuery).Result();

                if (vResult.Estado == State.Success)
                {
                    vResult.Objeto = _Mapper!.Map<List<TDto>>(vQuery);
                }
            }
        }
        catch (Exception e)
        {
            vResult = new ResultAction().Entidad(null)
                .MessageError(_Logger!.Register(e))
                .Result();
        }

        return vResult;
    }

    public async Task<ResponseAction> GetById<TEntity, TDto>(int id) where TEntity : AuditableBaseEntity where TDto : class
    {
        var vResult = new ResponseAction();
        try
        {
            await using (_repo)
            {
                var vQuery = await _repo!.Entity<TEntity>().FindAsync(id);
                vResult = new ResultAction().Entidad(vQuery).Result();

                if (vResult.Estado == State.Success)
                {
                    vResult.Objeto = _Mapper!.Map<TDto>(vQuery!);
                    vResult.Id = id;
                }
            }
        }
        catch (Exception e)
        {
            vResult = new ResultAction().Entidad(null)
                .MessageError(_Logger!.Register(e))
                .Result();
        }

        return vResult;
    }

}
