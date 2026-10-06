namespace Backend.Models
{
    public class TokenEmail
    {
        public int Id { get; set; }
        public required string Codigo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string? EmailNuevo { get; set; }
        public bool Usado { get; set; }
        public int IdTipoTokenEmail { get; set; }
        public int IdUsuario { get; set; }

        public TipoTokenEmail TipoTokenEmail { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}
