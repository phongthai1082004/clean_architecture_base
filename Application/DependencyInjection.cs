using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    internal class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(IServiceCollection services)
        {
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            });

            return services;
        }
    }
}
