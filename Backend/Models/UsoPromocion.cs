namespace Backend.Models
{
    public class UsoPromocion
    {
        public int Id { get; set; }
        public DateTime FechaHoraUso { get; set; }
        public int IdMembresia { get; set; }
        public int IdPromocion { get; set; }
        public int IdUsuarioTienda { get; set; }

        public Membresia Membresia { get; set; } = null!;
        public Promocion Promocion { get; set; } = null!;
        public UsuarioTienda UsuarioTienda { get; set; } = null!;
    }
}
