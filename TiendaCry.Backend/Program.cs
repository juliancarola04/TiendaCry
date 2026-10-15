
using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data;
using TiendaCry.Backend.Data.Seeder;

namespace TiendaCry.Backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            builder.Services.AddScoped<SeederUbicacion>();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider
                    .GetRequiredService<SeederUbicacion>();

                await seeder.SeedAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
