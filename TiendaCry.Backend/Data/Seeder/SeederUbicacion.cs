using TiendaCry.Backend.Data.Seeder.Response;
using TiendaCry.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace TiendaCry.Backend.Data.Seeder
{
    public class SeederUbicacion
    {
        private readonly DataContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public SeederUbicacion(DataContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }
        public async Task SeedAsync()
        {
            await SeedProvinciasAsync();
            await SeedLocalidadesAsync();
        }

        private async Task SeedProvinciasAsync()
        {
            if (await _context.Provincias.AnyAsync())
                return;

            var httpClient = _httpClientFactory.CreateClient("Georef");

            var response =
                await httpClient.GetFromJsonAsync<ProvinciaResponse>(
                    "provincias");

            if (response is null)
                return;

            var provincias = response.Provincias
                .Select(p => new Provincia
                {
                    GeorefId = p.Id,
                    Nombre = p.Nombre
                });

            _context.Provincias.AddRange(provincias);

            await _context.SaveChangesAsync();
        }

        // Recordar que no usaba localidades.json porque me redireccionaba a un http y no andaba.
        private async Task SeedLocalidadesAsync()
        {
            if (await _context.Ciudades.AnyAsync())
                return;

            var httpClient =
                _httpClientFactory.CreateClient("Georef");

            var provincias = await _context.Provincias
                .ToDictionaryAsync(p => p.GeorefId);

            const int max = 5000;
            var inicio = 0;

            while (true)
            {
                var response =
                    await httpClient.GetFromJsonAsync<LocalidadResponse>(
                        $"localidades?max={max}&inicio={inicio}");

                if (response is null ||
                    response.Localidades.Count == 0)
                    break;

                var ciudades = response.Localidades
                    .Select(l => new Ciudad
                    {
                        GeorefId = l.Id,
                        Nombre = l.Nombre,
                        ProvinciaId = provincias[l.Provincia.Id].Id
                    })
                    .ToList();

                _context.Ciudades.AddRange(ciudades);

                await _context.SaveChangesAsync();

                if (response.Localidades.Count < max)
                    break;

                inicio += max;
            }
        }
    }
}
