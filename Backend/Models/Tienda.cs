namespace Backend.Models
{
    public class Tienda
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Calle { get; set; }
        public required string NumCalle { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaRevision { get; set; }
        public string? MotivoRechazo { get; set; }
        public int IdCiudad { get; set; }
        public int IdEstadoTienda { get; set; }
        public int IdCategoria { get; set; }

        public Ciudad Ciudad { get; set; } = null!;
        public EstadoTienda EstadoTienda { get; set; } = null!;
        public Categoria Categoria { get; set; } = null!;

        public ICollection<UsuarioTienda> UsuariosTienda { get; set; } = new List<UsuarioTienda>();
        public ICollection<Promocion> Promociones { get; set; } = new List<Promocion>();
    }
}
