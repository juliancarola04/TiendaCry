namespace TiendaCry.Backend.Excepciones
{
    public class DatosLlegaronErradosException : Exception
    {
        public DatosLlegaronErradosException(string mensaje)
            : base(mensaje)
        {
        }
    }
}
