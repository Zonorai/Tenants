using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Infrastructure.Persistence;

namespace Zonorai.Tenants.Net6WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var tenantContext = services.GetRequiredService<TenantDbContext>();
                if (tenantContext.Database.IsSqlite() || tenantContext.Database.IsNpgsql() || tenantContext.Database.IsSqlServer())
                {
                    await tenantContext.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                logger.LogError(ex, "An error occurred while migrating or seeding the database");
            }
        }

        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}