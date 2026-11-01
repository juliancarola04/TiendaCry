namespace TiendaCry.Backend.Models
{
    public class Imagen
    {
        public int Id { get; set; }
        public required string NombreOriginal { get; set; }
        public required string NombreArchivo { get; set; }
        public required string RutaRelativa { get; set; }
        public required string TipoContenido { get; set; }
        public long TamanoBytes { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int IdPromocion { get; set; }

        public Promocion Promocion { get; set; } = null!;
    }
}
