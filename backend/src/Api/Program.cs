using Application.Bootstrap;
using Infrastructure.Bootstrap;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDependencyInjectionApplication();
builder.Services.AddDependencyInjectionInfrastructure();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SubastaYa API v1");
        c.RoutePrefix = "swagger";
        c.DefaultModelsExpandDepth(2);
        c.DefaultModelExpandDepth(2);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
