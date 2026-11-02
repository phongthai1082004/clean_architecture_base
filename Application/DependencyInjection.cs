using Application.Interfaces;
using Application.Services;
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

            // Register Service
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }
    }
}
