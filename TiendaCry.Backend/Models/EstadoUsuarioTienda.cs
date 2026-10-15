namespace TiendaCry.Backend.Models
{
    public class EstadoUsuarioTienda
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }

        public ICollection<UsuarioTienda>? UsuariosTienda { get; set; } = new List<UsuarioTienda>();
    }
}
