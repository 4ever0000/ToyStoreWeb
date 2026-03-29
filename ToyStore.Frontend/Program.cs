using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ToyStore.Frontend;
using ToyStore.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Əsas komponentləri əlavə edirik
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Backend API-nin ünvanı. 
// DİQQƏT: Backend-i işə salanda Swagger-də görünən ünvanı (məs: localhost:5289) bura yazmalısınız.
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5289/api/v1/")
});

// Servisləri qeydiyyatdan keçiririk
builder.Services.AddSingleton<CartService>();
builder.Services.AddSingleton<WishlistService>();
builder.Services.AddSingleton<ThemeService>();

await builder.Build().RunAsync();