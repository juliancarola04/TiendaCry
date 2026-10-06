namespace Backend.Models
{
    public class Socio
    {
        public int Id { get; set; }
        public required string NumSocio { get; set; }
        public int IdUsuario { get; set; }
        public int IdEstadoSocio { get; set; }
        public int? IdCiudad { get; set; }
        public Ciudad? Ciudad { get; set; }

        public Usuario Usuario { get; set; } = null!;
        public EstadoSocio EstadoSocio { get; set; } = null!;
        public ICollection<Membresia> Membresias { get; set; } = new List<Membresia>();
    }
}
