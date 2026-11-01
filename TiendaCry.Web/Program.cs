using Microsoft.AspNetCore.Components.Authorization;
using TiendaCry.Web.Auth;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TiendaCry.Web;
using TiendaCry.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("TiendaCry", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Apis:TiendaCry"]!);
});

builder.Services.AddScoped<AlmacenamientoTokensService>();
builder.Services.AddScoped<AutenticacionService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, EstadoAutenticacionProvider>();
builder.Services.AddScoped<TiendaService>();
builder.Services.AddScoped<EmpleadoService>();
builder.Services.AddScoped<ProvinciaService>();
builder.Services.AddScoped<CategoriaService>();

await builder.Build().RunAsync();
