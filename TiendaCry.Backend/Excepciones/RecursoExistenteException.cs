namespace TiendaCry.Backend.Excepciones;

public class RecursoExistenteException : Exception
{
    public RecursoExistenteException(string mensaje)
        : base(mensaje)
    {
    }
}
