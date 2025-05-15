using Application.Contracts.Commons;
using Application.Dto.Commons;
using Infraestructure.Persistence.Contexts.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infraestructure.Persistence.FactoryRepo;

public abstract class BaseService<Tclass, TDbContext> where Tclass : class where TDbContext : BaseContext
{
    public IGValidators? _Valid { get; }
    public IGMapper? _Mapper { get; }
    public IAppLogger<Tclass>? _Logger { get; }   
    public ISecurity _security { get; }

    protected readonly JwtOptions _jwtOption;
    protected readonly SettingsApp _setting;
    protected readonly IRepository<TDbContext> _repo;
  
    protected BaseService()
    {
        _Valid = InfraestructurePersistenceExtension._provider!.GetRequiredService<IGValidators>();
        _security = InfraestructurePersistenceExtension._provider!.GetRequiredService<ISecurity>();
        _Mapper = InfraestructurePersistenceExtension._provider!.GetRequiredService<IGMapper>();
        _Logger = InfraestructurePersistenceExtension._provider!.GetRequiredService<IAppLogger<Tclass>>();
        _jwtOption = InfraestructurePersistenceExtension._provider!.GetRequiredService<IOptions<JwtOptions>>().Value;
        _setting = InfraestructurePersistenceExtension._provider!.GetRequiredService<IOptions<SettingsApp>>().Value;    
        _repo = InfraestructurePersistenceExtension._provider!.GetRequiredService<IRepository<TDbContext>>();
        _repo!._dbContext = _repo!.DbFactory().CreateDbContext();
    }

}