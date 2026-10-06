namespace Backend.Models
{
    public class RolTienda
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }

        public ICollection<UsuarioTienda>? UsuariosTienda { get; set; } = new List<UsuarioTienda>();
    }
}
