namespace Backend.Models
{
    public class TipoTokenEmail
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }

        public ICollection<TokenEmail>? TokensEmail { get; set; } = new List<TokenEmail>();
    }
}
