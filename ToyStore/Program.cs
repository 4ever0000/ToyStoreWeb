using Microsoft.EntityFrameworkCore;
using ToyStore.Data;
using ToyStore.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. API üçün Controller servislərini əlavə edirik
builder.Services.AddControllers();

// 2. Swagger/OpenAPI tənzimləmələri
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Database (DbContext) qeydiyyatı
// appsettings.json-dakı "DefaultConnection" (localhost,1434) istifadə olunur
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. Repository Injection (Sənin aldığın "Unable to resolve service" xətasını bu sətir düzəldir)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// 5. CORS Siyasəti (Frontend-in API-ya qoşulması üçün mütləqdir)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Bütün ünvanlara icazə ver
              .AllowAnyMethod()   // Bütün metodlara (GET, POST və s.) icazə ver
              .AllowAnyHeader();  // Bütün başlıqlara icazə ver
    });
});

var app = builder.Build();

// 6. Middleware Sıralaması (BU ARDICILLIQ ÇOX VACİBDİR!)

// Swagger hər zaman görünsün (yoxlamaq asan olsun deyə if şərtindən çıxardım)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToyStore API v1");
    c.RoutePrefix = "swagger"; // http://localhost:5289/swagger ünvanında açılır
});

// Marşrutlaşdırma
app.UseRouting();

// CORS (Mütləq UseRouting-dən sonra, UseAuthorization-dan əvvəl gəlməlidir)
app.UseCors("AllowAll");

app.UseAuthorization();

// Controller-lərin aktiv edilməsi
app.MapControllers();

// API-ın işləkliyini yoxlamaq üçün ana səhifə linki
app.MapGet("/", () => "ToyStore API artıq tam hazırdır və işləyir!");

app.Run();