using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Distributed.Redis;
using OrchardCore.Distributed.Redis.Drivers;
using OrchardCore.Distributed.Redis.Options;
using OrchardCore.Distributed.Redis.Services;
using OrchardCore.Environment.Cache;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;

namespace OrchardCore.Distributed
{
    /// <summary>
    /// These services are registered on the tenant service collection
    /// </summary>
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DistributedSignal>();
            services.AddSingleton<ISignal>(sp => sp.GetRequiredService<DistributedSignal>());
            services.AddSingleton<IModularTenantEvents>(sp => sp.GetRequiredService<DistributedSignal>());
            services.AddSingleton<IModularTenantEvents, DistributedShell>();
        }
    }

    [Feature("OrchardCore.Distributed.Redis")]
    public class RedisStartup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IPermissionProvider, RedisPermissions>();
            services.AddScoped<INavigationProvider, RedisAdminMenu>();
            services.AddScoped<IDisplayDriver<ISite>, RedisSiteSettingsDisplayDriver>();

            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<RedisOptions>, RedisOptionsSetup>());

            services.AddSingleton<IRedis, Redis.Services.Redis>();
        }
    }

    [Feature("OrchardCore.Distributed.Redis.Cache")]
    public class RedisCacheStartup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<RedisCacheOptions>, RedisCacheOptionsSetup>());

            services.AddDistributedRedisCache(o => { });
        }
    }

    [Feature("OrchardCore.Distributed.Redis.MessageBus")]
    public class RedisSignalStartup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMessageBus, RedisMessageBus>();
        }
    }

    [Feature("OrchardCore.Distributed.Redis.Lock")]
    public class RedisLockStartup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILock, RedisLock>();
        }
    }
}