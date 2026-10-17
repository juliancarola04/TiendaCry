namespace TiendaCry.Backend.Excepciones
{
    public class CredencialesInvalidasException : Exception
    {
        public CredencialesInvalidasException()
            : base("No se pudo iniciar sesión con las credenciales proporcionadas.")
        {
        }
    }
}
