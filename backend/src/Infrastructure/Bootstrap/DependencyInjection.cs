using Application.Interfaces.Persistencia;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repository;
using Infrastructure.Persistence.Repository.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Bootstrap
{
    public static class DependencyInjection
    {
        public static void AddDependencyInjectionInfrastructure(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IRepositoryCommand, RepositoryCommand>();
            services.AddScoped<ISubastaRepositoryCommand, SubastaRepositoryCommand>();
            services.AddScoped<IRepositoryQuery, RepositoryQuery<ApplicationDbContext>>();
        }
    }
}
