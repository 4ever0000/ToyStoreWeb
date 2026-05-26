using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ToyStore.Data;
using ToyStore.Middlewares;
using ToyStore.Repositories;
using DotNetEnv;
using Microsoft.OpenApi.Models;

// .env faylını mütləq builder-dən əvvəl yükləyirik
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// .env-dən dəyişənləri götürürük
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

// 1. API üçün Controller servislərini əlavə edirik
builder.Services.AddControllers();

// 2. Swagger Tənzimləmələri
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Tokeni bura yazın: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// 3. Database (DbContext) - Artıq .env-dən gəlir
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 4. Repository Injection
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// 5. CORS Siyasəti
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 6. JWT Authentication - Artıq .env-dən gəlir
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var app = builder.Build();

// 7. Middleware Sıralaması
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToyStore API v1");
    c.RoutePrefix = "swagger";
});

app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication(); // Kim olduğunu yoxla
app.UseAuthorization();  // İcazəni yoxla

app.MapControllers();
app.MapGet("/", () => "ToyStore API artıq .env ilə tam təhlükəsiz işləyir!");

app.Run();