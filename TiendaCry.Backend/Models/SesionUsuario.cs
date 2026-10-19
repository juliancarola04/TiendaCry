namespace TiendaCry.Backend.Models
{
    public class SesionUsuario
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public required string RefreshToken { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaRevocacion { get; set; }

        public Usuario Usuario { get; set; } = null!;
    }
}
