namespace TiendaCry.Backend.Data.Seeder.DTOs
{
    public class LocalidadDto
    {
        public string Id { get; set; } = null!;
        public string Nombre { get; set; } = null!;

        public ProvinciaDto Provincia { get; set; } = null!;
    }
}
