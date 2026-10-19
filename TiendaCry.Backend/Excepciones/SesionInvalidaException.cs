namespace TiendaCry.Backend.Excepciones
{
    public class SesionInvalidaException : Exception
    {
        public SesionInvalidaException()
            : base("La sesión no es válida o venció. Volvé a iniciar sesión.")
        {
        }
    }
}
