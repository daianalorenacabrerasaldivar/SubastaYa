using Microsoft.Extensions.DependencyInjection;
using Application.Dto.Auctions;
using Application.Interfaces;
using Application.Services;
using Application.UseCases.Subasta.Command.Creacion;
using Application.Validators;
using FluentValidation;
using MediatR;

namespace Application.Bootstrap
{
    public static class DependencyInjection
    {
        public static void AddDependencyInjectionApplication(this IServiceCollection services)
        {
            services.AddScoped<ISubastaService, SubastaService>();
            services.AddScoped<IValidator<CreateAuctionCommand>, CreateAuctionRequestValidator>();
            services.AddScoped<IValidator<CreateAuctionCommand>, CreateAuctionValidation>();
            services.AddMediatR(configuration =>
                configuration.RegisterServicesFromAssembly(typeof(CreateAuctionHandler).Assembly));
        }
    }
}
