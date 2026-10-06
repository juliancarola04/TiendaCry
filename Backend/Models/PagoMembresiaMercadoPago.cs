namespace Backend.Models
{
    public class PagoMembresiaMercadoPago
    {
        public int Id { get; set; }
        public int Monto { get; set; }
        public required string Moneda { get; set; }
        public DateTime? FechaPago { get; set; }
        public string? IdPagoMP { get; set; }
        public DateTime FechaCreacion { get; set; }
        public required string IdPreferenciaMP { get; set; }
        public required string ExternalReferenceMP { get; set; }
        public int IdEstadoPagoMembresia { get; set; }
        public int IdMembresia { get; set; }

        public EstadoPagoMembresia EstadoPagoMembresia { get; set; } = null!;
        public Membresia Membresia { get; set; } = null!;
    }
}
