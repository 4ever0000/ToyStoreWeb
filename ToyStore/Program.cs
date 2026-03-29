using Microsoft.EntityFrameworkCore;
using ToyStore.Data;
using ToyStore.Repositories;

var builder = WebApplication.CreateBuilder(args);
// 1. CORS siyasətini qeydiyyatdan keçiririk
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Hər yerdən gələn sorğuya icazə ver
              .AllowAnyMethod()   // GET, POST, DELETE və s. hamısına icazə ver
              .AllowAnyHeader();  // Bütün başlıqlara icazə ver
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Bu sətir bütün modellər üçün repository-ni aktiv edir
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

// CORS'i hemen kullan (middleware sırası önemli!)
app.UseCors("AllowAll");

// Static dosyaları serve et (Frontend build outputs)
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToyStore API v1");
        c.RoutePrefix = "api/swagger"; // Swagger URL: /api/swagger
    });
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// API routes
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" }));

// Frontend Blazor WebAssembly fallback route
app.MapFallbackToFile("index.html");

app.Run();
