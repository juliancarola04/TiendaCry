namespace TiendaCry.Backend.Excepciones
{
    public class SinPermisoException : Exception
    {
        public SinPermisoException(string mensaje) : base(mensaje)
        {
        }
    }
}
