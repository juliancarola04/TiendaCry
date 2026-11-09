using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;
using TiendaCry.Backend.Utilidades;
using TiendaCry.Shared.DTO.Categoria.Request;
using TiendaCry.Shared.DTO.Categoria.Response;

namespace TiendaCry.Backend.Services
{
    public class CategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<CategoriaService> _logger;

        public CategoriaService(ICategoriaRepository categoriaRepository, IUsuarioRepository usuarioRepository, ILogger<CategoriaService> logger)
        {
            _categoriaRepository = categoriaRepository;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<List<CategoriaResponse>> ObtenerCategorias()
        {
            List<Categoria> categorias = await _categoriaRepository.ObtenerCategorias();

            return categorias.Select(c => new CategoriaResponse(c.Id, c.Nombre)).ToList();
        }

        public async Task Crear(int idAdministrador, GuardarCategoriaRequest request)
        {
            await ValidarAdministrador(idAdministrador);

            if (!Validaciones.EstanDatosBien(request.Nombre))
            {
                throw new DatosLlegaronErradosException("Ingresá el nombre de la categoría.");
            }

            string nombre = request.Nombre.Trim();

            if (await _categoriaRepository.ExisteNombre(nombre, null))
            {
                throw new RecursoExistenteException("Ya existe una categoría con ese nombre.");
            }

            Categoria categoria = new Categoria
            {
                Nombre = nombre
            };

            await _categoriaRepository.Crear(categoria);

            _logger.LogInformation("El administrador {IdAdministrador} creó la categoría {IdCategoria}.", idAdministrador, categoria.Id);
        }

        public async Task<bool> Modificar(int idCategoria, int idAdministrador, GuardarCategoriaRequest request)
        {
            await ValidarAdministrador(idAdministrador);

            if (!Validaciones.EstanDatosBien(request.Nombre))
            {
                throw new DatosLlegaronErradosException("Ingresá el nombre de la categoría.");
            }

            Categoria? categoria = await _categoriaRepository.BuscarPorId(idCategoria);

            if (categoria is null)
            {
                return false;
            }

            string nombre = request.Nombre.Trim();

            if (await _categoriaRepository.ExisteNombre(nombre, idCategoria))
            {
                throw new RecursoExistenteException("Ya existe una categoría con ese nombre.");
            }

            categoria.Nombre = nombre;

            await _categoriaRepository.GuardarCambios();

            _logger.LogInformation("El administrador {IdAdministrador} modificó la categoría {IdCategoria}.", idAdministrador, idCategoria);

            return true;
        }

        public async Task<bool> Eliminar(int idCategoria, int idAdministrador)
        {
            await ValidarAdministrador(idAdministrador);

            Categoria? categoria = await _categoriaRepository.BuscarPorId(idCategoria);

            if (categoria is null)
            {
                return false;
            }

            if (await _categoriaRepository.TieneTiendas(idCategoria))
            {
                throw new RecursoExistenteException("No se puede eliminar la categoría porque tiene tiendas asociadas.");
            }

            await _categoriaRepository.Eliminar(categoria);

            _logger.LogInformation("El administrador {IdAdministrador} eliminó la categoría {IdCategoria}.", idAdministrador, idCategoria);

            return true;
        }

        private async Task ValidarAdministrador(int idAdministrador)
        {
            Usuario? administrador = await _usuarioRepository.BuscarPorId(idAdministrador);

            if (administrador is null || !await _usuarioRepository.EstaActivo(idAdministrador))
            {
                throw new CredencialesInvalidasException();
            }

            if (!administrador.EsAdministradorTiendaCry)
            {
                throw new SinPermisoException("No tenés permiso para gestionar categorías.");
            }

            if (!administrador.EmailVerificado)
            {
                throw new SinPermisoException("Verificá tu correo para continuar.");
            }

            if (administrador.DebeCambiarContrasena)
            {
                throw new SinPermisoException("Cambiá tu contraseña temporal para continuar.");
            }
        }
    }
}
