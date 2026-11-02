using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;

namespace TiendaCry.Backend.Services;

public class ImagenService
{
    private const long TamanoMaximo = 5 * 1024 * 1024;

    private readonly IImagenRepository _imagenRepository;
    private readonly IPromocionRepository _promocionRepository;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ImagenService> _logger;

    public ImagenService(IImagenRepository imagenRepository, IPromocionRepository promocionRepository, IWebHostEnvironment environment, ILogger<ImagenService> logger)
    {
        _imagenRepository = imagenRepository;
        _promocionRepository = promocionRepository;
        _environment = environment;
        _logger = logger;
    }

    public async Task<Imagen> SubirImagen(int idPromocion, int idUsuario, IFormFile archivo)
    {
        Promocion? promocion = await _promocionRepository.BuscarPromocionAdministrable(idPromocion, idUsuario);

        if (promocion is null)
        {
            throw new SinPermisoException("No tenés permiso para cargar imágenes en esta promoción.");
        }

        if (promocion.EstadoPromocion.Nombre != "PendienteRevision"
            && promocion.EstadoPromocion.Nombre != "Rechazada")
        {
            throw new DatosLlegaronErradosException("Solo podés cambiar la imagen de promociones pendientes o rechazadas.");
        }

        if (archivo is null || archivo.Length == 0)
        {
            throw new DatosLlegaronErradosException("Seleccioná una imagen.");
        }

        if (archivo.Length > TamanoMaximo)
        {
            throw new DatosLlegaronErradosException("La imagen no puede superar los 5 MB.");
        }

        string extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

        bool esPng = extension == ".png"
            && string.Equals(archivo.ContentType, "image/png", StringComparison.OrdinalIgnoreCase);

        bool esJpeg = (extension == ".jpg" || extension == ".jpeg")
            && string.Equals(archivo.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase);

        if (!esPng && !esJpeg)
        {
            throw new DatosLlegaronErradosException("Solo se permiten imágenes JPG, JPEG o PNG.");
        }

        string webRoot = _environment.WebRootPath
            ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

        string directorio = Path.Combine(webRoot, "uploads", "promociones");
        Directory.CreateDirectory(directorio);

        string nombreArchivo = $"{Guid.NewGuid():N}{extension}";
        string rutaFisica = Path.Combine(directorio, nombreArchivo);
        string? nombreAnterior = promocion.Imagen?.NombreArchivo;

        Imagen imagen = promocion.Imagen ?? new Imagen
        {
            NombreOriginal = string.Empty,
            NombreArchivo = string.Empty,
            RutaRelativa = string.Empty,
            TipoContenido = string.Empty,
            IdPromocion = idPromocion
        };

        try
        {
            await using (FileStream stream = new FileStream(rutaFisica, FileMode.CreateNew))
            {
                await archivo.CopyToAsync(stream);
            }

            imagen.NombreOriginal = Path.GetFileName(archivo.FileName);
            imagen.NombreArchivo = nombreArchivo;
            imagen.RutaRelativa = $"uploads/promociones/{nombreArchivo}";
            imagen.TipoContenido = esPng ? "image/png" : "image/jpeg";
            imagen.TamanoBytes = archivo.Length;
            imagen.FechaCreacion = DateTime.UtcNow;

            await _imagenRepository.Guardar(imagen);
        }
        catch
        {
            EliminarArchivo(directorio, nombreArchivo);
            throw;
        }

        if (!string.IsNullOrWhiteSpace(nombreAnterior))
        {
            EliminarArchivo(directorio, nombreAnterior);
        }

        _logger.LogInformation("El usuario {IdUsuario} guardó la imagen {IdImagen} de la promoción {IdPromocion}.",
            idUsuario, imagen.Id, idPromocion);

        return imagen;
    }

    private void EliminarArchivo(string directorio, string nombreArchivo)
    {
        string ruta = Path.Combine(directorio, Path.GetFileName(nombreArchivo));

        try
        {
            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }
        }
        catch (IOException e)
        {
            _logger.LogWarning(e, "No se pudo eliminar el archivo {NombreArchivo}.", nombreArchivo);
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogWarning(e, "No se pudo eliminar el archivo {NombreArchivo}.", nombreArchivo);
        }
    }
}
