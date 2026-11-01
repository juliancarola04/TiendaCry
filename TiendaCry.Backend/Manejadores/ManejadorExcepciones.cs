using Microsoft.AspNetCore.Diagnostics;

namespace TiendaCry.Backend.Manejadores;

// Este lo voy a usar para poder registrar bien las excepciones en los diversos lados y not ener que ir poniéndolo mano a mano todo. Se usaría
// por si en algún momento se me escapa el de algún try catch.
public class ManejadorExcepciones : IExceptionHandler
{
    private readonly ILogger<ManejadorExcepciones> _logger;

    public ManejadorExcepciones(ILogger<ManejadorExcepciones> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        string idSolicitud = context.TraceIdentifier;

        _logger.LogError(exception, "Error inesperado en {Metodo} {Ruta}. Solicitud {IdSolicitud}.",
            context.Request.Method, context.Request.Path.Value, idSolicitud);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "text/plain; charset=utf-8";

        await context.Response.WriteAsync($"No se pudo completar la solicitud. Código de error: {idSolicitud}.", cancellationToken);

        return true;
    }
}
