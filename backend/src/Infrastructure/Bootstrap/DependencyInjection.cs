using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Aplication.Common.Interface;
using Infraestructure.Persistencia.Repository;

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
            services.AddScoped<ISubastaCommandRepository, RepositoryCommand>();
            services.AddScoped<IRepositoryQuery, RepositoryQuery<ApplicationDbContext>>();
        }
    }
}
