using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AttractionCatalog.Application;
using AttractionCatalog.Infrastructure;
using AttractionCatalog.API.Handlers;

var builder = WebApplication.CreateBuilder(args);

// 1. Core Layers DI
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// 2. Register the "Ultra-Modern" .NET 8 Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3. Enable the modern Exception Handler middleware
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
