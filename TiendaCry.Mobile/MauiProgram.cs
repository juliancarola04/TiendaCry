using Microsoft.AspNetCore.Components.Authorization;
using TiendaCry.Mobile.Auth;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using TiendaCry.Mobile.Services;

namespace TiendaCry.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddMudServices();
		builder.Services.AddSingleton<AlmacenamientoTokensService>();
		builder.Services.AddHttpClient("TiendaCry", cliente =>
		{
			cliente.BaseAddress = new Uri("https://tiendacry-backend.runasp.net/");
		})
		.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
		{
			UseCookies = false
		});
		builder.Services.AddScoped<AutenticacionService>();
		builder.Services.AddAuthorizationCore();
		builder.Services.AddCascadingAuthenticationState();
		builder.Services.AddScoped<AuthenticationStateProvider, EstadoAutenticacionProvider>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
