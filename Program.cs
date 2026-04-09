using Modeer360.BackgroundServices;
using Modeer360.Repositories;
using Modeer360.Services;
using Scalar.AspNetCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// In-Memory Store
builder.Services.AddSingleton<InMemoryStore>();

// Services
builder.Services.AddScoped<CountryService>();
builder.Services.AddHttpClient<IpService>();
builder.Services.AddScoped<IpService>();

// Background Service
builder.Services.AddHostedService<TemporalBlockCleanupService>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Modeer360 API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Modeer360 API v1");
    c.RoutePrefix = "swagger";
});

app.MapControllers();

app.Run();