using Infraestructure.Persistence.Contexts.Generic;
using Infraestructure.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Contexts;

public class AppblazingurlsContext : BaseContext
{
    public AppblazingurlsContext(DbContextOptions<AppblazingurlsContext> options) : base(options)
    {
        AppContext.SetSwitch("EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        HelpersBuilders.ScanToRegisterDbSet(modelBuilder, nameof(AppblazingurlsContext));
    }
}