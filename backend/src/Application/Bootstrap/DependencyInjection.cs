using Application.UseCases.Subasta.Command.Creacion;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Bootstrap
{
    public static class DependencyInjection
    {
        public static void AddDependencyInjectionApplication(this IServiceCollection services)
        {
            services.AddScoped<IValidator<CreateAuctionCommand>, CreateAuctionValidation>();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        }
    }
}
