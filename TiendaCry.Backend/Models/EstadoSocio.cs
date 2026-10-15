namespace TiendaCry.Backend.Models
{
    public class EstadoSocio
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }

        public ICollection<Socio>? Socios { get; set; } = new List<Socio>();
    }
}
