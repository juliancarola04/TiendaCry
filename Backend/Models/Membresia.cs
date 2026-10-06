namespace Backend.Models
{
    public class Membresia
    {
        public int Id { get; set; }
        public int MontoMensualCupones { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int IdSocio { get; set; }
        public int IdEstadoMembresia { get; set; }

        public Socio Socio { get; set; } = null!;
        public EstadoMembresia EstadoMembresia { get; set; } = null!;

        public ICollection<Cupon> Cupones { get; set; } = new List<Cupon>();
        public ICollection<TokenQR> TokensQR { get; set; } = new List<TokenQR>();
        public ICollection<PagoMembresiaMercadoPago> PagosMembresiaMercadoPago { get; set; } = new List<PagoMembresiaMercadoPago>();
        public ICollection<UsoPromocion> UsosPromocion { get; set; } = new List<UsoPromocion>();
    }
}
