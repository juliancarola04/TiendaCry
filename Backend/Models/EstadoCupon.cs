namespace Backend.Models
{
    public class EstadoCupon
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }

        public ICollection<Cupon>? Cupones { get; set; } = new List<Cupon>();
    }
}
