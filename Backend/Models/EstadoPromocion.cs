namespace Backend.Models
{
    public class EstadoPromocion
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }

        public ICollection<Promocion>? Promociones { get; set; } = new List<Promocion>();
    }
}
