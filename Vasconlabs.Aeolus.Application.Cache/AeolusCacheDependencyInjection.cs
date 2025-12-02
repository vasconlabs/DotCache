using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vasconlabs.Aeolus.Application.Cache.Operations;
using Vasconlabs.Aeolus.Application.Cache.Sessions;
using Vasconlabs.Aeolus.Application.Cache.Store;
using Vasconlabs.Aeolus.Domain.Contracts.Interfaces;

namespace Vasconlabs.Aeolus.Application.Cache;

public static class AeolusCacheDependencyInjection
{
    public static IServiceCollection AddAeolusCacheService(this IServiceCollection services)
    { 
        ArgumentNullException.ThrowIfNull(services);
        
        services.AddSingleton<CacheStore>();
        services.AddSingleton<SessionPool>();

        services.AddSingleton<ICacheOperations, CacheOperations>();
        
        return services;
    }

    public static IHost UseAeolusCacheService(this IHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        CacheStore cacheStore = host.Services.GetRequiredService<CacheStore>();

        cacheStore.InitializeStore();

        return host;
    }
}