using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Backend.Models
{
    public class Usuario
    {
        public int Id { get; set;  }
        public required string NomUsuario { get; set; }
        public required string Contrasena { get; set; }
        public required string Email { get; set; }
        public bool EmailVerificado { get; set; } = false;
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public bool EsAdministradorTiendaCry { get; set; } = false;
        public bool DebeCambiarContrasena { get; set; } = false;
        public required int IdEstadoUsuario { get; set; }
        public EstadoUsuario EstadoUsuario { get; set; } = null!;

        public Socio? Socio { get; set; }
        public ICollection<TokenEmail> TokensEmail { get; set; } = new List<TokenEmail>();
        public ICollection<UsuarioTienda> UsuariosTienda { get; set; } = new List<UsuarioTienda>();
    }
}
