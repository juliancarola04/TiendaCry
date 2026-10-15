namespace TiendaCry.Backend.Models
{
    public class UsuarioTienda
    {
        public int Id { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
        public int IdUsuario { get; set; }
        public int IdTienda { get; set; }
        public int IdRolTienda { get; set; }
        public int IdEstadoUsuarioTienda { get; set; }

        public Usuario Usuario { get; set; } = null!;
        public Tienda Tienda { get; set; } = null!;
        public RolTienda RolTienda { get; set; } = null!;
        public EstadoUsuarioTienda EstadoUsuarioTienda { get; set; } = null!;
        public ICollection<UsoPromocion> UsosPromocion { get; set; } = new List<UsoPromocion>();
    }
}
