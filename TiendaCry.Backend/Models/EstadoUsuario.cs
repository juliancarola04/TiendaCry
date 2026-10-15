namespace TiendaCry.Backend.Models
{
    public class EstadoUsuario
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
