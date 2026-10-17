using System.Net.Mail;

namespace TiendaCry.Backend.Utilidades
{
    public static class Validaciones
    {
        public static bool EstanDatosBien(params string?[] valores)
        {
            if (valores is null || valores.Length == 0)
            {
                return false;
            }

            return valores.All(
                valor => !string.IsNullOrWhiteSpace(valor));
        }

        public static bool EsUnEmailValido(string email)
        {
            return MailAddress.TryCreate(email, out var direccion)
                && direccion.Address == email;
        }

        public static bool CumpleLongitudMaxima(
            string valor,
            int longitudMaxima)
        {
            return valor.Length <= longitudMaxima;
        }
    }
}
