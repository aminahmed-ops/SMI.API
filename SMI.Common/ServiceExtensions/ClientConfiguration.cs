using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMI.Common.Configuration;

namespace SMI.Common.ServiceExtensions
{
    public static class ClientConfiguration
    {
        public static IServiceCollection AddSocialMediaClient(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<BaseClient>(o =>
            {
                config.GetSection("ClientConfiguration").Bind(o);
                config.GetSection("ClientConfiguration:Facebook").Bind(o);
            });
            return services;
        }
    }
}
