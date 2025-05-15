using Infraestructure.Persistence.Contexts.Generic;
using Infraestructure.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Contexts;

public class AppsgpContext : BaseContext
{
    public AppsgpContext(DbContextOptions<AppsgpContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        HelpersBuilders.ScanToRegisterDbSet(modelBuilder, nameof(AppsgpContext));
    }
}
