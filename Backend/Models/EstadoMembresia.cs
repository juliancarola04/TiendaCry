namespace Backend.Models
{
    public class EstadoMembresia
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }

        public ICollection<Membresia>? Membresias { get; set; } = new List<Membresia>();
    }
}
