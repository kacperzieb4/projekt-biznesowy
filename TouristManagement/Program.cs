using TouristManagement.Infrastructure.Persistence;
using TouristManagement.Domain.Modules.CatalogSearch;
using TouristManagement.Application.UseCases;

var builder = WebApplication.CreateBuilder(args);

// --- Rejestracja warstw systemu ---

// 1. Infrastruktura (Repozytorium jako Singleton, żeby dane w pamięci nie znikały)
builder.Services.AddSingleton<IAttractionRepository, InMemoryAttractionRepository>();

// 2. Moduły Domenowe (Serwisy logiczne)
builder.Services.AddSingleton<CatalogSearchService>();

// 3. Przypadki Użycia (Use Cases)
builder.Services.AddScoped<CreateAttractionUseCase>();
builder.Services.AddScoped<SearchCatalogUseCase>();

// --- Konfiguracja ASP.NET Core ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Dokumentacja API

var app = builder.Build();

// Włączamy Swaggera w trybie deweloperskim
    app.UseSwagger();
    app.UseSwaggerUI();

app.MapControllers();

app.Run();