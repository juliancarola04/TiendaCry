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
#if DEBUG
			cliente.BaseAddress = new Uri("https://10.0.2.2:7283/");
#else
			cliente.BaseAddress = new Uri("https://tiendacry-backend.runasp.net/");
#endif
		})
		.ConfigurePrimaryHttpMessageHandler(() =>
		{
			HttpClientHandler handler = new HttpClientHandler
			{
				UseCookies = false
			};

#if DEBUG
			handler.ServerCertificateCustomValidationCallback = (solicitud, certificado, cadena, errores) =>
			{
				return errores == System.Net.Security.SslPolicyErrors.None
					|| (solicitud.RequestUri?.Host == "10.0.2.2"
						&& certificado?.GetNameInfo(System.Security.Cryptography.X509Certificates.X509NameType.DnsName, false) == "localhost");
			};
#endif

			return handler;
		});
		builder.Services.AddScoped<AutenticacionService>();
		builder.Services.AddScoped<EmpleadoTiendaService>();
		builder.Services.AddScoped<SocioService>();
		builder.Services.AddScoped<ProvinciaService>();
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
