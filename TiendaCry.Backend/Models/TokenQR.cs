namespace TiendaCry.Backend.Models
{
    public class TokenQR
    {
        public int Id { get; set; }
        public required string Token { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public int IdMembresia { get; set; }

        public Membresia Membresia { get; set; } = null!;
    }
}
