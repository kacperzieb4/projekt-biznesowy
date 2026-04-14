using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation;
using AttractionCatalog.Application.Common.Behaviors;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;
using AttractionCatalog.Domain.Modules.CatalogSearch.Services;

namespace AttractionCatalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        // Domain services — rules would normally come from a database
        services.AddSingleton(new List<RuleDefinition>());
        services.AddScoped<RuleSpecificationCompiler>();
        services.AddScoped<CatalogSearchService>();

        return services;
    }
}
