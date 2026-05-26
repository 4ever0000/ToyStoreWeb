using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ToyStore.Frontend;
using ToyStore.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// API ünvanı - Sonunda mütləq "/" olmalıdır
builder.Services.AddScoped(sp => new HttpClient
{
    // Ünvanın sonuna mütləq "api/v1/" əlavə edirik
    BaseAddress = new Uri("http://localhost:5289/api/v1/")
});

// ✅ Servislər (Bura UserStateService əlavə olundu)
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<WishlistService>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<UserStateService>(); 

await builder.Build().RunAsync();