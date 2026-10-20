using TiendaCry.Backend.Implementacion;
using TiendaCry.Backend.Repositories;
using TiendaCry.Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TiendaCry.Backend.Options;
using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data;
using TiendaCry.Backend.Data.Seeder;
using TiendaCry.Backend.Filters;

namespace TiendaCry.Backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<ITokenService, TokenService>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryPsqlEF>();
            builder.Services.AddScoped<IRegisterRepository, RegisterRepositoryPsqlEF>();
            builder.Services.AddScoped<ISesionUsuarioRepository, SesionUsuarioRepositoryPsqlEF>();
            builder.Services.AddScoped<LoginService>();
            builder.Services.AddScoped<SesionService>();
            builder.Services.AddScoped<RegisterService>();
            builder.Services.AddScoped<UsuarioService>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<ITokenEmailRepository, TokenEmailRepositoryPsqlEF>();
            builder.Services.AddScoped<VerificacionEmailService>();

            builder.Services.AddOptions<EmailSettings>()
                .BindConfiguration("EmailSettings");

            builder.Services.AddOptions<JwtSettings>()
                .BindConfiguration("JwtSettings");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptions<JwtSettings>>((options, jwtSettingsOptions) =>
                {
                    var jwtSettings = jwtSettingsOptions.Value;

                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (!context.Request.Headers.ContainsKey("Authorization"))
                            {
                                context.Token = context.Request.Cookies["access_token"];
                            }

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = async context =>
                        {
                            var sub = context.Principal?
                                .FindFirst("sub")?.Value;

                            int idUsuario = Convert.ToInt32(sub);

                            var usuarioRepository = context.HttpContext
                                .RequestServices
                                .GetRequiredService<IUsuarioRepository>();

                            // Reocrdar que esto lo puse porque quizá el usuario se da de baja y tenemos que evitar que pueda seguir haciendo cosas.
                            // Este evento se va a validar desp de que el backend haga las validaciones estándar sobre el token.
                            if (!await usuarioRepository.EstaActivo(idUsuario))
                            {
                                context.Fail("La cuenta no está habilitada.");
                            }
                        }
                    };
                });

            // Add services to the container.
            builder.Services.AddDbContext<DataContext>(options => 
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Recordar que le permití que redireccione porque la API me llevaba a otro lado.
            builder.Services.AddHttpClient("Georef", client =>
            {
                client.BaseAddress =
                    new Uri("https://apis.datos.gob.ar/georef/api/v2.0/");
            }).ConfigurePrimaryHttpMessageHandler(() =>
            new HttpClientHandler
            {
                AllowAutoRedirect = true
            });
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Web", policy =>
                {
                    policy.WithOrigins("https://localhost:7264")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddScoped<SeederEstados>();
            builder.Services.AddScoped<SeederTiposTokenEmail>();
            builder.Services.AddScoped<SeederUbicacion>();

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<CookieCsrfFilter>();
            });

            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var seederEstados = scope.ServiceProvider
                    .GetRequiredService<SeederEstados>();

                await seederEstados.SeedAsync();

                var seederTiposTokenEmail = scope.ServiceProvider
                    .GetRequiredService<SeederTiposTokenEmail>();

                await seederTiposTokenEmail.SeedAsync();

                var seederUbicacion = scope.ServiceProvider
                    .GetRequiredService<SeederUbicacion>();

                await seederUbicacion.SeedAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors("Web");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
