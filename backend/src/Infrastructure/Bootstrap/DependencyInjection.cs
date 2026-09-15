using Application.Interfaces.Persistencia;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.Auditoria;
using Infrastructure.Persistence.Repositories.Billeteras;
using Infrastructure.Persistence.Repositories.Categorias;
using Infrastructure.Persistence.Repositories.Common;
using Infrastructure.Persistence.Repositories.Pujas;
using Infrastructure.Persistence.Repositories.Subastas;
using Infrastructure.Persistence.Repositories.Usuarios;
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

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ISubastaCommandRepository, SubastaCommandRepository>();
            services.AddScoped<ISubastaQueryRepository, SubastaQueryRepository>();
            services.AddScoped<IUsuarioQueryRepository, UsuarioQueryRepository>();
            services.AddScoped<ICategoriaQueryRepository, CategoriaQueryRepository>();
            services.AddScoped<IBilleteraCommandRepository, BilleteraCommandRepository>();
            services.AddScoped<IBilleteraQueryRepository, BilleteraQueryRepository>();
            services.AddScoped<IAuditoriaLogCommandRepository, AuditoriaLogCommandRepository>();
            services.AddScoped<IPujaQueryRepository, PujaQueryRepository>();
        }
    }
}
