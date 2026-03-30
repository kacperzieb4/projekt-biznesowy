using Application.Interfaces;
using Application.UseCases;
using Domain.Core.Attractions.Interfaces;
using Domain.Modules.CatalogSearch.Services;
using Domain.Modules.CatalogSearch.Specifications;
using Domain.Modules.CatalogSearch.ValueObjects;
using Infrastructure.Repositories;
using Infrastructure.Specifications;

var builder = WebApplication.CreateBuilder(args);

// ── Domain Services ────────────────────────────────────────────────────
builder.Services.AddSingleton<RuleSpecificationCompiler>();
builder.Services.AddSingleton<CatalogSearchService>();

// ── Infrastructure (Repositories) ──────────────────────────────────────
builder.Services.AddSingleton<InMemoryAttractionRepository>();
builder.Services.AddSingleton<IAttractionRepository>(sp => sp.GetRequiredService<InMemoryAttractionRepository>());

// ── Application Use Cases ──────────────────────────────────────────────
builder.Services.AddScoped<CreateAttractionUseCase>();
builder.Services.AddScoped<PublishAttractionUseCase>();
builder.Services.AddScoped<CreateAttractionGroupUseCase>();
builder.Services.AddScoped(sp =>
{
    var repo = sp.GetRequiredService<IAttractionRepository>();
    var searchService = sp.GetRequiredService<CatalogSearchService>();

    // Factory for creating LocationQuerySpecification conditionally
    Func<GeoArea, IQuerySpecification<IAttractionComponent>> locationSpecFactory =
        geoArea => new LocationQuerySpecification(geoArea);

    return new SearchCatalogUseCase(repo, searchService, locationSpecFactory);
});

// ── API / MVC ──────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Tourist Attraction Management API",
        Version = "v1",
        Description = "Phase 1: Attractions, Categories, Relations & Catalog Search"
    });
});

var app = builder.Build();

// ── Middleware Pipeline ────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tourist Attraction API v1");
        options.RoutePrefix = string.Empty; // Swagger UI at root
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
