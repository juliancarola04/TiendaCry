namespace Backend.Models
{
    public class EstadoPagoMembresia
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }

        public ICollection<PagoMembresiaMercadoPago>? PagosMembresiaMercadoPago { get; set; } = new List<PagoMembresiaMercadoPago>();
    }
}
