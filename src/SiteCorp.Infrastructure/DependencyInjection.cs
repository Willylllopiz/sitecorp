using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SiteCorp.Application.Authentication;
using SiteCorp.Application.HumanResources;
using SiteCorp.Infrastructure.Authentication;
using SiteCorp.Infrastructure.Data;

namespace SiteCorp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSiteCorpInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SiteCorp")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:SiteCorp en la configuracion.");

        services.AddDbContext<SiteCorpDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IAuthRepository, EfAuthRepository>();
        services.AddScoped<IPasswordHashService, IdentityPasswordHashService>();
        services.AddSingleton<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IHumanResourcesRepository, EfHumanResourcesRepository>();

        return services;
    }
}
