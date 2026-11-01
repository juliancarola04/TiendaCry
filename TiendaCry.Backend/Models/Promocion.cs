namespace TiendaCry.Backend.Models
{
    public class Promocion
    {
        public int Id { get; set; }
        public required string Titulo { get; set; }
        public required string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaRevision { get; set; }
        public string? MotivoRechazo { get; set; }
        public int IdTienda { get; set; }
        public int IdEstadoPromocion { get; set; }

        public Tienda Tienda { get; set; } = null!;
        public EstadoPromocion EstadoPromocion { get; set; } = null!;
        public Imagen? Imagen { get; set; }

        public ICollection<UsoPromocion> UsosPromocion { get; set; } = new List<UsoPromocion>();
    }
}
