namespace TiendaCry.Backend.Models
{
    public class Provincia
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string GeorefId { get; set; } = null!;

        public ICollection<Ciudad> Ciudades { get; set; } = [];
    }
}
