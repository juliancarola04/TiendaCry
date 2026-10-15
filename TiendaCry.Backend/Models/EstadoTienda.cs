namespace TiendaCry.Backend.Models
{
    public class EstadoTienda
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }

        public ICollection<Tienda>? Tiendas { get; set; } = new List<Tienda>();
    }
}
