namespace TiendaCry.Backend.Models
{
    public class Ciudad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string GeorefId { get; set; } = null!;
        public int ProvinciaId { get; set; }
        public Provincia Provincia { get; set; } = null!;

        public ICollection<Socio> Socios { get; set; } = new List<Socio>();
        public ICollection<Tienda> Tiendas { get; set; } = new List<Tienda>();
    }
}
