namespace Backend.Models
{
    public class Cupon
    {
        public int Id { get; set; }
        public required string IdTiendaNube { get; set; }
        public int Monto { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime? FechaUso { get; set; }
        public int IdEstadoCupon { get; set; }
        public int IdMembresia { get; set; }

        public EstadoCupon EstadoCupon { get; set; } = null!;
        public Membresia Membresia { get; set; } = null!;
    }
}
