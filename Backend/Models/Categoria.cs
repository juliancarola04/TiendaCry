namespace Backend.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }

        public ICollection<Tienda>? Tiendas { get; set; } = new List<Tienda>();
    }
}
