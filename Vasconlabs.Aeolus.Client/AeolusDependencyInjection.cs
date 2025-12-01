using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Vasconlabs.Aeolus.Client.Interfaces;
using Vasconlabs.Aeolus.Client.Services;
using Vasconlabs.Aeolus.Domain.Contracts.V1;

namespace Vasconlabs.Aeolus.Client;

public static class AeolusDependencyInjection
{
    public static IServiceCollection AddAeolusClient(this IServiceCollection services, Action<AeolusDependencyOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddSingleton<IAeolusCacheService, AeolusCacheService>();
        
        services.AddGrpcClient<AeolusCacheGrpcService.AeolusCacheGrpcServiceClient>((serviceProvider, options) =>
        {
            IOptions<AeolusDependencyOptions> aeolusOptions = serviceProvider.GetRequiredService<IOptions<AeolusDependencyOptions>>();
            
            options.Address = new Uri(aeolusOptions.Value.BaseUrl);
        }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true,
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(10),
            KeepAlivePingDelay = TimeSpan.FromSeconds(15),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(20),
            KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always
        });
        
        return services;
    }
}